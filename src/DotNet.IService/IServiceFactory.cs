//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2023, DotNet.
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
        /// 创建选项明细服务
        /// </summary>
        /// <returns>服务接口</returns>
        IBaseDictionaryService CreateBaseBaseDictionaryService();

        /// <summary>
        /// 创建选项服务
        /// </summary>
        /// <returns>服务接口</returns>
        IBaseDictionaryItemService CreateBaseBaseDictionaryItemService();

        /// <summary>
        /// 创建业务数据库服务
        /// </summary>
        /// <returns>服务接口</returns>
        IDbHelperService CreateBaseBusinessDbHelperService();

        /// <summary>
        /// 创建用户中心数据库服务
        /// </summary>
        /// <returns>服务接口</returns>
        IDbHelperService CreateBaseUserCenterDbHelperService();

        /// <summary>
        /// 登录日志数据库服务
        /// </summary>
        /// <returns>服务接口</returns>
        IDbHelperService CreateBaseLogonLogDbHelperService();

        /// <summary>
        /// 创建异常服务
        /// </summary>
        /// <returns>服务接口</returns>
        IBaseExceptionService CreateBaseExceptionService();

        /// <summary>
        /// 创建登录服务
        /// </summary>
        /// <returns>服务接口</returns>
        IBaseUserLogonService CreateBaseLogonService();

        /// <summary>
        /// 创建用户服务
        /// </summary>
        /// <returns>服务接口</returns>
        IBaseUserService CreateBaseUserService();
        
        /// <summary>
        /// 创建日志服务
        /// </summary>
        /// <returns>服务接口</returns>
        IBaseLogService CreateBaseLogService();

        /// <summary>
        /// 创建模块服务
        /// </summary>
        /// <returns>服务接口</returns>
        IBaseModuleService CreateBaseModuleService();

        /// <summary>
        /// 修改记录
        /// </summary>
        /// <returns>服务接口</returns>
        IBaseChangeLogService CreateBaseChangeLogService();

        /// <summary>
        /// 创建组织机构服务
        /// </summary>
        /// <returns>服务接口</returns>
        IBaseOrganizationService CreateBaseOrganizationService();

        /// <summary>
        /// 创建参数服务
        /// </summary>
        /// <returns>服务接口</returns>
        IBaseParameterService CreateBaseParameterService();

        /// <summary>
        /// 创建权限管理服务
        /// </summary>
        /// <returns>服务接口</returns>
        IBasePermissionService CreateBasePermissionService();

        /// <summary>
        /// 创建角色服务
        /// </summary>
        /// <returns>服务接口</returns>
        IBaseRoleService CreateBaseRoleService();

        /// <summary>
        /// 创建序列服务
        /// </summary>
        /// <returns>服务接口</returns>
        IBaseSequenceService CreateBaseSequenceService();

        /// <summary>
        /// 创建员工服务
        /// </summary>
        /// <returns>服务接口</returns>
        IBaseStaffService CreateBaseStaffService();
    }
}