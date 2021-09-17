//--------------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//--------------------------------------------------------------------

using System;


namespace DotNet.Model
{
    using Util;

    /// <summary>
    /// BaseMessageEntity
    /// 消息表
    /// 
    /// 修改记录
    /// 
    /// 2015-10-12 版本：1.1 JiRiGaLa 增加创建公司。
    /// 2012-07-03 版本：1.0 JiRiGaLa 创建主键。
    /// 
    /// <author>
    ///     <name>Troy.Cui</name>
    ///     <date>2015-10-12</date>
    /// </author>
    /// </summary>
    [Serializable]
    public partial class BaseMessageEntity : BaseEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// 父亲节点主键
        /// </summary>
        public string ParentId { get; set; } = string.Empty;

        /// <summary>
        /// 部门主键
        /// </summary>
        public string ReceiverDepartmentId { get; set; } = string.Empty;

        /// <summary>
        /// 部门名称
        /// </summary>
        public string ReceiverDepartmentName { get; set; } = string.Empty;

        /// <summary>
        /// 接收者主键
        /// </summary>
        public string ReceiverId { get; set; } = string.Empty;

        /// <summary>
        /// 接收着姓名
        /// </summary>
        public string ReceiverRealName { get; set; } = string.Empty;

        /// <summary>
        /// 功能分类主键
        /// </summary>
        public string FunctionCode { get; set; } = "Message";

        /// <summary>
        /// Send发送、Receiver接收
        /// </summary>
        public string CategoryCode { get; set; } = string.Empty;

        /// <summary>
        /// 唯一识别主键
        /// </summary>
        public string ObjectId { get; set; } = string.Empty;

        /// <summary>
        /// 主题
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// 内容
        /// </summary>
        public string Contents { get; set; } = string.Empty;

        /// <summary>
        /// QQ
        /// </summary>
        public string Qq { get; set; } = string.Empty;

        /// <summary>
        /// 电子邮件
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// 电话
        /// </summary>
        public string Telephone { get; set; } = string.Empty;

        /// <summary>
        /// 是否新信息
        /// </summary>
        public int? IsNew { get; set; } = 1;

        /// <summary>
        /// 被阅读次数
        /// </summary>
        public int? ReadCount { get; set; } = 0;

        /// <summary>
        /// 阅读日期
        /// </summary>
        public DateTime? ReadDate { get; set; } = null;

        /// <summary>
        /// 消息的指向网页页面
        /// </summary>
        public string TargetUrl { get; set; } = string.Empty;

        /// <summary>
        /// IP地址
        /// </summary>
        public string IpAddress { get; set; } = string.Empty;

        /// <summary>
        /// 公司主键
        /// </summary>
        public string CreateCompanyId { get; set; } = string.Empty;

        /// <summary>
        /// 公司名称
        /// </summary>
        public string CreateCompanyName { get; set; } = string.Empty;

        /// <summary>
        /// 部门主键
        /// </summary>
        public string CreateDepartmentId { get; set; } = string.Empty;

        /// <summary>
        /// 部门名称
        /// </summary>
        public string CreateDepartmentName { get; set; } = string.Empty;

        /// <summary>
        /// 是否删除
        /// </summary>
        public int? DeletionStateCode { get; set; } = 0;

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateOn { get; set; } = DateTime.Now;

        /// <summary>
        /// 创建人用户编号
        /// </summary>
        public string CreateUserId { get; set; } = string.Empty;

        /// <summary>
        /// 创建人
        /// </summary>
        public string CreateBy { get; set; } = string.Empty;

        /// <summary>
        /// 从数据行读取
        /// </summary>
        /// <param name="dr">数据行</param>
        protected override BaseEntity GetFrom(IDataRow dr)
        {
            Id = BaseUtil.ConvertToString(dr[FieldId]);
            ParentId = BaseUtil.ConvertToString(dr[FieldParentId]);
            ReceiverDepartmentId = BaseUtil.ConvertToString(dr[FieldReceiverDepartmentId]);
            ReceiverDepartmentName = BaseUtil.ConvertToString(dr[FieldReceiverDepartmentName]);
            ReceiverId = BaseUtil.ConvertToString(dr[FieldReceiverId]);
            ReceiverRealName = BaseUtil.ConvertToString(dr[FieldReceiverRealName]);
            FunctionCode = BaseUtil.ConvertToString(dr[FieldFunctionCode]);
            CategoryCode = BaseUtil.ConvertToString(dr[FieldCategoryCode]);
            ObjectId = BaseUtil.ConvertToString(dr[FieldObjectId]);
            Title = BaseUtil.ConvertToString(dr[FieldTitle]);
            Contents = BaseUtil.ConvertToString(dr[FieldContents]);
            Qq = BaseUtil.ConvertToString(dr[FieldQq]);
            Email = BaseUtil.ConvertToString(dr[FieldEmail]);
            Telephone = BaseUtil.ConvertToString(dr[FieldTelephone]);
            IsNew = BaseUtil.ConvertToInt(dr[FieldIsNew]);
            ReadCount = BaseUtil.ConvertToInt(dr[FieldReadCount]);
            ReadDate = BaseUtil.ConvertToNullableDateTime(dr[FieldReadDate]);
            TargetUrl = BaseUtil.ConvertToString(dr[FieldTargetUrl]);
            IpAddress = BaseUtil.ConvertToString(dr[FieldIpAddress]);
            DeletionStateCode = BaseUtil.ConvertToInt(dr[FieldDeleted]);
            CreateCompanyId = BaseUtil.ConvertToString(dr[FieldCreateCompanyId]);
            CreateCompanyName = BaseUtil.ConvertToString(dr[FieldCreateCompanyName]);
            CreateDepartmentId = BaseUtil.ConvertToString(dr[FieldCreateDepartmentId]);
            CreateDepartmentName = BaseUtil.ConvertToString(dr[FieldCreateDepartmentName]);
            CreateUserId = BaseUtil.ConvertToString(dr[FieldCreateUserId]);
            CreateBy = BaseUtil.ConvertToString(dr[FieldCreateBy]);
            CreateOn = BaseUtil.ConvertToNullableDateTime(dr[FieldCreateTime]);
            // 获取扩展属性
            GetFromExtend(dr);
            return this;
        }

        ///<summary>
        /// 消息表
        ///</summary>
        [NonSerialized]
        public const string TableName = "BaseMessage";

        ///<summary>
        /// 主键
        ///</summary>
        [NonSerialized]
        public const string FieldId = "Id";

        ///<summary>
        /// 父亲节点主键
        ///</summary>
        [NonSerialized]
        public const string FieldParentId = "ParentId";

        ///<summary>
        /// 部门主键
        ///</summary>
        [NonSerialized]
        public const string FieldReceiverDepartmentId = "ReceiverDepartmentId";

        ///<summary>
        /// 部门名称
        ///</summary>
        [NonSerialized]
        public const string FieldReceiverDepartmentName = "ReceiverDepartmentName";

        ///<summary>
        /// 接收者主键
        ///</summary>
        [NonSerialized]
        public const string FieldReceiverId = "ReceiverId";

        ///<summary>
        /// 接收着姓名
        ///</summary>
        [NonSerialized]
        public const string FieldReceiverRealName = "ReceiverRealName";

        ///<summary>
        /// 功能分类主键，Send发送、Receiver接收
        ///</summary>
        [NonSerialized]
        public const string FieldFunctionCode = "FunctionCode";

        ///<summary>
        /// 分类主键
        ///</summary>
        [NonSerialized]
        public const string FieldCategoryCode = "CategoryCode";

        ///<summary>
        /// 唯一识别主键
        ///</summary>
        [NonSerialized]
        public const string FieldObjectId = "ObjectId";

        ///<summary>
        /// 主题
        ///</summary>
        [NonSerialized]
        public const string FieldTitle = "Title";

        ///<summary>
        /// 内容
        ///</summary>
        [NonSerialized]
        public const string FieldContents = "Contents";

        ///<summary>
        /// QQ
        ///</summary>
        [NonSerialized]
        public const string FieldQq = "QQ";

        ///<summary>
        /// Email
        ///</summary>
        [NonSerialized]
        public const string FieldEmail = "Email";

        ///<summary>
        /// 电话
        ///</summary>
        [NonSerialized]
        public const string FieldTelephone = "Telephone";

        ///<summary>
        /// 是否新信息
        ///</summary>
        [NonSerialized]
        public const string FieldIsNew = "IsNew";

        ///<summary>
        /// 被阅读次数
        ///</summary>
        [NonSerialized]
        public const string FieldReadCount = "ReadCount";

        ///<summary>
        /// 阅读日期
        ///</summary>
        [NonSerialized]
        public const string FieldReadDate = "ReadDate";

        ///<summary>
        /// 消息的指向网页页面
        ///</summary>
        [NonSerialized]
        public const string FieldTargetUrl = "TargetURL";

        ///<summary>
        /// IP地址
        ///</summary>
        [NonSerialized]
        public const string FieldIpAddress = "IPAddress";

        ///<summary>
        /// 是否删除
        ///</summary>
        [NonSerialized]
        public const string FieldDeleted = "DeletionStateCode";

        ///<summary>
        /// 公司主键
        ///</summary>
        [NonSerialized]
        public const string FieldCreateCompanyId = "CreateCompanyId";

        ///<summary>
        /// 公司名称
        ///</summary>
        [NonSerialized]
        public const string FieldCreateCompanyName = "CreateCompanyName";

        ///<summary>
        /// 部门主键
        ///</summary>
        [NonSerialized]
        public const string FieldCreateDepartmentId = "CreateDepartmentId";

        ///<summary>
        /// 部门名称
        ///</summary>
        [NonSerialized]
        public const string FieldCreateDepartmentName = "CreateDepartmentName";


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
        /// 创建时间
        ///</summary>
        [NonSerialized]
        public const string FieldCreateTime = "CreateOn";
    }
}
