//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2023, DotNet.
//-----------------------------------------------------------------

using System;
using System.Data;

namespace DotNet.Business
{
    using Model;
    using Util;
    /// <summary>
    ///	BaseManager
    /// 通用基类部分（分页）
    /// 
    /// 总觉得自己写的程序不上档次，这些新技术也玩玩，也许做出来的东西更专业了。
    /// 修改记录
    /// 
    ///     2017.11.24 版本：3.0 Troy Cui 继续优化，Resharper化
    ///     2016.05.21 版本：2.0 Troy Cui 自定义View分页怎能没有condition条件带入
    ///		2012.02.04 版本：1.0 JiRiGaLa 进行提炼，把代码进行分组。
    ///
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2012.02.04</date>
    /// </author> 
    /// </summary>
    public partial class BaseManager : IBaseManager
    {
        #region public virtual DataTable GetDataTableByPage(out int recordCount, int pageNo, int pageSize, string condition, IDbDataParameter[] dbParameters, string order)
        /// <summary>
        /// 获取分页DataTable
        /// </summary>
        /// <param name="recordCount">记录总数</param>
        /// <param name="pageNo">当前页数</param>
        /// <param name="pageSize">每页显示多少条</param>
        /// <param name="condition">条件</param>
        /// <param name="dbParameters">参数</param>
        /// <param name="order">排序(不包含ORDER BY)</param>
        /// <returns>数据表</returns>
        public virtual DataTable GetDataTableByPage(out int recordCount, int pageNo, int pageSize, string condition, IDbDataParameter[] dbParameters, string order)
        {
            recordCount = DbHelper.GetCount(CurrentTableName, condition, dbParameters, CurrentIndex);
            return DbHelper.GetDataTableByPage(CurrentTableName, SelectFields, pageNo, pageSize, condition, dbParameters, order, CurrentIndex);
        }

        #endregion

        #region public virtual DataTable GetDataTableByPage(out int recordCount, int pageNo = 1, int pageSize = 20, string sortExpression = null, string sortDirection = null, string tableName = null, string condition = null, IDbDataParameter[] dbParameters = null, string selectField = null)
        /// <summary>
        /// 分页读取数据
        /// </summary>
        /// <param name="recordCount">条数</param>
        /// <param name="pageNo">当前页</param>
        /// <param name="pageSize">每页显示</param>
        /// <param name="sortExpression">排序字段</param>
        /// <param name="sortDirection">排序顺序</param>
        /// <param name="tableName">表名</param>
        /// <param name="condition">查询条件</param>
        /// <param name="dbParameters">数据参数</param>
        /// <param name="selectField">选择哪些字段</param>
        /// <returns>数据表</returns>
        public virtual DataTable GetDataTableByPage(out int recordCount, int pageNo = 1, int pageSize = 20, string sortExpression = null, string sortDirection = null, string tableName = null, string condition = null, IDbDataParameter[] dbParameters = null, string selectField = null)
        {
            if (string.IsNullOrEmpty(tableName))
            {
                tableName = CurrentTableName;
            }
            if (tableName.IndexOf("SELECT", StringComparison.OrdinalIgnoreCase) >= 0 || DbHelper.CurrentDbType == CurrentDbType.MySql || DbHelper.CurrentDbType == CurrentDbType.Oracle)
            {
                // 统计总条数
                var commandText = string.Empty;
                if (string.IsNullOrEmpty(tableName))
                {
                    tableName = CurrentTableName;
                }
                var sb = Pool.StringBuilder.Get();
                if (!string.IsNullOrEmpty(condition))
                {
                    sb.Append(" WHERE " + condition);
                }
                commandText = tableName;
                if (tableName.IndexOf("SELECT", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    commandText = "(" + tableName + ") T ";
                    // commandText = "(" + tableName + ") AS T ";
                }
                commandText = string.Format("SELECT COUNT(*) AS recordCount FROM {0} {1}", commandText, sb.Put());
                var obj = DbHelper.ExecuteScalar(commandText, dbParameters);
                if (obj != null)
                {
                    recordCount = obj.ToInt();
                }
                else
                {
                    recordCount = 0;
                }
                //return DbUtil.GetDataTableByPage(DbHelper, recordCount, pageNo, pageSize, tableName, dbParameters, sortExpression, sortDirection);
                //Troy 20160521 自定义View分页怎能没有查询条件带入
                return DbHelper.GetDataTableByPage(recordCount, pageNo, pageSize, tableName, condition, dbParameters, sortExpression, sortDirection);
            }
            // 这个是调用存储过程的方法
            return DbHelper.GetDataTableByPage(out recordCount, pageNo, pageSize, sortExpression, sortDirection, tableName, condition, selectField);
        }
        #endregion

        #region public virtual DataTable GetDataTableByPage(IDbHelper dbHelper, out int recordCount, int pageNo = 1, int pageSize = 20, string sortExpression = null, string sortDirection = null, string tableName = null, string condition = null, IDbDataParameter[] dbParameters = null, string selectField = null)

        /// <summary>
        /// 分页读取数据
        /// </summary>
        /// <param name="dbHelper">指定数据IDbHelper</param>
        /// <param name="recordCount">条数</param>
        /// <param name="pageNo">当前页</param>
        /// <param name="pageSize">每页显示</param>
        /// <param name="sortExpression">排序字段</param>
        /// <param name="sortDirection">排序顺序</param>
        /// <param name="tableName">表名</param>
        /// <param name="condition">查询条件</param>
        /// <param name="dbParameters">数据参数</param>
        /// <param name="selectField">选择哪些字段</param>
        /// <returns>数据表</returns>
        public virtual DataTable GetDataTableByPage(IDbHelper dbHelper, out int recordCount, int pageNo = 1, int pageSize = 20, string sortExpression = null, string sortDirection = null, string tableName = null, string condition = null, IDbDataParameter[] dbParameters = null, string selectField = null)
        {
            if (string.IsNullOrEmpty(tableName))
            {
                tableName = CurrentTableName;
            }
            if (tableName.IndexOf("SELECT", StringComparison.OrdinalIgnoreCase) >= 0 || dbHelper.CurrentDbType == CurrentDbType.MySql || dbHelper.CurrentDbType == CurrentDbType.Oracle)
            {
                // 统计总条数
                var commandText = string.Empty;
                if (string.IsNullOrEmpty(tableName))
                {
                    tableName = CurrentTableName;
                }
                var sb = Pool.StringBuilder.Get();
                if (!string.IsNullOrEmpty(condition))
                {
                    sb.Append(" WHERE " + condition);
                }
                commandText = tableName;
                if (tableName.IndexOf("SELECT", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    commandText = "(" + tableName + ") T ";
                    // commandText = "(" + tableName + ") AS T ";
                }
                commandText = string.Format("SELECT COUNT(*) AS recordCount FROM {0} {1}", commandText, sb.Put());
                var obj = dbHelper.ExecuteScalar(commandText, dbParameters);
                if (obj != null)
                {
                    recordCount = obj.ToInt();
                }
                else
                {
                    recordCount = 0;
                }
                //return DbUtil.GetDataTableByPage(DbHelper, recordCount, pageNo, pageSize, tableName, dbParameters, sortExpression, sortDirection);
                //Troy 20160521 自定义View分页怎能没有查询条件带入
                return dbHelper.GetDataTableByPage(recordCount, pageNo, pageSize, tableName, condition, dbParameters, sortExpression, sortDirection);
            }
            // 这个是调用存储过程的方法
            return dbHelper.GetDataTableByPage(out recordCount, pageNo, pageSize, sortExpression, sortDirection, tableName, condition, selectField);
        }
        #endregion

        #region public virtual DataTable GetDataTableByPage(string companyId, string departmentId, string userId, string startTime, string endTime, string searchKey, out int recordCount, int pageNo = 1, int pageSize = 20, string sortExpression = InsuranceSalesDetailEntity.FieldCreateTime, string sortDirection = "DESC", bool showDisabled = false, bool showDeleted = false)
        /// <summary>
        /// 按条件分页查询(带记录状态Enabled和删除状态Deleted)
        /// </summary>
        /// <param name="companyId">查看公司主键</param>
        /// <param name="departmentId">查看部门主键</param>
        /// <param name="userId">查看用户主键</param>
        /// <param name="startTime">创建开始时间</param>
        /// <param name="endTime">创建结束时间</param>
        /// <param name="searchKey">查询关键字</param>
        /// <param name="recordCount">记录数</param>
        /// <param name="pageNo">当前页</param>
        /// <param name="pageSize">每页显示</param>
        /// <param name="sortExpression">排序字段</param>
        /// <param name="sortDirection">排序方向</param>
        /// <param name="showDisabled">是否显示无效记录</param>
        /// <param name="showDeleted">是否显示已删除记录</param>
        /// <returns>数据表</returns>
        public virtual DataTable GetDataTableByPage(string companyId, string departmentId, string userId, string startTime, string endTime, string searchKey, out int recordCount, int pageNo = 1, int pageSize = 20, string sortExpression = BaseEntity.FieldCreateTime, string sortDirection = "DESC", bool showDisabled = true, bool showDeleted = true)
        {
            var sb = Pool.StringBuilder.Get().Append(" 1 = 1");
            //是否显示无效记录
            if (!showDisabled)
            {
                sb.Append(" AND " + BaseEntity.FieldEnabled + " = 1");
            }
            //是否显示已删除记录
            if (!showDeleted)
            {
                sb.Append(" AND " + BaseEntity.FieldDeleted + " = 0");
            }

            if (ValidateUtil.IsInt(companyId))
            {
                //sb.Append(" AND " + InsuranceSalesDetailEntity.FieldCompanyId + " = " + companyId);
            }
            // 只有管理员才能看到所有的
            //if (!(UserInfo.IsAdministrator && BaseSystemInfo.AdministratorEnabled))
            //{
            //sb.Append(" AND (" + BaseEntity.FieldUserCompanyId + " = 0 OR " + BaseEntity.FieldUserCompanyId + " = " + UserInfo.CompanyId + ")");
            //}
            if (ValidateUtil.IsInt(departmentId))
            {
                //sb.Append(" AND " + BaseEntity.FieldDepartmentId + " = " + departmentId);
            }
            if (ValidateUtil.IsInt(userId))
            {
                //sb.Append(" AND " + BaseEntity.FieldUserId + " = " + userId);
            }
            //创建时间
            if (ValidateUtil.IsDateTime(startTime))
            {
                sb.Append(" AND " + BaseEntity.FieldCreateTime + " >= " + dbHelper.ToDbTime(startTime));
            }
            if (ValidateUtil.IsDateTime(endTime))
            {
                sb.Append(" AND " + BaseEntity.FieldCreateTime + " <= " + dbHelper.ToDbTime(endTime.ToDateTime().Date.AddDays(1).AddMilliseconds(-1)));
            }
            if (!string.IsNullOrEmpty(searchKey))
            {
                searchKey = StringUtil.GetLikeSearchKey(dbHelper.SqlSafe(searchKey));
                sb.Append(" AND (Name LIKE N'%" + searchKey + "%' OR Description LIKE N'%" + searchKey + "%')");
            }
            sb.Replace(" 1 = 1 AND ", "");
            return GetDataTableByPage(out recordCount, pageNo, pageSize, sortExpression, sortDirection, CurrentTableName, sb.Put(), null, "*");
        }
        #endregion
    }
}