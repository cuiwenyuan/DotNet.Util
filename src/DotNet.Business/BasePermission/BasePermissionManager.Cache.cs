//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2024, DotNet.
//-----------------------------------------------------------------

using System;
using System.Linq;

namespace DotNet.Business
{
    using Model;
    using Util;

    /// <summary>
    /// BasePermissionManager
    /// 资源权限管理，操作权限管理（这里实现了用户操作权限，角色的操作权限）
    /// 
    /// 修改记录
    ///
    ///     2015.07.10 版本：2.1 JiRiGaLa 把删除补上来。
    ///     2010.09.21 版本：2.0 JiRiGaLa 智能权限判断、后台自动增加权限，增加并发锁PermissionLock。
    ///     2009.09.22 版本：1.1 JiRiGaLa 前台判断的权限，后台都需要记录起来，防止后台缺失前台的判断权限。
    ///     2008.03.28 版本：1.0 JiRiGaLa 创建主键。
    ///     
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2015.07.10</date>
    /// </author>
    /// </summary>
    public partial class BasePermissionManager : BaseManager
    {
        /// <summary>
        /// 是否验证
        /// </summary>
        /// <param name="systemCode"></param>
        /// <param name="userId"></param>
        /// <param name="permissionCode">权限编码</param>
        /// <returns></returns>
        public static bool IsAuthorizedByCache(string systemCode, string userId, string permissionCode)
        {
            var result = false;
            var hashId = "User:IsAuthorized:" + userId;
            var key = systemCode + ":" + permissionCode;
            result = CacheUtil.Cache(key, () => new BasePermissionManager().IsAuthorized(systemCode, userId, permissionCode), true);
            return result;
        }


        /// <summary>
        /// 获取用户的权限主键数组
        /// </summary>
        /// <param name="systemCode">系统编号</param>
        /// <param name="userId">用户主键</param>
        /// <param name="companyId">公司主键</param>
        /// <param name="containPublic"></param>
        /// <returns>拥有权限数组</returns>
        public static string[] GetPermissionIdsByUserByCache(string systemCode, string userId, string companyId = null, bool containPublic = true)
        {
            // 公开的操作权限需要计算
            string[] result = null;

            var errorMark = 0;
            var moduleTableName = GetModuleTableName(systemCode);

            try
            {
                errorMark = 1;

                // 01: 把公开的部分获取出来（把公开的主键数组从缓存里获取出来，减少数据库的读取次数）
                if (containPublic)
                {
                    var moduleEntities = new BaseModuleManager().GetEntitiesByCache(systemCode);
                    if (moduleEntities != null)
                    {
                        result = moduleEntities.Where((t => t.IsPublic == 1 && t.Enabled == 1 && t.Deleted == 0)).Select(t => t.Id.ToString()).ToArray();
                    }
                }

                // 02: 获取用户本身拥有的权限 
                var userPermissionIds = BasePermissionManager.GetPermissionIdsByCache(systemCode, userId);
                result = StringUtil.Concat(result, userPermissionIds);

                // 03: 用户角色的操作权限

                // 用户都在哪些角色里？通过缓存读取？没有角色的，没必要进行运算了
                var roleIds = BaseUserManager.GetRoleIdsByCache(systemCode, userId, companyId);
                if (roleIds != null && roleIds.Length > 0)
                {
                    var userRolePermissionIds = BasePermissionManager.GetPermissionIdsByCache(systemCode, roleIds);
                    result = StringUtil.Concat(result, userRolePermissionIds);
                }
            }
            catch (Exception ex)
            {
                var exception = "BasePermissionManager.GetPermissionIdsByUser:发生时间:" + DateTime.Now
                    + Environment.NewLine + "errorMark = " + errorMark
                    + Environment.NewLine + "Message:" + ex.Message
                    + Environment.NewLine + "Source:" + ex.Source
                    + Environment.NewLine + "StackTrace:" + ex.StackTrace
                    + Environment.NewLine + "TargetSite:" + ex.TargetSite
                    + Environment.NewLine;

                LogUtil.WriteLog(exception, "Exception");
            }

            return result;
        }
        /// <summary>
        /// 检查角色权限
        /// </summary>
        /// <param name="systemCode"></param>
        /// <param name="roleId"></param>
        /// <param name="permissionCode">权限编码</param>
        /// <returns></returns>
        public static bool CheckPermissionByRoleByCache(string systemCode, string roleId, string permissionCode)
        {
            var permissionId = string.Empty;
            permissionId = new BaseModuleManager().GetIdByCodeByCache(systemCode, permissionCode);
            // 没有找到相应的权限
            if (string.IsNullOrEmpty(permissionId))
            {
                return false;
            }

            var permissionIds = BasePermissionManager.GetPermissionIdsByCache(systemCode, new string[] { roleId });
            return Array.IndexOf(permissionIds, permissionId) >= 0;
        }
    }
}