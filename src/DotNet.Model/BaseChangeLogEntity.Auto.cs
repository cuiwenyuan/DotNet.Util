//-----------------------------------------------------------------------
// <copyright file="BaseChangeLogEntity.Auto.cs" company="DotNet">
//     Copyright (c) 2025, All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace DotNet.Model
{
    using Util;

    /// <summary>
    /// BaseChangeLogEntity
    /// 变更日志
    /// 
    /// 修改记录
    /// 
    /// 2022-10-23 版本：1.0 Troy.Cui 创建文件。
    /// 
    /// <author>
    ///     <name>Troy.Cui</name>
    ///     <date>2022-10-23</date>
    /// </author>
    /// </summary>
    [Table(CurrentTableName)]
    public partial class BaseChangeLogEntity : BaseEntity
    {
        /// <summary>
        /// 子系统编码
        /// </summary>
        [FieldDescription("子系统编码")]
        [Description("子系统编码")]
        [Column(FieldSystemCode)]
        public string SystemCode { get; set; } = "Base";

        /// <summary>
        /// 表名
        /// </summary>
        [FieldDescription("表名")]
        [Description("表名")]
        [Column(FieldTableName)]
        public string TableName { get; set; } = string.Empty;

        /// <summary>
        /// 表备注
        /// </summary>
        [FieldDescription("表备注")]
        [Description("表备注")]
        [Column(FieldTableDescription)]
        public string TableDescription { get; set; } = string.Empty;

        /// <summary>
        /// 列名
        /// </summary>
        [FieldDescription("列名")]
        [Description("列名")]
        [Column(FieldColumnName)]
        public string ColumnName { get; set; } = string.Empty;

        /// <summary>
        /// 列备注
        /// </summary>
        [FieldDescription("列备注")]
        [Description("列备注")]
        [Column(FieldColumnDescription)]
        public string ColumnDescription { get; set; } = string.Empty;

        /// <summary>
        /// 记录主键
        /// </summary>
        [FieldDescription("记录主键")]
        [Description("记录主键")]
        [Column(FieldRecordKey)]
        public string RecordKey { get; set; } = string.Empty;

        /// <summary>
        /// 原值主键
        /// </summary>
        [FieldDescription("原值主键")]
        [Description("原值主键")]
        [Column(FieldOldKey)]
        public string OldKey { get; set; } = string.Empty;

        /// <summary>
        /// 原值
        /// </summary>
        [FieldDescription("原值")]
        [Description("原值")]
        [Column(FieldOldValue)]
        public string OldValue { get; set; } = string.Empty;

        /// <summary>
        /// 现值主键
        /// </summary>
        [FieldDescription("现值主键")]
        [Description("现值主键")]
        [Column(FieldNewKey)]
        public string NewKey { get; set; } = string.Empty;

        /// <summary>
        /// 现值
        /// </summary>
        [FieldDescription("现值")]
        [Description("现值")]
        [Column(FieldNewValue)]
        public string NewValue { get; set; } = string.Empty;

        /// <summary>
        /// 从数据行读取
        /// </summary>
        /// <param name="dr">数据行</param>
        protected override BaseEntity GetFrom(IDataRow dr)
        {
            GetFromExtend(dr);
            GetBase(dr);
            if (dr.ContainsColumn(FieldSystemCode))
            {
                SystemCode = BaseUtil.ConvertToString(dr[FieldSystemCode]);
            }
            if (dr.ContainsColumn(FieldTableName))
            {
                TableName = BaseUtil.ConvertToString(dr[FieldTableName]);
            }
            if (dr.ContainsColumn(FieldTableDescription))
            {
                TableDescription = BaseUtil.ConvertToString(dr[FieldTableDescription]);
            }
            if (dr.ContainsColumn(FieldColumnName))
            {
                ColumnName = BaseUtil.ConvertToString(dr[FieldColumnName]);
            }
            if (dr.ContainsColumn(FieldColumnDescription))
            {
                ColumnDescription = BaseUtil.ConvertToString(dr[FieldColumnDescription]);
            }
            if (dr.ContainsColumn(FieldRecordKey))
            {
                RecordKey = BaseUtil.ConvertToString(dr[FieldRecordKey]);
            }
            if (dr.ContainsColumn(FieldOldKey))
            {
                OldKey = BaseUtil.ConvertToString(dr[FieldOldKey]);
            }
            if (dr.ContainsColumn(FieldOldValue))
            {
                OldValue = BaseUtil.ConvertToString(dr[FieldOldValue]);
            }
            if (dr.ContainsColumn(FieldNewKey))
            {
                NewKey = BaseUtil.ConvertToString(dr[FieldNewKey]);
            }
            if (dr.ContainsColumn(FieldNewValue))
            {
                NewValue = BaseUtil.ConvertToString(dr[FieldNewValue]);
            }
            return this;
        }

        ///<summary>
        /// 变更日志
        ///</summary>
        [FieldDescription("变更日志")]
        public const string CurrentTableName = "BaseChangeLog";

        ///<summary>
        /// 表名
        ///</summary>
        public const string CurrentTableDescription = "变更日志";

        ///<summary>
        /// 子系统编码
        ///</summary>
        public const string FieldSystemCode = "SystemCode";

        ///<summary>
        /// 表名
        ///</summary>
        public const string FieldTableName = "TableName";

        ///<summary>
        /// 表备注
        ///</summary>
        public const string FieldTableDescription = "TableDescription";

        ///<summary>
        /// 列名
        ///</summary>
        public const string FieldColumnName = "ColumnName";

        ///<summary>
        /// 列备注
        ///</summary>
        public const string FieldColumnDescription = "ColumnDescription";

        ///<summary>
        /// 记录主键
        ///</summary>
        public const string FieldRecordKey = "RecordKey";

        ///<summary>
        /// 原值主键
        ///</summary>
        public const string FieldOldKey = "OldKey";

        ///<summary>
        /// 原值
        ///</summary>
        public const string FieldOldValue = "OldValue";

        ///<summary>
        /// 现值主键
        ///</summary>
        public const string FieldNewKey = "NewKey";

        ///<summary>
        /// 现值
        ///</summary>
        public const string FieldNewValue = "NewValue";
    }
}
