﻿//-----------------------------------------------------------------------
// <copyright file="BaseUserContactManager.Auto.cs" company="DotNet">
//     Copyright (c) 2021, All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;

namespace DotNet.Business
{
    using Model;
    using Util;

    /// <summary>
    /// BaseUserContactManager
    /// 用户联系方式
    /// 
    /// 修改记录
    ///
    /// 2021-09-28 版本：1.0 Troy.Cui 创建文件。
    ///
    /// <author>
    ///     <name>Troy.Cui</name>
    ///     <date>2021-09-28</date>
    /// </author>
    /// </summary>
    public partial class BaseUserContactManager : BaseManager, IBaseManager
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public BaseUserContactManager()
        {
            if (dbHelper == null)
            {
                dbHelper = DbHelperFactory.Create(BaseSystemInfo.UserCenterDbType, BaseSystemInfo.UserCenterDbConnection);
            }
            if (string.IsNullOrEmpty(CurrentTableName))
            {
                CurrentTableName = BaseUserContactEntity.CurrentTableName;
                //按用户公司分表
                //CurrentTableName = BaseUserContactEntity.CurrentTableName + GetTableSuffix();
            }
            CurrentTableDescription = FieldExtensions.ToDescription(typeof(BaseUserContactEntity), "CurrentTableName");
            PrimaryKey = "Id";
        }

        /// <summary>
        /// 构造函数
        /// <param name="tableName">指定表名</param>
        /// </summary>
        public BaseUserContactManager(string tableName) : this()
        {
            CurrentTableName = tableName;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        public BaseUserContactManager(IDbHelper dbHelper) : this()
        {
            DbHelper = dbHelper;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        public BaseUserContactManager(BaseUserInfo userInfo) : this()
        {
            UserInfo = userInfo;
            //按用户公司分表
            //CurrentTableName = BaseUserContactEntity.CurrentTableName + GetTableSuffix();
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        /// <param name="tableName">指定表名</param>
        public BaseUserContactManager(BaseUserInfo userInfo, string tableName) : this(userInfo)
        {
            CurrentTableName = tableName;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        /// <param name="userInfo">用户信息</param>
        public BaseUserContactManager(IDbHelper dbHelper, BaseUserInfo userInfo) : this(dbHelper)
        {
            UserInfo = userInfo;
            //按用户公司分表
            //CurrentTableName = BaseUserContactEntity.CurrentTableName + GetTableSuffix();
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        /// <param name="userInfo">用户信息</param>
        /// <param name="tableName">指定表名</param>
        public BaseUserContactManager(IDbHelper dbHelper, BaseUserInfo userInfo, string tableName) : this(dbHelper, userInfo)
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
        public string Add(BaseUserContactEntity entity, bool identity = true, bool returnId = true)
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
        public string AddOrUpdate(BaseUserContactEntity entity, bool identity = true, bool returnId = true)
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
        public int Update(BaseUserContactEntity entity)
        {
            return UpdateEntity(entity);
        }

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="id">主键</param>
        public BaseUserContactEntity GetEntity(string id)
        {
            return ValidateUtil.IsInt(id) ? GetEntity(int.Parse(id)) : null;
        }

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="id">主键</param>
        public BaseUserContactEntity GetEntity(int id)
        {
            return BaseEntity.Create<BaseUserContactEntity>(GetDataTable(new KeyValuePair<string, object>(PrimaryKey, id)));
            //var cacheKey = CurrentTableName + ".Entity." + id;
            //var cacheTime = TimeSpan.FromMilliseconds(86400000);
            //return CacheUtil.Cache<BaseUserContactEntity>(cacheKey, () => BaseEntity.Create<BaseUserContactEntity>(GetDataTable(new KeyValuePair<string, object>(PrimaryKey, id))), true, false, cacheTime);
        }

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="parameters">参数</param>
        public BaseUserContactEntity GetEntity(List<KeyValuePair<string, object>> parameters)
        {
            return BaseEntity.Create<BaseUserContactEntity>(GetDataTable(parameters));
        }

        /// <summary>
        /// 添加实体
        /// </summary>
        /// <param name="entity">实体</param>
        public string AddEntity(BaseUserContactEntity entity)
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
                        sqlBuilder.SetFormula(PrimaryKey, CurrentTableName.ToUpper() + "_SEQ.NEXTVAL");
                    }
                    if (DbHelper.CurrentDbType == CurrentDbType.Db2)
                    {
                        sqlBuilder.SetFormula(PrimaryKey, "NEXT VALUE FOR " + CurrentTableName.ToUpper() + "_SEQ");
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
                sqlBuilder.SetValue(BaseUserContactEntity.FieldCreateUserId, UserInfo.UserId);
                sqlBuilder.SetValue(BaseUserContactEntity.FieldCreateUserName, UserInfo.UserName);
                sqlBuilder.SetValue(BaseUserContactEntity.FieldCreateBy, UserInfo.RealName);
            }
            else
            {
                sqlBuilder.SetValue(BaseUserContactEntity.FieldCreateBy, entity.CreateBy);
                sqlBuilder.SetValue(BaseUserContactEntity.FieldCreateUserName, entity.CreateUserName);
            }
            sqlBuilder.SetDbNow(BaseUserContactEntity.FieldCreateTime);
            sqlBuilder.SetValue(BaseUserContactEntity.FieldCreateIp, Utils.GetIp());
            if (UserInfo != null)
            {
                sqlBuilder.SetValue(BaseUserContactEntity.FieldUpdateUserId, UserInfo.UserId);
                sqlBuilder.SetValue(BaseUserContactEntity.FieldUpdateUserName, UserInfo.UserName);
                sqlBuilder.SetValue(BaseUserContactEntity.FieldUpdateBy, UserInfo.RealName);
            }
            sqlBuilder.SetDbNow(BaseUserContactEntity.FieldUpdateTime);
            sqlBuilder.SetValue(BaseUserContactEntity.FieldUpdateIp, Utils.GetIp());
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
        public int UpdateEntity(BaseUserContactEntity entity)
        {
            var sqlBuilder = new SqlBuilder(DbHelper);
            sqlBuilder.BeginUpdate(CurrentTableName);
            SetEntity(sqlBuilder, entity);
            if (UserInfo != null)
            {
                sqlBuilder.SetValue(BaseUserContactEntity.FieldUpdateUserId, UserInfo.UserId);
                sqlBuilder.SetValue(BaseUserContactEntity.FieldUpdateUserName, UserInfo.UserName);
                sqlBuilder.SetValue(BaseUserContactEntity.FieldUpdateBy, UserInfo.RealName);
            }
            sqlBuilder.SetDbNow(BaseUserContactEntity.FieldUpdateTime);
            sqlBuilder.SetValue(BaseUserContactEntity.FieldUpdateIp, Utils.GetIp());
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
        partial void SetEntityExtend(SqlBuilder sqlBuilder, BaseUserContactEntity entity);

        /// <summary>
        /// 设置实体
        /// </summary>
        /// <param name="sqlBuilder">Sql语句生成器</param>
        /// <param name="entity">实体</param>
        private void SetEntity(SqlBuilder sqlBuilder, BaseUserContactEntity entity)
        {
            SetEntityExtend(sqlBuilder, entity);
            sqlBuilder.SetValue(BaseUserContactEntity.FieldUserId, entity.UserId);
            sqlBuilder.SetValue(BaseUserContactEntity.FieldShowEmail, entity.ShowEmail);
            sqlBuilder.SetValue(BaseUserContactEntity.FieldEmail, entity.Email);
            sqlBuilder.SetValue(BaseUserContactEntity.FieldEmailValidated, entity.EmailValidated);
            sqlBuilder.SetValue(BaseUserContactEntity.FieldShowMobile, entity.ShowMobile);
            sqlBuilder.SetValue(BaseUserContactEntity.FieldMobile, entity.Mobile);
            sqlBuilder.SetValue(BaseUserContactEntity.FieldMobileValidated, entity.MobileValidated);
            sqlBuilder.SetValue(BaseUserContactEntity.FieldMobileValidatedTime, entity.MobileValidatedTime);
            sqlBuilder.SetValue(BaseUserContactEntity.FieldShortNumber, entity.ShortNumber);
            sqlBuilder.SetValue(BaseUserContactEntity.FieldTelephone, entity.Telephone);
            sqlBuilder.SetValue(BaseUserContactEntity.FieldExtension, entity.Extension);
            sqlBuilder.SetValue(BaseUserContactEntity.FieldQq, entity.Qq);
            sqlBuilder.SetValue(BaseUserContactEntity.FieldWw, entity.Ww);
            sqlBuilder.SetValue(BaseUserContactEntity.FieldIm, entity.Im);
            sqlBuilder.SetValue(BaseUserContactEntity.FieldWeChat, entity.WeChat);
            sqlBuilder.SetValue(BaseUserContactEntity.FieldWeChatValidated, entity.WeChatValidated);
            sqlBuilder.SetValue(BaseUserContactEntity.FieldWeChatOpenId, entity.WeChatOpenId);
            sqlBuilder.SetValue(BaseUserContactEntity.FieldCompanyId, entity.CompanyId);
            sqlBuilder.SetValue(BaseUserContactEntity.FieldCompanyEmail, entity.CompanyEmail);
            sqlBuilder.SetValue(BaseUserContactEntity.FieldEmergencyContact, entity.EmergencyContact);
            sqlBuilder.SetValue(BaseUserContactEntity.FieldEmergencyMobile, entity.EmergencyMobile);
            sqlBuilder.SetValue(BaseUserContactEntity.FieldEmergencyTelephone, entity.EmergencyTelephone);
            sqlBuilder.SetValue(BaseUserContactEntity.FieldSortCode, entity.SortCode);
            sqlBuilder.SetValue(BaseUserContactEntity.FieldDeleted, entity.Deleted);
            sqlBuilder.SetValue(BaseUserContactEntity.FieldEnabled, entity.Enabled);
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
