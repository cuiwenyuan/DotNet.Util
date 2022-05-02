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
        /// <typeparam name="Service">服务</typeparam>
        /// <typeparam name="IService">服务泛型</typeparam>
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
        /// 创建登录服务
        /// </summary>
        /// <returns></returns>
        public virtual IBaseUserLogonService CreateLogonService()
        {
            return new LogonService();
        }

        /// <summary>
        /// 创建序列服务
        /// </summary>
        /// <returns></returns>
        public virtual IBaseSequenceService CreateSequenceService()
        {
            return new BaseSequenceService();
        }

        /// <summary>
        /// 创建用户服务
        /// </summary>
        /// <returns></returns>
        public virtual IBaseUserService CreateUserService()
        {
            return new BaseUserService();
        }

        /// <summary>
        /// 创建日志服务
        /// </summary>
        /// <returns></returns>
        public virtual IBaseLogService CreateLogService()
        {
            return new LogService();
        }

        /// <summary>
        /// 创建异常服务
        /// </summary>
        /// <returns></returns>
        public virtual IBaseExceptionService CreateExceptionService()
        {
            return new BaseExceptionService();
        }

        /// <summary>
        /// 创建权限服务
        /// </summary>
        /// <returns></returns>
        public virtual IBasePermissionService CreatePermissionService()
        {
            return new BasePermissionService();
        }

        /// <summary>
        /// 创建组织机构服务
        /// </summary>
        /// <returns></returns>
        public virtual IBaseOrganizationService CreateOrganizationService()
        {
            return new BaseOrganizationService();
        }

        /// <summary>
        /// 创建字典服务
        /// </summary>
        /// <returns></returns>
        public virtual IBaseDictionaryService CreateBaseDictionaryService()
        {
            return new BaseDictionaryService();
        }

        /// <summary>
        /// 创建字典明细服务
        /// </summary>
        /// <returns></returns>
        public virtual IBaseDictionaryItemService CreateBaseDictionaryItemService()
        {
            return new BaseDictionaryItemService();
        }

        /// <summary>
        /// 创建模块菜单服务
        /// </summary>
        /// <returns></returns>
        public virtual IBaseModuleService CreateModuleService()
        {
            return new BaseModuleService();
        }

        /// <summary>
        /// 创建修改记录服务
        /// </summary>
        /// <returns></returns>
        public virtual IBaseChangeLogService CreateChangeLogService()
        {
            return new BaseChangeLogService();
        }

        /// <summary>
        /// 创建员工服务
        /// </summary>
        /// <returns></returns>
        public virtual IBaseStaffService CreateStaffService()
        {
            return new BaseStaffService();
        }

        /// <summary>
        /// 创建角色服务
        /// </summary>
        /// <returns></returns>
        public virtual IBaseRoleService CreateRoleService()
        {
            return new BaseRoleService();
        }

        /// <summary>
        /// 创建参数服务
        /// </summary>
        /// <returns></returns>
        public virtual IBaseParameterService CreateParameterService()
        {
            return new BaseParameterService();
        }
    }
}