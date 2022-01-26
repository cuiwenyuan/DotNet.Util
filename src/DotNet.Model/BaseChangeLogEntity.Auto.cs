﻿//-----------------------------------------------------------------------
// <copyright file="BaseChangeLogEntity.Auto.cs" company="DotNet">
//     Copyright (c) 2021, All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;


namespace DotNet.Model
{
    using Util;

    /// <summary>
    /// BaseChangeLogEntity
    /// 变更日志表
    /// 
    /// 修改记录
    /// 
    /// 2021-10-04 版本：1.0 Troy.Cui 创建文件。
    /// 
    /// <author>
    ///     <name>Troy.Cui</name>
    ///     <date>2021-10-04</date>
    /// </author>
    /// </summary>
    public partial class BaseChangeLogEntity : BaseEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        [FieldDescription("主键")]
        public int Id { get; set; }

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
        /// 列名
        /// </summary>
        [FieldDescription("列名")]
        public string ColumnName { get; set; } = string.Empty;

        /// <summary>
        /// 列备注
        /// </summary>
        [FieldDescription("列备注")]
        public string ColumnDescription { get; set; } = string.Empty;

        /// <summary>
        /// 记录主键
        /// </summary>
        [FieldDescription("记录主键")]
        public string RecordKey { get; set; } = string.Empty;

        /// <summary>
        /// 原值主键
        /// </summary>
        [FieldDescription("原值主键")]
        public string OldKey { get; set; } = string.Empty;

        /// <summary>
        /// 原值
        /// </summary>
        [FieldDescription("原值")]
        public string OldValue { get; set; } = string.Empty;

        /// <summary>
        /// 现值主键
        /// </summary>
        [FieldDescription("现值主键")]
        public string NewKey { get; set; } = string.Empty;

        /// <summary>
        /// 现值
        /// </summary>
        [FieldDescription("现值")]
        public string NewValue { get; set; } = string.Empty;

        /// <summary>
        /// 排序编号
        /// </summary>
        [FieldDescription("排序编号")]
        public int SortCode { get; set; } = 0;

        /// <summary>
        /// 是否删除
        /// </summary>
        [FieldDescription("是否删除")]
        public int Deleted { get; set; } = 0;

        /// <summary>
        /// 是否有效
        /// </summary>
        [FieldDescription("是否有效")]
        public int Enabled { get; set; } = 1;

        /// <summary>
        /// 创建时间
        /// </summary>
        [FieldDescription("创建时间")]
        public DateTime CreateTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 创建人编号
        /// </summary>
        [FieldDescription("创建人编号")]
        public int CreateUserId { get; set; } = 0;

        /// <summary>
        /// 创建人用户名
        /// </summary>
        [FieldDescription("创建人用户名")]
        public string CreateUserName { get; set; } = string.Empty;

        /// <summary>
        /// 创建人姓名
        /// </summary>
        [FieldDescription("创建人姓名")]
        public string CreateBy { get; set; } = string.Empty;

        /// <summary>
        /// 创建IP
        /// </summary>
        [FieldDescription("创建IP")]
        public string CreateIp { get; set; } = string.Empty;

        /// <summary>
        /// 修改时间
        /// </summary>
        [FieldDescription("修改时间")]
        public DateTime UpdateTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 修改人编号
        /// </summary>
        [FieldDescription("修改人编号")]
        public int UpdateUserId { get; set; } = 0;

        /// <summary>
        /// 修改人用户名
        /// </summary>
        [FieldDescription("修改人用户名")]
        public string UpdateUserName { get; set; } = string.Empty;

        /// <summary>
        /// 修改人姓名
        /// </summary>
        [FieldDescription("修改人姓名")]
        public string UpdateBy { get; set; } = string.Empty;

        /// <summary>
        /// 修改IP
        /// </summary>
        [FieldDescription("修改IP")]
        public string UpdateIp { get; set; } = string.Empty;

        /// <summary>
        /// 从数据行读取
        /// </summary>
        /// <param name="dr">数据行</param>
        protected override BaseEntity GetFrom(IDataRow dr)
        {
            GetFromExtend(dr);
            if (dr.ContainsColumn(FieldId))
            {
                Id = BaseUtil.ConvertToInt(dr[FieldId]);
            }
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
            if (dr.ContainsColumn(FieldSortCode))
            {
                SortCode = BaseUtil.ConvertToInt(dr[FieldSortCode]);
            }
            if (dr.ContainsColumn(FieldDeleted))
            {
                Deleted = BaseUtil.ConvertToInt(dr[FieldDeleted]);
            }
            if (dr.ContainsColumn(FieldEnabled))
            {
                Enabled = BaseUtil.ConvertToInt(dr[FieldEnabled]);
            }
            if (dr.ContainsColumn(FieldCreateTime))
            {
                CreateTime = BaseUtil.ConvertToDateTime(dr[FieldCreateTime]);
            }
            if (dr.ContainsColumn(FieldCreateUserId))
            {
                CreateUserId = BaseUtil.ConvertToInt(dr[FieldCreateUserId]);
            }
            if (dr.ContainsColumn(FieldCreateUserName))
            {
                CreateUserName = BaseUtil.ConvertToString(dr[FieldCreateUserName]);
            }
            if (dr.ContainsColumn(FieldCreateBy))
            {
                CreateBy = BaseUtil.ConvertToString(dr[FieldCreateBy]);
            }
            if (dr.ContainsColumn(FieldCreateIp))
            {
                CreateIp = BaseUtil.ConvertToString(dr[FieldCreateIp]);
            }
            if (dr.ContainsColumn(FieldUpdateTime))
            {
                UpdateTime = BaseUtil.ConvertToDateTime(dr[FieldUpdateTime]);
            }
            if (dr.ContainsColumn(FieldUpdateUserId))
            {
                UpdateUserId = BaseUtil.ConvertToInt(dr[FieldUpdateUserId]);
            }
            if (dr.ContainsColumn(FieldUpdateUserName))
            {
                UpdateUserName = BaseUtil.ConvertToString(dr[FieldUpdateUserName]);
            }
            if (dr.ContainsColumn(FieldUpdateBy))
            {
                UpdateBy = BaseUtil.ConvertToString(dr[FieldUpdateBy]);
            }
            if (dr.ContainsColumn(FieldUpdateIp))
            {
                UpdateIp = BaseUtil.ConvertToString(dr[FieldUpdateIp]);
            }
            return this;
        }

        ///<summary>
        /// 变更日志表
        ///</summary>
        [FieldDescription("变更日志表")]
        public const string CurrentTableName = "BaseChangeLog";

        ///<summary>
        /// 主键
        ///</summary>
        public const string FieldId = "Id";

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

        ///<summary>
        /// 排序编号
        ///</summary>
        public const string FieldSortCode = "SortCode";

        ///<summary>
        /// 是否删除
        ///</summary>
        public const string FieldDeleted = "Deleted";

        ///<summary>
        /// 是否有效
        ///</summary>
        public const string FieldEnabled = "Enabled";

        ///<summary>
        /// 创建时间
        ///</summary>
        public const string FieldCreateTime = "CreateTime";

        ///<summary>
        /// 创建人编号
        ///</summary>
        public const string FieldCreateUserId = "CreateUserId";

        ///<summary>
        /// 创建人用户名
        ///</summary>
        public const string FieldCreateUserName = "CreateUserName";

        ///<summary>
        /// 创建人姓名
        ///</summary>
        public const string FieldCreateBy = "CreateBy";

        ///<summary>
        /// 创建IP
        ///</summary>
        public const string FieldCreateIp = "CreateIp";

        ///<summary>
        /// 修改时间
        ///</summary>
        public const string FieldUpdateTime = "UpdateTime";

        ///<summary>
        /// 修改人编号
        ///</summary>
        public const string FieldUpdateUserId = "UpdateUserId";

        ///<summary>
        /// 修改人用户名
        ///</summary>
        public const string FieldUpdateUserName = "UpdateUserName";

        ///<summary>
        /// 修改人姓名
        ///</summary>
        public const string FieldUpdateBy = "UpdateBy";

        ///<summary>
        /// 修改IP
        ///</summary>
        public const string FieldUpdateIp = "UpdateIp";
    }
}