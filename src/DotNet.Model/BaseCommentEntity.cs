//-----------------------------------------------------------------------
// <copyright file="BaseCommentEntity.cs" company="DotNet">
//     Copyright (C) 2021, All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;


namespace DotNet.Model
{
    using Util;

    /// <summary>
    /// BaseCommentEntity
    /// 评论表
    /// 
    /// 修改记录
    /// 
    /// 2012-05-14 版本：1.0 JiRiGaLa 创建主键。
    /// Important，PriorityId。
    /// 
    /// <author>
    ///     <name>Troy.Cui</name>
    ///     <date>2012-05-14</date>
    /// </author>
    /// </summary>
    [Serializable]
    public partial class BaseCommentEntity : BaseEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 部门主键
        /// </summary>
        public string DepartmentId { get; set; }

        /// <summary>
        /// 部门名称
        /// </summary>
        public string DepartmentName { get; set; }

        /// <summary>
        /// 父亲节点主键
        /// </summary>
        public string ParentId { get; set; }

        /// <summary>
        /// 分类编号
        /// </summary>
        public string CategoryCode { get; set; }

        /// <summary>
        /// 唯一识别主键
        /// </summary>
        public string ObjectId { get; set; }

        /// <summary>
        /// 消息的指向网页页面
        /// </summary>
        public string TargetUrl { get; set; }

        /// <summary>
        /// 主题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        public string Contents { get; set; }

        /// <summary>
        /// IP地址
        /// </summary>
        public string IpAddress { get; set; }

        /// <summary>
        /// 已被处理标志
        /// </summary>
        public int? Worked { get; set; }

        /// <summary>
        /// 是否删除
        /// </summary>
        public int? DeletionStateCode { get; set; }

        /// <summary>
        /// 有效
        /// </summary>
        public int? Enabled { get; set; }

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
            DepartmentId = BaseUtil.ConvertToString(dr[FieldDepartmentId]);
            DepartmentName = BaseUtil.ConvertToString(dr[FieldDepartmentName]);
            ParentId = BaseUtil.ConvertToString(dr[FieldParentId]);
            CategoryCode = BaseUtil.ConvertToString(dr[FieldCategoryCode]);
            ObjectId = BaseUtil.ConvertToString(dr[FieldObjectId]);
            TargetUrl = BaseUtil.ConvertToString(dr[FieldTargetUrl]);
            Title = BaseUtil.ConvertToString(dr[FieldTitle]);
            Contents = BaseUtil.ConvertToString(dr[FieldContents]);
            IpAddress = BaseUtil.ConvertToString(dr[FieldIpAddress]);
            Worked = BaseUtil.ConvertToInt(dr[FieldWorked]);
            DeletionStateCode = BaseUtil.ConvertToInt(dr[FieldDeleted]);
            Enabled = BaseUtil.ConvertToInt(dr[FieldEnabled]);
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
        /// 评论表
        ///</summary>
        [NonSerialized]
        public const string TableName = "BaseComment";

        ///<summary>
        /// 主键
        ///</summary>
        [NonSerialized]
        public const string FieldId = "Id";

        ///<summary>
        /// 部门代码
        ///</summary>
        [NonSerialized]
        public const string FieldDepartmentId = "DepartmentId";

        ///<summary>
        /// 部门名称
        ///</summary>
        [NonSerialized]
        public const string FieldDepartmentName = "DepartmentName";

        ///<summary>
        /// 父亲节点主键
        ///</summary>
        [NonSerialized]
        public const string FieldParentId = "ParentId";

        ///<summary>
        /// 分类编号
        ///</summary>
        [NonSerialized]
        public const string FieldCategoryCode = "CategoryCode";

        ///<summary>
        /// 唯一识别主键
        ///</summary>
        [NonSerialized]
        public const string FieldObjectId = "ObjectId";

        ///<summary>
        /// 消息的指向网页页面
        ///</summary>
        [NonSerialized]
        public const string FieldTargetUrl = "TargetURL";

        ///<summary>
        /// 主题
        ///</summary>
        [NonSerialized]
        public const string FieldTitle = "Title";

        ///<summary>
        /// 内容
        ///</summary>
        [NonSerialized]
        public const string FieldContents = "Contents";

        ///<summary>
        /// IP地址
        ///</summary>
        [NonSerialized]
        public const string FieldIpAddress = "IPAddress";

        ///<summary>
        /// 已被处理标
        ///</summary>
        [NonSerialized]
        public const string FieldWorked = "Worked";

        ///<summary>
        /// 是否删除
        ///</summary>
        [NonSerialized]
        public const string FieldDeleted = "DeletionStateCode";

        ///<summary>
        /// 有效
        ///</summary>
        [NonSerialized]
        public const string FieldEnabled = "Enabled";

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
