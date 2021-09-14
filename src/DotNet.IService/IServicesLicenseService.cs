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
    /// IServicesLicenseService
    /// 参数服务接口
    /// 
    /// 修改记录
    /// 
    ///		2015.12.26 版本：1.0 JiRiGaLa 添加接口定义。
    ///		
    /// <author>
    ///		<name>JiRiGaLa</name>
    ///		<date>2015.12.26</date>
    /// </author> 
    /// </summary>
    public partial interface IServicesLicenseService
    {
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="userId">用户主键</param>
        /// <returns>数据表</returns>
        DataTable GetDataTableByUser(BaseUserInfo userInfo, string userId);

        /// <summary>
        /// 添加实体
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="entity">实体</param>
        /// <returns>主键</returns>
        string Add(BaseUserInfo userInfo, BaseServicesLicenseEntity entity);

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="id">主键</param>
        /// <returns>实体</returns>
        BaseServicesLicenseEntity GetObject(BaseUserInfo userInfo, string id);

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="tableName">表名</param>
        /// <param name="entity">实体</param>
        /// <returns>影响行数</returns>
        int Update(BaseUserInfo userInfo, string tableName, BaseServicesLicenseEntity entity);

        /// <summary>
        /// 批量设置删除
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="tableName">表名</param>
        /// <param name="ids">主键数组</param>
        /// <returns>影响行数</returns>
        int SetDeleted(BaseUserInfo userInfo, string tableName, string[] ids);
    }
}
