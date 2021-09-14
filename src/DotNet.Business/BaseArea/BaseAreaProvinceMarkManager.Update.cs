//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System;
using System.Linq;

namespace DotNet.Business
{
    using Model;
    using Util;

    /// <summary>
    /// BaseAreaProvinceMarkManager 
    /// 地区表(省、市、县)
    ///
    /// 修改记录
    ///
    ///		2015.07.03 版本：1.0 JiRiGaLa  修改记录独立化。
    ///
    /// <author>
    ///		<name>JiRiGaLa</name>
    ///		<date>2015.07.03</date>
    /// </author>
    /// </summary>
    public partial class BaseAreaProvinceMarkManager : BaseManager
    {
        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int Update(BaseAreaProvinceMarkEntity entity)
        {
            // 获取原始实体信息
            var entityOld = GetObject(entity.Id);
            // 保存修改记录
            UpdateEntityLog(entity, entityOld);

            return UpdateObject(entity);
        }

        #region public void UpdateEntityLog(BaseAreaProvinceMarkEntity newEntity, BaseAreaProvinceMarkEntity oldEntity)
        /// <summary>
        /// 实体修改记录
        /// </summary>
        /// <param name="newEntity">修改前的实体对象</param>
        /// <param name="oldEntity">修改后的实体对象</param>
        /// <param name="tableName">表名称</param>
        public void UpdateEntityLog(BaseAreaProvinceMarkEntity newEntity, BaseAreaProvinceMarkEntity oldEntity, string tableName = null)
        {
            if (string.IsNullOrEmpty(tableName))
            {
                //统一放在一个公共表 Troy.Cui 2016-08-17
                tableName = BaseModifyRecordEntity.TableName;
            }
            var manager = new BaseModifyRecordManager(UserInfo, tableName);
            foreach (var property in typeof(BaseAreaProvinceMarkEntity).GetProperties())
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
                    TableDescription = FieldExtensions.ToDescription(typeof(BaseAreaProvinceMarkEntity), "TableName"),
                    RecordKey = oldEntity.Id,
                    IpAddress = Utils.GetIp()
                };
                manager.Add(record, true, false);
            }
        }
        #endregion
    }
}