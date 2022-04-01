using System;
using System.Drawing;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;

#if NET40_OR_GREATER
using System.Web;
using System.Web.UI;
using System.Net;
using System.Web.Hosting;
#elif NETSTANDARD2_0_OR_GREATER
using Microsoft.Extensions.Hosting.Internal;
#endif

namespace DotNet.Util
{
    /// <summary>
    /// 工具类参考DTcms
    /// </summary>
    public partial class Utils
    {
#if NET40_OR_GREATER
        /// <summary>
        /// 得到正则编译参数设置
        /// </summary>
        /// <returns>参数设置</returns>
        public static RegexOptions GetRegexCompiledOptions()
        {
            return RegexOptions.None;
        }

        /// <summary>
        /// 返回字符串真实长度, 1个汉字长度为2
        /// </summary>
        /// <returns>字符长度</returns>
        public static int GetStringLength(string str)
        {
            return Encoding.Default.GetBytes(str).Length;
        }
        /// <summary>
        /// 是否是压缩
        /// </summary>
        /// <param name="str"></param>
        /// <param name="stringarray"></param>
        /// <param name="strsplit"></param>
        /// <returns></returns>
        public static bool IsCompriseStr(string str, string stringarray, string strsplit)
        {
            if (StrIsNullOrEmpty(stringarray))
                return false;

            str = str.ToLower();
            var stringArray = SplitString(stringarray.ToLower(), strsplit);
            for (var i = 0; i < stringArray.Length; i++)
            {
                if (str.IndexOf(stringArray[i]) > -1)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 判断指定字符串在指定字符串数组中的位置
        /// </summary>
        /// <param name="strSearch">字符串</param>
        /// <param name="stringArray">字符串数组</param>
        /// <param name="caseInsensetive">是否不区分大小写, true为不区分, false为区分</param>
        /// <returns>字符串在指定字符串数组中的位置, 如不存在则返回-1</returns>
        public static int GetInArrayId(string strSearch, string[] stringArray, bool caseInsensetive)
        {
            for (var i = 0; i < stringArray.Length; i++)
            {
                if (caseInsensetive)
                {
                    if (strSearch.ToLower() == stringArray[i].ToLower())
                        return i;
                }
                else if (strSearch == stringArray[i])
                    return i;
            }
            return -1;
        }


        /// <summary>
        /// 判断指定字符串在指定字符串数组中的位置
        /// </summary>
        /// <param name="strSearch">字符串</param>
        /// <param name="stringArray">字符串数组</param>
        /// <returns>字符串在指定字符串数组中的位置, 如不存在则返回-1</returns>		
        public static int GetInArrayId(string strSearch, string[] stringArray)
        {
            return GetInArrayId(strSearch, stringArray, true);
        }

        /// <summary>
        /// 判断指定字符串是否属于指定字符串数组中的一个元素
        /// </summary>
        /// <param name="strSearch">字符串</param>
        /// <param name="stringArray">字符串数组</param>
        /// <param name="caseInsensetive">是否不区分大小写, true为不区分, false为区分</param>
        /// <returns>判断结果</returns>
        public static bool InArray(string strSearch, string[] stringArray, bool caseInsensetive)
        {
            return GetInArrayId(strSearch, stringArray, caseInsensetive) >= 0;
        }

        /// <summary>
        /// 判断指定字符串是否属于指定字符串数组中的一个元素
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="stringarray">字符串数组</param>
        /// <returns>判断结果</returns>
        public static bool InArray(string str, string[] stringarray)
        {
            return InArray(str, stringarray, false);
        }

        /// <summary>
        /// 判断指定字符串是否属于指定字符串数组中的一个元素
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="stringarray">内部以逗号分割单词的字符串</param>
        /// <returns>判断结果</returns>
        public static bool InArray(string str, string stringarray)
        {
            return InArray(str, SplitString(stringarray, ","), false);
        }

        /// <summary>
        /// 判断指定字符串是否属于指定字符串数组中的一个元素
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="stringarray">内部以逗号分割单词的字符串</param>
        /// <param name="strsplit">分割字符串</param>
        /// <returns>判断结果</returns>
        public static bool InArray(string str, string stringarray, string strsplit)
        {
            return InArray(str, SplitString(stringarray, strsplit), false);
        }

        /// <summary>
        /// 判断指定字符串是否属于指定字符串数组中的一个元素
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="stringarray">内部以逗号分割单词的字符串</param>
        /// <param name="strsplit">分割字符串</param>
        /// <param name="caseInsensetive">是否不区分大小写, true为不区分, false为区分</param>
        /// <returns>判断结果</returns>
        public static bool InArray(string str, string stringarray, string strsplit, bool caseInsensetive)
        {
            return InArray(str, SplitString(stringarray, strsplit), caseInsensetive);
        }


        /// <summary>
        /// 删除字符串尾部的回车/换行/空格
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string RTrim(string str)
        {
            for (var i = str.Length; i >= 0; i--)
            {
                if (str[i].Equals(" ") || str[i].Equals("\r") || str[i].Equals("\n"))
                {
                    str.Remove(i, 1);
                }
            }
            return str;
        }


        /// <summary>
        /// 清除给定字符串中的回车及换行符
        /// </summary>
        /// <param name="str">要清除的字符串</param>
        /// <returns>清除后返回的字符串</returns>
        public static string ClearBr(string str)
        {
            Match m = null;

            var regexBr = new Regex(@"(\r\n)", RegexOptions.IgnoreCase);

            for (m = regexBr.Match(str); m.Success; m = m.NextMatch())
            {
                str = str.Replace(m.Groups[0].ToString(), "");
            }
            return str;
        }

        /// <summary>
        /// 从字符串的指定位置截取指定长度的子字符串
        /// </summary>
        /// <param name="str">原字符串</param>
        /// <param name="startIndex">子字符串的起始位置</param>
        /// <param name="length">子字符串的长度</param>
        /// <returns>子字符串</returns>
        public static string CutString(string str, int startIndex, int length)
        {
            if (startIndex >= 0)
            {
                if (length < 0)
                {
                    length = length * -1;
                    if (startIndex - length < 0)
                    {
                        length = startIndex;
                        startIndex = 0;
                    }
                    else
                        startIndex = startIndex - length;
                }

                if (startIndex > str.Length)
                    return "";
            }
            else
            {
                if (length < 0)
                    return "";
                else
                {
                    if (length + startIndex > 0)
                    {
                        length = length + startIndex;
                        startIndex = 0;
                    }
                    else
                        return "";
                }
            }

            if (str.Length - startIndex < length)
                length = str.Length - startIndex;

            return str.Substring(startIndex, length);
        }

        /// <summary>
        /// 从字符串的指定位置开始截取到字符串结尾的了符串
        /// </summary>
        /// <param name="str">原字符串</param>
        /// <param name="startIndex">子字符串的起始位置</param>
        /// <returns>子字符串</returns>
        public static string CutString(string str, int startIndex)
        {
            return CutString(str, startIndex, str.Length);
        }

        /// <summary>
        /// 返回文件是否存在
        /// </summary>
        /// <param name="filename">文件名</param>
        /// <returns>是否存在</returns>
        public static bool FileExists(string filename)
        {
            return File.Exists(filename);
        }

        /// <summary>
        /// 判断文件名是否为浏览器可以直接显示的图片文件名
        /// </summary>
        /// <param name="filename">文件名</param>
        /// <returns>是否可以直接显示</returns>
        public static bool IsImgFilename(string filename)
        {
            filename = filename.Trim();
            if (filename.EndsWith(".") || filename.IndexOf(".") == -1)
                return false;

            var extname = filename.Substring(filename.LastIndexOf(".") + 1).ToLower();
            return (extname == "jpg" || extname == "jpeg" || extname == "png" || extname == "bmp" || extname == "gif");
        }


        /// <summary>
        /// int型转换为string型
        /// </summary>
        /// <returns>转换后的string类型结果</returns>
        public static string IntToStr(int intValue)
        {
            return Convert.ToString(intValue);
        }
        /// <summary>
        /// MD5函数
        /// </summary>
        /// <param name="str">原始字符串</param>
        /// <returns>MD5结果</returns>
        public static string Md5(string str)
        {
            var b = Encoding.UTF8.GetBytes(str);
            b = new MD5CryptoServiceProvider().ComputeHash(b);
            var ret = "";
            for (var i = 0; i < b.Length; i++)
                ret += b[i].ToString("x").PadLeft(2, '0');

            return ret;
        }

        /// <summary>
        /// SHA256函数
        /// </summary>
        /// /// <param name="str">原始字符串</param>
        /// <returns>SHA256结果</returns>
        public static string SHA256(string str)
        {
            var sha256Data = Encoding.UTF8.GetBytes(str);
            var sha256 = new SHA256Managed();
            var result = sha256.ComputeHash(sha256Data);
            return Convert.ToBase64String(result);  //返回长度为44字节的字符串
        }

        /// <summary>
        /// 字符串如果操过指定长度则将超出的部分用指定字符串代替
        /// </summary>
        /// <param name="pSrcString">要检查的字符串</param>
        /// <param name="pLength">指定长度</param>
        /// <param name="pTailString">用于替换的字符串</param>
        /// <returns>截取后的字符串</returns>
        public static string GetSubString(string pSrcString, int pLength, string pTailString)
        {
            return GetSubString(pSrcString, 0, pLength, pTailString);
        }
        /// <summary>
        /// 获取Unicode字符
        /// </summary>
        /// <param name="str"></param>
        /// <param name="len"></param>
        /// <param name="pTailString"></param>
        /// <returns></returns>
        public static string GetUnicodeSubString(string str, int len, string pTailString)
        {
            str = str.TrimEnd();
            var result = string.Empty;// 最终返回的结果
            var byteLen = Encoding.Default.GetByteCount(str);// 单字节字符长度
            var charLen = str.Length;// 把字符平等对待时的字符串长度
            var byteCount = 0;// 记录读取进度
            var pos = 0;// 记录截取位置
            if (byteLen > len)
            {
                for (var i = 0; i < charLen; i++)
                {
                    if (Convert.ToInt32(str.ToCharArray()[i]) > 255)// 按中文字符计算加2
                        byteCount += 2;
                    else// 按英文字符计算加1
                        byteCount += 1;
                    if (byteCount > len)// 超出时只记下上一个有效位置
                    {
                        pos = i;
                        break;
                    }
                    else if (byteCount == len)// 记下当前位置
                    {
                        pos = i + 1;
                        break;
                    }
                }

                if (pos >= 0)
                    result = str.Substring(0, pos) + pTailString;
            }
            else
                result = str;

            return result;
        }

        /// <summary>
        /// 取指定长度的字符串
        /// </summary>
        /// <param name="pSrcString">要检查的字符串</param>
        /// <param name="pStartIndex">起始位置</param>
        /// <param name="pLength">指定长度</param>
        /// <param name="pTailString">用于替换的字符串</param>
        /// <returns>截取后的字符串</returns>
        public static string GetSubString(string pSrcString, int pStartIndex, int pLength, string pTailString)
        {
            var myResult = pSrcString;

            var bComments = Encoding.UTF8.GetBytes(pSrcString);
            foreach (var c in Encoding.UTF8.GetChars(bComments))
            {    //当是日文或韩文时(注:中文的范围:\u4e00 - \u9fa5, 日文在\u0800 - \u4e00, 韩文为\xAC00-\xD7A3)
                if ((c > '\u0800' && c < '\u4e00') || (c > '\xAC00' && c < '\xD7A3'))
                {
                    //if (System.Text.RegularExpressions.Regex.IsMatch(p_SrcString, "[\u0800-\u4e00]+") || System.Text.RegularExpressions.Regex.IsMatch(p_SrcString, "[\xAC00-\xD7A3]+"))
                    //当截取的起始位置超出字段串长度时
                    if (pStartIndex >= pSrcString.Length)
                        return "";
                    else
                        return pSrcString.Substring(pStartIndex,
                                                       ((pLength + pStartIndex) > pSrcString.Length) ? (pSrcString.Length - pStartIndex) : pLength);
                }
            }

            if (pLength >= 0)
            {
                var bsSrcString = Encoding.Default.GetBytes(pSrcString);

                //当字符串长度大于起始位置
                if (bsSrcString.Length > pStartIndex)
                {
                    var pEndIndex = bsSrcString.Length;

                    //当要截取的长度在字符串的有效长度范围内
                    if (bsSrcString.Length > (pStartIndex + pLength))
                    {
                        pEndIndex = pLength + pStartIndex;
                    }
                    else
                    {   //当不在有效范围内时,只取到字符串的结尾

                        pLength = bsSrcString.Length - pStartIndex;
                        pTailString = "";
                    }

                    var nRealLength = pLength;
                    var anResultFlag = new int[pLength];
                    byte[] bsResult = null;

                    var nFlag = 0;
                    for (var i = pStartIndex; i < pEndIndex; i++)
                    {
                        if (bsSrcString[i] > 127)
                        {
                            nFlag++;
                            if (nFlag == 3)
                                nFlag = 1;
                        }
                        else
                            nFlag = 0;

                        anResultFlag[i] = nFlag;
                    }

                    if ((bsSrcString[pEndIndex - 1] > 127) && (anResultFlag[pLength - 1] == 1))
                        nRealLength = pLength + 1;

                    bsResult = new byte[nRealLength];

                    Array.Copy(bsSrcString, pStartIndex, bsResult, 0, nRealLength);

                    myResult = Encoding.Default.GetString(bsResult);
                    myResult = myResult + pTailString;
                }
            }

            return myResult;
        }

        /// <summary>
        /// 自定义的替换字符串函数
        /// </summary>
        public static string ReplaceString(string sourceString, string searchString, string replaceString, bool isCaseInsensetive)
        {
            return Regex.Replace(sourceString, Regex.Escape(searchString), replaceString, isCaseInsensetive ? RegexOptions.IgnoreCase : RegexOptions.None);
        }

        /// <summary>
        /// 生成指定数量的html空格符号
        /// </summary>
        public static string GetSpacesString(int spacesCount)
        {
            var sb = Pool.StringBuilder.Get();
            for (var i = 0; i < spacesCount; i++)
            {
                sb.Append(" &nbsp;&nbsp;");
            }
            return sb.Put();
        }

        /// <summary>
        /// 检测是否符合email格式
        /// </summary>
        /// <param name="strEmail">要判断的email字符串</param>
        /// <returns>判断结果</returns>
        public static bool IsValidEmail(string strEmail)
        {
            return Regex.IsMatch(strEmail, @"^[\w\.]+([-]\w+)*@[A-Za-z0-9-_]+[\.][A-Za-z0-9-_]");
        }
        /// <summary>
        /// 检测是否符合email格式
        /// </summary>
        /// <param name="strEmail"></param>
        /// <returns></returns>
        public static bool IsValidDoEmail(string strEmail)
        {
            return Regex.IsMatch(strEmail, @"^@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
        }

        /// <summary>
        /// 检测是否是正确的Url
        /// </summary>
        /// <param name="strUrl">要验证的Url</param>
        /// <returns>判断结果</returns>
        public static bool IsUrl(string strUrl)
        {
            return Regex.IsMatch(strUrl, @"^(http|https)\://([a-zA-Z0-9\.\-]+(\:[a-zA-Z0-9\.&%\$\-]+)*@)*((25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9])\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[0-9])|localhost|([a-zA-Z0-9\-]+\.)*[a-zA-Z0-9\-]+\.(com|edu|gov|int|mil|net|org|biz|arpa|info|name|pro|aero|coop|museum|[a-zA-Z]{1,10}))(\:[0-9]+)*(/($|[a-zA-Z0-9\.\,\?\'\\\+&%\$#\=~_\-]+))*$");
        }
        /// <summary>
        /// GetEmailHostName
        /// </summary>
        /// <param name="strEmail"></param>
        /// <returns></returns>
        public static string GetEmailHostName(string strEmail)
        {
            if (strEmail.IndexOf("@") < 0)
            {
                return "";
            }
            return strEmail.Substring(strEmail.LastIndexOf("@")).ToLower();
        }

        /// <summary>
        /// 判断是否为base64字符串
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsBase64String(string str)
        {
            //A-Z, a-z, 0-9, +, /, =
            return Regex.IsMatch(str, @"[A-Za-z0-9\+\/\=]");
        }
        /// <summary>
        /// 检测是否有Sql危险字符
        /// </summary>
        /// <param name="str">要判断字符串</param>
        /// <returns>判断结果</returns>
        public static bool IsSafeSqlString(string str)
        {
            return !Regex.IsMatch(str, @"[-|;|,|\/|\(|\)|\[|\]|\}|\{|%|@|\*|!|\']");
        }

        /// <summary>
        /// 检测是否有危险的可能用于链接的字符串
        /// </summary>
        /// <param name="str">要判断字符串</param>
        /// <returns>判断结果</returns>
        public static bool IsSafeUserInfoString(string str)
        {
            return !Regex.IsMatch(str, @"^\s*$|^c:\\con\\con$|[%,\*" + "\"" + @"\s\t\<\>\&]|游客|^Guest");
        }

        /// <summary>
        /// 清理字符串
        /// </summary>
        public static string CleanInput(string strIn)
        {
            return Regex.Replace(strIn.Trim(), @"[^\w\.@-]", "");
        }

        /// <summary>
        /// 返回URL中结尾的文件名
        /// </summary>		
        public static string GetFilename(string url)
        {
            if (url == null)
            {
                return "";
            }
            var strs1 = url.Split(new char[] { '/' });
            return strs1[strs1.Length - 1].Split(new char[] { '?' })[0];
        }

        /// <summary>
        /// 根据阿拉伯数字返回月份的名称(可更改为某种语言)
        /// </summary>	
        public static string[] Monthes
        {
            get
            {
                return new string[] { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };
            }
        }

        /// <summary>
        /// 替换回车换行符为html换行符
        /// </summary>
        public static string StrFormat(string str)
        {
            string str2;

            if (str == null)
            {
                str2 = "";
            }
            else
            {
                str = str.Replace("\r\n", "<br />");
                str = str.Replace("\n", "<br />");
                str2 = str;
            }
            return str2;
        }

        /// <summary>
        /// 返回标准日期格式string
        /// </summary>
        public static string GetDate()
        {
            return DateTime.Now.ToString("yyyy-MM-dd");
        }

        /// <summary>
        /// 返回指定日期格式
        /// </summary>
        public static string GetDate(string datetimestr, string replacestr)
        {
            if (datetimestr == null)
                return replacestr;

            if (datetimestr.Equals(""))
                return replacestr;

            try
            {
                datetimestr = Convert.ToDateTime(datetimestr).ToString("yyyy-MM-dd").Replace("1900-01-01", replacestr);
            }
            catch
            {
                return replacestr;
            }
            return datetimestr;
        }


        /// <summary>
        /// 返回标准时间格式string
        /// </summary>
        public static string GetTime()
        {
            return DateTime.Now.ToString("HH:mm:ss");
        }

        /// <summary>
        /// 返回标准时间格式string
        /// </summary>
        public static string GetDateTime()
        {
            return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }

        /// <summary>
        /// 返回相对于当前时间的相对天数
        /// </summary>
        public static string GetDateTime(int relativeday)
        {
            return DateTime.Now.AddDays(relativeday).ToString("yyyy-MM-dd HH:mm:ss");
        }

        /// <summary>
        /// 返回标准时间格式string
        /// </summary>
        public static string GetDateTimeF()
        {
            return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fffffff");
        }

        /// <summary>
        /// 返回标准时间
        /// </summary>
        /// <param name="fDateTime"></param>
        /// <param name="formatStr"></param>
        /// <returns></returns>
        public static string GetStandardDateTime(string fDateTime, string formatStr)
        {
            if (fDateTime == "0000-0-0 0:00:00")
                return fDateTime;
            var time = new DateTime(1900, 1, 1, 0, 0, 0, 0);
            if (DateTime.TryParse(fDateTime, out time))
                return time.ToString(formatStr);
            else
                return "N/A";
        }

        /// <summary>
        /// 返回标准时间 yyyy-MM-dd HH:mm:ss
        /// </summary>
        /// <param name="fDateTime"></param>
        /// <returns></returns>
        public static string GetStandardDateTime(string fDateTime)
        {
            return GetStandardDateTime(fDateTime, "yyyy-MM-dd HH:mm:ss");
        }

        /// <summary>
        /// 返回标准时间 yyyy-MM-dd
        /// </summary>
        /// <param name="fDate"></param>
        /// <returns></returns>
        public static string GetStandardDate(string fDate)
        {
            return GetStandardDateTime(fDate, "yyyy-MM-dd");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static bool IsTime(string timeval)
        {
            return Regex.IsMatch(timeval, @"^((([0-1]?[0-9])|(2[0-3])):([0-5]?[0-9])(:[0-5]?[0-9])?)$");
        }

        /// <summary>
        /// 是否在白名单
        /// </summary>
        /// <param name="ip">Ip地址</param>
        /// <returns></returns>
        public static bool IsInWhiteList(string ip = null)
        {
            var result = false;
            if (string.IsNullOrEmpty(ip))
            {
                ip = GetIp();
            }
            if (!string.IsNullOrEmpty(ip) && !string.IsNullOrEmpty(BaseSystemInfo.WhiteList))
            {
                var whiteLists = BaseSystemInfo.WhiteList.Split(',');
                for (var i = 0; i < whiteLists.Length; i++)
                {
                    if (whiteLists[i].Equals(ip))
                    {
                        result = true;
                        break;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 改正sql语句中的转义字符
        /// </summary>
        public static string MashSql(string str)
        {
            return (str == null) ? "" : str.Replace("\'", "'");
        }

        /// <summary>
        /// 替换sql语句中的有问题符号
        /// </summary>
        public static string ChkSql(string str)
        {
            return (str == null) ? "" : str.Replace("'", "''");
        }


        /// <summary>
        /// 转换为静态html
        /// </summary>
        public void TransHtml(string path, string outpath)
        {
            var page = new Page();
            var writer = new StringWriter();
            page.Server.Execute(path, writer);
            FileStream fs;
            if (File.Exists(page.Server.MapPath("") + "\\" + outpath))
            {
                File.Delete(page.Server.MapPath("") + "\\" + outpath);
                fs = File.Create(page.Server.MapPath("") + "\\" + outpath);
            }
            else
            {
                fs = File.Create(page.Server.MapPath("") + "\\" + outpath);
            }
            var bt = Encoding.Default.GetBytes(writer.ToString());
            fs.Write(bt, 0, bt.Length);
            fs.Close();
        }

        /// <summary>
        /// 进行指定的替换(脏字过滤)
        /// </summary>
        public static string StrFilter(string str, string bantext)
        {
            string text1 = "", text2 = "";
            var textArray1 = SplitString(bantext, "\r\n");
            for (var num1 = 0; num1 < textArray1.Length; num1++)
            {
                text1 = textArray1[num1].Substring(0, textArray1[num1].IndexOf("="));
                text2 = textArray1[num1].Substring(textArray1[num1].IndexOf("=") + 1);
                str = str.Replace(text1, text2);
            }
            return str;
        }

        /// <summary>
        /// 获得伪静态页码显示链接
        /// </summary>
        /// <param name="curPage">当前页数</param>
        /// <param name="countPage">总页数</param>
        /// <param name="url">超级链接地址</param>
        /// <param name="expname"></param>
        /// <param name="extendPage">周边页码显示个数上限</param>
        /// <returns>页码html</returns>
        public static string GetStaticPageNumbers(int curPage, int countPage, string url, string expname, int extendPage)
        {
            return GetStaticPageNumbers(curPage, countPage, url, expname, extendPage, 0);
        }


        /// <summary>
        /// 获得伪静态页码显示链接
        /// </summary>
        /// <param name="curPage">当前页数</param>
        /// <param name="countPage">总页数</param>
        /// <param name="url">超级链接地址</param>
        /// <param name="expname"></param>
        /// <param name="extendPage">周边页码显示个数上限</param>
        /// <param name="forumrewrite">当前版块是否使用URL重写</param>
        /// <returns>页码html</returns>
        public static string GetStaticPageNumbers(int curPage, int countPage, string url, string expname, int extendPage, int forumrewrite)
        {
            var startPage = 1;
            var endPage = 1;

            var t1 = "<a href=\"" + url + "-1" + expname + "\">&laquo;</a>";
            var t2 = "<a href=\"" + url + "-" + countPage + expname + "\">&raquo;</a>";

            if (forumrewrite == 1)
            {
                t1 = "<a href=\"" + url + "/1/list" + expname + "\">&laquo;</a>";
                t2 = "<a href=\"" + url + "/" + countPage + "/list" + expname + "\">&raquo;</a>";
            }

            if (forumrewrite == 2)
            {
                t1 = "<a href=\"" + url + "/\">&laquo;</a>";
                t2 = "<a href=\"" + url + "/" + countPage + "/\">&raquo;</a>";
            }

            if (countPage < 1) countPage = 1;
            if (extendPage < 3) extendPage = 2;

            if (countPage > extendPage)
            {
                if (curPage - (extendPage / 2) > 0)
                {
                    if (curPage + (extendPage / 2) < countPage)
                    {
                        startPage = curPage - (extendPage / 2);
                        endPage = startPage + extendPage - 1;
                    }
                    else
                    {
                        endPage = countPage;
                        startPage = endPage - extendPage + 1;
                        t2 = "";
                    }
                }
                else
                {
                    endPage = extendPage;
                    t1 = "";
                }
            }
            else
            {
                startPage = 1;
                endPage = countPage;
                t1 = "";
                t2 = "";
            }

            var s = Pool.StringBuilder.Get();

            s.Append(t1);
            for (var i = startPage; i <= endPage; i++)
            {
                if (i == curPage)
                {
                    s.Append("<span>");
                    s.Append(i);
                    s.Append("</span>");
                }
                else
                {
                    s.Append("<a href=\"");
                    if (forumrewrite == 1)
                    {
                        s.Append(url);
                        if (i != 1)
                        {
                            s.Append("/");
                            s.Append(i);
                        }
                        s.Append("/list");
                        s.Append(expname);
                    }
                    else if (forumrewrite == 2)
                    {
                        s.Append(url);
                        s.Append("/");
                        if (i != 1)
                        {
                            s.Append(i);
                            s.Append("/");
                        }
                    }
                    else
                    {
                        s.Append(url);
                        if (i != 1)
                        {
                            s.Append("-");
                            s.Append(i);
                        }
                        s.Append(expname);
                    }
                    s.Append("\">");
                    s.Append(i);
                    s.Append("</a>");
                }
            }
            s.Append(t2);

            return s.Put();
        }


        /// <summary>
        /// 获得帖子的伪静态页码显示链接
        /// </summary>
        /// <param name="expname"></param>
        /// <param name="countPage">总页数</param>
        /// <param name="url">超级链接地址</param>
        /// <param name="extendPage">周边页码显示个数上限</param>
        /// <returns>页码html</returns>
        public static string GetPostPageNumbers(int countPage, string url, string expname, int extendPage)
        {
            var startPage = 1;
            var endPage = 1;
            var curPage = 1;

            var t1 = "<a href=\"" + url + "-1" + expname + "\">&laquo;</a>";
            var t2 = "<a href=\"" + url + "-" + countPage + expname + "\">&raquo;</a>";

            if (countPage < 1) countPage = 1;
            if (extendPage < 3) extendPage = 2;

            if (countPage > extendPage)
            {
                if (curPage - (extendPage / 2) > 0)
                {
                    if (curPage + (extendPage / 2) < countPage)
                    {
                        startPage = curPage - (extendPage / 2);
                        endPage = startPage + extendPage - 1;
                    }
                    else
                    {
                        endPage = countPage;
                        startPage = endPage - extendPage + 1;
                        t2 = "";
                    }
                }
                else
                {
                    endPage = extendPage;
                    t1 = "";
                }
            }
            else
            {
                startPage = 1;
                endPage = countPage;
                t1 = "";
                t2 = "";
            }

            var s = Pool.StringBuilder.Get();

            s.Append(t1);
            for (var i = startPage; i <= endPage; i++)
            {
                s.Append("<a href=\"");
                s.Append(url);
                s.Append("-");
                s.Append(i);
                s.Append(expname);
                s.Append("\">");
                s.Append(i);
                s.Append("</a>");
            }
            s.Append(t2);

            return s.Put();
        }



        /// <summary>
        /// 获得页码显示链接
        /// </summary>
        /// <param name="curPage">当前页数</param>
        /// <param name="countPage">总页数</param>
        /// <param name="url">超级链接地址</param>
        /// <param name="extendPage">周边页码显示个数上限</param>
        /// <returns>页码html</returns>
        public static string GetPageNumbers(int curPage, int countPage, string url, int extendPage)
        {
            return GetPageNumbers(curPage, countPage, url, extendPage, "page");
        }

        /// <summary>
        /// 获得页码显示链接
        /// </summary>
        /// <param name="curPage">当前页数</param>
        /// <param name="countPage">总页数</param>
        /// <param name="url">超级链接地址</param>
        /// <param name="extendPage">周边页码显示个数上限</param>
        /// <param name="pagetag">页码标记</param>
        /// <returns>页码html</returns>
        public static string GetPageNumbers(int curPage, int countPage, string url, int extendPage, string pagetag)
        {
            return GetPageNumbers(curPage, countPage, url, extendPage, pagetag, null);
        }

        /// <summary>
        /// 获得页码显示链接
        /// </summary>
        /// <param name="curPage">当前页数</param>
        /// <param name="countPage">总页数</param>
        /// <param name="url">超级链接地址</param>
        /// <param name="extendPage">周边页码显示个数上限</param>
        /// <param name="pagetag">页码标记</param>
        /// <param name="anchor">锚点</param>
        /// <returns>页码html</returns>
        public static string GetPageNumbers(int curPage, int countPage, string url, int extendPage, string pagetag, string anchor)
        {
            if (pagetag == "")
                pagetag = "page";
            var startPage = 1;
            var endPage = 1;

            if (url.IndexOf("?") > 0)
                url = url + "&";
            else
                url = url + "?";

            var t1 = "<a href=\"" + url + "&" + pagetag + "=1";
            var t2 = "<a href=\"" + url + "&" + pagetag + "=" + countPage;
            if (anchor != null)
            {
                t1 += anchor;
                t2 += anchor;
            }
            t1 += "\">&laquo;</a>";
            t2 += "\">&raquo;</a>";

            if (countPage < 1)
                countPage = 1;
            if (extendPage < 3)
                extendPage = 2;

            if (countPage > extendPage)
            {
                if (curPage - (extendPage / 2) > 0)
                {
                    if (curPage + (extendPage / 2) < countPage)
                    {
                        startPage = curPage - (extendPage / 2);
                        endPage = startPage + extendPage - 1;
                    }
                    else
                    {
                        endPage = countPage;
                        startPage = endPage - extendPage + 1;
                        t2 = "";
                    }
                }
                else
                {
                    endPage = extendPage;
                    t1 = "";
                }
            }
            else
            {
                startPage = 1;
                endPage = countPage;
                t1 = "";
                t2 = "";
            }

            var s = Pool.StringBuilder.Get();

            s.Append(t1);
            for (var i = startPage; i <= endPage; i++)
            {
                if (i == curPage)
                {
                    s.Append("<span>");
                    s.Append(i);
                    s.Append("</span>");
                }
                else
                {
                    s.Append("<a href=\"");
                    s.Append(url);
                    s.Append(pagetag);
                    s.Append("=");
                    s.Append(i);
                    if (anchor != null)
                    {
                        s.Append(anchor);
                    }
                    s.Append("\">");
                    s.Append(i);
                    s.Append("</a>");
                }
            }
            s.Append(t2);

            return s.Put();
        }

        /// <summary>
        /// 返回 HTML 字符串的编码结果
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns>编码结果</returns>
        public static string HtmlEncode(string str)
        {
            return HttpUtility.HtmlEncode(str);
        }

        /// <summary>
        /// 返回 HTML 字符串的解码结果
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns>解码结果</returns>
        public static string HtmlDecode(string str)
        {
            return HttpUtility.HtmlDecode(str);
        }

        /// <summary>
        /// 返回 URL 字符串的编码结果
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns>编码结果</returns>
        public static string UrlEncode(string str)
        {
            return HttpUtility.UrlEncode(str);
        }

        /// <summary>
        /// 返回 URL 字符串的编码结果
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns>解码结果</returns>
        public static string UrlDecode(string str)
        {
            return HttpUtility.UrlDecode(str);
        }


        /// <summary>
        /// 返回指定目录下的非 UTF8 字符集文件
        /// </summary>
        /// <param name="path">路径</param>
        /// <returns>文件名的字符串数组</returns>
        public static string[] FindNoUtf8File(string path)
        {
            var sb = Pool.StringBuilder.Get();
            var folder = new DirectoryInfo(path);
            var subFiles = folder.GetFiles();

            for (var j = 0; j < subFiles.Length; j++)
            {
                if (subFiles[j].Extension.ToLower().Equals(".htm"))
                {
                    var fs = new FileStream(subFiles[j].FullName, FileMode.Open, FileAccess.Read);
                    var bUtf8 = IsUtf8(fs);
                    fs.Close();
                    if (!bUtf8)
                    {
                        sb.Append(subFiles[j].FullName);
                        sb.Append("\r\n");
                    }
                }
            }
            return SplitString(sb.Put(), "\r\n");

        }

        //0000 0000-0000 007F - 0xxxxxxx  (ascii converts to 1 octet!)
        //0000 0080-0000 07FF - 110xxxxx 10xxxxxx    ( 2 octet format)
        //0000 0800-0000 FFFF - 1110xxxx 10xxxxxx 10xxxxxx (3 octet format)

        /// <summary>
        /// 判断文件流是否为UTF8字符集
        /// </summary>
        /// <param name="sbInputStream">文件流</param>
        /// <returns>判断结果</returns>
        private static bool IsUtf8(FileStream sbInputStream)
        {
            int i;
            byte cOctets;  // octets to go in this UTF-8 encoded character 
            byte chr;
            var bAllAscii = true;
            var iLen = sbInputStream.Length;

            cOctets = 0;
            for (i = 0; i < iLen; i++)
            {
                chr = (byte)sbInputStream.ReadByte();

                if ((chr & 0x80) != 0) bAllAscii = false;

                if (cOctets == 0)
                {
                    if (chr >= 0x80)
                    {
                        do
                        {
                            chr <<= 1;
                            cOctets++;
                        }
                        while ((chr & 0x80) != 0);

                        cOctets--;
                        if (cOctets == 0)
                            return false;
                    }
                }
                else
                {
                    if ((chr & 0xC0) != 0x80)
                        return false;

                    cOctets--;
                }
            }

            if (cOctets > 0)
                return false;

            if (bAllAscii)
                return false;

            return true;
        }

        /// <summary>
        /// 格式化字节数字符串
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string FormatBytesStr(int bytes)
        {
            if (bytes > 1073741824)
                return ((double)(bytes / 1073741824)).ToString("0") + "G";

            if (bytes > 1048576)
                return ((double)(bytes / 1048576)).ToString("0") + "M";

            if (bytes > 1024)
                return ((double)(bytes / 1024)).ToString("0") + "K";

            return bytes + "Bytes";
        }


        /// <summary>
        /// 建立文件夹
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool CreateDir(string name)
        {
            return MakeSureDirectoryPathExists(name);
        }

        /// <summary>
        /// 为脚本替换特殊字符串
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ReplaceStrToScript(string str)
        {
            return str.Replace("\\", "\\\\").Replace("'", "\\'").Replace("\"", "\\\"");
        }

        /// <summary>
        /// 是否为ip
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static bool IsIp(string ip)
        {
            return Regex.IsMatch(ip, @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$");
        }
        /// <summary>
        /// 是否为IP地址
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static bool IsIpSect(string ip)
        {
            return Regex.IsMatch(ip, @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){2}((2[0-4]\d|25[0-5]|[01]?\d\d?|\*)\.)(2[0-4]\d|25[0-5]|[01]?\d\d?|\*)$");
        }


        /// <summary>
        /// 返回指定IP是否在指定的IP数组所限定的范围内, IP数组内的IP地址可以使用*表示该IP段任意, 例如192.168.1.*
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="iparray"></param>
        /// <returns></returns>
        public static bool InIpArray(string ip, string[] iparray)
        {
            var userip = SplitString(ip, @".");

            for (var ipIndex = 0; ipIndex < iparray.Length; ipIndex++)
            {
                var tmpip = SplitString(iparray[ipIndex], @".");
                var r = 0;
                for (var i = 0; i < tmpip.Length; i++)
                {
                    if (tmpip[i] == "*")
                        return true;

                    if (userip.Length > i)
                    {
                        if (tmpip[i] == userip[i])
                            r++;
                        else
                            break;
                    }
                    else
                        break;
                }

                if (r == 4)
                    return true;
            }
            return false;
        }


        /// <summary>
        /// 创建目录
        /// </summary>
        /// <param name="name">名称</param>
        /// <returns>创建是否成功</returns>
        [DllImport("dbgHelp", SetLastError = true)]
        private static extern bool MakeSureDirectoryPathExists(string name);

        /// <summary>
        /// 得到论坛的真实路径
        /// </summary>
        /// <returns></returns>
        public static string GetTrueForumPath()
        {
            var forumPath = HttpContext.Current.Request.Path;
            if (forumPath.LastIndexOf("/") != forumPath.IndexOf("/"))
                forumPath = forumPath.Substring(forumPath.IndexOf("/"), forumPath.LastIndexOf("/") + 1);
            else
                forumPath = "/";

            return forumPath;
        }

        /// <summary>
        /// 判断字符串是否是yy-mm-dd字符串
        /// </summary>
        /// <param name="str">待判断字符串</param>
        /// <returns>判断结果</returns>
        public static bool IsDateString(string str)
        {
            return Regex.IsMatch(str, @"(\d{4})-(\d{1,2})-(\d{1,2})");
        }

        /// <summary>
        /// 移除Html标记
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string RemoveHtml(string content)
        {
            return Regex.Replace(content, @"<[^>]*>", string.Empty, RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// 过滤HTML中的不安全标签
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string RemoveUnsafeHtml(string content)
        {
            content = Regex.Replace(content, @"(\<|\s+)o([a-z]+\s?=)", "$1$2", RegexOptions.IgnoreCase);
            content = Regex.Replace(content, @"(script|frame|form|meta|behavior|style)([\s|:|>])+", "$1.$2", RegexOptions.IgnoreCase);
            return content;
        }

        /// <summary>
        /// 从HTML中获取文本,保留br,p,img
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static string GetTextFromHtml(string html)
        {
            var regEx = new Regex(@"</?(?!br|/?p|img)[^>]*>", RegexOptions.IgnoreCase);

            return regEx.Replace(html, "");
        }

        /// <summary>
        /// object型转换为bool型
        /// </summary>
        /// <param name="expression">要转换的字符串</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>转换后的bool类型结果</returns>
        public static bool StrToBool(object expression, bool defValue)
        {
            return DataTypeConverter.StrToBool(expression, defValue);
        }

        /// <summary>
        /// string型转换为bool型
        /// </summary>
        /// <param name="expression">要转换的字符串</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>转换后的bool类型结果</returns>
        public static bool StrToBool(string expression, bool defValue)
        {
            return DataTypeConverter.StrToBool(expression, defValue);
        }

        /// <summary>
        /// 将对象转换为Int32类型
        /// </summary>
        /// <param name="expression">要转换的字符串</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>转换后的int类型结果</returns>
        public static int StrToInt(object expression, int defValue)
        {
            return DataTypeConverter.ObjectToInt(expression, defValue);
        }

        /// <summary>
        /// 将字符串转换为Int32类型
        /// </summary>
        /// <param name="expression">要转换的字符串</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>转换后的int类型结果</returns>
        public static int StrToInt(string expression, int defValue)
        {
            return DataTypeConverter.StrToInt(expression, defValue);
        }

        /// <summary>
        /// Object型转换为float型
        /// </summary>
        /// <param name="strValue">要转换的字符串</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>转换后的int类型结果</returns>
        public static float StrToFloat(object strValue, float defValue)
        {
            return DataTypeConverter.StrToFloat(strValue, defValue);
        }

        /// <summary>
        /// string型转换为float型
        /// </summary>
        /// <param name="strValue">要转换的字符串</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>转换后的int类型结果</returns>
        public static float StrToFloat(string strValue, float defValue)
        {
            return DataTypeConverter.StrToFloat(strValue, defValue);
        }

        /// <summary>
        /// Object型转换为decimal型
        /// </summary>
        /// <param name="strValue">要转换的字符串</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>转换后的int类型结果</returns>
        public static decimal StrToDecimal(object strValue, decimal defValue)
        {
            return DataTypeConverter.StrToDecimal(strValue, defValue);
        }

        /// <summary>
        /// string型转换为decimal型
        /// </summary>
        /// <param name="strValue">要转换的字符串</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>转换后的int类型结果</returns>
        public static decimal StrToDecimal(string strValue, decimal defValue)
        {
            return DataTypeConverter.StrToDecimal(strValue, defValue);
        }

        /// <summary>
        /// 验证是否为正整数
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsInt(string str)
        {
            return Regex.IsMatch(str, @"^[0-9]*$");
        }
        /// <summary>
        /// 是否符合
        /// </summary>
        /// <param name="newHash"></param>
        /// <param name="ruleType"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool IsRuleTip(Hashtable newHash, string ruleType, out string key)
        {
            key = "";
            foreach (DictionaryEntry str in newHash)
            {

                try
                {
                    var single = SplitString(str.Value.ToString(), "\r\n");

                    foreach (var strs in single)
                    {
                        if (strs != "")


                            switch (ruleType.Trim().ToLower())
                            {
                                case "email":
                                    if (IsValidDoEmail(strs) == false)
                                        throw new Exception();
                                    break;

                                case "ip":
                                    if (IsIpSect(strs) == false)
                                        throw new Exception();
                                    break;

                                case "timesect":
                                    var splitetime = strs.Split('-');
                                    if (IsTime(splitetime[1]) == false || IsTime(splitetime[0]) == false)
                                        throw new Exception();
                                    break;
                            }
                    }
                }
                catch
                {
                    key = str.Key.ToString();
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 删除最后一个字符
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ClearLastChar(string str)
        {
            return (str == "") ? "" : str.Substring(0, str.Length - 1);
        }

        /// <summary>
        /// 备份文件
        /// </summary>
        /// <param name="sourceFileName">源文件名</param>
        /// <param name="destFileName">目标文件名</param>
        /// <param name="overwrite">当目标文件存在时是否覆盖</param>
        /// <returns>操作是否成功</returns>
        public static bool BackupFile(string sourceFileName, string destFileName, bool overwrite)
        {
            if (!File.Exists(sourceFileName))
                throw new FileNotFoundException(sourceFileName + "文件不存在！");

            if (!overwrite && File.Exists(destFileName))
                return false;

            try
            {
                File.Copy(sourceFileName, destFileName, true);
                return true;
            }
            catch (Exception e)
            {
                throw e;
            }
        }


        /// <summary>
        /// 备份文件,当目标文件存在时覆盖
        /// </summary>
        /// <param name="sourceFileName">源文件名</param>
        /// <param name="destFileName">目标文件名</param>
        /// <returns>操作是否成功</returns>
        public static bool BackupFile(string sourceFileName, string destFileName)
        {
            return BackupFile(sourceFileName, destFileName, true);
        }


        /// <summary>
        /// 恢复文件
        /// </summary>
        /// <param name="backupFileName">备份文件名</param>
        /// <param name="targetFileName">要恢复的文件名</param>
        /// <param name="backupTargetFileName">要恢复文件再次备份的名称,如果为null,则不再备份恢复文件</param>
        /// <returns>操作是否成功</returns>
        public static bool RestoreFile(string backupFileName, string targetFileName, string backupTargetFileName)
        {
            try
            {
                if (!File.Exists(backupFileName))
                    throw new FileNotFoundException(backupFileName + "文件不存在！");

                if (backupTargetFileName != null)
                {
                    if (!File.Exists(targetFileName))
                        throw new FileNotFoundException(targetFileName + "文件不存在！无法备份此文件！");
                    else
                        File.Copy(targetFileName, backupTargetFileName, true);
                }
                File.Delete(targetFileName);
                File.Copy(backupFileName, targetFileName);
            }
            catch (Exception e)
            {
                throw e;
            }
            return true;
        }
        /// <summary>
        /// 恢复文件
        /// </summary>
        /// <param name="backupFileName"></param>
        /// <param name="targetFileName"></param>
        /// <returns></returns>
        public static bool RestoreFile(string backupFileName, string targetFileName)
        {
            return RestoreFile(backupFileName, targetFileName, null);
        }


        /// <summary>
        /// 将全角数字转换为数字
        /// </summary>
        /// <param name="sbcCase"></param>
        /// <returns></returns>
        public static string SbcCaseToNumberic(string sbcCase)
        {
            var c = sbcCase.ToCharArray();
            for (var i = 0; i < c.Length; i++)
            {
                var b = Encoding.Unicode.GetBytes(c, i, 1);
                if (b.Length == 2)
                {
                    if (b[1] == 255)
                    {
                        b[0] = (byte)(b[0] + 32);
                        b[1] = 0;
                        c[i] = Encoding.Unicode.GetChars(b)[0];
                    }
                }
            }
            return new string(c);
        }

        /// <summary>
        /// 将字符串转换为Color
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static Color ToColor(string color)
        {
            int red, green, blue = 0;
            char[] rgb;
            color = color.TrimStart('#');
            color = Regex.Replace(color.ToLower(), "[g-zG-Z]", "");
            switch (color.Length)
            {
                case 3:
                    rgb = color.ToCharArray();
                    red = Convert.ToInt32(rgb[0] + rgb[0].ToString(), 16);
                    green = Convert.ToInt32(rgb[1] + rgb[1].ToString(), 16);
                    blue = Convert.ToInt32(rgb[2] + rgb[2].ToString(), 16);
                    return Color.FromArgb(red, green, blue);
                case 6:
                    rgb = color.ToCharArray();
                    red = Convert.ToInt32(rgb[0] + rgb[1].ToString(), 16);
                    green = Convert.ToInt32(rgb[2] + rgb[3].ToString(), 16);
                    blue = Convert.ToInt32(rgb[4] + rgb[5].ToString(), 16);
                    return Color.FromArgb(red, green, blue);
                default:
                    return Color.FromName(color);

            }
        }

        /// <summary>
        /// 转换长文件名为短文件名
        /// </summary>
        /// <param name="fullName"></param>
        /// <param name="repString"></param>
        /// <param name="leftNum"></param>
        /// <param name="rightNum"></param>
        /// <param name="charNum"></param>
        /// <returns></returns>
        public static string ConvertSimpleFileName(string fullName, string repString, int leftNum, int rightNum, int charNum)
        {
            string simplefilename = "", leftstring = "", rightstring = "", filename = "";
            var extname = GetFileExtName(fullName);

            if (StrIsNullOrEmpty(extname))
                return fullName;

            int filelength = 0, dotindex = 0;

            dotindex = fullName.LastIndexOf('.');
            filename = fullName.Substring(0, dotindex);
            filelength = filename.Length;
            if (dotindex > charNum)
            {
                leftstring = filename.Substring(0, leftNum);
                rightstring = filename.Substring(filelength - rightNum, rightNum);
                if (repString == "" || repString == null)
                    simplefilename = leftstring + rightstring + "." + extname;
                else
                    simplefilename = leftstring + repString + rightstring + "." + extname;
            }
            else
                simplefilename = fullName;

            return simplefilename;
        }


        /// <summary>
        /// 检查颜色值是否为3/6位的合法颜色
        /// </summary>
        /// <param name="color">待检查的颜色</param>
        /// <returns></returns>
        public static bool CheckColorValue(string color)
        {
            if (StrIsNullOrEmpty(color))
                return false;

            color = color.Trim().Trim('#');

            if (color.Length != 3 && color.Length != 6)
                return false;

            //不包含0-9  a-f以外的字符
            if (!Regex.IsMatch(color, "[^0-9a-f]", RegexOptions.IgnoreCase))
                return true;

            return false;
        }

        /// <summary>
        /// 获取ajax形式的分页链接
        /// </summary>
        /// <param name="curPage">当前页数</param>
        /// <param name="countPage">总页数</param>
        /// <param name="callback">回调函数</param>
        /// <param name="extendPage">周边页码显示个数上限</param>
        /// <returns></returns>
        public static string GetAjaxPageNumbers(int curPage, int countPage, string callback, int extendPage)
        {
            var pagetag = "page";
            var startPage = 1;
            var endPage = 1;

            var t1 = "<a href=\"###\" onclick=\"" + string.Format(callback, "&" + pagetag + "=1");
            var t2 = "<a href=\"###\" onclick=\"" + string.Format(callback, "&" + pagetag + "=" + countPage);

            t1 += "\">&laquo;</a>";
            t2 += "\">&raquo;</a>";

            if (countPage < 1)
                countPage = 1;
            if (extendPage < 3)
                extendPage = 2;

            if (countPage > extendPage)
            {
                if (curPage - (extendPage / 2) > 0)
                {
                    if (curPage + (extendPage / 2) < countPage)
                    {
                        startPage = curPage - (extendPage / 2);
                        endPage = startPage + extendPage - 1;
                    }
                    else
                    {
                        endPage = countPage;
                        startPage = endPage - extendPage + 1;
                        t2 = "";
                    }
                }
                else
                {
                    endPage = extendPage;
                    t1 = "";
                }
            }
            else
            {
                startPage = 1;
                endPage = countPage;
                t1 = "";
                t2 = "";
            }

            var s = Pool.StringBuilder.Get();

            s.Append(t1);
            for (var i = startPage; i <= endPage; i++)
            {
                if (i == curPage)
                {
                    s.Append("<span>");
                    s.Append(i);
                    s.Append("</span>");
                }
                else
                {
                    s.Append("<a href=\"###\" onclick=\"");
                    s.Append(string.Format(callback, pagetag + "=" + i));
                    s.Append("\">");
                    s.Append(i);
                    s.Append("</a>");
                }
            }
            s.Append(t2);

            return s.Put();
        }

        /// <summary>
        /// 根据Url获得源文件内容
        /// </summary>
        /// <param name="url">合法的Url地址</param>
        /// <returns></returns>
        public static string GetSourceTextByUrl(string url)
        {
            try
            {
                var request = WebRequest.Create(url);
                request.Timeout = 20000;//20秒超时
                var response = request.GetResponse();

                var resStream = response.GetResponseStream();
                var sr = new StreamReader(resStream);
                return sr.ReadToEnd();
            }
            catch { return ""; }
        }

        /// <summary>
        /// 转换时间为unix时间戳
        /// </summary>
        /// <param name="utcDatetime">需要传递UTC时间,例:DataTime.UTCNow</param>
        /// <returns></returns>
        public static double ConvertToUnixTimestamp(DateTime utcDatetime)
        {
            var origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            var diff = utcDatetime - origin;
            return Math.Floor(diff.TotalSeconds);
        }


        /// <summary>
        /// 合并字符
        /// </summary>
        /// <param name="source">要合并的源字符串</param>
        /// <param name="target">要被合并到的目的字符串</param>
        /// <returns>合并到的目的字符串</returns>
        public static string MergeString(string source, string target)
        {
            return MergeString(source, target, ",");
        }

        /// <summary>
        /// 合并字符
        /// </summary>
        /// <param name="source">要合并的源字符串</param>
        /// <param name="target">要被合并到的目的字符串</param>
        /// <param name="mergeChar">合并符</param>
        /// <returns>并到字符串</returns>
        public static string MergeString(string source, string target, string mergeChar)
        {
            if (StrIsNullOrEmpty(target))
                target = source;
            else
                target += mergeChar + source;

            return target;
        }


        /// <summary>
        /// 清除UBB标签
        /// </summary>
        /// <param name="sDetail">帖子内容</param>
        /// <returns>帖子内容</returns>
        public static string ClearUbb(string sDetail)
        {
            return Regex.Replace(sDetail, @"\[[^\]]*?\]", string.Empty, RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// 获取站点根目录URL
        /// </summary>
        /// <returns></returns>
        public static string GetRootUrl(string forumPath)
        {
            var port = HttpContext.Current.Request.Url.Port;
            return string.Format("{0}://{1}{2}{3}",
                                 HttpContext.Current.Request.Url.Scheme,
                                 HttpContext.Current.Request.Url.Host,
                                 (port == 80 || port == 0) ? "" : ":" + port,
                                 forumPath);
        }


        /// <summary>
        /// 获取指定文件的扩展名
        /// </summary>
        /// <param name="fileName">指定文件名</param>
        /// <returns>扩展名</returns>
        public static string GetFileExtName(string fileName)
        {
            if (StrIsNullOrEmpty(fileName) || fileName.IndexOf('.') <= 0)
                return "";

            fileName = fileName.ToLower().Trim();

            return fileName.Substring(fileName.LastIndexOf('.'), fileName.Length - fileName.LastIndexOf('.'));
        }

        /// <summary>
        /// 根据字符串获取枚举值
        /// </summary>
        /// <typeparam name="T">枚举类型</typeparam>
        /// <param name="value">字符串枚举值</param>
        /// <param name="defValue">缺省值</param>
        /// <returns></returns>
        public static T GetEnum<T>(string value, T defValue)
        {
            try
            {
                return (T)Enum.Parse(typeof(T), value, true);
            }
            catch (ArgumentException)
            {
                return defValue;
            }
        }


        /// <summary>
        /// 将8位日期型整型数据转换为日期字符串数据
        /// </summary>
        /// <param name="date">整型日期</param>
        /// <param name="chnType">是否以中文年月日输出</param>
        /// <returns></returns>
        public static string FormatDate(int date, bool chnType)
        {
            var dateStr = date.ToString();

            if (date <= 0 || dateStr.Length != 8)
                return dateStr;

            if (chnType)
                return dateStr.Substring(0, 4) + "年" + dateStr.Substring(4, 2) + "月" + dateStr.Substring(6) + "日";
            return dateStr.Substring(0, 4) + "-" + dateStr.Substring(4, 2) + "-" + dateStr.Substring(6);
        }

        /// <summary>
        /// hack tip:通过更新web.config文件方式来重启IIS进程池（注：iis中web园数量须大于1,且为非虚拟主机用户才可调用该方法）
        /// </summary>
        public static void RestartIisProcess()
        {
            try
            {
                var xmldoc = new System.Xml.XmlDocument();
                xmldoc.Load(GetMapPath("~/web.config"));
                var writer = new System.Xml.XmlTextWriter(GetMapPath("~/web.config"), null);
                writer.Formatting = System.Xml.Formatting.Indented;
                xmldoc.WriteTo(writer);
                writer.Flush();
                writer.Close();
            }
            catch
            {; }
        }

        /// <summary>
        /// 判断当前客户端请求是否为IE
        /// </summary>
        /// <returns></returns>
        public static bool IsIe()
        {
            return HttpContext.Current.Request.ServerVariables["HTTP_USER_AGENT"].IndexOf("MSIE", StringComparison.Ordinal) >= 0;
        }

        #region 参考DTcms

        #region 生成日期随机码
        /// <summary>
        /// 生成日期随机码
        /// </summary>
        /// <returns></returns>
        public static string GetRamCode()
        {
        #region
            return DateTime.Now.ToString("yyyyMMddHHmmssffff");
        #endregion
        }
        #endregion

        #region 返回文件扩展名，不含“.”
        /// <summary>
        /// 返回文件扩展名，不含“.”
        /// </summary>
        /// <param name="filepath">文件全名称</param>
        /// <returns>string</returns>
        public static string GetFileExt(string filepath)
        {
            if (string.IsNullOrEmpty(filepath))
            {
                return "";
            }
            if (filepath.LastIndexOf(".") > 0)
            {
                return filepath.Substring(filepath.LastIndexOf(".") + 1); //文件扩展名，不含“.”
            }
            return "";
        }
        #endregion

        #region 删除上传文件
        /// <summary>
        /// 删除上传的文件(及缩略图)
        /// </summary>
        /// <param name="filepath"></param>
        public static void DeleteUpFile(string filepath)
        {
            if (string.IsNullOrEmpty(filepath))
            {
                return;
            }
            var fullpath = GetMapPath(filepath); //原图
            if (File.Exists(fullpath))
            {
                File.Delete(fullpath);
            }
            if (filepath.LastIndexOf("/") >= 0)
            {
                var thumbnailpath = filepath.Substring(0, filepath.LastIndexOf("/")) + "mall_" + filepath.Substring(filepath.LastIndexOf("/") + 1);
                var fullTpath = GetMapPath(thumbnailpath); //宿略图
                if (File.Exists(fullTpath))
                {
                    File.Delete(fullTpath);
                }
            }
        }
        #endregion

        #region 清空指定的文件夹，但不删除文件夹

        /// <summary>
        /// 清空指定的文件夹，但不删除文件夹
        /// </summary>
        /// <param name="dir">目录</param>
        /// <param name="fileExtension">后缀</param>
        /// <param name="days">天</param>
        /// <param name="hours">小时</param>
        public static void EmptyFolder(string dir, string fileExtension = null, int days = 0, int hours = 0)
        {
            var dt = DateTime.Now;
            if (!string.IsNullOrEmpty(fileExtension))
            {
                fileExtension = "*." + fileExtension.Replace(".", "");
            }
            else
            {
                fileExtension = "*.*";
            }

            try
            {
                var directories = Directory.GetFileSystemEntries(dir, fileExtension, SearchOption.AllDirectories);

                foreach (var d in directories)
                {
                    if (File.Exists(d))
                    {
                        var file = new FileInfo(d);
                        if (file.Attributes.ToString().IndexOf("ReadOnly", StringComparison.OrdinalIgnoreCase) != -1)
                        {
                            file.Attributes = FileAttributes.Normal;
                        }

                        //fi.CreationTime;//创建时间
                        var ts = dt.Subtract(file.LastWriteTime);
                        //如果超过指定天数、小时则删除
                        if ((hours == 0 && days == 0) || (days == 0 && hours > 0 && ts.TotalHours > hours) ||
                            (days > 0 && hours == 0 && ts.TotalDays > days))
                        {
                            if (file.Exists)
                            {
                                file.Delete();
                            }
                        }
                    }
                    else
                    {
                        var directoryInfo = new DirectoryInfo(d);

                        if (Directory.Exists(directoryInfo.FullName))
                        {
                            //如果当前文件夹下面没有任何子文件夹或者文件，就把子目录也删除了
                            if (directoryInfo.GetFiles().Length == 0)
                            {
                                //递归删除
                                directoryInfo.Delete(true);
                            }
                            else
                            {
                                //递归删除子文件夹下的文件
                                EmptyFolder(directoryInfo.FullName, fileExtension, days, hours);
                            }

                        }
                    }
                }
            }
            catch (ArgumentNullException)
            {
                LogUtil.WriteLog("Path is a null reference.", "Exception");
            }
            catch (System.Security.SecurityException)
            {
                LogUtil.WriteLog("The caller does not have the required permission.", "Exception");
            }
            catch (ArgumentException)
            {
                LogUtil.WriteLog("Path is an empty string, contains only white spaces, or contains invalid characters.",
                    "Exception");
            }
            catch (DirectoryNotFoundException)
            {
                LogUtil.WriteLog("The path encapsulated in the Directory object does not exist.", "Exception");
            }
            catch (Exception ex)
            {
                LogUtil.WriteException(ex);
            }
        }
        #endregion

        #endregion

        #region 清除HTML标记
        /// <summary>
        /// 清除HTML标记
        /// </summary>
        /// <param name="htmlstring"></param>
        /// <returns></returns>
        public static string DropHtml(string htmlstring)
        {
            if (string.IsNullOrEmpty(htmlstring)) return "";
            //删除脚本  
            htmlstring = Regex.Replace(htmlstring, @"<script[^>]*?>.*?</script>", "", RegexOptions.IgnoreCase);
            //删除HTML  
            htmlstring = Regex.Replace(htmlstring, @"<(.[^>]*)>", "", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"([\r\n])[\s]+", "", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"-->", "", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"<!--.*", "", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"&(quot|#34);", "\"", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"&(amp|#38);", "&", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"&(lt|#60);", "<", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"&(gt|#62);", ">", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"&(nbsp|#160);", " ", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"&(iexcl|#161);", "\xa1", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"&(cent|#162);", "\xa2", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"&(pound|#163);", "\xa3", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"&(copy|#169);", "\xa9", RegexOptions.IgnoreCase);

            htmlstring = Regex.Replace(htmlstring, @"&#(\d+);", "", RegexOptions.IgnoreCase);
            htmlstring.Replace("<", "");
            htmlstring.Replace(">", "");
            htmlstring.Replace("\r\n", "");
            htmlstring.Replace("&emsp;", "");
            htmlstring = HttpContext.Current.Server.HtmlEncode(htmlstring).Trim();
            return htmlstring;
        }
        #endregion

        #region DropHtml
        /// <summary>
        /// 清除HTML标记且返回相应的长度
        /// </summary>
        /// <param name="htmlstring"></param>
        /// <param name="strLen"></param>
        /// <returns></returns>
        public static string DropHtml(string htmlstring, int strLen)
        {
            return CutString(DropHtml(htmlstring), strLen);
        }
        #endregion

        #region TXT代码转换成HTML格式
        /// <summary>
        /// 字符串字符处理
        /// </summary>
        /// <param name="input">等待处理的字符串</param>
        /// <returns>处理后的字符串</returns>
        /// //把TXT代码转换成HTML格式
        public static String ToHtml(string input)
        {
            var sb = Pool.StringBuilder.Get();
            sb.Append(input);
            sb.Replace("'", "&apos;");
            sb.Replace("&", "&amp;");
            sb.Replace("<", "&lt;");
            sb.Replace(">", "&gt;");
            sb.Replace("\r\n", "<br />");
            sb.Replace("\n", "<br />");
            sb.Replace("\t", " ");
            //sb.Replace(" ", "&nbsp;");
            return sb.Put();
        }
        #endregion

        #region HTML代码转换成TXT格式
        /// <summary>
        /// 字符串字符处理
        /// </summary>
        /// <param name="input">等待处理的字符串</param>
        /// <returns>处理后的字符串</returns>
        /// //把HTML代码转换成TXT格式
        public static String ToTxt(String input)
        {
            var sb = Pool.StringBuilder.Get();
            sb.Append(input);
            sb.Replace("&nbsp;", " ");
            sb.Replace("<br>", "\r\n");
            sb.Replace("<br>", "\n");
            sb.Replace("<br />", "\n");
            sb.Replace("<br />", "\r\n");
            sb.Replace("&lt;", "<");
            sb.Replace("&gt;", ">");
            sb.Replace("&amp;", "&");
            return sb.Put();
        }
        #endregion

        #region URL请求数据
        /// <summary>
        /// HTTP POST方式请求数据
        /// </summary>
        /// <param name="url">URL.</param>
        /// <param name="param">POST的数据</param>
        /// <returns></returns>
        public static string HttpPost(string url, string param)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.Accept = "*/*";
            request.Timeout = 15000;
            request.AllowAutoRedirect = false;

            StreamWriter requestStream = null;
            WebResponse response = null;
            string responseStr = null;

            try
            {
                requestStream = new StreamWriter(request.GetRequestStream());
                requestStream.Write(param);
                requestStream.Close();

                response = request.GetResponse();
                if (response != null)
                {
                    var reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                    responseStr = reader.ReadToEnd();
                    reader.Close();
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                request = null;
                requestStream = null;
                response = null;
            }

            return responseStr;
        }

        /// <summary>
        /// HTTP GET方式请求数据.
        /// </summary>
        /// <param name="url">URL.</param>
        /// <returns></returns>
        public static string HttpGet(string url)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            //request.ContentType = "application/x-www-form-urlencoded";
            request.Accept = "*/*";
            request.Timeout = 15000;
            request.AllowAutoRedirect = false;

            WebResponse response = null;
            string responseStr = null;

            try
            {
                response = request.GetResponse();

                if (response != null)
                {
                    var reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                    responseStr = reader.ReadToEnd();
                    reader.Close();
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                request = null;
                response = null;
            }

            return responseStr;
        }

        /// <summary>
        /// 执行URL获取页面内容
        /// </summary>
        public static string UrlExecute(string urlPath)
        {
            if (string.IsNullOrEmpty(urlPath))
            {
                return "error";
            }
            var sw = new StringWriter();
            try
            {
                HttpContext.Current.Server.Execute(urlPath, sw);
                return sw.ToString();
            }
            catch (Exception)
            {
                return "error";
            }
            finally
            {
                sw.Close();
                sw.Dispose();
            }
        }
        #endregion
#endif

        /// <summary>
        /// 推荐使用的获取IP方式
        /// </summary>
        /// <returns></returns>
        public static string GetIp()
        {
            var result = string.Empty;
#if NET40_OR_GREATER
            //优先使用GetRealIp
            result = RequestUtil.GetRealIp();
            //其次使用GetIp
            if (string.IsNullOrEmpty(result))
            {
                result = RequestUtil.GetIp();
            }
#endif
            //最后使用MachineInfo
            if (string.IsNullOrEmpty(result))
            {
                result = MachineInfo.GetIpAddress();
            }
            return result;
        }

        /// <summary>
        /// 清除字符串数组中的重复项
        /// </summary>
        /// <param name="strArray">字符串数组</param>
        /// <param name="maxElementLength">字符串数组中单个元素的最大长度</param>
        /// <returns></returns>
        public static string[] DistinctStringArray(string[] strArray, int maxElementLength)
        {
            var h = new Hashtable();

            foreach (var s in strArray)
            {
                var k = s;
                if (maxElementLength > 0 && k.Length > maxElementLength)
                {
                    k = k.Substring(0, maxElementLength);
                }
                h[k.Trim()] = s;
            }

            var result = new string[h.Count];

            h.Keys.CopyTo(result, 0);

            return result;
        }

        /// <summary>
        /// 清除字符串数组中的重复项
        /// </summary>
        /// <param name="strArray">字符串数组</param>
        /// <returns></returns>
        public static string[] DistinctStringArray(string[] strArray)
        {
            return DistinctStringArray(strArray, 0);
        }

        /// <summary>
        /// 替换html字符
        /// </summary>
        public static string EncodeHtml(string strHtml)
        {
            if (strHtml != "")
            {
                strHtml = strHtml.Replace(",", "&def");
                strHtml = strHtml.Replace("'", "&dot");
                strHtml = strHtml.Replace(";", "&dec");
                return strHtml;
            }
            return "";
        }
        /// <summary>
        /// 分割字符串
        /// </summary>
        /// <param name="strContent">被分割的字符串</param>
        /// <param name="strSplit">分割符</param>
        /// <param name="ignoreRepeatItem">忽略重复项</param>
        /// <param name="maxElementLength">单个元素最大长度</param>
        /// <returns></returns>
        public static string[] SplitString(string strContent, string strSplit, bool ignoreRepeatItem, int maxElementLength)
        {
            var result = SplitString(strContent, strSplit);

            return ignoreRepeatItem ? DistinctStringArray(result, maxElementLength) : result;
        }
        /// <summary>
        /// 分割字符
        /// </summary>
        /// <param name="strContent"></param>
        /// <param name="strSplit"></param>
        /// <param name="ignoreRepeatItem"></param>
        /// <param name="minElementLength"></param>
        /// <param name="maxElementLength"></param>
        /// <returns></returns>
        public static string[] SplitString(string strContent, string strSplit, bool ignoreRepeatItem, int minElementLength, int maxElementLength)
        {
            var result = SplitString(strContent, strSplit);

            if (ignoreRepeatItem)
            {
                result = DistinctStringArray(result);
            }
            return PadStringArray(result, minElementLength, maxElementLength);
        }

        /// <summary>
        /// 分割字符串
        /// </summary>
        /// <param name="strContent">被分割的字符串</param>
        /// <param name="strSplit">分割符</param>
        /// <param name="ignoreRepeatItem">忽略重复项</param>
        /// <returns></returns>
        public static string[] SplitString(string strContent, string strSplit, bool ignoreRepeatItem)
        {
            return SplitString(strContent, strSplit, ignoreRepeatItem, 0);
        }

        /// <summary>
        /// 分割字符串
        /// </summary>
        public static string[] SplitString(string strContent, string strSplit)
        {
            if (!StrIsNullOrEmpty(strContent))
            {
                if (strContent.IndexOf(strSplit) < 0)
                    return new string[] { strContent };

                return Regex.Split(strContent, Regex.Escape(strSplit), RegexOptions.IgnoreCase);
            }
            else
                return new string[0] { };
        }

        /// <summary>
        /// 分割字符串
        /// </summary>
        /// <returns></returns>
        public static string[] SplitString(string strContent, string strSplit, int count)
        {
            var result = new string[count];
            var splited = SplitString(strContent, strSplit);

            for (var i = 0; i < count; i++)
            {
                if (i < splited.Length)
                    result[i] = splited[i];
                else
                    result[i] = string.Empty;
            }

            return result;
        }

        /// <summary>
        /// 过滤字符串数组中每个元素为合适的大小
        /// 当长度小于minLength时，忽略掉,-1为不限制最小长度
        /// 当长度大于maxLength时，取其前maxLength位
        /// 如果数组中有null元素，会被忽略掉
        /// </summary>
        /// <param name="strArray"></param>
        /// <param name="minLength">单个元素最小长度</param>
        /// <param name="maxLength">单个元素最大长度</param>
        /// <returns></returns>
        public static string[] PadStringArray(string[] strArray, int minLength, int maxLength)
        {
            if (minLength > maxLength)
            {
                var t = maxLength;
                maxLength = minLength;
                minLength = t;
            }

            var iMiniStringCount = 0;
            for (var i = 0; i < strArray.Length; i++)
            {
                if (minLength > -1 && strArray[i].Length < minLength)
                {
                    strArray[i] = null;
                    continue;
                }
                if (strArray[i].Length > maxLength)
                    strArray[i] = strArray[i].Substring(0, maxLength);

                iMiniStringCount++;
            }

            var result = new string[iMiniStringCount];
            for (int i = 0, j = 0; i < strArray.Length && j < result.Length; i++)
            {
                if (strArray[i] != null && strArray[i] != string.Empty)
                {
                    result[j] = strArray[i];
                    j++;
                }
            }
            return result;
        }

        /// <summary>
        /// 字段串是否为NULL或为""(空)
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool StrIsNullOrEmpty(string str)
        {
            if (str == null || str.Trim() == string.Empty)
                return true;

            return false;
        }

        /// <summary>
        /// 获得当前绝对路径
        /// </summary>
        /// <param name="virtualPath">指定的路径</param>
        /// <returns>绝对路径</returns>
        public static string GetMapPath(string virtualPath)
        {
#if NET40_OR_GREATER
            //HttpContext.Current并非无处不在
            if (HttpContext.Current != null)
            {
                return HttpContext.Current.Server.MapPath(virtualPath);
            }
            else
            {
                //非web程序引用
                virtualPath = virtualPath.Replace("/", "\\");
                if (virtualPath.StartsWith("~"))
                {
                    return HostingEnvironment.MapPath(virtualPath);
                }
                else
                {
                    return HostingEnvironment.MapPath("~/" + virtualPath);
                }
            }
#elif NETSTANDARD2_0_OR_GREATER || NET6_0_OR_GREATER
            //非web程序引用
            virtualPath = virtualPath.Replace("/", "\\");
            if (virtualPath.StartsWith("~"))
            {
                //HostingEnvironment.MapPath(@"~/Views/Emails");
                return Path.GetFullPath(virtualPath);
            }
            else
            {
                return Path.GetFullPath("~/" + virtualPath);
            }
#endif
        }
    }
}
