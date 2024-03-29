﻿//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2023, DotNet.
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
    /// BaseChangeLogService
    /// 
    /// 修改记录
    /// 
    ///		2015.04.30 版本：3.0 JiRiGaLa 加强QL语句安全漏洞。
    ///		
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2015.04.30</date>
    /// </author> 
    /// </summary>


    public class BaseChangeLogService : IBaseChangeLogService
    {
        /// <summary>
        /// 获取分页数据（防注入功能的）
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        /// <param name="recordCount">记录条数</param>
        /// <param name="tableName">表名</param>
        /// <param name="selectField">选择字段</param>
        /// <param name="pageNo">当前页</param>
        /// <param name="pageSize">每页显示多少条</param>
        /// <param name="conditions">查询条件</param>
        /// <param name="dbParameters">查询参数</param>
        /// <param name="orderBy">排序字段</param>
        /// <returns>数据表</returns>
        public DataTable GetDataTableByPage(BaseUserInfo userInfo, out int recordCount, string tableName, string selectField, int pageNo, int pageSize, string conditions, List<KeyValuePair<string, object>> dbParameters, string orderBy)
        {
            DataTable result = null;

            var myRecordCount = 0;
            var dt = new DataTable(BaseModuleEntity.CurrentTableName);

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                // 判断是否已经登录的用户？
                var userManager = new BaseUserManager(userInfo);
                // 判断是否已经登录的用户？
                if (userManager.UserIsLogon(userInfo))
                {
                    if (SecretUtil.IsSqlSafe(conditions))
                    {
                        myRecordCount = dbHelper.GetCount(tableName, conditions, dbHelper.MakeParameters(dbParameters));
                        result = dbHelper.GetDataTableByPage(tableName, selectField, pageNo, pageSize, conditions, dbHelper.MakeParameters(dbParameters), orderBy);
                    }
                    else
                    {
                        // 记录注入日志
                        LogUtil.WriteLog("userInfo:" + userInfo.Serialize() + " " + conditions, "SqlSafe");
                    }
                }
            });

            recordCount = myRecordCount;

            return result;
        }
    }
}