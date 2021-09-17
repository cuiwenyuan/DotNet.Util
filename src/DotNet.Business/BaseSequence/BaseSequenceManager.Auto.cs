//--------------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//--------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace DotNet.Business
{
    using Model;
    using Util;

    /// <summary>
    /// BaseSequenceManager
    /// 序列产生器表
    /// 
    /// 修改记录
    /// 
    /// 2012-04-23 版本：1.0 JiRiGaLa 创建主键。
    /// 
    /// <author>
    ///     <name>Troy.Cui</name>
    ///     <date>2012-04-23</date>
    /// </author>
    /// </summary>
    public partial class BaseSequenceManager : BaseManager, IBaseManager
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public BaseSequenceManager()
        {
            if (dbHelper == null)
            {
                //base.dbHelper = DbHelperFactory.GetHelper(BaseSystemInfo.BusinessDbType, BaseSystemInfo.BusinessDbConnection);
                // 原先连到业务库了 改为用户中心库 songbiao
                dbHelper = DbHelperFactory.GetHelper(BaseSystemInfo.UserCenterDbType, BaseSystemInfo.UserCenterDbConnection);
            }
            CurrentTableName = BaseSequenceEntity.TableName;
            PrimaryKey = "Id";
        }

        /// <summary>
        /// 构造函数
        /// <param name="tableName">指定表名</param>
        /// </summary>
        public BaseSequenceManager(string tableName)
        {
            CurrentTableName = tableName;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        public BaseSequenceManager(IDbHelper dbHelper): this()
        {
            DbHelper = dbHelper;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        public BaseSequenceManager(BaseUserInfo userInfo) : this()
        {
            UserInfo = userInfo;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        /// <param name="userInfo">用户信息</param>
        public BaseSequenceManager(IDbHelper dbHelper, BaseUserInfo userInfo) : this(dbHelper)
        {
            UserInfo = userInfo;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        /// <param name="userInfo">用户信息</param>
        /// <param name="tableName">指定表名</param>
        public BaseSequenceManager(IDbHelper dbHelper, BaseUserInfo userInfo, string tableName) : this(dbHelper, userInfo)
        {
            CurrentTableName = tableName;
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns>主键</returns>
        public string Add(BaseSequenceEntity entity)
        {
            return AddEntity(entity);
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="identity">自增量方式</param>
        /// <param name="returnId">返回主键</param>
        /// <returns>主键</returns>
        public string Add(BaseSequenceEntity entity, bool identity, bool returnId)
        {
            Identity = identity;
            ReturnId = returnId;
            return AddEntity(entity);
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="entity">实体</param>
        public int Update(BaseSequenceEntity entity)
        {
            return UpdateEntity(entity);
        }

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="id">主键</param>
        public BaseSequenceEntity GetEntity(string id)
        {
            return BaseEntity.Create<BaseSequenceEntity>(ExecuteReader(new KeyValuePair<string, object>(BaseSequenceEntity.FieldId, id)));
            // return BaseEntity.Create<BaseSequenceEntity>(this.GetDataTable(new KeyValuePair<string, object>(BaseSequenceEntity.FieldId, id)));
        }

        /// <summary>
        /// 添加实体
        /// </summary>
        /// <param name="entity">实体</param>
        public string AddEntity(BaseSequenceEntity entity)
        {
            var sequence = string.Empty;
            Identity = false; 
            if (entity.Id != null)
            {
                sequence = entity.Id;
            }
            var sqlBuilder = new SqlBuilder(DbHelper, Identity, ReturnId);
            sqlBuilder.BeginInsert(CurrentTableName, BaseSequenceEntity.FieldId);
            if (!Identity) 
            {
                if (string.IsNullOrEmpty(entity.Id)) 
                {
                    sequence = Guid.NewGuid().ToString("N"); 
                    entity.Id = sequence ;
                }
                sqlBuilder.SetValue(BaseSequenceEntity.FieldId, entity.Id);
            }
            else
            {
                if (!ReturnId && (DbHelper.CurrentDbType == CurrentDbType.Oracle || DbHelper.CurrentDbType == CurrentDbType.Db2))
                {
                    if (DbHelper.CurrentDbType == CurrentDbType.Oracle)
                    {
                        sqlBuilder.SetFormula(BaseSequenceEntity.FieldId, "SEQ_" + CurrentTableName.ToUpper() + ".NEXTVAL ");
                    }
                    if (DbHelper.CurrentDbType == CurrentDbType.Db2)
                    {
                        sqlBuilder.SetFormula(BaseSequenceEntity.FieldId, "NEXT VALUE FOR SEQ_" + CurrentTableName.ToUpper());
                    }
                }
                else
                {
                    if (Identity && (DbHelper.CurrentDbType == CurrentDbType.Oracle || DbHelper.CurrentDbType == CurrentDbType.Db2))
                    {
                        if (string.IsNullOrEmpty(entity.Id))
                        {
                            if (string.IsNullOrEmpty(sequence))
                            {
                                var managerSequence = new BaseSequenceManager(DbHelper, Identity);
                                sequence = managerSequence.Increment(CurrentTableName);
                            }
                            entity.Id = sequence;
                        }
                        sqlBuilder.SetValue(BaseSequenceEntity.FieldId, entity.Id);
                    }
                }
            }
            SetEntity(sqlBuilder, entity);
            if (UserInfo != null) 
            { 
                sqlBuilder.SetValue(BaseSequenceEntity.FieldCreateUserId, UserInfo.Id);
                sqlBuilder.SetValue(BaseSequenceEntity.FieldCreateBy, UserInfo.RealName);
            } 
            sqlBuilder.SetDbNow(BaseSequenceEntity.FieldCreateTime);
            if (UserInfo != null) 
            { 
                sqlBuilder.SetValue(BaseSequenceEntity.FieldUpdateUserId, UserInfo.Id);
                sqlBuilder.SetValue(BaseSequenceEntity.FieldUpdateBy, UserInfo.RealName);
            } 
            sqlBuilder.SetDbNow(BaseSequenceEntity.FieldUpdateTime);
            if (Identity && (DbHelper.CurrentDbType == CurrentDbType.SqlServer || DbHelper.CurrentDbType == CurrentDbType.Access))
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
        public int UpdateEntity(BaseSequenceEntity entity)
        {
            var sqlBuilder = new SqlBuilder(DbHelper);
            sqlBuilder.BeginUpdate(CurrentTableName);
            SetEntity(sqlBuilder, entity);
            if (UserInfo != null) 
            { 
                sqlBuilder.SetValue(BaseSequenceEntity.FieldUpdateUserId, UserInfo.Id);
                sqlBuilder.SetValue(BaseSequenceEntity.FieldUpdateBy, UserInfo.RealName);
            } 
            sqlBuilder.SetDbNow(BaseSequenceEntity.FieldUpdateTime);
            sqlBuilder.SetWhere(BaseSequenceEntity.FieldId, entity.Id);
            return sqlBuilder.EndUpdate();
        }

        partial void SetEntityExtend(SqlBuilder sqlBuilder, BaseSequenceEntity entity);

        /// <summary>
        /// 设置实体
        /// </summary>
        /// <param name="sqlBuilder">SQL语句生成器</param>
        /// <param name="entity">实体</param>
        private void SetEntity(SqlBuilder sqlBuilder, BaseSequenceEntity entity)
        {
            sqlBuilder.SetValue(BaseSequenceEntity.FieldFullName, entity.FullName);
            sqlBuilder.SetValue(BaseSequenceEntity.FieldPrefix, entity.Prefix);
            sqlBuilder.SetValue(BaseSequenceEntity.FieldDelimiter, entity.Delimiter);
            sqlBuilder.SetValue(BaseSequenceEntity.FieldSequence, entity.Sequence);
            sqlBuilder.SetValue(BaseSequenceEntity.FieldReduction, entity.Reduction);
            sqlBuilder.SetValue(BaseSequenceEntity.FieldStep, entity.Step);
            sqlBuilder.SetValue(BaseSequenceEntity.FieldIsVisible, entity.IsVisible);
            sqlBuilder.SetValue(BaseSequenceEntity.FieldDescription, entity.Description);
            SetEntityExtend(sqlBuilder, entity);
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
