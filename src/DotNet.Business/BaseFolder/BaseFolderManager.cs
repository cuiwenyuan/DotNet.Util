//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System.Collections.Generic;
using System.Data;

namespace DotNet.Business
{
    using Model;
    using Util;

    /// <summary>
    ///	BaseFolderManager
    ///	
    ///	修改记录
    /// 
    ///     2007.06.06 版本：2.5 JiRiGaLa   统一进行主键整理。
    ///     2007.05.29 版本：2.4 JiRiGaLa   ErrorDeleted，ErrorChanged 状态进行改进整理。
    ///	    2007.05.29 版本：2.3 JiRiGaLa   BatchSave，ErrorDataRelated，force 进行改进整理。
    ///	    2007.05.23 版本：2.2 JiRiGaLa   StatusCode，StatusMessage 进行改进。
    ///	    2007.05.12 版本：2.1 JiRiGaLa   BatchSave 进行改进。
    ///	    2007.01.04 版本：2.0 JiRiGaLa   重新整理主键。
    ///	    2006.02.12 版本：1.2 JiRiGaLa   调整主键的规范化。
    ///	    2006.02.12 版本：1.2 JiRiGaLa   调整主键的规范化。
    ///	    2004.11.19 版本：1.1 JiRiGaLa   增加员工类别判断部分。
    ///     2004.11.18 版本：1.0 JiRiGaLa   主键进行了绝对的优化，这是个好东西啊，平时要多用，用得要灵活些。
    ///		
    /// <author>
    ///		<name>JiRiGaLa</name>
    ///		<date>2007.06.06</date>
    /// </author> 
    /// </summary>
    public partial class BaseFolderManager : BaseManager
    {
        #region public string Add(BaseFolderEntity entity, out string statusCode) 添加
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="entity">文件夹的基类表结构定义</param>
        /// <param name="statusCode">状态返回码</param>
        /// <returns>主键</returns>
        public string Add(BaseFolderEntity entity, out string statusCode)
        {
            var result = string.Empty;
            // 检查文件夹名是否重复
            if (Exists(new KeyValuePair<string, object>(BaseFolderEntity.FieldParentId, entity.ParentId), new KeyValuePair<string, object>(BaseFolderEntity.FieldFolderName, entity.FolderName)))
            {
                // 文件夹名已重复
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

        #region public int Update(BaseFolderEntity folderEntity, out string statusCode) 更新
        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="entity">文件夹的基类表结构定义</param>
        /// <param name="statusCode">状态返回码</param>
        /// <returns>影响行数</returns>
        public int Update(BaseFolderEntity entity, out string statusCode)
        {
            var result = 0;
            //if (DbUtil.IsModifed(DbHelper, BaseFolderEntity.TableName, folderEntity.Id, folderEntity.ModifiedUserId, folderEntity.ModifiedOn))
            //{
            //    // 数据已经被修改
            //    statusCode = StatusCode.ErrorChanged.ToString();
            //}
            //else
            //{
                // 检查文件夹名是否重复
                if (Exists(new KeyValuePair<string, object>(BaseFolderEntity.FieldParentId, entity.ParentId), new KeyValuePair<string, object>(BaseFolderEntity.FieldFolderName, entity.FolderName), entity.Id))
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

        #region public DataTable Search(string searchKey) 查询
        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="searchKey">查询</param>
        /// <returns>数据表</returns>
        public DataTable Search(string searchKey)
        {
            var sql = "SELECT * "
                     + " FROM " + BaseFolderEntity.TableName
                     + " WHERE " + BaseFolderEntity.FieldFolderName + " LIKE " + DbHelper.GetParameter(BaseFolderEntity.FieldFolderName)
                     + " OR " + BaseFolderEntity.FieldDescription + " LIKE " + DbHelper.GetParameter(BaseFolderEntity.FieldDescription);
            var dt = new DataTable(BaseFolderEntity.TableName);
            searchKey = searchKey.Trim();
            if (searchKey.IndexOf("%") < 0)
            {
                searchKey = string.Format("%{0}%", searchKey);
            }
            var dbParameters = new List<IDbDataParameter>
            {
                DbHelper.MakeParameter(BaseFolderEntity.FieldFolderName, searchKey),
                DbHelper.MakeParameter(BaseFolderEntity.FieldDescription, searchKey)
            };
            DbHelper.Fill(dt, sql, dbParameters.ToArray());
            return dt;
        }
        #endregion

        /// <summary>
        /// 文件夹检查
        /// </summary>
        public void FolderCheck()
        {
            FolderCheck(UserInfo.DepartmentId, UserInfo.DepartmentName);
        }

        #region public void FolderCheck(string departmentId, string departmentName)
        /// <summary>
        /// 检查相应的系统必备文件夹
        /// </summary>
        public void FolderCheck(string departmentId, string departmentName, bool sendReceive = false)
        {
            var entity = new BaseFolderEntity
            {
                Enabled = 1,
                AllowDelete = 0,
                AllowEdit = 0,
                Description = AppMessage.FileServiceSystemCreateDirectory
            };
            // 01:判断公司文件夹是否存在？
            if (!Exists("CompanyFile"))
            {
                entity.FolderName = AppMessage.FileServiceCompanyFile;
                entity.Enabled = 1;
                entity.AllowDelete = 0;
                entity.AllowEdit = 0;
                entity.IsPublic = 0;
                entity.Id = "CompanyFile";
                AddEntity(entity);
            }
            // 01:判断公司文件夹是否存在？
            if (!Exists("ShareFolder"))
            {
                entity.FolderName = AppMessage.FileServiceShareFolder;
                entity.Enabled = 1;
                entity.AllowDelete = 0;
                entity.AllowEdit = 0;
                entity.IsPublic = 1;
                entity.Id = "ShareFolder";
                entity.ParentId = "CompanyFile";
                AddEntity(entity);
            }
            // 02:部门文件夹
            if (!string.IsNullOrEmpty(departmentId) && !Exists(departmentId))
            {
                entity.FolderName = departmentName;
                entity.ParentId = "CompanyFile";
                entity.Enabled = 1;
                entity.IsPublic = 0;
                entity.AllowDelete = 0;
                entity.AllowEdit = 0;
                entity.Id = departmentId;
                AddEntity(entity);

                /*
                if (!this.Exists(departmentId + "_Public"))
                {
                    folderEntity = new BaseFolderEntity();
                    folderEntity.FolderName = "公共文档";
                    folderEntity.ParentId = departmentId;
                    folderEntity.Enabled = 1;
                    folderEntity.IsPublic = 1;
                    folderEntity.AllowDelete = 0;
                    folderEntity.AllowEdit = 0;
                    folderEntity.Id = departmentId + "_Public";
                    this.AddEntity(folderEntity);
                }
                */
            }
            // 03:用户空间
            /*
            if (!this.Exists("UserSpace"))
            {
                folderEntity.FolderName = AppMessage.FileService_UserSpace;
                folderEntity.AllowDelete = 0;
                folderEntity.AllowEdit = 0;
                folderEntity.Id = "UserSpace";
                this.AddEntity(folderEntity);
            }
            */
            // 04:判断用户的空间是否存在？
            if (!Exists(UserInfo.Id))
            {
                entity.FolderName = UserInfo.RealName + AppMessage.FileServiceFolder;
                entity.ParentId = "UserSpace";
                if (!string.IsNullOrEmpty(UserInfo.DepartmentId))
                {
                    entity.ParentId = UserInfo.DepartmentId;
                }
                entity.Id = UserInfo.Id;
                entity.IsPublic = 0;
                entity.Enabled = 1;
                entity.AllowDelete = 0;
                entity.AllowEdit = 0;
                AddEntity(entity);
            }
            else
            {
                if (!string.IsNullOrEmpty(UserInfo.DepartmentId))
                {
                    SetProperty(new KeyValuePair<string, object>(BaseFolderEntity.FieldId, UserInfo.Id), new KeyValuePair<string, object>(BaseFolderEntity.FieldParentId, UserInfo.DepartmentId));
                }
            }
            if (sendReceive)
            {
                // 05:判断已经已发文件是否存在？
                if (!Exists(UserInfo.Id + "_Send"))
                {
                    entity.FolderName = AppMessage.FileServiceSendFile;
                    entity.ParentId = UserInfo.Id;
                    entity.Enabled = 1;
                    entity.AllowDelete = 1;
                    entity.IsPublic = 0;
                    entity.AllowEdit = 1;
                    entity.Id = UserInfo.Id + "_Send";
                    AddEntity(entity);
                }
                // 06:判断接收文件是否存在？
                if (!Exists(UserInfo.Id + "_Receive"))
                {
                    entity.FolderName = AppMessage.FileServiceReceiveFile;
                    entity.ParentId = UserInfo.Id;
                    entity.Enabled = 1;
                    entity.AllowDelete = 1;
                    entity.IsPublic = 0;
                    entity.AllowEdit = 1;
                    entity.Id = UserInfo.Id + "_Receive";
                    AddEntity(entity);
                }
            }
        }
        #endregion

        #region public int MoveTo(string id, string parentId) 移动
        /// <summary>
        /// 移动
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="parentId">父级主键</param>
        /// <returns>影响行数</returns>
        public int MoveTo(string id, string parentId)
        {
            return SetProperty(id, new KeyValuePair<string, object>(BaseOrganizeEntity.FieldParentId, parentId));
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
            var entity = new BaseFolderEntity();
            foreach (DataRow dr in dt.Rows)
            {
                // 删除状态
                if (dr.RowState == DataRowState.Deleted)
                {
                    var id = dr[BaseFolderEntity.FieldId, DataRowVersion.Original].ToString();
                    if (id.Length > 0)
                    {
                        result += DeleteObject(id);
                    }
                }
                // 被修改过
                if (dr.RowState == DataRowState.Modified)
                {
                    var id = dr[BaseFolderEntity.FieldId, DataRowVersion.Original].ToString();
                    if (id.Length > 0)
                    {
                        entity.GetFrom(dr);
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
    }
}