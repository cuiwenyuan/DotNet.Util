//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System.Data;
using System.Reflection;


namespace DotNet.Business
{
    using IService;
    using Model;
    using Util;

    /// <summary>
    /// LogService
    /// 日志服务
    /// 
    /// 修改记录
    /// 
    ///		2013.06.06 张祈璟重构
    ///		2008.03.23 版本：1.0 JiRiGaLa 创建。 
    /// 
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2008.03.23</date>
    /// </author> 
    /// </summary>


    public class LogService : ILogService
    {
        /// <summary>
        /// 业务中心数据库连接
        /// </summary>
        private readonly string _businessDbConnection = BaseSystemInfo.BusinessDbConnection;

        #region public void WriteLog(BaseUserInfo userInfo, string processId, string processName, string methodId, string methodName) 写入日志
        /// <summary>
        /// 写入日志
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="processId">服务</param>
        /// <param name="processName">服务名称</param>
        /// <param name="methodId">操作</param>
        /// <param name="methodName">操作名称</param>
        public void WriteLog(BaseUserInfo userInfo, string processId, string processName, string methodId, string methodName)
        {
            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                // BaseLogManager.Instance.Add(result, processName, methodName, processId, methodId, string.Empty);
            });
        }
        #endregion

        #region public void WriteExit(BaseUserInfo userInfo, string logId) 离开时的日志记录
        /// <summary>
        /// 离开时的日志记录
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="logId">日志主键</param>
        public void WriteExit(BaseUserInfo userInfo, string logId)
        {
            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                var sqlBuilder = new SqlBuilder(dbHelper);
                sqlBuilder.BeginUpdate(BaseLogEntity.TableName);
                // sqlBuilder.SetDBNow(BaseLogEntity.FieldModifiedOn);
                sqlBuilder.SetWhere(BaseLogEntity.FieldId, logId);
                sqlBuilder.EndUpdate();
            });
        }
        #endregion

        #region public int ResetVisitInfo(BaseUserInfo userInfo, string[] ids) 重置用户访问情况
        /// <summary>
        /// 重置用户访问情况
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="ids">日志主键</param>
        /// <returns>影响行数</returns>
        public int ResetVisitInfo(BaseUserInfo userInfo, string[] ids)
        {
            var result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDbWithTransaction(userInfo, parameter, (dbHelper) =>
            {
                var manager = new BaseUserLogOnManager(dbHelper, userInfo);
                // 重置访问情况
                result = manager.ResetVisitInfo(ids);
            });
            return result;
        }
        #endregion

        #region public DataTable GetDataTableByDate(BaseUserInfo userInfo, string beginDate, string endDate, string userId, string moduleId, string processId=null)

        /// <summary>
        /// 按日期获取日志
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="beginDate">开始时间</param>
        /// <param name="endDate">结束时间</param>
        /// <param name="userId">用户Id</param>
        /// <param name="moduleId">模块Id</param>
        /// <param name="processId">日志类型</param>
        /// <returns></returns>
        public DataTable GetDataTableByDate(BaseUserInfo userInfo, string beginDate, string endDate, string userId, string moduleId, string processId = null)
        {
            var dt = new DataTable(BaseLogEntity.TableName);

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                // var logManager = new BaseLogManager(dbHelper, result);
                if (!string.IsNullOrEmpty(userId))
                {
                    // dt = logManager.GetDataTableByDateByUserIds(new string[] { userId }, BaseLogEntity.FieldProcessId, moduleId, beginDate, endDate, processId);
                }
                else
                {
                    if (BaseUserManager.IsAdministrator(userInfo.Id))
                    {
                        // dt = logManager.GetDataTableByDate(BaseLogEntity.FieldProcessId, moduleId, beginDate, endDate, processId);
                    }
                    else
                    {
                        var basePermissionScopeManager = new BasePermissionScopeManager(dbHelper, userInfo);
                        var userIds = basePermissionScopeManager.GetUserIds(userInfo.SystemCode, userInfo.Id, "Resource.ManagePermission");
                        // dt = logManager.GetDataTableByDateByUserIds(userIds, BaseLogEntity.FieldProcessId, moduleId, beginDate, endDate, processId);
                    }
                }
                dt.TableName = BaseLogEntity.TableName;
            });
            return dt;
        }
        #endregion

        #region public DataTable GetDataTableByModule(BaseUserInfo userInfo, string processId, string beginDate,string endDate) 按模块获取日志
        /// <summary>
        /// 按模块获取日志
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="processId">服务名称</param>
        /// <param name="beginDate">开始时间</param>
        /// <param name="endDate">结束时间</param>
        /// <returns>数据表</returns>
        public DataTable GetDataTableByModule(BaseUserInfo userInfo, string processId, string beginDate, string endDate)
        {
            var result = new DataTable(BaseLogEntity.TableName);

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                // var logManager = new BaseLogManager(dbHelper, result);
                // if(userInfo.IsAdministrator)
                //{
                // dt = logManager.GetDataTableByDate(BaseLogEntity.FieldProcessId, processId, beginDate, endDate);
                // }
                // else
                // {
                var permissionScopeManager = new BasePermissionScopeManager(dbHelper, userInfo);
                var userIds = permissionScopeManager.GetUserIds(userInfo.SystemCode, userInfo.Id, "Resource.ManagePermission");
                // dt = logManager.GetDataTableByDateByUserIds(userIds, BaseLogEntity.FieldProcessId, processId, beginDate, endDate);
                // }
                result.TableName = BaseLogEntity.TableName;
            });

            return result;
        }
        #endregion

        #region public DataTable GetDataTableByUser(BaseUserInfo userInfo, string userId, string beginDate, string endDate) 按用户获取日志
        /// <summary>
        /// 按用户获取日志
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="userId">用户主键</param>
        /// <param name="beginDate">开始时间</param>
        /// <param name="endDate">结束时间</param>
        /// <returns>数据表</returns>
        public DataTable GetDataTableByUser(BaseUserInfo userInfo, string userId, string beginDate, string endDate)
        {
            var dt = new DataTable(BaseLogEntity.TableName);

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                // var logManager = new BaseLogManager(dbHelper, result);
                // dt = logManager.GetDataTableByDate(BaseLogEntity.FieldUserId, userId, beginDate, endDate);
                dt.TableName = BaseLogEntity.TableName;
            });
            return dt;
        }
        #endregion

        #region public DataTable SearchUserByPage(BaseUserInfo userInfo, out int recordCount, int pageIndex, int pageSize, string permissionCode, string condition, string sort = null);
        /// <summary>
        /// 查询用户列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="recordCount">记录条数</param>
        /// <param name="pageIndex">第几页</param>
        /// <param name="pageSize">每页显示条数</param>
        /// <param name="permissionCode">操作权限</param>
        /// <param name="conditions">条件</param>
        /// <param name="sort">排序</param>
        /// <returns>数据表</returns>
        public DataTable SearchUserByPage(BaseUserInfo userInfo, out int recordCount, int pageIndex, int pageSize, string permissionCode, string conditions, string sort = null)
        {
            var departmentId = string.Empty;
            var myrecordCount = 0;
            var dt = new DataTable(BaseUserEntity.TableName);

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                if (SecretUtil.IsSqlSafe(conditions))
                {
                    var userManager = new BaseUserManager(dbHelper, userInfo)
                    {
                        ShowUserLogOnInfo = true
                    };
                    dt = userManager.SearchLogByPage(out myrecordCount, pageIndex, pageSize, permissionCode, conditions, sort);
                    dt.TableName = BaseUserEntity.TableName;
                }
                else
                {
                    // 记录注入日志
                    LogUtil.WriteLog("userInfo:" + userInfo.Serialize() + " " + conditions, "SqlSafe");
                }
            });
            recordCount = myrecordCount;
            return dt;
        }
        #endregion

        #region public int Delete(BaseUserInfo userInfo, string id) 删除日志
        /// <summary>
        /// 删除日志
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="id">主键</param>
        /// <returns>影响行数</returns>
        public int Delete(BaseUserInfo userInfo, string id)
        {
            var result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                // var manager = new BaseLogManager(dbHelper, result);
                // result = manager.Delete(new List<KeyValuePair<string, object>> { new KeyValuePair<string, object>(BaseLogEntity.FieldId, id) });
            });
            return result;
        }
        #endregion

        #region public int BatchDelete(BaseUserInfo userInfo, string[] ids) 批量删除日志
        /// <summary>
        /// 批量删除日志
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="ids">主键数组</param>
        /// <returns>影响行数</returns>
        public int BatchDelete(BaseUserInfo userInfo, string[] ids)
        {
            var result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                // var manager = new BaseLogManager(dbHelper, result);
                // result = manager.Delete(BaseLogEntity.FieldId, ids);
            });
            return result;
        }
        #endregion

        #region public void Truncate(BaseUserInfo userInfo) 全部清除日志
        /// <summary>
        /// 全部清除日志
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <returns>数据表</returns>
        public void Truncate(BaseUserInfo userInfo)
        {
            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                // var manager = new BaseLogManager(dbHelper, result);
                // manager.Truncate();
            });
        }
        #endregion

        #region public DataTable GetDataTableApplicationByDate(BaseUserInfo userInfo, string beginDate, string endDate) 按日期获取日志（业务）
        /// <summary>
        /// 按日期获取日志（业务）
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="beginDate">开始时间</param>
        /// <param name="endDate">结束时间</param>
        /// <returns>数据表</returns>
        public DataTable GetDataTableApplicationByDate(BaseUserInfo userInfo, string beginDate, string endDate)
        {
            var dt = new DataTable(BaseLogEntity.TableName);

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessBusinessDb(userInfo, parameter, (dbHelper) =>
            {
                // var manager = new BaseLogManager(dbHelper, result);
                // dt = manager.GetDataTableByDate(string.Empty, string.Empty, beginDate, endDate);
                dt.TableName = BaseLogEntity.TableName;
            });
            return dt;
        }
        #endregion

        #region public int BatchDeleteApplication(BaseUserInfo userInfo, string[] ids) 批量删除日志(业务)
        /// <summary>
        /// 批量删除日志(业务)
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="ids">主键数组</param>
        /// <returns>影响行数</returns>
        public int BatchDeleteApplication(BaseUserInfo userInfo, string[] ids)
        {
            var result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessBusinessDb(userInfo, parameter, (dbHelper) =>
            {
                // var manager = new BaseLogManager(dbHelper, result);
                // result = manager.Delete(BaseLogEntity.FieldId, ids);
            });
            return result;
        }
        #endregion

        #region public void TruncateApplication(BaseUserInfo userInfo) 全部清除日志(业务)
        /// <summary>
        /// 全部清除日志(业务)
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <returns>数据表</returns>
        public void TruncateApplication(BaseUserInfo userInfo)
        {
            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessBusinessDb(userInfo, parameter, (dbHelper) =>
            {
                // var manager = new BaseLogManager(dbHelper, result);
                // manager.Truncate();
            });
        }
        #endregion
    }
}