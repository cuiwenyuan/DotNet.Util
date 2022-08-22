//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System.Data;


namespace DotNet.IService
{
    using Util;

    /// <summary>
    /// IBaseLogService
    /// 
    /// 修改记录
    /// 
    ///		2008.04.02 版本：1.0 JiRiGaLa 添加接口定义。
    ///		2011.03.27 版本：1.1 JiRiGaLa 离开时的日志记录WriteExit函数补充。
    ///		
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2008.04.02</date>
    /// </author> 
    /// </summary>
    public partial interface IBaseLogService
    {
        /// <summary>
        /// 写入日志
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="processId">服务</param>
        /// <param name="processName">服务名称</param>
        /// <param name="methodId">操作</param>
        /// <param name="methodName">操作名称</param>
        void WriteLog(BaseUserInfo userInfo, string processId, string processName, string methodId, string methodName);

        /// <summary>
        /// 离开时的日志记录
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="logId">日志主键</param>
        void WriteExit(BaseUserInfo userInfo, string logId);

        /// <summary>
        /// 重置访问情况
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="ids">日志主键</param>
        /// <returns>影响行数</returns>
        int ResetVisitInfo(BaseUserInfo userInfo, string[] ids);

        /// <summary>
        /// 按日期获取日志
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="beginDate">开始时间</param>
        /// <param name="endDate">结束时间</param>
        /// <param name="userId">用户主键</param>
        /// <param name="moduleId">模块主键</param>
        /// <param name="processId"></param> 
        /// <returns>数据表</returns>
        DataTable GetDataTableByDate(BaseUserInfo userInfo, string beginDate, string endDate, string userId, string moduleId, string processId = null);

        /// <summary>
        /// 按模块获取日志
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="moduleId">模块主键</param>
        /// <param name="beginDate">开始时间</param>
        /// <param name="endDate">结束时间</param>
        /// <returns>数据表</returns>
        DataTable GetDataTableByModule(BaseUserInfo userInfo, string moduleId, string beginDate, string endDate);

        /// <summary>
        /// 按员工获取日志
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="userId">用户主键</param>
        /// <param name="beginDate">开始时间</param>
        /// <param name="endDate">结束时间</param>
        /// <returns>数据表</returns>
        DataTable GetDataTableByUser(BaseUserInfo userInfo, string userId, string beginDate, string endDate);

        /// <summary>
        /// 查询用户列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="recordCount">记录条数</param>
        /// <param name="pageNo">当前页</param>
        /// <param name="pageSize">每页显示条数</param>
        /// <param name="permissionCode">操作权限</param>
        /// <param name="whereClause">条件</param>
        /// <param name="sort">排序</param>
        /// <returns>数据表</returns>
        DataTable SearchUserByPage(BaseUserInfo userInfo, out int recordCount, int pageNo, int pageSize, string permissionCode, string whereClause, string sort = null);

        /// <summary>
        /// 清除日志
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <returns>数据表</returns>
        void Truncate(BaseUserInfo userInfo);

        /// <summary>
        /// 按日期获取日志（业务）
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="beginDate">开始时间</param>
        /// <param name="endDate">结束时间</param>
        /// <returns>数据表</returns>
        DataTable GetDataTableApplicationByDate(BaseUserInfo userInfo, string beginDate, string endDate);

        /// <summary>
        /// 批量删除日志(业务)
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="ids">主键数组</param>
        /// <returns>影响行数</returns>
        int BatchDeleteApplication(BaseUserInfo userInfo, string[] ids);

        /// <summary>
        /// 清除日志(业务)
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <returns>数据表</returns>
        void TruncateApplication(BaseUserInfo userInfo);

        /// <summary>
        /// 删除日志
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="id">主键</param>
        /// <returns>影响行数</returns>
        int Delete(BaseUserInfo userInfo, string id);

        /// <summary>
        /// 批量删除日志
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="ids">主键数组</param>
        /// <returns>影响行数</returns>
        int BatchDelete(BaseUserInfo userInfo, string[] ids);
    }
}