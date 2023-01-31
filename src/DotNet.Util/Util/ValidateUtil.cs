//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2023, DotNet.
//-----------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace DotNet.Util
{
    /// <summary>
    /// 验证工具
    /// </summary>
    public static class ValidateUtil
    {
        /// <summary>
        /// 正整数
        /// </summary>
        public static string PositiveInteger = @"^[0-9]*[1-9][0-9]*$";// 正整数正则表达式
        /// <summary>
        /// 整数
        /// </summary>
        public static string Integer = @"^-?\d+$";// 整数正则表达式
        /// <summary>
        /// 浮点数
        /// </summary>
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
        /// <summary>
        /// 是否为Boolean
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
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
        /// <summary>
        /// 是否为Double
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
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
        /// <summary>
        /// 检查邮件
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
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

        #region UPC-A
        /// <summary>
        /// 是否为UPC-A编码
        /// </summary>
        /// <param name="code">UPC-A code</param>
        /// <returns></returns>
        public static bool IsUPCA(string code)
        {
            var result = false;
            if (!string.IsNullOrEmpty(code))
            {
                var checkDigit = 0;
                var isDigitsOnly = IsDigitsOnly(code);
                if (code.Length == 11 && isDigitsOnly)
                {
                    // Add Fake CheckSum
                    checkDigit = CalculateUPCACheckDigit(code);
                    code += checkDigit.ToString();
                }
                else if (code.Length == 12)
                {
                    checkDigit = code[11];
                }
                if (code.Length == 12 && isDigitsOnly)
                {
                    //60984399883
                    //Check digit calculation is based on modulus 10 with digits in an odd
                    //position (from right to left) being weighted 1 and even position digits
                    //being weighted 3. 
                    //Implementation based on http://stackoverflow.com/questions/10143547/how-do-i-validate-a-upc-or-ean-code
                    result = checkDigit == CalculateUPCACheckDigit(code);
                }
            }
            return result;
        }

        /// <summary>
        /// Calculate UPCA Check Digit
        /// </summary>
        /// <param name="code">This should exclude the check digit</param>
        /// <returns></returns>
        private static int CalculateUPCACheckDigit(string code)
        {
            //5 - step algorithm for check digit calculation:
            //Let's assume that we are using the fictitious code 05432122345.
            //Add all of the digits in the odd positions(digits in position 1, 3, 5, 7, 9, and 11)
            //0 + 4 + 2 + 2 + 3 + 5 = 16
            //Multiply by 3.
            //16 * 3 = 48
            //Add all of the digits in even positions(digits in position 2, 4, 6, 8 and 10).
            //5 + 3 + 1 + 2 + 4 = 15
            //Sum the results of steps 3 and 2.
            //48 + 15 = 63
            //Determine what number needs to be added to the result of step 4 in order to create a multiple of 10.
            //63 + 7 = 70
            //The check digit therefore equals 7.
            var check = 0;
            var sum = 0;
            if (code.Length >= 11)
            {
                code = code.Cut(11);
                for (var i = 0; i < code.Length; i++)
                {
                    if (i % 2 == 0)
                    {
                        sum += int.Parse(code.Substring(code.Length - 1 - i, 1)) * 3;
                    }
                    else
                    {
                        sum += int.Parse(code.Substring(code.Length - 1 - i, 1));
                    }

                }
                check = (10 - (sum % 10)) % 10;
            }
            return check;
        }

        private static bool IsDigitsOnly(string str)
        {
            foreach (var c in str)
            {
                if (c < '0' || c > '9')
                    return false;
            }

            return true;
        }
        #endregion

        #region VIN

        /// <summary>
        /// 是否为合法的VIN
        /// </summary>
        /// <param name="vin"></param>
        /// <returns></returns>
        public static bool IsVIN(string vin)
        {
            var result = false;
            var upperVin = vin.ToUpper();
            //排除字母I、O、Q
            if (!string.IsNullOrEmpty(vin) && vin.Length == 17 && !(upperVin.IndexOf("I", StringComparison.OrdinalIgnoreCase) >= 0 || upperVin.IndexOf("O", StringComparison.OrdinalIgnoreCase) >= 0 || upperVin.IndexOf("Q", StringComparison.OrdinalIgnoreCase) >= 0))
            {
                // VIN码从第1位到第17位的“加权值”：
                var vinMapWeighting = new Dictionary<int, int>
                    {
                        { 1, 8 },
                        { 2, 7 },
                        { 3, 6 },
                        { 4, 5 },
                        { 5, 4 },
                        { 6, 3 },
                        { 7, 2 },
                        { 8, 10 },
                        { 9, 0 },
                        { 10, 9 },
                        { 11, 8 },
                        { 12, 7 },
                        { 13, 6 },
                        { 14, 5 },
                        { 15, 4 },
                        { 16, 3 },
                        { 17, 2 }
                    };
                // VIN码各位数字的“对应值”
                var vinMapValue = new Dictionary<string, int>
                    {
                        { "0", 0 },
                        { "1", 1 },
                        { "2", 2 },
                        { "3", 3 },
                        { "4", 4 },
                        { "5", 5 },
                        { "6", 6 },
                        { "7", 7 },
                        { "8", 8 },
                        { "9", 9 },
                        { "A", 1 },
                        { "B", 2 },
                        { "C", 3 },
                        { "D", 4 },
                        { "E", 5 },
                        { "F", 6 },
                        { "G", 7 },
                        { "H", 8 },
                        //{ "I", 9 },
                        { "J", 1 },
                        { "K", 2 },
                        { "L", 3 },
                        { "M", 4 },
                        { "N", 5 },
                        //{ "O", 6 },
                        { "P", 7 },
                        //{ "Q", 8 },
                        { "R", 9 },
                        { "S", 2 },
                        { "T", 3 },
                        { "U", 4 },
                        { "V", 5 },
                        { "W", 6 },
                        { "X", 7 },
                        { "Y", 8 },
                        { "Z", 9 }
                    };

                var len = upperVin.Length;
                var vinArr = new string[len];
                for (var i = 0; i < len; i++)
                {
                    vinArr[i] = upperVin.Substring(i, 1);
                }
                var amount = 0;
                for (var i = 0; i < vinArr.Length; i++)
                {
                    //VIN码从从第一位开始，码数字的对应值×该位的加权值，计算全部17位的乘积值相加
                    if (vinMapValue.ContainsKey(vinArr[i].ToUpper()))
                        amount += vinMapValue[vinArr[i].ToUpper()] * vinMapWeighting[i + 1];
                }
                //乘积值相加除以11、若余数为10，即为字母Ｘ
                if (amount % 11 == 10)
                {
                    if (vinArr[8].ToUpper() == "X")
                    {
                        result = true;
                    }

                }
                else
                {
                    //VIN码从从第一位开始，码数字的对应值×该位的加权值，计算全部17位的乘积值相加除以11，所得的余数，即为第九位校验值
                    if (vinMapValue.ContainsKey(vinArr[8].ToUpper()))
                    {
                        if (amount % 11 == vinMapValue[vinArr[8].ToUpper()])
                        {
                            result = true;
                        }
                    }
                }

            }
            return result;
        }

        #endregion

        #region IsPlateNumber
        /// <summary>
        /// 是否为车牌号
        /// </summary>
        /// <param name="plateNumber">车牌号</param>
        /// <returns></returns>
        public static bool IsPlateNumber(string plateNumber)
        {
            var result = false;
            if (!string.IsNullOrEmpty(plateNumber) && plateNumber.Length == 7)
            {
                const string pattern = @"^[京津沪渝冀豫云辽黑湘皖鲁新苏浙赣鄂桂甘晋蒙陕吉闽贵粤青藏川宁琼使领A-Z]{1}[A-Z]{1}[A-Z0-9]{4}[A-Z0-9挂学警港澳]{1}$";
                result = Regex.IsMatch(plateNumber, pattern);
            }
            return result;
        }

        #endregion

    }
}