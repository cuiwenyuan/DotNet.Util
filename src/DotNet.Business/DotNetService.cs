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
        /// 区域服务
        /// </summary>
        public virtual IAreaService AreaService => _serviceFactory.CreateAreaService();
        /// <summary>
        /// 登录服务
        /// </summary>
        public virtual ILogOnService LogOnService => _serviceFactory.CreateLogOnService();
        /// <summary>
        /// 序号服务
        /// </summary>
        public virtual ISequenceService SequenceService => _serviceFactory.CreateSequenceService();
        /// <summary>
        /// 用户服务
        /// </summary>
        public virtual IUserService UserService => _serviceFactory.CreateUserService();

        /// <summary>
        /// 日志服务
        /// </summary>
        public virtual ILogService LogService => _serviceFactory.CreateLogService();

        /// <summary>
        /// 站点服务
        /// </summary>
        public virtual IStationService StationService => _serviceFactory.CreateStationService();

        /// <summary>
        /// 异常服务
        /// </summary>
        public virtual IExceptionService ExceptionService => _serviceFactory.CreateExceptionService();

        /// <summary>
        /// 权限服务
        /// </summary>
        public virtual IPermissionService PermissionService => _serviceFactory.CreatePermissionService();

        /// <summary>
        /// 服务授权服务
        /// </summary>
        public virtual IServicesLicenseService ServicesLicenseService => _serviceFactory.CreateServicesLicenseService();

        /// <summary>
        /// 组织机构服务
        /// </summary>
        public virtual IOrganizeService OrganizeService => _serviceFactory.CreateOrganizeService();

        /// <summary>
        /// 部门服务
        /// </summary>
        public virtual IDepartmentService DepartmentService => _serviceFactory.CreateDepartmentService();

        /// <summary>
        /// 字典服务
        /// </summary>
        public virtual IBaseItemsService BaseItemsService => _serviceFactory.CreateBaseItemsService();

        /// <summary>
        /// 字典明细服务
        /// </summary>
        public virtual IBaseItemDetailsService BaseItemDetailsService => _serviceFactory.CreateBaseItemDetailsService();

        /// <summary>
        /// 字典服务
        /// </summary>
        public virtual IBaseItemsService ItemsService => _serviceFactory.CreateItemsService();

        /// <summary>
        /// 字典明细服务
        /// </summary>
        public virtual IBaseItemDetailsService ItemDetailsService => _serviceFactory.CreateItemDetailsService();

        /// <summary>
        /// 模块菜单服务
        /// </summary>
        public virtual IModuleService ModuleService => _serviceFactory.CreateModuleService();

        /// <summary>
        /// 手机服务
        /// </summary>
        public virtual IMobileService MobileService => _serviceFactory.CreateMobileService();

        /// <summary>
        /// 修改记录服务
        /// </summary>
        public virtual IModifyRecordService ModifyRecordService => _serviceFactory.CreateModifyRecordService();

        /// <summary>
        /// 员工服务
        /// </summary>
        public virtual IStaffService StaffService => _serviceFactory.CreateStaffService();

        /// <summary>
        /// 角色服务
        /// </summary>
        public virtual IRoleService RoleService => _serviceFactory.CreateRoleService();

        /// <summary>
        /// 语言服务
        /// </summary>
        public virtual ILanguageService LanguageService => _serviceFactory.CreateLanguageService();

        /// <summary>
        /// 消息服务
        /// </summary>
        public virtual IMessageService MessageService => _serviceFactory.CreateMessageService();

        /// <summary>
        /// 文件服务
        /// </summary>
        public virtual IFileService FileService => _serviceFactory.CreateFileService();

        /// <summary>
        /// 文件夹服务
        /// </summary>
        public virtual IFolderService FolderService => _serviceFactory.CreateFolderService();

        /// <summary>
        /// 参数服务
        /// </summary>
        public virtual IParameterService ParameterService => _serviceFactory.CreateParameterService();

        /// <summary>
        /// 表字段结构
        /// </summary>
        /// <returns>服务接口</returns>
        public virtual ITableColumnsService TableColumnsService => _serviceFactory.CreateTableColumnsService();
    }
}