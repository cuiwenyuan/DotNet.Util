//-----------------------------------------------------------------------
// <copyright file="BaseUserManager.Auto.cs" company="DotNet">
//     Copyright (c) 2021, All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace DotNet.Business
{
    using Model;
    using Util;

    /// <summary>
    /// BaseUserManager
    /// 用户账号
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
    public partial class BaseUserManager : BaseManager, IBaseManager
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public BaseUserManager()
        {
            if (dbHelper == null)
            {
                dbHelper = DbHelperFactory.Create(BaseSystemInfo.UserCenterDbType, BaseSystemInfo.UserCenterDbConnection);
            }
            if (string.IsNullOrEmpty(CurrentTableName))
            {
                CurrentTableName = BaseUserEntity.CurrentTableName;
                //按用户公司分表
                //CurrentTableName = BaseUserEntity.CurrentTableName + GetTableSuffix();
            }
            CurrentTableDescription = FieldExtensions.ToDescription(typeof(BaseUserEntity), "CurrentTableName");
            PrimaryKey = "Id";
        }

        /// <summary>
        /// 构造函数
        /// <param name="tableName">指定表名</param>
        /// </summary>
        public BaseUserManager(string tableName) : this()
        {
            CurrentTableName = tableName;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        public BaseUserManager(IDbHelper dbHelper) : this()
        {
            DbHelper = dbHelper;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        public BaseUserManager(BaseUserInfo userInfo) : this()
        {
            UserInfo = userInfo;
            //按用户公司分表
            //CurrentTableName = BaseUserEntity.CurrentTableName + GetTableSuffix();
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        /// <param name="tableName">指定表名</param>
        public BaseUserManager(BaseUserInfo userInfo, string tableName) : this(userInfo)
        {
            CurrentTableName = tableName;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        /// <param name="userInfo">用户信息</param>
        public BaseUserManager(IDbHelper dbHelper, BaseUserInfo userInfo) : this(dbHelper)
        {
            UserInfo = userInfo;
            //按用户公司分表
            //CurrentTableName = BaseUserEntity.CurrentTableName + GetTableSuffix();
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        /// <param name="userInfo">用户信息</param>
        /// <param name="tableName">指定表名</param>
        public BaseUserManager(IDbHelper dbHelper, BaseUserInfo userInfo, string tableName) : this(dbHelper, userInfo)
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
        public string Add(BaseUserEntity entity, bool identity = true, bool returnId = true)
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
        public string AddOrUpdate(BaseUserEntity entity, bool identity = true, bool returnId = true)
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
        public int Update(BaseUserEntity entity)
        {
            return UpdateEntity(entity);
        }

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="id">主键</param>
        public BaseUserEntity GetEntity(string id)
        {
            return ValidateUtil.IsInt(id) ? GetEntity(int.Parse(id)) : null;
        }

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="id">主键</param>
        public BaseUserEntity GetEntity(int id)
        {
            return BaseEntity.Create<BaseUserEntity>(GetDataTable(new KeyValuePair<string, object>(PrimaryKey, id)));
            //var cacheKey = CurrentTableName + ".Entity." + id;
            //var cacheTime = TimeSpan.FromMilliseconds(86400000);
            //return CacheUtil.Cache<BaseUserEntity>(cacheKey, () => BaseEntity.Create<BaseUserEntity>(GetDataTable(new KeyValuePair<string, object>(PrimaryKey, id))), true, false, cacheTime);
        }

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="parameters">参数</param>
        public BaseUserEntity GetEntity(List<KeyValuePair<string, object>> parameters)
        {
            return BaseEntity.Create<BaseUserEntity>(GetDataTable(parameters));
        }

        /// <summary>
        /// 添加实体
        /// </summary>
        /// <param name="entity">实体</param>
        public string AddEntity(BaseUserEntity entity)
        {
            var key = string.Empty;
            if (entity.SortCode == 0)
            {
                var managerSequence = new BaseSequenceManager(DbHelper, Identity);
                key = managerSequence.Increment(CurrentTableName);
                entity.SortCode = int.Parse(key);
            }

            // 2015-12-11 吉日嘎拉 全部小写，提高Oracle的效率
            entity.QuickQuery = StringUtil.GetPinyin(entity.RealName).ToLower();
            entity.SimpleSpelling = StringUtil.GetSimpleSpelling(entity.RealName).ToLower();

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
                        sqlBuilder.SetFormula(PrimaryKey, CurrentTableName.ToUpper() + "_SEQ.NEXTVAL ");
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
                sqlBuilder.SetValue(BaseUserEntity.FieldCreateUserId, UserInfo.Id);
                sqlBuilder.SetValue(BaseUserEntity.FieldCreateUserName, UserInfo.UserName);
                sqlBuilder.SetValue(BaseUserEntity.FieldCreateBy, UserInfo.RealName);
            }
            else
            {
                sqlBuilder.SetValue(BaseUserEntity.FieldCreateBy, entity.CreateBy);
                sqlBuilder.SetValue(BaseUserEntity.FieldCreateUserName, entity.CreateUserName);
            }
            sqlBuilder.SetDbNow(BaseUserEntity.FieldCreateTime);
            sqlBuilder.SetValue(BaseUserEntity.FieldCreateIp, Utils.GetIp());
            if (UserInfo != null)
            {
                sqlBuilder.SetValue(BaseUserEntity.FieldUpdateUserId, UserInfo.Id);
                sqlBuilder.SetValue(BaseUserEntity.FieldUpdateUserName, UserInfo.UserName);
                sqlBuilder.SetValue(BaseUserEntity.FieldUpdateBy, UserInfo.RealName);
            }
            sqlBuilder.SetDbNow(BaseUserEntity.FieldUpdateTime);
            sqlBuilder.SetValue(BaseUserEntity.FieldUpdateIp, Utils.GetIp());
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
        public int UpdateEntity(BaseUserEntity entity)
        {
            if (string.IsNullOrEmpty(entity.QuickQuery))
            {
                // 2015-12-11 吉日嘎拉 全部小写，提高Oracle的效率
                entity.QuickQuery = StringUtil.GetPinyin(entity.RealName).ToLower();
            }
            if (string.IsNullOrEmpty(entity.SimpleSpelling))
            {
                // 2015-12-11 吉日嘎拉 全部小写，提高Oracle的效率
                entity.SimpleSpelling = StringUtil.GetSimpleSpelling(entity.RealName).ToLower();
            }

            var sqlBuilder = new SqlBuilder(DbHelper);
            sqlBuilder.BeginUpdate(CurrentTableName);
            SetEntity(sqlBuilder, entity);
            if (UserInfo != null)
            {
                sqlBuilder.SetValue(BaseUserEntity.FieldUpdateUserId, UserInfo.Id);
                sqlBuilder.SetValue(BaseUserEntity.FieldUpdateUserName, UserInfo.UserName);
                sqlBuilder.SetValue(BaseUserEntity.FieldUpdateBy, UserInfo.RealName);
            }
            sqlBuilder.SetDbNow(BaseUserEntity.FieldUpdateTime);
            sqlBuilder.SetValue(BaseUserEntity.FieldUpdateIp, Utils.GetIp());
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
        partial void SetEntityExtend(SqlBuilder sqlBuilder, BaseUserEntity entity);

        /// <summary>
        /// 设置实体
        /// </summary>
        /// <param name="sqlBuilder">Sql语句生成器</param>
        /// <param name="entity">实体</param>
        private void SetEntity(SqlBuilder sqlBuilder, BaseUserEntity entity)
        {
            SetEntityExtend(sqlBuilder, entity);
            sqlBuilder.SetValue(BaseUserEntity.FieldUserFrom, entity.UserFrom);
            sqlBuilder.SetValue(BaseUserEntity.FieldUserName, entity.UserName);
            sqlBuilder.SetValue(BaseUserEntity.FieldRealName, entity.RealName);
            sqlBuilder.SetValue(BaseUserEntity.FieldNickName, entity.NickName);
            sqlBuilder.SetValue(BaseUserEntity.FieldAvatarUrl, entity.AvatarUrl);
            sqlBuilder.SetValue(BaseUserEntity.FieldCode, entity.Code);
            sqlBuilder.SetValue(BaseUserEntity.FieldEmployeeNumber, entity.EmployeeNumber);
            sqlBuilder.SetValue(BaseUserEntity.FieldIdCard, entity.IdCard);
            sqlBuilder.SetValue(BaseUserEntity.FieldQuickQuery, entity.QuickQuery);
            sqlBuilder.SetValue(BaseUserEntity.FieldSimpleSpelling, entity.SimpleSpelling);
            sqlBuilder.SetValue(BaseUserEntity.FieldCompanyId, entity.CompanyId);
            sqlBuilder.SetValue(BaseUserEntity.FieldCompanyCode, entity.CompanyCode);
            sqlBuilder.SetValue(BaseUserEntity.FieldCompanyName, entity.CompanyName);
            sqlBuilder.SetValue(BaseUserEntity.FieldSubCompanyId, entity.SubCompanyId);
            sqlBuilder.SetValue(BaseUserEntity.FieldSubCompanyName, entity.SubCompanyName);
            sqlBuilder.SetValue(BaseUserEntity.FieldDepartmentId, entity.DepartmentId);
            sqlBuilder.SetValue(BaseUserEntity.FieldDepartmentName, entity.DepartmentName);
            sqlBuilder.SetValue(BaseUserEntity.FieldSubDepartmentId, entity.SubDepartmentId);
            sqlBuilder.SetValue(BaseUserEntity.FieldSubDepartmentName, entity.SubDepartmentName);
            sqlBuilder.SetValue(BaseUserEntity.FieldWorkgroupId, entity.WorkgroupId);
            sqlBuilder.SetValue(BaseUserEntity.FieldWorkgroupName, entity.WorkgroupName);
            sqlBuilder.SetValue(BaseUserEntity.FieldWorkCategory, entity.WorkCategory);
            sqlBuilder.SetValue(BaseUserEntity.FieldSecurityLevel, entity.SecurityLevel);
            sqlBuilder.SetValue(BaseUserEntity.FieldTitle, entity.Title);
            sqlBuilder.SetValue(BaseUserEntity.FieldDuty, entity.Duty);
            sqlBuilder.SetValue(BaseUserEntity.FieldLang, entity.Lang);
            sqlBuilder.SetValue(BaseUserEntity.FieldGender, entity.Gender);
            sqlBuilder.SetValue(BaseUserEntity.FieldBirthday, entity.Birthday);
            sqlBuilder.SetValue(BaseUserEntity.FieldScore, entity.Score);
            sqlBuilder.SetValue(BaseUserEntity.FieldFans, entity.Fans);
            sqlBuilder.SetValue(BaseUserEntity.FieldHomeAddress, entity.HomeAddress);
            sqlBuilder.SetValue(BaseUserEntity.FieldSignature, entity.Signature);
            sqlBuilder.SetValue(BaseUserEntity.FieldTheme, entity.Theme);
            sqlBuilder.SetValue(BaseUserEntity.FieldIsStaff, entity.IsStaff);
            sqlBuilder.SetValue(BaseUserEntity.FieldIsVisible, entity.IsVisible);
            sqlBuilder.SetValue(BaseUserEntity.FieldCountry, entity.Country);
            sqlBuilder.SetValue(BaseUserEntity.FieldState, entity.State);
            sqlBuilder.SetValue(BaseUserEntity.FieldProvince, entity.Province);
            sqlBuilder.SetValue(BaseUserEntity.FieldCity, entity.City);
            sqlBuilder.SetValue(BaseUserEntity.FieldDistrict, entity.District);
            sqlBuilder.SetValue(BaseUserEntity.FieldAuditStatus, entity.AuditStatus);
            sqlBuilder.SetValue(BaseUserEntity.FieldManagerUserId, entity.ManagerUserId);
            sqlBuilder.SetValue(BaseUserEntity.FieldIsAdministrator, entity.IsAdministrator);
            sqlBuilder.SetValue(BaseUserEntity.FieldIsCheckBalance, entity.IsCheckBalance);
            sqlBuilder.SetValue(BaseUserEntity.FieldDescription, entity.Description);
            sqlBuilder.SetValue(BaseUserEntity.FieldSortCode, entity.SortCode);
            sqlBuilder.SetValue(BaseUserEntity.FieldDeleted, entity.Deleted);
            sqlBuilder.SetValue(BaseUserEntity.FieldEnabled, entity.Enabled);
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
