//-----------------------------------------------------------------------
// <copyright file="BaseUploadLogEntity.Auto.cs" company="DotNet">
//     Copyright (c) 2020, All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace DotNet.Model
{
    using Util;

    /// <summary>
    /// BaseUploadLogEntity
    /// 文件上传日志
    /// 
    /// 修改纪录
    /// 
    /// 2020-03-22 版本：1.0 Troy.Cui 创建文件。
    /// 
    /// <author>
    ///     <name>Troy.Cui</name>
    ///     <date>2020-03-22</date>
    /// </author>
    /// </summary>
    public partial class BaseUploadLogEntity : BaseEntity
    {
        /// <summary>
        /// 编号
        /// </summary>
        [FieldDescription("编号")]
        public int Id { get; set; }

        /// <summary>
        /// 公司编号
        /// </summary>
        [FieldDescription("公司编号")]
        public int UserCompanyId { get; set; } = 0;

        /// <summary>
        /// 子公司编号
        /// </summary>
        [FieldDescription("子公司编号")]
        public int UserSubCompanyId { get; set; } = 0;

        /// <summary>
        /// 文件名
        /// </summary>
        [FieldDescription("文件名")]
        public string FileName { get; set; } = string.Empty;

        /// <summary>
        /// 文件扩展名
        /// </summary>
        [FieldDescription("文件扩展名")]
        public string FileExtension { get; set; } = string.Empty;

        /// <summary>
        /// 文件名
        /// </summary>
        [FieldDescription("文件名")]
        public string FilePath { get; set; } = string.Empty;

        /// <summary>
        /// 文件大小
        /// </summary>
        [FieldDescription("文件大小")]
        public int? FileSize { get; set; } = null;

        /// <summary>
        /// 备注
        /// </summary>
        [FieldDescription("备注")]
        public string Remark { get; set; } = string.Empty;

        /// <summary>
        /// 排序编号
        /// </summary>
        [FieldDescription("排序编号")]
        public int? SortCode { get; set; } = 0;

        /// <summary>
        /// 删除状态代码
        /// </summary>
        [FieldDescription("删除状态代码")]
        public int? DeletionStateCode { get; set; } = 0;

        /// <summary>
        /// 是否有效
        /// </summary>
        [FieldDescription("是否有效")]
        public int? Enabled { get; set; } = 1;

        /// <summary>
        /// 创建时间
        /// </summary>
        [FieldDescription("创建时间")]
        public DateTime? CreateOn { get; set; } = DateTime.Now;

        /// <summary>
        /// 创建人编号
        /// </summary>
        [FieldDescription("创建人编号")]
        public int? CreateUserId { get; set; } = null;

        /// <summary>
        /// 创建人姓名
        /// </summary>
        [FieldDescription("创建人姓名")]
        public string CreateBy { get; set; } = string.Empty;

        /// <summary>
        /// 创建IP
        /// </summary>
        [FieldDescription("创建IP")]
        public string CreateIp { get; set; } = string.Empty;

        /// <summary>
        /// 最近修改时间
        /// </summary>
        [FieldDescription("最近修改时间")]
        public DateTime? ModifiedOn { get; set; } = DateTime.Now;

        /// <summary>
        /// 最近修改人编号
        /// </summary>
        [FieldDescription("最近修改人编号")]
        public int? ModifiedUserId { get; set; } = null;

        /// <summary>
        /// 最近修改人姓名
        /// </summary>
        [FieldDescription("最近修改人姓名")]
        public string ModifiedBy { get; set; } = string.Empty;

        /// <summary>
        /// 最近修改IP
        /// </summary>
        [FieldDescription("最近修改IP")]
        public string ModifiedIp { get; set; } = string.Empty;

        /// <summary>
        /// 从数据行读取
        /// </summary>
        /// <param name="dr">数据行</param>
        protected override BaseEntity GetFrom(IDataRow dr)
        {
            GetFromExpand(dr);
            if (dr.ContainsColumn(FieldId))
            {
                Id = BaseUtil.ConvertToInt(dr[FieldId]);
            }
            if (dr.ContainsColumn(FieldUserCompanyId))
            {
                UserCompanyId = BaseUtil.ConvertToInt(dr[FieldUserCompanyId]);
            }
            if (dr.ContainsColumn(FieldUserSubCompanyId))
            {
                UserSubCompanyId = BaseUtil.ConvertToInt(dr[FieldUserSubCompanyId]);
            }
            if (dr.ContainsColumn(FieldFileName))
            {
                FileName = BaseUtil.ConvertToString(dr[FieldFileName]);
            }
            if (dr.ContainsColumn(FieldFileExtension))
            {
                FileExtension = BaseUtil.ConvertToString(dr[FieldFileExtension]);
            }
            if (dr.ContainsColumn(FieldFilePath))
            {
                FilePath = BaseUtil.ConvertToString(dr[FieldFilePath]);
            }
            if (dr.ContainsColumn(FieldFileSize))
            {
                FileSize = BaseUtil.ConvertToNullableInt(dr[FieldFileSize]);
            }
            if (dr.ContainsColumn(FieldRemark))
            {
                Remark = BaseUtil.ConvertToString(dr[FieldRemark]);
            }
            if (dr.ContainsColumn(FieldSortCode))
            {
                SortCode = BaseUtil.ConvertToNullableInt(dr[FieldSortCode]);
            }
            if (dr.ContainsColumn(FieldDeleted))
            {
                DeletionStateCode = BaseUtil.ConvertToNullableInt(dr[FieldDeleted]);
            }
            if (dr.ContainsColumn(FieldEnabled))
            {
                Enabled = BaseUtil.ConvertToNullableInt(dr[FieldEnabled]);
            }
            if (dr.ContainsColumn(FieldCreateTime))
            {
                CreateOn = BaseUtil.ConvertToNullableDateTime(dr[FieldCreateTime]);
            }
            if (dr.ContainsColumn(FieldCreateUserId))
            {
                CreateUserId = BaseUtil.ConvertToNullableInt(dr[FieldCreateUserId]);
            }
            if (dr.ContainsColumn(FieldCreateBy))
            {
                CreateBy = BaseUtil.ConvertToString(dr[FieldCreateBy]);
            }
            if (dr.ContainsColumn(FieldCreateIp))
            {
                CreateIp = BaseUtil.ConvertToString(dr[FieldCreateIp]);
            }
            if (dr.ContainsColumn(FieldUpdateTime))
            {
                ModifiedOn = BaseUtil.ConvertToNullableDateTime(dr[FieldUpdateTime]);
            }
            if (dr.ContainsColumn(FieldUpdateUserId))
            {
                ModifiedUserId = BaseUtil.ConvertToNullableInt(dr[FieldUpdateUserId]);
            }
            if (dr.ContainsColumn(FieldUpdateBy))
            {
                ModifiedBy = BaseUtil.ConvertToString(dr[FieldUpdateBy]);
            }
            if (dr.ContainsColumn(FieldUpdateIp))
            {
                ModifiedIp = BaseUtil.ConvertToString(dr[FieldUpdateIp]);
            }
            return this;
        }

        ///<summary>
        /// 文件上传日志
        ///</summary>
        [FieldDescription("文件上传日志")]
        public const string TableName = "BaseUploadLog";

        ///<summary>
        /// 编号
        ///</summary>
        public const string FieldId = "Id";

        ///<summary>
        /// 公司编号
        ///</summary>
        public const string FieldUserCompanyId = "UserCompanyId";

        ///<summary>
        /// 子公司编号
        ///</summary>
        public const string FieldUserSubCompanyId = "UserSubCompanyId";

        ///<summary>
        /// 文件名
        ///</summary>
        public const string FieldFileName = "FileName";

        ///<summary>
        /// 文件扩展名
        ///</summary>
        public const string FieldFileExtension = "FileExtension";

        ///<summary>
        /// 文件名
        ///</summary>
        public const string FieldFilePath = "FilePath";

        ///<summary>
        /// 文件大小
        ///</summary>
        public const string FieldFileSize = "FileSize";

        ///<summary>
        /// 备注
        ///</summary>
        public const string FieldRemark = "Remark";

        ///<summary>
        /// 排序编号
        ///</summary>
        public const string FieldSortCode = "SortCode";

        ///<summary>
        /// 删除状态代码
        ///</summary>
        public const string FieldDeleted = "DeletionStateCode";

        ///<summary>
        /// 是否有效
        ///</summary>
        public const string FieldEnabled = "Enabled";

        ///<summary>
        /// 创建时间
        ///</summary>
        public const string FieldCreateTime = "CreateOn";

        ///<summary>
        /// 创建人编号
        ///</summary>
        public const string FieldCreateUserId = "CreateUserId";

        ///<summary>
        /// 创建人姓名
        ///</summary>
        public const string FieldCreateBy = "CreateBy";

        ///<summary>
        /// 创建IP
        ///</summary>
        public const string FieldCreateIp = "CreateIp";

        ///<summary>
        /// 最近修改时间
        ///</summary>
        public const string FieldUpdateTime = "ModifiedOn";

        ///<summary>
        /// 最近修改人编号
        ///</summary>
        public const string FieldUpdateUserId = "ModifiedUserId";

        ///<summary>
        /// 最近修改人姓名
        ///</summary>
        public const string FieldUpdateBy = "ModifiedBy";

        ///<summary>
        /// 最近修改IP
        ///</summary>
        public const string FieldUpdateIp = "ModifiedIp";
    }
}
