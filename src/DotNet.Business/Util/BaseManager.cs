//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;

namespace DotNet.Business
{
    using DotNet.Model;
    using Util;

    /// <summary>
    ///	BaseManager
    /// 通用基类部分
    /// 
    /// 总觉得自己写的程序不上档次，这些新技术也玩玩，也许做出来的东西更专业了。
    /// 修改记录
    /// 
    ///		2012.02.04 版本：1.5 JiRiGaLa 文件进行分割，简化处理。
    ///		2010.06.23 版本：1.4 JiRiGaLa 删除简化了一些重复的函数功能。
    ///		2007.11.22 版本：1.3 JiRiGaLa 创建没有BaseSystemInfo的构造函数。
    ///		2007.11.20 版本：1.2 JiRiGaLa 完善有数据库连接、当前用户信息的构造函数、增加NonSerialized。
    ///		2007.11.15 版本：1.1 JiRiGaLa 设置 SetParameter 函数功能。
    ///		2007.08.01 版本：1.0 JiRiGaLa 提炼了最基础的方法部分、觉得这些是很有必要的方法。
    ///
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2012.02.04</date>
    /// </author> 
    /// </summary>
    public partial class BaseManager : IBaseManager
    {
        /// <summary>
        /// 任务标识
        /// </summary>
        public string TaskId = string.Empty;

        /// <summary>
        /// 数据表主键，需要用单一字段做为主键，建议默认为Id字段
        /// </summary>
        public string PrimaryKey = "Id";

        /// <summary>
        /// 是否自增量？大并发数据主键生成需要用这个方法
        /// 不是所有的情况下，都是在进行插入操作的，也不都是有Id字段的
        /// </summary>
        public bool Identity = true;

        /// <summary>
        /// 插入数据时，是否需要返回主键
        /// 默认都是需要插入操作时都要返回主键的
        /// </summary>
        public bool ReturnId = true;

        /// <summary>
        /// 通过远程接口调用
        /// </summary>
        public bool RemoteInterface = false;

        /// <summary>
        /// 选取的字段
        /// </summary>
        public string SelectFields { get; set; } = "*";

        /// <summary>
        /// 当前表名
        /// </summary>
        public string CurrentTableName { get; set; }

        /// <summary>
        /// 当前表备注
        /// </summary>
        public string CurrentTableDescription { get; set; }

        /// <summary>
        /// 当前索引
        /// </summary>
        public string CurrentIndex { get; set; }

        private static readonly object _locker = new Object();

        /// <summary>
        /// 当前的数据库连接
        /// </summary>
        protected IDbHelper dbHelper = null;
        /// <summary>
        /// 当前的数据库连接
        /// </summary>
        public IDbHelper DbHelper
        {
            set => dbHelper = value;
            get
            {
                if (dbHelper == null)
                {
                    lock (_locker)
                    {
                        if (dbHelper == null)
                        {
                            //dbHelper = DbHelperFactory.Create(BaseSystemInfo.UserCenterDbType, BaseSystemInfo.UserCenterReadDbConnection);
                            //Troy 20160521不要强制默认UserCenter，要读取当前的数据库
                            dbHelper = DbHelperFactory.Create();
                        }
                    }
                }
                return dbHelper;
            }
        }

        /// <summary>
        /// 用户信息
        /// </summary>
        protected BaseUserInfo UserInfo = null;

        /// <summary>
        /// 构造函数
        /// </summary>
        public BaseManager()
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbHelper">数据库帮助接口</param>
        public BaseManager(IDbHelper dbHelper) : this()
        {
            DbHelper = dbHelper;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbHelper">数据库帮助接口</param>
        /// <param name="userInfo">用户信息</param>
        public BaseManager(IDbHelper dbHelper, BaseUserInfo userInfo) : this()
        {
            DbHelper = dbHelper;
            UserInfo = userInfo;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbHelper">数据库帮助接口</param>
        /// <param name="tableName">表名</param>
        public BaseManager(IDbHelper dbHelper, string tableName) : this()
        {
            DbHelper = dbHelper;
            CurrentTableName = tableName;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="tableName">表名</param>
        public BaseManager(string tableName) : this()
        {
            CurrentTableName = tableName;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        public BaseManager(BaseUserInfo userInfo) : this()
        {
            UserInfo = userInfo;
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        /// <param name="tableName">表名</param>
        public BaseManager(BaseUserInfo userInfo, string tableName) : this()
        {
            UserInfo = userInfo;
            CurrentTableName = tableName;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbHelper">数据库帮助接口</param>
        /// <param name="userInfo">用户信息</param>
        /// <param name="tableName">表名</param>
        public BaseManager(IDbHelper dbHelper, BaseUserInfo userInfo, string tableName) : this()
        {
            DbHelper = dbHelper;
            UserInfo = userInfo;
            CurrentTableName = tableName;
        }

        #region public void SetConnection(IDbHelper dbHelper)

        /// <summary>
        /// 设置数据库连接
        /// </summary>
        /// <param name="dbHelper">数据库帮助接口</param>
        public void SetConnection(IDbHelper dbHelper)
        {
            DbHelper = dbHelper;
        }

        #endregion

        #region public void SetConnection(BaseUserInfo userInfo)
        /// <summary>
        /// 设置当前用户
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        public void SetConnection(BaseUserInfo userInfo)
        {
            UserInfo = userInfo;
        }
        #endregion

        #region public void SetConnection(IDbHelper dbHelper, BaseUserInfo userInfo)
        /// <summary>
        /// 设置数据库连接、当前用户
        /// </summary>
        /// <param name="dbHelper">数据库连接接口</param>
        /// <param name="userInfo">用户信息</param>
        public void SetConnection(IDbHelper dbHelper, BaseUserInfo userInfo)
        {
            SetConnection(dbHelper);
            UserInfo = userInfo;
        }
        #endregion

        #region public virtual void SetParameter(IDbHelper dbHelper)
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbHelper">数据库连接接口</param>
        public virtual void SetParameter(IDbHelper dbHelper)
        {
            DbHelper = dbHelper;
        }
        #endregion

        #region public virtual void SetParameter(BaseUserInfo userInfo)
        /// <summary>
        /// 设置参数
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        public virtual void SetParameter(BaseUserInfo userInfo)
        {
            UserInfo = userInfo;
        }
        #endregion

        #region public virtual void SetParameter(IDbHelper dbHelper, BaseUserInfo userInfo)
        /// <summary>
        /// 设置参数
        /// </summary>
        /// <param name="dbHelper">数据库连接接口</param>
        /// <param name="userInfo">用户信息</param>
        public virtual void SetParameter(IDbHelper dbHelper, BaseUserInfo userInfo)
        {
            DbHelper = dbHelper;
            UserInfo = userInfo;
        }
        #endregion

        #region public virtual string AddEntity(object entity)

        /// <summary>
        /// 添加对象
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        public virtual string AddEntity(object entity)
        {
            return string.Empty;
        }

        #endregion

        #region public virtual int UpdateEntity(object entity)

        /// <summary>
        /// 更新对象
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        public virtual int UpdateEntity(object entity)
        {
            return 0;
        }

        #endregion

        #region public virtual int DeleteEntity(object id)

        /// <summary>
        /// 删除对象
        /// </summary>
        /// <param name="id">编号</param>
        /// <returns></returns>
        public virtual int DeleteEntity(object id)
        {
            return DeleteEntity(new KeyValuePair<string, object>(BaseUtil.FieldId, id));
        }

        #endregion

        #region public virtual int DeleteEntity(params KeyValuePair<string, object>[] parameters)

        /// <summary>
        /// 删除对象
        /// </summary>
        /// <param name="parameters">参数</param>
        /// <returns></returns>
        public virtual int DeleteEntity(params KeyValuePair<string, object>[] parameters)
        {
            var parametersList = new List<KeyValuePair<string, object>>();
            for (var i = 0; i < parameters.Length; i++)
            {
                parametersList.Add(parameters[i]);
            }
            return MyDelete(parametersList);
            //return DbUtil.Delete(DbHelper, this.CurrentTableName, parametersList);
        }

        #endregion

        #region public virtual int Delete(int id)

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns>影响行数</returns>
        public virtual int Delete(int id)
        {
            var result = Delete(new List<KeyValuePair<string, object>> { new KeyValuePair<string, object>(PrimaryKey, id) });
            if (result > 0)
            {
                RemoveCache(id);
            }
            return result;
        }

        #endregion

        #region public virtual int Delete(string id)

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns>影响行数</returns>
        public virtual int Delete(string id)
        {
            var result = Delete(new List<KeyValuePair<string, object>> { new KeyValuePair<string, object>(PrimaryKey, id) });
            if (result > 0)
            {
                RemoveCache(id);
            }
            return result;
        }

        #endregion

        #region 对象事件触发器（编写程序的人员，可以不实现这些方法）

        /// <summary>
        /// 增加后（这个函数需要覆盖）
        /// </summary>
        /// <returns></returns>
        public virtual bool AddBefore()
        {
            return true;
        }
        /// <summary>
        /// 增加前（这个函数需要覆盖）
        /// </summary>
        /// <returns></returns>
        public virtual bool AddAfter()
        {
            return true;
        }

        /// <summary>
        /// 更新前（这个函数需要覆盖）
        /// </summary>
        /// <returns></returns>
        public virtual bool UpdateBefore()
        {
            return true;
        }

        /// <summary>
        /// 更新后（这个函数需要覆盖）
        /// </summary>
        /// <returns></returns>
        public virtual bool UpdateAfter()
        {
            return true;
        }

        /// <summary>
        /// 获取前（这个函数需要覆盖）
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual bool GetBefore(string id)
        {
            return true;
        }

        /// <summary>
        /// 获取后（这个函数需要覆盖）
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual bool GetAfter(string id)
        {
            return true;
        }
        /// <summary>
        /// 删除前（这个函数需要覆盖）
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual bool DeleteBefore(string id)
        {
            // 这个函数需要覆盖
            return true;
        }

        /// <summary>
        /// 删除后（这个函数需要覆盖）
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual bool DeleteAfter(string id)
        {
            return true;
        }

        /// <summary>
        /// 批量操作保存（这个函数需要覆盖）
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public virtual int BatchSave(DataTable dt)
        {
            return 0;
        }

        #endregion

        /// <summary>
        /// 状态码的获取
        /// </summary>
        private Status _status = Status.Error;

        /// <summary>
        /// 运行状态返回值
        /// </summary>
        public Status Status
        {
            get => _status;
            set => _status = value;
        }

        /// <summary>
        /// 状态码的获取
        /// </summary>
        private string _statusCode = Status.Error.ToString();

        /// <summary>
        /// 运行状态返回值
        /// </summary>
        public string StatusCode
        {
            get => _statusCode;
            set => _statusCode = value;
        }

        private string _statusMessage = string.Empty;
        /// <summary>
        /// 运行状态的信息
        /// </summary>
        public string StatusMessage
        {
            get => _statusMessage;
            set => _statusMessage = value;
        }
        /// <summary>
        /// 获取状态信息
        /// </summary>
        /// <returns></returns>
        public string GetStateMessage()
        {
            StatusMessage = GetStateMessage(StatusCode);
            return StatusMessage;
        }
        /// <summary>
        /// 获取状态信息
        /// </summary>
        /// <param name="statusCode"></param>
        /// <returns></returns>
        public string GetStateMessage(string statusCode)
        {
            if (string.IsNullOrEmpty(statusCode))
            {
                return string.Empty;
            }
            var status = (Status)Enum.Parse(typeof(Status), statusCode, true);
            return GetStateMessage(status);
        }

        #region public string GetStateMessage(StatusCode statusCode) 获得状态的信息
        /// <summary>
        /// 获得状态的信息
        /// </summary>
        /// <param name="statusCode">程序运行状态</param>
        /// <returns>返回信息</returns>
        public string GetStateMessage(Status statusCode)
        {
            var result = string.Empty;

            switch (statusCode)
            {
                case Status.AccessDeny:
                    result = AppMessage.Msg0800;
                    break;
                case Status.DbError:
                    result = AppMessage.Msg0002;
                    break;
                case Status.Error:
                    result = AppMessage.Msg0001;
                    break;
                case Status.Ok:
                    result = AppMessage.Msg9965;
                    break;
                case Status.UserNotFound:
                    result = AppMessage.Msg9966;
                    break;
                case Status.PasswordError:
                    result = AppMessage.Msg9967;
                    break;
                case Status.LogonDeny:
                    result = AppMessage.Msg9968;
                    break;
                case Status.ErrorOnline:
                    result = AppMessage.Msg0048;
                    break;
                case Status.ErrorMacAddress:
                    result = AppMessage.Msg0049;
                    break;
                case Status.ErrorIpAddress:
                    result = string.Format(AppMessage.Msg0050, UserInfo.IpAddress);
                    break;
                case Status.ErrorOnlineLimit:
                    result = AppMessage.Msg0051;
                    break;
                case Status.PasswordCanNotBeNull:
                    result = AppMessage.Format(AppMessage.Msg0007, AppMessage.Msg9961);
                    break;
                case Status.PasswordCanNotBeRepeat:
                    result = AppMessage.Format(AppMessage.Msg0102);
                    break;
                case Status.ErrorDeleted:
                    result = AppMessage.Msg0005;
                    break;
                case Status.SetPasswordOk:
                    result = AppMessage.Format(AppMessage.Msg9963, AppMessage.Msg9964);
                    break;
                case Status.OldPasswordError:
                    result = AppMessage.Format(AppMessage.Msg0040, AppMessage.Msg9961);
                    break;
                case Status.ChangePasswordOk:
                    result = AppMessage.Format(AppMessage.Msg9962, AppMessage.Msg9964);
                    break;
                case Status.OkAdd:
                    result = AppMessage.Msg0009;
                    break;
                case Status.CanNotLock:
                    result = AppMessage.Msg0043;
                    break;
                case Status.LockOk:
                    result = AppMessage.Msg0044;
                    break;
                case Status.OkUpdate:
                    result = AppMessage.Msg0010;
                    break;
                case Status.OkDelete:
                    result = AppMessage.Msg0013;
                    break;
                case Status.Exist:
                    // "编号已存在,不可以重复."
                    result = AppMessage.Format(AppMessage.Msg0008, AppMessage.Msg9955);
                    break;
                case Status.ErrorCodeExist:
                    // "编号已存在,不可以重复."
                    result = AppMessage.Format(AppMessage.Msg0008, AppMessage.Msg9977);
                    break;
                case Status.ErrorNameExist:
                    // "名称已存在,不可以重复."
                    result = AppMessage.Format(AppMessage.Msg0008, AppMessage.Msg9978);
                    break;
                case Status.ErrorValueExist:
                    // "值已存在,不可以重复."
                    result = AppMessage.Format(AppMessage.Msg0008, AppMessage.Msg9800);
                    break;
                case Status.ErrorUserExist:
                    // "用户名已存在,不可以重复."
                    result = AppMessage.Format(AppMessage.Msg0008, AppMessage.Msg9957);
                    break;
                case Status.ErrorDataRelated:
                    result = AppMessage.Msg0033;
                    break;
                case Status.ErrorChanged:
                    result = AppMessage.Msg0006;
                    break;

                case Status.UserNotEmail:
                    result = AppMessage.Msg9910;
                    break;

                case Status.UserLocked:
                    result = AppMessage.Msg9911;
                    break;

                case Status.WaitForAudit:
                case Status.UserNotActive:
                    result = AppMessage.Msg9912;
                    break;

                case Status.UserIsActivate:
                    result = AppMessage.Msg9913;
                    break;

                case Status.NotFound:
                    result = AppMessage.Msg9956;
                    break;

                case Status.ErrorLogon:
                    result = AppMessage.Msg9000;
                    break;

                case Status.UserDuplicate:
                    result = AppMessage.Format(AppMessage.Msg0008, AppMessage.Msg9957);
                    break;
                case Status.ServiceNotStart:
                    result = AppMessage.Msg9660;
                    break;
                case Status.ServiceExpired:
                    result = AppMessage.Msg9665;
                    break;
            }
            StatusMessage = result;
            return result;
        }
        #endregion

        #region public int BatchSetSortCode(string[] ids, string[] codes) 重置编号
        /// <summary>
        /// 重置编号
        /// </summary>
        /// <param name="ids">主键数组</param>
        /// <param name="codes">编号数组</param>
        /// <returns>影响行数</returns>
        public int BatchSetSortCode(string[] ids, string[] codes)
        {
            var result = 0;
            for (var i = 0; i < ids.Length; i++)
            {
                result += SetProperty(ids[i], new KeyValuePair<string, object>(BaseUtil.FieldCode, codes[i]));
            }
            return result;
        }
        #endregion

        #region public int BatchSetSortCode(string[] ids) 批量设置排序码
        /// <summary>
        /// 批量设置排序码
        /// </summary>
        /// <param name="ids">主键数组</param>
        /// <returns>影响行数</returns>
        public int BatchSetSortCode(string[] ids)
        {
            var result = 0;
            var managerSequence = new BaseSequenceManager(dbHelper);
            var sortCodes = managerSequence.GetBatchSequence(CurrentTableName, ids.Length);
            for (var i = 0; i < ids.Length; i++)
            {
                result += SetProperty(ids[i], new KeyValuePair<string, object>(BaseUtil.FieldSortCode, sortCodes[i]));
            }
            return result;
        }
        #endregion

        #region public virtual int ResetSortCode() 重新设置表的排序码
        /// <summary>
        /// 重新设置表的排序码
        /// </summary>
        /// <returns>影响行数</returns>
        public virtual int ResetSortCode()
        {
            var result = 0;
            var dt = GetDataTable(0, BaseUtil.FieldSortCode);
            var managerSequence = new BaseSequenceManager(dbHelper);
            var sortCode = managerSequence.GetBatchSequence(CurrentTableName, dt.Rows.Count);
            var i = 0;
            foreach (DataRow dr in dt.Rows)
            {
                result += SetProperty(dr[BaseUtil.FieldId].ToString(), new KeyValuePair<string, object>(BaseUtil.FieldSortCode, sortCode[i]));
                i++;
            }
            return result;
        }
        #endregion

        #region public virtual int ChangeEnabled(object id) 变更有效状态
        /// <summary>
        /// 变更有效状态
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns>影响行数</returns>
        public virtual int ChangeEnabled(object id)
        {
            var result = 0;
            var sb = Pool.StringBuilder.Get();
            sb.Append("UPDATE " + CurrentTableName + " SET " + BaseUtil.FieldEnabled + " = (CASE " + BaseUtil.FieldEnabled + " WHEN 0 THEN 1 WHEN 1 THEN 0 END) WHERE (" + BaseUtil.FieldId + " = " + DbHelper.GetParameter(BaseUtil.FieldId) + ")");
            var names = new string[1];
            var values = new Object[1];
            names[0] = BaseUtil.FieldId;
            values[0] = id;
            result = DbHelper.ExecuteNonQuery(sb.Put(), DbHelper.MakeParameters(names, values));
            return result;
        }
        #endregion

        #region public virtual void SetEntityCreate<T>(SqlBuilder sqlBuilder, T t) 设置创建信息
        /// <summary>
        /// 设置创建信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sqlBuilder"></param>
        /// <param name="t"></param>
        public virtual void SetEntityCreate<T>(SqlBuilder sqlBuilder, T t)
        {
            if (t is BaseEntity entity)
            {
                if (UserInfo != null)
                {
                    entity.CreateUserId = UserInfo.UserId;
                    entity.CreateUserName = UserInfo.UserName;
                    entity.CreateBy = UserInfo.RealName;
                    sqlBuilder.SetValue(BaseEntity.FieldCreateUserId, UserInfo.UserId);
                    sqlBuilder.SetValue(BaseEntity.FieldCreateUserName, UserInfo.UserName);
                    sqlBuilder.SetValue(BaseEntity.FieldCreateBy, UserInfo.RealName);
                }
                else
                {
                    sqlBuilder.SetValue(BaseEntity.FieldCreateBy, entity.CreateBy);
                    sqlBuilder.SetValue(BaseEntity.FieldCreateUserName, entity.CreateUserName);
                }
                // 取数据库时间，还是UTC时间，还是本机时间？
                entity.CreateTime = DateTime.Now;
                entity.CreateIp = Utils.GetIp();
                sqlBuilder.SetDbNow(BaseEntity.FieldCreateTime);
                sqlBuilder.SetValue(BaseEntity.FieldCreateIp, Utils.GetIp());
            }
        }
        #endregion

        #region public virtual void SetEntityUpdate<T>(SqlBuilder sqlBuilder, T t) 设置更新信息
        /// <summary>
        /// 设置更新信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sqlBuilder"></param>
        /// <param name="t"></param>
        public virtual void SetEntityUpdate<T>(SqlBuilder sqlBuilder, T t)
        {
            if (t is BaseEntity entity)
            {
                if (UserInfo != null)
                {
                    entity.UpdateUserId = UserInfo.UserId;
                    entity.UpdateUserName = UserInfo.UserName;
                    entity.UpdateBy = UserInfo.RealName;
                    sqlBuilder.SetValue(BaseEntity.FieldUpdateUserId, UserInfo.UserId);
                    sqlBuilder.SetValue(BaseEntity.FieldUpdateUserName, UserInfo.UserName);
                    sqlBuilder.SetValue(BaseEntity.FieldUpdateBy, UserInfo.RealName);
                }
                else
                {
                    sqlBuilder.SetValue(BaseEntity.FieldUpdateBy, entity.CreateBy);
                    sqlBuilder.SetValue(BaseEntity.FieldUpdateUserName, entity.CreateUserName);
                }
                // 取数据库时间，还是UTC时间，还是本机时间？
                entity.UpdateTime = DateTime.Now;
                entity.UpdateIp = Utils.GetIp();
                sqlBuilder.SetDbNow(BaseEntity.FieldUpdateTime);
                sqlBuilder.SetValue(BaseEntity.FieldUpdateIp, Utils.GetIp());
            }
        }
        #endregion
    }
}