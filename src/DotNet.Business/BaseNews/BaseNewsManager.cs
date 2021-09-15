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
    ///	新闻表
    ///	
    ///	修改记录
    /// 
    ///     2010.08.23 版本：2.6 JiRiGaLa    每个人管理每个人自己的文章。
    ///     2008.10.04 版本：2.5 JiRiGaLa    移动文件时的覆盖文件问题解决。
    ///     2007.05.30 版本：2.4 JiRiGaLa    ErrorDeleted，ErrorChanged 状态进行改进整理。
    ///	    2007.05.29 版本：2.3 JiRiGaLa    BatchSave，ErrorDataRelated，force 进行改进整理。
    ///	    2007.05.23 版本：2.2 JiRiGaLa    StatusCode，StatusMessage 进行改进。
    ///	    2007.05.12 版本：2.1 JiRiGaLa    BatchSave 进行改进。
    ///	    2007.01.04 版本：2.0 JiRiGaLa    重新整理主键。
    ///	    2006.02.12 版本：1.2 JiRiGaLa    调整主键的规范化。
    ///	    2006.02.12 版本：1.2 JiRiGaLa    调整主键的规范化。
    ///	    2004.11.19 版本：1.1 JiRiGaLa    增加职员类别判断部分。
    ///     2004.11.18 版本：1.0 JiRiGaLa    主键进行了绝对的优化，这是个好东西啊，平时要多用，用得要灵活些。
    ///		
    /// <author>
    ///		<name>JiRiGaLa</name>
    ///		<date>2008.10.04</date>
    /// </author> 
    /// </remarks>
    public partial class BaseNewsManager : BaseManager, IBaseManager
    {
        /// <summary>
        /// 只有草稿状态的才可以删除
        /// </summary>
        /// <param name="ids">主键</param>
        /// <param name="tableVersion">版本默认4为老版本</param>
        /// <param name="enabled"></param>
        /// <param name="recordUser"></param>
        /// <returns>影响行数</returns>
        public override int SetDeleted(object[] ids, bool enabled = true, bool recordUser = true, int tableVersion = 4)
        {
            var result = 0;
            for (var i = 0; i < ids.Length; i++)
            {
                // if (this.GetEntity(ids[i].ToString()).AuditStatus.Equals(AuditStatus.Draft.ToString()))
                // {
                result += SetDeleted(ids[i], enabled, recordUser, tableVersion);
                //}
            }
            return result;
        }

        /// <summary>
        /// 获取新闻
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public BaseNewsEntity GetNews(string id)
        {
            // 阅读次数要加一
            UpdateReadCount(id);
            return GetEntity(id);
        }

        /// <summary>
        /// 更新阅读次数
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private int UpdateReadCount(string id)
        {
            // 阅读次数要加一
            var sqlBuilder = new SqlBuilder(DbHelper);
            sqlBuilder.BeginUpdate(CurrentTableName);
            sqlBuilder.SetFormula(BaseNewsEntity.FieldReadCount, BaseNewsEntity.FieldReadCount + " + 1 ");
            sqlBuilder.SetWhere(BaseNewsEntity.FieldId, id);
            return sqlBuilder.EndUpdate();
        }

        /// <summary>
        /// 从数据库中读取文件
        /// </summary>
        /// <param name="id">文件主键</param>
        /// <returns>文件</returns>
        public string ShowNews(string id)
        {
            // 阅读次数要加一
            UpdateReadCount(id);
            // 下载文件
            string fileContent = null;
            var sql = "SELECT " + BaseNewsEntity.FieldContents
                            + " FROM " + CurrentTableName
                            + "  WHERE " + BaseNewsEntity.FieldId + " = " + DbHelper.GetParameter(BaseNewsEntity.FieldId);
            try
            {
                using (var dataReader = DbHelper.ExecuteReader(sql, new IDbDataParameter[] { DbHelper.MakeParameter(BaseNewsEntity.FieldId, id) }))
                {
                    if (dataReader.Read())
                    {
                        fileContent = dataReader[BaseNewsEntity.FieldContents].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                // 在本地记录异常
                BaseExceptionManager.LogException(DbHelper, UserInfo, ex);
            }

            return fileContent;
        }
        /// <summary>
        /// 上传
        /// </summary>
        /// <param name="folderId"></param>
        /// <param name="title"></param>
        /// <param name="contents"></param>
        /// <returns></returns>
        public string Upload(string folderId, string title, string contents)
        {
            // 检查是否已经存在
            var result = GetId(new KeyValuePair<string, object>(BaseNewsEntity.FieldFolderId, folderId), new KeyValuePair<string, object>(BaseNewsEntity.FieldTitle, title));
            if (!string.IsNullOrEmpty(result))
            {
                // 更新数据
                UpdateFile(result, title, contents);
            }
            else
            {
                // 添加数据
                var fileEntity = new BaseNewsEntity
                {
                    FolderId = folderId,
                    Title = title,
                    Contents = contents
                };
                result = AddEntity(fileEntity);
            }
            return result;
        }

        #region public DataTable GetDataTableByFolder(string folderid) 获取信息
        /// <summary>
        /// 获取信息
        /// </summary>
        /// <param name="folderId">主键</param>
        /// <returns>数据表</returns>
        public DataTable GetDataTableByFolder(string folderId)
        {
            var sql = "SELECT " + BaseNewsEntity.FieldId
                    + "        ," + BaseNewsEntity.FieldFolderId
                    + "        ," + BaseNewsEntity.FieldCode
                    + "        ," + BaseNewsEntity.FieldTitle
                    + "        ," + BaseNewsEntity.FieldHomePage
                    + "        ," + BaseNewsEntity.FieldSubPage
                    + "        ," + BaseNewsEntity.FieldFilePath
                    + "        ," + BaseNewsEntity.FieldFileSize
                    + "        ," + BaseNewsEntity.FieldReadCount
                    + "        ," + BaseNewsEntity.FieldCategoryCode
                    + "        ," + BaseNewsEntity.FieldDescription
                    + "        ," + BaseNewsEntity.FieldAuditStatus
                    + "        ," + BaseNewsEntity.FieldEnabled
                    + "        ," + BaseNewsEntity.FieldDeleted
                    + "        ," + BaseNewsEntity.FieldSortCode
                    + "        ," + BaseNewsEntity.FieldCreateUserId
                    + "        ," + BaseNewsEntity.FieldCreateBy
                    + "        ," + BaseNewsEntity.FieldCreateTime
                    + "        ," + BaseNewsEntity.FieldUpdateUserId
                    + "        ," + BaseNewsEntity.FieldUpdateBy
                    + "        ," + BaseNewsEntity.FieldUpdateTime
                    + "       , (SELECT " + BaseFolderEntity.FieldFolderName
                                + " FROM " + BaseFolderEntity.TableName
                                + " WHERE " + BaseFolderEntity.FieldId + " = " + BaseNewsEntity.FieldFolderId + ") AS FolderFullName "
                    + " FROM " + CurrentTableName
                    + " WHERE " + BaseNewsEntity.FieldFolderId + " = " + DbHelper.GetParameter(BaseNewsEntity.FieldFolderId)
                    + " ORDER BY " + BaseNewsEntity.FieldSortCode + " DESC ";
            return DbHelper.Fill(sql, new IDbDataParameter[] { DbHelper.MakeParameter(BaseNewsEntity.FieldFolderId, folderId) });
        }
        #endregion

        #region public string Add(BaseNewsEntity fileEntity, out string statusCode) 添加文件
        /// <summary>
        /// 添加文件
        /// </summary>
        /// <param name="fileEntity"></param>
        /// <param name="statusCode">状态</param>
        /// <returns>主键</returns>
        public string Add(BaseNewsEntity fileEntity, out string statusCode)
        {
            statusCode = string.Empty;
            var result = string.Empty;
            // 检查是否重复
            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseNewsEntity.FieldFolderId, fileEntity.FolderId),
                new KeyValuePair<string, object>(BaseNewsEntity.FieldTitle, fileEntity.Title),
                new KeyValuePair<string, object>(BaseNewsEntity.FieldDeleted, 0)
            };

            if (Exists(parameters))
            {
                // 名称已重复
                statusCode = Status.ErrorNameExist.ToString();
            }
            else
            {
                result = AddEntity(fileEntity);
                // 运行成功
                statusCode = Status.OkAdd.ToString();
            }
            return result;
        }
        #endregion

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="fileEntity"></param>
        /// <param name="statusCode"></param>
        /// <returns></returns>
        public int Update(BaseNewsEntity fileEntity, out string statusCode)
        {
            statusCode = string.Empty;
            var result = UpdateEntity(fileEntity);
            statusCode = result > 0 ? Status.OkUpdate.ToString() : Status.ErrorDeleted.ToString();
            return result;
        }

        /// <summary>
        /// 更新文件
        /// </summary>
        /// <param name="id"></param>
        /// <param name="fileName"></param>
        /// <param name="contents"></param>
        /// <returns></returns>
        public int UpdateFile(string id, string fileName, string contents)
        {
            var sqlBuilder = new SqlBuilder(DbHelper);
            sqlBuilder.BeginUpdate(CurrentTableName);
            sqlBuilder.SetValue(BaseNewsEntity.FieldTitle, fileName);
            sqlBuilder.SetValue(BaseNewsEntity.FieldContents, contents);
            sqlBuilder.SetValue(BaseNewsEntity.FieldFileSize, contents.Length);
            sqlBuilder.SetValue(BaseNewsEntity.FieldUpdateUserId, UserInfo.Id);
            sqlBuilder.SetValue(BaseNewsEntity.FieldUpdateBy, UserInfo.RealName);
            sqlBuilder.SetDbNow(BaseNewsEntity.FieldUpdateTime);
            sqlBuilder.SetWhere(BaseNewsEntity.FieldId, id);
            return sqlBuilder.EndUpdate();
        }

        #region public DataTable Search(string searchKey) 查询
        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="searchKey">查询</param>
        /// <returns>数据表</returns>
        public DataTable Search(string searchKey)
        {
            return Search(string.Empty, string.Empty, searchKey);
        }
        #endregion
        /// <summary>
        /// 
        /// </summary>
        /// <param name="departmentId"></param>
        /// <param name="userId"></param>
        /// <param name="searchKey"></param>
        /// <returns></returns>
        public DataTable Search(string departmentId, string userId, string searchKey)
        {
            return Search(departmentId, userId, string.Empty, searchKey);
        }

        #region public DataTable Search(string userId, string categoryCode, string searchKey) 查询
        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="departmentId">部门编号</param>
        /// <param name="userId">用户主键</param>
        /// <param name="categoryCode">目录</param>
        /// <param name="searchKey">查询条件</param>
        /// <returns>数据表</returns>
        public DataTable Search(string departmentId, string userId, string categoryCode, string searchKey)
        {
            return Search(departmentId, userId, categoryCode, searchKey, null, null);
        }
        #endregion

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="departmentId"></param>
        /// <param name="userId"></param>
        /// <param name="categoryCode"></param>
        /// <param name="searchKey"></param>
        /// <param name="enabled"></param>
        /// <param name="deletionStateCode"></param>
        /// <returns></returns>
        public DataTable Search(string departmentId, string userId, string categoryCode, string searchKey, bool? enabled, bool? deletionStateCode)
        {
            // 一、这里是将Boolean值转换为int类型。
            var delete = 0;
            if (deletionStateCode != null)
            {
                delete = (bool)deletionStateCode ? 1 : 0;
            }

            // 二、这里是开始进行动态SQL语句拼接，字段名、表明都进行了常量定义，表名字段名发生变化时，很容易就知道程序哪里都调用了这些。
            var sql = "SELECT " + BaseNewsEntity.FieldId
                    + "        ," + BaseNewsEntity.FieldFolderId
                    + "        ," + BaseNewsEntity.FieldDepartmentId
                    + "        ," + BaseNewsEntity.FieldDepartmentName
                    + "        ," + BaseNewsEntity.FieldCategoryCode
                    + "        ," + BaseNewsEntity.FieldCode
                    + "        ," + BaseNewsEntity.FieldTitle
                    + "        ," + BaseNewsEntity.FieldSource
                    + "        ," + BaseNewsEntity.FieldIntroduction
                    + "        ," + BaseNewsEntity.FieldKeywords
                    + "        ," + BaseNewsEntity.FieldContents
                    + "        ," + BaseNewsEntity.FieldHomePage
                    + "        ," + BaseNewsEntity.FieldSubPage
                    + "        ," + BaseNewsEntity.FieldFilePath
                    + "        ," + BaseNewsEntity.FieldFileSize
                    + "        ," + BaseNewsEntity.FieldImageUrl
                    + "        ," + BaseNewsEntity.FieldReadCount
                    + "        ," + BaseNewsEntity.FieldDescription
                    + "        ," + BaseNewsEntity.FieldAuditStatus
                    + "        ," + BaseNewsEntity.FieldEnabled
                    + "        ," + BaseNewsEntity.FieldDeleted
                    + "        ," + BaseNewsEntity.FieldSortCode
                    + "        ," + BaseNewsEntity.FieldCreateUserId
                    + "        ," + BaseNewsEntity.FieldCreateBy
                    + "        ," + BaseNewsEntity.FieldCreateTime
                    + "        ," + BaseNewsEntity.FieldUpdateUserId
                    + "        ," + BaseNewsEntity.FieldUpdateBy
                    + "        ," + BaseNewsEntity.FieldUpdateTime
                    + " FROM " + CurrentTableName
                    + " WHERE " + BaseNewsEntity.FieldDeleted + " = " + delete;

            if (enabled != null)
            {
                var isEnabled = (bool)enabled ? 1 : 0;
                sql += string.Format(" AND {0} = {1}", BaseNewsEntity.FieldEnabled, isEnabled);
            }
            if (!string.IsNullOrEmpty(departmentId))
            {
                sql += string.Format(" AND {0} = '{1}' ", BaseNewsEntity.FieldDepartmentId, departmentId);
            }
            // 三、我们认为 userId 这个查询条件是安全，不是人为输入的参数，所以直接进行了SQL语句拼接
            if (!string.IsNullOrEmpty(userId))
            {
                sql += string.Format(" AND {0} = {1}", BaseNewsEntity.FieldCreateUserId, userId);
            }
            /*
            if (!string.IsNullOrEmpty(folderId))
            {
                sql += " AND " + BaseNewsEntity.FieldCategoryCode + " = " + folderId;
            }
            */

            if (!string.IsNullOrEmpty(categoryCode))
            {
                sql += string.Format(" AND {0} = '{1}'", BaseNewsEntity.FieldCategoryCode, categoryCode);
            }

            // 四、这里是进行参数化的准备，因为是多个不确定的查询参数，所以用了List。
            var dbParameters = new List<IDbDataParameter>();

            // 五、这里看查询条件是否为空
            searchKey = searchKey.Trim();
            if (!string.IsNullOrEmpty(searchKey))
            {
                // 六、这里是进行支持多种数据库的参数化查询
                sql += string.Format(" AND ({0} LIKE {1}", BaseNewsEntity.FieldTitle, DbHelper.GetParameter(BaseNewsEntity.FieldTitle));
                sql += string.Format(" OR {0} LIKE {1}", BaseNewsEntity.FieldDepartmentName, DbHelper.GetParameter(BaseNewsEntity.FieldDepartmentName));
                sql += string.Format(" OR {0} LIKE {1}", BaseNewsEntity.FieldCategoryCode, DbHelper.GetParameter(BaseNewsEntity.FieldCategoryCode));
                sql += string.Format(" OR {0} LIKE {1}", BaseNewsEntity.FieldCreateBy, DbHelper.GetParameter(BaseNewsEntity.FieldCreateBy));
                sql += string.Format(" OR {0} LIKE {1}", BaseNewsEntity.FieldContents, DbHelper.GetParameter(BaseNewsEntity.FieldContents));
                sql += string.Format(" OR {0} LIKE {1})", BaseNewsEntity.FieldDescription, DbHelper.GetParameter(BaseNewsEntity.FieldDescription));

                // 七、这里是判断，用户是否已经输入了%
                if (searchKey.IndexOf("%") < 0)
                {
                    searchKey = string.Format("%{0}%", searchKey);
                }

                // 八、这里生成支持多数据库的参数
                dbParameters.Add(DbHelper.MakeParameter(BaseNewsEntity.FieldTitle, searchKey));
                dbParameters.Add(DbHelper.MakeParameter(BaseNewsEntity.FieldDepartmentName, searchKey));
                dbParameters.Add(DbHelper.MakeParameter(BaseNewsEntity.FieldCategoryCode, searchKey));
                dbParameters.Add(DbHelper.MakeParameter(BaseNewsEntity.FieldCreateBy, searchKey));
                dbParameters.Add(DbHelper.MakeParameter(BaseNewsEntity.FieldContents, searchKey));
                dbParameters.Add(DbHelper.MakeParameter(BaseNewsEntity.FieldDescription, searchKey));
            }
            sql += " ORDER BY " + BaseNewsEntity.FieldSortCode + " DESC ";

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
            var fileName = GetProperty(id, BaseNewsEntity.FieldTitle);
            Delete(new KeyValuePair<string, object>(BaseNewsEntity.FieldFolderId, folderId), new KeyValuePair<string, object>(BaseNewsEntity.FieldTitle, fileName));
            return SetProperty(id, new KeyValuePair<string, object>(BaseNewsEntity.FieldFolderId, folderId));
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

        /// <summary>
        /// 读取
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int Read(string id)
        {
            var result = 0;
            var sqlBuilder = new SqlBuilder(DbHelper);
            sqlBuilder.BeginUpdate(CurrentTableName);
            // 增加阅读次数
            sqlBuilder.SetFormula(BaseNewsEntity.FieldReadCount, BaseNewsEntity.FieldReadCount + " + 1");
            sqlBuilder.SetWhere(BaseNewsEntity.FieldId, id);
            result = sqlBuilder.EndUpdate();
            return result;
        }
    }
}