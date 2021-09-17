//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace DotNet.Business
{
    using Model;

    /// <summary>
    /// BaseUserManager
    /// 用户管理
    /// 
    /// 修改纪录
    /// 
    ///		2013.10.20 版本：2.0 JiRiGaLa	集成K8物流系统的登录功能。
    ///		2011.10.17 版本：1.0 JiRiGaLa	主键整理。
    /// 
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2011.10.17</date>
    /// </author> 
    /// </summary>
    public partial class BaseUserManager : BaseManager
    {
        /// <summary>
        /// 导入用户
        /// </summary>
        /// <param name="dataReader"></param>
        /// <param name="organizeManager"></param>
        /// <param name="userLogOnManager"></param>
        /// <param name="userContactManager"></param>
        /// <returns></returns>
        public int ImportUser(System.Data.IDataReader dataReader, BaseOrganizeManager organizeManager, BaseUserLogOnManager userLogOnManager, BaseUserContactManager userContactManager)
        {
            var result = 0;
            var userEntity = GetEntity(dataReader["ID"].ToString());
            if (userEntity == null)
            {
                userEntity = new BaseUserEntity
                {
                    Id = dataReader["ID"].ToString()
                };
            }
            userEntity.Id = dataReader["ID"].ToString();
            userEntity.UserFrom = "K8";
            userEntity.UserName = dataReader["USER_NAME"].ToString();
            userEntity.IdCard = dataReader["ID_Card"].ToString();
            userEntity.Code = dataReader["EMPLOYEE_CODE"].ToString();
            userEntity.RealName = dataReader["REAL_NAME"].ToString();
            if (string.IsNullOrWhiteSpace(userEntity.RealName))
            {
                userEntity.RealName = dataReader["EMPLOYEE_NAME"].ToString();
            }
            userEntity.NickName = dataReader["ONLY_USER_NAME"].ToString();
            userEntity.CompanyName = dataReader["OWNER_SITE"].ToString();
            userEntity.Description = dataReader["REMARK"].ToString();
            // 把被删除的数据恢复过来
            userEntity.DeletionStateCode = 0;
            if (string.IsNullOrEmpty(userEntity.CompanyId))
            {
                userEntity.CompanyId = organizeManager.GetProperty(new KeyValuePair<string, object>(BaseOrganizeEntity.FieldFullName, userEntity.CompanyName), BaseOrganizeEntity.FieldId);
                if (string.IsNullOrEmpty(userEntity.CompanyId))
                {
                    Console.WriteLine("无CompanyId " + userEntity.Id + ":" + userEntity.UserName + ":" + userEntity.RealName);
                    return 0;
                }
            }
            // 不是内部组织机构的才进行调整
            if (string.IsNullOrEmpty(userEntity.DepartmentId))
            {
                userEntity.DepartmentName = dataReader["DEPT_NAME"].ToString();
            }
            if (!string.IsNullOrEmpty(dataReader["IM_NAME"].ToString()))
            {
                // userEntity.QQ = dataReader["IM_NAME"].ToString();
            }

            userEntity.Enabled = int.Parse(dataReader["BL_LOCK_FLAG"].ToString());
            Console.WriteLine("ImportK8User:" + userEntity.Id + ":" + userEntity.RealName);
            // 02：可以把读取到的数据能写入到用户中心的。
            result = UpdateEntity(userEntity);
            if (result == 0)
            {
                AddEntity(userEntity);
            }
            // 添加用户密码表
            var userLogOnEntity = userLogOnManager.GetEntity(userEntity.Id);
            if (userLogOnEntity == null)
            {
                userLogOnEntity = new BaseUserLogOnEntity
                {
                    Id = userEntity.Id,
                    // 邦定mac地址
                    CheckIpAddress = 1,
                    UserPassword = dataReader["USER_PASSWD"].ToString(),
                    Salt = dataReader["SALT"].ToString()
                };
                // 是否检查机器码 MAC地址
                var checkIpAddress = 1;
                int.TryParse(dataReader["BL_CHECK_COMPUTER"].ToString(), out checkIpAddress);
                userLogOnEntity.CheckIpAddress = checkIpAddress;
                if (!string.IsNullOrEmpty(dataReader["CHANGEPASSWORDDATE"].ToString()))
                {
                    userLogOnEntity.ChangePasswordDate = DateTime.Parse(dataReader["CHANGEPASSWORDDATE"].ToString());
                }
                userLogOnManager.AddEntity(userLogOnEntity);
            }
            else
            {
                userLogOnEntity.Id = userEntity.Id;
                userLogOnEntity.UserPassword = dataReader["USER_PASSWD"].ToString();
                userLogOnEntity.Salt = dataReader["SALT"].ToString();
                if (!string.IsNullOrEmpty(dataReader["CHANGEPASSWORDDATE"].ToString()))
                {
                    userLogOnEntity.ChangePasswordDate = DateTime.Parse(dataReader["CHANGEPASSWORDDATE"].ToString());
                }
                result = userLogOnManager.UpdateEntity(userLogOnEntity);
            }
            // 用户的联系方式
            var userContactEntity = userContactManager.GetEntity(userEntity.Id);
            if (userContactEntity == null)
            {
                userContactEntity = new BaseUserContactEntity
                {
                    Id = userEntity.Id,
                    Qq = dataReader["QQ"].ToString(),
                    Mobile = dataReader["Mobile"].ToString(),
                    Email = dataReader["Email"].ToString()
                };
                userContactManager.AddEntity(userContactEntity);
            }
            else
            {
                if (!string.IsNullOrEmpty(dataReader["QQ"].ToString()))
                {
                    userContactEntity.Qq = dataReader["QQ"].ToString();
                }
                if (!string.IsNullOrEmpty(dataReader["Mobile"].ToString()))
                {
                    userContactEntity.Mobile = dataReader["Mobile"].ToString();
                }
                if (!string.IsNullOrEmpty(dataReader["Email"].ToString()))
                {
                    userContactEntity.Email = dataReader["Email"].ToString();
                }
                userContactManager.UpdateEntity(userContactEntity);
            }
            return result;
        }
    }
}