// <copyright file="BaseContactDetailsManager.Auto.cs" company="DotNet">
//     Copyright (C) 2021, All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;

namespace DotNet.Business
{
    using Model;
    using Util;

    /// <summary>
    /// BaseContactDetailsManager
    /// 联络单明细表
    ///
    /// 修改记录
    ///
    ///		2010-07-15 版本：1.0 JiRiGaLa 创建主键。
    ///
    /// 版本：1.0
    ///
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2010-07-15</date>
    /// </author>
    /// </summary>
    public partial class BaseContactDetailsManager : BaseManager, IBaseManager
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public BaseContactDetailsManager()
        {
            if (dbHelper == null)
            {
                dbHelper = DbHelperFactory.GetHelper(BaseSystemInfo.MessageDbType, BaseSystemInfo.MessageDbConnection);
            }
            CurrentTableName = BaseContactDetailsEntity.TableName;
            PrimaryKey = "Id";
        }

        /// <summary>
        /// 构造函数
        /// <param name="tableName">指定表名</param>
        /// </summary>
        public BaseContactDetailsManager(string tableName)
        {
            CurrentTableName = tableName;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        public BaseContactDetailsManager(IDbHelper dbHelper)
            : this()
        {
            DbHelper = dbHelper;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="userInfo">操作员信息</param>
        public BaseContactDetailsManager(BaseUserInfo userInfo)
            : this()
        {
            UserInfo = userInfo;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        /// <param name="userInfo">操作员信息</param>
        public BaseContactDetailsManager(IDbHelper dbHelper, BaseUserInfo userInfo)
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
        public BaseContactDetailsManager(IDbHelper dbHelper, BaseUserInfo userInfo, string tableName)
            : this(dbHelper, userInfo)
        {
            CurrentTableName = tableName;
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns>主键</returns>
        public string Add(BaseContactDetailsEntity entity)
        {
            return AddEntity(entity);
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="identity">自增量方式</param>
        /// <returns>主键</returns>
        public string Add(BaseContactDetailsEntity entity, bool identity)
        {
            Identity = identity;
            return AddEntity(entity);
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="entity">实体</param>
        public int Update(BaseContactDetailsEntity entity)
        {
            return UpdateEntity(entity);
        }

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="id">主键</param>
        public BaseContactDetailsEntity GetEntity(string id)
        {
            var entity = new BaseContactDetailsEntity(GetDataTableById(id));
            return entity;
        }

        /// <summary>
        /// 添加实体
        /// </summary>
        /// <param name="entity">实体</param>
        public string AddEntity(BaseContactDetailsEntity entity)
        {
            var sequence = entity.Id;
            if (entity.SortCode == 0)
            {
                var managerSequence = new BaseSequenceManager(DbHelper, Identity);
                sequence = managerSequence.Increment(CurrentTableName);
                entity.SortCode = int.Parse(sequence);
            }
            var sqlBuilder = new SqlBuilder(DbHelper, Identity, ReturnId);
            sqlBuilder.BeginInsert(BaseContactDetailsEntity.TableName, BaseContactDetailsEntity.FieldId);
            if (entity.Id is string)
            {
                Identity = false;
            }
            if (!Identity)
            {
                sqlBuilder.SetValue(BaseContactDetailsEntity.FieldId, entity.Id);
            }
            else
            {
                if (!ReturnId && DbHelper.CurrentDbType == CurrentDbType.Oracle)
                {
                    sqlBuilder.SetFormula(BaseContactDetailsEntity.FieldId, "SEQ_" + CurrentTableName.ToUpper() + ".NEXTVAL ");
                }
                else
                {
                    if (Identity && DbHelper.CurrentDbType == CurrentDbType.Oracle)
                    {
                        if (string.IsNullOrEmpty(sequence))
                        {
                            var managerSequence = new BaseSequenceManager(DbHelper, Identity);
                            sequence = managerSequence.Increment(CurrentTableName);
                        }
                        entity.Id = sequence;
                        sqlBuilder.SetValue(BaseContactDetailsEntity.FieldId, entity.Id);
                    }
                }
            }
            SetEntity(sqlBuilder, entity);
            if (UserInfo != null)
            {
                sqlBuilder.SetValue(BaseContactDetailsEntity.FieldCreateUserId, UserInfo.Id);
                sqlBuilder.SetValue(BaseContactDetailsEntity.FieldCreateBy, UserInfo.RealName);
            }
            sqlBuilder.SetDbNow(BaseContactDetailsEntity.FieldCreateTime);
            if (UserInfo != null)
            {
                sqlBuilder.SetValue(BaseContactDetailsEntity.FieldUpdateUserId, UserInfo.Id);
                sqlBuilder.SetValue(BaseContactDetailsEntity.FieldUpdateBy, UserInfo.RealName);
            }
            sqlBuilder.SetDbNow(BaseContactDetailsEntity.FieldUpdateTime);
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
        /// <param name="entity">实体</param>
        public int UpdateEntity(BaseContactDetailsEntity entity)
        {
            var sqlBuilder = new SqlBuilder(DbHelper);
            sqlBuilder.BeginUpdate(BaseContactDetailsEntity.TableName);
            SetEntity(sqlBuilder, entity);
            if (UserInfo != null)
            {
                sqlBuilder.SetValue(BaseContactDetailsEntity.FieldUpdateUserId, UserInfo.Id);
                sqlBuilder.SetValue(BaseContactDetailsEntity.FieldUpdateBy, UserInfo.RealName);
            }
            sqlBuilder.SetDbNow(BaseContactDetailsEntity.FieldUpdateTime);
            sqlBuilder.SetWhere(BaseContactDetailsEntity.FieldId, entity.Id);
            return sqlBuilder.EndUpdate();
        }

        /// <summary>
        /// 设置实体
        /// </summary>
        /// <param name="sqlBuilder">SQL语句生成器</param>
        /// <param name="entity">实体</param>
        private void SetEntity(SqlBuilder sqlBuilder, BaseContactDetailsEntity entity)
        {
            sqlBuilder.SetValue(BaseContactDetailsEntity.FieldContactId, entity.ContactId);
            sqlBuilder.SetValue(BaseContactDetailsEntity.FieldCategory, entity.Category);
            sqlBuilder.SetValue(BaseContactDetailsEntity.FieldReceiverId, entity.ReceiverId);
            sqlBuilder.SetValue(BaseContactDetailsEntity.FieldReceiverRealName, entity.ReceiverRealName);
            sqlBuilder.SetValue(BaseContactDetailsEntity.FieldIsNew, entity.IsNew);
            sqlBuilder.SetValue(BaseContactDetailsEntity.FieldNewComment, entity.NewComment);
            sqlBuilder.SetValue(BaseContactDetailsEntity.FieldReplied, entity.Replied);
            sqlBuilder.SetValue(BaseContactDetailsEntity.FieldLastViewIp, entity.LastViewIp);
            sqlBuilder.SetValue(BaseContactDetailsEntity.FieldLastViewDate, entity.LastViewDate);
            sqlBuilder.SetValue(BaseContactDetailsEntity.FieldEnabled, entity.Enabled);
            sqlBuilder.SetValue(BaseContactDetailsEntity.FieldDeleted, entity.DeletionStateCode);
            sqlBuilder.SetValue(BaseContactDetailsEntity.FieldSortCode, entity.SortCode);
            sqlBuilder.SetValue(BaseContactDetailsEntity.FieldDescription, entity.Description);
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
