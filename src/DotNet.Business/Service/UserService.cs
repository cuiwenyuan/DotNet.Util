//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;


namespace DotNet.Business
{
    using IService;
    using Model;
    using Util;

    /// <summary>
    /// UserService
    /// 用户管理服务
    /// 
    /// 修改记录
    /// 
    ///     2014.02.20 版本：3.1 JiRiGaLa 用户审核功能改进。
    ///     2013.02.13 版本：3.0 JiRiGaLa GetDataTableByPage 大数据分页程序改进。
    ///     2009.09.11 版本：2.4 JiRiGaLa SetUserAuditStates 函数进行改进，严格按审核状态来，还需要按工作流改进。
    ///		2008.03.17 版本：2.3 JiRiGaLa 增加，已经在线，进行重新登录及扮演情况下在线状态事件处理。
    ///		2007.08.18 版本：2.2 JiRiGaLa 将文件名修改为 UserService。
    ///		2007.08.15 版本：2.1 JiRiGaLa 改进运行速度采用 WebService 变量定义 方式处理数据。
    ///		2007.08.14 版本：2.0 JiRiGaLa 改进运行速度采用 Instance 方式处理数据。
    ///     2007.06.12 版本：1.1 JiRiGaLa 加入调试信息#if (DEBUG)。
    ///		2007.04.16 版本：1.0 JiRiGaLa 窗体与数据库连接的分离。
    ///		
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2009.09.11</date>
    /// </author> 
    /// </summary>


    public partial class UserService : IUserService
    {
        #region public bool Exists(BaseUserInfo userInfo, List<KeyValuePair<string, object>> parameters, string id)
        /// <summary>
        /// 判断字段是否重复
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="parameters">字段名,字段值</param>
        /// <param name="id">主键</param>
        /// <returns>已存在</returns>
        public bool Exists(BaseUserInfo userInfo, List<KeyValuePair<string, object>> parameters, string id)
        {
            var result = false;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                var manager = new BaseManager(dbHelper, userInfo, BaseUserEntity.TableName);
                result = manager.Exists(parameters, id);
            });
            return result;
        }
        #endregion

        #region public string CreateUser(IDbHelper dbHelper, BaseUserInfo userInfo, BaseUserEntity entity, BaseUserContactEntity userContactEntity, out string statusCode, out string statusMessage)

        /// <summary>
        /// 添加用户
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        /// <param name="userInfo">用户信息</param>
        /// <param name="entity">用户实体</param>
        /// <param name="userLogonEntity">用户登录实体</param>
        /// <param name="userContactEntity">用户联系方式</param>
        /// <param name="statusCode">状态码</param>
        /// <param name="statusMessage">状态信息</param>
        /// <returns>主键</returns>
        public string CreateUser(IDbHelper dbHelper, BaseUserInfo userInfo, BaseUserEntity entity, BaseUserLogonEntity userLogonEntity, BaseUserContactEntity userContactEntity, out string statusCode, out string statusMessage)
        {
            var result = string.Empty;

            // 加强安全验证防止未授权匿名调用
#if (!DEBUG)
            BaseSystemInfo.IsAuthorized(userInfo);
#endif

            var userManager = new BaseUserManager(dbHelper, userInfo);
            result = userManager.Add(entity, userLogonEntity);
            statusCode = userManager.StatusCode;
            statusMessage = userManager.GetStateMessage();

            // 20140219 JiRiGaLa 添加成功的用户才增加联系方式
            if (!string.IsNullOrEmpty(result) && statusCode.Equals(Status.OkAdd.ToString()) && userContactEntity != null)
            {
                // 添加联系方式
                userContactEntity.Id = result;
                var userContactManager = new BaseUserContactManager(dbHelper, userInfo);
                userContactEntity.CompanyId = entity.CompanyId;
                userContactManager.Add(userContactEntity);
            }

            // 自己不用给自己发提示信息，这个提示信息是为了提高工作效率的，还是需要审核通过的，否则垃圾信息太多了
            if (entity.Enabled == 0 && statusCode.Equals(Status.OkAdd.ToString()))
            {
                // 不是系统管理员添加
                if (!BaseUserManager.IsAdministrator(userInfo.Id))
                {
                    // 给超级管理员群组发信息
                    var roleManager = new BaseRoleManager(dbHelper, userInfo);
                    var roleIds = roleManager.GetIds(new KeyValuePair<string, object>(BaseRoleEntity.FieldCode, "Administrators"));
                    var userIds = userManager.GetIds(new KeyValuePair<string, object>(BaseUserEntity.FieldCode, "Administrator"));
                    // 发送请求审核的信息
                    var messageEntity = new BaseMessageEntity
                    {
                        FunctionCode = MessageFunction.WaitForAudit.ToString(),

                        // Pcsky 2012.05.04 显示申请的用户名
                        Contents = userInfo.RealName + "(" + userInfo.IpAddress + ")" + AppMessage.UserServiceApplication + entity.UserName + AppMessage.UserServiceCheck
                    };
                    //messageEntity.Contents = result.RealName + "(" + result.IPAddress + ")" + AppMessage.UserService_Application + userEntity.RealName + AppMessage.UserService_Check;

                    //var messageManager = new BaseMessageManager(dbHelper, userInfo);
                    //messageManager.BatchSend(userIds, null, roleIds, messageEntity, false);
                }
            }

            return result;
        }
        #endregion

        #region public string CreateUser(BaseUserInfo userInfo, BaseUserEntity userEntity, out string statusCode, out string statusMessage)

        /// <summary>
        /// 添加用户
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        /// <param name="userEntity">用户实体</param>
        /// <param name="userLogonEntity">用户登录实体</param>
        /// <param name="userContactEntity"></param>
        /// <param name="statusCode">状态码</param>
        /// <param name="statusMessage">状态信息</param>
        /// <returns>主键</returns>
        public string CreateUser(BaseUserInfo userInfo, BaseUserEntity userEntity, BaseUserLogonEntity userLogonEntity, BaseUserContactEntity userContactEntity, out string statusCode, out string statusMessage)
        {
            var result = string.Empty;

            var returnCode = string.Empty;
            var returnMessage = string.Empty;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                result = CreateUser(dbHelper, userInfo, userEntity, userLogonEntity, userContactEntity, out returnCode, out returnMessage);
            });
            statusCode = returnCode;
            statusMessage = returnMessage;

            return result;
        }
        #endregion


        #region public BaseUserEntity GetEntity(BaseUserInfo userInfo, string id)
        /// <summary>
        /// 获取用户实体
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="id">主键</param>
        /// <returns>实体</returns>
        public BaseUserEntity GetEntity(BaseUserInfo userInfo, string id)
        {
            BaseUserEntity entity = null;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                var userManager = new BaseUserManager(dbHelper, userInfo);
                // 判断是否已经登录的用户？
                if (userManager.UserIsLogon(userInfo))
                {
                    entity = userManager.GetEntity(id);
                }
            });
            return entity;
        }
        #endregion

        #region public BaseUserEntity GetEntityByCache(BaseUserInfo userInfo, string id)
        /// <summary>
        /// 获取用户实体
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="id">主键</param>
        /// <returns>实体</returns>
        public BaseUserEntity GetEntityByCache(BaseUserInfo userInfo, string id)
        {
            BaseUserEntity entity = null;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                // var userManager = new BaseUserManager(dbHelper, userInfo);
                // 判断是否已经登录的用户？
                // if (userManager.UserIsLogon(userInfo))
                // {
                entity = BaseUserManager.GetEntityByCache(id);
                // }
            });
            return entity;
        }
        #endregion


        #region public BaseUserEntity GetEntityByCode(BaseUserInfo userInfo, string code)
        /// <summary>
        /// 按编号获取实体
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="code">编号</param>
        /// <returns>实体</returns>
        public BaseUserEntity GetEntityByCode(BaseUserInfo userInfo, string code)
        {
            BaseUserEntity entity = null;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                var manager = new BaseUserManager(dbHelper, userInfo);
                entity = manager.GetEntityByCode(code);
            });
            return entity;
        }
        #endregion

        #region public BaseUserEntity GetEntityByName(BaseUserInfo userInfo, string userName)
        /// <summary>
        /// 按名称获取实体
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="userName">用户名</param>
        /// <returns>实体</returns>
        public BaseUserEntity GetEntityByUserName(BaseUserInfo userInfo, string userName)
        {
            BaseUserEntity entity = null;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                var manager = new BaseUserManager(dbHelper, userInfo);
                entity = manager.GetEntityByUserName(userName);
            });
            return entity;
        }
        #endregion

        #region public BaseUserEntity GetEntityByRealName(BaseUserInfo userInfo, string fullName)
        /// <summary>
        /// 按名称获取实体
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="realName">名称</param>
        /// <returns>实体</returns>
        public BaseUserEntity GetEntityByRealName(BaseUserInfo userInfo, string realName)
        {
            BaseUserEntity entity = null;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                var manager = new BaseUserManager(dbHelper, userInfo);
                entity = manager.GetEntityByRealName(realName);
            });
            return entity;
        }
        #endregion


        #region public BaseUserEntity GetEntityByRealName(BaseUserInfo userInfo, string nickName)
        /// <summary>
        /// 按昵称获取实体
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="nickName">昵称</param>
        /// <returns>实体</returns>
        public BaseUserEntity GetEntityByNickName(BaseUserInfo userInfo, string nickName)
        {
            BaseUserEntity entity = null;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                var manager = new BaseUserManager(dbHelper, userInfo);
                entity = manager.GetEntityByNickName(nickName);
            });
            return entity;
        }
        #endregion


        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="id">主键</param>
        /// <returns>实体</returns>
        public BaseUserContactEntity GetUserContactObject(BaseUserInfo userInfo, string id)
        {
            BaseUserContactEntity entity = null;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                var userManager = new BaseUserManager(dbHelper, userInfo);
                // 判断是否已经登录的用户？
                if (userManager.UserIsLogon(userInfo))
                {
                    var userContactManager = new BaseUserContactManager(dbHelper, userInfo);
                    entity = userContactManager.GetEntity(id);
                }
            });

            return entity;
        }

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="id">主键</param>
        /// <returns>实体</returns>
        public BaseUserContactEntity GetUserContactObjectByCache(BaseUserInfo userInfo, string id)
        {
            BaseUserContactEntity entity = null;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                entity = BaseUserContactManager.GetEntityByCache(id);
            });

            return entity;
        }


        #region public DataTable GetDataTable(BaseUserInfo userInfo, bool showRole = true) 获取用户列表
        /// <summary>
        /// 获取用户列表
        /// 当用户非常多时，不需要显示角色
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="showRole">显示角色</param>
        /// <returns>数据表</returns>
        public DataTable GetDataTable(BaseUserInfo userInfo, bool showRole = true)
        {
            var result = new DataTable(BaseUserEntity.TableName);

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                // 这里是获取用户列表
                var userManager = new BaseUserManager(dbHelper, userInfo);
                // 获取允许登录列表
                var parameters = new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>(BaseUserEntity.FieldDeleted, 0),
                    new KeyValuePair<string, object>(BaseUserEntity.FieldEnabled, 1),
                    new KeyValuePair<string, object>(BaseUserEntity.FieldIsVisible, 1)
                };
                result = userManager.GetDataTable(parameters, BaseUserEntity.FieldSortCode);
                // 是否显示角色信息
                if (showRole)
                {
                    // 这里是获取角色列表
                    var tableName = userInfo.SystemCode + "Role";
                    var roleManager = new BaseRoleManager(dbHelper, userInfo, tableName);
                    var dataTableRole = roleManager.GetDataTable();
                    if (!result.Columns.Contains("RoleName"))
                    {
                        result.Columns.Add("RoleName");
                    }
                    // 友善的显示属于多个角色的功能
                    var roleName = string.Empty;
                    foreach (DataRow dr in result.Rows)
                    {
                        roleName = string.Empty;
                        // 获取所在角色
                        var roleIds = userManager.GetRoleIds(dr[BaseUserEntity.FieldId].ToString());
                        if (roleIds != null)
                        {
                            for (var i = 0; i < roleIds.Length; i++)
                            {
                                roleName = roleName + BaseUtil.GetProperty(dataTableRole, roleIds[i], BaseRoleEntity.FieldRealName) + " ";
                            }
                        }
                        // 设置角色的名称
                        if (!string.IsNullOrEmpty(roleName))
                        {
                            dr["RoleName"] = roleName;
                        }
                    }
                    result.AcceptChanges();
                }
                result.TableName = BaseUserEntity.TableName;
            });

            return result;
        }
        #endregion

        #region public DataTable GetDataTableByIds(BaseUserInfo userInfo, string[] ids) 获取用户列表
        /// <summary>
        /// 按主键获取用户数据
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="ids">主键数组</param>
        /// <returns>数据表</returns>
        public DataTable GetDataTableByIds(BaseUserInfo userInfo, string[] ids)
        {
            var result = new DataTable(BaseUserEntity.TableName);

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                var userManager = new BaseUserManager(dbHelper, userInfo);
                result = userManager.GetDataTable(ids);
                result.TableName = BaseUserEntity.TableName;
                result.DefaultView.Sort = BaseUserEntity.FieldSortCode;
            });

            return result;
        }
        #endregion

        #region public List<BaseUserEntity> GetList(BaseUserInfo userInfo) 获取用户列表
        /// <summary>
        /// 获取用户列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <returns>数据表</returns>
        public List<BaseUserEntity> GetList(BaseUserInfo userInfo)
        {
            var result = new List<BaseUserEntity>();

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                var userManager = new BaseUserManager(dbHelper, userInfo);
                // 获取允许登录列表
                var parameters = new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>(BaseUserEntity.FieldDeleted, 0)
                };
                result = userManager.GetList<BaseUserEntity>(parameters, BaseUserEntity.FieldSortCode);
            });

            return result;
        }
        #endregion

        #region public List<BaseUserEntity> GetListByIds(BaseUserInfo userInfo, string[] ids) 按主键获取列表
        /// <summary>
        /// 按主键获取列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="ids">主键数组</param>
        /// <returns>数据表</returns>
        public List<BaseUserEntity> GetListByIds(BaseUserInfo userInfo, string[] ids)
        {
            var result = new List<BaseUserEntity>();

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                var userManager = new BaseUserManager(dbHelper, userInfo);
                result = userManager.GetList<BaseUserEntity>(BaseUserEntity.FieldId, ids, BaseUserEntity.FieldSortCode);
            });

            return result;
        }
        #endregion

        #region public List<BaseUserEntity> GetListByManager(BaseUserInfo userInfo, string managerId) 按上级主管获取下属用户列表
        /// <summary>
        /// 按上级主管获取下属用户列表
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        /// <param name="managerId">主管主键</param>
        /// <returns>用户列表</returns>
        public List<BaseUserEntity> GetListByManager(BaseUserInfo userInfo, string managerId)
        {
            var result = new List<BaseUserEntity>();

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                var userManager = new BaseUserManager(dbHelper, userInfo);
                result = userManager.GetListByManager(managerId);
            });

            return result;
        }
        #endregion

        #region public string[] GetIdsByManager(string managerId) 按上级主管获取下属用户主键数组
        /// <summary>
        /// 按上级主管获取下属用户主键数组
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        /// <param name="managerId">主管主键</param>
        /// <returns>用户主键数组</returns>
        public string[] GetIdsByManager(BaseUserInfo userInfo, string managerId)
        {
            string[] result = null;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                var userManager = new BaseUserManager(dbHelper, userInfo);
                result = userManager.GetIdsByManager(managerId);
            });

            return result;
        }
        #endregion

        #region public DataTable Search(BaseUserInfo userInfo, string searchKey, string auditStates, string[] roleIds) 查询用户
        /// <summary>
        /// 查询用户
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="searchKey">查询</param>
        /// <param name="auditStates">有效</param>
        /// <param name="roleIds">用户角色</param>
        /// <returns>数据表</returns>
        public DataTable Search(BaseUserInfo userInfo, string searchKey, string auditStates, string[] roleIds)
        {
            var result = new DataTable(BaseUserEntity.TableName);

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                var userManager = new BaseUserManager(dbHelper, userInfo)
                {
                    ShowUserLogonInfo = false
                };
                result = userManager.Search(userInfo.SystemCode, string.Empty, searchKey, roleIds, null, auditStates, string.Empty);
                result.TableName = BaseUserEntity.TableName;
            });

            return result;
        }
        #endregion

        private void GetUserRoles(BaseUserInfo userInfo, IDbHelper dbHelper, DataTable dt)
        {
            string[] roleIds = null;
            // 这里是获取角色列表
            var tableName = userInfo.SystemCode + "Role";
            var roleManager = new BaseRoleManager(dbHelper, userInfo, tableName);
            if (!dt.Columns.Contains("RoleName"))
            {
                dt.Columns.Add("RoleName");
            }
            // 友善的显示属于多个角色的功能
            var roleName = string.Empty;
            foreach (DataRow dr in dt.Rows)
            {
                roleName = string.Empty;
                // 获取所在角色
                var userManager = new BaseUserManager(dbHelper, userInfo);
                roleIds = userManager.GetRoleIds(dr[BaseUserEntity.FieldId].ToString());
                if (roleIds != null && roleIds.Length > 0)
                {
                    for (var i = 0; i < roleIds.Length; i++)
                    {
                        roleName = roleName + BaseRoleManager.GetRealNameByCache(userInfo.SystemCode, roleIds[i]) + ", ";
                    }
                }
                // 设置角色的名称
                if (!string.IsNullOrEmpty(roleName))
                {
                    roleName = roleName.Substring(0, roleName.Length - 2);
                    dr["RoleName"] = roleName;
                }
            }
            dt.AcceptChanges();
        }


        #region public DataTable SearchByPageByDepartment(BaseUserInfo userInfo, string permissionCode, string searchKey, string auditStates, string[] roleIds, bool showRole, bool userAllInformation, out int recordCount, int pageIndex = 0, int pageSize = 100, string sort = null, string departmentId = null)
        /// <summary>
        /// 根据部门查询用户列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="permissionCode">权限编号</param>
        /// <param name="searchKey">查询</param>
        /// <param name="enabled"></param>
        /// <param name="auditStates">有效</param>
        /// <param name="roleIds">用户角色</param>
        /// <param name="showRole"></param>
        /// <param name="userAllInformation"></param>
        /// <param name="recordCount"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="sort"></param>
        /// <param name="departmentId">部门主键</param>
        /// <returns>数据表</returns>
        public DataTable SearchByPageByDepartment(BaseUserInfo userInfo, string permissionCode, string searchKey, bool? enabled, string auditStates, string[] roleIds, bool showRole, bool userAllInformation, out int recordCount, int pageIndex = 0, int pageSize = 100, string sort = null, string departmentId = null)
        {
            var result = new DataTable(BaseUserEntity.TableName);

            if (departmentId == null)
            {
                departmentId = string.Empty;
            }
            var myRecordCount = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                var userManager = new BaseUserManager(dbHelper, userInfo)
                {
                    ShowUserLogonInfo = false
                };
                result = userManager.SearchByPage(userInfo.SystemCode, permissionCode, searchKey, roleIds, enabled, auditStates, null, departmentId, showRole, userAllInformation, false, out myRecordCount, pageIndex, pageSize, sort);
                result.TableName = BaseUserEntity.TableName;
                // 是否显示角色信息
                if (showRole)
                {
                    GetUserRoles(userInfo, dbHelper, result);
                }
            });
            recordCount = myRecordCount;

            return result;
        }
        #endregion

        #region public DataTable SearchByPage(BaseUserInfo userInfo, string permissionCode, string searchKey, string auditStates, string[] roleIds, out int recordCount, int pageIndex = 0, int pageSize = 100, string sort = null) 查询用户
        /// <summary>
        /// 查询用户
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="permissionCode">权限编码</param>
        /// <param name="companyId"></param>
        /// <param name="condition">查询</param>
        /// <param name="auditStates">有效</param>
        /// <param name="roleIds">用户角色</param>
        /// <param name="enabled"></param>
        /// <param name="showRole"></param>
        /// <param name="userAllInformation"></param>
        /// <param name="recordCount"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="sort"></param>
        /// <returns>数据表</returns>
        public DataTable SearchByPage(BaseUserInfo userInfo, string permissionCode, string companyId, string condition, string auditStates, string[] roleIds, bool? enabled, bool showRole, bool userAllInformation, out int recordCount, int pageIndex = 0, int pageSize = 100, string sort = null)
        {
            var result = new DataTable();

            recordCount = 0;
            var myRecordCount = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                var userManager = new BaseUserManager(dbHelper, userInfo);
                result = userManager.SearchByPage(userInfo.SystemCode, permissionCode, condition, roleIds, enabled, auditStates, companyId, null, showRole, userAllInformation, false, out myRecordCount, pageIndex, pageSize, sort);
                result.TableName = BaseUserEntity.TableName;
                // 是否显示角色信息
                if (showRole)
                {
                    GetUserRoles(userInfo, dbHelper, result);
                }
            });
            recordCount = myRecordCount;

            return result;
        }
        #endregion

        #region public int UpdateUser(BaseUserInfo userInfo, BaseUserEntity entity, BaseUserContactEntity userContactEntity, out string statusCode, out string statusMessage)

        /// <summary>
        /// 更新用户
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        /// <param name="entity">用户实体</param>
        /// <param name="userLogonEntity">用户登录实体</param>
        /// <param name="userContactEntity">用户联系方式实体</param>
        /// <param name="statusCode">状态码</param>
        /// <param name="statusMessage">状态信息</param>
        /// <returns>影响行数</returns>
        public int UpdateUser(BaseUserInfo userInfo, BaseUserEntity entity, BaseUserLogonEntity userLogonEntity, BaseUserContactEntity userContactEntity, out string statusCode, out string statusMessage)
        {
            var result = 0;

            var returnCode = string.Empty;
            var returnMessage = string.Empty;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                var userManager = new BaseUserManager(dbHelper, userInfo);
                // 调用方法，并且返回运行结果，判断重复
                // result = userManager.Update(entity, out StatusCode);
                // 不判断重复直接更新
                if (entity != null)
                {
                    // 2015-12-09 吉日嘎拉 确认更新日志功能
                    result = userManager.Update(entity);
                    // 若是超级管理员，就是名字编号重复了，也应该能修改数据比较好，否则有些事情无法办理下去了，由于历史原因导致数据重复的什么的，也需要能修改才可以。
                    if (BaseUserManager.IsAdministrator(userInfo.Id))
                    {
                        if (userManager.StatusCode == Status.ErrorUserExist.ToString()
                            || userManager.StatusCode == Status.ErrorCodeExist.ToString())
                        {
                            result = userManager.UpdateEntity(entity);
                        }
                    }
                }
                //用户登录 Troy.Cui 2018-10-06增加
                if (userLogonEntity != null)
                {
                    var userLogonManager = new BaseUserLogonManager(dbHelper, userInfo);
                    userLogonManager.Update(userLogonEntity);
                }
                if (userContactEntity != null)
                {
                    var userContactManager = new BaseUserContactManager(dbHelper, userInfo);
                    userContactManager.SetEntity(userContactEntity);
                }
                if (result == 1)
                {
                    userManager.StatusCode = Status.OkUpdate.ToString();
                    returnCode = userManager.StatusCode;
                }
                userManager.StatusMessage = userManager.GetStateMessage(returnCode);
                // 更新员工信息
                if (entity != null)
                {
                    if (entity.IsStaff != null && entity.IsStaff > 0)
                    {
                        //BaseStaffManager staffManager = new BaseStaffManager(dbHelper, result);
                        //string staffId = staffManager.GetIdByUserId(entity.Id);
                        //if (!string.IsNullOrEmpty(staffId))
                        //{
                        //    BaseStaffEntity staffEntity = staffManager.GetEntity(staffId);
                        //    staffEntity.Code = entity.Code;
                        //    staffEntity.Birthday = entity.Birthday;
                        //    staffEntity.Gender = entity.Gender;
                        //    staffEntity.UserName = entity.UserName;
                        //    staffEntity.RealName = entity.RealName;
                        //    staffEntity.QQ = entity.QQ;
                        //    staffEntity.Mobile = entity.Mobile;
                        //    staffEntity.Telephone = entity.Telephone;
                        //    staffEntity.Email = entity.Email;
                        //    staffEntity.CompanyId = entity.CompanyId;
                        //    staffEntity.SubCompanyId = entity.SubCompanyId;
                        //    staffEntity.DepartmentId = entity.DepartmentId;
                        //    staffEntity.WorkgroupId = entity.WorkgroupId;
                        //    staffManager.Update(staffEntity);
                        //}
                    }
                }
                returnCode = userManager.StatusCode;
                returnMessage = userManager.StatusMessage;
            });
            statusCode = returnCode;
            statusMessage = returnMessage;

            return result;
        }
        #endregion

        #region public int SetUserAuditStates(BaseUserInfo userInfo, string[] ids, AuditStatus auditStates) 设置用户审核状态
        /// <summary>
        /// 设置用户审核状态
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="ids">主键数组</param>
        /// <param name="auditStates">审核状态</param>
        /// <returns>影响行数</returns>
        public int SetUserAuditStates(BaseUserInfo userInfo, string[] ids, AuditStatus auditStates)
        {
            var result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                var parameters = new List<KeyValuePair<string, object>>();
                var userManager = new BaseUserManager(dbHelper, userInfo);
                // 被审核通过
                if (auditStates == AuditStatus.AuditPass)
                {
                    parameters.Add(new KeyValuePair<string, object>(BaseUserEntity.FieldAuditStatus, auditStates.ToString()));
                    parameters.Add(new KeyValuePair<string, object>(BaseUserEntity.FieldEnabled, 1));
                    result = userManager.SetProperty(ids, parameters);

                    // 锁定时间需要去掉
                    // 密码错误次数需要修改掉
                    var userLogonManager = new BaseUserLogonManager(dbHelper, userInfo);
                    parameters = new List<KeyValuePair<string, object>>
                    {
                        new KeyValuePair<string, object>(BaseUserLogonEntity.FieldLockStartDate, null),
                        new KeyValuePair<string, object>(BaseUserLogonEntity.FieldLockEndDate, null),
                        new KeyValuePair<string, object>(BaseUserLogonEntity.FieldUserOnline, 0),
                        new KeyValuePair<string, object>(BaseUserLogonEntity.FieldPasswordErrorCount, 0)
                    };
                    result = userLogonManager.SetProperty(ids, parameters);

                    // var staffManager = new BaseStaffManager(dbHelper, result);
                    // string[] staffIds = staffManager.GetIds(BaseStaffEntity.FieldUserId, ids);
                    // staffManager.SetProperty(staffIds, new KeyValuePair<string, object>(BaseStaffEntity.FieldEnabled, 1));
                }
                // 被退回
                if (auditStates == AuditStatus.AuditReject)
                {
                    parameters.Add(new KeyValuePair<string, object>(BaseUserEntity.FieldAuditStatus, auditStates.ToString()));
                    parameters.Add(new KeyValuePair<string, object>(BaseUserEntity.FieldEnabled, 0));
                    // parameters.Add(new KeyValuePair<string, object>(BaseUserEntity.FieldAuditStatus, Status.UserLocked.ToString()));
                    result = userManager.SetProperty(ids, parameters);
                }
            });

            return result;
        }
        #endregion

        #region public int SetUserManagerAuditStates(BaseUserInfo userInfo, string[] ids, AuditStatus auditStates) 设置用户主管的审核状态
        /// <summary>
        /// 设置用户主管的审核状态
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="ids">主键数组</param>
        /// <param name="auditStates">审核状态</param>
        /// <returns>影响行数</returns>
        public int SetUserManagerAuditStates(BaseUserInfo userInfo, string[] ids, AuditStatus auditStates)
        {
            var result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                var parameters = new List<KeyValuePair<string, object>>();
                var userManager = new BaseUserManager(dbHelper, userInfo);
                // 被审核通过
                if (auditStates == AuditStatus.AuditPass)
                {
                    parameters.Add(new KeyValuePair<string, object>(BaseUserEntity.FieldManagerAuditStatus, auditStates.ToString()));
                    parameters.Add(new KeyValuePair<string, object>(BaseUserEntity.FieldManagerAuditDate, DateTime.Now));
                    parameters.Add(new KeyValuePair<string, object>(BaseUserEntity.FieldEnabled, 1));
                    result = userManager.SetProperty(ids, parameters);

                    // 锁定时间需要去掉
                    // 密码错误次数需要修改掉
                    var userLogonManager = new BaseUserLogonManager(dbHelper, userInfo);
                    parameters = new List<KeyValuePair<string, object>>
                    {
                        new KeyValuePair<string, object>(BaseUserLogonEntity.FieldLockStartDate, null),
                        new KeyValuePair<string, object>(BaseUserLogonEntity.FieldLockEndDate, null),
                        new KeyValuePair<string, object>(BaseUserLogonEntity.FieldUserOnline, 0),
                        new KeyValuePair<string, object>(BaseUserLogonEntity.FieldPasswordErrorCount, 0)
                    };
                    result = userLogonManager.SetProperty(ids, parameters);

                    // var staffManager = new BaseStaffManager(dbHelper, result);
                    // string[] staffIds = staffManager.GetIds(BaseStaffEntity.FieldUserId, ids);
                    // staffManager.SetProperty(staffIds, new KeyValuePair<string, object>(BaseStaffEntity.FieldEnabled, 1));
                }
                // 被退回
                if (auditStates == AuditStatus.AuditReject)
                {
                    parameters.Add(new KeyValuePair<string, object>(BaseUserEntity.FieldManagerAuditStatus, auditStates.ToString()));
                    parameters.Add(new KeyValuePair<string, object>(BaseUserEntity.FieldManagerAuditDate, DateTime.Now));
                    parameters.Add(new KeyValuePair<string, object>(BaseUserEntity.FieldEnabled, 0));
                    parameters.Add(new KeyValuePair<string, object>(BaseUserEntity.FieldAuditStatus, auditStates.ToString()));
                    result = userManager.SetProperty(ids, parameters);
                }
            });

            return result;
        }
        #endregion

        #region public int RemoveMobileBinding(BaseUserInfo userInfo, string mobile) 解除手机认证帮定
        /// <summary>
        /// 解除手机认证帮定
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="mobile">手机号码</param>
        /// <returns>影响行数</returns>
        public int RemoveMobileBinding(BaseUserInfo userInfo, string mobile)
        {
            var result = 0;

            var returnCode = string.Empty;
            var returnMessage = string.Empty;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                var manager = new BaseUserContactManager(dbHelper, userInfo);
                result = manager.RemoveMobileBinding(mobile);
            });

            return result;
        }
        #endregion

        #region public int BatchSave(BaseUserInfo userInfo, DataTable result)
        /// <summary>
        /// 批量保存
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="dt">数据表</param>
        /// <returns>影响行数</returns>
        public int BatchSave(BaseUserInfo userInfo, DataTable dt)
        {
            var result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                var userManager = new BaseUserManager(dbHelper, userInfo);
                result = userManager.BatchSave(dt);
            });

            return result;
        }
        #endregion

        /// <summary>
        /// 同步数据
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="all">同步所有数据</param>
        /// <returns>影响行数</returns>
        public int Synchronous(BaseUserInfo userInfo, bool all = false)
        {
            var result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                // var userManager = new BaseUserManager(dbHelper, userInfo);
                // result = userManager.Synchronous(all);
            });

            return result;
        }

        #region public int SetDeleted(BaseUserInfo userInfo, string[] ids) 批量设置删除
        /// <summary>
        /// 批量设置删除
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="ids">主键数组</param>
        /// <returns>影响行数</returns>
        public int SetDeleted(BaseUserInfo userInfo, string[] ids)
        {
            var result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                var userManager = new BaseUserManager(dbHelper, userInfo);
                result = userManager.SetDeleted(ids, true, true, 4);
            });

            return result;
        }
        #endregion

        #region public int Delete(BaseUserInfo userInfo, string id) 单个删除
        /// <summary>
        /// 单个删除
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="id">主键</param>
        /// <returns>影响行数</returns>
        public int Delete(BaseUserInfo userInfo, string id)
        {
            var result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                var userManager = new BaseUserManager(dbHelper, userInfo);
                result = userManager.Delete(id);
                // 用户已经被删除的员工的UserId设置为Null，说白了，是需要整理数据
                userManager.CheckUserStaff();
            });

            return result;
        }
        #endregion

        #region public int BatchDelete(BaseUserInfo userInfo, string[] ids) 批量删除
        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="ids">主键数组</param>
        /// <returns>影响行数</returns>
        public int BatchDelete(BaseUserInfo userInfo, string[] ids)
        {
            var result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                var userManager = new BaseUserManager(dbHelper, userInfo);
                result = userManager.Delete(ids);
                // 用户已经被删除的员工的UserId设置为Null，说白了，是需要整理数据
                userManager.CheckUserStaff();
            });

            return result;
        }
        #endregion

        #region public DataTable GetDataTableByPage(BaseUserInfo userInfo, out int recordCount, int pageIndex, int pageSize, string condition, List<KeyValuePair<string, object>> dbParameters, string order = null)
        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="recordCount">记录数</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">每页显示</param>
        /// <param name="condition">条件</param>
        /// <param name="dbParameters">参数</param>
        /// <param name="order">排序</param>
        /// <returns>数据表</returns>
        public DataTable GetDataTableByPage(BaseUserInfo userInfo, out int recordCount, int pageIndex, int pageSize, string condition, List<KeyValuePair<string, object>> dbParameters, string order = null)
        {
            var result = new DataTable(BaseUserEntity.TableName);
            var myRecordCount = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                if (SecretUtil.IsSqlSafe(condition))
                {
                    var userManager = new BaseUserManager(dbHelper, userInfo)
                    {
                        ShowUserLogonInfo = false
                    };
                    result = userManager.GetDataTableByPage(out myRecordCount, pageIndex, pageSize, condition, dbHelper.MakeParameters(dbParameters), order);
                    result.TableName = BaseUserEntity.TableName;
                }
                else
                {
                    // 记录注入日志
                    LogUtil.WriteLog("userInfo:" + userInfo.Serialize() + " " + condition, "SqlSafe");
                }
            });
            recordCount = myRecordCount;

            return result;
        }
        #endregion
    }
}