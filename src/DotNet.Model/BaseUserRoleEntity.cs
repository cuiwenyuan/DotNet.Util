//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System;


namespace DotNet.Model
{
    using Util;

    /// <summary>
    /// BaseUserRoleEntity
    /// 用户角色表
    ///
    /// 修改记录
    ///
    ///		2016-05-21 版本：2.0 JiRiGaLa 主键数据类型修改，方便数据同步用的。
    ///		2010-07-15 版本：1.0 JiRiGaLa 创建主键。
    ///
    /// <author>
    ///		<name>JiRiGaLa</name>
    ///		<date>2016-05-21</date>
    /// </author>
    /// </summary>
    [Serializable]
    public partial class BaseUserRoleEntity : BaseEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        public string Id { get; set; } = null;

        /// <summary>
        /// 用户主键
        /// </summary>
        public string UserId { get; set; } = null;

        /// <summary>
        /// 角色主键
        /// </summary>
        public string RoleId { get; set; } = null;

        /// <summary>
        /// 是否有效
        /// </summary>
        public int? Enabled { get; set; } = 1;

        /// <summary>
        /// 是否删除
        /// </summary>
        public int? DeletionStateCode { get; set; } = 0;

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
        /// 从数据行读取
        /// </summary>
        /// <param name="dr">数据行</param>
		protected override BaseEntity GetFrom(IDataRow dr)
        {
            Id = BaseUtil.ConvertToString(dr[FieldId]);
            UserId = BaseUtil.ConvertToString(dr[FieldUserId]);
            //去掉按公司分配用户和角色 2017.12.19 Troy Cui,懒得改数据库表结构了
            //CompanyId = BaseUtil.ConvertToString(dr[FieldCompanyId]);
            RoleId = BaseUtil.ConvertToString(dr[FieldRoleId]);
            Enabled = BaseUtil.ConvertToInt(dr[FieldEnabled]);
            Description = BaseUtil.ConvertToString(dr[FieldDescription]);
            DeletionStateCode = BaseUtil.ConvertToInt(dr[FieldDeleted]);
            CreateOn = BaseUtil.ConvertToNullableDateTime(dr[FieldCreateTime]);
            CreateUserId = BaseUtil.ConvertToString(dr[FieldCreateUserId]);
            CreateBy = BaseUtil.ConvertToString(dr[FieldCreateBy]);
            ModifiedOn = BaseUtil.ConvertToNullableDateTime(dr[FieldUpdateTime]);
            ModifiedUserId = BaseUtil.ConvertToString(dr[FieldUpdateUserId]);
            ModifiedBy = BaseUtil.ConvertToString(dr[FieldUpdateBy]);
            // 获取扩展属性
            GetFromExtend(dr);
            return this;
        }

        ///<summary>
        /// 用户角色表
        ///</summary>
        [NonSerialized]
        public const string TableName = "BaseUserRole";

        ///<summary>
        /// 主键
        ///</summary>
        [NonSerialized]
        public const string FieldId = "Id";

        ///<summary>
        /// 用户主键
        ///</summary>
        [NonSerialized]
        public const string FieldUserId = "UserId";

        //去掉按公司分配用户和角色 2017.12.19 Troy Cui,懒得改数据库表结构了
        /////<summary>
        ///// 公司主键
        /////</summary>
        //[NonSerialized]
        //public const string FieldCompanyId = "CompanyId";

        ///<summary>
        /// 角色主键
        ///</summary>
        [NonSerialized]
        public const string FieldRoleId = "RoleId";

        ///<summary>
        /// 有效
        ///</summary>
        [NonSerialized]
        public const string FieldEnabled = "Enabled";

        ///<summary>
        /// 备注
        ///</summary>
        [NonSerialized]
        public const string FieldDescription = "Description";

        ///<summary>
        /// 是否删除
        ///</summary>
        [NonSerialized]
        public const string FieldDeleted = "DeletionStateCode";

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
