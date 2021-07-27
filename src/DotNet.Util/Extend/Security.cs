using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotNet.Util
{
    /// <summary>
    /// 用户输入信息安全操作类
    /// </summary>
    public partial class Security
    {
        /// <summary>
        /// 替换sql语句中的有问题符号'为''
        /// </summary>
        public static string FilterSql(string strInput)
        {
            return (strInput == null) ? "" : strInput.Replace("'", "''");
        }
    }
}
