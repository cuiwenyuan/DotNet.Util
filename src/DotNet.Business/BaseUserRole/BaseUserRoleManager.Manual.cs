//-----------------------------------------------------------------------
// <copyright file="BaseUserRoleManager.cs" company="DotNet">
//     Copyright (c) 2021, All rights reserved.
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
    /// 用户角色
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
    public partial class BaseUserRoleManager : BaseManager, IBaseManager
    {
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
