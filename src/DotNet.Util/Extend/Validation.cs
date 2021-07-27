using System.Text.RegularExpressions;
using System.IO;

namespace DotNet.Util
{
    public class Validation
    {
        /// <summary>
        /// 文件夹名检查
        /// </summary>
        /// <param name="folderName">文件夹名</param>
        /// <returns>检查通过</returns>
        public static bool CheckFolderName(string folderName)
        {
            var returnValue = true;
            if (folderName.Trim().Length == 0)
            {
                returnValue = false;
            }
            else
            {
                if ((folderName.IndexOfAny(Path.GetInvalidPathChars()) >= 0) || (folderName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0))
                {
                    returnValue = false;
                }
            }
            return returnValue;
        }

        /// <summary>
        /// 文件名检查
        /// </summary>
        /// <param name="folderName">文件名</param>
        /// <returns>检查通过</returns>
        public static bool CheckFileName(string fileName)
        {
            var returnValue = true;
            if (fileName.Trim().Length == 0)
            {
                returnValue = false;
            }
            else
            {
                if (fileName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
                {
                    returnValue = false;
                }
            }
            return returnValue;
        }

        /// <summary>
        /// 密码强度检查
        /// </summary>
        /// <param name="password">密码</param>
        /// <returns>检查通过</returns>
        public static bool CheckPasswordStrength(string password)
        {
            var returnValue = true;
            if (string.IsNullOrEmpty(password))
            {
                returnValue = false;
            }
            var isDigit = false;
            var isLetter = false;
            for (var i = 0; i < password.Length; i++)
            {
                if (!isDigit)
                {
                    isDigit = char.IsDigit(password[i]);
                }
                if (!isLetter)
                {
                    isLetter = char.IsLetter(password[i]);
                }
            }
            returnValue = (isDigit && isLetter);
            // 密码至少为6位，为数字加字母
            if (password.Length < 6)
            {
                returnValue = false;
            }
            return returnValue;
        }

        /// <summary>是否空</summary>
        /// <param name="strInput">输入字符串</param>
        /// <returns>true/false</returns>
        public static bool IsBlank(string strInput)
        {
            return string.IsNullOrEmpty(strInput);
        }

        /// <summary>是否数字</summary>
        /// <param name="strInput">输入字符串</param>
        /// <returns>true/false</returns>
        public static bool IsNumeric(string strInput)
        {
            if (string.IsNullOrEmpty(strInput))
                return false;
            var reg = new Regex(@"^[-]?\d+[.]?\d*$");
            return reg.IsMatch(strInput);
        }

        /// <summary>
        /// 是不是时间类型数据
        /// </summary>
        /// <param name="strDate"></param>
        /// <returns></returns>
        public static bool IsDateTime(string strDate)
        {
            var reg =
                new Regex(
                    @"(((^((1[8-9]\d{2})|([2-9]\d{3}))([-\/\._])(10|12|0?[13578])([-\/\._])(3[01]|[12][0-9]|0?[1-9]))|(^((1[8-9]\d{2})|([2-9]\d{3}))([-\/\._])(11|0?[469])([-\/\._])(30|[12][0-9]|0?[1-9]))|(^((1[8-9]\d{2})|([2-9]\d{3}))([-\/\._])(0?2)([-\/\._])(2[0-8]|1[0-9]|0?[1-9]))|(^([2468][048]00)([-\/\._])(0?2)([-\/\._])(29))|(^([3579][26]00)([-\/\._])(0?2)([-\/\._])(29))|(^([1][89][0][48])([-\/\._])(0?2)([-\/\._])(29))|(^([2-9][0-9][0][48])([-\/\._])(0?2)([-\/\._])(29))|(^([1][89][2468][048])([-\/\._])(0?2)([-\/\._])(29))|(^([2-9][0-9][2468][048])([-\/\._])(0?2)([-\/\._])(29))|(^([1][89][13579][26])([-\/\._])(0?2)([-\/\._])(29))|(^([2-9][0-9][13579][26])([-\/\._])(0?2)([-\/\._])(29)))((\s+(0?[1-9]|1[012])(:[0-5]\d){0,2}(\s[AP]M))?$|(\s+([01]\d|2[0-3])(:[0-5]\d){0,2})?$))");
            return reg.IsMatch(strDate);
        }

        /// <summary>
        /// 检查邮件是否合法
        /// </summary>
        /// <param name="inputEmail"></param>
        /// <returns></returns>
        public static bool IsEmail(string email)
        {
            email = email.Trim();
            const string regexString =
                @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
            var regex = new Regex(regexString);
            return regex.IsMatch(email);
        }

        /// <summary>
        /// 验证手机
        /// </summary>
        /// <param name="mobile">手机号</param>
        /// <returns>成功与否</returns>
        public static bool IsMobile(string mobile)
        {
            var format = @"^13[0-9]{9}|15[012356789][0-9]{8}|18[0256789][0-9]{8}|147[0-9]{8}$";
            return Regex.IsMatch(mobile, format);
        }

        /// <summary>
        /// 验证密码格式
        /// </summary>
        /// <param name="password">密码</param>
        /// <returns>成功与否</returns>
        public static bool IsPassword(string password)
        {
            // 6-20位
            var format = @"^\S{6,20}$";
            return Regex.IsMatch(password, format);
        }

        /// <summary>
        /// 验证用户名格式
        /// </summary>
        /// <param name="password">密码</param>
        /// <returns>成功与否</returns>
        public static bool IsLoginName(string loginname)
        {
            // 1-30位
            // \w 匹配字母或数字或下划线或汉字
            // \W 匹配任意不是字母，数字，下划线，汉字的字符
            // \S 匹配任意不是空白符的字符 
            var format = @"^\S{1,30}$";
            return Regex.IsMatch(loginname, format);
        }

        /// <summary>
        /// 验证QQ格式
        /// </summary>
        /// <param name="qq">QQ</param>
        /// <returns>成功与否</returns>
        public static bool IsQq(string qq)
        {
            // 最多10位
            var format = @"^[1-9]*[1-9][0-9]*$";
            return Regex.IsMatch(qq, format);
        }
    }
}