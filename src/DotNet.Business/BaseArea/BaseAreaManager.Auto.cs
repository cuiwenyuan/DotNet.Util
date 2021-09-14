//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System.Collections.Generic;

namespace DotNet.Business
{
    using Model;
    using Util;

    /// <summary>
    /// BaseAreaManager
    /// 地区表(省、市、县)
    ///
    /// 修改记录
    ///
    ///		2014-02-11 版本：1.0 JiRiGaLa 创建主键。
    ///
    /// <author>
    ///		<name>JiRiGaLa</name>
    ///		<date>2014-02-11</date>
    /// </author>
    /// </summary>
    public partial class BaseAreaManager : BaseManager, IBaseManager
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public BaseAreaManager()
        {
            if (dbHelper == null)
            {
                dbHelper = DbHelperFactory.GetHelper(BaseSystemInfo.UserCenterDbType, BaseSystemInfo.UserCenterDbConnection);
            }
            if (string.IsNullOrEmpty(CurrentTableName))
            {
                CurrentTableName = BaseAreaEntity.TableName;
            }
        }

        /// <summary>
        /// 构造函数
        /// <param name="tableName">指定表名</param>
        /// </summary>
        public BaseAreaManager(string tableName) : this()
        {
            CurrentTableName = tableName;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        public BaseAreaManager(IDbHelper dbHelper)
            : this()
        {
            DbHelper = dbHelper;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        public BaseAreaManager(BaseUserInfo userInfo)
            : this()
        {
            UserInfo = userInfo;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        /// <param name="userInfo">用户信息</param>
        public BaseAreaManager(IDbHelper dbHelper, BaseUserInfo userInfo)
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
        public BaseAreaManager(IDbHelper dbHelper, BaseUserInfo userInfo, string tableName)
            : this(dbHelper, userInfo)
        {
            CurrentTableName = tableName;
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns>主键</returns>
        public string Add(BaseAreaEntity entity)
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
        public string Add(BaseAreaEntity entity, bool identity, bool returnId)
        {
            Identity = identity;
            ReturnId = returnId;
            return AddObject(entity);
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="entity">实体</param>
        public int Update(BaseAreaEntity entity)
        {
            return UpdateObject(entity);
        }

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="id">主键</param>
        public BaseAreaEntity GetObject(int? id)
        {
            return GetObject(id.ToString());
        }

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="id">主键</param>
        public BaseAreaEntity GetObject(string id)
        {
            return BaseEntity.Create<BaseAreaEntity>(ExecuteReader(new KeyValuePair<string, object>(BaseAreaEntity.FieldId, id)));
			// return BaseEntity.Create<BaseAreaEntity>(this.GetDataTable(new KeyValuePair<string, object>(BaseAreaEntity.FieldId, id)));
        }

        /// <summary>
        /// 添加实体
        /// </summary>
        /// <param name="entity">实体</param>
        public string AddObject(BaseAreaEntity entity)
        {
            var sequence = string.Empty;
            if (!entity.SortCode.HasValue)
            {
                var managerSequence = new BaseSequenceManager(DbHelper, Identity);
                sequence = managerSequence.Increment(CurrentTableName);
                entity.SortCode = int.Parse(sequence);
            }
            var sqlBuilder = new SqlBuilder(DbHelper, Identity, ReturnId);
            sqlBuilder.BeginInsert(CurrentTableName, BaseAreaEntity.FieldId);
            if (!string.IsNullOrEmpty(entity.Id) || !Identity)
            {
                sqlBuilder.SetValue(BaseAreaEntity.FieldId, entity.Id);
            }
            else
            {
                if (!ReturnId && (DbHelper.CurrentDbType == CurrentDbType.Oracle || DbHelper.CurrentDbType == CurrentDbType.Db2))
                {
                    if (DbHelper.CurrentDbType == CurrentDbType.Oracle)
                    {
                        sqlBuilder.SetFormula(BaseAreaEntity.FieldId, "SEQ_" + CurrentTableName.ToUpper() + ".NEXTVAL ");
                    }
                    if (DbHelper.CurrentDbType == CurrentDbType.Db2)
                    {
                        sqlBuilder.SetFormula(BaseAreaEntity.FieldId, "NEXT VALUE FOR SEQ_" + CurrentTableName.ToUpper());
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
                            entity.Id = sequence;
                        }
                        sqlBuilder.SetValue(BaseAreaEntity.FieldId, entity.Id);
                    }
                }
            }
            SetObject(sqlBuilder, entity);

            // 创建人信息
            if (!string.IsNullOrEmpty(entity.CreateUserId))
            {
                sqlBuilder.SetValue(BaseAreaEntity.FieldCreateUserId, entity.CreateUserId);
            }
            else
            {
                if (UserInfo != null)
                {
                    sqlBuilder.SetValue(BaseAreaEntity.FieldCreateUserId, UserInfo.Id);
                }
            }
            if (!string.IsNullOrEmpty(entity.CreateBy))
            {
                sqlBuilder.SetValue(BaseAreaEntity.FieldCreateBy, entity.CreateBy);
            }
            else
            {
                if (UserInfo != null)
                {
                    sqlBuilder.SetValue(BaseAreaEntity.FieldCreateBy, UserInfo.RealName);
                }
            }
            if (entity.CreateOn.HasValue)
            {
                sqlBuilder.SetValue(BaseAreaEntity.FieldCreateTime, entity.CreateOn);
            }
            else
            {
                sqlBuilder.SetDbNow(BaseAreaEntity.FieldCreateTime);
            }
            
            // 修改人信息
            if (!string.IsNullOrEmpty(entity.ModifiedUserId))
            {
                sqlBuilder.SetValue(BaseAreaEntity.FieldUpdateUserId, entity.ModifiedUserId);
            }
            else
            {
                if (UserInfo != null)
                {
                    sqlBuilder.SetValue(BaseAreaEntity.FieldUpdateUserId, UserInfo.Id);
                }
            }
            if (!string.IsNullOrEmpty(entity.ModifiedBy))
            {
                sqlBuilder.SetValue(BaseAreaEntity.FieldUpdateBy, entity.ModifiedBy);
            }
            else
            {
                if (UserInfo != null)
                {
                    sqlBuilder.SetValue(BaseAreaEntity.FieldUpdateBy, UserInfo.RealName);
                }
            }
            if (entity.ModifiedOn.HasValue)
            {
                sqlBuilder.SetValue(BaseAreaEntity.FieldUpdateTime, entity.ModifiedOn);
            }
            else
            {
                sqlBuilder.SetDbNow(BaseAreaEntity.FieldUpdateTime);
            }

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
        public int UpdateObject(BaseAreaEntity entity)
        {
            var result = 0;
            var sqlBuilder = new SqlBuilder(DbHelper);
            sqlBuilder.BeginUpdate(CurrentTableName);
            SetObject(sqlBuilder, entity);
            if (UserInfo != null)
            {
                sqlBuilder.SetValue(BaseAreaEntity.FieldUpdateUserId, UserInfo.Id);
                sqlBuilder.SetValue(BaseAreaEntity.FieldUpdateBy, UserInfo.RealName);
            }
            // 若有修改时间标示，那就按修改时间来，不是按最新的时间来
            if (entity.ModifiedOn.HasValue)
            {
                sqlBuilder.SetValue(BaseAreaEntity.FieldUpdateTime, entity.ModifiedOn.Value);
            }
            else
            {
                sqlBuilder.SetDbNow(BaseAreaEntity.FieldUpdateTime);
            }
            sqlBuilder.SetWhere(BaseAreaEntity.FieldId, entity.Id);
            result = sqlBuilder.EndUpdate();

            return result;
        }

        partial void SetObjectExpand(SqlBuilder sqlBuilder, BaseAreaEntity entity);

        /// <summary>
        /// 设置实体
        /// </summary>
        /// <param name="sqlBuilder">SQL语句生成器</param>
        /// <param name="entity">实体</param>
        private void SetObject(SqlBuilder sqlBuilder, BaseAreaEntity entity)
        {   
            sqlBuilder.SetValue(BaseAreaEntity.FieldParentId, entity.ParentId);
            sqlBuilder.SetValue(BaseAreaEntity.FieldCode, entity.Code);
            sqlBuilder.SetValue(BaseAreaEntity.FieldFullName, entity.FullName);
            sqlBuilder.SetValue(BaseAreaEntity.FieldShortName, entity.ShortName);
            sqlBuilder.SetValue(BaseAreaEntity.FieldPostalcode, entity.Postalcode);
            sqlBuilder.SetValue(BaseAreaEntity.FieldQuickQuery, entity.QuickQuery);
            sqlBuilder.SetValue(BaseAreaEntity.FieldSimpleSpelling, entity.SimpleSpelling);
            sqlBuilder.SetValue(BaseAreaEntity.FieldProvince, entity.Province);
            sqlBuilder.SetValue(BaseAreaEntity.FieldCity, entity.City);
            sqlBuilder.SetValue(BaseAreaEntity.FieldDistrict, entity.District);
            sqlBuilder.SetValue(BaseAreaEntity.FieldLongitude, entity.Longitude);
            sqlBuilder.SetValue(BaseAreaEntity.FieldLatitude, entity.Latitude);
            sqlBuilder.SetValue(BaseAreaEntity.FieldManageCompanyId, entity.ManageCompanyId);
            sqlBuilder.SetValue(BaseAreaEntity.FieldManageCompanyCode, entity.ManageCompanyCode);
            sqlBuilder.SetValue(BaseAreaEntity.FieldManageCompany, entity.ManageCompany);
            sqlBuilder.SetValue(BaseAreaEntity.FieldNetworkOrders, entity.NetworkOrders);
            sqlBuilder.SetValue(BaseAreaEntity.FieldDelayDay, entity.DelayDay);
            sqlBuilder.SetValue(BaseAreaEntity.FieldOpening, entity.Opening);
            sqlBuilder.SetValue(BaseAreaEntity.FieldWhole, entity.Whole);
            sqlBuilder.SetValue(BaseAreaEntity.FieldReceive, entity.Receive);
            sqlBuilder.SetValue(BaseAreaEntity.FieldSend, entity.Send);
            sqlBuilder.SetValue(BaseAreaEntity.FieldLayer, entity.Layer);
            sqlBuilder.SetValue(BaseAreaEntity.FieldAllowToPay, entity.AllowToPay);
            sqlBuilder.SetValue(BaseAreaEntity.FieldMaxToPayment, entity.MaxToPayment);
            sqlBuilder.SetValue(BaseAreaEntity.FieldAllowGoodsPay, entity.AllowGoodsPay);
            sqlBuilder.SetValue(BaseAreaEntity.FieldMaxGoodsPayment, entity.MaxGoodsPayment);
            sqlBuilder.SetValue(BaseAreaEntity.FieldMark, entity.Mark);
            sqlBuilder.SetValue(BaseAreaEntity.FieldPrintMark, entity.PrintMark);
            sqlBuilder.SetValue(BaseAreaEntity.FieldOutOfRange, entity.OutOfRange);
            sqlBuilder.SetValue(BaseAreaEntity.FieldStatistics, entity.Statistics);
            sqlBuilder.SetValue(BaseAreaEntity.FieldEnabled, entity.Enabled);
            sqlBuilder.SetValue(BaseAreaEntity.FieldDeleted, entity.DeletionStateCode);
            sqlBuilder.SetValue(BaseAreaEntity.FieldSortCode, entity.SortCode);
            sqlBuilder.SetValue(BaseAreaEntity.FieldDescription, entity.Description);
            SetObjectExpand(sqlBuilder, entity);
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
