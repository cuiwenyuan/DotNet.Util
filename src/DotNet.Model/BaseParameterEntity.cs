//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System;


namespace DotNet.Model
{
    using Util;

    /// <summary>
    /// BaseParameterEntity
    /// 参数表的基类结构定义
    ///
    /// 修改记录
    /// 
    ///     2014.08.25 版本：2.2 JiRiGaLa   修改时间，创建时间进行日期类型规范化。
    ///     2011.07.05 版本：2.1 zgl        修改enable  默认值为true
    ///     2007.06.07 版本：2.0 JiRiGaLa   字段名变更
    ///		2006.02.05 版本：1.1 JiRiGaLa	重新调整主键的规范化。
    ///		2004.08.29 版本：1.0 JiRiGaLa	主键ID需要进行倒序排序，这样数据库管理员很方便地找到最新的纪录及被修改的纪录。
    ///										CategoryId 需要进行外键数据库完整性约束。
    ///										CreateOn 需要进行有默认值，不需要赋值也能获得当前的系统时间。
    ///										CreateUserId、ModifiedUserId 需要有外件数据库完整性约束。
    ///										Content、CreateUserId 不可以为空，减少垃圾数据。
    ///		2005.08.13 版本：1.0 JiRiGaLa	增加版权信息。
    /// 
    /// <author>
    ///		<name>JiRiGaLa</name>
    ///		<date>2014.08.25</date>
    /// </author> 
    /// </summary>
    [Serializable]
    public partial class BaseParameterEntity : BaseEntity
    {
        #region public void ClearProperty(BaseParameterEntity entity)
        /// <summary>
        /// 清除内容
        /// <param name="entity">实体</param>
        /// </summary>
        public void ClearProperty(BaseParameterEntity entity)
        {
            entity.Id = string.Empty;
            entity.CategoryCode = string.Empty;
            entity.ParameterId = string.Empty;
            entity.ParameterCode = string.Empty;
            entity.ParameterContent = string.Empty;
            entity.Worked = false;
            entity.Enabled = false;
            entity.SortCode = null;
            entity.Description = string.Empty;
            entity.CreateUserId = string.Empty;
            entity.CreateOn = null;
            entity.ModifiedUserId = string.Empty;
            entity.ModifiedOn = null;
        }

        #endregion

        /// <summary>
        /// 主键
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// 分类编号
        /// </summary>
        public string CategoryCode { get; set; } = string.Empty;

        /// <summary>
        /// 参数主键
        /// </summary>
        public string ParameterId { get; set; } = string.Empty;

        /// <summary>
        /// 参数编码
        /// </summary>
        public string ParameterCode { get; set; } = string.Empty;

        /// <summary>
        /// 参数编码
        /// </summary>
        public string ParameterContent { get; set; } = string.Empty;

        /// <summary>
        /// 处理状态
        /// </summary>
        public bool Worked { get; set; } = false;

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
        /// 排序码
        /// </summary>
        public int? SortCode { get; set; } = 0;

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
            CategoryCode = BaseUtil.ConvertToString(dr[FieldCategoryCode]);
            ParameterId = BaseUtil.ConvertToString(dr[FieldParameterId]);
            ParameterCode = BaseUtil.ConvertToString(dr[FieldParameterCode]);
            ParameterContent = BaseUtil.ConvertToString(dr[FieldParameterContent]);
            Worked = BaseUtil.ConvertIntToBoolean(dr[FieldWorked]);
            Enabled = BaseUtil.ConvertIntToBoolean(dr[FieldEnabled]);
            SortCode = BaseUtil.ConvertToNullableInt(dr[FieldSortCode]);
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
        public const string TableName = "BaseParameter";

        /// <summary>
        /// 主键
        /// </summary>
        [NonSerialized]
        public const string FieldId = "Id";

        /// <summary>
        /// 类别主键
        /// </summary>
        [NonSerialized]
        public const string FieldCategoryCode = "CategoryCode";

        /// <summary>
        /// 参数主键
        /// </summary>
        [NonSerialized]
        public const string FieldParameterId = "ParameterId";

        /// <summary>
        /// 参数编码
        /// </summary>
        [NonSerialized]
        public const string FieldParameterCode = "ParameterCode";

        /// <summary>
        /// 参数内容
        /// </summary>
        [NonSerialized]
        public const string FieldParameterContent = "ParameterContent";

        /// <summary>
        /// 处理状态
        /// </summary>
        [NonSerialized]
        public const string FieldWorked = "Worked";

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
        /// 排序码
        /// </summary>
        [NonSerialized]
        public const string FieldSortCode = "SortCode";

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