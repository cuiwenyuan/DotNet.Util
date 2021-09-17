//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

namespace DotNet.Business
{
    /// <summary>
    /// BaseUserManager
    /// 用户管理
    /// 
    /// 修改纪录
    /// 
    ///		2013.11.11 版本：1.0 JiRiGaLa	主键整理。
    /// 
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2013.11.11</date>
    /// </author> 
    /// </summary>
    public partial class BaseUserManager : BaseManager
    {
        /// <summary>
        /// 同步
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static int Synchronous(string userId)
        {
            var result = 0;

            result = RefreshCache(userId);

            return result;
        }
        /*
        /// <summary>
        /// 同步数据
        /// </summary>
        /// <param name="all">同步所有数据</param>
        /// <returns>影响行数</returns>
        public int Synchronous(bool all = false)
        {
            int result = 0;
            string connectionString = ConfigurationHelper.AppSettings("K8Connection", BaseSystemInfo.EncryptDbConnection);
            string condition = null;
            string commandText = string.Empty;
            if (!all)
            {
                int id = 0;
                commandText = "SELECT MAX(id) FROM " + BaseUserEntity.TableName + " WHERE id < 1000000";
                Object maxObject = DbHelper.ExecuteScalar(commandText);
                if (maxObject != null)
                {
                    id = int.Parse(maxObject.ToString());
                }
                conditional = " AND ID > " + id.ToString();
            }
            else
            {
                commandText = "DELETE FROM baseusercontact WHERE id NOT IN (SELECT id FROM baseuser)";
                DbHelper.ExecuteNonQuery(commandText);
            }
            result = Synchronous(connectionString, conditional);
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
            int result = 0;
            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = ConfigurationHelper.AppSettings("K8Connection", BaseSystemInfo.EncryptDbConnection);
            }
            if (!string.IsNullOrEmpty(connectionString))
            {
                // 01：可以从k8里读取公司、用户、密码的。
                IDbHelper dbHelper = DbHelperFactory.GetHelper(CurrentDbType.Oracle, connectionString);
                string commandText = string.Empty;
                BaseUserLogOnManager userLogOnManager = new Business.BaseUserLogOnManager(this.UserInfo);
                if (string.IsNullOrEmpty(conditional))
                {
                    // 不不存在的用户删除掉tab_user是远程试图
                    commandText = "DELETE FROM " + BaseUserEntity.TableName + " WHERE id < 10000 AND id NOT IN (SELECT id FROM tab_user WHERE id < 10000 AND bl_type != 1)";
                    userLogOnManager.DbHelper.ExecuteNonQuery(commandText);
                    System.Console.WriteLine(commandText);

                    commandText = "DELETE FROM " + BaseUserEntity.TableName + " WHERE id < 20000 AND id >= 10000 AND id NOT IN (SELECT id FROM tab_user WHERE id <20000 AND bl_type != 1 AND id >= 10000)";
                    userLogOnManager.DbHelper.ExecuteNonQuery(commandText);
                    System.Console.WriteLine(commandText);

                    commandText = "DELETE FROM " + BaseUserEntity.TableName + " WHERE id < 30000 AND id >= 20000 AND id NOT IN (SELECT id FROM tab_user WHERE id < 30000 AND bl_type != 1 AND id >= 20000)";
                    userLogOnManager.DbHelper.ExecuteNonQuery(commandText);
                    System.Console.WriteLine(commandText);

                    commandText = "DELETE FROM " + BaseUserEntity.TableName + " WHERE id < 40000 AND id >= 30000 AND id NOT IN (SELECT id FROM tab_user WHERE id < 40000 AND bl_type != 1 AND id >= 30000)";
                    userLogOnManager.DbHelper.ExecuteNonQuery(commandText);
                    System.Console.WriteLine(commandText);

                    commandText = "DELETE FROM " + BaseUserEntity.TableName + " WHERE id < 50000 AND id >= 40000 AND id NOT IN (SELECT id FROM tab_user WHERE id < 50000 AND bl_type != 1 AND id >= 40000)";
                    userLogOnManager.DbHelper.ExecuteNonQuery(commandText);
                    System.Console.WriteLine(commandText);

                    commandText = "DELETE FROM " + BaseUserEntity.TableName + " WHERE id < 60000 AND id >= 50000 AND id NOT IN (SELECT id FROM tab_user WHERE id < 60000 AND bl_type != 1 AND id >= 50000)";
                    userLogOnManager.DbHelper.ExecuteNonQuery(commandText);
                    System.Console.WriteLine(commandText);

                    commandText = "DELETE FROM " + BaseUserEntity.TableName + " WHERE id < 70000 AND id >= 60000 AND id NOT IN (SELECT id FROM tab_user WHERE id < 70000 AND bl_type != 1 AND id >= 60000)";
                    userLogOnManager.DbHelper.ExecuteNonQuery(commandText);
                    System.Console.WriteLine(commandText);

                    commandText = "DELETE FROM " + BaseUserEntity.TableName + " WHERE id < 80000 AND id >= 70000 AND id NOT IN (SELECT id FROM tab_user WHERE id < 80000 AND bl_type != 1 AND id >= 70000)";
                    userLogOnManager.DbHelper.ExecuteNonQuery(commandText);
                    System.Console.WriteLine(commandText);

                    commandText = "DELETE FROM " + BaseUserEntity.TableName + " WHERE id < 90000 AND id >= 80000 AND id NOT IN (SELECT id FROM tab_user WHERE id < 90000 AND bl_type != 1 AND id >= 80000)";
                    userLogOnManager.DbHelper.ExecuteNonQuery(commandText);
                    System.Console.WriteLine(commandText);

                    commandText = "DELETE FROM " + BaseUserEntity.TableName + " WHERE id < 100000 AND id >= 90000 AND id NOT IN (SELECT id FROM tab_user WHERE id < 100000 AND bl_type != 1 AND id >= 90000)";
                    userLogOnManager.DbHelper.ExecuteNonQuery(commandText);
                    System.Console.WriteLine(commandText);

                    commandText = "DELETE FROM " + BaseUserEntity.TableName + " WHERE id < 110000 AND id >= 100000 AND id NOT IN (SELECT id FROM tab_user WHERE id < 110000 AND bl_type != 1 AND id >= 100000)";
                    userLogOnManager.DbHelper.ExecuteNonQuery(commandText);
                    System.Console.WriteLine(commandText);

                    commandText = "DELETE FROM " + BaseUserEntity.TableName + " WHERE id < 120000 AND id >= 110000 AND id NOT IN (SELECT id FROM tab_user WHERE id < 120000 AND bl_type != 1 AND id >= 110000)";
                    userLogOnManager.DbHelper.ExecuteNonQuery(commandText);
                    System.Console.WriteLine(commandText);

                    commandText = "DELETE FROM " + BaseUserEntity.TableName + " WHERE id < 130000 AND id >= 120000 AND id NOT IN (SELECT id FROM tab_user WHERE id < 130000 AND bl_type != 1 AND id >= 120000)";
                    userLogOnManager.DbHelper.ExecuteNonQuery(commandText);
                    System.Console.WriteLine(commandText);

                    commandText = "DELETE FROM " + BaseUserEntity.TableName + " WHERE id < 1000000 AND id >= 130000 AND id NOT IN (SELECT id FROM tab_user WHERE id < 1000000 AND bl_type != 1 AND id >= 130000)";
                    userLogOnManager.DbHelper.ExecuteNonQuery(commandText);
                    System.Console.WriteLine(commandText);
                }

                string commandText = "SELECT Id FROM baseuser WHERE id < 1000000";
                using (IDataReader dataReader = userLogOnManager.DbHelper.ExecuteReader(commandText))
                {
                    while (dataReader.Read())
                    {
                        string id = dataReader["id"].ToString();
                        commandText = "SELECT COUNT(*) AS Rcount FROM TAB_USER WHERE id='" + id + "'");
                        object rcount = dbHelper.ExecuteScalar(commandText);
                        if (rcount == null || rcount.ToString().Equals("0"))
                        {
                            commandText = "DELETE FROM baseuser WHERE id ='" + id + "'");
                            userLogOnManager.DbHelper.ExecuteNonQuery(commandText);
                        }
                    }
                }
                
                commandText = "SELECT * FROM TAB_USER WHERE bl_type != 1 "; // BL_LOCK_FLAG = 1
                if (!string.IsNullOrEmpty(conditional))
                {
                    commandText += conditional + " ORDER BY MODIFIER_DATE DESC";
                }
                else
                {
                    // 只更新今天有变化的数据就可以了
                    // commandText += " AND TO_CHAR(SYSDATE,'yy-mm-dd') = TO_CHAR(MODIFIER_DATE,'yy-mm-dd') ";
                }
                System.Console.WriteLine(commandText);
                BaseOrganizeManager organizeManager = new BaseOrganizeManager(this.UserInfo);
                BaseUserContactManager userContactManager = new BaseUserContactManager(this.UserInfo);

                int deleteFlag = 0;
                using (IDataReader dataReader = dbHelper.ExecuteReader(commandText))
                {
                    while (dataReader.Read())
                    {
                        result += ImportUser(dataReader, organizeManager, userLogOnManager, userContactManager);
                        deleteFlag++;
                    }
                    dataReader.Close();
                }
                if (deleteFlag == 0 && !string.IsNullOrWhiteSpace(conditional) && conditional.IndexOf("ONLY_USER_NAME") < 0)
                {
                    //删除BASEUSER
                    commandText = "DELETE FROM " + BaseUserEntity.TableName + " WHERE Id < 1000000 " + conditional;
                    userLogOnManager.DbHelper.ExecuteNonQuery(commandText);
                    System.Console.WriteLine(commandText);
                    
                    //删除BASEUSERCONTACT
                    commandText = "DELETE FROM BASEUSERCONTACT WHERE Id < 1000000 " + conditional;
                    userLogOnManager.DbHelper.ExecuteNonQuery(commandText);
                    
                    System.Console.WriteLine(commandText);
                }

                if (conditional == null)
                {
                    // 设置用户的公司主键，有时候不需要同步所有的账户，只同步增量账户
                    // 设置用户的公司主键
                    commandText = @"UPDATE " + BaseUserEntity.TableName + " SET companyid = (SELECT MAX(Id) FROM baseorganize WHERE baseorganize.fullname = " + BaseUserEntity.TableName + ".companyname AND baseorganize.ID < 1000000) WHERE companyId IS NULL OR companyId = ''";
                    // 公司名称重复的数据需要找出来
                    this.DbHelper.ExecuteNonQuery(commandText);
                    System.Console.WriteLine(commandText);
                }
            }
            return result;
        }


        #region public virtual void AfterAddSynchronous(BaseUserEntity entity) 用户添加后执行的方法
        /// <summary>
        /// 用户添加之后执行的方法
        /// </summary>
        /// <param name="entity">用户实体</param>
        public virtual void AfterAddSynchronous(BaseUserEntity entity)
        {

        }
        #endregion

        #region public virtual void AfterUpdateSynchronous(BaseUserEntity entity) 用户添加后执行的方法
        /// <summary>
        /// 用户更新之后执行的方法
        /// </summary>
        /// <param name="entity">用户实体</param>
        public virtual void AfterUpdateSynchronous(BaseUserEntity entity)
        {
            //获取是否检查MAC地址
            int checkIPAddress = 1;
            BaseUserLogOnManager userLogOnManager = new BaseUserLogOnManager(this.UserInfo);
            BaseUserLogOnEntity userLogOnEntity = new BaseUserLogOnEntity();
            userLogOnEntity = userLogOnManager.GetEntity(entity.Id);
            if (userLogOnEntity != null)
            {
                int.TryParse(userLogOnEntity.CheckIPAddress.ToString(), out checkIPAddress);
            }

            string connectionString = ConfigurationHelper.AppSettings("K8Connection", BaseSystemInfo.EncryptDbConnection);
            if (!string.IsNullOrEmpty(connectionString))
            {
                IDbHelper dbHelper = DbHelperFactory.GetHelper(CurrentDbType.Oracle, connectionString);
                string commandText = string.Format(@"UPDATE TAB_USER 
                                                        SET USER_NAME = {0} 
                                                            , ONLY_USER_NAME = {1}
                                                            , EMPLOYEE_NAME = {2}
                                                            , EMPLOYEE_CODE = {3}                                                            
                                                            , ID_Card = {4}                                                            
                                                            , OWNER_SITE = {5}
                                                            , DEPT_NAME = {6}
                                                            , BL_LOCK_FLAG = {7}
                                                            , REMARK = {8}
                                                            , BL_CHECK_COMPUTER = {9}
                                                      WHERE Id = {10}"
                    , dbHelper.GetParameter("UserName")
                    , dbHelper.GetParameter("Nickname")
                    , dbHelper.GetParameter("RealName")
                    , dbHelper.GetParameter("Code")
                    , dbHelper.GetParameter("IDCard")
                    , dbHelper.GetParameter("CompanyName")
                    , dbHelper.GetParameter("DepartmentName")
                    , dbHelper.GetParameter("Enabled")
                    , dbHelper.GetParameter("DESCRIPTION")
                    , dbHelper.GetParameter("CheckIPAddress")
                    , dbHelper.GetParameter("Id"));
                dbHelper.ExecuteNonQuery(commandText, new IDbDataParameter[] { 
                    dbHelper.MakeParameter("UserName", entity.UserName)
                    , dbHelper.MakeParameter("Nickname", entity.NickName)
                    , dbHelper.MakeParameter("RealName", entity.RealName)
                    , dbHelper.MakeParameter("Code", entity.Code)
                    , dbHelper.MakeParameter("IDCard", entity.IDCard)
                    , dbHelper.MakeParameter("CompanyName", entity.CompanyName)
                    , dbHelper.MakeParameter("DepartmentName", entity.DepartmentName)
                    , dbHelper.MakeParameter("Enabled", entity.Enabled)
                    , dbHelper.MakeParameter("Description", entity.Description)
                    , dbHelper.MakeParameter("CheckIPAddress", checkIPAddress)
                    , dbHelper.MakeParameter("Id", entity.Id)
                });
            }
        }
        #endregion

        /// <summary>
        /// 设置密码(扩展方法)
        /// </summary>
        /// <param name="userId">被设置的用户主键</param>
        /// <param name="password">新密码</param>
        /// <returns>影响行数</returns>
        public virtual int AfterSetPassword(string userId, string salt, string password)
        {
            int result = 0;
            string connectionString = ConfigurationHelper.AppSettings("K8Connection", BaseSystemInfo.EncryptDbConnection);
            if (!string.IsNullOrEmpty(connectionString))
            {
                IDbHelper dbHelper = DbHelperFactory.GetHelper(CurrentDbType.Oracle, connectionString);
                string commandText = string.Format("UPDATE TAB_USER SET USER_PASSWORD = NULL, USER_PASSWD = {0}, SALT = {1}, CHANGEPASSWORDDATE = {2}, OPENID = {3} WHERE ID = {4}"
                    , dbHelper.GetParameter("password")
                    , dbHelper.GetParameter("salt")
                    , dbHelper.GetParameter("changePasswordDate")
                    , dbHelper.GetParameter("openId")
                    , dbHelper.GetParameter("userId"));
                result = dbHelper.ExecuteNonQuery(commandText, new IDbDataParameter[] 
                { 
                    dbHelper.MakeParameter("password", this.EncryptUserPassword(password, salt))
                  , dbHelper.MakeParameter("salt", salt)
                  , dbHelper.MakeParameter("changePasswordDate", DateTime.Now)
                  , dbHelper.MakeParameter("openId", Guid.NewGuid().ToString("N"))
                  , dbHelper.MakeParameter("userId", userId) 
                });
                // System.Console.WriteLine(commandText);
            }
            return result;
        }

        /// <summary>
        /// 批量设置密码(扩展方法)
        /// </summary>
        /// <param name="userIds">主键数组</param>
        /// <param name="password">密码</param>
        /// <returns>影响行数</returns>
        public virtual int AfterBatchSetPassword(string[] userIds, string password)
        {
            int result = 0;
            string connectionString = ConfigurationHelper.AppSettings("K8Connection", BaseSystemInfo.EncryptDbConnection);
            if (!string.IsNullOrEmpty(connectionString))
            {
                IDbHelper dbHelper = DbHelperFactory.GetHelper(CurrentDbType.Oracle, connectionString);
                BaseManager manager = new BaseManager(dbHelper, UserInfo, "TAB_USER");
                result = manager.SetProperty(new KeyValuePair<string, object>("Id", userIds), new KeyValuePair<string, object>("USER_PASSWORD", password));
            }
            return result;
        }

        public virtual int AfterChangePassword(string userId, string salt, string oldPassword, string newPassword)
        {
            int result = 0;
            string connectionString = ConfigurationHelper.AppSettings("K8Connection", BaseSystemInfo.EncryptDbConnection);
            if (!string.IsNullOrEmpty(connectionString))
            {
                IDbHelper dbHelper = DbHelperFactory.GetHelper(CurrentDbType.Oracle, connectionString);
                string commandText = string.Format("UPDATE TAB_USER SET USER_PASSWORD = NULL, USER_PASSWD = {0}, SALT = {1}, CHANGEPASSWORDDATE = {2} WHERE ID = {3}"
                    , dbHelper.GetParameter("password")
                    , dbHelper.GetParameter("salt")
                    , dbHelper.GetParameter("changePasswordDate")
                    , dbHelper.GetParameter("userId"));
                result = dbHelper.ExecuteNonQuery(commandText, new IDbDataParameter[] 
                { 
                    dbHelper.MakeParameter("password", this.EncryptUserPassword(newPassword, salt))
                  , dbHelper.MakeParameter("salt", salt)
                  , dbHelper.MakeParameter("changePasswordDate", DateTime.Now)
                  , dbHelper.MakeParameter("userId", userId) 
                });
            }
            return result;
        }
        */
    }
}