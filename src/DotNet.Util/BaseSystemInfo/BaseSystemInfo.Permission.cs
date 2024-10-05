//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2024, DotNet.
//-----------------------------------------------------------------

namespace DotNet.Util
{
    /// <summary>
    /// BaseSystemInfo
    /// 这是系统的核心基础信息部分
    /// 
    /// 修改记录
    ///		2024.09.17 版本：1.0 Troy	创建。
    ///		
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2024.09.17</date>
    /// </author>
    /// </summary>
    public partial class BaseSystemInfo
    {
        /// <summary>
        /// 使用Base表存储权限相关数据
        /// </summary>
        public static bool UseBaseTable = false;

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
    }
}