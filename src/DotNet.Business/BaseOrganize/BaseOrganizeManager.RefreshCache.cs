//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

namespace DotNet.Business
{
    /// <summary>
    /// BaseOrganizeManager
    /// 组织机构管理
    /// 
    /// 修改纪录
    /// 
    ///		2016.02.29 版本：1.0 JiRiGaLa	主键整理。
    /// 
    /// <author>
    ///		<name>JiRiGaLa</name>
    ///		<date>2016.02.29</date>
    /// </author> 
    /// </summary>
    public partial class BaseOrganizeManager : BaseManager
    {
        /// <summary>
        /// 刷新缓存
        /// </summary>
        /// <param name="organizeId"></param>
        /// <returns></returns>
        public static int RefreshCache(string organizeId)
        {
            var result = 0;

            // 2016-02-29 吉日嘎拉 强制刷新缓存
            var organizeEntity = GetObjectByCache(organizeId, true);
            if (organizeEntity != null)
            {
                var systemCodes = BaseSystemManager.GetSystemCodes();
                for (var i = 0; i < systemCodes.Length; i++)
                {
                    BaseOrganizePermissionManager.ResetPermissionByCache(systemCodes[i], organizeId);
                }
            }

            return result;
        }
    }
}