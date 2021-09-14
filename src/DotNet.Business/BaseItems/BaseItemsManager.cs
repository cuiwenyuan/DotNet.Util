//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System.Collections.Generic;
using System.Data;

namespace DotNet.Business
{
    using Model;
    using Util;

    /// <summary>
    /// BaseItemsManager 
    /// 主键管理表结构定义部分（程序OK）
    ///
	/// 注意事项;
	///		Id 为主键
	///		CreateOn不为空，默认值
	///		ParentId、FullName 需要建立唯一索引
	///		CategoryId 是为了解决多重数据库兼容的问题
	///		ParentId 是为了解决形成树行结构的问题
	///
	/// 修改记录
    ///
    ///     2007.12.03 版本：4.3 JiRiGaLa  进行规范化整理。
    ///     2007.06.03 版本：4.2 JiRiGaLa  进行改进整理。
    ///     2007.05.31 版本：4.1 JiRiGaLa  进行改进整理。
    ///		2007.01.15 版本：4.0 JiRiGaLa  重新整理主键。
    ///		2007.01.03 版本：3.0 JiRiGaLa  进行大批量主键整理。
    ///		2006.12.05 版本：2.0 JiRiGaLa  GetFullName 方法更新。
	///		2006.01.23 版本：1.0 JiRiGaLa  获取ItemDetails方法的改进。
	///		2004.11.12 版本：1.0 JiRiGaLa  主键进行了绝对的优化，基本上看上去还过得去了。
    ///     2007.12.03 版本：2.2 JiRiGaLa  进行规范化整理。
    ///     2007.05.30 版本：2.1 JiRiGaLa  整理主键，调整GetFrom()方法,增加AddObject(),UpdateObject(),DeleteObject()
    ///		2007.01.15 版本：2.0 JiRiGaLa  重新整理主键。
	///		2006.02.06 版本：1.1 JiRiGaLa  重新调整主键的规范化。
	///		2005.10.03 版本：1.0 JiRiGaLa  表中添加是否可删除，可修改字段。
	///
	/// <author>
    ///		<name>JiRiGaLa</name>
    ///		<date>2007.12.03</date>
	/// </author>
	/// </summary>
    public partial class BaseItemsManager : BaseManager
    {
        #region public string Add(BaseItemsEntity entity, out string statusCode) 添加
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="statusCode">返回状态码</param>
        /// <returns>主键</returns>
        public string Add(BaseItemsEntity entity, out string statusCode)
        {
            var result = string.Empty;
            // 检查编号是否重复
            if (Exists(new KeyValuePair<string, object>(BaseItemsEntity.FieldParentId, entity.ParentId), new KeyValuePair<string, object>(BaseItemsEntity.FieldCode, entity.Code)))
            {
                // 编号已重复
                statusCode = Status.ErrorCodeExist.ToString();
            }
            else
            {
                // 检查名称是否重复
                if (Exists(new KeyValuePair<string, object>(BaseItemsEntity.FieldParentId, entity.ParentId), new KeyValuePair<string, object>(BaseItemsEntity.FieldFullName, entity.FullName)))
                {
                    // 名称已重复
                    statusCode = Status.ErrorNameExist.ToString();
                }
                else
                {
                    result = AddObject(entity);
                    // 运行成功
                    statusCode = Status.OkAdd.ToString();
                }
            }
            return result;
        }
        #endregion

        #region public int Update(BaseItemsEntity entity, out string statusCode) 更新
        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="statusCode">返回状态码</param>
        /// <returns>影响行数</returns>
        public int Update(BaseItemsEntity entity, out string statusCode)
        {
            var result = 0;
            // 检查是否已被其他人修改            
            //if (DbUtil.IsModifed(DbHelper, this.CurrentTableName, itemsEntity.Id, itemsEntity.ModifiedUserId, itemsEntity.ModifiedOn))
            //{
            //    // 数据已经被修改
            //    statusCode = StatusCode.ErrorChanged.ToString();
            //}
            //else
            //{
                // 检查编号是否重复
                if (Exists(new KeyValuePair<string, object>(BaseItemsEntity.FieldParentId, entity.ParentId), new KeyValuePair<string, object>(BaseItemsEntity.FieldCode, entity.Code), entity.Id))
                {
                    // 编号已重复
                    statusCode = Status.ErrorCodeExist.ToString();
                }
                else
                {
                    // 检查名称是否重复
                    if (Exists(new KeyValuePair<string, object>(BaseItemsEntity.FieldParentId, entity.ParentId), new KeyValuePair<string, object>(BaseItemsEntity.FieldFullName, entity.FullName), entity.Id))
                    {
                        // 名称已重复
                        statusCode = Status.ErrorNameExist.ToString();
                    }
                    else
                    {
                        result = UpdateObject(entity);
                        if (result == 1)
                        {
                            statusCode = Status.OkUpdate.ToString();
                        }
                        else
                        {
                            statusCode = Status.ErrorDeleted.ToString();
                        }
                    }
                }
            //}
            return result;
        }
        #endregion

        #region public void CreateTable(string tableName, out string statusCode)

        /// <summary>
        /// 创建表结构
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="statusCode">状态返回码</param>
        public void CreateTable(string tableName, out string statusCode)
        {
            statusCode = Status.Error.ToString();
            var commandText = string.Empty;
            if (DbHelper.CurrentDbType == CurrentDbType.Access)
                // Access 中没有使用临时表，故直接返回成的标识
                statusCode = Status.Ok.ToString();
            else if (DbHelper.CurrentDbType == CurrentDbType.SqlServer)
            {
                commandText = @"
CREATE TABLE [dbo].[{tableName}](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ParentId] [int] NULL,
	[ItemCode] [nvarchar](50) NULL,
	[ItemName] [nvarchar](50)  NULL,
	[ItemValue] [nvarchar](50)  NULL,
	[Enabled] [tinyint] NOT NULL CONSTRAINT [DF_{tableName}_Enabled]  DEFAULT ((1)),
	[AllowEdit] [tinyint] NOT NULL CONSTRAINT [DF_{tableName}_AllowEdit]  DEFAULT ((1)),
	[AllowDelete] [tinyint] NOT NULL CONSTRAINT [DF_{tableName}_AllowDelete]  DEFAULT ((1)),
	[IsPublic] [tinyint] NOT NULL CONSTRAINT [DF_{tableName}_IsPublic]  DEFAULT ((1)),
	[DeletionStateCode] [tinyint] NOT NULL CONSTRAINT [DF_{tableName}_DeletionStateCode]  DEFAULT ((0)),
	[SortCode] [int] NULL,
	[Description] [nvarchar](MAX)  NULL,
	[CreateOn] [datetime] NOT NULL CONSTRAINT [DF_{tableName}_CreateOn]  DEFAULT (GETDATE()),
	[CreateUserId] [int]  NULL DEFAULT ((0)),
	[CreateUserName] [nvarchar](50)  NULL,
	[CreateBy] [nvarchar](50)  NULL,
	[CreateIp] [nvarchar](50)  NULL,
	[ModifiedOn] [datetime] NOT NULL CONSTRAINT [DF_{tableName}_ModifiedOn]  DEFAULT (GETDATE()),
	[ModifiedUserId] [int]  NULL DEFAULT ((0)),
	[ModifiedUserName] [nvarchar](50)  NULL,
	[ModifiedBy] [nvarchar](50)  NULL,
	[ModifiedIp] [nvarchar](50)  NULL,
    CONSTRAINT [PK_{tableName}] PRIMARY KEY CLUSTERED 
    (
	    [Id] ASC
    )WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]";
                // 替换表明
                commandText = commandText.Replace("{tableName}", tableName);
                // 执行数据库指令
                DbHelper.ExecuteNonQuery(commandText);
                // 创建成功
                statusCode = Status.Ok.ToString();
            }
            else if (DbHelper.CurrentDbType == CurrentDbType.Oracle)
            {
                commandText = @"
create table {tableName} 
(
    Id                 INT                not null,
    ParentId           INT,
    ItemCode           VARCHAR2(50),
    ItemName           VARCHAR2(50)       not null,
    ItemValue          VARCHAR2(50),
    AllowEdit          NUMBER(1)                  default 1 not null,
    AllowDelete        NUMBER(1)                  default 1 not null,
    IsPublic           NUMBER(1)                  default 1 not null,
    Enabled            NUMBER(1)                  default 1 not null,
    DeletionStateCode  NUMBER(1)                  default 0,
    SortCode           INT,
    Description        VARCHAR2(800),
    CreateOn           DATE,
    CreateUserId       INT default 0,
    CreateUserName     VARCHAR2(50),
    CreateBy           VARCHAR2(50),
    CreateIp           VARCHAR2(50),
    ModifiedOn         DATE,
    ModifiedUserId     INT default 0,
    ModifiedUserName   VARCHAR2(50),
    ModifiedBy         VARCHAR2(50),
    ModifiedIp         VARCHAR2(50),
    constraint PK_{tableName} primary key (Id)
)";
                // 替换表明
                commandText = commandText.Replace("{tableName}", tableName);
                // 创建表结构
                DbHelper.ExecuteNonQuery(commandText);
                commandText = "CREATE SEQUENCE SEQ_" + tableName + " MINVALUE 1 MAXVALUE 9999999999999999999 START WITH 1 INCREMENT BY 1 CACHE 20";
                // 创建序列
                DbHelper.ExecuteNonQuery(commandText);
                // 运行成功
                statusCode = Status.Ok.ToString();
            }
        }
        #endregion

        #region public override int BatchSave(DataTable result) 批量进行保存
        /// <summary>
        /// 批量进行保存
        /// </summary>
        /// <param name="dt">数据表</param>
        /// <returns>影响行数</returns>
        public override int BatchSave(DataTable dt)
        {
            var result = 0;
            var itemsEntity = new BaseItemsEntity();
            foreach (DataRow dr in dt.Rows)
            {
                // 删除状态
                if (dr.RowState == DataRowState.Deleted)
                {
                    var id = dr[BaseItemsEntity.FieldId, DataRowVersion.Original].ToString();
                    if (id.Length > 0)
                    {
                        if (itemsEntity.AllowDelete == 1)
                        {
                            result += Delete(id);
                        }
                    }
                }
                // 被修改过
                if (dr.RowState == DataRowState.Modified)
                {
                    var id = dr[BaseItemsEntity.FieldId, DataRowVersion.Original].ToString();
                    if (id.Length > 0)
                    {
                        itemsEntity.GetFrom(dr);
                        // 判断是否允许编辑
                        if (itemsEntity.AllowEdit == 1)
                        {
                            result += UpdateObject(itemsEntity);
                        }
                        else
                        {
                            // 不允许编辑，但是排序还是允许的
                            result += SetProperty(itemsEntity.Id, new KeyValuePair<string, object>(BaseItemsEntity.FieldSortCode, itemsEntity.SortCode));
                        }
                    }
                }
                // 添加状态
                if (dr.RowState == DataRowState.Added)
                {
                    itemsEntity.GetFrom(dr);
                    result += AddObject(itemsEntity).Length > 0 ? 1 : 0;
                }
                if (dr.RowState == DataRowState.Unchanged)
                {
                    continue;
                }
                if (dr.RowState == DataRowState.Detached)
                {
                    continue;
                }
            }
            return result;
        }
        #endregion
	}
}