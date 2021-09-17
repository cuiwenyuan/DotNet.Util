//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System.Collections.Generic;
using System.Data;
using System.Reflection;


namespace DotNet.Business
{
    using IService;
    using Model;
    using Util;

    /// <summary>
    /// ExceptionService
    /// 异常记录服务
    /// 
    /// 修改记录
    /// 
    ///		2013.06.05 张祈璟重构
    ///     2007.06.12 版本：1.1 JiRiGaLa 加入调试信息#if (DEBUG)。
    ///		2007.06.07 版本：1.0 JiRiGaLa 窗体与数据库连接的分离。
    ///		
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2007.06.12</date>
    /// </author> 
    /// </summary>


    public class ExceptionService : IExceptionService
    {
        #region public DataTable GetDataTable(BaseUserInfo userInfo) 获取异常列表
        /// <summary>
        /// 获取异常列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <returns>数据表</returns>
        public DataTable GetDataTable(BaseUserInfo userInfo)
        {
            var dt = new DataTable(BaseExceptionEntity.TableName);

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                var manager = new BaseExceptionManager(dbHelper, userInfo);
                dt = manager.GetDataTable();
                dt.TableName = BaseExceptionEntity.TableName;
            });
            return dt;
        }
        #endregion

        #region public DataTable GetDataTableByPage(BaseUserInfo userInfo, out int recordCount, int pageIndex, int pageSize, string condition, List<KeyValuePair<string, object>> dbParameters, string order = null)
        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="recordCount">记录数</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">每页显示</param>
        /// <param name="condition">条件</param>
        /// <param name="dbParameters">参数</param>
        /// <param name="order">排序</param>
        /// <returns>数据表</returns>
        public DataTable GetDataTableByPage(BaseUserInfo userInfo, out int recordCount, int pageIndex, int pageSize, string condition, List<KeyValuePair<string, object>> dbParameters, string order = null)
        {
            var myRecordCount = 0;
            var dt = new DataTable(BaseExceptionEntity.TableName);

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                if (SecretUtil.IsSqlSafe(condition))
                {
                    var exceptionManager = new BaseExceptionManager(dbHelper, userInfo);
                    dt = exceptionManager.GetDataTableByPage(out myRecordCount, pageIndex, pageSize, condition, dbHelper.MakeParameters(dbParameters), order);
                    dt.TableName = BaseExceptionEntity.TableName;
                }
                else
                {
                    // 记录注入日志
                    LogUtil.WriteLog("userInfo:" + userInfo.Serialize() + " " + condition, "SqlSafe");
                }
            });
            recordCount = myRecordCount;
            return dt;
        }
        #endregion

        #region public DataTable Delete(BaseUserInfo userInfo, string id) 删除异常
        /// <summary>
        /// 删除异常
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="id">主键</param>
        /// <returns>数据表</returns>
        public DataTable Delete(BaseUserInfo userInfo, string id)
        {
            var dt = new DataTable(BaseExceptionEntity.TableName);

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                var manager = new BaseExceptionManager(dbHelper, userInfo);
                manager.Delete(BaseExceptionEntity.FieldId, new string[] { id });
                dt = manager.GetDataTable();
            });
            return dt;
        }
        #endregion

        #region public int BatchDelete(BaseUserInfo userInfo, string[] ids) 批量删除异常
        /// <summary>
        /// 批量删除异常
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="ids">主键数组</param>
        /// <returns>数据表</returns>
        public int BatchDelete(BaseUserInfo userInfo, string[] ids)
        {
            var result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                var manager = new BaseExceptionManager(dbHelper, userInfo);
                result = manager.Delete(BaseExceptionEntity.FieldId, ids);
            });
            return result;
        }
        #endregion

        #region public int Truncate(BaseUserInfo userInfo) 清除全部异常
        /// <summary>
        /// 清除全部异常
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <returns>数据表</returns>
        public int Truncate(BaseUserInfo userInfo)
        {
            var result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                var manager = new BaseExceptionManager(dbHelper, userInfo);
                manager.Truncate();
            });
            return result;
        }
        #endregion
    }
}