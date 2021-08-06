//-----------------------------------------------------------------
// // All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System;

namespace DotNet.Util
{
    /// <summary>
    /// BaseConfiguration
    /// 连接配置。
    /// 
    /// 修改记录
    /// 
    ///     2011.09.29 版本：3.8 JiRiGaLa 删除一些多余的变量定义。
    ///     2011.07.06 版本：3.7 zgl      增加 CHECKIPADDRESS。
    ///		2011.01.21 版本：3.6 JiRiGaLa 自动登录、加密数据库连接功能完善。
    ///		2008.06.08 版本：3.6 JiRiGaLa 将读取配置文件进行分离。
    ///		2008.05.08 版本：3.4 JiRiGaLa 获得不同的数据库连接字符串 OracleConnection、SqlConnection、OleDbConnection。
    ///		2007.11.28 版本：3.2 JiRiGaLa 获得数据连接字符串，减少配置文件的读的次序，提高性能。
    ///		2007.05.23 版本：3.1 JiRiGaLa 增加 public const string 定义部分。
    ///		2007.04.14 版本：3.0 JiRiGaLa 检查程序格式通过，不再进行修改主键操作。
    ///		2006.11.17 版本：2.4 JiRiGaLa GetFromRegistryKey() 方法主键进行整理，删除多余的引用。
    ///		2006.05.02 版本：2.3 JiRiGaLa GetFromConfig 增加从配置文件读取数据库联接的方法。
    ///		2006.04.18 版本：2.2 JiRiGaLa 重新调整主键的规范化。
    ///		2006.02.02 版本：2.0 JiRiGaLa 删除数据库连接池的想法，修改了命名，更规范化，贴切了。 
    ///		2005.12.29 版本：1.0 JiRiGaLa 从配置文件读取数据库连接。
    /// 
    /// <author>
    ///		<name>Troy Cui</name>
    ///		<date>2011.09.29</date>
    /// </author> 
    /// </summary>
    public partial class BaseConfiguration
    {
        #region public BaseConfiguration(string softName) 设定当前软件Id
        /// <summary>
        /// 设定当前软件Id
        /// </summary>
        /// <param name="softName">当前软件Id</param>
        public BaseConfiguration(string softName)
        {
            BaseSystemInfo.SoftName = softName;
        }
        #endregion

        #region public static ConfigurationCategory GetConfiguration(string configuration) 配置信息的类型判断
        /// <summary>
        /// 配置信息的类型判断
        /// </summary>
        /// <param name="configuration">配置信息类型</param>
        /// <returns>配置信息类型</returns>
        public static ConfigurationCategory GetConfiguration(string configuration)
        {
            var result = ConfigurationCategory.Configuration;
            foreach (ConfigurationCategory configurationCategory in Enum.GetValues(typeof(ConfigurationCategory)))
            {
                if (configurationCategory.ToString().Equals(configuration))
                {
                    result = configurationCategory;
                    break;
                }
            }
            return result;
        }
        #endregion

        #region public static void GetSetting() 读取配置信息
        /// <summary>
        /// 读取配置信息
        /// </summary>
        public static void GetSetting()
        {
            // 读取配置文件
            if (BaseSystemInfo.ConfigurationFrom == ConfigurationCategory.Configuration)
            {
                ConfigurationHelper.GetConfig();
            }
            // 读取个性化配置文件
            if (BaseSystemInfo.ConfigurationFrom == ConfigurationCategory.UserConfig)
            {
                UserConfigHelper.GetConfig();
            }
            // 读取注册表
            if (BaseSystemInfo.ConfigurationFrom == ConfigurationCategory.RegistryKey)
            {
                RegistryHelper.GetConfig();
            }
        }
        #endregion
    }
}