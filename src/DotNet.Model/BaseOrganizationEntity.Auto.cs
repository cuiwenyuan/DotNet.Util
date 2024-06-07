//-----------------------------------------------------------------------
// <copyright file="BaseOrganizationEntity.Auto.cs" company="DotNet">
//     Copyright (c) 2024, All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace DotNet.Model
{
    using Util;

    /// <summary>
    /// BaseOrganizationEntity
    /// 组织机构
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
    public partial class BaseOrganizationEntity : BaseEntity
    {
        /// <summary>
        /// 父级主键
        /// </summary>
        [FieldDescription("父级主键")]
        [Description("父级主键")]
        [Column(FieldParentId)]
        public int ParentId { get; set; } = 0;

        /// <summary>
        /// 父级名称
        /// </summary>
        [FieldDescription("父级名称")]
        [Description("父级名称")]
        [Column(FieldParentName)]
        public string ParentName { get; set; } = string.Empty;

        /// <summary>
        /// 编号
        /// </summary>
        [FieldDescription("编号")]
        [Description("编号")]
        [Column(FieldCode)]
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// 名称
        /// </summary>
        [FieldDescription("名称")]
        [Description("名称")]
        [Column(FieldName)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 简称
        /// </summary>
        [FieldDescription("简称")]
        [Description("简称")]
        [Column(FieldShortName)]
        public string ShortName { get; set; } = string.Empty;

        /// <summary>
        /// 标准名称
        /// </summary>
        [FieldDescription("标准名称")]
        [Description("标准名称")]
        [Column(FieldStandardName)]
        public string StandardName { get; set; } = string.Empty;

        /// <summary>
        /// 标准编号
        /// </summary>
        [FieldDescription("标准编号")]
        [Description("标准编号")]
        [Column(FieldStandardCode)]
        public string StandardCode { get; set; } = string.Empty;

        /// <summary>
        /// 快速查询
        /// </summary>
        [FieldDescription("快速查询")]
        [Description("快速查询")]
        [Column(FieldQuickQuery)]
        public string QuickQuery { get; set; } = string.Empty;

        /// <summary>
        /// 简拼
        /// </summary>
        [FieldDescription("简拼")]
        [Description("简拼")]
        [Column(FieldSimpleSpelling)]
        public string SimpleSpelling { get; set; } = string.Empty;

        /// <summary>
        /// 分类编码
        /// </summary>
        [FieldDescription("分类编码")]
        [Description("分类编码")]
        [Column(FieldCategoryCode)]
        public string CategoryCode { get; set; } = string.Empty;

        /// <summary>
        /// 外线电话
        /// </summary>
        [FieldDescription("外线电话")]
        [Description("外线电话")]
        [Column(FieldOuterPhone)]
        public string OuterPhone { get; set; } = string.Empty;

        /// <summary>
        /// 内线电话
        /// </summary>
        [FieldDescription("内线电话")]
        [Description("内线电话")]
        [Column(FieldInnerPhone)]
        public string InnerPhone { get; set; } = string.Empty;

        /// <summary>
        /// 传真
        /// </summary>
        [FieldDescription("传真")]
        [Description("传真")]
        [Column(FieldFax)]
        public string Fax { get; set; } = string.Empty;

        /// <summary>
        /// 邮编
        /// </summary>
        [FieldDescription("邮编")]
        [Description("邮编")]
        [Column(FieldPostalCode)]
        public string PostalCode { get; set; } = string.Empty;

        /// <summary>
        /// 省名称
        /// </summary>
        [FieldDescription("省名称")]
        [Description("省名称")]
        [Column(FieldProvince)]
        public string Province { get; set; } = string.Empty;

        /// <summary>
        /// 市名称
        /// </summary>
        [FieldDescription("市名称")]
        [Description("市名称")]
        [Column(FieldCity)]
        public string City { get; set; } = string.Empty;

        /// <summary>
        /// 县名称
        /// </summary>
        [FieldDescription("县名称")]
        [Description("县名称")]
        [Column(FieldDistrict)]
        public string District { get; set; } = string.Empty;

        /// <summary>
        /// 所属公司主键
        /// </summary>
        [FieldDescription("所属公司主键")]
        [Description("所属公司主键")]
        [Column(FieldCompanyId)]
        public int CompanyId { get; set; } = 0;

        /// <summary>
        /// 所属公司编号
        /// </summary>
        [FieldDescription("所属公司编号")]
        [Description("所属公司编号")]
        [Column(FieldCompanyCode)]
        public string CompanyCode { get; set; } = string.Empty;

        /// <summary>
        /// 所属公司名称
        /// </summary>
        [FieldDescription("所属公司名称")]
        [Description("所属公司名称")]
        [Column(FieldCompanyName)]
        public string CompanyName { get; set; } = string.Empty;

        /// <summary>
        /// 所属区域
        /// </summary>
        [FieldDescription("所属区域")]
        [Description("所属区域")]
        [Column(FieldArea)]
        public string Area { get; set; } = string.Empty;

        /// <summary>
        /// 成本中心
        /// </summary>
        [FieldDescription("成本中心")]
        [Description("成本中心")]
        [Column(FieldCostCenter)]
        public string CostCenter { get; set; } = string.Empty;

        /// <summary>
        /// 财务中心
        /// </summary>
        [FieldDescription("财务中心")]
        [Description("财务中心")]
        [Column(FieldFinancialCenter)]
        public string FinancialCenter { get; set; } = string.Empty;

        /// <summary>
        /// 地址
        /// </summary>
        [FieldDescription("地址")]
        [Description("地址")]
        [Column(FieldAddress)]
        public string Address { get; set; } = string.Empty;

        /// <summary>
        /// 网址
        /// </summary>
        [FieldDescription("网址")]
        [Description("网址")]
        [Column(FieldWeb)]
        public string Web { get; set; } = string.Empty;

        /// <summary>
        /// 开户行
        /// </summary>
        [FieldDescription("开户行")]
        [Description("开户行")]
        [Column(FieldBank)]
        public string Bank { get; set; } = string.Empty;

        /// <summary>
        /// 银行帐号
        /// </summary>
        [FieldDescription("银行帐号")]
        [Description("银行帐号")]
        [Column(FieldBankAccount)]
        public string BankAccount { get; set; } = string.Empty;

        /// <summary>
        /// 层
        /// </summary>
        [FieldDescription("层")]
        [Description("层")]
        [Column(FieldLayer)]
        public int? Layer { get; set; } = null;

        /// <summary>
        /// 百度经度
        /// </summary>
        [FieldDescription("百度经度")]
        [Description("百度经度")]
        [Column(FieldLongitude)]
        public string Longitude { get; set; } = string.Empty;

        /// <summary>
        /// 百度维度
        /// </summary>
        [FieldDescription("百度维度")]
        [Description("百度维度")]
        [Column(FieldLatitude)]
        public string Latitude { get; set; } = string.Empty;

        /// <summary>
        /// 是否有子节点
        /// </summary>
        [FieldDescription("是否有子节点")]
        [Description("是否有子节点")]
        [Column(FieldContainChildNodes)]
        public int ContainChildNodes { get; set; } = 0;

        /// <summary>
        /// 是否内部组织机构
        /// </summary>
        [FieldDescription("是否内部组织机构")]
        [Description("是否内部组织机构")]
        [Column(FieldIsInnerOrganization)]
        public int IsInnerOrganization { get; set; } = 1;

        /// <summary>
        /// 省主键
        /// </summary>
        [FieldDescription("省主键")]
        [Description("省主键")]
        [Column(FieldProvinceId)]
        public int? ProvinceId { get; set; } = null;

        /// <summary>
        /// 市主键
        /// </summary>
        [FieldDescription("市主键")]
        [Description("市主键")]
        [Column(FieldCityId)]
        public int? CityId { get; set; } = null;

        /// <summary>
        /// 县主键
        /// </summary>
        [FieldDescription("县主键")]
        [Description("县主键")]
        [Column(FieldDistrictId)]
        public int? DistrictId { get; set; } = null;

        /// <summary>
        /// 街道主键
        /// </summary>
        [FieldDescription("街道主键")]
        [Description("街道主键")]
        [Column(FieldStreetId)]
        public int? StreetId { get; set; } = null;

        /// <summary>
        /// 街道名称
        /// </summary>
        [FieldDescription("街道名称")]
        [Description("街道名称")]
        [Column(FieldStreet)]
        public string Street { get; set; } = string.Empty;

        /// <summary>
        /// 成本中心主键
        /// </summary>
        [FieldDescription("成本中心主键")]
        [Description("成本中心主键")]
        [Column(FieldCostCenterId)]
        public string CostCenterId { get; set; } = string.Empty;

        /// <summary>
        /// 财务中心主键
        /// </summary>
        [FieldDescription("财务中心主键")]
        [Description("财务中心主键")]
        [Column(FieldFinancialCenterId)]
        public string FinancialCenterId { get; set; } = string.Empty;

        /// <summary>
        /// 领导
        /// </summary>
        [FieldDescription("领导")]
        [Description("领导")]
        [Column(FieldLeader)]
        public string Leader { get; set; } = string.Empty;

        /// <summary>
        /// 领导手机
        /// </summary>
        [FieldDescription("领导手机")]
        [Description("领导手机")]
        [Column(FieldLeaderMobile)]
        public string LeaderMobile { get; set; } = string.Empty;

        /// <summary>
        /// 经理
        /// </summary>
        [FieldDescription("经理")]
        [Description("经理")]
        [Column(FieldManager)]
        public string Manager { get; set; } = string.Empty;

        /// <summary>
        /// 经理手机
        /// </summary>
        [FieldDescription("经理手机")]
        [Description("经理手机")]
        [Column(FieldManagerMobile)]
        public string ManagerMobile { get; set; } = string.Empty;

        /// <summary>
        /// 紧急联系电话
        /// </summary>
        [FieldDescription("紧急联系电话")]
        [Description("紧急联系电话")]
        [Column(FieldEmergencyCall)]
        public string EmergencyCall { get; set; } = string.Empty;

        /// <summary>
        /// 业务咨询电话
        /// </summary>
        [FieldDescription("业务咨询电话")]
        [Description("业务咨询电话")]
        [Column(FieldBusinessPhone)]
        public string BusinessPhone { get; set; } = string.Empty;

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
            if (dr.ContainsColumn(FieldParentId))
            {
                ParentId = BaseUtil.ConvertToInt(dr[FieldParentId]);
            }
            if (dr.ContainsColumn(FieldParentName))
            {
                ParentName = BaseUtil.ConvertToString(dr[FieldParentName]);
            }
            if (dr.ContainsColumn(FieldCode))
            {
                Code = BaseUtil.ConvertToString(dr[FieldCode]);
            }
            if (dr.ContainsColumn(FieldName))
            {
                Name = BaseUtil.ConvertToString(dr[FieldName]);
            }
            if (dr.ContainsColumn(FieldShortName))
            {
                ShortName = BaseUtil.ConvertToString(dr[FieldShortName]);
            }
            if (dr.ContainsColumn(FieldStandardName))
            {
                StandardName = BaseUtil.ConvertToString(dr[FieldStandardName]);
            }
            if (dr.ContainsColumn(FieldStandardCode))
            {
                StandardCode = BaseUtil.ConvertToString(dr[FieldStandardCode]);
            }
            if (dr.ContainsColumn(FieldQuickQuery))
            {
                QuickQuery = BaseUtil.ConvertToString(dr[FieldQuickQuery]);
            }
            if (dr.ContainsColumn(FieldSimpleSpelling))
            {
                SimpleSpelling = BaseUtil.ConvertToString(dr[FieldSimpleSpelling]);
            }
            if (dr.ContainsColumn(FieldCategoryCode))
            {
                CategoryCode = BaseUtil.ConvertToString(dr[FieldCategoryCode]);
            }
            if (dr.ContainsColumn(FieldOuterPhone))
            {
                OuterPhone = BaseUtil.ConvertToString(dr[FieldOuterPhone]);
            }
            if (dr.ContainsColumn(FieldInnerPhone))
            {
                InnerPhone = BaseUtil.ConvertToString(dr[FieldInnerPhone]);
            }
            if (dr.ContainsColumn(FieldFax))
            {
                Fax = BaseUtil.ConvertToString(dr[FieldFax]);
            }
            if (dr.ContainsColumn(FieldPostalCode))
            {
                PostalCode = BaseUtil.ConvertToString(dr[FieldPostalCode]);
            }
            if (dr.ContainsColumn(FieldProvince))
            {
                Province = BaseUtil.ConvertToString(dr[FieldProvince]);
            }
            if (dr.ContainsColumn(FieldCity))
            {
                City = BaseUtil.ConvertToString(dr[FieldCity]);
            }
            if (dr.ContainsColumn(FieldDistrict))
            {
                District = BaseUtil.ConvertToString(dr[FieldDistrict]);
            }
            if (dr.ContainsColumn(FieldCompanyId))
            {
                CompanyId = BaseUtil.ConvertToInt(dr[FieldCompanyId]);
            }
            if (dr.ContainsColumn(FieldCompanyCode))
            {
                CompanyCode = BaseUtil.ConvertToString(dr[FieldCompanyCode]);
            }
            if (dr.ContainsColumn(FieldCompanyName))
            {
                CompanyName = BaseUtil.ConvertToString(dr[FieldCompanyName]);
            }
            if (dr.ContainsColumn(FieldArea))
            {
                Area = BaseUtil.ConvertToString(dr[FieldArea]);
            }
            if (dr.ContainsColumn(FieldCostCenter))
            {
                CostCenter = BaseUtil.ConvertToString(dr[FieldCostCenter]);
            }
            if (dr.ContainsColumn(FieldFinancialCenter))
            {
                FinancialCenter = BaseUtil.ConvertToString(dr[FieldFinancialCenter]);
            }
            if (dr.ContainsColumn(FieldAddress))
            {
                Address = BaseUtil.ConvertToString(dr[FieldAddress]);
            }
            if (dr.ContainsColumn(FieldWeb))
            {
                Web = BaseUtil.ConvertToString(dr[FieldWeb]);
            }
            if (dr.ContainsColumn(FieldBank))
            {
                Bank = BaseUtil.ConvertToString(dr[FieldBank]);
            }
            if (dr.ContainsColumn(FieldBankAccount))
            {
                BankAccount = BaseUtil.ConvertToString(dr[FieldBankAccount]);
            }
            if (dr.ContainsColumn(FieldLayer))
            {
                Layer = BaseUtil.ConvertToNullableInt(dr[FieldLayer]);
            }
            if (dr.ContainsColumn(FieldLongitude))
            {
                Longitude = BaseUtil.ConvertToString(dr[FieldLongitude]);
            }
            if (dr.ContainsColumn(FieldLatitude))
            {
                Latitude = BaseUtil.ConvertToString(dr[FieldLatitude]);
            }
            if (dr.ContainsColumn(FieldContainChildNodes))
            {
                ContainChildNodes = BaseUtil.ConvertToInt(dr[FieldContainChildNodes]);
            }
            if (dr.ContainsColumn(FieldIsInnerOrganization))
            {
                IsInnerOrganization = BaseUtil.ConvertToInt(dr[FieldIsInnerOrganization]);
            }
            if (dr.ContainsColumn(FieldProvinceId))
            {
                ProvinceId = BaseUtil.ConvertToNullableInt(dr[FieldProvinceId]);
            }
            if (dr.ContainsColumn(FieldCityId))
            {
                CityId = BaseUtil.ConvertToNullableInt(dr[FieldCityId]);
            }
            if (dr.ContainsColumn(FieldDistrictId))
            {
                DistrictId = BaseUtil.ConvertToNullableInt(dr[FieldDistrictId]);
            }
            if (dr.ContainsColumn(FieldStreetId))
            {
                StreetId = BaseUtil.ConvertToNullableInt(dr[FieldStreetId]);
            }
            if (dr.ContainsColumn(FieldStreet))
            {
                Street = BaseUtil.ConvertToString(dr[FieldStreet]);
            }
            if (dr.ContainsColumn(FieldCostCenterId))
            {
                CostCenterId = BaseUtil.ConvertToString(dr[FieldCostCenterId]);
            }
            if (dr.ContainsColumn(FieldFinancialCenterId))
            {
                FinancialCenterId = BaseUtil.ConvertToString(dr[FieldFinancialCenterId]);
            }
            if (dr.ContainsColumn(FieldLeader))
            {
                Leader = BaseUtil.ConvertToString(dr[FieldLeader]);
            }
            if (dr.ContainsColumn(FieldLeaderMobile))
            {
                LeaderMobile = BaseUtil.ConvertToString(dr[FieldLeaderMobile]);
            }
            if (dr.ContainsColumn(FieldManager))
            {
                Manager = BaseUtil.ConvertToString(dr[FieldManager]);
            }
            if (dr.ContainsColumn(FieldManagerMobile))
            {
                ManagerMobile = BaseUtil.ConvertToString(dr[FieldManagerMobile]);
            }
            if (dr.ContainsColumn(FieldEmergencyCall))
            {
                EmergencyCall = BaseUtil.ConvertToString(dr[FieldEmergencyCall]);
            }
            if (dr.ContainsColumn(FieldBusinessPhone))
            {
                BusinessPhone = BaseUtil.ConvertToString(dr[FieldBusinessPhone]);
            }
            if (dr.ContainsColumn(FieldDescription))
            {
                Description = BaseUtil.ConvertToString(dr[FieldDescription]);
            }
            return this;
        }

        ///<summary>
        /// 组织机构
        ///</summary>
        [FieldDescription("组织机构")]
        public const string CurrentTableName = "BaseOrganization";

        ///<summary>
        /// 父级主键
        ///</summary>
        public const string FieldParentId = "ParentId";

        ///<summary>
        /// 父级名称
        ///</summary>
        public const string FieldParentName = "ParentName";

        ///<summary>
        /// 编号
        ///</summary>
        public const string FieldCode = "Code";

        ///<summary>
        /// 名称
        ///</summary>
        public const string FieldName = "Name";

        ///<summary>
        /// 简称
        ///</summary>
        public const string FieldShortName = "ShortName";

        ///<summary>
        /// 标准名称
        ///</summary>
        public const string FieldStandardName = "StandardName";

        ///<summary>
        /// 标准编号
        ///</summary>
        public const string FieldStandardCode = "StandardCode";

        ///<summary>
        /// 快速查询
        ///</summary>
        public const string FieldQuickQuery = "QuickQuery";

        ///<summary>
        /// 简拼
        ///</summary>
        public const string FieldSimpleSpelling = "SimpleSpelling";

        ///<summary>
        /// 分类编码
        ///</summary>
        public const string FieldCategoryCode = "CategoryCode";

        ///<summary>
        /// 外线电话
        ///</summary>
        public const string FieldOuterPhone = "OuterPhone";

        ///<summary>
        /// 内线电话
        ///</summary>
        public const string FieldInnerPhone = "InnerPhone";

        ///<summary>
        /// 传真
        ///</summary>
        public const string FieldFax = "Fax";

        ///<summary>
        /// 邮编
        ///</summary>
        public const string FieldPostalCode = "PostalCode";

        ///<summary>
        /// 省名称
        ///</summary>
        public const string FieldProvince = "Province";

        ///<summary>
        /// 市名称
        ///</summary>
        public const string FieldCity = "City";

        ///<summary>
        /// 县名称
        ///</summary>
        public const string FieldDistrict = "District";

        ///<summary>
        /// 所属公司主键
        ///</summary>
        public const string FieldCompanyId = "CompanyId";

        ///<summary>
        /// 所属公司编号
        ///</summary>
        public const string FieldCompanyCode = "CompanyCode";

        ///<summary>
        /// 所属公司名称
        ///</summary>
        public const string FieldCompanyName = "CompanyName";

        ///<summary>
        /// 所属区域
        ///</summary>
        public const string FieldArea = "Area";

        ///<summary>
        /// 成本中心
        ///</summary>
        public const string FieldCostCenter = "CostCenter";

        ///<summary>
        /// 财务中心
        ///</summary>
        public const string FieldFinancialCenter = "FinancialCenter";

        ///<summary>
        /// 地址
        ///</summary>
        public const string FieldAddress = "Address";

        ///<summary>
        /// 网址
        ///</summary>
        public const string FieldWeb = "Web";

        ///<summary>
        /// 开户行
        ///</summary>
        public const string FieldBank = "Bank";

        ///<summary>
        /// 银行帐号
        ///</summary>
        public const string FieldBankAccount = "BankAccount";

        ///<summary>
        /// 层
        ///</summary>
        public const string FieldLayer = "Layer";

        ///<summary>
        /// 百度经度
        ///</summary>
        public const string FieldLongitude = "Longitude";

        ///<summary>
        /// 百度维度
        ///</summary>
        public const string FieldLatitude = "Latitude";

        ///<summary>
        /// 是否有子节点
        ///</summary>
        public const string FieldContainChildNodes = "ContainChildNodes";

        ///<summary>
        /// 是否内部组织机构
        ///</summary>
        public const string FieldIsInnerOrganization = "IsInnerOrganization";

        ///<summary>
        /// 省主键
        ///</summary>
        public const string FieldProvinceId = "ProvinceId";

        ///<summary>
        /// 市主键
        ///</summary>
        public const string FieldCityId = "CityId";

        ///<summary>
        /// 县主键
        ///</summary>
        public const string FieldDistrictId = "DistrictId";

        ///<summary>
        /// 街道主键
        ///</summary>
        public const string FieldStreetId = "StreetId";

        ///<summary>
        /// 街道名称
        ///</summary>
        public const string FieldStreet = "Street";

        ///<summary>
        /// 成本中心主键
        ///</summary>
        public const string FieldCostCenterId = "CostCenterId";

        ///<summary>
        /// 财务中心主键
        ///</summary>
        public const string FieldFinancialCenterId = "FinancialCenterId";

        ///<summary>
        /// 领导
        ///</summary>
        public const string FieldLeader = "Leader";

        ///<summary>
        /// 领导手机
        ///</summary>
        public const string FieldLeaderMobile = "LeaderMobile";

        ///<summary>
        /// 经理
        ///</summary>
        public const string FieldManager = "Manager";

        ///<summary>
        /// 经理手机
        ///</summary>
        public const string FieldManagerMobile = "ManagerMobile";

        ///<summary>
        /// 紧急联系电话
        ///</summary>
        public const string FieldEmergencyCall = "EmergencyCall";

        ///<summary>
        /// 业务咨询电话
        ///</summary>
        public const string FieldBusinessPhone = "BusinessPhone";

        ///<summary>
        /// 描述
        ///</summary>
        public const string FieldDescription = "Description";
    }
}
