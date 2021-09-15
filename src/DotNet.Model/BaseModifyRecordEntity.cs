//-----------------------------------------------------------------------
// <copyright file="BaseModifyRecordEntity.cs" company="DotNet">
//     Copyright (C) 2021, All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;


namespace DotNet.Model
{
    using Util;

    /// <summary>
    /// BaseModifyRecordEntity
    /// 修改记录表
    /// 
    /// 修改记录
    /// 
    /// 2013-12-16 版本：1.0 JiRiGaLa 创建文件。
    /// 
    /// <author>
    ///     <name>JiRiGaLa</name>
    ///     <date>2013-12-16</date>
    /// </author>
    /// </summary>
    [Serializable]
    public partial class BaseModifyRecordEntity : BaseEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 表名
        /// </summary>
        public string TableCode { get; set; } = string.Empty;

        /// <summary>
        /// 表备注
        /// </summary>
        public string TableDescription { get; set; } = string.Empty;

        /// <summary>
        /// 列名
        /// </summary>
        public string ColumnCode { get; set; } = string.Empty;

        /// <summary>
        /// 列备注
        /// </summary>
        public string ColumnDescription { get; set; } = string.Empty;

        /// <summary>
        /// 记录主键
        /// </summary>
        public string RecordKey { get; set; } = string.Empty;

        /// <summary>
        /// 原值主键
        /// </summary>
        public string OldKey { get; set; } = string.Empty;

        /// <summary>
        /// 原值
        /// </summary>
        public string OldValue { get; set; } = string.Empty;

        /// <summary>
        /// 现值主键
        /// </summary>
        public string NewKey { get; set; } = string.Empty;

        /// <summary>
        /// 现值
        /// </summary>
        public string NewValue { get; set; } = string.Empty;

        /// <summary>
        /// IPAddress 地址
        /// </summary>
        public string IpAddress { get; set; } = string.Empty;

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateOn { get; set; } = DateTime.Now;

        /// <summary>
        /// 创建人用户编号
        /// </summary>
        public string CreateUserId { get; set; } = string.Empty;

        /// <summary>
        /// 创建人
        /// </summary>
        public string CreateBy { get; set; } = string.Empty;

        /// <summary>
        /// 从数据行读取
        /// </summary>
        /// <param name="dr">数据行</param>
        protected override BaseEntity GetFrom(IDataRow dr)
        {
            Id = BaseUtil.ConvertToInt(dr[FieldId]);
            TableCode = BaseUtil.ConvertToString(dr[FieldTableCode]);
            TableDescription = BaseUtil.ConvertToString(dr[FieldTableDescription]);
            ColumnCode = BaseUtil.ConvertToString(dr[FieldColumnCode]);
            ColumnDescription = BaseUtil.ConvertToString(dr[FieldColumnDescription]);
            RecordKey = BaseUtil.ConvertToString(dr[FieldRecordKey]);
            OldKey = BaseUtil.ConvertToString(dr[FieldOldKey]);
            OldValue = BaseUtil.ConvertToString(dr[FieldOldValue]);
            NewKey = BaseUtil.ConvertToString(dr[FieldNewKey]);
            NewValue = BaseUtil.ConvertToString(dr[FieldNewValue]);
            IpAddress = BaseUtil.ConvertToString(dr[FieldIpAddress]);
            CreateOn = BaseUtil.ConvertToNullableDateTime(dr[FieldCreateTime]);
            CreateUserId = BaseUtil.ConvertToString(dr[FieldCreateUserId]);
            CreateBy = BaseUtil.ConvertToString(dr[FieldCreateBy]);
            // 获取扩展属性
            GetFromExtend(dr);
            return this;
        }

        ///<summary>
        /// 修改记录表
        ///</summary>
        [NonSerialized]
        public const string TableName = "BaseModifyRecord";

        ///<summary>
        /// 主键
        ///</summary>
        [NonSerialized]
        public const string FieldId = "Id";

        ///<summary>
        /// 表名
        ///</summary>
        [NonSerialized]
        public const string FieldTableCode = "TableCode";

        ///<summary>
        /// 表备注
        ///</summary>
        [NonSerialized]
        public const string FieldTableDescription = "TableDescription";

        ///<summary>
        /// 列名
        ///</summary>
        [NonSerialized]
        public const string FieldColumnCode = "ColumnCode";

        ///<summary>
        /// 列备注
        ///</summary>
        [NonSerialized]
        public const string FieldColumnDescription = "ColumnDescription";

        ///<summary>
        /// 记录主键
        /// </summary>
        [NonSerialized]
        public const string FieldRecordKey = "RecordKey";

        ///<summary>
        /// 原值主键
        ///</summary>
        [NonSerialized]
        public const string FieldOldKey = "OldKey";

        ///<summary>
        /// 原值
        ///</summary>
        [NonSerialized]
        public const string FieldOldValue = "OldValue";

        ///<summary>
        /// 现值主键
        ///</summary>
        [NonSerialized]
        public const string FieldNewKey = "NewKey";

        ///<summary>
        /// 现值
        ///</summary>
        [NonSerialized]
        public const string FieldNewValue = "NewValue";

        ///<summary>
        /// IPAddress 地址
        ///</summary>
        [NonSerialized]
        public const string FieldIpAddress = "IPAddress";

        ///<summary>
        /// 创建时间
        ///</summary>
        [NonSerialized]
        public const string FieldCreateTime = "CreateOn";

        ///<summary>
        /// 创建人用户编号
        ///</summary>
        [NonSerialized]
        public const string FieldCreateUserId = "CreateUserId";

        ///<summary>
        /// 创建人
        ///</summary>
        [NonSerialized]
        public const string FieldCreateBy = "CreateBy";
    }
}
