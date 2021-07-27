using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.DirectoryServices;

namespace DotNet.Util
{
    public static class DomainUtil
    {
        public static DomainUserInfo GetDomainUserInfo(string domainUserName)
        {
            try
            {
                if (string.IsNullOrEmpty(domainUserName))
                {
                    return null;
                }

                var userArr = domainUserName.Split('\\');
                var domain = userArr[0];
                var loginName = userArr[1];

                var entry = new DirectoryEntry(string.Concat("LDAP://", domain));
                var search = new DirectorySearcher(entry);
                search.Filter = string.Format("(SAMAccountName={0})", loginName);
                search.PropertiesToLoad.Add("SAMAccountName");
                search.PropertiesToLoad.Add("givenName");
                search.PropertiesToLoad.Add("cn");
                search.PropertiesToLoad.Add("mail");

                var result = search.FindOne();
                if (result != null)
                {
                    var info = new DomainUserInfo
                    {
                        SamAccountName = result.Properties["SAMAccountName"][0].ToString(),
                        GivenName = result.Properties["givenName"][0].ToString(),
                        Cn = result.Properties["cn"][0].ToString(),
                        Email = result.Properties["mail"][0].ToString()
                    };
                    return info;
                }
            }
            catch (Exception ex)
            {
                LogUtil.WriteException(ex);
            }

            return null;
        }

        public sealed class DomainUserInfo
        {
            public string SamAccountName;
            public string GivenName;
            public string Cn;
            public string Email;
        }
    }
}
