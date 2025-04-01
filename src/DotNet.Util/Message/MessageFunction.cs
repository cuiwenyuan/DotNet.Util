//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2025, DotNet.
//-----------------------------------------------------------------

namespace DotNet.Util
{
    #region public enum MessageFunction 消息功能分类
    /// <summary>
    /// 消息功能分类
    /// </summary>
    public enum MessageFunction
    {
        /// <summary>
        /// 0 消息
        /// </summary>
        Message = 0,

        /// <summary>
        /// 1 提示
        /// </summary>
        Remind = 1,

        /// <summary>
        /// 2 警示
        /// </summary>
        Warning = 2,

        /// <summary>
        /// 3 待审核事项
        /// </summary>
        WaitForAudit = 3,

        /// <summary>
        /// 4 评论
        /// </summary>
        Comment = 4,

        /// <summary>
        /// 5 待审核
        /// </summary>
        TodoList = 5,

        /// <summary>
        /// 6 备忘录
        /// </summary>
        Note = 6,

        /// <summary>
        /// 7 用户消息
        /// </summary>
        UserMessage = 7,

        /// <summary>
        /// 8 角色消息
        /// </summary>
        RoleMessage = 8,

        /// <summary>
        /// 9 组织消息
        /// </summary>
        OrganizationMessage = 9,

        /// <summary>
        /// 外部微信信息
        /// </summary>
        WeChat = 10,

        /// <summary>
        /// 外部易信信息
        /// </summary>
        YiXin = 11,

        /// <summary>
        /// 微信等外部信息
        /// </summary>
        //  External = 12,

        SystemPush = 13
    }
    #endregion
}