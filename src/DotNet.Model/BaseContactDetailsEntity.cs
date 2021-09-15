//-----------------------------------------------------------------------
// <copyright file="BaseContactDetailsEntity.cs" company="DotNet">
//     Copyright (C) 2021, All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Data;

namespace DotNet.Model
{
    using Util;

    /// <summary>
    /// BaseContactDetailsEntity
    /// 联络单明细表
    ///
    /// 修改记录
    ///
    ///     给人的？阅读状态
    /// 
    ///		2015-10-30 版本：2.0 JiRiGaLa 已回复。
    ///		2010-07-15 版本：1.0 JiRiGaLa 创建主键。
    ///
    /// 版本：1.0
    ///
    /// <author>
    ///		<name>JiRiGaLa</name>
    ///		<date>2015-10-30</date>
    /// </author>
    /// </summary>
    [Serializable]
    public partial class BaseContactDetailsEntity : BaseEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        public string Id { get; set; } = null;
        /// <summary>
        /// 联络单主主键
        /// </summary>
        public string ContactId { get; set; } = null;
        /// <summary>
        /// 接收者分类
        /// </summary>
        public string Category { get; set; } = null;
        /// <summary>
        /// 接收者主键
        /// </summary>
        public string ReceiverId { get; set; } = null;
        /// <summary>
        /// 接收者姓名
        /// </summary>
        public string ReceiverRealName { get; set; } = null;
        /// <summary>
        /// 是否新邮件
        /// </summary>
        public int? IsNew { get; set; } = 0;
        /// <summary>
        /// 是否有新的评论
        /// </summary>
        public int? NewComment { get; set; } = 0;
        /// <summary>
        /// 已回复
        /// </summary>
        public int? Replied { get; set; } = 0;
        /// <summary>
        /// 最后阅读IP
        /// </summary>
        public string LastViewIp { get; set; } = null;
        /// <summary>
        /// 最后阅读时间
        /// </summary>
        public string LastViewDate { get; set; } = null;
        /// <summary>
        /// 有新评论是否提示
        /// </summary>
        public int? Enabled { get; set; } = 0;
        /// <summary>
        /// 是否删除
        /// </summary>
        public int? DeletionStateCode { get; set; } = 0;
        /// <summary>
        /// 排序码
        /// </summary>
        public int? SortCode { get; set; } = 0;
        /// <summary>
        /// 备注
        /// </summary>
        public string Description { get; set; } = null;
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateOn { get; set; } = DateTime.Now;
        /// <summary>
        /// 创建人用户编号
        /// </summary>
        public string CreateUserId { get; set; } = null;
        /// <summary>
        /// 创建人
        /// </summary>
        public string CreateBy { get; set; } = null;
        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime? ModifiedOn { get; set; } = DateTime.Now;
        /// <summary>
        /// 修改人用户编号
        /// </summary>
        public string ModifiedUserId { get; set; } = null;
        /// <summary>
        /// 修改人
        /// </summary>
        public string ModifiedBy { get; set; } = null;

        /// <summary>
        /// 构造函数
        /// </summary>
        public BaseContactDetailsEntity()
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dataRow">数据行</param>
        public BaseContactDetailsEntity(DataRow dataRow)
        {
            GetFrom(dataRow);
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dataReader">数据流</param>
        public BaseContactDetailsEntity(IDataReader dataReader)
        {
            GetFrom(dataReader);
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dataTable">数据表</param>
        public BaseContactDetailsEntity(DataTable dataTable)
        {
            GetSingle(dataTable);
        }

        /// <summary>
        /// 从数据表读取
        /// </summary>
        /// <param name="dataTable">数据表</param>
        public new BaseContactDetailsEntity GetSingle(DataTable dataTable)
        {
            if ((dataTable == null) || (dataTable.Rows.Count == 0))
            {
                return null;
            }
            foreach (DataRow dataRow in dataTable.Rows)
            {
                GetFrom(dataRow);
                break;
            }
            return this;
        }

        /// <summary>
        /// 从数据流读取
        /// </summary>
        /// <param name="dr">数据流</param>
        protected override BaseEntity GetFrom(IDataRow dr)
        {
            GetFromExtend(dr);
            Id = BaseUtil.ConvertToString(dr[FieldId]);
            ContactId = BaseUtil.ConvertToString(dr[FieldContactId]);
            Category = BaseUtil.ConvertToString(dr[FieldCategory]);
            ReceiverId = BaseUtil.ConvertToString(dr[FieldReceiverId]);
            ReceiverRealName = BaseUtil.ConvertToString(dr[FieldReceiverRealName]);
            IsNew = BaseUtil.ConvertToInt(dr[FieldIsNew]);
            NewComment = BaseUtil.ConvertToInt(dr[FieldNewComment]);
            Replied = BaseUtil.ConvertToInt(dr[FieldReplied]);
            LastViewIp = BaseUtil.ConvertToString(dr[FieldLastViewIp]);
            LastViewDate = BaseUtil.ConvertToString(dr[FieldLastViewDate]);
            Enabled = BaseUtil.ConvertToInt(dr[FieldEnabled]);
            DeletionStateCode = BaseUtil.ConvertToInt(dr[FieldDeleted]);
            SortCode = BaseUtil.ConvertToInt(dr[FieldSortCode]);
            Description = BaseUtil.ConvertToString(dr[FieldDescription]);
            CreateOn = BaseUtil.ConvertToDateTime(dr[FieldCreateTime]);
            CreateUserId = BaseUtil.ConvertToString(dr[FieldCreateUserId]);
            CreateBy = BaseUtil.ConvertToString(dr[FieldCreateBy]);
            ModifiedOn = BaseUtil.ConvertToDateTime(dr[FieldUpdateTime]);
            ModifiedUserId = BaseUtil.ConvertToString(dr[FieldUpdateUserId]);
            ModifiedBy = BaseUtil.ConvertToString(dr[FieldUpdateBy]);
            return this;
        }

        ///<summary>
        /// 联络单明细表
        ///</summary>
        [NonSerialized]
        public const string TableName = "BaseContactDetails";

        ///<summary>
        /// 主键
        ///</summary>
        [NonSerialized]
        public const string FieldId = "Id";

        ///<summary>
        /// 联络单主主键
        ///</summary>
        [NonSerialized]
        public const string FieldContactId = "ContactId";

        ///<summary>
        /// 接收者分类
        ///</summary>
        [NonSerialized]
        public const string FieldCategory = "Category";

        ///<summary>
        /// 接收者主键
        ///</summary>
        [NonSerialized]
        public const string FieldReceiverId = "ReceiverId";

        ///<summary>
        /// 接收者姓名
        ///</summary>
        [NonSerialized]
        public const string FieldReceiverRealName = "ReceiverRealName";

        ///<summary>
        /// 是否新邮件
        ///</summary>
        [NonSerialized]
        public const string FieldIsNew = "IsNew";

        ///<summary>
        /// 是否有新的评论
        ///</summary>
        [NonSerialized]
        public const string FieldNewComment = "NewComment";

        ///<summary>
        /// 已回复
        ///</summary>
        [NonSerialized]
        public const string FieldReplied = "Replied";

        ///<summary>
        /// 最后阅读IP
        ///</summary>
        [NonSerialized]
        public const string FieldLastViewIp = "LastViewIP";

        ///<summary>
        /// 最后阅读时间
        ///</summary>
        [NonSerialized]
        public const string FieldLastViewDate = "LastViewDate";

        ///<summary>
        /// 有新评论是否提示
        ///</summary>
        [NonSerialized]
        public const string FieldEnabled = "Enabled";

        ///<summary>
        /// 是否删除
        ///</summary>
        [NonSerialized]
        public const string FieldDeleted = "DeletionStateCode";

        ///<summary>
        /// 排序码
        ///</summary>
        [NonSerialized]
        public const string FieldSortCode = "SortCode";

        ///<summary>
        /// 备注
        ///</summary>
        [NonSerialized]
        public const string FieldDescription = "Description";

        ///<summary>
        /// 创建时间
        ///</summary>
        [NonSerialized]
        public const string FieldCreateTime = "CreateOn";

        ///<summary>
        /// 创建人用户编号
        ///</summary>
        [NonSerialized]
        public const string FieldCreateUserId = "CreateUserId";

        ///<summary>
        /// 创建人
        ///</summary>
        [NonSerialized]
        public const string FieldCreateBy = "CreateBy";

        ///<summary>
        /// 修改时间
        ///</summary>
        [NonSerialized]
        public const string FieldUpdateTime = "ModifiedOn";

        ///<summary>
        /// 修改人用户编号
        ///</summary>
        [NonSerialized]
        public const string FieldUpdateUserId = "ModifiedUserId";

        ///<summary>
        /// 修改人
        ///</summary>
        [NonSerialized]
        public const string FieldUpdateBy = "ModifiedBy";
    }
}