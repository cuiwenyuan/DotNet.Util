//-----------------------------------------------------------------------
// <copyright file="BaseOrganizationScopeManager.Auto.cs" company="DotNet">
//     Copyright (C) 2021, All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;

namespace DotNet.Business
{
    using Model;
    using Util;

    /// <summary>
    /// BaseOrganizationScopeManager
    /// 基于组织机构的权限范围
    /// 
    /// 修改记录
    /// 
    /// 2013-12-24 版本：1.0 JiRiGaLa 创建文件。
    /// 
    /// <author>
    ///     <name>Troy.Cui</name>
    ///     <date>2013-12-24</date>
    /// </author>
    /// </summary>
    public partial class BaseOrganizationScopeManager : BaseManager, IBaseManager
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public BaseOrganizationScopeManager()
        {
            if (dbHelper == null)
            {
                dbHelper = DbHelperFactory.GetHelper(BaseSystemInfo.UserCenterDbType, BaseSystemInfo.UserCenterDbConnection);
            }
            if (string.IsNullOrEmpty(CurrentTableName))
            {
                CurrentTableName = BaseOrganizationScopeEntity.TableName;
            }
            PrimaryKey = "Id";
        }

        /// <summary>
        /// 构造函数
        /// <param name="tableName">指定表名</param>
        /// </summary>
        public BaseOrganizationScopeManager(string tableName)
        {
            CurrentTableName = tableName;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        public BaseOrganizationScopeManager(IDbHelper dbHelper): this()
        {
            DbHelper = dbHelper;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        public BaseOrganizationScopeManager(BaseUserInfo userInfo) : this()
        {
            UserInfo = userInfo;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        /// <param name="tableName">指定表名</param>
        public BaseOrganizationScopeManager(BaseUserInfo userInfo, string tableName) : this(userInfo)
        {
            CurrentTableName = tableName;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        /// <param name="userInfo">用户信息</param>
        public BaseOrganizationScopeManager(IDbHelper dbHelper, BaseUserInfo userInfo) : this(dbHelper)
        {
            UserInfo = userInfo;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        /// <param name="userInfo">用户信息</param>
        /// <param name="tableName">指定表名</param>
        public BaseOrganizationScopeManager(IDbHelper dbHelper, BaseUserInfo userInfo, string tableName) : this(dbHelper, userInfo)
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
        public string Add(BaseOrganizationScopeEntity entity, bool identity = true, bool returnId = true)
        {
            Identity = identity;
            ReturnId = returnId;
            entity.Id = AddEntity(entity);
            return entity.Id;
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="entity">实体</param>
        public int Update(BaseOrganizationScopeEntity entity)
        {
            return UpdateEntity(entity);
        }

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="id">主键</param>
        public BaseOrganizationScopeEntity GetEntity(string id)
        {
            return BaseEntity.Create<BaseOrganizationScopeEntity>(ExecuteReader(new KeyValuePair<string, object>(BaseOrganizationScopeEntity.FieldId, id)));
            //return GetEntity(int.Parse(id));
        }

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="id">主键</param>
        public BaseOrganizationScopeEntity GetEntity(int id)
        {
            return BaseEntity.Create<BaseOrganizationScopeEntity>(ExecuteReader(new KeyValuePair<string, object>(BaseOrganizationScopeEntity.FieldId, id)));
            // return BaseEntity.Create<BaseOrganizationScopeEntity>(this.GetDataTable(new KeyValuePair<string, object>(this.PrimaryKey, id)));
        }

        /// <summary>
        /// 添加实体
        /// </summary>
        /// <param name="entity">实体</param>
        public string AddEntity(BaseOrganizationScopeEntity entity)
        {
            var result = string.Empty;

            var sqlBuilder = new SqlBuilder(DbHelper, Identity, ReturnId);
            sqlBuilder.BeginInsert(CurrentTableName, PrimaryKey);

            // 若是非空主键，表明已经指定了主键了
            if (!string.IsNullOrEmpty(entity.Id))
            {
                // 这里已经是指定了主键了，所以不需要返回主键了
                sqlBuilder.SetValue(BaseOrganizationScopeEntity.FieldId, entity.Id);
                result = entity.Id;
            }
            else
            {
                if (DbHelper.CurrentDbType == CurrentDbType.Oracle)
                {
                    // 2015-12-23 吉日嘎拉 这里需要兼容一下以前的老的数据结构
                    sqlBuilder.SetFormula(PrimaryKey, "SEQ_" + BaseOrganizationEntity.TableName.ToUpper() + ".NEXTVAL ");
                }
                //MSSQL数据库是自增字段 Troy.Cui 2016-08-17
                //else
                //{
                //    entity.Id = Guid.NewGuid().ToString("N");
                //    result = entity.Id;
                //    sqlBuilder.SetValue(BaseOrganizationScopeEntity.FieldId, entity.Id);
                //}
            }

            SetEntity(sqlBuilder, entity);
            if (UserInfo != null) 
            { 
                sqlBuilder.SetValue(BaseOrganizationScopeEntity.FieldCreateUserId, UserInfo.Id);
                sqlBuilder.SetValue(BaseOrganizationScopeEntity.FieldCreateBy, UserInfo.RealName);
            } 
            sqlBuilder.SetDbNow(BaseOrganizationScopeEntity.FieldCreateTime);
            if (UserInfo != null) 
            { 
                sqlBuilder.SetValue(BaseOrganizationScopeEntity.FieldUpdateUserId, UserInfo.Id);
                sqlBuilder.SetValue(BaseOrganizationScopeEntity.FieldUpdateBy, UserInfo.RealName);
            } 
            sqlBuilder.SetDbNow(BaseOrganizationScopeEntity.FieldUpdateTime);
            if (Identity && (DbHelper.CurrentDbType == CurrentDbType.SqlServer || DbHelper.CurrentDbType == CurrentDbType.Access))
            {
                result = sqlBuilder.EndInsert().ToString();
            }
            else
            {
                sqlBuilder.EndInsert();
            }
            if (Identity && (DbHelper.CurrentDbType == CurrentDbType.Oracle || DbHelper.CurrentDbType == CurrentDbType.Db2))
            {
                return entity.Id;
            }

            return result;
        }

        /// <summary>
        /// 更新实体
        /// </summary>
        /// <param name="entity">实体</param>
        public int UpdateEntity(BaseOrganizationScopeEntity entity)
        {
            var sqlBuilder = new SqlBuilder(DbHelper);
            sqlBuilder.BeginUpdate(CurrentTableName);
            SetEntity(sqlBuilder, entity);
            if (UserInfo != null) 
            { 
                sqlBuilder.SetValue(BaseOrganizationScopeEntity.FieldUpdateUserId, UserInfo.Id);
                sqlBuilder.SetValue(BaseOrganizationScopeEntity.FieldUpdateBy, UserInfo.RealName);
            } 
            sqlBuilder.SetDbNow(BaseOrganizationScopeEntity.FieldUpdateTime);
            sqlBuilder.SetWhere(PrimaryKey, entity.Id);
            return sqlBuilder.EndUpdate();
        }

        // 这个是声明扩展方法
        partial void SetEntityExtend(SqlBuilder sqlBuilder, BaseOrganizationScopeEntity entity);

        /// <summary>
        /// 设置实体
        /// </summary>
        /// <param name="sqlBuilder">SQL语句生成器</param>
        /// <param name="entity">实体</param>
        private void SetEntity(SqlBuilder sqlBuilder, BaseOrganizationScopeEntity entity)
        {
            SetEntityExtend(sqlBuilder, entity);
            sqlBuilder.SetValue(BaseOrganizationScopeEntity.FieldResourceCategory, entity.ResourceCategory);
            sqlBuilder.SetValue(BaseOrganizationScopeEntity.FieldResourceId, entity.ResourceId);
            sqlBuilder.SetValue(BaseOrganizationScopeEntity.FieldPermissionId, entity.PermissionId);
            sqlBuilder.SetValue(BaseOrganizationScopeEntity.FieldAllData, entity.AllData);
            sqlBuilder.SetValue(BaseOrganizationScopeEntity.FieldProvince, entity.Province);
            sqlBuilder.SetValue(BaseOrganizationScopeEntity.FieldCity, entity.City);
            sqlBuilder.SetValue(BaseOrganizationScopeEntity.FieldDistrict, entity.District);
            sqlBuilder.SetValue(BaseOrganizationScopeEntity.FieldStreet, entity.Street);
            sqlBuilder.SetValue(BaseOrganizationScopeEntity.FieldUserCompany, entity.UserCompany);
            sqlBuilder.SetValue(BaseOrganizationScopeEntity.FieldUserSubCompany, entity.UserSubCompany);
            sqlBuilder.SetValue(BaseOrganizationScopeEntity.FieldUserDepartment, entity.UserDepartment);
            sqlBuilder.SetValue(BaseOrganizationScopeEntity.FieldUserSubDepartment, entity.UserSubDepartment);
            sqlBuilder.SetValue(BaseOrganizationScopeEntity.FieldUserWorkgroup, entity.UserWorkgroup);
            sqlBuilder.SetValue(BaseOrganizationScopeEntity.FieldOnlyOwnData, entity.OnlyOwnData);
            sqlBuilder.SetValue(BaseOrganizationScopeEntity.FieldNotAllowed, entity.NotAllowed);
            sqlBuilder.SetValue(BaseOrganizationScopeEntity.FieldByDetails, entity.ByDetails);
            sqlBuilder.SetValue(BaseOrganizationScopeEntity.FieldContainChild, entity.ContainChild);
            sqlBuilder.SetValue(BaseOrganizationScopeEntity.FieldEnabled, entity.Enabled);
            sqlBuilder.SetValue(BaseOrganizationScopeEntity.FieldDeleted, entity.DeletionStateCode);
            sqlBuilder.SetValue(BaseOrganizationScopeEntity.FieldDescription, entity.Description);
        }

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns>影响行数</returns>
        public int Delete(int id)
        {
            return Delete(new List<KeyValuePair<string, object>> { new KeyValuePair<string, object>(PrimaryKey, id) });
        }
    }
}
