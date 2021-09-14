//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System;


namespace DotNet.Model
{
    using Util;

    /// <summary>
    /// BaseNewsEntity
    /// 新闻表
    ///
    /// 修改记录
    ///
    ///		2015-06-07 版本：2.0 JiRiGaLa 公司主键。
    ///		2010-07-28 版本：1.0 JiRiGaLa 创建主键。
    ///
    /// <author>
    ///		<name>JiRiGaLa</name>
    ///		<date>2015-06-07</date>
    /// </author>
    /// </summary>
    [Serializable]
    public partial class BaseNewsEntity : BaseEntity
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
        /// 公司主键
        /// </summary>
        public string CompanyId { get; set; }

        /// <summary>
        /// 公司名称
        /// </summary>
        public string CompanyName { get; set; }

        /// <summary>
        /// 部门主键
        /// </summary>
        public string DepartmentId { get; set; }

        /// <summary>
        /// 部门名称
        /// </summary>
        public string DepartmentName { get; set; }

        /// <summary>
        /// 分类编号
        /// </summary>
        public string CategoryCode { get; set; } = null;

        /// <summary>
        /// 编号
        /// </summary>
        public string Code { get; set; } = null;

        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; } = null;
        /// <summary>
        /// 文件路径
        /// </summary>

        public string FilePath { get; set; } = null;

        /// <summary>
        /// 内容简介
        /// </summary>
        public string Introduction { get; set; } = null;

        /// <summary>
        /// 内容
        /// </summary>
        public string Contents { get; set; } = null;

        /// <summary>
        /// 新闻来源
        /// </summary>
        public string Source { get; set; } = null;

        /// <summary>
        /// 关键字
        /// </summary>
        public string Keywords { get; set; } = null;

        /// <summary>
        /// 文件大小
        /// </summary>
        public int? FileSize { get; set; } = 0;

        /// <summary>
        /// 图片文件位置(图片新闻)
        /// </summary>
        public string ImageUrl { get; set; } = null;

        /// <summary>
        /// 置首页
        /// </summary>
        public int? HomePage { get; set; } = 0;

        /// <summary>
        /// 置二级页
        /// </summary>
        public int? SubPage { get; set; } = 0;

        /// <summary>
        /// 审核状态
        /// </summary>
        public string AuditStatus { get; set; } = null;

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
            CompanyId = BaseUtil.ConvertToString(dr[FieldCompanyId]);
            CompanyName = BaseUtil.ConvertToString(dr[FieldCompanyName]);
            DepartmentId = BaseUtil.ConvertToString(dr[FieldDepartmentId]);
            DepartmentName = BaseUtil.ConvertToString(dr[FieldDepartmentName]);
            FolderId = BaseUtil.ConvertToString(dr[FieldFolderId]);
            CategoryCode = BaseUtil.ConvertToString(dr[FieldCategoryCode]);
            Code = BaseUtil.ConvertToString(dr[FieldCode]);
            Title = BaseUtil.ConvertToString(dr[FieldTitle]);
            FilePath = BaseUtil.ConvertToString(dr[FieldFilePath]);
            Introduction = BaseUtil.ConvertToString(dr[FieldIntroduction]);
            Contents = BaseUtil.ConvertToString(dr[FieldContents]);
            Source = BaseUtil.ConvertToString(dr[FieldSource]);
            Keywords = BaseUtil.ConvertToString(dr[FieldKeywords]);
            FileSize = BaseUtil.ConvertToInt(dr[FieldFileSize]);
            ImageUrl = BaseUtil.ConvertToString(dr[FieldImageUrl]);
            SubPage = BaseUtil.ConvertToInt(dr[FieldSubPage]);
            HomePage = BaseUtil.ConvertToInt(dr[FieldHomePage]);
            AuditStatus = BaseUtil.ConvertToString(dr[FieldAuditStatus]);
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
        /// 新闻表
        ///</summary>
        [NonSerialized]
        public const string TableName = "BaseNews";

        ///<summary>
        /// 代码
        ///</summary>
        [NonSerialized]
        public const string FieldId = "Id";

        ///<summary>
        /// 文件夹节点代码
        ///</summary>
        [NonSerialized]
        public const string FieldFolderId = "FolderId";

        ///<summary>
        /// 公司主键
        ///</summary>
        [NonSerialized]
        public const string FieldCompanyId = "CompanyId";

        ///<summary>
        /// 公司名称
        ///</summary>
        [NonSerialized]
        public const string FieldCompanyName = "CompanyName";

        ///<summary>
        /// 部门主键
        ///</summary>
        [NonSerialized]
        public const string FieldDepartmentId = "DepartmentId";

        ///<summary>
        /// 部门名称
        ///</summary>
        [NonSerialized]
        public const string FieldDepartmentName = "DepartmentName";

        ///<summary>
        /// 文件类别码
        ///</summary>
        [NonSerialized]
        public const string FieldCategoryCode = "CategoryCode";

        ///<summary>
        /// 文件编号
        ///</summary>
        [NonSerialized]
        public const string FieldCode = "Code";

        ///<summary>
        /// 文件名
        ///</summary>
        [NonSerialized]
        public const string FieldTitle = "Title";

        ///<summary>
        /// 文件路径
        ///</summary>
        [NonSerialized]
        public const string FieldFilePath = "FilePath";

        ///<summary>
        /// 内容简介
        ///</summary>
        [NonSerialized]
        public const string FieldIntroduction = "Introduction";

        ///<summary>
        /// 文件内容
        ///</summary>
        [NonSerialized]
        public const string FieldContents = "Contents";

        ///<summary>
        /// 新闻来源
        ///</summary>
        [NonSerialized]
        public const string FieldSource = "Source";

        ///<summary>
        /// 关键字
        ///</summary>
        [NonSerialized]
        public const string FieldKeywords = "Keywords";

        ///<summary>
        /// 文件大小
        ///</summary>
        [NonSerialized]
        public const string FieldFileSize = "FileSize";

        ///<summary>
        /// 图片文件位置(图片新闻)
        ///</summary>
        [NonSerialized]
        public const string FieldImageUrl = "ImageUrl";

        ///<summary>
        /// 置首页
        ///</summary>
        [NonSerialized]
        public const string FieldHomePage = "HomePage";

        ///<summary>
        /// 置二级页
        ///</summary>
        [NonSerialized]
        public const string FieldSubPage = "SubPage";

        ///<summary>
        /// 审核状态
        ///</summary>
        [NonSerialized]
        public const string FieldAuditStatus = "AuditStatus";

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
