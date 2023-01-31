//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2023, DotNet.
//-----------------------------------------------------------------



namespace DotNet.Business
{
    using Util;

    /// <summary>
    /// UserCenterDbHelperService
    /// 
    /// 修改记录
    /// 
    ///		2011.05.07 版本：1.0 JiRiGaLa 创建。
    ///		
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2011.05.07</date>
    /// </author> 
    /// </summary>


    public class UserCenterDbHelperService : DbHelperService
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public UserCenterDbHelperService()
        {
            ServiceDbConnection = BaseSystemInfo.UserCenterDbConnection;
            ServiceDbType = BaseSystemInfo.UserCenterDbType;
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbConnection"></param>
        public UserCenterDbHelperService(string dbConnection)
        {
            ServiceDbConnection = dbConnection;
        }
    }
}