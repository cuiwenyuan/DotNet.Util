//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2022, DotNet.
//-----------------------------------------------------------------
using System.Data;
using System.Collections.Generic;

namespace DotNet.Business
{
    using Model;
    using Util;

    /// <summary>
    /// BaseUserLogonManager
    /// 用户登录管理
    /// 
    /// 修改记录
    /// 
    ///		2022.02.08 版本：1.0 Troy.Cui	新增。
    /// 
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2022.02.08</date>
    /// </author> 
    /// </summary>
    public partial class BaseUserManager : BaseManager
    {
        #region SetAdministrator设置超级管理员

        /// <summary>
        /// 设置超级管理员
        /// </summary>
        /// <param name="userIds">用户编号</param>
        /// <returns>更新成功记录数</returns>
        public int SetAdministrator(string[] userIds)
        {
            var result = 0;
            if (userIds != null)
            {
                result = UpdateProperty(BaseUserEntity.FieldId, userIds, new KeyValuePair<string, object>(BaseUserEntity.FieldIsAdministrator, 1));
                //操作日志
                var entity = new BaseLogEntity
                {
                    UserId = int.Parse(UserInfo.Id),
                    RealName = UserInfo.RealName,
                    Parameters = userIds.ToString(),
                    Description = "设置超级管理员：" + ((result == 1) ? "成功" : "失败")
                };
                new BaseLogManager(UserInfo).Add(entity);
            }

            return result;
        }
        #endregion

        #region SetAdministrator撤销设置超级管理员

        /// <summary>
        /// 撤销设置超级管理员
        /// </summary>
        /// <param name="userIds">用户编号</param>
        /// <returns>更新成功记录数</returns>
        public int UndoSetAdministrator(string[] userIds)
        {
            var result = 0;
            if (userIds != null)
            {
                result = UpdateProperty(BaseUtil.FieldId, userIds, new KeyValuePair<string, object>(BaseUserEntity.FieldIsAdministrator, 0));
                //操作日志
                var entity = new BaseLogEntity
                {
                    UserId = int.Parse(UserInfo.Id),
                    RealName = UserInfo.RealName,
                    Parameters = userIds.ToString(),
                    Description = "撤销设置超级管理员：" + ((result == 1) ? "成功" : "失败")
            };
            new BaseLogManager(UserInfo).Add(entity);
        }

            return result;
        }
    #endregion
}
}