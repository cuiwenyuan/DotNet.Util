//-----------------------------------------------------------------------
// <copyright file="BaseUserOAuthEntity.Auto.cs" company="DotNet">
//     Copyright (c) 2020, All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace DotNet.Model
{
    using Util;

    /// <summary>
    /// BaseUserOAuthEntity
    /// 用户OAuth表
    /// 
    /// 修改纪录
    /// 
    /// 2020-02-13 版本：1.0 Troy.Cui 创建文件。
    /// 
    /// <author>
    ///     <name>Troy.Cui</name>
    ///     <date>2020-02-13</date>
    /// </author>
    /// </summary>
    public partial class BaseUserOAuthEntity : BaseEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        [FieldDescription("主键")]
        public int Id { get; set; }

        /// <summary>
        /// 子系统编号
        /// </summary>
        [FieldDescription("子系统编号")]
        public string SystemCode { get; set; } = string.Empty;

        /// <summary>
        /// 用户编号
        /// </summary>
        [FieldDescription("用户编号")]
        public int UserId { get; set; }

        /// <summary>
        /// OAuth Name
        /// </summary>
        [FieldDescription("OAuth Name")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// OAuth Access Token
        /// </summary>
        [FieldDescription("OAuth Access Token")]
        public string AccessToken { get; set; } = string.Empty;

        /// <summary>
        /// OAuth Refresh Token
        /// </summary>
        [FieldDescription("OAuth Refresh Token")]
        public string RefreshToken { get; set; } = string.Empty;

        /// <summary>
        /// OAuth OpenId
        /// </summary>
        [FieldDescription("OAuth OpenId")]
        public string OpenId { get; set; } = string.Empty;

        /// <summary>
        /// OAuth UnionId
        /// </summary>
        [FieldDescription("OAuth UnionId")]
        public string UnionId { get; set; } = string.Empty;

        /// <summary>
        /// 描述
        /// </summary>
        [FieldDescription("描述")]
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// 有效
        /// </summary>
        [FieldDescription("是否有效")]
        public int Enabled { get; set; } = 1;

        /// <summary>
        /// 是否删除
        /// </summary>
        [FieldDescription("是否删除")]
        public int DeletionStateCode { get; set; } = 0;

        /// <summary>
        /// 创建时间
        /// </summary>
        [FieldDescription("创建时间")]
        public DateTime? CreateOn { get; set; } = DateTime.Now;

        /// <summary>
        /// 创建人用户编号
        /// </summary>
        [FieldDescription("创建人用户编号")]
        public string CreateUserId { get; set; } = string.Empty;

        /// <summary>
        /// 创建人
        /// </summary>
        [FieldDescription("创建人")]
        public string CreateBy { get; set; } = string.Empty;

        /// <summary>
        /// 修改时间
        /// </summary>
        [FieldDescription("修改时间")]
        public DateTime? ModifiedOn { get; set; } = DateTime.Now;

        /// <summary>
        /// 修改人用户编号
        /// </summary>
        [FieldDescription("修改人用户编号")]
        public string ModifiedUserId { get; set; } = string.Empty;

        /// <summary>
        /// 修改人
        /// </summary>
        [FieldDescription("修改人")]
        public string ModifiedBy { get; set; } = string.Empty;

        /// <summary>
        /// 从数据行读取
        /// </summary>
        /// <param name="dr">数据行</param>
        protected override BaseEntity GetFrom(IDataRow dr)
        {
            GetFromExpand(dr);
            if (dr.ContainsColumn(FieldId))
            {
                Id = BaseUtil.ConvertToInt(dr[FieldId]);
            }
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
            if (dr.ContainsColumn(FieldEnabled))
            {
                Enabled = BaseUtil.ConvertToInt(dr[FieldEnabled]);
            }
            if (dr.ContainsColumn(FieldDeleted))
            {
                DeletionStateCode = BaseUtil.ConvertToInt(dr[FieldDeleted]);
            }
            if (dr.ContainsColumn(FieldCreateTime))
            {
                CreateOn = BaseUtil.ConvertToNullableDateTime(dr[FieldCreateTime]);
            }
            if (dr.ContainsColumn(FieldCreateUserId))
            {
                CreateUserId = BaseUtil.ConvertToString(dr[FieldCreateUserId]);
            }
            if (dr.ContainsColumn(FieldCreateBy))
            {
                CreateBy = BaseUtil.ConvertToString(dr[FieldCreateBy]);
            }
            if (dr.ContainsColumn(FieldUpdateTime))
            {
                ModifiedOn = BaseUtil.ConvertToNullableDateTime(dr[FieldUpdateTime]);
            }
            if (dr.ContainsColumn(FieldUpdateUserId))
            {
                ModifiedUserId = BaseUtil.ConvertToString(dr[FieldUpdateUserId]);
            }
            if (dr.ContainsColumn(FieldUpdateBy))
            {
                ModifiedBy = BaseUtil.ConvertToString(dr[FieldUpdateBy]);
            }
            return this;
        }

        ///<summary>
        /// 用户OAuth表
        ///</summary>
        [FieldDescription("用户OAuth表")]
        public const string TableName = "BaseUserOAuth";

        ///<summary>
        /// 主键
        ///</summary>
        public const string FieldId = "Id";

        ///<summary>
        /// 子系统编号
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

        ///<summary>
        /// 有效
        ///</summary>
        public const string FieldEnabled = "Enabled";

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
