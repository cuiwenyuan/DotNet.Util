//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

namespace DotNet.Business
{
    using Model;
    using Util;

    /// <summary>
    /// BaseRoleManager
    /// 角色管理
    /// 
    /// 修改纪录
    /// 
    ///		2016.02.29 版本：1.0 JiRiGaLa	分离方法。
    /// 
    /// <author>
    ///		<name>JiRiGaLa</name>
    ///		<date>2016.02.29</date>
    /// </author> 
    /// </summary>
    public partial class BaseRoleManager : BaseManager
    {
        /// <summary>
        /// 缓存预热,强制重新缓存
        /// </summary>
        /// <returns></returns>
        public static int CachePreheating()
        {
            var result = 0;

            var systemCodes = BaseSystemManager.GetSystemCodes();
            for (var i = 0; i < systemCodes.Length; i++)
            {
                GetEntitiesByCache(systemCodes[i], true);
                result += CachePreheating(systemCodes[i]);
            }

            return result;
        }

        /// <summary>
        /// 缓存预热,强制重新缓存
        /// 2016-02-26 每个角色的权限也进行缓存起来
        /// </summary>
        /// <param name="systemCode">系统编号</param>
        /// <returns>影响行数</returns>
        public static int CachePreheating(string systemCode)
        {
            var result = 0;

            // 把所有的组织机构都缓存起来的代码
            var manager = new BaseRoleManager
            {
                CurrentTableName = systemCode + "Role"
            };
            using (var dataReader = manager.ExecuteReader())
            {
                while (dataReader.Read())
                {
                    var entity = BaseEntity.Create<BaseRoleEntity>(dataReader, false);
                    if (entity != null)
                    {
                        // 设置角色本身的缓存
                        SetCache(systemCode, entity);
                        // 重置权限缓存数据
                        BaseRolePermissionManager.ResetPermissionByCache(systemCode, entity.Id);
                        result++;
                        System.Console.WriteLine(result + " : " + entity.RealName);
                    }
                }
                dataReader.Close();
            }

            return result;
        }
        /// <summary>
        /// 刷新缓存
        /// </summary>
        /// <param name="systemCode"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public static int RefreshCache(string systemCode, string roleId)
        {
            var result = 0;

            // 2016-02-29 吉日嘎拉 强制刷新缓存
            var roleEntity = GetObjectByCache(systemCode, roleId, true);
            if (roleEntity != null)
            {
                var systemCodes = BaseSystemManager.GetSystemCodes();
                for (var i = 0; i < systemCodes.Length; i++)
                {
                    BaseRolePermissionManager.ResetPermissionByCache(systemCodes[i], roleId);
                }
            }

            return result;
        }
        /// <summary>
        /// 刷新缓存
        /// </summary>
        /// <param name="systemCode"></param>
        /// <returns></returns>
        public static int RefreshCache(string systemCode)
        {
            var result = 0;

            var list = GetEntitiesByCache(systemCode, true);
            foreach (var entity in list)
            {
                // 2016-02-29 吉日嘎拉 强制刷新缓存
                var roleEntity = GetObjectByCache(systemCode, entity.Id, true);
                if (roleEntity != null)
                {
                    BaseRolePermissionManager.ResetPermissionByCache(systemCode, entity.Id);
                }
            }

            return result;
        }
    }
}