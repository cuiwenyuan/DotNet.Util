﻿//-----------------------------------------------------------------
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
    /// BaseModuleManager
    /// 模块(菜单)类（程序OK）
    /// 
    /// 修改记录
    /// 
    ///		2015.07.02 版本：1.0 JiRiGaLa 独立出来修改记录 
    ///
    /// </summary>
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2015.07.02</date>
    /// </author>
    public partial class BaseModuleManager : BaseManager
    {
        #region public int Update(BaseModuleEntity entity, out string statusCode) 更新
        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="statusCode">返回状态码</param>
        /// <returns>返回</returns>
        public int Update(BaseModuleEntity entity, out string statusCode)
        {
            var result = 0;
            // 检查是否已被其他人修改            
            //if (DbUtil.IsModifed(DbHelper, BaseModuleEntity.TableName, moduleEntity.Id, moduleEntity.ModifiedUserId, moduleEntity.ModifiedOn))
            //{
            //    // 数据已经被修改
            //    statusCode = StatusCode.ErrorChanged.ToString();
            //}
            //else
            //{

            var parameters = new List<KeyValuePair<string, object>>();
            if (!string.IsNullOrEmpty(entity.ParentId))
            {
                parameters.Add(new KeyValuePair<string, object>(BaseModuleEntity.FieldParentId, entity.ParentId));
            }
            else
            {
                parameters.Add(new KeyValuePair<string, object>(BaseModuleEntity.FieldParentId, null));
            }
            parameters.Add(new KeyValuePair<string, object>(BaseModuleEntity.FieldCode, entity.Code));
            parameters.Add(new KeyValuePair<string, object>(BaseModuleEntity.FieldFullName, entity.FullName));
            parameters.Add(new KeyValuePair<string, object>(BaseModuleEntity.FieldDeleted, 0));

            // 检查编号是否重复
            if ((entity.Code.Length > 0) && (Exists(parameters, entity.Id)))
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
                // 2015-07-14 吉日嘎拉 只有允许修改的，才可以修改，不允许修改的，不让修改，但是把修改记录会保存起来的。
                if (entityOld.AllowEdit.HasValue && entityOld.AllowEdit.Value == 1)
                {
                    result = UpdateEntity(entity);
                    statusCode = Status.AccessDeny.ToString();
                }
                if (result == 1)
                {
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

        #region 实体修改记录 public void UpdateEntityLog(BaseModuleEntity newEntity, BaseModuleEntity oldEntity)
        /// <summary>
        /// 保存实体修改记录
        /// </summary>
        /// <param name="newEntity">修改前的实体对象</param>
        /// <param name="oldEntity">修改后的实体对象</param>
        /// <param name="tableName">表名称</param>
        public void UpdateEntityLog(BaseModuleEntity newEntity, BaseModuleEntity oldEntity, string tableName = null)
        {
            if (string.IsNullOrEmpty(tableName))
            {
                //统一放在一个公共表 Troy.Cui 2016-08-17
                tableName = BaseModifyRecordEntity.TableName;
            }
            var manager = new BaseModifyRecordManager(UserInfo, tableName);
            foreach (var property in typeof(BaseModuleEntity).GetProperties())
            {
                var oldValue = Convert.ToString(property.GetValue(oldEntity, null));
                var newValue = Convert.ToString(property.GetValue(newEntity, null));
                var fieldDescription = property.GetCustomAttributes(typeof(FieldDescription), false).FirstOrDefault() as FieldDescription;
                //不记录创建人、修改人、没有修改的记录
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
                    TableDescription = FieldExtensions.ToDescription(typeof(BaseModuleEntity), "TableName"),
                    RecordKey = oldEntity.Id,
                    IpAddress = Utils.GetIp()
                };
                manager.Add(record, true, false);
            }
        }
        #endregion
    }
}