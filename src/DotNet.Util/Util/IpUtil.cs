//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2022, DotNet.
//-----------------------------------------------------------------

using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Collections.Specialized;

namespace DotNet.Util
{
    /// <summary>
    /// Ip信息
    /// </summary>
    public class IpInfo
    {
        /// <summary>
        /// 省份
        /// </summary>
        public string Province { get; set; }
        /// <summary>
        /// 城市
        /// </summary>
        public string City { get; set; }
        /// <summary>
        /// IP地址
        /// </summary>
        public string Ip { get; set; }
    }
    /// <summary>
    /// Ip地址信息，17monipdb来自IPIP.net
    /// </summary>
    public partial class IpUtil
    {
        //readonly string ipBinaryFilePath = HttpRuntime.AppDomainAppPath + "\\DataSource\\17monipdb.dat";
        string _ipBinaryFilePath = string.Empty;
        readonly byte[] _dataBuffer, _indexBuffer;
        readonly uint[] _index = new uint[256];
        readonly int _offset;

        private static IpUtil _instance;

        private static object _lock = new object();
        /// <summary>
        /// 获取实例
        /// </summary>
        /// <returns></returns>
        public static IpUtil GetInstance()
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new IpUtil();
                        return _instance;
                    }
                }
            }
            return _instance;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="filePath"></param>
        public IpUtil(string filePath = null)
        {
            try
            {
                if (!string.IsNullOrEmpty(filePath))
                {
                    _ipBinaryFilePath = filePath;
                }
                else
                {
                    _ipBinaryFilePath = AppDomain.CurrentDomain.BaseDirectory + "\\DataBase\\17monipdb.dat";
                }

                var file = new FileInfo(_ipBinaryFilePath);
                _dataBuffer = new byte[file.Length];
                using (var fs = new FileStream(file.FullName, FileMode.Open, FileAccess.Read))
                {
                    fs.Read(_dataBuffer, 0, _dataBuffer.Length);
                }

                var indexLength = BytesToLong(_dataBuffer[0], _dataBuffer[1], _dataBuffer[2], _dataBuffer[3]);
                _indexBuffer = new byte[indexLength];
                Array.Copy(_dataBuffer, 4, _indexBuffer, 0, indexLength);
                _offset = (int)indexLength;

                for (var loop = 0; loop < 256; loop++)
                {
                    _index[loop] = BytesToLong(_indexBuffer[loop * 4 + 3], _indexBuffer[loop * 4 + 2], _indexBuffer[loop * 4 + 1], _indexBuffer[loop * 4]);
                }
            }
            catch (Exception)
            {
            }
        }

        private static uint BytesToLong(byte a, byte b, byte c, byte d)
        {
            return ((uint)a << 24) | ((uint)b << 16) | ((uint)c << 8) | (uint)d;
        }

        private string[] Find(string ip)
        {
            //处理端口号的异常数据
            if (ip.IndexOf(':') > 0)
            {
                ip = ip.Split(':')[0];
            }
            var match =
                new Regex(@"((?:(?:25[0-5]|2[0-4]\d|((1\d{2})|([1-9]?\d)))\.){3}(?:25[0-5]|2[0-4]\d|((1\d{2})|([1-9]?\d))))");
            if (!match.IsMatch(ip))
            {
                return new[] { "", "", "", "" };
            }
            var ips = ip.Split('.');
            
            var ipPrefixValue = int.Parse(ips[0]);
            long ip2LongValue = BytesToLong(byte.Parse(ips[0]), byte.Parse(ips[1]), byte.Parse(ips[2]), byte.Parse(ips[3]));
            var start = _index[ipPrefixValue];
            var maxCompLen = _offset - 1028;
            long indexOffset = -1;
            var indexLength = -1;
            byte b = 0;
            for (start = start * 8 + 1024; start < maxCompLen; start += 8)
            {
                if (BytesToLong(_indexBuffer[start + 0], _indexBuffer[start + 1], _indexBuffer[start + 2], _indexBuffer[start + 3]) >= ip2LongValue)
                {
                    indexOffset = BytesToLong(b, _indexBuffer[start + 6], _indexBuffer[start + 5], _indexBuffer[start + 4]);
                    indexLength = 0xFF & _indexBuffer[start + 7];
                    break;
                }
            }
            if (indexLength > 0)
            {
                var areaBytes = new byte[indexLength];
                Array.Copy(_dataBuffer, _offset + (int)indexOffset - 1024, areaBytes, 0, indexLength);
                return Encoding.UTF8.GetString(areaBytes).Split('\t');
            }
            return null;
        }
        /// <summary>
        /// 查找IP
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public IpInfo FindIp(string ip)
        {
            ip = ip.Split(',')[0].Trim();
            if (string.IsNullOrWhiteSpace(ip) || ip.Length < 7)
            {
                // 错误的地址，不进行处理
                return null;
            }

            var location = Find(ip);
            if (location == null)
            {
                return null;
            }
            else if (string.IsNullOrEmpty(location[2]))
            {
                location[2] = location[1];
            }
            return new IpInfo { Province = location[1], City = location[2], Ip = ip };
        }
        /// <summary>
        /// FindName
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public string FindName(string ip)
        {
            var result = string.Empty;
            var ipInfo = FindIp(ip);
            if (ipInfo != null)
            {
                if (!string.IsNullOrEmpty(ipInfo.Province))
                {
                    result = ipInfo.Province;
                    if (!string.IsNullOrEmpty(ipInfo.City))
                    {
                        if (!ipInfo.Province.Equals(ipInfo.City))
                        {
                            result += "-" + ipInfo.City;
                        }
                    }
                }
            }
            return result;
        }
        /// <summary>
        /// 是否本地IP
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <returns></returns>
        public static bool IsLocalIp(string ipAddress)
        {
            var result = false;
            if (!string.IsNullOrEmpty(ipAddress))
            {
                if (ipAddress.StartsWith("192.168.")
                    || ipAddress.StartsWith("172.")
                    || ipAddress.StartsWith("10.")
                    || ipAddress.StartsWith("127."))
                {
                    result = true;
                }
                // 检查是否在公司新任的列表里
                if (!result)
                {
                    if (!string.IsNullOrEmpty(BaseSystemInfo.WhiteList))
                    {
                        var whiteLists = BaseSystemInfo.WhiteList.Split(',');
                        for (var i = 0; i < whiteLists.Length; i++)
                        {
                            if (whiteLists[i].Equals(ipAddress))
                            {
                                result = true;
                                break;
                            }
                        }
                    }
                }
            }

            return result;
        }
    }
}
