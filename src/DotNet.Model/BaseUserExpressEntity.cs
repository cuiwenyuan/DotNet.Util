//-----------------------------------------------------------------------
// <copyright file="BaseUserExpressEntity.cs" company="DotNet">
//     Copyright (C) 2021, All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;


namespace DotNet.Model
{
    using Util;

    /// <summary>
    /// BaseUserExpressEntity
    /// 
    /// 
    /// 修改记录
    /// 
    /// 2014-11-08 版本：1.0 JiRiGaLa 创建文件。
    /// 
    /// <author>
    ///     <name>JiRiGaLa</name>
    ///     <date>2014-11-08</date>
    /// </author>
    /// </summary>
    public partial class BaseUserExpressEntity : BaseEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        [FieldDescription("主键", false)]
        public decimal Id { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        [FieldDescription("修改时间", false)]
        public DateTime? ModifiedOn { get; set; } = DateTime.Now;

        /// <summary>
        /// 中转附加费
        /// </summary>
        [FieldDescription("中转附加费")]
        public decimal? TransferAddFee { get; set; } = null;

        /// <summary>
        /// 所属承包区ID
        /// </summary>
        [FieldDescription("所属承包区ID")]
        public decimal? OwnerId { get; set; } = null;

        /// <summary>
        /// 所属承包区
        /// </summary>
        [FieldDescription("所属承包区")]
        public string OwnerRange { get; set; } = string.Empty;

        /// <summary>
        /// 修改人用户编号
        /// </summary>
        [FieldDescription("修改人用户编号", false)]
        public string ModifiedUserId { get; set; } = string.Empty;

        /// <summary>
        /// 派件附加费
        /// </summary>
        [FieldDescription("派件附加费")]
        public decimal? DispatchAddFee { get; set; } = null;

        /// <summary>
        /// 修改人
        /// </summary>
        [FieldDescription("修改人", false)]
        public string ModifiedBy { get; set; } = string.Empty;

        /// <summary>
        /// 创建人
        /// </summary>
        [FieldDescription("创建人", false)]
        public string CreateBy { get; set; } = string.Empty;

        /// <summary>
        /// 创建时间
        /// </summary>
        [FieldDescription("创建时间", false)]
        public DateTime? CreateOn { get; set; } = DateTime.Now;

        /// <summary>
        /// 创建人用户编号
        /// </summary>
        [FieldDescription("创建人用户编号", false)]
        public string CreateUserId { get; set; } = string.Empty;

        /// <summary>
        /// 从数据行读取
        /// </summary>
        /// <param name="dr">数据行</param>
        protected override BaseEntity GetFrom(IDataRow dr)
        {
            Id = BaseUtil.ConvertToDecimal(dr[FieldId]);
            ModifiedOn = BaseUtil.ConvertToNullableDateTime(dr[FieldUpdateTime]);
            TransferAddFee = BaseUtil.ConvertToNullableDecimal(dr[FieldTransferAddFee]);
            OwnerId = BaseUtil.ConvertToDecimal(dr[FieldOwnerId]);
            OwnerRange = BaseUtil.ConvertToString(dr[FieldOwnerRange]);
            ModifiedUserId = BaseUtil.ConvertToString(dr[FieldUpdateUserId]);
            DispatchAddFee = BaseUtil.ConvertToNullableDecimal(dr[FieldDispatchAddFee]);
            ModifiedBy = BaseUtil.ConvertToString(dr[FieldUpdateBy]);
            CreateBy = BaseUtil.ConvertToString(dr[FieldCreateBy]);
            CreateOn = BaseUtil.ConvertToNullableDateTime(dr[FieldCreateTime]);
            CreateUserId = BaseUtil.ConvertToString(dr[FieldCreateUserId]);
            // 获取扩展属性
            GetFromExtend(dr);
            return this;
        }

        ///<summary>
        /// 用户扩展表
        ///</summary>
        [FieldDescription("用户扩展表")]
        public const string TableName = "BaseUser_EXPRESS";

        ///<summary>
        /// 主键
        ///</summary>
        public const string FieldId = "ID";

        ///<summary>
        /// 修改时间
        ///</summary>
        public const string FieldUpdateTime = "ModifiedOn";

        ///<summary>
        /// 中转附加费
        ///</summary>
        public const string FieldTransferAddFee = "TRANSFER_ADD_FEE";

        ///<summary>
        /// 所属承包区ID
        ///</summary>
        public const string FieldOwnerId = "OWNER_ID";

        ///<summary>
        /// 所属承包区
        ///</summary>
        public const string FieldOwnerRange = "OWNER_RANGE";

        ///<summary>
        /// 修改人用户编号
        ///</summary>
        public const string FieldUpdateUserId = "ModifiedUserId";

        ///<summary>
        /// 派件附加费
        ///</summary>
        public const string FieldDispatchAddFee = "DISPATCH_ADD_FEE";

        ///<summary>
        /// 修改人
        ///</summary>
        public const string FieldUpdateBy = "ModifiedBy";

        ///<summary>
        /// 创建人
        ///</summary>
        public const string FieldCreateBy = "CreateBy";

        ///<summary>
        /// 创建时间
        ///</summary>
        public const string FieldCreateTime = "CreateOn";

        ///<summary>
        /// 创建人用户编号
        ///</summary>
        public const string FieldCreateUserId = "CreateUserId";
    }
}