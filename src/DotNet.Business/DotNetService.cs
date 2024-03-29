﻿//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2023, DotNet.
//-----------------------------------------------------------------

using System;


namespace DotNet.Business
{
    using Util;
    using IService;

    /// <summary>
    /// DotNetService
    /// 
    /// 修改记录
    /// 
    ///		2011.08.21 版本：2.0 JiRiGaLa 方便在系统组件化用,命名进行了修改。
    ///		2007.12.27 版本：1.0 JiRiGaLa 创建。
    ///		
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2011.08.21</date>
    /// </author> 
    /// </summary>

    public partial class DotNetService : AbstractServiceFactory
    {
        // private static readonly string servicePath = BaseSystemInfo.Service;
        // private static string servicePath = BaseSystemInfo.Service;
        private static readonly string ServiceFactoryClass = BaseSystemInfo.ServiceFactory;
        /// <summary>
        /// DotNet服务
        /// </summary>
        public DotNetService()
        {
            _serviceFactory = GetServiceFactory(BaseSystemInfo.Service, ServiceFactoryClass);
        }

        /// <summary>
        /// 可以从外部指定调用哪个服务
        /// </summary>
        /// <param name="service">实现的服务</param>
        public DotNetService(string service)
        {
            BaseSystemInfo.Service = service;
            _serviceFactory = GetServiceFactory(BaseSystemInfo.Service, ServiceFactoryClass);
        }

        private IServiceFactory _serviceFactory = null;
        /// <summary>
        /// 初始化服务
        /// </summary>
        public void InitService()
        {
            _serviceFactory.InitService();
        }


        // 这里不能继续用单实例了，会遇到WCF回收资源的问题

        private static DotNetService _instance = null;
        private static object _locker = new Object();
        /// <summary>
        /// 实例
        /// </summary>
        public static DotNetService Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_locker)
                    {
                        if (_instance == null)
                        {
                            _instance = new DotNetService();
                        }
                    }
                }
                return _instance;
            }
        }
        /// <summary>
        /// 登录服务
        /// </summary>
        public virtual IBaseUserLogonService LogonService => _serviceFactory.CreateBaseLogonService();
        /// <summary>
        /// 序号服务
        /// </summary>
        public virtual IBaseSequenceService SequenceService => _serviceFactory.CreateBaseSequenceService();
        /// <summary>
        /// 用户服务
        /// </summary>
        public virtual IBaseUserService UserService => _serviceFactory.CreateBaseUserService();

        /// <summary>
        /// 日志服务
        /// </summary>
        public virtual IBaseLogService LogService => _serviceFactory.CreateBaseLogService();

        /// <summary>
        /// 异常服务
        /// </summary>
        public virtual IBaseExceptionService ExceptionService => _serviceFactory.CreateBaseExceptionService();

        /// <summary>
        /// 权限服务
        /// </summary>
        public virtual IBasePermissionService PermissionService => _serviceFactory.CreateBasePermissionService();

        /// <summary>
        /// 组织机构服务
        /// </summary>
        public virtual IBaseOrganizationService OrganizationService => _serviceFactory.CreateBaseOrganizationService();

        ///// <summary>
        ///// 字典服务
        ///// </summary>
        //public virtual IBaseItemsService BaseItemsService => _serviceFactory.CreateBaseBaseItemsService();

        ///// <summary>
        ///// 字典明细服务
        ///// </summary>
        //public virtual IBaseItemDetailsService BaseItemDetailsService => _serviceFactory.CreateBaseBaseItemDetailsService();

        ///// <summary>
        ///// 字典服务
        ///// </summary>
        //public virtual IBaseItemsService ItemsService => _serviceFactory.CreateBaseItemsService();

        ///// <summary>
        ///// 字典明细服务
        ///// </summary>
        //public virtual IBaseItemDetailsService ItemDetailsService => _serviceFactory.CreateBaseItemDetailsService();

        /// <summary>
        /// 模块菜单服务
        /// </summary>
        public virtual IBaseModuleService ModuleService => _serviceFactory.CreateBaseModuleService();

        /// <summary>
        /// 修改记录服务
        /// </summary>
        public virtual IBaseChangeLogService ChangeLogService => _serviceFactory.CreateBaseChangeLogService();

        /// <summary>
        /// 员工服务
        /// </summary>
        public virtual IBaseStaffService StaffService => _serviceFactory.CreateBaseStaffService();

        /// <summary>
        /// 角色服务
        /// </summary>
        public virtual IBaseRoleService RoleService => _serviceFactory.CreateBaseRoleService();

        /// <summary>
        /// 参数服务
        /// </summary>
        public virtual IBaseParameterService ParameterService => _serviceFactory.CreateBaseParameterService();
    }
}