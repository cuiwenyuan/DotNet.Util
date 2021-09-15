//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System.Collections.Generic;

namespace DotNet.Business
{
    using Model;
    using Util;

    /// <summary>
    /// BaseTableColumnsManager
    /// 表字段结构定义说明
    ///
    /// 修改记录
    ///
    ///		2010-07-15 版本：1.0 JiRiGaLa 创建主键。
    ///
    /// <author>
    ///		<name>JiRiGaLa</name>
    ///		<date>2010-07-15</date>
    /// </author>
    /// </summary>
    public partial class BaseTableColumnsManager : BaseManager, IBaseManager
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public BaseTableColumnsManager()
        {
            if (dbHelper == null)
            {
                dbHelper = DbHelperFactory.GetHelper(BaseSystemInfo.UserCenterDbType, BaseSystemInfo.UserCenterDbConnection);
            }
            CurrentTableName = BaseTableColumnsEntity.TableName;
        }

        /// <summary>
        /// 构造函数
        /// <param name="tableName">指定表名</param>
        /// </summary>
        public BaseTableColumnsManager(string tableName)
        {
            CurrentTableName = tableName;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        public BaseTableColumnsManager(IDbHelper dbHelper)
            : this()
        {
            DbHelper = dbHelper;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        public BaseTableColumnsManager(BaseUserInfo userInfo)
            : this()
        {
            UserInfo = userInfo;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        /// <param name="userInfo">用户信息</param>
        public BaseTableColumnsManager(IDbHelper dbHelper, BaseUserInfo userInfo)
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
        public BaseTableColumnsManager(IDbHelper dbHelper, BaseUserInfo userInfo, string tableName)
            : this(dbHelper, userInfo)
        {
            CurrentTableName = tableName;
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns>主键</returns>
        public string Add(BaseTableColumnsEntity entity)
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
        public string Add(BaseTableColumnsEntity entity, bool identity, bool returnId)
        {
            Identity = identity;
            ReturnId = returnId;
            return AddEntity(entity);
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="entity">实体</param>
        public int Update(BaseTableColumnsEntity entity)
        {
            return UpdateEntity(entity);
        }

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="id">主键</param>
        public BaseTableColumnsEntity GetEntity(int id)
        {
            return BaseEntity.Create<BaseTableColumnsEntity>(ExecuteReader(new KeyValuePair<string, object>(BaseTableColumnsEntity.FieldId, id)));
            // return BaseEntity.Create<BaseTableColumnsEntity>(this.GetDataTable(new KeyValuePair<string, object>(BaseTableColumnsEntity.FieldId, id)));
        }

        /// <summary>
        /// 添加实体
        /// </summary>
        /// <param name="entity">实体</param>
        public string AddEntity(BaseTableColumnsEntity entity)
        {
            var sequence = string.Empty;
            if (entity.SortCode == 0)
            {
                var managerSequence = new BaseSequenceManager(DbHelper, Identity);
                sequence = managerSequence.Increment(CurrentTableName);
                entity.SortCode = int.Parse(sequence);
            }
            var sqlBuilder = new SqlBuilder(DbHelper, Identity, ReturnId);
            sqlBuilder.BeginInsert(CurrentTableName, BaseTableColumnsEntity.FieldId);
            if (!Identity)
            {
                sqlBuilder.SetValue(BaseTableColumnsEntity.FieldId, entity.Id);
            }
            else
            {
                if (!ReturnId && (DbHelper.CurrentDbType == CurrentDbType.Oracle || DbHelper.CurrentDbType == CurrentDbType.Db2))
                {
                    if (DbHelper.CurrentDbType == CurrentDbType.Oracle)
                    {
                        sqlBuilder.SetFormula(BaseTableColumnsEntity.FieldId, "SEQ_" + CurrentTableName.ToUpper() + ".NEXTVAL ");
                    }
                    if (DbHelper.CurrentDbType == CurrentDbType.Db2)
                    {
                        sqlBuilder.SetFormula(BaseTableColumnsEntity.FieldId, "NEXT VALUE FOR SEQ_" + CurrentTableName.ToUpper());
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
                        sqlBuilder.SetValue(BaseTableColumnsEntity.FieldId, entity.Id);
                    }
                }
            }
            SetEntity(sqlBuilder, entity);
            if (UserInfo != null)
            {
                sqlBuilder.SetValue(BaseTableColumnsEntity.FieldCreateUserId, UserInfo.Id);
                sqlBuilder.SetValue(BaseTableColumnsEntity.FieldCreateBy, UserInfo.RealName);
            }
            sqlBuilder.SetDbNow(BaseTableColumnsEntity.FieldCreateTime);
            if (UserInfo != null)
            {
                sqlBuilder.SetValue(BaseTableColumnsEntity.FieldUpdateUserId, UserInfo.Id);
                sqlBuilder.SetValue(BaseTableColumnsEntity.FieldUpdateBy, UserInfo.RealName);
            }
            sqlBuilder.SetDbNow(BaseTableColumnsEntity.FieldUpdateTime);
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
        public int UpdateEntity(BaseTableColumnsEntity entity)
        {
            var sqlBuilder = new SqlBuilder(DbHelper);
            sqlBuilder.BeginUpdate(CurrentTableName);
            SetEntity(sqlBuilder, entity);
            if (UserInfo != null)
            {
                sqlBuilder.SetValue(BaseTableColumnsEntity.FieldUpdateUserId, UserInfo.Id);
                sqlBuilder.SetValue(BaseTableColumnsEntity.FieldUpdateBy, UserInfo.RealName);
            }
            sqlBuilder.SetDbNow(BaseTableColumnsEntity.FieldUpdateTime);
            sqlBuilder.SetWhere(BaseTableColumnsEntity.FieldId, entity.Id);
            return sqlBuilder.EndUpdate();
        }

        partial void SetEntityExtend(SqlBuilder sqlBuilder, BaseTableColumnsEntity entity);

        /// <summary>
        /// 设置实体
        /// </summary>
        /// <param name="sqlBuilder">SQL语句生成器</param>
        /// <param name="entity">实体</param>
        private void SetEntity(SqlBuilder sqlBuilder, BaseTableColumnsEntity entity)
        {
            sqlBuilder.SetValue(BaseTableColumnsEntity.FieldTableCode, entity.TableCode);
            sqlBuilder.SetValue(BaseTableColumnsEntity.FieldColumnCode, entity.ColumnCode);
            sqlBuilder.SetValue(BaseTableColumnsEntity.FieldColumnName, entity.ColumnName);
            sqlBuilder.SetValue(BaseTableColumnsEntity.FieldIsPublic, entity.IsPublic);
            sqlBuilder.SetValue(BaseTableColumnsEntity.FieldEnabled, entity.Enabled);
            sqlBuilder.SetValue(BaseTableColumnsEntity.FieldAllowEdit, entity.AllowEdit);
            sqlBuilder.SetValue(BaseTableColumnsEntity.FieldAllowDelete, entity.AllowDelete);
            sqlBuilder.SetValue(BaseTableColumnsEntity.FieldDeleted, entity.DeletionStateCode);
            sqlBuilder.SetValue(BaseTableColumnsEntity.FieldSortCode, entity.SortCode);
            sqlBuilder.SetValue(BaseTableColumnsEntity.FieldDescription, entity.Description);
            SetEntityExtend(sqlBuilder, entity);
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

        /// <summary>
        /// 获取删除的参数
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
		protected override List<KeyValuePair<string, object>> GetDeleteExtParam(List<KeyValuePair<string, object>> parameters)
		{
			var result = base.GetDeleteExtParam(parameters);
			result.Add(new KeyValuePair<string, object>(BaseTableColumnsEntity.FieldAllowDelete, 1));
			return result;
		}
    }
}
