//-----------------------------------------------------------------------
// <copyright file="BaseOperationLogEntity.Auto.cs" company="DotNet">
//     Copyright (c) 2022, All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;

namespace DotNet.Model
{
    using Util;

    /// <summary>
    /// BaseOperationLogEntity
    /// 操作日志
    /// 
    /// 修改记录
    /// 
    /// 2021-11-08 版本：1.0 Troy.Cui 创建文件。
    /// 
    /// <author>
    ///     <name>Troy.Cui</name>
    ///     <date>2021-11-08</date>
    /// </author>
    /// </summary>
    public partial class BaseOperationLogEntity : BaseEntity
    {
        /// <summary>
        /// 子系统编码
        /// </summary>
        [FieldDescription("子系统编码")]
        public string SystemCode { get; set; } = "Base";

        /// <summary>
        /// 表名
        /// </summary>
        [FieldDescription("表名")]
        public string TableName { get; set; } = string.Empty;

        /// <summary>
        /// 表备注
        /// </summary>
        [FieldDescription("表备注")]
        public string TableDescription { get; set; } = string.Empty;

        /// <summary>
        /// 操作类型
        /// </summary>
        [FieldDescription("操作类型")]
        public string Operation { get; set; } = string.Empty;

        /// <summary>
        /// 记录主键
        /// </summary>
        [FieldDescription("记录主键")]
        public string RecordKey { get; set; } = string.Empty;

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
            if (dr.ContainsColumn(FieldOperation))
            {
                Operation = BaseUtil.ConvertToString(dr[FieldOperation]);
            }
            if (dr.ContainsColumn(FieldRecordKey))
            {
                RecordKey = BaseUtil.ConvertToString(dr[FieldRecordKey]);
            }
            return this;
        }

        ///<summary>
        /// 操作日志
        ///</summary>
        [FieldDescription("操作日志")]
        public const string CurrentTableName = "BaseOperationLog";

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
        /// 操作类型
        ///</summary>
        public const string FieldOperation = "Operation";

        ///<summary>
        /// 记录主键
        ///</summary>
        public const string FieldRecordKey = "RecordKey";
    }
}
