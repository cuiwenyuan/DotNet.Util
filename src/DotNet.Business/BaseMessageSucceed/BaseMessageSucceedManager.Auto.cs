//-----------------------------------------------------------------------
// <copyright file="BaseMessageSucceedManager.Auto.cs" company="DotNet">
//     Copyright (c) 2020, All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;

namespace DotNet.Business
{
    using Model;
    using Util;

    /// <summary>
    /// BaseMessageSucceedManager
    /// 成功消息
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
    public partial class BaseMessageSucceedManager : BaseManager, IBaseManager
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public BaseMessageSucceedManager()
        {
            if (dbHelper == null)
            {
                dbHelper = DbHelperFactory.GetHelper(BaseSystemInfo.UserCenterDbType, BaseSystemInfo.UserCenterDbConnection);
            }
            if (string.IsNullOrEmpty(CurrentTableName))
            {
                CurrentTableName = BaseMessageSucceedEntity.TableName;
                //按用户公司分表
                //CurrentTableName = BaseMessageSucceedEntity.TableName + GetTableSuffix();
            }
            PrimaryKey = "Id";
        }

        /// <summary>
        /// 构造函数
        /// <param name="tableName">指定表名</param>
        /// </summary>
        public BaseMessageSucceedManager(string tableName)
        {
            CurrentTableName = tableName;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        public BaseMessageSucceedManager(IDbHelper dbHelper)
            : this()
        {
            DbHelper = dbHelper;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        public BaseMessageSucceedManager(BaseUserInfo userInfo)
            : this()
        {
            UserInfo = userInfo;
            //按用户公司分表
            //CurrentTableName = BaseMessageSucceedEntity.TableName + GetTableSuffix();
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        /// <param name="tableName">指定表名</param>
        public BaseMessageSucceedManager(BaseUserInfo userInfo, string tableName)
            : this(userInfo)
        {
            CurrentTableName = tableName;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        /// <param name="userInfo">用户信息</param>
        public BaseMessageSucceedManager(IDbHelper dbHelper, BaseUserInfo userInfo)
            : this(dbHelper)
        {
            UserInfo = userInfo;
            //按用户公司分表
            //CurrentTableName = BaseMessageSucceedEntity.TableName + GetTableSuffix();
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        /// <param name="userInfo">用户信息</param>
        /// <param name="tableName">指定表名</param>
        public BaseMessageSucceedManager(IDbHelper dbHelper, BaseUserInfo userInfo, string tableName)
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
        public string Add(BaseMessageSucceedEntity entity, bool identity = true, bool returnId = true)
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
        public int Update(BaseMessageSucceedEntity entity)
        {
            return UpdateEntity(entity);
        }

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="id">主键</param>
        public BaseMessageSucceedEntity GetEntity(string id)
        {
            return GetEntity(int.Parse(id));
        }

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="id">主键</param>
        public BaseMessageSucceedEntity GetEntity(int id)
        {
            return BaseEntity.Create<BaseMessageSucceedEntity>(GetDataTable(new KeyValuePair<string, object>(PrimaryKey, id)));
            //var cacheKey = CurrentTableName + ".Entity." + id;
            //var cacheTime = TimeSpan.FromMilliseconds(BaseSystemInfo.MemoryCacheMillisecond * 1000 * 60 * 12);
            //return CacheUtil.Cache<BaseMessageSucceedEntity>(cacheKey, () => BaseEntity.Create<BaseMessageSucceedEntity>(GetDataTable(new KeyValuePair<string, object>(PrimaryKey, id))), true, false, cacheTime);
        }

        /// <summary>
        /// 添加实体
        /// </summary>
        /// <param name="entity">实体</param>
        public string AddEntity(BaseMessageSucceedEntity entity)
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
            if (UserInfo != null)
            {
                if (ValidateUtil.IsInt(UserInfo.CompanyId))
                {
                    sqlBuilder.SetValue(BaseMessageSucceedEntity.FieldUserCompanyId, UserInfo.CompanyId);
                }
                if (ValidateUtil.IsInt(UserInfo.SubCompanyId))
                {
                    sqlBuilder.SetValue(BaseMessageSucceedEntity.FieldUserSubCompanyId, UserInfo.SubCompanyId);
                }
                sqlBuilder.SetValue(BaseMessageSucceedEntity.FieldCreateUserId, UserInfo.Id);
                sqlBuilder.SetValue(BaseMessageSucceedEntity.FieldCreateBy, UserInfo.RealName);
                sqlBuilder.SetValue(BaseMessageSucceedEntity.FieldCreateIp, UserInfo.IpAddress);
            }
            else
            {
                sqlBuilder.SetValue(BaseMessageSucceedEntity.FieldCreateBy, entity.CreateBy);
                sqlBuilder.SetValue(BaseMessageSucceedEntity.FieldCreateIp, Utils.GetIp());
            }
            sqlBuilder.SetDbNow(BaseMessageSucceedEntity.FieldCreateTime);
            if (UserInfo != null)
            {
                sqlBuilder.SetValue(BaseMessageSucceedEntity.FieldUpdateUserId, UserInfo.Id);
                sqlBuilder.SetValue(BaseMessageSucceedEntity.FieldUpdateBy, UserInfo.RealName);
                sqlBuilder.SetValue(BaseMessageSucceedEntity.FieldUpdateIp, UserInfo.IpAddress);
            }
            else
            {
                sqlBuilder.SetValue(BaseMessageSucceedEntity.FieldUpdateIp, Utils.GetIp());
            }
            sqlBuilder.SetDbNow(BaseMessageSucceedEntity.FieldUpdateTime);
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
        public int UpdateEntity(BaseMessageSucceedEntity entity)
        {
            var sqlBuilder = new SqlBuilder(DbHelper);
            sqlBuilder.BeginUpdate(CurrentTableName);
            SetEntity(sqlBuilder, entity);
            if (UserInfo != null)
            {
                sqlBuilder.SetValue(BaseMessageSucceedEntity.FieldUpdateUserId, UserInfo.Id);
                sqlBuilder.SetValue(BaseMessageSucceedEntity.FieldUpdateBy, UserInfo.RealName);
                sqlBuilder.SetValue(BaseMessageSucceedEntity.FieldUpdateIp, UserInfo.IpAddress);
            }
            else
            {
                sqlBuilder.SetValue(BaseMessageSucceedEntity.FieldUpdateIp, Utils.GetIp());
            }
            sqlBuilder.SetDbNow(BaseMessageSucceedEntity.FieldUpdateTime);
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
        partial void SetEntityExpand(SqlBuilder sqlBuilder, BaseMessageSucceedEntity entity);

        /// <summary>
        /// 设置实体
        /// </summary>
        /// <param name="sqlBuilder">Sql语句生成器</param>
        /// <param name="entity">实体</param>
        private void SetEntity(SqlBuilder sqlBuilder, BaseMessageSucceedEntity entity)
        {
            SetEntityExpand(sqlBuilder, entity);
            sqlBuilder.SetValue(BaseMessageSucceedEntity.FieldSource, entity.Source);
            sqlBuilder.SetValue(BaseMessageSucceedEntity.FieldMessageType, entity.MessageType);
            sqlBuilder.SetValue(BaseMessageSucceedEntity.FieldRecipient, entity.Recipient);
            sqlBuilder.SetValue(BaseMessageSucceedEntity.FieldSubject, entity.Subject);
            sqlBuilder.SetValue(BaseMessageSucceedEntity.FieldBody, entity.Body);
            sqlBuilder.SetValue(BaseMessageSucceedEntity.FieldSortCode, entity.SortCode);
            sqlBuilder.SetValue(BaseMessageSucceedEntity.FieldDeleted, entity.DeletionStateCode);
            sqlBuilder.SetValue(BaseMessageSucceedEntity.FieldEnabled, entity.Enabled);
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
