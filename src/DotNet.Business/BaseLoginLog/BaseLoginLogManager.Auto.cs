//-----------------------------------------------------------------------
// <copyright file="BaseLoginLogManager.Auto.cs" company="DotNet">
//     Copyright (C) 2021, All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;

namespace DotNet.Business
{
    using Model;
    using Util;

    /// <summary>
    /// BaseLoginLogManager
    /// 系统登录日志表
    /// 
    /// 修改记录
    /// 
    /// 2014-03-18 版本：1.0 JiRiGaLa 创建文件。
    /// 
    /// <author>
    ///     <name>Troy.Cui</name>
    ///     <date>2014-03-18</date>
    /// </author>
    /// </summary>
    public partial class BaseLoginLogManager : BaseManager, IBaseManager
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public BaseLoginLogManager()
        {
            if (dbHelper == null)
            {
                dbHelper = DbHelperFactory.GetHelper(BaseSystemInfo.LoginLogDbType, BaseSystemInfo.LoginLogDbConnection);
            }
            if (string.IsNullOrEmpty(CurrentTableName))
            {
                CurrentTableName = BaseLoginLogEntity.TableName;
            }
            PrimaryKey = "Id";
        }

        /// <summary>
        /// 构造函数
        /// <param name="tableName">指定表名</param>
        /// </summary>
        public BaseLoginLogManager(string tableName) : this()
        {
            CurrentTableName = tableName;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        public BaseLoginLogManager(IDbHelper dbHelper): this()
        {
            DbHelper = dbHelper;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        public BaseLoginLogManager(BaseUserInfo userInfo) : this()
        {
            UserInfo = userInfo;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        /// <param name="tableName">指定表名</param>
        public BaseLoginLogManager(BaseUserInfo userInfo, string tableName) : this(userInfo)
        {
            CurrentTableName = tableName;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        /// <param name="tableName">表名</param>
        public BaseLoginLogManager(IDbHelper dbHelper, string tableName) : this(dbHelper)
        {
            CurrentTableName = tableName;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        /// <param name="userInfo">用户信息</param>
        public BaseLoginLogManager(IDbHelper dbHelper, BaseUserInfo userInfo) : this(dbHelper)
        {
            UserInfo = userInfo;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        /// <param name="userInfo">用户信息</param>
        /// <param name="tableName">指定表名</param>
        public BaseLoginLogManager(IDbHelper dbHelper, BaseUserInfo userInfo, string tableName) : this(dbHelper, userInfo)
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
        public string Add(BaseLoginLogEntity entity, bool identity = false, bool returnId = false)
        {
            Identity = identity;
            ReturnId = returnId;
            return AddEntity(entity);
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="entity">实体</param>
        public int Update(BaseLoginLogEntity entity)
        {
            return UpdateEntity(entity);
        }

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="id">主键</param>
        public BaseLoginLogEntity GetEntity(string id)
        {
            return BaseEntity.Create<BaseLoginLogEntity>(ExecuteReader(new KeyValuePair<string, object>(BaseLoginLogEntity.FieldId, id)));
            // return BaseEntity.Create<BaseLoginLogEntity>(this.GetDataTable(new KeyValuePair<string, object>(this.PrimaryKey, id)));
        }

        /// <summary>
        /// 添加实体
        /// </summary>
        /// <param name="entity">实体</param>
        public string AddEntity(BaseLoginLogEntity entity)
        {
            var result = string.Empty;

            /*
            if (string.IsNullOrEmpty(entity.Id))
            {
                entity.Id = Guid.NewGuid().ToString("N");
            }
            result = entity.Id.ToString();
            */

            var sqlBuilder = new SqlBuilder(DbHelper, Identity, ReturnId);
            sqlBuilder.BeginInsert(CurrentTableName, PrimaryKey);
            if (DbHelper.CurrentDbType == CurrentDbType.Oracle)
            {
                if (string.IsNullOrEmpty(entity.Id))
                {
                    sqlBuilder.SetFormula(PrimaryKey, "SEQ_BASE_LOGINLOG.NEXTVAL ");
                }
            }
            else if (string.IsNullOrEmpty(entity.Id))
            {
                //Troy.Cui改为自增主键20160923
                //entity.Id = Guid.NewGuid().ToString("N");
                result = entity.Id;
            }
            
            if (!string.IsNullOrEmpty(entity.Id)) 
            {
                // 这里已经是指定了主键了，所以不需要返回主键了
                sqlBuilder.ReturnId = false;
                sqlBuilder.SetValue(PrimaryKey, entity.Id);
            }
            /*
            else
            {
                if ((DbHelper.CurrentDbType == CurrentDbType.Oracle || DbHelper.CurrentDbType == CurrentDbType.DB2))
                {
                    if (DbHelper.CurrentDbType == CurrentDbType.Oracle)
                    {
                        sqlBuilder.SetFormula(this.PrimaryKey, "SEQ_BASE_LOGINLOG.NEXTVAL ");
                    }
                    if (DbHelper.CurrentDbType == CurrentDbType.DB2)
                    {
                        sqlBuilder.SetFormula(this.PrimaryKey, "NEXT VALUE FOR SEQ_" + this.CurrentTableName.ToUpper());
                    }
                }
                else
                {
                    entity.Id = Guid.NewGuid().ToString("N");
                    sqlBuilder.SetValue(this.PrimaryKey, entity.Id);
                }
            }
            */

            SetEntity(sqlBuilder, entity);
            sqlBuilder.SetDbNow(BaseLoginLogEntity.FieldCreateTime);
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
                result =  entity.Id;
            }

            return result;
        }

        /// <summary>
        /// 更新实体
        /// </summary>
        /// <param name="entity">实体</param>
        public int UpdateEntity(BaseLoginLogEntity entity)
        {
            var sqlBuilder = new SqlBuilder(DbHelper);
            sqlBuilder.BeginUpdate(CurrentTableName);
            SetEntity(sqlBuilder, entity);
            sqlBuilder.SetWhere(PrimaryKey, entity.Id);
            return sqlBuilder.EndUpdate();
        }

        // 这个是声明扩展方法
        partial void SetEntityExtend(SqlBuilder sqlBuilder, BaseLoginLogEntity entity);

        /// <summary>
        /// 设置实体
        /// </summary>
        /// <param name="sqlBuilder">sql生成器</param>
        /// <param name="entity">实体</param>
        private void SetEntity(SqlBuilder sqlBuilder, BaseLoginLogEntity entity)
        {
            sqlBuilder.SetValue(BaseLoginLogEntity.FieldSystemCode, entity.SystemCode);
            sqlBuilder.SetValue(BaseLoginLogEntity.FieldUserName, entity.UserName);
            sqlBuilder.SetValue(BaseLoginLogEntity.FieldNickName, entity.NickName);
            sqlBuilder.SetValue(BaseLoginLogEntity.FieldUserId, entity.UserId);
            sqlBuilder.SetValue(BaseLoginLogEntity.FieldRealName, entity.RealName);

            sqlBuilder.SetValue(BaseLoginLogEntity.FieldCompanyId, entity.CompanyId);
            sqlBuilder.SetValue(BaseLoginLogEntity.FieldCompanyName, entity.CompanyName);
            sqlBuilder.SetValue(BaseLoginLogEntity.FieldCompanyCode, entity.CompanyCode);

            sqlBuilder.SetValue(BaseLoginLogEntity.FieldTargetApplication, entity.TargetApplication);
            sqlBuilder.SetValue(BaseLoginLogEntity.FieldTargetIp, entity.TargetIp);

            sqlBuilder.SetValue(BaseLoginLogEntity.FieldSourceType, entity.SourceType);
            sqlBuilder.SetValue(BaseLoginLogEntity.FieldResult, entity.Result);

            sqlBuilder.SetValue(BaseLoginLogEntity.FieldProvince, entity.Province);
            sqlBuilder.SetValue(BaseLoginLogEntity.FieldCity, entity.City);

            sqlBuilder.SetValue(BaseLoginLogEntity.FieldOperationType, entity.OperationType);
            sqlBuilder.SetValue(BaseLoginLogEntity.FieldLoginStatus, entity.LoginStatus);
            sqlBuilder.SetValue(BaseLoginLogEntity.FieldMacAddress, entity.MacAddress);
            sqlBuilder.SetValue(BaseLoginLogEntity.FieldIpAddress, entity.IpAddress);
            sqlBuilder.SetValue(BaseLoginLogEntity.FieldIpAddressName, entity.IpAddressName);

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
