//-----------------------------------------------------------------------
// <copyright file="BaseUploadLogEntity.Auto.cs" company="DotNet">
//     Copyright (c) 2021, All rights reserved.
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
    /// 修改记录
    /// 
    /// 2021-09-28 版本：2.0 Troy.Cui 创建文件。
    /// 2020-03-22 版本：1.0 Troy.Cui 创建文件。
    /// 
    /// <author>
    ///     <name>Troy.Cui</name>
    ///     <date>2021-09-28</date>
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
        public int SortCode { get; set; } = 0;

        /// <summary>
        /// 是否删除
        /// </summary>
        [FieldDescription("是否删除")]
        public int Deleted { get; set; } = 0;

        /// <summary>
        /// 是否有效
        /// </summary>
        [FieldDescription("是否有效")]
        public int Enabled { get; set; } = 1;

        /// <summary>
        /// 创建时间
        /// </summary>
        [FieldDescription("创建时间")]
        public DateTime CreateTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 创建人编号
        /// </summary>
        [FieldDescription("创建人编号")]
        public int CreateUserId { get; set; } = 0;

        /// <summary>
        /// 创建人用户名
        /// </summary>
        [FieldDescription("创建人用户名")]
        public string CreateUserName { get; set; } = string.Empty;

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
        /// 修改时间
        /// </summary>
        [FieldDescription("修改时间")]
        public DateTime UpdateTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 修改人编号
        /// </summary>
        [FieldDescription("修改人编号")]
        public int UpdateUserId { get; set; } = 0;

        /// <summary>
        /// 修改人用户名
        /// </summary>
        [FieldDescription("修改人用户名")]
        public string UpdateUserName { get; set; } = string.Empty;

        /// <summary>
        /// 修改人姓名
        /// </summary>
        [FieldDescription("修改人姓名")]
        public string UpdateBy { get; set; } = string.Empty;

        /// <summary>
        /// 修改IP
        /// </summary>
        [FieldDescription("修改IP")]
        public string UpdateIp { get; set; } = string.Empty;

        /// <summary>
        /// 从数据行读取
        /// </summary>
        /// <param name="dr">数据行</param>
        protected override BaseEntity GetFrom(IDataRow dr)
        {
            GetFromExtend(dr);
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
                SortCode = BaseUtil.ConvertToInt(dr[FieldSortCode]);
            }
            if (dr.ContainsColumn(FieldDeleted))
            {
                Deleted = BaseUtil.ConvertToInt(dr[FieldDeleted]);
            }
            if (dr.ContainsColumn(FieldEnabled))
            {
                Enabled = BaseUtil.ConvertToInt(dr[FieldEnabled]);
            }
            if (dr.ContainsColumn(FieldCreateTime))
            {
                CreateTime = BaseUtil.ConvertToDateTime(dr[FieldCreateTime]);
            }
            if (dr.ContainsColumn(FieldCreateUserId))
            {
                CreateUserId = BaseUtil.ConvertToInt(dr[FieldCreateUserId]);
            }
            if (dr.ContainsColumn(FieldCreateUserName))
            {
                CreateUserName = BaseUtil.ConvertToString(dr[FieldCreateUserName]);
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
                UpdateTime = BaseUtil.ConvertToDateTime(dr[FieldUpdateTime]);
            }
            if (dr.ContainsColumn(FieldUpdateUserId))
            {
                UpdateUserId = BaseUtil.ConvertToInt(dr[FieldUpdateUserId]);
            }
            if (dr.ContainsColumn(FieldUpdateUserName))
            {
                UpdateUserName = BaseUtil.ConvertToString(dr[FieldUpdateUserName]);
            }
            if (dr.ContainsColumn(FieldUpdateBy))
            {
                UpdateBy = BaseUtil.ConvertToString(dr[FieldUpdateBy]);
            }
            if (dr.ContainsColumn(FieldUpdateIp))
            {
                UpdateIp = BaseUtil.ConvertToString(dr[FieldUpdateIp]);
            }
            return this;
        }

        ///<summary>
        /// 文件上传日志
        ///</summary>
        [FieldDescription("文件上传日志")]
        public const string CurrentTableName = "BaseUploadLog";

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
        /// 是否删除
        ///</summary>
        public const string FieldDeleted = "Deleted";

        ///<summary>
        /// 是否有效
        ///</summary>
        public const string FieldEnabled = "Enabled";

        ///<summary>
        /// 创建时间
        ///</summary>
        public const string FieldCreateTime = "CreateTime";

        ///<summary>
        /// 创建人编号
        ///</summary>
        public const string FieldCreateUserId = "CreateUserId";

        ///<summary>
        /// 创建人用户名
        ///</summary>
        public const string FieldCreateUserName = "CreateUserName";

        ///<summary>
        /// 创建人姓名
        ///</summary>
        public const string FieldCreateBy = "CreateBy";

        ///<summary>
        /// 创建IP
        ///</summary>
        public const string FieldCreateIp = "CreateIp";

        ///<summary>
        /// 修改时间
        ///</summary>
        public const string FieldUpdateTime = "UpdateTime";

        ///<summary>
        /// 修改人编号
        ///</summary>
        public const string FieldUpdateUserId = "UpdateUserId";

        ///<summary>
        /// 修改人用户名
        ///</summary>
        public const string FieldUpdateUserName = "UpdateUserName";

        ///<summary>
        /// 修改人姓名
        ///</summary>
        public const string FieldUpdateBy = "UpdateBy";

        ///<summary>
        /// 修改IP
        ///</summary>
        public const string FieldUpdateIp = "UpdateIp";
    }
}
