//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System;


namespace DotNet.Model
{
    using Util;

    /// <summary>
    /// BaseOrganizeEntity
    /// 组织机构、部门表
    ///
    /// 修改记录
    ///
    ///     2015-01-22 版本：2.0 panqimin 添加FieldDescriptionName，用于修改记录显示字段名称
    ///		2010-07-15 版本：1.0 JiRiGaLa 创建主键。
    ///
    /// <author>
    ///		<name>JiRiGaLa</name>
    ///		<date>2010-07-15</date>
    /// </author>
    /// </summary>
    [Serializable]
    public partial class BaseOrganizeEntity : BaseEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        [FieldDescription("主键", false)]
        public string Id { get; set; } = null;

        /// <summary>
        /// 父级名称
        /// </summary>
        [FieldDescription("父级名称")]
        public string ParentName { get; set; } = null;

        /// <summary>
        /// 父级主键
        /// </summary>
        [FieldDescription("父级主键")]
        public string ParentId { get; set; } = null;

        /// <summary>
        /// 层
        /// </summary>
        [FieldDescription("层")]
        public int? Layer { get; set; } = 0;

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
        /// 标准名称
        /// </summary>
        [FieldDescription("标准名称")]
        public string StandardName { get; set; } = null;

        /// <summary>
        /// 标准编号
        /// </summary>
        [FieldDescription("标准编号")]
        public string StandardCode { get; set; } = null;

        /// <summary>
        /// 快速查询，全拼
        /// </summary>
        [FieldDescription("快速查询，全拼")]
        public string QuickQuery { get; set; } = string.Empty;

        /// <summary>
        /// 快速查询，简拼
        /// </summary>
        [FieldDescription("快速查询，简拼")]
        public string SimpleSpelling { get; set; } = string.Empty;

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

        /// <summary>
        /// 邮编
        /// </summary>
        [FieldDescription("邮编")]
        public string Postalcode { get; set; } = null;

        /// <summary>
        /// 地址
        /// </summary>
        [FieldDescription("地址")]
        public string Address { get; set; } = null;

        /// <summary>
        /// 省主键
        /// </summary>
        [FieldDescription("省主键")]
        public string ProvinceId { get; set; } = null;

        /// <summary>
        /// 省
        /// </summary>
        [FieldDescription("省")]
        public string Province { get; set; } = null;

        /// <summary>
        /// 市主键
        /// </summary>
        [FieldDescription("市主键")]
        public string CityId { get; set; } = null;

        /// <summary>
        /// 市
        /// </summary>
        [FieldDescription("市")]
        public string City { get; set; } = null;

        /// <summary>
        /// 县主键
        /// </summary>
        [FieldDescription("县主键")]
        public string DistrictId { get; set; } = null;

        /// <summary>
        /// 县
        /// </summary>
        [FieldDescription("县")]
        public string District { get; set; } = null;

        /// <summary>
        /// 街道主键
        /// </summary>
        [FieldDescription("街道主键")]
        public string StreetId { get; set; } = null;

        /// <summary>
        /// 街道
        /// </summary>
        [FieldDescription("街道")]
        public string Street { get; set; } = null;

        /// <summary>
        /// 网址
        /// </summary>
        [FieldDescription("网址")]
        public string Web { get; set; } = null;

        /// <summary>
        /// 内部组织机构
        /// </summary>
        [FieldDescription("内部组织机构")]
        public int? IsInnerOrganize { get; set; } = 1;

        /// <summary>
        /// 开户行
        /// </summary>
        [FieldDescription("开户行")]
        public string Bank { get; set; } = null;

        /// <summary>
        /// 银行帐号
        /// </summary>
        [FieldDescription("银行帐号")]
        public string BankAccount { get; set; } = null;

        /// <summary>
        /// 所属公司（各地的分公司，一级网点）主键
        /// </summary>
        [FieldDescription("所属公司（一级网点）主键")]
        public string CompanyId { get; set; } = null;

        /// <summary>
        /// 所属公司（各地的分公司，一级网点）编号
        /// </summary>
        [FieldDescription("所属公司（一级网点）编号")]
        public string CompanyCode { get; set; } = null;

        /// <summary>
        /// 所属公司（各地的分公司，一级网点）名称
        /// </summary>
        [FieldDescription("所属公司（一级网点）名称")]
        public string CompanyName { get; set; } = null;

        /// <summary>
        /// 结算中心主键
        /// </summary>
        [FieldDescription("结算中心主键")]
        public string CostCenterId { get; set; } = null;

        /// <summary>
        /// 结算中心
        /// </summary>
        [FieldDescription("结算中心")]
        public string CostCenter { get; set; } = null;

        /// <summary>
        /// 财务中心主键
        /// </summary>
        [FieldDescription("财务中心主键")]
        public string FinancialCenterId { get; set; } = null;

        /// <summary>
        /// 财务中心
        /// </summary>
        [FieldDescription("财务中心")]
        public string FinancialCenter { get; set; } = null;

        /// <summary>
        /// 所属区域（片区）
        /// </summary>
        [FieldDescription("所属区域（片区）")]
        public string Area { get; set; } = null;

        /// <summary>
        /// 加盟方式
        /// </summary>
        [FieldDescription("加盟方式")]
        public string JoiningMethods { get; set; } = null;

        /// <summary>
        /// 是否删除
        /// </summary>
        [FieldDescription("是否删除", false)]
        public int? DeletionStateCode { get; set; } = 0;

        /// <summary>
        /// 有效
        /// </summary>
        [FieldDescription("是否有效")]
        public int? Enabled { get; set; } = 1;

        /// <summary>
        /// 排序码
        /// </summary>
        [FieldDescription("排序码", false)]
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
        /// 组织架构名称
        /// </summary>
        [FieldDescription("组织架构名称")]
        public string ManageName { get; set; } = null;

        /// <summary>
        /// 组织架构主键
        /// </summary>
        [FieldDescription("组织架构主键")]
        public string ManageId { get; set; } = null;

        /// <summary>
        /// 从数据行读取
        /// </summary>
        /// <param name="dr">数据行</param>
        protected override BaseEntity GetFrom(IDataRow dr)
        {
            Id = BaseUtil.ConvertToString(dr[FieldId]);
            ParentId = BaseUtil.ConvertToString(dr[FieldParentId]);
            ParentName = BaseUtil.ConvertToString(dr[FieldParentName]);
            Layer = BaseUtil.ConvertToInt(dr[FieldLayer]);
            Code = BaseUtil.ConvertToString(dr[FieldCode]);
            FullName = BaseUtil.ConvertToString(dr[FieldFullName]);
            ShortName = BaseUtil.ConvertToString(dr[FieldShortName]);
            StandardName = BaseUtil.ConvertToString(dr[FieldStandardName]);
            StandardCode = BaseUtil.ConvertToString(dr[FieldStandardCode]);

            QuickQuery = BaseUtil.ConvertToString(dr[FieldQuickQuery]);
            SimpleSpelling = BaseUtil.ConvertToString(dr[FieldSimpleSpelling]);
            CategoryCode = BaseUtil.ConvertToString(dr[FieldCategoryCode]);
            OuterPhone = BaseUtil.ConvertToString(dr[FieldOuterPhone]);
            InnerPhone = BaseUtil.ConvertToString(dr[FieldInnerPhone]);
            Fax = BaseUtil.ConvertToString(dr[FieldFax]);
            Postalcode = BaseUtil.ConvertToString(dr[FieldPostalcode]);

            ProvinceId = BaseUtil.ConvertToString(dr[FieldProvinceId]);
            Province = BaseUtil.ConvertToString(dr[FieldProvince]);
            CityId = BaseUtil.ConvertToString(dr[FieldCityId]);
            City = BaseUtil.ConvertToString(dr[FieldCity]);
            DistrictId = BaseUtil.ConvertToString(dr[FieldDistrictId]);
            District = BaseUtil.ConvertToString(dr[FieldDistrict]);
            StreetId = BaseUtil.ConvertToString(dr[FieldStreetId]);
            Street = BaseUtil.ConvertToString(dr[FieldStreet]);

            Address = BaseUtil.ConvertToString(dr[FieldAddress]);
            Web = BaseUtil.ConvertToString(dr[FieldWeb]);
            IsInnerOrganize = BaseUtil.ConvertToInt(dr[FieldIsInnerOrganize]);
            Bank = BaseUtil.ConvertToString(dr[FieldBank]);
            BankAccount = BaseUtil.ConvertToString(dr[FieldBankAccount]);

            CompanyId = BaseUtil.ConvertToString(dr[FieldCompanyId]);
            CompanyCode = BaseUtil.ConvertToString(dr[FieldCompanyCode]);
            CompanyName = BaseUtil.ConvertToString(dr[FieldCompanyName]);

            if (dr.ContainsColumn(FieldCostCenterId))
            {
                CostCenterId = BaseUtil.ConvertToString(dr[FieldCostCenterId]);
            }
            CostCenter = BaseUtil.ConvertToString(dr[FieldCostCenter]);
            if (dr.ContainsColumn(FieldFinancialCenterId))
            {
                FinancialCenterId = BaseUtil.ConvertToString(dr[FieldFinancialCenterId]);
            }
            FinancialCenter = BaseUtil.ConvertToString(dr[FieldFinancialCenter]);
            if (dr.ContainsColumn(FieldManageId))
            {
                ManageId = BaseUtil.ConvertToString(dr[FieldManageId]);
            }
            if (dr.ContainsColumn(FieldManageName))
            {
                ManageName = BaseUtil.ConvertToString(dr[FieldManageName]);
            }
            Area = BaseUtil.ConvertToString(dr[FieldArea]);
            JoiningMethods = BaseUtil.ConvertToString(dr[FieldJoiningMethods]);

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
            // 获取扩展属性
            GetFromExtend(dr);
            return this;
        }

        ///<summary>
        /// 组织机构、部门表
        ///</summary>
        [NonSerialized]
        [FieldDescription("组织机构、部门表")]
        public const string TableName = "BaseOrganize";

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
        /// 父级名称
        ///</summary>
        [NonSerialized]
        [FieldDescription("父级名称")]
        public const string FieldParentName = "ParentName";

        ///<summary>
        /// 层
        ///</summary>
        [NonSerialized]
        [FieldDescription("层")]
        public const string FieldLayer = "Layer";

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

        /// <summary>
        /// 标准名称
        /// </summary>
        [NonSerialized]
        [FieldDescription("标准名称")]
        public const string FieldStandardName = "StandardName";

        /// <summary>
        /// 标准编号
        /// </summary>
        [NonSerialized]
        [FieldDescription("标准编号")]
        public const string FieldStandardCode = "StandardCode";

        ///<summary>
        /// 快速查询，全拼
        ///</summary>
        [NonSerialized]
        [FieldDescription("快速查询，全拼")]
        public const string FieldQuickQuery = "QuickQuery";

        ///<summary>
        /// 快速查询，简拼
        ///</summary>
        [NonSerialized]
        [FieldDescription("快速查询，简拼")]
        public const string FieldSimpleSpelling = "SimpleSpelling";

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
        /// 邮编
        ///</summary>
        [NonSerialized]
        [FieldDescription("邮编")]
        public const string FieldPostalcode = "Postalcode";

        ///<summary>
        /// 省主键
        ///</summary>
        [NonSerialized]
        [FieldDescription("省主键")]
        public const string FieldProvinceId = "ProvinceId";

        ///<summary>
        /// 省
        ///</summary>
        [NonSerialized]
        [FieldDescription("省")]
        public const string FieldProvince = "Province";

        ///<summary>
        /// 市主键
        ///</summary>
        [NonSerialized]
        [FieldDescription("市主键")]
        public const string FieldCityId = "CityId";

        ///<summary>
        /// 市
        ///</summary>
        [NonSerialized]
        [FieldDescription("市")]
        public const string FieldCity = "City";

        ///<summary>
        /// 县主键
        ///</summary>
        [NonSerialized]
        [FieldDescription("县主键")]
        public const string FieldDistrictId = "DistrictId";

        ///<summary>
        /// 县
        ///</summary>
        [NonSerialized]
        [FieldDescription("县")]
        public const string FieldDistrict = "District";

        ///<summary>
        /// 街道键
        ///</summary>
        [NonSerialized]
        [FieldDescription("街道主键")]
        public const string FieldStreetId = "StreetId";

        ///<summary>
        /// 街道
        ///</summary>
        [NonSerialized]
        [FieldDescription("街道")]
        public const string FieldStreet = "Street";

        ///<summary>
        /// 地址
        ///</summary>
        [NonSerialized]
        [FieldDescription("地址")]
        public const string FieldAddress = "Address";

        ///<summary>
        /// 网址
        ///</summary>
        [NonSerialized]
        [FieldDescription("网址")]
        public const string FieldWeb = "Web";

        ///<summary>
        /// 内部组织机构
        ///</summary>
        [NonSerialized]
        [FieldDescription("内部组织机构")]
        public const string FieldIsInnerOrganize = "IsInnerOrganize";

        ///<summary>
        /// 开户行
        ///</summary>
        [NonSerialized]
        [FieldDescription("开户行")]
        public const string FieldBank = "Bank";

        ///<summary>
        /// 银行帐号
        ///</summary>
        [NonSerialized]
        [FieldDescription("银行帐号")]
        public const string FieldBankAccount = "BankAccount";

        /// <summary>
        /// 所属公司（各地的分公司，一级网点）主键
        /// </summary>
        [NonSerialized]
        [FieldDescription("一级网点主键")]
        public const string FieldCompanyId = "CompanyId";

        /// <summary>
        /// 所属公司（各地的分公司，一级网点）编号
        /// </summary>
        [NonSerialized]
        [FieldDescription("一级网点编号")]
        public const string FieldCompanyCode = "CompanyCode";

        /// <summary>
        /// 所属公司（各地的分公司，一级网点）名称
        /// </summary>
        [NonSerialized]
        [FieldDescription("一级网点名称")]
        public const string FieldCompanyName = "CompanyName";

        /// <summary>
        /// 结算中心
        /// </summary>
        [NonSerialized]
        [FieldDescription("结算中心主键")]
        public const string FieldCostCenterId = "CostCenterId";

        /// <summary>
        /// 结算中心
        /// </summary>
        [NonSerialized]
        [FieldDescription("结算中心")]
        public const string FieldCostCenter = "CostCenter";

        /// <summary>
        /// 财务中心主键
        /// </summary>
        [NonSerialized]
        [FieldDescription("财务中心主键")]
        public const string FieldFinancialCenterId = "FinancialCenterId";

        /// <summary>
        /// 财务中心
        /// </summary>
        [NonSerialized]
        [FieldDescription("财务中心")]
        public const string FieldFinancialCenter = "FinancialCenter";

        /// <summary>
        /// 所属区域（片区）
        /// </summary>
        [NonSerialized]
        [FieldDescription("片区")]
        public const string FieldArea = "Area";

        /// <summary>
        /// 加盟方式
        /// </summary>
        [NonSerialized]
        [FieldDescription("加盟方式")]
        public const string FieldJoiningMethods = "JoiningMethods";

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
        /// 组织架构主键
        ///</summary>
        [NonSerialized]
        [FieldDescription("组织架构主键")]
        public const string FieldManageId = "ManageId";

        ///<summary>
        /// 组织架构名称
        ///</summary>
        [NonSerialized]
        [FieldDescription("组织架构名称")]
        public const string FieldManageName = "ManageName";
    }
}
