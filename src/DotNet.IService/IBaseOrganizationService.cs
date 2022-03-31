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
    /// IOrganizationService
    /// 
    /// 修改记录
    /// 
    ///		2008.03.23 版本：1.0 JiRiGaLa 添加。
    ///		
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2008.03.23</date>
    /// </author> 
    /// </summary>
    public partial interface IBaseOrganizationService
    {
        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="id">主键</param>
        /// <returns>实体</returns>
        BaseOrganizationEntity GetEntity(BaseUserInfo userInfo, string id);

        /// <summary>
        /// 获取实体按编号
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="code">编号</param>
        /// <returns>实体</returns>
        BaseOrganizationEntity GetEntityByCode(BaseUserInfo userInfo, string code);

        /// <summary>
        /// 获取实体按名称
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="fullName">名称</param>
        /// <returns>实体</returns>
        BaseOrganizationEntity GetEntityByName(BaseUserInfo userInfo, string fullName);

        /// <summary>
        /// 判断字段是否重复
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="parameters">字段名称，值</param>
        /// <param name="id">主键</param>
        /// <returns>已存在</returns>
        bool Exists(BaseUserInfo userInfo, List<KeyValuePair<string, object>> parameters, string id);

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="entity">实体</param>
        /// <param name="status">状态</param>
        /// <param name="statusMessage">返回状消息</param>
        /// <returns>主键</returns>
        string Add(BaseUserInfo userInfo, BaseOrganizationEntity entity, out Status status, out string statusMessage);

        /// <summary>
        /// 按主键数组获取列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="ids">组织机构主键</param>
        /// <returns>数据表</returns>
        DataTable GetDataTableByIds(BaseUserInfo userInfo, string[] ids);

        /// <summary>
        /// 获得部门列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <returns>数据表</returns>
        DataTable GetDataTable(BaseUserInfo userInfo);

        ///// <summary>
        ///// 获取部门列表
        ///// </summary>
        ///// <param name="userInfo">用户</param>
        ///// <param name="parameters">参数</param>
        ///// <returns>数据表</returns>
        // DataTable GetDataTable(BaseUserInfo userInfo, List<KeyValuePair<string, object>> parameters);

        /// <summary>
        /// 获得列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="parentId">父亲节点主键</param>
        /// <returns>数据表</returns>
        DataTable GetErrorDataTable(BaseUserInfo userInfo, string parentId);
        
        /// <summary>
        /// 获得列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="parentId">父亲节点主键</param>
        /// <returns>数据表</returns>
        DataTable GetDataTableByParent(BaseUserInfo userInfo, string parentId);

        /// <summary>
        /// 获得列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="provinceId">省主键</param>
        /// <returns>数据表</returns>
        DataTable GetDataTableByProvinceId(BaseUserInfo userInfo, string provinceId);

        /// <summary>
        /// 获得列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="cityId">市主键</param>
        /// <returns>数据表</returns>
        DataTable GetDataTableByCityId(BaseUserInfo userInfo, string cityId);

        /// <summary>
        /// 获得列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="districtId">区县主键</param>
        /// <returns>数据表</returns>
        DataTable GetDataTableByDistrictId(BaseUserInfo userInfo, string districtId);

        /// <summary>
        /// 获得列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="streetId">街道主键</param>
        /// <returns>数据表</returns>
        DataTable GetDataTableByStreetId(BaseUserInfo userInfo, string streetId);

        /// <summary>
        /// 按角色获取列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="roleIds">角色主键</param>
        /// <returns>数据表</returns>
        DataTable GetDataTableByRole(BaseUserInfo userInfo, string[] roleIds);

        /// <summary>
        /// 获取内部部门
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="organizationId">主键</param>
        /// <returns>数据表</returns>
        DataTable GetInnerOrganizationDT(BaseUserInfo userInfo, string organizationId);

        /// <summary>
        /// 获取公司列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="organizationId">主键</param>
        /// <returns>数据表</returns>
        DataTable GetCompanyDT(BaseUserInfo userInfo, string organizationId);

        /// <summary>
        /// 获得部门的列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="organizationId">主键</param>
        /// <returns>数据表</returns>
        DataTable GetDepartmentDT(BaseUserInfo userInfo, string organizationId);

        /// <summary>
        /// 按主键获取列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="ids">主键数组</param>
        /// <returns>数据表</returns>
        List<BaseOrganizationEntity> GetListByIds(BaseUserInfo userInfo, string[] ids);

        /// <summary>
        /// 搜索部门
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="organizationId">主键</param>
        /// <param name="searchKey">查询字符</param>
        /// <returns>数据表</returns>
        DataTable Search(BaseUserInfo userInfo, string organizationId, string searchKey);

        /// <summary>
        /// 更新一个
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="entity">实体</param>
        /// <param name="status">状态</param>
        /// <param name="statusMessage">返回状消息</param>
        /// <returns>影响行数</returns>
        int Update(BaseUserInfo userInfo, BaseOrganizationEntity entity, out Status status, out string statusMessage);

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
        /// 批量保存
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="dt">数据表</param>
        /// <returns>影响行数</returns>
        int BatchSave(BaseUserInfo userInfo, DataTable dt);

        /// <summary>
        /// 移动数据
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="id">主键</param>
        /// <param name="parentId">父结点主键</param>
        /// <returns>影响行数</returns>
        int MoveTo(BaseUserInfo userInfo, string id, string parentId);

        /// <summary>
        /// 批量移动数据
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="ids">主键数组</param>
        /// <param name="parentId">父结点主键</param>
        /// <returns>影响行数</returns>
        int BatchMoveTo(BaseUserInfo userInfo, string[] ids, string parentId);

        /// <summary>
        /// 保存组织机构编号
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="ids">主键数组</param>
        /// <param name="codes">编号数组</param>
        /// <returns>影响行数</returns>
        int BatchSetCode(BaseUserInfo userInfo, string[] ids, string[] codes);

        /// <summary>
        /// 保存组织机构排序顺序
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="ids">主键数组</param>
        /// <returns>影响行数</returns>
        int BatchSetSortCode(BaseUserInfo userInfo, string[] ids);

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="recordCount">记录数</param>
        /// <param name="pageNo">当前页</param>
        /// <param name="pageSize">每页显示</param>
        /// <param name="whereClause">条件</param>
        /// <param name="dbParameters">参数</param>
        /// <param name="order">排序</param>
        /// <returns>数据表</returns>
        DataTable GetDataTableByPage(BaseUserInfo userInfo, out int recordCount, int pageNo, int pageSize, string whereClause, List<KeyValuePair<string, object>> dbParameters, string order = null);

        /// <summary>
        /// 刷新缓存列表
        /// 2015-12-11 吉日嘎拉 刷新缓存功能优化
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <returns>数据表</returns>
        void CachePreheating(BaseUserInfo userInfo);

        ///// <summary>
        ///// 删除一个
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