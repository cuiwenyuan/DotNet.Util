//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System.Collections.Generic;

namespace DotNet.Business
{
    using Model;
    using Util;

    /// <summary>
    /// BaseAreaProvinceMarkManager
    /// 地区表(省、市、县)
    ///
    /// 修改记录
    ///
    ///		2015-06-23 版本：1.0 JiRiGaLa 创建主键。
    ///
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2015-06-23</date>
    /// </author>
    /// </summary>
    public partial class BaseAreaProvinceMarkManager : BaseManager, IBaseManager
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public BaseAreaProvinceMarkManager()
        {
            if (dbHelper == null)
            {
                dbHelper = DbHelperFactory.GetHelper(BaseSystemInfo.UserCenterDbType, BaseSystemInfo.UserCenterDbConnection);
            }
            if (string.IsNullOrEmpty(CurrentTableName))
            {
                CurrentTableName = BaseAreaProvinceMarkEntity.TableName;
            }
        }

        /// <summary>
        /// 构造函数
        /// <param name="tableName">指定表名</param>
        /// </summary>
        public BaseAreaProvinceMarkManager(string tableName) : this()
        {
            CurrentTableName = tableName;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        public BaseAreaProvinceMarkManager(IDbHelper dbHelper)
            : this()
        {
            DbHelper = dbHelper;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        public BaseAreaProvinceMarkManager(BaseUserInfo userInfo)
            : this()
        {
            UserInfo = userInfo;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        /// <param name="userInfo">用户信息</param>
        public BaseAreaProvinceMarkManager(IDbHelper dbHelper, BaseUserInfo userInfo)
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
        public BaseAreaProvinceMarkManager(IDbHelper dbHelper, BaseUserInfo userInfo, string tableName)
            : this(dbHelper, userInfo)
        {
            CurrentTableName = tableName;
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns>主键</returns>
        public string Add(BaseAreaProvinceMarkEntity entity)
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
        public string Add(BaseAreaProvinceMarkEntity entity, bool identity, bool returnId)
        {
            Identity = identity;
            ReturnId = returnId;
            return AddEntity(entity);
        }

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="id">主键</param>
        public BaseAreaProvinceMarkEntity GetEntity(int? id)
        {
            return GetEntity(id.ToString());
        }

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="id">主键</param>
        public BaseAreaProvinceMarkEntity GetEntity(string id)
        {
            return BaseEntity.Create<BaseAreaProvinceMarkEntity>(ExecuteReader(new KeyValuePair<string, object>(BaseAreaProvinceMarkEntity.FieldId, id)));
            // return BaseEntity.Create<BaseAreaProvinceMarkEntity>(this.GetDataTable(new KeyValuePair<string, object>(BaseAreaProvinceMarkEntity.FieldId, id)));
        }

        /// <summary>
        /// 添加实体
        /// </summary>
        /// <param name="entity">实体</param>
        public string AddEntity(BaseAreaProvinceMarkEntity entity)
        {
            var sequence = string.Empty;

            var sqlBuilder = new SqlBuilder(DbHelper, Identity, ReturnId);
            sqlBuilder.BeginInsert(CurrentTableName, BaseAreaProvinceMarkEntity.FieldId);
            if (!string.IsNullOrEmpty(entity.Id) || !Identity)
            {
                sqlBuilder.SetValue(BaseAreaProvinceMarkEntity.FieldId, entity.Id);
            }
            else
            {
                if (!ReturnId && (DbHelper.CurrentDbType == CurrentDbType.Oracle || DbHelper.CurrentDbType == CurrentDbType.Db2))
                {
                    if (DbHelper.CurrentDbType == CurrentDbType.Oracle)
                    {
                        sqlBuilder.SetFormula(BaseAreaProvinceMarkEntity.FieldId, "SEQ_" + BaseAreaEntity.TableName + ".NEXTVAL ");
                    }
                    if (DbHelper.CurrentDbType == CurrentDbType.Db2)
                    {
                        sqlBuilder.SetFormula(BaseAreaProvinceMarkEntity.FieldId, "NEXT VALUE FOR SEQ_" + BaseAreaEntity.TableName.ToUpper());
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
            SetEntity(sqlBuilder, entity);

            // 创建人信息
            if (!string.IsNullOrEmpty(entity.CreateUserId))
            {
                sqlBuilder.SetValue(BaseAreaProvinceMarkEntity.FieldCreateUserId, entity.CreateUserId);
            }
            else
            {
                if (UserInfo != null)
                {
                    sqlBuilder.SetValue(BaseAreaProvinceMarkEntity.FieldCreateUserId, UserInfo.Id);
                }
            }
            if (!string.IsNullOrEmpty(entity.CreateBy))
            {
                sqlBuilder.SetValue(BaseAreaProvinceMarkEntity.FieldCreateBy, entity.CreateBy);
            }
            else
            {
                if (UserInfo != null)
                {
                    sqlBuilder.SetValue(BaseAreaProvinceMarkEntity.FieldCreateBy, UserInfo.RealName);
                }
            }
            if (entity.CreateOn.HasValue)
            {
                sqlBuilder.SetValue(BaseAreaProvinceMarkEntity.FieldCreateTime, entity.CreateOn);
            }
            else
            {
                sqlBuilder.SetDbNow(BaseAreaProvinceMarkEntity.FieldCreateTime);
            }
            
            // 修改人信息
            if (!string.IsNullOrEmpty(entity.ModifiedUserId))
            {
                sqlBuilder.SetValue(BaseAreaProvinceMarkEntity.FieldUpdateUserId, entity.ModifiedUserId);
            }
            else
            {
                if (UserInfo != null)
                {
                    sqlBuilder.SetValue(BaseAreaProvinceMarkEntity.FieldUpdateUserId, UserInfo.Id);
                }
            }
            if (!string.IsNullOrEmpty(entity.ModifiedBy))
            {
                sqlBuilder.SetValue(BaseAreaProvinceMarkEntity.FieldUpdateBy, entity.ModifiedBy);
            }
            else
            {
                if (UserInfo != null)
                {
                    sqlBuilder.SetValue(BaseAreaProvinceMarkEntity.FieldUpdateBy, UserInfo.RealName);
                }
            }
            if (entity.ModifiedOn.HasValue)
            {
                sqlBuilder.SetValue(BaseAreaProvinceMarkEntity.FieldUpdateTime, entity.ModifiedOn);
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
        public int UpdateEntity(BaseAreaProvinceMarkEntity entity)
        {
            var sqlBuilder = new SqlBuilder(DbHelper);
            sqlBuilder.BeginUpdate(CurrentTableName);
            SetEntity(sqlBuilder, entity);
            if (UserInfo != null)
            {
                sqlBuilder.SetValue(BaseAreaProvinceMarkEntity.FieldUpdateUserId, UserInfo.Id);
                sqlBuilder.SetValue(BaseAreaProvinceMarkEntity.FieldUpdateBy, UserInfo.RealName);
            }
            // 若有修改时间标示，那就按修改时间来，不是按最新的时间来
            if (entity.ModifiedOn.HasValue)
            {
                sqlBuilder.SetValue(BaseAreaProvinceMarkEntity.FieldUpdateTime, entity.ModifiedOn.Value);
            }
            else
            {
                sqlBuilder.SetDbNow(BaseAreaProvinceMarkEntity.FieldUpdateTime);
            }
            sqlBuilder.SetWhere(BaseAreaProvinceMarkEntity.FieldId, entity.Id);
            return sqlBuilder.EndUpdate();
        }

        partial void SetEntityExtend(SqlBuilder sqlBuilder, BaseAreaProvinceMarkEntity entity);

        /// <summary>
        /// 设置实体
        /// </summary>
        /// <param name="sqlBuilder">SQL语句生成器</param>
        /// <param name="entity">实体</param>
        private void SetEntity(SqlBuilder sqlBuilder, BaseAreaProvinceMarkEntity entity)
        {
            sqlBuilder.SetValue(BaseAreaProvinceMarkEntity.FieldArea, entity.Area);
            sqlBuilder.SetValue(BaseAreaProvinceMarkEntity.FieldAreaId, entity.AreaId);
            sqlBuilder.SetValue(BaseAreaProvinceMarkEntity.FieldProvince, entity.Province);
            sqlBuilder.SetValue(BaseAreaProvinceMarkEntity.FieldProvinceId, entity.ProvinceId);
            sqlBuilder.SetValue(BaseAreaProvinceMarkEntity.FieldMark, entity.Mark);
            sqlBuilder.SetValue(BaseAreaProvinceMarkEntity.FieldEnabled, entity.Enabled);
            sqlBuilder.SetValue(BaseAreaProvinceMarkEntity.FieldDescription, entity.Description);
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
