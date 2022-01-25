//-----------------------------------------------------------------------
// <copyright file="BaseDictionaryItemEntity.Auto.cs" company="DotNet">
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
    /// BaseDictionaryItemEntity
    /// 字典项
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
    public partial class BaseDictionaryItemEntity : BaseEntity
    {
        /// <summary>
        /// 编号
        /// </summary>
        [FieldDescription("编号")]
        public int Id { get; set; }

        /// <summary>
        /// 字典编号
        /// </summary>
        [FieldDescription("字典编号")]
        public int DictionaryId { get; set; }

        /// <summary>
        /// 父节点主键
        /// </summary>
        [FieldDescription("父节点主键")]
        public int ParentId { get; set; } = 0;

        /// <summary>
        /// 键
        /// </summary>
        [FieldDescription("键")]
        public string ItemKey { get; set; } = string.Empty;

        /// <summary>
        /// 名称
        /// </summary>
        [FieldDescription("名称")]
        public string ItemName { get; set; } = string.Empty;

        /// <summary>
        /// 值
        /// </summary>
        [FieldDescription("值")]
        public string ItemValue { get; set; } = string.Empty;

        /// <summary>
        /// 语言(i18n)
        /// </summary>
        [FieldDescription("语言(i18n)")]
        public string Language { get; set; } = "global";

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
            if (dr.ContainsColumn(FieldDictionaryId))
            {
                DictionaryId = BaseUtil.ConvertToInt(dr[FieldDictionaryId]);
            }
            if (dr.ContainsColumn(FieldParentId))
            {
                ParentId = BaseUtil.ConvertToInt(dr[FieldParentId]);
            }
            if (dr.ContainsColumn(FieldItemKey))
            {
                ItemKey = BaseUtil.ConvertToString(dr[FieldItemKey]);
            }
            if (dr.ContainsColumn(FieldItemName))
            {
                ItemName = BaseUtil.ConvertToString(dr[FieldItemName]);
            }
            if (dr.ContainsColumn(FieldItemValue))
            {
                ItemValue = BaseUtil.ConvertToString(dr[FieldItemValue]);
            }
            if (dr.ContainsColumn(FieldLanguage))
            {
                Language = BaseUtil.ConvertToString(dr[FieldLanguage]);
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
        /// 字典项
        ///</summary>
        [FieldDescription("字典项")]
        public const string CurrentTableName = "BaseDictionaryItem";

        ///<summary>
        /// 编号
        ///</summary>
        public const string FieldId = "Id";

        ///<summary>
        /// 字典编号
        ///</summary>
        public const string FieldDictionaryId = "DictionaryId";

        ///<summary>
        /// 父节点主键
        ///</summary>
        public const string FieldParentId = "ParentId";

        ///<summary>
        /// 键
        ///</summary>
        public const string FieldItemKey = "ItemKey";

        ///<summary>
        /// 名称
        ///</summary>
        public const string FieldItemName = "ItemName";

        ///<summary>
        /// 值
        ///</summary>
        public const string FieldItemValue = "ItemValue";

        ///<summary>
        /// 语言(i18n)
        ///</summary>
        public const string FieldLanguage = "Language";

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
