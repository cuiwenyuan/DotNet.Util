//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------



namespace DotNet.Business
{
    using Util;

    /// <summary>
    /// LogonLogDbHelperService
    /// 
    /// 修改记录
    /// 
    ///		2015.04.29 版本：1.0 JiRiGaLa 创建。
    ///		
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2015.04.29</date>
    /// </author> 
    /// </summary>


    public class LogonLogDbHelperService : DbHelperService
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public LogonLogDbHelperService()
        {
            ServiceDbConnection = BaseSystemInfo.LogonLogDbConnection;
            ServiceDbType = BaseSystemInfo.LogonLogDbType;
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbConnection"></param>
        public LogonLogDbHelperService(string dbConnection)
        {
            ServiceDbConnection = dbConnection;
        }
    }
}