//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2023, DotNet.
//-----------------------------------------------------------------

using System.Collections.Generic;
using System.Data;
using System.Reflection;


namespace DotNet.Business
{
    using IService;
    using Model;
    using System;
    using Util;

    /// <summary>
    /// BaseRoleService
    /// 角色管理服务
    /// 
    /// 修改记录
    /// 
    ///     2010.03.12 版本：1.5 JiRiGaLa 程序注释整理、函数整理、函数顺序等。
    ///     2008.04.09 版本：1.4 JiRiGaLa 重新整理主键。
    ///     2007.06.11 版本：1.3 JiRiGaLa 加入调试信息#if (DEBUG)。
    ///     2007.06.04 版本：1.2 JiRiGaLa 加入WebService。
    ///     2007.05.29 版本：1.1 JiRiGaLa 排版，修改，完善。
    ///		2007.05.11 版本：1.0 JiRiGaLa 窗体与数据库连接的分离。
    ///		
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2010.03.12</date>
    /// </author> 
    /// </summary>


    public partial class BaseRoleService : IBaseRoleService
    {
        /// <summary>
        /// 添加角色
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="entity">实体</param>
        /// <param name="status">状态</param>
        /// <param name="statusMessage">返回状态信息</param>
        /// <returns>主键</returns>
        public string Add(BaseUserInfo userInfo, BaseRoleEntity entity, out Status status, out string statusMessage)
        {
            var result = string.Empty;

            var returnCode = Status.Ok;
            var returnMessage = string.Empty;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDbWithTransaction(userInfo, parameter, (dbHelper) =>
            {
                // 这里是判断已经登录的用户是否有调用当前函数的权限，加强服务层被远程调用的安全性的，会损失服务器的性能
                // var permissionManager = new BasePermissionManager(result);
                // if (permissionManager.CheckPermissionByUser(result.Id, "RoleAdmin.Add", "添加角色"))
                // {
                var tableName = userInfo.SystemCode + "Role";
                var manager = new BaseRoleManager(dbHelper, userInfo, tableName);
                result = manager.Add(entity, out returnCode);
                returnMessage = manager.GetStateMessage(returnCode);
                // }
                //else
                //{
                //    Status = Status.AccessDeny;
                //    StatusCode = Status.AccessDeny.ToString();
                //    permissionManager.GetStateMessage(StatusCode);
                //}
            });
            status = returnCode;
            statusMessage = returnMessage;

            return result;
        }

        #region public DataTable GetDataTable(BaseUserInfo userInfo, string systemCode) 获取角色列表
        /// <summary>
        /// 获取角色列表
        /// </summary>
        /// <param name="systemCode">系统编号</param>
        /// <param name="userInfo">用户</param>
        /// <returns>数据表</returns>
        public DataTable GetDataTable(BaseUserInfo userInfo, string systemCode)
        {
            var result = new DataTable(BaseRoleEntity.CurrentTableName);

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                var tableName = systemCode + "Role";
                // 获得角色列表
                var manager = new BaseRoleManager(dbHelper, userInfo, tableName);
                var parameters = new List<KeyValuePair<string, object>>();
                // parameters.Add(new KeyValuePair<string, object>(BaseRoleEntity.FieldDeleted, 0));
                // parameters.Add(new KeyValuePair<string, object>(BaseRoleEntity.FieldEnabled, 1)); //如果1 只显示有效用户
                // parameters.Add(new KeyValuePair<string, object>(BaseRoleEntity.FieldIsVisible, 1));
                // dt = manager.GetDataTable(parameters, BaseRoleEntity.FieldSortCode);
                result = manager.GetDataTable(0, BaseRoleEntity.FieldSortCode);
                result.TableName = tableName;
                if (!userInfo.SystemCode.Equals("Base", StringComparison.OrdinalIgnoreCase))
                {
                    manager = new BaseRoleManager(dbHelper, userInfo);
                    parameters = new List<KeyValuePair<string, object>>
                    {
                        new KeyValuePair<string, object>(BaseRoleEntity.FieldCategoryCode, "SystemRole"),
                        new KeyValuePair<string, object>(BaseRoleEntity.FieldDeleted, 0),
                        new KeyValuePair<string, object>(BaseRoleEntity.FieldEnabled, 1)
                    };
                    var dtBase = manager.GetDataTable(parameters);
                    result.Merge(dtBase, true);
                }
            });

            return result;
        }
        #endregion

        #region public List<BaseRoleEntity> GetList(BaseUserInfo userInfo, string systemCode) 获取角色列表
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="systemCode">系统编号</param>
        /// <returns>列表</returns>
        public List<BaseRoleEntity> GetList(BaseUserInfo userInfo, string systemCode)
        {
            var result = new List<BaseRoleEntity>();

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                var tableName = systemCode + "Role";
                // 获得角色列表
                var manager = new BaseRoleManager(dbHelper, userInfo, tableName);
                var parameters = new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>(BaseRoleEntity.FieldDeleted, 0)
                };
                result = manager.GetList<BaseRoleEntity>(parameters, BaseRoleEntity.FieldSortCode);
            });

            return result;
        }
        #endregion

        #region public List<BaseRoleEntity> GetListByIds(BaseUserInfo userInfo, string[] ids) 按主键获取列表
        /// <summary>
        /// 按主键获取列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="ids">主键数组</param>
        /// <returns>数据表</returns>
        public List<BaseRoleEntity> GetListByIds(BaseUserInfo userInfo, string[] ids)
        {
            var result = new List<BaseRoleEntity>();

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                var roleManager = new BaseRoleManager(dbHelper, userInfo);
                result = roleManager.GetList<BaseRoleEntity>(BaseRoleEntity.FieldId, ids, BaseRoleEntity.FieldSortCode);
            });

            return result;
        }
        #endregion

        /// <summary>
        /// 获取默认角色列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <returns>数据表</returns>
        public DataTable GetDefaultDutyDT(BaseUserInfo userInfo)
        {
            var dt = new DataTable(BaseRoleEntity.CurrentTableName);

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                // 获得角色列表
                var tableName = userInfo.SystemCode + "Role";
                var manager = new BaseRoleManager(dbHelper, userInfo, tableName);
                var parameters = new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>(BaseRoleEntity.FieldCategoryCode, "DefaultDuty"),
                    new KeyValuePair<string, object>(BaseRoleEntity.FieldDeleted, 0),
                    new KeyValuePair<string, object>(BaseRoleEntity.FieldIsVisible, 1)
                };
                dt = manager.GetDataTable(parameters, BaseRoleEntity.FieldSortCode);
                dt.TableName = BaseRoleEntity.CurrentTableName;
            });
            return dt;
        }

        /// <summary>
        /// 获取业务角色列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <returns>数据表</returns>
        public DataTable GetApplicationRole(BaseUserInfo userInfo)
        {
            var dt = new DataTable(BaseRoleEntity.CurrentTableName);

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                var tableName = BaseRoleEntity.CurrentTableName;
                if (!string.IsNullOrEmpty(userInfo.SystemCode))
                {
                    tableName = userInfo.SystemCode + "Role";
                }
                // 获得角色列表
                var manager = new BaseRoleManager(dbHelper, userInfo, tableName);
                var parameters = new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>(BaseRoleEntity.FieldCategoryCode, "ApplicationRole"),
                    new KeyValuePair<string, object>(BaseRoleEntity.FieldDeleted, 0),
                    new KeyValuePair<string, object>(BaseRoleEntity.FieldIsVisible, 1)
                };
                dt = manager.GetDataTable(parameters, BaseRoleEntity.FieldSortCode);
                dt.TableName = tableName;
            });
            return dt;
        }

        /// <summary>
        /// 获取用户群组列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <returns>数据表</returns>
        public DataTable GetUserGroup(BaseUserInfo userInfo)
        {
            var dt = new DataTable(BaseRoleEntity.CurrentTableName);

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                var tableName = userInfo.SystemCode + "Role";
                // 获得角色列表
                var manager = new BaseRoleManager(dbHelper, userInfo, tableName);
                var where = " (CreateUserId = '" + userInfo.Id + "' OR Id IN (SELECT RoleId FROM BaseUserRole WHERE (UserId = '" + userInfo.Id + "'))) AND (" + BaseUserRoleEntity.FieldDeleted + " = 0) AND (Enabled = 1) AND (CategoryCode = 'UserGroup')";
                dt = manager.GetDataTable(where);
                dt.TableName = tableName;
            });
            return dt;
        }

        /// <summary>
        /// 获取用户群组列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <returns>数据表</returns>
        public DataTable GetUserGroupByUser(BaseUserInfo userInfo)
        {
            var dt = new DataTable(BaseRoleEntity.CurrentTableName);

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                var tableName = userInfo.SystemCode + "Role";
                // 获得角色列表
                var manager = new BaseRoleManager(dbHelper, userInfo, tableName);
                var parameters = new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>(BaseRoleEntity.FieldCategoryCode, "UserGroup"),
                    new KeyValuePair<string, object>(BaseRoleEntity.FieldDeleted, 0),
                    new KeyValuePair<string, object>(BaseRoleEntity.FieldCreateUserId, userInfo.Id)
                };
                dt = manager.GetDataTable(parameters, BaseRoleEntity.FieldSortCode);
                dt.TableName = tableName;
            });
            return dt;
        }

        /// <summary>
        /// 按组织机构获取角色列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="organizationId">组织机构主键</param>
        /// <param name="showUser">显示用户</param>
        /// <returns>数据表</returns>
        public DataTable GetDataTableByOrganization(BaseUserInfo userInfo, string organizationId, bool showUser = true)
        {
            var dt = new DataTable(BaseRoleEntity.CurrentTableName);

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                var tableName = userInfo.SystemCode + "Role";
                // 获得角色列表
                var manager = new BaseRoleManager(dbHelper, userInfo, tableName);
                dt = manager.GetDataTableByOrganization(organizationId);
                var userManager = new BaseUserManager(dbHelper, userInfo, tableName);
                if (showUser)
                {
                    var dataTableUser = userManager.GetDataTable();
                    if (!dt.Columns.Contains("Users"))
                    {
                        dt.Columns.Add("Users");
                    }
                    // 友善的显示属于多个角色的功能
                    var userName = string.Empty;
                    foreach (DataRow dr in dt.Rows)
                    {
                        userName = string.Empty;
                        // 获取所在用户
                        var userIds = userManager.GetUserIdsInRoleId(userInfo.SystemCode, dr[BaseRoleEntity.FieldId].ToString());
                        if (userIds != null)
                        {
                            for (var i = 0; i < userIds.Length; i++)
                            {
                                userName = userName + BaseUtil.GetProperty(dataTableUser, userIds[i], BaseUserEntity.FieldRealName) + ", ";
                            }
                        }
                        if (!string.IsNullOrEmpty(userName))
                        {
                            userName = userName.Substring(0, userName.Length - 2);
                            // 设置用户的名称
                            dr["Users"] = userName;
                        }
                    }
                    dt.AcceptChanges();
                }
                dt.TableName = BaseRoleEntity.CurrentTableName;
            });
            return dt;
        }

        /// <summary>
        /// 按角色名获取角色列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="roleName">角色名</param>
        /// <returns>数据表</returns>
        public DataTable GetDataTableByRoleName(BaseUserInfo userInfo, string roleName)
        {
            var dt = new DataTable(BaseRoleEntity.CurrentTableName);

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                var tableName = userInfo.SystemCode + "Role";
                // 获得角色列表
                var manager = new BaseRoleManager(dbHelper, userInfo, tableName);
                dt = manager.GetDataTableByName(roleName);
                dt.TableName = BaseRoleEntity.CurrentTableName;
            });
            return dt;
        }

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="id">主键</param>
        /// <returns>实体</returns>
        public BaseRoleEntity GetEntity(BaseUserInfo userInfo, string id)
        {
            BaseRoleEntity result = null;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                var tableName = userInfo.SystemCode + "Role";
                var manager = new BaseRoleManager(dbHelper, userInfo, tableName);
                result = manager.GetEntity(id);
            });

            return result;
        }

        /// <summary>
        /// 更新实体
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="entity">实体</param>
        /// <param name="status">状态</param>
        /// <param name="statusMessage">返回状态信息</param>
        /// <returns>影响行数</returns>
        public int Update(BaseUserInfo userInfo, BaseRoleEntity entity, out Status status, out string statusMessage)
        {
            var result = 0;

            var returnCode = Status.Ok;
            var returnMessage = string.Empty;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDbWithTransaction(userInfo, parameter, (dbHelper) =>
            {
                var tableName = userInfo.SystemCode + "Role";
                var manager = new BaseRoleManager(dbHelper, userInfo, tableName);
                result = manager.UniqueUpdate(entity, out returnCode);
                returnMessage = manager.GetStateMessage(returnCode);
            });
            status = returnCode;
            statusMessage = returnMessage;

            return result;
        }

        /// <summary>
        /// 按主键数组获取列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="ids">角色主键</param>
        /// <returns>数据表</returns>
        public DataTable GetDataTableByIds(BaseUserInfo userInfo, string[] ids)
        {
            var result = new DataTable(BaseRoleEntity.CurrentTableName);

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                var tableName = userInfo.SystemCode + "Role";
                var manager = new BaseRoleManager(dbHelper, userInfo, tableName);
                result = manager.GetDataTable(BaseRoleEntity.FieldId, ids, BaseRoleEntity.FieldSortCode);
                result.TableName = BaseRoleEntity.CurrentTableName;
            });

            return result;
        }

        /// <summary>
        /// 查询角色列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="organizationId">组织机构主键</param>
        /// <param name="searchKey">查询字符串</param>
        /// <returns>数据表</returns>
        public DataTable Search(BaseUserInfo userInfo, string organizationId, string searchKey)
        {
            var result = new DataTable(BaseRoleEntity.CurrentTableName);

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                var tableName = userInfo.SystemCode + "Role";
                // 获得角色列表
                var manager = new BaseRoleManager(dbHelper, userInfo, tableName);
                result = manager.Search(organizationId, searchKey);
                result.TableName = BaseRoleEntity.CurrentTableName;
            });

            return result;
        }

        /// <summary>
        /// 移动角色数据
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="id">主键</param>
        /// <param name="targetId">目标主键</param>
        /// <returns>影响行数</returns>
        public int MoveTo(BaseUserInfo userInfo, string id, string targetId)
        {
            var result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                var tableName = userInfo.SystemCode + "Role";
                var manager = new BaseRoleManager(dbHelper, userInfo, tableName);
                result = manager.MoveTo(id, targetId);
            });

            return result;
        }

        /// <summary>
        /// 批量移动角色数据
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="ids">主键数组</param>
        /// <param name="targetId">目标主键</param>
        /// <returns>影响行数</returns>
        public int BatchMoveTo(BaseUserInfo userInfo, string[] ids, string targetId)
        {
            var result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                var tableName = userInfo.SystemCode + "Role";
                var manager = new BaseRoleManager(dbHelper, userInfo, tableName);
                result = manager.BatchMoveTo(ids, targetId);
            });

            return result;
        }

        /// <summary>
        /// 排序角色顺序
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="organizationId">组织机构主键</param>
        /// <returns>影响行数</returns>
        public int ResetSortCode(BaseUserInfo userInfo, string organizationId)
        {
            var result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                var tableName = userInfo.SystemCode + "Role";
                var manager = new BaseRoleManager(dbHelper, userInfo, tableName);
                result = manager.ResetSortCode(organizationId);
            });

            return result;
        }

        /// <summary>
        /// 删除角色
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="id">主键</param>
        /// <returns>数据表</returns>
        public int Delete(BaseUserInfo userInfo, string id)
        {
            var result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                var tableName = userInfo.SystemCode + "Role";
                var manager = new BaseRoleManager(dbHelper, userInfo, tableName);
                result = manager.Delete(id);
            });

            return result;
        }

        /// <summary>
        /// 批量删除角色
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="ids">主键数组</param>
        /// <returns>影响行数</returns>
        public int BatchDelete(BaseUserInfo userInfo, string[] ids)
        {
            var result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDbWithTransaction(userInfo, parameter, (dbHelper) =>
            {
                var tableName = userInfo.SystemCode + "Role";
                var manager = new BaseRoleManager(dbHelper, userInfo, tableName);
                // 这里是直接删除功能的实现
                // result = roleManager.BatchDelete(ids);

                BaseRoleEntity roleEntity = null;
                // 把删除的记录放到被删除的表里(表名后面加了后缀Deleted，也可以放在另外一个数据库里也可以的)
                var roleDeletedManager = new BaseRoleManager(dbHelper, userInfo, tableName + "Deleted");
                foreach (var id in ids)
                {
                    // 逐个删除，逐个备份
                    roleEntity = manager.GetEntity(id);
                    // 先添加到被删除的表里，这时候原先数据的主键需要保留的，否则恢复数据时可能会乱套
                    roleDeletedManager.Add(roleEntity);
                    // 数据备份好后再进行删除处理
                    result += manager.Delete(id);
                }
            });

            return result;
        }

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
                var tableName = userInfo.SystemCode + "Role";
                var manager = new BaseRoleManager(dbHelper, userInfo, tableName);
                result = manager.SetDeleted(ids, true);
            });

            return result;
        }

        /// <summary>
        /// 批量保存角色
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="roleEntites">角色列表</param>
        /// <returns>影响行数</returns>
        public int BatchSave(BaseUserInfo userInfo, List<BaseRoleEntity> roleEntites)
        {
            var result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDbWithTransaction(userInfo, parameter, (dbHelper) =>
            {
                var tableName = userInfo.SystemCode + "Role";
                var manager = new BaseRoleManager(dbHelper, userInfo, tableName);
                result = manager.BatchSave(roleEntites);
                // clude：380405622 如果Enable=0，则BaseRoleUser中的用户角色对应关系 enable=0；如果Enable=1，则BaseRoleUser中的用户角色对应关系 enable=1
                tableName = BaseSystemInfo.SystemCode + "UserRole";
                var keyValuePairs = new List<KeyValuePair<string, object>>();
                foreach (var roleEntity in roleEntites)
                {
                    keyValuePairs.Clear();
                    keyValuePairs.Add(new KeyValuePair<string, object>(BaseUserRoleEntity.FieldRoleId, roleEntity.Id));
                    if (roleEntity.Enabled == 0)
                    {
                        new BaseUserRoleManager(dbHelper, userInfo, tableName).Update(keyValuePairs, new KeyValuePair<string, object>(BaseUserRoleEntity.FieldEnabled, "0"));
                    }
                    else
                    {
                        new BaseUserRoleManager(dbHelper, userInfo, tableName).Update(keyValuePairs, new KeyValuePair<string, object>(BaseUserRoleEntity.FieldEnabled, "1"));
                    }
                }
            });
            return result;
        }

        /// <summary>
        /// 获取属性
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        /// <param name="parameters">参数</param>
        /// <param name="targetField">目标字段</param>
        /// <returns></returns>
        public string GetProperty(BaseUserInfo userInfo, List<KeyValuePair<string, object>> parameters, string targetField)
        {
            var result = string.Empty;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterDb(userInfo, parameter, (dbHelper) =>
            {
                var manager = new BaseRoleManager(dbHelper, userInfo);
                result = manager.GetProperty(parameters, targetField);
            });

            return result;
        }

        /// <summary>
        /// 刷新列表
        /// 2015-12-11 吉日嘎拉 刷新缓存功能优化
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <returns>数据表</returns>
        public void CachePreheating(BaseUserInfo userInfo)
        {
            BaseRoleManager.CachePreheating();
        }
    }
}