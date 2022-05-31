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
                    Parameters = userIds.ToString(),
                    Description = "设置超级管理员：" + ((result >= 1) ? "成功" : "失败")
                };
                if (UserInfo != null)
                {
                    entity.UserId = int.Parse(UserInfo.Id);
                    entity.RealName = UserInfo.RealName;
                }
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
                    Parameters = userIds.ToString(),
                    Description = "撤销设置超级管理员：" + ((result >= 1) ? "成功" : "失败")
                };
                if (UserInfo != null)
                {
                    entity.UserId = int.Parse(UserInfo.Id);
                    entity.RealName = UserInfo.RealName;
                }
                new BaseLogManager(UserInfo).Add(entity);
            }

            return result;
        }
        #endregion

        #region GetViewEntity
        /// <summary>
        /// 获取用户实体
        /// </summary>
        /// <param name="id">编号</param>
        /// <returns></returns>
        public ViewBaseUserEntity GetViewEntity(string id)
        {
            ViewBaseUserEntity entity = null;
            if (ValidateUtil.IsInt(id))
            {
                var entityUser = GetEntity(id);
                if (entityUser != null)
                {
                    entity = new ViewBaseUserEntity
                    {
                        Id = entityUser.Id,
                        UserFrom = entityUser.UserFrom,
                        UserName = entityUser.UserName,
                        RealName = entityUser.RealName,
                        NickName = entityUser.NickName,
                        AvatarUrl = entityUser.AvatarUrl,
                        Code = entityUser.Code,
                        EmployeeNumber = entityUser.EmployeeNumber,
                        IdCard = entityUser.IdCard,
                        QuickQuery = entityUser.QuickQuery,
                        SimpleSpelling = entityUser.SimpleSpelling,
                        CompanyId = entityUser.CompanyId,
                        CompanyCode = entityUser.CompanyCode,
                        CompanyName = entityUser.CompanyName,
                        SubCompanyId = entityUser.SubCompanyId,
                        SubCompanyName = entityUser.SubCompanyName,
                        DepartmentId = entityUser.DepartmentId,
                        DepartmentName = entityUser.DepartmentName,
                        SubDepartmentId = entityUser.SubDepartmentId,
                        SubDepartmentName = entityUser.SubDepartmentName,
                        WorkgroupId = entityUser.WorkgroupId,
                        WorkgroupName = entityUser.WorkgroupName,
                        WorkCategory = entityUser.WorkCategory,
                        SecurityLevel = entityUser.SecurityLevel,
                        Title = entityUser.Title,
                        Duty = entityUser.Duty,
                        Lang = entityUser.Lang,
                        Gender = entityUser.Gender,
                        Birthday = entityUser.Birthday,
                        Score = entityUser.Score,
                        Fans = entityUser.Fans,
                        HomeAddress = entityUser.HomeAddress,
                        Signature = entityUser.Signature,
                        Theme = entityUser.Theme,
                        IsStaff = entityUser.IsStaff,
                        IsVisible = entityUser.IsVisible,
                        Country = entityUser.Country,
                        State = entityUser.State,
                        Province = entityUser.Province,
                        City = entityUser.City,
                        District = entityUser.District,
                        AuditStatus = entityUser.AuditStatus,
                        ManagerUserId = entityUser.ManagerUserId,
                        IsAdministrator = entityUser.IsAdministrator,
                        IsCheckBalance = entityUser.IsCheckBalance,
                        Description = entityUser.Description,
                        SortCode = entityUser.SortCode,
                        Deleted = entityUser.Deleted,
                        Enabled = entityUser.Enabled,
                        CreateTime = entityUser.CreateTime,
                        CreateUserId = entityUser.CreateUserId,
                        CreateUserName = entityUser.CreateUserName,
                        CreateBy = entityUser.CreateBy,
                        CreateIp = entityUser.CreateIp,
                        UpdateTime = entityUser.UpdateTime,
                        UpdateUserId = entityUser.UpdateUserId,
                        UpdateUserName = entityUser.UpdateUserName,
                        UpdateBy = entityUser.UpdateBy,
                        UpdateIp = entityUser.UpdateIp
                    };

                    var entityUserLogon = new BaseUserLogonManager(UserInfo).GetEntityByUserId(id);
                    if (entityUserLogon != null)
                    {
                        //entity.UserPassword = entityUserLogon.UserPassword;
                        entity.OpenId = entityUserLogon.OpenId;
                        entity.AllowStartTime = entityUserLogon.AllowStartTime;
                        entity.AllowEndTime = entityUserLogon.AllowEndTime;
                        entity.LockStartTime = entityUserLogon.LockStartTime;
                        entity.LockEndTime = entityUserLogon.LockEndTime;
                        entity.FirstVisitTime = entityUserLogon.FirstVisitTime;
                        entity.PreviousVisitTime = entityUserLogon.PreviousVisitTime;
                        entity.LastVisitTime = entityUserLogon.LastVisitTime;
                        entity.ChangePasswordTime = entityUserLogon.ChangePasswordTime;
                        entity.LogonCount = entityUserLogon.LogonCount;
                        entity.ConcurrentUser = entityUserLogon.ConcurrentUser;
                        entity.ShowCount = entityUserLogon.ShowCount;
                        entity.PasswordErrorCount = entityUserLogon.PasswordErrorCount;
                        entity.UserOnline = entityUserLogon.UserOnline;
                        entity.CheckIpAddress = entityUserLogon.CheckIpAddress;
                        entity.VerificationCode = entityUserLogon.VerificationCode;
                        entity.IpAddress = entityUserLogon.IpAddress;
                        entity.MacAddress = entityUserLogon.MacAddress;
                        entity.Question = entityUserLogon.Question;
                        entity.AnswerQuestion = entityUserLogon.AnswerQuestion;
                        //entity.Salt = entityUserLogon.Salt;
                        entity.OpenIdTimeoutTime = entityUserLogon.OpenIdTimeoutTime;
                        entity.SystemCode = entityUserLogon.SystemCode;
                        entity.IpAddressName = entityUserLogon.IpAddressName;
                        entity.PasswordStrength = entityUserLogon.PasswordStrength;
                        entity.ComputerName = entityUserLogon.ComputerName;
                        entity.NeedModifyPassword = entityUserLogon.NeedModifyPassword;
                    }

                    var entityUserContact = new BaseUserContactManager(UserInfo).GetEntityByUserId(id);
                    if (entityUserContact != null)
                    {
                        entity.Mobile = entityUserContact.Mobile;
                        entity.ShortNumber = entityUserContact.ShortNumber;
                        entity.Ww = entityUserContact.Ww;
                        entity.WeChat = entityUserContact.WeChat;
                        entity.Telephone = entityUserContact.Telephone;
                        entity.Extension = entityUserContact.Extension;
                        entity.Qq = entityUserContact.Qq;
                        entity.Email = entityUserContact.Email;
                        entity.CompanyEmail = entityUserContact.CompanyEmail;
                        entity.EmergencyContact = entityUserContact.EmergencyContact;
                    }
                }
            }

            return entity;
        }
        #endregion
    }
}