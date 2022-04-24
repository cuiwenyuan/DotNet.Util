//-----------------------------------------------------------------------
// <copyright file="BasePermissionScopeManager.Auto.cs" company="DotNet">
//     Copyright (c) 2021, All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;

namespace DotNet.Business
{
    using Model;
    using Util;

    /// <summary>
    /// BasePermissionScopeManager
    /// 数据权限
    ///
    /// 修改记录
    ///
    /// 2021-09-27 版本：1.0 Troy.Cui 创建文件。
    ///
    /// <author>
    ///     <name>Troy.Cui</name>
    ///     <date>2021-09-27</date>
    /// </author>
    /// </summary>
    public partial class BasePermissionScopeManager : BaseManager, IBaseManager
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public BasePermissionScopeManager()
        {
            if (dbHelper == null)
            {
                dbHelper = DbHelperFactory.GetHelper(BaseSystemInfo.UserCenterDbType, BaseSystemInfo.UserCenterDbConnection);
            }
            if (string.IsNullOrEmpty(CurrentTableName))
            {
                CurrentTableName = BasePermissionScopeEntity.CurrentTableName;
                //按用户公司分表
                //CurrentTableName = BasePermissionScopeEntity.CurrentTableName + GetTableSuffix();
            }
            CurrentTableDescription = FieldExtensions.ToDescription(typeof(BasePermissionScopeEntity), "CurrentTableName");
            PrimaryKey = "Id";
        }

        /// <summary>
        /// 构造函数
        /// <param name="tableName">指定表名</param>
        /// </summary>
        public BasePermissionScopeManager(string tableName) : this()
        {
            CurrentTableName = tableName;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        public BasePermissionScopeManager(IDbHelper dbHelper) : this()
        {
            DbHelper = dbHelper;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        public BasePermissionScopeManager(BaseUserInfo userInfo) : this()
        {
            UserInfo = userInfo;
            //按用户公司分表
            //CurrentTableName = BasePermissionScopeEntity.CurrentTableName + GetTableSuffix();
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        /// <param name="tableName">指定表名</param>
        public BasePermissionScopeManager(BaseUserInfo userInfo, string tableName) : this(userInfo)
        {
            CurrentTableName = tableName;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        /// <param name="userInfo">用户信息</param>
        public BasePermissionScopeManager(IDbHelper dbHelper, BaseUserInfo userInfo) : this(dbHelper)
        {
            UserInfo = userInfo;
            //按用户公司分表
            //CurrentTableName = BasePermissionScopeEntity.CurrentTableName + GetTableSuffix();
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        /// <param name="userInfo">用户信息</param>
        /// <param name="tableName">指定表名</param>
        public BasePermissionScopeManager(IDbHelper dbHelper, BaseUserInfo userInfo, string tableName) : this(dbHelper, userInfo)
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
        public string Add(BasePermissionScopeEntity entity, bool identity = true, bool returnId = true)
        {
            Identity = identity;
            ReturnId = returnId;
            entity.Id = int.Parse(AddEntity(entity));
            return entity.Id.ToString();
        }

        /// <summary>
        /// 添加或更新(主键是否为0)
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="identity">自增量方式，表主键是否采用自增的策略</param>
        /// <param name="returnId">返回主键，不返回程序允许速度会快，主要是为了主细表批量插入数据优化用的</param>
        /// <returns>主键</returns>
        public string AddOrUpdate(BasePermissionScopeEntity entity, bool identity = true, bool returnId = true)
        {
            Identity = identity;
            ReturnId = returnId;
            if (entity.Id == 0)
            {
                entity.Id = int.Parse(AddEntity(entity));
                return entity.Id.ToString();
            }
            else
            {
                return UpdateEntity(entity) > 0 ? entity.Id.ToString() : string.Empty;
            }
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="entity">实体</param>
        public int Update(BasePermissionScopeEntity entity)
        {
            return UpdateEntity(entity);
        }

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="id">主键</param>
        public BasePermissionScopeEntity GetEntity(string id)
        {
            return ValidateUtil.IsInt(id) ? GetEntity(int.Parse(id)) : null;
        }

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="id">主键</param>
        public BasePermissionScopeEntity GetEntity(int id)
        {
            return BaseEntity.Create<BasePermissionScopeEntity>(ExecuteReader(new KeyValuePair<string, object>(PrimaryKey, id)));
            //var cacheKey = CurrentTableName + ".Entity." + id;
            //var cacheTime = TimeSpan.FromMilliseconds(86400000);
            //return CacheUtil.Cache<BasePermissionScopeEntity>(cacheKey, () => BaseEntity.Create<BasePermissionScopeEntity>(ExecuteReader(new KeyValuePair<string, object>(PrimaryKey, id))), true, false, cacheTime);
        }

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="parameters">参数</param>
        public BasePermissionScopeEntity GetEntity(List<KeyValuePair<string, object>> parameters)
        {
            return BaseEntity.Create<BasePermissionScopeEntity>(ExecuteReader(parameters));
        }

        /// <summary>
        /// 添加实体
        /// </summary>
        /// <param name="entity">实体</param>
        public string AddEntity(BasePermissionScopeEntity entity)
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
                sqlBuilder.SetValue(BasePermissionScopeEntity.FieldCreateUserId, UserInfo.Id);
                sqlBuilder.SetValue(BasePermissionScopeEntity.FieldCreateUserName, UserInfo.UserName);
                sqlBuilder.SetValue(BasePermissionScopeEntity.FieldCreateBy, UserInfo.RealName);
            }
            else
            {
                sqlBuilder.SetValue(BasePermissionScopeEntity.FieldCreateBy, entity.CreateBy);
                sqlBuilder.SetValue(BasePermissionScopeEntity.FieldCreateUserName, entity.CreateUserName);
            }
            sqlBuilder.SetDbNow(BasePermissionScopeEntity.FieldCreateTime);
            sqlBuilder.SetValue(BasePermissionScopeEntity.FieldCreateIp, Utils.GetIp());
            if (UserInfo != null)
            {
                sqlBuilder.SetValue(BasePermissionScopeEntity.FieldUpdateUserId, UserInfo.Id);
                sqlBuilder.SetValue(BasePermissionScopeEntity.FieldUpdateUserName, UserInfo.UserName);
                sqlBuilder.SetValue(BasePermissionScopeEntity.FieldUpdateBy, UserInfo.RealName);
            }
            sqlBuilder.SetDbNow(BasePermissionScopeEntity.FieldUpdateTime);
            sqlBuilder.SetValue(BasePermissionScopeEntity.FieldUpdateIp, Utils.GetIp());
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
        public int UpdateEntity(BasePermissionScopeEntity entity)
        {
            var sqlBuilder = new SqlBuilder(DbHelper);
            sqlBuilder.BeginUpdate(CurrentTableName);
            SetEntity(sqlBuilder, entity);
            if (UserInfo != null)
            {
                sqlBuilder.SetValue(BasePermissionScopeEntity.FieldUpdateUserId, UserInfo.Id);
                sqlBuilder.SetValue(BasePermissionScopeEntity.FieldUpdateUserName, UserInfo.UserName);
                sqlBuilder.SetValue(BasePermissionScopeEntity.FieldUpdateBy, UserInfo.RealName);
            }
            sqlBuilder.SetDbNow(BasePermissionScopeEntity.FieldUpdateTime);
            sqlBuilder.SetValue(BasePermissionScopeEntity.FieldUpdateIp, Utils.GetIp());
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
        partial void SetEntityExtend(SqlBuilder sqlBuilder, BasePermissionScopeEntity entity);

        /// <summary>
        /// 设置实体
        /// </summary>
        /// <param name="sqlBuilder">Sql语句生成器</param>
        /// <param name="entity">实体</param>
        private void SetEntity(SqlBuilder sqlBuilder, BasePermissionScopeEntity entity)
        {
            SetEntityExtend(sqlBuilder, entity);
            sqlBuilder.SetValue(BasePermissionScopeEntity.FieldSystemCode, entity.SystemCode);
            sqlBuilder.SetValue(BasePermissionScopeEntity.FieldResourceCategory, entity.ResourceCategory);
            sqlBuilder.SetValue(BasePermissionScopeEntity.FieldResourceId, entity.ResourceId);
            sqlBuilder.SetValue(BasePermissionScopeEntity.FieldTargetCategory, entity.TargetCategory);
            sqlBuilder.SetValue(BasePermissionScopeEntity.FieldTargetId, entity.TargetId);
            sqlBuilder.SetValue(BasePermissionScopeEntity.FieldPermissionId, entity.PermissionId);
            sqlBuilder.SetValue(BasePermissionScopeEntity.FieldContainChild, entity.ContainChild);
            sqlBuilder.SetValue(BasePermissionScopeEntity.FieldPermissionConstraint, entity.PermissionConstraint);
            sqlBuilder.SetValue(BasePermissionScopeEntity.FieldStartTime, entity.StartTime);
            sqlBuilder.SetValue(BasePermissionScopeEntity.FieldEndTime, entity.EndTime);
            sqlBuilder.SetValue(BasePermissionScopeEntity.FieldDescription, entity.Description);
            sqlBuilder.SetValue(BasePermissionScopeEntity.FieldSortCode, entity.SortCode);
            sqlBuilder.SetValue(BasePermissionScopeEntity.FieldDeleted, entity.Deleted);
            sqlBuilder.SetValue(BasePermissionScopeEntity.FieldEnabled, entity.Enabled);
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
