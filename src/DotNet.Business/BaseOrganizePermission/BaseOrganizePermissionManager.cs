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
    /// BaseOrganizePermissionManager
    /// 组织机构权限
    /// 
    /// 修改记录
    ///
    ///     2012.03.22 版本：1.0 JiRiGaLa 创建主键。
    ///
    /// <author>
    ///		<name>JiRiGaLa</name>
    ///		<date>2012.03.22</date>
    /// </author>
    /// </summary>
    public partial class BaseOrganizePermissionManager : BaseManager, IBaseManager
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public BaseOrganizePermissionManager()
        {
            CurrentTableName = BasePermissionEntity.TableName;
        }

        /// <summary>
        /// 构造函数
        /// <param name="tableName">指定表名</param>
        /// </summary>
        public BaseOrganizePermissionManager(string tableName)
        {
            CurrentTableName = tableName;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        public BaseOrganizePermissionManager(IDbHelper dbHelper)
            : this()
        {
            DbHelper = dbHelper;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        public BaseOrganizePermissionManager(BaseUserInfo userInfo)
            : this()
        {
            UserInfo = userInfo;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        /// <param name="userInfo">用户信息</param>
        public BaseOrganizePermissionManager(IDbHelper dbHelper, BaseUserInfo userInfo)
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
        public BaseOrganizePermissionManager(IDbHelper dbHelper, BaseUserInfo userInfo, string tableName)
            : this(dbHelper, userInfo)
        {
            CurrentTableName = tableName;
        }

        /// <summary>
        /// 重置缓存
        /// </summary>
        /// <param name="systemCode"></param>
        /// <param name="organizeId"></param>
        /// <returns></returns>
        public static string[] ResetPermissionByCache(string systemCode, string organizeId)
        {
            var key = "Permission:" + systemCode + ":Organize:" + organizeId;
            CacheUtil.Remove(key);
            return GetPermissionIdsByCache(systemCode, organizeId);
        }

        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="systemCode"></param>
        /// <param name="organizeId"></param>
        /// <returns></returns>
        public static string[] GetPermissionIdsByCache(string systemCode, string organizeId)
        {
            string[] result = null;

            var key = string.Empty;
            key = "Permission:" + systemCode + ":Organize:" + organizeId;
            result = CacheUtil.Cache(key, () => new BaseOrganizePermissionManager().GetPermissionIds(organizeId), true);
            return result;
        }

        #region public string[] GetPermissionIds(string organizeId) 获取组织机构的权限主键数组
        /// <summary>
        /// 获取组织机构的权限主键数组
        /// </summary>
        /// <param name="organizeId">组织机构主键</param>
        /// <returns>主键数组</returns>
        public string[] GetPermissionIds(string organizeId)
        {
            string[] result = null;

            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BasePermissionEntity.FieldResourceCategory, BaseOrganizeEntity.TableName),
                new KeyValuePair<string, object>(BasePermissionEntity.FieldResourceId, organizeId),
                new KeyValuePair<string, object>(BasePermissionEntity.FieldEnabled, 1),
                new KeyValuePair<string, object>(BasePermissionEntity.FieldDeleted, 0)
            };
            result = GetProperties(parameters, BasePermissionEntity.FieldPermissionId);

            return result;
        }
        #endregion

        #region public string[] GetOrganizeIds(string result) 获取组织机构主键数组
        /// <summary>
        /// 获取组织机构主键数组
        /// </summary>
        /// <param name="permissionId">操作权限</param>
        /// <returns>主键数组</returns>
        public string[] GetOrganizeIds(string permissionId)
        {
            string[] result = null;

            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BasePermissionEntity.FieldResourceCategory, BaseOrganizeEntity.TableName),
                new KeyValuePair<string, object>(BasePermissionEntity.FieldPermissionId, permissionId),
                new KeyValuePair<string, object>(BasePermissionEntity.FieldEnabled, 1),
                new KeyValuePair<string, object>(BasePermissionEntity.FieldDeleted, 0)
            };

            result = GetProperties(parameters, BasePermissionEntity.FieldResourceId);
            return result;
        }
        #endregion

        //
        // 授予权限的实现部分
        //

        #region private string Grant(BasePermissionManager permissionManager, string systemCode, string organizeId, string result, bool chekExists = true) 为了提高授权的运行速度
        /// <summary>
        /// 为了提高授权的运行速度
        /// </summary>
        /// <param name="permissionManager">资源权限读写器</param>
        /// <param name="systemCode">系统编码</param>
        /// <param name="organizeId">组织机构主键</param>
        /// <param name="permissionId">权限主键</param>
        /// <param name="chekExists">判断是否存在</param>
        /// <returns>主键</returns>
        private string Grant(BasePermissionManager permissionManager, string systemCode, string organizeId, string permissionId, bool chekExists = true)
        {
            var result = string.Empty;

            var currentId = string.Empty;
            // 判断是否已经存在这个权限，若已经存在就不重复增加了
            if (chekExists)
            {
                var whereParameters = new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>(BasePermissionEntity.FieldResourceCategory, BaseOrganizeEntity.TableName),
                    new KeyValuePair<string, object>(BasePermissionEntity.FieldResourceId, organizeId),
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
                var resourcePermission = new BasePermissionEntity
                {
                    ResourceCategory = BaseOrganizeEntity.TableName,
                    ResourceId = organizeId,
                    PermissionId = permissionId,
                    // 防止不允许为NULL的错误发生
                    Enabled = 1,
                    DeletionStateCode = 0
                };
                result = permissionManager.Add(resourcePermission, true, false);
            }

            if (string.IsNullOrEmpty(systemCode))
            {
                if (UserInfo != null && !string.IsNullOrWhiteSpace(UserInfo.SystemCode))
                {
                    systemCode = UserInfo.SystemCode;
                }
            }
            if (string.IsNullOrEmpty(systemCode))
            {
                systemCode = "Base";
            }

            // 2015-09-21 吉日嘎拉 这里增加变更日志
            var tableName = systemCode + ".Permission.Organize";
            var sqlBuilder = new SqlBuilder(DbHelper);
            sqlBuilder.BeginInsert(BaseModifyRecordEntity.TableName);
            sqlBuilder.SetValue(BaseModifyRecordEntity.FieldTableCode, tableName);
            if (DbHelper.CurrentDbType == CurrentDbType.Oracle)
            {
                sqlBuilder.SetFormula(BaseModifyRecordEntity.FieldId, "SEQ_" + BaseModifyRecordEntity.TableName + ".NEXTVAL");
            }
            sqlBuilder.SetValue(BaseModifyRecordEntity.FieldRecordKey, organizeId);
            sqlBuilder.SetValue(BaseModifyRecordEntity.FieldColumnCode, permissionId);
            sqlBuilder.SetValue(BaseModifyRecordEntity.FieldColumnDescription, BaseModuleManager.GetNameByCache(systemCode, permissionId));
            sqlBuilder.SetValue(BaseModifyRecordEntity.FieldOldValue, null);
            sqlBuilder.SetValue(BaseModifyRecordEntity.FieldNewValue, "授权");
            sqlBuilder.SetValue(BaseModifyRecordEntity.FieldCreateUserId, UserInfo.Id);
            sqlBuilder.SetValue(BaseModifyRecordEntity.FieldCreateBy, UserInfo.RealName);
            sqlBuilder.SetDbNow(BaseModifyRecordEntity.FieldCreateTime);
            sqlBuilder.SetValue(BaseModifyRecordEntity.FieldIpAddress, UserInfo.IpAddress);
            sqlBuilder.EndInsert();

            return result;
        }
        #endregion

        #region public string Grant(string organizeId, string result) 组织机构授予权限
        /// <summary>
        /// 组织机构授予权限
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="organizeId">组织机构主键</param>
        /// <param name="permissionId">权限主键</param>
        public string Grant(string systemCode, string organizeId, string permissionId)
        {
            var permissionManager = new BasePermissionManager(DbHelper, UserInfo, CurrentTableName);
            return Grant(permissionManager, systemCode, organizeId, permissionId);
        }
        #endregion

        /// <summary>
        /// 授权
        /// </summary>
        /// <param name="systemCode"></param>
        /// <param name="organizeId"></param>
        /// <param name="permissionIds"></param>
        /// <returns></returns>
        public int Grant(string systemCode, string organizeId, string[] permissionIds)
        {
            var result = 0;
            var permissionManager = new BasePermissionManager(DbHelper, UserInfo, CurrentTableName);
            for (var i = 0; i < permissionIds.Length; i++)
            {
                Grant(permissionManager, systemCode, organizeId, permissionIds[i]);
                result++;
            }
            return result;
        }

        /// <summary>
        /// 授权
        /// </summary>
        /// <param name="systemCode"></param>
        /// <param name="organizeIds"></param>
        /// <param name="permissionId"></param>
        /// <returns></returns>
        public int Grant(string systemCode, string[] organizeIds, string permissionId)
        {
            var result = 0;

            var permissionManager = new BasePermissionManager(DbHelper, UserInfo, CurrentTableName);
            for (var i = 0; i < organizeIds.Length; i++)
            {
                Grant(permissionManager, systemCode, organizeIds[i], permissionId);
                result++;
            }

            return result;
        }

        /// <summary>
        /// 授权
        /// </summary>
        /// <param name="systemCode"></param>
        /// <param name="organizeIds"></param>
        /// <param name="permissionIds"></param>
        /// <returns></returns>
        public int Grant(string systemCode, string[] organizeIds, string[] permissionIds)
        {
            var result = 0;

            var permissionManager = new BasePermissionManager(DbHelper, UserInfo, CurrentTableName);
            for (var i = 0; i < organizeIds.Length; i++)
            {
                for (var j = 0; j < permissionIds.Length; j++)
                {
                    Grant(permissionManager, systemCode, organizeIds[i], permissionIds[j]);
                    result++;
                }
            }

            return result;
        }


        //
        //  撤销权限的实现部分
        //

        #region private int Revoke(BasePermissionManager permissionManager, string systemCode, string organizeId, string result) 为了提高撤销的运行速度
        /// <summary>
        /// 为了提高撤销的运行速度
        /// </summary>
        /// <param name="permissionManager">资源权限读写器</param>
        /// <param name="systemCode">系统编码</param>
        /// <param name="organizeId">组织机构主键</param>
        /// <param name="permissionId">权限主键</param>
        /// <returns>影响行数</returns>
        private int Revoke(BasePermissionManager permissionManager, string systemCode, string organizeId, string permissionId)
        {
            var result = 0;

            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BasePermissionEntity.FieldResourceCategory, BaseOrganizeEntity.TableName),
                new KeyValuePair<string, object>(BasePermissionEntity.FieldResourceId, organizeId),
                new KeyValuePair<string, object>(BasePermissionEntity.FieldPermissionId, permissionId)
            };
            result = permissionManager.Delete(parameters);

            // 2015-09-21 吉日嘎拉 这里增加变更日志
            var tableName = systemCode + ".Permission.Organize";
            var sqlBuilder = new SqlBuilder(DbHelper);
            sqlBuilder.BeginInsert(BaseModifyRecordEntity.TableName);
            sqlBuilder.SetValue(BaseModifyRecordEntity.FieldTableCode, tableName);
            sqlBuilder.SetFormula(BaseModifyRecordEntity.FieldId, "SEQ_" + BaseModifyRecordEntity.TableName + ".NEXTVAL");
            sqlBuilder.SetValue(BaseModifyRecordEntity.FieldRecordKey, organizeId);
            sqlBuilder.SetValue(BaseModifyRecordEntity.FieldColumnCode, permissionId);
            sqlBuilder.SetValue(BaseModifyRecordEntity.FieldColumnDescription, BaseModuleManager.GetNameByCache(systemCode, permissionId));
            sqlBuilder.SetValue(BaseModifyRecordEntity.FieldOldValue, "1");
            sqlBuilder.SetValue(BaseModifyRecordEntity.FieldNewValue, "撤销授权");
            sqlBuilder.SetValue(BaseModifyRecordEntity.FieldCreateUserId, UserInfo.Id);
            sqlBuilder.SetValue(BaseModifyRecordEntity.FieldCreateBy, UserInfo.RealName);
            sqlBuilder.SetDbNow(BaseModifyRecordEntity.FieldCreateTime);
            sqlBuilder.SetValue(BaseModifyRecordEntity.FieldIpAddress, UserInfo.IpAddress);
            sqlBuilder.EndInsert();

            return result;
        }
        #endregion

        #region public int Revoke(string systemCode, string organizeId, string result) 撤销组织机构权限
        /// <summary>
        /// 撤销组织机构权限
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="organizeId">组织机构主键</param>
        /// <param name="permissionId">权限主键</param>
        /// <returns>影响行数</returns>
        public int Revoke(string systemCode, string organizeId, string permissionId)
        {
            var permissionManager = new BasePermissionManager(DbHelper, UserInfo, CurrentTableName);
            return Revoke(permissionManager, systemCode, organizeId, permissionId);
        }
        #endregion

        /// <summary>
        /// 撤回
        /// </summary>
        /// <param name="systemCode"></param>
        /// <param name="organizeId"></param>
        /// <param name="permissionIds"></param>
        /// <returns></returns>
        public int Revoke(string systemCode, string organizeId, string[] permissionIds)
        {
            var result = 0;
            var permissionManager = new BasePermissionManager(DbHelper, UserInfo, CurrentTableName);
            for (var i = 0; i < permissionIds.Length; i++)
            {
                result += Revoke(permissionManager, systemCode, organizeId, permissionIds[i]);
            }
            return result;
        }

        /// <summary>
        /// 撤回
        /// </summary>
        /// <param name="systemCode"></param>
        /// <param name="organizeIds"></param>
        /// <param name="permissionId"></param>
        /// <returns></returns>
        public int Revoke(string systemCode, string[] organizeIds, string permissionId)
        {
            var result = 0;
            var permissionManager = new BasePermissionManager(DbHelper, UserInfo, CurrentTableName);
            for (var i = 0; i < organizeIds.Length; i++)
            {
                result += Revoke(permissionManager, systemCode, organizeIds[i], permissionId);
            }
            return result;
        }

        /// <summary>
        /// 撤回
        /// </summary>
        /// <param name="systemCode"></param>
        /// <param name="organizeIds"></param>
        /// <param name="permissionIds"></param>
        /// <returns></returns>
        public int Revoke(string systemCode, string[] organizeIds, string[] permissionIds)
        {
            var result = 0;

            var permissionManager = new BasePermissionManager(DbHelper, UserInfo, CurrentTableName);
            for (var i = 0; i < organizeIds.Length; i++)
            {
                for (var j = 0; j < permissionIds.Length; j++)
                {
                    result += Revoke(permissionManager, systemCode, organizeIds[i], permissionIds[j]);
                }
            }

            return result;
        }

        #region public int RevokeAll(string organizeId) 撤销组织机构全部权限
        /// <summary>
        /// 撤销组织机构全部权限
        /// </summary>
        /// <param name="organizeId">组织机构主键</param>
        /// <returns>影响行数</returns>
        public int RevokeAll(string organizeId)
        {
            var permissionManager = new BasePermissionManager(DbHelper, UserInfo, CurrentTableName);
            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BasePermissionEntity.FieldResourceCategory, BaseOrganizeEntity.TableName),
                new KeyValuePair<string, object>(BasePermissionEntity.FieldResourceId, organizeId)
            };
            return permissionManager.Delete(parameters);
        }
        #endregion
    }
}