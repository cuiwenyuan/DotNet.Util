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
    /// BaseRoleManager 
    /// 角色表结构定义部分
    ///
    /// 修改记录
    ///
    ///     2015.07.02 版本：1.0 JiRiGaLa 独立修改日志。
    ///     
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2015.07.02</date>
    /// </author>
    /// </summary>
    public partial class BaseRoleManager : BaseManager , IBaseManager
    {
        #region public int Update(BaseRoleEntity entity, out string statusCode) 更新
        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="statusCode">返回状态码</param>
        /// <returns>影响行数</returns>
        public int UniqueUpdate(BaseRoleEntity entity, out string statusCode)
        {
            var result = 0;
            // 检查是否已被其他人修改

            if (DbUtil.IsUpdate(DbHelper, CurrentTableName, entity.Id, entity.UpdateUserId.ToString(), entity.UpdateTime))
            {
                // 数据已经被修改
                statusCode = Status.ErrorChanged.ToString();
            }
            else
            {
                // 检查名称是否重复
                var parameters = new List<KeyValuePair<string, object>>();
                if (!string.IsNullOrEmpty(entity.OrganizationId.ToString()))
                {
                    parameters.Add(new KeyValuePair<string, object>(BaseRoleEntity.FieldOrganizationId, entity.OrganizationId));
                }
                parameters.Add(new KeyValuePair<string, object>(BaseRoleEntity.FieldRealName, entity.RealName));
                parameters.Add(new KeyValuePair<string, object>(BaseRoleEntity.FieldDeleted, 0));
                //检查角色Code是否重复 Troy.Cui 2016-08-17
                var parametersCode = new List<KeyValuePair<string, object>>();
                if (!string.IsNullOrEmpty(entity.OrganizationId.ToString()))
                {
                    parametersCode.Add(new KeyValuePair<string, object>(BaseRoleEntity.FieldOrganizationId, entity.OrganizationId));
                }
                parametersCode.Add(new KeyValuePair<string, object>(BaseRoleEntity.FieldCode, entity.Code));
                parametersCode.Add(new KeyValuePair<string, object>(BaseRoleEntity.FieldDeleted, 0));
                if (Exists(parameters, entity.Id))
                {
                    // 名称已重复
                    statusCode = Status.ErrorNameExist.ToString();
                }
                else if (Exists(parametersCode, entity.Id))
                {
                    // 编码已重复
                    statusCode = Status.ErrorCodeExist.ToString();
                }
                else
                {
                    // 获取原始实体信息
                    var entityOld = GetEntity(entity.Id.ToString());
                    // 保存修改记录
                    UpdateEntityLog(entity, entityOld);

                    result = UpdateEntity(entity);
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

            return result;
        }
        #endregion

        #region public void UpdateEntityLog(BaseRoleEntity newEntity, BaseRoleEntity oldEntity, string tableName = null)
        /// <summary>
        /// 保存实体修改记录
        /// </summary>
        /// <param name="newEntity">修改前的实体对象</param>
        /// <param name="oldEntity">修改后的实体对象</param>
        /// <param name="tableName">表名称</param>
        public void UpdateEntityLog(BaseRoleEntity newEntity, BaseRoleEntity oldEntity, string tableName = null)
        {
            if (string.IsNullOrEmpty(tableName))
            {
                //统一放在一个公共表 Troy.Cui 2016-08-17
                tableName = BaseChangeLogEntity.CurrentTableName;
            }
            var manager = new BaseChangeLogManager(UserInfo, tableName);
            foreach (var property in typeof(BaseRoleEntity).GetProperties())
            {
                var oldValue = Convert.ToString(property.GetValue(oldEntity, null));
                var newValue = Convert.ToString(property.GetValue(newEntity, null));
                var fieldDescription = property.GetCustomAttributes(typeof(FieldDescription), false).FirstOrDefault() as FieldDescription;
                //不记录创建人、修改人、没有修改的记录
                if (!fieldDescription.NeedLog || oldValue == newValue)
                {
                    continue;
                }
                var record = new BaseChangeLogEntity
                {
                    TableName = CurrentTableName,
                    TableDescription = FieldExtensions.ToDescription(typeof(BaseRoleEntity), "CurrentTableName"),
                    ColumnName = property.Name,
                    ColumnDescription = fieldDescription.Text,
                    NewValue = newValue,
                    OldValue = oldValue,                    
                    RecordKey = oldEntity.Id.ToString()
                };
                manager.Add(record, true, false);
            }
        }
        #endregion
    }
}
