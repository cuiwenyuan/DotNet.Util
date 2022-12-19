//-----------------------------------------------------------------------
// <copyright file="BaseUserRoleManager.cs" company="DotNet">
//     Copyright (c) 2022, All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace DotNet.Business
{
    using DotNet.Model;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using Util;

    /// <summary>
    /// BaseUserRoleManager
    /// 用户角色管理层
    /// 
    /// 修改记录
    ///
    ///     2021-01-12 版本：5.1 Troy.Cui   增加AddOrUpdate。
    ///
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2021-01-12</date>
    /// </author>
    /// </summary>
    public partial class BaseUserRoleManager : BaseManager
    {
        #region 删除缓存

        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <returns></returns>
        public override bool RemoveCache()
        {
            var result = false;
            var cacheKey = "DataTable." + CurrentTableName;
            var cacheKeyListBase = "List.Base.UserRole";
            var cacheKeyListSystemCode = "List.UserBase.Role";
            var cacheKeySystemCode = "DataTable.Base.UserRole";
            var cacheKeySystemCodeUserId = "DataTable.";
            if (UserInfo != null)
            {
                //cacheKey += "." + UserInfo.CompanyId;
                cacheKeyListSystemCode = "List." + UserInfo.SystemCode + ".UserRole";
                cacheKeySystemCode = "DataTable." + UserInfo.SystemCode + ".UserRole";
                cacheKeySystemCodeUserId += UserInfo.SystemCode + "." + UserInfo.Id + ".UserRole";
            }
            CacheUtil.Remove(cacheKeyListBase);
            CacheUtil.Remove(cacheKeyListSystemCode);
            CacheUtil.Remove(cacheKeySystemCode);
            CacheUtil.Remove(cacheKeySystemCodeUserId);
            result = CacheUtil.Remove(cacheKey);
            return result;
        }
        #endregion

        #region public string Add(BaseUserRoleEntity entity) 添加(判断数据是否重复，防止垃圾数据产生)

        /// <summary>
        /// 添加(判断数据是否重复，防止垃圾数据产生)
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns>主键</returns>
        public string Add(BaseUserRoleEntity entity)
        {
            var result = string.Empty;

            // 判断是否数据重复
            var whereParameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseUserRoleEntity.FieldSystemCode, entity.SystemCode),
                new KeyValuePair<string, object>(BaseUserRoleEntity.FieldEnabled, 1),
                new KeyValuePair<string, object>(BaseUserRoleEntity.FieldDeleted, 0),
                new KeyValuePair<string, object>(BaseUserRoleEntity.FieldRoleId, entity.RoleId),
                new KeyValuePair<string, object>(BaseUserRoleEntity.FieldUserId, entity.UserId)
            };
            if (!Exists(whereParameters))
            {
                result = AddEntity(entity);
            }
            else
            {
                var parameters = new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>(BaseUserRoleEntity.FieldEnabled, 1),
                    new KeyValuePair<string, object>(BaseUserRoleEntity.FieldDeleted, 0)
                };
                Update(whereParameters, parameters);
            }

            return result;
        }

        #endregion

        #region 新增或激活 AddOrActive
        /// <summary>
        /// 新增或激活
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        public string AddOrActive(BaseUserRoleEntity entity)
        {
            var result = string.Empty;
            var whereParameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseUserRoleEntity.FieldSystemCode, entity.SystemCode),
                new KeyValuePair<string, object>(BaseUserRoleEntity.FieldRoleId, entity.RoleId),
                new KeyValuePair<string, object>(BaseUserRoleEntity.FieldUserId, entity.UserId)
            };
            var entityOld = GetEntity(whereParameters);
            if (entityOld != null)
            {
                result = entityOld.Id.ToString();
                entity.Id = entityOld.Id;
                entity.Enabled = 1;
                entity.Deleted = 0;
                //激活
                UpdateEntity(entity);
            }
            else
            {
                result = AddEntity(entity);
                if (!string.IsNullOrEmpty(result))
                {
                    //运行成功
                    StatusCode = Status.OkAdd.ToString();
                    StatusMessage = Status.OkAdd.ToDescription();
                }
                else
                {
                    //保存失败
                    StatusCode = Status.DbError.ToString();
                    StatusMessage = Status.DbError.ToDescription();
                }
            }

            return result;
        }
        #endregion
    }
}
