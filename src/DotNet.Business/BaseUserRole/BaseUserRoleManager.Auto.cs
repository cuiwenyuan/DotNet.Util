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
    /// BaseUserRoleManager
    /// 用户-角色 关系
    ///
    /// 修改记录
    ///
    ///     2018-09-07 版本：4.1 Troy.Cui   增加删除缓存功能。
    ///		2016-03-02 版本：2.0 JiRiGaLa 角色用户关联关系、方便下拉属于自己公司的数据。
    ///		2010-07-15 版本：1.0 JiRiGaLa 创建主键。
    ///
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2016-03-02</date>
    /// </author>
    /// </summary>
    public partial class BaseUserRoleManager : BaseManager, IBaseManager
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public BaseUserRoleManager()
        {
            if (dbHelper == null)
            {
                dbHelper = DbHelperFactory.GetHelper(BaseSystemInfo.UserCenterDbType, BaseSystemInfo.UserCenterDbConnection);
            }
            CurrentTableName = BaseUserRoleEntity.TableName;
        }

        /// <summary>
        /// 构造函数
        /// <param name="tableName">指定表名</param>
        /// </summary>
        public BaseUserRoleManager(string tableName)
        {
            CurrentTableName = tableName;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        public BaseUserRoleManager(IDbHelper dbHelper)
            : this()
        {
            DbHelper = dbHelper;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        public BaseUserRoleManager(BaseUserInfo userInfo)
            : this()
        {
            UserInfo = userInfo;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        /// <param name="userInfo">用户信息</param>
        public BaseUserRoleManager(IDbHelper dbHelper, BaseUserInfo userInfo)
            : this(dbHelper)
        {
            UserInfo = userInfo;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        /// <param name="tableName">指定表名</param>
        public BaseUserRoleManager(BaseUserInfo userInfo, string tableName)
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
        public BaseUserRoleManager(IDbHelper dbHelper, BaseUserInfo userInfo, string tableName)
            : this(dbHelper, userInfo)
        {
            CurrentTableName = tableName;
        }

        /// <summary>
        /// 添加(判断数据是否重复，防止垃圾数据产生)
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns>主键</returns>
        public string Add(BaseUserRoleEntity entity)
        {
            var result = string.Empty;

            // 判断是否数据重复
            var whereParameters = new List<KeyValuePair<string, object>>
            {
                // parameters.Add(new KeyValuePair<string, object>(BaseUserRoleEntity.FieldEnabled, 1));
                new KeyValuePair<string, object>(BaseUserRoleEntity.FieldDeleted, 0),
                new KeyValuePair<string, object>(BaseUserRoleEntity.FieldRoleId, entity.RoleId),
                new KeyValuePair<string, object>(BaseUserRoleEntity.FieldUserId, entity.UserId)
            };
            if (!Exists(whereParameters))
            {
                result = AddEntity(entity);
            }
            else
            {
                // 2015-12-04 吉日嘎拉 这里有严重错误，重复申请就会变成自己审核了
                var parameters = new List<KeyValuePair<string, object>>();
                if (UserInfo != null)
                {
                    parameters.Add(new KeyValuePair<string, object>(BaseUserRoleEntity.FieldUpdateUserId, UserInfo.Id));
                    parameters.Add(new KeyValuePair<string, object>(BaseUserRoleEntity.FieldUpdateBy, UserInfo.RealName));
                    parameters.Add(new KeyValuePair<string, object>(BaseUserRoleEntity.FieldUpdateTime, DateTime.Now));
                }
                else
                {
                    parameters.Add(new KeyValuePair<string, object>(BaseUserRoleEntity.FieldUpdateUserId, entity.ModifiedUserId));
                    parameters.Add(new KeyValuePair<string, object>(BaseUserRoleEntity.FieldUpdateBy, entity.ModifiedBy));
                    parameters.Add(new KeyValuePair<string, object>(BaseUserRoleEntity.FieldUpdateTime, DateTime.Now));
                }
                parameters.Add(new KeyValuePair<string, object>(BaseUserRoleEntity.FieldEnabled, entity.Enabled));
                SetProperty(whereParameters, parameters);
            }

            return result;
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="identity">自增量方式</param>
        /// <param name="returnId">返回主鍵</param>
        /// <returns>主键</returns>
        public string Add(BaseUserRoleEntity entity, bool identity, bool returnId)
        {
            Identity = identity;
            ReturnId = returnId;
            return AddEntity(entity);
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="entity">实体</param>
        public int Update(BaseUserRoleEntity entity)
        {
            return UpdateEntity(entity);
        }

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="id">主键</param>
        public BaseUserRoleEntity GetEntity(int id)
        {
            return BaseEntity.Create<BaseUserRoleEntity>(ExecuteReader(new KeyValuePair<string, object>(BaseUserRoleEntity.FieldId, id)));
        }

        /// <summary>
        /// 添加实体
        /// </summary>
        /// <param name="entity">实体</param>
        public string AddEntity(BaseUserRoleEntity entity)
        {
            var result = string.Empty;

            var sqlBuilder = new SqlBuilder(DbHelper, Identity, ReturnId);
            sqlBuilder.BeginInsert(CurrentTableName, BaseUserRoleEntity.FieldId);

            // 若是非空主键，表明已经指定了主键了
            if (!string.IsNullOrEmpty(entity.Id))
            {
                // 这里已经是指定了主键了，所以不需要返回主键了
                sqlBuilder.SetValue(BaseUserRoleEntity.FieldId, entity.Id);
                result = entity.Id;
            }
            else
            {
                if (DbHelper.CurrentDbType == CurrentDbType.Oracle)
                {
                    // 2015-12-23 吉日嘎拉 这里需要兼容一下以前的老的数据结构
                    sqlBuilder.SetFormula(BaseUserRoleEntity.FieldId, "SEQ_" + BaseRoleEntity.TableName.ToUpper() + ".NEXTVAL ");

                    /*
                    if (DbHelper.CurrentDbType == CurrentDbType.DB2)
                    {
                        sqlBuilder.SetFormula(BaseUserRoleEntity.FieldId, "NEXT VALUE FOR SEQ_" + BaseRoleEntity.TableName.ToUpper());
                    }
                    */
                }
                //MSSQL数据库是自增字段 Troy.Cui 2016-08-17
                //else
                //{
                //    entity.Id = Guid.NewGuid().ToString("N");
                //    result = entity.Id;
                //    sqlBuilder.SetValue(BaseRoleEntity.FieldId, entity.Id);
                //}
            }

            SetEntity(sqlBuilder, entity);
            if (UserInfo != null)
            {
                sqlBuilder.SetValue(BaseUserRoleEntity.FieldCreateUserId, UserInfo.Id);
                sqlBuilder.SetValue(BaseUserRoleEntity.FieldCreateBy, UserInfo.RealName);
            }
            sqlBuilder.SetDbNow(BaseUserRoleEntity.FieldCreateTime);
            if (UserInfo != null)
            {
                sqlBuilder.SetValue(BaseUserRoleEntity.FieldUpdateUserId, UserInfo.Id);
                sqlBuilder.SetValue(BaseUserRoleEntity.FieldUpdateBy, UserInfo.RealName);
            }
            sqlBuilder.SetDbNow(BaseUserRoleEntity.FieldUpdateTime);
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
        public int UpdateEntity(BaseUserRoleEntity entity)
        {
            var sqlBuilder = new SqlBuilder(DbHelper);
            sqlBuilder.BeginUpdate(CurrentTableName);
            SetEntity(sqlBuilder, entity);
            if (UserInfo != null)
            {
                sqlBuilder.SetValue(BaseUserRoleEntity.FieldUpdateUserId, UserInfo.Id);
                sqlBuilder.SetValue(BaseUserRoleEntity.FieldUpdateBy, UserInfo.RealName);
            }
            sqlBuilder.SetDbNow(BaseUserRoleEntity.FieldUpdateTime);
            sqlBuilder.SetWhere(BaseUserRoleEntity.FieldId, entity.Id);
            //return sqlBuilder.EndUpdate();
            var result = sqlBuilder.EndUpdate();
            if (result > 0)
            {
                RemoveCache();
            }
            return result;
        }

        partial void SetEntityExtend(SqlBuilder sqlBuilder, BaseUserRoleEntity entity);

        /// <summary>
        /// 设置实体
        /// </summary>
        /// <param name="sqlBuilder">SQL语句生成器</param>
        /// <param name="entity">实体</param>
        private void SetEntity(SqlBuilder sqlBuilder, BaseUserRoleEntity entity)
        {
            sqlBuilder.SetValue(BaseUserRoleEntity.FieldUserId, entity.UserId);
            sqlBuilder.SetValue(BaseUserRoleEntity.FieldRoleId, entity.RoleId);
            //去掉按公司分配用户和角色 2017.12.19 Troy Cui,懒得改数据库表结构了
            //if (!string.IsNullOrEmpty(entity.CompanyId))
            //{
            //    sqlBuilder.SetValue(BaseUserRoleEntity.FieldCompanyId, entity.CompanyId);
            //}
            sqlBuilder.SetValue(BaseUserRoleEntity.FieldEnabled, entity.Enabled);
            sqlBuilder.SetValue(BaseUserRoleEntity.FieldDescription, entity.Description);
            sqlBuilder.SetValue(BaseUserRoleEntity.FieldDeleted, entity.DeletionStateCode);
            SetEntityExtend(sqlBuilder, entity);
        }

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns>影响行数</returns>
        public int Delete(int id)
        {
            var result = Delete(new List<KeyValuePair<string, object>> { new KeyValuePair<string, object>(BaseUserRoleEntity.FieldId, id) });
            if (result > 0)
            {
                RemoveCache();
            }
            return result;
        }
    }
}
