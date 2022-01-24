//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System.Collections.Generic;
using System.Data;


namespace DotNet.IService
{
    using Model;
    using Util;

    /// <summary>
    /// IUserService
    /// 
    /// 修改记录
    /// 
    ///     2012.11.07 版本：1.4 JiRiGaLa 整理接口顺序。
    ///		2010.09.26 版本：1.3 JiRiGaLa 添加GetDataTableByDepartment接口。
    ///		2010.04.25 版本：1.2 JiRiGaLa 添加Exists接口。
    ///		2008.10.25 版本：1.1 JiRiGaLa 添加OpenId登录接口。
    ///		2008.04.01 版本：1.0 JiRiGaLa 添加接口定义。
    ///		
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2010.09.26</date>
    /// </author> 
    /// </summary>
    public partial interface IUserService
    {
        /// <summary>
        /// 判断字段是否重复
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="parameters">字段名称，值</param>
        /// <param name="id">主键</param>
        /// <returns>已存在</returns>
        bool Exists(BaseUserInfo userInfo, List<KeyValuePair<string, object>> parameters, string id);

        /// <summary>
        /// 添加用户
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        /// <param name="entity">实体</param>
        /// <param name="userLogonEntity">用户登录实体</param>
        /// <param name="userContactEntity">联系方式</param>
        /// <param name="statusCode">状态码</param>
        /// <param name="statusMessage">状态信息</param>
        /// <returns>主键</returns>
        string CreateUser(BaseUserInfo userInfo, BaseUserEntity entity, BaseUserLogonEntity userLogonEntity, BaseUserContactEntity userContactEntity, out string statusCode, out string statusMessage);

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="id">主键</param>
        /// <returns>实体</returns>
        BaseUserEntity GetEntity(BaseUserInfo userInfo, string id);

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="id">主键</param>
        /// <returns>实体</returns>
        BaseUserEntity GetEntityByCache(BaseUserInfo userInfo, string id);

        /// <summary>
        /// 获取实体按编号
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="code">编号</param>
        /// <returns>实体</returns>
        BaseUserEntity GetEntityByCode(BaseUserInfo userInfo, string code);

        /// <summary>
        /// 获取实体按名称
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="userName">用户名</param>
        /// <returns>实体</returns>
        BaseUserEntity GetEntityByUserName(BaseUserInfo userInfo, string userName);

        /// <summary>
        /// 获取实体按名称
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="realName">姓名</param>
        /// <returns>实体</returns>
        BaseUserEntity GetEntityByRealName(BaseUserInfo userInfo, string realName);

        /// <summary>
        /// 获取实体按昵称
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="nickName">昵称</param>
        /// <returns>实体</returns>
        BaseUserEntity GetEntityByNickName(BaseUserInfo userInfo, string nickName);

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="id">主键</param>
        /// <returns>实体</returns>
        BaseUserContactEntity GetUserContactObject(BaseUserInfo userInfo, string id);

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="id">主键</param>
        /// <returns>实体</returns>
        BaseUserContactEntity GetUserContactObjectByCache(BaseUserInfo userInfo, string id);

        /// <summary>
        /// 获取用户列表
        /// 当用户非常多时，不需要显示角色
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="showRole">显示角色</param>
        /// <returns>数据表</returns>
        DataTable GetDataTable(BaseUserInfo userInfo, bool showRole);

        /// <summary>
        /// 按主键获取用户数据
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="ids">主键数组</param>
        /// <returns>数据表</returns>
        DataTable GetDataTableByIds(BaseUserInfo userInfo, string[] ids);

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <returns>列表</returns>
        List<BaseUserEntity> GetList(BaseUserInfo userInfo);

        /// <summary>
        /// 按主键获取列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="ids">主键数组</param>
        /// <returns>数据表</returns>
        List<BaseUserEntity> GetListByIds(BaseUserInfo userInfo, string[] ids);

        /// <summary>
        /// 按上级主管获取下属用户列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="managerId">主管主键</param>
        /// <returns>用户列表</returns>
        List<BaseUserEntity> GetListByManager(BaseUserInfo userInfo, string managerId);

        /// <summary>
        /// 按上级主管获取下属用户主键数组
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="managerId">主管主键</param>
        /// <returns>用户主键数组</returns>
        string[] GetIdsByManager(BaseUserInfo userInfo, string managerId);

        /// <summary>
        /// 查询用户列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="userName">用户名</param>
        /// <param name="auditStates">用户状态</param>
        /// <param name="roleIds">角色主键</param>
        /// <returns>数据表</returns>
        DataTable Search(BaseUserInfo userInfo, string userName, string auditStates, string[] roleIds);

        /// <summary>
        /// 查询用户列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="permissionCode"></param>
        /// <param name="companyId"></param>
        /// <param name="userName"></param>
        /// <param name="auditStates"></param>
        /// <param name="roleIds"></param>
        /// <param name="enabled"></param>
        /// <param name="showRole"></param>
        /// <param name="userAllInformation"></param>
        /// <param name="recordCount"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        DataTable SearchByPage(BaseUserInfo userInfo, string permissionCode, string companyId, string userName, string auditStates, string[] roleIds, bool? enabled, bool showRole, bool userAllInformation, out int recordCount, int pageIndex = 0, int pageSize = 20, string sort = null);

        /// <summary>
        /// 根据部门查询用户列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="permissionCode"></param>
        /// <param name="searchKey"></param>
        /// <param name="enabled"></param>
        /// <param name="auditStates">用户状态</param>
        /// <param name="roleIds">角色主键</param>
        /// <param name="showRole"></param>
        /// <param name="userAllInformation"></param>
        /// <param name="recordCount"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="sort"></param>
        /// <param name="departmentId">部门主键</param>
        /// <returns>数据表</returns>
        DataTable SearchByPageByDepartment(BaseUserInfo userInfo, string permissionCode, string searchKey, bool? enabled, string auditStates, string[] roleIds, bool showRole, bool userAllInformation, out int recordCount, int pageIndex = 0, int pageSize = 100, string sort = null, string departmentId = null);

        /// <summary>
        /// 更新用户
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        /// <param name="entity">用户实体</param>
        /// <param name="userLogonEntity">用户登录实体</param>
        /// <param name="userContactEntity">用户联系方式实体</param>
        /// <param name="statusCode">状态码</param>
        /// <param name="statusMessage">状态信息</param>
        /// <returns>影响行数</returns>
        int UpdateUser(BaseUserInfo userInfo, BaseUserEntity entity, BaseUserLogonEntity userLogonEntity, BaseUserContactEntity userContactEntity, out string statusCode, out string statusMessage);

        /// <summary>
        /// 设置用户状态
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="ids">主键数组</param>
        /// <param name="auditStatus">审核状态</param>
        /// <returns>影响行数</returns>
        int SetUserAuditStates(BaseUserInfo userInfo, string[] ids, AuditStatus auditStatus);

        /// <summary>
        /// 设置用户主管的审核状态
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="ids">主键数组</param>
        /// <param name="auditStates">审核状态</param>
        /// <returns>影响行数</returns>
        int SetUserManagerAuditStates(BaseUserInfo userInfo, string[] ids, AuditStatus auditStates);

        /// <summary>
        /// 解除手机认证帮定
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="mobile">手机号码</param>
        /// <returns>影响行数</returns>
        int RemoveMobileBinding(BaseUserInfo userInfo, string mobile);

        /// <summary>
        /// 同步数据
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="all">同步所有数据</param>
        /// <returns>影响行数</returns>
        int Synchronous(BaseUserInfo userInfo, bool all = false);

        /// <summary>
        /// 批量设置删除
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="ids">主键数组</param>
        /// <returns>影响行数</returns>
        int SetDeleted(BaseUserInfo userInfo, string[] ids);

        /// <summary>
        /// 按部门获取部门用户
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="departmentId">部门主键</param>
        /// <param name="containChildren">含子部门</param>
        /// <returns>数据表</returns>
        List<BaseUserEntity> GetListByDepartment(BaseUserInfo userInfo, string departmentId, bool containChildren);

        /// <summary>
        /// 按部门获取部门用户
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="departmentId">部门主键</param>
        /// <param name="containChildren">含子部门</param>
        /// <returns>数据表</returns>
        DataTable GetDataTableByDepartment(BaseUserInfo userInfo, string departmentId, bool containChildren);

        /// <summary>
        /// 按角色获取用户
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="roleIds">角色主键</param>
        /// <returns>数据表</returns>
        DataTable GetDataTableByRole(BaseUserInfo userInfo, string[] roleIds);

        /// <summary>
        /// 获取用户类型
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <returns>数据表</returns>
        DataTable GetRoleDataTable(BaseUserInfo userInfo);

        /// <summary>
        /// 用户是否在某个角色里
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="userId">用户主键</param>
        /// <param name="roleCode">角色编号</param>
        /// <returns>存在</returns>
        bool UserIsInRole(BaseUserInfo userInfo, string userId, string roleCode);

        /// <summary>
        /// 获取员工角色列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="userId">用户主键</param>
        /// <returns>主键数组</returns>
        string[] GetUserRoleIds(BaseUserInfo userInfo, string userId);

        /// <summary>
        /// 批量保存
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="dt">数据表</param>
        /// <returns>影响行数</returns>
        int BatchSave(BaseUserInfo userInfo, DataTable dt);

        /// <summary>
        /// 获得用户的组织机构兼职情况
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="userId">用户主键</param>
        /// <returns>数据表</returns>
        DataTable GetUserOrganizationDT(BaseUserInfo userInfo, string userId);

        /// <summary>
        /// 用户是否在组织中
        /// </summary>
        /// <param name="userInfo"></param>
        /// <param name="userId"></param>
        /// <param name="organizeName"></param>
        /// <returns></returns>
        bool UserIsInOrganization(BaseUserInfo userInfo, string userId, string organizeName);

        /// <summary>
        /// 用户帐户添加到组织机构
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="entity">实体</param>
        /// <param name="statusCode">状态码</param>
        /// <param name="statusMessage">状态信息</param>
        /// <returns>主键</returns>
        string AddUserToOrganization(BaseUserInfo userInfo, BaseUserOrganizationEntity entity, out string statusCode, out string statusMessage);

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="ids">主键数组</param>
        /// <returns>影响行数</returns>
        int BatchDeleteUserOrganization(BaseUserInfo userInfo, string[] ids);

        /// <summary>
        /// 按角色获取用户
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="roleIds">角色主键</param>
        /// <returns>数据表</returns>
        List<BaseUserEntity> GetListByRole(BaseUserInfo userInfo, string[] roleIds);

        /// <summary>
        /// 获取用户的所有角色列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="systemCode">系统编号</param>
        /// <param name="userId">用户主键</param>
        /// <returns>列表</returns>
        List<BaseRoleEntity> GetUserRoleList(BaseUserInfo userInfo, string systemCode, string userId);

        /// <summary>
        /// 获取用户的所有角色列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="systemCode">系统编号</param>
        /// <param name="userId">用户主键</param>
        /// <returns>列表</returns>
        DataTable GetUserRoleDataTable(BaseUserInfo userInfo, string systemCode, string userId);
       
        /// <summary>
        /// 清除用户归属的角色
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="userId">用户主键</param>
        /// <returns>影响行数</returns>
        int ClearUserRole(BaseUserInfo userInfo, string userId);

        /// <summary>
        /// 申请加入到角色
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="userId">用户主键</param>
        /// <param name="addToRoleIds">加入角色主键集</param>
        /// <returns>影响的行数</returns>
        int ApplyForJointRole(BaseUserInfo userInfo, string userId, string[] addToRoleIds);
        
        /// <summary>
        /// 批量加入到角色
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="userId">员工主键</param>
        /// <param name="addToRoleIds">加入角色主键集</param>
        /// <returns>影响的行数</returns>
        int AddUserToRole(BaseUserInfo userInfo, string userId, string[] addToRoleIds);

        /// <summary>
        /// 批量移出角色
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="userId">用户主键</param>
        /// <param name="removeRoleIds">移出角色主键集</param>
        /// <returns>影响的行数</returns>
        int RemoveUserFromRole(BaseUserInfo userInfo, string userId, string[] removeRoleIds);

        /// <summary>
        /// 批量移出角色（按角色编号）
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="userId">用户主键</param>
        /// <param name="systemCode">系统编号</param>
        /// <param name="roleCode">角色编号</param>
        /// <returns>影响的行数</returns>
        int RemoveUserFromRoleByCode(BaseUserInfo userInfo, string userId, string systemCode, string roleCode);
        
        /// <summary>
        /// 批量设置用户的角色
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="userId">用户主键</param>
        /// <param name="roleIds">角色数组</param>
        /// <returns>影响的行数</returns>
        int SetUserRoles(BaseUserInfo userInfo, string userId, string[] roleIds);

        /// <summary>
        /// 按公司按角色获取用户
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="systemCode">系统编号</param>
        /// <param name="companyId">公司主键</param>
        /// <param name="roleId">角色主键</param>
        /// <returns>数据表</returns>
        DataTable GetDataTableByCompanyByRole(BaseUserInfo userInfo, string systemCode, string companyId, string roleId);
        
        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="recordCount">记录数</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">每页显示</param>
        /// <param name="whereClause">条件</param>
        /// <param name="dbParameters">参数</param>
        /// <param name="order">排序</param>
        /// <returns>数据表</returns>
        DataTable GetDataTableByPage(BaseUserInfo userInfo, out int recordCount, int pageIndex, int pageSize, string whereClause, List<KeyValuePair<string, object>> dbParameters, string order = null);

        ///// <summary>
        ///// 批量删除
        ///// </summary>
        ///// <param name="userInfo">用户</param>
        ///// <param name="ids">主键数组</param>
        ///// <returns>影响行数</returns>
        // int BatchDelete(BaseUserInfo userInfo, string[] ids);

        ///// <summary>
        ///// 单个删除
        ///// </summary>
        ///// <param name="userInfo">用户</param>
        ///// <param name="id">主键</param>
        ///// <returns>影响行数</returns>
        // int Delete(BaseUserInfo userInfo, string id);
    }
}