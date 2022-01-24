//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

namespace DotNet.Business
{
    /// <summary>
    /// BaseOrganizationManager
    /// 组织机构管理
    /// 
    /// 修改纪录
    /// 
    ///		2016.02.29 版本：1.0 JiRiGaLa	主键整理。
    /// 
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2016.02.29</date>
    /// </author> 
    /// </summary>
    public partial class BaseOrganizationManager : BaseManager
    {
        /// <summary>
        /// 刷新缓存
        /// </summary>
        /// <param name="organizationId"></param>
        /// <returns></returns>
        public static int RefreshCache(string organizationId)
        {
            var result = 0;

            // 2016-02-29 吉日嘎拉 强制刷新缓存
            var organizeEntity = GetEntityByCache(organizationId, true);
            if (organizeEntity != null)
            {
                var systemCodes = BaseSystemManager.GetSystemCodes();
                for (var i = 0; i < systemCodes.Length; i++)
                {
                    BaseOrganizationPermissionManager.ResetPermissionByCache(systemCodes[i], organizationId);
                }
            }

            return result;
        }
    }
}