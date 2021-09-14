//-----------------------------------------------------------------------
// <copyright file="BaseUserIdentityAuditManager.Auto.cs" company="DotNet">
//     Copyright (C) 2021, All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;

namespace DotNet.Business
{
    using Model;
    using Util;

    /// <summary>
    /// BaseUserIdentityAuditManager
    /// 用户身份需要人工审核的：目前有身份证的审核，淘宝每日请求次数要求加入到此表中，后期用户其它身份的人工审核也加入到这里，如银行卡等
    /// 
    /// 修改记录
    /// 
    /// 2015-02-10 版本：1.0 SongBiao 创建文件。
    /// 
    /// <author>
    ///     <name>SongBiao</name>
    ///     <date>2015-02-10</date>
    /// </author>
    /// </summary>
    public partial class BaseUserIdentityAuditManager : BaseManager, IBaseManager
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public BaseUserIdentityAuditManager()
        {
            if (dbHelper == null)
            {
                dbHelper = DbHelperFactory.GetHelper(BaseSystemInfo.UserCenterDbType, BaseSystemInfo.UserCenterDbConnection);
            }
            if (string.IsNullOrEmpty(CurrentTableName))
            {
                CurrentTableName = BaseUserIdentityAuditEntity.TableName;
            }
            PrimaryKey = "Id";
        }

        /// <summary>
        /// 构造函数
        /// <param name="tableName">指定表名</param>
        /// </summary>
        public BaseUserIdentityAuditManager(string tableName)
        {
            CurrentTableName = tableName;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        public BaseUserIdentityAuditManager(IDbHelper dbHelper)
            : this()
        {
            DbHelper = dbHelper;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        public BaseUserIdentityAuditManager(BaseUserInfo userInfo)
            : this()
        {
            UserInfo = userInfo;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        /// <param name="tableName">指定表名</param>
        public BaseUserIdentityAuditManager(BaseUserInfo userInfo, string tableName)
            : this(userInfo)
        {
            CurrentTableName = tableName;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        /// <param name="userInfo">用户信息</param>
        public BaseUserIdentityAuditManager(IDbHelper dbHelper, BaseUserInfo userInfo)
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
        public BaseUserIdentityAuditManager(IDbHelper dbHelper, BaseUserInfo userInfo, string tableName)
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
        public string Add(BaseUserIdentityAuditEntity entity, bool identity = true, bool returnId = true)
        {
            Identity = identity;
            ReturnId = returnId;
            //entity.Id = int.Parse(this.AddObject(entity));
            //return entity.Id.ToString();
            //关联的ID 非自增的 
            return AddObject(entity);
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="entity">实体</param>
        public int Update(BaseUserIdentityAuditEntity entity)
        {
            return UpdateObject(entity);
        }

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="id">主键</param>
        public BaseUserIdentityAuditEntity GetObject(string id)
        {
            return GetObject(int.Parse(id));
        }
        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public BaseUserIdentityAuditEntity GetObject(int id)
        {
            return BaseEntity.Create<BaseUserIdentityAuditEntity>(ExecuteReader(new KeyValuePair<string, object>(BaseUserIdentityAuditEntity.FieldId, id)));
            // return BaseEntity.Create<BaseUserIdentityAuditEntity>(this.GetDataTable(new KeyValuePair<string, object>(this.PrimaryKey, id)));
        }

        /// <summary>
        /// 添加实体
        /// </summary>
        /// <param name="entity">实体</param>
        public string AddObject(BaseUserIdentityAuditEntity entity)
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
                        var managerSequence = new BaseSequenceManager(DbHelper);
                        entity.Id = int.Parse(managerSequence.Increment(CurrentTableName));
                        sqlBuilder.SetValue(PrimaryKey, entity.Id);
                    }
                }
            }
            SetObject(sqlBuilder, entity);
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
            //非主键 关联外部ID
            if (ReturnId)
            {
                return entity.Id.ToString();
            }
            return key;
        }

        /// <summary>
        /// 更新实体
        /// </summary>
        /// <param name="entity">实体</param>
        public int UpdateObject(BaseUserIdentityAuditEntity entity)
        {
            var sqlBuilder = new SqlBuilder(DbHelper);
            sqlBuilder.BeginUpdate(CurrentTableName);
            SetObject(sqlBuilder, entity);
            sqlBuilder.SetWhere(PrimaryKey, entity.Id);
            return sqlBuilder.EndUpdate();
        }

        // 这个是声明扩展方法
        partial void SetObjectExpand(SqlBuilder sqlBuilder, BaseUserIdentityAuditEntity entity);

        /// <summary>
        /// 设置实体
        /// </summary>
        /// <param name="sqlBuilder">SQL语句生成器</param>
        /// <param name="entity">实体</param>
        private void SetObject(SqlBuilder sqlBuilder, BaseUserIdentityAuditEntity entity)
        {
            SetObjectExpand(sqlBuilder, entity);
            sqlBuilder.SetValue(BaseUserIdentityAuditEntity.FieldUpdateTime, entity.ModifiedOn);
            sqlBuilder.SetValue(BaseUserIdentityAuditEntity.FieldIdcardPhotoHand, entity.IdcardPhotoHand);
            sqlBuilder.SetValue(BaseUserIdentityAuditEntity.FieldOrganizeFullname, entity.OrganizeFullname);
            sqlBuilder.SetValue(BaseUserIdentityAuditEntity.FieldCreateTime, entity.CreateOn);
            sqlBuilder.SetValue(BaseUserIdentityAuditEntity.FieldAuditDate, entity.AuditDate);
            sqlBuilder.SetValue(BaseUserIdentityAuditEntity.FieldAuditStatus, entity.AuditStatus);
            sqlBuilder.SetValue(BaseUserIdentityAuditEntity.FieldInterfaceDayLimit, entity.InterfaceDayLimit);
            sqlBuilder.SetValue(BaseUserIdentityAuditEntity.FieldNickName, entity.NickName);
            sqlBuilder.SetValue(BaseUserIdentityAuditEntity.FieldUserRealName, entity.UserRealName);
            sqlBuilder.SetValue(BaseUserIdentityAuditEntity.FieldIdcard, entity.Idcard);
            sqlBuilder.SetValue(BaseUserIdentityAuditEntity.FieldAuditUserId, entity.AuditUserId);
            sqlBuilder.SetValue(BaseUserIdentityAuditEntity.FieldAuditIdea, entity.AuditIdea);
            sqlBuilder.SetValue(BaseUserIdentityAuditEntity.FieldAuditUserRealName, entity.AuditUserRealName);
            sqlBuilder.SetValue(BaseUserIdentityAuditEntity.FieldOrganizeId, entity.OrganizeId);
            sqlBuilder.SetValue(BaseUserIdentityAuditEntity.FieldAuditUserNickName, entity.AuditUserNickName);
            sqlBuilder.SetValue(BaseUserIdentityAuditEntity.FieldIdcardPhotoFront, entity.IdcardPhotoFront);
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