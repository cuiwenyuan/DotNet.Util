//-----------------------------------------------------------------------
// <copyright file="BaseExceptionEntity.Auto.cs" company="DotNet">
//     Copyright (c) 2021, All rights reserved.
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
    /// 修改记录
    /// 
    /// 2021-09-27 版本：1.0 Troy.Cui 创建文件。
    /// 
    /// <author>
    ///     <name>Troy.Cui</name>
    ///     <date>2016-12-21</date>
    /// </author>
    /// </summary>
    public partial class BaseExceptionEntity : BaseEntity
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
        /// 系统异常表
        ///</summary>
        [FieldDescription("系统异常表")]
        public const string CurrentTableName = "BaseException";

        ///<summary>
        /// 主键
        ///</summary>
        public const string FieldId = "Id";

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