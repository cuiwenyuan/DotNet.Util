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
    /// IModuleService
    /// 
    /// 修改记录
    /// 
    ///		2008.04.03 版本：1.0 JiRiGaLa 添加接口定义。
    ///		
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2008.04.03</date>
    /// </author> 
    /// </summary>
    public partial interface IBaseModuleService
    {
        /// <summary>
        /// 获取模块列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <returns>数据表</returns>
        DataTable GetDataTable(BaseUserInfo userInfo);

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <returns>列表</returns>
        List<BaseModuleEntity> GetList(BaseUserInfo userInfo);

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="ids">主键数组</param>
        /// <returns>数据表</returns>
        DataTable GetDataTableByIds(BaseUserInfo userInfo, string[] ids);

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="id">主键</param>
        /// <returns>实体</returns>
        BaseModuleEntity GetEntity(BaseUserInfo userInfo, string id);

        /// <summary>
        /// 按编号获取实体
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="code">编号</param>
        /// <returns>实体</returns>
        BaseModuleEntity GetEntityByCode(BaseUserInfo userInfo, string code);

        /// <summary>
        /// 按窗体名称获取实体
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="formName">窗体名称</param>
        /// <returns>实体</returns>
        BaseModuleEntity GetEntityByFormName(BaseUserInfo userInfo, string formName);

        /// <summary>
        /// 判断字段是否重复
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="parameters">字段名称，值</param>
        /// <param name="id">主键</param>
        /// <returns>已存在</returns>
        bool Exists(BaseUserInfo userInfo, List<KeyValuePair<string, object>> parameters, string id);

        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="code">编号</param>
        /// <returns>名称</returns>
        string GetFullNameByCode(BaseUserInfo userInfo, string code);

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="entity">实体</param>
        /// <param name="statusCode">返回状态码</param>
        /// <param name="statusMessage">返回状消息</param>
        /// <returns>主键</returns>
        string Add(BaseUserInfo userInfo, BaseModuleEntity entity, out string statusCode, out string statusMessage);

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="entity">实体</param>
        /// <param name="statusCode">返回状态码</param>
        /// <param name="statusMessage">返回状消息</param>
        /// <returns>影响行数</returns>
        int Update(BaseUserInfo userInfo, BaseModuleEntity entity, out string statusCode, out string statusMessage);        

        /// <summary>
        /// 获得列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="parentId">父结点主键</param>
        /// <returns>数据表</returns>
        DataTable GetDataTableByParent(BaseUserInfo userInfo, string parentId);

        /// <summary>
        /// 获取范围权限项列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <returns>列表</returns>
        List<BaseModuleEntity> GetScopePermissionList(BaseUserInfo userInfo);

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
        /// <param name="organizationId">组织机构主键</param>
        /// <param name="parentId">父结点主键</param>
        /// <returns>影响行数</returns>
        int MoveTo(BaseUserInfo userInfo, string organizationId, string parentId);

        /// <summary>
        /// 批量移动数据
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="moduleIds">组织机构主键数组</param>
        /// <param name="parentId">父结点主键</param>
        /// <returns>影响行数</returns>
        int BatchMoveTo(BaseUserInfo userInfo, string[] moduleIds, string parentId);

        /// <summary>
        /// 批量保存
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="dt">数据表</param>
        /// <returns>影响行数</returns>
        int BatchSave(BaseUserInfo userInfo, DataTable dt);

        /// <summary>
        /// 保存排序顺序
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="ids">主键数组</param>
        /// <returns>影响行数</returns>
        int SetSortCode(BaseUserInfo userInfo, string[] ids);

        /// <summary>
        /// 获取菜单的用户列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="systemCode">系统编号</param>
        /// <param name="moduleId">模块主键</param>
        /// <param name="companyId">公司主键</param>
        /// <param name="userId">用户主键</param>
        /// <returns>列表</returns>
        DataTable GetModuleUserDataTable(BaseUserInfo userInfo, string systemCode, string moduleId, string companyId, string userId);

        /// <summary>
        /// 获取菜单的所有角色列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="systemCode">系统编号</param>
        /// <param name="moduleId">模块主键</param>
        /// <returns>列表</returns>
        DataTable GetModuleRoleDataTable(BaseUserInfo userInfo, string systemCode, string moduleId);

        /// <summary>
        /// 获取菜单的所有组织机构列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="systemCode">系统编号</param>
        /// <param name="moduleId">模块主键</param>
        /// <returns>列表</returns>
        DataTable GetModuleOrganizationDataTable(BaseUserInfo userInfo, string systemCode, string moduleId);

        /// <summary>
        /// 刷新缓存列表
        /// 2015-12-11 吉日嘎拉 刷新缓存功能优化
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <returns>数据表</returns>
        void CachePreheating(BaseUserInfo userInfo);
        
        ///// <summary>
        ///// 单个删除
        ///// </summary>
        ///// <param name="userInfo">用户</param>
        ///// <param name="id">主键</param>
        ///// <returns>影响行数</returns>
        // int Delete(BaseUserInfo userInfo, string id);

        ///// <summary>
        ///// 批量删除模块
        ///// </summary>
        ///// <param name="userInfo">用户</param>
        ///// <param name="ids">主键数组</param>
        ///// <param name="parentId">父结点主键</param>
        ///// <returns>影响行数</returns>
        // int BatchDelete(BaseUserInfo userInfo, string[] ids);
    }
}