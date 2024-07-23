//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2024, DotNet.
//-----------------------------------------------------------------

using DotNet.Util;

namespace DotNet.Business
{
    /// <summary>
    ///	BaseManager
    /// 通用基类部分
    /// 
    /// 修改记录
    /// 
    ///		2024.07.17 版本：Troy.Cui进行扩展。
    ///
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2024.07.17</date>
    /// </author> 
    /// </summary>
    public partial class BaseManager : IBaseManager
    {
        #region 获取Module表名

        /// <summary>
        /// 获取Module表名
        /// </summary>
        /// <returns>表名</returns>
        public static string GetModuleTableName(string systemCode = null)
        {
            var result = string.Empty;
            if (systemCode.IsNullOrEmpty())
            {
                systemCode = "Base";
            }
            result = systemCode + "Module";

            return result;
        }

        #endregion

        #region 获取Permission表名

        /// <summary>
        /// 获取Permission表名
        /// </summary>
        /// <returns>表名</returns>
        public static string GetPermissionTableName(string systemCode = null)
        {
            var result = string.Empty;
            if (systemCode.IsNullOrEmpty())
            {
                systemCode = "Base";
            }
            result = systemCode + "Permission";

            return result;
        }

        #endregion

        #region 获取PermissionScope表名

        /// <summary>
        /// 获取PermissionScope表名
        /// </summary>
        /// <returns>表名</returns>
        public static string GetPermissionScopeTableName(string systemCode = null)
        {
            var result = string.Empty;
            if (systemCode.IsNullOrEmpty())
            {
                systemCode = "Base";
            }
            result = systemCode + "PermissionScope";

            return result;
        }

        #endregion

        #region 获取Role表名

        /// <summary>
        /// 获取Role表名
        /// </summary>
        /// <returns>表名</returns>
        public static string GetRoleTableName(string systemCode = null)
        {
            var result = string.Empty;
            if (systemCode.IsNullOrEmpty())
            {
                systemCode = "Base";
            }
            result = systemCode + "Role";

            return result;
        }

        #endregion

        #region 获取RoleOrganization表名

        /// <summary>
        /// 获取RoleOrganization表名
        /// </summary>
        /// <returns>表名</returns>
        public static string GetRoleOrganizationTableName(string systemCode = null)
        {
            var result = string.Empty;
            if (systemCode.IsNullOrEmpty())
            {
                systemCode = "Base";
            }
            result = systemCode + "RoleOrganization";

            return result;
        }

        #endregion

        #region 获取UserRole表名

        /// <summary>
        /// 获取UserRole表名
        /// </summary>
        /// <returns>表名</returns>
        public static string GetUserRoleTableName(string systemCode = null)
        {
            var result = string.Empty;
            if (systemCode.IsNullOrEmpty())
            {
                systemCode = "Base";
            }
            result = systemCode + "UserRole";

            return result;
        }

        #endregion
    }
}