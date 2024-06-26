﻿//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2024, DotNet.
//-----------------------------------------------------------------

using System;
using System.Data;

namespace DotNet.Business
{
    using Model;
    using Util;

    /// <summary>
    /// BaseStaffManager
    /// 员工管理
    /// 
    /// 修改记录
    /// 
    ///		2014.08.10 版本：1.0 JiRiGaLa	主键整理。
    /// 
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2014.08.10</date>
    /// </author> 
    /// </summary>
    public partial class BaseStaffManager : BaseManager
    {
        /// <summary>
        /// 同步数据
        /// </summary>
        /// <param name="all">同步所有数据</param>
        /// <returns>影响行数</returns>
        public int Synchronous(bool all = false)
        {
            var result = 0;
            var connectionString = ConfigurationUtil.AppSettings("K8Connection", BaseSystemInfo.EncryptDbConnection);
            string condition = null;
            if (!all)
            {
                /*
                int id = 0;
                string commandText = "SELECT MAX(id) FROM BaseStaff WHERE id < 1000000";
                Object maxObject = DbHelper.ExecuteScalar(commandText);
                if (maxObject != null)
                {
                    id = int.Parse(maxObject.ToString());
                }
                conditional = " AND Id > " + id.ToString();
                */
            }
            result = Synchronous(connectionString, condition);
            return result;
        }

        /// <summary>
        /// 导入K8系统用户账户
        /// </summary>
        /// <param name="connectionString">数据库连接</param>
        /// <param name="condition">条件，不需要同步所有的数据</param>
        /// <returns></returns>
        public int Synchronous(string connectionString = null, string condition = null)
        {
            var result = 0;
            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = ConfigurationUtil.AppSettings("K8Connection", BaseSystemInfo.EncryptDbConnection);
            }
            if (!string.IsNullOrEmpty(connectionString))
            {
                // 01：可以从k8里读取公司、用户、密码的。
                var dbHelper = DbHelperFactory.Create(CurrentDbType.Oracle, connectionString);
                var staffManager = new BaseStaffManager(UserInfo);
                if (string.IsNullOrEmpty(condition))
                {
                    // 不不存在的用户删除掉tab_user是远程试图
                    /*
                    commandText = "DELETE FROM " + BaseStaffEntity.CurrentTableName + " WHERE id < 10000 AND id NOT IN (SELECT id FROM TAB_EMPLOYEE WHERE id < 10000 )";
                    staffManager.ExecuteNonQuery(commandText);
                    System.Console.WriteLine(commandText);

                    commandText = "DELETE FROM " + BaseStaffEntity.CurrentTableName + " WHERE id < 20000 AND id >= 10000 AND id NOT IN (SELECT id FROM TAB_EMPLOYEE WHERE id <20000 AND id >= 10000)";
                    staffManager.ExecuteNonQuery(commandText);
                    System.Console.WriteLine(commandText);

                    commandText = "DELETE FROM " + BaseStaffEntity.CurrentTableName + " WHERE id < 30000 AND id >= 20000 AND id NOT IN (SELECT id FROM TAB_EMPLOYEE WHERE id < 30000 AND id >= 20000)";
                    staffManager.ExecuteNonQuery(commandText);
                    System.Console.WriteLine(commandText);

                    commandText = "DELETE FROM " + BaseStaffEntity.CurrentTableName + " WHERE id < 40000 AND id >= 30000 AND id NOT IN (SELECT id FROM TAB_EMPLOYEE WHERE id < 40000 AND id >= 30000)";
                    staffManager.ExecuteNonQuery(commandText);
                    System.Console.WriteLine(commandText);

                    commandText = "DELETE FROM " + BaseStaffEntity.CurrentTableName + " WHERE id < 50000 AND id >= 40000 AND id NOT IN (SELECT id FROM TAB_EMPLOYEE WHERE id < 50000 AND id >= 40000)";
                    staffManager.ExecuteNonQuery(commandText);
                    System.Console.WriteLine(commandText);

                    commandText = "DELETE FROM " + BaseStaffEntity.CurrentTableName + " WHERE id < 60000 AND id >= 50000 AND id NOT IN (SELECT id FROM TAB_EMPLOYEE WHERE id < 60000 AND id >= 50000)";
                    staffManager.ExecuteNonQuery(commandText);
                    System.Console.WriteLine(commandText);

                    commandText = "DELETE FROM " + BaseStaffEntity.CurrentTableName + " WHERE id < 70000 AND id >= 60000 AND id NOT IN (SELECT id FROM TAB_EMPLOYEE WHERE id < 70000 AND id >= 60000)";
                    staffManager.ExecuteNonQuery(commandText);
                    System.Console.WriteLine(commandText);

                    commandText = "DELETE FROM " + BaseStaffEntity.CurrentTableName + " WHERE id < 80000 AND id >= 70000 AND id NOT IN (SELECT id FROM TAB_EMPLOYEE WHERE id < 80000 AND id >= 70000)";
                    staffManager.ExecuteNonQuery(commandText);
                    System.Console.WriteLine(commandText);

                    commandText = "DELETE FROM " + BaseStaffEntity.CurrentTableName + " WHERE id < 90000 AND id >= 80000 AND id NOT IN (SELECT id FROM TAB_EMPLOYEE WHERE id < 90000 AND id >= 80000)";
                    staffManager.ExecuteNonQuery(commandText);
                    System.Console.WriteLine(commandText);

                    commandText = "DELETE FROM " + BaseStaffEntity.CurrentTableName + " WHERE id < 100000 AND id >= 90000 AND id NOT IN (SELECT id FROM TAB_EMPLOYEE WHERE id < 100000 AND id >= 90000)";
                    staffManager.ExecuteNonQuery(commandText);
                    System.Console.WriteLine(commandText);

                    commandText = "DELETE FROM " + BaseStaffEntity.CurrentTableName + " WHERE id < 110000 AND id >= 100000 AND id NOT IN (SELECT id FROM TAB_EMPLOYEE WHERE id < 110000 AND id >= 100000)";
                    staffManager.ExecuteNonQuery(commandText);
                    System.Console.WriteLine(commandText);

                    commandText = "DELETE FROM " + BaseStaffEntity.CurrentTableName + " WHERE id < 120000 AND id >= 110000 AND id NOT IN (SELECT id FROM TAB_EMPLOYEE WHERE id < 120000 AND id >= 110000)";
                    staffManager.ExecuteNonQuery(commandText);
                    System.Console.WriteLine(commandText);

                    commandText = "DELETE FROM " + BaseStaffEntity.CurrentTableName + " WHERE id < 130000 AND id >= 120000 AND id NOT IN (SELECT id FROM TAB_EMPLOYEE WHERE id < 130000 AND id >= 120000)";
                    staffManager.ExecuteNonQuery(commandText);
                    System.Console.WriteLine(commandText);

                    commandText = "DELETE FROM " + BaseStaffEntity.CurrentTableName + " WHERE id < 200000 AND id >= 130000 AND id NOT IN (SELECT id FROM TAB_EMPLOYEE WHERE id < 200000 AND id >= 130000)";
                    staffManager.ExecuteNonQuery(commandText);
                    System.Console.WriteLine(commandText);

                    commandText = "DELETE FROM " + BaseStaffEntity.CurrentTableName + " WHERE id < 300000 AND id >= 200000 AND id NOT IN (SELECT id FROM TAB_EMPLOYEE WHERE id < 300000 AND id >= 200000)";
                    staffManager.ExecuteNonQuery(commandText);
                    System.Console.WriteLine(commandText);

                    commandText = "DELETE FROM " + BaseStaffEntity.CurrentTableName + " WHERE id < 400000 AND id >= 300000 AND id NOT IN (SELECT id FROM TAB_EMPLOYEE WHERE id < 400000 AND id >= 300000)";
                    staffManager.ExecuteNonQuery(commandText);
                    System.Console.WriteLine(commandText);

                    commandText = "DELETE FROM " + BaseStaffEntity.CurrentTableName + " WHERE id < 600000 AND id >= 400000 AND id NOT IN (SELECT id FROM TAB_EMPLOYEE WHERE id < 600000 AND id >= 400000)";
                    staffManager.ExecuteNonQuery(commandText);
                    System.Console.WriteLine(commandText);

                    commandText = "DELETE FROM " + BaseStaffEntity.CurrentTableName + " WHERE id < 800000 AND id >= 600000 AND id NOT IN (SELECT id FROM TAB_EMPLOYEE WHERE id < 800000 AND id >= 600000)";
                    staffManager.ExecuteNonQuery(commandText);
                    System.Console.WriteLine(commandText);

                    commandText = "DELETE FROM " + BaseStaffEntity.CurrentTableName + " WHERE id < 1000000 AND id >= 800000 AND id NOT IN (SELECT id FROM TAB_EMPLOYEE WHERE id < 1000000 AND id >= 800000)";
                    staffManager.ExecuteNonQuery(commandText);
                    System.Console.WriteLine(commandText);
                    */
                }
                // 01：可以从k8里读取公司、用户、密码的。
                var commandText = "SELECT * FROM TAB_EMPLOYEE WHERE 1 = 1 ";
                if (!string.IsNullOrEmpty(condition))
                {
                    commandText += condition + " ORDER BY UPDATETIME DESC";
                }
                else
                {
                    // 只更新今天有变化的数据就可以了
                    // commandText += " AND TO_CHAR(SYSDATE, 'yy-mm-dd') = TO_CHAR(UPDATETIME, 'yy-mm-dd') ";
                }
                Console.WriteLine(commandText);

                //var sTaffExpressManager = new BaseUserExpressManager(UserInfo);
                //var userManager = new BaseUserManager(UserInfo);
                //var userLogonManager = new BaseUserLogonManager(UserInfo);
                //var dataReader = DbHelper.ExecuteReader(commandText);
                //if (dataReader != null && !dataReader.IsClosed)
                //{
                //    while (dataReader.Read())
                //    {
                //        result += ImportStaff(dataReader, staffManager, sTaffExpressManager, userManager,
                //            userLogonManager);
                //    }
                //    dataReader.Close();
                //}

                // 设置用户的公司主键，有时候不需要同步所有的账户，只同步增量账户
                // 设置用户的公司主键
                // commandText = @"UPDATE basestaff SET companyid = (SELECT MAX(Id) FROM baseorganization WHERE baseorganization.name = basestaff.companyname AND baseorganization.Id < 1000000) WHERE companyId IS NULL OR companyId = ''";
                // 公司名称重复的数据需要找出来
                ExecuteNonQuery(commandText);
                Console.WriteLine(commandText);
            }
            return result;
        }

        BaseOrganizationManager _organizationManager = new BaseOrganizationManager();

        /// <summary>
        /// 导入员工
        /// </summary>
        /// <param name="dataReader"></param>
        /// <param name="staffManager"></param>
        /// <param name="userManager"></param>
        /// <param name="userLogonManager"></param>
        /// <returns></returns>
        //public int ImportStaff(IDataReader dataReader, BaseStaffManager staffManager, BaseUserExpressManager sTaffExpressManager, BaseUserManager userManager, BaseUserLogonManager userLogonManager)
        public int ImportStaff(IDataReader dataReader, BaseStaffManager staffManager, BaseUserManager userManager, BaseUserLogonManager userLogonManager)
        {
            var result = 0;

            /*
            BaseStaffEntity staffEntity = staffManager.GetEntity(dataReader["Id"].ToString());
            if (staffEntity == null)
            {
                staffEntity = new BaseStaffEntity();
            }
            staffEntity.Id = int.Parse(dataReader["Id"].ToString());
            staffEntity.Code = dataReader["EMPLOYEE_CODE"].ToString();
            staffEntity.RealName = dataReader["EMPLOYEE_NAME"].ToString();
            staffEntity.Telephone = dataReader["PHONE"].ToString();
            staffEntity.HomeAddress = dataReader["ADDRESS"].ToString();
            staffEntity.IDCard = dataReader["ID_CARD"].ToString();

            // staffEntity.CompanyId = dataReader["OWNER_SITE"].ToString();
            staffEntity.CompanyName = dataReader["OWNER_SITE"].ToString();
            // staffEntity.DepartmentId = dataReader["DEPT_NAME"].ToString();
            staffEntity.DepartmentName = dataReader["DEPT_NAME"].ToString();
            // staffEntity.WorkgroupId = dataReader["GROUP_NAME"].ToString();
            staffEntity.WorkgroupName = dataReader["GROUP_NAME"].ToString();

            System.Console.WriteLine("ImportK8Staff:" + staffEntity.Id + ":" + staffEntity.RealName);
            // 02：可以把读取到的数据能写入到用户中心的。
            result = staffManager.UpdateEntity(staffEntity);
            if (result == 0)
            {
                staffManager.AddEntity(staffEntity);
            }
            */

            /*
            BaseUserExpressEntity sTAFF_EXPRESSEntity = sTAFF_EXPRESSManager.GetEntity(dataReader["Id"].ToString());
            if (sTAFF_EXPRESSEntity == null)
            {
                sTAFF_EXPRESSEntity = new BaseUserExpressEntity();
            }
            sTAFF_EXPRESSEntity.Id = int.Parse(dataReader["Id"].ToString());
            sTAFF_EXPRESSEntity.OWNER_RANGE = dataReader["OWNER_RANGE"].ToString();
            Decimal tRANSFER_ADD_FEE = 0;
            if (!string.IsNullOrEmpty(dataReader["TRANSFER_ADD_FEE"].ToString()) && ValidateUtil.IsDouble(dataReader["TRANSFER_ADD_FEE"].ToString()))
            {
                tRANSFER_ADD_FEE = Decimal.Parse(dataReader["TRANSFER_ADD_FEE"].ToString());
            }
            sTAFF_EXPRESSEntity.TRANSFER_ADD_FEE = tRANSFER_ADD_FEE;
            Decimal dISPATCH_ADD_FEE = 0;
            if (!string.IsNullOrEmpty(dataReader["DISPATCH__ADD_FEE"].ToString()) && ValidateUtil.IsDouble(dataReader["DISPATCH__ADD_FEE"].ToString()))
            {
                dISPATCH_ADD_FEE = Decimal.Parse(dataReader["DISPATCH__ADD_FEE"].ToString());
            }
            sTAFF_EXPRESSEntity.DISPATCH_ADD_FEE = dISPATCH_ADD_FEE;

            System.Console.WriteLine("ImportK8Staffexpress:" + staffEntity.Id + ":" + staffEntity.RealName);
            // 02：可以把读取到的数据能写入到用户中心的。
            result = sTAFF_EXPRESSManager.Update(sTAFF_EXPRESSEntity);
            if (result == 0)
            {
                sTAFF_EXPRESSManager.Add(sTAFF_EXPRESSEntity);
            }
            */

            var userEntity = new BaseUserEntity
            {
                Id = dataReader["ID"].ToString().ToInt(),
                UserFrom = "PDA",
                Code = dataReader["EMPLOYEE_CODE"].ToString(),
                UserName = dataReader["EMPLOYEE_NAME"].ToString(),
                RealName = dataReader["REAL_NAME"].ToString(),
                Description = dataReader["PHONE"].ToString(),
                CompanyName = dataReader["OWNER_SITE"].ToString()
            };
            if (userEntity.CompanyId > 0)
            {
                if (BaseOrganizationManager.GetEntityByNameByCache(userEntity.CompanyName) == null)
                {
                    Console.WriteLine("无CompanyId " + userEntity.Id + ":" + userEntity.UserName + ":" + userEntity.RealName);
                    return 0;
                }
            }
            userEntity.DepartmentName = dataReader["DEPT_NAME"].ToString();
            userEntity.WorkgroupName = dataReader["GROUP_NAME"].ToString();
            userEntity.HomeAddress = dataReader["ADDRESS"].ToString();
            userEntity.IdCard = dataReader["ID_CARD"].ToString();
            if (!string.IsNullOrEmpty(dataReader["cardnum"].ToString()))
            {
                userEntity.IdCard = dataReader["cardnum"].ToString();
            }
            userEntity.Signature = dataReader["EMPLOYEE_TYPE"].ToString();
            userEntity.SortCode = int.Parse(dataReader["ID"].ToString());

            if (userEntity.UpdateTime < DateTime.Parse(dataReader["UPDATETIME"].ToString()))
            {
                userEntity.UpdateTime = DateTime.Parse(dataReader["UPDATETIME"].ToString());
            }
            // 修改日期需要同步
            // result = userManager.UpdateEntity(userEntity);
            if (result == 0)
            {
                userManager.AddEntity(userEntity);

                var userContactEntity = new BaseUserContactEntity
                {
                    UserId = dataReader["ID"].ToString().ToInt(),
                    Telephone = dataReader["PHONE"].ToString()
                };
                new BaseUserContactManager().AddEntity(userContactEntity);

                var userLogonEntity = new BaseUserLogonEntity
                {
                    UserId = dataReader["ID"].ToString().ToInt(),
                    UserPassword = dataReader["BAR_PASSWORD"].ToString()
                };
                userLogonManager.AddEntity(userLogonEntity);
            }

            // 处理角色
            /*
            string roleName = dataReader["EMPLOYEE_TYPE"].ToString();
            // 看是否在这个角色里，若没有增加上去。
            userManager.AddToRole("PDA", userEntity.Id, roleName);

            // 添加用户密码表
            BaseUserLogonEntity userLogonEntity = userLogonManager.GetEntity(userEntity.Id);
            if (userLogonEntity == null)
            {
                userLogonEntity = new BaseUserLogonEntity();
                userLogonEntity.Id = userEntity.Id;
                userLogonEntity.UserPassword = dataReader["BAR_PASSWORD"].ToString();
                //userLogonEntity.Salt = dataReader["SALT"].ToString();
                //if (!string.IsNullOrEmpty(dataReader["CHANGEPASSWORDDATE"].ToString()))
                //{
                //    userLogonEntity.ChangePasswordTime = DateTime.Parse(dataReader["CHANGEPASSWORDDATE"].ToString());
                //}
                userLogonManager.AddEntity(userLogonEntity);
            }
            else
            {
                userLogonEntity.Id = userEntity.Id;
                userLogonEntity.UserPassword = dataReader["BAR_PASSWORD"].ToString();
                //userLogonEntity.Salt = dataReader["SALT"].ToString();
                //if (!string.IsNullOrEmpty(dataReader["CHANGEPASSWORDDATE"].ToString()))
                //{
                //    userLogonEntity.ChangePasswordTime = DateTime.Parse(dataReader["CHANGEPASSWORDDATE"].ToString());
                //}
                result = userLogonManager.UpdateEntity(userLogonEntity);
            }
             */

            return result;
        }
    }
}