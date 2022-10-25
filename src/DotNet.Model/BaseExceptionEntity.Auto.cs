//-----------------------------------------------------------------------
// <copyright file="BaseExceptionEntity.Auto.cs" company="DotNet">
//     Copyright (c) 2022, All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace DotNet.Model
{
    using Util;

    /// <summary>
    /// BaseExceptionEntity
    /// 系统异常
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
    public partial class BaseExceptionEntity : BaseEntity
    {
        /// <summary>
        /// 子系统编码
        /// </summary>
        [FieldDescription("子系统编码")]
        [Description("子系统编码")]
        [Column(FieldSystemCode)]
        public string SystemCode { get; set; } = string.Empty;

        /// <summary>
        /// 事件编号
        /// </summary>
        [FieldDescription("事件编号")]
        [Description("事件编号")]
        [Column(FieldEventId)]
        public int? EventId { get; set; } = null;

        /// <summary>
        /// 类别
        /// </summary>
        [FieldDescription("类别")]
        [Description("类别")]
        [Column(FieldCategory)]
        public string Category { get; set; } = string.Empty;

        /// <summary>
        /// 优先级
        /// </summary>
        [FieldDescription("优先级")]
        [Description("优先级")]
        [Column(FieldPriority)]
        public int? Priority { get; set; } = null;

        /// <summary>
        /// 严重级别
        /// </summary>
        [FieldDescription("严重级别")]
        [Description("严重级别")]
        [Column(FieldSeverity)]
        public string Severity { get; set; } = string.Empty;

        /// <summary>
        /// 标题
        /// </summary>
        [FieldDescription("标题")]
        [Description("标题")]
        [Column(FieldTitle)]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// 时间戳
        /// </summary>
        [FieldDescription("时间戳")]
        [Description("时间戳")]
        [Column(FieldTimestamp)]
        public DateTime? Timestamp { get; set; } = null;

        /// <summary>
        /// 机器名
        /// </summary>
        [FieldDescription("机器名")]
        [Description("机器名")]
        [Column(FieldMachineName)]
        public string MachineName { get; set; } = string.Empty;

        /// <summary>
        /// IP地址
        /// </summary>
        [FieldDescription("IP地址")]
        [Description("IP地址")]
        [Column(FieldIpAddress)]
        public string IpAddress { get; set; } = string.Empty;

        /// <summary>
        /// 应用域
        /// </summary>
        [FieldDescription("应用域")]
        [Description("应用域")]
        [Column(FieldAppDomainName)]
        public string AppDomainName { get; set; } = string.Empty;

        /// <summary>
        /// 进程编号
        /// </summary>
        [FieldDescription("进程编号")]
        [Description("进程编号")]
        [Column(FieldProcessId)]
        public string ProcessId { get; set; } = string.Empty;

        /// <summary>
        /// 进程名
        /// </summary>
        [FieldDescription("进程名")]
        [Description("进程名")]
        [Column(FieldProcessName)]
        public string ProcessName { get; set; } = string.Empty;

        /// <summary>
        /// 线程名
        /// </summary>
        [FieldDescription("线程名")]
        [Description("线程名")]
        [Column(FieldThreadName)]
        public string ThreadName { get; set; } = string.Empty;

        /// <summary>
        /// 线程编号
        /// </summary>
        [FieldDescription("线程编号")]
        [Description("线程编号")]
        [Column(FieldWin32ThreadId)]
        public string Win32ThreadId { get; set; } = string.Empty;

        /// <summary>
        /// 消息
        /// </summary>
        [FieldDescription("消息")]
        [Description("消息")]
        [Column(FieldMessage)]
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// 格式化消息
        /// </summary>
        [FieldDescription("格式化消息")]
        [Description("格式化消息")]
        [Column(FieldFormattedMessage)]
        public string FormattedMessage { get; set; } = string.Empty;

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
            if (dr.ContainsColumn(FieldEventId))
            {
                EventId = BaseUtil.ConvertToNullableInt(dr[FieldEventId]);
            }
            if (dr.ContainsColumn(FieldCategory))
            {
                Category = BaseUtil.ConvertToString(dr[FieldCategory]);
            }
            if (dr.ContainsColumn(FieldPriority))
            {
                Priority = BaseUtil.ConvertToNullableInt(dr[FieldPriority]);
            }
            if (dr.ContainsColumn(FieldSeverity))
            {
                Severity = BaseUtil.ConvertToString(dr[FieldSeverity]);
            }
            if (dr.ContainsColumn(FieldTitle))
            {
                Title = BaseUtil.ConvertToString(dr[FieldTitle]);
            }
            if (dr.ContainsColumn(FieldTimestamp))
            {
                Timestamp = BaseUtil.ConvertToNullableDateTime(dr[FieldTimestamp]);
            }
            if (dr.ContainsColumn(FieldMachineName))
            {
                MachineName = BaseUtil.ConvertToString(dr[FieldMachineName]);
            }
            if (dr.ContainsColumn(FieldIpAddress))
            {
                IpAddress = BaseUtil.ConvertToString(dr[FieldIpAddress]);
            }
            if (dr.ContainsColumn(FieldAppDomainName))
            {
                AppDomainName = BaseUtil.ConvertToString(dr[FieldAppDomainName]);
            }
            if (dr.ContainsColumn(FieldProcessId))
            {
                ProcessId = BaseUtil.ConvertToString(dr[FieldProcessId]);
            }
            if (dr.ContainsColumn(FieldProcessName))
            {
                ProcessName = BaseUtil.ConvertToString(dr[FieldProcessName]);
            }
            if (dr.ContainsColumn(FieldThreadName))
            {
                ThreadName = BaseUtil.ConvertToString(dr[FieldThreadName]);
            }
            if (dr.ContainsColumn(FieldWin32ThreadId))
            {
                Win32ThreadId = BaseUtil.ConvertToString(dr[FieldWin32ThreadId]);
            }
            if (dr.ContainsColumn(FieldMessage))
            {
                Message = BaseUtil.ConvertToString(dr[FieldMessage]);
            }
            if (dr.ContainsColumn(FieldFormattedMessage))
            {
                FormattedMessage = BaseUtil.ConvertToString(dr[FieldFormattedMessage]);
            }
            return this;
        }

        ///<summary>
        /// 系统异常
        ///</summary>
        [FieldDescription("系统异常")]
        public const string CurrentTableName = "BaseException";

        ///<summary>
        /// 子系统编码
        ///</summary>
        public const string FieldSystemCode = "SystemCode";

        ///<summary>
        /// 事件编号
        ///</summary>
        public const string FieldEventId = "EventId";

        ///<summary>
        /// 类别
        ///</summary>
        public const string FieldCategory = "Category";

        ///<summary>
        /// 优先级
        ///</summary>
        public const string FieldPriority = "Priority";

        ///<summary>
        /// 严重级别
        ///</summary>
        public const string FieldSeverity = "Severity";

        ///<summary>
        /// 标题
        ///</summary>
        public const string FieldTitle = "Title";

        ///<summary>
        /// 时间戳
        ///</summary>
        public const string FieldTimestamp = "Timestamp";

        ///<summary>
        /// 机器名
        ///</summary>
        public const string FieldMachineName = "MachineName";

        ///<summary>
        /// IP地址
        ///</summary>
        public const string FieldIpAddress = "IpAddress";

        ///<summary>
        /// 应用域
        ///</summary>
        public const string FieldAppDomainName = "AppDomainName";

        ///<summary>
        /// 进程编号
        ///</summary>
        public const string FieldProcessId = "ProcessId";

        ///<summary>
        /// 进程名
        ///</summary>
        public const string FieldProcessName = "ProcessName";

        ///<summary>
        /// 线程名
        ///</summary>
        public const string FieldThreadName = "ThreadName";

        ///<summary>
        /// 线程编号
        ///</summary>
        public const string FieldWin32ThreadId = "Win32ThreadId";

        ///<summary>
        /// 消息
        ///</summary>
        public const string FieldMessage = "Message";

        ///<summary>
        /// 格式化消息
        ///</summary>
        public const string FieldFormattedMessage = "FormattedMessage";
    }
}