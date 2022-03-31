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
    /// IBaseItemsService
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
    public partial interface IBaseDictionaryService
    {
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        DataTable GetDataTable(BaseUserInfo userInfo);

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <returns>列表</returns>
        List<BaseDictionaryEntity> GetList(BaseUserInfo userInfo);

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="id">主键</param>
        BaseDictionaryEntity GetEntity(BaseUserInfo userInfo, string id);

        /// <summary>
        /// 获取子列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="parentId">父节点主键</param>
        DataTable GetDataTableByParent(BaseUserInfo userInfo, string parentId);

        /// <summary>
        /// 添加编码
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="entity">实体</param>
        /// <param name="status">状态</param>
        /// <param name="statusMessage">状态返回信息</param>
        string Add(BaseUserInfo userInfo, BaseDictionaryEntity entity, out Status status, out string statusMessage);
                
        /// <summary>
        /// 更新编码
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="entity">实体</param>
        /// <param name="status">状态</param>
        /// <param name="statusMessage">状态返回信息</param>
        int Update(BaseUserInfo userInfo, BaseDictionaryEntity entity, out Status status, out string statusMessage);

        /// <summary>
        /// 批量保存编码
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="dt">数据表</param>
        int BatchSave(BaseUserInfo userInfo, DataTable dt);

        /// <summary>
        /// 批量设置删除
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="tableName">目标表</param>
        /// <param name="ids">主键数组</param>
        int SetDeleted(BaseUserInfo userInfo, string tableName, string[] ids);

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
