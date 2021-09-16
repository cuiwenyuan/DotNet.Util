//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System.Data;


namespace DotNet.IService
{
    using Model;
    using Util;

    /// <summary>
    /// ITableColumnsService
    /// 表字段结构
    /// 
    /// 修改记录
    /// 
    ///		2011.05.16 版本：1.0 JiRiGaLa 创建主键。
    ///		
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2011.05.16</date>
    /// </author> 
    /// </summary>
    public partial interface ITableColumnsService
    {
        /// <summary>
        /// 按表名获取列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="tableCode">表名</param>
        /// <returns>数据表</returns>
        DataTable GetDataTableByTable(BaseUserInfo userInfo, string tableCode);

        /// <summary>
        /// 获取约束条件（所有的约束）
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="resourceCategory">资源类别</param>
        /// <param name="resourceId">资源主键</param>
        /// <returns>数据表</returns>
        DataTable GetConstraintDT(BaseUserInfo userInfo, string resourceCategory, string resourceId);

        /// <summary>
        /// 设置约束条件
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="resourceCategory">资源类别</param>
        /// <param name="resourceId">资源主键</param>
        /// <param name="tableName">表名</param>
        /// <param name="permissionCode">权限编码</param>
        /// <param name="constraint">约束</param>
        /// <param name="enabled">有效</param>
        /// <returns>主键</returns>
        string SetConstraint(BaseUserInfo userInfo, string resourceCategory, string resourceId, string tableName, string permissionCode, string constraint, bool enabled);

        /// <summary>
        /// 获取约束条件
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="resourceCategory">资源类别</param>
        /// <param name="resourceId">资源主键</param>
        /// <param name="tableName">表名</param>
        /// <returns>约束条件</returns>
        string GetConstraint(BaseUserInfo userInfo, string resourceCategory, string resourceId, string tableName);

        /// <summary>
        /// 获取约束条件
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="resourceCategory">资源类别</param>
        /// <param name="resourceId">资源主键</param>
        /// <param name="tableName">表名</param>
        /// <param name="permissionCode">权限编码</param>
        /// <returns>约束条件</returns>
        BasePermissionScopeEntity GetConstraintEntity(BaseUserInfo userInfo, string resourceCategory, string resourceId, string tableName, string permissionCode);

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="ids">主键数组</param>
        /// <returns>影响行数</returns>
        int BatchDeleteConstraint(BaseUserInfo userInfo, string[] ids);

        /// <summary>
        /// 获取用户的件约束表达式
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="tableName">表名</param>
        /// <returns>主键</returns>
        string GetUserConstraint(BaseUserInfo userInfo, string tableName);
    }
}