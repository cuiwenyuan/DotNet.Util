//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2023, DotNet.
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
        #region public DataTable GetDataTableByPage(string searchKey, string companyId, string departmentId, string roleId, out int recordCount, int pageNo = 1, int pageSize = 20, string order = null) 分页查询
        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="searchKey">查询关键字</param>
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
            var condition = BaseStaffEntity.CurrentTableName + "." + BaseStaffEntity.FieldDeleted + " = 0 AND " + BaseStaffEntity.CurrentTableName + "." + BaseStaffEntity.FieldEnabled + " = 1 ";

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
                condition += " AND (" + BaseStaffEntity.FieldRealName + " LIKE '%" + searchKey + "'";
                condition += " OR " + BaseStaffEntity.FieldUserName + " LIKE '%" + searchKey + "'";
                condition += " OR " + BaseStaffEntity.FieldQuickQuery + " LIKE '%" + searchKey + "')";
            }
            recordCount = DbHelper.GetCount(CurrentTableName, condition);
            CurrentTableName = "BaseStaff";

            return DbHelper.GetDataTableByPage(CurrentTableName, SelectFields, pageNo, pageSize, condition, order);
        }
        #endregion
    }
}