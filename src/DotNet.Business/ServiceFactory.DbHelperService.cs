//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2022, DotNet.
//-----------------------------------------------------------------

namespace DotNet.Business
{
    using IService;

    /// <summary>
    /// ServiceFactory
    /// 本地服务的具体实现接口
    /// 
    /// 修改记录
    /// 
    ///		2015.04.30 版本：3.0 JiRiGaLa 分离方法，提高安全性。
    ///		2011.08.21 版本：2.0 JiRiGaLa 方便在系统组件化用,命名进行了修改。
    ///		2007.12.30 版本：1.0 JiRiGaLa 创建。
    ///		
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2015.04.30</date>
    /// </author> 
    /// </summary>
    public partial class ServiceFactory : IServiceFactory
    {
        /// <summary>
        /// 创建业务数据库服务
        /// </summary>
        /// <returns>数据库服务</returns>
        public virtual IDbHelperService CreateBaseBusinessDbHelperService()
        {
            return new BusinessDbHelperService();
        }

        /// <summary>
        /// 创建用户中心数据库服务
        /// </summary>
        /// <returns>数据库服务</returns>
        public virtual IDbHelperService CreateBaseUserCenterDbHelperService()
        {
            return new UserCenterDbHelperService();
        }

        /// <summary>
        /// 登录日志数据库服务
        /// </summary>
        /// <returns>数据库服务</returns>
        public virtual IDbHelperService CreateBaseLogonLogDbHelperService()
        {
            return new LogonLogDbHelperService();
        }
    }
}