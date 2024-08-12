//-----------------------------------------------------------------------
// <copyright file="BaseUserOAuthManager.cs" company="DotNet">
//     Copyright (c) 2024, All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace DotNet.Business
{
    using Model;
    using Util;

    /// <summary>
    /// BaseUserOpenAuthManager
    /// 用户OAuth管理层
    /// 
    /// 修改记录
    /// 
    ///	2020-02-12 版本：1.0 Troy.Cui 创建文件。
    ///		
    /// <author>
    ///	<name>Troy.Cui</name>
    ///	<date>2020-02-12</date>
    /// </author> 
    /// </summary>
    public partial class BaseUserOAuthManager : BaseManager
    {
        #region 是否唯一的

        /// <summary>
        /// 是否唯一的
        /// </summary>
        /// <param name="name">OAuth Name</param>
        /// <param name="unionId">OAuth UnionId</param>
        /// <param name="openId">OAuth OpenId</param>
        /// <param name="excludeId">排除行id</param>
        /// <returns>是否</returns>
        public bool IsUnique(string name, string openId, string unionId, string excludeId = null)
        {
            var result = false;
            //安全过滤一下
            name = dbHelper.SqlSafe(name);
            openId = dbHelper.SqlSafe(openId);
            unionId = dbHelper.SqlSafe(unionId);
            var sb = PoolUtil.StringBuilder.Get();
            sb.Append("SELECT COUNT(*) FROM " + CurrentTableName + " WHERE " + BaseUserOAuthEntity.FieldName + " = N'" + name + "'");
            sb.Append(" AND " + BaseUserOAuthEntity.FieldOpenId + " = N'" + openId + "'");
            sb.Append(" AND " + BaseUserOAuthEntity.FieldUnionId + " = N'" + unionId + "'");
            //未删除
            sb.Append(" AND " + BaseUserOAuthEntity.FieldDeleted + " = 0 AND " + BaseUserOAuthEntity.FieldEnabled + " = 1");
            //当前用户所在公司或者系统公用数据
            //sb.Append(" AND (" + BaseUserOAuthEntity.FieldUserCompanyId + " = 0 OR " + BaseUserOAuthEntity.FieldUserCompanyId + " = " + UserInfo.CompanyId + ")");
            if (ValidateUtil.IsInt(excludeId))
            {
                sb.Append(" AND Id <> " + excludeId);
            }
            //需要显示未被删除的记录
            var obj = ExecuteScalar(sb.Return());
            if (obj != null && obj.ToInt() == 0)
            {
                result = true;
            }
            return result;
        }

        /// <summary>
        /// 是否唯一的
        /// </summary>
        /// <param name="name">OAuth Name</param>
        /// <param name="userId">用户编号</param>
        /// <returns>是否</returns>
        public bool IsUnique(string name, string userId)
        {
            var result = false;
            //安全过滤一下
            name = dbHelper.SqlSafe(name);
            var sb = PoolUtil.StringBuilder.Get();
            sb.Append("SELECT COUNT(*) FROM " + CurrentTableName + " WHERE " + BaseUserOAuthEntity.FieldName + " = N'" + name + "'");
            sb.Append(" AND " + BaseUserOAuthEntity.FieldUserId + " = " + userId + "");
            //未删除
            sb.Append(" AND " + BaseUserOAuthEntity.FieldDeleted + " = 0 AND " + BaseUserOAuthEntity.FieldEnabled + " = 1");
            //当前用户所在公司或者系统公用数据
            //sb.Append(" AND (" + BaseUserOpenAuthEntity.FieldUserCompanyId + " = 0 OR " + BaseUserOpenAuthEntity.FieldUserCompanyId + " = " + UserInfo.CompanyId + ")");
            //需要显示未被删除的记录
            var obj = ExecuteScalar(sb.Return());
            if (obj != null && obj.ToInt() == 0)
            {
                result = true;
            }
            return result;
        }
        #endregion

        #region 获取实体
        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="openId">OpenId</param>
        /// <param name="unionId">UnionId</param>
        /// <param name="systemCode">子系统编码</param>
        /// <returns></returns>
        public BaseUserOAuthEntity GetEntity(string name, string openId, string unionId, string systemCode)
        {
            BaseUserOAuthEntity result = null;
            if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(openId))
            {
                name = dbHelper.SqlSafe(name);
                var sb = PoolUtil.StringBuilder.Get();
                //需要显示未被删除的记录
                sb.Append("SELECT TOP 1 * FROM " + CurrentTableName + " WHERE " + BaseUserOAuthEntity.FieldName + " = N'" + name + "'");
                if (!string.IsNullOrEmpty(openId))
                {
                    sb.Append(" AND " + BaseUserOAuthEntity.FieldOpenId + " = N'" + openId + "'");
                }

                if (!string.IsNullOrEmpty(unionId))
                {
                    sb.Append(" AND " + BaseUserOAuthEntity.FieldUnionId + " = N'" + unionId + "'");
                }

                //未删除
                sb.Append(" AND " + BaseUserOAuthEntity.FieldDeleted + " = 0 AND " + BaseUserOAuthEntity.FieldEnabled + " = 1");
                //排序
                sb.Append(" ORDER BY " + BaseUserOAuthEntity.FieldId + " DESC");
                var dt = DbHelper.Fill(sb.Return());
                if (dt != null && dt.Rows.Count != 0)
                {
                    //result = BaseEntity.Create<AppContentEntity>(dt);
                    var cacheKey = CurrentTableName + ".Entity." + openId;
                    var cacheTime = TimeSpan.FromMilliseconds(86400000);
                    result = CacheUtil.Cache<BaseUserOAuthEntity>(cacheKey, () => BaseEntity.Create<BaseUserOAuthEntity>(dt), true, false, cacheTime);
                }
            }
            return result;
        }
        #endregion
    }
}
