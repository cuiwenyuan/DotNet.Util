//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System.Collections.Generic;

namespace DotNet.Business
{
    using Model;
    using Util;

    /// <summary>
    /// BaseUserAddressManager
    /// 用户送货地址表
    ///
    /// 修改记录
    ///
    ///		2010-07-15 版本：1.0 JiRiGaLa 创建主键。
    ///
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2010-07-15</date>
    /// </author>
    /// </summary>
    public partial class BaseUserAddressManager : BaseManager, IBaseManager
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public BaseUserAddressManager()
        {
            if (dbHelper == null)
            {
                dbHelper = DbHelperFactory.GetHelper(BaseSystemInfo.UserCenterDbType, BaseSystemInfo.UserCenterDbConnection);
            }
            CurrentTableName = BaseUserAddressEntity.TableName;
        }

        /// <summary>
        /// 构造函数
        /// <param name="tableName">指定表名</param>
        /// </summary>
        public BaseUserAddressManager(string tableName)
        {
            CurrentTableName = tableName;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        public BaseUserAddressManager(IDbHelper dbHelper)
            : this()
        {
            DbHelper = dbHelper;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        public BaseUserAddressManager(BaseUserInfo userInfo)
            : this()
        {
            UserInfo = userInfo;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        /// <param name="userInfo">用户信息</param>
        public BaseUserAddressManager(IDbHelper dbHelper, BaseUserInfo userInfo)
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
        public BaseUserAddressManager(IDbHelper dbHelper, BaseUserInfo userInfo, string tableName)
            : this(dbHelper, userInfo)
        {
            CurrentTableName = tableName;
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns>主键</returns>
        public string Add(BaseUserAddressEntity entity)
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
        public string Add(BaseUserAddressEntity entity, bool identity, bool returnId)
        {
            Identity = identity;
            ReturnId = returnId;
            return AddEntity(entity);
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="entity">实体</param>
        public int Update(BaseUserAddressEntity entity)
        {
            return UpdateEntity(entity);
        }

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="id">主键</param>
        public BaseUserAddressEntity GetEntity(string id)
        {
            return BaseEntity.Create<BaseUserAddressEntity>(ExecuteReader(new KeyValuePair<string, object>(BaseUserAddressEntity.FieldId, id)));
            // return BaseEntity.Create<BaseUserAddressEntity>(this.GetDataTable(new KeyValuePair<string, object>(BaseUserAddressEntity.FieldId, id)));
        }

        /// <summary>
        /// 添加实体
        /// </summary>
        /// <param name="entity">实体</param>
        public string AddEntity(BaseUserAddressEntity entity)
        {
            var sequence = entity.Id;
            if (entity.SortCode == 0)
            {
                var managerSequence = new BaseSequenceManager(DbHelper, Identity);
                sequence = managerSequence.Increment(CurrentTableName);
                entity.SortCode = int.Parse(sequence);
            }
            var sqlBuilder = new SqlBuilder(DbHelper, Identity, ReturnId);
            sqlBuilder.BeginInsert(CurrentTableName, BaseUserAddressEntity.FieldId);
            if (entity.Id is string)
            {
                Identity = false;
            }
            if (!Identity)
            {
                sqlBuilder.SetValue(BaseUserAddressEntity.FieldId, entity.Id);
            }
            else
            {
                if (!ReturnId && (DbHelper.CurrentDbType == CurrentDbType.Oracle || DbHelper.CurrentDbType == CurrentDbType.Db2))
                {
                    if (DbHelper.CurrentDbType == CurrentDbType.Oracle)
                    {
                        sqlBuilder.SetFormula(BaseUserAddressEntity.FieldId, "SEQ_" + CurrentTableName.ToUpper() + ".NEXTVAL ");
                    }
                    if (DbHelper.CurrentDbType == CurrentDbType.Db2)
                    {
                        sqlBuilder.SetFormula(BaseUserAddressEntity.FieldId, "NEXT VALUE FOR SEQ_" + CurrentTableName.ToUpper());
                    }
                }
                else
                {
                    if (Identity && (DbHelper.CurrentDbType == CurrentDbType.Oracle || DbHelper.CurrentDbType == CurrentDbType.Db2))
                    {
                        if (string.IsNullOrEmpty(entity.Id))
                        {
                            if (string.IsNullOrEmpty(sequence))
                            {
                                var managerSequence = new BaseSequenceManager(DbHelper, Identity);
                                sequence = managerSequence.Increment(CurrentTableName);
                            }
                            entity.Id = sequence;
                        }
                        sqlBuilder.SetValue(BaseUserAddressEntity.FieldId, entity.Id);
                    }
                }
            }
            SetEntity(sqlBuilder, entity);
            if (UserInfo != null)
            {
                sqlBuilder.SetValue(BaseUserAddressEntity.FieldCreateUserId, UserInfo.Id);
                sqlBuilder.SetValue(BaseUserAddressEntity.FieldCreateBy, UserInfo.RealName);
            }
            sqlBuilder.SetDbNow(BaseUserAddressEntity.FieldCreateTime);
            if (UserInfo != null)
            {
                sqlBuilder.SetValue(BaseUserAddressEntity.FieldUpdateUserId, UserInfo.Id);
                sqlBuilder.SetValue(BaseUserAddressEntity.FieldUpdateBy, UserInfo.RealName);
            }
            sqlBuilder.SetDbNow(BaseUserAddressEntity.FieldUpdateTime);
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
        public int UpdateEntity(BaseUserAddressEntity entity)
        {
            var sqlBuilder = new SqlBuilder(DbHelper);
            sqlBuilder.BeginUpdate(CurrentTableName);
            SetEntity(sqlBuilder, entity);
            if (UserInfo != null)
            {
                sqlBuilder.SetValue(BaseUserAddressEntity.FieldUpdateUserId, UserInfo.Id);
                sqlBuilder.SetValue(BaseUserAddressEntity.FieldUpdateBy, UserInfo.RealName);
            }
            sqlBuilder.SetDbNow(BaseUserAddressEntity.FieldUpdateTime);
            sqlBuilder.SetWhere(BaseUserAddressEntity.FieldId, entity.Id);
            return sqlBuilder.EndUpdate();
        }

        partial void SetEntityExtend(SqlBuilder sqlBuilder, BaseUserAddressEntity entity);

        /// <summary>
        /// 设置实体
        /// </summary>
        /// <param name="sqlBuilder">SQL语句生成器</param>
        /// <param name="entity">实体</param>
        private void SetEntity(SqlBuilder sqlBuilder, BaseUserAddressEntity entity)
        {
            sqlBuilder.SetValue(BaseUserAddressEntity.FieldUserId, entity.UserId);
            sqlBuilder.SetValue(BaseUserAddressEntity.FieldRealName, entity.RealName);
            sqlBuilder.SetValue(BaseUserAddressEntity.FieldProvinceId, entity.ProvinceId);
            sqlBuilder.SetValue(BaseUserAddressEntity.FieldProvince, entity.Province);
            sqlBuilder.SetValue(BaseUserAddressEntity.FieldCityId, entity.CityId);
            sqlBuilder.SetValue(BaseUserAddressEntity.FieldCity, entity.City);
            sqlBuilder.SetValue(BaseUserAddressEntity.FieldAreaId, entity.AreaId);
            sqlBuilder.SetValue(BaseUserAddressEntity.FieldArea, entity.Area);
            sqlBuilder.SetValue(BaseUserAddressEntity.FieldAddress, entity.Address);
            sqlBuilder.SetValue(BaseUserAddressEntity.FieldPostCode, entity.PostCode);
            sqlBuilder.SetValue(BaseUserAddressEntity.FieldPhone, entity.Phone);
            sqlBuilder.SetValue(BaseUserAddressEntity.FieldFax, entity.Fax);
            sqlBuilder.SetValue(BaseUserAddressEntity.FieldMobile, entity.Mobile);
            sqlBuilder.SetValue(BaseUserAddressEntity.FieldEmail, entity.Email);

            sqlBuilder.SetValue(BaseUserAddressEntity.FieldDeliverCategory, entity.DeliverCategory);
            sqlBuilder.SetValue(BaseUserAddressEntity.FieldSortCode, entity.SortCode);
            sqlBuilder.SetValue(BaseUserAddressEntity.FieldDeleted, entity.DeletionStateCode);
            sqlBuilder.SetValue(BaseUserAddressEntity.FieldEnabled, entity.Enabled);
            sqlBuilder.SetValue(BaseUserAddressEntity.FieldDescription, entity.Description);
            SetEntityExtend(sqlBuilder, entity);
        }

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns>影响行数</returns>
        public int Delete(string id)
        {
            return Delete(new List<KeyValuePair<string, object>> { new KeyValuePair<string, object>(PrimaryKey, id) });
        }
    }
}
