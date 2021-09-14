//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------



namespace DotNet.Business
{
    using Util;

    /// <summary>
    /// LoginLogDbHelperService
    /// 
    /// 修改纪录
    /// 
    ///		2015.04.29 版本：1.0 JiRiGaLa 创建。
    ///		
    /// <author>
    ///		<name>JiRiGaLa</name>
    ///		<date>2015.04.29</date>
    /// </author> 
    /// </summary>


    public class LoginLogDbHelperService : DbHelperService
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public LoginLogDbHelperService()
        {
            ServiceDbConnection = BaseSystemInfo.LoginLogDbConnection;
            ServiceDbType = BaseSystemInfo.LoginLogDbType;
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbConnection"></param>
        public LoginLogDbHelperService(string dbConnection)
        {
            ServiceDbConnection = dbConnection;
        }
    }
}