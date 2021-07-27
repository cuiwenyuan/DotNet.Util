//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2020, DotNet.
//-----------------------------------------------------------------

namespace DotNet.Util
{
    #region public enum MessageFunction 消息功能分类
    /// <summary>
    /// 消息功能分类
    /// </summary>
    public enum MessageFunction
    {
        Message = 0,        // 0 消息。

        Remind = 1,         // 1 提示。
        
        Warning = 2,        // 2 警示。
        
        WaitForAudit = 3,   // 3 待审核事项。
        
        Comment = 4,        // 4 评论。
        
        TodoList = 5,       // 5 待审核。
        
        Note = 6,           // 6 备忘录。

        UserMessage = 7,
        
        RoleMessage = 8,
        
        OrganizeMessage = 9,

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