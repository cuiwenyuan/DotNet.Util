﻿//-----------------------------------------------------------------------
// <copyright file="BaseLogEntity.Auto.cs" company="DotNet">
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
    /// BaseLogEntity
    /// 系统日志
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
    public partial class BaseLogEntity : BaseEntity
    {
        /// <summary>
        /// 公司主键
        /// </summary>
        [FieldDescription("公司主键")]
        [Description("公司主键")]
        [Column(FieldCompanyId)]
        public int CompanyId { get; set; } = 0;

        /// <summary>
        /// 用户主键
        /// </summary>
        [FieldDescription("用户主键")]
        [Description("用户主键")]
        [Column(FieldUserId)]
        public int UserId { get; set; } = 0;

        /// <summary>
        /// 用户名
        /// </summary>
        [FieldDescription("用户名")]
        [Description("用户名")]
        [Column(FieldUserName)]
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// 用户姓名
        /// </summary>
        [FieldDescription("用户姓名")]
        [Description("用户姓名")]
        [Column(FieldRealName)]
        public string RealName { get; set; } = string.Empty;

        /// <summary>
        /// 服务
        /// </summary>
        [FieldDescription("服务")]
        [Description("服务")]
        [Column(FieldService)]
        public string Service { get; set; } = string.Empty;

        /// <summary>
        /// 任务
        /// </summary>
        [FieldDescription("任务")]
        [Description("任务")]
        [Column(FieldTaskId)]
        public string TaskId { get; set; } = string.Empty;

        /// <summary>
        /// 操作记录,添加,编辑,删除参数
        /// </summary>
        [FieldDescription("操作记录,添加,编辑,删除参数")]
        [Description("操作记录,添加,编辑,删除参数")]
        [Column(FieldParameters)]
        public string Parameters { get; set; } = string.Empty;

        /// <summary>
        /// IP地址
        /// </summary>
        [FieldDescription("IP地址")]
        [Description("IP地址")]
        [Column(FieldClientIp)]
        public string ClientIp { get; set; } = string.Empty;

        /// <summary>
        /// IP地址
        /// </summary>
        [FieldDescription("IP地址")]
        [Description("IP地址")]
        [Column(FieldServerIp)]
        public string ServerIp { get; set; } = string.Empty;

        /// <summary>
        /// 上一网络地址
        /// </summary>
        [FieldDescription("上一网络地址")]
        [Description("上一网络地址")]
        [Column(FieldUrlReferrer)]
        public string UrlReferrer { get; set; } = string.Empty;

        /// <summary>
        /// 网络地址
        /// </summary>
        [FieldDescription("网络地址")]
        [Description("网络地址")]
        [Column(FieldWebUrl)]
        public string WebUrl { get; set; } = string.Empty;

        /// <summary>
        /// 耗时
        /// </summary>
        [FieldDescription("耗时")]
        [Description("耗时")]
        [Column(FieldElapsedTicks)]
        public decimal? ElapsedTicks { get; set; } = 0;

        /// <summary>
        /// 开始时间
        /// </summary>
        [FieldDescription("开始时间")]
        [Description("开始时间")]
        [Column(FieldStartTime)]
        public DateTime? StartTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 描述
        /// </summary>
        [FieldDescription("描述")]
        [Description("描述")]
        [Column(FieldDescription)]
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// 从数据行读取
        /// </summary>
        /// <param name="dr">数据行</param>
        protected override BaseEntity GetFrom(IDataRow dr)
        {
            GetFromExtend(dr);
            GetBase(dr);
            if (dr.ContainsColumn(FieldCompanyId))
            {
                CompanyId = BaseUtil.ConvertToInt(dr[FieldCompanyId]);
            }
            if (dr.ContainsColumn(FieldUserId))
            {
                UserId = BaseUtil.ConvertToInt(dr[FieldUserId]);
            }
            if (dr.ContainsColumn(FieldUserName))
            {
                UserName = BaseUtil.ConvertToString(dr[FieldUserName]);
            }
            if (dr.ContainsColumn(FieldRealName))
            {
                RealName = BaseUtil.ConvertToString(dr[FieldRealName]);
            }
            if (dr.ContainsColumn(FieldService))
            {
                Service = BaseUtil.ConvertToString(dr[FieldService]);
            }
            if (dr.ContainsColumn(FieldTaskId))
            {
                TaskId = BaseUtil.ConvertToString(dr[FieldTaskId]);
            }
            if (dr.ContainsColumn(FieldParameters))
            {
                Parameters = BaseUtil.ConvertToString(dr[FieldParameters]);
            }
            if (dr.ContainsColumn(FieldClientIp))
            {
                ClientIp = BaseUtil.ConvertToString(dr[FieldClientIp]);
            }
            if (dr.ContainsColumn(FieldServerIp))
            {
                ServerIp = BaseUtil.ConvertToString(dr[FieldServerIp]);
            }
            if (dr.ContainsColumn(FieldUrlReferrer))
            {
                UrlReferrer = BaseUtil.ConvertToString(dr[FieldUrlReferrer]);
            }
            if (dr.ContainsColumn(FieldWebUrl))
            {
                WebUrl = BaseUtil.ConvertToString(dr[FieldWebUrl]);
            }
            if (dr.ContainsColumn(FieldElapsedTicks))
            {
                ElapsedTicks = BaseUtil.ConvertToNullableDecimal(dr[FieldElapsedTicks]);
            }
            if (dr.ContainsColumn(FieldStartTime))
            {
                StartTime = BaseUtil.ConvertToNullableDateTime(dr[FieldStartTime]);
            }
            if (dr.ContainsColumn(FieldDescription))
            {
                Description = BaseUtil.ConvertToString(dr[FieldDescription]);
            }
            return this;
        }

        ///<summary>
        /// 系统日志
        ///</summary>
        [FieldDescription("系统日志")]
        public const string CurrentTableName = "BaseLog";

        ///<summary>
        /// 表名
        ///</summary>
        public const string CurrentTableDescription = "系统日志";

        ///<summary>
        /// 公司主键
        ///</summary>
        public const string FieldCompanyId = "CompanyId";

        ///<summary>
        /// 用户主键
        ///</summary>
        public const string FieldUserId = "UserId";

        ///<summary>
        /// 用户名
        ///</summary>
        public const string FieldUserName = "UserName";

        ///<summary>
        /// 用户姓名
        ///</summary>
        public const string FieldRealName = "RealName";

        ///<summary>
        /// 服务
        ///</summary>
        public const string FieldService = "Service";

        ///<summary>
        /// 任务
        ///</summary>
        public const string FieldTaskId = "TaskId";

        ///<summary>
        /// 操作记录,添加,编辑,删除参数
        ///</summary>
        public const string FieldParameters = "Parameters";

        ///<summary>
        /// IP地址
        ///</summary>
        public const string FieldClientIp = "ClientIp";

        ///<summary>
        /// IP地址
        ///</summary>
        public const string FieldServerIp = "ServerIp";

        ///<summary>
        /// 上一网络地址
        ///</summary>
        public const string FieldUrlReferrer = "UrlReferrer";

        ///<summary>
        /// 网络地址
        ///</summary>
        public const string FieldWebUrl = "WebUrl";

        ///<summary>
        /// 耗时
        ///</summary>
        public const string FieldElapsedTicks = "ElapsedTicks";

        ///<summary>
        /// 开始时间
        ///</summary>
        public const string FieldStartTime = "StartTime";

        ///<summary>
        /// 描述
        ///</summary>
        public const string FieldDescription = "Description";
    }
}