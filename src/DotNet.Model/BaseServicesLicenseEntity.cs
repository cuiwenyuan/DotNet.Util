//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System;


namespace DotNet.Model
{
    using Util;

    /// <summary>
    /// BaseServicesLicenseEntity
    /// 接口调用设置定义
    ///
    /// 修改记录
    /// 
    ///		2015.12.24 版本：1.0 JiRiGaLa	添加。
    /// 
    /// <author>
    ///		<name>JiRiGaLa</name>
    ///		<date>2015.12.24</date>
    /// </author> 
    /// </summary>
    [Serializable]
    public partial class BaseServicesLicenseEntity : BaseEntity
    {
        /// <summary>
        /// 主键
        /// </summary>

        public string Id { get; set; } = string.Empty;

        private string _userId = string.Empty;
        /// <summary>
        /// 参数主键
        /// </summary>

        public string UserId
        {
            get => _userId;
            set => _userId = value;
        }
        /// <summary>
        /// 私钥
        /// </summary>
        public string PrivateKey { get; set; } = null;
        /// <summary>
        /// 公钥
        /// </summary>
        public string PublicKey { get; set; } = null;
        /// <summary>
        /// 开始生效日期
        /// </summary>

        public DateTime? StartTime { get; set; } = null;
        /// <summary>
        /// 结束生效日期
        /// </summary>

        public DateTime? EndTime { get; set; } = null;
        /// <summary>
        /// 有效
        /// </summary>

        public bool Enabled { get; set; } = true;
        /// <summary>
        /// 是否删除
        /// </summary>

        public int? DeletionStateCode { get; set; } = 0;
        /// <summary>
        /// 备注
        /// </summary>

        public string Description { get; set; } = string.Empty;
        /// <summary>
        /// 创建人主键
        /// </summary>

        public string CreateUserId { get; set; } = string.Empty;
        /// <summary>
        /// 创建人
        /// </summary>

        public string CreateBy { get; set; } = null;
        /// <summary>
        /// 创建时间
        /// </summary>

        public DateTime? CreateOn { get; set; } = DateTime.Now;
        /// <summary>
        /// 最后修改人主键
        /// </summary>

        public string ModifiedUserId { get; set; } = string.Empty;
        /// <summary>
        /// 修改人
        /// </summary>

        public string ModifiedBy { get; set; } = null;
        /// <summary>
        /// 修改时间
        /// </summary>

        public DateTime? ModifiedOn { get; set; } = DateTime.Now;

        #region public BaseParameterEntity GetFrom(DataRow dr)
        /// <summary>
        /// 从数据行读取
        /// </summary>
        /// <param name="dr">数据行</param>
        /// <returns>BaseParameterEntity</returns>
		protected override BaseEntity GetFrom(IDataRow dr)
        {
            Id = BaseUtil.ConvertToString(dr[FieldId]);
            UserId = BaseUtil.ConvertToString(dr[FieldUserId]);
            PrivateKey = BaseUtil.ConvertToString(dr[FieldPrivateKey]);
            PublicKey = BaseUtil.ConvertToString(dr[FieldPublicKey]);
            StartTime = BaseUtil.ConvertToNullableDateTime(dr[FieldStartDate]);
            EndTime = BaseUtil.ConvertToNullableDateTime(dr[FieldEndDate]);
            Enabled = BaseUtil.ConvertIntToBoolean(dr[FieldEnabled]);
            Description = BaseUtil.ConvertToString(dr[FieldDescription]);
            CreateUserId = BaseUtil.ConvertToString(dr[FieldCreateUserId]);
            CreateOn = BaseUtil.ConvertToNullableDateTime(dr[FieldCreateTime]);
            CreateBy = BaseUtil.ConvertToString(dr[FieldCreateBy]);
            ModifiedUserId = BaseUtil.ConvertToString(dr[FieldUpdateUserId]);
            ModifiedOn = BaseUtil.ConvertToNullableDateTime(dr[FieldUpdateTime]);
            ModifiedBy = BaseUtil.ConvertToString(dr[FieldUpdateBy]);
            // 获取扩展属性
            GetFromExtend(dr);
            return this;
        }
        #endregion

        /// <summary>
        /// 表名
        /// </summary>
        [NonSerialized]
        public const string TableName = "BaseServicesLicense";

        /// <summary>
        /// 主键
        /// </summary>
        [NonSerialized]
        public const string FieldId = "Id";

        /// <summary>
        /// 参数主键
        /// </summary>
        [NonSerialized]
        public const string FieldUserId = "UserId";

        /// <summary>
        /// 私钥
        /// </summary>
        [NonSerialized]
        public const string FieldPrivateKey = "PrivateKey";

        /// <summary>
        /// 公钥
        /// </summary>
        [NonSerialized]
        public const string FieldPublicKey = "PublicKey";

        ///<summary>
        /// 开始生效日期
        ///</summary>
        [NonSerialized]
        public const string FieldStartDate = "StartDate";

        ///<summary>
        /// 结束生效日期
        ///</summary>
        [NonSerialized]
        public const string FieldEndDate = "EndDate";

        /// <summary>
        /// 有效性
        /// </summary>
        [NonSerialized]
        public const string FieldEnabled = "Enabled";

        /// <summary>
        /// 备注
        /// </summary>
        [NonSerialized]
        public const string FieldDescription = "Description";

        ///<summary>
        /// 是否删除
        ///</summary>
        [NonSerialized]
        public const string FieldDeleted = "DeletionStateCode";

        /// <summary>
        /// 创建人
        /// </summary>
        [NonSerialized]
        public const string FieldCreateBy = "CreateBy";

        /// <summary>
        /// 创建人主键
        /// </summary>
        [NonSerialized]
        public const string FieldCreateUserId = "CreateUserId";

        /// <summary>
        /// 创建时间
        /// </summary>
        [NonSerialized]
        public const string FieldCreateTime = "CreateOn";

        /// <summary>
        /// 最后修改人主键
        /// </summary>
        [NonSerialized]
        public const string FieldUpdateUserId = "ModifiedUserId";

        /// <summary>
        /// 最后修改人
        /// </summary>
        [NonSerialized]
        public const string FieldUpdateBy = "ModifiedBy";

        /// <summary>
        /// 最后修改时间
        /// </summary>
        [NonSerialized]
        public const string FieldUpdateTime = "ModifiedOn";
    }
}