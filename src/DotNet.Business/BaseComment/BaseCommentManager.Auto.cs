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
    /// BaseCommentManager
    /// 评论表
    /// 
    /// 修改记录
    /// 
    /// 2012-05-14 版本：1.0 JiRiGaLa 创建主键。
    /// 
    /// <author>
    ///     <name>JiRiGaLa</name>
    ///     <date>2012-05-14</date>
    /// </author>
    /// </summary>
    public partial class BaseCommentManager : BaseManager, IBaseManager
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public BaseCommentManager()
        {
            if (dbHelper == null)
            {
                dbHelper = DbHelperFactory.GetHelper(BaseSystemInfo.MessageDbType, BaseSystemInfo.MessageDbConnection);
            }
            CurrentTableName = BaseCommentEntity.TableName;
            PrimaryKey = "Id";
        }

        /// <summary>
        /// 构造函数
        /// <param name="tableName">指定表名</param>
        /// </summary>
        public BaseCommentManager(string tableName)
        {
            CurrentTableName = tableName;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        public BaseCommentManager(IDbHelper dbHelper): this()
        {
            DbHelper = dbHelper;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        public BaseCommentManager(BaseUserInfo userInfo) : this()
        {
            UserInfo = userInfo;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        /// <param name="userInfo">用户信息</param>
        public BaseCommentManager(IDbHelper dbHelper, BaseUserInfo userInfo) : this(dbHelper)
        {
            UserInfo = userInfo;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        /// <param name="userInfo">用户信息</param>
        /// <param name="tableName">指定表名</param>
        public BaseCommentManager(IDbHelper dbHelper, BaseUserInfo userInfo, string tableName) : this(dbHelper, userInfo)
        {
            CurrentTableName = tableName;
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns>主键</returns>
        public string Add(BaseCommentEntity entity)
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
        public string Add(BaseCommentEntity entity, bool identity, bool returnId)
        {
            Identity = identity;
            ReturnId = returnId;
            return AddEntity(entity);
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="entity">实体</param>
        public int Update(BaseCommentEntity entity)
        {
            return UpdateEntity(entity);
        }

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="id">主键</param>
        public BaseCommentEntity GetEntity(string id)
        {
            return BaseEntity.Create<BaseCommentEntity>(ExecuteReader(new KeyValuePair<string, object>(BaseCommentEntity.FieldId, id)));
            // return BaseEntity.Create<BaseCommentEntity>(this.GetDataTable(new KeyValuePair<string, object>(BaseCommentEntity.FieldId, id)));
        }

        /// <summary>
        /// 添加实体
        /// </summary>
        /// <param name="entity">实体</param>
        public string AddEntity(BaseCommentEntity entity)
        {
            var sequence = string.Empty;
            Identity = false; 
            if (entity.Id != null)
            {
                sequence = entity.Id;
            }
            var sqlBuilder = new SqlBuilder(DbHelper, Identity, ReturnId);
            sqlBuilder.BeginInsert(CurrentTableName, BaseCommentEntity.FieldId);
            if (!Identity) 
            {
                if (string.IsNullOrEmpty(entity.Id)) 
                {
                    sequence = Guid.NewGuid().ToString("N"); 
                    entity.Id = sequence ;
                }
                sqlBuilder.SetValue(BaseCommentEntity.FieldId, entity.Id);
            }
            else
            {
                if (!ReturnId && (DbHelper.CurrentDbType == CurrentDbType.Oracle || DbHelper.CurrentDbType == CurrentDbType.Db2))
                {
                    if (DbHelper.CurrentDbType == CurrentDbType.Oracle)
                    {
                        sqlBuilder.SetFormula(BaseCommentEntity.FieldId, "SEQ_" + CurrentTableName.ToUpper() + ".NEXTVAL ");
                    }
                    if (DbHelper.CurrentDbType == CurrentDbType.Db2)
                    {
                        sqlBuilder.SetFormula(BaseCommentEntity.FieldId, "NEXT VALUE FOR SEQ_" + CurrentTableName.ToUpper());
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
                        sqlBuilder.SetValue(BaseCommentEntity.FieldId, entity.Id);
                    }
                }
            }
            SetEntity(sqlBuilder, entity);
            if (UserInfo != null) 
            { 
                sqlBuilder.SetValue(BaseCommentEntity.FieldCreateUserId, UserInfo.Id);
                sqlBuilder.SetValue(BaseCommentEntity.FieldCreateBy, UserInfo.RealName);
            } 
            sqlBuilder.SetDbNow(BaseCommentEntity.FieldCreateTime);
            if (UserInfo != null) 
            { 
                sqlBuilder.SetValue(BaseCommentEntity.FieldUpdateUserId, UserInfo.Id);
                sqlBuilder.SetValue(BaseCommentEntity.FieldUpdateBy, UserInfo.RealName);
            } 
            sqlBuilder.SetDbNow(BaseCommentEntity.FieldUpdateTime);
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
        public int UpdateEntity(BaseCommentEntity entity)
        {
            var sqlBuilder = new SqlBuilder(DbHelper);
            sqlBuilder.BeginUpdate(CurrentTableName);
            SetEntity(sqlBuilder, entity);
            if (UserInfo != null) 
            { 
                sqlBuilder.SetValue(BaseCommentEntity.FieldUpdateUserId, UserInfo.Id);
                sqlBuilder.SetValue(BaseCommentEntity.FieldUpdateBy, UserInfo.RealName);
            } 
            sqlBuilder.SetDbNow(BaseCommentEntity.FieldUpdateTime);
            sqlBuilder.SetWhere(BaseCommentEntity.FieldId, entity.Id);
            return sqlBuilder.EndUpdate();
        }

        partial void SetEntityExtend(SqlBuilder sqlBuilder, BaseCommentEntity entity);

        /// <summary>
        /// 设置实体
        /// </summary>
        /// <param name="sqlBuilder">SQL语句生成器</param>
        /// <param name="entity">实体</param>
        private void SetEntity(SqlBuilder sqlBuilder, BaseCommentEntity entity)
        {   
            sqlBuilder.SetValue(BaseCommentEntity.FieldDepartmentId, entity.DepartmentId);
            sqlBuilder.SetValue(BaseCommentEntity.FieldDepartmentName, entity.DepartmentName);
            sqlBuilder.SetValue(BaseCommentEntity.FieldParentId, entity.ParentId);
            sqlBuilder.SetValue(BaseCommentEntity.FieldCategoryCode, entity.CategoryCode);
            sqlBuilder.SetValue(BaseCommentEntity.FieldObjectId, entity.ObjectId);
            sqlBuilder.SetValue(BaseCommentEntity.FieldTargetUrl, entity.TargetUrl);
            sqlBuilder.SetValue(BaseCommentEntity.FieldTitle, entity.Title);
            sqlBuilder.SetValue(BaseCommentEntity.FieldContents, entity.Contents);
            sqlBuilder.SetValue(BaseCommentEntity.FieldIpAddress, entity.IpAddress);
            sqlBuilder.SetValue(BaseCommentEntity.FieldWorked, entity.Worked);
            sqlBuilder.SetValue(BaseCommentEntity.FieldDeleted, entity.DeletionStateCode);
            sqlBuilder.SetValue(BaseCommentEntity.FieldEnabled, entity.Enabled);
            sqlBuilder.SetValue(BaseCommentEntity.FieldDescription, entity.Description);
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
