//-----------------------------------------------------------------------
// <copyright file="BaseLogEntity.Auto.cs" company="DotNet">
//     Copyright (c) 2021, All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;


namespace DotNet.Model
{
    using Util;

    /// <summary>
    /// BaseLogEntity
    /// 系统日志
    /// 
    /// 修改记录
    /// 
    /// 2021-09-27 版本：1.0 Troy.Cui 创建文件。
    /// 
    /// <author>
    ///     <name>Troy.Cui</name>
    ///     <date>2021-09-27</date>
    /// </author>
    /// </summary>
    public partial class BaseLogEntity : BaseEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        [FieldDescription("主键")]
        public int Id { get; set; }

        /// <summary>
        /// 公司主键
        /// </summary>
        [FieldDescription("公司主键")]
        public int CompanyId { get; set; } = 0;

        /// <summary>
        /// 用户主键
        /// </summary>
        [FieldDescription("用户主键")]
        public int UserId { get; set; } = 0;

        /// <summary>
        /// 用户名
        /// </summary>
        [FieldDescription("用户名")]
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// 用户姓名
        /// </summary>
        [FieldDescription("用户姓名")]
        public string RealName { get; set; } = string.Empty;

        /// <summary>
        /// 服务
        /// </summary>
        [FieldDescription("服务")]
        public string Service { get; set; } = string.Empty;

        /// <summary>
        /// 任务
        /// </summary>
        [FieldDescription("任务")]
        public string TaskId { get; set; } = string.Empty;

        /// <summary>
        /// 操作记录,添加,编辑,删除参数
        /// </summary>
        [FieldDescription("操作记录,添加,编辑,删除参数")]
        public string Parameters { get; set; } = string.Empty;

        /// <summary>
        /// IP地址
        /// </summary>
        [FieldDescription("IP地址")]
        public string ClientIp { get; set; } = string.Empty;

        /// <summary>
        /// IP地址
        /// </summary>
        [FieldDescription("IP地址")]
        public string ServerIp { get; set; } = string.Empty;

        /// <summary>
        /// 上一网络地址
        /// </summary>
        [FieldDescription("上一网络地址")]
        public string UrlReferrer { get; set; } = string.Empty;

        /// <summary>
        /// 网络地址
        /// </summary>
        [FieldDescription("网络地址")]
        public string WebUrl { get; set; } = string.Empty;

        /// <summary>
        /// 耗时
        /// </summary>
        [FieldDescription("耗时")]
        public decimal? ElapsedTicks { get; set; } = 0;

        /// <summary>
        /// 开始时间
        /// </summary>
        [FieldDescription("开始时间")]
        public DateTime? StartTime { get; set; } = DateTime.Now;

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
            if (dr.ContainsColumn(FieldCompanyId))
            {
                CompanyId = BaseUtil.ConvertToInt(dr[FieldCompanyId]);
            }
            if (dr.ContainsColumn(FieldUserId))
            {
                UserId = BaseUtil.ConvertToInt(dr[FieldUserId]);
            }
            if (dr.ContainsColumn(FieldUserName))
            {
                UserName = BaseUtil.ConvertToString(dr[FieldUserName]);
            }
            if (dr.ContainsColumn(FieldRealName))
            {
                RealName = BaseUtil.ConvertToString(dr[FieldRealName]);
            }
            if (dr.ContainsColumn(FieldService))
            {
                Service = BaseUtil.ConvertToString(dr[FieldService]);
            }
            if (dr.ContainsColumn(FieldTaskId))
            {
                TaskId = BaseUtil.ConvertToString(dr[FieldTaskId]);
            }
            if (dr.ContainsColumn(FieldParameters))
            {
                Parameters = BaseUtil.ConvertToString(dr[FieldParameters]);
            }
            if (dr.ContainsColumn(FieldClientIp))
            {
                ClientIp = BaseUtil.ConvertToString(dr[FieldClientIp]);
            }
            if (dr.ContainsColumn(FieldServerIp))
            {
                ServerIp = BaseUtil.ConvertToString(dr[FieldServerIp]);
            }
            if (dr.ContainsColumn(FieldUrlReferrer))
            {
                UrlReferrer = BaseUtil.ConvertToString(dr[FieldUrlReferrer]);
            }
            if (dr.ContainsColumn(FieldWebUrl))
            {
                WebUrl = BaseUtil.ConvertToString(dr[FieldWebUrl]);
            }
            if (dr.ContainsColumn(FieldElapsedTicks))
            {
                ElapsedTicks = BaseUtil.ConvertToNullableDecimal(dr[FieldElapsedTicks]);
            }
            if (dr.ContainsColumn(FieldStartTime))
            {
                StartTime = BaseUtil.ConvertToNullableDateTime(dr[FieldStartTime]);
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
        /// 系统日志
        ///</summary>
        [FieldDescription("系统日志")]
        public const string CurrentTableName = "BaseLog";

        ///<summary>
        /// 主键
        ///</summary>
        public const string FieldId = "Id";

        ///<summary>
        /// 公司主键
        ///</summary>
        public const string FieldCompanyId = "CompanyId";

        ///<summary>
        /// 用户主键
        ///</summary>
        public const string FieldUserId = "UserId";

        ///<summary>
        /// 用户名
        ///</summary>
        public const string FieldUserName = "UserName";

        ///<summary>
        /// 用户姓名
        ///</summary>
        public const string FieldRealName = "RealName";

        ///<summary>
        /// 服务
        ///</summary>
        public const string FieldService = "Service";

        ///<summary>
        /// 任务
        ///</summary>
        public const string FieldTaskId = "TaskId";

        ///<summary>
        /// 操作记录,添加,编辑,删除参数
        ///</summary>
        public const string FieldParameters = "Parameters";

        ///<summary>
        /// IP地址
        ///</summary>
        public const string FieldClientIp = "ClientIp";

        ///<summary>
        /// IP地址
        ///</summary>
        public const string FieldServerIp = "ServerIp";

        ///<summary>
        /// 上一网络地址
        ///</summary>
        public const string FieldUrlReferrer = "UrlReferrer";

        ///<summary>
        /// 网络地址
        ///</summary>
        public const string FieldWebUrl = "WebUrl";

        ///<summary>
        /// 耗时
        ///</summary>
        public const string FieldElapsedTicks = "ElapsedTicks";

        ///<summary>
        /// 开始时间
        ///</summary>
        public const string FieldStartTime = "StartTime";

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