#if NET46_OR_GREATER
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
            if (strInput.IsNullOrEmpty())
            {
                return false;
            }
            var captchaCode = SessionUtil.Get("CaptchaCode");
            if (captchaCode != null)
            {
                // 一次性使用：取出后立即清除，防止验证码重放
                SessionUtil.Clear("CaptchaCode");
                if (string.Equals(captchaCode.Trim(), strInput.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
#endif
