//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2024, DotNet.
//-----------------------------------------------------------------

namespace DotNet.Business
{
    using Util;

    /// <summary>
    /// BaseUserManager
    /// 用户管理
    /// 
    /// 修改记录
    ///
    ///     2020.12.08 版本：1.5 Troy.Cui    使用CacheUtil缓存
    ///		2016.02.29 版本：1.0 JiRiGaLa	主键整理。
    /// 
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2016.02.29</date>
    /// </author> 
    /// </summary>
    public partial class BaseUserManager : BaseManager
    {
        /// <summary>
        /// 刷新缓存
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static int RefreshCache(string userId)
        {
            var result = 0;

            // 刷新用户的缓存
            var userEntity = GetEntityByCache(userId, true);
            if (userEntity != null)
            {
                // 刷新用户的登录限制 用户名的限制

                // 刷新用户的登录限制
                ResetIpAddressByCache(userId);
                ResetMacAddressByCache(userId);
                // 刷新组织机构缓存
                BaseOrganizationManager.GetEntityByCache(userEntity.CompanyId.ToString(), true);
                // 2016-02-18 吉日嘎拉 刷新拒绝权限(把用户的权限放在一起方便直接移除、刷新)
                var key = "User:IsAuthorized:" + userId;
                CacheUtil.Remove(key);

                // 2016-05-24 吉日嘎拉 解除登录限制的方法，防止一天都登录不上的问题发生
                //if (!string.IsNullOrEmpty(userEntity.NickName))
                //{
                //    key = "u:" + userEntity.NickName;
                //    PooledRedisHelper.CallLimitRemove(key);
                //}
                //if (!string.IsNullOrEmpty(userEntity.Code))
                //{
                //    key = "u:" + userEntity.Code;
                //    PooledRedisHelper.CallLimitRemove(key);
                //}

                // 每个子系统都可以循环一次
                var systemCodes = BaseSystemManager.GetSystemCodes();
                foreach (var entity in systemCodes)
                {
                    BasePermissionManager.ResetPermissionByCache(entity.ItemKey, userId, null);
                }
            }

            return result;
        }
    }
}