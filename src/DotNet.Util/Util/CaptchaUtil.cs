#if NET40_OR_GREATER
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotNet.Util
{
    /// <summary>
    /// 验证码工具
    /// </summary>
    public partial class CaptchaUtil
    {
        /// <summary>
        /// 验证安全码
        /// </summary>
        /// <param name="strInput">输入的安全码</param>
        /// <returns>成功与否</returns>
        public static bool IsCorrectCaptchaCode(string strInput)
        {
            if (SessionUtil.Get("CaptchaCode") != null)
            {
                var captchCode = SessionUtil.Get("CaptchaCode").Trim();
                if (string.Equals(captchCode.ToLower(), strInput.ToLower(), StringComparison.Ordinal))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
#endif