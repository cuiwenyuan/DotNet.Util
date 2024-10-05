//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2024, DotNet.
//-----------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;


namespace DotNet.Business
{
    using Util;
    using Model;

    /// <summary>
    ///	BaseManager
    /// 通用基类部分
    /// 
    /// 修改记录
    /// 
    /// 
    ///		2022.04.26 版本：1.0 Troy.Cui 新增。
    ///
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2022.04.26</date>
    /// </author> 
    /// </summary>
    public partial class BaseManager : IBaseManager
    {
        #region SaveEntityChangeLog 保存实体修改记录
        /// <summary>
        /// 保存实体修改记录
        /// </summary>
        /// <param name="recordKey">记录主键</param>
        /// <param name="entityNew">修改后的实体对象</param>
        /// <param name="entityOld">修改前的实体对象</param>
        /// <param name="tableName">表名</param>
        /// <param name="systemCode">子系统编码</param>
        public virtual void SaveEntityChangeLog(string recordKey, object entityOld, object entityNew, string tableName = null, string systemCode = null)
        {
            var oldType = entityOld.GetType();
            var newType = entityNew.GetType();

            if (newType.Equals(oldType) && oldType.IsClass && newType.IsClass)
            {
                if (string.IsNullOrEmpty(systemCode))
                {
                    systemCode = BaseSystemInfo.SystemCode;
                }
                if (string.IsNullOrEmpty(tableName))
                {
                    tableName = BaseChangeLogEntity.CurrentTableName;
                }
                var manager = new BaseChangeLogManager(UserInfo, tableName);
                foreach (var property in oldType.GetProperties())
                {
                    var oldValue = Convert.ToString(property.GetValue(entityOld, null));
                    var newValue = Convert.ToString(property.GetValue(entityNew, null));
                    var fieldDescription = property.GetCustomAttributes(typeof(FieldDescription), false).FirstOrDefault() as FieldDescription;
                    if (!fieldDescription.NeedLog || oldValue == newValue)
                    {
                        continue;
                    }
                    var baseChangeLogEntity = new BaseChangeLogEntity
                    {
                        SystemCode = systemCode,
                        TableName = CurrentTableName,
                        TableDescription = CurrentTableDescription,
                        ColumnName = property.Name,
                        ColumnDescription = fieldDescription.Text,
                        NewValue = newValue,
                        OldValue = oldValue,
                        RecordKey = recordKey,
                        SortCode = 1 // 不要排序了，加快写入速度
                    };
                    manager.Add(baseChangeLogEntity, true, false);
                }
            }
            else
            {
                LogUtil.WriteLog("比较的实体类型不一样或非实体类型", "SaveEntityChangeLog");
                //throw new ArgumentException();
            }
        }
        #endregion
    }
}