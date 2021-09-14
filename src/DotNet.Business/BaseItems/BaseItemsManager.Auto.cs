//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System.Collections.Generic;

namespace DotNet.Business
{
    using Model;
    using Util;

    /// <summary>
    /// BaseItemsManager
    /// 选项主表（资源）
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
    public partial class BaseItemsManager : BaseManager, IBaseManager
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public BaseItemsManager()
        {
            if (dbHelper == null)
            {
                dbHelper = DbHelperFactory.GetHelper(BaseSystemInfo.UserCenterDbType, BaseSystemInfo.UserCenterDbConnection);
            }
            if (string.IsNullOrEmpty(CurrentTableName))
            {
                CurrentTableName = BaseItemsEntity.TableName;
            }
        }

        /// <summary>
        /// 构造函数
        /// <param name="tableName">指定表名</param>
        /// </summary>
        public BaseItemsManager(string tableName)
        {
            CurrentTableName = tableName;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        public BaseItemsManager(IDbHelper dbHelper)
            : this()
        {
            DbHelper = dbHelper;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        public BaseItemsManager(BaseUserInfo userInfo)
            : this()
        {
            UserInfo = userInfo;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        /// <param name="userInfo">用户信息</param>
        public BaseItemsManager(IDbHelper dbHelper, BaseUserInfo userInfo)
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
        public BaseItemsManager(IDbHelper dbHelper, BaseUserInfo userInfo, string tableName)
            : this(dbHelper, userInfo)
        {
            CurrentTableName = tableName;
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns>主键</returns>
        public string Add(BaseItemsEntity entity)
        {
            return AddObject(entity);
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="identity">自增量方式</param>
        /// <param name="returnId">返回主鍵</param>
        /// <returns>主键</returns>
        public string Add(BaseItemsEntity entity, bool identity, bool returnId)
        {
            Identity = identity;
            ReturnId = returnId;
            return AddObject(entity);
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="entity">实体</param>
        public int Update(BaseItemsEntity entity)
        {
            return UpdateObject(entity);
        }

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="id">主键</param>
        public BaseItemsEntity GetObject(int id)
        {
            return GetObject(id.ToString());
        }

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="id">主键</param>
        public BaseItemsEntity GetObject(string id)
        {
            return BaseEntity.Create<BaseItemsEntity>(ExecuteReader(new KeyValuePair<string, object>(BaseItemsEntity.FieldId, id)));
            // return BaseEntity.Create<BaseItemsEntity>(this.GetDataTable(new KeyValuePair<string, object>(BaseItemsEntity.FieldId, id)));
        }

        /// <summary>
        /// 添加实体
        /// </summary>
        /// <param name="entity">实体</param>
        public string AddObject(BaseItemsEntity entity)
        {
            var result = string.Empty;

            if (entity.SortCode == 0)
            {
                var managerSequence = new BaseSequenceManager(DbHelper, Identity);
                result = managerSequence.Increment(CurrentTableName);
                entity.SortCode = int.Parse(result);
            }
            var sqlBuilder = new SqlBuilder(DbHelper, Identity, ReturnId);
            sqlBuilder.BeginInsert(CurrentTableName, BaseItemsEntity.FieldId);
            if (!Identity)
            {
                sqlBuilder.SetValue(BaseItemsEntity.FieldId, entity.Id);
            }
            else
            {
                if (!ReturnId && (DbHelper.CurrentDbType == CurrentDbType.Oracle || DbHelper.CurrentDbType == CurrentDbType.Db2))
                {
                    if (DbHelper.CurrentDbType == CurrentDbType.Oracle)
                    {
                        // // 2015-09-25 吉日嘎拉 用一个序列就可以了，不用那么多序列了
                        // sqlBuilder.SetFormula(BaseItemsEntity.FieldId, "SEQ_" + this.CurrentTableName.ToUpper() + ".NEXTVAL ");
                        sqlBuilder.SetFormula(BaseItemsEntity.FieldId, "SEQ_" + BaseItemsEntity.TableName.ToUpper() + ".NEXTVAL ");
                    }
                    if (DbHelper.CurrentDbType == CurrentDbType.Db2)
                    {
                        // sqlBuilder.SetFormula(BaseItemsEntity.FieldId, "NEXT VALUE FOR SEQ_" + this.CurrentTableName.ToUpper());
                        sqlBuilder.SetFormula(BaseItemsEntity.FieldId, "NEXT VALUE FOR SEQ_" + BaseItemsEntity.TableName.ToUpper());
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
                        sqlBuilder.SetValue(BaseItemsEntity.FieldId, entity.Id);
                    }
                }
            }
            SetObject(sqlBuilder, entity);
            if (UserInfo != null)
            {
                sqlBuilder.SetValue(BaseItemsEntity.FieldCreateUserId, UserInfo.Id);
                sqlBuilder.SetValue(BaseItemsEntity.FieldCreateBy, UserInfo.RealName);
            }
            sqlBuilder.SetDbNow(BaseItemsEntity.FieldCreateTime);
            if (UserInfo != null)
            {
                sqlBuilder.SetValue(BaseItemsEntity.FieldUpdateUserId, UserInfo.Id);
                sqlBuilder.SetValue(BaseItemsEntity.FieldUpdateBy, UserInfo.RealName);
            }
            sqlBuilder.SetDbNow(BaseItemsEntity.FieldUpdateTime);
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
        public int UpdateObject(BaseItemsEntity entity)
        {
            var sqlBuilder = new SqlBuilder(DbHelper);
            sqlBuilder.BeginUpdate(CurrentTableName);
            SetObject(sqlBuilder, entity);
            if (UserInfo != null)
            {
                sqlBuilder.SetValue(BaseItemsEntity.FieldUpdateUserId, UserInfo.Id);
                sqlBuilder.SetValue(BaseItemsEntity.FieldUpdateBy, UserInfo.RealName);
            }
            sqlBuilder.SetDbNow(BaseItemsEntity.FieldUpdateTime);
            sqlBuilder.SetWhere(BaseItemsEntity.FieldId, entity.Id);
            //return sqlBuilder.EndUpdate();
            var result = sqlBuilder.EndUpdate();
            if (result > 0)
            {
                RemoveCache();
            }
            return result;
        }

        partial void SetObjectExpand(SqlBuilder sqlBuilder, BaseItemsEntity entity);

        /// <summary>
        /// 设置实体
        /// </summary>
        /// <param name="sqlBuilder">SQL语句生成器</param>
        /// <param name="entity">实体</param>
        private void SetObject(SqlBuilder sqlBuilder, BaseItemsEntity entity)
        {
            sqlBuilder.SetValue(BaseItemsEntity.FieldParentId, entity.ParentId);
            sqlBuilder.SetValue(BaseItemsEntity.FieldCode, entity.Code);
            sqlBuilder.SetValue(BaseItemsEntity.FieldFullName, entity.FullName);
            sqlBuilder.SetValue(BaseItemsEntity.FieldTargetTable, entity.TargetTable);
            sqlBuilder.SetValue(BaseItemsEntity.FieldIsTree, entity.IsTree);
            sqlBuilder.SetValue(BaseItemsEntity.FieldAllowEdit, entity.AllowEdit);
            sqlBuilder.SetValue(BaseItemsEntity.FieldAllowDelete, entity.AllowDelete);
            sqlBuilder.SetValue(BaseItemsEntity.FieldDeleted, entity.DeletionStateCode);
            sqlBuilder.SetValue(BaseItemsEntity.FieldDescription, entity.Description);
            sqlBuilder.SetValue(BaseItemsEntity.FieldEnabled, entity.Enabled);
            sqlBuilder.SetValue(BaseItemsEntity.FieldSortCode, entity.SortCode);
            SetObjectExpand(sqlBuilder, entity);
        }

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns>影响行数</returns>
        public int Delete(int id)
        {
            var result = Delete(new List<KeyValuePair<string, object>> { new KeyValuePair<string, object>(BaseItemsEntity.FieldId, id) });
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
			result.Add(new KeyValuePair<string, object>(BaseItemsEntity.FieldAllowDelete, 1));
			return result;
		}
    }
}