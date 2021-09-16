//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System.Collections.Generic;

namespace DotNet.Business
{
    using Model;
    using Util;

	/// <summary>
	/// BaseParameterManager
	/// 参数类
	/// 
	/// 修改记录
	///     2011.04.05 版本：2.2 zgl        修改AddEntity 为public 方法，ip限制功能中使用
	///     2009.04.01 版本：2.1 JiRiGaLa   创建者、修改者进行完善。
	///     2008.04.30 版本：2.0 JiRiGaLa   按面向对象，面向服务进行改进。
	///     2007.06.08 版本：1.4 JiRiGaLa   重新调整方法。
	///		2006.02.05 版本：1.3 JiRiGaLa	重新调整主键的规范化。
	///		2006.01.28 版本：1.2 JiRiGaLa	对一些方法进行改进，主键整理，调用性能也进行了修改，主键顺序进行整理。
	///		2005.08.13 版本：1.1 JiRiGaLa	主键整理好。
	///		2004.11.12 版本：1.0 JiRiGaLa	主键进行了绝对的优化，这是个好东西啊，平时要多用，用得要灵活些。
	///
	/// <author>
	///		<name>Troy.Cui</name>
	///		<date>2008.04.30</date>
	/// </author> 
	/// </summary>
    public partial class BaseParameterManager : BaseManager
	{
        /// <summary>
        /// 构造函数
        /// </summary>
		public BaseParameterManager()
		{
			if (dbHelper == null)
			{
				dbHelper = DbHelperFactory.GetHelper(BaseSystemInfo.UserCenterDbType, BaseSystemInfo.UserCenterDbConnection);
			}
			CurrentTableName = BaseParameterEntity.TableName;
		}

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbHelper"></param>
		public BaseParameterManager(IDbHelper dbHelper) : this()
		{
			DbHelper = dbHelper;
		}

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="userInfo"></param>
		public BaseParameterManager(BaseUserInfo userInfo) : this()
		{
			UserInfo = userInfo;
		}

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbHelper"></param>
        /// <param name="userInfo"></param>
		public BaseParameterManager(IDbHelper dbHelper, BaseUserInfo userInfo) : this()
		{
			DbHelper = dbHelper;
			UserInfo = userInfo;
		}

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbHelper"></param>
        /// <param name="tableName"></param>
        public BaseParameterManager(IDbHelper dbHelper, string tableName)
            : this()
        {
            DbHelper = dbHelper;
            CurrentTableName = tableName;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbHelper"></param>
        /// <param name="userInfo"></param>
        /// <param name="tableName"></param>
        public BaseParameterManager(IDbHelper dbHelper, BaseUserInfo userInfo, string tableName)
            : this()
        {
            DbHelper = dbHelper;
            UserInfo = userInfo;
            CurrentTableName = tableName;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="tableName"></param>
        public BaseParameterManager(string tableName)
            : this()
        {
            CurrentTableName = tableName;
        }

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="id">主键</param>
        public BaseParameterEntity GetEntity(string id)
        {
            return BaseEntity.Create<BaseParameterEntity>(ExecuteReader(new KeyValuePair<string, object>(BaseParameterEntity.FieldId, id)));
            // return BaseEntity.Create<BaseParameterEntity>(this.GetDataTable(new KeyValuePair<string, object>(BaseParameterEntity.FieldId, id)));
        }
        
        /// <summary>
        /// 添加实体
        /// </summary>
        /// <param name="entity">实体</param>
        public string AddEntity(BaseParameterEntity entity)
        {
            var result = string.Empty;
            //if (!entity.SortCode.HasValue)
            //{
            //    BaseSequenceManager managerSequence = new BaseSequenceManager(DbHelper, this.Identity);
            //    result = managerSequence.Increment(this.CurrentTableName);
            //    entity.SortCode = int.Parse(result);
            //}
            var sqlBuilder = new SqlBuilder(DbHelper, Identity, ReturnId);
            sqlBuilder.BeginInsert(CurrentTableName, BaseParameterEntity.FieldId);
            if (!string.IsNullOrEmpty(entity.Id) || !Identity)
            {
                result = entity.Id;
                sqlBuilder.SetValue(BaseParameterEntity.FieldId, entity.Id);
            }
            else
            {
                if (!ReturnId && (DbHelper.CurrentDbType == CurrentDbType.Oracle || DbHelper.CurrentDbType == CurrentDbType.Db2))
                {
                    if (DbHelper.CurrentDbType == CurrentDbType.Oracle)
                    {
                        sqlBuilder.SetFormula(BaseParameterEntity.FieldId, "SEQ_" + BaseParameterEntity.TableName.ToUpper() + ".NEXTVAL ");
                        // sqlBuilder.SetFormula(BaseParameterEntity.FieldId, "SEQ_" + this.CurrentTableName.ToUpper() + ".NEXTVAL ");
                    }
                    if (DbHelper.CurrentDbType == CurrentDbType.Db2)
                    {
                        sqlBuilder.SetFormula(BaseParameterEntity.FieldId, "NEXT VALUE FOR SEQ_" + CurrentTableName.ToUpper());
                    }
                }
                else
                {
                    if (Identity && (DbHelper.CurrentDbType == CurrentDbType.Oracle || DbHelper.CurrentDbType == CurrentDbType.Db2))
                    {
                        if (entity.Id == null)
                        {
                            if (string.IsNullOrEmpty(result))
                            {
                                var managerSequence = new BaseSequenceManager(DbHelper, Identity);
                                result = managerSequence.Increment(BaseParameterEntity.TableName);
                                // result = managerSequence.Increment(this.CurrentTableName);
                            }
                            entity.Id = result;
                        }
                        sqlBuilder.SetValue(BaseParameterEntity.FieldId, entity.Id);
                    }
                }
            }
            SetEntity(sqlBuilder, entity);

            // 创建人信息
            if (!string.IsNullOrEmpty(entity.CreateUserId))
            {
                sqlBuilder.SetValue(BaseParameterEntity.FieldCreateUserId, entity.CreateUserId);
            }
            else
            {
                if (UserInfo != null)
                {
                    sqlBuilder.SetValue(BaseParameterEntity.FieldCreateUserId, UserInfo.Id);
                }
            }
            if (!string.IsNullOrEmpty(entity.CreateBy))
            {
                sqlBuilder.SetValue(BaseParameterEntity.FieldCreateBy, entity.CreateBy);
            }
            else
            {
                if (UserInfo != null)
                {
                    sqlBuilder.SetValue(BaseParameterEntity.FieldCreateBy, UserInfo.RealName);
                }
            }
            if (entity.CreateOn.HasValue)
            {
                sqlBuilder.SetValue(BaseParameterEntity.FieldCreateTime, entity.CreateOn);
            }
            else
            {
                sqlBuilder.SetDbNow(BaseParameterEntity.FieldCreateTime);
            }

            // 修改人信息
            if (!string.IsNullOrEmpty(entity.ModifiedUserId))
            {
                sqlBuilder.SetValue(BaseParameterEntity.FieldUpdateUserId, entity.ModifiedUserId);
            }
            else
            {
                if (UserInfo != null)
                {
                    sqlBuilder.SetValue(BaseParameterEntity.FieldUpdateUserId, UserInfo.Id);
                }
            }
            if (!string.IsNullOrEmpty(entity.ModifiedBy))
            {
                sqlBuilder.SetValue(BaseParameterEntity.FieldUpdateBy, entity.ModifiedBy);
            }
            else
            {
                if (UserInfo != null)
                {
                    sqlBuilder.SetValue(BaseParameterEntity.FieldUpdateBy, UserInfo.RealName);
                }
            }
            if (entity.ModifiedOn.HasValue)
            {
                sqlBuilder.SetValue(BaseParameterEntity.FieldUpdateTime, entity.ModifiedOn);
            }
            else
            {
                sqlBuilder.SetDbNow(BaseParameterEntity.FieldUpdateTime);
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
        public int UpdateEntity(BaseParameterEntity entity)
        {
            var sqlBuilder = new SqlBuilder(DbHelper);
            sqlBuilder.BeginUpdate(CurrentTableName);
            SetEntity(sqlBuilder, entity);
            if (UserInfo != null)
            {
                sqlBuilder.SetValue(BaseParameterEntity.FieldUpdateUserId, UserInfo.Id);
                sqlBuilder.SetValue(BaseParameterEntity.FieldUpdateBy, UserInfo.RealName);
            }
            // 若有修改时间标示，那就按修改时间来，不是按最新的时间来
            if (entity.ModifiedOn.HasValue)
            {
                sqlBuilder.SetValue(BaseParameterEntity.FieldUpdateTime, entity.ModifiedOn.Value);
            }
            else
            {
                sqlBuilder.SetDbNow(BaseParameterEntity.FieldUpdateTime);
            }
            sqlBuilder.SetWhere(BaseParameterEntity.FieldId, entity.Id);
            //return sqlBuilder.EndUpdate();
            var result = sqlBuilder.EndUpdate();
            if (result > 0)
            {
                RemoveCache();
            }
            return result;
        }

        partial void SetEntityExtend(SqlBuilder sqlBuilder, BaseParameterEntity entity);

	    /// <summary>
	    /// 设置实体
	    /// </summary>
	    /// <param name="sqlBuilder">SQL语句生成器</param>
	    /// <param name="entity">实体</param>
	    private void SetEntity(SqlBuilder sqlBuilder, BaseParameterEntity entity)
        {
            sqlBuilder.SetValue(BaseParameterEntity.FieldCategoryCode, entity.CategoryCode);
            sqlBuilder.SetValue(BaseParameterEntity.FieldParameterCode, entity.ParameterCode);
            sqlBuilder.SetValue(BaseParameterEntity.FieldParameterId, entity.ParameterId);
            sqlBuilder.SetValue(BaseParameterEntity.FieldParameterContent, entity.ParameterContent);
            sqlBuilder.SetValue(BaseParameterEntity.FieldWorked, entity.Worked ? 1 : 0);
            sqlBuilder.SetValue(BaseParameterEntity.FieldEnabled, entity.Enabled ? 1 : 0);
            sqlBuilder.SetValue(BaseParameterEntity.FieldDeleted, entity.DeletionStateCode);
            SetEntityExtend(sqlBuilder, entity);
        }

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns>影响行数</returns>
        public int Delete(int id)
        {
            var result = Delete(new List<KeyValuePair<string, object>> { new KeyValuePair<string, object>(BaseParameterEntity.FieldId, id) });
            if (result > 0)
            {
                RemoveCache();
            }
            return result;
        }
	}
}