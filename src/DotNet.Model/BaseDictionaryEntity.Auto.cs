//-----------------------------------------------------------------------
// <copyright file="BaseDictionaryEntity.Auto.cs" company="DotNet">
//     Copyright (c) 2021, All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;

namespace DotNet.Model
{
    using Model;
    using Util;

    /// <summary>
    /// BaseDictionaryEntity
    /// 字典
    /// 
    /// 修改记录
    /// 
    /// 2021-10-26 版本：1.0 Troy.Cui 创建文件。
    /// 
    /// <author>
    ///     <name>Troy.Cui</name>
    ///     <date>2021-10-26</date>
    /// </author>
    /// </summary>
    public partial class BaseDictionaryEntity : BaseEntity
    {
        /// <summary>
        /// 租户号
        /// </summary>
        [FieldDescription("租户号")]
        public int TenantId { get; set; } = 0;

        /// <summary>
        /// 编码
        /// </summary>
        [FieldDescription("编码")]
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// 名称
        /// </summary>
        [FieldDescription("名称")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 树型结构
        /// </summary>
        [FieldDescription("树型结构")]
        public int IsTree { get; set; } = 0;

        /// <summary>
        /// 允许编辑
        /// </summary>
        [FieldDescription("允许编辑")]
        public int AllowEdit { get; set; } = 1;

        /// <summary>
        /// 允许删除
        /// </summary>
        [FieldDescription("允许删除")]
        public int AllowDelete { get; set; } = 1;

        /// <summary>
        /// 描述
        /// </summary>
        [FieldDescription("描述")]
        public string Description { get; set; } = string.Empty;
        
        /// <summary>
        /// 从数据行读取
        /// </summary>
        /// <param name="dr">数据行</param>
        protected override BaseEntity GetFrom(IDataRow dr)
        {
            GetFromExtend(dr);
            GetBase(dr);
            if (dr.ContainsColumn(FieldTenantId))
            {
                TenantId = BaseUtil.ConvertToInt(dr[FieldTenantId]);
            }
            if (dr.ContainsColumn(FieldCode))
            {
                Code = BaseUtil.ConvertToString(dr[FieldCode]);
            }
            if (dr.ContainsColumn(FieldName))
            {
                Name = BaseUtil.ConvertToString(dr[FieldName]);
            }
            if (dr.ContainsColumn(FieldIsTree))
            {
                IsTree = BaseUtil.ConvertToInt(dr[FieldIsTree]);
            }
            if (dr.ContainsColumn(FieldAllowEdit))
            {
                AllowEdit = BaseUtil.ConvertToInt(dr[FieldAllowEdit]);
            }
            if (dr.ContainsColumn(FieldAllowDelete))
            {
                AllowDelete = BaseUtil.ConvertToInt(dr[FieldAllowDelete]);
            }
            if (dr.ContainsColumn(FieldDescription))
            {
                Description = BaseUtil.ConvertToString(dr[FieldDescription]);
            }            
            return this;
        }

        ///<summary>
        /// 字典
        ///</summary>
        [FieldDescription("字典")]
        public const string CurrentTableName = "BaseDictionary";

        ///<summary>
        /// 租户号
        ///</summary>
        public const string FieldTenantId = "TenantId";

        ///<summary>
        /// 编码
        ///</summary>
        public const string FieldCode = "Code";

        ///<summary>
        /// 名称
        ///</summary>
        public const string FieldName = "Name";

        ///<summary>
        /// 树型结构
        ///</summary>
        public const string FieldIsTree = "IsTree";

        ///<summary>
        /// 允许编辑
        ///</summary>
        public const string FieldAllowEdit = "AllowEdit";

        ///<summary>
        /// 允许删除
        ///</summary>
        public const string FieldAllowDelete = "AllowDelete";

        ///<summary>
        /// 描述
        ///</summary>
        public const string FieldDescription = "Description";
    }
}
