//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System.Collections.Generic;

namespace DotNet.Business
{
    using Model;
    using Util;

    /// <summary>
    /// BaseFileManager
    /// 文件新闻表
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
    public partial class BaseFileManager : BaseManager, IBaseManager
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public BaseFileManager()
        {
            if (dbHelper == null)
            {
                dbHelper = DbHelperFactory.GetHelper(BaseSystemInfo.UserCenterDbType, BaseSystemInfo.UserCenterDbConnection);
            }
            CurrentTableName = BaseFileEntity.TableName;
        }

        /// <summary>
        /// 构造函数
        /// <param name="tableName">指定表名</param>
        /// </summary>
        public BaseFileManager(string tableName)
        {
            CurrentTableName = tableName;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        public BaseFileManager(IDbHelper dbHelper)
            : this()
        {
            DbHelper = dbHelper;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="userInfo">操作员信息</param>
        public BaseFileManager(BaseUserInfo userInfo)
            : this()
        {
            UserInfo = userInfo;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        /// <param name="userInfo">操作员信息</param>
        public BaseFileManager(IDbHelper dbHelper, BaseUserInfo userInfo)
            : this(dbHelper)
        {
            UserInfo = userInfo;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbHelper">数据库连接</param>
        /// <param name="userInfo">操作员信息</param>
        /// <param name="tableName">指定表名</param>
        public BaseFileManager(IDbHelper dbHelper, BaseUserInfo userInfo, string tableName)
            : this(dbHelper, userInfo)
        {
            CurrentTableName = tableName;
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns>主键</returns>
        public string Add(BaseFileEntity entity)
        {
            return AddEntity(entity);
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="identity">自增量方式</param>
        /// /// <param name="returnId">返回主鍵</param>
        /// <returns>主键</returns>
        public string Add(BaseFileEntity entity, bool identity, bool returnId)
        {
            Identity = identity;
            ReturnId = returnId;
            return AddEntity(entity);
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="entity">实体</param>
        public int Update(BaseFileEntity entity)
        {
            return UpdateEntity(entity);
        }

        /// <summary>
        /// 添加实体
        /// </summary>
        /// <param name="entity">实体</param>
        public string AddEntity(BaseFileEntity entity)
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
            sqlBuilder.BeginInsert(CurrentTableName, BaseFileEntity.FieldId);
            if (!Identity)
            {
                sqlBuilder.SetValue(BaseFileEntity.FieldId, entity.Id);
            }
            else
            {
                if (!ReturnId && DbHelper.CurrentDbType == CurrentDbType.Oracle)
                {
                    sqlBuilder.SetFormula(BaseFileEntity.FieldId, "SEQ_" + CurrentTableName.ToUpper() + ".NEXTVAL ");
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
                        sqlBuilder.SetValue(BaseFileEntity.FieldId, entity.Id);
                    }
                }
            }
            SetEntity(sqlBuilder, entity);
            if (UserInfo != null)
            {
                sqlBuilder.SetValue(BaseFileEntity.FieldCreateUserId, UserInfo.Id);
                sqlBuilder.SetValue(BaseFileEntity.FieldCreateBy, UserInfo.RealName);
            }
            sqlBuilder.SetDbNow(BaseFileEntity.FieldCreateTime);
            // 这里主要是为了列表里的数据库更好看
            sqlBuilder.SetValue(BaseFileEntity.FieldUpdateUserId, entity.ModifiedUserId);
            sqlBuilder.SetValue(BaseFileEntity.FieldUpdateBy, entity.ModifiedBy);
            sqlBuilder.SetValue(BaseFileEntity.FieldUpdateTime, entity.ModifiedOn);
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
        public int UpdateEntity(BaseFileEntity entity)
        {
            var sqlBuilder = new SqlBuilder(DbHelper);
            sqlBuilder.BeginUpdate(CurrentTableName);
            SetEntity(sqlBuilder, entity);
            if (UserInfo != null)
            {
                sqlBuilder.SetValue(BaseFileEntity.FieldUpdateUserId, UserInfo.Id);
                sqlBuilder.SetValue(BaseFileEntity.FieldUpdateBy, UserInfo.RealName);
            }
            sqlBuilder.SetDbNow(BaseFileEntity.FieldUpdateTime);
            sqlBuilder.SetWhere(BaseFileEntity.FieldId, entity.Id);
            return sqlBuilder.EndUpdate();
        }

        partial void SetEntityExtend(SqlBuilder sqlBuilder, BaseFileEntity entity);

        /// <summary>
        /// 设置实体
        /// </summary>
        /// <param name="sqlBuilder">SQL语句生成器</param>
        /// <param name="entity">实体</param>
        private void SetEntity(SqlBuilder sqlBuilder, BaseFileEntity entity)
        {
            sqlBuilder.SetValue(BaseFileEntity.FieldFolderId, entity.FolderId);
            sqlBuilder.SetValue(BaseFileEntity.FieldFileName, entity.FileName);
            sqlBuilder.SetValue(BaseFileEntity.FieldFilePath, entity.FilePath);
            sqlBuilder.SetValue(BaseFileEntity.FieldContents, entity.Contents);
            if (entity.Contents != null && (entity.FileSize == null || entity.FileSize == 0))
            {
                sqlBuilder.SetValue(BaseFileEntity.FieldFileSize, entity.Contents.Length);
            }
            else
            {
                sqlBuilder.SetValue(BaseFileEntity.FieldFileSize, entity.FileSize);
            }
            sqlBuilder.SetValue(BaseFileEntity.FieldReadCount, entity.ReadCount);
            sqlBuilder.SetValue(BaseFileEntity.FieldDeleted, entity.DeletionStateCode);
            sqlBuilder.SetValue(BaseFileEntity.FieldDescription, entity.Description);
            sqlBuilder.SetValue(BaseFileEntity.FieldEnabled, entity.Enabled);
            sqlBuilder.SetValue(BaseFileEntity.FieldSortCode, entity.SortCode);
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
