//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System;
using System.Reflection;

namespace DotNet.Business
{
    using Util;
    using IService;

    /// <summary>
    /// AbstractServiceFactory
    /// 
    /// 修改记录
    ///
    ///     2019.12.27 版本：1.0 Troy.Cui 优化。
    ///		2007.12.27 版本：1.0 JiRiGaLa 创建。
    ///		
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2007.12.27</date>
    /// </author> 
    /// </summary>
    public abstract partial class AbstractServiceFactory: MarshalByRefObject
    {
        // Look up the Dao implementation we should be using
        // private static readonly string serviceFactoryClass = ConfigurationManager.AppSettings["ServiceFactory"];
        /// <summary>
        /// 获取服务工厂
        /// </summary>
        public AbstractServiceFactory() 
        {
        }
        /// <summary>
        /// 获取服务工厂
        /// </summary>
        /// <returns></returns>
        public virtual IServiceFactory GetServiceFactory()
        {
            return GetServiceFactory(BaseSystemInfo.Service);
        }
        /// <summary>
        /// 获取服务工厂
        /// </summary>
        /// <param name="servicePath">服务路径</param>
        /// <returns></returns>
        public virtual IServiceFactory GetServiceFactory(string servicePath)
        {
            var className = servicePath + "." + BaseSystemInfo.ServiceFactory;
            return (IServiceFactory)Assembly.Load(servicePath).CreateInstance(className);
        }
        /// <summary>
        /// 获取服务工厂
        /// </summary>
        /// <param name="servicePath">服务路径</param>
        /// <param name="serviceFactoryClass">服务工厂类</param>
        /// <returns></returns>
        public virtual IServiceFactory GetServiceFactory(string servicePath, string serviceFactoryClass)
        {
            var className = servicePath + "." + serviceFactoryClass;
            return (IServiceFactory)Assembly.Load(servicePath).CreateInstance(className);
        }
    }
}