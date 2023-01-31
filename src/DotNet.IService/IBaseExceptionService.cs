//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2023, DotNet.
//-----------------------------------------------------------------

using System.Collections.Generic;
using System.Data;


namespace DotNet.IService
{
    using Util;

    /// <summary>
    /// IBaseExceptionService
    /// 
    /// 修改记录
    /// 
    ///		2008.04.02 版本：1.0 JiRiGaLa 添加接口定义。
    ///		
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2008.04.02</date>
    /// </author> 
    /// </summary>
    public partial interface IBaseExceptionService
    {
        /// <summary>
        /// 获取异常列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <returns>数据表</returns>
        DataTable GetDataTable(BaseUserInfo userInfo);

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="recordCount">记录数</param>
        /// <param name="pageNo">当前页</param>
        /// <param name="pageSize">每页显示</param>
        /// <param name="whereClause">条件</param>
        /// <param name="dbParameters">参数</param>
        /// <param name="order">排序</param>
        /// <returns>数据表</returns>
        DataTable GetDataTableByPage(BaseUserInfo userInfo, out int recordCount, int pageNo, int pageSize, string whereClause, List<KeyValuePair<string, object>> dbParameters, string order = null);

        /// <summary>
        /// 批量删除异常
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="id">主键</param>
        /// <returns>数据表</returns>
        DataTable Delete(BaseUserInfo userInfo, string id);

        /// <summary>
        /// 批量删除异常
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="ids">主键数组</param>
        /// <returns>影响行数</returns>
        int BatchDelete(BaseUserInfo userInfo, string[] ids);

        /// <summary>
        /// 清除
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <returns>影响行数</returns>
        int Truncate(BaseUserInfo userInfo);
    }
}