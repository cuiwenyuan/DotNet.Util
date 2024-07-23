//-----------------------------------------------------------------------
// <copyright file="BaseUserRoleManager.cs" company="DotNet">
//     Copyright (c) 2024, All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Data;

namespace DotNet.Business
{
    using Model;
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
                    Status = Status.OkAdd;
                    StatusCode = Status.OkAdd.ToString();
                    StatusMessage = Status.OkAdd.ToDescription();
                }
                else
                {
                    //保存失败
                    Status = Status.DbError;
                    StatusCode = Status.DbError.ToString();
                    StatusMessage = Status.DbError.ToDescription();
                }
            }

            return result;
        }
        #endregion

        #region 复制用户角色到新用户
        /// <summary>
        /// 复制用户角色到新用户
        /// </summary>
        /// <param name="referenceUserId">源用户编号</param>
        /// <param name="targetUserId">目标用户编号</param>
        /// <returns></returns>
        public int CopyRole(string systemCode, int referenceUserId, int targetUserId)
        {
            var result = 0;
            if (string.IsNullOrEmpty(systemCode))
            {
                systemCode = "Base";
            }
            var whereParameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseUserRoleEntity.FieldSystemCode, systemCode),
                new KeyValuePair<string, object>(BaseUserRoleEntity.FieldUserId, referenceUserId),
                new KeyValuePair<string, object>(BaseUserRoleEntity.FieldEnabled, 1),
                new KeyValuePair<string, object>(BaseUserRoleEntity.FieldDeleted, 0),
            };
            var tableName = GetUserRoleTableName(systemCode);
            var manager = new BaseUserRoleManager(DbHelper, UserInfo, tableName);
            var ls = manager.GetList<BaseUserRoleEntity>(whereParameters, order: BaseUserRoleEntity.FieldCreateTime + " ASC");
            if (ls != null)
            {
                foreach (var item in ls)
                {
                    item.UserId = targetUserId;
                    if (!manager.AddOrActive(item).IsNullOrEmpty())
                    {
                        result++;
                    }
                }
                //运行成功
                Status = Status.OkAdd;
                StatusCode = Status.OkAdd.ToString();
                StatusMessage = Status.OkAdd.ToDescription();
            }
            else
            {
                //未找到记录
                Status = Status.NotFound;
                StatusCode = Status.NotFound.ToString();
                StatusMessage = Status.NotFound.ToDescription();
            }

            return result;
        }
        #endregion
    }
}
