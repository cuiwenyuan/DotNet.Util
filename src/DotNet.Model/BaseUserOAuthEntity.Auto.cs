//-----------------------------------------------------------------------
// <copyright file="BaseUserOAuthEntity.Auto.cs" company="DotNet">
//     Copyright (c) 2025, All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace DotNet.Model
{
    using Util;

    /// <summary>
    /// BaseUserOAuthEntity
    /// 用户OAuth
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
    public partial class BaseUserOAuthEntity : BaseEntity
    {
        /// <summary>
        /// 子系统编码
        /// </summary>
        [FieldDescription("子系统编码")]
        [Description("子系统编码")]
        [Column(FieldSystemCode)]
        public string SystemCode { get; set; } = string.Empty;

        /// <summary>
        /// 用户编号
        /// </summary>
        [FieldDescription("用户编号")]
        [Description("用户编号")]
        [Column(FieldUserId)]
        public int UserId { get; set; }

        /// <summary>
        /// OAuth Name
        /// </summary>
        [FieldDescription("OAuth Name")]
        [Description("OAuth Name")]
        [Column(FieldName)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// OAuth Access Token
        /// </summary>
        [FieldDescription("OAuth Access Token")]
        [Description("OAuth Access Token")]
        [Column(FieldAccessToken)]
        public string AccessToken { get; set; } = string.Empty;

        /// <summary>
        /// OAuth Refresh Token
        /// </summary>
        [FieldDescription("OAuth Refresh Token")]
        [Description("OAuth Refresh Token")]
        [Column(FieldRefreshToken)]
        public string RefreshToken { get; set; } = string.Empty;

        /// <summary>
        /// OAuth OpenId
        /// </summary>
        [FieldDescription("OAuth OpenId")]
        [Description("OAuth OpenId")]
        [Column(FieldOpenId)]
        public string OpenId { get; set; } = string.Empty;

        /// <summary>
        /// OAuth UnionId
        /// </summary>
        [FieldDescription("OAuth UnionId")]
        [Description("OAuth UnionId")]
        [Column(FieldUnionId)]
        public string UnionId { get; set; } = string.Empty;

        /// <summary>
        /// 描述
        /// </summary>
        [FieldDescription("描述")]
        [Description("描述")]
        [Column(FieldDescription)]
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// 从数据行读取
        /// </summary>
        /// <param name="dr">数据行</param>
        protected override BaseEntity GetFrom(IDataRow dr)
        {
            GetFromExtend(dr);
            GetBase(dr);
            if (dr.ContainsColumn(FieldSystemCode))
            {
                SystemCode = BaseUtil.ConvertToString(dr[FieldSystemCode]);
            }
            if (dr.ContainsColumn(FieldUserId))
            {
                UserId = BaseUtil.ConvertToInt(dr[FieldUserId]);
            }
            if (dr.ContainsColumn(FieldName))
            {
                Name = BaseUtil.ConvertToString(dr[FieldName]);
            }
            if (dr.ContainsColumn(FieldAccessToken))
            {
                AccessToken = BaseUtil.ConvertToString(dr[FieldAccessToken]);
            }
            if (dr.ContainsColumn(FieldRefreshToken))
            {
                RefreshToken = BaseUtil.ConvertToString(dr[FieldRefreshToken]);
            }
            if (dr.ContainsColumn(FieldOpenId))
            {
                OpenId = BaseUtil.ConvertToString(dr[FieldOpenId]);
            }
            if (dr.ContainsColumn(FieldUnionId))
            {
                UnionId = BaseUtil.ConvertToString(dr[FieldUnionId]);
            }
            if (dr.ContainsColumn(FieldDescription))
            {
                Description = BaseUtil.ConvertToString(dr[FieldDescription]);
            }
            return this;
        }

        ///<summary>
        /// 用户OAuth
        ///</summary>
        [FieldDescription("用户OAuth")]
        public const string CurrentTableName = "BaseUserOAuth";

        ///<summary>
        /// 表名
        ///</summary>
        public const string CurrentTableDescription = "用户OAuth";

        ///<summary>
        /// 子系统编码
        ///</summary>
        public const string FieldSystemCode = "SystemCode";

        ///<summary>
        /// 用户编号
        ///</summary>
        public const string FieldUserId = "UserId";

        ///<summary>
        /// OAuth Name
        ///</summary>
        public const string FieldName = "Name";

        ///<summary>
        /// OAuth Access Token
        ///</summary>
        public const string FieldAccessToken = "AccessToken";

        ///<summary>
        /// OAuth Refresh Token
        ///</summary>
        public const string FieldRefreshToken = "RefreshToken";

        ///<summary>
        /// OAuth OpenId
        ///</summary>
        public const string FieldOpenId = "OpenId";

        ///<summary>
        /// OAuth UnionId
        ///</summary>
        public const string FieldUnionId = "UnionId";

        ///<summary>
        /// 描述
        ///</summary>
        public const string FieldDescription = "Description";
    }
}
