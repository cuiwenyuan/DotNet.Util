//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2023, DotNet.
//-----------------------------------------------------------------

using System.Collections.Generic;
using System.Data;
using System;

namespace DotNet.Business
{
    using Util;
    using Model;


    /// <summary>
    /// BaseOrganizationManager（程序OK）
    /// 组织机构
    ///
    /// 修改记录
    ///     
    ///		2015.12.08 版本：1.1 JiRiGaLa	参数顺序更换。
    ///		2014.04.15 版本：1.0 JiRiGaLa	有些思想进行了改进。
    ///
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2015.12.08</date>
    /// </author>
    /// </summary>
    public partial class BaseOrganizationManager : BaseManager //, IBaseOrganizationManager
    {
        #region public string[] GetIdsInRole(string systemCode, string roleId) 按角色主键获得组织机构主键数组
        /// <summary>
        /// 按角色主键获得组织机构主键数组
        /// </summary>
        /// <param name="systemCode">系统编号</param>
        /// <param name="roleId">角色主键</param>
        /// <returns>主键数组</returns>
        public string[] GetIdsInRole(string systemCode, string roleId)
        {
            string[] result = null;

            var tableName = systemCode + "RoleOrganization";

            // 需要显示未被删除的用户
            var sb = PoolUtil.StringBuilder.Get();
            sb.Append("SELECT OrganizationId FROM " + tableName
                            + " WHERE RoleId = " + DbHelper.GetParameter(BaseRoleOrganizationEntity.FieldRoleId)
                                  + " AND " + BaseOrganizationEntity.FieldDeleted + " = 0 "
                                  + " AND OrganizationId IN (SELECT Id FROM BaseOrganization WHERE " + BaseOrganizationEntity.FieldDeleted + " = 0)");

            var dbParameters = new List<IDbDataParameter>
            {
                DbHelper.MakeParameter(BaseRoleOrganizationEntity.FieldRoleId, roleId)
            };

            var organizationIds = new List<string>();
            var dataReader = DbHelper.ExecuteReader(sb.Return(), dbParameters.ToArray());
            if (dataReader != null && !dataReader.IsClosed)
            {
                while (dataReader.Read())
                {
                    organizationIds.Add(dataReader[BaseRoleOrganizationEntity.FieldOrganizationId].ToString());
                }

                dataReader.Close();
            }

            result = organizationIds.ToArray();

            // 2015-12-08 吉日嘎拉 提高效率参数化执行
            // var dt = DbHelper.Fill(sql, dbParameters.ToArray());
            // BaseUtil.FieldToArray(dt, BaseRoleOrganizationEntity.FieldOrganizationId).Distinct<string>().Where(t => !string.IsNullOrEmpty(t)).ToArray();

            return result;
        }
        #endregion

        string GetSqlQueryByRole(string systemCode, string[] roleIds)
        {
            var tableNameRoleOrganization = systemCode + "RoleOrganization";
            var sb = PoolUtil.StringBuilder.Get();
            sb.Append("SELECT * FROM " + BaseOrganizationEntity.CurrentTableName
                            + " WHERE " + BaseOrganizationEntity.FieldEnabled + " = 1 "
                            + " AND " + BaseOrganizationEntity.FieldDeleted + "= 0 "
                            + " AND ( " + BaseOrganizationEntity.FieldId + " IN "
                            + " (SELECT  " + BaseRoleOrganizationEntity.FieldOrganizationId
                            + " FROM " + tableNameRoleOrganization
                            + " WHERE " + BaseRoleOrganizationEntity.FieldRoleId + " IN (" + StringUtil.ArrayToList(roleIds) + ")"
                            + " AND " + BaseRoleOrganizationEntity.FieldEnabled + " = 1"
                            + " AND " + BaseRoleOrganizationEntity.FieldDeleted + " = 0)) "
                            + " ORDER BY  " + BaseOrganizationEntity.FieldSortCode);

            return sb.Return();
        }

        /// <summary>
        /// 获取数据表
        /// </summary>
        /// <param name="systemCode"></param>
        /// <param name="roleIds"></param>
        /// <returns></returns>
        public DataTable GetDataTableByRole(string systemCode, string[] roleIds)
        {
            return DbHelper.Fill(GetSqlQueryByRole(systemCode, roleIds));
        }

        /// <summary>
        /// 为了提高授权的运行速度
        /// </summary>
        /// <param name="systemCode">系统编号</param>
        /// <param name="organizationId">组织机构主键</param>
        /// <param name="roleId">角色主键</param>
        /// <returns>主键</returns>
        public string AddToRole(string systemCode, string organizationId, string roleId)
        {
            var result = string.Empty;

            var entity = new BaseRoleOrganizationEntity
            {
                OrganizationId = organizationId.ToInt(),
                RoleId = roleId.ToInt(),
                Enabled = 1,
                Deleted = 0
            };
            var tableName = systemCode + "RoleOrganization";
            var manager = new BaseRoleOrganizationManager(DbHelper, UserInfo, tableName);
            return manager.Add(entity);
        }

        /// <summary>
        /// 添加到角色
        /// </summary>
        /// <param name="systemCode"></param>
        /// <param name="organizationIds"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public int AddToRole(string systemCode, string[] organizationIds, string roleId)
        {
            var result = 0;

            for (var i = 0; i < organizationIds.Length; i++)
            {
                AddToRole(systemCode, organizationIds[i], roleId);
                result++;
            }

            return result;
        }

        /// <summary>
        /// 移除角色成功
        /// </summary>
        /// <param name="systemCode">系统编号</param>
        /// <param name="organizationId">组织机构主键</param>
        /// <param name="roleId">角色主键</param>
        /// <returns>影响行数</returns>
        public int RemoveFromRole(string systemCode, string organizationId, string roleId)
        {
            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseRoleOrganizationEntity.FieldRoleId, roleId),
                new KeyValuePair<string, object>(BaseRoleOrganizationEntity.FieldOrganizationId, organizationId)
            };
            var tableName = systemCode + "RoleOrganization";
            var manager = new BaseRoleOrganizationManager(DbHelper, UserInfo, tableName);
            return manager.Delete(parameters);
        }
        /// <summary>
        /// 从角色中删除
        /// </summary>
        /// <param name="systemCode"></param>
        /// <param name="organizationIds"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public int RemoveFromRole(string systemCode, string[] organizationIds, string roleId)
        {
            var result = 0;

            for (var i = 0; i < organizationIds.Length; i++)
            {
                // 移除用户角色
                result += RemoveFromRole(systemCode, organizationIds[i], roleId);
            }

            return result;
        }

        /// <summary>
        /// 清空组织机构
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public int ClearOrganization(string systemCode, string roleId)
        {
            var result = 0;

            var tableName = systemCode + "RoleOrganization";
            var manager = new BaseRoleOrganizationManager(DbHelper, UserInfo, tableName);
            result += manager.Delete(new List<KeyValuePair<string, object>> { new KeyValuePair<string, object>(BaseRoleOrganizationEntity.FieldRoleId, roleId) });

            return result;
        }
    }
}