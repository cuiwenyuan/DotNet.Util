//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2023, DotNet.
//-----------------------------------------------------------------



namespace DotNet.Business
{
    using Util;

    /// <summary>
    /// BusinessDbHelperService
    /// 业务数据库服务
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


    public class BusinessDbHelperService : DbHelperService
    {
        /// <summary>
        /// BusinessDbHelperService
        /// </summary>
        public BusinessDbHelperService()
        {
            ServiceDbConnection = BaseSystemInfo.BusinessDbConnection;
            ServiceDbType = BaseSystemInfo.BusinessDbType;
        }
        /// <summary>
        /// BusinessDbHelperService
        /// </summary>
        /// <param name="dbConnection"></param>
        public BusinessDbHelperService(string dbConnection)
        {
            ServiceDbConnection = dbConnection;
        }
    }
}