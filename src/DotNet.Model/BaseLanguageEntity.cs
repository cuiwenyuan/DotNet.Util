//-----------------------------------------------------------------------
// <copyright file="BaseLanguageEntity.cs" company="DotNet">
//     Copyright (C) 2021, All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace DotNet.Model
{
    using Util;

    /// <summary>
    /// BaseLanguageEntity
    /// 多语言
    /// 
    /// 修改记录
    /// 
    /// 2015-02-25 版本：1.0 JiRiGaLa 创建文件。
    /// 
    /// <author>
    ///     <name>JiRiGaLa</name>
    ///     <date>2015-02-25</date>
    /// </author>
    /// </summary>
    public partial class BaseLanguageEntity : BaseEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 语言编号
        /// </summary>
        public string LanguageCode { get; set; } = string.Empty;
        /// <summary>
        /// 消息编号
        /// </summary>
        public string MessageCode { get; set; } = string.Empty;
        /// <summary>
        /// 解说词
        /// </summary>
        public string Caption { get; set; } = string.Empty;
        /// <summary>
        /// 是否删除
        /// </summary>
        public int DeletionStateCode { get; set; } = 0;
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
        /// 修改时间
        /// </summary>
        public DateTime? ModifiedOn { get; set; } = DateTime.Now;
        /// <summary>
        /// 修改人用户编号
        /// </summary>
        public string ModifiedUserId { get; set; } = string.Empty;
        /// <summary>
        /// 修改人
        /// </summary>
        public string ModifiedBy { get; set; } = string.Empty;

        /// <summary>
        /// 从数据行读取
        /// </summary>
        /// <param name="dr">数据行</param>
        protected override BaseEntity GetFrom(IDataRow dr)
        {
            GetFromExpand(dr);
            Id = BaseUtil.ConvertToInt(dr[FieldId]);
            LanguageCode = BaseUtil.ConvertToString(dr[FieldLanguageCode]);
            MessageCode = BaseUtil.ConvertToString(dr[FieldMessageCode]);
            Caption = BaseUtil.ConvertToString(dr[FieldCaption]);
            DeletionStateCode = BaseUtil.ConvertToInt(dr[FieldDeleted]);
            CreateOn = BaseUtil.ConvertToNullableDateTime(dr[FieldCreateTime]);
            CreateUserId = BaseUtil.ConvertToString(dr[FieldCreateUserId]);
            CreateBy = BaseUtil.ConvertToString(dr[FieldCreateBy]);
            ModifiedOn = BaseUtil.ConvertToNullableDateTime(dr[FieldUpdateTime]);
            ModifiedUserId = BaseUtil.ConvertToString(dr[FieldUpdateUserId]);
            ModifiedBy = BaseUtil.ConvertToString(dr[FieldUpdateBy]);
            return this;
        }

        ///<summary>
        /// 多语言
        ///</summary>
        public const string TableName = "BaseLanguage";

        ///<summary>
        /// 主键
        ///</summary>
        public const string FieldId = "Id";

        ///<summary>
        /// 语言编号
        ///</summary>
        public const string FieldLanguageCode = "LanguageCode";

        ///<summary>
        /// 消息编号
        ///</summary>
        public const string FieldMessageCode = "MessageCode";

        ///<summary>
        /// 解说词
        ///</summary>
        public const string FieldCaption = "Caption";

        ///<summary>
        /// 是否删除
        ///</summary>
        public const string FieldDeleted = "DeletionStateCode";

        ///<summary>
        /// 创建时间
        ///</summary>
        public const string FieldCreateTime = "CreateOn";

        ///<summary>
        /// 创建人用户编号
        ///</summary>
        public const string FieldCreateUserId = "CreateUserId";

        ///<summary>
        /// 创建人
        ///</summary>
        public const string FieldCreateBy = "CreateBy";

        ///<summary>
        /// 修改时间
        ///</summary>
        public const string FieldUpdateTime = "ModifiedOn";

        ///<summary>
        /// 修改人用户编号
        ///</summary>
        public const string FieldUpdateUserId = "ModifiedUserId";

        ///<summary>
        /// 修改人
        ///</summary>
        public const string FieldUpdateBy = "ModifiedBy";
    }
}
