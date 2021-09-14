//-----------------------------------------------------------------------
// <copyright file="BaseCalendarEntity.Auto.cs" company="DotNet">
//     Copyright (c) 2020, All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace DotNet.Model
{
    using Util;

    /// <summary>
    /// BaseCalendarEntity
    /// 日历
    /// 
    /// 修改纪录
    /// 
    /// 2020-03-22 版本：1.0 Troy.Cui 创建文件。
    /// 
    /// <author>
    ///     <name>Troy.Cui</name>
    ///     <date>2020-03-22</date>
    /// </author>
    /// </summary>
    public partial class BaseCalendarEntity : BaseEntity
    {
        /// <summary>
        /// 编号
        /// </summary>
        [FieldDescription("编号")]
        public int Id { get; set; }

        /// <summary>
        /// 公司编号
        /// </summary>
        [FieldDescription("公司编号")]
        public int UserCompanyId { get; set; } = 0;

        /// <summary>
        /// 子公司编号
        /// </summary>
        [FieldDescription("子公司编号")]
        public int UserSubCompanyId { get; set; } = 0;

        /// <summary>
        /// 年度
        /// </summary>
        [FieldDescription("年度")]
        public int? FiscalYear { get; set; } = null;

        /// <summary>
        /// 月份
        /// </summary>
        [FieldDescription("月份")]
        public int? FiscalMonth { get; set; } = null;

        /// <summary>
        /// 日
        /// </summary>
        [FieldDescription("日")]
        public int? FiscalDay { get; set; } = null;

        /// <summary>
        /// 操作日期
        /// </summary>
        [FieldDescription("操作日期")]
        public DateTime? TransactionDate { get; set; } = DateTime.Now;

        /// <summary>
        /// 排序编号
        /// </summary>
        [FieldDescription("排序编号")]
        public int? SortCode { get; set; } = 0;

        /// <summary>
        /// 删除状态代码
        /// </summary>
        [FieldDescription("删除状态代码")]
        public int? DeletionStateCode { get; set; } = 0;

        /// <summary>
        /// 是否有效
        /// </summary>
        [FieldDescription("是否有效")]
        public int? Enabled { get; set; } = 1;

        /// <summary>
        /// 创建时间
        /// </summary>
        [FieldDescription("创建时间")]
        public DateTime? CreateOn { get; set; } = DateTime.Now;

        /// <summary>
        /// 创建人编号
        /// </summary>
        [FieldDescription("创建人编号")]
        public int? CreateUserId { get; set; } = null;

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
        /// 最近修改时间
        /// </summary>
        [FieldDescription("最近修改时间")]
        public DateTime? ModifiedOn { get; set; } = DateTime.Now;

        /// <summary>
        /// 最近修改人编号
        /// </summary>
        [FieldDescription("最近修改人编号")]
        public int? ModifiedUserId { get; set; } = null;

        /// <summary>
        /// 最近修改人姓名
        /// </summary>
        [FieldDescription("最近修改人姓名")]
        public string ModifiedBy { get; set; } = string.Empty;

        /// <summary>
        /// 最近修改IP
        /// </summary>
        [FieldDescription("最近修改IP")]
        public string ModifiedIp { get; set; } = string.Empty;

        /// <summary>
        /// 从数据行读取
        /// </summary>
        /// <param name="dr">数据行</param>
        protected override BaseEntity GetFrom(IDataRow dr)
        {
            GetFromExpand(dr);
            if (dr.ContainsColumn(FieldId))
            {
                Id = BaseUtil.ConvertToInt(dr[FieldId]);
            }
            if (dr.ContainsColumn(FieldUserCompanyId))
            {
                UserCompanyId = BaseUtil.ConvertToInt(dr[FieldUserCompanyId]);
            }
            if (dr.ContainsColumn(FieldUserSubCompanyId))
            {
                UserSubCompanyId = BaseUtil.ConvertToInt(dr[FieldUserSubCompanyId]);
            }
            if (dr.ContainsColumn(FieldFiscalYear))
            {
                FiscalYear = BaseUtil.ConvertToNullableInt(dr[FieldFiscalYear]);
            }
            if (dr.ContainsColumn(FieldFiscalMonth))
            {
                FiscalMonth = BaseUtil.ConvertToNullableInt(dr[FieldFiscalMonth]);
            }
            if (dr.ContainsColumn(FieldFiscalDay))
            {
                FiscalDay = BaseUtil.ConvertToNullableInt(dr[FieldFiscalDay]);
            }
            if (dr.ContainsColumn(FieldTransactionDate))
            {
                TransactionDate = BaseUtil.ConvertToNullableDateTime(dr[FieldTransactionDate]);
            }
            if (dr.ContainsColumn(FieldSortCode))
            {
                SortCode = BaseUtil.ConvertToNullableInt(dr[FieldSortCode]);
            }
            if (dr.ContainsColumn(FieldDeleted))
            {
                DeletionStateCode = BaseUtil.ConvertToNullableInt(dr[FieldDeleted]);
            }
            if (dr.ContainsColumn(FieldEnabled))
            {
                Enabled = BaseUtil.ConvertToNullableInt(dr[FieldEnabled]);
            }
            if (dr.ContainsColumn(FieldCreateTime))
            {
                CreateOn = BaseUtil.ConvertToNullableDateTime(dr[FieldCreateTime]);
            }
            if (dr.ContainsColumn(FieldCreateUserId))
            {
                CreateUserId = BaseUtil.ConvertToNullableInt(dr[FieldCreateUserId]);
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
                ModifiedOn = BaseUtil.ConvertToNullableDateTime(dr[FieldUpdateTime]);
            }
            if (dr.ContainsColumn(FieldUpdateUserId))
            {
                ModifiedUserId = BaseUtil.ConvertToNullableInt(dr[FieldUpdateUserId]);
            }
            if (dr.ContainsColumn(FieldUpdateBy))
            {
                ModifiedBy = BaseUtil.ConvertToString(dr[FieldUpdateBy]);
            }
            if (dr.ContainsColumn(FieldUpdateIp))
            {
                ModifiedIp = BaseUtil.ConvertToString(dr[FieldUpdateIp]);
            }
            return this;
        }

        ///<summary>
        /// 日历
        ///</summary>
        [FieldDescription("日历")]
        public const string TableName = "BaseCalendar";

        ///<summary>
        /// 编号
        ///</summary>
        public const string FieldId = "Id";

        ///<summary>
        /// 公司编号
        ///</summary>
        public const string FieldUserCompanyId = "UserCompanyId";

        ///<summary>
        /// 子公司编号
        ///</summary>
        public const string FieldUserSubCompanyId = "UserSubCompanyId";

        ///<summary>
        /// 年度
        ///</summary>
        public const string FieldFiscalYear = "FiscalYear";

        ///<summary>
        /// 月份
        ///</summary>
        public const string FieldFiscalMonth = "FiscalMonth";

        ///<summary>
        /// 日
        ///</summary>
        public const string FieldFiscalDay = "FiscalDay";

        ///<summary>
        /// 操作日期
        ///</summary>
        public const string FieldTransactionDate = "TransactionDate";

        ///<summary>
        /// 排序编号
        ///</summary>
        public const string FieldSortCode = "SortCode";

        ///<summary>
        /// 删除状态代码
        ///</summary>
        public const string FieldDeleted = "DeletionStateCode";

        ///<summary>
        /// 是否有效
        ///</summary>
        public const string FieldEnabled = "Enabled";

        ///<summary>
        /// 创建时间
        ///</summary>
        public const string FieldCreateTime = "CreateOn";

        ///<summary>
        /// 创建人编号
        ///</summary>
        public const string FieldCreateUserId = "CreateUserId";

        ///<summary>
        /// 创建人姓名
        ///</summary>
        public const string FieldCreateBy = "CreateBy";

        ///<summary>
        /// 创建IP
        ///</summary>
        public const string FieldCreateIp = "CreateIP";

        ///<summary>
        /// 最近修改时间
        ///</summary>
        public const string FieldUpdateTime = "ModifiedOn";

        ///<summary>
        /// 最近修改人编号
        ///</summary>
        public const string FieldUpdateUserId = "ModifiedUserId";

        ///<summary>
        /// 最近修改人姓名
        ///</summary>
        public const string FieldUpdateBy = "ModifiedBy";

        ///<summary>
        /// 最近修改IP
        ///</summary>
        public const string FieldUpdateIp = "ModifiedIP";
    }
}
