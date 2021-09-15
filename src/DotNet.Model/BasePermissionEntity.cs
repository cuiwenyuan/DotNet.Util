//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System;


namespace DotNet.Model
{
    using Util;

    /// <summary>
    /// BasePermissionEntity
    /// 操作权限表
    ///
    /// 修改记录
    ///
    ///		2016-05-21 版本：2.0 JiRiGaLa 主键数据类型修改，方便数据同步用的。
    ///		2011-03-07 版本：1.1 JiRiGaLa 表名修正。
    ///		2010-07-15 版本：1.0 JiRiGaLa 创建主键。
    ///
    /// <author>
    ///		<name>JiRiGaLa</name>
    ///		<date>2016-05-21</date>
    /// </author>
    /// </summary>
    [Serializable]
	public partial class BasePermissionEntity : BaseEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        public string Id { get; set; } = null;

        /// <summary>
        /// 资源主键
        /// </summary>
        public string ResourceId { get; set; } = null;

        /// <summary>
        /// 资料类别
        /// </summary>
        public string ResourceCategory { get; set; } = null;

        /// <summary>
        /// 权限（菜单模块）主键
        /// </summary>
        public string PermissionId { get; set; } = null;

        /// <summary>
        /// 公司主键
        /// </summary>
        public string CompanyId { get; set; } = null;

        /// <summary>
        /// 公司名称
        /// </summary>
        public string CompanyName { get; set; } = string.Empty;

        /// <summary>
        /// 权限条件限制
        /// </summary>
        public string PermissionConstraint { get; set; } = null;

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
        /// 创建人
        /// </summary>
        public string CreateBy { get; set; } = null;

        /// <summary>
        /// 创建人用户编号
        /// </summary>
        public string CreateUserId { get; set; } = null;

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime? ModifiedOn { get; set; } = DateTime.Now;

        /// <summary>
        /// 修改人
        /// </summary>
        public string ModifiedBy { get; set; } = null;

        /// <summary>
        /// 修改人用户编号
        /// </summary>
        public string ModifiedUserId { get; set; } = null;

        /// <summary>
        /// 从数据行读取
        /// </summary>
        /// <param name="dr">数据行</param>
		protected override BaseEntity GetFrom(IDataRow dr)
        {
            Id = BaseUtil.ConvertToString(dr[FieldId]);
            ResourceId = BaseUtil.ConvertToString(dr[FieldResourceId]);
            ResourceCategory = BaseUtil.ConvertToString(dr[FieldResourceCategory]);
            PermissionId = BaseUtil.ConvertToString(dr[FieldPermissionId]);
            PermissionConstraint = BaseUtil.ConvertToString(dr[FieldPermissionConstraint]);
            CompanyId = BaseUtil.ConvertToString(dr[FieldCompanyId]);
            CompanyName = BaseUtil.ConvertToString(dr[FieldCompanyName]);
            Enabled = BaseUtil.ConvertToInt(dr[FieldEnabled]);
            DeletionStateCode = BaseUtil.ConvertToInt(dr[FieldDeleted]);
            Description = BaseUtil.ConvertToString(dr[FieldDescription]);
            CreateOn = BaseUtil.ConvertToNullableDateTime(dr[FieldCreateTime]);
            CreateBy = BaseUtil.ConvertToString(dr[FieldCreateBy]);
            CreateUserId = BaseUtil.ConvertToString(dr[FieldCreateUserId]);
            ModifiedOn = BaseUtil.ConvertToNullableDateTime(dr[FieldUpdateTime]);
            ModifiedBy = BaseUtil.ConvertToString(dr[FieldUpdateBy]);
            ModifiedUserId = BaseUtil.ConvertToString(dr[FieldUpdateUserId]);
            // 获取扩展属性
            GetFromExtend(dr);
            return this;
        }

        ///<summary>
        /// 操作权限表
        ///</summary>
        [NonSerialized]
        public const string TableName = "BasePermission";

        ///<summary>
        /// 主键
        ///</summary>
        [NonSerialized]
        public const string FieldId = "Id";

        ///<summary>
        /// 资源主键
        ///</summary>
        [NonSerialized]
        public const string FieldResourceId = "ResourceId";

        ///<summary>
        /// 资料类别
        ///</summary>
        [NonSerialized]
        public const string FieldResourceCategory = "ResourceCategory";

        ///<summary>
        /// 权限主键
        ///</summary>
        [NonSerialized]
        public const string FieldPermissionId = "PermissionId";

        ///<summary>
        /// 权限条件限制
        ///</summary>
        [NonSerialized]
        public const string FieldPermissionConstraint = "PermissionConstraint";

        ///<summary>
        /// 公司主键
        ///</summary>
        [NonSerialized]
        public const string FieldCompanyId = "CompanyId";

        ///<summary>
        /// 公司名称
        ///</summary>
        [NonSerialized]
        public const string FieldCompanyName = "CompanyName";

        ///<summary>
        /// 有效
        ///</summary>
        [NonSerialized]
        public const string FieldEnabled = "Enabled";

        ///<summary>
        /// 是否删除
        ///</summary>
        [NonSerialized]
        public const string FieldDeleted = "DeletionStateCode";

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
        /// 创建人
        ///</summary>
        [NonSerialized]
        public const string FieldCreateBy = "CreateBy";

        ///<summary>
        /// 创建人用户编号
        ///</summary>
        [NonSerialized]
        public const string FieldCreateUserId = "CreateUserId";

        ///<summary>
        /// 修改时间
        ///</summary>
        [NonSerialized]
        public const string FieldUpdateTime = "ModifiedOn";

        ///<summary>
        /// 修改人
        ///</summary>
        [NonSerialized]
        public const string FieldUpdateBy = "ModifiedBy";

        ///<summary>
        /// 修改人用户编号
        ///</summary>
        [NonSerialized]
        public const string FieldUpdateUserId = "ModifiedUserId";
    }
}
