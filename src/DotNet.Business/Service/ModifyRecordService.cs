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
    /// ModifyRecordService
    /// 执行传入的SQL语句
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


    public class ModifyRecordService : IModifyRecordService
    {
        /// <summary>
        /// 获取分页数据（防注入功能的）
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        /// <param name="recordCount">记录条数</param>
        /// <param name="tableName">数据来源表名</param>
        /// <param name="selectField">选择字段</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">每页显示多少条</param>
        /// <param name="conditions">查询条件</param>
        /// <param name="dbParameters">查询参数</param>
        /// <param name="orderBy">排序字段</param>
        /// <returns>数据表</returns>
        public DataTable GetDataTableByPage(BaseUserInfo userInfo, out int recordCount, string tableName, string selectField, int pageIndex, int pageSize, string conditions, List<KeyValuePair<string, object>> dbParameters, string orderBy)
        {
            DataTable result = null;

            var myRecordCount = 0;
            var dt = new DataTable(BaseModuleEntity.TableName);

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                // 判断是否已经登录的用户？
                var userManager = new BaseUserManager(userInfo);
                // 判断是否已经登录的用户？
                if (userManager.UserIsLogOn(userInfo))
                {
                    if (SecretUtil.IsSqlSafe(conditions))
                    {
                        myRecordCount = DbUtil.GetCount(dbHelper, tableName, conditions, dbHelper.MakeParameters(dbParameters));
                        result = DbUtil.GetDataTableByPage(dbHelper, tableName, selectField, pageIndex, pageSize, conditions, dbHelper.MakeParameters(dbParameters), orderBy);
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