//-----------------------------------------------------------------------
// <copyright file="BaseContactManager.cs" company="Hairihan">
//     Copyright (C) 2016 , All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;

namespace DotNet.Business
{
    using Model;
    using Util;

    /// <summary>
    /// BaseContactManager
    /// 联络单
    ///
    /// 修改记录
    ///
    ///     2015-09-14 版本：3.2 JiRiGaLa  通过缓存获取分类前几行的方法。 
    ///     2009-12-23 版本：3.1 JiRiGaLa  接受人姓名也查询。 
    ///     2009-11-30 版本：3.0 JiRiGaLa  ParentId 是父目录分类、categoryId是类别分类，彻底搞明白，思路明确了，以前都是在瞎折腾了。 
    ///     2009-11-02 版本：2.0 JiRiGaLa  主键整理，表名需要能被指定，能给用户、部门发邮件、角色发邮件。 
    ///     2009-03-24 版本：1.1 Chenbotao 添加发送邮件方法。 
    ///		2009-03-18 版本：1.0 Chenbotao 创建主键。
    ///
    /// 版本：3.2
    ///
    /// <author>
    ///		<name>JiRiGaLa</name>
    ///		<date>2015-09-14</date>
    /// </author>
    /// </summary>
    public partial class BaseContactManager : BaseManager, IBaseManager
    {
        #region public DataTable SearchByParent(string userId, string parentId, string searchValue, bool deletionStateCode) 按目录进行查询
        /// <summary>
        /// 按目录进行查询，这个目录下的所有的内容都可以看
        /// </summary>
        /// <param name="userId">用户主键</param>
        /// <param name="parentId">父目录</param>
        /// <param name="searchKey">查询内容</param>
        /// <param name="deletionStateCode">是否删除</param>
        /// <returns>数据表</returns>
        public DataTable SearchByParent(string userId, string parentId, string searchKey, bool deletionStateCode)
        {
            DataTable dataTable = new DataTable(CurrentTableName);

            string sqlQuery = " SELECT Contact.ID AS ContactId "
                                    + " ,Contact.Title "
                                    + " ,Contact.SendCount"
                                    + " ,Contact.ReplyCount"
                                    + " ,Contact.ReadCount"
                                    + " ,Contact.CreateOn"
                                    + " ,Contact.CreateBy "
                                    + " ,Contact." + BaseContactEntity.FieldSortCode
                                    + " ,ContactDetails.ID"
                                    + " ,ContactDetails.IsNew"
                                    + " ,ContactDetails.NewComment"
                             + " FROM ( "
                                + " SELECT BaseContact.* FROM BaseContact WHERE " + BaseContactEntity.FieldDeleted + " = 0 ";


            if (!String.IsNullOrEmpty(parentId))
            {
                sqlQuery += " AND (BaseContact.ParentId = '" + parentId + "') ";
            }
            sqlQuery += " ) Contact "
                     + " LEFT OUTER JOIN "
                     + "  ( SELECT BaseContactDetails.* FROM BaseContactDetails WHERE " + BaseContactDetailsEntity.FieldDeleted + " = 0 AND Category = 'User' AND ReceiverId = '" + userId + "') ContactDetails "
                     + " ON Contact.ID = ContactDetails.ContactId ";

            List<IDbDataParameter> dbParameters = new List<IDbDataParameter>();
            if (!String.IsNullOrEmpty(searchKey))
            {
                sqlQuery += " AND (" + BaseContactEntity.FieldTitle + " LIKE " + DbHelper.GetParameter(BaseContactEntity.FieldTitle);
                sqlQuery += " OR " + BaseContactEntity.FieldContents + " LIKE " + DbHelper.GetParameter(BaseContactEntity.FieldContents) + ")";
                searchKey = StringUtil.GetSearchString(searchKey);
                dbParameters.Add(DbHelper.MakeParameter(BaseContactEntity.FieldTitle, searchKey));
                dbParameters.Add(DbHelper.MakeParameter(BaseContactEntity.FieldContents, searchKey));
            }

            // 最新的文件排序在最前面
            sqlQuery += " ORDER BY Contact." + BaseContactEntity.FieldCreateTime + " DESC ";
            DbHelper.Fill(dataTable, sqlQuery, dbParameters.ToArray());
            return dataTable;
            // " AND (BaseContactDetails." + BaseContactDetailsEntity.FieldDeleted + " = " + (deletionStateCode ? 1 : 0) + ")"
            // " AND (BaseContact." + BaseContactEntity.FieldDeleted + " = " + (deletionStateCode ? 1 : 0) + ")";
        }
        #endregion

        #region public DataTable SearchSent(string userId, string parentId, string searchValue, bool deletionStateCode) 查找用户自己发送的邮件
        /// <summary>
        /// 查找用户自己发送的邮件，接受人姓名也查询
        /// </summary>
        /// <param name="userId">用户主键</param>
        /// <param name="parentId">父目录</param>
        /// <param name="searchValue">查询内容</param>
        /// <param name="deletionStateCode">是否删除</param>
        /// <returns>数据表</returns>
        public DataTable SearchSent(string userId, string parentId, string searchValue, bool deletionStateCode)
        {
            DataTable dataTable = new DataTable(CurrentTableName);

            string sqlQuery = " SELECT BaseContact.Title "
                                + " ,BaseContact.SendCount"
                                + " ,BaseContact.ReplyCount"
                                + " ,BaseContact.ReadCount"
                                + " ,BaseContact.CreateOn"
                                + " ,BaseContact." + BaseContactEntity.FieldSortCode
                                + " ,BaseContactDetails.Id "
                                + ", BaseContactDetails.IsNew "
                                + " , BaseContactDetails.NewComment "
                                + " FROM BaseContact INNER JOIN "
                                + " BaseContactDetails ON BaseContact.Id = BaseContactDetails.ContactId ";
            if (!String.IsNullOrEmpty(parentId))
            {
                sqlQuery += " WHERE (BaseContact.CreateUserId = '" + userId + "') ";
            }
            sqlQuery += " AND (BaseContact.ParentId = '" + parentId + "')"
                                + " AND (BaseContactDetails.ReceiverId = '" + userId + "')"
                                + " AND (BaseContactDetails.Category = 'User')"
                                + " AND (BaseContactDetails." + BaseContactDetailsEntity.FieldDeleted + " = " + (deletionStateCode ? 1 : 0) + ")"
                                + " AND (BaseContact." + BaseContactEntity.FieldDeleted + " = " + (deletionStateCode ? 1 : 0) + ")";

            List<IDbDataParameter> dbParameters = new List<IDbDataParameter>();
            if (!String.IsNullOrEmpty(searchValue))
            {

                sqlQuery += " AND (" + BaseContactEntity.FieldTitle + " LIKE " + DbHelper.GetParameter(BaseContactEntity.FieldTitle);
                sqlQuery += " OR " + BaseContactEntity.FieldContents + " LIKE " + DbHelper.GetParameter(BaseContactEntity.FieldContents) + ")";
                searchValue = StringUtil.GetSearchString(searchValue);
                dbParameters.Add(DbHelper.MakeParameter(BaseContactEntity.FieldTitle, searchValue));
                dbParameters.Add(DbHelper.MakeParameter(BaseContactEntity.FieldContents, searchValue));
                //
                sqlQuery += " OR " + BaseContactEntity.TableName + "." + BaseContactEntity.FieldId + " IN ( "
                                   + " SELECT ContactId "
                                   + " FROM BaseContactDetails "
                                   + "  WHERE ( ReceiverRealName LIKE " + DbHelper.GetParameter(BaseContactEntity.FieldTitle) + ") AND ( CreateUserId = '" + userId + "')) ";
                dbParameters.Add(DbHelper.MakeParameter(BaseContactEntity.FieldId, searchValue));
            }
            // 有新评论的排序在前面
            sqlQuery += " ORDER BY BaseContactDetails." + BaseContactDetailsEntity.FieldNewComment + " DESC ";
            sqlQuery += " ,BaseContact." + BaseContactEntity.FieldCreateTime + " DESC ";
            DbHelper.Fill(dataTable, sqlQuery, dbParameters.ToArray());
            return dataTable;
        }
        #endregion

        #region public DataTable SearchReceive(string userId, string parentId, string searchValue, bool deletionStateCode)
        /// <summary>
        /// 查找收件箱邮件
        /// </summary>
        /// <param name="userId">用户主键</param>
        /// <param name="parentId">父目录</param>
        /// <param name="searchKey">查找内容</param>
        /// <param name="deletionStateCode">是否删除</param>
        /// <returns>数据表</returns>
        public DataTable SearchReceive(string userId, string parentId, string searchKey, bool deletionStateCode)
        {
            return SearchReceive(userId, parentId, searchKey, deletionStateCode, 0);
        }
        #endregion

        #region public DataTable SearchReceive(string userId, string parentId, string searchValue, bool deletionStateCode, int topN)
        /// <summary>
        /// 查找收件箱邮件,
        /// 吉日嘎拉的必杀技SQL语句，写SQL的最高的境界了
        /// </summary>
        /// <param name="userId">用户主键</param>
        /// <param name="parentId">父目录</param>
        /// <param name="searchValue">查找内容</param>
        /// <param name="deletionStateCode">是否删除</param>
        /// <param name="topN">前几个邮件</param>
        /// <returns>数据表</returns>
        public DataTable SearchReceive(string userId, string parentId, string searchValue, bool deletionStateCode, int topN)
        {
            DataTable dataTable = new DataTable(BaseContactEntity.TableName);
            string sqlQuery = " SELECT ";
            if (topN != 0)
            {
                sqlQuery += " TOP " + topN.ToString();
            }

            sqlQuery += @"    BaseContactDetails.ReceiverRealName
                            , BaseContactDetails.Id
                            , BaseContactDetails.IsNew
                            , BaseContactDetails.NewComment
                            , A.Id AS ContactId
                            , A.Title
                            , A.SendCount
                            , A.ReadCount
                            , A.CreateOn
                            , A.CreateBy 
 FROM (
                                SELECT   Id
                                       , Title
                                       , SendCount
                                       , ReadCount
                                       , CreateOn
                                       , CreateBy
  FROM BaseContact
                                 WHERE (
                                       (
                                        " + BaseContactEntity.FieldDeleted + @" = 0 
                                        AND Enabled = 1";
            if (!String.IsNullOrEmpty(parentId))
            {
                sqlQuery += " AND ParentId='" + parentId + "'";
            }
            sqlQuery += @" ) 
                                       AND (IsOpen = 1
                                            OR Id IN (
                                                    
                                                    SELECT ContactId          
                      FROM BaseContactDetails  
                                                     WHERE BaseContactDetails.ReceiverId= '" + userId + @"'
                                                           AND BaseContactDetails.Category = 'User'
                                                           AND BaseContactDetails.Enabled = 1
                                                           AND BaseContactDetails." + BaseContactDetailsEntity.FieldDeleted + @" = 0


                                                     UNION

                                                    SELECT ContactId
                      FROM BaseContactDetails
                                                     WHERE Category = 'Organize'
                                                       AND ReceiverId IN (
                                                                            SELECT CompanyId
                                              FROM BaseStaff
                                                                             WHERE UserId = '" + userId + @"'
                                                                                   AND (CompanyId IS NOT NULL) AND Enabled =1
                                                                             UNION
                                                                            SELECT DepartmentId
                                              FROM BaseStaff
                                                                             WHERE UserId = '" + userId + @"'
                                                                                   AND (DepartmentId IS NOT NULL) AND Enabled =1
                                                                             UNION 
                                                                            SELECT WorkgroupId
                                              FROM BaseStaff
                                                                             WHERE UserId = '" + userId + @"'
                                                                                   AND (WorkgroupId IS NOT NULL) AND Enabled =1
                                                                             
                                                                       --    UNION
                                                                       --   SELECT OrganizeId
                                                                       -- FROM BaseUserOrganize 
                                                                       --    WHERE (UserId = '" + userId + @"') AND Enabled = 1
                                                                          )
                                                    UNION

                                                   SELECT ContactId
                     FROM BaseContactDetails
                                                    WHERE BaseContactDetails.Category = 'Role'
                                                          AND (ReceiverId IN (
                                                                            SELECT RoleId
                                              FROM BaseUserRole
                                                                             WHERE UserId = '" + userId + @"' AND Enabled = 1
                                                                              )
                                                   )
                                              )
                                              AND Id NOT IN (
                                                    SELECT ContactId          
                      FROM BaseContactDetails  
                                                     WHERE BaseContactDetails.ReceiverId= '" + userId + @"'
                                                           AND BaseContactDetails.Category = 'User'
                                                           AND (BaseContactDetails.Enabled = 0
                                                                OR BaseContactDetails.DeletionStateCode = 1)
                                              )
                                       )";

            List<IDbDataParameter> dbParameters = new List<IDbDataParameter>();
            if (!String.IsNullOrEmpty(searchValue))
            {
                sqlQuery += " AND (BaseContact." + BaseContactEntity.FieldTitle + " LIKE " + DbHelper.GetParameter(BaseContactEntity.FieldTitle);
                sqlQuery += " OR BaseContact." + BaseContactEntity.FieldContents + " LIKE " + DbHelper.GetParameter(BaseContactEntity.FieldContents) + ")";
                searchValue = StringUtil.GetSearchString(searchValue);
                dbParameters.Add(DbHelper.MakeParameter(BaseContactEntity.FieldTitle, searchValue));
                dbParameters.Add(DbHelper.MakeParameter(BaseContactEntity.FieldContents, searchValue));
            }

            sqlQuery += @" )

                        ) AS A 

                        LEFT JOIN BaseContactDetails ON A.ID = BaseContactDetails.ContactId

                    AND BaseContactDetails.ReceiverId = '" + userId + @"'  
                    AND BaseContactDetails.Category = 'User'
                    AND BaseContactDetails." + BaseContactDetailsEntity.FieldDeleted + " = 0 ";

            // 新邮件排序在前面
            sqlQuery += " ORDER BY BaseContactDetails.IsNew DESC   ";
            // 有新评论的排序在前面
            sqlQuery += " , BaseContactDetails.NewComment DESC ";
            sqlQuery += " , A.CreateOn DESC ";
            DbHelper.Fill(dataTable, sqlQuery, dbParameters.ToArray());
            return dataTable;
        }
        #endregion

        #region public int Send(string contactId, string[] receiverIds, string[] organizeIds, string[] roleIds)
        /// <summary>
        /// 发送联络单
        /// </summary>
        /// <param name="contactId"></param>
        /// <param name="receiverIds">接收者</param>
        /// <param name="organizeIds">组织机构数组</param>
        /// <param name="roleIds">角色数组</param>
        /// <returns>影响行数</returns>
        public int Send(string contactId, string[] receiverIds, string[] organizeIds, string[] roleIds)
        {
            BaseUserManager userManager = new BaseUserManager(DbHelper, UserInfo);
            receiverIds = userManager.GetUserIds(receiverIds, organizeIds, roleIds);

            // 删除邮件的处理技巧、发送给部门的、发送给角色的。
            // 删除邮件的列表过滤问题解决
            BaseContactDetailsManager contactDetailsManager = new BaseContactDetailsManager(DbHelper, UserInfo);
            BaseContactDetailsEntity contactDetailsEntity = null;

            // 组织机构数组
            if (organizeIds != null)
            {
                BaseOrganizeManager organizeManager = new BaseOrganizeManager(DbHelper, UserInfo);
                for (int i = 0; i < organizeIds.Length; i++)
                {
                    contactDetailsEntity = new BaseContactDetailsEntity
                    {
                        // 这里一定要给个不可猜测的主键，为了提高安全性
                        Id = Guid.NewGuid().ToString("N"),
                        ContactId = contactId,
                        Category = "Organize",
                        ReceiverId = organizeIds[i],
                        ReceiverRealName = organizeManager.GetProperty(organizeIds[i], BaseOrganizeEntity.FieldFullName),
                        IsNew = 1,
                        Enabled = 1,
                        NewComment = 0
                    };
                    contactDetailsManager.Add(contactDetailsEntity, false);
                }
            }

            // 角色数组
            if (roleIds != null)
            {
                BaseRoleManager roleManager = new BaseRoleManager(DbHelper, UserInfo);
                for (int i = 0; i < roleIds.Length; i++)
                {
                    contactDetailsEntity = new BaseContactDetailsEntity
                    {
                        // 这里一定要给个不可猜测的主键，为了提高安全性
                        Id = Guid.NewGuid().ToString("N"),
                        ContactId = contactId,
                        Category = "Role",
                        ReceiverId = roleIds[i],
                        ReceiverRealName = roleManager.GetProperty(roleIds[i], BaseRoleEntity.FieldRealName),
                        IsNew = 1,
                        Enabled = 1,
                        NewComment = 0
                    };
                    contactDetailsManager.Add(contactDetailsEntity, false);
                }
            }

            return Send(contactId, receiverIds);
        }
        #endregion

        /// <summary>
        /// 发送
        /// </summary>
        /// <param name="contactId"></param>
        /// <param name="receiverId"></param>
        /// <returns></returns>
        public int Send(string contactId, string receiverId)
        {
            int result = 0;

            if (!String.IsNullOrEmpty(receiverId))
            {
                result = Send(contactId, new string[] { receiverId });
            }

            return result;
        }

        #region public int Send(string ContactId, string[] receiverIds)
        /// <summary>
        /// 发送联络单
        /// </summary>
        /// <param name="contactId">联络单主键</param>
        /// <param name="receiverIds">接收者</param>
        /// <returns>影响行数</returns>
        public int Send(string contactId, string[] receiverIds)
        {
            int returnValue = 0;
            // 是否给自己发过这封邮件
            bool findSend = false;
            BaseContactDetailsManager contactDetailsManager = new BaseContactDetailsManager(DbHelper, UserInfo);
            BaseUserManager userManager = new BaseUserManager(DbHelper, UserInfo);
            BaseUserEntity useEntity = null;
            BaseContactDetailsEntity contactDetailsEntity = null;
            for (int i = 0; i < receiverIds.Length; i++)
            {
                useEntity = userManager.GetEntity(receiverIds[i]);
                // 是有效的用户，而且是未必删除的用户才发邮件
                if (useEntity.Enabled == 1 && useEntity.DeletionStateCode == 0)
                {
                    contactDetailsEntity = new BaseContactDetailsEntity
                    {
                        // 这里一定要给个不可猜测的主键，为了提高安全性
                        Id = Guid.NewGuid().ToString("N"),
                        ContactId = contactId,
                        Category = "User",
                        ReceiverId = useEntity.Id.ToString(),
                        ReceiverRealName = useEntity.RealName,
                        IsNew = 1,
                        Enabled = 1,
                        NewComment = 0
                    };
                    contactDetailsManager.Add(contactDetailsEntity, false);
                }
                // 若已经有发过，就不用再判断了
                if (!findSend)
                {
                    if (UserInfo.Id.Equals(receiverIds[i]))
                    {
                        findSend = true;
                    }
                }
                returnValue++;
            }
            // 没有给自己发过
            if (!findSend)
            {
                // 发送给自己一份
                Send(contactId);
                returnValue++;
            }
            // 设置总共发送了几个人
            SetProperty(new KeyValuePair<string, object>(BaseContactEntity.FieldId, contactId), new KeyValuePair<string, object>(BaseContactEntity.FieldSendCount, receiverIds.Length.ToString()));
            return returnValue;
        }
        #endregion

        #region public string Send(string contactId)
        /// <summary>
        /// 发送给自己一份
        /// </summary>
        /// <param name="contactId">联络单主键</param>
        public string Send(string contactId)
        {
            BaseUserManager userManager = new BaseUserManager(DbHelper, UserInfo);
            BaseUserEntity useEntity = userManager.GetEntity(UserInfo.Id);
            BaseContactDetailsEntity contactDetailsEntity = new BaseContactDetailsEntity
            {
                // 这里一定要给个不可猜测的主键，为了提高安全性
                Id = Guid.NewGuid().ToString("N"),
                ContactId = contactId,
                Category = "User",
                ReceiverId = useEntity.Id.ToString(),
                ReceiverRealName = useEntity.RealName,
                IsNew = 0,
                Enabled = 1,
                NewComment = 0
            };
            BaseContactDetailsManager contactDetailsManager = new BaseContactDetailsManager(DbHelper, UserInfo);
            return contactDetailsManager.Add(contactDetailsEntity, false);
        }
        #endregion

        #region public bool CanRead(string contactId) 是否可以阅读某个联络单
        /// <summary>
        /// 是否可以阅读某个联络单
        /// </summary>
        /// <param name="contactId">联络单主键</param>
        /// <returns>允许阅读</returns>
        public bool CanRead(string contactId)
        {
            // 是否公开的
            if (IsOpen(contactId))
            {
                return true;
            }
            // 自己是否能阅读
            if (UserCanRead(contactId))
            {
                return true;
            }
            // 组织能否阅读
            if (OrganizeCanRead(contactId))
            {
                return true;
            }
            // 角色能否阅读
            if (RoleCanRead(contactId))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 是否公告开的
        /// </summary>
        /// <param name="contactId">联络单主键</param>
        /// <returns>是公开</returns>
        private bool IsOpen(string contactId)
        {
            return GetProperty(contactId, BaseContactEntity.FieldIsOpen).Equals("1");
        }

        /// <summary>
        /// 自己能否阅读
        /// </summary>
        /// <param name="contactId">联络单主键</param>
        /// <returns>能阅读</returns>
        private bool UserCanRead(string contactId)
        {
            var returnValue = false;
            // 当前操作员自己发布的
            var sqlQuery = "SELECT COUNT(Id) AS ContactCount "
                      + " FROM BaseContact "
                      + "  WHERE Id = '" + contactId + "'"
                      + "        AND CreateUserId = '" + UserInfo.Id + "'";

            object returnObject = DbHelper.ExecuteScalar(sqlQuery);
            returnValue = int.Parse(returnObject.ToString()) >= 1;
            if (returnValue)
            {
                return returnValue;
            }
            // 允许当前操作员看的
            sqlQuery = " SELECT COUNT(ContactId) AS ContactCount "
                      + " FROM BaseContactDetails "
                      + "  WHERE ReceiverId= '" + UserInfo.Id + "'"
                      + "        AND Category = 'User'"
                      + "        AND Enabled = 1 "
                      + "        AND " + BaseContactEntity.FieldDeleted + " = 0 "
                      + "        AND ContactId = '" + contactId + "'";
            returnObject = DbHelper.ExecuteScalar(sqlQuery);
            returnValue = int.Parse(returnObject.ToString()) >= 1;
            return returnValue;
        }

        /// <summary>
        /// 组织能否阅读
        /// </summary>
        /// <param name="contactId">联络单主键</param>
        /// <returns>能阅读</returns>
        private bool OrganizeCanRead(string contactId)
        {
            bool returnValue = false;
            string sqlQuery = "   SELECT COUNT(ContactId) AS ContactCount "
                         + " FROM BaseContactDetails "
                            + " WHERE Category = 'Organize' "
                                 + "  AND Enabled = 1 "
                                 + "  AND " + BaseContactDetailsEntity.FieldDeleted + " = 0 "
                                 + "  AND ContactId = '" + contactId + "'"
                                 + "  AND ReceiverId IN ( "

                                                   + "    SELECT CompanyId "
                                                   + " FROM BaseStaff "
                                                   + "     WHERE UserId = '" + UserInfo.Id + "' "
                                                   + "           AND (CompanyId IS NOT NULL) AND Enabled =1 "

                                                   + "     UNION "

                                                   + "    SELECT DepartmentId "
                                                   + " FROM BaseStaff "
                                                   + "     WHERE UserId = '" + UserInfo.Id + "'"
                                                   + "           AND (DepartmentId IS NOT NULL) AND Enabled =1 "

                                                   + "     UNION "

                                                   + "    SELECT WorkgroupId "
                                                   + " FROM BaseStaff "
                                                   + "     WHERE UserId = '" + UserInfo.Id + "'"
                                                   + "           AND (WorkgroupId IS NOT NULL) AND Enabled =1 "

                                                   + "     UNION "

                                                   + "    SELECT OrganizeId "
                                                   + " FROM BaseUserOrganize "
                                                   + "     WHERE (UserId = '" + UserInfo.Id + "') AND Enabled = 1 "

                                                   + " ) ";
            object returnObject = DbHelper.ExecuteScalar(sqlQuery);
            returnValue = int.Parse(returnObject.ToString()) >= 1;
            return returnValue;
        }


        /// <summary>
        /// 角色能否阅读
        /// </summary>
        /// <param name="contactId">联络单主键</param>
        /// <returns>能阅读</returns>
        private bool RoleCanRead(string contactId)
        {
            bool returnValue = false;
            string sqlQuery = "SELECT COUNT(ContactId) AS ContactCount "
                      + " FROM BaseContactDetails "
                      + " WHERE BaseContactDetails.Category = 'Role' "
                      + " AND Enabled = 1 "
                      + " AND " + BaseContactDetailsEntity.FieldDeleted + " = 0 "
                      + " AND ContactId = '" + contactId + "'"
                      + " AND (ReceiverId IN ( "
                                         + " SELECT Role "
                                         + " FROM BaseUser "
                                         + " WHERE (ROLE IS NOT NULL) AND Id = '" + UserInfo.Id + "'"
                                         + " UNION "
                                         + " SELECT RoleId "
                                         + " FROM BaseUserRole "
                                         + " WHERE UserId = '" + UserInfo.Id + "' AND Enabled = 1 "
                                         + " )"
                                         + " )";
            object returnObject = DbHelper.ExecuteScalar(sqlQuery);
            returnValue = int.Parse(returnObject.ToString()) >= 1;
            return returnValue;
        }
        #endregion

        /// <summary>
        /// 获取清单
        /// </summary>
        /// <param name="parentId"></param>
        /// <param name="topLimit"></param>
        /// <param name="containContents"></param>
        /// <returns></returns>
        public List<BaseContactEntity> GetTopList(string parentId, int topLimit, bool containContents = false)
        {
            List<BaseContactEntity> result = new List<BaseContactEntity>();

            string order = BaseContactEntity.FieldPriority + "," + BaseContactEntity.FieldCreateTime + " DESC ";
            List<KeyValuePair<string, object>> parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseContactEntity.FieldParentId, parentId),
                new KeyValuePair<string, object>(BaseContactEntity.FieldEnabled, 1),
                new KeyValuePair<string, object>(BaseContactEntity.FieldDeleted, 0)
            };

            using (IDataReader dataReader = ExecuteReader(parameters, topLimit, order))
            {
                while (dataReader.Read())
                {
                    // 2015-11-18 吉日嘎拉 消息的内容不能有，否则会出错，缓存的内容也太大
                    BaseContactEntity contactEntity = BaseEntity.Create<BaseContactEntity>(dataReader, false);
                    // 是否要内容
                    if (!containContents)
                    {
                        contactEntity.Contents = null;
                    }
                    result.Add(contactEntity);
                }
            }

            return result;
        }

        /// <summary>
        /// 获取用户的通知列表
        /// </summary>
        /// <param name="userId">用户主键</param>
        /// <param name="parentId">那个类别</param>
        /// <param name="topLimit">获取前几个</param>
        /// <param name="containContents">是否包含内容</param>
        /// <returns>通知列表</returns>
        public List<BaseContactEntity> GetTopListByUser(string userId, string parentId, int topLimit, bool containContents = false)
        {
            List<BaseContactEntity> result = new List<BaseContactEntity>();
            // 获取用户信息
            string commandText = string.Empty;
            BaseUserEntity userEntity = BaseUserManager.GetEntityByCache(userId);
            if (userEntity != null)
            {
                commandText = " (Enabled = 1 AND " + BaseUserEntity.FieldDeleted + " = 0 AND AuditStatus=2 AND IsOpen = 1) OR (Enabled = 1 AND AuditStatus=2 AND " + BaseUserEntity.FieldDeleted + " = 0 AND Id IN (";
                if (!string.IsNullOrEmpty(parentId))
                {
                    commandText = " (ParentId = '" + parentId + "' AND Enabled = 1 AND " + BaseUserEntity.FieldDeleted + " = 0 AND AuditStatus=2 AND IsOpen = 1) OR (ParentId = '" + parentId + "' AND Enabled = 1 AND " + BaseUserEntity.FieldDeleted + " = 0 AND Id IN (";
                }
                // 获取用户所在的单位的信息
                BaseOrganizeEntity organizeEntity = BaseOrganizeManager.GetEntityByCache(userEntity.CompanyId);
                if (organizeEntity != null)
                {
                    // 所在省
                    if (!string.IsNullOrEmpty(organizeEntity.ProvinceId))
                    {
                        commandText += " SELECT (ContactId FROM BaseContactDetails WHERE Category = '0' AND ReceiverId = '" + organizeEntity.ProvinceId + "') ";
                    }
                    // 所在市
                    if (!string.IsNullOrEmpty(organizeEntity.CityId))
                    {
                        commandText += " UNION SELECT (ContactId FROM BaseContactDetails WHERE Category = '1' AND ReceiverId = '" + organizeEntity.CityId + "') ";
                    }
                    // 所在县
                    if (!string.IsNullOrEmpty(organizeEntity.DistrictId))
                    {
                        commandText += " UNION SELECT (ContactId FROM BaseContactDetails WHERE Category = '2' AND ReceiverId = '" + organizeEntity.DistrictId + "') ";
                    }
                }
                // 发给所在单位的
                if (!string.IsNullOrEmpty(userEntity.CompanyId))
                {
                    commandText += " UNION SELECT (ContactId FROM BaseContactDetails WHERE Category = '3' AND ReceiverId = '" + userEntity.CompanyId + "') ";
                }
                // 发给自己的
                if (!string.IsNullOrEmpty(userEntity.Id))
                {
                    commandText += " UNION SELECT (ContactId FROM BaseContactDetails WHERE Category = '4' AND ReceiverId = '" + userEntity.Id + "') ) )";
                }
                string order = BaseContactEntity.FieldPriority + "," + BaseContactEntity.FieldCreateTime + " DESC ";
                using (IDataReader dataReader = ExecuteReaderByWhere(commandText, null, topLimit, order))
                {
                    while (dataReader.Read())
                    {
                        // 2015-11-18 吉日嘎拉 消息的内容不能有，否则会出错，缓存的内容也太大
                        BaseContactEntity contactEntity = BaseEntity.Create<BaseContactEntity>(dataReader, false);
                        // 是否要内容
                        if (!containContents)
                        {
                            contactEntity.Contents = null;
                        }
                        result.Add(contactEntity);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// 获取用户的通知列表
        /// </summary>
        /// <param name="companyId">网点主键</param>
        /// <param name="parentId"></param>
        /// <param name="topLimit">获取前几个</param>
        /// <param name="containContents">是否包含内容</param>
        /// <returns>通知列表</returns>
        public List<BaseContactEntity> GetTopListByCompanyId(string companyId,string  parentId,int topLimit, bool containContents = false)
        {
            List<BaseContactEntity> result = new List<BaseContactEntity>();
            // 获取用户信息
            string commandText = string.Empty;

            // 获取用户所在的单位的信息
            BaseOrganizeEntity organizeEntity = BaseOrganizeManager.GetEntityByCache(companyId);

            if (organizeEntity != null)
            {
                // 2015-11-26 吉日嘎拉 需要过去数据，最近的30天的数据就可以了，太久了意义不大
                commandText = " ( " + BaseContactEntity.FieldCreateTime + " > SYSDATE - 30) AND ((ParentId = '" + parentId + "' AND " +
                              "Enabled = 1 AND " + BaseContactEntity.FieldDeleted + " = 0 AND IsOpen = 1 AND AuditStatus=2) OR (Enabled = 1 AND AuditStatus=2 AND " + BaseContactEntity.FieldDeleted + " = 0 AND Id IN (";
                // 所在省
                if (!string.IsNullOrEmpty(organizeEntity.ProvinceId))
                {
                    commandText += " (SELECT ContactId FROM BaseContactTarget WHERE Category = '0' AND ReceiverId = '" + organizeEntity.ProvinceId + "') ";
                }
                // 所在市
                if (!string.IsNullOrEmpty(organizeEntity.CityId))
                {
                    commandText += " UNION (SELECT ContactId FROM BaseContactTarget WHERE Category = '1' AND ReceiverId = '" + organizeEntity.CityId + "') ";
                }
                // 所在县
                if (!string.IsNullOrEmpty(organizeEntity.DistrictId))
                {
                    commandText += " UNION (SELECT ContactId FROM BaseContactTarget WHERE Category = '2' AND ReceiverId = '" + organizeEntity.DistrictId + "') ";
                }
            }
            // 发给所在单位的
            if (!string.IsNullOrEmpty(companyId))
            {
                commandText += " UNION (SELECT ContactId FROM BaseContactTarget WHERE Category = '3' AND ReceiverId = '" + companyId + "')))) ";
            }

            string order = BaseContactEntity.FieldPriority + "," + BaseContactEntity.FieldCreateTime + " DESC ";
            using (IDataReader dataReader = ExecuteReaderByWhere(commandText, null, topLimit, order))
            {
                while (dataReader.Read())
                {
                    // 2015-11-18 吉日嘎拉 消息的内容不能有，否则会出错，缓存的内容也太大
                    BaseContactEntity contactEntity = BaseEntity.Create<BaseContactEntity>(dataReader, false);
                    // 是否要内容
                    if (!containContents)
                    {
                        contactEntity.Contents = null;
                    }
                    result.Add(contactEntity);
                }
            }

            return result;
        }

        /// <summary>
        /// 定期执行，把置顶的新闻自动下架
        /// 2016-01-04 吉日嘎拉
        /// </summary>
        /// <returns>影响行数</returns>
        public static int Task()
        {
            int result = 0;

            BaseContactManager contactManager = new BaseContactManager();
            string commandText = "UPDATE " + BaseContactEntity.TableName
                                 + "  SET " + BaseContactEntity.FieldPriority + " = 4 "
                                  + "  , " + BaseContactEntity.FieldCancelTopDay + " = 0 "
                                 + "WHERE " + BaseContactEntity.FieldEnabled + " = 1 "
                                 + "      AND " + BaseContactEntity.FieldDeleted + " = 0 "
                             //    + "      AND " + BaseContactEntity.FieldCancelTopDay + " > 0 "
                                 + "      AND " + BaseContactEntity.FieldPriority + " < 4 "
                                 + "      AND " + BaseContactEntity.FieldCreateTime + " > SYSDATE - 30 "
                                 + "      AND " + BaseContactEntity.FieldCreateTime + " < SYSDATE - " + BaseContactEntity.FieldCancelTopDay;
            result = contactManager.DbHelper.ExecuteNonQuery(commandText);

            return result;
        }

    }
}
