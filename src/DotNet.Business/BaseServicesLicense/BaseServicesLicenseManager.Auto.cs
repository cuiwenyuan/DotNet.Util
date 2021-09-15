//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System.Collections.Generic;

namespace DotNet.Business
{
    using Model;
    using Util;

	/// <summary>
    /// BaseServicesLicenseManager
	/// 参数类
	/// 
	/// 修改记录
	///		2015.12.25 版本：1.0 JiRiGaLa	创建。
	///
	/// <author>
	///		<name>JiRiGaLa</name>
    ///		<date>2015.12.25</date>
	/// </author> 
	/// </summary>
    public partial class BaseServicesLicenseManager : BaseManager
	{
        /// <summary>
        /// 构造函数
        /// </summary>
		public BaseServicesLicenseManager()
		{
			if (dbHelper == null)
			{
				dbHelper = DbHelperFactory.GetHelper(BaseSystemInfo.UserCenterDbType, BaseSystemInfo.UserCenterDbConnection);
			}
			CurrentTableName = BaseServicesLicenseEntity.TableName;
		}
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbHelper"></param>
		public BaseServicesLicenseManager(IDbHelper dbHelper) : this()
		{
			DbHelper = dbHelper;
		}
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="userInfo"></param>
		public BaseServicesLicenseManager(BaseUserInfo userInfo) : this()
		{
			UserInfo = userInfo;
		}
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbHelper"></param>
        /// <param name="userInfo"></param>
		public BaseServicesLicenseManager(IDbHelper dbHelper, BaseUserInfo userInfo) : this()
		{
			DbHelper = dbHelper;
			UserInfo = userInfo;
		}
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbHelper"></param>
        /// <param name="tableName"></param>
        public BaseServicesLicenseManager(IDbHelper dbHelper, string tableName)
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
        public BaseServicesLicenseManager(IDbHelper dbHelper, BaseUserInfo userInfo, string tableName)
            : this()
        {
            DbHelper = dbHelper;
            UserInfo = userInfo;
            CurrentTableName = tableName;
        }

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="id">主键</param>
        public BaseServicesLicenseEntity GetEntity(string id)
        {
            return BaseEntity.Create<BaseServicesLicenseEntity>(ExecuteReader(new KeyValuePair<string, object>(BaseServicesLicenseEntity.FieldId, id)));
        }
        
        /// <summary>
        /// 添加实体
        /// </summary>
        /// <param name="entity">实体</param>
        public string AddEntity(BaseServicesLicenseEntity entity)
        {
            var result = string.Empty;
            var sqlBuilder = new SqlBuilder(DbHelper, Identity, ReturnId);
            sqlBuilder.BeginInsert(CurrentTableName, BaseServicesLicenseEntity.FieldId);
            if (!string.IsNullOrEmpty(entity.Id) || !Identity)
            {
                result = entity.Id;
                sqlBuilder.SetValue(BaseServicesLicenseEntity.FieldId, entity.Id);
            }
            else
            {
                if (!ReturnId && (DbHelper.CurrentDbType == CurrentDbType.Oracle || DbHelper.CurrentDbType == CurrentDbType.Db2))
                {
                    if (DbHelper.CurrentDbType == CurrentDbType.Oracle)
                    {
                        sqlBuilder.SetFormula(BaseServicesLicenseEntity.FieldId, "SEQ_" + BaseServicesLicenseEntity.TableName.ToUpper() + ".NEXTVAL ");
                    }
                    if (DbHelper.CurrentDbType == CurrentDbType.Db2)
                    {
                        sqlBuilder.SetFormula(BaseServicesLicenseEntity.FieldId, "NEXT VALUE FOR SEQ_" + CurrentTableName.ToUpper());
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
                                result = managerSequence.Increment(CurrentTableName);
                            }
                            entity.Id = result;
                        }
                        sqlBuilder.SetValue(BaseServicesLicenseEntity.FieldId, entity.Id);
                    }
                }
            }
            SetEntity(sqlBuilder, entity);

            // 创建人信息
            if (!string.IsNullOrEmpty(entity.CreateUserId))
            {
                sqlBuilder.SetValue(BaseServicesLicenseEntity.FieldCreateUserId, entity.CreateUserId);
            }
            else
            {
                if (UserInfo != null)
                {
                    sqlBuilder.SetValue(BaseServicesLicenseEntity.FieldCreateUserId, UserInfo.Id);
                }
            }
            if (!string.IsNullOrEmpty(entity.CreateBy))
            {
                sqlBuilder.SetValue(BaseServicesLicenseEntity.FieldCreateBy, entity.CreateBy);
            }
            else
            {
                if (UserInfo != null)
                {
                    sqlBuilder.SetValue(BaseServicesLicenseEntity.FieldCreateBy, UserInfo.RealName);
                }
            }
            if (entity.CreateOn.HasValue)
            {
                sqlBuilder.SetValue(BaseServicesLicenseEntity.FieldCreateTime, entity.CreateOn);
            }
            else
            {
                sqlBuilder.SetDbNow(BaseServicesLicenseEntity.FieldCreateTime);
            }

            // 修改人信息
            if (!string.IsNullOrEmpty(entity.ModifiedUserId))
            {
                sqlBuilder.SetValue(BaseServicesLicenseEntity.FieldUpdateUserId, entity.ModifiedUserId);
            }
            else
            {
                if (UserInfo != null)
                {
                    sqlBuilder.SetValue(BaseServicesLicenseEntity.FieldUpdateUserId, UserInfo.Id);
                }
            }
            if (!string.IsNullOrEmpty(entity.ModifiedBy))
            {
                sqlBuilder.SetValue(BaseServicesLicenseEntity.FieldUpdateBy, entity.ModifiedBy);
            }
            else
            {
                if (UserInfo != null)
                {
                    sqlBuilder.SetValue(BaseServicesLicenseEntity.FieldUpdateBy, UserInfo.RealName);
                }
            }
            if (entity.ModifiedOn.HasValue)
            {
                sqlBuilder.SetValue(BaseServicesLicenseEntity.FieldUpdateTime, entity.ModifiedOn);
            }
            else
            {
                sqlBuilder.SetDbNow(BaseServicesLicenseEntity.FieldUpdateTime);
            }

            if (DbHelper.CurrentDbType == CurrentDbType.SqlServer && Identity)
            {
                result = sqlBuilder.EndInsert().ToString();
            }
            else
            {
                sqlBuilder.EndInsert();
            }
            return result;
        }

        /// <summary>
        /// 更新实体
        /// </summary>
        /// <param name="entity">实体</param>
        public int UpdateEntity(BaseServicesLicenseEntity entity)
        {
            var sqlBuilder = new SqlBuilder(DbHelper);
            sqlBuilder.BeginUpdate(CurrentTableName);
            SetEntity(sqlBuilder, entity);
            if (UserInfo != null)
            {
                sqlBuilder.SetValue(BaseServicesLicenseEntity.FieldUpdateUserId, UserInfo.Id);
                sqlBuilder.SetValue(BaseServicesLicenseEntity.FieldUpdateBy, UserInfo.RealName);
            }
            // 若有修改时间标示，那就按修改时间来，不是按最新的时间来
            if (entity.ModifiedOn.HasValue)
            {
                sqlBuilder.SetValue(BaseServicesLicenseEntity.FieldUpdateTime, entity.ModifiedOn.Value);
            }
            else
            {
                sqlBuilder.SetDbNow(BaseServicesLicenseEntity.FieldUpdateTime);
            }
            sqlBuilder.SetWhere(BaseServicesLicenseEntity.FieldId, entity.Id);
            return sqlBuilder.EndUpdate();
        }

        partial void SetEntityExtend(SqlBuilder sqlBuilder, BaseServicesLicenseEntity entity);

        /// <summary>
        /// 设置实体
        /// </summary>
        /// <param name="sqlBuilder">SQL语句生成器</param>
        /// <param name="entity">实体</param>
        private void SetEntity(SqlBuilder sqlBuilder, BaseServicesLicenseEntity entity)
        {
            sqlBuilder.SetValue(BaseServicesLicenseEntity.FieldUserId, entity.UserId);
            sqlBuilder.SetValue(BaseServicesLicenseEntity.FieldPrivateKey, entity.PrivateKey);
            sqlBuilder.SetValue(BaseServicesLicenseEntity.FieldPublicKey, entity.PublicKey);
            sqlBuilder.SetValue(BaseServicesLicenseEntity.FieldStartDate, entity.StartTime);
            sqlBuilder.SetValue(BaseServicesLicenseEntity.FieldEndDate, entity.EndTime);
            sqlBuilder.SetValue(BaseServicesLicenseEntity.FieldEnabled, entity.Enabled ? 1 : 0);
            sqlBuilder.SetValue(BaseServicesLicenseEntity.FieldDeleted, entity.DeletionStateCode);
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
	}
}