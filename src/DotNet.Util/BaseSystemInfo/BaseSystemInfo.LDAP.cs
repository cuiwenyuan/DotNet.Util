//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2024, DotNet.
//-----------------------------------------------------------------

using System;
using System.Configuration;

namespace DotNet.Util
{
    /// <summary>
    /// BaseSystemInfo
    /// 这是系统的核心基础信息部分
    /// 
    /// 修改记录
    ///		2017.11.23 版本：1.0 Troy Cui	主键创建。
    ///		
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2017.11.23</date>
    /// </author>
    /// </summary>
    public partial class BaseSystemInfo
    {

        private static string _ldapPath = string.Empty;
        /// <summary>
        /// LDAP路径
        /// </summary>
        public static string LdapPath
        {
            get
            {
                if (ConfigurationManager.AppSettings["LDAPPath"] != null)
                {
                    _ldapPath = ConfigurationManager.AppSettings["LDAPPath"];
                }
                if (string.IsNullOrEmpty(_ldapPath))
                {
                    _ldapPath = "LDAP://DC=CORP,DC=wangcaisoft,DC=com";
                }
                return _ldapPath;
            }
            set => _ldapPath = value;
        }
        

        private static string _ldapDomain = string.Empty;
        /// <summary>
        /// LDAP域
        /// </summary>
        public static string LdapDomain
        {
            get
            {
                if (string.IsNullOrEmpty(_ldapDomain))
                {
                    if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["LDAPDomain"]))
                    {
                        _ldapDomain = ConfigurationManager.AppSettings["LDAPDomain"];
                    }
                    if (string.IsNullOrEmpty(_ldapDomain))
                    {
                        _ldapDomain = "wangcaisoft";
                    }
                }
                return _ldapDomain;
            }
            set => _ldapDomain = value;
        }
        
    }
}