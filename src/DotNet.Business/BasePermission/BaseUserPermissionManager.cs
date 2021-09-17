//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;

namespace DotNet.Business
{
    using Model;
    using Util;

    /// <summary>
    /// BaseUserPermissionManager
    /// 用户权限
    /// 
    /// 修改记录
    ///
    ///     2016.02.25 版本：2.2 JiRiGaLa 通过缓存获取权限的方法改进。
    ///     2016.01.06 版本：2.1 JiRiGaLa 分 systemCode 进行代码整顿。
    ///     2015.07.03 版本：2.0 JiRiGaLa 每个公司有查看每个公司自己授权情况的权限。
    ///     2010.04.23 版本：1.2 JiRiGaLa Enabled 不允许为NULL的错误解决。
    ///     2008.10.23 版本：1.1 JiRiGaLa 修改为用户权限。
    ///     2008.05.23 版本：1.0 JiRiGaLa 创建主键。
    ///
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2016.02.25</date>
    /// </author>
    /// </summary>
    public class BaseUserPermissionManager : BaseManager, IBaseManager
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public BaseUserPermissionManager()
        {
            CurrentTableName = BasePermissionEntity.TableName;
        }

        /// <summary>
        /// 构造函数
        /// <param name="tableName">指定表名</param>
        /// </summary>
        public BaseUserPermissionManager(string tableName)
        {
            CurrentTableName = tableName;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        public BaseUserPermissionManager(IDbHelper dbHelper)
            : this()
        {
            DbHelper = dbHelper;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        public BaseUserPermissionManager(BaseUserInfo userInfo)
            : this()
        {
            UserInfo = userInfo;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        /// <param name="userInfo">用户信息</param>
        public BaseUserPermissionManager(IDbHelper dbHelper, BaseUserInfo userInfo)
            : this(dbHelper)
        {
            UserInfo = userInfo;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        /// <param name="userInfo">用户信息</param>
        /// <param name="tableName">指定表名</param>
        public BaseUserPermissionManager(IDbHelper dbHelper, BaseUserInfo userInfo, string tableName)
            : this(dbHelper, userInfo)
        {
            CurrentTableName = tableName;
        }

        /// <summary>
        /// 判断用户是否有有相应的权限
        /// </summary>
        /// <param name="systemCode">系统编号</param>
        /// <param name="userId">用户主键</param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>有权限</returns>
        public bool CheckPermission(string systemCode, string userId, string permissionCode)
        {
            if (string.IsNullOrEmpty(systemCode))
            {
                return false;
            }

            if (string.IsNullOrEmpty(userId))
            {
                return false;
            }

            var permissionId = BaseModuleManager.GetIdByCodeByCache(systemCode, permissionCode);
            // 没有找到相应的权限
            if (string.IsNullOrEmpty(permissionId))
            {
                return false;
            }

            CurrentTableName = systemCode + "Permission";
            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BasePermissionEntity.FieldResourceCategory, BaseUserEntity.TableName),
                new KeyValuePair<string, object>(BasePermissionEntity.FieldResourceId, userId),
                new KeyValuePair<string, object>(BasePermissionEntity.FieldEnabled, 1),
                new KeyValuePair<string, object>(BasePermissionEntity.FieldDeleted, 0),
                //宋彪注：permisssionId先没加上
                new KeyValuePair<string, object>(BasePermissionEntity.FieldPermissionId, permissionId)
            };
            return Exists(parameters);
        }

        /// <summary>
        /// 重置权限
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static string[] ResetPermissionByCache(string systemCode, string userId)
        {
            var key = "Permission:" + systemCode + ":User:" + userId;
            CacheUtil.Remove(key);
            return GetPermissionIdsByCache(systemCode, userId);
        }

       /// <summary>
       /// 获取权限编号
       /// </summary>
       /// <param name="systemCode">系统编码</param>
       /// <param name="userId"></param>
       /// <returns></returns>
        public static string[] GetPermissionIdsByCache(string systemCode, string userId)
        {
            string[] result = null;

            var key = string.Empty;
            key = "Permission:" + systemCode + ":User:" + userId;

            result = CacheUtil.Cache(key, () => new BaseUserPermissionManager().GetPermissionIds(systemCode, userId), true);
            
            return result;
        }

        #region public string[] GetPermissionIds(string systemCode, string userId) 获取用户的权限主键数组
        /// <summary>
        /// 获取用户的权限主键数组
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="userId">用户主键</param>
        /// <returns>主键数组</returns>
        public string[] GetPermissionIds(string systemCode, string userId)
        {
            CurrentTableName = systemCode + "Permission";
            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BasePermissionEntity.FieldResourceCategory, BaseUserEntity.TableName),
                new KeyValuePair<string, object>(BasePermissionEntity.FieldResourceId, userId),
                new KeyValuePair<string, object>(BasePermissionEntity.FieldEnabled, 1),
                new KeyValuePair<string, object>(BasePermissionEntity.FieldDeleted, 0)
            };

            return GetProperties(parameters, BasePermissionEntity.FieldPermissionId);
        }
        #endregion

        #region public string[] GetUserIds(string systemCode, string permissionId) 获取用户主键数组
        /// <summary>
        /// 获取用户主键数组
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="permissionId">操作权限主键</param>
        /// <returns>主键数组</returns>
        public string[] GetUserIds(string systemCode, string permissionId)
        {
            CurrentTableName = systemCode + "Permission";
            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BasePermissionEntity.FieldPermissionId, permissionId),
                new KeyValuePair<string, object>(BasePermissionEntity.FieldResourceCategory, BaseUserEntity.TableName),
                new KeyValuePair<string, object>(BasePermissionEntity.FieldEnabled, 1),
                new KeyValuePair<string, object>(BasePermissionEntity.FieldDeleted, 0)
            };

            return GetProperties(parameters, BasePermissionEntity.FieldResourceId);
        }
        #endregion

        //
        // 授予权限的实现部分
        //

        #region public string Grant(string systemCode, string userId, string permissionId, bool chekExists = true) 为了提高授权的运行速度
        /// <summary>
        /// 为了提高授权的运行速度
        /// </summary>
        /// <param name="systemCode">系统编号</param>
        /// <param name="userId">用户主键</param>
        /// <param name="permissionId">权限主键</param>
        /// <param name="chekExists">判断是否存在</param>
        /// <returns>主键</returns>
        public string Grant(string systemCode, string userId, string permissionId, bool chekExists = true)
        {
            var result = string.Empty;

            if (string.IsNullOrEmpty(userId) && string.IsNullOrEmpty(permissionId))
            {
                return result;
            }

            CurrentTableName = systemCode + "Permission";

            var currentId = string.Empty;
            // 判断是否已经存在这个权限，若已经存在就不重复增加了
            if (chekExists)
            {
                var whereParameters = new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>(BasePermissionEntity.FieldResourceCategory, BaseUserEntity.TableName),
                    new KeyValuePair<string, object>(BasePermissionEntity.FieldResourceId, userId),
                    new KeyValuePair<string, object>(BasePermissionEntity.FieldPermissionId, permissionId)
                };
                currentId = GetId(whereParameters);
                if (!string.IsNullOrEmpty(currentId))
                {
                    var parameters = new List<KeyValuePair<string, object>>
                    {
                        new KeyValuePair<string, object>(BasePermissionEntity.FieldEnabled, 1),
                        new KeyValuePair<string, object>(BasePermissionEntity.FieldDeleted, 0),
                        new KeyValuePair<string, object>(BasePermissionEntity.FieldUpdateUserId, UserInfo.Id),
                        new KeyValuePair<string, object>(BasePermissionEntity.FieldUpdateBy, UserInfo.RealName),
                        new KeyValuePair<string, object>(BasePermissionEntity.FieldUpdateTime, DateTime.Now)
                    };
                    // 更新状态，设置为有效、并取消删除，权限也不是天天变动的，所以可以更新一下
                    SetProperty(currentId, parameters);

                    result = currentId;
                }
            }

            if (string.IsNullOrEmpty(currentId))
            {
                var permissionEntity = new BasePermissionEntity
                {
                    ResourceCategory = BaseUserEntity.TableName,
                    ResourceId = userId,
                    PermissionId = permissionId,
                    Enabled = 1
                };
                // 2015-07-03 吉日嘎拉 若是没有公司相关的信息，就把公司区分出来，每个公司可以看每个公司的数据
                if (string.IsNullOrEmpty(permissionEntity.CompanyId))
                {
                    var entity = BaseUserManager.GetEntityByCache(userId);
                    if (entity != null)
                    {
                        permissionEntity.CompanyId = entity.CompanyId;
                        permissionEntity.CompanyName = entity.CompanyName;
                    }
                }

                var permissionManager = new BasePermissionManager(DbHelper, UserInfo, CurrentTableName);
                result = permissionManager.Add(permissionEntity, true, false);
            }

            // 2015-09-21 吉日嘎拉 这里增加变更日志
            var tableName = systemCode + ".Permission.User";
            var sqlBuilder = new SqlBuilder(DbHelper);
            sqlBuilder.BeginInsert(BaseModifyRecordEntity.TableName);
            sqlBuilder.SetValue(BaseModifyRecordEntity.FieldTableCode, tableName);
            if (DbHelper.CurrentDbType == CurrentDbType.Oracle)
            {
                sqlBuilder.SetFormula(BaseModifyRecordEntity.FieldId, "SEQ_" + BaseModifyRecordEntity.TableName + ".NEXTVAL");
            }
            sqlBuilder.SetValue(BaseModifyRecordEntity.FieldRecordKey, userId);
            sqlBuilder.SetValue(BaseModifyRecordEntity.FieldColumnCode, "授权");
            sqlBuilder.SetValue(BaseModifyRecordEntity.FieldColumnDescription, BaseModuleManager.GetNameByCache(systemCode, permissionId));
            sqlBuilder.SetValue(BaseModifyRecordEntity.FieldOldValue, null);
            sqlBuilder.SetValue(BaseModifyRecordEntity.FieldNewValue, permissionId);
            sqlBuilder.SetValue(BaseModifyRecordEntity.FieldCreateUserId, UserInfo.Id);
            sqlBuilder.SetValue(BaseModifyRecordEntity.FieldCreateBy, UserInfo.RealName);
            sqlBuilder.SetDbNow(BaseModifyRecordEntity.FieldCreateTime);
            sqlBuilder.SetValue(BaseModifyRecordEntity.FieldIpAddress, UserInfo.IpAddress);
            sqlBuilder.EndInsert();

            return result;
        }
        #endregion

        #region public string GrantByPermissionCode(string systemCode, string userId, string permissionCode) 用户授予权限
        /// <summary>
        /// 用户授予权限
        /// </summary>
        /// <param name="systemCode">系统编号</param>
        /// <param name="userId">用户主键</param>
        /// <param name="permissionCode">权限编号</param>
        public string GrantByPermissionCode(string systemCode, string userId, string permissionCode)
        {
            var result = string.Empty;

            var permissionId = BaseModuleManager.GetIdByCodeByCache(systemCode, permissionCode);
            if (!string.IsNullOrEmpty(permissionId))
            {
                result = Grant(systemCode, userId, permissionId);
            }

            return result;
        }
        #endregion

        /// <summary>
        /// 授权
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="userId"></param>
        /// <param name="permissionIds"></param>
        /// <returns></returns>
        public int Grant(string systemCode, string userId, string[] permissionIds)
        {
            var result = 0;

            for (var i = 0; i < permissionIds.Length; i++)
            {
                Grant(systemCode, userId, permissionIds[i]);
                result++;
            }

            return result;
        }

        /// <summary>
        /// 授权
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="userIds"></param>
        /// <param name="permissionId"></param>
        /// <returns></returns>
        public int Grant(string systemCode, string[] userIds, string permissionId)
        {
            var result = 0;

            for (var i = 0; i < userIds.Length; i++)
            {
                Grant(systemCode, userIds[i], permissionId);
                result++;
            }

            return result;
        }
        
        /// <summary>
        /// 授权
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="userIds"></param>
        /// <param name="permissionIds"></param>
        /// <returns></returns>
        public int Grant(string systemCode, string[] userIds, string[] permissionIds)
        {
            var result = 0;

            for (var i = 0; i < userIds.Length; i++)
            {
                for (var j = 0; j < permissionIds.Length; j++)
                {
                    Grant(systemCode, userIds[i], permissionIds[j]);
                    result++;
                }
            }

            return result;
        }


        //
        //  撤销权限的实现部分
        //

        #region public int Revoke(string systemCode, string userId, string permissionId) 为了提高撤销的运行速度
        /// <summary>
        /// 为了提高撤销的运行速度
        /// </summary>
        /// <param name="systemCode">系统编号</param>
        /// <param name="userId">用户主键</param>
        /// <param name="permissionId">权限主键</param>
        /// <returns>影响行数</returns>
        public int Revoke(string systemCode, string userId, string permissionId)
        {
            var result = 0;

            if (string.IsNullOrEmpty(userId) && string.IsNullOrEmpty(permissionId))
            {
                return result;
            }

            CurrentTableName = systemCode + "Permission";

            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BasePermissionEntity.FieldResourceCategory, BaseUserEntity.TableName),
                new KeyValuePair<string, object>(BasePermissionEntity.FieldResourceId, userId),
                new KeyValuePair<string, object>(BasePermissionEntity.FieldPermissionId, permissionId)
            };
            // 伪删除、数据有冗余，但是有历史记录
            // result = permissionManager.SetDeleted(parameters);
            // 真删除、执行效率高、但是没有历史记录
            result = Delete(parameters);

            // 2015-09-21 吉日嘎拉 这里增加变更日志
            var tableName = systemCode + ".Permission.User";
            var sqlBuilder = new SqlBuilder(DbHelper);
            sqlBuilder.BeginInsert(BaseModifyRecordEntity.TableName);
            sqlBuilder.SetValue(BaseModifyRecordEntity.FieldTableCode, tableName);
            if (DbHelper.CurrentDbType == CurrentDbType.Oracle)
            {
                sqlBuilder.SetFormula(BaseModifyRecordEntity.FieldId, "SEQ_" + BaseModifyRecordEntity.TableName + ".NEXTVAL");
            }
            sqlBuilder.SetValue(BaseModifyRecordEntity.FieldRecordKey, userId);
            sqlBuilder.SetValue(BaseModifyRecordEntity.FieldColumnCode, "撤销授权");
            sqlBuilder.SetValue(BaseModifyRecordEntity.FieldColumnDescription, BaseModuleManager.GetNameByCache(systemCode, permissionId));
            sqlBuilder.SetValue(BaseModifyRecordEntity.FieldOldValue, "1");
            sqlBuilder.SetValue(BaseModifyRecordEntity.FieldNewValue, permissionId);
            sqlBuilder.SetValue(BaseModifyRecordEntity.FieldCreateUserId, UserInfo.Id);
            sqlBuilder.SetValue(BaseModifyRecordEntity.FieldCreateBy, UserInfo.RealName);
            sqlBuilder.SetDbNow(BaseModifyRecordEntity.FieldCreateTime);
            sqlBuilder.SetValue(BaseModifyRecordEntity.FieldIpAddress, UserInfo.IpAddress);
            sqlBuilder.EndInsert();

            return result;
        }
        #endregion

        #region public int RevokeByPermissionCode(string systemCode, string userId, string permissionCode) 用户授予权限
        /// <summary>
        /// 用户授予权限
        /// </summary>
        /// <param name="systemCode">系统编号</param>
        /// <param name="userId">用户主键</param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>影响行数</returns>
        public int RevokeByPermissionCode(string systemCode, string userId, string permissionCode)
        {
            var result = 0;

            var permissionId = BaseModuleManager.GetIdByCodeByCache(systemCode, permissionCode);
            if (!string.IsNullOrEmpty(permissionId))
            {
                result = Revoke(systemCode, userId, permissionId);
            }

            return result;
        }
        #endregion

        /// <summary>
        /// 撤回
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="userId"></param>
        /// <param name="permissionIds"></param>
        /// <returns></returns>
        public int Revoke(string systemCode, string userId, string[] permissionIds)
        {
            var result = 0;

            for (var i = 0; i < permissionIds.Length; i++)
            {
                result += Revoke(systemCode, userId, permissionIds[i]);
            }

            return result;
        }

        /// <summary>
        /// 撤回
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="userIds"></param>
        /// <param name="permissionId"></param>
        /// <returns></returns>
        public int Revoke(string systemCode, string[] userIds, string permissionId)
        {
            var result = 0;

            for (var i = 0; i < userIds.Length; i++)
            {
                result += Revoke(systemCode, userIds[i], permissionId);
            }

            return result;
        }
        /// <summary>
        /// 撤回
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="userIds"></param>
        /// <param name="permissionIds"></param>
        /// <returns></returns>
        public int Revoke(string systemCode, string[] userIds, string[] permissionIds)
        {
            var result = 0;

            for (var i = 0; i < userIds.Length; i++)
            {
                for (var j = 0; j < permissionIds.Length; j++)
                {
                    result += Revoke(systemCode, userIds[i], permissionIds[j]);
                }
            }

            return result;
        }

        /// <summary>
        /// 撤回所有
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public int RevokeAll(string systemCode, string userId)
        {
            var result = 0;

            CurrentTableName = systemCode + "Permission";
            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BasePermissionEntity.FieldResourceCategory, BaseUserEntity.TableName),
                new KeyValuePair<string, object>(BasePermissionEntity.FieldResourceId, userId)
            };
            result = Delete(parameters);

            return result;
        }
    }
}