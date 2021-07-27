//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2020, DotNet.
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
    ///		<name>Troy Cui</name>
    ///		<date>2007.05.17</date>
    /// </author> 
    /// </summary>
    public partial class AppMessage
    {
        // ExceptionService异常纪录服务及相关的方法名称定义
        public static string ExceptionService = "异常纪录服务";
        public static string ExceptionServiceGetDataTable = "取得异常列表";
        public static string ExceptionServiceBatchDelete = "删除异常";
        public static string ExceptionServiceDelete = "批量删除异常";
        public static string ExceptionServiceTruncate = "清除全部异常";

        // FileService档案服务及相关的方法名称定义
        public static string FileService = "档案服务";
        public static string FileServiceSystemCreateDirectory = "系统创建目录";
        public static string FileServiceCompanyFile = "公司文档";
        public static string FileServiceShareFolder = "公共文档";
        public static string FileServiceUserSpace = "用户空间";
        public static string FileServiceFolder = " 的文件夾";
        public static string FileServiceSendFile = "发送的文件";
        public static string FileServiceFile = " 的文件";
        public static string FileServiceReceiveFile = "收到的文件";
        public static string FileServiceSendFileFrom = " 发送文件 ";
        public static string FileServiceCheckReceiveFile = "，请注意查收。";
        public static string FileServiceGetObject = "取得实体";
        public static string FileServiceExists = "判断是否存在";
        public static string FileServiceDownload = "下载文件";
        public static string FileServiceUpload = "上传档案";
        public static string FileServiceGetDataTableByFolder = "依文件夹取得档案列表";
        public static string FileServiceGetDataTableByIds = "依文件ID取得档案列表";
        public static string FileServiceDeleteByFolder = "依文件夹删除档案";

        // FolderService文件夹服务及相关的方法名称定义
        public static string FolderService = "文件夹服务";

        // ItemDetailsService选项明细管理服务及相关的方法名称定义
        public static string ItemDetailsService = "选项明细管理服务";
        public static string ItemDetailsServiceGetDataTable = "取得列表";
        public static string ItemDetailsServiceGetDataTableByParent = "依父节点取得列表";
        public static string ItemDetailsServiceGetDataTableByCode = "依编号取得列表";
        public static string ItemDetailsServiceGetDsByCodes = "批量取得资料";
        public static string ItemDetailsServiceGetObject = "取得实体";
        public static string ItemDetailsServiceAdd = "新增实体";
        public static string ItemDetailsServiceUpdate = "更新实体";
        public static string ItemDetailsServiceDelete = "删除实体";
        public static string ItemDetailsServiceBatchMoveTo = "批量移动";
        public static string ItemDetailsServiceBatchDelete = "批量删除";
        public static string ItemDetailsServiceBatchSave = "批量储存";
        public static string ItemDetailsServiceBatchSetSortCode = "批量重新产生排序码";

        // ItemsService选项管理服务及相关的方法名称定义
        public static string ItemsService = "选项管理服务";
        public static string ItemsServiceGetDataTable = "取得列表";
        public static string ItemsServiceGetObject = "取得实体";
        public static string ItemsServiceGetDataTableByParent = "依父节点取得列表";
        public static string ItemsServiceAdd = "新增实体";
        public static string ItemsServiceUpdate = "更新实体";
        public static string ItemsServiceCreateTable = "新增数据表";
        public static string ItemsServiceDelete = "删除实体";
        public static string ItemsServiceBatchMoveTo = "批量移动";
        public static string ItemsServiceBatchDelete = "批量删除";
        public static string ItemsServiceBatchSave = "批量储存";

        // LogOnService登入服务及相关的方法名称定义
        public static string LogOnService = "登入服务";
        public static string LogOnServiceGetObject = "登入服务获取实体";
        public static string LogOnServiceUpdate = "登入服务更新实体";
        public static string LogOnServiceGetUserDt = "取得用户列表";
        public static string LogOnServiceGetStaffUserDt = "取得内部员工列表";
        public static string LogOnServiceOnLine = "用户在线报导";
        public static string LogOnServiceSignOut = "用户退出";
        public static string LogOnServiceLockUser = "锁定用户";
        public static string LogOnServiceSetPassword = "设定用户密码";
        public static string LogOnServiceChangePassword = "用户变更密码";
        public static string LogOnServiceChangeCommunicationPassword = "用户变更通讯密码";
        public static string LogOnServiceCommunicationPassword = "验证用户通讯密码";
        public static string LogOnServiceCreateDigitalSignature = "建立数字证书签名";
        public static string LogOnServiceGetPublicKey = "取得当前用户的公钥";
        public static string LogOnServiceChangeSignedPassword = "用户变更签名密码";
        public static string LogOnServiceSignedPassword = "验证用户数字签名密码";

        // LogService日志服务及相关的方法名称定义
        public static string LogService = "日志服务";
        public static string LogServiceGetLogGeneral = "取得用户访问情况日志";
        public static string LogServiceResetVisitInfo = "重置用户访问情况";
        public static string LogServiceGetDataTableByDate = "依日期取得日志";
        public static string LogServiceGetDataTableByModule = "依菜单取得日志";
        public static string LogServiceGetDataTableByUser = "依用户取得日志";
        public static string LogServiceDelete = "删除日志";
        public static string LogServiceBatchDelete = "批量删除日志";
        public static string LogServiceTruncate = "清除全部日志";
        public static string LogServiceGetDataTableApplicationByDate = "依日期取得日志(商务)";
        public static string LogServiceBatchDeleteApplication = "批量删除日志(商务)";
        public static string LogServiceTruncateApplication = "清除全部日志(商务)";

        public static string AreaService = "区域服务";

        // MessageService讯息服务及相关的方法名称定义
        public static string MessageService = "讯息服务";
        public static string MessageServiceGetInnerOrganize = "取得内部组织机构";
        public static string MessageServiceGetUser = "取得用户信息";
        public static string MessageServiceGetUserDtByDepartment = "按部门取得用户信息";
        public static string MessageServiceBatchSend = "批量发送站内讯息";
        public static string MessageServiceSend = "发送讯息";
        public static string MessageServiceMessageChek = "取得讯息状态";
        public static string MessageServiceReadFromReceiver = "取得特定用户的新讯息";
        public static string MessageServiceRead = "阅读讯息";

        // ModuleService菜单服务及相关的方法名称定义
        public static string ModuleService = "菜单服务";
        public static string ModuleServiceGetDataTable = "取得列表";
        public static string ModuleServiceGetObject = "取得实体";
        public static string ModuleServiceGetFullNameByCode = "透过编号取得菜单名称";
        public static string ModuleServiceAdd = "新增菜单";
        public static string ModuleServiceUpdate = "更新菜单";
        public static string ModuleServiceGetDataTableByParent = "依父节点取得列表";
        public static string ModuleServiceDelete = "删除菜单";
        public static string ModuleServiceBatchDelete = "批量删除";
        public static string ModuleServiceSetDeleted = "批量设置删除";
        public static string ModuleServiceMoveTo = "移动菜单";
        public static string ModuleServiceBatchMoveTo = "批量移动";
        public static string ModuleServiceBatchSave = "批量储存";
        public static string ModuleServiceSetSortCode = "储存排序顺序";
        public static string ModuleServiceGetPermissionDt = "取得关联的权限项列表";
        public static string ModuleServiceGetIdsByPermission = "依操作权限项取得关联的菜单主键";

        public static string ModuleServiceBatchAddPermissions = "菜单批量新增关联操作权限项";
        public static string ModuleServiceBatchAddModules = "新增操作权限项关联菜单";
        public static string ModuleServiceBatchDletePermissions = "删除菜单与操作权限项的关联";
        public static string ModuleServiceBatchDleteModules = "删除操作权限项与菜单的关联";

        public static string DepartmentService = "部门服务";

        // OrganizeService组织机构服务及相关的方法名称定义
        public static string OrganizeService = "组织机构服务";
        public static string OrganizeServiceAdd = "新增实体";
        public static string OrganizeServiceAddByDetail = "依明细情况新增实体";
        public static string OrganizeServiceGetObject = "取得实体";
        public static string OrganizeServiceGetDataTable = "取得列表";
        public static string OrganizeServiceGetDataTableByParent = "依父节点取得列表";
        public static string OrganizeServiceGetInnerOrganizeDt = "取得内部组织机构";
        public static string OrganizeServiceGetCompanyDt = "取得公司列表";
        public static string OrganizeServiceGetDepartmentDt = "取得部门列表";
        public static string OrganizeServiceGetArea = "取得区域列表";
        public static string OrganizeServiceGetProvince = "取得省份列表";
        public static string OrganizeServiceGetCity = "取得城市列表";
        public static string OrganizeServiceGetDistrict = "取得县区列表";
        public static string OrganizeServiceGetOrganizeByProvince = "按省份取得组织机构列表";
        public static string OrganizeServiceGetOrganizeByCity = "按城市取得组织机构列表";
        public static string OrganizeServiceGetOrganizeByDistrict = "按县区取得组织机构列表";
        public static string OrganizeServiceSearch = "查询组织机构";
        public static string OrganizeServiceUpdate = "更新组织机构";
        public static string OrganizeServiceDelete = "删除组织机构";
        public static string OrganizeServiceBatchDelete = "批量删除";
        public static string OrganizeServiceSetDeleted = "批量设置删除";
        public static string OrganizeServiceBatchSave = "批量储存";
        public static string OrganizeServiceMoveTo = "移动组织机构";
        public static string OrganizeServiceBatchMoveTo = "批量移动";
        public static string OrganizeServiceBatchSetCode = "批量重新产生编号";
        public static string OrganizeServiceBatchSetSortCode = "批量重新产生排序码";

        // ParameterService参数服务及相关的方法名称定义
        public static string ParameterService = "参数服务";
        public static string ParameterServiceGetParameter = "取得参数值";
        public static string ParameterServiceSetParameter = "设置参数值";
        public static string ParameterServiceAdd = "参数服务添加实体";
        public static string ParameterServiceGetDataTableByParameter = "取得列表";
        public static string ParameterServiceGetDataTableParameterCode = "按编号取得列表";
        public static string ParameterServiceDeleteByParameter = "删除参数";
        public static string ParameterServiceDeleteByParameterCode = "按参数编号删除";
        public static string ParameterServiceDelete = "删除参数";
        public static string ParameterServiceBatchDelete = "批量删除参数";

        // PermissionService权限判断服务及相关的方法名称定义
        public static string PermissionService = "权限服务";
        public static string PermissionServiceGetObjectByCode = "按编号获取模块实体";
        public static string PermissionServiceIsInRole = "用户是否在指定的角色中";
        public static string PermissionServiceIsAuthorized = "该用户是否有相应的操作权限";
        public static string PermissionServiceCheckPermissionByRole = "该角色是否有相应的操作权限";
        public static string PermissionServiceIsAdministratorByUser = "该用户是否为超级管理员";
        public static string PermissionServiceGetPermissionDtByUser = "取得该用户的所有权限列表";
        public static string PermissionServiceIsModuleAuthorized = "当前用户是否对某个菜单有相应的权限";
        public static string PermissionServiceIsModuleAuthorizedByUser = "该用户是否对某个菜单有相应的权限";
        public static string PermissionServiceGetUserPermissionScope = "取得用户的数据权限范围";
        public static string PermissionServiceGetOrganizeDtByPermission = "依某个权限域取得组织机构列表";
        public static string PermissionServiceGetOrganizeIdsByPermission = "依某个数据权限取得组织机构主键数组";
        public static string PermissionServiceGetRoleDtByPermission = "依某个权限域取得角色列表";
        public static string PermissionServiceGetRoleIdsByPermission = "按权限取得角色数组列表";
        public static string PermissionServiceGetUserDtByPermission = "依某个权限域取得用户列表";
        public static string PermissionServiceGetUserIdsByPermission = "依某个数据权限取得用户主键数组";
        public static string PermissionServiceGetModuleDtByPermission = "依某个权限域取得菜单列表";
        public static string PermissionServiceGetPermissionDtByPermission = "用户的所有可授权范围(有授权权限的权限列表)";
        public static string PermissionServiceGetRolePermissionIds = "取得角色权限主键数组";
        public static string PermissionServiceGrantRolePermissions = "授予角色的权限";
        public static string PermissionServiceGrantRolePermissionById = "授予角色的权限";
        public static string PermissionServiceRevokeRolePermissions = "删除角色的权限";
        public static string PermissionServiceClearRolePermission = "清除角色权限";
        public static string PermissionServiceRevokeRolePermissionById = "删除角色的权限";
        public static string PermissionServiceGetRoleScopeUserIds = "取得角色的某个权限域的组织范围";
        public static string PermissionServiceGetRoleScopeRoleIds = "取得角色的某个权限域的组织范围";
        public static string PermissionServiceGetRoleScopeOrganizeIds = "取得角色的某个权限域的组织范围";
        public static string PermissionServiceGrantRoleUserScopes = "授予角色的某个权限域的组织范围";
        public static string PermissionServiceRevokeRoleUserScopes = "删除角色的某个权限域的组织范围";
        public static string PermissionServiceGrantRoleRoleScopes = "授予角色的某个权限域的组织范围";
        public static string PermissionServiceRevokeRoleRoleScopes = "删除角色的某个权限域的组织范围";
        public static string PermissionServiceGrantRoleOrganizeScopes = "授予角色的某个权限域的组织范围";
        public static string PermissionServiceRevokeRoleOrganizeScopes = "删除角色的某个权限域的组织范围";
        public static string PermissionServiceGetRoleScopePermissionIds = "取得角色授权权限列表";
        public static string PermissionServiceGrantRolePermissionScopes = "授予角色的授权权限范围";
        public static string PermissionServiceRevokeRolePermissionScopes = "授予角色的授权权限范围";
        public static string PermissionServiceClearRolePermissionScope = "清除角色权限范围";
        public static string PermissionServiceGetUserPermissionIds = "取得用户权力主键数组";
        public static string PermissionServiceGrantUserPermissions = "授予用户操作权限";
        public static string PermissionServiceGrantUserPermissionById = "授予用户操作权限";
        public static string PermissionServiceRevokeUserPermission = "删除用户操作权限";
        public static string PermissionServiceRevokeUserPermissionById = "删除用户操作权限";
        public static string PermissionServiceGetUserScopeOrganizeIds = "取得用户的某个权限域的组织范围";
        public static string PermissionServiceGrantUserOrganizeScopes = "设置用户的某个权限域的组织范围";
        public static string PermissionServiceRevokeUserOrganizeScopes = "设置用户的某个权限域的组织范围";
        public static string PermissionServiceGetUserScopeUserIds = "取得用户的某个权限域的组织范围";
        public static string PermissionServiceGrantUserUserScopes = "设置用户的某个权限域的组织范围";
        public static string PermissionServiceRevokeUserUserScopes = "设置用户的某个权限域的用户范围";
        public static string PermissionServiceGetUserScopeRoleIds = "取得用户的某个权限域的用户范围";
        public static string PermissionServiceGrantUserRoleScopes = "设置用户的某个权限域的用户范围";
        public static string PermissionServiceRevokeUserRoleScopes = "设置用户的某个权限域的用户范围";
        public static string PermissionServiceGetUserScopePermissionIds = "取得用户授权权限列表";
        public static string PermissionServiceGrantUserPermissionScopes = "授予用户的授权权限范围";
        public static string PermissionServiceRevokeUserPermissionScopes = "删除用户的授权权限范围";
        public static string PermissionServiceClearUserPermission = "清除用户权力";
        public static string PermissionServiceClearUserPermissionScope = "清除用户权力范围";
        public static string PermissionServiceGetPermissionListByUser = "取得用户有访问权限的菜单列表";
        public static string PermissionServiceGetUserScopeModuleIds = "取得用户菜单权限范围主键数组";
        public static string PermissionServiceGrantUserModuleScopes = "授予用户菜单的权限范围";
        public static string PermissionServiceGrantUserModuleScope = "授予用户菜单的权限范围";
        public static string PermissionServiceRevokeUserModuleScope = "删除用户菜单的权限范围";
        public static string PermissionServiceRevokeUserModuleScopes = "删除用户菜单的权限范围";
        public static string PermissionServiceGetPermissionTreeUserIds = "获取用户权限树";
        public static string PermissionServiceGetRoleScopeModuleIds = "取得用户菜单权限范围主键数组";
        public static string PermissionServiceGrantRoleModuleScopes = "授予用户菜单的权限范围";
        public static string PermissionServiceGrantRoleModuleScope = "授予用户菜单的权限范围";
        public static string PermissionServiceRevokeRoleModuleScopes = "删除用户菜单的权限范围";
        public static string PermissionServiceRevokeRoleModuleScope = "删除用户菜单的权限范围";

        public static string PermissionServiceGetOrganizePermissionIds = "取得组织机构权限主键数组";
        public static string PermissionServiceGrantOrganizePermissions = "授予组织机构的权限";
        public static string PermissionServiceGrantOrganizePermissionById = "授予组织机构的权限";
        public static string PermissionServiceRevokeOrganizePermissions = "删除组织机构的权限";
        public static string PermissionServiceRevokeOrganizePermissionById = "删除组织机构的权限";
        public static string PermissionServiceClearOrganizePermission = "清除组织机构权限";
        public static string PermissionServiceGetOrganizeScopeModuleIds = "取得组织机构菜单权限范围主键数组";
        public static string PermissionServiceGrantOrganizeModuleScopes = "授予组织机构菜单的权限范围";
        public static string PermissionServiceGrantOrganizeModuleScope = "授予组织机构菜单的权限范围";
        public static string PermissionServiceRevokeOrganizeModuleScopes = "删除组织机构菜单的权限范围";
        public static string PermissionServiceRevokeOrganizeModuleScope = "删除组织机构菜单的权限范围";
        public static string PermissionServiceGetUserOrganizeScope = "获取用户的某个权限域的组织范围";
        public static string PermissionServiceSetUserOrganizeScope = "设置用户某个权限的组织机构范围";
        public static string PermissionServiceGetRoleOrganizeScope = "获取角色的某个权限域的组织范围";
        public static string PermissionServiceSetRoleOrganizeScope = "设置角色某个权限的组织机构范围";

        public static string PermissionServiceGetResourcePermissionIds = "取得资源权限主键数组";
        public static string PermissionServiceGetUserPermissionScopeList = "取得用户资源权限列表";
        public static string PermissionServiceGetRolePermissionScopeList = "取得角色资源权限列表";
        public static string PermissionServiceGrantResourcePermission = "授予资源的权限";
        public static string PermissionServiceRevokeResourcePermission = "删除资源的权限";
        public static string PermissionServiceGetPermissionScopeTargetIds = "取得资源权限范围主键数组";
        public static string PermissionServiceRevokePermissionScopeTargets = "删除资源的权限范围";
        public static string PermissionServiceClearPermissionScopeTarget = "删除资源的权限范围";
        public static string PermissionServiceGetResourceScopeIds = "取得用户的某个资源的权限范围";
        public static string PermissionServiceGetTreeResourceScopeIds = "取得用户的某个资源的权限范围(树型资源)";

        // RoleService角色管理服务及相关的方法名称定义
        public static string RoleService = "角色管理服务";
        public static string RoleServiceAdd = "新增角色";
        public static string RoleServiceGetDataTable = "取得列表";
        public static string RoleServiceGetDataTableByOrganize = "依组织机构取得角色列表";
        public static string RoleServiceGetObject = "取得实体";
        public static string RoleServiceUpdate = "更新实体";
        public static string RoleServiceGetDataTableByIds = "依主键数组取得角色列表";
        public static string RoleServiceSearch = "查询角色列表";
        public static string RoleServiceBatchSave = "批量储存角色";
        public static string RoleServiceMoveTo = "移动角色数据";
        public static string RoleServiceBatchMoveTo = "批量移动角色数据";
        public static string RoleServiceResetSortCode = "排序角色顺序";
        public static string RoleServiceGetRoleUserIds = "取得角色中的用户主键";
        public static string RoleServiceAddUserToRole = "用户新增至角色";
        public static string RoleServiceRemoveUserFromRole = "将用户从角色中移除";
        public static string RoleServiceDelete = "删除角色";
        public static string RoleServiceBatchDelete = "批量删除角色";
        public static string RoleServiceSetDeleted = "批量设置删除";
        public static string RoleServiceClearRoleUser = "清除角色用户关联";
        public static string RoleServiceSetUsersToRole = "设置角色中的用户";

        // SequenceService序列管理服务及相关的方法名称定义
        public static string SequenceService = "序列管理服务";
        public static string SequenceServiceAdd = "新增序列";
        public static string SequenceServiceGetDataTable = "取得列表";
        public static string SequenceServiceGetObject = "取得实体";
        public static string SequenceServiceUpdate = "更新序列";
        public static string SequenceServiceReset = "批量重置序列";
        public static string SequenceServiceDelete = "删除序列";
        public static string SequenceServiceBatchDelete = "批量删除序列";

        // StaffService职员管理服务及相关的方法名称定义
        public static string StaffService = "职员管理服务";
        public static string StaffServiceGetAddressDt = "取得内部通讯簿";
        public static string StaffServiceGetAddressPageDt = "取得内部通讯簿";
        public static string StaffServiceUpdateAddress = "更新通讯地址";
        public static string StaffServiceBatchUpdateAddress = "批量更新通讯地址";
        public static string StaffServiceAddStaff = "新增职员";
        public static string StaffServiceUpdateStaff = "更新职员";
        public static string StaffServiceGetDataTable = "取得列表";
        public static string StaffServiceGetObject = "取得实体";
        public static string StaffServiceGetDataTableByIds = "取得职员列表";
        public static string StaffServiceGetDataTableByCompany = "按公司取得列表";
        public static string StaffServiceGetDataTableByDepartment = "按部门取得列表";
        public static string StaffServiceGetDataTableByOrganize = "按组织机构取得列表";
        public static string StaffServiceSetStaffUser = "职员关联用户";

        // TableColumnsService表字段权限服务及相关的方法名称定义
        public static string TableColumnsService = "表字段权限服务";
        public static string TableColumnsServiceGetDataTableByTable = "依表明取得字段列表";
        public static string TableColumnsServiceGetConstraintDt = "取得约束条件(所有的约束)";
        public static string TableColumnsServiceGetUserConstraint = "取得当前用户的约束条件";
        public static string TableColumnsServiceSetConstraint = "设置约束条件";
        public static string TableColumnsServiceBatchDeleteConstraint = "批量删除";

        // UserService用户管理服务及相关的方法名称定义
        public static string UserService = "用户管理服务";
        public static string UserServiceCheck = "请审核。";
        public static string UserServiceApplication = " 申请帐户：";
        public static string UserServiceAddUser = "新增用户";
        public static string UserServiceGetDataTableByDepartment = "依部门取得用户列表";
        public static string UserServiceGetObject = "取得实体";
        public static string UserServiceGetDataTable = "取得列表";
        public static string UserServiceGetDataTableByRole = "依角色取得列表";
        public static string UserServiceGetDataTableByIds = "依主键取得列表";
        public static string UserServiceGetRoleDataTable = "取得用户的角色列表";
        public static string UserServiceUserInRole = "判断用户是否在某个角色中";
        public static string UserServiceUpdateUser = "更新用户";
        public static string UserServiceSearch = "查询用户";
        public static string UserServiceSetUserAuditStates = "设置用户审核状态";
        public static string UserServiceSetDefaultRole = "设置用户的预设角色";

        // WorkFlowActivityAdminService工作流程定义服务及相关的方法名称定义
        public static string WorkFlowActivityAdminService = "工作流程定义服务";

        // WorkFlowCurrentService当前工作流程服务
        public static string WorkFlowCurrentService = "当前工作流程服务";

        // WorkFlowProcessAdminService工作流程处理过程服务
        public static string WorkFlowProcessAdminService = "工作流程处理过程服务";

        // BillTemplateService单据模板服务
        public static string BillTemplateService = "单据模板服务";
    }
}