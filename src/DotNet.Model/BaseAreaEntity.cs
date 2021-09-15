//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System;


namespace DotNet.Model
{
    using Util;

    /// <summary>
    /// BaseAreaEntity
    /// 地区表(省、市、县)
    ///
    /// 修改记录
    /// 
    ///		2015-11-24 版本：2.1 JiRiGaLa Statistics int, 1 区域表， 是否纳入统计 增加。
    ///		2015-03-09 版本：2.0 JiRiGaLa DelayDay 增加。
    ///		2014-02-11 版本：1.0 JiRiGaLa 创建主键。
    ///
    /// <author>
    ///		<name>JiRiGaLa</name>
    ///		<date>2015-03-09</date>
    /// </author>
    /// </summary>
    [Serializable]
    public partial class BaseAreaEntity : BaseEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        [FieldDescription("主键", false)]
        public string Id { get; set; } = null;
        /// <summary>
        /// 父节点主键
        /// </summary>
        [FieldDescription("父节点主键")]
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
        /// 邮编
        /// </summary>
        [FieldDescription("邮编")]
        public string Postalcode { get; set; } = null;
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
        /// 省
        /// </summary>
        [FieldDescription("省")]
        public string Province { get; set; } = null;
        /// <summary>
        /// 市
        /// </summary>
        [FieldDescription("市")]
        public string City { get; set; } = null;
        /// <summary>
        /// 县
        /// </summary>
        [FieldDescription("县")]
        public string District { get; set; } = null;
        /// <summary>
        /// 延迟天数
        /// </summary>
        [FieldDescription("延迟天数")]
        public int DelayDay { get; set; } = 0;
        /// <summary>
        /// 经度
        /// </summary>
        [FieldDescription("经度")]
        public string Longitude { get; set; } = null;
        /// <summary>
        /// 维度
        /// </summary>
        [FieldDescription("维度")]
        public string Latitude { get; set; } = null;
        /// <summary>
        /// 管理网点主键
        /// </summary>
        [FieldDescription("管理网点主键")]
        public string ManageCompanyId { get; set; } = null;
        /// <summary>
        /// 管理网点编号
        /// </summary>
        [FieldDescription("管理网点编号")]
        public string ManageCompanyCode { get; set; } = null;
        /// <summary>
        /// 管理网点
        /// </summary>
        [FieldDescription("管理网点")]
        public string ManageCompany { get; set; } = null;
        /// <summary>
        /// 开通业务
        /// </summary>
        [FieldDescription("开通业务")]
        public int? Opening { get; set; } = 0;
        /// <summary>
        /// 全境派送
        /// </summary>
        [FieldDescription("全境派送")]
        public int? Whole { get; set; } = 0;
        /// <summary>
        /// 揽收
        /// </summary>
        [FieldDescription("揽收")]
        public int? Receive { get; set; } = 0;
        /// <summary>
        /// 发件
        /// </summary>
        [FieldDescription("发件")]
        public int? Send { get; set; } = 0;
        /// <summary>
        /// 层级
        ///    0：虚拟的？
        ///    1：哪个州的？
        ///    2：哪个国家？
        ///    3：哪个大区？
        ///    4：哪个省？
        ///    5：哪个市？
        ///    6：哪个县、区？
        ///    7：哪街道？
        /// </summary>
        [FieldDescription("层级")]
        public int? Layer { get; set; } = 0;
        /// <summary>
        /// 允许到付
        /// </summary>
        [FieldDescription("允许到付")]
        public int? AllowToPay { get; set; } = 0;
        /// <summary>
        /// 允许的最大到付款
        /// </summary>
        [FieldDescription("允许的最大到付款")]
        public int? MaxToPayment { get; set; } = 0;
        /// <summary>
        /// 允许代收
        /// </summary>
        [FieldDescription("允许代收")]
        public int? AllowGoodsPay { get; set; } = 0;
        /// <summary>
        /// 允许的最大代收款
        /// </summary>
        [FieldDescription("允许的最大代收款")]
        public int? MaxGoodsPayment { get; set; } = 0;
        /// <summary>
        /// 手写大头笔
        /// </summary>
        [FieldDescription("手写大头笔")]
        public string Mark { get; set; } = null;
        /// <summary>
        /// 机打大头笔
        /// </summary>
        [FieldDescription("机打大头笔")]
        public string PrintMark { get; set; } = null;
        /// <summary>
        /// 超区、超出业务范围
        /// </summary>
        [FieldDescription("超区、超出业务范围")]
        public int? OutOfRange { get; set; } = 0;
        /// <summary>
        /// 是否删除
        /// </summary>
        [FieldDescription("是否删除")]
        public int? DeletionStateCode { get; set; } = 0;
        /// <summary>
        /// 备注
        /// </summary>
        [FieldDescription("备注")]
        public string Description { get; set; } = null;
        /// <summary>
        /// 开通网络订单
        /// </summary>
        [FieldDescription("开通网络订单")]
        public int NetworkOrders { get; set; } = 0;
        /// <summary>
        /// 有效
        /// </summary>
        [FieldDescription("是否有效")]
        public int? Enabled { get; set; } = 1;
        /// <summary>
        /// 纳入统计
        /// </summary>
        [FieldDescription("纳入统计")]
        public int? Statistics { get; set; } = 1;
        /// <summary>
        /// 排序码
        /// </summary>
        [FieldDescription("排序码")]
        public int? SortCode { get; set; } = 0;
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
        /// 从数据行读取
        /// </summary>
        /// <param name="dr">数据行</param>
        protected override BaseEntity GetFrom(IDataRow dr)
        {
            Id = BaseUtil.ConvertToString(dr[FieldId]);
            ParentId = BaseUtil.ConvertToString(dr[FieldParentId]);
            Code = BaseUtil.ConvertToString(dr[FieldCode]);
            QuickQuery = BaseUtil.ConvertToString(dr[FieldQuickQuery]);
            SimpleSpelling = BaseUtil.ConvertToString(dr[FieldSimpleSpelling]);
            Province = BaseUtil.ConvertToString(dr[FieldProvince]);
            City = BaseUtil.ConvertToString(dr[FieldCity]);
            District = BaseUtil.ConvertToString(dr[FieldDistrict]);
            FullName = BaseUtil.ConvertToString(dr[FieldFullName]);
            ShortName = BaseUtil.ConvertToString(dr[FieldShortName]);
            Postalcode = BaseUtil.ConvertToString(dr[FieldPostalcode]);
            DelayDay = BaseUtil.ConvertToInt(dr[FieldDelayDay]);
            Longitude = BaseUtil.ConvertToString(dr[FieldLongitude]);
            Latitude = BaseUtil.ConvertToString(dr[FieldLatitude]);
            NetworkOrders = BaseUtil.ConvertToInt(dr[FieldNetworkOrders]);
            ManageCompanyId = BaseUtil.ConvertToString(dr[FieldManageCompanyId]);
            ManageCompanyCode = BaseUtil.ConvertToString(dr[FieldManageCompanyCode]);
            ManageCompany = BaseUtil.ConvertToString(dr[FieldManageCompany]);
            Whole = BaseUtil.ConvertToInt(dr[FieldWhole]);
            Receive = BaseUtil.ConvertToInt(dr[FieldReceive]);
            Send = BaseUtil.ConvertToInt(dr[FieldSend]);
            Layer = BaseUtil.ConvertToInt(dr[FieldLayer]);
            Opening = BaseUtil.ConvertToInt(dr[FieldOpening]);
            AllowToPay = BaseUtil.ConvertToInt(dr[FieldAllowToPay]);
            MaxToPayment = BaseUtil.ConvertToInt(dr[FieldMaxToPayment]);
            AllowGoodsPay = BaseUtil.ConvertToInt(dr[FieldAllowGoodsPay]);
            MaxGoodsPayment = BaseUtil.ConvertToInt(dr[FieldMaxGoodsPayment]);
            DeletionStateCode = BaseUtil.ConvertToInt(dr[FieldDeleted]);
            Mark = BaseUtil.ConvertToString(dr[FieldMark]);
            PrintMark = BaseUtil.ConvertToString(dr[FieldPrintMark]);
            OutOfRange = BaseUtil.ConvertToInt(dr[FieldOutOfRange]);
            Description = BaseUtil.ConvertToString(dr[FieldDescription]);
            Enabled = BaseUtil.ConvertToInt(dr[FieldEnabled]);
            Statistics = BaseUtil.ConvertToInt(dr[FieldStatistics]);
            SortCode = BaseUtil.ConvertToInt(dr[FieldSortCode]);
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
        /// 区域表
        ///</summary>
        [NonSerialized]
        [FieldDescription("区域表")]
        public const string TableName = "BaseArea";

        ///<summary>
        /// 主键
        ///</summary>
        [NonSerialized]
        [FieldDescription("主键")]
        public const string FieldId = "Id";

        ///<summary>
        /// 父节点主键
        ///</summary>
        [NonSerialized]
        [FieldDescription("父节点主键")]
        public const string FieldParentId = "ParentId";

        ///<summary>
        /// 编号
        ///</summary>
        [NonSerialized]
        [FieldDescription("编号")]
        public const string FieldCode = "Code";

        ///<summary>
        /// 省
        ///</summary>
        [NonSerialized]
        [FieldDescription("省")]
        public const string FieldProvince = "Province";

        ///<summary>
        /// 市
        ///</summary>
        [NonSerialized]
        [FieldDescription("市")]
        public const string FieldCity = "City";

        ///<summary>
        /// 县
        ///</summary>
        [NonSerialized]
        [FieldDescription("县")]
        public const string FieldDistrict = "District";

        ///<summary>
        /// 名称
        ///</summary>
        [NonSerialized]
        [FieldDescription("名称")]
        public const string FieldFullName = "FullName";

        ///<summary>
        /// 延迟天数
        ///</summary>
        [NonSerialized]
        [FieldDescription("延迟天数")]
        public const string FieldDelayDay = "DelayDay";

        ///<summary>
        /// 邮编
        ///</summary>
        [NonSerialized]
        [FieldDescription("邮编")]
        public const string FieldPostalcode = "Postalcode";

        ///<summary>
        /// 快速查询，全拼
        ///</summary>
        [NonSerialized]
        [FieldDescription("全拼")]
        public const string FieldQuickQuery = "QuickQuery";

        ///<summary>
        /// 快速查询，简拼
        ///</summary>
        [NonSerialized]
        [FieldDescription("简拼")]
        public const string FieldSimpleSpelling = "SimpleSpelling";

        ///<summary>
        /// 简称
        ///</summary>
        [NonSerialized]
        [FieldDescription("简称")]
        public const string FieldShortName = "ShortName";

        /// <summary>
        /// 百度经度
        /// </summary>
        [NonSerialized]
        [FieldDescription("百度经度")]
        public const string FieldLongitude = "Longitude";

        /// <summary>
        /// 百度纬度
        /// </summary>
        [NonSerialized]
        [FieldDescription("百度纬度")]
        public const string FieldLatitude = "Latitude";

        /// <summary>
        /// 开通业务
        /// </summary>
        [NonSerialized]
        [FieldDescription("开通业务")]
        public const string FieldOpening = "Opening";

        ///<summary>
        /// 网络订单
        ///</summary>
        [NonSerialized]
        [FieldDescription("网络订单")]
        public const string FieldNetworkOrders = "NetworkOrders";

        ///<summary>
        /// 管理网点主键
        ///</summary>
        [NonSerialized]
        [FieldDescription("管理网点主键")]
        public const string FieldManageCompanyId = "ManageCompanyId";

        ///<summary>
        /// 管理网点编号
        ///</summary>
        [NonSerialized]
        [FieldDescription("管理网点编号")]
        public const string FieldManageCompanyCode = "ManageCompanyCode";

        ///<summary>
        /// 管理网点
        ///</summary>
        [NonSerialized]
        [FieldDescription("管理网点")]
        public const string FieldManageCompany = "ManageCompany";

        /// <summary>
        /// 全境派送
        /// </summary>
        [NonSerialized]
        [FieldDescription("全境派送")]
        public const string FieldWhole = "Whole";

        /// <summary>
        /// 揽收
        /// </summary>
        [NonSerialized]
        [FieldDescription("揽收")]
        public const string FieldReceive = "Receive";

        /// <summary>
        /// 发件
        /// </summary>
        [NonSerialized]
        [FieldDescription("发件")]
        public const string FieldSend = "Send";

        ///<summary>
        /// 层级
        ///</summary>
        [NonSerialized]
        [FieldDescription("层级")]
        public const string FieldLayer = "Layer";

        /// <summary>
        /// 允许到付
        /// </summary>
        [NonSerialized]
        [FieldDescription("允许到付")]
        public const string FieldAllowToPay = "AllowToPay";

        /// <summary>
        /// 允许的最大到付款
        /// </summary>
        [NonSerialized]
        [FieldDescription("最大到付款")]
        public const string FieldMaxToPayment = "MaxToPayment";

        /// <summary>
        /// 允许代收
        /// </summary>
        [NonSerialized]
        [FieldDescription("允许代收")]
        public const string FieldAllowGoodsPay = "AllowGoodsPay";

        /// <summary>
        /// 允许的最大代收款
        /// </summary>
        [NonSerialized]
        [FieldDescription("最大代收款")]
        public const string FieldMaxGoodsPayment = "MaxGoodsPayment";

        /// <summary>
        /// 手写大头笔
        /// </summary>
        [NonSerialized]
        [FieldDescription("手写大头笔")]
        public const string FieldMark = "Mark";

        /// <summary>
        /// 机打大头笔
        /// </summary>
        [NonSerialized]
        [FieldDescription("机打大头笔")]
        public const string FieldPrintMark = "PrintMark";

        /// <summary>
        /// 超区、超出业务范围
        /// </summary>
        [NonSerialized]
        [FieldDescription("超区")]
        public const string FieldOutOfRange = "OutOfRange";

        ///<summary>
        /// 是否删除
        ///</summary>
        [NonSerialized]
        [FieldDescription("是否删除")]
        public const string FieldDeleted = "DeletionStateCode";

        ///<summary>
        /// 纳入统计
        ///</summary>
        [NonSerialized]
        [FieldDescription("纳入统计")]
        public const string FieldStatistics = "Statistics";

        ///<summary>
        /// 备注
        ///</summary>
        [NonSerialized]
        [FieldDescription("备注")]
        public const string FieldDescription = "Description";

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
    }
}
