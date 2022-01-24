//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;

namespace DotNet.Business
{
    using Model;
    using Util;

    /// <summary>
    /// BaseDepartmentManager
    /// 部门表
    ///
    /// 修改记录
    ///     
    ///     2015.07.02 版本：1.0 JiRiGaLa 独立修改记录。
    ///
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2015.07.02</date>
    /// </author>
    /// </summary>
    public partial class BaseDepartmentManager : BaseManager //, IBaseOrganizationManager
    {
        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="statusCode"></param>
        /// <returns></returns>
        public int Update(BaseDepartmentEntity entity, out string statusCode)
        {
            var result = 0;

            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseDepartmentEntity.FieldParentId, entity.ParentId),
                new KeyValuePair<string, object>(BaseDepartmentEntity.FieldFullName, entity.FullName),
                new KeyValuePair<string, object>(BaseDepartmentEntity.FieldDeleted, 0)
            };

            if (Exists(parameters, entity.Id))
            {
                // 名称已重复
                statusCode = Status.ErrorNameExist.ToString();
            }
            else
            {
                // 检查编号是否重复
                parameters = new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>(BaseDepartmentEntity.FieldCode, entity.Code),
                    new KeyValuePair<string, object>(BaseDepartmentEntity.FieldDeleted, 0)
                };

                if (entity.Code.Length > 0 && Exists(parameters, entity.Id))
                {
                    // 编号已重复
                    statusCode = Status.ErrorCodeExist.ToString();
                }
                else
                {
                    // 获取原始实体信息
                    var entityOld = GetEntity(entity.Id);
                    // 保存修改记录
                    UpdateEntityLog(entity, entityOld);

                    // 1:更新部门的信息
                    result = UpdateEntity(entity);
                    // 2:组织机构修改时，用户表的公司，部门，工作组数据给同步更新。
                    var userManager = new BaseUserManager(DbHelper, UserInfo);
                    userManager.SetProperty(new KeyValuePair<string, object>(BaseUserEntity.FieldDepartmentId, entity.Id), new KeyValuePair<string, object>(BaseUserEntity.FieldDepartmentName, entity.FullName));
                    // 03：组织机构修改时，文件夹同步更新
                    // BaseFolderManager folderManager = new BaseFolderManager(this.DbHelper, this.UserInfo);
                    // folderManager.SetProperty(new KeyValuePair<string, object>(BaseFolderEntity.FieldFolderName, entity.FullName), new KeyValuePair<string, object>(BaseFolderEntity.FieldId, entity.Id));
                    if (result == 1)
                    {
                        statusCode = Status.OkUpdate.ToString();
                    }
                    else
                    {
                        statusCode = Status.ErrorDeleted.ToString();
                    }
                }
            }
            //}
            return result;
        }

        #region public void UpdateEntityLog(BaseDepartmentEntity newEntity, BaseDepartmentEntity oldEntity)
        /// <summary>
        /// 实体修改记录
        /// </summary>
        /// <param name="newEntity">修改前的实体对象</param>
        /// <param name="oldEntity">修改后的实体对象</param>
        /// <param name="tableName">表名称</param>
        public void UpdateEntityLog(BaseDepartmentEntity newEntity, BaseDepartmentEntity oldEntity, string tableName = null)
        {
            if (string.IsNullOrEmpty(tableName))
            {
                //统一放在一个公共表 Troy.Cui 2016-08-17
                tableName = BaseModifyRecordEntity.TableName;
            }
            var manager = new BaseModifyRecordManager(UserInfo, tableName);
            foreach (var property in typeof(BaseDepartmentEntity).GetProperties())
            {
                var oldValue = Convert.ToString(property.GetValue(oldEntity, null));
                var newValue = Convert.ToString(property.GetValue(newEntity, null));
                //不记录创建人、修改人、没有修改的记录
                var fieldDescription = property.GetCustomAttributes(typeof(FieldDescription), false).FirstOrDefault() as FieldDescription;
                if (!fieldDescription.NeedLog || oldValue == newValue)
                {
                    continue;
                }
                var record = new BaseModifyRecordEntity
                {
                    ColumnCode = property.Name.ToUpper(),
                    ColumnDescription = fieldDescription.Text,
                    NewValue = newValue,
                    OldValue = oldValue,
                    TableCode = CurrentTableName.ToUpper(),
                    TableDescription = FieldExtensions.ToDescription(typeof(BaseDepartmentEntity), "TableName"),
                    RecordKey = oldEntity.Id.ToString(),
                    IpAddress = Utils.GetIp()
                };
                manager.Add(record, true, false);
            }
        }
        #endregion
    }
}