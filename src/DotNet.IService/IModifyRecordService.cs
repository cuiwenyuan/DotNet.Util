//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System.Collections.Generic;
using System.Data;


namespace DotNet.IService
{
    using Util;

    /// <summary>
    /// IModifyRecordService
    /// 数据库访问通用类标准接口。
    /// 
    /// 修改记录
    /// 
    ///		2015.04.30 版本：1.0 JiRiGaLa 支持调用存储过程的扩展。
    /// 
    /// <author>
    ///		<name>JiRiGaLa</name>
    ///		<date>2015.04.30</date>
    /// </author> 
    /// </summary>
    
    // [ServiceKnownType(typeof(System.DBNull))]
    // [ServiceKnownType(typeof(System.Data.SqlClient.SqlParameter))]
    // [ServiceKnownType(typeof(Oracle.DataAccess.Client.OracleParameter))]
    
    public partial interface IModifyRecordService
    {
        /// <summary>
        /// 获取分页数据（防注入功能的）
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="recordCount">记录条数</param>
        /// <param name="tableName">数据来源表名</param>
        /// <param name="selectField">选择字段</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">每页显示多少条</param>
        /// <param name="conditions">查询条件</param>
        /// <param name="dbParameters">查询参数</param>
        /// <param name="orderBy">排序字段</param>
        /// <returns>数据表</returns>
        DataTable GetDataTableByPage(BaseUserInfo userInfo, out int recordCount, string tableName, string selectField, int pageIndex, int pageSize, string conditions, List<KeyValuePair<string, object>> dbParameters, string orderBy);        
    }
}