//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System;
using System.Linq;
using DotNet.Util;

namespace DotNet.Business
{
    using Model;

    /// <summary>
    /// BaseUserLogonManager
    /// 用户管理
    /// 
    /// 修改记录
    /// 
    ///     2015.07.02 版本：1.0 JiRiGaLa 修改记录独立 
    ///     
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2015.07.02</date>
    /// </author> 
    /// </summary>
    public partial class BaseUserLogonManager
    {
        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="logHistory">记录历史</param>
        public int Update(BaseUserLogonEntity entity, bool logHistory)
        {
            var result = 0;
            if (logHistory)
            {
                // 获取原始实体信息
                var entityOld = GetEntity(entity.Id);
                //2016-11-23 欧腾飞加入判断 原始实体信息如果为null 则不去保存修改记录(原始方法 实体可能为null如果去保存修改记录则会报错)
                if (entityOld != null)
                {
                    // 保存修改记录
                    SaveEntityChangeLog(entity, entityOld);
                }
            }            
            // 更新数据
            result = UpdateEntity(entity);
            // 重新缓存

            return result;
        }

        #region SaveEntityChangeLog
        /// <summary>
        /// 保存实体修改记录
        /// </summary>
        /// <param name="entityNew">修改后的实体对象</param>
        /// <param name="entityOld">修改前的实体对象</param>
        /// <param name="tableName">表名称</param>
        public void SaveEntityChangeLog(BaseUserLogonEntity entityNew, BaseUserLogonEntity entityOld, string tableName = null)
        {
            if (string.IsNullOrEmpty(tableName))
            {
                //统一放在一个公共表 Troy.Cui 2016-08-17
                tableName = BaseChangeLogEntity.CurrentTableName;
            }
            var manager = new BaseChangeLogManager(UserInfo, tableName);
            foreach (var property in typeof(BaseUserLogonEntity).GetProperties())
            {
                var oldValue = Convert.ToString(property.GetValue(entityOld, null));
                var newValue = Convert.ToString(property.GetValue(entityNew, null));
                var fieldDescription = property.GetCustomAttributes(typeof(FieldDescription), false).FirstOrDefault() as FieldDescription;
                //不记录创建人、修改人、没有修改的记录
                if (!fieldDescription.NeedLog || oldValue == newValue)
                {
                    continue;
                }
                var entity = new BaseChangeLogEntity
                {
                    TableName = CurrentTableName,
                    TableDescription = FieldExtensions.ToDescription(typeof(BaseUserLogonEntity), "CurrentTableName"),
                    ColumnName = property.Name,
                    ColumnDescription = fieldDescription.Text,
                    NewValue = newValue,
                    OldValue = oldValue,                    
                    RecordKey = entityOld.Id.ToString()
                };
                manager.Add(entity, true, false);
            }
        }
        #endregion
    }
}