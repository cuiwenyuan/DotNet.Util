//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System;


namespace DotNet.Model
{
    using Util;

    /// <summary>
    /// BaseOrganizeLogOnEntity
    /// 登录信息表
    ///
    /// 修改记录
    ///
    ///		2016-03-24 版本：1.0 JiRiGaLa 创建主键。
    ///
    /// <author>
    ///		<name>JiRiGaLa</name>
    ///		<date>2016-03-24</date>
    /// </author>
    /// </summary>
    [Serializable]
    public partial class BaseOrganizeLogOnEntity : BaseEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        [FieldDescription("主键", false)]
        public string Id { get; set; } = null;

        /// <summary>
        /// 赞成数
        /// </summary>
        [FieldDescription("赞成", false)]
        public int? Agree { get; set; } = 0;

        /// <summary>
        /// 反对数
        /// </summary>
        [FieldDescription("反对", false)]
        public int? Oppose { get; set; } = 0;

        /// <summary>
        /// 第一次访问时间
        /// </summary>
        [FieldDescription("第一次访问时间", false)]
        public DateTime? FirstVisit { get; set; } = null;

        /// <summary>
        /// 最后访问时间
        /// </summary>
        [FieldDescription("最后访问时间", false)]
        public DateTime? LastVisit { get; set; } = null;

        /// <summary>
        /// 登录次数
        /// </summary>
        [FieldDescription("登录次数", false)]
        public int? LogOnCount { get; set; } = 0;

        /// <summary>
        /// 展示次数
        /// </summary>
        [FieldDescription("展示次数", false)]
        public int? ShowCount { get; set; } = 0;

        /// <summary>
        /// 在线状态
        /// </summary>
        [FieldDescription("在线状态", false)]
        public int? UserOnLine { get; set; } = 0;

        /// <summary>
        /// IP地址
        /// </summary>
        [FieldDescription("IP地址")]
        public string IpAddress { get; set; } = null;

        /// <summary>
        /// MAC地址
        /// </summary>
        [FieldDescription("MAC地址")]
        public string MacAddress { get; set; } = null;

        /// <summary>
        /// 修改时间
        /// </summary>
        [FieldDescription("修改时间", false)]
        public DateTime? ModifiedOn { get; set; } = DateTime.Now;

        /// <summary>
        /// 从数据行读取
        /// </summary>
        /// <param name="dr">数据行</param>
        protected override BaseEntity GetFrom(IDataRow dr)
        {
            Id = BaseUtil.ConvertToString(dr[FieldId]);
            Agree = BaseUtil.ConvertToInt(dr[FieldAgree]);
            Oppose = BaseUtil.ConvertToInt(dr[FieldOppose]);
            FirstVisit = BaseUtil.ConvertToNullableDateTime(dr[FieldFirstVisit]);
            LastVisit = BaseUtil.ConvertToNullableDateTime(dr[FieldLastVisit]);
            LogOnCount = BaseUtil.ConvertToInt(dr[FieldLogOnCount]);
            ShowCount = BaseUtil.ConvertToInt(dr[FieldShowCount]);
            UserOnLine = BaseUtil.ConvertToInt(dr[FieldUserOnLine]);
            IpAddress = BaseUtil.ConvertToString(dr[FieldIpAddress]);
            ModifiedOn = BaseUtil.ConvertToNullableDateTime(dr[BaseOrganizeEntity.FieldUpdateTime]);
            // 获取扩展属性
            GetFromExtend(dr);
            return this;
        }

        ///<summary>
        /// 系统用户账户表
        ///</summary>
        [NonSerialized]
        [FieldDescription("网点登录信息表")]
        public const string TableName = "BaseOrganizeLogOn";

        ///<summary>
        /// 主键
        ///</summary>
        [NonSerialized]
        public const string FieldId = "Id";

        ///<summary>
        /// 赞成数
        ///</summary>
        [NonSerialized]
        public const string FieldAgree = "Agree";

        ///<summary>
        /// 反对数
        ///</summary>
        [NonSerialized]
        public const string FieldOppose = "Oppose";

        ///<summary>
		/// 第一次访问时间
		///</summary>
		[NonSerialized]
        public const string FieldFirstVisit = "FirstVisit";

        ///<summary>
        /// 最后访问时间
        ///</summary>
        [NonSerialized]
        public const string FieldLastVisit = "LastVisit";

        ///<summary>
        /// 登录次数
        ///</summary>
        [NonSerialized]
        public const string FieldLogOnCount = "LogOnCount";

        ///<summary>
        /// 展示次数
        ///</summary>
        [NonSerialized]
        public const string FieldShowCount = "ShowCount";

        ///<summary>
        /// 在线状态
        ///</summary>
        [NonSerialized]
        public const string FieldUserOnLine = "UserOnLine";

        ///<summary>
        /// 登录IP地址
        ///</summary>
        [NonSerialized]
        public const string FieldIpAddress = "IPAddress";

        ///<summary>
        /// 修改时间
        ///</summary>
        [NonSerialized]
        [FieldDescription("修改时间")]
        public const string FieldUpdateTime = "ModifiedOn";
    }
}
