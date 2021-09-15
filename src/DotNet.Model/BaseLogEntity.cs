//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System;


namespace DotNet.Model
{
    using Util;

    /// <summary>
    /// BaseLogEntity
    /// 系统日志
    /// 
    /// 想在这里实现访问记录、继承以前的比较好的思路。
    ///
    /// 修改记录
    /// 
    ///     2016.02.13 版本：2.7 JiRiGaLa   增加字段 TaskId、Service、ElapsedTicks。
    ///     2011.03.24 版本：2.6 JiRiGaLa   讲程序转移到DotNet.BaseManager命名空间中。
    ///     2007.12.03 版本：2.3 JiRiGaLa   进行规范化整理。
    ///     2007.11.08 版本：2.2 JiRiGaLa   整理多余的主键（OK）。
    ///		2007.07.09 版本：2.1 JiRiGaLa   程序整理，修改 Static 方法，采用 Instance 方法。
    ///		2006.12.02 版本：2.0 JiRiGaLa   程序整理，错误方法修改。
    ///		2004.07.28 版本：1.0 JiRiGaLa   进行了排版、方法规范化、接口继承、索引器。
    ///		2004.11.12 版本：1.0 JiRiGaLa   删除了一些方法。
    ///		2005.09.30 版本：1.0 JiRiGaLa   又进行一次飞跃，把一些思想进行了统一。
    /// 
    /// <author>
    ///		<name>JiRiGaLa</name>
    ///		<date>2016.02.13</date>
    /// </author> 
    /// </summary>
    [Serializable]
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
        /// 描述
        /// </summary>
        [FieldDescription("描述")]
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// 开始时间
        /// </summary>
        [FieldDescription("开始时间")]
        public DateTime? StartTime { get; set; } = DateTime.Now;

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
        public DateTime CreateOn { get; set; } = DateTime.Now;

        /// <summary>
        /// 创建人编号
        /// </summary>
        [FieldDescription("创建人编号")]
        public int CreateUserId { get; set; } = 0;

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
        public DateTime ModifiedOn { get; set; } = DateTime.Now;

        /// <summary>
        /// 最近修改人编号
        /// </summary>
        [FieldDescription("最近修改人编号")]
        public int ModifiedUserId { get; set; } = 0;

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
        /// 创建人用户名
        /// </summary>
        [FieldDescription("创建人用户名")]
        public string CreateUserName { get; set; } = string.Empty;

        /// <summary>
        /// 修改人用户名
        /// </summary>
        [FieldDescription("修改人用户名")]
        public string ModifiedUserName { get; set; } = string.Empty;

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
            if (dr.ContainsColumn(FieldDescription))
            {
                Description = BaseUtil.ConvertToString(dr[FieldDescription]);
            }
            if (dr.ContainsColumn(FieldStartTime))
            {
                StartTime = BaseUtil.ConvertToNullableDateTime(dr[FieldStartTime]);
            }
            if (dr.ContainsColumn(FieldSortCode))
            {
                SortCode = BaseUtil.ConvertToNullableInt(dr[FieldSortCode]);
            }
            if (dr.ContainsColumn(FieldDeleted))
            {
                DeletionStateCode = BaseUtil.ConvertToNullableByteInt(dr[FieldDeleted]);
            }
            if (dr.ContainsColumn(FieldEnabled))
            {
                Enabled = BaseUtil.ConvertToNullableByteInt(dr[FieldEnabled]);
            }
            if (dr.ContainsColumn(FieldCreateTime))
            {
                CreateOn = BaseUtil.ConvertToDateTime(dr[FieldCreateTime]);
            }
            if (dr.ContainsColumn(FieldCreateUserId))
            {
                CreateUserId = BaseUtil.ConvertToInt(dr[FieldCreateUserId]);
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
                ModifiedOn = BaseUtil.ConvertToDateTime(dr[FieldUpdateTime]);
            }
            if (dr.ContainsColumn(FieldUpdateUserId))
            {
                ModifiedUserId = BaseUtil.ConvertToInt(dr[FieldUpdateUserId]);
            }
            if (dr.ContainsColumn(FieldUpdateBy))
            {
                ModifiedBy = BaseUtil.ConvertToString(dr[FieldUpdateBy]);
            }
            if (dr.ContainsColumn(FieldUpdateIp))
            {
                ModifiedIp = BaseUtil.ConvertToString(dr[FieldUpdateIp]);
            }
            if (dr.ContainsColumn(FieldCreateUserName))
            {
                CreateUserName = BaseUtil.ConvertToString(dr[FieldCreateUserName]);
            }
            if (dr.ContainsColumn(FieldUpdateUserName))
            {
                ModifiedUserName = BaseUtil.ConvertToString(dr[FieldUpdateUserName]);
            }
            return this;
        }

        ///<summary>
        /// 系统日志
        ///</summary>
        [FieldDescription("系统日志")]
        public const string TableName = "BaseLog";

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

        /// <summary>
        /// IP地址
        /// </summary>
        public const string FieldClientIp = "ClientIP";

        /// <summary>
        /// IP地址
        /// </summary>
        public const string FieldServerIp = "ServerIP";

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
        /// 描述
        ///</summary>
        public const string FieldDescription = "Description";

        ///<summary>
        /// 开始时间
        ///</summary>
        public const string FieldStartTime = "StartTime";

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

        ///<summary>
        /// 创建人用户名
        ///</summary>
        public const string FieldCreateUserName = "CreateUserName";

        ///<summary>
        /// 修改人用户名
        ///</summary>
        public const string FieldUpdateUserName = "ModifiedUserName";
    }
}