//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2025, DotNet.
//-----------------------------------------------------------------

namespace DotNet.Util
{
    /// <summary>
    /// AuditStatus
    /// 审核状态。
    /// 从送审人的角度，从审核人的角度，其实从2个角度看待，名称是需要区别对待才可以的。
    /// 
    /// 修改记录
    /// 
    ///     2013.01.24 版本：2.1 JiRiGaLa 改进枚举类型的说明。
    ///     2011.10.13 版本：2.0 JiRiGaLa 改进枚举类型的说明。
    ///		2009.09.04 版本：1.0 JiRiGaLa 重新调整主键的规范化。
    ///		
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2013.01.24</date>
    /// </author> 
    /// </summary>    
    #region public enum AuditStatus 审核状态
    public enum AuditStatus
    {
        /// <summary>
        /// 00 流程暂停
        /// </summary>
        [EnumDescription("暂停")]
        Pause = 0,

        /// <summary>
        /// 01 草稿状态
        /// </summary>
        [EnumDescription("草稿")]
        Draft = 1,

        /// <summary>
        /// 02 开始审核,送审
        /// </summary>
        [EnumDescription("提交")]
        StartAudit = 2,

        /// <summary>
        /// 03 审核通过
        /// </summary>
        [EnumDescription("通过")]
        AuditPass = 3,

        /// <summary>
        /// 04 待审核
        /// </summary>
        [EnumDescription("待审")]
        WaitForAudit = 4,

        /// <summary>
        /// 05 转发
        /// </summary>
        [EnumDescription("转发")]
        Transmit = 5,

        /// <summary>
        /// 06 已退回
        /// </summary>
        [EnumDescription("退回")]
        AuditReject = 6,

        /// <summary>
        /// 07 审核结束
        /// </summary>
        [EnumDescription("完成")]
        AuditComplete = 7,

        /// <summary>
        /// 08 撤销,废弃
        /// </summary>
        [EnumDescription("废弃")]
        AuditQuash = 8,

        /// <summary>
        /// 09 受理，信息已经看到，但是需要一个过程处理
        /// </summary>
        [EnumDescription("受理")]
        AuditAccept = 9
    }
    #endregion
}