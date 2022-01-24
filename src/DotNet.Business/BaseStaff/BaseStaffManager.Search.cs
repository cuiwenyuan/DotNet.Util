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
            var sb = BaseStaffEntity.TableName + "." + BaseStaffEntity.FieldDeleted + " = 0 ";
            if (enabled.HasValue)
            {
                if (enabled == true)
                {
                    sb += " AND " + BaseStaffEntity.TableName + "." + BaseStaffEntity.FieldEnabled + " = 1 ";
                }
                else
                {
                    sb += " AND " + BaseStaffEntity.TableName + "." + BaseStaffEntity.FieldEnabled + " = 0 ";
                }
            }
            if (!string.IsNullOrEmpty(condition))
            {
                // 传递过来的表达式，还是搜索值？
                if (condition.IndexOf("AND") < 0 && condition.IndexOf("=") < 0)
                {
                    condition = StringUtil.GetSearchString(condition);
                    sb += " AND ("
                                + BaseStaffEntity.TableName + "." + BaseStaffEntity.FieldUserName + " LIKE '%" + condition + "%'"
                                // + " OR " + BaseStaffEntity.TableName + "." + BaseStaffEntity.FieldSimpleSpelling + " LIKE '%" + where + "%'"
                                + " OR " + BaseStaffEntity.TableName + "." + BaseStaffEntity.FieldCode + " LIKE '%" + condition + "%'"
                                + " OR " + BaseStaffEntity.TableName + "." + BaseStaffEntity.FieldRealName + " LIKE '%" + condition + "%'"
                                + " OR " + BaseStaffEntity.TableName + "." + BaseStaffEntity.FieldQuickQuery + " LIKE '%" + condition + "%'"
                                + " OR " + BaseStaffEntity.TableName + "." + BaseStaffEntity.FieldCompanyName + " LIKE '%" + condition + "%'"
                                + " OR " + BaseStaffEntity.TableName + "." + BaseStaffEntity.FieldDepartmentName + " LIKE '%" + condition + "%'"
                                // + " OR " + BaseStaffEntity.TableName + "." + BaseStaffEntity.FieldDescription + " LIKE '%" + search + "%'"
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
                BaseOrganizationManager organizeManager = new BaseOrganizationManager(this.DbHelper, this.UserInfo);
                string[] ids = organizeManager.GetChildrensId(BaseOrganizationEntity.FieldId, departmentId, BaseOrganizationEntity.FieldParentId);
                if (ids != null && ids.Length > 0)
                {
                    condition += " AND (" + BaseStaffEntity.TableName + "." + BaseStaffEntity.FieldCompanyId + " IN (" + StringUtil.ArrayToList(ids) + ")"
                     + " OR " + BaseStaffEntity.TableName + "." + BaseStaffEntity.FieldDepartmentId + " IN (" + StringUtil.ArrayToList(ids) + ")"
                     + " OR " + BaseStaffEntity.TableName + "." + BaseStaffEntity.FieldWorkgroupId + " IN (" + StringUtil.ArrayToList(ids) + "))";
                }
                */
                sb += " AND (" + BaseStaffEntity.TableName + "." + BaseStaffEntity.FieldDepartmentId + " = " + departmentId + ")";
            }
            if (!string.IsNullOrEmpty(companyId))
            {
                sb += " AND (" + BaseStaffEntity.TableName + "." + BaseStaffEntity.FieldCompanyId + " = " + companyId + ")";
            }
            if (enabled != null)
            {
                sb += " AND (" + BaseStaffEntity.TableName + "." + BaseStaffEntity.FieldEnabled + " = " + ((bool)enabled ? 1 : 0) + ")";
            }

            // 是否过滤用户， 获得组织机构列表， 这里需要一个按用户过滤得功能
            if (!string.IsNullOrEmpty(permissionCode) && (!BaseUserManager.IsAdministrator(UserInfo.Id)) && (BaseSystemInfo.UsePermissionScope))
            {
                // string permissionCode = "Resource.ManagePermission";
                var permissionId = BaseModuleManager.GetIdByCodeByCache(UserInfo.SystemCode, permissionCode);
                if (!string.IsNullOrEmpty(permissionId))
                {
                    // 从小到大的顺序进行显示，防止错误发生
                    var userPermissionScopeManager = new BaseUserScopeManager(DbHelper, UserInfo);
                    var organizationIds = userPermissionScopeManager.GetOrganizationIds(UserInfo.SystemCode, UserInfo.Id, permissionId);

                    // 没有任何数据权限
                    if (StringUtil.Exists(organizationIds, ((int)PermissionOrganizationScope.NotAllowed).ToString()))
                    {
                        sb += " AND (" + BaseStaffEntity.TableName + "." + BaseStaffEntity.FieldId + " = NULL ) ";
                    }
                    // 按详细设定的数据
                    if (StringUtil.Exists(organizationIds, ((int)PermissionOrganizationScope.ByDetails).ToString()))
                    {
                        var permissionScopeManager = new BasePermissionScopeManager(DbHelper, UserInfo, CurrentTableName); ;
                        var userIds = permissionScopeManager.GetUserIds(UserInfo.SystemCode, UserInfo.Id, permissionCode);
                        sb += " AND (" + BaseStaffEntity.TableName + "." + BaseStaffEntity.FieldId + " IN (" + string.Join(",", userIds) + ")) ";
                    }
                    // 自己的数据，仅本人
                    if (StringUtil.Exists(organizationIds, ((int)PermissionOrganizationScope.OnlyOwnData).ToString()))
                    {
                        sb += " AND (" + BaseStaffEntity.TableName + "." + BaseStaffEntity.FieldId + " = " + UserInfo.Id + ") ";
                    }
                    // 用户所在工作组数据
                    if (StringUtil.Exists(organizationIds, ((int)PermissionOrganizationScope.UserWorkgroup).ToString()))
                    {
                        // condition += " AND (" + BaseStaffEntity.TableName + "." + BaseStaffEntity.FieldWorkgroupId + " = " + this.UserInfo.WorkgroupId + ") ";
                    }
                    // 用户所在部门数据
                    if (StringUtil.Exists(organizationIds, ((int)PermissionOrganizationScope.UserDepartment).ToString()))
                    {
                        sb += " AND (" + BaseStaffEntity.TableName + "." + BaseStaffEntity.FieldDepartmentId + " = " + UserInfo.DepartmentId + ") ";
                    }
                    // 用户所在分支机构数据
                    if (StringUtil.Exists(organizationIds, ((int)PermissionOrganizationScope.UserSubCompany).ToString()))
                    {
                        // condition += " AND (" + BaseStaffEntity.TableName + "." + BaseStaffEntity.FieldSubCompanyId + " = " + this.UserInfo.SubCompanyId + ") ";
                    }
                    // 用户所在公司数据
                    if (StringUtil.Exists(organizationIds, ((int)PermissionOrganizationScope.UserCompany).ToString()))
                    {
                        sb += " AND (" + BaseStaffEntity.TableName + "." + BaseStaffEntity.FieldCompanyId + " = " + UserInfo.CompanyId + ") ";
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
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public DataTable SearchByPage(string permissionCode, string condition, bool? enabled, string auditStates, string companyId, string departmentId, out int recordCount, int pageIndex = 0, int pageSize = 20, string order = null)
        {
            condition = GetSearchConditional(permissionCode, condition, enabled, auditStates, companyId, departmentId);
            return GetDataTableByPage(out recordCount, pageIndex, pageSize, condition, null, order);
        }

        /// <summary>
        /// 分页获取日志
        /// </summary>
        /// <param name="recordCount"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="permissionCode">权限编码</param>
        /// <param name="condition"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public DataTable SearchLogByPage(out int recordCount, int pageIndex, int pageSize, string permissionCode, string condition, string order = null)
        {
            switch (DbHelper.CurrentDbType)
            {
                case CurrentDbType.SqlServer:
                case CurrentDbType.Oracle:
                    SelectFields = "*";
                    CurrentTableName = "BaseStaff";
                    break;
            }
            return GetDataTableByPage(out recordCount, pageIndex, pageSize, condition, null, order);
        }

        #region public DataTable GetDataTableByPage(string searchKey, string companyId, string departmentId, string roleId, out int recordCount, int pageIndex = 0, int pageSize = 20, string order = null) 分页查询
        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="searchKey">查询字段</param>
        /// <param name="companyId">公司主键</param>
        /// <param name="departmentId">部门主键</param>
        /// <param name="roleId">角色主键</param>
        /// <param name="recordCount">记录数</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">每页显示</param>
        /// <param name="order">排序</param>
        /// <returns>数据表</returns>
        public DataTable GetDataTableByPage(string searchKey, string companyId, string departmentId, string roleId, out int recordCount, int pageIndex = 0, int pageSize = 20, string order = null)
        {
            var condition = BaseStaffEntity.TableName + "." + BaseStaffEntity.FieldDeleted + " = 0 "
                 + " AND " + BaseStaffEntity.TableName + "." + BaseStaffEntity.FieldEnabled + " = 1 ";

            if (!string.IsNullOrEmpty(companyId))
            {
                condition += " AND (" + BaseStaffEntity.TableName + "." + BaseStaffEntity.FieldCompanyId + " = " + companyId + ")";
            }
            if (!string.IsNullOrEmpty(departmentId))
            {
                condition += " AND (" + BaseStaffEntity.TableName + "." + BaseStaffEntity.FieldDepartmentId + " = " + departmentId + ")";
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

            return DbUtil.GetDataTableByPage(DbHelper, CurrentTableName, SelectFields, pageIndex, pageSize, condition, order);
        }
        #endregion
    }
}