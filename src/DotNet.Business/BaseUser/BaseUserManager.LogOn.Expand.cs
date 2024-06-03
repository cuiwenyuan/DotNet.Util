//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2024, DotNet.
//-----------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace DotNet.Business
{
    using Model;
    using Util;

    /// <summary>
    /// BaseUserManager 
    /// 用户管理 扩展类 
    /// 
    /// 修改记录
    /// 
    ///		2015.01.25 版本：3.1 SongBiao 扩展登录提醒功能。
    /// 
    /// <author>
    ///		<name>SongBiao</name>
    ///		<date>2015.01.25</date>
    /// </author> 
    /// </summary>
    public partial class BaseUserManager : BaseManager
    {
        #region public int GetRateUserPass(string pass) 密码强度级别
        /// <summary>
        /// 密码强度级别 小于3不允许登录
        /// </summary>
        /// <param name="pass">密码</param>
        /// <returns>强度级别</returns>
        public int GetRateUserPass(string pass)
        {
            /*  
             * 返回值值表示口令等级  
             * 0 不合法口令  
             * 1 太短  
             * 2 弱  
             * 3 一般  
             * 4 很好  
             * 5 极佳  
             */
            var i = 0;
            //if(pass==null || pass.length()==0)
            if (string.IsNullOrWhiteSpace(pass))
            {
                return 0;
            }
            var hasLetter = MatcherLength(pass, "[a-zA-Z]", "");
            var hasNumber = MatcherLength(pass, "[0-9]", "");
            var passLen = pass.Length;
            if (passLen >= 6)
            {
                /* 如果仅包含数字或仅包含字母 */
                if ((passLen - hasLetter) == 0 || (passLen - hasNumber) == 0)
                {
                    if (passLen < 8)
                    {
                        i = 2;
                    }
                    else
                    {
                        i = 3;
                    }
                }
                /* 如果口令大于6位且即包含数字又包含字母 */
                else if (hasLetter > 0 && hasNumber > 0)
                {
                    if (passLen >= 10)
                    {
                        i = 5;
                    }
                    else if (passLen >= 8)
                    {
                        i = 4;
                    }
                    else
                    {
                        i = 3;
                    }
                }
                /* 如果既不包含数字又不包含字母 */
                else if (hasLetter == 0 && hasNumber == 0)
                {
                    if (passLen >= 7)
                    {
                        i = 5;
                    }
                    else
                    {
                        i = 4;
                    }
                }
                /* 字母或数字有一方为0 */
                else if (hasNumber == 0 || hasLetter == 0)
                {
                    if ((passLen - hasLetter) == 0 || (passLen - hasNumber) == 0)
                    {
                        i = 2;
                    }
                    /*   
                     * 字母数字任意一种类型小于6且总长度大于等于6  
                     * 则说明此密码是字母或数字加任意其他字符组合而成  
                     */
                    else
                    {
                        if (passLen > 8)
                        {
                            i = 5;
                        }
                        else if (passLen == 8)
                        {
                            i = 4;
                        }
                        else
                        {
                            i = 3;
                        }
                    }
                }
            }
            else
            { //口令小于6位则显示太短  
                if (passLen > 0)
                {
                    i = 1; //口令太短  
                }
                else
                {
                    i = 0;
                }
            }
            return i;
        }
        #endregion

        #region public int MatcherLength(string str, string cp, string s) 查询匹配长度
        /// <summary>
        /// 查询匹配长度
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="cp">规则</param>
        /// <param name="s"></param>
        /// <returns></returns>
        public int MatcherLength(string str, string cp, string s)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return 0;
            }
            var mc = Regex.Matches(str, cp);
            return mc.Count;
        }
        #endregion
    }
}
