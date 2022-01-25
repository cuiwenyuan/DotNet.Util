//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System;

namespace DotNet.Business
{
    using Util;
    using IService;

    /// <summary>
    /// DotNetService
    /// 
    /// 修改纪录
    ///
    ///		2015.04.30 版本：3.0 JiRiGaLa 分离方法，提高安全性。
    ///		2011.08.21 版本：2.0 JiRiGaLa 方便在系统组件化用,命名进行了修改。
    ///		2007.12.27 版本：1.0 JiRiGaLa 创建。
    ///		
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2011.08.21</date>
    /// </author> 
    /// </summary>
    public partial class DotNetService : AbstractServiceFactory		//, IDotNetService
    {
        /// <summary>
        /// 业务数据服务
        /// </summary>
        public virtual IDbHelperService BusinessDbHelperService
        {
            get
            {
                return _serviceFactory.CreateBusinessDbHelperService();
            }
        }

        /// <summary>
        /// 用户中心服务
        /// </summary>
        public virtual IDbHelperService UserCenterDbHelperService
        {
            get
            {
                return _serviceFactory.CreateUserCenterDbHelperService();
            }
        }

        /// <summary>
        /// 登录日志服务
        /// </summary>
        public virtual IDbHelperService LoginLogDbHelperService
        {
            get
            {
                return _serviceFactory.CreateLogonLogDbHelperService();
            }
        }
    }
}