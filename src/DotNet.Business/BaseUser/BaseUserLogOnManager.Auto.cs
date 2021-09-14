//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System.Collections.Generic;

namespace DotNet.Business
{
    using Model;
    using Util;

	/// <summary>
	/// BaseUserLogOnManager
	/// 系统用户表登录信息
	///
	/// 修改记录
	///
	///		2013-04-21 版本：1.0 JiRiGaLa 创建主键。
	///
	/// <author>
	///		<name>JiRiGaLa</name>
	///		<date>2013-04-21</date>
	/// </author>
	/// </summary>
	public partial class BaseUserLogOnManager : BaseManager, IBaseManager
	{
		/// <summary>
		/// 构造函数
		/// </summary>
		public BaseUserLogOnManager()
		{
			if (dbHelper == null)
			{
				dbHelper = DbHelperFactory.GetHelper(BaseSystemInfo.UserCenterDbType, BaseSystemInfo.UserCenterDbConnection);
			}
			if (string.IsNullOrEmpty(CurrentTableName))
			{
				CurrentTableName = BaseUserLogOnEntity.TableName;
			}
			// 不是自增量添加
			Identity = false;
		}

		/// <summary>
		/// 构造函数
		/// <param name="tableName">指定表名</param>
		/// </summary>
		public BaseUserLogOnManager(string tableName): this()
		{
			CurrentTableName = tableName;
		}

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="dbHelper">数据库连接</param>
		public BaseUserLogOnManager(IDbHelper dbHelper)
			: this()
		{
			DbHelper = dbHelper;
		}

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="userInfo">用户信息</param>
		public BaseUserLogOnManager(BaseUserInfo userInfo)
			: this()
		{
			UserInfo = userInfo;
		}

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="dbHelper">数据库连接</param>
		/// <param name="userInfo">用户信息</param>
		public BaseUserLogOnManager(IDbHelper dbHelper, BaseUserInfo userInfo)
			: this(dbHelper)
		{
			UserInfo = userInfo;
		}

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="userInfo">用户信息</param>
		/// <param name="tableName">指定表名</param>
		public BaseUserLogOnManager(BaseUserInfo userInfo, string tableName)
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
		public BaseUserLogOnManager(IDbHelper dbHelper, BaseUserInfo userInfo, string tableName)
			: this(dbHelper, userInfo)
		{
			CurrentTableName = tableName;
		}

		/// <summary>
		/// 添加
		/// </summary>
		/// <param name="entity">实体</param>
		/// <returns>主键</returns>
		public string Add(BaseUserLogOnEntity entity)
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
		public string Add(BaseUserLogOnEntity entity, bool identity, bool returnId)
		{
			Identity = identity;
			ReturnId = returnId;
			return AddObject(entity);
		}

		/// <summary>
		/// 获取实体
		/// </summary>
		/// <param name="id">主键</param>
		public BaseUserLogOnEntity GetObject(int? id)
		{
            return BaseEntity.Create<BaseUserLogOnEntity>(ExecuteReader(new KeyValuePair<string, object>(BaseUserLogOnEntity.FieldId, id)));
            // return BaseEntity.Create<BaseUserLogOnEntity>(this.GetDataTable(new KeyValuePair<string, object>(BaseUserLogOnEntity.FieldId, id)));
		}

		/// <summary>
		/// 获取实体
		/// </summary>
		/// <param name="id">主键</param>
		public BaseUserLogOnEntity GetObject(string id)
		{
            return BaseEntity.Create<BaseUserLogOnEntity>(ExecuteReader(new KeyValuePair<string, object>(BaseUserLogOnEntity.FieldId, id)));
            // return BaseEntity.Create<BaseUserLogOnEntity>(this.GetDataTable(new KeyValuePair<string, object>(BaseUserLogOnEntity.FieldId, id)));
		}

		/// <summary>
		/// 添加实体
		/// </summary>
		/// <param name="entity">实体</param>
		public string AddObject(BaseUserLogOnEntity entity)
		{
			var result = string.Empty;
			if (string.IsNullOrEmpty(entity.Id))
			{
				var manager = new BaseSequenceManager(DbHelper, Identity);
				result = manager.Increment(CurrentTableName);
				entity.Id = result;
			}
			var sqlBuilder = new SqlBuilder(DbHelper, Identity, ReturnId);
			sqlBuilder.BeginInsert(CurrentTableName, BaseUserLogOnEntity.FieldId);
			if (!Identity)
			{
				sqlBuilder.SetValue(BaseUserLogOnEntity.FieldId, entity.Id);
			}
			else
			{
				if (!ReturnId && (DbHelper.CurrentDbType == CurrentDbType.Oracle || DbHelper.CurrentDbType == CurrentDbType.Db2))
				{
					if (DbHelper.CurrentDbType == CurrentDbType.Oracle)
					{
						sqlBuilder.SetFormula(BaseUserLogOnEntity.FieldId, "SEQ_" + CurrentTableName.ToUpper() + ".NEXTVAL ");
					}
					if (DbHelper.CurrentDbType == CurrentDbType.Db2)
					{
						sqlBuilder.SetFormula(BaseUserLogOnEntity.FieldId, "NEXT VALUE FOR SEQ_" + CurrentTableName.ToUpper());
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
						sqlBuilder.SetValue(BaseUserLogOnEntity.FieldId, entity.Id);
					}
				}
			}
			SetObject(sqlBuilder, entity);
			if (UserInfo != null)
			{
				sqlBuilder.SetValue(BaseUserLogOnEntity.FieldCreateUserId, UserInfo.Id);
				sqlBuilder.SetValue(BaseUserLogOnEntity.FieldCreateBy, UserInfo.RealName);
			}
			sqlBuilder.SetDbNow(BaseUserLogOnEntity.FieldCreateTime);
			if (UserInfo != null)
			{
				sqlBuilder.SetValue(BaseUserLogOnEntity.FieldUpdateUserId, UserInfo.Id);
				sqlBuilder.SetValue(BaseUserLogOnEntity.FieldUpdateBy, UserInfo.RealName);
			}
			sqlBuilder.SetDbNow(BaseUserLogOnEntity.FieldUpdateTime);
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
		public int UpdateObject(BaseUserLogOnEntity entity)
		{
			var sqlBuilder = new SqlBuilder(DbHelper);
			sqlBuilder.BeginUpdate(CurrentTableName);
			SetObject(sqlBuilder, entity);
			if (UserInfo != null)
			{
				sqlBuilder.SetValue(BaseUserLogOnEntity.FieldUpdateUserId, UserInfo.Id);
				sqlBuilder.SetValue(BaseUserLogOnEntity.FieldUpdateBy, UserInfo.RealName);
			}
			sqlBuilder.SetDbNow(BaseUserLogOnEntity.FieldUpdateTime);
			sqlBuilder.SetWhere(BaseUserLogOnEntity.FieldId, entity.Id);
			return sqlBuilder.EndUpdate();
		}

	    /// <summary>
	    /// 设置实体
	    /// </summary>
	    /// <param name="sqlBuilder">SQL语句生成器</param>
	    /// <param name="entity">实体</param>
	    private void SetObject(SqlBuilder sqlBuilder, BaseUserLogOnEntity entity)
		{
            // 2016-03-02 吉日嘎拉 增加按公司可以区别数据的功能。
            if (DbHelper.CurrentDbType == CurrentDbType.MySql)
            {
                sqlBuilder.SetValue(BaseUserContactEntity.FieldCompanyId, entity.CompanyId);
            }
			sqlBuilder.SetValue(BaseUserLogOnEntity.FieldUserPassword, entity.UserPassword);
			sqlBuilder.SetValue(BaseUserLogOnEntity.FieldPasswordErrorCount, entity.PasswordErrorCount);
			sqlBuilder.SetValue(BaseUserLogOnEntity.FieldMultiUserLogin, entity.MultiUserLogin);
			sqlBuilder.SetValue(BaseUserLogOnEntity.FieldOpenId, entity.OpenId);
			sqlBuilder.SetValue(BaseUserLogOnEntity.FieldQuestion, entity.Question);
			sqlBuilder.SetValue(BaseUserLogOnEntity.FieldAnswerQuestion, entity.AnswerQuestion);
			sqlBuilder.SetValue(BaseUserLogOnEntity.FieldChangePasswordDate, entity.ChangePasswordDate);
			sqlBuilder.SetValue(BaseUserLogOnEntity.FieldSystemCode, entity.SystemCode);
            sqlBuilder.SetValue(BaseUserLogOnEntity.FieldEnabled, entity.Enabled);
			//sqlBuilder.SetValue(BaseUserLogOnEntity.FieldCommunicationPassword, entity.CommunicationPassword);
			sqlBuilder.SetValue(BaseUserLogOnEntity.FieldAllowStartTime, entity.AllowStartTime);
			sqlBuilder.SetValue(BaseUserLogOnEntity.FieldAllowEndTime, entity.AllowEndTime);
			sqlBuilder.SetValue(BaseUserLogOnEntity.FieldLockStartDate, entity.LockStartDate);
			sqlBuilder.SetValue(BaseUserLogOnEntity.FieldLockEndDate, entity.LockEndDate);
			sqlBuilder.SetValue(BaseUserLogOnEntity.FieldFirstVisit, entity.FirstVisit);
			sqlBuilder.SetValue(BaseUserLogOnEntity.FieldPreviousVisit, entity.PreviousVisit);
			sqlBuilder.SetValue(BaseUserLogOnEntity.FieldLastVisit, entity.LastVisit);
			sqlBuilder.SetValue(BaseUserLogOnEntity.FieldCheckIpAddress, entity.CheckIpAddress);
            sqlBuilder.SetValue(BaseUserLogOnEntity.FieldShowCount, entity.ShowCount);
			sqlBuilder.SetValue(BaseUserLogOnEntity.FieldLogOnCount, entity.LogOnCount);
			sqlBuilder.SetValue(BaseUserLogOnEntity.FieldUserOnLine, entity.UserOnLine);
			sqlBuilder.SetValue(BaseUserLogOnEntity.FieldIpAddress, entity.IpAddress);
			sqlBuilder.SetValue(BaseUserLogOnEntity.FieldIpAddressName, entity.IpAddressName);
			sqlBuilder.SetValue(BaseUserLogOnEntity.FieldMacAddress, entity.MacAddress);
			sqlBuilder.SetValue(BaseUserLogOnEntity.FieldSalt, entity.Salt);
			sqlBuilder.SetValue(BaseUserLogOnEntity.FieldPasswordStrength, entity.PasswordStrength);
            sqlBuilder.SetValue(BaseUserLogOnEntity.FieldNeedModifyPassword, entity.NeedModifyPassword);
		}
	}
}
