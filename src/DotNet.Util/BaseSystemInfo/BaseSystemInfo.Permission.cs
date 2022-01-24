//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

namespace DotNet.Util
{
    /// <summary>
    /// BaseSystemInfo
    /// 这是系统的核心基础信息部分
    /// 
    /// 修改记录
    ///		2012.04.14 版本：1.0 JiRiGaLa	主键创建。
    ///		
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2012.04.14</date>
    /// </author>
    /// </summary>
    public partial class BaseSystemInfo
    {
        /// <summary>
        /// 采用角色组织关联关系
        /// </summary>
        public static bool UseRoleOrganization = false;

        /// <summary>
        /// 是否启用表格数据权限
        /// 启用分级管理范围权限设置，启用逐级授权
        /// </summary>
        public static bool UsePermissionScope = true;

        /// <summary>
        /// 启用授权范围（逐级授权）
        /// </summary>
        public static bool UseAuthorizationScope = false;

        /// <summary>
        /// 启用按用户权限
        /// </summary>
        public static bool UseUserPermission = true;

        /// <summary>
        /// 启用按组织机构权限
        /// </summary>
        public static bool UseOrganizationPermission = false;

        /// <summary>
        /// 启用数据表的约束条件限制
        /// </summary>
        public static bool UseTableScopePermission = false;

        /// <summary>
        /// 启用数据表的列权限
        /// </summary>
        public static bool UseTableColumnPermission = false;

        /// <summary>
        /// 设置手写签名
        /// </summary>
        public static bool HandwrittenSignature = false;

        /// <summary>
        /// 记录登录日志
        /// </summary>
        public static bool RecordLogonLog = true;

        /// <summary>
        /// 记录服务调用日志
        /// </summary>
        public static bool RecordLog = false;

        /// <summary>
        /// 是否更新访问日期信息
        /// </summary>
        public static bool UpdateVisit = true;

        /// <summary>
        /// 同时在线用户数量限制
        /// </summary>
        public static int OnlineLimit = 0;

        /// <summary>
        /// 是否检查用户IP地址
        /// </summary>
        public static bool CheckIpAddress = false;

        /// <summary>
        /// 是否登记异常
        /// </summary>
        public static bool LogException = true;

        /// <summary>
        /// 是否记录数据库操作日志
        /// </summary>
        public static bool LogSql = false;

        /// <summary>
        /// 是否登记到 Windows 系统异常中
        /// </summary>
        public static bool EventLog = false;
    }
}