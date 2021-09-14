//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace DotNet.Business
{
    using Model;
    using Util;

	/// <summary>
	/// BaseUserManager
	/// 系统用户表
	///
	/// 修改记录
	///
	///		2010-08-07 版本：1.0 JiRiGaLa 创建主键。
	///
	/// <author>
	///		<name>JiRiGaLa</name>
	///		<date>2010-08-07</date>
	/// </author>
	/// </summary>
	public partial class BaseUserManager : BaseManager, IBaseManager
	{
		/// <summary>
		/// 构造函数
		/// </summary>
		public BaseUserManager()
		{
			if (dbHelper == null)
			{
				dbHelper = DbHelperFactory.GetHelper(BaseSystemInfo.UserCenterDbType, BaseSystemInfo.UserCenterDbConnection);
			}
			if (string.IsNullOrEmpty(CurrentTableName))
			{
				CurrentTableName = BaseUserEntity.TableName;
			}
		}

		/// <summary>
		/// 构造函数
		/// <param name="tableName">指定表名</param>
		/// </summary>
		public BaseUserManager(string tableName): this()
		{
			CurrentTableName = tableName;
		}

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="dbHelper">数据库连接</param>
		public BaseUserManager(IDbHelper dbHelper)
			: this()
		{
			DbHelper = dbHelper;
		}

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="userInfo">用户信息</param>
		public BaseUserManager(BaseUserInfo userInfo)
			: this()
		{
			UserInfo = userInfo;
		}

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="dbHelper">数据库连接</param>
		/// <param name="userInfo">用户信息</param>
		public BaseUserManager(IDbHelper dbHelper, BaseUserInfo userInfo)
			: this()
		{
            DbHelper = dbHelper;
			UserInfo = userInfo;
		}

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="dbHelper">数据库连接</param>
		/// <param name="userInfo">用户信息</param>
		/// <param name="tableName">指定表名</param>
		public BaseUserManager(IDbHelper dbHelper, BaseUserInfo userInfo, string tableName)
			: this(dbHelper, userInfo)
		{
			CurrentTableName = tableName;
		}

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="userInfo">用户信息</param>
		/// <param name="tableName">指定表名</param>
		public BaseUserManager(BaseUserInfo userInfo, string tableName)
			: this(userInfo)
		{
			CurrentTableName = tableName;
		}

		/// <summary>
		/// 添加
		/// </summary>
		/// <param name="entity">实体</param>
		/// <param name="identity">自增量方式</param>
		/// <param name="returnId">返回主鍵</param>
		/// <returns>主键</returns>
		public string Add(BaseUserEntity entity, bool identity, bool returnId)
		{
			Identity = identity;
			ReturnId = returnId;
			return AddObject(entity);
		}

		/// <summary>
		/// 获取实体
		/// </summary>
		/// <param name="id">主键</param>
		public BaseUserEntity GetObject(int? id)
		{
            return BaseEntity.Create<BaseUserEntity>(ExecuteReader(new KeyValuePair<string, object>(BaseUserEntity.FieldId, id)));
            // return BaseEntity.Create<BaseUserEntity>(this.GetDataTable(new KeyValuePair<string, object>(BaseUserEntity.FieldId, id)));
		}

		/// <summary>
		/// 获取实体
		/// </summary>
		/// <param name="id">主键</param>
		public BaseUserEntity GetObject(string id)
		{
            return BaseEntity.Create<BaseUserEntity>(ExecuteReader(new KeyValuePair<string, object>(BaseUserEntity.FieldId, id)));
            // return BaseEntity.Create<BaseUserEntity>(this.GetDataTable(new KeyValuePair<string, object>(BaseUserEntity.FieldId, id)));
		}

		/// <summary>
		/// 添加实体
		/// </summary>
		/// <param name="entity">实体</param>
		public string AddObject(BaseUserEntity entity)
		{
			var result = string.Empty;

			if (entity.SortCode == 0)
			{
				var managerSequence = new BaseSequenceManager(DbHelper, Identity);
				result = managerSequence.Increment(CurrentTableName);
				entity.SortCode = int.Parse(result);
			}

            // 2015-12-11 吉日嘎拉 全部小写，提高Oracle的效率
			entity.QuickQuery = StringUtil.GetPinyin(entity.RealName).ToLower();
            entity.SimpleSpelling = StringUtil.GetSimpleSpelling(entity.RealName).ToLower();

			var sqlBuilder = new SqlBuilder(DbHelper, Identity, ReturnId);
			sqlBuilder.BeginInsert(CurrentTableName, BaseUserEntity.FieldId);

            if (DbHelper.CurrentDbType == CurrentDbType.MySql && string.IsNullOrEmpty(entity.Id))
            {
                entity.Id = Guid.NewGuid().ToString("N");
                result = entity.Id;
            }

			if (!Identity || !string.IsNullOrEmpty(entity.Id))
			{
				sqlBuilder.SetValue(BaseUserEntity.FieldId, entity.Id);
				result = entity.Id;
			}
			else
			{
				if (!ReturnId && (DbHelper.CurrentDbType == CurrentDbType.Oracle || DbHelper.CurrentDbType == CurrentDbType.Db2))
				{
					if (DbHelper.CurrentDbType == CurrentDbType.Oracle)
					{
						sqlBuilder.SetFormula(BaseUserEntity.FieldId, "SEQ_" + CurrentTableName.ToUpper() + ".NEXTVAL ");
					}
					if (DbHelper.CurrentDbType == CurrentDbType.Db2)
					{
						sqlBuilder.SetFormula(BaseUserEntity.FieldId, "NEXT VALUE FOR SEQ_" + CurrentTableName.ToUpper());
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
						sqlBuilder.SetValue(BaseUserEntity.FieldId, entity.Id);
					}
				}
			}

			SetObject(sqlBuilder, entity);

			if (UserInfo != null)
			{
				sqlBuilder.SetValue(BaseUserEntity.FieldCreateUserId, UserInfo.Id);
				sqlBuilder.SetValue(BaseUserEntity.FieldCreateBy, UserInfo.RealName);
			}

			sqlBuilder.SetDbNow(BaseUserEntity.FieldCreateTime);

			if (UserInfo != null)
			{
				sqlBuilder.SetValue(BaseUserEntity.FieldUpdateUserId, UserInfo.Id);
				sqlBuilder.SetValue(BaseUserEntity.FieldUpdateBy, UserInfo.RealName);
			}

			sqlBuilder.SetDbNow(BaseUserEntity.FieldUpdateTime);

            // 2015-12-16 吉日嘎拉 优化返回值问题、提高自增Id导入的问题的效率，关联数据产生问题
            if (Identity 
                && (DbHelper.CurrentDbType == CurrentDbType.SqlServer || DbHelper.CurrentDbType == CurrentDbType.MySql))
			{
				result = sqlBuilder.EndInsert().ToString();
				entity.Id = result;
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
		public int UpdateObject(BaseUserEntity entity)
		{
			if (string.IsNullOrEmpty(entity.QuickQuery))
			{
                // 2015-12-11 吉日嘎拉 全部小写，提高Oracle的效率
				entity.QuickQuery = StringUtil.GetPinyin(entity.RealName).ToLower();
			}
			if (string.IsNullOrEmpty(entity.SimpleSpelling))
			{
                // 2015-12-11 吉日嘎拉 全部小写，提高Oracle的效率
				entity.SimpleSpelling = StringUtil.GetSimpleSpelling(entity.RealName).ToLower();
			}

			var sqlBuilder = new SqlBuilder(DbHelper);
			sqlBuilder.BeginUpdate(CurrentTableName);
            // 2015-12-11 吉日嘎拉 全部小写，提高Oracle的效率
			entity.QuickQuery = StringUtil.GetPinyin(entity.RealName).ToLower();
			SetObject(sqlBuilder, entity);
			if (UserInfo != null)
			{
				sqlBuilder.SetValue(BaseUserEntity.FieldUpdateUserId, UserInfo.Id);
				sqlBuilder.SetValue(BaseUserEntity.FieldUpdateBy, UserInfo.RealName);
			}
			// 若有修改时间标示，那就按修改时间来，不是按最新的时间来
			if (entity.ModifiedOn.HasValue)
			{
				sqlBuilder.SetValue(BaseUserEntity.FieldUpdateTime, entity.ModifiedOn.Value);
			}
			else
			{
				sqlBuilder.SetDbNow(BaseUserEntity.FieldUpdateTime);
			}
			sqlBuilder.SetWhere(BaseUserEntity.FieldId, entity.Id);
			return sqlBuilder.EndUpdate();
		}

	    /// <summary>
	    /// 设置实体
	    /// </summary>
	    /// <param name="sqlBuilder">SQL语句生成器</param>
	    /// <param name="entity">实体</param>
	    private void SetObject(SqlBuilder sqlBuilder, BaseUserEntity entity)
		{
			sqlBuilder.SetValue(BaseUserEntity.FieldUserFrom, entity.UserFrom);
			sqlBuilder.SetValue(BaseUserEntity.FieldCode, entity.Code);
			sqlBuilder.SetValue(BaseUserEntity.FieldUserName, entity.UserName);
			sqlBuilder.SetValue(BaseUserEntity.FieldRealName, entity.RealName);
			sqlBuilder.SetValue(BaseUserEntity.FieldNickName, entity.NickName);
			sqlBuilder.SetValue(BaseUserEntity.FieldIdCard, entity.IdCard);
			sqlBuilder.SetValue(BaseUserEntity.FieldQuickQuery, entity.QuickQuery);
			sqlBuilder.SetValue(BaseUserEntity.FieldSimpleSpelling, entity.SimpleSpelling);
			sqlBuilder.SetValue(BaseUserEntity.FieldSecurityLevel, entity.SecurityLevel);
			sqlBuilder.SetValue(BaseUserEntity.FieldWorkCategory, entity.WorkCategory);
			sqlBuilder.SetValue(BaseUserEntity.FieldCompanyId, entity.CompanyId);
			sqlBuilder.SetValue(BaseUserEntity.FieldCompanyName, entity.CompanyName);
			sqlBuilder.SetValue(BaseUserEntity.FieldSubCompanyId, entity.SubCompanyId);
			sqlBuilder.SetValue(BaseUserEntity.FieldSubCompanyName, entity.SubCompanyName);
			sqlBuilder.SetValue(BaseUserEntity.FieldDepartmentId, entity.DepartmentId);
			sqlBuilder.SetValue(BaseUserEntity.FieldDepartmentName, entity.DepartmentName);
			sqlBuilder.SetValue(BaseUserEntity.FieldSubDepartmentId, entity.SubDepartmentId);
			sqlBuilder.SetValue(BaseUserEntity.FieldSubDepartmentName, entity.SubDepartmentName);
			sqlBuilder.SetValue(BaseUserEntity.FieldWorkgroupId, entity.WorkgroupId);
			sqlBuilder.SetValue(BaseUserEntity.FieldWorkgroupName, entity.WorkgroupName);
			sqlBuilder.SetValue(BaseUserEntity.FieldGender, entity.Gender);
			sqlBuilder.SetValue(BaseUserEntity.FieldBirthday, entity.Birthday);
			sqlBuilder.SetValue(BaseUserEntity.FieldDuty, entity.Duty);
			sqlBuilder.SetValue(BaseUserEntity.FieldTitle, entity.Title);
			sqlBuilder.SetValue(BaseUserEntity.FieldProvince, entity.Province);
			sqlBuilder.SetValue(BaseUserEntity.FieldCity, entity.City);
			sqlBuilder.SetValue(BaseUserEntity.FieldDistrict, entity.District);
			sqlBuilder.SetValue(BaseUserEntity.FieldHomeAddress, entity.HomeAddress);
			sqlBuilder.SetValue(BaseUserEntity.FieldScore, entity.Score);
			sqlBuilder.SetValue(BaseUserEntity.FieldLang, entity.Lang);
			sqlBuilder.SetValue(BaseUserEntity.FieldTheme, entity.Theme);
			sqlBuilder.SetValue(BaseUserEntity.FieldIsStaff, entity.IsStaff);
            sqlBuilder.SetValue(BaseUserEntity.FieldIsCheckBalance, entity.IsCheckBalance);
			sqlBuilder.SetValue(BaseUserEntity.FieldManagerId, entity.ManagerId);
			sqlBuilder.SetValue(BaseUserEntity.FieldManagerAuditStatus, entity.ManagerAuditStatus);
			sqlBuilder.SetValue(BaseUserEntity.FieldManagerAuditDate, entity.ManagerAuditDate);

			sqlBuilder.SetValue(BaseUserEntity.FieldAuditStatus, entity.AuditStatus);
			sqlBuilder.SetValue(BaseUserEntity.FieldIsVisible, entity.IsVisible);
			sqlBuilder.SetValue(BaseUserEntity.FieldDeleted, entity.DeletionStateCode);
			sqlBuilder.SetValue(BaseUserEntity.FieldEnabled, entity.Enabled);
			sqlBuilder.SetValue(BaseUserEntity.FieldSortCode, entity.SortCode);
			sqlBuilder.SetValue(BaseUserEntity.FieldDescription, entity.Description);
			sqlBuilder.SetValue(BaseUserEntity.FieldSignature, entity.Signature);
		}
	}
}
