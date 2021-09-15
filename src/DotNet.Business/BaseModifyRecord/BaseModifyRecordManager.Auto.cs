//-----------------------------------------------------------------------
// <copyright file="BaseModifyRecordManager.Auto.cs" company="DotNet">
//     Copyright (C) 2021, All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;

namespace DotNet.Business
{
    using Model;
    using Util;

    /// <summary>
    /// BaseModifyRecordManager
    /// 修改记录表
    /// 
    /// 修改记录
    /// 
    /// 2015-12-16 版本：2.1 JiRiGaLa 返回值处理。
    /// 2015-12-14 版本：2.0 JiRiGaLa 提高MYSQL的兼容性。
    /// 2013-12-16 版本：1.0 JiRiGaLa 创建文件。
    /// 
    /// <author>
    ///     <name>JiRiGaLa</name>
    ///     <date>2015-12-16</date>
    /// </author>
    /// </summary>
    public partial class BaseModifyRecordManager : BaseManager, IBaseManager
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public BaseModifyRecordManager()
        {
            if (dbHelper == null)
            {
                dbHelper = DbHelperFactory.GetHelper(BaseSystemInfo.UserCenterDbType, BaseSystemInfo.UserCenterDbConnection);
            }
            if (string.IsNullOrEmpty(CurrentTableName))
            {
                CurrentTableName = BaseModifyRecordEntity.TableName;
            }
            PrimaryKey = "Id";
        }

        /// <summary>
        /// 构造函数
        /// <param name="tableName">指定表名</param>
        /// </summary>
        public BaseModifyRecordManager(string tableName)
        {
            CurrentTableName = tableName;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        public BaseModifyRecordManager(IDbHelper dbHelper)
            : this()
        {
            DbHelper = dbHelper;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        public BaseModifyRecordManager(BaseUserInfo userInfo)
            : this()
        {
            UserInfo = userInfo;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        /// <param name="tableName">指定表名</param>
        public BaseModifyRecordManager(BaseUserInfo userInfo, string tableName)
            : this(userInfo)
        {
            CurrentTableName = tableName;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        /// <param name="userInfo">用户信息</param>
        public BaseModifyRecordManager(IDbHelper dbHelper, BaseUserInfo userInfo)
            : this(dbHelper)
        {
            UserInfo = userInfo;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        /// <param name="userInfo">用户信息</param>
        /// <param name="tableName">指定表名</param>
        public BaseModifyRecordManager(IDbHelper dbHelper, BaseUserInfo userInfo, string tableName)
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
        public string Add(BaseModifyRecordEntity entity, bool identity = true, bool returnId = true)
        {
            var result = string.Empty;
            Identity = identity;
            ReturnId = returnId;
            result = AddEntity(entity);

            if (!string.IsNullOrEmpty(result))
            {
                entity.Id = int.Parse(result);
            }

            return entity.Id.ToString();
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="entity">实体</param>
        public int Update(BaseModifyRecordEntity entity)
        {
            return UpdateEntity(entity);
        }

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="id">主键</param>
        public BaseModifyRecordEntity GetEntity(string id)
        {
            return GetEntity(int.Parse(id));
        }

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="id">主键</param>
        public BaseModifyRecordEntity GetEntity(int id)
        {
            return BaseEntity.Create<BaseModifyRecordEntity>(ExecuteReader(new KeyValuePair<string, object>(BaseModifyRecordEntity.FieldId, id)));
            // return BaseEntity.Create<BaseModifyRecordEntity>(this.GetDataTable(new KeyValuePair<string, object>(this.PrimaryKey, id)));
        }

        /// <summary>
        /// 添加实体
        /// </summary>
        /// <param name="entity">实体</param>
        public string AddEntity(BaseModifyRecordEntity entity)
        {
            var key = string.Empty;
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
                        var managerSequence = new BaseSequenceManager();
                        entity.Id = int.Parse(managerSequence.Increment(CurrentTableName));
                        sqlBuilder.SetValue(PrimaryKey, entity.Id);
                    }
                }
            }
            SetEntity(sqlBuilder, entity);
            if (UserInfo != null)
            {
                sqlBuilder.SetValue(BaseModifyRecordEntity.FieldCreateUserId, UserInfo.Id);
                sqlBuilder.SetValue(BaseModifyRecordEntity.FieldCreateBy, UserInfo.RealName);
            }
            sqlBuilder.SetDbNow(BaseModifyRecordEntity.FieldCreateTime);
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
                return entity.Id.ToString();
            }
            return key;
        }

        /// <summary>
        /// 更新实体
        /// </summary>
        /// <param name="entity">实体</param>
        public int UpdateEntity(BaseModifyRecordEntity entity)
        {
            var sqlBuilder = new SqlBuilder(DbHelper);
            sqlBuilder.BeginUpdate(CurrentTableName);
            SetEntity(sqlBuilder, entity);
            sqlBuilder.SetWhere(PrimaryKey, entity.Id);
            return sqlBuilder.EndUpdate();
        }

        // 这个是声明扩展方法
        partial void SetEntityExtend(SqlBuilder sqlBuilder, BaseModifyRecordEntity entity);

        /// <summary>
        /// 设置实体
        /// </summary>
        /// <param name="sqlBuilder">SQL语句生成器</param>
        /// <param name="entity">实体</param>
        private void SetEntity(SqlBuilder sqlBuilder, BaseModifyRecordEntity entity)
        {
            SetEntityExtend(sqlBuilder, entity);
            sqlBuilder.SetValue(BaseModifyRecordEntity.FieldTableCode, entity.TableCode);
            sqlBuilder.SetValue(BaseModifyRecordEntity.FieldTableDescription, entity.TableDescription);
            sqlBuilder.SetValue(BaseModifyRecordEntity.FieldColumnCode, entity.ColumnCode);
            sqlBuilder.SetValue(BaseModifyRecordEntity.FieldColumnDescription, entity.ColumnDescription);
            sqlBuilder.SetValue(BaseModifyRecordEntity.FieldRecordKey, entity.RecordKey);
            sqlBuilder.SetValue(BaseModifyRecordEntity.FieldOldKey, entity.OldKey);
            sqlBuilder.SetValue(BaseModifyRecordEntity.FieldOldValue, entity.OldValue);
            sqlBuilder.SetValue(BaseModifyRecordEntity.FieldNewKey, entity.NewKey);
            sqlBuilder.SetValue(BaseModifyRecordEntity.FieldNewValue, entity.NewValue);
            sqlBuilder.SetValue(BaseModifyRecordEntity.FieldIpAddress, entity.IpAddress);
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
