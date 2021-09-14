//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System;


namespace DotNet.Model
{
    using Util;

    /// <summary>
    /// BaseDepartmentEntity
    /// 部门表
    ///
    /// 修改记录
    ///
    ///		2014-12-08 版本：1.0 JiRiGaLa 创建主键。
    ///
    /// <author>
    ///		<name>JiRiGaLa</name>
    ///		<date>2014-12-08</date>
    /// </author>
    /// </summary>
    [Serializable]
    public partial class BaseDepartmentEntity : BaseEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        [FieldDescription("主键", false)]
        public int? Id { get; set; } = null;
        /// <summary>
        /// 父级主键
        /// </summary>
        [FieldDescription("父级主键")]
        public string ParentId { get; set; } = null;
        /// <summary>
        /// 编号
        /// </summary>
        [FieldDescription("编号")]
        public string Code { get; set; } = null;
        /// <summary>
        /// 名称
        /// </summary>
        [FieldDescription("名称")]
        public string FullName { get; set; } = null;
        /// <summary>
        /// 简称
        /// </summary>
        [FieldDescription("简称")]
        public string ShortName { get; set; } = null;
        /// <summary>
        /// 分类编码
        /// </summary>
        [FieldDescription("分类编码")]
        public string CategoryCode { get; set; } = null;
        /// <summary>
        /// 外线电话
        /// </summary>
        [FieldDescription("外线电话")]
        public string OuterPhone { get; set; } = null;
        /// <summary>
        /// 内线电话
        /// </summary>
        [FieldDescription("内线电话")]
        public string InnerPhone { get; set; } = null;
        /// <summary>
        /// 传真
        /// </summary>
        [FieldDescription("传真")]
        public string Fax { get; set; } = null;
        ///<summary>
        /// 业务负责人
        ///</summary>
        [FieldDescription("业务负责人")]
        public string Manager { get; set; } = string.Empty;
        ///<summary>
        /// 业务负责人手机
        ///</summary>
        [FieldDescription("业务负责人手机")]
        public string ManagerMobile { get; set; } = string.Empty;
        ///<summary>
        /// 业务负责人QQ
        ///</summary>
        [FieldDescription("业务负责人QQ")]
        public string ManagerQq { get; set; } = string.Empty;
        /// <summary>
        /// 是否删除
        /// </summary>
        [FieldDescription("是否删除")]
        public int? DeletionStateCode { get; set; } = 0;
        /// <summary>
        /// 有效
        /// </summary>
        [FieldDescription("是否有效")]
        public int? Enabled { get; set; } = 1;
        /// <summary>
        /// 排序码
        /// </summary>
        [FieldDescription("排序码")]
        public int? SortCode { get; set; } = 0;
        /// <summary>
        /// 备注
        /// </summary>
        [FieldDescription("备注")]
        public string Description { get; set; } = null;
        /// <summary>
        /// 创建时间
        /// </summary>
        [FieldDescription("创建时间", false)]
        public DateTime? CreateOn { get; set; } = DateTime.Now;
        /// <summary>
        /// 创建人用户编号
        /// </summary>
        [FieldDescription("创建人用户编号", false)]
        public string CreateUserId { get; set; } = null;
        /// <summary>
        /// 创建人
        /// </summary>
        [FieldDescription("创建人", false)]
        public string CreateBy { get; set; } = null;
        /// <summary>
        /// 修改时间
        /// </summary>
        [FieldDescription("修改时间", false)]
        public DateTime? ModifiedOn { get; set; } = DateTime.Now;
        /// <summary>
        /// 修改人用户编号
        /// </summary>
        [FieldDescription("修改人用户编号", false)]
        public string ModifiedUserId { get; set; } = null;
        /// <summary>
        /// 修改人
        /// </summary>
        [FieldDescription("修改人", false)]
        public string ModifiedBy { get; set; } = null;
        /// <summary>
        /// 父级名称
        /// </summary>
        [FieldDescription("父级名称")]
        public string ParentName { get; set; } = null;
        /// <summary>
        /// 网点主键
        /// </summary>
        [FieldDescription("网点主键")]
        public string CompanyId { get; set; } = null;
        /// <summary>
        /// 网点名称
        /// </summary>
        [FieldDescription("网点名称")]
        public string CompanyName { get; set; } = null;
        /// <summary>
        /// 网点编号
        /// </summary>
        [FieldDescription("网点编号")]
        public string CompanyCode { get; set; } = null;
        /// <summary>
        /// 分类名称
        /// </summary>
        [FieldDescription("分类名称")]
        public string CategoryName { get; set; } = null;
        /// <summary>
        /// 父级编号
        /// </summary>
        [FieldDescription("父级编号")]
        public string ParentCode { get; set; } = null;
        /// <summary>
        /// 主管主键
        /// </summary>
        [FieldDescription("主管主键")]
        public string ManagerId { get; set; } = null;

        /// <summary>
        /// 从数据行读取
        /// </summary>
        /// <param name="dr">数据行</param>
        protected override BaseEntity GetFrom(IDataRow dr)
        {
            Id = BaseUtil.ConvertToInt(dr[FieldId]);
            ParentId = BaseUtil.ConvertToString(dr[FieldParentId]);
            Code = BaseUtil.ConvertToString(dr[FieldCode]);
            FullName = BaseUtil.ConvertToString(dr[FieldFullName]);
            ShortName = BaseUtil.ConvertToString(dr[FieldShortName]);
            CategoryCode = BaseUtil.ConvertToString(dr[FieldCategoryCode]);
            OuterPhone = BaseUtil.ConvertToString(dr[FieldOuterPhone]);
            InnerPhone = BaseUtil.ConvertToString(dr[FieldInnerPhone]);
            Fax = BaseUtil.ConvertToString(dr[FieldFax]);
            Manager = BaseUtil.ConvertToString(dr[FieldManager]);
            ManagerMobile = BaseUtil.ConvertToString(dr[FieldManagerMobile]);
            ManagerQq = BaseUtil.ConvertToString(dr[FieldManagerQq]);
            DeletionStateCode = BaseUtil.ConvertToInt(dr[FieldDeleted]);
            Enabled = BaseUtil.ConvertToInt(dr[FieldEnabled]);
            SortCode = BaseUtil.ConvertToInt(dr[FieldSortCode]);
            Description = BaseUtil.ConvertToString(dr[FieldDescription]);
            CreateOn = BaseUtil.ConvertToNullableDateTime(dr[FieldCreateTime]);
            CreateUserId = BaseUtil.ConvertToString(dr[FieldCreateUserId]);
            CreateBy = BaseUtil.ConvertToString(dr[FieldCreateBy]);
            ModifiedOn = BaseUtil.ConvertToNullableDateTime(dr[FieldUpdateTime]);
            ModifiedUserId = BaseUtil.ConvertToString(dr[FieldUpdateUserId]);
            ModifiedBy = BaseUtil.ConvertToString(dr[FieldUpdateBy]);
            ParentName = BaseUtil.ConvertToString(dr[FieldParentName]);
            CompanyId = BaseUtil.ConvertToString(dr[FieldCompanyId]);
            CompanyName = BaseUtil.ConvertToString(dr[FieldCompanyName]);
            CompanyCode = BaseUtil.ConvertToString(dr[FieldCompanyCode]);
            CategoryName = BaseUtil.ConvertToString(dr[FieldCategoryName]);
            ParentCode = BaseUtil.ConvertToString(dr[FieldParentCode]);
            ManagerId = BaseUtil.ConvertToString(dr[FieldManagerId]);
            // 获取扩展属性
            GetFromExpand(dr);
            return this;
        }

        ///<summary>
        /// 部门表
        ///</summary>
        [NonSerialized]
        [FieldDescription("部门表")]
        public const string TableName = "BaseDepartment";

        ///<summary>
        /// 主键
        ///</summary>
        [NonSerialized]
        [FieldDescription("主键")]
        public const string FieldId = "Id";

        ///<summary>
        /// 父级主键
        ///</summary>
        [NonSerialized]
        [FieldDescription("父级主键")]
        public const string FieldParentId = "ParentId";

        ///<summary>
        /// 编号
        ///</summary>
        [NonSerialized]
        [FieldDescription("编号")]
        public const string FieldCode = "Code";

        ///<summary>
        /// 简称
        ///</summary>
        [NonSerialized]
        [FieldDescription("简称")]
        public const string FieldShortName = "ShortName";

        ///<summary>
        /// 名称
        ///</summary>
        [NonSerialized]
        [FieldDescription("名称")]
        public const string FieldFullName = "FullName";

        ///<summary>
        /// 分类编号
        ///</summary>
        [NonSerialized]
        [FieldDescription("分类编码")]
        public const string FieldCategoryCode = "CategoryCode";

        ///<summary>
        /// 外线电话
        ///</summary>
        [NonSerialized]
        [FieldDescription("外线电话")]
        public const string FieldOuterPhone = "OuterPhone";

        ///<summary>
        /// 内线电话
        ///</summary>
        [NonSerialized]
        [FieldDescription("内线电话")]
        public const string FieldInnerPhone = "InnerPhone";

        ///<summary>
        /// 传真
        ///</summary>
        [NonSerialized]
        [FieldDescription("传真")]
        public const string FieldFax = "Fax";

        ///<summary>
        /// 业务负责人
        ///</summary>
        [NonSerialized]
        [FieldDescription("业务负责人")]
        public const string FieldManager = "Manager";

        ///<summary>
        /// 业务负责人手机
        ///</summary>
        [NonSerialized]
        [FieldDescription("业务负责人手")]
        public const string FieldManagerMobile = "ManagerMobile";

        ///<summary>
        /// 业务负责人QQ
        ///</summary>
        [NonSerialized]
        [FieldDescription("业务负责人QQ")]
        public const string FieldManagerQq = "ManagerQQ";

        ///<summary>
        /// 是否删除
        ///</summary>
        [NonSerialized]
        [FieldDescription("是否删除")]
        public const string FieldDeleted = "DeletionStateCode";

        ///<summary>
        /// 有效
        ///</summary>
        [NonSerialized]
        [FieldDescription("是否有效")]
        public const string FieldEnabled = "Enabled";

        ///<summary>
        /// 排序码
        ///</summary>
        [NonSerialized]
        [FieldDescription("排序码")]
        public const string FieldSortCode = "SortCode";

        ///<summary>
        /// 备注
        ///</summary>
        [NonSerialized]
        [FieldDescription("备注")]
        public const string FieldDescription = "Description";

        ///<summary>
        /// 创建时间
        ///</summary>
        [NonSerialized]
        [FieldDescription("创建时间")]
        public const string FieldCreateTime = "CreateOn";

        ///<summary>
        /// 创建人用户编号
        ///</summary>
        [NonSerialized]
        [FieldDescription("创建人用户编号")]
        public const string FieldCreateUserId = "CreateUserId";

        ///<summary>
        /// 创建人
        ///</summary>
        [NonSerialized]
        [FieldDescription("创建人")]
        public const string FieldCreateBy = "CreateBy";

        ///<summary>
        /// 修改时间
        ///</summary>
        [NonSerialized]
        [FieldDescription("修改时间")]
        public const string FieldUpdateTime = "ModifiedOn";

        ///<summary>
        /// 修改人用户编号
        ///</summary>
        [NonSerialized]
        [FieldDescription("修改人用户编号")]
        public const string FieldUpdateUserId = "ModifiedUserId";

        ///<summary>
        /// 修改人
        ///</summary>
        [NonSerialized]
        [FieldDescription("修改人")]
        public const string FieldUpdateBy = "ModifiedBy";

        ///<summary>
        /// 父级名称
        ///</summary>
        [NonSerialized]
        [FieldDescription("父级名称")]
        public const string FieldParentName = "PARENTNAME";

        ///<summary>
        /// 网点主键
        ///</summary>
        [NonSerialized]
        [FieldDescription("网点主键")]
        public const string FieldCompanyId = "COMPANYID";

        ///<summary>
        /// 网点名称
        ///</summary>
        [NonSerialized]
        [FieldDescription("网点名称")]
        public const string FieldCompanyName = "COMPANYNAME";

        ///<summary>
        /// 网点编号
        ///</summary>
        [NonSerialized]
        [FieldDescription("网点编号")]
        public const string FieldCompanyCode = "COMPANYCODE";

        ///<summary>
        /// 分类名称
        ///</summary>
        [NonSerialized]
        [FieldDescription("分类名称")]
        public const string FieldCategoryName = "CATEGORYNAME";

        ///<summary>
        /// 父级编号
        ///</summary>
        [NonSerialized]
        [FieldDescription("父级编号")]
        public const string FieldParentCode = "PARENTCODE";

        ///<summary>
        /// 主管主键
        ///</summary>
        [NonSerialized]
        [FieldDescription("主管主键")]
        public const string FieldManagerId = "MANAGERID";

    }
}
