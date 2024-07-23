//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2024, DotNet.
//-----------------------------------------------------------------

using System;
using System.Collections.Specialized;
using System.Net;
using System.Text;

namespace DotNet.Business
{
    using Model;
    using Util;

    /// <summary>
    /// BaseUserManager
    /// 用户管理
    /// 
    /// 修改记录
    ///
    ///     2020.12.08 版本：1.5 Troy.Cui    使用CacheUtil缓存
    ///		2015.04.23 版本：1.0 JiRiGaLa	主键整理。
    /// 
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2015.04.23</date>
    /// </author> 
    /// </summary>
    public partial class BaseUserManager : BaseManager
    {
        /// <summary>
        /// 离职处理
        /// </summary>
        /// <param name="userEntity"></param>
        /// <param name="userLogonEntity"></param>
        /// <param name="comment"></param>
        /// <returns>影响行数</returns>
        public int Leave(BaseUserEntity userEntity, BaseUserLogonEntity userLogonEntity, string comment)
        {
            var result = 0;

            if (userEntity != null)
            {
                // 更新用户实体
                UpdateEntity(userEntity);
            }

            // 更新登录信息
            if (userLogonEntity != null)
            {
                var userLogonManager = new BaseUserLogonManager(UserInfo);
                userLogonManager.UpdateEntity(userLogonEntity);
            }

            // 2016-03-17 吉日嘎拉 停止吉信的号码
            if (userEntity != null && !string.IsNullOrEmpty(userEntity.NickName))
            {
                //AfterLeaveStopIm(userEntity);
            }

            // 2016-03-17 吉日嘎拉 停止吉信的号码
            if (userEntity != null && userEntity.Id > 0)
            {
                BaseUserContactEntity userContactEntity = null;
                // 2015-12-08 吉日嘎拉 提高效率、从缓存获取数据
                userContactEntity = BaseUserContactManager.GetEntityByCache(userEntity.Id);

                if (userContactEntity != null && !string.IsNullOrEmpty(userContactEntity.CompanyEmail))
                {
                    // 自动停用邮箱
                }
            }

            return result;
        }

    }
}