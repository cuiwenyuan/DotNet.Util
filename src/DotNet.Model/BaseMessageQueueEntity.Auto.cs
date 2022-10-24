//-----------------------------------------------------------------------
// <copyright file="BaseMessageQueueEntity.Auto.cs" company="DotNet">
//     Copyright (c) 2022, All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace DotNet.Model
{
    using Util;

    /// <summary>
    /// BaseMessageQueueEntity
    /// 消息队列
    /// 
    /// 修改记录
    /// 
    /// 2022-10-23 版本：1.0 Troy.Cui 创建文件。
    /// 
    /// <author>
    ///     <name>Troy.Cui</name>
    ///     <date>2022-10-23</date>
    /// </author>
    /// </summary>
    [Table(CurrentTableName)]
    public partial class BaseMessageQueueEntity : BaseEntity
    {
        /// <summary>
        /// 公司编号
        /// </summary>
        [FieldDescription("公司编号")]
        [Description("公司编号")]
        [Column(FieldUserCompanyId)]
        public int UserCompanyId { get; set; } = 0;

        /// <summary>
        /// 子公司编号
        /// </summary>
        [FieldDescription("子公司编号")]
        [Description("子公司编号")]
        [Column(FieldUserSubCompanyId)]
        public int UserSubCompanyId { get; set; } = 0;

        /// <summary>
        /// 来源
        /// </summary>
        [FieldDescription("来源")]
        [Description("来源")]
        [Column(FieldSource)]
        public string Source { get; set; } = string.Empty;

        /// <summary>
        /// 消息类型
        /// </summary>
        [FieldDescription("消息类型")]
        [Description("消息类型")]
        [Column(FieldMessageType)]
        public string MessageType { get; set; } = "Email";

        /// <summary>
        /// 接收人
        /// </summary>
        [FieldDescription("接收人")]
        [Description("接收人")]
        [Column(FieldRecipient)]
        public string Recipient { get; set; } = string.Empty;

        /// <summary>
        /// 主题
        /// </summary>
        [FieldDescription("主题")]
        [Description("主题")]
        [Column(FieldSubject)]
        public string Subject { get; set; } = string.Empty;

        /// <summary>
        /// 正文内容
        /// </summary>
        [FieldDescription("正文内容")]
        [Description("正文内容")]
        [Column(FieldBody)]
        public string Body { get; set; } = string.Empty;

        /// <summary>
        /// 失败次数
        /// </summary>
        [FieldDescription("失败次数")]
        [Description("失败次数")]
        [Column(FieldFailCount)]
        public int? FailCount { get; set; } = 0;

        /// <summary>
        /// 从数据行读取
        /// </summary>
        /// <param name="dr">数据行</param>
        protected override BaseEntity GetFrom(IDataRow dr)
        {
            GetFromExtend(dr);
            GetBase(dr);
            if (dr.ContainsColumn(FieldUserCompanyId))
            {
                UserCompanyId = BaseUtil.ConvertToInt(dr[FieldUserCompanyId]);
            }
            if (dr.ContainsColumn(FieldUserSubCompanyId))
            {
                UserSubCompanyId = BaseUtil.ConvertToInt(dr[FieldUserSubCompanyId]);
            }
            if (dr.ContainsColumn(FieldSource))
            {
                Source = BaseUtil.ConvertToString(dr[FieldSource]);
            }
            if (dr.ContainsColumn(FieldMessageType))
            {
                MessageType = BaseUtil.ConvertToString(dr[FieldMessageType]);
            }
            if (dr.ContainsColumn(FieldRecipient))
            {
                Recipient = BaseUtil.ConvertToString(dr[FieldRecipient]);
            }
            if (dr.ContainsColumn(FieldSubject))
            {
                Subject = BaseUtil.ConvertToString(dr[FieldSubject]);
            }
            if (dr.ContainsColumn(FieldBody))
            {
                Body = BaseUtil.ConvertToString(dr[FieldBody]);
            }
            if (dr.ContainsColumn(FieldFailCount))
            {
                FailCount = BaseUtil.ConvertToNullableInt(dr[FieldFailCount]);
            }
            return this;
        }

        ///<summary>
        /// 消息队列
        ///</summary>
        [FieldDescription("消息队列")]
        public const string CurrentTableName = "BaseMessageQueue";

        ///<summary>
        /// 公司编号
        ///</summary>
        public const string FieldUserCompanyId = "UserCompanyId";

        ///<summary>
        /// 子公司编号
        ///</summary>
        public const string FieldUserSubCompanyId = "UserSubCompanyId";

        ///<summary>
        /// 来源
        ///</summary>
        public const string FieldSource = "Source";

        ///<summary>
        /// 消息类型
        ///</summary>
        public const string FieldMessageType = "MessageType";

        ///<summary>
        /// 接收人
        ///</summary>
        public const string FieldRecipient = "Recipient";

        ///<summary>
        /// 主题
        ///</summary>
        public const string FieldSubject = "Subject";

        ///<summary>
        /// 正文内容
        ///</summary>
        public const string FieldBody = "Body";

        ///<summary>
        /// 失败次数
        ///</summary>
        public const string FieldFailCount = "FailCount";
    }
}
