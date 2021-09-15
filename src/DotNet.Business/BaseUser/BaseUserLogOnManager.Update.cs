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
    /// BaseUserLogOnManager
    /// 用户管理
    /// 
    /// 修改记录
    /// 
    ///     2015.07.02 版本：1.0 JiRiGaLa 修改记录独立 
    ///     
    /// <author>
    ///		<name>JiRiGaLa</name>
    ///		<date>2015.07.02</date>
    /// </author> 
    /// </summary>
    public partial class BaseUserLogOnManager
    {
        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="entity">实体</param>
        public int Update(BaseUserLogOnEntity entity)
        {
            var result = 0;
            // 获取原始实体信息
            var entityOld = GetEntity(entity.Id);
            //2016-11-23 欧腾飞加入判断 原始实体信息如果为null 则不去保存修改记录(原始方法 实体可能为null如果去保存修改记录则会报错)
            if (entityOld != null)
            {
                // 保存修改记录
                UpdateEntityLog(entity, entityOld);
            }
            // 更新数据
            result = UpdateEntity(entity);
            // 重新缓存

            return result;
        }

        #region public void UpdateEntityLog(BaseUserLogOnEntity newEntity, BaseUserLogOnEntity oldEntity, string tableName = null)
        /// <summary>
        /// 保存实体修改记录
        /// </summary>
        /// <param name="newEntity">修改前的实体对象</param>
        /// <param name="oldEntity">修改后的实体对象</param>
        /// <param name="tableName">表名称</param>
        public void UpdateEntityLog(BaseUserLogOnEntity newEntity, BaseUserLogOnEntity oldEntity, string tableName = null)
        {
            if (string.IsNullOrEmpty(tableName))
            {
                tableName = BaseUserEntity.TableName + "_LOG";
                //统一放在一个公共表 Troy.Cui 2016-08-17
                tableName = BaseModifyRecordEntity.TableName;
            }
            var manager = new BaseModifyRecordManager(UserInfo, tableName);
            foreach (var property in typeof(BaseUserLogOnEntity).GetProperties())
            {
                var oldValue = Convert.ToString(property.GetValue(oldEntity, null));
                var newValue = Convert.ToString(property.GetValue(newEntity, null));
                var fieldDescription = property.GetCustomAttributes(typeof(FieldDescription), false).FirstOrDefault() as FieldDescription;
                //不记录创建人、修改人、没有修改的记录
                if (!fieldDescription.NeedLog || oldValue == newValue)
                {
                    continue;
                }
                var record = new BaseModifyRecordEntity();
                record.ColumnCode = property.Name.ToUpper();
                record.ColumnDescription = fieldDescription.Text;
                record.NewValue = newValue;
                record.OldValue = oldValue;
                record.TableCode = CurrentTableName.ToUpper();
                record.TableDescription = FieldExtensions.ToDescription(typeof(BaseUserLogOnEntity), "TableName");
                record.RecordKey = oldEntity.Id;
                record.IpAddress = Utils.GetIp();
                manager.Add(record, true, false);
            }
        }
        #endregion
    }
}