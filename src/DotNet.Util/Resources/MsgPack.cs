//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2026, DotNet.
//-----------------------------------------------------------------

namespace DotNet.Util
{
    /// <summary>
    ///	MsgPack
    /// 内置语言包登记处。各语言的词条按文件分区维护（partial class 合并）：
    ///		MsgPack.zh-CN.cs	中文包（默认）
    ///		MsgPack.en.cs		英文包（第一个语言包）
    /// 
    /// 约定：所有语言包的键集合必须完全一致，由单元测试 MsgTests 断言校验。
    /// 
    /// 修改记录
    /// 
    ///		2026.09.15 版本：1.0	Troy.Cui 建立。
    ///	
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2026.09.15</date>
    /// </author> 
    /// </summary>
    public static partial class MsgPack
    {
        private static bool _registered;

        /// <summary>
        /// 确保内置语言包已注册到 Msg。由 Msg 惰性调用，Register 不会反向触发，避免递归。
        /// </summary>
        internal static void EnsureRegistered()
        {
            if (_registered)
            {
                return;
            }

            Msg.RegisterCore(Msg.DefaultLanguage, ZhCnMessages());
            Msg.RegisterCore("en", EnMessages());

            _registered = true;
        }

        /// <summary>
        /// 复位登记状态，供 Msg.Clear() 调用，使下次取值能重新载入内置语言包。
        /// </summary>
        internal static void Reset()
        {
            _registered = false;
        }
    }
}
