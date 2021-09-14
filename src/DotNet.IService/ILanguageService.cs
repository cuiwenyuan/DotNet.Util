//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System.Collections.Generic;


namespace DotNet.IService
{
    using Model;
    using Util;

    /// <summary>
    /// ILanguageService
    /// 多语言接口
    /// 
    /// 修改记录
    /// 
    ///		2015.02.24 版本：1.0 JiRiGaLa 创建主键。
    ///		
    /// <author>
    ///		<name>JiRiGaLa</name>
    ///		<date>2015.02.24</date>
    /// </author> 
    /// </summary>
    public partial interface ILanguageService
    {
        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="id">主键</param>
        /// <returns>实体</returns>
        
        BaseLanguageEntity GetObject(BaseUserInfo userInfo, string id);

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="entity">实体</param>
        /// <param name="statusCode">返回状态码</param>
        /// <param name="statusMessage">返回状态信息</param>
        /// <returns>主键</returns>
        
        string Add(BaseUserInfo userInfo, BaseLanguageEntity entity, out string statusCode, out string statusMessage);

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="entity">实体</param>
        /// <param name="statusCode">返回状态码</param>
        /// <param name="statusMessage">返回状态信息</param>
        /// <returns>影响行数</returns>
        
        int Update(BaseUserInfo userInfo, BaseLanguageEntity entity, out string statusCode, out string statusMessage);

        /// <summary>
        /// 设置多语言
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="messageCode">语言编号</param>
        /// <param name="enUS">英文</param>
        /// <param name="zhCN">简体中文</param>
        /// <param name="zhTW">繁体中文</param>
        /// <returns>影响行数</returns>
        
        int SetLanguage(BaseUserInfo userInfo, string messageCode, string enUS, string zhCN, string zhTW);

        /// <summary>
        /// 按编号获取多语言
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="messageCode">语言编号</param>
        /// <returns>列表</returns>
        List<BaseLanguageEntity> GetLanguageByMessageCode(BaseUserInfo userInfo, string messageCode);

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
        /// <param name="list">列表</param>
        /// <returns>影响行数</returns>
        
        int BatchSave(BaseUserInfo userInfo, List<BaseLanguageEntity> list);
	}
}