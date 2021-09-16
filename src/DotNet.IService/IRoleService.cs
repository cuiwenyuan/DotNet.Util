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
    /// IRoleService
    /// 角色接口
    /// 
    /// 修改记录
    /// 
    ///		2009.09.06 版本：1.2 JiRiGaLa IsInRole 增加。
    ///		2008.11.26 版本：1.1 JiRiGaLa 角色用户关系及角色权限关系相关函数整理。
    ///		2008.04.09 版本：1.0 JiRiGaLa 创建主键。
    ///		
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2008.11.26</date>
    /// </author> 
    /// </summary>
    public partial interface IRoleService
    {
        /// <summary>
        /// 添加角色(同时添加用户，一个数据库事务里进行处理)
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="entity">实体</param>
        /// <param name="userIds">用户主键数组</param>
        /// <param name="statusCode">返回状态码</param>
        /// <param name="statusMessage">返回状态信息</param>
        /// <returns>主键</returns>
        string AddWithUser(BaseUserInfo userInfo, BaseRoleEntity entity, string[] userIds, out string statusCode, out string statusMessage);
        
        /// <summary>
        /// 添加角色
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="entity">实体</param>
        /// <param name="statusCode">返回状态码</param>
        /// <param name="statusMessage">返回状态信息</param>
        /// <returns>主键</returns>
        string Add(BaseUserInfo userInfo, BaseRoleEntity entity, out string statusCode, out string statusMessage);

        /// <summary>
        /// 获取角色列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="systemCode">系统编号</param>
        /// <returns>数据表</returns>
        DataTable GetDataTable(BaseUserInfo userInfo, string systemCode);

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="systemCode">系统编号</param>
        /// <returns>列表</returns>
        List<BaseRoleEntity> GetList(BaseUserInfo userInfo, string systemCode);

        /// <summary>
        /// 按主键获取列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="ids">主键数组</param>
        /// <returns>数据表</returns>
        List<BaseRoleEntity> GetListByIds(BaseUserInfo userInfo, string[] ids);

        /// <summary>
        /// 获取默认岗位列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <returns>数据表</returns>
        DataTable GetDefaultDutyDT(BaseUserInfo userInfo);

        /// <summary>
        /// 获取业务角色列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <returns>数据表</returns>
        DataTable GetApplicationRole(BaseUserInfo userInfo);

        /// <summary>
        /// 获取用户群组列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <returns>数据表</returns>
        DataTable GetUserGroup(BaseUserInfo userInfo);

        /// <summary>
        /// 获取用户群组列表(管理用)
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <returns>数据表</returns>
        DataTable GetUserGroupByUser(BaseUserInfo userInfo);
        
        /// <summary>
        /// 按组织机构获取角色
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="organizeId">组织机构主键</param>
        /// <param name="showUser">显示用户</param>
        /// <returns>数据表</returns>
        DataTable GetDataTableByOrganize(BaseUserInfo userInfo, string organizeId, bool showUser);
        
        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="id">主键</param>
        /// <returns>实体</returns>
        BaseRoleEntity GetEntity(BaseUserInfo userInfo, string id);
        
        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="entity">实体</param>
        /// <param name="statusCode">返回状态码</param>
        /// <param name="statusMessage">返回状态信息</param>
        /// <returns>影响行数</returns>
        int Update(BaseUserInfo userInfo, BaseRoleEntity entity, out string statusCode, out string statusMessage);
        
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="ids">主键数组</param>
        /// <returns>数据表</returns>
        DataTable GetDataTableByIds(BaseUserInfo userInfo, string[] ids);

        /// <summary>
        /// 获取用户的角色列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="targetUserId">目标角色</param>
        /// <returns>数据表</returns>
        DataTable GetDataTableByUser(BaseUserInfo userInfo, string targetUserId);
        
        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="organizeId">组织机构主键</param>
        /// <param name="searchKey">查询字符串</param>
        /// <returns>数据表</returns>
        DataTable Search(BaseUserInfo userInfo, string organizeId, string searchKey);
        
        /// <summary>
        /// 批量保存
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="list">角色列表</param>
        /// <returns>影响行数</returns>
        int BatchSave(BaseUserInfo userInfo, List<BaseRoleEntity> list);
        
        /// <summary>
        /// 移动数据
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="id">主键</param>
        /// <param name="targetId">目标主键</param>
        /// <returns>影响行数</returns>
        int MoveTo(BaseUserInfo userInfo, string id, string targetId);
        
        /// <summary>
        /// 批量移动数据
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="ids">主键数组</param>
        /// <param name="targetId">目标主键</param>
        /// <returns>影响行数</returns>
        int BatchMoveTo(BaseUserInfo userInfo, string[] ids, string targetId);
        
        /// <summary>
        /// 排序顺序
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="organizeId">组织机构主键</param>
        /// <returns>影响行数</returns>
        int ResetSortCode(BaseUserInfo userInfo, string organizeId);
        
        /// <summary>
        /// 获得角色中的用户主键
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="roleId">角色主键</param>
        /// <returns>用户主键</returns>
        string[] GetRoleUserIds(BaseUserInfo userInfo, string roleId);
        
        /// <summary>
        /// 用户添加到角色
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="roleId">角色主键</param>
        /// <param name="addUserIds">用户主键</param>
        /// <returns>影响行数</returns>
        int AddUserToRole(BaseUserInfo userInfo, string roleId, string[] addUserIds);

        /// <summary>
        /// 组织机构添加到角色
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="roleId">角色主键</param>
        /// <param name="addOrganizeIds">组织机构主键</param>
        /// <returns>影响行数</returns>
        int AddOrganizeToRole(BaseUserInfo userInfo, string roleId, string[] addOrganizeIds);

        /// <summary>
        /// 将用户从角色中移除
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="roleId">角色主键</param>
        /// <param name="userIds">用户主键</param>
        /// <returns>影响行数</returns>
        int RemoveUserFromRole(BaseUserInfo userInfo, string roleId, string[] userIds);

        /// <summary>
        /// 将用户从角色中移除
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="roleId">角色主键</param>
        /// <param name="organizeIds">组织机构主键</param>
        /// <returns>影响行数</returns>
        int RemoveOrganizeFromRole(BaseUserInfo userInfo, string roleId, string[] organizeIds);

        /// <summary>
        /// 清楚角色用户关联
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="roleId">角色主键</param>
        /// <returns>影响行数</returns>
        int ClearRoleUser(BaseUserInfo userInfo, string roleId);

        /// <summary>
        /// 设置角色中的用户
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="roleId">角色</param>
        /// <param name="userIds">用户主键数组</param>
        /// <returns>影响行数</returns>
        int SetUsersToRole(BaseUserInfo userInfo, string roleId, string[] userIds);

        /// <summary>
        /// 按角色名获取角色列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="roleName">角色名</param>
        /// <returns>数据表</returns>
		DataTable GetDataTableByRoleName(BaseUserInfo userInfo, string roleName);

        /// <summary>
        /// 获取角色的用户列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="systemCode">系统编号</param>
        /// <param name="roleId">角色主键</param>
        /// <param name="companyId">网点主键</param>
        /// <param name="userId">用户主键</param>
        /// <param name="searchKey">关键字</param>
        /// <param name="recordCount">记录数</param>
        /// <param name="pageIndex">第几页</param>
        /// <param name="pageSize">每页多少</param>
        /// <param name="orderBy">排序</param>
        /// <returns>数据表</returns>
        DataTable GetRoleUserDataTable(BaseUserInfo userInfo, string systemCode, string roleId, string companyId, string userId, string searchKey, out int recordCount, int pageIndex, int pageSize, string orderBy);
        
        /// <summary>
        /// 获取角色的所有组织机构列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="systemCode">系统编号</param>
        /// <param name="roleId">角色主键</param>
        /// <returns>数据表</returns>
        DataTable GetRoleOrganizeDataTable(BaseUserInfo userInfo, string systemCode, string roleId);

		/// <summary>
		/// 读取属性
		/// </summary>
		/// <param name="userInfo">用户</param>
		/// <param name="parameters">参数</param>
		/// <param name="targetField">目标字段</param>
		/// <returns>属性</returns>
		string GetProperty(BaseUserInfo userInfo, List<KeyValuePair<string, object>> parameters, string targetField);

        /// <summary>
        /// 批量设置删除
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="ids">主键数组</param>
        /// <returns>影响行数</returns>
        int SetDeleted(BaseUserInfo userInfo, string[] ids);

        /// <summary>
        /// 刷新缓存列表
        /// 2015-12-11 吉日嘎拉 刷新缓存功能优化
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <returns>数据表</returns>
        void CachePreheating(BaseUserInfo userInfo);

        ///// <summary>
        ///// 删除
        ///// </summary>
        ///// <param name="userInfo">用户</param>
        ///// <param name="id">主键</param>
        ///// <returns>数据表</returns>
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