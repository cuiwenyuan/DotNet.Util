//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System.Data;

namespace DotNet.Business
{
    using Model;

    /// <summary>
    /// BaseUserManager
    /// 角色管理之角色管理
    /// 
    /// 修改记录
    /// 
    ///		2016.08.08 版本：1.0 Troy.Cui	新增。
    /// 
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2016.08.08</date>
    /// </author> 
    /// </summary>
    public partial class BaseUserManager : BaseManager
    {
        #region 根据角色Code获取用户表
        /// <summary>
        /// 根据角色Code获取用户表
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="roleCode"></param>
        /// <param name="companyId"></param>
        /// <returns></returns>
        public DataTable GetDataTableByRoleCode(string systemCode, string roleCode, string companyId = null)
        {
            if (string.IsNullOrEmpty(systemCode))
            {
                systemCode = "Base";
            }
            var tableNameUserRole = systemCode + "UserRole";
            var tableNameRole = systemCode + "Role";

            var sql = "SELECT " + SelectFields + " FROM " + BaseUserEntity.TableName
                            + " WHERE " + BaseUserEntity.FieldEnabled + " = 1 "
                            + "       AND " + BaseUserEntity.FieldDeleted + "= 0 "
                            + "       AND ( " + BaseUserEntity.FieldId + " IN "
                            + "           (SELECT  " + BaseUserRoleEntity.FieldUserId
                            + " FROM " + tableNameUserRole
                            + "             WHERE " + BaseUserRoleEntity.FieldRoleId + " IN (SELECT " + BaseRoleEntity.FieldId + " FROM " + tableNameRole + " WHERE " + BaseRoleEntity.FieldCode + " = N'" + roleCode + "')"
                            + "               AND " + BaseUserRoleEntity.FieldEnabled + " = 1"
                            + "                AND " + BaseUserRoleEntity.FieldDeleted + " = 0)) "
                            + " ORDER BY  " + BaseUserEntity.FieldSortCode;

            return DbHelper.Fill(sql);
        }
        #endregion
    }
}