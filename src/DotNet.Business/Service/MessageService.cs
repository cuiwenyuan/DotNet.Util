//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;


namespace DotNet.Business
{
    using IService;
    using Model;
    using Util;

    /// <summary>
    /// MessageService
    /// 消息服务
    /// 
    /// 修改记录
    /// 
    ///     2011.12.11 版本：2.0 JiRiGaLa Broadcast 广播功能增强。
    ///		2011.09.28 版本：1.2 JiRiGaLa 获取用户状态，内部组织机构的方法进行改进。
    ///		2011.09.20 版本：1.1 JiRiGaLa 优化代码。
    ///		2007.10.30 版本：1.0 JiRiGaLa 创建。
    ///		
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2011.12.11</date>
    /// </author> 
    /// </summary>


    public partial class MessageService : IMessageService
    {
        /// <summary>
        /// 用户中心数据库连接
        /// </summary>
        private readonly string _messageDbConnection = BaseSystemInfo.MessageDbConnection;

        /// <summary>
        /// 是否有信息被发过来
        /// </summary>
        public static bool MessageSend = true;

        /// <summary>
        /// 最后检查在线状态时间
        /// </summary>
        public static DateTime LaseOnlineStateCheck = DateTime.MinValue;

        #region public string Send(BaseUserInfo userInfo, string receiverId, string contents, string functionCode) 发送消息
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="receiverId">接收者主键</param>
        /// <param name="contents">内容</param>
        /// <param name="functionCode">内容</param>
        /// <returns>主键</returns>
        public string Send(BaseUserInfo userInfo, string receiverId, string contents, string functionCode)
        {
            var result = string.Empty;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessMessageDb(userInfo, parameter, (dbHelper) =>
            {
                var manager = new BaseMessageManager(dbHelper, userInfo);
                var entity = manager.Send(receiverId, contents, functionCode);
                result = entity.Id;
            });

            return result;
        }
        #endregion

        /// <summary>
        /// 发送群组消息
        /// </summary>
        /// <param name="userInfo"></param>
        /// <param name="organizationId"></param>
        /// <param name="roleId"></param>
        /// <param name="contents"></param>
        /// <returns></returns>
        public int SendGroupMessage(BaseUserInfo userInfo, string organizationId, string roleId, string contents)
        {
            var result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessMessageDb(userInfo, parameter, (dbHelper) =>
            {
                // 发给角色里所有的
                var entity = new BaseMessageEntity
                {
                    //messageEntity.Id = Guid.NewGuid().ToString("N");
                    CategoryCode = MessageCategory.Send.ToString()
                };
                if (!string.IsNullOrEmpty(organizationId))
                {
                    entity.FunctionCode = MessageFunction.OrganizationMessage.ToString();
                    entity.ObjectId = organizationId;
                }
                if (!string.IsNullOrEmpty(roleId))
                {
                    entity.FunctionCode = MessageFunction.RoleMessage.ToString();
                    entity.ObjectId = roleId;
                }
                entity.Contents = contents;
                entity.IsNew = (int)MessageStateCode.New;
                entity.ReadCount = 0;
                entity.DeletionStateCode = 0;
                var manager = new BaseMessageManager(dbHelper, userInfo)
                {
                    Identity = true
                };
                result = manager.BatchSend(string.Empty, organizationId, roleId, entity, false);
            });

            return result;
        }

        #region public string Remind(BaseUserInfo userInfo, string userId, string contents) 发送系统提示消息
        /// <summary>
        /// 发送系统提示消息
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="userId">接收者主键</param>
        /// <param name="contents">内容</param>
        /// <returns>主键</returns>
        public string Remind(BaseUserInfo userInfo, string userId, string contents)
        {
            var result = string.Empty;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessMessageDb(userInfo, parameter, (dbHelper) =>
            {
                var manager = new BaseMessageManager(dbHelper, userInfo);
                result = manager.Remind(userId, contents);
            });

            return result;
        }
        #endregion

        /// <summary>
        /// 批量发送站内信息
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="receiverIds">接受者主键数组</param>
        /// <param name="organizationIds">组织机构主键数组</param>
        /// <param name="roleIds">角色主键数组</param>
        /// <param name="messageEntity">消息内容</param>
        /// <returns>影响行数</returns>
        public int BatchSend(BaseUserInfo userInfo, string[] receiverIds, string[] organizationIds, string[] roleIds, BaseMessageEntity messageEntity)
        {
            var result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessMessageDb(userInfo, parameter, (dbHelper) =>
            {
                var manager = new BaseMessageManager(dbHelper, userInfo);
                //manager.Identity = false;
                result = manager.BatchSend(receiverIds, organizationIds, roleIds, messageEntity, true);
            });

            return result;
        }

        /// <summary>
        /// 广播消息
        /// 2015-09-29 吉日嘎拉 改进发送消息功能
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="allcompany">是否发全公司</param>
        /// <param name="roleIds">角色主键</param>
        /// <param name="areaIds">区域主键</param>
        /// <param name="companyIds">公司主键</param>
        /// <param name="subCompany">发下属公司</param>
        /// <param name="departmentIds">部门主键数组</param>
        /// <param name="subDepartment">发下属部门</param>
        /// <param name="userIds">用户主键数组</param>
        /// <param name="message">消息内容</param>
        /// <param name="onlineOnly">只发送给在线用户</param>
        /// <returns>影响行数</returns>
        public int Broadcast(BaseUserInfo userInfo, bool allcompany, string[] roleIds, string[] areaIds, string[] companyIds, bool subCompany, string[] departmentIds, bool subDepartment, string[] userIds, string message, bool onlineOnly)
        {
            var result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessMessageDb(userInfo, parameter, (dbHelper) =>
            {
                var manager = new BaseMessageManager(userInfo);
                result = manager.Broadcast(userInfo.SystemCode, allcompany, roleIds, areaIds, companyIds, subCompany, departmentIds, subDepartment, userIds, message, onlineOnly);
            });

            return result;
        }


        #region public string[] MessageCheck(BaseUserInfo userInfo, string lastChekDate) 获取消息状态
        /// <summary>
        /// 获取消息状态
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="onLineState">用户在线状态</param>
        /// <param name="lastChekDate">最后检查日期</param>
        /// <returns>消息状态数组</returns>
        public string[] MessageCheck(BaseUserInfo userInfo, int onLineState, string lastChekDate)
        {
            string[] result = null;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                // 设置为在线状态
                var userLogonManager = new BaseUserLogonManager(dbHelper, userInfo);
                userLogonManager.Online(userInfo.Id, onLineState);
                // 读取信息状态
                var messageManager = new BaseMessageManager(userInfo);
                result = messageManager.MessageChek();
            });

            return result;
        }
        #endregion

        #region public DataTable ReadFromReceiver(BaseUserInfo userInfo, string receiverId) 获取特定用户的新信息
        /// <summary>
        /// 获取特定用户的新信息
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="receiverId">当前交互的用户</param>
        /// <returns>数据表</returns>
        public DataTable ReadFromReceiver(BaseUserInfo userInfo, string receiverId)
        {
            var dt = new DataTable(BaseMessageEntity.TableName);

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessMessageDb(userInfo, parameter, (dbHelper) =>
            {
                var messageManager = new BaseMessageManager(dbHelper, userInfo);
                dt = messageManager.ReadFromReceiver(receiverId);
                dt.TableName = BaseMessageEntity.TableName;
            });

            return dt;
        }
        #endregion

        #region public int Read(BaseUserInfo userInfo, string id) 阅读消息
        /// <summary>
        /// 阅读消息
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="id">主键</param>
        public int Read(BaseUserInfo userInfo, string id)
        {
            var result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessMessageDb(userInfo, parameter, (dbHelper) =>
            {
                var messageManager = new BaseMessageManager(dbHelper, userInfo);
                if (messageManager.Read(id, true) != null)
                {
                    result++;
                }
            });

            return result;
        }
        #endregion

        #region public int CheckOnline(BaseUserInfo userInfo, int onLineState) 检查在线状态
        /// <summary>
        /// 检查在线状态
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="onLineState">用户在线状态</param>
        /// <returns>离线人数</returns>
        public int CheckOnline(BaseUserInfo userInfo, int onLineState)
        {
            var result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                var userLogonManager = new BaseUserLogonManager(dbHelper);
                // 设置为在线状态
                userLogonManager.Online(userInfo.Id, onLineState);
                result = userLogonManager.CheckOnline();
            });
            return result;
        }
        #endregion

        /// <summary>
        /// 在线状态表
        /// </summary>
        public static DataTable OnlineStateDt = null;

        #region public DataTable GetOnlineState(BaseUserInfo userInfo) 获取在线用户列表
        /// <summary>
        /// 获取在线用户列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <returns>数据表</returns>
        public DataTable GetOnlineState(BaseUserInfo userInfo)
        {
            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                var userLogonManager = new BaseUserLogonManager(dbHelper, userInfo);
                // 设置为在线状态
                userLogonManager.Online(userInfo.Id);
                if (LaseOnlineStateCheck == DateTime.MinValue)
                {
                }
                else
                {
                    // 2008.01.23 JiRiGaLa 修正错误
                    var timeSpan = DateTime.Now - LaseOnlineStateCheck;
                    if ((timeSpan.Minutes * 60 + timeSpan.Seconds) >= BaseSystemInfo.OnlineCheck)
                    {

                    }
                }
                if (OnlineStateDt == null)
                {
                    // 检查用户在线状态(服务器专用)
                    userLogonManager.CheckOnline();
                    // 获取在线状态列表
                    OnlineStateDt = userLogonManager.GetOnlineStateDt();
                    OnlineStateDt.TableName = BaseUserEntity.TableName;
                    LaseOnlineStateCheck = DateTime.Now;
                }
            });

            return InnerOrganizationDt;
        }
        #endregion

        #region public DataTable GetDataTableByPage(BaseUserInfo userInfo, out int recordCount, int pageIndex, int pageSize, string condition, List<KeyValuePair<string, object>> dbParameters, string order = null)
        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="recordCount">记录数</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">每页显示</param>
        /// <param name="condition">条件</param>
        /// <param name="dbParameters">参数</param>
        /// <param name="order">排序</param>
        /// <returns>数据表</returns>
        public DataTable GetDataTableByPage(BaseUserInfo userInfo, out int recordCount, int pageIndex, int pageSize, string condition, List<KeyValuePair<string, object>> dbParameters, string order = null)
        {
            var result = new DataTable(BaseMessageEntity.TableName);

            var myRecordCount = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessMessageDb(userInfo, parameter, (dbHelper) =>
            {
                if (SecretUtil.IsSqlSafe(condition))
                {
                    var messageManager = new BaseMessageManager(dbHelper, userInfo);
                    result = messageManager.GetDataTableByPage(out myRecordCount, pageIndex, pageSize, condition, dbHelper.MakeParameters(dbParameters), order);
                    result.TableName = BaseMessageEntity.TableName;

                }
                else
                {
                    // 记录注入日志
                    LogUtil.WriteLog("userInfo:" + userInfo.Serialize() + " " + condition, "SqlSafe");
                }
            });
            recordCount = myRecordCount;

            return result;
        }
        #endregion
    }
}