//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System.Collections.Generic;
using System.Reflection;


namespace DotNet.Business
{
    using IService;
    using Model;
    using Util;

    /// <summary>
    /// LanguageService
    /// 多语言服务
    /// 
    /// 修改记录
    /// 
    ///		2015.02.25 版本：1.0 JiRiGaLa 窗体与数据库连接的分离。
    ///		
    /// <author>
    ///		<name>JiRiGaLa</name>
    ///		<date>2015.02.25</date>
    /// </author> 
    /// </summary>


    public partial class LanguageService : ILanguageService
    {
        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="id">主键</param>
        /// <returns>实体</returns>
        public BaseLanguageEntity GetEntity(BaseUserInfo userInfo, string id)
        {
            BaseLanguageEntity entity = null;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                var manager = new BaseLanguageManager(dbHelper, userInfo);
                entity = manager.GetEntity(id);
            });

            return entity;
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="entity">实体</param>
        /// <param name="statusCode">返回状态码</param>
        /// <param name="statusMessage">返回状态信息</param>
        /// <returns>主键</returns>
        public string Add(BaseUserInfo userInfo, BaseLanguageEntity entity, out string statusCode, out string statusMessage)
        {
            var result = string.Empty;
            var returnCode = string.Empty;
            var returnMessage = string.Empty;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDbWithTransaction(userInfo, parameter, (dbHelper) =>
            {
                var manager = new BaseLanguageManager(dbHelper, userInfo);
                result = manager.Add(entity, out returnCode);
                returnMessage = manager.GetStateMessage(returnCode);
            });
            statusCode = returnCode;
            statusMessage = returnMessage;
            return result;
        }

        /// <summary>
        /// 更新实体
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="entity">实体</param>
        /// <param name="statusCode">返回状态码</param>
        /// <param name="statusMessage">返回状态信息</param>
        /// <returns>影响行数</returns>
        public int Update(BaseUserInfo userInfo, BaseLanguageEntity entity, out string statusCode, out string statusMessage)
        {
            var result = 0;

            var returnCode = string.Empty;
            var returnMessage = string.Empty;
            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDbWithTransaction(userInfo, parameter, (dbHelper) =>
            {
                var manager = new BaseLanguageManager(dbHelper, userInfo);
                result = manager.Update(entity, out returnCode);
                returnMessage = manager.GetStateMessage(returnCode);
            });
            statusCode = returnCode;
            statusMessage = returnMessage;
            return result;
        }

        /// <summary>
        /// 设置多语言
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="messageCode">语言编号</param>
        /// <param name="enUs">英文</param>
        /// <param name="zhCn">简体中文</param>
        /// <param name="zhTw">繁体中文</param>
        /// <returns>影响行数</returns>
        public int SetLanguage(BaseUserInfo userInfo, string messageCode, string enUs, string zhCn, string zhTw)
        {
            var result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                var manager = new BaseLanguageManager(dbHelper, userInfo);
                result += manager.SetLanguage("en-US", messageCode, enUs);
                result += manager.SetLanguage("zh-CN", messageCode, zhCn);
                result += manager.SetLanguage("zh-TW", messageCode, zhTw);
            });
            return result;
        }

        /// <summary>
        /// 按编号获取多语言
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="messageCode">语言编号</param>
        /// <returns>列表</returns>
        public List<BaseLanguageEntity> GetLanguageByMessageCode(BaseUserInfo userInfo, string messageCode)
        {
            var result = new List<BaseLanguageEntity>();

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                // 这里可以缓存起来，提高效率
                var manager = new BaseLanguageManager(dbHelper, userInfo);
                // 这里是条件字段
                var parameters = new List<KeyValuePair<string, object>>();
                parameters.Add(new KeyValuePair<string, object>(BaseLanguageEntity.FieldMessageCode, messageCode));
                parameters.Add(new KeyValuePair<string, object>(BaseLanguageEntity.FieldDeleted, 0));
                // 获取列表
                result = manager.GetList<BaseLanguageEntity>(parameters);
            });
            return result;
        }

        /// <summary>
        /// 批量设置删除
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="ids">主键数组</param>
        /// <returns>影响行数</returns>
        public int SetDeleted(BaseUserInfo userInfo, string[] ids)
        {
            var result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                var manager = new BaseLanguageManager(dbHelper, userInfo);
                result = manager.SetDeleted(ids, true, true, 4);
            });
            return result;
        }

        /// <summary>
        /// 批量保存
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="roleEntites">角色列表</param>
        /// <returns>影响行数</returns>
        public int BatchSave(BaseUserInfo userInfo, List<BaseLanguageEntity> roleEntites)
        {
            var result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDbWithTransaction(userInfo, parameter, (dbHelper) =>
            {
                var manager = new BaseLanguageManager(dbHelper, userInfo);
                result = manager.BatchSave(roleEntites);
            });
            return result;
        }
    }
}