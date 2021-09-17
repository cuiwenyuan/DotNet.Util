//-----------------------------------------------------------------------
// <copyright file="BaseContactManager.Auto.cs" company="DotNet">
//     Copyright (C) 2021, All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;

namespace DotNet.Business
{
    using Model;
    using Util;

    /// <summary>
    /// BaseContactManager
    /// 联络单主表
    ///
    /// 修改记录
    ///
    ///		2015-10-30 版本：2.0 JiRiGaLa 必读、必回。
    ///		2015-09-09 版本：2.0 JiRiGaLa 添加支持单实例。
    ///		2010-07-15 版本：1.0 JiRiGaLa 创建主键。
    ///
    /// 版本：2.0
    ///
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2015-10-30</date>
    /// </author>
    /// </summary>
    public partial class BaseContactManager : BaseManager, IBaseManager
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public BaseContactManager()
        {
            if (dbHelper == null)
            {
                dbHelper = DbHelperFactory.GetHelper(BaseSystemInfo.MessageDbType, BaseSystemInfo.MessageDbConnection);
            }
            CurrentTableName = BaseContactEntity.TableName;
            PrimaryKey = "Id";
        }

        /// <summary>
        /// 构造函数
        /// <param name="tableName">指定表名</param>
        /// </summary>
        public BaseContactManager(string tableName)
        {
            CurrentTableName = tableName;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        public BaseContactManager(IDbHelper dbHelper)
            : this()
        {
            DbHelper = dbHelper;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="userInfo">操作员信息</param>
        public BaseContactManager(BaseUserInfo userInfo)
            : this()
        {
            UserInfo = userInfo;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        /// <param name="userInfo">操作员信息</param>
        public BaseContactManager(IDbHelper dbHelper, BaseUserInfo userInfo)
            : this(dbHelper)
        {
            UserInfo = userInfo;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        /// <param name="userInfo">操作员信息</param>
        /// <param name="tableName">指定表名</param>
        public BaseContactManager(IDbHelper dbHelper, BaseUserInfo userInfo, string tableName)
            : this(dbHelper, userInfo)
        {
            CurrentTableName = tableName;
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns>主键</returns>
        public string Add(BaseContactEntity entity)
        {
            return AddEntity(entity);
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="userInfo">操作员信息</param>
        /// <param name="entity">实体</param>
        /// <returns>主键</returns>
        public string Add(BaseUserInfo userInfo, BaseContactEntity entity)
        {
            return AddEntity(userInfo, entity);
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="identity">自增量方式</param>
        /// <returns>主键</returns>
        public string Add(BaseContactEntity entity, bool identity)
        {
            Identity = identity;
            return AddEntity(entity);
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns>影响行数</returns>
        public int Update(BaseContactEntity entity)
        {
            return UpdateEntity(UserInfo, entity);
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="userInfo">操作员</param>
        /// <param name="entity">实体</param>
        /// <returns>影响行数</returns>
        public int Update(BaseUserInfo userInfo, BaseContactEntity entity)
        {
            return UpdateEntity(userInfo, entity);
        }

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="id">主键</param>
        public BaseContactEntity GetEntity(string id)
        {
            return new BaseContactEntity(GetDataTableById(id));
        }

        /// <summary>
        /// 添加实体
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns>主键</returns>
        public string AddEntity(BaseContactEntity entity)
        {
            return AddEntity(UserInfo, entity);
        }

        /// <summary>
        /// 添加实体
        /// </summary>
        /// <param name="userInfo">操作员</param>
        /// <param name="entity">实体</param>
        /// <returns>主键</returns>
        public string AddEntity(BaseUserInfo userInfo, BaseContactEntity entity)
        {
            var sequence = entity.Id;
            if (entity.SortCode == 0)
            {
                var managerSequence = new BaseSequenceManager(DbHelper, Identity);
                sequence = managerSequence.Increment(CurrentTableName);
                entity.SortCode = int.Parse(sequence);
            }
            if (entity.Id is string)
            {
                Identity = false;
                ReturnId = false;
            }
            var sqlBuilder = new SqlBuilder(DbHelper, Identity, ReturnId);
            sqlBuilder.BeginInsert(BaseContactEntity.TableName, BaseContactEntity.FieldId);
            if (!Identity)
            {
                sqlBuilder.SetValue(BaseContactEntity.FieldId, entity.Id);
            }
            SetEntity(sqlBuilder, entity);
            if (userInfo != null)
            {
                sqlBuilder.SetValue(BaseContactEntity.FieldCreateUserId, userInfo.Id);
                sqlBuilder.SetValue(BaseContactEntity.FieldCreateBy, userInfo.RealName);
            }
            sqlBuilder.SetDbNow(BaseContactEntity.FieldCreateTime);
            if (userInfo != null)
            {
                sqlBuilder.SetValue(BaseContactEntity.FieldUpdateUserId, userInfo.Id);
                sqlBuilder.SetValue(BaseContactEntity.FieldUpdateBy, userInfo.RealName);
            }
            sqlBuilder.SetDbNow(BaseContactEntity.FieldUpdateTime);
            if (DbHelper.CurrentDbType == CurrentDbType.SqlServer && Identity)
            {
                sequence = sqlBuilder.EndInsert().ToString();
            }
            else
            {
                sqlBuilder.EndInsert();
            }
            return sequence;
        }

        /// <summary>
        /// 更新实体
        /// </summary>
        /// <param name="userInfo">操作员</param>
        /// <param name="entity">实体</param>
        /// <returns>影响行数</returns>
        public int UpdateEntity(BaseUserInfo userInfo, BaseContactEntity entity)
        {
            var sqlBuilder = new SqlBuilder(DbHelper);
            sqlBuilder.BeginUpdate(BaseContactEntity.TableName);
            SetEntity(sqlBuilder, entity);
            if (userInfo != null)
            {
                sqlBuilder.SetValue(BaseContactEntity.FieldUpdateUserId, userInfo.Id);
                sqlBuilder.SetValue(BaseContactEntity.FieldUpdateBy, userInfo.RealName);
            }
            sqlBuilder.SetDbNow(BaseContactEntity.FieldUpdateTime);
            sqlBuilder.SetWhere(BaseContactEntity.FieldId, entity.Id);
            return sqlBuilder.EndUpdate();
        }

        /// <summary>
        /// 设置实体
        /// </summary>
        /// <param name="sqlBuilder">SQL语句生成器</param>
        /// <param name="entity">实体</param>
        private void SetEntity(SqlBuilder sqlBuilder, BaseContactEntity entity)
        {
            sqlBuilder.SetValue(BaseContactEntity.FieldParentId, entity.ParentId);
            sqlBuilder.SetValue(BaseContactEntity.FieldTitle, entity.Title);
            sqlBuilder.SetValue(BaseContactEntity.FieldColor, entity.Color);
            sqlBuilder.SetValue(BaseContactEntity.FieldStyle, entity.Style);
            sqlBuilder.SetValue(BaseContactEntity.FieldContents, entity.Contents);
            sqlBuilder.SetValue(BaseContactEntity.FieldPriority, entity.Priority);
            sqlBuilder.SetValue(BaseContactEntity.FieldCancelTopDay, entity.CancelTopDay);
            sqlBuilder.SetValue(BaseContactEntity.FieldSendCount, entity.SendCount);
            sqlBuilder.SetValue(BaseContactEntity.FieldReadCount, entity.ReadCount);
            sqlBuilder.SetValue(BaseContactEntity.FieldReplyCount, entity.ReplyCount);
            sqlBuilder.SetValue(BaseContactEntity.FieldSource, entity.Source);
            sqlBuilder.SetValue(BaseContactEntity.FieldIsOpen, entity.IsOpen);
            sqlBuilder.SetValue(BaseContactEntity.FieldCategoryCode, entity.CategoryCode);
            sqlBuilder.SetValue(BaseContactEntity.FieldLabelMark, entity.LabelMark);
            sqlBuilder.SetValue(BaseContactEntity.FieldIpAddress, entity.IpAddress);
            sqlBuilder.SetValue(BaseContactEntity.FieldAllowComments, entity.AllowComments);
            sqlBuilder.SetValue(BaseContactEntity.FieldMustRead, entity.MustRead);
            sqlBuilder.SetValue(BaseContactEntity.FieldMustReply, entity.MustReply);
            sqlBuilder.SetValue(BaseContactEntity.FieldCommentUserId, entity.CommentUserId);
            sqlBuilder.SetValue(BaseContactEntity.FieldCommentUserRealName, entity.CommentUserRealName);
            sqlBuilder.SetValue(BaseContactEntity.FieldCommentDate, entity.CommentDate);
            sqlBuilder.SetValue(BaseContactEntity.FieldDeleted, entity.DeletionStateCode);
            sqlBuilder.SetValue(BaseContactEntity.FieldAuditStatus, entity.AuditStatus);
            sqlBuilder.SetValue(BaseContactEntity.FieldAuditUserId, entity.AuditUserId);
            sqlBuilder.SetValue(BaseContactEntity.FieldAuditUserRealName, entity.AuditUserRealName);
            sqlBuilder.SetValue(BaseContactEntity.FieldEnabled, entity.Enabled);
            sqlBuilder.SetValue(BaseContactEntity.FieldCreateDepartment, entity.CreateDepartment);
            sqlBuilder.SetValue(BaseContactEntity.FieldCreateCompany, entity.CreateCompany);
            sqlBuilder.SetValue(BaseContactEntity.FieldCreateCompanyId, entity.CreateCompanyId);
            sqlBuilder.SetValue(BaseContactEntity.FieldSortCode, entity.SortCode);
            sqlBuilder.SetValue(BaseContactEntity.FieldDescription, entity.Description);
        }

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns>影响行数</returns>
        public int Delete(string id)
        {
            return Delete(new List<KeyValuePair<string, object>> { new KeyValuePair<string, object>(PrimaryKey, id) });
        }
    }
}
