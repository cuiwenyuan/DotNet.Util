//-----------------------------------------------------------------------
// <copyright file="BaseMessageQueueManager.Auto.cs" company="DotNet">
//     Copyright (c) 2020, All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;

namespace DotNet.Business
{
    using Model;
    using Util;

    /// <summary>
    /// BaseMessageQueueManager
    /// 消息队列
    /// 
    /// 修改纪录
    /// 
    /// 2020-03-22 版本：1.0 Troy.Cui 创建文件。
    /// 
    /// <author>
    ///     <name>Troy.Cui</name>
    ///     <date>2020-03-22</date>
    /// </author>
    /// </summary>
    public partial class BaseMessageQueueManager : BaseManager, IBaseManager
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public BaseMessageQueueManager()
        {
            if (dbHelper == null)
            {
                dbHelper = DbHelperFactory.GetHelper(BaseSystemInfo.UserCenterDbType, BaseSystemInfo.UserCenterDbConnection);
            }
            if (string.IsNullOrEmpty(CurrentTableName))
            {
                CurrentTableName = BaseMessageQueueEntity.TableName;
                //按用户公司分表
                //CurrentTableName = BaseMessageQueueEntity.TableName + GetTableSuffix();
            }
            PrimaryKey = "Id";
        }

        /// <summary>
        /// 构造函数
        /// <param name="tableName">指定表名</param>
        /// </summary>
        public BaseMessageQueueManager(string tableName)
        {
            CurrentTableName = tableName;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        public BaseMessageQueueManager(IDbHelper dbHelper)
            : this()
        {
            DbHelper = dbHelper;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        public BaseMessageQueueManager(BaseUserInfo userInfo)
            : this()
        {
            UserInfo = userInfo;
            //按用户公司分表
            //CurrentTableName = BaseMessageQueueEntity.TableName + GetTableSuffix();
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        /// <param name="tableName">指定表名</param>
        public BaseMessageQueueManager(BaseUserInfo userInfo, string tableName)
            : this(userInfo)
        {
            CurrentTableName = tableName;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        /// <param name="userInfo">用户信息</param>
        public BaseMessageQueueManager(IDbHelper dbHelper, BaseUserInfo userInfo)
            : this(dbHelper)
        {
            UserInfo = userInfo;
            //按用户公司分表
            //CurrentTableName = BaseMessageQueueEntity.TableName + GetTableSuffix();
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        /// <param name="userInfo">用户信息</param>
        /// <param name="tableName">指定表名</param>
        public BaseMessageQueueManager(IDbHelper dbHelper, BaseUserInfo userInfo, string tableName)
            : this(dbHelper, userInfo)
        {
            CurrentTableName = tableName;
        }

        /// <summary>
        /// 添加, 这里可以人工干预，提高程序的性能
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="identity">自增量方式，表主键是否采用自增的策略</param>
        /// <param name="returnId">返回主键，不返回程序允许速度会快，主要是为了主细表批量插入数据优化用的</param>
        /// <returns>主键</returns>
        public string Add(BaseMessageQueueEntity entity, bool identity = true, bool returnId = true)
        {
            Identity = identity;
            ReturnId = returnId;
            entity.Id = int.Parse(AddEntity(entity));
            return entity.Id.ToString();
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="entity">实体</param>
        public int Update(BaseMessageQueueEntity entity)
        {
            return UpdateEntity(entity);
        }

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="id">主键</param>
        public BaseMessageQueueEntity GetEntity(string id)
        {
            return GetEntity(int.Parse(id));
        }

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="id">主键</param>
        public BaseMessageQueueEntity GetEntity(int id)
        {
            return BaseEntity.Create<BaseMessageQueueEntity>(GetDataTable(new KeyValuePair<string, object>(PrimaryKey, id)));
            //var cacheKey = CurrentTableName + ".Entity." + id;
            //var cacheTime = TimeSpan.FromMilliseconds(BaseSystemInfo.MemoryCacheMillisecond * 1000 * 60 * 12);
            //return CacheUtil.Cache<BaseMessageQueueEntity>(cacheKey, () => BaseEntity.Create<BaseMessageQueueEntity>(GetDataTable(new KeyValuePair<string, object>(PrimaryKey, id))), true, false, cacheTime);
        }

        /// <summary>
        /// 添加实体
        /// </summary>
        /// <param name="entity">实体</param>
        public string AddEntity(BaseMessageQueueEntity entity)
        {
            var key = string.Empty;
            if (entity.SortCode == 0)
            {
                var managerSequence = new BaseSequenceManager(DbHelper, Identity);
                key = managerSequence.Increment(CurrentTableName);
                entity.SortCode = int.Parse(key);
            }
            var sqlBuilder = new SqlBuilder(DbHelper, Identity, ReturnId);
            sqlBuilder.BeginInsert(CurrentTableName, PrimaryKey);
            if (!Identity)
            {
                // 这里已经是指定了主键了，所以不需要返回主键了
                sqlBuilder.ReturnId = false;
                sqlBuilder.SetValue(PrimaryKey, entity.Id);
            }
            else
            {
                if (!ReturnId && (DbHelper.CurrentDbType == CurrentDbType.Oracle || DbHelper.CurrentDbType == CurrentDbType.Db2))
                {
                    if (DbHelper.CurrentDbType == CurrentDbType.Oracle)
                    {
                        sqlBuilder.SetFormula(PrimaryKey, "SEQ_" + CurrentTableName.ToUpper() + ".NEXTVAL ");
                    }
                    if (DbHelper.CurrentDbType == CurrentDbType.Db2)
                    {
                        sqlBuilder.SetFormula(PrimaryKey, "NEXT VALUE FOR SEQ_" + CurrentTableName.ToUpper());
                    }
                }
                else
                {
                    if (Identity && (DbHelper.CurrentDbType == CurrentDbType.Oracle || DbHelper.CurrentDbType == CurrentDbType.Db2))
                    {
                        var managerSequence = new BaseSequenceManager(DbHelper);
                        entity.Id = int.Parse(managerSequence.Increment(CurrentTableName));
                        sqlBuilder.SetValue(PrimaryKey, entity.Id);
                    }
                }
            }
            SetEntity(sqlBuilder, entity);
            //设置默认的Source为站点的
            if (string.IsNullOrWhiteSpace(entity.Source))
            {
                entity.Source = BaseSystemInfo.ApplicationId;
            }

            if (UserInfo != null)
            {
                if (ValidateUtil.IsInt(UserInfo.CompanyId))
                {
                    sqlBuilder.SetValue(BaseMessageQueueEntity.FieldUserCompanyId, UserInfo.CompanyId);
                }
                if (ValidateUtil.IsInt(UserInfo.SubCompanyId))
                {
                    sqlBuilder.SetValue(BaseMessageQueueEntity.FieldUserSubCompanyId, UserInfo.SubCompanyId);
                }
                sqlBuilder.SetValue(BaseMessageQueueEntity.FieldCreateUserId, UserInfo.Id);
                sqlBuilder.SetValue(BaseMessageQueueEntity.FieldCreateBy, UserInfo.RealName);
                sqlBuilder.SetValue(BaseMessageQueueEntity.FieldCreateIp, UserInfo.IpAddress);
            }
            else
            {
                sqlBuilder.SetValue(BaseMessageQueueEntity.FieldCreateBy, entity.CreateBy);
                sqlBuilder.SetValue(BaseMessageQueueEntity.FieldCreateIp, Utils.GetIp());
            }
            sqlBuilder.SetDbNow(BaseMessageQueueEntity.FieldCreateTime);
            if (UserInfo != null)
            {
                sqlBuilder.SetValue(BaseMessageQueueEntity.FieldUpdateUserId, UserInfo.Id);
                sqlBuilder.SetValue(BaseMessageQueueEntity.FieldUpdateBy, UserInfo.RealName);
                sqlBuilder.SetValue(BaseMessageQueueEntity.FieldUpdateIp, UserInfo.IpAddress);
            }
            else
            {
                sqlBuilder.SetValue(BaseMessageQueueEntity.FieldUpdateIp, Utils.GetIp());
            }
            sqlBuilder.SetDbNow(BaseMessageQueueEntity.FieldUpdateTime);
            if (Identity && (DbHelper.CurrentDbType == CurrentDbType.SqlServer || DbHelper.CurrentDbType == CurrentDbType.Access))
            {
                key = sqlBuilder.EndInsert().ToString();
            }
            else
            {
                sqlBuilder.EndInsert();
            }
            if (Identity && (DbHelper.CurrentDbType == CurrentDbType.Oracle || DbHelper.CurrentDbType == CurrentDbType.Db2))
            {
                //return entity.Id.ToString();
                key = entity.Id.ToString();
            }
            if (!string.IsNullOrWhiteSpace(key))
            {
                RemoveCache();
            }
            return key;
        }

        /// <summary>
        /// 更新实体
        /// </summary>
        /// <param name="entity">实体</param>
        public int UpdateEntity(BaseMessageQueueEntity entity)
        {
            var sqlBuilder = new SqlBuilder(DbHelper);
            sqlBuilder.BeginUpdate(CurrentTableName);
            SetEntity(sqlBuilder, entity);
            if (UserInfo != null)
            {
                sqlBuilder.SetValue(BaseMessageQueueEntity.FieldUpdateUserId, UserInfo.Id);
                sqlBuilder.SetValue(BaseMessageQueueEntity.FieldUpdateBy, UserInfo.RealName);
                sqlBuilder.SetValue(BaseMessageQueueEntity.FieldUpdateIp, UserInfo.IpAddress);
            }
            else
            {
                sqlBuilder.SetValue(BaseMessageQueueEntity.FieldUpdateIp, Utils.GetIp());
            }
            sqlBuilder.SetDbNow(BaseMessageQueueEntity.FieldUpdateTime);
            sqlBuilder.SetWhere(PrimaryKey, entity.Id);
            //return sqlBuilder.EndUpdate();
            var result = sqlBuilder.EndUpdate();
            if (result > 0)
            {
                RemoveCache(entity.Id);
            }
            return result;
        }

        // 这个是声明扩展方法
        partial void SetEntityExpand(SqlBuilder sqlBuilder, BaseMessageQueueEntity entity);

        /// <summary>
        /// 设置实体
        /// </summary>
        /// <param name="sqlBuilder">Sql语句生成器</param>
        /// <param name="entity">实体</param>
        private void SetEntity(SqlBuilder sqlBuilder, BaseMessageQueueEntity entity)
        {
            SetEntityExpand(sqlBuilder, entity);
            sqlBuilder.SetValue(BaseMessageQueueEntity.FieldSource, entity.Source);
            sqlBuilder.SetValue(BaseMessageQueueEntity.FieldMessageType, entity.MessageType);
            sqlBuilder.SetValue(BaseMessageQueueEntity.FieldRecipient, entity.Recipient);
            sqlBuilder.SetValue(BaseMessageQueueEntity.FieldSubject, entity.Subject);
            sqlBuilder.SetValue(BaseMessageQueueEntity.FieldBody, entity.Body);
            sqlBuilder.SetValue(BaseMessageQueueEntity.FieldFailCount, entity.FailCount);
            sqlBuilder.SetValue(BaseMessageQueueEntity.FieldSortCode, entity.SortCode);
            sqlBuilder.SetValue(BaseMessageQueueEntity.FieldDeleted, entity.DeletionStateCode);
            sqlBuilder.SetValue(BaseMessageQueueEntity.FieldEnabled, entity.Enabled);
        }

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns>影响行数</returns>
        public int Delete(int id)
        {
            var result = Delete(new List<KeyValuePair<string, object>> { new KeyValuePair<string, object>(PrimaryKey, id) });
            if (result > 0)
            {
                RemoveCache(id);
            }
            return result;
        }
    }
}
