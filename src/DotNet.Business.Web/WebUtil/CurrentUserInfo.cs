//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System;

namespace DotNet.Business
{
    using Util;
    using Model;

    /// <remarks>
    /// CurrentUserInfo
    /// 当前用户信息
    /// 
    /// 修改记录
    /// 
    ///	版本：1.0 2014.03.12    JiRiGaLa    创建。
    ///	
    /// <author>  
    ///		<name>JiRiGaLa</name>
    ///		<date>2014.03.12</date>
    /// </author> 
    /// </remarks>
    public class CurrentUserInfo : BaseUserInfo
    {
        /// <summary>
        /// 扩展的信息，从哪里读取？
        /// 数据库？
        /// 远程URL读取？
        /// </summary>
        public bool GetExtensionsFromDb = false;

        /// <summary>
        /// 当前用户信息
        /// </summary>
        public CurrentUserInfo()
        {
        }
        /// <summary>
        /// 当前用户信息
        /// </summary>
        /// <param name="userInfo"></param>
        public CurrentUserInfo(BaseUserInfo userInfo)
        {
            // Troy.Cui 2016-06-25
            if (userInfo != null)
            {
                BaseUtil.CopyObjectProperties(userInfo, this);
            }
        }

        /// <summary>
        /// 当前的组织结构公司名称
        /// </summary>
        public BaseOrganizeEntity Company
        {
            get
            {
                BaseOrganizeEntity company = null;
                // 读取组织机构的信息
                if (!string.IsNullOrEmpty(CompanyId))
                {
                    //company = new BaseOrganizeManager().GetObjectByCache(CompanyId);
                }
                return company;
            }
        }
    }
}