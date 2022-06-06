//-----------------------------------------------------------------------
// <copyright file="BaseUserLogonManager.Auto.cs" company="DotNet">
//     Copyright (c) 2021, All rights reserved.
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
    /// BaseUserLogonManager
    /// 用户登录信息
    /// 
    /// 修改记录
    ///
    /// 2021-09-26 版本：1.0 Troy.Cui 创建文件。
    ///
    /// <author>
    ///     <name>Troy.Cui</name>
    ///     <date>2021-09-26</date>
    /// </author>
    /// </summary>
    public partial class BaseUserLogonManager : BaseManager, IBaseManager
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public BaseUserLogonManager()
        {
            if (dbHelper == null)
            {
                dbHelper = DbHelperFactory.Create(BaseSystemInfo.UserCenterDbType, BaseSystemInfo.UserCenterDbConnection);
            }
            if (string.IsNullOrEmpty(CurrentTableName))
            {
                CurrentTableName = BaseUserLogonEntity.CurrentTableName;
                //按用户公司分表
                //CurrentTableName = BaseUserLogonEntity.CurrentTableName + GetTableSuffix();
            }
            CurrentTableDescription = FieldExtensions.ToDescription(typeof(BaseUserLogonEntity), "CurrentTableName");
            PrimaryKey = "Id";
        }

        /// <summary>
        /// 构造函数
        /// <param name="tableName">指定表名</param>
        /// </summary>
        public BaseUserLogonManager(string tableName) : this()
        {
            CurrentTableName = tableName;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        public BaseUserLogonManager(IDbHelper dbHelper) : this()
        {
            DbHelper = dbHelper;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        public BaseUserLogonManager(BaseUserInfo userInfo) : this()
        {
            UserInfo = userInfo;
            //按用户公司分表
            //CurrentTableName = BaseUserLogonEntity.CurrentTableName + GetTableSuffix();
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        /// <param name="tableName">指定表名</param>
        public BaseUserLogonManager(BaseUserInfo userInfo, string tableName) : this(userInfo)
        {
            CurrentTableName = tableName;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        /// <param name="userInfo">用户信息</param>
        public BaseUserLogonManager(IDbHelper dbHelper, BaseUserInfo userInfo) : this(dbHelper)
        {
            UserInfo = userInfo;
            //按用户公司分表
            //CurrentTableName = BaseUserLogonEntity.CurrentTableName + GetTableSuffix();
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        /// <param name="userInfo">用户信息</param>
        /// <param name="tableName">指定表名</param>
        public BaseUserLogonManager(IDbHelper dbHelper, BaseUserInfo userInfo, string tableName) : this(dbHelper, userInfo)
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
        public string Add(BaseUserLogonEntity entity, bool identity = true, bool returnId = true)
        {
            Identity = identity;
            ReturnId = returnId;
            entity.Id = AddEntity(entity).ToInt();
            return entity.Id.ToString();
        }

        /// <summary>
        /// 添加或更新(主键是否为0)
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="identity">自增量方式，表主键是否采用自增的策略</param>
        /// <param name="returnId">返回主键，不返回程序允许速度会快，主要是为了主细表批量插入数据优化用的</param>
        /// <returns>主键</returns>
        public string AddOrUpdate(BaseUserLogonEntity entity, bool identity = true, bool returnId = true)
        {
            Identity = identity;
            ReturnId = returnId;
            if (entity.Id == 0)
            {
                entity.Id = AddEntity(entity).ToInt();
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
        public int Update(BaseUserLogonEntity entity)
        {
            return UpdateEntity(entity);
        }

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="id">主键</param>
        public BaseUserLogonEntity GetEntity(string id)
        {
            return ValidateUtil.IsInt(id) ? GetEntity(id.ToInt()) : null;
        }

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="id">主键</param>
        public BaseUserLogonEntity GetEntity(int id)
        {
            return BaseEntity.Create<BaseUserLogonEntity>(GetDataTable(new KeyValuePair<string, object>(PrimaryKey, id)));
            //var cacheKey = CurrentTableName + ".Entity." + id;
            //var cacheTime = TimeSpan.FromMilliseconds(86400000);
            //return CacheUtil.Cache<BaseUserLogonEntity>(cacheKey, () => BaseEntity.Create<BaseUserLogonEntity>(GetDataTable(new KeyValuePair<string, object>(PrimaryKey, id))), true, false, cacheTime);
        }

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="parameters">参数</param>
        public BaseUserLogonEntity GetEntity(List<KeyValuePair<string, object>> parameters)
        {
            return BaseEntity.Create<BaseUserLogonEntity>(GetDataTable(parameters));
        }

        /// <summary>
        /// 添加实体
        /// </summary>
        /// <param name="entity">实体</param>
        public string AddEntity(BaseUserLogonEntity entity)
        {
            var key = string.Empty;
            if (entity.SortCode == 0)
            {
                var managerSequence = new BaseSequenceManager(DbHelper, Identity);
                if (DbHelper.CurrentDbType == CurrentDbType.Oracle || DbHelper.CurrentDbType == CurrentDbType.Db2)
                {
                    key = managerSequence.Increment($"SC_{CurrentTableName}_SEQ");
                }
                else
                {
                    key = managerSequence.Increment(CurrentTableName);
                }
                entity.SortCode = key.ToInt();
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
                        sqlBuilder.SetFormula(PrimaryKey, $"{CurrentTableName}_SEQ.NEXTVAL");
                    }
                    if (DbHelper.CurrentDbType == CurrentDbType.Db2)
                    {
                        sqlBuilder.SetFormula(PrimaryKey, $"NEXT VALUE FOR {CurrentTableName}_SEQ");
                    }
                }
                else
                {
                    if (Identity && (DbHelper.CurrentDbType == CurrentDbType.Oracle || DbHelper.CurrentDbType == CurrentDbType.Db2))
                    {
                        var managerSequence = new BaseSequenceManager(DbHelper);
                        entity.Id = managerSequence.Increment($"{CurrentTableName}_SEQ").ToInt();
                        sqlBuilder.SetValue(PrimaryKey, entity.Id);
                    }
                }
            }
            SetEntity(sqlBuilder, entity);
            if (UserInfo != null)
            {
                sqlBuilder.SetValue(BaseUserLogonEntity.FieldCreateUserId, UserInfo.UserId);
                sqlBuilder.SetValue(BaseUserLogonEntity.FieldCreateUserName, UserInfo.UserName);
                sqlBuilder.SetValue(BaseUserLogonEntity.FieldCreateBy, UserInfo.RealName);
            }
            else
            {
                sqlBuilder.SetValue(BaseUserLogonEntity.FieldCreateBy, entity.CreateBy);
                sqlBuilder.SetValue(BaseUserLogonEntity.FieldCreateUserName, entity.CreateUserName);
            }
            sqlBuilder.SetDbNow(BaseUserLogonEntity.FieldCreateTime);
            sqlBuilder.SetValue(BaseUserLogonEntity.FieldCreateIp, Utils.GetIp());
            if (UserInfo != null)
            {
                sqlBuilder.SetValue(BaseUserLogonEntity.FieldUpdateUserId, UserInfo.UserId);
                sqlBuilder.SetValue(BaseUserLogonEntity.FieldUpdateUserName, UserInfo.UserName);
                sqlBuilder.SetValue(BaseUserLogonEntity.FieldUpdateBy, UserInfo.RealName);
            }
            sqlBuilder.SetDbNow(BaseUserLogonEntity.FieldUpdateTime);
            sqlBuilder.SetValue(BaseUserLogonEntity.FieldUpdateIp, Utils.GetIp());
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
        public int UpdateEntity(BaseUserLogonEntity entity)
        {
            var sqlBuilder = new SqlBuilder(DbHelper);
            sqlBuilder.BeginUpdate(CurrentTableName);
            SetEntity(sqlBuilder, entity);
            if (UserInfo != null)
            {
                sqlBuilder.SetValue(BaseUserLogonEntity.FieldUpdateUserId, UserInfo.UserId);
                sqlBuilder.SetValue(BaseUserLogonEntity.FieldUpdateUserName, UserInfo.UserName);
                sqlBuilder.SetValue(BaseUserLogonEntity.FieldUpdateBy, UserInfo.RealName);
            }
            sqlBuilder.SetDbNow(BaseUserLogonEntity.FieldUpdateTime);
            sqlBuilder.SetValue(BaseUserLogonEntity.FieldUpdateIp, Utils.GetIp());
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
        partial void SetEntityExtend(SqlBuilder sqlBuilder, BaseUserLogonEntity entity);

        /// <summary>
        /// 设置实体
        /// </summary>
        /// <param name="sqlBuilder">Sql语句生成器</param>
        /// <param name="entity">实体</param>
        private void SetEntity(SqlBuilder sqlBuilder, BaseUserLogonEntity entity)
        {
            SetEntityExtend(sqlBuilder, entity);
            sqlBuilder.SetValue(BaseUserLogonEntity.FieldUserId, entity.UserId);
            sqlBuilder.SetValue(BaseUserLogonEntity.FieldUserPassword, entity.UserPassword);
            sqlBuilder.SetValue(BaseUserLogonEntity.FieldOpenId, entity.OpenId);
            sqlBuilder.SetValue(BaseUserLogonEntity.FieldAllowStartTime, entity.AllowStartTime);
            sqlBuilder.SetValue(BaseUserLogonEntity.FieldAllowEndTime, entity.AllowEndTime);
            sqlBuilder.SetValue(BaseUserLogonEntity.FieldLockStartTime, entity.LockStartTime);
            sqlBuilder.SetValue(BaseUserLogonEntity.FieldLockEndTime, entity.LockEndTime);
            sqlBuilder.SetValue(BaseUserLogonEntity.FieldFirstVisitTime, entity.FirstVisitTime);
            sqlBuilder.SetValue(BaseUserLogonEntity.FieldPreviousVisitTime, entity.PreviousVisitTime);
            sqlBuilder.SetValue(BaseUserLogonEntity.FieldLastVisitTime, entity.LastVisitTime);
            sqlBuilder.SetValue(BaseUserLogonEntity.FieldChangePasswordTime, entity.ChangePasswordTime);
            sqlBuilder.SetValue(BaseUserLogonEntity.FieldLogonCount, entity.LogonCount);
            sqlBuilder.SetValue(BaseUserLogonEntity.FieldConcurrentUser, entity.ConcurrentUser);
            sqlBuilder.SetValue(BaseUserLogonEntity.FieldShowCount, entity.ShowCount);
            sqlBuilder.SetValue(BaseUserLogonEntity.FieldPasswordErrorCount, entity.PasswordErrorCount);
            sqlBuilder.SetValue(BaseUserLogonEntity.FieldUserOnline, entity.UserOnline);
            sqlBuilder.SetValue(BaseUserLogonEntity.FieldCheckIpAddress, entity.CheckIpAddress);
            sqlBuilder.SetValue(BaseUserLogonEntity.FieldVerificationCode, entity.VerificationCode);
            sqlBuilder.SetValue(BaseUserLogonEntity.FieldIpAddress, entity.IpAddress);
            sqlBuilder.SetValue(BaseUserLogonEntity.FieldMacAddress, entity.MacAddress);
            sqlBuilder.SetValue(BaseUserLogonEntity.FieldQuestion, entity.Question);
            sqlBuilder.SetValue(BaseUserLogonEntity.FieldAnswerQuestion, entity.AnswerQuestion);
            sqlBuilder.SetValue(BaseUserLogonEntity.FieldSalt, entity.Salt);
            sqlBuilder.SetValue(BaseUserLogonEntity.FieldOpenIdTimeoutTime, entity.OpenIdTimeoutTime);
            sqlBuilder.SetValue(BaseUserLogonEntity.FieldSystemCode, entity.SystemCode);
            sqlBuilder.SetValue(BaseUserLogonEntity.FieldIpAddressName, entity.IpAddressName);
            sqlBuilder.SetValue(BaseUserLogonEntity.FieldPasswordStrength, entity.PasswordStrength);
            sqlBuilder.SetValue(BaseUserLogonEntity.FieldComputerName, entity.ComputerName);
            sqlBuilder.SetValue(BaseUserLogonEntity.FieldNeedModifyPassword, entity.NeedModifyPassword);
            sqlBuilder.SetValue(BaseUserLogonEntity.FieldSortCode, entity.SortCode);
            sqlBuilder.SetValue(BaseUserLogonEntity.FieldDeleted, entity.Deleted);
            sqlBuilder.SetValue(BaseUserLogonEntity.FieldEnabled, entity.Enabled);
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
