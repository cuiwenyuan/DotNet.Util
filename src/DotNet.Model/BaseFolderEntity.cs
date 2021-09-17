//--------------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//--------------------------------------------------------------------

using System;


namespace DotNet.Model
{
    using Util;

    /// <summary>
    /// BaseFolderEntity
    /// 文件夹表
    /// 
    /// 修改记录
    /// 
    /// 2012-05-17 版本：1.1 Serwif 补充完整AllowEdit,AllowDelete
    /// 2012-05-17 版本：1.0 JiRiGaLa 创建主键。
    /// 
    /// <author>
    ///     <name>Troy.Cui</name>
    ///     <date>2012-05-17</date>
    /// </author>
    /// </summary>
    [Serializable]
    public partial class BaseFolderEntity : BaseEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 父亲节点主键
        /// </summary>
        public string ParentId { get; set; }

        /// <summary>
        /// 文件夹名
        /// </summary>
        public string FolderName { get; set; }

        /// <summary>
        /// 排序码
        /// </summary>
        public int? SortCode { get; set; } = 0;

        /// <summary>
        /// 允许编辑
        /// </summary>        
        public int? AllowEdit { get; set; } = 1;

        /// <summary>
        /// 允许删除
        /// </summary>
        public int? AllowDelete { get; set; } = 1;

        /// <summary>
        /// 是否公开
        /// </summary>
        public int? IsPublic { get; set; }

        /// <summary>
        /// 有效
        /// </summary>
        public int? Enabled { get; set; }
        /// <summary>
        /// 是否删除
        /// </summary>
        public int? DeletionStateCode { get; set; } = 0;

        /// <summary>
        /// 备注
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>

        public DateTime? CreateOn { get; set; }
        /// <summary>
        /// 创建人用户编号
        /// </summary>

        public string CreateUserId { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>

        public string CreateBy { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime? ModifiedOn { get; set; }

        /// <summary>
        /// 修改人用户编号
        /// </summary>
        public string ModifiedUserId { get; set; }

        /// <summary>
        /// 修改人
        /// </summary>
        public string ModifiedBy { get; set; }

        /// <summary>
        /// 从数据行读取
        /// </summary>
        /// <param name="dr">数据行</param>
		protected override BaseEntity GetFrom(IDataRow dr)
        {
            Id = BaseUtil.ConvertToString(dr[FieldId]);
            ParentId = BaseUtil.ConvertToString(dr[FieldParentId]);
            FolderName = BaseUtil.ConvertToString(dr[FieldFolderName]);
            SortCode = BaseUtil.ConvertToInt(dr[FieldSortCode]);
            AllowEdit = BaseUtil.ConvertToInt(dr[FieldAllowEdit]);
            AllowDelete = BaseUtil.ConvertToInt(dr[FieldAllowDelete]);
            IsPublic = BaseUtil.ConvertToInt(dr[FieldIsPublic]);
            Enabled = BaseUtil.ConvertToInt(dr[FieldEnabled]);
            DeletionStateCode = BaseUtil.ConvertToInt(dr[FieldDeleted]);
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
        /// 文件夹表
        ///</summary>
        [NonSerialized]
        public const string TableName = "BaseFolder";

        ///<summary>
        /// 主键
        ///</summary>
        [NonSerialized]
        public const string FieldId = "Id";

        ///<summary>
        /// 父亲节点主键
        ///</summary>
        [NonSerialized]
        public const string FieldParentId = "ParentId";

        ///<summary>
        /// 文件夹名
        ///</summary>
        [NonSerialized]
        public const string FieldFolderName = "FolderName";

        ///<summary>
        /// 排序码
        ///</summary>
        [NonSerialized]
        public const string FieldSortCode = "SortCode";

        ///<summary>
        /// 允许编辑
        ///</summary>
        [NonSerialized]
        public const string FieldAllowEdit = "AllowEdit";

        ///<summary>
        /// 备注
        ///</summary>
        [NonSerialized]
        public const string FieldAllowDelete = "AllowDelete";

        ///<summary>
        /// 是否公开
        ///</summary>
        [NonSerialized]
        public const string FieldIsPublic = "IsPublic";

        ///<summary>
        /// 有效
        ///</summary>
        [NonSerialized]
        public const string FieldEnabled = "Enabled";

        ///<summary>
        /// 状态
        ///</summary>
        [NonSerialized]
        public const string FieldDeleted = "DeletionStateCode";

        ///<summary>
        /// 备注
        ///</summary>
        [NonSerialized]
        public const string FieldDescription = "Description";

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
