//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;

namespace DotNet.Business
{
    using Model;
    using Util;

    /// <summary>
    /// BaseMessageManager（程序OK）
    /// 消息表
    ///
    /// 修改记录
    ///     
    ///     2015.10.09 版本：2.3 JiRiGaLa 消息发送接口进行简化、提高健壮性。
    ///     2015.09.26 版本：2.2 JiRiGaLa 增加缓存功能。
    ///     2009.03.16 版本：2.1 JiRiGaLa 已发信息查询功能整理。
    ///     2009.02.20 版本：2.0 JiRiGaLa 主键分类，表结构进行改进，主键重新整理。
    ///     2008.04.15 版本：1.0 JiRiGaLa 创建主键。
    ///
    /// <author>
    ///		<name>JiRiGaLa</name>
    ///		<date>2008.04.15</date>
    /// </author>
    /// </summary>
    public partial class BaseMessageManager : BaseManager
    {
        private string _query = "SELECT * FROM " + BaseMessageEntity.TableName;

        /// <summary>
        /// 发送消息
        /// 20151009 吉日嘎拉 消息发送接口参数最少化，能少一个参数，算一个参数，让调用的人更简单一些
        /// </summary>
        /// <param name="receiverId">接收者主键</param>
        /// <param name="contents">消息内容</param>
        /// <param name="functionCode">消息类型</param>
        /// <returns>消息实体</returns>
        public BaseMessageEntity Send(string receiverId, string contents, string functionCode = null)
        {
            var result = new BaseMessageEntity
            {
                Id = Guid.NewGuid().ToString("N")
            };
            if (string.IsNullOrEmpty(functionCode))
            {
                functionCode = MessageFunction.UserMessage.ToString();
            }
            result.CategoryCode = MessageCategory.Send.ToString();
            result.FunctionCode = functionCode;
            result.ReceiverId = receiverId;
            result.Contents = contents;
            result.IsNew = (int)MessageStateCode.New;
            result.ReadCount = 0;
            result.DeletionStateCode = 0;
            result.CreateOn = DateTime.Now;

            if (UserInfo != null)
            {
                result.CreateCompanyId = UserInfo.CompanyId;
                result.CreateCompanyName = UserInfo.CompanyName;
                result.CreateDepartmentId = UserInfo.DepartmentId;
                result.CreateDepartmentName = UserInfo.DepartmentName;
                result.CreateUserId = UserInfo.Id;
                result.CreateBy = UserInfo.RealName;
            }

            // 发送消息
            Send(result, true);
            // 最近联系人, 这个处理得不太好，先去掉
            // this.SetRecent(receiverId);

            // 进行缓存处理
            // CacheProcessing(result);

            return result;
        }

        #region public int Send(BaseMessageEntity entity, bool saveSend = true) 添加短信，只能发给一个人
        /// <summary>
        /// 添加一条短信，只能发给一个人，在数据库中加入两条记录
        /// </summary>
        /// <param name="entity">添加对象</param>
        /// <param name="saveSend"></param>
        /// <returns>影响行数</returns>
        public int Send(BaseMessageEntity entity, bool saveSend = true)
        {
            var receiverIds = new string[1];
            receiverIds[0] = entity.ReceiverId;
            return Send(entity, receiverIds, saveSend);
        }
        #endregion

        #region public int Send(BaseMessageEntity entity, string[] receiverIds, bool saveSend = true) 添加短信，可以发给多个人
        /// <summary>
        /// 添加短信，可以发给多个人
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="receiverIds">接收者主键组</param>
        /// <param name="saveSend">保存每个发送记录</param>
        /// <returns>影响行数</returns>
        public int Send(BaseMessageEntity entity, string[] receiverIds, bool saveSend = true)
        {
            var result = 0;
            result = Send(entity, receiverIds, saveSend);
            return result;
        }
        #endregion

        #region public int Send(BaseMessageEntity entity, string[] receiverIds, bool saveSend = true) 添加短信，可以发给多个人
        /// <summary>
        /// 添加短信，可以发给多个人
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="receiverIds">接收者主键组</param>
        /// <param name="saveSend">保存每个发送记录</param>
        /// <param name="expireAt"></param>
        /// <returns>影响行数</returns>
        public int Send(BaseMessageEntity entity, string[] receiverIds, bool saveSend = true, DateTime? expireAt = null)
        {
            // 每发一条短信，数据库中需要记录两条记录，他们的CreateUserId都为创建者ID。
            // 接收者多人的话，不要重复设置创建人的记录了，即对发送者来说，只要记录一条记录就够了  
            var result = 0;

            entity.CategoryCode = MessageCategory.Receiver.ToString();
            entity.IsNew = (int)MessageStateCode.New;

            BaseUserEntity userEntity = null;

            for (var i = 0; i < receiverIds.Length; i++)
            {
                entity.ReceiverId = receiverIds[i];
                // 没必要给自己发了, 其实给自己也需要发，否则不知道是否发送成功了没
                //if (entity.ReceiverId.Equals(UserInfo.Id))
                //{
                //    entity.IsNew = (int)MessageStateCode.Old;
                //    continue;
                //}
                // messageEntity.ParentId = null;
                entity.Id = Guid.NewGuid().ToString("N");
                entity.CategoryCode = MessageCategory.Receiver.ToString();
                userEntity = BaseUserManager.GetEntityByCache(receiverIds[i]);
                if (userEntity != null && !string.IsNullOrEmpty(userEntity.Id))
                {
                    entity.ReceiverRealName = userEntity.RealName;
                    // 发给了哪个部门的人，意义不大，是来自哪个部门的人，意义更大一些
                    entity.ReceiverDepartmentId = userEntity.DepartmentId;
                    entity.ReceiverDepartmentName = userEntity.DepartmentName;
                }
                entity.IsNew = 1;
                // 接收信息
                //string parentId = this.Add(entity, this.Identity, false);
                var parentId = AddEntity(entity);
                if (saveSend)
                {
                    // 已发送信息
                    entity.Id = Guid.NewGuid().ToString("N");
                    entity.ParentId = parentId;
                    entity.IsNew = (int)MessageStateCode.Old;
                    entity.CategoryCode = MessageCategory.Send.ToString();
                    entity.DeletionStateCode = 0;
                    //this.Add(entity, this.Identity, false);
                    AddEntity(entity);
                }
                result++;
            }

            // 是群发的消息，还是需要保存已发消息的功能
            if (!saveSend && receiverIds != null && receiverIds.Length > 1)
            {
                // 已发送信息
                entity.Id = Guid.NewGuid().ToString("N");
                entity.IsNew = (int)MessageStateCode.Old;
                entity.CategoryCode = MessageCategory.Send.ToString();
                entity.DeletionStateCode = 0;
                //this.Add(entity, this.Identity, false);
                AddEntity(entity);
            }

            return result;
        }
        #endregion

        #region public int Send(BaseMessageEntity messageEntity, string organizeId, bool saveSend = true) 按部门群发短信
        /// <summary>
        /// 按部门群发短信
        /// </summary>
        /// <param name="messageEntity">实体</param>
        /// <param name="organizeId">部门主键</param>
        /// <param name="saveSend"></param>
        /// <returns>影响行数</returns>
        public int Send(BaseMessageEntity messageEntity, string organizeId, bool saveSend = true)
        {
            var result = 0;
            //int i = 0;
            var userManager = new BaseUserManager(UserInfo);
            var entityList = userManager.GetChildrenUserList(organizeId);
            var receiverIds = new List<string>();
            foreach (var entity in entityList)
            {
                receiverIds.Add(entity.Id);
            }
            result = Send(messageEntity, receiverIds.ToArray(), saveSend);
            return result;
        }
        #endregion

        /// <summary>
        /// 批量保存
        /// </summary>
        /// <param name="receiverIds"></param>
        /// <param name="organizeId"></param>
        /// <param name="roleId"></param>
        /// <param name="entity"></param>
        /// <param name="saveSend"></param>
        /// <returns></returns>
        public int BatchSend(string[] receiverIds, string organizeId, string roleId, BaseMessageEntity entity, bool saveSend = true)
        {
            string[] organizeIds = null;
            string[] roleIds = null;
            if (!string.IsNullOrEmpty(organizeId))
            {
                organizeIds = new string[] { organizeId };
            }
            if (!string.IsNullOrEmpty(roleId))
            {
                roleIds = new string[] { roleId };
            }
            return BatchSend(receiverIds, organizeIds, roleIds, entity, saveSend);
        }

        /// <summary>
        /// 批量发送
        /// </summary>
        /// <param name="receiverId"></param>
        /// <param name="organizeId"></param>
        /// <param name="roleId"></param>
        /// <param name="entity"></param>
        /// <param name="saveSend"></param>
        /// <returns></returns>
        public int BatchSend(string receiverId, string organizeId, string roleId, BaseMessageEntity entity, bool saveSend = true)
        {
            string[] receiverIds = null;
            string[] organizeIds = null;
            string[] roleIds = null;
            if (!string.IsNullOrEmpty(receiverId))
            {
                receiverIds = new string[] { receiverId };
            }
            if (!string.IsNullOrEmpty(organizeId))
            {
                organizeIds = new string[] { organizeId };
            }
            if (!string.IsNullOrEmpty(roleId))
            {
                roleIds = new string[] { roleId };
            }
            return BatchSend(receiverIds, organizeIds, roleIds, entity, saveSend);
        }

        #region public int BatchSend(string[] receiverIds, string[] ids, string[] roleIds, BaseMessageEntity entity, bool saveSend  = true) 批量发送消息
        /// <summary>
        /// 批量发送消息
        /// </summary>
        /// <param name="receiverIds">接收者主键组</param>
        /// <param name="organizeIds">组织机构主键组</param>
        /// <param name="roleIds">角色主键组</param>
        /// <param name="entity"></param>
        /// <param name="saveSend"></param>
        /// <returns>影响行数</returns>
        public int BatchSend(string[] receiverIds, string[] organizeIds, string[] roleIds, BaseMessageEntity entity, bool saveSend = true)
        {
            var userManager = new BaseUserManager(UserInfo);
            receiverIds = userManager.GetUserIds(receiverIds, organizeIds, roleIds);
            return Send(entity, receiverIds, saveSend);
        }
        #endregion

        #region public string Remind(string receiverId, string contents) 发送系统提示消息
        /// <summary>
        /// 发送系统提示消息
        /// </summary>
        /// <param name="receiverId">接收者主键</param>
        /// <param name="contents">内容</param>
        /// <returns>主键</returns>
        public string Remind(string receiverId, string contents)
        {
            var result = string.Empty;

            var entity = new BaseMessageEntity
            {
                Id = Guid.NewGuid().ToString("N"),
                CategoryCode = MessageCategory.Receiver.ToString(),
                FunctionCode = MessageFunction.Remind.ToString(),
                ReceiverId = receiverId,
                Contents = contents,
                IsNew = (int)MessageStateCode.New,
                ReadCount = 0,
                DeletionStateCode = 0,
                CreateOn = DateTime.Now
            };
            Identity = true;
            result = Add(entity);

            // 20151120 吉日嘎拉 让程序兼容不用缓存也可以用
            // 进行缓存处理
            CacheProcessing(entity);

            return result;
        }
        #endregion

        #region public int GetNewCount() 获取新信息个数
        /// <summary>
        /// 获取新信息个数
        /// </summary>
        /// <returns>记录个数</returns>
        public int GetNewCount()
        {
            return GetNewCount(MessageFunction.Message);
        }
        #endregion

        #region public int GetNewCount(MessageFunction messageFunction) 获取新信息个数
        /// <summary>
        /// 获取新信息个数，类别应该是收的信息，不是发的信息
        /// </summary>
        /// <returns>记录个数</returns>
        public int GetNewCount(MessageFunction messageFunction)
        {
            var result = 0;
            var sql = "SELECT COUNT(*) "
                            + " FROM " + BaseMessageEntity.TableName
                            + "  WHERE (" + BaseMessageEntity.FieldIsNew + " = " + ((int)MessageStateCode.New) + " ) "
                            + "        AND (" + BaseMessageEntity.FieldCategoryCode + " = 'Receiver' )"
                            + "        AND (" + BaseMessageEntity.FieldReceiverId + " = '" + UserInfo.Id + "' )"
                            + "        AND (" + BaseMessageEntity.FieldDeleted + " = 0 )"
                            + "        AND (" + BaseMessageEntity.FieldFunctionCode + " = '" + messageFunction + "' )";
            var returnObject = DbHelper.ExecuteScalar(sql);
            if (returnObject != null)
            {
                result = int.Parse(returnObject.ToString());
            }
            return result;
        }
        #endregion

        #region public BaseMessageEntity GetNewOne() 获取最新一条信息
        /// <summary>
        /// 获取最新一条信息
        /// </summary>
        /// <returns>记录个数</returns>
        public BaseMessageEntity GetNewOne()
        {
            var messageEntity = new BaseMessageEntity();
            var sql = "SELECT * "
                            + " FROM (SELECT * FROM " + BaseMessageEntity.TableName + " WHERE (" + BaseMessageEntity.FieldIsNew + " = " + ((int)MessageStateCode.New) + " ) "
                            + "         AND (" + BaseMessageEntity.FieldReceiverId + " = '" + UserInfo.Id + "') "
                            + " ORDER BY " + BaseMessageEntity.FieldCreateTime + " DESC) "
                            + " WHERE ROWNUM = 1 ";
            var dt = DbHelper.Fill(sql);
            return (BaseMessageEntity)messageEntity.GetSingle(dt);
        }
        #endregion

        #region public string[] MessageChek() 检查信息状态
        /// <summary>
        /// 检查信息状态
        /// </summary>
        /// <returns>信息状态数组</returns>
        public string[] MessageChek()
        {
            var result = new string[6];
            // 0.新信息的个数
            var messageCount = GetNewCount();
            result[0] = messageCount.ToString();
            if (messageCount > 0)
            {
                var messageEntity = GetNewOne();
                var lastChekDate = DateTime.MinValue;
                if (messageEntity.CreateOn != null)
                {
                    // 1.最后检查时间
                    lastChekDate = Convert.ToDateTime(messageEntity.CreateOn);
                    result[1] = lastChekDate.ToString(BaseSystemInfo.DateTimeFormat);
                }
                result[2] = messageEntity.CreateUserId; // 2.最新消息的发出者
                result[3] = messageEntity.CreateBy; // 3.最新消息的发出者名称
                result[4] = messageEntity.Id;            // 4.最新消息的主键
                result[5] = messageEntity.Contents;       // 5.最新信息的内容
            }
            return result;
        }
        #endregion

        #region public BaseMessageEntity Read(string id) 阅读短信
        /// <summary>
        /// 阅读短信
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="setOld"></param>
        /// <returns>数据权限</returns>
        public BaseMessageEntity Read(string id, bool setOld = false)
        {
            // 这里需要改进一下，运行一个高性能的sql语句就可以了，效率会高一些
            var dt = GetDataTableById(id);
            var entity = BaseEntity.Create<BaseMessageEntity>(dt);
            if (entity != null)
            {
                OnRead(UserInfo, id, setOld);
                entity.ReadCount++;
            }
            return entity;
        }
        #endregion

        #region public static int OnRead(string id, bool setOld = false) 阅读短信后设置状态值和阅读次数
        /// <summary>
        /// 阅读短信后设置状态值和阅读次数
        /// 修改为静态方法、提高执行效率。
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        /// <param name="id">短信主键</param>
        /// <param name="setOld">设置为已读</param>
        /// <returns>影响的条数</returns>
        public static int OnRead(BaseUserInfo userInfo, string id, bool setOld = false)
        {
            var result = 0;
            CacheUtil.Remove("m" + id);
            using (var dbHelper = DbHelperFactory.GetHelper(BaseSystemInfo.MessageDbType, BaseSystemInfo.MessageDbConnection))
            {
                var sqlBuilder = new SqlBuilder(dbHelper);
                sqlBuilder.BeginUpdate(BaseMessageEntity.TableName);
                if (setOld)
                {
                    sqlBuilder.SetValue(BaseMessageEntity.FieldIsNew, ((int)MessageStateCode.Old).ToString());
                }
                sqlBuilder.SetDbNow(BaseMessageEntity.FieldReadDate);
                // 增加阅读次数
                sqlBuilder.SetFormula(BaseMessageEntity.FieldReadCount, BaseMessageEntity.FieldReadCount + " + 1");
                sqlBuilder.SetWhere(BaseMessageEntity.FieldId, id);
                result = sqlBuilder.EndUpdate();
            }

            return result;
        }
        #endregion

        #region public DataTable ReadFromReceiver(string receiverId) 获取当前即时聊天者的所有新信息
        /// <summary>
        /// 获取当前即时聊天者的所有新信息
        /// </summary>
        /// <param name="receiverId">目标聊天者</param>
        /// <returns>数据表</returns>
        public DataTable ReadFromReceiver(string receiverId)
        {
            // 读取发给我的信息
            var sql = _query
                            + " WHERE (" + BaseMessageEntity.FieldIsNew + " = " + ((int)MessageStateCode.New) + " ) "
                            + " AND (" + BaseMessageEntity.FieldReceiverId + " = '" + UserInfo.Id + "' ) "
                            + " AND (" + BaseMessageEntity.FieldCreateUserId + " = '" + receiverId + "' ) "
                            + " ORDER BY " + BaseMessageEntity.FieldCreateTime;
            var dt = DbHelper.Fill(sql);
            // 标记为已读
            var id = string.Empty;
            foreach (DataRow dr in dt.Rows)
            {
                // 这是别人发过来的信息
                if (dr[BaseMessageEntity.FieldReceiverId].ToString() == UserInfo.Id)
                {
                    id = dr[BaseMessageEntity.FieldId].ToString();
                    SetProperty(id, new KeyValuePair<string, object>(BaseMessageEntity.FieldIsNew, (int)MessageStateCode.Old));
                }
            }
            return dt;
        }
        #endregion

        #region public DataTable GetDataTableNew() 获取我的未读短信列表
        /// <summary>
        /// 获取我的未读短信列表
        /// </summary>		
        /// <returns>数据表</returns>
        public DataTable GetDataTableNew()
        {
            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseMessageEntity.FieldReceiverId, UserInfo.Id),
                new KeyValuePair<string, object>(BaseMessageEntity.FieldCategoryCode, MessageCategory.Receiver.ToString()),
                new KeyValuePair<string, object>(BaseMessageEntity.FieldIsNew, (int)MessageStateCode.New),
                new KeyValuePair<string, object>(BaseMessageEntity.FieldDeleted, 0)
            };
            return GetDataTable(parameters, 20, BaseMessageEntity.FieldCreateUserId + "," + BaseMessageEntity.FieldCreateTime);

            /*
            string sql = "   SELECT TOP 10 * "
                            + " FROM " + BaseMessageEntity.TableName
                            + "    WHERE " + BaseMessageEntity.FieldIsNew + " = " + ((int)MessageStateCode.New).ToString()
                            + "          AND " + BaseMessageEntity.FieldReceiverId + " = " + DbHelper.GetParameter(BaseMessageEntity.FieldReceiverId)
                            + "          AND " + BaseMessageEntity.FieldDeleted + " = 0 "
                            + "          AND " + BaseMessageEntity.FieldEnabled + " = 1 "
                            + " ORDER BY " + BaseMessageEntity.FieldCreateUserId
                            + "          ," + BaseMessageEntity.FieldCreateTime;
            var result = new DataTable(BaseMessageEntity.TableName);
            DbHelper.Fill(result, sql, new IDbDataParameter[] { DbHelper.MakeParameter(BaseMessageEntity.FieldReceiverId, UserInfo.Id) });
            return result;
            */
        }
        #endregion

        #region public DataTable SearchNewList(string searchKey) 查询我的未读短信列表
        /// <summary>
        /// 查询我的未读短信列表
        /// </summary>
        /// <param name="searchKey">查询字符</param>
        /// <returns>数据权限</returns>
        public DataTable SearchNewList(string searchKey)
        {
            if (searchKey.Length == 0)
            {
                return GetDataTableNew();
            }
            var dt = new DataTable(BaseMessageEntity.TableName);
            var sql = _query
                            + " WHERE ((" + BaseMessageEntity.FieldContents + " LIKE " + DbHelper.GetParameter(BaseMessageEntity.FieldContents) + " ) "
                            + " OR ( " + BaseMessageEntity.FieldTitle + " LIKE " + DbHelper.GetParameter(BaseMessageEntity.FieldReceiverId) + " ) "
                            + " OR ( " + BaseMessageEntity.FieldReceiverRealName + " LIKE " + DbHelper.GetParameter(BaseMessageEntity.FieldReceiverId) + " )) "
                            + " AND (" + BaseMessageEntity.FieldIsNew + " = " + ((int)MessageStateCode.New) + " ) "
                            + " AND (" + BaseMessageEntity.FieldReceiverId + " = " + DbHelper.GetParameter(BaseMessageEntity.FieldReceiverId) + " ) "
                            + " ORDER BY " + BaseMessageEntity.FieldCreateTime;
            var names = new string[4];
            var values = new Object[4];
            for (var i = 0; i < 3; i++)
            {
                names[i] = BaseMessageEntity.FieldContents;
                values[i] = searchKey;
            }
            names[3] = BaseMessageEntity.FieldReceiverId;
            values[3] = UserInfo.Id;
            DbHelper.Fill(dt, sql, DbHelper.MakeParameters(names, values));
            return dt;
        }
        #endregion

        #region public DataTable GetOldDT() 获取我的已读短信列表
        /// <summary>
        /// 获取我的已读短信列表
        /// </summary>		
        /// <returns>数据权限</returns>
        public DataTable GetOldDt()
        {
            var dt = new DataTable(BaseMessageEntity.TableName);
            var sql = _query
                            + " WHERE (" + BaseMessageEntity.FieldIsNew + " = " + ((int)MessageStateCode.Old) + " ) "
                            + " AND (" + BaseMessageEntity.FieldCategoryCode + " = '" + MessageCategory.Receiver + "' ) "
                            + " AND (" + BaseMessageEntity.FieldDeleted + " = 0 ) "
                            + " AND (" + BaseMessageEntity.FieldReceiverId + " = " + DbHelper.GetParameter(BaseMessageEntity.FieldReceiverId) + " ) "
                            + " ORDER BY " + BaseMessageEntity.FieldCreateTime;
            var names = new string[1];
            var values = new Object[1];
            names[0] = BaseMessageEntity.FieldReceiverId;
            values[0] = UserInfo.Id;
            DbHelper.Fill(dt, sql, DbHelper.MakeParameters(names, values));
            return dt;
        }
        #endregion

        #region public DataTable SearchOldDT(string searchKey) 查询我的已读短信列表
        /// <summary>
        /// 查询我的已读短信列表
        /// </summary>
        /// <param name="searchKey">查询字符</param>
        /// <returns>数据权限</returns>
        public DataTable SearchOldDt(string searchKey)
        {
            if (searchKey.Length == 0)
            {
                return GetOldDt();
            }
            var dt = new DataTable(BaseMessageEntity.TableName);
            var sql = _query
                            + " WHERE ((" + BaseMessageEntity.FieldContents + " LIKE " + DbHelper.GetParameter(BaseMessageEntity.FieldContents) + " ) "
                            + " OR (" + BaseMessageEntity.FieldReceiverRealName + " LIKE " + DbHelper.GetParameter(BaseMessageEntity.FieldReceiverId) + " ) "
                            + " OR (" + BaseMessageEntity.FieldCreateTime + " LIKE " + DbHelper.GetParameter(BaseMessageEntity.FieldCreateTime) + " )) "
                            + " AND (" + BaseMessageEntity.FieldIsNew + " = " + ((int)MessageStateCode.Old) + " ) "
                            + " AND (" + BaseMessageEntity.FieldReceiverId + " = " + DbHelper.GetParameter(BaseMessageEntity.FieldReceiverId) + " ) "
                            + " ORDER BY " + BaseMessageEntity.FieldCreateTime;
            var names = new string[4];
            var values = new Object[4];
            for (var i = 0; i < 3; i++)
            {
                names[i] = BaseMessageEntity.FieldContents;
                values[i] = searchKey;
            }
            names[3] = BaseMessageEntity.FieldReceiverId;
            values[3] = UserInfo.Id;
            DbHelper.Fill(dt, sql, DbHelper.MakeParameters(names, values));
            return dt;
        }
        #endregion

        #region public DataTable GetDataTableByFunction(string categoryId, string functionId, string userId, string createUserId = null) 按消息功能获取消息列表
        /// <summary>
        /// 按消息功能获取消息列表
        /// </summary>
        /// <param name="categoryCode">消息分类</param>
        /// <param name="functionCode">消息功能</param>
        /// <param name="userId">用户主键</param>
        /// <param name="createUserId"></param>
        /// <returns>数据表</returns>
        public DataTable GetDataTableByFunction(string categoryCode, string functionCode, string userId, string createUserId = null)
        {
            var sql = _query
                            + "    WHERE (" + BaseMessageEntity.FieldDeleted + " = 0 ) "
                            + "          AND (" + BaseMessageEntity.FieldCategoryCode + " = '" + DbHelper.SqlSafe(categoryCode) + "') ";
            if (!string.IsNullOrEmpty(functionCode))
            {
                sql += string.Format("          AND ({0} = '{1}' ) ", BaseMessageEntity.FieldFunctionCode, DbHelper.SqlSafe(functionCode));
            }
            if (!string.IsNullOrEmpty(createUserId))
            {
                sql += string.Format("          AND ({0} = '{1}' ) ", BaseMessageEntity.FieldFunctionCode, DbHelper.SqlSafe(createUserId));
            }
            if (categoryCode.Equals(MessageCategory.Send.ToString()))
            {
                // 已经发送出去的信息
                sql += string.Format("          AND ({0} = {1}) ", BaseMessageEntity.FieldCreateUserId, DbHelper.GetParameter(BaseMessageEntity.FieldReceiverId));
            }
            else
            {
                // 已收到的信息
                sql += string.Format("          AND ({0} = {1}) ", BaseMessageEntity.FieldReceiverId, DbHelper.GetParameter(BaseMessageEntity.FieldReceiverId));
            }

            sql += " ORDER BY " + BaseMessageEntity.FieldIsNew + " DESC "
                     + "          ," + BaseMessageEntity.FieldCreateTime;
            var dt = new DataTable(BaseMessageEntity.TableName);
            DbHelper.Fill(dt, sql, new IDbDataParameter[] { DbHelper.MakeParameter(BaseMessageEntity.FieldReceiverId, userId) });
            return dt;
        }
        #endregion
        /// <summary>
        /// 获取消息
        /// </summary>
        /// <param name="createUserId"></param>
        /// <param name="searchKey"></param>
        /// <returns></returns>
        public DataTable GetMessageDt(string createUserId, string searchKey = null)
        {
            searchKey = StringUtil.GetSearchString(searchKey);
            var sql = _query
                            + "    WHERE (" + BaseMessageEntity.FieldDeleted + " = 0 ) ";
            sql += string.Format("          AND ({0} = '{1}' ) ", BaseMessageEntity.FieldFunctionCode, MessageFunction.Message.ToString());
            // 已收到的信息
            sql += string.Format("          AND (( {0} = '{1}' AND {2} = '{3}' AND {4} = '{5}') ", BaseMessageEntity.FieldCategoryCode, MessageCategory.Receiver.ToString(), BaseMessageEntity.FieldReceiverId, UserInfo.Id, BaseMessageEntity.FieldCreateUserId, DbHelper.SqlSafe(createUserId));
            // 已发出的消息
            sql += string.Format("               OR ({0} = '{1}' AND {2} = '{3}' AND {4} = '{5}')) ", BaseMessageEntity.FieldCategoryCode, MessageCategory.Send.ToString(), BaseMessageEntity.FieldReceiverId, DbHelper.SqlSafe(createUserId), BaseMessageEntity.FieldCreateUserId, UserInfo.Id);

            if (!string.IsNullOrEmpty(searchKey))
            {
                sql += " AND ((" + BaseMessageEntity.FieldTitle + " LIKE " + DbHelper.GetParameter(BaseMessageEntity.FieldTitle) + " ) "
                          + " OR (" + BaseMessageEntity.FieldContents + " LIKE " + DbHelper.GetParameter(BaseMessageEntity.FieldContents) + " )) ";
            }

            sql += " ORDER BY " + BaseMessageEntity.FieldCreateTime + " DESC ";

            // sql += " ORDER BY " + BaseMessageEntity.FieldIsNew + " DESC "
            //          + "          ," + BaseMessageEntity.FieldCreateTime + " DESC ";
            // + "          ," + BaseMessageEntity.FieldCreateUserId;
            var dt = new DataTable(BaseMessageEntity.TableName);

            var names = new string[2];
            var values = new Object[2];
            names[0] = BaseMessageEntity.FieldTitle;
            values[0] = searchKey;
            names[1] = BaseMessageEntity.FieldContents;
            values[1] = searchKey;
            // names[2] = BaseMessageEntity.FieldCreateBy;
            // values[2] = UserInfo.Id;
            DbHelper.Fill(dt, sql, DbHelper.MakeParameters(names, values));
            return dt;
        }

        /// <summary>
        /// 获取警告
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public DataTable GetWarningDt(string userId = null)
        {
            if (string.IsNullOrEmpty(userId))
            {
                userId = UserInfo.Id;
            }
            return GetDataTableByFunction(MessageCategory.Receiver.ToString(), MessageFunction.Warning.ToString(), userId);
        }

        /// <summary>
        /// 获取警告
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="topN"></param>
        /// <returns></returns>
        public DataTable GetWarningDt(string userId, int topN)
        {
            return SearchWarningDt(string.Empty, userId, topN);
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="search"></param>
        /// <param name="userId"></param>
        /// <param name="topN"></param>
        /// <returns></returns>
        public DataTable SearchWarningDt(string search, string userId = null, int topN = 0)
        {
            if (string.IsNullOrEmpty(userId))
            {
                userId = UserInfo.Id;
            }
            if (search.Length == 0 && topN == 0)
            {
                return GetWarningDt();
            }
            search = StringUtil.GetSearchString(search);

            var sql = "SELECT ";
            if (topN != 0)
            {
                sql += " TOP " + topN;
            }
            sql += " * FROM " + BaseMessageEntity.TableName

                            + "    WHERE (" + BaseMessageEntity.FieldDeleted + " = 0 ) "
                            + "          AND (" + BaseMessageEntity.FieldCategoryCode + " = '" + MessageCategory.Receiver + "') ";
            sql += string.Format("          AND ({0} = '{1}' ) ", BaseMessageEntity.FieldFunctionCode, MessageFunction.Warning.ToString());
            // 已收到的信息
            sql += string.Format("          AND ({0} = {1}) ", BaseMessageEntity.FieldReceiverId, DbHelper.GetParameter(BaseMessageEntity.FieldReceiverId));

            var dbParameters = new List<IDbDataParameter>
            {
                DbHelper.MakeParameter(BaseMessageEntity.FieldReceiverId, userId)
            };

            if (!string.IsNullOrEmpty(search))
            {
                sql += " AND ((" + BaseMessageEntity.FieldTitle + " LIKE " + DbHelper.GetParameter(BaseMessageEntity.FieldTitle) + " ) "
                          + " OR (" + BaseMessageEntity.FieldContents + " LIKE " + DbHelper.GetParameter(BaseMessageEntity.FieldContents) + " ) "
                          + " OR (" + BaseMessageEntity.FieldCreateBy + " LIKE " + DbHelper.GetParameter(BaseMessageEntity.FieldCreateBy) + " )) ";

                dbParameters.Add(DbHelper.MakeParameter(BaseMessageEntity.FieldTitle, search));
                dbParameters.Add(DbHelper.MakeParameter(BaseMessageEntity.FieldContents, search));
                dbParameters.Add(DbHelper.MakeParameter(BaseMessageEntity.FieldCreateBy, search));
            }

            sql += " ORDER BY " + BaseMessageEntity.FieldIsNew + " DESC "
                     + "          ," + BaseMessageEntity.FieldCreateTime + " DESC ";
            var dt = new DataTable(BaseMessageEntity.TableName);

            DbHelper.Fill(dt, sql, dbParameters.ToArray());
            return dt;
        }

        #region public DataTable GetSendDT() 获取我的已发送短信列表
        /// <summary>
        /// 获取我的已发送短信列表
        /// </summary>		
        /// <returns>数据权限</returns>
        public DataTable GetSendDt()
        {
            var sql = _query
                            + " WHERE (" + BaseMessageEntity.FieldCategoryCode + " = '" + (MessageCategory.Send) + "') "
                            + " AND (" + BaseMessageEntity.FieldDeleted + " = 0) "
                            + " AND (" + BaseMessageEntity.FieldCreateUserId + " = '" + UserInfo.Id + "') "
                            + " ORDER BY " + BaseMessageEntity.FieldCreateTime;
            return DbHelper.Fill(sql);
        }
        #endregion

        #region public DataTable SearchSendDT(string searchKey) 查询我的已发送短信列表
        /// <summary>
        /// 查询我的已发送短信列表
        /// </summary>
        /// <param name="searchKey">查询字符</param>
        /// <returns>数据权限</returns>
        public DataTable SearchSendDt(string searchKey)
        {
            if (searchKey.Length == 0)
            {
                return GetSendDt();
            }
            searchKey = StringUtil.GetSearchString(searchKey);
            var dt = new DataTable(BaseMessageEntity.TableName);
            var sql = _query
                            + " WHERE ((" + BaseMessageEntity.FieldContents + " LIKE " + DbHelper.GetParameter(BaseMessageEntity.FieldContents) + " ) "
                            + " OR (" + BaseMessageEntity.FieldReceiverRealName + " LIKE " + DbHelper.GetParameter(BaseMessageEntity.FieldReceiverRealName) + " ) "
                            + " OR (" + BaseMessageEntity.FieldCreateTime + " LIKE " + DbHelper.GetParameter(BaseMessageEntity.FieldCreateTime) + " )) "
                            + " AND (" + BaseMessageEntity.FieldDeleted + " = 0) "
                            + " AND (" + BaseMessageEntity.FieldCategoryCode + " = '" + (MessageCategory.Send) + "') "
                            + " AND (" + BaseMessageEntity.FieldCreateUserId + " = " + DbHelper.GetParameter(BaseMessageEntity.FieldCreateUserId) + " ) "
                            + " ORDER BY " + BaseMessageEntity.FieldCreateTime;
            var names = new string[4];
            var values = new Object[4];
            names[0] = BaseMessageEntity.FieldContents;
            values[0] = searchKey;
            names[1] = BaseMessageEntity.FieldReceiverRealName;
            values[1] = searchKey;
            names[2] = BaseMessageEntity.FieldCreateTime;
            values[2] = searchKey;
            names[3] = BaseMessageEntity.FieldCreateUserId;
            values[3] = UserInfo.Id;
            DbHelper.Fill(dt, sql, DbHelper.MakeParameters(names, values));
            return dt;
        }
        #endregion

        #region public DataTable GetDeletedDT() 获取我的删除的短信列表
        /// <summary>
        /// 获取我的删除的短信列表
        /// </summary>		
        /// <returns>数据权限</returns>
        public DataTable GetDeletedDt()
        {
            var dt = new DataTable(BaseMessageEntity.TableName);
            var sql = _query
                            + " WHERE (" + BaseMessageEntity.FieldDeleted + " = 1 ) "
                            + " AND ((" + BaseMessageEntity.FieldReceiverId + " = " + DbHelper.GetParameter(BaseMessageEntity.FieldReceiverId) + " ) "
                            + " OR (" + BaseMessageEntity.FieldCreateUserId + " = " + DbHelper.GetParameter(BaseMessageEntity.FieldCreateUserId) + " AND " + BaseMessageEntity.FieldCategoryCode + " = '" + MessageCategory.Send + "')) "
                            + " ORDER BY " + BaseMessageEntity.FieldCreateTime;
            DbHelper.Fill(dt, sql, new IDbDataParameter[] {
                DbHelper.MakeParameter(BaseMessageEntity.FieldReceiverId, UserInfo.Id),
                DbHelper.MakeParameter(BaseMessageEntity.FieldCreateUserId, UserInfo.Id)
            });
            return dt;
        }
        #endregion

        #region public DataTable SearchDeletedDT(string searchKey) 查询我的已删除短信列表
        /// <summary>
        /// 查询我的已删除短信列表
        /// </summary>
        /// <param name="searchKey">查询字符</param>
        /// <returns>数据权限</returns>
        public DataTable SearchDeletedDt(string searchKey)
        {
            if (searchKey.Length == 0)
            {
                return GetDeletedDt();
            }
            var dt = new DataTable(BaseMessageEntity.TableName);
            var sql = _query
                            + " WHERE ((" + BaseMessageEntity.FieldContents + " LIKE ? ) "
                            + " OR ( " + BaseMessageEntity.FieldReceiverRealName + " LIKE ? ) "
                            + " OR (" + BaseMessageEntity.FieldCreateTime + " LIKE ? )) "
                            + " AND (" + BaseMessageEntity.FieldDeleted + " = 1 ) "
                            + " AND (" + BaseMessageEntity.FieldReceiverId + " = ? ) "
                            + " ORDER BY " + BaseMessageEntity.FieldCreateTime;
            var names = new string[4];
            var values = new Object[4];
            for (var i = 0; i < 3; i++)
            {
                names[i] = BaseMessageEntity.FieldContents;
                values[i] = searchKey;
            }
            names[3] = BaseMessageEntity.FieldReceiverId;
            values[3] = UserInfo.Id;
            DbHelper.Fill(dt, sql, DbHelper.MakeParameters(names, values));
            return dt;
        }
        #endregion

    }
}