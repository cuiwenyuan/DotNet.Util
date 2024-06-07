//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2024, DotNet.
//-----------------------------------------------------------------
using System.Collections.Generic;
using System;

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
    public partial class BaseUserLogonManager : BaseManager
    {
        #region ForceOffline强制下线

        /// <summary>
        /// 强制下线
        /// </summary>
        /// <param name="userIds">用户编号</param>
        /// <returns>更新成功记录数</returns>
        public int ForceOffline(string[] userIds)
        {
            var result = 0;
            if (userIds != null)
            {
                result = Update(BaseUtil.FieldUserId, userIds, new KeyValuePair<string, object>(BaseUserLogonEntity.FieldUserOnline, 0));
                //操作日志
                var entity = new BaseLogEntity
                {
                    Parameters = userIds.ToString(),
                    Description = "强制下线：" + ((result >= 1) ? "成功" : "失败")
                };
                if (UserInfo != null)
                {
                    entity.UserId = UserInfo.Id.ToInt();
                    entity.RealName = UserInfo.RealName;
                }
                new BaseLogManager(UserInfo).Add(entity);
            }

            return result;
        }
        #endregion

        #region SetConcurrentUser设置并发用户

        /// <summary>
        /// 设置并发用户
        /// </summary>
        /// <param name="userIds">用户编号</param>
        /// <returns>更新成功记录数</returns>
        public int SetConcurrentUser(string[] userIds)
        {
            var result = 0;
            if (userIds != null)
            {
                result = Update(BaseUtil.FieldUserId, userIds, new KeyValuePair<string, object>(BaseUserLogonEntity.FieldConcurrentUser, 1));
                //操作日志
                var entity = new BaseLogEntity
                {
                    Parameters = userIds.ToString(),
                    Description = "设置并发用户：" + ((result >= 1) ? "成功" : "失败")
                };
                if (UserInfo != null)
                {
                    entity.UserId = UserInfo.Id.ToInt();
                    entity.RealName = UserInfo.RealName;
                }
                new BaseLogManager(UserInfo).Add(entity);
            }

            return result;
        }
        #endregion

        #region SetConcurrentUser撤销设置并发用户

        /// <summary>
        /// 撤销设置并发用户
        /// </summary>
        /// <param name="userIds">用户编号</param>
        /// <returns>更新成功记录数</returns>
        public int UndoSetConcurrentUser(string[] userIds)
        {
            var result = 0;
            if (userIds != null)
            {
                result = Update(BaseUtil.FieldUserId, userIds, new KeyValuePair<string, object>(BaseUserLogonEntity.FieldConcurrentUser, 0));
                //操作日志
                var entity = new BaseLogEntity
                {
                    Parameters = userIds.ToString(),
                    Description = "撤销设置并发用户：" + ((result >= 1) ? "成功" : "失败")
                };
                if (UserInfo != null)
                {
                    entity.UserId = UserInfo.Id.ToInt();
                    entity.RealName = UserInfo.RealName;
                }
                new BaseLogManager(UserInfo).Add(entity);
            }

            return result;
        }
        #endregion

        #region Lock锁定用户

        /// <summary>
        /// 锁定用户
        /// </summary>
        /// <param name="userIds">用户编号</param>
        /// <param name="minutes">锁定分钟数</param>
        /// <returns>更新成功记录数</returns>
        public int Lock(string[] userIds, int minutes = 365)
        {
            var result = 0;
            if (minutes > 9999)
            {
                minutes = 9999;
            }
            if (userIds != null)
            {
                var parameters = new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>(BaseUserLogonEntity.FieldLockStartTime, DateTime.Now),
                    new KeyValuePair<string, object>(BaseUserLogonEntity.FieldLockEndTime, DateTime.Now.AddMinutes(minutes))
                };

                result = Update(BaseUtil.FieldUserId, userIds, parameters);
                //操作日志
                var entity = new BaseLogEntity
                {
                    Parameters = userIds.ToString(),
                    Description = "锁定用户：" + ((result >= 1) ? "成功" : "失败")
                };
                if (UserInfo != null)
                {
                    entity.UserId = UserInfo.Id.ToInt();
                    entity.RealName = UserInfo.RealName;
                }
                new BaseLogManager(UserInfo).Add(entity);
            }

            return result;
        }
        #endregion

        #region Unlock解除锁定用户

        /// <summary>
        /// 解除锁定用户
        /// </summary>
        /// <param name="userIds">用户编号</param>
        /// <returns>更新成功记录数</returns>
        public int Unlock(string[] userIds)
        {
            var result = 0;
            if (userIds != null)
            {
                var parameters = new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>(BaseUserLogonEntity.FieldLockStartTime, null),
                    new KeyValuePair<string, object>(BaseUserLogonEntity.FieldLockEndTime, null)
                };

                result = Update(BaseUtil.FieldUserId, userIds, parameters);
                //操作日志
                var entity = new BaseLogEntity
                {
                    Parameters = userIds.ToString(),
                    Description = "解除锁定用户：" + ((result >= 1) ? "成功" : "失败")
                };
                if (UserInfo != null)
                {
                    entity.UserId = UserInfo.Id.ToInt();
                    entity.RealName = UserInfo.RealName;
                }
                new BaseLogManager(UserInfo).Add(entity);
            }

            return result;
        }
        #endregion
    }
}