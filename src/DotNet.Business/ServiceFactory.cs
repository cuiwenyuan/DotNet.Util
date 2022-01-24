//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
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
    ///		2011.08.21 版本：2.0 JiRiGaLa 方便在系统组件化用,命名进行了修改。
    ///		2007.12.30 版本：1.0 JiRiGaLa 创建。
    ///		
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2011.08.21</date>
    /// </author> 
    /// </summary>
    public partial class ServiceFactory : IServiceFactory
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <typeparam name="Service"></typeparam>
        /// <typeparam name="IService"></typeparam>
        /// <returns></returns>
		public IService CreateService<Service, IService>() where Service : IService, new()
        {
            return new Service();
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <returns></returns>
		public TService CreateService<TService>() where TService : new()
        {
            return new TService();
        }

        /// <summary>
        /// 初始化服务
        /// </summary>
        public void InitService()
        {
        }

        /// <summary>
        /// 创建区域服务
        /// </summary>
        /// <returns></returns>
        public virtual IAreaService CreateAreaService()
        {
            return new AreaService();
        }

        /// <summary>
        /// 创建登录服务
        /// </summary>
        /// <returns></returns>
        public virtual ILogonService CreateLogonService()
        {
            return new LogonService();
        }

        /// <summary>
        /// 创建序列服务
        /// </summary>
        /// <returns></returns>
        public virtual ISequenceService CreateSequenceService()
        {
            return new SequenceService();
        }

        /// <summary>
        /// 创建用户服务
        /// </summary>
        /// <returns></returns>
        public virtual IUserService CreateUserService()
        {
            return new UserService();
        }

        /// <summary>
        /// 创建日志服务
        /// </summary>
        /// <returns></returns>
        public virtual ILogService CreateLogService()
        {
            return new LogService();
        }

        /// <summary>
        /// 创建站点服务
        /// </summary>
        /// <returns></returns>
        public virtual IStationService CreateStationService()
        {
            return new StationService();
        }

        /// <summary>
        /// 创建异常服务
        /// </summary>
        /// <returns></returns>
        public virtual IExceptionService CreateExceptionService()
        {
            return new ExceptionService();
        }

        /// <summary>
        /// 创建权限服务
        /// </summary>
        /// <returns></returns>
        public virtual IPermissionService CreatePermissionService()
        {
            return new PermissionService();
        }

        /// <summary>
        /// 创建组织机构服务
        /// </summary>
        /// <returns></returns>
        public virtual IOrganizationService CreateOrganizationService()
        {
            return new OrganizationService();
        }

        /// <summary>
        /// 创建部门服务
        /// </summary>
        /// <returns></returns>
        public virtual IDepartmentService CreateDepartmentService()
        {
            return new DepartmentService();
        }

        /// <summary>
        /// 创建字典服务
        /// </summary>
        /// <returns></returns>
        public virtual IBaseItemsService CreateBaseItemsService()
        {
            return new BaseItemsService();
        }

        /// <summary>
        /// 创建字典明细服务
        /// </summary>
        /// <returns></returns>
        public virtual IBaseItemDetailsService CreateBaseItemDetailsService()
        {
            return new BaseItemDetailsService();
        }

        /// <summary>
        /// 创建字典服务
        /// </summary>
        /// <returns></returns>
        public virtual IBaseItemsService CreateItemsService()
        {
            return new ItemsService();
        }

        /// <summary>
        /// 创建字典明细服务
        /// </summary>
        /// <returns></returns>
        public virtual IBaseItemDetailsService CreateItemDetailsService()
        {
            return new ItemDetailsService();
        }

        /// <summary>
        /// 创建模块菜单服务
        /// </summary>
        /// <returns></returns>
        public virtual IModuleService CreateModuleService()
        {
            return new ModuleService();
        }

        /// <summary>
        /// 创建手机服务
        /// </summary>
        /// <returns></returns>
        public virtual IMobileService CreateMobileService()
        {
            return new MobileService();
        }

        /// <summary>
        /// 创建修改记录服务
        /// </summary>
        /// <returns></returns>
        public virtual IModifyRecordService CreateModifyRecordService()
        {
            return new ModifyRecordService();
        }

        /// <summary>
        /// 创建员工服务
        /// </summary>
        /// <returns></returns>
        public virtual IStaffService CreateStaffService()
        {
            return new StaffService();
        }

        /// <summary>
        /// 创建角色服务
        /// </summary>
        /// <returns></returns>
        public virtual IRoleService CreateRoleService()
        {
            return new RoleService();
        }

        /// <summary>
        /// 创建语言服务
        /// </summary>
        /// <returns></returns>
        public virtual ILanguageService CreateLanguageService()
        {
            return new LanguageService();
        }

        /// <summary>
        /// 创建消息服务
        /// </summary>
        /// <returns></returns>
        public virtual IMessageService CreateMessageService()
        {
            return new MessageService();
        }

        /// <summary>
        /// 创建文件服务
        /// </summary>
        /// <returns></returns>
        public virtual IFileService CreateFileService()
        {
            return new FileService();
        }

        /// <summary>
        /// 创建文件夹服务
        /// </summary>
        /// <returns></returns>
        public virtual IFolderService CreateFolderService()
        {
            return new FolderService();
        }

        /// <summary>
        /// 创建参数服务
        /// </summary>
        /// <returns></returns>
        public virtual IParameterService CreateParameterService()
        {
            return new ParameterService();
        }

        /// <summary>
        /// 创建服务授权服务
        /// </summary>
        /// <returns></returns>
        public virtual IServicesLicenseService CreateServicesLicenseService()
        {
            return new ServicesLicenseService();
        }

        /// <summary>
        /// 创建表字段服务
        /// </summary>
        /// <returns></returns>
        public virtual ITableColumnsService CreateTableColumnsService()
        {
            return new TableColumnsService();
        }
    }
}