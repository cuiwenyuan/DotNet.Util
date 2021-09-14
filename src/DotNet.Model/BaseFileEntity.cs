//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System;


namespace DotNet.Model
{
    using Util;

    /// <remarks>
    ///	BaseFileEntity
    /// 文件信息
    ///
    /// 注意事项
    ///     1.主键与编号一定要一致否则以后比较难扩展。
    ///     2.或者模块权限添加时，能自动添加到基本权限表里，这样也能解决问题。
    ///
    /// 修改记录
    ///
    ///     2008.04.29 版本：2.4 JiRiGaLa 整理 Entity 主键部分。
    ///     2007.05.30 版本：2.3 JiRiGaLa 整理 Entity 主键部分。
    ///     2007.01.20 版本：2.2 JiRiGaLa 添加AllowEdit,AllowDelete两个字段。
    ///     2007.01.19 版本：2.1 JiRiGaLa SQLBuild修改为SQLBuild。
    ///     2007.01.04 版本：2.0 JiRiGaLa 重新整理主键。
    ///		2006.03.16 版本：1.0 JiRiGaLa 规范化主键。
    /// 
    /// <author>
    ///		<name>JiRiGaLa</name>
    ///		<date>2008.04.29</date>
    /// </author>
    /// </remarks>
    [Serializable]
    public partial class BaseFileEntity : BaseEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        public string Id { get; set; } = null;
        /// <summary>
        /// 文件夹节点主键
        /// </summary>
        public string FolderId { get; set; } = null;
        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName { get; set; } = null;
        /// <summary>
        /// 文件路径
        /// </summary>
        public string FilePath { get; set; } = null;
        /// <summary>
        /// 文件内容
        /// </summary>
        public byte[] Contents { get; set; } = null;
        /// <summary>
        /// 文件大小
        /// </summary>
        public int? FileSize { get; set; } = 0;
        /// <summary>
        /// 被阅读次数
        /// </summary>
        public int? ReadCount { get; set; } = 0;
        /// <summary>
        /// 是否删除
        /// </summary>
        public int? DeletionStateCode { get; set; } = 0;
        /// <summary>
        /// 备注
        /// </summary>
        public string Description { get; set; } = null;
        /// <summary>
        /// 有效
        /// </summary>
        public int? Enabled { get; set; } = 1;
        /// <summary>
        /// 排序码
        /// </summary>
        public int? SortCode { get; set; } = 0;
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
            FolderId = BaseUtil.ConvertToString(dr[FieldFolderId]);
            FileName = BaseUtil.ConvertToString(dr[FieldFileName]);
            FilePath = BaseUtil.ConvertToString(dr[FieldFilePath]);
            FileSize = BaseUtil.ConvertToInt(dr[FieldFileSize]);
            ReadCount = BaseUtil.ConvertToInt(dr[FieldReadCount]);
            DeletionStateCode = BaseUtil.ConvertToInt(dr[FieldDeleted]);
            Description = BaseUtil.ConvertToString(dr[FieldDescription]);
            Enabled = BaseUtil.ConvertToInt(dr[FieldEnabled]);
            SortCode = BaseUtil.ConvertToInt(dr[FieldSortCode]);
            CreateOn = BaseUtil.ConvertToNullableDateTime(dr[FieldCreateTime]);
            CreateBy = BaseUtil.ConvertToString(dr[FieldCreateBy]);
            CreateUserId = BaseUtil.ConvertToString(dr[FieldCreateUserId]);
            ModifiedOn = BaseUtil.ConvertToNullableDateTime(dr[FieldUpdateTime]);
            ModifiedBy = BaseUtil.ConvertToString(dr[FieldUpdateBy]);
            ModifiedUserId = BaseUtil.ConvertToString(dr[FieldUpdateUserId]);
            // 获取扩展属性
            GetFromExpand(dr);
            return this;
        }

        ///<summary>
        /// 文件新闻表
        ///</summary>
        [NonSerialized]
        public const string TableName = "BaseFile";

        ///<summary>
        /// 主键
        ///</summary>
        [NonSerialized]
        public const string FieldId = "Id";

        ///<summary>
        /// 文件夹节点主键
        ///</summary>
        [NonSerialized]
        public const string FieldFolderId = "FolderId";

        ///<summary>
        /// 文件名
        ///</summary>
        [NonSerialized]
        public const string FieldFileName = "FileName";

        ///<summary>
        /// 文件路径
        ///</summary>
        [NonSerialized]
        public const string FieldFilePath = "FilePath";

        ///<summary>
        /// 文件内容
        ///</summary>
        [NonSerialized]
        public const string FieldContents = "Contents";

        ///<summary>
        /// 文件大小
        ///</summary>
        [NonSerialized]
        public const string FieldFileSize = "FileSize";

        ///<summary>
        /// 被阅读次数
        ///</summary>
        [NonSerialized]
        public const string FieldReadCount = "ReadCount";

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
        /// 有效
        ///</summary>
        [NonSerialized]
        public const string FieldEnabled = "Enabled";

        ///<summary>
        /// 排序码
        ///</summary>
        [NonSerialized]
        public const string FieldSortCode = "SortCode";

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