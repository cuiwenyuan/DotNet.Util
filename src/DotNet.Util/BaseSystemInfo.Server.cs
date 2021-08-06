//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
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
    ///		2012.04.14 版本：1.0 JiRiGaLa	主键创建。
    ///		
    /// <author>
    ///		<name>Troy Cui</name>
    ///		<date>2012.04.14</date>
    /// </author>
    /// </summary>
    public partial class BaseSystemInfo
    {
        private static string _userCenterHost = string.Empty;
        /// <summary>
        /// UserCenterHost
        /// </summary>
        public static string UserCenterHost
        {
            get
            {
                // 这个是测试用的
                // return "http://mas.zt-express.com/";

                if (string.IsNullOrEmpty(_userCenterHost))
                {
                    // 针对内部服务调用配置，内部可以指定服务位置
                    if (ConfigurationManager.AppSettings["UserCenterHost"] != null)
                    {
                        _userCenterHost = ConfigurationManager.AppSettings["UserCenterHost"];
                    }
                    // 若没配置用户中心？看是否选了明确的主机？针对CS客户端
                    if (string.IsNullOrEmpty(_userCenterHost))
                    {
                        if (!string.IsNullOrWhiteSpace(Host))
                        {
                            if (Host.IndexOf("http", StringComparison.OrdinalIgnoreCase) < 0)
                            {
                                _userCenterHost = "http://" + Host + "/";
                            }
                            else
                            {
                                _userCenterHost = Host + "/";
                            }
                        }
                    }
                    // 若还是都找不到配置，就用默认的配置文件
                    if (string.IsNullOrEmpty(_userCenterHost))
                    {
                        _userCenterHost = "https://userCenter.zt-express.com/";
                    }
                }
                return _userCenterHost;
            }
            set => _userCenterHost = value;
        }

        /// <summary>
        /// 短信服务器器的地址
        /// 一般都是ip限制了，所以是不能负载均衡的独立的一个服务器
        /// </summary>
        // public static string MobileHost = "http://122.225.117.230/WebAPIV5/API/Mobile/";
        // public static string MobileHost = "http://123.157.107.232/WebAPIV5/API/Mobile/";
        // public static string MobileHost = "http://mas.zto.cn/WebAPIV5/API/Mobile/";

        private static string _mobileHost = string.Empty;
        /// <summary>
        /// MobileHost
        /// </summary>
        public static string MobileHost
        {
            get
            {
                if (ConfigurationManager.AppSettings["MobileHost"] != null)
                {
                    _mobileHost = ConfigurationManager.AppSettings["MobileHost"];
                }
                if (string.IsNullOrEmpty(_mobileHost))
                {
                    _mobileHost = "http://mas.zto.cn/WebAPIV5/API/Mobile/";
                }
                return _mobileHost;
            }
            set => _mobileHost = value;
        }
        /// <summary>
        /// WebHost
        /// </summary>
        public static string WebHost
        {
            get
            {
                // 这个是测试用的
                // return "http://mas.zt-express.com/";

                var webHost = "http://userCenter.zt-express.com/";
                if (ConfigurationManager.AppSettings["WebHost"] != null)
                {
                    webHost = ConfigurationManager.AppSettings["WebHost"];
                }
                if (!string.IsNullOrWhiteSpace(Host))
                {
                    if (Host.IndexOf("http", StringComparison.OrdinalIgnoreCase) < 0)
                    {
                        webHost = "http://" + Host + "/";
                    }
                    else
                    {
                        webHost = Host + "/";
                    }
                }
                return webHost;
            }
        }
        /// <summary>
        /// 启用Redis缓存
        /// </summary>
        public static bool RedisEnabled = false;
        /*
        <add key="RedisHosts" value="ztredis6482(*)134&amp;^%xswed@redis-Read.ztosys.com:6482"/>
        */

        private static string[] _redisHosts = null;
        /// <summary>
        /// RedisHosts
        /// </summary>
        public static string[] RedisHosts
        {
            get
            {
                if (_redisHosts == null)
                {
                    if (ConfigurationManager.AppSettings["RedisHosts"] != null)
                    {
                        _redisHosts = ConfigurationManager.AppSettings["RedisHosts"].Split(',');
                    }
                    if (_redisHosts == null)
                    {
                        _redisHosts = new string[] { "ztredis6482(*)134&^%xswed@redis-Read.ztosys.com:6482" };
                    }
                }
                return _redisHosts;
            }
            set => _redisHosts = value;
        }

        /*
        <add key="RedisHosts" value="ztredis6488(*)134&amp;^%xswed@redis.ztosys.com:6488"/>
        */

        private static string[] _redisReadOnlyHosts = null;
        /// <summary>
        /// RedisReadOnlyHosts
        /// </summary>
        public static string[] RedisReadOnlyHosts
        {
            get
            {
                if (_redisReadOnlyHosts == null)
                {
                    if (ConfigurationManager.AppSettings["RedisReadOnlyHosts"] != null)
                    {
                        _redisReadOnlyHosts = ConfigurationManager.AppSettings["RedisReadOnlyHosts"].Split(',');
                    }
                    if (_redisReadOnlyHosts == null)
                    {
                        _redisReadOnlyHosts = new string[] { "ztredis6488(*)134&^%xswed@redis.ztosys.com:6488" };
                    }
                }
                return _redisReadOnlyHosts;
            }
            set => _redisReadOnlyHosts = value;
        }




        private static string[] _redisOpenIdHosts = null;
        /// <summary>
        /// RedisOpenIdHosts
        /// </summary>
        public static string[] RedisOpenIdHosts
        {
            get
            {
                if (_redisOpenIdHosts == null)
                {
                    if (ConfigurationManager.AppSettings["RedisOpenIdHosts"] != null)
                    {
                        _redisOpenIdHosts = ConfigurationManager.AppSettings["RedisOpenIdHosts"].Split(',');
                    }
                    if (_redisOpenIdHosts == null)
                    {
                        _redisOpenIdHosts = new string[] { "ShZtoRds053##@192.168.1.141:7000" };
                    }
                }
                return _redisOpenIdHosts;
            }
            set => _redisOpenIdHosts = value;
        }

        private static string[] _redisOpenIdReadOnlyHosts = null;
        /// <summary>
        /// RedisOpenIdReadOnlyHosts
        /// </summary>
        public static string[] RedisOpenIdReadOnlyHosts
        {
            get
            {
                if (_redisOpenIdReadOnlyHosts == null)
                {
                    if (ConfigurationManager.AppSettings["RedisOpenIdReadOnlyHosts"] != null)
                    {
                        _redisOpenIdReadOnlyHosts = ConfigurationManager.AppSettings["RedisOpenIdReadOnlyHosts"].Split(',');
                    }
                    if (_redisOpenIdReadOnlyHosts == null)
                    {
                        _redisOpenIdReadOnlyHosts = new string[] { "ShZtoRds053##@192.168.1.142:7000" };
                    }
                }
                return _redisOpenIdReadOnlyHosts;
            }
            set => _redisOpenIdReadOnlyHosts = value;
        }


        /// <summary>
        /// 主机地址
        /// Host = "192.168.0.122";
        /// </summary>
        public static string Host = string.Empty;

        /// <summary>
        /// 端口号
        /// </summary>
        public static int Port = 0;

        /// <summary>
        /// 强制Https访问
        /// </summary>
        public static bool ForceHttps = false;

        /// <summary>
        /// 允许新用户注册
        /// </summary>
        public static bool AllowUserRegister = false;

        /// <summary>
        /// 禁止用户重复登录
        /// 只允许登录一次
        /// </summary>
        public static bool CheckOnLine = true;

        /// <summary>
        /// 软件是否需要注册
        /// </summary>
        public static bool NeedRegister = false;

        /// <summary>
        /// 注册码
        /// </summary>
        public static string RegisterKey = string.Empty;

        private static string _systemCode = string.Empty;
        /// <summary>
        /// 这里是设置，读取哪个系统的菜单
        /// </summary>
        public static string SystemCode
        {
            get
            {
                if (string.IsNullOrEmpty(_systemCode))
                {
                    if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["SystemCode"]))
                    {
                        _systemCode = ConfigurationManager.AppSettings["SystemCode"];
                    }
                    if (string.IsNullOrEmpty(_systemCode))
                    {
                        _systemCode = "Base";
                    }
                }
                return _systemCode;
            }
            set => _systemCode = value;
        }

        /// <summary>
        /// （单位：秒）检查周期3分钟内不在线的，就认为是已经没在线了，生命周期检查
        /// </summary>
        public static int OnLineTimeout = 3 * 60;

        /// <summary>
        /// 每过1分钟，检查一次在线状态
        /// </summary>
        public static int OnLineCheck = 60;

        /// <summary>
        /// 锁不住记录时的循环次数(数据库相关)
        /// </summary>
        public static int LockNoWaitCount = 5;

        /// <summary>
        /// 锁不住记录时的等待时间(数据库相关)
        /// </summary>
        public static int LockNoWaitTickMilliSeconds = 30;

        /// <summary>
        /// 上传文件路径
        /// </summary>
        public static string UploadDirectory = "Document/";

        /// <summary>
        /// 服务实现包
        /// </summary>
        public static string Service = "DotNet.Business";

        /// <summary>
        /// 服务映射工厂
        /// </summary>
        public static string ServiceFactory = "ServiceFactory";
    }
}