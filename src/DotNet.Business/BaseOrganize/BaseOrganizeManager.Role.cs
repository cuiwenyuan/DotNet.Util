//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System.Collections.Generic;
using System.Data;

namespace DotNet.Business
{
    using Model;

    /// <summary>
    /// BaseOrganizeManager（程序OK）
    /// 组织机构、部门表
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
    public partial class BaseOrganizeManager : BaseManager //, IBaseOrganizeManager
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

            var tableName = systemCode + "RoleOrganize";

            // 需要显示未被删除的用户
            var sql = "SELECT OrganizeId FROM " + tableName 
                            + " WHERE RoleId = " + DbHelper.GetParameter(BaseRoleOrganizeEntity.FieldRoleId) 
                                  + " AND " + BaseOrganizeEntity.FieldDeleted + " = 0 "
                                  + " AND OrganizeId IN (SELECT Id FROM BaseOrganize WHERE " + BaseOrganizeEntity.FieldDeleted + " = 0)";

            var dbParameters = new List<IDbDataParameter>
            {
                DbHelper.MakeParameter(BaseRoleOrganizeEntity.FieldRoleId, roleId)
            };

            var organizeIds = new List<string>();
            using (var dataReader = DbHelper.ExecuteReader(sql, dbParameters.ToArray()))
            {
                while (dataReader.Read())
                {
                    organizeIds.Add(dataReader[BaseRoleOrganizeEntity.FieldOrganizeId].ToString());
                }
            }
            result = organizeIds.ToArray();

            // 2015-12-08 吉日嘎拉 提高效率参数化执行
            // var dt = DbHelper.Fill(sql, dbParameters.ToArray());
            // BaseUtil.FieldToArray(dt, BaseRoleOrganizeEntity.FieldOrganizeId).Distinct<string>().Where(t => !string.IsNullOrEmpty(t)).ToArray();
            
            return result;
        }
        #endregion

        string GetSqlQueryByRole(string systemCode, string[] roleIds)
        {
            var tableNameRoleOrganize = systemCode + "RoleOrganize";

            var sql = "SELECT * FROM " + BaseOrganizeEntity.TableName
                            + " WHERE " + BaseOrganizeEntity.FieldEnabled + " = 1 "
                            + " AND " + BaseOrganizeEntity.FieldDeleted + "= 0 "
                            + " AND ( " + BaseOrganizeEntity.FieldId + " IN "
                            + " (SELECT  " + BaseRoleOrganizeEntity.FieldOrganizeId
                            + " FROM " + tableNameRoleOrganize
                            + " WHERE " + BaseRoleOrganizeEntity.FieldRoleId + " IN (" + string.Join(",", roleIds) + ")"
                            + " AND " + BaseRoleOrganizeEntity.FieldEnabled + " = 1"
                            + " AND " + BaseRoleOrganizeEntity.FieldDeleted + " = 0)) "
                            + " ORDER BY  " + BaseOrganizeEntity.FieldSortCode;

            return sql;
        }

        /// <summary>
        /// 获取数据表
        /// </summary>
        /// <param name="systemCode"></param>
        /// <param name="roleIds"></param>
        /// <returns></returns>
        public DataTable GetDataTableByRole(string systemCode, string[] roleIds)
        {
            var sql = GetSqlQueryByRole(systemCode, roleIds);
            return DbHelper.Fill(sql);
        }

        /// <summary>
        /// 为了提高授权的运行速度
        /// </summary>
        /// <param name="systemCode">系统编号</param>
        /// <param name="organizeId">组织机构主键</param>
        /// <param name="roleId">角色主键</param>
        /// <returns>主键</returns>
        public string AddToRole(string systemCode, string organizeId, string roleId)
        {
            var result = string.Empty;

            var entity = new BaseRoleOrganizeEntity
            {
                OrganizeId = organizeId,
                RoleId = roleId,
                Enabled = 1,
                DeletionStateCode = 0
            };
            var tableName = systemCode + "RoleOrganize";
            var manager = new BaseRoleOrganizeManager(DbHelper, UserInfo, tableName);
            return manager.Add(entity);
        }

        /// <summary>
        /// 添加到角色
        /// </summary>
        /// <param name="systemCode"></param>
        /// <param name="organizeIds"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public int AddToRole(string systemCode, string[] organizeIds, string roleId)
        {
            var result = 0;

            for (var i = 0; i < organizeIds.Length; i++)
            {
                AddToRole(systemCode, organizeIds[i], roleId);
                result++;
            }

            return result;
        }

        /// <summary>
        /// 移除角色成功
        /// </summary>
        /// <param name="systemCode">系统编号</param>
        /// <param name="organizeId">组织机构主键</param>
        /// <param name="roleId">角色主键</param>
        /// <returns>影响行数</returns>
        public int RemoveFormRole(string systemCode, string organizeId, string roleId)
        {
            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseRoleOrganizeEntity.FieldRoleId, roleId),
                new KeyValuePair<string, object>(BaseRoleOrganizeEntity.FieldOrganizeId, organizeId)
            };
            var tableName = systemCode + "RoleOrganize";
            var manager = new BaseRoleOrganizeManager(DbHelper, UserInfo, tableName);
            return manager.Delete(parameters);
        }
        /// <summary>
        /// 从角色中删除
        /// </summary>
        /// <param name="systemCode"></param>
        /// <param name="organizeIds"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public int RemoveFormRole(string systemCode, string[] organizeIds, string roleId)
        {
            var result = 0;

            for (var i = 0; i < organizeIds.Length; i++)
            {
                // 移除用户角色
                result += RemoveFormRole(systemCode, organizeIds[i], roleId);
            }

            return result;
        }

        /// <summary>
        /// 清空组织机构
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public int ClearOrganize(string systemCode, string roleId)
        {
            var result = 0;

            var tableName = systemCode + "RoleOrganize";
            var manager = new BaseRoleOrganizeManager(DbHelper, UserInfo, tableName);
            result += manager.Delete(new List<KeyValuePair<string, object>> { new KeyValuePair<string, object>(BaseRoleOrganizeEntity.FieldRoleId, roleId) });

            return result;
        }
    }
}