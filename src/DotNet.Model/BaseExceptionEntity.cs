//-----------------------------------------------------------------------
// <copyright file="BaseExceptionEntity.Auto.cs" company="DotNet">
//     Copyright (c) 2016, All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace DotNet.Model
{
    using Util;


    /// <summary>
    /// BaseExceptionEntity
    /// 系统异常表
    /// 
    /// 修改纪录
    /// 
    /// 2016-12-21 版本：1.0 Troy.Cui 创建文件。
    /// 
    /// <author>
    ///     <name>Troy.Cui</name>
    ///     <date>2016-12-21</date>
    /// </author>
    /// </summary>
    [Serializable]
    public partial class BaseExceptionEntity : BaseEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
		[FieldDescription("主键")]
        public int Id { get; set; }
        /// <summary>
        /// 子系统编号
        /// </summary>
        [FieldDescription("子系统编号")]
        public string SystemCode { get; set; } = string.Empty;
        /// <summary>
        /// 事件编号
        /// </summary>
        [FieldDescription("事件编号")]
        public int? EventId { get; set; } = null;
        /// <summary>
        /// 类别
        /// </summary>
        [FieldDescription("类别")]
        public string Category { get; set; } = string.Empty;
        /// <summary>
        /// 优先级
        /// </summary>
        [FieldDescription("优先级")]
        public int? Priority { get; set; } = null;
        /// <summary>
        /// 严重级别
        /// </summary>
        [FieldDescription("严重级别")]
        public string Severity { get; set; } = string.Empty;
        /// <summary>
        /// 标题
        /// </summary>
        [FieldDescription("标题")]
        public string Title { get; set; } = string.Empty;
        /// <summary>
        /// 时间戳
        /// </summary>
        [FieldDescription("时间戳")]
        public DateTime? Timestamp { get; set; } = null;
        /// <summary>
        /// 机器名
        /// </summary>
        [FieldDescription("机器名")]
        public string MachineName { get; set; } = string.Empty;
        /// <summary>
        /// IP地址
        /// </summary>
        [FieldDescription("IP地址")]
        public string IpAddress { get; set; } = string.Empty;
        /// <summary>
        /// 应用域
        /// </summary>
        [FieldDescription("应用域")]
        public string AppDomainName { get; set; } = string.Empty;
        /// <summary>
        /// 进程编号
        /// </summary>
        [FieldDescription("进程编号")]
        public string ProcessId { get; set; } = string.Empty;
        /// <summary>
        /// 进程名
        /// </summary>
        [FieldDescription("进程名")]
        public string ProcessName { get; set; } = string.Empty;
        /// <summary>
        /// 线程名
        /// </summary>
        [FieldDescription("线程名")]
        public string ThreadName { get; set; } = string.Empty;
        /// <summary>
        /// 线程编号
        /// </summary>
        [FieldDescription("线程编号")]
        public string Win32ThreadId { get; set; } = string.Empty;
        /// <summary>
        /// 消息
        /// </summary>
        [FieldDescription("消息")]
        public string Message { get; set; } = string.Empty;
        /// <summary>
        /// 格式化消息
        /// </summary>
        [FieldDescription("格式化消息")]
        public string FormattedMessage { get; set; } = string.Empty;
        /// <summary>
        /// 创建人主键
        /// </summary>
        [FieldDescription("创建人主键")]
        public string CreateUserId { get; set; } = string.Empty;
        /// <summary>
        /// 创建人
        /// </summary>
        [FieldDescription("创建人")]
        public string CreateBy { get; set; } = string.Empty;
        /// <summary>
        /// 创建时间
        /// </summary>
        [FieldDescription("创建时间")]
        public DateTime CreateOn { get; set; } = DateTime.Now;

        #region public void ClearProperty()
        /// <summary>
        /// 清除内容
        /// </summary>
        public void ClearProperty()
        {
            CreateBy = string.Empty;
        }
        #endregion

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
            if (dr.ContainsColumn(FieldCreateUserId))
            {
                CreateUserId = BaseUtil.ConvertToString(dr[FieldCreateUserId]);
            }
            if (dr.ContainsColumn(FieldCreateBy))
            {
                CreateBy = BaseUtil.ConvertToString(dr[FieldCreateBy]);
            }
            if (dr.ContainsColumn(FieldCreateTime))
            {
                CreateOn = BaseUtil.ConvertToDateTime(dr[FieldCreateTime]);
            }
            return this;
        }

        ///<summary>
        /// 系统异常表
        ///</summary>
		[NonSerialized]
        [FieldDescription("系统异常表")]
        public const string TableName = "BaseException";

        ///<summary>
        /// 主键
        ///</summary>
        [NonSerialized]
        public const string FieldId = "Id";

        ///<summary>
        /// 子系统编号
        ///</summary>
        [NonSerialized]
        public const string FieldSystemCode = "SystemCode";

        ///<summary>
        /// 事件编号
        ///</summary>
        [NonSerialized]
        public const string FieldEventId = "EventId";

        ///<summary>
        /// 类别
        ///</summary>
        [NonSerialized]
        public const string FieldCategory = "Category";

        ///<summary>
        /// 优先级
        ///</summary>
        [NonSerialized]
        public const string FieldPriority = "Priority";

        ///<summary>
        /// 严重级别
        ///</summary>
        [NonSerialized]
        public const string FieldSeverity = "Severity";

        ///<summary>
        /// 标题
        ///</summary>
        [NonSerialized]
        public const string FieldTitle = "Title";

        ///<summary>
        /// 时间戳
        ///</summary>
        [NonSerialized]
        public const string FieldTimestamp = "Timestamp";

        ///<summary>
        /// 机器名
        ///</summary>
        [NonSerialized]
        public const string FieldMachineName = "MachineName";

        ///<summary>
        /// IP地址
        ///</summary>
        [NonSerialized]
        public const string FieldIpAddress = "IPAddress";

        ///<summary>
        /// 应用域
        ///</summary>
        [NonSerialized]
        public const string FieldAppDomainName = "AppDomainName";

        ///<summary>
        /// 进程编号
        ///</summary>
        [NonSerialized]
        public const string FieldProcessId = "ProcessId";

        ///<summary>
        /// 进程名
        ///</summary>
        [NonSerialized]
        public const string FieldProcessName = "ProcessName";

        ///<summary>
        /// 线程名
        ///</summary>
        [NonSerialized]
        public const string FieldThreadName = "ThreadName";

        ///<summary>
        /// 线程编号
        ///</summary>
        [NonSerialized]
        public const string FieldWin32ThreadId = "Win32ThreadId";

        ///<summary>
        /// 消息
        ///</summary>
        [NonSerialized]
        public const string FieldMessage = "Message";

        ///<summary>
        /// 格式化消息
        ///</summary>
        [NonSerialized]
        public const string FieldFormattedMessage = "FormattedMessage";

        ///<summary>
        /// 创建人主键
        ///</summary>
        [NonSerialized]
        public const string FieldCreateUserId = "CreateUserId";

        ///<summary>
        /// 创建人
        ///</summary>
        [NonSerialized]
        public const string FieldCreateBy = "CreateBy";

        ///<summary>
        /// 创建时间
        ///</summary>
        [NonSerialized]
        public const string FieldCreateTime = "CreateOn";
    }
}