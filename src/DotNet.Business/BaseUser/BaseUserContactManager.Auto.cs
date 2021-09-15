//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System.Collections.Generic;

namespace DotNet.Business
{
    using Model;
    using Util;

    /// <summary>
    /// BaseUserContactManager
    /// 系统用户联系方式表
    ///
    /// 修改记录
    ///
    ///		2014-01-13 版本：1.0 JiRiGaLa 创建主键。
    ///
    /// <author>
    ///		<name>JiRiGaLa</name>
    ///		<date>2014-01-13</date>
    /// </author>
    /// </summary>
    public partial class BaseUserContactManager : BaseManager, IBaseManager
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public BaseUserContactManager()
        {
            if (dbHelper == null)
            {
                dbHelper = DbHelperFactory.GetHelper(BaseSystemInfo.UserCenterDbType, BaseSystemInfo.UserCenterDbConnection);
            }
            if (string.IsNullOrEmpty(CurrentTableName))
            {
                CurrentTableName = BaseUserContactEntity.TableName;
            }
        }

        /// <summary>
        /// 构造函数
        /// <param name="tableName">指定表名</param>
        /// </summary>
        public BaseUserContactManager(string tableName): this()
        {
            CurrentTableName = tableName;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        public BaseUserContactManager(IDbHelper dbHelper)
            : this()
        {
            DbHelper = dbHelper;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        public BaseUserContactManager(BaseUserInfo userInfo)
            : this()
        {
            UserInfo = userInfo;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        /// <param name="userInfo">用户信息</param>
        public BaseUserContactManager(IDbHelper dbHelper, BaseUserInfo userInfo)
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
        public BaseUserContactManager(IDbHelper dbHelper, BaseUserInfo userInfo, string tableName)
            : this(dbHelper, userInfo)
        {
            CurrentTableName = tableName;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        /// <param name="tableName">指定表名</param>
        public BaseUserContactManager(BaseUserInfo userInfo, string tableName)
            : this(userInfo)
        {
            CurrentTableName = tableName;
        }

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="id">主键</param>
        public BaseUserContactEntity GetEntity(int? id)
        {
            return BaseEntity.Create<BaseUserContactEntity>(ExecuteReader(new KeyValuePair<string, object>(BaseUserContactEntity.FieldId, id)));
            // return BaseEntity.Create<BaseUserContactEntity>(this.GetDataTable(new KeyValuePair<string, object>(BaseUserContactEntity.FieldId, id)));
        }

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="id">主键</param>
        public BaseUserContactEntity GetEntity(string id)
        {
            return BaseEntity.Create<BaseUserContactEntity>(ExecuteReader(new KeyValuePair<string, object>(BaseUserContactEntity.FieldId, id)));
            // return BaseEntity.Create<BaseUserContactEntity>(this.GetDataTable(new KeyValuePair<string, object>(BaseUserContactEntity.FieldId, id)));
        }

        /// <summary>
        /// 添加实体
        /// </summary>
        /// <param name="entity">实体</param>
        public string AddEntity(BaseUserContactEntity entity)
        {
            var sqlBuilder = new SqlBuilder(DbHelper, Identity, ReturnId);
            sqlBuilder.BeginInsert(CurrentTableName, BaseUserContactEntity.FieldId);
            sqlBuilder.SetValue(BaseUserContactEntity.FieldId, entity.Id);
            SetEntity(sqlBuilder, entity);
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
            sqlBuilder.EndInsert();
            return entity.Id;
        }

        /// <summary>
        /// 更新实体
        /// </summary>
        /// <param name="entity">实体</param>
        public int UpdateEntity(BaseUserContactEntity entity)
        {
            var sqlBuilder = new SqlBuilder(DbHelper);
            sqlBuilder.BeginUpdate(CurrentTableName);
            SetEntity(sqlBuilder, entity);
            if (UserInfo != null)
            {
                sqlBuilder.SetValue(BaseUserEntity.FieldUpdateUserId, UserInfo.Id);
                sqlBuilder.SetValue(BaseUserEntity.FieldUpdateBy, UserInfo.RealName);
            }
            sqlBuilder.SetDbNow(BaseUserEntity.FieldUpdateTime);
            sqlBuilder.SetWhere(BaseUserEntity.FieldId, entity.Id);
            return sqlBuilder.EndUpdate();
        }

        /// <summary>
        /// 设置实体
        /// </summary>
        /// <param name="sqlBuilder">SQL语句生成器</param>
        /// <param name="entity">实体</param>
        private void SetEntity(SqlBuilder sqlBuilder, BaseUserContactEntity entity)
        {
            // 2016-03-02 吉日嘎拉 增加按公司可以区别数据的功能。
            sqlBuilder.SetValue(BaseUserContactEntity.FieldCompanyId, entity.CompanyId);
            sqlBuilder.SetValue(BaseUserContactEntity.FieldExtension, entity.Extension);
            sqlBuilder.SetValue(BaseUserContactEntity.FieldMobile, entity.Mobile);
            sqlBuilder.SetValue(BaseUserContactEntity.FieldShowMobile, entity.ShowMobile);
            sqlBuilder.SetValue(BaseUserContactEntity.FieldMobileValiated, entity.MobileValiated);
            sqlBuilder.SetValue(BaseUserContactEntity.FieldMobileVerificationDate, entity.MobileVerificationDate);
            sqlBuilder.SetValue(BaseUserContactEntity.FieldTelephone, entity.Telephone);
            sqlBuilder.SetValue(BaseUserContactEntity.FieldShortNumber, entity.ShortNumber);
            sqlBuilder.SetValue(BaseUserContactEntity.FieldWw, entity.Ww);
            sqlBuilder.SetValue(BaseUserContactEntity.FieldQq, entity.Qq);
            sqlBuilder.SetValue(BaseUserContactEntity.FieldWeChat, entity.WeChat);
            sqlBuilder.SetValue(BaseUserContactEntity.FieldWeChatOpenId, entity.WeChatOpenId);
            sqlBuilder.SetValue(BaseUserContactEntity.FieldWeChatValiated, entity.WeChatValiated);
            sqlBuilder.SetValue(BaseUserContactEntity.FieldYy, entity.Yy);
            sqlBuilder.SetValue(BaseUserContactEntity.FieldEmergencyContact, entity.EmergencyContact);
            sqlBuilder.SetValue(BaseUserContactEntity.FieldYiXin, entity.YiXin);
            sqlBuilder.SetValue(BaseUserContactEntity.FieldYiXinValiated, entity.YiXinValiated);
            sqlBuilder.SetValue(BaseUserContactEntity.FieldCompanyMail, entity.CompanyMail);
            sqlBuilder.SetValue(BaseUserContactEntity.FieldEmail, entity.Email);
            sqlBuilder.SetValue(BaseUserContactEntity.FieldEmailValiated, entity.EmailValiated);
        }
    }
}
