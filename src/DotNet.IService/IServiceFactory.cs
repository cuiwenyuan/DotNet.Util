//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

namespace DotNet.IService
{
    /// <summary>
    /// IServiceFactory
    /// 服务工厂接口定义
    /// 
    /// 修改记录
    /// 
    ///	    2013.06.07 版本：3.1 JiRiGaLa 整理函数顺序，这里用到了设计模式的闭包。
    ///	    2011.05.07 版本：3.0 JiRiGaLa 整理目录结构。
    ///	    2011.04.30 版本：2.0 JiRiGaLa 修改注释。
    ///	    2007.12.30 版本：1.0 JiRiGaLa 创建。
    ///	    
    /// <author>
    ///     <name>Troy.Cui</name>
    ///     <date>2013.06.07</date>
    /// </author> 
    /// </summary>
    public partial interface IServiceFactory
    {
        /// <summary>
        /// 初始化服务
        /// </summary>
        void InitService();

        /// <summary>
        /// 区域服务
        /// </summary>
        /// <returns>服务接口</returns>
        IAreaService CreateAreaService();
        
        /// <summary>
        /// 创建选项明细服务
        /// </summary>
        /// <returns>服务接口</returns>
        IBaseItemDetailsService CreateBaseItemDetailsService();

        /// <summary>
        /// 创建选项服务
        /// </summary>
        /// <returns>服务接口</returns>
        IBaseItemsService CreateBaseItemsService();
        
        /// <summary>
        /// 创建选项服务
        /// </summary>
        /// <returns>服务接口</returns>
        IBaseItemsService CreateItemsService();

        /// <summary>
        /// 创建选项明细服务
        /// </summary>
        /// <returns>服务接口</returns>
        IBaseItemDetailsService CreateItemDetailsService();

        /// <summary>
        /// 创建业务数据库服务
        /// </summary>
        /// <returns>服务接口</returns>
        IDbHelperService CreateBusinessDbHelperService();

        /// <summary>
        /// 创建用户中心数据库服务
        /// </summary>
        /// <returns>服务接口</returns>
        IDbHelperService CreateUserCenterDbHelperService();

        /// <summary>
        /// 登录日志数据库服务
        /// </summary>
        /// <returns>服务接口</returns>
        IDbHelperService CreateLoginLogDbHelperService();

        /// <summary>
        /// 创建异常服务
        /// </summary>
        /// <returns>服务接口</returns>
        IExceptionService CreateExceptionService();

        /// <summary>
        /// 创建文件服务
        /// </summary>
        /// <returns>服务接口</returns>
        IFileService CreateFileService();

        /// <summary>
        /// 创建目录服务
        /// </summary>
        /// <returns>服务接口</returns>
        IFolderService CreateFolderService();
        
        /// <summary>
        /// 创建登录服务
        /// </summary>
        /// <returns>服务接口</returns>
        ILogOnService CreateLogOnService();

        /// <summary>
        /// 创建用户服务
        /// </summary>
        /// <returns>服务接口</returns>
        IUserService CreateUserService();
        
        /// <summary>
        /// 创建日志服务
        /// </summary>
        /// <returns>服务接口</returns>
        ILogService CreateLogService();

        /// <summary>
        /// 创建计算机服务
        /// </summary>
        /// <returns>服务接口</returns>
        IStationService CreateStationService();

        /// <summary>
        /// 创建消息服务
        /// </summary>
        /// <returns>服务接口</returns>
        IMessageService CreateMessageService();

        /// <summary>
        /// 创建模块服务
        /// </summary>
        /// <returns>服务接口</returns>
        IModuleService CreateModuleService();

        /// <summary>
        /// 创建手机短信服务
        /// </summary>
        /// <returns>服务接口</returns>
        IMobileService CreateMobileService();

        /// <summary>
        /// 修改记录
        /// </summary>
        /// <returns>服务接口</returns>
        IModifyRecordService CreateModifyRecordService();

        /// <summary>
        /// 创建组织机构服务
        /// </summary>
        /// <returns>服务接口</returns>
        IOrganizeService CreateOrganizeService();

        /// <summary>
        /// 创建部门管理服务
        /// </summary>
        /// <returns>服务接口</returns>
        IDepartmentService CreateDepartmentService();

        /// <summary>
        /// 创建参数服务
        /// </summary>
        /// <returns>服务接口</returns>
        IParameterService CreateParameterService();

        /// <summary>
        /// 创建权限管理服务
        /// </summary>
        /// <returns>服务接口</returns>
        IPermissionService CreatePermissionService();

        /// <summary>
        /// 创建接口调用服务
        /// </summary>
        /// <returns>服务接口</returns>
        IServicesLicenseService CreateServicesLicenseService();
        
        /// <summary>
        /// 创建角色服务
        /// </summary>
        /// <returns>服务接口</returns>
        IRoleService CreateRoleService();

        /// <summary>
        /// 创建多语言服务
        /// </summary>
        /// <returns>服务接口</returns>
        ILanguageService CreateLanguageService();

        /// <summary>
        /// 创建序列服务
        /// </summary>
        /// <returns>服务接口</returns>
        ISequenceService CreateSequenceService();

        /// <summary>
        /// 创建员工服务
        /// </summary>
        /// <returns>服务接口</returns>
        IStaffService CreateStaffService();

        /// <summary>
        /// 表字段结构
        /// </summary>
        /// <returns>服务接口</returns>
        ITableColumnsService CreateTableColumnsService();


		//IService CreateService<Service, IService>() where Service : IService, new();

		//Service CreateService<Service>() where Service : new();

        ///// <summary>
        ///// 创建名片服务
        ///// </summary>
        ///// <returns>服务接口</returns>
        // IBusinessCardService CreateBusinessCardService();
    }
}