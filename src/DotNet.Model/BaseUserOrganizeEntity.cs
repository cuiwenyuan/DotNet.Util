//--------------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//--------------------------------------------------------------------

using System;


namespace DotNet.Model
{
    using Util;

    /// <summary>
    /// BaseUserOrganizeEntity
    /// 用户组织关系（兼任）表
    /// 
    /// 修改记录
    /// 
    /// 2012-07-27 版本：1.0 JiRiGaLa 创建主键。
    /// 
    /// <author>
    ///     <name>JiRiGaLa</name>
    ///     <date>2012-07-27</date>
    /// </author>
    /// </summary>
    [Serializable]
	public partial class BaseUserOrganizeEntity : BaseEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        
        public string Id { get; set; } = null;
        /// <summary>
        /// 用户账户主键
        /// </summary>
        
        public string UserId { get; set; } = null;
        /// <summary>
        /// 公司主键，数据库中可以设置为int
        /// </summary>
        
        public string CompanyId { get; set; } = string.Empty;
        /// <summary>
        /// 分支机构主键，数据库中可以设置为int
        /// </summary>
        
        public string SubCompanyId { get; set; } = string.Empty;
        /// <summary>
        /// 部门主键，数据库中可以设置为int
        /// </summary>
        
        public string DepartmentId { get; set; } = string.Empty;
        /// <summary>
        /// 工作组主键，数据库中可以设置为int
        /// </summary>
        
        public string WorkgroupId { get; set; } = string.Empty;
        /// <summary>
        /// 有效
        /// </summary>
        
        public int? Enabled { get; set; } = 1;
        /// <summary>
        /// 备注
        /// </summary>
        
        public string Description { get; set; } = string.Empty;
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
            Id = BaseUtil.ConvertToString(dr[FieldId]);
            UserId = BaseUtil.ConvertToString(dr[FieldUserId]);
            CompanyId = BaseUtil.ConvertToString(dr[FieldCompanyId]);
            SubCompanyId = BaseUtil.ConvertToString(dr[FieldSubCompanyId]);
            DepartmentId = BaseUtil.ConvertToString(dr[FieldDepartmentId]);
            WorkgroupId = BaseUtil.ConvertToString(dr[FieldWorkgroupId]);
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
            GetFromExpand(dr);
            return this;
        }

        ///<summary>
        /// 用户组织关系表
        ///</summary>
        [NonSerialized]
        public const string TableName = "BaseUserOrganize";

        ///<summary>
        /// 主键
        ///</summary>
        [NonSerialized]
        public const string FieldId = "Id";

        ///<summary>
        /// 用户账户主键
        ///</summary>
        [NonSerialized]
        public const string FieldUserId = "UserId";

        ///<summary>
        /// 公司主键，数据库中可以设置为int
        ///</summary>
        [NonSerialized]
        public const string FieldCompanyId = "CompanyId";

        ///<summary>
        /// 分支机构主键，数据库中可以设置为int
        ///</summary>
        [NonSerialized]
        public const string FieldSubCompanyId = "SubCompanyId";

        ///<summary>
        /// 部门代码，数据库中可以设置为int
        ///</summary>
        [NonSerialized]
        public const string FieldDepartmentId = "DepartmentId";

        ///<summary>
        /// 工作组主键，数据库中可以设置为int
        ///</summary>
        [NonSerialized]
        public const string FieldWorkgroupId = "WorkgroupId";

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
