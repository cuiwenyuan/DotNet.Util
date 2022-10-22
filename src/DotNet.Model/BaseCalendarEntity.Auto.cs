//-----------------------------------------------------------------------
// <copyright file="BaseCalendarEntity.Auto.cs" company="DotNet">
//     Copyright (c) 2022, All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;

namespace DotNet.Model
{
    using Util;

    /// <summary>
    /// BaseCalendarEntity
    /// 日历
    /// 
    /// 修改记录
    /// 
    /// 2021-09-27 版本：2.0 Troy.Cui 创建文件
    /// 2020-03-22 版本：1.0 Troy.Cui 创建文件。
    /// 
    /// <author>
    ///     <name>Troy.Cui</name>
    ///     <date>2021-09-27</date>
    /// </author>
    /// </summary>
    [Table(CurrentTableName)]
    public partial class BaseCalendarEntity : BaseEntity
    {
        /// <summary>
        /// 公司编号
        /// </summary>
        [FieldDescription("公司编号")]
        [Description("公司编号")]
        [Column(FieldUserCompanyId)]
        public int UserCompanyId { get; set; } = 0;

        /// <summary>
        /// 子公司编号
        /// </summary>
        [FieldDescription("子公司编号")]
        [Description("子公司编号")]
        [Column(FieldUserSubCompanyId)]
        public int UserSubCompanyId { get; set; } = 0;

        /// <summary>
        /// 年度
        /// </summary>
        [FieldDescription("年度")]
        [Description("年度")]
        [Column(FieldFiscalYear)]
        public int? FiscalYear { get; set; } = null;

        /// <summary>
        /// 月份
        /// </summary>
        [FieldDescription("月份")]
        [Description("月份")]
        [Column(FieldFiscalMonth)]
        public int? FiscalMonth { get; set; } = null;

        /// <summary>
        /// 日
        /// </summary>
        [FieldDescription("日")]
        [Description("日")]
        [Column(FieldFiscalDay)]
        public int? FiscalDay { get; set; } = null;

        /// <summary>
        /// 操作日期
        /// </summary>
        [FieldDescription("操作日期")]
        [Description("操作日期")]
        [Column(FieldTransactionDate)]
        public DateTime? TransactionDate { get; set; } = DateTime.Now;

        /// <summary>
        /// 从数据行读取
        /// </summary>
        /// <param name="dr">数据行</param>
        protected override BaseEntity GetFrom(IDataRow dr)
        {
            GetFromExtend(dr);
            GetBase(dr);
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
                FiscalMonth = BaseUtil.ConvertToNullableByteInt(dr[FieldFiscalMonth]);
            }
            if (dr.ContainsColumn(FieldFiscalDay))
            {
                FiscalDay = BaseUtil.ConvertToNullableByteInt(dr[FieldFiscalDay]);
            }
            if (dr.ContainsColumn(FieldTransactionDate))
            {
                TransactionDate = BaseUtil.ConvertToNullableDateTime(dr[FieldTransactionDate]);
            }
            return this;
        }

        ///<summary>
        /// 日历
        ///</summary>
        [FieldDescription("日历")]
        public const string CurrentTableName = "BaseCalendar";

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
    }
}
