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
    /// IDepartmentService
    /// 
    /// 修改记录
    /// 
    ///		2014.12.08 版本：1.0 JiRiGaLa 添加。
    ///		
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2014.12.08</date>
    /// </author> 
    /// </summary>
    public partial interface IDepartmentService
    {
        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="id">主键</param>
        /// <returns>实体</returns>
        BaseDepartmentEntity GetEntity(BaseUserInfo userInfo, string id);

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
        /// <param name="statusCode">返回状态码</param>
        /// <param name="statusMessage">返回状消息</param>
        /// <returns>主键</returns>
        string Add(BaseUserInfo userInfo, BaseDepartmentEntity entity, out string statusCode, out string statusMessage);

        /// <summary>
        /// 获得部门列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="companyId">公司主键</param>
        /// <returns>数据表</returns>
        DataTable GetDataTable(BaseUserInfo userInfo, string companyId);

        /// <summary>
        /// 获得列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="parentId">父亲节点主键</param>
        /// <returns>数据表</returns>
        DataTable GetDataTableByParent(BaseUserInfo userInfo, string parentId);

        /// <summary>
        /// 按主键数组获取列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="ids">组织机构主键</param>
        /// <returns>数据表</returns>
        DataTable GetDataTableByIds(BaseUserInfo userInfo, string[] ids);

        /// <summary>
        /// 按主键获取列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="ids">主键数组</param>
        /// <returns>数据表</returns>
        List<BaseDepartmentEntity> GetListByIds(BaseUserInfo userInfo, string[] ids);

        /// <summary>
        /// 搜索部门
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="organizeId">主键</param>
        /// <param name="searchKey">查询字符</param>
        /// <returns>数据表</returns>
        DataTable Search(BaseUserInfo userInfo, string organizeId, string searchKey);

        /// <summary>
        /// 更新一个
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="entity">实体</param>
        /// <param name="statusCode">返回状态码</param>
        /// <param name="statusMessage">返回状消息</param>
        /// <returns>影响行数</returns>
        int Update(BaseUserInfo userInfo, BaseDepartmentEntity entity, out string statusCode, out string statusMessage);

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
    }
}