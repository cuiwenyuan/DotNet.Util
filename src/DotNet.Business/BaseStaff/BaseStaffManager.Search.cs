//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System.Data;

namespace DotNet.Business
{
    using Model;
    using Util;

    /// <summary>
    /// BaseStaffManager
    /// 职员管理
    /// 
    /// 修改记录
    /// 
    ///		2014.08.10 版本：1.0 JiRiGaLa	主键整理。
    /// 
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2014.08.10</date>
    /// </author> 
    /// </summary>
    public partial class BaseStaffManager : BaseManager
    {
        private string GetSearchConditional(string permissionCode, string condition, bool? enabled, string auditStates, string companyId = null, string departmentId = null)
        {
            var sb = BaseStaffEntity.CurrentTableName + "." + BaseStaffEntity.FieldDeleted + " = 0 ";
            if (enabled.HasValue)
            {
                if (enabled == true)
                {
                    sb += " AND " + BaseStaffEntity.CurrentTableName + "." + BaseStaffEntity.FieldEnabled + " = 1 ";
                }
                else
                {
                    sb += " AND " + BaseStaffEntity.CurrentTableName + "." + BaseStaffEntity.FieldEnabled + " = 0 ";
                }
            }
            if (!string.IsNullOrEmpty(condition))
            {
                // 传递过来的表达式，还是搜索值？
                if (condition.IndexOf("AND") < 0 && condition.IndexOf("=") < 0)
                {
                    condition = StringUtil.GetSearchString(condition);
                    sb += " AND ("
                                + BaseStaffEntity.CurrentTableName + "." + BaseStaffEntity.FieldUserName + " LIKE '%" + condition + "%'"
                                // + " OR " + BaseStaffEntity.CurrentTableName + "." + BaseStaffEntity.FieldSimpleSpelling + " LIKE '%" + where + "%'"
                                + " OR " + BaseStaffEntity.CurrentTableName + "." + BaseStaffEntity.FieldEmployeeNumber + " LIKE '%" + condition + "%'"
                                + " OR " + BaseStaffEntity.CurrentTableName + "." + BaseStaffEntity.FieldRealName + " LIKE '%" + condition + "%'"
                                + " OR " + BaseStaffEntity.CurrentTableName + "." + BaseStaffEntity.FieldQuickQuery + " LIKE '%" + condition + "%'"
                                + " OR " + BaseStaffEntity.CurrentTableName + "." + BaseStaffEntity.FieldCompanyName + " LIKE '%" + condition + "%'"
                                + " OR " + BaseStaffEntity.CurrentTableName + "." + BaseStaffEntity.FieldDepartmentName + " LIKE '%" + condition + "%'"
                                // + " OR " + BaseStaffEntity.CurrentTableName + "." + BaseStaffEntity.FieldDescription + " LIKE '%" + search + "%'"
                                + ")";
                }
                else
                {
                    sb += " AND (" + condition + ")";
                }
            }
            if (!string.IsNullOrEmpty(departmentId))
            {
                /*
                BaseOrganizationManager organizationManager = new BaseOrganizationManager(this.DbHelper, this.UserInfo);
                string[] ids = organizationManager.GetChildrensId(BaseOrganizationEntity.FieldId, departmentId, BaseOrganizationEntity.FieldParentId);
                if (ids != null && ids.Length > 0)
                {
                    condition += " AND (" + BaseStaffEntity.CurrentTableName + "." + BaseStaffEntity.FieldCompanyId + " IN (" + StringUtil.ArrayToList(ids) + ")"
                     + " OR " + BaseStaffEntity.CurrentTableName + "." + BaseStaffEntity.FieldDepartmentId + " IN (" + StringUtil.ArrayToList(ids) + ")"
                     + " OR " + BaseStaffEntity.CurrentTableName + "." + BaseStaffEntity.FieldWorkgroupId + " IN (" + StringUtil.ArrayToList(ids) + "))";
                }
                */
                sb += " AND (" + BaseStaffEntity.CurrentTableName + "." + BaseStaffEntity.FieldDepartmentId + " = " + departmentId + ")";
            }
            if (!string.IsNullOrEmpty(companyId))
            {
                sb += " AND (" + BaseStaffEntity.CurrentTableName + "." + BaseStaffEntity.FieldCompanyId + " = " + companyId + ")";
            }
            if (enabled != null)
            {
                sb += " AND (" + BaseStaffEntity.CurrentTableName + "." + BaseStaffEntity.FieldEnabled + " = " + ((bool)enabled ? 1 : 0) + ")";
            }

            // 是否过滤用户， 获得组织机构列表， 这里需要一个按用户过滤得功能
            if (!string.IsNullOrEmpty(permissionCode) && (!BaseUserManager.IsAdministrator(UserInfo.Id.ToString())) && (BaseSystemInfo.UsePermissionScope))
            {
                // string permissionCode = "Resource.ManagePermission";
                var permissionId = new BaseModuleManager().GetIdByCodeByCache(UserInfo.SystemCode, permissionCode);
                if (!string.IsNullOrEmpty(permissionId))
                {
                    // 从小到大的顺序进行显示，防止错误发生
                    var userPermissionScopeManager = new BaseUserScopeManager(DbHelper, UserInfo);
                    var organizationIds = userPermissionScopeManager.GetOrganizationIds(UserInfo.SystemCode, UserInfo.Id.ToString(), permissionId);

                    // 没有任何数据权限
                    if (StringUtil.Exists(organizationIds, ((int)PermissionOrganizationScope.NotAllowed).ToString()))
                    {
                        sb += " AND (" + BaseStaffEntity.CurrentTableName + "." + BaseStaffEntity.FieldId + " = NULL ) ";
                    }
                    // 按详细设定的数据
                    if (StringUtil.Exists(organizationIds, ((int)PermissionOrganizationScope.ByDetails).ToString()))
                    {
                        var permissionScopeManager = new BasePermissionScopeManager(DbHelper, UserInfo, CurrentTableName); ;
                        var userIds = permissionScopeManager.GetUserIds(UserInfo.SystemCode, UserInfo.Id.ToString(), permissionCode);
                        sb += " AND (" + BaseStaffEntity.CurrentTableName + "." + BaseStaffEntity.FieldId + " IN (" + StringUtil.ArrayToList(userIds) + ")) ";
                    }
                    // 自己的数据，仅本人
                    if (StringUtil.Exists(organizationIds, ((int)PermissionOrganizationScope.OnlyOwnData).ToString()))
                    {
                        sb += " AND (" + BaseStaffEntity.CurrentTableName + "." + BaseStaffEntity.FieldId + " = " + UserInfo.Id + ") ";
                    }
                    // 用户所在工作组数据
                    if (StringUtil.Exists(organizationIds, ((int)PermissionOrganizationScope.UserWorkgroup).ToString()))
                    {
                        // condition += " AND (" + BaseStaffEntity.CurrentTableName + "." + BaseStaffEntity.FieldWorkgroupId + " = " + this.UserInfo.WorkgroupId + ") ";
                    }
                    // 用户所在部门数据
                    if (StringUtil.Exists(organizationIds, ((int)PermissionOrganizationScope.UserDepartment).ToString()))
                    {
                        sb += " AND (" + BaseStaffEntity.CurrentTableName + "." + BaseStaffEntity.FieldDepartmentId + " = " + UserInfo.DepartmentId + ") ";
                    }
                    // 用户所在分支机构数据
                    if (StringUtil.Exists(organizationIds, ((int)PermissionOrganizationScope.UserSubCompany).ToString()))
                    {
                        // condition += " AND (" + BaseStaffEntity.CurrentTableName + "." + BaseStaffEntity.FieldSubCompanyId + " = " + this.UserInfo.SubCompanyId + ") ";
                    }
                    // 用户所在公司数据
                    if (StringUtil.Exists(organizationIds, ((int)PermissionOrganizationScope.UserCompany).ToString()))
                    {
                        sb += " AND (" + BaseStaffEntity.CurrentTableName + "." + BaseStaffEntity.FieldCompanyId + " = " + UserInfo.CompanyId + ") ";
                    }
                    // 全部数据，这里就不用设置过滤条件了
                    if (StringUtil.Exists(organizationIds, ((int)PermissionOrganizationScope.AllData).ToString()))
                    {
                    }
                }
            }
            return sb;
        }

        /// <summary>
        /// 根据分页获取
        /// </summary>
        /// <param name="permissionCode">权限编码</param>
        /// <param name="condition"></param>
        /// <param name="enabled"></param>
        /// <param name="auditStates"></param>
        /// <param name="companyId"></param>
        /// <param name="departmentId"></param>
        /// <param name="recordCount"></param>
        /// <param name="pageNo"></param>
        /// <param name="pageSize"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public DataTable SearchByPage(string permissionCode, string condition, bool? enabled, string auditStates, string companyId, string departmentId, out int recordCount, int pageNo = 1, int pageSize = 20, string order = null)
        {
            condition = GetSearchConditional(permissionCode, condition, enabled, auditStates, companyId, departmentId);
            return GetDataTableByPage(out recordCount, pageNo, pageSize, condition, null, order);
        }

        /// <summary>
        /// 分页获取日志
        /// </summary>
        /// <param name="recordCount"></param>
        /// <param name="pageNo"></param>
        /// <param name="pageSize"></param>
        /// <param name="permissionCode">权限编码</param>
        /// <param name="condition"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public DataTable SearchLogByPage(out int recordCount, int pageNo, int pageSize, string permissionCode, string condition, string order = null)
        {
            switch (DbHelper.CurrentDbType)
            {
                case CurrentDbType.SqlServer:
                case CurrentDbType.Oracle:
                    SelectFields = "*";
                    CurrentTableName = "BaseStaff";
                    break;
            }
            return GetDataTableByPage(out recordCount, pageNo, pageSize, condition, null, order);
        }

        #region public DataTable GetDataTableByPage(string searchKey, string companyId, string departmentId, string roleId, out int recordCount, int pageNo = 1, int pageSize = 20, string order = null) 分页查询
        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="searchKey">查询字段</param>
        /// <param name="companyId">公司主键</param>
        /// <param name="departmentId">部门主键</param>
        /// <param name="roleId">角色主键</param>
        /// <param name="recordCount">记录数</param>
        /// <param name="pageNo">当前页</param>
        /// <param name="pageSize">每页显示</param>
        /// <param name="order">排序</param>
        /// <returns>数据表</returns>
        public DataTable GetDataTableByPage(string searchKey, string companyId, string departmentId, string roleId, out int recordCount, int pageNo = 1, int pageSize = 20, string order = null)
        {
            var condition = BaseStaffEntity.CurrentTableName + "." + BaseStaffEntity.FieldDeleted + " = 0 "
                 + " AND " + BaseStaffEntity.CurrentTableName + "." + BaseStaffEntity.FieldEnabled + " = 1 ";

            if (!string.IsNullOrEmpty(companyId))
            {
                condition += " AND (" + BaseStaffEntity.CurrentTableName + "." + BaseStaffEntity.FieldCompanyId + " = " + companyId + ")";
            }
            if (!string.IsNullOrEmpty(departmentId))
            {
                condition += " AND (" + BaseStaffEntity.CurrentTableName + "." + BaseStaffEntity.FieldDepartmentId + " = " + departmentId + ")";
            }
            if (!string.IsNullOrEmpty(searchKey))
            {
                searchKey = "'" + StringUtil.GetSearchString(searchKey) + "'";
                condition += " AND (" + BaseStaffEntity.FieldRealName + " LIKE " + searchKey;
                condition += " OR " + BaseStaffEntity.FieldUserName + " LIKE " + searchKey;
                condition += " OR " + BaseStaffEntity.FieldQuickQuery + " LIKE " + searchKey + ")";
                // condition += " OR " + BaseStaffEntity.FieldSimpleSpelling + " LIKE " + searchKey + ")";
            }
            recordCount = DbUtil.GetCount(DbHelper, CurrentTableName, condition);
            CurrentTableName = "BaseStaff";

            return DbUtil.GetDataTableByPage(DbHelper, CurrentTableName, SelectFields, pageNo, pageSize, condition, order);
        }
        #endregion
    }
}