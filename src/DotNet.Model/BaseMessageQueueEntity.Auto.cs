//-----------------------------------------------------------------------
// <copyright file="BaseMessageQueueEntity.Auto.cs" company="DotNet">
//     Copyright (c) 2021, All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace DotNet.Model
{
    using Util;

    /// <summary>
    /// BaseMessageQueueEntity
    /// 消息队列
    /// 
    /// 修改记录
    /// 
    /// 2021-09-27 版本：2.0 Troy.Cui 创建文件。
    /// 2020-03-22 版本：1.0 Troy.Cui 创建文件。
    /// 
    /// <author>
    ///     <name>Troy.Cui</name>
    ///     <date>2021-09-27</date>
    /// </author>
    /// </summary>
    public partial class BaseMessageQueueEntity : BaseEntity
    {
        /// <summary>
        /// 公司编号
        /// </summary>
        [FieldDescription("公司编号")]
        public int UserCompanyId { get; set; } = 0;

        /// <summary>
        /// 子公司编号
        /// </summary>
        [FieldDescription("子公司编号")]
        public int UserSubCompanyId { get; set; } = 0;

        /// <summary>
        /// 来源
        /// </summary>
        [FieldDescription("来源")]
        public string Source { get; set; } = string.Empty;

        /// <summary>
        /// 消息类型
        /// </summary>
        [FieldDescription("消息类型")]
        public string MessageType { get; set; } = "Email";

        /// <summary>
        /// 接收人
        /// </summary>
        [FieldDescription("接收人")]
        public string Recipient { get; set; } = string.Empty;

        /// <summary>
        /// 主题
        /// </summary>
        [FieldDescription("主题")]
        public string Subject { get; set; } = string.Empty;

        /// <summary>
        /// 正文内容
        /// </summary>
        [FieldDescription("正文内容")]
        public string Body { get; set; } = string.Empty;

        /// <summary>
        /// 失败次数
        /// </summary>
        [FieldDescription("失败次数")]
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
