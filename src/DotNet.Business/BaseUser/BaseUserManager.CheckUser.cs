//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System;

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
    ///		2015.12.24 版本：1.0 JiRiGaLa	主键整理。
    /// 
    /// <author>
    ///		<name>JiRiGaLa</name>
    ///		<date>2015.12.24</date>
    /// </author> 
    /// </summary>
    public partial class BaseUserManager : BaseManager
    {
        /// <summary>
        /// 检查用户的登录许可信息
        /// </summary>
        /// <param name="userId">用户主键</param>
        /// <returns>用户登录信息</returns>
        public UserLogOnResult CheckUser(string userId)
        {
            // 这个从缓存获取，效率高，一般不会有经常在修改的事情，缓存的时间很短才可以，否则读取脏数据了
            var userEntity = GetObject(userId);

            // 获取登录状态表
            var userLogOnManager = new BaseUserLogOnManager(UserInfo, UserLogOnTable);
            var userLogOnEntity = userLogOnManager.GetObject(userId);

            return CheckUser(userEntity, userLogOnEntity);
        }

        /// <summary>
        /// 检查用户的登录许可信息
        /// </summary>
        /// <param name="userEntity">用户实体</param>
        /// <param name="userLogOnEntity">用户登录实体</param>
        /// <returns>用户登录信息</returns>
        public static UserLogOnResult CheckUser(BaseUserEntity userEntity, BaseUserLogOnEntity userLogOnEntity)
        {
            var result = new UserLogOnResult();

            //int errorMark = 0;
            // 05. 是否允许登录，是否离职是否正确
            if (!string.IsNullOrEmpty(userEntity.AuditStatus)
                && userEntity.AuditStatus.EndsWith(AuditStatus.WaitForAudit.ToString()))
            {
                result.StatusCode = AuditStatus.WaitForAudit.ToString();
                result.StatusMessage = AuditStatus.WaitForAudit.ToDescription();
                //errorMark = 1;
                return result;
            }

            // 用户是否有效的
            if (userEntity.Enabled.HasValue && userEntity.Enabled == 0)
            {
                result.StatusCode = Status.LogOnDeny.ToString();
                result.StatusMessage = Status.LogOnDeny.ToDescription();
                //errorMark = 2;
                return result;
            }

            // 用户是否有效的
            if (userEntity.Enabled.HasValue && userEntity.Enabled == -1)
            {
                result.StatusCode = Status.UserNotActive.ToString();
                result.StatusMessage = Status.UserNotActive.ToDescription();
                //errorMark = 3;
                return result;
            }

            // 01: 系统是否采用了在线用户的限制, 这里是登录到哪个表里去？
            //errorMark = 6;
            // 2015-12-08 吉日嘎拉  
            if (userLogOnEntity == null)
            {
                result.StatusCode = Status.MissingData.ToString();
                result.StatusMessage = Status.MissingData.ToDescription();
                return result;
            }
            // 06. 允许登录时间是否有限制

            // 2015-05-28 jirigala 子系统的用户是否有效的
            //errorMark = 7;
            if (userLogOnEntity.Enabled == 0)
            {
                //errorMark = 8;
                result.StatusCode = Status.LogOnDeny.ToString();
                result.StatusMessage = Status.LogOnDeny.ToDescription();
                return result;
            }

            //errorMark = 11;
            if (userLogOnEntity.AllowEndTime != null)
            {
                //errorMark = 12;
                //userLogOnEntity.AllowEndTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, userLogOnEntity.AllowEndTime.Value.Hour, userLogOnEntity.AllowEndTime.Value.Minute, userLogOnEntity.AllowEndTime.Value.Second);
            }

            //errorMark = 13;
            if (userLogOnEntity.AllowStartTime != null)
            {
                //errorMark = 14;
                //userLogOnEntity.AllowStartTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, userLogOnEntity.AllowStartTime.Value.Hour, userLogOnEntity.AllowStartTime.Value.Minute, userLogOnEntity.AllowStartTime.Value.Second);
                //errorMark = 15;
                if (DateTime.Now < userLogOnEntity.AllowStartTime)
                {
                    result.StatusCode = Status.ServiceNotStart.ToString();
                    result.StatusMessage = Status.ServiceNotStart.ToDescription();
                    //errorMark = 17;
                    return result;
                }
            }

            //errorMark = 18;
            if (userLogOnEntity.AllowEndTime != null)
            {
                //errorMark = 19;
                if (DateTime.Now > userLogOnEntity.AllowEndTime)
                {
                    result.StatusCode = Status.ServiceExpired.ToString();
                    result.StatusMessage = Status.ServiceExpired.ToDescription();
                    //errorMark = 20;
                    return result;
                }
            }

            // 07. 锁定日期是否有限制
            //errorMark = 21;
            if (userLogOnEntity.LockStartDate != null)
            {
                //errorMark = 22;
                if (DateTime.Now > userLogOnEntity.LockStartDate)
                {
                    //errorMark = 23;
                    if (userLogOnEntity.LockEndDate == null || DateTime.Now < userLogOnEntity.LockEndDate)
                    {
                        result.StatusCode = Status.UserLocked.ToString();
                        result.StatusMessage = Status.UserLocked.ToDescription();
                        //errorMark = 24;
                        return result;
                    }
                }
            }

            //errorMark = 25;
            if (userLogOnEntity.LockEndDate != null)
            {
                //errorMark = 26;
                if (DateTime.Now < userLogOnEntity.LockEndDate)
                {
                    //errorMark = 27;
                    result.StatusCode = Status.UserLocked.ToString();
                    result.StatusMessage = Status.UserLocked.ToDescription();
                    //errorMark = 28;
                    return result;
                }
            }

            result.StatusCode = Status.Ok.ToString();
            result.StatusMessage = Status.Ok.ToDescription();

            return result;
        }
    }
}