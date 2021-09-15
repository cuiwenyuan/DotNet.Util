//-----------------------------------------------------------------------
// <copyright file="BaseContactEntity.cs" company="DotNet">
//     Copyright (C) 2021, All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Data;


namespace DotNet.Model
{
    using Util;

    /// <summary>
    /// BaseContactEntity
    /// 联络单主表
    ///
    /// 修改记录
    ///
    ///		2015-12-29 版本：2.1 JiRiGaLa 允许下级可以看？
    ///		2015-10-30 版本：2.0 JiRiGaLa 必读、必回。
    ///                颜色、（标题样式 加粗、斜体、粗斜体）、来源部门、来源公司？
    ///                省、审核人、
    ///		2010-07-15 版本：1.0 JiRiGaLa 创建主键。
    ///
    /// 版本：2.1
    ///
    /// <author>
    ///		<name>JiRiGaLa</name>
    ///		<date>2015-12-29</date>
    /// </author>
    /// </summary>
    [Serializable]
    public partial class BaseContactEntity : BaseEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        public string Id { get; set; } = null;
        /// <summary>
        /// 父主键
        /// </summary>
        public string ParentId { get; set; } = null;
        /// <summary>
        /// 主题
        /// </summary>
        public string Title { get; set; } = null;
        /// <summary>
        /// 颜色
        /// </summary>
        public string Color { get; set; } = null;
        /// <summary>
        /// 样式
        /// </summary>
        public string Style { get; set; } = null;
        /// <summary>
        /// 内容
        /// </summary>
        public string Contents { get; set; } = null;
        /// <summary>
        /// 等级(置顶, 1,2,3,4)
        /// </summary>
        public string Priority { get; set; } = null;
        /// <summary>
        /// 取消置顶期限
        /// </summary>

        public int? CancelTopDay { get; set; } = 0;
        /// <summary>
        /// 新闻来源(供稿人)
        /// </summary>

        public string Source { get; set; } = null;
        /// <summary>
        /// 分类
        /// </summary>
        [FieldDescription("分类")]

        public string CategoryCode { get; set; } = null;
        /// <summary>
        /// 标签
        /// </summary>
        [FieldDescription("标签")]

        public string LabelMark { get; set; } = null;
        /// <summary>
        /// 发送邮件总数
        /// </summary>
        public int? SendCount { get; set; } = 0;
        /// <summary>
        /// 已阅读人数
        /// </summary>
        public int? ReadCount { get; set; } = 0;
        /// <summary>
        /// 回复数
        /// </summary>
        public int? ReplyCount { get; set; } = 0;
        /// <summary>
        /// 必读
        /// </summary>
        public int? MustRead { get; set; } = 0;
        /// <summary>
        /// 必须回复
        /// </summary>
        public int? MustReply { get; set; } = 0;
        /// <summary>
        /// 是否公开（允许下属网点看）
        /// </summary>
        public int? IsOpen { get; set; } = 0;
        /// <summary>
        /// IP地址
        /// </summary>
        public string IpAddress { get; set; } = null;
        /// <summary>
        /// 允许评论
        /// </summary>
        public int? AllowComments { get; set; } = 0;
        /// <summary>
        /// 最后评论人主键
        /// </summary>
        public string CommentUserId { get; set; } = null;
        /// <summary>
        /// 最后评论人姓名
        /// </summary>
        public string CommentUserRealName { get; set; } = null;
        /// <summary>
        /// 最后评论时间
        /// </summary>
        public DateTime? CommentDate { get; set; } = null;
        /// <summary>
        /// 是否删除
        /// </summary>
        public int? DeletionStateCode { get; set; } = 0;
        /// <summary>
        /// 审核状态
        /// </summary>

        public string AuditStatus { get; set; } = null;
        /// <summary>
        /// 审核员主键
        /// </summary>

        public string AuditUserId { get; set; } = null;
        /// <summary>
        /// 审核员
        /// </summary>

        public string AuditUserRealName { get; set; } = null;
        /// <summary>
        /// 有效
        /// </summary>
        public int? Enabled { get; set; } = 0;
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
        /// 来源部门
        /// </summary>
        public string CreateDepartment { get; set; } = null;
        /// <summary>
        /// 创建公司主键
        /// </summary>
        public string CreateCompanyId { get; set; } = null;
        /// <summary>
        /// 来源公司
        /// </summary>
        public string CreateCompany { get; set; } = null;
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
        public BaseContactEntity()
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dataRow">数据行</param>
        public BaseContactEntity(DataRow dataRow)
        {
            GetFrom(dataRow);
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dataReader">数据流</param>
        public BaseContactEntity(IDataReader dataReader)
        {
            GetFrom(dataReader);
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dataTable">数据表</param>
        public BaseContactEntity(DataTable dataTable)
        {
            GetSingle(dataTable);
        }

        /// <summary>
        /// 从数据行读取
        /// </summary>
        /// <param name="dr">数据行</param>
        protected override BaseEntity GetFrom(IDataRow dr)
        {
            GetFromExtend(dr);
            Id = BaseUtil.ConvertToString(dr[FieldId]);
            ParentId = BaseUtil.ConvertToString(dr[FieldParentId]);
            Title = BaseUtil.ConvertToString(dr[FieldTitle]);
            Color = BaseUtil.ConvertToString(dr[FieldColor]);
            Style = BaseUtil.ConvertToString(dr[FieldStyle]);
            Contents = BaseUtil.ConvertToString(dr[FieldContents]);
            CategoryCode = BaseUtil.ConvertToString(dr[FieldCategoryCode]);
            LabelMark = BaseUtil.ConvertToString(dr[FieldLabelMark]);
            Priority = BaseUtil.ConvertToString(dr[FieldPriority]);
            SendCount = BaseUtil.ConvertToInt(dr[FieldSendCount]);
            ReadCount = BaseUtil.ConvertToInt(dr[FieldReadCount]);
            Source = BaseUtil.ConvertToString(dr[FieldSource]);
            IsOpen = BaseUtil.ConvertToInt(dr[FieldIsOpen]);
            IpAddress = BaseUtil.ConvertToString(dr[FieldIpAddress]);
            AllowComments = BaseUtil.ConvertToInt(dr[FieldAllowComments]);
            ReplyCount = BaseUtil.ConvertToInt(dr[FieldReplyCount]);
            MustRead = BaseUtil.ConvertToInt(dr[FieldMustRead]);
            MustReply = BaseUtil.ConvertToInt(dr[FieldMustReply]);
            CommentUserId = BaseUtil.ConvertToString(dr[FieldCommentUserId]);
            CommentUserRealName = BaseUtil.ConvertToString(dr[FieldCommentUserRealName]);
            CancelTopDay = BaseUtil.ConvertToInt(dr[FieldCancelTopDay]);
            CommentDate = BaseUtil.ConvertToDateTime(dr[FieldCommentDate]);
            DeletionStateCode = BaseUtil.ConvertToInt(dr[FieldDeleted]);
            Enabled = BaseUtil.ConvertToInt(dr[FieldEnabled]);
            AuditUserId = BaseUtil.ConvertToString(dr[FieldAuditUserId]);
            AuditUserRealName = BaseUtil.ConvertToString(dr[FieldAuditUserRealName]);
            AuditStatus = BaseUtil.ConvertToString(dr[FieldAuditStatus]);
            SortCode = BaseUtil.ConvertToInt(dr[FieldSortCode]);
            Description = BaseUtil.ConvertToString(dr[FieldDescription]);
            CreateOn = BaseUtil.ConvertToDateTime(dr[FieldCreateTime]);
            CreateUserId = BaseUtil.ConvertToString(dr[FieldCreateUserId]);
            CreateBy = BaseUtil.ConvertToString(dr[FieldCreateBy]);
            CreateDepartment = BaseUtil.ConvertToString(dr[FieldCreateDepartment]);
            CreateCompany = BaseUtil.ConvertToString(dr[FieldCreateCompany]);
            CreateCompanyId = BaseUtil.ConvertToString(dr[FieldCreateCompanyId]);
            ModifiedOn = BaseUtil.ConvertToDateTime(dr[FieldUpdateTime]);
            ModifiedUserId = BaseUtil.ConvertToString(dr[FieldUpdateUserId]);
            ModifiedBy = BaseUtil.ConvertToString(dr[FieldUpdateBy]);
            return this;
        }

        ///<summary>
        /// 联络单主表
        ///</summary>
        [NonSerialized]
        public const string TableName = "BaseContact";

        ///<summary>
        /// 主键
        ///</summary>
        [NonSerialized]
        public const string FieldId = "Id";

        ///<summary>
        /// 父主键
        ///</summary>
        [NonSerialized]
        public const string FieldParentId = "ParentId";

        ///<summary>
        /// 主题
        ///</summary>
        [NonSerialized]
        public const string FieldTitle = "Title";

        ///<summary>
        /// 颜色
        ///</summary>
        [NonSerialized]
        public const string FieldColor = "Color";

        ///<summary>
        /// 样式
        ///</summary>
        [NonSerialized]
        public const string FieldStyle = "Style";

        ///<summary>
        /// 内容
        ///</summary>
        [NonSerialized]
        public const string FieldContents = "Contents";

        ///<summary>
        /// 邮件等级
        ///</summary>
        [NonSerialized]
        public const string FieldPriority = "Priority";

        ///<summary>
        /// 取消置顶期限
        ///</summary>
        [NonSerialized]
        public const string FieldCancelTopDay = "CancelTopDay";

        ///<summary>
        /// 发送邮件总数
        ///</summary>
        [NonSerialized]
        public const string FieldSendCount = "SendCount";

        ///<summary>
        /// 已阅读人数
        ///</summary>
        [NonSerialized]
        public const string FieldReadCount = "ReadCount";

        /// <summary>
        /// 回复数量
        /// </summary>
        [NonSerialized]
        public const string FieldReplyCount = "ReplyCount";

        ///<summary>
        /// 是否公开
        ///</summary>
        [NonSerialized]
        public const string FieldIsOpen = "IsOpen";

        ///<summary>
        /// IP地址
        ///</summary>
        [NonSerialized]
        public const string FieldIpAddress = "IPAddress";

        ///<summary>
        /// 允许评论
        ///</summary>
        [NonSerialized]
        public const string FieldAllowComments = "AllowComments";

        ///<summary>
        /// 必读
        ///</summary>
        [NonSerialized]
        public const string FieldMustRead = "MustRead";

        ///<summary>
        /// 必须回复
        ///</summary>
        [NonSerialized]
        public const string FieldMustReply = "MustReply";

        ///<summary>
        /// 最后评论人主键
        ///</summary>
        [NonSerialized]
        public const string FieldCommentUserId = "CommentUserId";

        ///<summary>
        /// 最后评论人姓名
        ///</summary>
        [NonSerialized]
        public const string FieldCommentUserRealName = "CommentUserRealName";

        ///<summary>
        /// 最后评论时间
        ///</summary>
        [NonSerialized]
        public const string FieldCommentDate = "CommentDate";

        ///<summary>
        /// 是否删除
        ///</summary>
        [NonSerialized]
        public const string FieldDeleted = "DeletionStateCode";

        ///<summary>
        /// 有效
        ///</summary>
        [NonSerialized]
        public const string FieldEnabled = "Enabled";

        ///<summary>
        /// 审核状态
        ///</summary>
        [NonSerialized]
        public const string FieldAuditStatus = "AuditStatus";

        ///<summary>
        /// 审核员主键
        ///</summary>
        [NonSerialized]
        public const string FieldAuditUserId = "AuditUserId";

        ///<summary>
        /// 审核员
        ///</summary>
        [NonSerialized]
        public const string FieldAuditUserRealName = "AuditUserRealName";

        ///<summary>
        /// 排序码
        ///</summary>
        [NonSerialized]
        public const string FieldSortCode = "SortCode";

        ///<summary>
        /// 新闻来源
        ///</summary>
        [NonSerialized]
        public const string FieldSource = "Source";

        ///<summary>
        /// 分类
        ///</summary>
        [NonSerialized]
        public const string FieldCategoryCode = "CategoryCode";

        ///<summary>
        /// 标签
        ///</summary>
        [NonSerialized]
        public const string FieldLabelMark = "LabelMark";

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
        /// 来源部门
        ///</summary>
        [NonSerialized]
        public const string FieldCreateDepartment = "CreateDepartment";

        ///<summary>
        /// 来源公司
        ///</summary>
        [NonSerialized]
        public const string FieldCreateCompany = "CreateCompany";

        ///<summary>
        /// 来源公司主键
        ///</summary>
        [NonSerialized]
        public const string FieldCreateCompanyId = "CreateCompanyId";

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
