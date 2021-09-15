//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System.Collections.Generic;

namespace DotNet.Business
{
    using Model;
    using Util;

    /// <summary>
    /// BaseItemDetailsManager
    /// 选项明细表（资源明细表结构）
    ///
    /// 修改记录
    ///
    ///		2010-07-28 版本：1.0 JiRiGaLa 创建主键。
    ///
    /// <author>
    ///		<name>JiRiGaLa</name>
    ///		<date>2010-07-28</date>
    /// </author>
    /// </summary>
    public partial class BaseItemDetailsManager : BaseManager, IBaseManager
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public BaseItemDetailsManager()
        {
            if (dbHelper == null)
            {
                dbHelper = DbHelperFactory.GetHelper(BaseSystemInfo.UserCenterDbType, BaseSystemInfo.UserCenterDbConnection);
            }
            if (string.IsNullOrEmpty(CurrentTableName))
            {
                CurrentTableName = BaseItemDetailsEntity.TableName;
            }
        }

        /// <summary>
        /// 构造函数
        /// <param name="tableName">指定表名</param>
        /// </summary>
        public BaseItemDetailsManager(string tableName)
            :this()
        {
            CurrentTableName = tableName;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        public BaseItemDetailsManager(IDbHelper dbHelper)
            : this()
        {
            DbHelper = dbHelper;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        public BaseItemDetailsManager(BaseUserInfo userInfo)
            : this()
        {
            UserInfo = userInfo;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        /// <param name="tableName">指定表名</param>
        public BaseItemDetailsManager(BaseUserInfo userInfo, string tableName)
            : this(userInfo)
        {
            CurrentTableName = tableName;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        /// <param name="userInfo">用户信息</param>
        public BaseItemDetailsManager(IDbHelper dbHelper, BaseUserInfo userInfo)
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
        public BaseItemDetailsManager(IDbHelper dbHelper, BaseUserInfo userInfo, string tableName)
            : this(dbHelper, userInfo)
        {
            CurrentTableName = tableName;
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns>主键</returns>
        public string Add(BaseItemDetailsEntity entity)
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
        public string Add(BaseItemDetailsEntity entity, bool identity, bool returnId)
        {
            Identity = identity;
            ReturnId = returnId;
            return AddEntity(entity);
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="entity">实体</param>
        public int Update(BaseItemDetailsEntity entity)
        {
            return UpdateEntity(entity);
        }

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="id">主键</param>
        public BaseItemDetailsEntity GetEntity(int id)
        {
            return GetEntity(id.ToString());
        }

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="id">主键</param>
        public BaseItemDetailsEntity GetEntity(string id)
        {
            return BaseEntity.Create<BaseItemDetailsEntity>(ExecuteReader(new KeyValuePair<string, object>(BaseItemDetailsEntity.FieldId, id)));
            // return BaseEntity.Create<BaseItemDetailsEntity>(this.GetDataTable(new KeyValuePair<string, object>(BaseItemDetailsEntity.FieldId, id)));
        }

        /// <summary>
        /// 添加实体
        /// </summary>
        /// <param name="entity">实体</param>
        public string AddEntity(BaseItemDetailsEntity entity)
        {
            var result = string.Empty;
            if (!entity.SortCode.HasValue)
            {
                var managerSequence = new BaseSequenceManager(DbHelper, Identity);
                result = managerSequence.Increment(CurrentTableName);
                entity.SortCode = int.Parse(result);
            }
            var sqlBuilder = new SqlBuilder(DbHelper, Identity, ReturnId);
            sqlBuilder.BeginInsert(CurrentTableName, BaseItemDetailsEntity.FieldId);
            if (!Identity || (entity.Id.HasValue && entity.Id != 0))
            {
                sqlBuilder.SetValue(BaseItemDetailsEntity.FieldId, entity.Id);
                result = entity.Id.ToString();
            }
            else
            {
                if (!ReturnId && (DbHelper.CurrentDbType == CurrentDbType.Oracle || DbHelper.CurrentDbType == CurrentDbType.Db2))
                {
                    if (DbHelper.CurrentDbType == CurrentDbType.Oracle)
                    {
                        sqlBuilder.SetFormula(BaseItemDetailsEntity.FieldId, "SEQ_" + CurrentTableName.ToUpper() + ".NEXTVAL ");
                    }
                    if (DbHelper.CurrentDbType == CurrentDbType.Db2)
                    {
                        sqlBuilder.SetFormula(BaseItemDetailsEntity.FieldId, "NEXT VALUE FOR SEQ_" + CurrentTableName.ToUpper());
                    }
                }
                else
                {
                    if (Identity && (DbHelper.CurrentDbType == CurrentDbType.Oracle || DbHelper.CurrentDbType == CurrentDbType.Db2))
                    {
                        if (string.IsNullOrEmpty(result))
                        {
                            var managerSequence = new BaseSequenceManager(DbHelper, Identity);
                            result = managerSequence.Increment(CurrentTableName);
                        }
                        entity.Id = int.Parse(result);
                        sqlBuilder.SetValue(BaseItemDetailsEntity.FieldId, entity.Id);
                    }
                }
            }
            SetEntity(sqlBuilder, entity);

            // 创建人信息
            if (!string.IsNullOrEmpty(entity.CreateUserId))
            {
                sqlBuilder.SetValue(BaseItemDetailsEntity.FieldCreateUserId, entity.CreateUserId);
            }
            else
            {
                if (UserInfo != null)
                {
                    sqlBuilder.SetValue(BaseItemDetailsEntity.FieldCreateUserId, UserInfo.Id);
                }
            }
            if (!string.IsNullOrEmpty(entity.CreateBy))
            {
                sqlBuilder.SetValue(BaseItemDetailsEntity.FieldCreateBy, entity.CreateBy);
            }
            else
            {
                if (UserInfo != null)
                {
                    sqlBuilder.SetValue(BaseItemDetailsEntity.FieldCreateBy, UserInfo.RealName);
                }
            }
            if (entity.CreateOn.HasValue)
            {
                sqlBuilder.SetValue(BaseItemDetailsEntity.FieldCreateTime, entity.CreateOn);
            }
            else
            {
                sqlBuilder.SetDbNow(BaseItemDetailsEntity.FieldCreateTime);
            }

            // 修改人信息
            if (!string.IsNullOrEmpty(entity.ModifiedUserId))
            {
                sqlBuilder.SetValue(BaseItemDetailsEntity.FieldUpdateUserId, entity.ModifiedUserId);
            }
            else
            {
                if (UserInfo != null)
                {
                    sqlBuilder.SetValue(BaseItemDetailsEntity.FieldUpdateUserId, UserInfo.Id);
                }
            }
            if (!string.IsNullOrEmpty(entity.ModifiedBy))
            {
                sqlBuilder.SetValue(BaseItemDetailsEntity.FieldUpdateBy, entity.ModifiedBy);
            }
            else
            {
                if (UserInfo != null)
                {
                    sqlBuilder.SetValue(BaseItemDetailsEntity.FieldUpdateBy, UserInfo.RealName);
                }
            }
            if (entity.ModifiedOn.HasValue)
            {
                sqlBuilder.SetValue(BaseItemDetailsEntity.FieldUpdateTime, entity.ModifiedOn);
            }
            else
            {
                sqlBuilder.SetDbNow(BaseItemDetailsEntity.FieldUpdateTime);
            }

            if (DbHelper.CurrentDbType == CurrentDbType.SqlServer && Identity)
            {
                result = sqlBuilder.EndInsert().ToString();
            }
            else
            {
                sqlBuilder.EndInsert();
            }
            if (!string.IsNullOrWhiteSpace(result))
            {
                RemoveCache();
            }
            return result;
        }

        /// <summary>
        /// 更新实体
        /// </summary>
        /// <param name="entity">实体</param>
        public int UpdateEntity(BaseItemDetailsEntity entity)
        {
            var sqlBuilder = new SqlBuilder(DbHelper);
            sqlBuilder.BeginUpdate(CurrentTableName);
            SetEntity(sqlBuilder, entity);
            if (UserInfo != null)
            {
                sqlBuilder.SetValue(BaseItemDetailsEntity.FieldUpdateUserId, UserInfo.Id);
                sqlBuilder.SetValue(BaseItemDetailsEntity.FieldUpdateBy, UserInfo.RealName);
            }
            sqlBuilder.SetDbNow(BaseItemDetailsEntity.FieldUpdateTime);
            sqlBuilder.SetWhere(BaseItemDetailsEntity.FieldId, entity.Id);
            //return sqlBuilder.EndUpdate();
            var result = sqlBuilder.EndUpdate();
            if (result > 0)
            {
                RemoveCache();
            }
            return result;
        }

        partial void SetEntityExtend(SqlBuilder sqlBuilder, BaseItemDetailsEntity entity);

        /// <summary>
        /// 设置实体
        /// </summary>
        /// <param name="sqlBuilder">SQL语句生成器</param>
        /// <param name="entity">实体</param>
        private void SetEntity(SqlBuilder sqlBuilder, BaseItemDetailsEntity entity)
        {
            sqlBuilder.SetValue(BaseItemDetailsEntity.FieldParentId, entity.ParentId);
            sqlBuilder.SetValue(BaseItemDetailsEntity.FieldItemCode, entity.ItemCode);
            sqlBuilder.SetValue(BaseItemDetailsEntity.FieldItemName, entity.ItemName);
            sqlBuilder.SetValue(BaseItemDetailsEntity.FieldItemValue, entity.ItemValue);
            sqlBuilder.SetValue(BaseItemDetailsEntity.FieldAllowEdit, entity.AllowEdit);
            sqlBuilder.SetValue(BaseItemDetailsEntity.FieldAllowDelete, entity.AllowDelete);
            sqlBuilder.SetValue(BaseItemDetailsEntity.FieldIsPublic, entity.IsPublic);
            sqlBuilder.SetValue(BaseItemDetailsEntity.FieldEnabled, entity.Enabled);
            sqlBuilder.SetValue(BaseItemDetailsEntity.FieldDeleted, entity.DeletionStateCode);
            sqlBuilder.SetValue(BaseItemDetailsEntity.FieldSortCode, entity.SortCode);
            sqlBuilder.SetValue(BaseItemDetailsEntity.FieldDescription, entity.Description);
            SetEntityExtend(sqlBuilder, entity);
        }

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns>影响行数</returns>
        public int Delete(int id)
        {
            var result = Delete(new List<KeyValuePair<string, object>> { new KeyValuePair<string, object>(BaseItemDetailsEntity.FieldId, id) });
            if (result > 0)
            {
                RemoveCache();
            }
            return result;
        }
        /// <summary>
        /// 添加删除的附加条件
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
		protected override List<KeyValuePair<string, object>> GetDeleteExtParam(List<KeyValuePair<string, object>> parameters)
		{
			var result = base.GetDeleteExtParam(parameters);
			result.Add(new KeyValuePair<string, object>(BaseItemDetailsEntity.FieldAllowDelete, 1));
			return result;
		}
    }
}
