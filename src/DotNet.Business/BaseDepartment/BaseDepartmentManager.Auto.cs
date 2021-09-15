//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System.Collections.Generic;

namespace DotNet.Business
{
    using Model;
    using Util;

    /// <summary>
    /// BaseDepartmentManager
    /// 部门表
    ///
    /// 修改记录
    ///
    ///		2014-12-08 版本：1.0 JiRiGaLa 创建主键。
    ///
    /// <author>
    ///		<name>JiRiGaLa</name>
    ///		<date>2014-12-08</date>
    /// </author>
    /// </summary>
    public partial class BaseDepartmentManager : BaseManager, IBaseManager
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public BaseDepartmentManager()
        {
            if (dbHelper == null)
            {
                dbHelper = DbHelperFactory.GetHelper(BaseSystemInfo.UserCenterDbType, BaseSystemInfo.UserCenterDbConnection);
            }
            if (string.IsNullOrEmpty(CurrentTableName))
            {
                CurrentTableName = BaseDepartmentEntity.TableName;
            }
        }

        /// <summary>
        /// 构造函数
        /// <param name="tableName">指定表名</param>
        /// </summary>
        public BaseDepartmentManager(string tableName) : this()
        {
            CurrentTableName = tableName;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        public BaseDepartmentManager(IDbHelper dbHelper)
            : this()
        {
            DbHelper = dbHelper;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        public BaseDepartmentManager(BaseUserInfo userInfo)
            : this()
        {
            UserInfo = userInfo;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        /// <param name="userInfo">用户信息</param>
        public BaseDepartmentManager(IDbHelper dbHelper, BaseUserInfo userInfo)
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
        public BaseDepartmentManager(IDbHelper dbHelper, BaseUserInfo userInfo, string tableName)
            : this(dbHelper, userInfo)
        {
            CurrentTableName = tableName;
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns>主键</returns>
        public string Add(BaseDepartmentEntity entity)
        {
            return AddEntity(entity);
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="identity">自增量方式</param>
        /// <param name="returnId">返回主鍵</param>
        /// <returns>主键</returns>
        public string Add(BaseDepartmentEntity entity, bool identity, bool returnId)
        {
            Identity = identity;
            ReturnId = returnId;
            return AddEntity(entity);
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="entity">实体</param>
        public int Update(BaseDepartmentEntity entity)
        {
            return UpdateEntity(entity);
        }

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="id">主键</param>
        public BaseDepartmentEntity GetEntity(int? id)
        {
            return GetEntity(id.ToString());
        }

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="id">主键</param>
        public BaseDepartmentEntity GetEntity(string id)
        {
            return BaseEntity.Create<BaseDepartmentEntity>(ExecuteReader(new KeyValuePair<string, object>(BaseDepartmentEntity.FieldId, id)));
            // return BaseEntity.Create<BaseDepartmentEntity>(this.GetDataTable(new KeyValuePair<string, object>(BaseDepartmentEntity.FieldId, id)));
        }

        /// <summary>
        /// 添加实体
        /// </summary>
        /// <param name="entity">实体</param>
        public string AddEntity(BaseDepartmentEntity entity)
        {
            var sequence = string.Empty;
            if (!entity.SortCode.HasValue)
            {
                var managerSequence = new BaseSequenceManager(DbHelper, Identity);
                sequence = managerSequence.Increment(CurrentTableName);
                entity.SortCode = int.Parse(sequence);
            }
            var sqlBuilder = new SqlBuilder(DbHelper, Identity, ReturnId);
            sqlBuilder.BeginInsert(CurrentTableName, BaseDepartmentEntity.FieldId);
            if (entity.Id.HasValue || !Identity)
            {
                sqlBuilder.SetValue(BaseDepartmentEntity.FieldId, entity.Id);
            }
            else
            {
                if (!ReturnId && (DbHelper.CurrentDbType == CurrentDbType.Oracle || DbHelper.CurrentDbType == CurrentDbType.Db2))
                {
                    if (DbHelper.CurrentDbType == CurrentDbType.Oracle)
                    {
                        sqlBuilder.SetFormula(BaseDepartmentEntity.FieldId, "SEQ_" + BaseDepartmentEntity.TableName.ToUpper() + ".NEXTVAL ");
                    }
                    if (DbHelper.CurrentDbType == CurrentDbType.Db2)
                    {
                        sqlBuilder.SetFormula(BaseDepartmentEntity.FieldId, "NEXT VALUE FOR SEQ_" + BaseDepartmentEntity.TableName.ToUpper());
                    }
                }
                else
                {
                    if (Identity && (DbHelper.CurrentDbType == CurrentDbType.Oracle || DbHelper.CurrentDbType == CurrentDbType.Db2))
                    {
                        if (entity.Id == null)
                        {
                            if (string.IsNullOrEmpty(sequence))
                            {
                                var managerSequence = new BaseSequenceManager(DbHelper, Identity);
                                sequence = managerSequence.Increment(CurrentTableName);
                            }
                            entity.Id = int.Parse(sequence);
                        }
                        sqlBuilder.SetValue(BaseDepartmentEntity.FieldId, entity.Id);
                    }
                }
            }
            SetEntity(sqlBuilder, entity);

            // 创建人信息
            if (!string.IsNullOrEmpty(entity.CreateUserId))
            {
                sqlBuilder.SetValue(BaseDepartmentEntity.FieldCreateUserId, entity.CreateUserId);
            }
            else
            {
                if (UserInfo != null)
                {
                    sqlBuilder.SetValue(BaseDepartmentEntity.FieldCreateUserId, UserInfo.Id);
                }
            }
            if (!string.IsNullOrEmpty(entity.CreateBy))
            {
                sqlBuilder.SetValue(BaseDepartmentEntity.FieldCreateBy, entity.CreateBy);
            }
            else
            {
                if (UserInfo != null)
                {
                    sqlBuilder.SetValue(BaseDepartmentEntity.FieldCreateBy, UserInfo.RealName);
                }
            }
            if (entity.CreateOn.HasValue)
            {
                sqlBuilder.SetValue(BaseDepartmentEntity.FieldCreateTime, entity.CreateOn);
            }
            else
            {
                sqlBuilder.SetDbNow(BaseDepartmentEntity.FieldCreateTime);
            }

            // 修改人信息
            if (!string.IsNullOrEmpty(entity.ModifiedUserId))
            {
                sqlBuilder.SetValue(BaseDepartmentEntity.FieldUpdateUserId, entity.ModifiedUserId);
            }
            else
            {
                if (UserInfo != null)
                {
                    sqlBuilder.SetValue(BaseDepartmentEntity.FieldUpdateUserId, UserInfo.Id);
                }
            }
            if (!string.IsNullOrEmpty(entity.ModifiedBy))
            {
                sqlBuilder.SetValue(BaseDepartmentEntity.FieldUpdateBy, entity.ModifiedBy);
            }
            else
            {
                if (UserInfo != null)
                {
                    sqlBuilder.SetValue(BaseDepartmentEntity.FieldUpdateBy, UserInfo.RealName);
                }
            }
            if (entity.ModifiedOn.HasValue)
            {
                sqlBuilder.SetValue(BaseDepartmentEntity.FieldUpdateTime, entity.ModifiedOn);
            }
            else
            {
                sqlBuilder.SetDbNow(BaseDepartmentEntity.FieldUpdateTime);
            }

            if (DbHelper.CurrentDbType == CurrentDbType.SqlServer && Identity)
            {
                sequence = sqlBuilder.EndInsert().ToString();
            }
            else
            {
                sqlBuilder.EndInsert();
            }
            return sequence;
        }

        /// <summary>
        /// 更新实体
        /// </summary>
        /// <param name="entity">实体</param>
        public int UpdateEntity(BaseDepartmentEntity entity)
        {
            var sqlBuilder = new SqlBuilder(DbHelper);
            sqlBuilder.BeginUpdate(CurrentTableName);
            SetEntity(sqlBuilder, entity);
            if (UserInfo != null)
            {
                sqlBuilder.SetValue(BaseDepartmentEntity.FieldUpdateUserId, UserInfo.Id);
                sqlBuilder.SetValue(BaseDepartmentEntity.FieldUpdateBy, UserInfo.RealName);
            }
            // 若有修改时间标示，那就按修改时间来，不是按最新的时间来
            if (entity.ModifiedOn.HasValue)
            {
                sqlBuilder.SetValue(BaseDepartmentEntity.FieldUpdateTime, entity.ModifiedOn.Value);
            }
            else
            {
                sqlBuilder.SetDbNow(BaseDepartmentEntity.FieldUpdateTime);
            }
            sqlBuilder.SetWhere(BaseDepartmentEntity.FieldId, entity.Id);
            return sqlBuilder.EndUpdate();
        }

        partial void SetEntityExpand(SqlBuilder sqlBuilder, BaseDepartmentEntity entity);

        /// <summary>
        /// 设置实体
        /// </summary>
        /// <param name="sqlBuilder">SQL语句生成器</param>
        /// <param name="entity">实体</param>
        private void SetEntity(SqlBuilder sqlBuilder, BaseDepartmentEntity entity)
        {
            sqlBuilder.SetValue(BaseDepartmentEntity.FieldParentId, entity.ParentId);
            sqlBuilder.SetValue(BaseDepartmentEntity.FieldCode, entity.Code);
            sqlBuilder.SetValue(BaseDepartmentEntity.FieldShortName, entity.ShortName);
            sqlBuilder.SetValue(BaseDepartmentEntity.FieldFullName, entity.FullName);
            sqlBuilder.SetValue(BaseDepartmentEntity.FieldCategoryCode, entity.CategoryCode);
            sqlBuilder.SetValue(BaseDepartmentEntity.FieldOuterPhone, entity.OuterPhone);
            sqlBuilder.SetValue(BaseDepartmentEntity.FieldInnerPhone, entity.InnerPhone);
            sqlBuilder.SetValue(BaseDepartmentEntity.FieldFax, entity.Fax);
            sqlBuilder.SetValue(BaseDepartmentEntity.FieldDeleted, entity.DeletionStateCode);
            sqlBuilder.SetValue(BaseDepartmentEntity.FieldEnabled, entity.Enabled);
            sqlBuilder.SetValue(BaseDepartmentEntity.FieldSortCode, entity.SortCode);
            sqlBuilder.SetValue(BaseDepartmentEntity.FieldDescription, entity.Description);
            sqlBuilder.SetValue(BaseDepartmentEntity.FieldParentName, entity.ParentName);
            sqlBuilder.SetValue(BaseDepartmentEntity.FieldCompanyId, entity.CompanyId);
            sqlBuilder.SetValue(BaseDepartmentEntity.FieldCompanyName, entity.CompanyName);
            sqlBuilder.SetValue(BaseDepartmentEntity.FieldCompanyCode, entity.CompanyCode);
            sqlBuilder.SetValue(BaseDepartmentEntity.FieldParentCode, entity.ParentCode);
            sqlBuilder.SetValue(BaseDepartmentEntity.FieldCategoryName, entity.CategoryName);
            sqlBuilder.SetValue(BaseDepartmentEntity.FieldManagerId, entity.ManagerId);
            sqlBuilder.SetValue(BaseDepartmentEntity.FieldManager, entity.Manager);
            sqlBuilder.SetValue(BaseDepartmentEntity.FieldManagerQq, entity.ManagerQq);
            sqlBuilder.SetValue(BaseDepartmentEntity.FieldManagerMobile, entity.ManagerMobile);
            SetEntityExpand(sqlBuilder, entity);
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
