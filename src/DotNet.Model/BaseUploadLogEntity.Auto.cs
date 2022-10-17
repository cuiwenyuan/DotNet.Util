//-----------------------------------------------------------------------
// <copyright file="BaseUploadLogEntity.Auto.cs" company="DotNet">
//     Copyright (c) 2022, All rights reserved.
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
    /// 2022-02-07 版本：1.0 Troy.Cui 增加SystemCode。
    /// 2021-09-28 版本：2.0 Troy.Cui 创建文件。
    /// 2020-03-22 版本：1.0 Troy.Cui 创建文件。
    /// 
    /// <author>
    ///     <name>Troy.Cui</name>
    ///     <date>2022-02-07</date>
    /// </author>
    /// </summary>
    public partial class BaseUploadLogEntity : BaseEntity
    {
        /// <summary>
        /// 子系统编码
        /// </summary>
        [FieldDescription("子系统编码")]
        public string SystemCode { get; set; } = "Base";

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
        /// 从数据行读取
        /// </summary>
        /// <param name="dr">数据行</param>
        protected override BaseEntity GetFrom(IDataRow dr)
        {
            GetFromExtend(dr);
            GetBase(dr);
            if (dr.ContainsColumn(FieldSystemCode))
            {
                SystemCode = BaseUtil.ConvertToString(dr[FieldSystemCode]);
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
            return this;
        }

        ///<summary>
        /// 文件上传日志
        ///</summary>
        [FieldDescription("文件上传日志")]
        public const string CurrentTableName = "BaseUploadLog";

        ///<summary>
        /// 子系统编码
        ///</summary>
        public const string FieldSystemCode = "SystemCode";

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
    }
}
