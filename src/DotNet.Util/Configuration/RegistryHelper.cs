//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using Microsoft.Win32;
using System.Runtime.InteropServices;

namespace DotNet.Util
{
    /// <summary>
    /// RegistryHelper
    /// 访问注册表的类。
    /// 
    /// 修改记录
    ///
    ///		2008.06.08 版本：3.2 JiRiGaLa 命名修改为 RegistryHelper。 
    ///		2007.07.30 版本：3.1 JiRiGaLa Exists 函数名规范化。 
    ///		2007.04.14 版本：3.0 JiRiGaLa 检查程序格式通过，不再进行修改主键操作。 
    ///     2006.11.17 版本：2.2 JiRiGaLa 添加方法CheckExistSubKey()。
    ///     2006.09.08 版本：2.1 JiRiGaLa 变量命名规范化。
    ///     2006.04.18 版本：2.0 JiRiGaLa 重新调整主键的规范化。
    ///		2005.08.08 版本：1.0 JiRiGaLa 专门读取注册表的类，主键书写格式改进。
    ///		
    ///	版本：3.0
    /// 
    /// <author>
    ///		<name>Troy Cui</name>
    ///		<date>2007.04.14</date>
    /// </author> 
    /// </summary>
    public partial class RegistryHelper
    {
        /// <summary>
        /// 注册表中的位置
        /// </summary>
        public static string SubKey = "Software\\" + "DotNet";

        #region public static string GetValue(string key) 读取注册表
        /// <summary>
	/// 读取注册表
	/// </summary>
        /// <param name="key">注册表子项</param>
		/// <returns>值</returns>
        public static string GetValue(string key)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var registryKey = Registry.LocalMachine.OpenSubKey(SubKey, false);
                return (string)registryKey.GetValue(key);
            }
            return null;
        }
        #endregion

        #region public static void SetValue(string key, string keyValue) 设置注册表
        /// <summary>
        /// 设置注册表
        /// </summary>
        /// <param name="subKey">注册表子项</param>
        /// <param name="registryKey">键</param>
        /// <param name="keyValue">值</param>
        public static void SetValue(string key, string keyValue)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var registryKey = Registry.LocalMachine.OpenSubKey(SubKey, true);
                registryKey.SetValue(key, keyValue);
            }
        }
        #endregion

        #region public static bool Exists(string key) 判断一个注册表项是否存在
        /// <summary>
        /// 判断一个注册表项是否存在
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>是否存在</returns>
        public static bool Exists(string key)
        {
            return Exists(SubKey, key);
        }
        #endregion

        #region public static bool Exists(string subKey, string key)
        /// <summary>
        /// 判断一个注册表项是否存在
        /// </summary>
        /// <param name="subKey">注册表子项</param>
        /// <param name="key">键</param>
        /// <returns>是否存在</returns>
        public static bool Exists(string subKey, string key)
        {
            var result = false;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var registryKey = Registry.LocalMachine.OpenSubKey(subKey, false);
                var subKeyNames = registryKey.GetSubKeyNames();
                for (var i = 0; i < subKeyNames.Length; i++)
                {
                    if (key.Equals(subKeyNames[i]))
                    {
                        result = true;
                        break;
                    }
                }
            }
            return result;
        }
        #endregion

        #region private static void GetValues() 获取注册表的值
        /// <summary>
        /// 获取注册表的值
        /// </summary>
        private static void GetValues()
        {
            SubKey = "Software\\" + "DotNet" + "\\" + BaseSystemInfo.SoftName;
            // 客户信息配置
            BaseSystemInfo.CustomerCompanyId = GetValue("CustomerCompanyId");
            BaseSystemInfo.CustomerCompanyName = GetValue("CustomerCompanyName");
            BaseSystemInfo.ConfigurationFrom = BaseConfiguration.GetConfiguration(GetValue("ConfigurationFrom"));
            BaseSystemInfo.TimeFormat = GetValue("TimeFormat");
            BaseSystemInfo.DateFormat = GetValue("DateFormat");
            BaseSystemInfo.DateTimeFormat = GetValue("DateTimeFormat");
            BaseSystemInfo.DateTimeLongFormat = GetValue("DateTimeLongFormat");
            BaseSystemInfo.SoftName = GetValue("SoftName");
            BaseSystemInfo.SoftFullName = GetValue("SoftFullName");
            BaseSystemInfo.CurrentLanguage = GetValue("CurrentLanguage");
            BaseSystemInfo.Version = GetValue("Version");

            // 数据库连接
            BaseSystemInfo.BusinessDbConnection = GetValue("BusinessDbConnection");
            BaseSystemInfo.UserCenterDbConnection = GetValue("UserCenterDbConnection");

            BaseSystemInfo.BusinessDbType = DbUtil.GetDbType(GetValue("DbType"));
            BaseSystemInfo.RegisterKey = GetValue("RegisterKey");
        }
        #endregion

        #region private static void SetValues() 设置注册表的值
        /// <summary>
        /// 设置注册表的值
        /// </summary>
        private static void SetValues()
        {
            // 默认的信息写入注册表
            SubKey = "Software\\" + "DotNet" + "\\" + BaseSystemInfo.SoftName;
            SetValue("CustomerCompanyId", BaseSystemInfo.CustomerCompanyId);
            SetValue("CustomerCompanyName", BaseSystemInfo.CustomerCompanyName);
            SetValue("ConfigurationFrom", BaseSystemInfo.RegisterKey);
            SetValue("TimeFormat", BaseSystemInfo.TimeFormat);
            SetValue("DateFormat", BaseSystemInfo.DateFormat);
            SetValue("DateTimeFormat", BaseSystemInfo.DateTimeFormat);
            SetValue("DateTimeLongFormat", BaseSystemInfo.DateTimeLongFormat);
            SetValue("SoftName", BaseSystemInfo.SoftName);
            SetValue("SoftFullName", BaseSystemInfo.SoftFullName);
            SetValue("CurrentLanguage", BaseSystemInfo.CurrentLanguage);

            // 数据库连接
            SetValue("DbType", CurrentDbType.SqlServer.ToString());
            SetValue("RegisterKey", "DotNet");
        }
        #endregion

        #region public static void GetConfig() 读取注册表信息
        /// <summary>
        /// 读取注册表信息
        /// 获取系统配置信息，在系统的原头解决问题，呵呵不错
        /// </summary>
        public static void GetConfig()
        {
            // 读取注册表信息
            // string subKey = "Software\\" + BaseConfiguration.COMPANY_NAME;
            if (!Exists("Software", "DotNet"))
            {
                // 设置注册表
                SetValues();
            }
            else
            {
                if (!Exists(BaseSystemInfo.SoftName))
                {
                    // 设置注册表
                    SetValues();
                }
            }
            // 检测是否已经有数据了，若已经有数据了，就不进行读取了。
            if (BaseSystemInfo.SoftName.Length == 0)
            {
                GetValues();
            }
        }
        #endregion
    }
}