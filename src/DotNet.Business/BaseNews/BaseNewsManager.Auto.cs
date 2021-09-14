//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System.Collections.Generic;

namespace DotNet.Business
{
    using Model;
    using Util;

    /// <summary>
    /// BaseNewsManager
    /// 新闻表
    ///
    /// 修改记录
    ///
    ///		2010-07-28 版本：1.0 JiRiGaLa 创建主键。
    ///
    /// <author>
    ///		<name>JiRiGaLa</name>
    ///		<date>2010-07-28</date>
    /// </author>
    /// </summary>
    public partial class BaseNewsManager : BaseManager, IBaseManager
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public BaseNewsManager()
        {
            if (dbHelper == null)
            {
                dbHelper = DbHelperFactory.GetHelper(BaseSystemInfo.UserCenterDbType, BaseSystemInfo.UserCenterDbConnection);
            }
            CurrentTableName = BaseNewsEntity.TableName;
        }

        /// <summary>
        /// 构造函数
        /// <param name="tableName">指定表名</param>
        /// </summary>
        public BaseNewsManager(string tableName)
        {
            CurrentTableName = tableName;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        public BaseNewsManager(IDbHelper dbHelper)
            : this()
        {
            DbHelper = dbHelper;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="userInfo">操作员信息</param>
        public BaseNewsManager(BaseUserInfo userInfo)
            : this()
        {
            UserInfo = userInfo;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        /// <param name="userInfo">操作员信息</param>
        public BaseNewsManager(IDbHelper dbHelper, BaseUserInfo userInfo)
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
        public BaseNewsManager(IDbHelper dbHelper, BaseUserInfo userInfo, string tableName)
            : this(dbHelper, userInfo)
        {
            CurrentTableName = tableName;
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns>主键</returns>
        public string Add(BaseNewsEntity entity)
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
        public string Add(BaseNewsEntity entity, bool identity, bool returnId)
        {
            Identity = identity;
            ReturnId = returnId;
            return AddObject(entity);
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="entity">实体</param>
        public int Update(BaseNewsEntity entity)
        {
            return UpdateObject(entity);
        }

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="id">主键</param>
        public BaseNewsEntity GetObject(string id)
        {
            return BaseEntity.Create<BaseNewsEntity>(ExecuteReader(new KeyValuePair<string, object>(BaseNewsEntity.FieldId, id)));
            // return BaseEntity.Create<BaseNewsEntity>(this.GetDataTable(new KeyValuePair<string, object>(BaseNewsEntity.FieldId, id)));
        }

        /// <summary>
        /// 添加实体
        /// </summary>
        /// <param name="entity">实体</param>
        public string AddObject(BaseNewsEntity entity)
        {
            var sequence = entity.Id;
            if (entity.SortCode == 0)
            {
                var managerSequence = new BaseSequenceManager(DbHelper, Identity);
                sequence = managerSequence.Increment(CurrentTableName);
                entity.SortCode = int.Parse(sequence);
            }
            if (entity.Id is string)
            {
                Identity = false;
            }
            var sqlBuilder = new SqlBuilder(DbHelper, Identity, ReturnId);
            sqlBuilder.BeginInsert(CurrentTableName, BaseNewsEntity.FieldId);
            if (!Identity)
            {
                sqlBuilder.SetValue(BaseNewsEntity.FieldId, entity.Id);
            }
            else
            {
                if (!ReturnId && DbHelper.CurrentDbType == CurrentDbType.Oracle)
                {
                    sqlBuilder.SetFormula(BaseNewsEntity.FieldId, "SEQ_" + CurrentTableName.ToUpper() + ".NEXTVAL ");
                }
                else
                {
                    if (Identity && DbHelper.CurrentDbType == CurrentDbType.Oracle)
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
                        sqlBuilder.SetValue(BaseNewsEntity.FieldId, entity.Id);
                    }
                }
            }
            SetObject(sqlBuilder, entity);
            if (UserInfo != null)
            {
                entity.CreateUserId = UserInfo.Id;
                entity.CreateBy = UserInfo.RealName;
                sqlBuilder.SetValue(BaseNewsEntity.FieldCreateUserId, entity.CreateUserId);
                sqlBuilder.SetValue(BaseNewsEntity.FieldCreateBy, entity.CreateBy);
                entity.ModifiedBy = UserInfo.RealName;
                sqlBuilder.SetValue(BaseNewsEntity.FieldUpdateUserId, entity.CreateUserId);
                sqlBuilder.SetValue(BaseNewsEntity.FieldUpdateBy, entity.CreateBy);
            }
            sqlBuilder.SetDbNow(BaseNewsEntity.FieldCreateTime);
            sqlBuilder.SetDbNow(BaseNewsEntity.FieldUpdateTime);
            /*
            if (UserInfo != null)
            {
                sqlBuilder.SetValue(BaseNewsEntity.FieldUpdateUserId, UserInfo.Id);
                sqlBuilder.SetValue(BaseNewsEntity.FieldUpdateBy, UserInfo.RealName);
            }
            sqlBuilder.SetDBNow(BaseNewsEntity.FieldUpdateTime);
             */
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
        public int UpdateObject(BaseNewsEntity entity)
        {
            var sqlBuilder = new SqlBuilder(DbHelper);
            sqlBuilder.BeginUpdate(CurrentTableName);
            SetObject(sqlBuilder, entity);
            if (UserInfo != null)
            {
                sqlBuilder.SetValue(BaseNewsEntity.FieldUpdateUserId, UserInfo.Id);
                sqlBuilder.SetValue(BaseNewsEntity.FieldUpdateBy, UserInfo.RealName);
            }
            sqlBuilder.SetDbNow(BaseNewsEntity.FieldUpdateTime);
            sqlBuilder.SetWhere(BaseNewsEntity.FieldId, entity.Id);
            return sqlBuilder.EndUpdate();
        }

        partial void SetObjectExpand(SqlBuilder sqlBuilder, BaseNewsEntity entity);

        /// <summary>
        /// 设置实体
        /// </summary>
        /// <param name="sqlBuilder">SQL语句生成器</param>
        /// <param name="entity">实体</param>
        private void SetObject(SqlBuilder sqlBuilder, BaseNewsEntity entity)
        {
            if (entity.Contents == null)
            {
                entity.FileSize = 0;
            }
            else
            {
                entity.FileSize = entity.Contents.Length;
            }
            sqlBuilder.SetValue(BaseNewsEntity.FieldCompanyId, entity.CompanyId);
            sqlBuilder.SetValue(BaseNewsEntity.FieldCompanyName, entity.CompanyName);
            sqlBuilder.SetValue(BaseNewsEntity.FieldDepartmentId, entity.DepartmentId);
            sqlBuilder.SetValue(BaseNewsEntity.FieldDepartmentName, entity.DepartmentName);
            sqlBuilder.SetValue(BaseNewsEntity.FieldFolderId, entity.FolderId);
            sqlBuilder.SetValue(BaseNewsEntity.FieldCategoryCode, entity.CategoryCode);
            sqlBuilder.SetValue(BaseNewsEntity.FieldCode, entity.Code);
            sqlBuilder.SetValue(BaseNewsEntity.FieldTitle, entity.Title);
            sqlBuilder.SetValue(BaseNewsEntity.FieldFilePath, entity.FilePath);
            sqlBuilder.SetValue(BaseNewsEntity.FieldIntroduction, entity.Introduction);
            sqlBuilder.SetValue(BaseNewsEntity.FieldContents, entity.Contents);
            sqlBuilder.SetValue(BaseNewsEntity.FieldSource, entity.Source);
            sqlBuilder.SetValue(BaseNewsEntity.FieldKeywords, entity.Keywords);
            sqlBuilder.SetValue(BaseNewsEntity.FieldFileSize, entity.FileSize);
            sqlBuilder.SetValue(BaseNewsEntity.FieldImageUrl, entity.ImageUrl);
            sqlBuilder.SetValue(BaseNewsEntity.FieldHomePage, entity.HomePage);
            sqlBuilder.SetValue(BaseNewsEntity.FieldSubPage, entity.SubPage);
            sqlBuilder.SetValue(BaseNewsEntity.FieldAuditStatus, entity.AuditStatus);
            sqlBuilder.SetValue(BaseNewsEntity.FieldReadCount, entity.ReadCount);
            sqlBuilder.SetValue(BaseNewsEntity.FieldDeleted, entity.DeletionStateCode);
            sqlBuilder.SetValue(BaseNewsEntity.FieldDescription, entity.Description);
            sqlBuilder.SetValue(BaseNewsEntity.FieldEnabled, entity.Enabled);
            sqlBuilder.SetValue(BaseNewsEntity.FieldSortCode, entity.SortCode);
            SetObjectExpand(sqlBuilder, entity);
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
