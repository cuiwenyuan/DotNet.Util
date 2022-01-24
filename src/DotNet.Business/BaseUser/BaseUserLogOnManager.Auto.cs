//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System.Collections.Generic;

namespace DotNet.Business
{
    using Model;
    using Util;

	/// <summary>
	/// BaseUserLogonManager
	/// 系统用户表登录信息
	///
	/// 修改记录
	///
	///		2013-04-21 版本：1.0 JiRiGaLa 创建主键。
	///
	/// <author>
	///		<name>Troy.Cui</name>
	///		<date>2013-04-21</date>
	/// </author>
	/// </summary>
	public partial class BaseUserLogonManager : BaseManager, IBaseManager
	{
		/// <summary>
		/// 构造函数
		/// </summary>
		public BaseUserLogonManager()
		{
			if (dbHelper == null)
			{
				dbHelper = DbHelperFactory.GetHelper(BaseSystemInfo.UserCenterDbType, BaseSystemInfo.UserCenterDbConnection);
			}
			if (string.IsNullOrEmpty(CurrentTableName))
			{
				CurrentTableName = BaseUserLogonEntity.TableName;
			}
			// 不是自增量添加
			Identity = false;
		}

		/// <summary>
		/// 构造函数
		/// <param name="tableName">指定表名</param>
		/// </summary>
		public BaseUserLogonManager(string tableName): this()
		{
			CurrentTableName = tableName;
		}

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="dbHelper">数据库连接</param>
		public BaseUserLogonManager(IDbHelper dbHelper)
			: this()
		{
			DbHelper = dbHelper;
		}

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="userInfo">用户信息</param>
		public BaseUserLogonManager(BaseUserInfo userInfo)
			: this()
		{
			UserInfo = userInfo;
		}

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="dbHelper">数据库连接</param>
		/// <param name="userInfo">用户信息</param>
		public BaseUserLogonManager(IDbHelper dbHelper, BaseUserInfo userInfo)
			: this(dbHelper)
		{
			UserInfo = userInfo;
		}

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="userInfo">用户信息</param>
		/// <param name="tableName">指定表名</param>
		public BaseUserLogonManager(BaseUserInfo userInfo, string tableName)
			: this(userInfo)
		{
			CurrentTableName = tableName;
		}

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="dbHelper">数据库连接</param>
		/// <param name="userInfo">用户信息</param>
		/// <param name="tableName">指定表名</param>
		public BaseUserLogonManager(IDbHelper dbHelper, BaseUserInfo userInfo, string tableName)
			: this(dbHelper, userInfo)
		{
			CurrentTableName = tableName;
		}

		/// <summary>
		/// 添加
		/// </summary>
		/// <param name="entity">实体</param>
		/// <returns>主键</returns>
		public string Add(BaseUserLogonEntity entity)
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
		public string Add(BaseUserLogonEntity entity, bool identity, bool returnId)
		{
			Identity = identity;
			ReturnId = returnId;
			return AddEntity(entity);
		}

		/// <summary>
		/// 获取实体
		/// </summary>
		/// <param name="id">主键</param>
		public BaseUserLogonEntity GetEntity(int? id)
		{
            return BaseEntity.Create<BaseUserLogonEntity>(ExecuteReader(new KeyValuePair<string, object>(BaseUserLogonEntity.FieldId, id)));
            // return BaseEntity.Create<BaseUserLogonEntity>(this.GetDataTable(new KeyValuePair<string, object>(BaseUserLogonEntity.FieldId, id)));
		}

		/// <summary>
		/// 获取实体
		/// </summary>
		/// <param name="id">主键</param>
		public BaseUserLogonEntity GetEntity(string id)
		{
            return BaseEntity.Create<BaseUserLogonEntity>(ExecuteReader(new KeyValuePair<string, object>(BaseUserLogonEntity.FieldId, id)));
            // return BaseEntity.Create<BaseUserLogonEntity>(this.GetDataTable(new KeyValuePair<string, object>(BaseUserLogonEntity.FieldId, id)));
		}

		/// <summary>
		/// 添加实体
		/// </summary>
		/// <param name="entity">实体</param>
		public string AddEntity(BaseUserLogonEntity entity)
		{
			var result = string.Empty;
			if (string.IsNullOrEmpty(entity.Id))
			{
				var manager = new BaseSequenceManager(DbHelper, Identity);
				result = manager.Increment(CurrentTableName);
				entity.Id = result;
			}
			var sqlBuilder = new SqlBuilder(DbHelper, Identity, ReturnId);
			sqlBuilder.BeginInsert(CurrentTableName, BaseUserLogonEntity.FieldId);
			if (!Identity)
			{
				sqlBuilder.SetValue(BaseUserLogonEntity.FieldId, entity.Id);
			}
			else
			{
				if (!ReturnId && (DbHelper.CurrentDbType == CurrentDbType.Oracle || DbHelper.CurrentDbType == CurrentDbType.Db2))
				{
					if (DbHelper.CurrentDbType == CurrentDbType.Oracle)
					{
						sqlBuilder.SetFormula(BaseUserLogonEntity.FieldId, "SEQ_" + CurrentTableName.ToUpper() + ".NEXTVAL ");
					}
					if (DbHelper.CurrentDbType == CurrentDbType.Db2)
					{
						sqlBuilder.SetFormula(BaseUserLogonEntity.FieldId, "NEXT VALUE FOR SEQ_" + CurrentTableName.ToUpper());
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
						sqlBuilder.SetValue(BaseUserLogonEntity.FieldId, entity.Id);
					}
				}
			}
			SetEntity(sqlBuilder, entity);
			if (UserInfo != null)
			{
				sqlBuilder.SetValue(BaseUserLogonEntity.FieldCreateUserId, UserInfo.Id);
				sqlBuilder.SetValue(BaseUserLogonEntity.FieldCreateBy, UserInfo.RealName);
			}
			sqlBuilder.SetDbNow(BaseUserLogonEntity.FieldCreateTime);
			if (UserInfo != null)
			{
				sqlBuilder.SetValue(BaseUserLogonEntity.FieldUpdateUserId, UserInfo.Id);
				sqlBuilder.SetValue(BaseUserLogonEntity.FieldUpdateBy, UserInfo.RealName);
			}
			sqlBuilder.SetDbNow(BaseUserLogonEntity.FieldUpdateTime);
			if (DbHelper.CurrentDbType == CurrentDbType.SqlServer && Identity)
			{
				result = sqlBuilder.EndInsert().ToString();
			}
			else
			{
     			sqlBuilder.EndInsert();
                result = entity.Id;
            }
			return result;
		}

		/// <summary>
		/// 更新实体
		/// </summary>
		/// <param name="entity">实体</param>
		public int UpdateEntity(BaseUserLogonEntity entity)
		{
			var sqlBuilder = new SqlBuilder(DbHelper);
			sqlBuilder.BeginUpdate(CurrentTableName);
			SetEntity(sqlBuilder, entity);
			if (UserInfo != null)
			{
				sqlBuilder.SetValue(BaseUserLogonEntity.FieldUpdateUserId, UserInfo.Id);
				sqlBuilder.SetValue(BaseUserLogonEntity.FieldUpdateBy, UserInfo.RealName);
			}
			sqlBuilder.SetDbNow(BaseUserLogonEntity.FieldUpdateTime);
			sqlBuilder.SetWhere(BaseUserLogonEntity.FieldId, entity.Id);
			return sqlBuilder.EndUpdate();
		}

	    /// <summary>
	    /// 设置实体
	    /// </summary>
	    /// <param name="sqlBuilder">SQL语句生成器</param>
	    /// <param name="entity">实体</param>
	    private void SetEntity(SqlBuilder sqlBuilder, BaseUserLogonEntity entity)
		{
            // 2016-03-02 吉日嘎拉 增加按公司可以区别数据的功能。
            if (DbHelper.CurrentDbType == CurrentDbType.MySql)
            {
                sqlBuilder.SetValue(BaseUserContactEntity.FieldCompanyId, entity.CompanyId);
            }
			sqlBuilder.SetValue(BaseUserLogonEntity.FieldUserPassword, entity.UserPassword);
			sqlBuilder.SetValue(BaseUserLogonEntity.FieldPasswordErrorCount, entity.PasswordErrorCount);
			sqlBuilder.SetValue(BaseUserLogonEntity.FieldMultiUserLogin, entity.MultiUserLogin);
			sqlBuilder.SetValue(BaseUserLogonEntity.FieldOpenId, entity.OpenId);
			sqlBuilder.SetValue(BaseUserLogonEntity.FieldQuestion, entity.Question);
			sqlBuilder.SetValue(BaseUserLogonEntity.FieldAnswerQuestion, entity.AnswerQuestion);
			sqlBuilder.SetValue(BaseUserLogonEntity.FieldChangePasswordDate, entity.ChangePasswordDate);
			sqlBuilder.SetValue(BaseUserLogonEntity.FieldSystemCode, entity.SystemCode);
            sqlBuilder.SetValue(BaseUserLogonEntity.FieldEnabled, entity.Enabled);
			//sqlBuilder.SetValue(BaseUserLogonEntity.FieldCommunicationPassword, entity.CommunicationPassword);
			sqlBuilder.SetValue(BaseUserLogonEntity.FieldAllowStartTime, entity.AllowStartTime);
			sqlBuilder.SetValue(BaseUserLogonEntity.FieldAllowEndTime, entity.AllowEndTime);
			sqlBuilder.SetValue(BaseUserLogonEntity.FieldLockStartDate, entity.LockStartDate);
			sqlBuilder.SetValue(BaseUserLogonEntity.FieldLockEndDate, entity.LockEndDate);
			sqlBuilder.SetValue(BaseUserLogonEntity.FieldFirstVisit, entity.FirstVisit);
			sqlBuilder.SetValue(BaseUserLogonEntity.FieldPreviousVisit, entity.PreviousVisit);
			sqlBuilder.SetValue(BaseUserLogonEntity.FieldLastVisit, entity.LastVisit);
			sqlBuilder.SetValue(BaseUserLogonEntity.FieldCheckIpAddress, entity.CheckIpAddress);
            sqlBuilder.SetValue(BaseUserLogonEntity.FieldShowCount, entity.ShowCount);
			sqlBuilder.SetValue(BaseUserLogonEntity.FieldLogonCount, entity.LogonCount);
			sqlBuilder.SetValue(BaseUserLogonEntity.FieldUserOnline, entity.UserOnline);
			sqlBuilder.SetValue(BaseUserLogonEntity.FieldIpAddress, entity.IpAddress);
			sqlBuilder.SetValue(BaseUserLogonEntity.FieldIpAddressName, entity.IpAddressName);
			sqlBuilder.SetValue(BaseUserLogonEntity.FieldMacAddress, entity.MacAddress);
			sqlBuilder.SetValue(BaseUserLogonEntity.FieldSalt, entity.Salt);
			sqlBuilder.SetValue(BaseUserLogonEntity.FieldPasswordStrength, entity.PasswordStrength);
            sqlBuilder.SetValue(BaseUserLogonEntity.FieldNeedModifyPassword, entity.NeedModifyPassword);
		}
	}
}
