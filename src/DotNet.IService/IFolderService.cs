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
    /// IFolderService
    /// 
    /// 修改记录
    /// 
    ///		2008.05.05 版本：1.0 JiRiGaLa 添加权限。
    ///		2011.04.30 版本：2.0 JiRiGaLa 添加权限。
    ///		
    /// <author>
    ///		<name>JiRiGaLa</name>
    ///		<date>2011.04.30</date>
    /// </author> 
    /// </summary>
    public partial interface IFolderService
    {
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <returns>数据表</returns>
        
        DataTable GetDataTable(BaseUserInfo userInfo);

        /// <summary>
        /// 获取一条
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="id">主键</param>
        /// <returns>实体</returns>
        
        BaseFolderEntity GetObject(BaseUserInfo userInfo, string id);

        /// <summary>
        /// 按目录获取列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="id">主键</param>
        /// <returns>数据表</returns>
        
        DataTable GetDataTableByParent(BaseUserInfo userInfo, string id);

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="entity">实体</param>
        /// <param name="statusCode">状态码</param>
        /// <param name="statusMessage">状态信息</param>
        /// <returns>主键</returns>
        
        string Add(BaseUserInfo userInfo, BaseFolderEntity entity, out string statusCode, out string statusMessage);

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="parentId">父主键</param>
        /// <param name="folderName">文件夹名称</param>
        /// <param name="enabled">有效</param>
        /// <param name="statusCode">状态码</param>
        /// <param name="statusMessage">状态信息</param>
        /// <returns></returns>
        
        string AddByFolderName(BaseUserInfo userInfo, string parentId, string folderName,  bool enabled, out string statusCode, out string statusMessage);

        /// <summary>
        /// 更新文件夹
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="entity">文件夹</param>
        /// <param name="statusCode">状态码</param>
        /// <param name="statusMessage">状态信息</param>
        /// <returns>影响行数</returns>
        
        int Update(BaseUserInfo userInfo, BaseFolderEntity entity, out string statusCode, out string statusMessage);

        /// <summary>
        /// 重命名
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="id">主键</param>
        /// <param name="newName">新名称</param>
        /// <param name="enabled">有效</param>
        /// <param name="statusCode">状态码</param>
        /// <param name="statusMessage">状态信息</param>
        /// <returns>影响行数</returns>
        
        int Rename(BaseUserInfo userInfo, string id, string newName, bool enabled, out string statusCode, out string statusMessage);

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="searchKey">查询</param>
        /// <returns>数据表</returns>

        DataTable Search(BaseUserInfo userInfo, string searchKey);

        /// <summary>
        /// 移动
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="folderId">文件夹主键</param>
        /// <param name="parentId">目标主键</param>
        /// <returns>影响行数</returns>
        
        int MoveTo(BaseUserInfo userInfo, string folderId, string parentId);

        /// <summary>
        /// 批量移动
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="folderIds">文件夹主键数组</param>
        /// <param name="parentId">目标主键</param>
        /// <returns>影响行数</returns>
        
        int BatchMoveTo(BaseUserInfo userInfo, string[] folderIds, string parentId);

        /// <summary>
        /// 批量保存
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="dt">数据表</param>
        /// <returns>影响行数</returns>

        int BatchSave(BaseUserInfo userInfo, DataTable dt);

		/// <summary>
		/// 取得ID
		/// </summary>
		/// <param name="userInfo">用户</param>
		/// <param name="parameter1">参数1</param>
		/// <param name="parameter2">参数2</param>
		/// <returns>Id</returns>
		
		string GetId(BaseUserInfo userInfo, KeyValuePair<string, object> parameter1, KeyValuePair<string, object> parameter2);

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="id">主键</param>
        /// <returns>影响行数</returns>
        
        int Delete(BaseUserInfo userInfo, string id);

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="ids">主键数组</param>
        /// <returns>影响行数</returns>
        
        int BatchDelete(BaseUserInfo userInfo, string[] ids);
	}
}