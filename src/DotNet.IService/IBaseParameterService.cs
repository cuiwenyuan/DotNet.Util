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
    /// IParameterService
    /// 参数服务接口
    /// 
    /// 修改记录
    /// 
    ///		2008.04.30 版本：1.0 JiRiGaLa 添加接口定义。
    ///		2011.07.15 版本：2.0 JiRiGaLa 获取服务器端配置的功能改进。
    ///		
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2011.07.15</date>
    /// </author> 
    /// </summary>
    public partial interface IBaseParameterService
    {
        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="tableName">表名</param>
        /// <param name="id">主键</param>
        /// <returns>实体</returns>
        BaseParameterEntity GetEntity(BaseUserInfo userInfo, string tableName, string id);

        /// <summary>
        /// 添加实体
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="tableName">表名</param>
        /// <param name="entity">实体</param>
        /// <returns>主键</returns>
        string Add(BaseUserInfo userInfo, string tableName, BaseParameterEntity entity);

        /// <summary>
        /// 更新一个
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="tableName">表名</param>
        /// <param name="entity">实体</param>
        /// <returns>影响行数</returns>
        int Update(BaseUserInfo userInfo, string tableName, BaseParameterEntity entity);

        /// <summary>
        /// 获取服务器上的配置信息
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="key">配置项主键</param>
        /// <returns>配置内容</returns>
        string GetServiceConfig(BaseUserInfo userInfo, string key);

        /// <summary>
        /// 获取参数
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="tableName">表名</param>
        /// <param name="categoryCode">类别编号</param>
        /// <param name="parameterId">参数主键</param>
        /// <param name="parameterCode">编码</param>
        /// <returns>数据权限</returns>
        string GetParameter(BaseUserInfo userInfo, string tableName, string categoryCode, string parameterId, string parameterCode);

        /// <summary>
        /// 更新参数设置
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="tableName">表名</param>
        /// <param name="categoryCode">类别编号</param>
        /// <param name="parameterId">参数主键</param>
        /// <param name="parameterCode">编码</param>
        /// <param name="parameterContent">参数内容</param>
        /// <returns>影响行数</returns>
        int SetParameter(BaseUserInfo userInfo, string tableName, string categoryCode, string parameterId, string parameterCode, string parameterContent);

        /// <summary>
        /// 获取记录
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="categoryCode">类别编号</param>
        /// <param name="parameterId">参数主键</param>
        /// <returns>数据表</returns>
        DataTable GetDataTableByParameter(BaseUserInfo userInfo, string categoryCode, string parameterId);

        /// <summary>
        /// 获取记录
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <returns>数据表</returns>
        DataTable GetSystemParameter(BaseUserInfo userInfo);

        /// <summary>
        /// 获取记录
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="categoryCode">类别编号</param>
        /// <param name="parameterId">参数主键</param>
        /// <param name="parameterCode">编码</param>
        /// <returns>数据表</returns>
        DataTable GetDataTableParameterCode(BaseUserInfo userInfo, string categoryCode, string parameterId, string parameterCode);

        /// <summary>
        /// 用户名是否重复
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="parameters">字段名,字段值</param>
        /// <returns>已存在</returns>
        bool Exists(BaseUserInfo userInfo, List<KeyValuePair<string, object>> parameters);

        /// <summary>
        /// 批量设置删除
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="tableName">表名</param>
        /// <param name="ids">主键数组</param>
        /// <returns>影响行数</returns>
        int SetDeleted(BaseUserInfo userInfo, string tableName, string[] ids);

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="categoryCode">类别编号</param>
        /// <param name="parameterId">参数主键</param>
        /// <returns>影响行数</returns>
        int DeleteByParameter(BaseUserInfo userInfo, string categoryCode, string parameterId);
        
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="categoryCode">类别编号</param>
        /// <param name="parameterId">参数主键</param>
        /// <param name="parameterCode">标志编号</param>
        /// <returns>影响行数</returns>
        int DeleteByParameterCode(BaseUserInfo userInfo, string categoryCode, string parameterId, string parameterCode);

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
