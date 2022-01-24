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
    /// IStaffService
    /// 
    /// 修改记录
    /// 
    ///		2010.04.08 版本：2.0 JiRiGaLa 面向对象的方式进行改进。
    ///		2008.04.05 版本：1.0 JiRiGaLa 添加接口定义。
    ///		
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2008.04.05</date>
    /// </author> 
    /// </summary>
    public partial interface IStaffService
    {
        /// <summary>
        /// 获取内部通讯录
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="organizationId">组织机构主键</param>
        /// <param name="searchKey">查询内容</param>
        /// <returns>数据表</returns>
        DataTable GetAddressDataTable(BaseUserInfo userInfo, string organizationId, string searchKey);

        /// <summary>
        /// 获取内部通讯录
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="organizationId">组织机构主键</param>
        /// <param name="searchKey">查询内容</param>
        /// <param name="pageSize">分页的条数</param>
        /// <param name="recordCount">记录数</param>
        /// <param name="pageIndex">当前页数</param>
        /// <param name="sort">排序</param>
        /// <returns>数据表</returns>
        DataTable GetAddressDataTableByPage(BaseUserInfo userInfo, string organizationId, string searchKey, out int recordCount, int pageIndex = 0, int pageSize = 20, string sort = null);

        /// <summary>
        /// 更新通讯地址
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="entity">实体</param>
        /// <param name="statusCode">状态消息码</param>
        /// <param name="statusMessage">状态消息</param>
        /// <returns>影响行数</returns>
        int UpdateAddress(BaseUserInfo userInfo, BaseStaffEntity entity, out string statusCode, out string statusMessage);

        /// <summary>
        /// 更新通讯地址
        /// </summary>
        /// <param name="userInfo"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        int BatchUpdateAddress(BaseUserInfo userInfo, DataTable dt);

        /// <summary>
        /// 添加员工
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="entity">实体</param>
        /// <param name="statusCode">返回状态码</param>
        /// <param name="statusMessage">返回状消息</param>
        /// <returns>主键</returns>
        string AddStaff(BaseUserInfo userInfo, BaseStaffEntity entity, out string statusCode, out string statusMessage);
        
        /// <summary>
        /// 更新员工
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="entity">实体</param>
        /// <param name="statusCode">返回状态码</param>
        /// <param name="statusMessage">返回状消息</param>
        /// <returns>影响行数</returns>
        int UpdateStaff(BaseUserInfo userInfo, BaseStaffEntity entity, out string statusCode, out string statusMessage);
        
        /// <summary>
        /// 获得员工列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <returns>数据表</returns>
        DataTable GetDataTable(BaseUserInfo userInfo);

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="userInfo"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        BaseStaffEntity GetEntity(BaseUserInfo userInfo, string id);

        /// <summary>
        /// 按主键获取
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="ids">主键数组</param>
        /// <returns>数据表</returns>
        DataTable GetDataTableByIds(BaseUserInfo userInfo, string[] ids);

        /// <summary>
        /// 查询职员列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="permissionCode"></param>
        /// <param name="companyId"></param>
        /// <param name="whereClause"></param>
        /// <param name="auditStates"></param>
        /// <param name="enabled">有效</param>
        /// <param name="recordCount"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        DataTable SearchByPage(BaseUserInfo userInfo, string permissionCode, string companyId, string whereClause, string auditStates, bool? enabled, out int recordCount, int pageIndex = 0, int pageSize = 20, string sort = null);

        /// <summary>
        /// 按公司获取部门员工
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="companyId">公司主键</param>
        /// <param name="containChildren">含子部门</param>
        /// <returns>数据表</returns>
        DataTable GetDataTableByCompany(BaseUserInfo userInfo, string companyId, bool containChildren);

        /// <summary>
        /// 按部门获取部门员工
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="departmentId">部门主键</param>
        /// <param name="containChildren">含子部门</param>
        /// <returns>数据表</returns>
        DataTable GetDataTableByDepartment(BaseUserInfo userInfo, string departmentId, bool containChildren);

        /// <summary>
        /// 获得员工列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="organizationId">组织机构主键</param>
        /// <param name="containChildren">含子部门</param>
        /// <returns>数据表</returns>
        DataTable GetDataTableByOrganization(BaseUserInfo userInfo, string organizationId, bool containChildren);

        /// <summary>
        /// 获取子节点成员
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="organizationId">主键</param>
        /// <returns>数据表</returns>
        DataTable GetChildrenStaffs(BaseUserInfo userInfo, string organizationId);

        /// <summary>
        /// 获取父子节点成员
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="organizationId">主键</param>
        /// <returns>数据表</returns>
        DataTable GetParentChildrenStaffs(BaseUserInfo userInfo, string organizationId);

        /// <summary>
        /// 获得员工列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="organizationId">组织机构主键</param>
        /// <param name="searchKey">查询</param>
        /// <returns>数据表</returns>
        DataTable Search(BaseUserInfo userInfo, string organizationId, string searchKey);

        /// <summary>
        /// 设置员工关联的用户
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="staffId">员工主键</param>
        /// <param name="userId">用户主键</param>
        /// <returns>影响行数</returns>
        int SetStaffUser(BaseUserInfo userInfo, string staffId, string userId);

        /// <summary>
        /// 同步数据
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="all">同步所有数据</param>
        /// <returns>影响行数</returns>
        int Synchronous(BaseUserInfo userInfo, bool all = false);

        /// <summary>
        /// 删除员工关联的用户
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="staffId">员工主键</param>
        /// <returns>影响行数</returns>
        int DeleteUser(BaseUserInfo userInfo, string staffId);

        /// <summary>
        /// 批量设置删除
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="ids">主键数组</param>
        /// <returns>影响行数</returns>
        int SetDeleted(BaseUserInfo userInfo, string[] ids);

        /// <summary>
        /// 移动数据
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="id">主键</param>
        /// <param name="organizationId">组织机构主键</param>
        /// <returns>影响行数</returns>
        int MoveTo(BaseUserInfo userInfo, string id, string organizationId);

        /// <summary>
        /// 批量移动数据
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="ids">主键数组</param>
        /// <param name="organizationId">组织机构主键</param>
        /// <returns>影响行数</returns>
        int BatchMoveTo(BaseUserInfo userInfo, string[] ids, string organizationId);

        /// <summary>
        /// 批量保存员工
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="dt">数据表</param>
        /// <returns>影响行数</returns>
        int BatchSave(BaseUserInfo userInfo, DataTable dt); 

        /// <summary>
        /// 重新排序数据
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <returns>影响行数</returns>
        int ResetSortCode(BaseUserInfo userInfo);

        /// <summary>
        /// 获取主键
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        string GetId(BaseUserInfo userInfo, string name, object value);

        ///// <summary>
        ///// 单个删除
        ///// </summary>
        ///// <param name="userInfo">用户</param>
        ///// <param name="id">主键</param>
        ///// <returns>影响行数</returns>
        // int Delete(BaseUserInfo userInfo, string id);

        ///// <summary>
        ///// 批量删除
        ///// </summary>
        ///// <param name="userInfo">用户</param>
        ///// <param name="ids">主键数组</param>
        ///// <returns>影响行数</returns>
        // int BatchDelete(BaseUserInfo userInfo, string[] ids);
    }
}