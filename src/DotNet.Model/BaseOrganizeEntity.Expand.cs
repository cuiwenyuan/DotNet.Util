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
    ///		2012-05-07 版本：1.0 JiRiGaLa 创建主键。
    ///
    /// 版本：1.0
    ///
    /// <author>
    ///		<name>JiRiGaLa</name>
    ///		<date>2012-05-07</date>
    /// </author>
    /// </summary>
    public partial class BaseOrganizeEntity
    {
        /// <summary>
        /// 统计名称
        /// </summary>
        [FieldDescription("统计名称")]
        public string StatisticalName { get; set; } = null;

        /// <summary>
        /// 体积重比率
        /// </summary>
        [FieldDescription("体积重比率")]
        public int? WeightRatio { get; set; } = 6000;

        /// <summary>
        /// 发航空件
        /// </summary>
        [FieldDescription("发航空件")]
        public int? SendAir { get; set; } = 1;

        /// <summary>
        /// 后台计算到件中转费标识
        /// </summary>
        [FieldDescription("后台计算到件中转费标识")]
        public int? CalculateComeFee { get; set; } = 1;

        /// <summary>
        /// 后台计算收件中转费标识
        /// </summary>
        [FieldDescription("后台计算收件中转费标识")]
        public int? CalculateReceiveFee { get; set; } = 1;

        ///<summary>
        /// 面单结算网点
        ///</summary>
        [FieldDescription("面单结算网点")]
        public string BillBalanceSite { get; set; } = string.Empty;

        ///<summary>
        /// 二级中转费结算中心
        ///</summary>
        [FieldDescription("二级中转费结算中心")]
        public string LevelTwoTransferCenter { get; set; } = string.Empty;

        ///<summary>
        /// 省级网点
        ///</summary>
        [FieldDescription("省级网点")]
        public string ProvinceSite { get; set; } = string.Empty;

        ///<summary>
        /// 大片区
        ///</summary>
        [FieldDescription("大片区")]
        public string BigArea { get; set; } = string.Empty;

        ///<summary>
        /// 有偿派送费率
        ///</summary>
        [FieldDescription("有偿派送费率")]
        public decimal? SendFee { get; set; } = null;

        ///<summary>
        /// 二级中转费率
        ///</summary>
        [FieldDescription("二级中转费率")]
        public decimal? LevelTwoTransferFee { get; set; } = null;

        ///<summary>
        /// 面单补贴费率
        ///</summary>
        [FieldDescription("面单补贴费率")]
        public decimal? BillSubsidy { get; set; } = null;

        ///<summary>
        /// 经理
        ///</summary>
        [FieldDescription("经理")]
        public string Master { get; set; } = string.Empty;

        ///<summary>
        /// 经理手机
        ///</summary>
        [FieldDescription("经理手机")]
        public string MasterMobile { get; set; } = string.Empty;

        ///<summary>
        /// 经理QQ
        ///</summary>
        [FieldDescription("经理QQ")]
        public string MasterQq { get; set; } = string.Empty;

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

        ///<summary>
        /// 紧急联系电话
        ///</summary>
        [FieldDescription("紧急联系电话")]
        public string EmergencyCall { get; set; } = string.Empty;

        ///<summary>
        /// 业务咨询电话
        ///</summary>
        [FieldDescription("业务咨询电话")]
        public string BusinessPhone { get; set; } = string.Empty;

        ///<summary>
        /// 扫描检测余额
        ///</summary>
        [FieldDescription("扫描检测余额")]
        public int? IsCheckBalance { get; set; } = 0;


        /// <summary>
        /// 统计名称
        /// </summary>
        [NonSerialized]
        [FieldDescription("统计名称")]
        public const string FieldStatisticalName = "StatisticalName";

        /// <summary>
        /// 体积重比率
        /// </summary>
        [NonSerialized]
        [FieldDescription("体积重比率")]
        public const string FieldWeightRatio = "WeightRatio";

        ///<summary>
        /// 发航空件
        ///</summary>
        [NonSerialized]
        [FieldDescription("发航空件")]
        public const string FieldSendAir = "SendAir";

        ///<summary>
        /// 后台计算到件中转费标识
        ///</summary>
        [NonSerialized]
        [FieldDescription("后台计算到件中转费标识")]
        public const string FieldCalculateComeFee = "CalculateComeFee";

        ///<summary>
        /// 后台计算收件中转费标识
        ///</summary>
        [NonSerialized]
        [FieldDescription("后台计算收件中转费标识")]
        public const string FieldCalculateReceiveFee = "CalculateReceiveFee";

        ///<summary>
        /// 面单结算网点
        ///</summary>
        [NonSerialized]
        [FieldDescription("面单结算网点")]
        public const string FieldBillBalanceSite = "BillBalanceSite";

        ///<summary>
        /// 二级中转费结算中心
        ///</summary>
        [NonSerialized]
        [FieldDescription("二级中转费结算中心")]
        public const string FieldLevelTwoTransferCenter = "LevelTwoTransferCenter";

        ///<summary>
        /// 省级网点
        ///</summary>
        [NonSerialized]
        [FieldDescription("省级网点")]
        public const string FieldProvinceSite = "ProvinceSite";

        ///<summary>
        /// 大片区
        ///</summary>
        [NonSerialized]
        [FieldDescription("大片区")]
        public const string FieldBigArea = "BigArea";

        ///<summary>
        /// 有偿派送费率
        ///</summary>
        [NonSerialized]
        [FieldDescription("有偿派送费率")]
        public const string FieldSendFee = "SendFee";

        ///<summary>
        /// 二级中转费率
        ///</summary>
        [NonSerialized]
        [FieldDescription("二级中转费率")]
        public const string FieldLevelTwoTransferFee = "LevelTwoTransferFee";

        ///<summary>
        /// 面单补贴费率
        ///</summary>
        [NonSerialized]
        [FieldDescription("面单补贴费率")]
        public const string FieldBillSubsidy = "BillSubsidy";

        ///<summary>
        /// 经理
        ///</summary>
        [NonSerialized]
        [FieldDescription("经理")]
        public const string FieldMaster = "Master";

        ///<summary>
        /// 经理手机
        ///</summary>
        [NonSerialized]
        [FieldDescription("经理手机")]
        public const string FieldMasterMobile = "MasterMobile";

        ///<summary>
        /// 经理QQ
        ///</summary>
        [NonSerialized]
        [FieldDescription("经理QQ")]
        public const string FieldMasterQq = "MasterQQ";

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
        /// 紧急联系电话
        ///</summary>
        [NonSerialized]
        [FieldDescription("紧急联系电话")]
        public const string FieldEmergencyCall = "EmergencyCall";

        ///<summary>
        /// 业务咨询电话
        ///</summary>
        [NonSerialized]
        [FieldDescription("业务咨询电话")]
        public const string FieldBusinessPhone = "BusinessPhone";

        ///<summary>
        /// 扫描检测余额
        ///</summary>
        [NonSerialized]
        [FieldDescription("扫描检测余额")]
        public const string FieldIsCheckBalance = "IsCheckBalance";

        /// <summary>
        /// 从数据行读取
        /// </summary>
        /// <param name="dr">数据行</param>
        public override void GetFromExtend(IDataRow dr)
        {
            if (dr.ContainsColumn(FieldWeightRatio))
            {
                StatisticalName = BaseUtil.ConvertToString(dr[FieldStatisticalName]);
                WeightRatio = BaseUtil.ConvertToInt(dr[FieldWeightRatio]);
                SendAir = BaseUtil.ConvertToInt(dr[FieldSendAir]);
                CalculateComeFee = BaseUtil.ConvertToInt(dr[FieldCalculateComeFee]);
                CalculateReceiveFee = BaseUtil.ConvertToInt(dr[FieldCalculateReceiveFee]);

                BillBalanceSite = BaseUtil.ConvertToString(dr[FieldBillBalanceSite]);
                LevelTwoTransferCenter = BaseUtil.ConvertToString(dr[FieldLevelTwoTransferCenter]);
                ProvinceSite = BaseUtil.ConvertToString(dr[FieldProvinceSite]);
                BigArea = BaseUtil.ConvertToString(dr[FieldBigArea]);

                SendFee = BaseUtil.ConvertToNullableDecimal(dr[FieldSendFee]);
                LevelTwoTransferFee = BaseUtil.ConvertToNullableDecimal(dr[FieldLevelTwoTransferFee]);
                BillSubsidy = BaseUtil.ConvertToNullableDecimal(dr[FieldBillSubsidy]);

                Master = BaseUtil.ConvertToString(dr[FieldMaster]);
                MasterMobile = BaseUtil.ConvertToString(dr[FieldMasterMobile]);
                MasterQq = BaseUtil.ConvertToString(dr[FieldMasterQq]);
                Manager = BaseUtil.ConvertToString(dr[FieldManager]);
                ManagerMobile = BaseUtil.ConvertToString(dr[FieldManagerMobile]);
                ManagerQq = BaseUtil.ConvertToString(dr[FieldManagerQq]);
                EmergencyCall = BaseUtil.ConvertToString(dr[FieldEmergencyCall]);
                BusinessPhone = BaseUtil.ConvertToString(dr[FieldBusinessPhone]);
                IsCheckBalance = BaseUtil.ConvertToInt(dr[FieldIsCheckBalance]);
            }
        }
    }
}
