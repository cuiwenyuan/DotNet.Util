//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;

namespace DotNet.Business
{
    using Model;
    using Util;

    /// <remarks>
    ///	BaseFileManager
    ///	
    ///	修改记录
    /// 
    ///     2012.05.21 版本：3.7 Serwif      检查欲上传的文件是否已经同名存在，加入删除状态为0的条件为前提。
    ///     2009.07.16 版本：3.0 JiRiGaLa    整理流量、文件相关。
    ///     2008.10.04 版本：2.5 JiRiGaLa    移动文件时的覆盖文件问题解决。
    ///     2007.05.30 版本：2.4 JiRiGaLa    ErrorDeleted，ErrorChanged 状态进行改进整理。
    ///	    2007.05.29 版本：2.3 JiRiGaLa    BatchSave，ErrorDataRelated，force 进行改进整理。
    ///	    2007.05.23 版本：2.2 JiRiGaLa    StatusCode，StatusMessage 进行改进。
    ///	    2007.05.12 版本：2.1 JiRiGaLa    BatchSave 进行改进。
    ///	    2007.01.04 版本：2.0 JiRiGaLa    重新整理主键。
    ///	    2006.02.12 版本：1.2 JiRiGaLa    调整主键的规范化。
    ///	    2006.02.12 版本：1.2 JiRiGaLa    调整主键的规范化。
    ///	    2004.11.19 版本：1.1 JiRiGaLa    增加员工类别判断部分。
    ///     2004.11.18 版本：1.0 JiRiGaLa    主键进行了绝对的优化，这是个好东西啊，平时要多用，用得要灵活些。
    ///		
    /// <author>
    ///		<name>JiRiGaLa</name>
    ///		<date>2008.10.04</date>
    /// </author> 
    /// </remarks>
    public partial class BaseFileManager : BaseManager
    {
        #region public BaseFileEntity GetEntity(string id) 获取信息
        /// <summary>
        /// 获取信息
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns>数据权限</returns>
        public BaseFileEntity GetEntity(string id)
        {
            var sql = "SELECT " + BaseFileEntity.FieldId
                    + "        ," + BaseFileEntity.FieldFolderId
                    + "        ," + BaseFileEntity.FieldFileName
                    + "        ," + BaseFileEntity.FieldFilePath
                    + "        ," + BaseFileEntity.FieldFileSize
                    + "        ," + BaseFileEntity.FieldReadCount
                    + "        ," + BaseFileEntity.FieldDescription
                    + "        ," + BaseFileEntity.FieldEnabled
                    + "        ," + BaseFileEntity.FieldDeleted
                    + "        ," + BaseFileEntity.FieldSortCode
                    + "        ," + BaseFileEntity.FieldCreateUserId
                    + "        ," + BaseFileEntity.FieldCreateBy
                    + "        ," + BaseFileEntity.FieldCreateTime
                    + "        ," + BaseFileEntity.FieldUpdateUserId
                    + "        ," + BaseFileEntity.FieldUpdateBy
                    + "        ," + BaseFileEntity.FieldUpdateTime
                        + " FROM " + CurrentTableName
                        + " WHERE " + BaseFileEntity.FieldId + " = " + DbHelper.GetParameter(BaseFileEntity.FieldId);
            var dt = new DataTable(BaseFileEntity.TableName);
            DbHelper.Fill(dt, sql, new IDbDataParameter[] { DbHelper.MakeParameter(BaseFileEntity.FieldId, id) });
            return BaseEntity.Create<BaseFileEntity>(dt);
        }
        #endregion

        /// <summary>
        /// 更新阅读次数
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int UpdateReadCount(string id)
        {
            // 阅读次数要加一
            var sqlBuilder = new SqlBuilder(DbHelper);
            sqlBuilder.BeginUpdate(CurrentTableName);
            sqlBuilder.SetFormula(BaseFileEntity.FieldReadCount, BaseFileEntity.FieldReadCount + " + 1 ");
            sqlBuilder.SetWhere(BaseFileEntity.FieldId, id);
            return sqlBuilder.EndUpdate();
        }

        /// <summary>
        /// 从数据库中读取文件
        /// </summary>
        /// <param name="id">文件主键</param>
        /// <returns>文件</returns>
        public byte[] Download(string id)
        {
            // 阅读次数要加一
            UpdateReadCount(id);
            // 下载文件
            byte[] fileContent = null;
            var sql = "SELECT " + BaseFileEntity.FieldContents
                            + " FROM " + BaseFileEntity.TableName
                            + " WHERE " + BaseFileEntity.FieldId + " = " + DbHelper.GetParameter(BaseFileEntity.FieldId);
            IDataReader dataReader = null;
            try
            {
                using (dataReader = DbHelper.ExecuteReader(sql, new IDbDataParameter[] { DbHelper.MakeParameter(BaseFileEntity.FieldId, id) }))
                {
                    if (dataReader.Read())
                    {
                        fileContent = (byte[])dataReader[BaseFileEntity.FieldContents];
                    }
                }
            }
            catch (Exception ex)
            {
                // 在本地记录异常
                BaseExceptionManager.LogException(DbHelper, UserInfo, ex);
            }
            finally
            {
                if (dataReader != null)
                {
                    dataReader.Close();
                }
            }
            return fileContent;
        }

        /// <summary>
        /// 上传
        /// </summary>
        /// <param name="folderId"></param>
        /// <param name="fileName"></param>
        /// <param name="file"></param>
        /// <param name="enabled"></param>
        /// <returns></returns>
        public string Upload(string folderId, string fileName, byte[] file, bool enabled)
        {
            // 检查是否已经存在
            //string result = this.GetId(new KeyValuePair<string, object>(BaseFileEntity.FieldFolderId, folderId), new KeyValuePair<string, object>(BaseFileEntity.FieldFileName, fileName));
            // 检查是否已经存在，加入删除状态为0的条件
            var parametersList = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseFileEntity.FieldFolderId, folderId),
                new KeyValuePair<string, object>(BaseFileEntity.FieldFileName, fileName),
                new KeyValuePair<string, object>(BaseFileEntity.FieldDeleted, 0)
            };
            var result = GetId(parametersList);

            if (!string.IsNullOrEmpty(result))
            {
                // 更新数据
                UpdateFile(result, fileName, file);
                //在能够真实模仿C/S中的提示确定信息对话框的B/S版本出来之前，先做如下处理：前面的文件有重复的，批量删除来处理，因为客户不会闲着没事，老传文件，且服务器都是几百个G的空间
                // 删除数据
                var manager = new BaseFileManager();
                var intReturnValue = manager.SetDeleted(result);
                // 添加数据
                var entity = new BaseFileEntity
                {
                    FolderId = folderId,
                    FileName = fileName,
                    Contents = file,
                    Enabled = enabled ? 1 : 0
                };
                result = AddEntity(entity);
            }
            else
            {
                // 添加数据
                var entity = new BaseFileEntity
                {
                    FolderId = folderId,
                    FileName = fileName,
                    Contents = file,
                    Enabled = enabled ? 1 : 0
                };
                result = AddEntity(entity);
            }
            return result;
        }
        /// <summary>
        /// 分块上传文件添加
        /// </summary>
        /// <param name="folderId"></param>
        /// <param name="filePath"></param>
        /// <param name="fileName"></param>
        /// <param name="file"></param>
        /// <param name="filesize"></param>
        /// <param name="enabled"></param>
        /// <returns></returns>
        public string UploadByBlock(string folderId, string filePath, string fileName, byte[] file, int filesize, bool enabled)
        {
            // 检查是否已经存在
            //string result = this.GetId(new KeyValuePair<string, object>(BaseFileEntity.FieldFolderId, folderId), new KeyValuePair<string, object>(BaseFileEntity.FieldFileName, fileName));
            // 检查是否已经存在，加入删除状态为0的条件
            var parametersList = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseFileEntity.FieldFolderId, folderId),
                new KeyValuePair<string, object>(BaseFileEntity.FieldFileName, fileName),
                new KeyValuePair<string, object>(BaseFileEntity.FieldDeleted, 0)
            };
            var result = GetId(parametersList);
            
            if (!string.IsNullOrEmpty(result))
            {
                // 更新数据
                UpdateFile(result, fileName, file);
                //在能够真实模仿C/S中的提示确定信息对话框的B/S版本出来之前，先做如下处理：前面的文件有重复的，批量删除来处理，因为客户不会闲着没事，老传文件，且服务器都是几百个G的空间
                // 删除数据
                var manager = new BaseFileManager();
                var intReturnValue = manager.SetDeleted(result);
                // 添加数据
                var entity = new BaseFileEntity
                {
                    FolderId = folderId,
                    FilePath = filePath,
                    FileName = fileName,
                    Contents = file,
                    FileSize = filesize,
                    Enabled = enabled ? 1 : 0
                };
                result = AddEntity(entity);
            }
            else
            {
                // 添加数据
                var entity = new BaseFileEntity
                {
                    FolderId = folderId,
                    FilePath = filePath,
                    FileName = fileName,
                    Contents = file,
                    FileSize = filesize,
                    Enabled = enabled ? 1 : 0
                };
                result = AddEntity(entity);
            }
            return result;
        }        

        /// <summary>
        /// 分段上传文件 ，2012-10-14 HJC Add 暂时支持SQL数据库 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="length"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        public string UploadByBlockUpdate(string id, int length, byte[] file)
        {
            var result = string.Empty;

            string commandText;
            var entity = new BaseFileEntity();
            commandText = "declare @content varbinary(16) ";
            commandText += string.Format("SELECT @content=textptr({0}) from {1} where {2} = {3}", BaseFileEntity.FieldContents, BaseFileEntity.TableName, BaseFileEntity.FieldId, id);
            commandText += string.Format(" updatetext {0}.{1} @content @length 0 @block", BaseFileEntity.TableName, BaseFileEntity.FieldContents);
            var dbParameters = new IDbDataParameter[2];
            dbParameters[0] = DbHelper.MakeParameter("block", file);
            dbParameters[1] = DbHelper.MakeParameter("length", length);
            dbHelper.ExecuteNonQuery(commandText, dbParameters);

            return result;
        }

        #region public DataTable GetDataTableByFolder(string id) 获取信息
        /// <summary>
        /// 获取信息
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns>数据表</returns>
        public DataTable GetDataTableByFolder(string id)
        {
            var sql = "SELECT " + BaseFileEntity.FieldId
                    + "        ," + BaseFileEntity.FieldFolderId
                    + "        ," + BaseFileEntity.FieldFileName
                    + "        ," + BaseFileEntity.FieldFilePath
                    + "        ," + BaseFileEntity.FieldFileSize
                    + "        ," + BaseFileEntity.FieldReadCount
                    + "        ," + BaseFileEntity.FieldDescription
                    + "        ," + BaseFileEntity.FieldEnabled
                    + "        ," + BaseFileEntity.FieldDeleted
                    + "        ," + BaseFileEntity.FieldSortCode
                    + "        ," + BaseFileEntity.FieldCreateUserId
                    + "        ," + BaseFileEntity.FieldCreateBy
                    + "        ," + BaseFileEntity.FieldCreateTime
                    + "        ," + BaseFileEntity.FieldUpdateUserId
                    + "        ," + BaseFileEntity.FieldUpdateBy
                    + "        ," + BaseFileEntity.FieldUpdateTime
                    + "       , (SELECT " + BaseFolderEntity.FieldFolderName 
                                + " FROM " + BaseFolderEntity.TableName 
                                + " WHERE " + BaseFolderEntity.FieldId + " = " + BaseFileEntity.FieldFolderId + ") AS FolderFullName "
                    + " FROM " + BaseFileEntity.TableName
                    + " WHERE " + BaseFileEntity.FieldFolderId + " = " + DbHelper.GetParameter(BaseFileEntity.FieldFolderId)
                    + " AND " + BaseFileEntity.FieldDeleted + " = '0'";
            var dt = new DataTable(BaseFileEntity.TableName);
            DbHelper.Fill(dt, sql, new IDbDataParameter[] { DbHelper.MakeParameter(BaseFileEntity.FieldFolderId, id)});
            if (id.Length == 0)
            {
                // 这里注意默认值
                var dr = dt.NewRow();
                dr[BaseFileEntity.FieldEnabled] = 1;
                dt.Rows.Add(dr);
                dt.AcceptChanges();
            }
            // this.GetSingle(result);
            return dt;
        }
        #endregion

        #region public override int BatchSave(DataTable result) 批量进行保存
        /// <summary>
        /// 批量进行保存
        /// </summary>
        /// <param name="dt">数据表</param>
        /// <returns>影响行数</returns>
        public override int BatchSave(DataTable dt)
        {
            var result = 0;
            var entity = new BaseFileEntity();
            foreach (DataRow dr in dt.Rows)
            {
                // 删除状态
                if (dr.RowState == DataRowState.Deleted)
                {
                    var id = dr[BaseFileEntity.FieldId, DataRowVersion.Original].ToString();
                    if (id.Length > 0)
                    {
                        result += Delete(id);
                    }
                }
                // 被修改过
                if (dr.RowState == DataRowState.Modified)
                {
                    var id = dr[BaseFileEntity.FieldId, DataRowVersion.Original].ToString();
                    if (id.Length > 0)
                    {
                        entity.GetFrom(dr);
                        // 判断是否允许编辑
                        result += UpdateEntity(entity);
                    }
                }
                // 添加状态
                if (dr.RowState == DataRowState.Added)
                {
                    entity.GetFrom(dr);
                    result += AddEntity(entity).Length > 0 ? 1 : 0;
                }
                if (dr.RowState == DataRowState.Unchanged)
                {
                    continue;
                }
                if (dr.RowState == DataRowState.Detached)
                {
                    continue;
                }
            }
            return result;
        }
        #endregion

        #region public string Add(string folderId, string fileName, byte[] file, bool enabled, out string statusCode) 添加文件
        /// <summary>
        /// 添加文件
        /// </summary>
        /// <param name="folderId">文件夹主键</param>
        /// <param name="fileName">文件名</param>
        /// <param name="file">文件</param>
        /// <param name="description"></param>
        /// <param name="enabled">有效</param>
        /// <param name="statusCode">状态</param>
        /// <returns>主键</returns>
        public string Add(string folderId, string fileName, byte[] file, string description, bool enabled, out string statusCode)
        {
            return Add(folderId, fileName, null, file, description, enabled, out statusCode);
        }
        #endregion

        #region public string Add(string folderId, string fileName, byte[] file, bool enabled, out string statusCode) 添加文件
        /// <summary>
        /// 添加文件
        /// </summary>
        /// <param name="folderId">文件夹主键</param>
        /// <param name="fileName">文件名</param>
        /// <param name="file">文件</param>
        /// <param name="description"></param>
        /// <param name="enabled">有效</param>
        /// <param name="statusCode">状态</param>
        /// <returns>主键</returns>
        public string Add(string folderId, string fileName, string file, string description, bool enabled, out string statusCode)
        {
            return Add(folderId, fileName, file, null, description, enabled, out statusCode);
        }
        #endregion

        #region private string Add(string folderId, string fileName, string file, byte[] byteFile, string category, bool enabled, out string statusCode) 添加文件
        /// <summary>
        /// 添加文件
        /// </summary>
        /// <param name="folderId">文件夹主键</param>
        /// <param name="fileName">文件名</param>
        /// <param name="file">文件</param>
        /// <param name="byteFile"></param>
        /// <param name="description"></param>
        /// <param name="enabled">有效</param>
        /// <param name="statusCode">状态</param>
        /// <returns>主键</returns>
        private string Add(string folderId, string fileName, string file, byte[] byteFile, string description, bool enabled, out string statusCode)
        {
            statusCode = string.Empty;
            var entity = new BaseFileEntity
            {
                FolderId = folderId,
                FileName = fileName,
                Contents = byteFile,
                Description = description,
                Enabled = enabled ? 1 : 0
            };
            var result = string.Empty;
            // 检查是否重复
            if (Exists(new KeyValuePair<string, object>(BaseFileEntity.FieldFolderId, entity.FolderId), new KeyValuePair<string, object>(BaseFileEntity.FieldFileName, entity.FileName)))
            {
                // 名称已重复
                statusCode = Status.ErrorNameExist.ToString();
            }
            else
            {
                result = AddEntity(entity);
                // 运行成功
                statusCode = Status.OkAdd.ToString();
            }
            return result;
        }
        #endregion

        #region public string Add(BaseNewsEntity entity, out string statusCode) 添加文件
        /// <summary>
        /// 添加文件
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="statusCode">状态</param>
        /// <returns>主键</returns>
        public string Add(BaseFileEntity entity, out string statusCode)
        {
            statusCode = string.Empty;
            var result = string.Empty;
            // 检查是否重复
            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseFileEntity.FieldFolderId, entity.FolderId),
                new KeyValuePair<string, object>(BaseFileEntity.FieldFileName, entity.FileName),
                new KeyValuePair<string, object>(BaseFileEntity.FieldDeleted, 0)
            };

            if (Exists(parameters))
            {
                // 名称已重复
                statusCode = Status.ErrorNameExist.ToString();
            }
            else
            {
                result = AddEntity(entity);
                // 运行成功
                statusCode = Status.OkAdd.ToString();
            }
            return result;
        }
        #endregion

        #region public int Update(BaseFileEntity entity, out string statusCode) 更新
        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="entity">文件夹的基类表结构定义</param>
        /// <param name="statusCode">状态返回码</param>
        /// <returns>影响行数</returns>
        public int Update(BaseFileEntity entity, out string statusCode)
        {
            var result = 0;
            //if (DbUtil.IsModifed(DbHelper, BaseFolderEntity.TableName, fileEntity.Id, fileEntity.ModifiedUserId, fileEntity.ModifiedOn))
            //{
            //    // 数据已经被修改
            //    statusCode = StatusCode.ErrorChanged.ToString();
            //}
            //else
            //{
                // 检查文件夹名是否重复
            if (Exists(new KeyValuePair<string, object>(BaseFileEntity.FieldFolderId, entity.FolderId), new KeyValuePair<string, object>(BaseFileEntity.FieldFileName, entity.FileName), entity.Id))
                {
                    // 文件夹名已重复
                    statusCode = Status.ErrorNameExist.ToString();
                }
                else
                {
                    result = UpdateEntity(entity);
                    if (result == 1)
                    {
                        // 运行成功
                        statusCode = Status.OkUpdate.ToString();
                    }
                    else
                    {
                        statusCode = Status.ErrorDeleted.ToString();
                    }
                }
            //}
            return result;
        }
        #endregion

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="id"></param>
        /// <param name="folderId"></param>
        /// <param name="fileName"></param>
        /// <param name="description"></param>
        /// <param name="enabled"></param>
        /// <param name="statusCode"></param>
        /// <returns></returns>
        public int Update(string id, string folderId, string fileName, string description, bool enabled, out string statusCode)
        {
            statusCode = string.Empty;
            var sqlBuilder = new SqlBuilder(DbHelper);
            sqlBuilder.BeginUpdate(CurrentTableName);
            sqlBuilder.SetValue(BaseFileEntity.FieldFolderId, folderId);
            sqlBuilder.SetValue(BaseFileEntity.FieldFileName, fileName);
            sqlBuilder.SetValue(BaseFileEntity.FieldEnabled, enabled);            
            sqlBuilder.SetValue(BaseFileEntity.FieldDescription, description);
            sqlBuilder.SetValue(BaseFileEntity.FieldUpdateUserId, UserInfo.Id);
            sqlBuilder.SetValue(BaseFileEntity.FieldUpdateBy, UserInfo.RealName);
            sqlBuilder.SetDbNow(BaseFileEntity.FieldUpdateTime);
            sqlBuilder.SetWhere(BaseFileEntity.FieldId, id);
            var result = sqlBuilder.EndUpdate();
            if (result > 0)
            {
                statusCode = Status.OkUpdate.ToString();
            }
            else
            {
                statusCode = Status.ErrorDeleted.ToString();
            }
            return result;
        }
        /// <summary>
        /// 更新字段
        /// </summary>
        /// <param name="id"></param>
        /// <param name="fileName"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        public int UpdateFile(string id, string fileName, byte[] file)
        {
            var sqlBuilder = new SqlBuilder(DbHelper);
            sqlBuilder.BeginUpdate(CurrentTableName);
            sqlBuilder.SetValue(BaseFileEntity.FieldFileName, fileName);
            if (file != null)
            {
                sqlBuilder.SetValue(BaseFileEntity.FieldContents, file);
                sqlBuilder.SetValue(BaseFileEntity.FieldFileSize, file.Length);
            }
            sqlBuilder.SetValue(BaseFileEntity.FieldUpdateUserId, UserInfo.Id);
            sqlBuilder.SetValue(BaseFileEntity.FieldUpdateBy, UserInfo.RealName);
            sqlBuilder.SetDbNow(BaseFileEntity.FieldUpdateTime);
            sqlBuilder.SetWhere(BaseFileEntity.FieldId, id);
            return sqlBuilder.EndUpdate();
        }
        /// <summary>
        /// 更新字段
        /// </summary>
        /// <param name="id"></param>
        /// <param name="fileName"></param>
        /// <param name="file"></param>
        /// <param name="statusCode"></param>
        /// <returns></returns>
        public int UpdateFile(string id, string fileName, byte[] file, out string statusCode)
        {
            statusCode = string.Empty;
            var result = UpdateFile(id, fileName, file);
            if (result > 0)
            {
                statusCode = Status.OkUpdate.ToString();
            }
            else
            {
                statusCode = Status.ErrorDeleted.ToString();
            }
            return result;
        }

        #region public DataTable Search(string searchKey) 查询
        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="searchKey">查询</param>
        /// <returns>数据表</returns>
        public DataTable Search(string searchKey)
        {
            return Search(string.Empty, searchKey);
        }
        #endregion

        #region public DataTable Search(string userId, string searchKey) 查询
        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="userId">用户主键</param>
        /// <param name="searchKey">查询条件</param>
        /// <returns>数据表</returns>
        public DataTable Search(string userId, string searchKey)
        {
            return Search(userId, searchKey, null, null);
        }
        #endregion

        /// <summary>
        /// 搜索
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="searchKey"></param>
        /// <param name="enabled"></param>
        /// <param name="deletionStateCode"></param>
        /// <returns></returns>
        public DataTable Search(string userId, string searchKey, bool? enabled, bool? deletionStateCode)
        {
            // 一、这里是将Boolean值转换为int类型。
            var delete = 0;
            if (deletionStateCode != null)
            {
                delete = (bool)deletionStateCode ? 1 : 0;
            }

            // 二、这里是开始进行动态SQL语句拼接，字段名、表明都进行了常量定义，表名字段名发生变化时，很容易就知道程序哪里都调用了这些。
            var sql = "SELECT " + BaseFileEntity.FieldId
                    + "        ," + BaseFileEntity.FieldFolderId
                    + "        ," + BaseFileEntity.FieldFileName
                    + "        ," + BaseFileEntity.FieldFilePath
                    + "        ," + BaseFileEntity.FieldFileSize
                    + "        ," + BaseFileEntity.FieldReadCount
                    + "        ," + BaseFileEntity.FieldDescription
                    + "        ," + BaseFileEntity.FieldEnabled
                    + "        ," + BaseFileEntity.FieldDeleted
                    + "        ," + BaseFileEntity.FieldSortCode
                    + "        ," + BaseFileEntity.FieldCreateUserId
                    + "        ," + BaseFileEntity.FieldCreateBy
                    + "        ," + BaseFileEntity.FieldCreateTime
                    + "        ," + BaseFileEntity.FieldUpdateUserId
                    + "        ," + BaseFileEntity.FieldUpdateBy
                    + "        ," + BaseFileEntity.FieldUpdateTime
                    + " FROM " + CurrentTableName
                    + " WHERE " + BaseFileEntity.FieldDeleted + " = " + delete;

            if (enabled != null)
            {
                var isEnabled = (bool)enabled ? 1 : 0;
                sql += string.Format(" AND {0} = {1}", BaseFileEntity.FieldEnabled, isEnabled);
            }
            // 三、我们认为 userId 这个查询条件是安全，不是人为输入的参数，所以直接进行了SQL语句拼接
            if (!string.IsNullOrEmpty(userId))
            {
                sql += string.Format(" AND {0} = {1}", BaseFileEntity.FieldCreateUserId, userId);
            }
            /*
            if (!string.IsNullOrEmpty(folderId))
            {
                sql += " AND " + BaseNewsEntity.FieldCategoryCode + " = " + folderId;
            }
            */

            // 四、这里是进行参数化的准备，因为是多个不确定的查询参数，所以用了List。
            var dbParameters = new List<IDbDataParameter>();

            // 五、这里看查询条件是否为空
            searchKey = searchKey.Trim();
            if (!string.IsNullOrEmpty(searchKey))
            {
                // 六、这里是进行支持多种数据库的参数化查询
                sql += string.Format(" AND ({0} LIKE {1}", BaseFileEntity.FieldFileName, DbHelper.GetParameter(BaseFileEntity.FieldFileName));
                sql += string.Format(" OR {0} LIKE {1}", BaseFileEntity.FieldCreateBy, DbHelper.GetParameter(BaseFileEntity.FieldCreateBy));
                sql += string.Format(" OR {0} LIKE {1}", BaseFileEntity.FieldContents, DbHelper.GetParameter(BaseFileEntity.FieldContents));
                sql += string.Format(" OR {0} LIKE {1})", BaseFileEntity.FieldDescription, DbHelper.GetParameter(BaseFileEntity.FieldDescription));

                // 七、这里是判断，用户是否已经输入了%
                if (searchKey.IndexOf("%") < 0)
                {
                    searchKey = string.Format("%{0}%", searchKey);
                }

                // 八、这里生成支持多数据库的参数
                dbParameters.Add(DbHelper.MakeParameter(BaseFileEntity.FieldFileName, searchKey));
                dbParameters.Add(DbHelper.MakeParameter(BaseFileEntity.FieldCreateBy, searchKey));
                dbParameters.Add(DbHelper.MakeParameter(BaseFileEntity.FieldContents, searchKey));
                dbParameters.Add(DbHelper.MakeParameter(BaseFileEntity.FieldDescription, searchKey));
            }
            sql += " ORDER BY " + BaseFileEntity.FieldSortCode + " DESC ";

            // 九、这里是将List转换为数组，进行数据库查询
            return DbHelper.Fill(sql, dbParameters.ToArray());
        }


        /// <summary>
        /// 移动（要考虑文件的覆盖问题，文件名重复了，需要覆盖文件的）
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="folderId">文件夹主键</param>
        /// <returns>影响行数</returns>
        public int MoveTo(string id, string folderId)
        {
            // 有重名的文件，需要进行覆盖
            var fileName = GetProperty(id, BaseFileEntity.FieldFileName);
            Delete(new KeyValuePair<string, object>(BaseFileEntity.FieldFolderId, folderId), new KeyValuePair<string, object>(BaseFileEntity.FieldFileName, fileName));
            return SetProperty(id, new KeyValuePair<string, object>(BaseFileEntity.FieldFolderId, folderId));
        }

        /// <summary>
        /// 按目录删除文件
        /// </summary>
        /// <param name="folderId">文件夹主键</param>
        /// <returns>影响行数</returns>
        public int DeleteByFolder(string folderId)
        {
            return Delete(new List<KeyValuePair<string, object>> { new KeyValuePair<string, object>(PrimaryKey, folderId) });
        }

        #region public Double GetSumFileSize() 服务器已用空间(单位Byte)
        /// <summary>
        /// 服务器已用空间(单位Byte)
        /// </summary>
        public Double GetSumFileSize()
        {
            // 已用空间
            var sql = "SELECT SUM(FileSize) FROM BaseFile ";
            return Double.Parse(DbHelper.ExecuteScalar(sql).ToString());
        }
        #endregion

        #region public Double GetSumFileSize(string userId)
        /// <summary>
        /// 当前用户已用空间
        /// </summary>
        public Double GetSumFileSize(string userId)
        {
            // 当前用户已用空间
            var sql = "SELECT SUM(FileSize) FROM BaseFile WHERE CreateUserId='" + userId + "'";
            return Double.Parse(DbHelper.ExecuteScalar(sql).ToString());
        }
        #endregion

        #region public Double GetSumFileSize(bool enabled) 服务器已用空间(单位Byte)
        /// <summary>
        /// 服务器已用空间(单位Byte)
        /// </summary>
        public Double GetSumFileSize(bool enabled)
        {
            // 已用空间
            var sql = "SELECT SUM(FileSize) FROM BaseFile WHERE Enabled = ";
            sql += enabled ? "1" : "0";
            return Double.Parse(DbHelper.ExecuteScalar(sql).ToString());
        }
        #endregion

        #region public int GetFileCount() 文件数量
        /// <summary>
        /// 文件数量
        /// </summary>
        public int GetFileCount()
        {
            // 文件数量
            var sql = "SELECT COUNT(*) FROM BaseFile ";
            return int.Parse(DbHelper.ExecuteScalar(sql).ToString());
        }
        #endregion

        #region public int GetFileCount() 文件数量
        /// <summary>
        /// 文件数量
        /// </summary>
        public int GetFileCount(bool enabled)
        {
            // 文件数量
            var sql = "SELECT COUNT(*) FROM BaseFile WHERE Enabled = ";
            sql += enabled ? "1" : "0";
            return int.Parse(DbHelper.ExecuteScalar(sql).ToString());
        }
        #endregion

        #region public int GetFileCount() 文件数量
        /// <summary>
        /// 文件数量
        /// </summary>
        public int GetFileCount(string userId)
        {
            // 文件数量
            var sql = "SELECT COUNT(*) FROM BaseFile WHERE CreateUserId='" + userId + "'";
            return int.Parse(DbHelper.ExecuteScalar(sql).ToString());
        }
        #endregion

        #region public int GetFlowmeter() 文件总的流量
        /// <summary>
        /// 文件总的流量
        /// </summary>
        public int GetFlowmeter()
        {
            // 文件数量
            var sql = "SELECT SUM(FileSize * ReadCount) FROM BaseFile ";
            return int.Parse(DbHelper.ExecuteScalar(sql).ToString());
        }
        #endregion

        #region public int GetFlowmeter(string userId) 文件总的流量
        /// <summary>
        /// 文件总的流量
        /// </summary>
        public int GetFlowmeter(string userId)
        {
            // 文件数量
            var sql = string.Format("SELECT SUM(FileSize * ReadCount) FROM BaseFile  WHERE CreateUserId='{0}'", userId);
            return int.Parse(DbHelper.ExecuteScalar(sql).ToString());
        }
        #endregion

        #region public int GetFileCount(string folderId, string userId) 文件夹中文件数量
        /// <summary>
        /// 文件夹中文件数量
        /// </summary>
        public int GetFileCount(string folderId, string userId)
        {
            // 文件数量
            var sql = string.Format("SELECT SUM(FileSize * ReadCount) FROM BaseFile  WHERE CreateUserId='{0}' AND {1} = '{2}'", userId, BaseFolderEntity.FieldParentId, folderId);
            return int.Parse(DbHelper.ExecuteScalar(sql).ToString());
        }
        #endregion
    }
}