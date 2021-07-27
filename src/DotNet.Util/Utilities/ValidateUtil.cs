﻿//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2020, DotNet.
//-----------------------------------------------------------------

using System;
using System.IO;
using System.Text.RegularExpressions;

namespace DotNet.Util
{
    public static class ValidateUtil
    {
        public static string PositiveInteger = @"^[0-9]*[1-9][0-9]*$";// 正整数正则表达式

        public static string Integer = @"^-?\d+$";// 整数正则表达式

        public static string Float = @"^[+|-]?\d*\.?\d*$";// 浮点数正则表达式

        /// <summary>
        /// 匹配是否是ipV4地址
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <returns>是返回 true 不是返回false</returns>
        public static bool IsIpv4(string ipAddress)
        {
            var match =
               new Regex(@"^(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9])\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[0-9])$");
            if (!match.IsMatch(ipAddress))
            {
                return (IsIpv6(ipAddress));
            }
            return true;
        }

        /// <summary>
        /// 判断输入的字符串是否是合法的IPV6 地址
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <returns></returns>
        public static bool IsIpv6(string ipAddress)
        {
            var pattern = "";
            var temp = ipAddress;
            var strs = temp.Split(':');
            if (strs.Length > 8)
            {
                return false;
            }
            var count = GetStringCount(ipAddress, "::");
            if (count > 1)
            {
                return false;
            }
            else if (count == 0)
            {
                pattern = @"^([\da-f]{1,4}:){7}[\da-f]{1,4}$";

                var regex = new Regex(pattern);
                return regex.IsMatch(ipAddress);
            }
            else
            {
                pattern = @"^([\da-f]{1,4}:){0,5}::([\da-f]{1,4}:){0,5}[\da-f]{1,4}$";
                var regex1 = new Regex(pattern);
                return regex1.IsMatch(ipAddress);
            }
        }

        /* *******************************************************************
         * 1、通过“:”来分割字符串看得到的字符串数组长度是否小于等于8
         * 2、判断输入的IPV6字符串中是否有“::”。
         * 3、如果没有“::”采用 ^([\da-f]{1,4}:){7}[\da-f]{1,4}$ 来判断
         * 4、如果有“::” ，判断"::"是否止出现一次
         * 5、如果出现一次以上 返回false
         * 6、^([\da-f]{1,4}:){0,5}::([\da-f]{1,4}:){0,5}[\da-f]{1,4}$
         * ******************************************************************/
        /// <summary>
        /// 判断字符串compare 在 input字符串中出现的次数
        /// </summary>
        /// <param name="input">源字符串</param>
        /// <param name="compare">用于比较的字符串</param>
        /// <returns>字符串compare 在 input字符串中出现的次数</returns>
        private static int GetStringCount(string input, string compare)
        {
            var index = input.IndexOf(compare, StringComparison.Ordinal);
            if (index != -1)
            {
                return 1 + GetStringCount(input.Substring(index + compare.Length), compare);
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// 验证是否为正整数
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public static bool UnsafeCharacter(string expression)
        {
            var result = false;
            if (!result)
            {
                result = expression.IndexOf('\'') >= 0;
            }
            if (!result)
            {
                result = expression.IndexOf('<') >= 0;
            }
            if (!result)
            {
                result = expression.IndexOf('>') >= 0;
            }
            if (!result)
            {
                result = expression.IndexOf('%') >= 0;
            }
            if (!result)
            {
                result = expression.IndexOf('_') >= 0;
            }
            if (!result)
            {
                result = expression.IndexOf('?') >= 0;
            }
            return result;
        }

        /// <summary>
        /// 验证是否为正整数
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public static bool IsInt(string expression)
        {
            var result = false;
            if (!string.IsNullOrWhiteSpace(expression))
            {
                result = Regex.IsMatch(expression.Trim(), @"^[0-9]*$");
            }
            return result;
        }

        /// <summary>
        /// 验证是否为正整数
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public static bool IsLong(string expression)
        {
            var result = false;
            if (!string.IsNullOrWhiteSpace(expression))
            {
                result = Regex.IsMatch(expression.Trim(), @"^[1-9]*$");
            }
            return result;
        }

        public static bool IsBoolean(string expression)
        {
            var result = false;
            if (!string.IsNullOrWhiteSpace(expression))
            {
                if (expression.Equals(true.ToString())
                    || expression.Equals(false.ToString()))
                {
                    result = true;
                }
            }
            return result;
        }


        /// <summary>
        /// 判断对象是否为Numeric类型的数字
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public static bool IsNumeric(object expression)
        {
            var result = false;
            if (expression != null)
            {
                var numericValue = expression.ToString();
                //if (numericValue.Length > 0 && numericValue.Length <= 11 && Regex.IsMatch(numericValue, @"^[-]?[0-9]*[.]?[0-9]*$"))
                //{
                //    if ((numericValue.Length < 10) || (numericValue.Length == 10 && numericValue[0] == '1') || (numericValue.Length == 11 && numericValue[0] == '-' && numericValue[1] == '1'))
                //    {
                //        return true;
                //    }
                //}
                //Troy.Cui 2018.07.26使用新的判断
                double value;
                result = double.TryParse(Convert.ToString(expression), System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo, out value);
            }
            return result;
        }

        /*
        /// <summary>是否数字</summary>
        /// <param name="strInput">输入字符串</param>
        /// <returns>true/false</returns>
        public static bool IsNumeric(string inputNumeric)
        {
            if (string.IsNullOrEmpty(inputNumeric))
            {
                return false;
            }
            var reg = new Regex(@"^[-]?\d+[.]?\d*$");
            return reg.IsMatch(inputNumeric);
        }
        */

        public static bool IsDouble(object expression)
        {
            var result = false;
            if (expression != null)
            {
                result = Regex.IsMatch(expression.ToString(), @"^([0-9])[0-9]*(\.\w*)?$");
            }
            return result;
        }

        /// <summary>
        /// 判断给定的字符串数组(strNumber)中的数据是不是都为数值型
        /// </summary>
        /// <param name="numbers">要确认的字符串数组</param>
        /// <returns>是则返加true 不是则返回 false</returns>
        public static bool IsNumericArray(string[] numbers)
        {
            if (numbers == null || numbers.Length < 1)
            {
                return false;
            }
            foreach (var id in numbers)
            {
                if (!IsNumeric(id))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 是不是时间类型数据
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public static bool IsDateTime(string expression)
        {
            var result = false;
            if (!string.IsNullOrWhiteSpace(expression))
            {
                expression = expression.Trim();
                var reg =
                    new Regex(
                        @"(((^((1[8-9]\d{2})|([2-9]\d{3}))([-\/\._])(10|12|0?[13578])([-\/\._])(3[01]|[12][0-9]|0?[1-9]))|(^((1[8-9]\d{2})|([2-9]\d{3}))([-\/\._])(11|0?[469])([-\/\._])(30|[12][0-9]|0?[1-9]))|(^((1[8-9]\d{2})|([2-9]\d{3}))([-\/\._])(0?2)([-\/\._])(2[0-8]|1[0-9]|0?[1-9]))|(^([2468][048]00)([-\/\._])(0?2)([-\/\._])(29))|(^([3579][26]00)([-\/\._])(0?2)([-\/\._])(29))|(^([1][89][0][48])([-\/\._])(0?2)([-\/\._])(29))|(^([2-9][0-9][0][48])([-\/\._])(0?2)([-\/\._])(29))|(^([1][89][2468][048])([-\/\._])(0?2)([-\/\._])(29))|(^([2-9][0-9][2468][048])([-\/\._])(0?2)([-\/\._])(29))|(^([1][89][13579][26])([-\/\._])(0?2)([-\/\._])(29))|(^([2-9][0-9][13579][26])([-\/\._])(0?2)([-\/\._])(29)))((\s+(0?[1-9]|1[012])(:[0-5]\d){0,2}(\s[AP]M))?$|(\s+([01]\d|2[0-3])(:[0-5]\d){0,2})?$))");
                result = reg.IsMatch(expression);

                //加强判断 Troy.Cui 2016-07-02
                if (!result)
                {
                    try
                    {
                        DateTime.Parse(expression);
                        result = true;
                    }
                    catch
                    {
                        result = false;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 检查邮件是否何法 add by wxg 20090531
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public static bool IsEmail(string email)
        {
            var result = false;
            if (!string.IsNullOrWhiteSpace(email))
            {
                email = email.Trim();
                const string regexString =
                    @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
                // const string regexString =
                //    @"^\\w+((-\\w+)|(\\.\\w+))*\\@[A-Za-z0-9]+((\\.|-)[A-Za-z0-9]+)*\\.[A-Za-z0-9]+$";
                var regex = new Regex(regexString);
                result = regex.IsMatch(email);
            }
            return result;
        }

        public static bool CheckEmail(string email)
        {
            var result = true;
            if (email.Trim().Length == 0)
            {
                // 先按可以为空
                result = true;
            }
            else
            {
                var regex = new Regex("[\\w-]+@([\\w-]+\\.)+[\\w-]+");
                if (!regex.IsMatch(email))
                {
                    result = false;
                }
            }
            return result;
        }

        /// <summary>
        /// 检手机号码是否何法
        /// </summary>
        /// <param name="mobile">手机号码</param>
        /// <returns>格式正确</returns>
        public static bool IsMobile(string mobile)
        {
            var result = false;

            // 2015-12-12 吉日嘎拉 手机号码是空的，认为不准确就可以了
            if (!string.IsNullOrEmpty(mobile))
            {
                mobile = mobile.Trim();
                //const string regexString = @"^(1(([34578][0-9])|(47)|[8][01236789]))\d{8}$";
                //根据最新号码段更新 https://www.qqzeng.com/article/phone.html 2021.07.22
                //const string regexString = @"^1([38][0-9]|4[579]|5[0-3,5-9]|6[6]|7[0135678]|9[89])\d{8}$";
                const string regexString = @"^1(3[0-9]|4[56789]|5[0-3,5-9]|6[2567]|7[012345678]|8[0123456789]|9[1389])\d{8}$";
                var regex = new Regex(regexString);
                result = regex.IsMatch(mobile);
            }

            return result;
        }

        /// <summary>
        /// 是否身份证号码
        /// </summary>
        /// <param name="idCard"></param>
        /// <returns></returns>
        public static bool IsIdCard(string idCard)
        {
            idCard = idCard.Trim();
            if (idCard.Length == 15 || idCard.Length == 18)
            {
                return true;
            }
            return false;
            // const string regexString = @"[\d]{6}(19|20)*[\d]{2}((0[1-9])|(11|12))*[\d]{2}((0[1-9])|^2[\d]{1}([0-9])|(30|31))*[\d]{3}[xX]|[\d]{4}";
            // const string regexString = @"^(^[1-9]\d{7}((0\d)|(1[0-2]))(([0|1|2]\d)|3[0-1])\d{3}$)|(^[1-9]\d{5}[1-9]\d{3}((0\d)|(1[0-2]))(([0|1|2]\d)|3[0-1])((\d{4})|\d{3}[Xx])$)$";
            // var regex = new Regex(regexString);
            // return regex.IsMatch(idCard);
        }

        /// <summary>
        /// 是否英文或者数字
        /// </summary>
        /// <param name="userName">文字或者数字</param>
        /// <returns></returns>
        public static bool IsUserName(string userName)
        {
            if (string.IsNullOrEmpty(userName))
            {
                userName = string.Empty;
            }
            const string regexString = @"^[0-9a-zA-Z._@]{3,30}$";
            var regex = new Regex(regexString);
            return regex.IsMatch(userName);
        }

        /// <summary>
        /// 是否英文或者数字
        /// </summary>
        /// <param name="letterOrIsDigit">文字或者数字</param>
        /// <returns></returns>
        public static bool IsLetterOrIsDigit(string letterOrIsDigit)
        {
            if (string.IsNullOrEmpty(letterOrIsDigit))
            {
                letterOrIsDigit = string.Empty;
            }
            const string regexString = @"^[0-9a-zA-Z]{3,30}$";
            var regex = new Regex(regexString);
            return regex.IsMatch(letterOrIsDigit);
        }

        /// <summary>
        /// 是否电话号码
        /// </summary>
        /// <param name="telephone">电话号码</param>
        /// <returns></returns>
        public static bool IsTelephone(string telephone)
        {
            var result = true;
            if (string.IsNullOrEmpty(telephone))
            {
                foreach (var t in telephone)
                {
                    if (t.Equals("-")
                        || t.Equals("0")
                        || t.Equals("1")
                        || t.Equals("2")
                        || t.Equals("3")
                        || t.Equals("4")
                        || t.Equals("5")
                        || t.Equals("6")
                        || t.Equals("7")
                        || t.Equals("8")
                        || t.Equals("9"))
                    {
                        result = true;
                    }
                    else
                    {
                        result = false;
                        break;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 是否汉子姓名
        /// </summary>
        /// <param name="realName">姓名</param>
        /// <returns></returns>
        public static bool IsChineseCharacters(string realName)
        {
            if (string.IsNullOrEmpty(realName))
            {
                return false;
            }
            realName = realName.Trim();
            const string regexString = @"[\u4e00-\u9fa5]{2,}";
            var regex = new Regex(regexString);
            return regex.IsMatch(realName);
        }

        /// <summary>
        /// 文件夹名检查
        /// </summary>
        /// <param name="folderName">文件夹名</param>
        /// <returns>检查通过</returns>
        public static bool CheckFolderName(string folderName)
        {
            var result = true;
            if (folderName.Trim().Length == 0)
            {
                result = false;
            }
            else
            {
                if ((folderName.IndexOfAny(Path.GetInvalidPathChars()) >= 0) || (folderName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0))
                {
                    result = false;
                }
            }
            return result;
        }

        /// <summary>
        /// 密码强度检查
        /// </summary>
        /// <param name="password">密码</param>
        /// <param name="userName"></param>
        /// <returns>检查通过</returns>
        public static bool CheckPasswordStrength(string password, string userName = null)
        {
            var result = true;
            if (!string.IsNullOrEmpty(password))
            {

                if (password.IndexOf("123", StringComparison.OrdinalIgnoreCase) > -1)
                {
                    result = false;
                }

                var isDigit = false;
                var isLetter = false;
                foreach (var t in password)
                {
                    if (!isDigit)
                    {
                        isDigit = char.IsDigit(t);
                    }

                    if (!isLetter)
                    {
                        isLetter = char.IsLetter(t);
                    }
                }

                result = (isDigit && isLetter);
                // 密码至少为8位，为数字加字母
                if (password.Length < 8)
                {
                    result = false;
                }
            }

            return result;
        }

        /// <summary>
        /// 文件名检查
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns>检查通过</returns>
        public static bool CheckFileName(string fileName)
        {
            var result = true;

            if (fileName.Trim().Length == 0)
            {
                result = false;
            }
            else
            {
                if (fileName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
                {
                    result = false;
                }
            }

            return result;
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