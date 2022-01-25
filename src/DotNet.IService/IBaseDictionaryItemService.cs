//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;


namespace DotNet.IService
{
    using Model;
    using Util;

    /// <summary>
    /// IBaseItemDetailsService
    /// 
    /// 修改记录
    /// 
    ///		2013.03.10 版本：2.0 JiRiGaLa 基础数据与应用数据分离。
    ///		2008.04.02 版本：1.0 JiRiGaLa 创建主键。
    ///
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2013.03.10</date>
    /// </author> 
    /// </summary>
    public partial interface IBaseDictionaryItemService
    {
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="tableName">目标表</param>
        DataTable GetDataTable(BaseUserInfo userInfo, string tableName);

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="userInfo"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        List<BaseDictionaryItemEntity> GetList(BaseUserInfo userInfo, string tableName);

        /// <summary>
        /// 获取子列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="tableName">目标表</param>
        /// <param name="parentId">父节点主键</param>
        DataTable GetDataTableByParent(BaseUserInfo userInfo, string tableName, string parentId);

        /// <summary>
        /// 获取下拉框数据
        /// </summary>
        /// <param name="userInfo"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        DataTable GetDataTableByCode(BaseUserInfo userInfo, string code);

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="userInfo"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        List<BaseDictionaryItemEntity> GetListByCode(BaseUserInfo userInfo, string code);

        /// <summary>
        /// 获取批量下拉框数据
        /// </summary>
        /// <param name="userInfo"></param>
        /// <param name="codes"></param>
        /// <returns></returns>
        DataSet GetDataSetByCodes(BaseUserInfo userInfo, string[] codes);

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="tableName">目标表</param>
        /// <param name="id">主键</param>
        BaseDictionaryItemEntity GetEntity(BaseUserInfo userInfo, string tableName, string id);

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="tableName">主键</param>
        /// <param name="code">code</param>
        /// <returns>数据表</returns>
        BaseDictionaryItemEntity GetEntityByCode(BaseUserInfo userInfo, string tableName, string code);

        /// <summary>
        /// 添加编码
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="tableName">表名</param>
        /// <param name="entity">实体</param>
        /// <param name="statusCode">状态返回码</param>
        /// <param name="statusMessage">状态返回信息</param>
        string Add(BaseUserInfo userInfo, string tableName, BaseDictionaryItemEntity entity, out string statusCode, out string statusMessage);

        /// <summary>
        /// 更新编码
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="tableName">表名</param>
        /// <param name="entity">实体</param>
        /// <param name="statusCode">状态返回码</param>
        /// <param name="statusMessage">状态返回信息</param>
        int Update(BaseUserInfo userInfo, string tableName, BaseDictionaryItemEntity entity, out string statusCode, out string statusMessage);

        /// <summary>
        /// 批量设置删除
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="tableName">目标表</param>
        /// <param name="ids">主键数组</param>
        int SetDeleted(BaseUserInfo userInfo, string tableName, string[] ids);

        /// <summary>
        /// 保存组织机构排序顺序
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="tableName">表名</param>
        /// <param name="ids">主键数组</param>
        /// <returns>影响行数</returns>
        int BatchSetSortCode(BaseUserInfo userInfo, string tableName, string[] ids);

        /// <summary>
        /// 按操作权限获取列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="tableName">目标表</param>
        /// <param name="permissionCode">操作权限</param>
        /// <returns>数据表</returns>
        DataTable GetDataTableByPermission(BaseUserInfo userInfo, string tableName, string permissionCode = "Resource.ManagePermission");

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="tableName">目标表</param>
        /// <param name="id">主键</param>
        int Delete(BaseUserInfo userInfo, string tableName, string id);

        /// <summary>
        /// 批量删除编码
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="tableName">目标表</param>
        /// <param name="ids">主键数组</param>
        int BatchDelete(BaseUserInfo userInfo, string tableName, string[] ids);
    }
}
