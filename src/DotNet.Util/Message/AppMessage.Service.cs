//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2023, DotNet.
//-----------------------------------------------------------------

namespace DotNet.Util
{
    /// <summary>
    ///	AppMessage
    /// 通用讯息控制基类
    /// 
    /// 修改记录
    ///		2007.05.17 版本：1.0	JiRiGaLa 建立，为了提高效率分开建立了类。
    ///	
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2007.05.17</date>
    /// </author> 
    /// </summary>
    public partial class AppMessage
    {
        //// ExceptionService异常纪录服务及相关的方法名称定义
        //public static string ExceptionService = "异常纪录服务";
        //public static string ExceptionServiceGetDataTable = "取得异常列表";
        //public static string ExceptionServiceBatchDelete = "删除异常";
        //public static string ExceptionServiceDelete = "批量删除异常";
        //public static string ExceptionServiceTruncate = "清除全部异常";

        // FileService档案服务及相关的方法名称定义
        /// <summary>
        /// 档案服务
        /// </summary>
        public static string FileService = "档案服务";

        /// <summary>
        /// 系统创建目录
        /// </summary>
        public static string FileServiceSystemCreateDirectory = "系统创建目录";

        /// <summary>
        /// 公司文档
        /// </summary>
        public static string FileServiceCompanyFile = "公司文档";

        /// <summary>
        /// 公共文档
        /// </summary>
        public static string FileServiceShareFolder = "公共文档";

        /// <summary>
        /// 用户空间
        /// </summary>
        public static string FileServiceUserSpace = "用户空间";

        /// <summary>
        ///  的文件夾
        /// </summary>
        public static string FileServiceFolder = " 的文件夾";

        /// <summary>
        /// 发送的文件
        /// </summary>
        public static string FileServiceSendFile = "发送的文件";

        /// <summary>
        ///  的文件
        /// </summary>
        public static string FileServiceFile = " 的文件";

        /// <summary>
        /// 收到的文件
        /// </summary>
        public static string FileServiceReceiveFile = "收到的文件";

        /// <summary>
        ///  发送文件 
        /// </summary>
        public static string FileServiceSendFileFrom = " 发送文件 ";

        /// <summary>
        /// ，请注意查收。
        /// </summary>
        public static string FileServiceCheckReceiveFile = "，请注意查收。";

        /// <summary>
        /// 取得实体
        /// </summary>
        public static string FileServiceGetEntity = "取得实体";

        /// <summary>
        /// 判断是否存在
        /// </summary>
        public static string FileServiceExists = "判断是否存在";

        /// <summary>
        /// 下载文件
        /// </summary>
        public static string FileServiceDownload = "下载文件";

        /// <summary>
        /// 上传档案
        /// </summary>
        public static string FileServiceUpload = "上传档案";

        /// <summary>
        /// 依文件夹取得档案列表
        /// </summary>
        public static string FileServiceGetDataTableByFolder = "依文件夹取得档案列表";

        /// <summary>
        /// 依文件ID取得档案列表
        /// </summary>
        public static string FileServiceGetDataTableByIds = "依文件ID取得档案列表";

        /// <summary>
        /// 依文件夹删除档案
        /// </summary>
        public static string FileServiceDeleteByFolder = "依文件夹删除档案";

        //// FolderService文件夹服务及相关的方法名称定义
        //public static string FolderService = "文件夹服务";

        //// ItemDetailsService选项明细管理服务及相关的方法名称定义
        //public static string ItemDetailsService = "选项明细管理服务";
        //public static string ItemDetailsServiceGetDataTable = "取得列表";
        //public static string ItemDetailsServiceGetDataTableByParent = "依父节点取得列表";
        //public static string ItemDetailsServiceGetDataTableByCode = "依编号取得列表";
        //public static string ItemDetailsServiceGetDsByCodes = "批量取得资料";
        //public static string ItemDetailsServiceGetEntity = "取得实体";
        //public static string ItemDetailsServiceAdd = "新增实体";
        //public static string ItemDetailsServiceUpdate = "更新实体";
        //public static string ItemDetailsServiceDelete = "删除实体";
        //public static string ItemDetailsServiceBatchMoveTo = "批量移动";
        //public static string ItemDetailsServiceBatchDelete = "批量删除";
        //public static string ItemDetailsServiceBatchSave = "批量储存";
        //public static string ItemDetailsServiceBatchSetSortCode = "批量重新产生排序码";

        //// ItemsService选项管理服务及相关的方法名称定义
        //public static string ItemsService = "选项管理服务";
        //public static string ItemsServiceGetDataTable = "取得列表";
        //public static string ItemsServiceGetEntity = "取得实体";
        //public static string ItemsServiceGetDataTableByParent = "依父节点取得列表";
        //public static string ItemsServiceAdd = "新增实体";
        //public static string ItemsServiceUpdate = "更新实体";
        //public static string ItemsServiceCreateTable = "新增数据表";
        //public static string ItemsServiceDelete = "删除实体";
        //public static string ItemsServiceBatchMoveTo = "批量移动";
        //public static string ItemsServiceBatchDelete = "批量删除";
        //public static string ItemsServiceBatchSave = "批量储存";

        //// LogonService登入服务及相关的方法名称定义
        //public static string LogonService = "登入服务";
        //public static string LogonServiceGetEntity = "登入服务获取实体";
        //public static string LogonServiceUpdate = "登入服务更新实体";
        //public static string LogonServiceGetUserDt = "取得用户列表";
        //public static string LogonServiceGetStaffUserDt = "取得内部员工列表";
        //public static string LogonServiceOnline = "用户在线报导";
        //public static string LogonServiceSignOut = "用户退出";
        //public static string LogonServiceLockUser = "锁定用户";
        //public static string LogonServiceSetPassword = "设定用户密码";
        //public static string LogonServiceChangePassword = "用户变更密码";
        //public static string LogonServiceChangeCommunicationPassword = "用户变更通讯密码";
        //public static string LogonServiceCommunicationPassword = "验证用户通讯密码";
        //public static string LogonServiceCreateDigitalSignature = "建立数字证书签名";
        //public static string LogonServiceGetPublicKey = "取得当前用户的公钥";
        //public static string LogonServiceChangeSignedPassword = "用户变更签名密码";
        //public static string LogonServiceSignedPassword = "验证用户数字签名密码";

        //// LogService日志服务及相关的方法名称定义
        //public static string LogService = "日志服务";
        //public static string LogServiceGetLogGeneral = "取得用户访问情况日志";
        //public static string LogServiceResetVisitInfo = "重置用户访问情况";
        //public static string LogServiceGetDataTableByDate = "依日期取得日志";
        //public static string LogServiceGetDataTableByModule = "依菜单取得日志";
        //public static string LogServiceGetDataTableByUser = "依用户取得日志";
        //public static string LogServiceDelete = "删除日志";
        //public static string LogServiceBatchDelete = "批量删除日志";
        //public static string LogServiceTruncate = "清除全部日志";
        //public static string LogServiceGetDataTableApplicationByDate = "依日期取得日志(商务)";
        //public static string LogServiceBatchDeleteApplication = "批量删除日志(商务)";
        //public static string LogServiceTruncateApplication = "清除全部日志(商务)";

        //public static string AreaService = "区域服务";

        //// MessageService讯息服务及相关的方法名称定义
        //public static string MessageService = "讯息服务";
        //public static string MessageServiceGetInnerOrganization = "取得内部组织机构";
        //public static string MessageServiceGetUser = "取得用户信息";
        //public static string MessageServiceGetUserDtByDepartment = "按部门取得用户信息";
        //public static string MessageServiceBatchSend = "批量发送站内讯息";
        //public static string MessageServiceSend = "发送讯息";
        //public static string MessageServiceMessageChek = "取得讯息状态";
        //public static string MessageServiceReadFromReceiver = "取得特定用户的新讯息";
        //public static string MessageServiceRead = "阅读讯息";

        //// ModuleService菜单服务及相关的方法名称定义
        //public static string ModuleService = "菜单服务";
        //public static string ModuleServiceGetDataTable = "取得列表";
        //public static string ModuleServiceGetEntity = "取得实体";
        //public static string ModuleServiceGetNameByCode = "透过编号取得菜单名称";
        //public static string ModuleServiceAdd = "新增菜单";
        //public static string ModuleServiceUpdate = "更新菜单";
        //public static string ModuleServiceGetDataTableByParent = "依父节点取得列表";
        //public static string ModuleServiceDelete = "删除菜单";
        //public static string ModuleServiceBatchDelete = "批量删除";
        //public static string ModuleServiceSetDeleted = "批量设置删除";
        //public static string ModuleServiceMoveTo = "移动菜单";
        //public static string ModuleServiceBatchMoveTo = "批量移动";
        //public static string ModuleServiceBatchSave = "批量储存";
        //public static string ModuleServiceSetSortCode = "储存排序顺序";
        //public static string ModuleServiceGetPermissionDt = "取得关联的权限项列表";
        //public static string ModuleServiceGetIdsByPermission = "依操作权限项取得关联的菜单主键";

        //public static string ModuleServiceBatchAddPermissions = "菜单批量新增关联操作权限项";
        //public static string ModuleServiceBatchAddModules = "新增操作权限项关联菜单";
        //public static string ModuleServiceBatchDletePermissions = "删除菜单与操作权限项的关联";
        //public static string ModuleServiceBatchDleteModules = "删除操作权限项与菜单的关联";

        //public static string DepartmentService = "部门服务";

        //// OrganizationService组织机构服务及相关的方法名称定义
        //public static string OrganizationService = "组织机构服务";
        //public static string OrganizationServiceAdd = "新增实体";
        //public static string OrganizationServiceAddByDetail = "依明细情况新增实体";
        //public static string OrganizationServiceGetEntity = "取得实体";
        //public static string OrganizationServiceGetDataTable = "取得列表";
        //public static string OrganizationServiceGetDataTableByParent = "依父节点取得列表";
        //public static string OrganizationServiceGetInnerOrganizationDt = "取得内部组织机构";
        //public static string OrganizationServiceGetCompanyDt = "取得公司列表";
        //public static string OrganizationServiceGetDepartmentDt = "取得部门列表";
        //public static string OrganizationServiceGetArea = "取得区域列表";
        //public static string OrganizationServiceGetProvince = "取得省份列表";
        //public static string OrganizationServiceGetCity = "取得城市列表";
        //public static string OrganizationServiceGetDistrict = "取得县区列表";
        //public static string OrganizationServiceGetOrganizationByProvince = "按省份取得组织机构列表";
        //public static string OrganizationServiceGetOrganizationByCity = "按城市取得组织机构列表";
        //public static string OrganizationServiceGetOrganizationByDistrict = "按县区取得组织机构列表";
        //public static string OrganizationServiceSearch = "查询组织机构";
        //public static string OrganizationServiceUpdate = "更新组织机构";
        //public static string OrganizationServiceDelete = "删除组织机构";
        //public static string OrganizationServiceBatchDelete = "批量删除";
        //public static string OrganizationServiceSetDeleted = "批量设置删除";
        //public static string OrganizationServiceBatchSave = "批量储存";
        //public static string OrganizationServiceMoveTo = "移动组织机构";
        //public static string OrganizationServiceBatchMoveTo = "批量移动";
        //public static string OrganizationServiceBatchSetCode = "批量重新产生编号";
        //public static string OrganizationServiceBatchSetSortCode = "批量重新产生排序码";

        //// ParameterService参数服务及相关的方法名称定义
        //public static string ParameterService = "参数服务";
        //public static string ParameterServiceGetParameter = "取得参数值";
        //public static string ParameterServiceSetParameter = "设置参数值";
        //public static string ParameterServiceAdd = "参数服务添加实体";
        //public static string ParameterServiceGetDataTableByParameter = "取得列表";
        //public static string ParameterServiceGetDataTableParameterCode = "按编号取得列表";
        //public static string ParameterServiceDeleteByParameter = "删除参数";
        //public static string ParameterServiceDeleteByParameterCode = "按参数编号删除";
        //public static string ParameterServiceDelete = "删除参数";
        //public static string ParameterServiceBatchDelete = "批量删除参数";

        //// PermissionService权限判断服务及相关的方法名称定义
        //public static string PermissionService = "权限服务";
        //public static string PermissionServiceGetEntityByCode = "按编号获取模块实体";
        //public static string PermissionServiceIsInRole = "用户是否在指定的角色中";
        //public static string PermissionServiceIsAuthorized = "该用户是否有相应的操作权限";
        //public static string PermissionServiceCheckPermissionByRole = "该角色是否有相应的操作权限";
        //public static string PermissionServiceIsAdministratorByUser = "该用户是否为超级管理员";
        //public static string PermissionServiceGetPermissionDtByUser = "取得该用户的所有权限列表";
        //public static string PermissionServiceIsModuleAuthorized = "当前用户是否对某个菜单有相应的权限";
        //public static string PermissionServiceIsModuleAuthorizedByUser = "该用户是否对某个菜单有相应的权限";
        //public static string PermissionServiceGetUserPermissionScope = "取得用户的数据权限范围";
        //public static string PermissionServiceGetOrganizationDtByPermission = "依某个权限域取得组织机构列表";
        //public static string PermissionServiceGetOrganizationIdsByPermission = "依某个数据权限取得组织机构主键数组";
        //public static string PermissionServiceGetRoleDtByPermission = "依某个权限域取得角色列表";
        //public static string PermissionServiceGetRoleIdsByPermission = "按权限取得角色数组列表";
        //public static string PermissionServiceGetUserDtByPermission = "依某个权限域取得用户列表";
        //public static string PermissionServiceGetUserIdsByPermission = "依某个数据权限取得用户主键数组";
        //public static string PermissionServiceGetModuleDtByPermission = "依某个权限域取得菜单列表";
        //public static string PermissionServiceGetPermissionDtByPermission = "用户的所有可授权范围(有授权权限的权限列表)";
        //public static string PermissionServiceGetRolePermissionIds = "取得角色权限主键数组";
        //public static string PermissionServiceGrantRolePermissions = "授予角色的权限";
        //public static string PermissionServiceGrantRolePermissionById = "授予角色的权限";
        //public static string PermissionServiceRevokeRolePermissions = "删除角色的权限";
        //public static string PermissionServiceClearRolePermission = "清除角色权限";
        //public static string PermissionServiceRevokeRolePermissionById = "删除角色的权限";
        //public static string PermissionServiceGetRoleScopeUserIds = "取得角色的某个权限域的组织范围";
        //public static string PermissionServiceGetRoleScopeRoleIds = "取得角色的某个权限域的组织范围";
        //public static string PermissionServiceGetRoleScopeOrganizationIds = "取得角色的某个权限域的组织范围";
        //public static string PermissionServiceGrantRoleUserScopes = "授予角色的某个权限域的组织范围";
        //public static string PermissionServiceRevokeRoleUserScopes = "删除角色的某个权限域的组织范围";
        //public static string PermissionServiceGrantRoleRoleScopes = "授予角色的某个权限域的组织范围";
        //public static string PermissionServiceRevokeRoleRoleScopes = "删除角色的某个权限域的组织范围";
        //public static string PermissionServiceGrantRoleOrganizationScopes = "授予角色的某个权限域的组织范围";
        //public static string PermissionServiceRevokeRoleOrganizationScopes = "删除角色的某个权限域的组织范围";
        //public static string PermissionServiceGetRoleScopePermissionIds = "取得角色授权权限列表";
        //public static string PermissionServiceGrantRolePermissionScopes = "授予角色的授权权限范围";
        //public static string PermissionServiceRevokeRolePermissionScopes = "授予角色的授权权限范围";
        //public static string PermissionServiceClearRolePermissionScope = "清除角色权限范围";
        //public static string PermissionServiceGetUserPermissionIds = "取得用户权力主键数组";
        //public static string PermissionServiceGrantUserPermissions = "授予用户操作权限";
        //public static string PermissionServiceGrantUserPermissionById = "授予用户操作权限";
        //public static string PermissionServiceRevokeUserPermission = "删除用户操作权限";
        //public static string PermissionServiceRevokeUserPermissionById = "删除用户操作权限";
        //public static string PermissionServiceGetUserScopeOrganizationIds = "取得用户的某个权限域的组织范围";
        //public static string PermissionServiceGrantUserOrganizationScopes = "设置用户的某个权限域的组织范围";
        //public static string PermissionServiceRevokeUserOrganizationScopes = "设置用户的某个权限域的组织范围";
        //public static string PermissionServiceGetUserScopeUserIds = "取得用户的某个权限域的组织范围";
        //public static string PermissionServiceGrantUserUserScopes = "设置用户的某个权限域的组织范围";
        //public static string PermissionServiceRevokeUserUserScopes = "设置用户的某个权限域的用户范围";
        //public static string PermissionServiceGetUserScopeRoleIds = "取得用户的某个权限域的用户范围";
        //public static string PermissionServiceGrantUserRoleScopes = "设置用户的某个权限域的用户范围";
        //public static string PermissionServiceRevokeUserRoleScopes = "设置用户的某个权限域的用户范围";
        //public static string PermissionServiceGetUserScopePermissionIds = "取得用户授权权限列表";
        //public static string PermissionServiceGrantUserPermissionScopes = "授予用户的授权权限范围";
        //public static string PermissionServiceRevokeUserPermissionScopes = "删除用户的授权权限范围";
        //public static string PermissionServiceClearUserPermission = "清除用户权力";
        //public static string PermissionServiceClearUserPermissionScope = "清除用户权力范围";
        //public static string PermissionServiceGetPermissionListByUser = "取得用户有访问权限的菜单列表";
        //public static string PermissionServiceGetUserScopeModuleIds = "取得用户菜单权限范围主键数组";
        //public static string PermissionServiceGrantUserModuleScopes = "授予用户菜单的权限范围";
        //public static string PermissionServiceGrantUserModuleScope = "授予用户菜单的权限范围";
        //public static string PermissionServiceRevokeUserModuleScope = "删除用户菜单的权限范围";
        //public static string PermissionServiceRevokeUserModuleScopes = "删除用户菜单的权限范围";
        //public static string PermissionServiceGetPermissionTreeUserIds = "获取用户权限树";
        //public static string PermissionServiceGetRoleScopeModuleIds = "取得用户菜单权限范围主键数组";
        //public static string PermissionServiceGrantRoleModuleScopes = "授予用户菜单的权限范围";
        //public static string PermissionServiceGrantRoleModuleScope = "授予用户菜单的权限范围";
        //public static string PermissionServiceRevokeRoleModuleScopes = "删除用户菜单的权限范围";
        //public static string PermissionServiceRevokeRoleModuleScope = "删除用户菜单的权限范围";

        //public static string PermissionServiceGetOrganizationPermissionIds = "取得组织机构权限主键数组";
        //public static string PermissionServiceGrantOrganizationPermissions = "授予组织机构的权限";
        //public static string PermissionServiceGrantOrganizationPermissionById = "授予组织机构的权限";
        //public static string PermissionServiceRevokeOrganizationPermissions = "删除组织机构的权限";
        //public static string PermissionServiceRevokeOrganizationPermissionById = "删除组织机构的权限";
        //public static string PermissionServiceClearOrganizationPermission = "清除组织机构权限";
        //public static string PermissionServiceGetOrganizationScopeModuleIds = "取得组织机构菜单权限范围主键数组";
        //public static string PermissionServiceGrantOrganizationModuleScopes = "授予组织机构菜单的权限范围";
        //public static string PermissionServiceGrantOrganizationModuleScope = "授予组织机构菜单的权限范围";
        //public static string PermissionServiceRevokeOrganizationModuleScopes = "删除组织机构菜单的权限范围";
        //public static string PermissionServiceRevokeOrganizationModuleScope = "删除组织机构菜单的权限范围";
        //public static string PermissionServiceGetUserOrganizationScope = "获取用户的某个权限域的组织范围";
        //public static string PermissionServiceSetUserOrganizationScope = "设置用户某个权限的组织机构范围";
        //public static string PermissionServiceGetRoleOrganizationScope = "获取角色的某个权限域的组织范围";
        //public static string PermissionServiceSetRoleOrganizationScope = "设置角色某个权限的组织机构范围";

        //public static string PermissionServiceGetResourcePermissionIds = "取得资源权限主键数组";
        //public static string PermissionServiceGetUserPermissionScopeList = "取得用户资源权限列表";
        //public static string PermissionServiceGetRolePermissionScopeList = "取得角色资源权限列表";
        //public static string PermissionServiceGrantResourcePermission = "授予资源的权限";
        //public static string PermissionServiceRevokeResourcePermission = "删除资源的权限";
        //public static string PermissionServiceGetPermissionScopeTargetIds = "取得资源权限范围主键数组";
        //public static string PermissionServiceRevokePermissionScopeTargets = "删除资源的权限范围";
        //public static string PermissionServiceClearPermissionScopeTarget = "删除资源的权限范围";
        //public static string PermissionServiceGetResourceScopeIds = "取得用户的某个资源的权限范围";
        //public static string PermissionServiceGetTreeResourceScopeIds = "取得用户的某个资源的权限范围(树型资源)";

        //// RoleService角色管理服务及相关的方法名称定义
        //public static string RoleService = "角色管理服务";
        //public static string RoleServiceAdd = "新增角色";
        //public static string RoleServiceGetDataTable = "取得列表";
        //public static string RoleServiceGetDataTableByOrganization = "依组织机构取得角色列表";
        //public static string RoleServiceGetEntity = "取得实体";
        //public static string RoleServiceUpdate = "更新实体";
        //public static string RoleServiceGetDataTableByIds = "依主键数组取得角色列表";
        //public static string RoleServiceSearch = "查询角色列表";
        //public static string RoleServiceBatchSave = "批量储存角色";
        //public static string RoleServiceMoveTo = "移动角色数据";
        //public static string RoleServiceBatchMoveTo = "批量移动角色数据";
        //public static string RoleServiceResetSortCode = "排序角色顺序";
        //public static string RoleServiceGetRoleUserIds = "取得角色中的用户主键";
        //public static string RoleServiceAddUserToRole = "用户新增至角色";
        //public static string RoleServiceRemoveUserFromRole = "将用户从角色中移除";
        //public static string RoleServiceDelete = "删除角色";
        //public static string RoleServiceBatchDelete = "批量删除角色";
        //public static string RoleServiceSetDeleted = "批量设置删除";
        //public static string RoleServiceClearRoleUser = "清除角色用户关联";
        //public static string RoleServiceSetUsersToRole = "设置角色中的用户";

        //// SequenceService序列管理服务及相关的方法名称定义
        //public static string SequenceService = "序列管理服务";
        //public static string SequenceServiceAdd = "新增序列";
        //public static string SequenceServiceGetDataTable = "取得列表";
        //public static string SequenceServiceGetEntity = "取得实体";
        //public static string SequenceServiceUpdate = "更新序列";
        //public static string SequenceServiceReset = "批量重置序列";
        //public static string SequenceServiceDelete = "删除序列";
        //public static string SequenceServiceBatchDelete = "批量删除序列";

        //// StaffService职员管理服务及相关的方法名称定义
        //public static string StaffService = "职员管理服务";
        //public static string StaffServiceGetAddressDt = "取得内部通讯簿";
        //public static string StaffServiceGetAddressPageDt = "取得内部通讯簿";
        //public static string StaffServiceUpdateAddress = "更新通讯地址";
        //public static string StaffServiceBatchUpdateAddress = "批量更新通讯地址";
        //public static string StaffServiceAddStaff = "新增职员";
        //public static string StaffServiceUpdateStaff = "更新职员";
        //public static string StaffServiceGetDataTable = "取得列表";
        //public static string StaffServiceGetEntity = "取得实体";
        //public static string StaffServiceGetDataTableByIds = "取得职员列表";
        //public static string StaffServiceGetDataTableByCompany = "按公司取得列表";
        //public static string StaffServiceGetDataTableByDepartment = "按部门取得列表";
        //public static string StaffServiceGetDataTableByOrganization = "按组织机构取得列表";
        //public static string StaffServiceSetStaffUser = "职员关联用户";

        //// TableColumnsService表字段权限服务及相关的方法名称定义
        //public static string TableColumnsService = "表字段权限服务";
        //public static string TableColumnsServiceGetDataTableByTable = "依表明取得字段列表";
        //public static string TableColumnsServiceGetConstraintDt = "取得约束条件(所有的约束)";
        //public static string TableColumnsServiceGetUserConstraint = "取得当前用户的约束条件";
        //public static string TableColumnsServiceSetConstraint = "设置约束条件";
        //public static string TableColumnsServiceBatchDeleteConstraint = "批量删除";

        //// UserService用户管理服务及相关的方法名称定义
        /// <summary>
        /// 用户管理服务
        /// </summary>
        public static string UserService = "用户管理服务";

        /// <summary>
        /// 请审核。
        /// </summary>
        public static string UserServiceCheck = "请审核。";

        /// <summary>
        ///  申请帐户：
        /// </summary>
        public static string UserServiceApplication = " 申请帐户：";

        /// <summary>
        /// 新增用户
        /// </summary>
        public static string UserServiceAddUser = "新增用户";

        /// <summary>
        /// 依部门取得用户列表
        /// </summary>
        public static string UserServiceGetDataTableByDepartment = "依部门取得用户列表";

        /// <summary>
        /// 取得实体
        /// </summary>
        public static string UserServiceGetEntity = "取得实体";

        /// <summary>
        /// 取得列表
        /// </summary>
        public static string UserServiceGetDataTable = "取得列表";

        /// <summary>
        /// 依角色取得列表
        /// </summary>
        public static string UserServiceGetDataTableByRole = "依角色取得列表";

        /// <summary>
        /// 依主键取得列表
        /// </summary>
        public static string UserServiceGetDataTableByIds = "依主键取得列表";

        /// <summary>
        /// 取得用户的角色列表
        /// </summary>
        public static string UserServiceGetRoleDataTable = "取得用户的角色列表";

        /// <summary>
        /// 判断用户是否在某个角色中
        /// </summary>
        public static string UserServiceUserInRole = "判断用户是否在某个角色中";

        /// <summary>
        /// 更新用户
        /// </summary>
        public static string UserServiceUpdateUser = "更新用户";

        /// <summary>
        /// 查询用户
        /// </summary>
        public static string UserServiceSearch = "查询用户";

        /// <summary>
        /// 设置用户审核状态
        /// </summary>
        public static string UserServiceSetUserAuditStates = "设置用户审核状态";

        /// <summary>
        /// 设置用户的预设角色
        /// </summary>
        public static string UserServiceSetDefaultRole = "设置用户的预设角色";
    }
}