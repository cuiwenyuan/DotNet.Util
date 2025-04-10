﻿//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2025, DotNet.
//-----------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;

namespace DotNet.Util
{
    /// <summary>
    /// MachineInfo
    /// 获取硬件信息的部分
    /// 
    /// 修改记录
    ///
    ///		2016.05.09 版本：1.1 JiRiGaLa	获取mac地址的算法进行优化。
    ///		2011.07.15 版本：1.0 JiRiGaLa	主键创建。
    ///		
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2016.05.09</date>
    /// </author>
    /// </summary>
    public partial class MachineInfo
    {
        /// <summary>
        /// 获取当前使用的IPV4地址
        /// </summary>
        /// <returns></returns>
        public static string GetIpAddress()
        {
            var result = string.Empty;

            // System.Net.IPHostEntry ipHostEntrys = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());
            var ipList = GetIpAddressList();
            foreach (var ip in ipList)
            {
                result = ip;
                break;
            }

            return result;
        }

        /// <summary>
        /// 获取IPv4地址列表
        /// </summary>
        /// <returns></returns>
        public static List<string> GetIpAddressList()
        {
            var result = new List<string>();

            var ipHostEntrys = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in ipHostEntrys.AddressList)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    result.Add(ip.ToString());
                }
            }

            return result;
        }

        /// <summary>
        /// GetWirelessIPList 获得无线网络接口的IpV4 地址列表
        /// </summary>
        /// <returns></returns>
        public static List<string> GetWirelessIpAddressList()
        {
            var result = new List<string>();

            var networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
            foreach (var ni in networkInterfaces)
            {
                if (ni.Description.Contains("Wireless"))
                {
                    foreach (var ip in ni.GetIPProperties().UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        {
                            result.Add(ip.Address.ToString());
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// 获取地址
        /// </summary>
        /// <returns>地址</returns>
        public static string GetMacAddress(bool single = true)
        {
            var result = string.Empty;

            var macAddress = string.Empty;
            var macAddressList = GetMacAddressList().OrderBy(ip => ip).Take(2).ToList();
            foreach (var mac in macAddressList)
            {
                if (!string.IsNullOrEmpty(mac))
                {
                    macAddress = mac;
                    // 格式化
                    macAddress = string.Format("{0}-{1}-{2}-{3}-{4}-{5}",
                        macAddress.Substring(0, 2),
                        macAddress.Substring(2, 2),
                        macAddress.Substring(4, 2),
                        macAddress.Substring(6, 2),
                        macAddress.Substring(8, 2),
                        macAddress.Substring(10, 2));
                    if (single)
                    {
                        result = macAddress;
                        break;
                    }
                    else
                    {
                        result += macAddress + ";";
                    }
                }
            }
            result = result.TrimEnd(';');

            return result;
        }

        /// <summary>
        /// 获取MAC地址列表，注意优先级高的放在了后面
        /// </summary>
        /// <returns></returns>
        private static List<string> GetMacAddressList()
        {
            var result = new List<string>();

            var networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
            foreach (var ni in networkInterfaces)
            {
                // 网卡备注中有wireless，则判断是无限网卡,过滤掉虚拟网卡和移动网卡
                // !ni.Description.Contains("WiFi") && 
                if (!ni.Description.Contains("Loopback")
                    && !ni.Description.Contains("VMware")
                    && !ni.Description.Contains("Teredo")
                    && !ni.Description.Contains("Microsoft")
                    && !ni.Description.Contains("Virtual")
                    && !ni.Description.Contains("Microsoft")
                    && !ni.Description.Contains("IEEE 1394")
                    && ni.OperationalStatus == OperationalStatus.Up)
                {
                    var macAddress = ni.GetPhysicalAddress().ToString();
                    if (!string.IsNullOrEmpty(macAddress) && !macAddress.StartsWith("000000000000") && macAddress.Length == 12)
                    {
                        result.Add(ni.GetPhysicalAddress().ToString());
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// GetWirelessMacList 获得无线网络接口的MAC地址列表
        /// </summary>
        /// <returns></returns>
        public static List<string> GetWirelessMacAddressList()
        {
            var macAddressList = new List<string>();
            var networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
            foreach (var ni in networkInterfaces)
            {
                // 网卡备注中有wireless，则判断是无限 网卡
                if (ni.Description.Contains("Wireless") && ni.OperationalStatus == OperationalStatus.Up)
                {
                    macAddressList.Add(ni.GetPhysicalAddress().ToString());
                }
            }
            return macAddressList;
        }

        /// <summary>
        /// 获取cpu序列号
        /// </summary>
        /// <returns>序列号</returns>
        public static string GetCpuSerialNo()
        {
            var cpuSerialNo = string.Empty;
#if NET452_OR_GREATER
            var managementClass = new ManagementClass("Win32_Processor");
            var managementObjectCollection = managementClass.GetInstances();
            foreach (var o in managementObjectCollection)
            {
                var managementObject = (ManagementObject)o;
                // 可能是有多个
                cpuSerialNo = managementObject.Properties["ProcessorId"].Value.ToString();
                break;
            }
#elif NETSTANDARD2_0_OR_GREATER
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var managementClass = new ManagementClass("Win32_Processor");
                var managementObjectCollection = managementClass.GetInstances();
                foreach (var o in managementObjectCollection)
                {
                    var managementObject = (ManagementObject)o;
                    // 可能是有多个
                    cpuSerialNo = managementObject.Properties["ProcessorId"].Value.ToString();
                    break;
                }
            }
#endif
            return cpuSerialNo;
        }
        /// <summary>
        /// 获取硬盘信息
        /// </summary>
        /// <returns></returns>
        public static string GetHardDiskInfo()
        {
            var hardDisk = string.Empty;
#if NET452_OR_GREATER
            var managementClass = new ManagementClass("Win32_DiskDrive");
            var managementObjectCollection = managementClass.GetInstances();
            foreach (var o in managementObjectCollection)
            {
                var managementObject = (ManagementObject)o;
                // 可能是有多个
                hardDisk = (string)managementObject.Properties["Model"].Value;
                break;
            }
#elif NETSTANDARD2_0_OR_GREATER
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var managementClass = new ManagementClass("Win32_DiskDrive");
                var managementObjectCollection = managementClass.GetInstances();
                foreach (var o in managementObjectCollection)
                {
                    var managementObject = (ManagementObject)o;
                    // 可能是有多个
                    hardDisk = (string)managementObject.Properties["Model"].Value;
                    break;
                }
            }
#endif
            return hardDisk;
        }
        /// <summary>
        /// 设置本地时间
        /// </summary>
        /// <param name="dateTime"></param>
        public static void SetLocalTime(DateTime dateTime)
        {
            var systemTime = new SystemTime();
            SetSystemDateTime.GetLocalTime(systemTime);
            systemTime.vYear = (ushort)dateTime.Year;
            systemTime.vMonth = (ushort)dateTime.Month;
            systemTime.vDay = (ushort)dateTime.Day;
            systemTime.vHour = (ushort)dateTime.Hour;
            systemTime.vMinute = (ushort)dateTime.Minute;
            systemTime.vSecond = (ushort)dateTime.Second;
            SetSystemDateTime.SetLocalTime(systemTime);
        }
        /// <summary>
        /// 获取本地默认
        /// </summary>
        /// <returns></returns>
        [DllImport("kernel32.dll", EntryPoint = "GetSystemDefaultLCID")]
        public static extern int GetSystemDefaultLCID();
        /// <summary>
        /// 设置Locale信息
        /// </summary>
        /// <param name="locale"></param>
        /// <param name="lcType"></param>
        /// <param name="lpLcData"></param>
        /// <returns></returns>
        [DllImport("kernel32.dll", EntryPoint = "SetLocaleInfoA")]
        public static extern int SetLocaleInfo(int locale, int lcType, string lpLcData);
        /// <summary>
        /// LocaleSlongdate
        /// </summary>
        public const int LocaleSlongdate = 0x20;
        /// <summary>
        /// LocaleSshortdate
        /// </summary>
        public const int LocaleSshortdate = 0x1F;
        /// <summary>
        /// LocaleStime
        /// </summary>
        public const int LocaleStime = 0x1003;

        /// <summary>
        /// 设置日期时间格式
        /// </summary>
        public static void SetDateTimeFormat()
        {
            try
            {
                var x = GetSystemDefaultLCID();
                // 时间格式 
                SetLocaleInfo(x, LocaleStime, "HH:mm:ss");
                // 短日期格式
                SetLocaleInfo(x, LocaleSshortdate, "yyyy-MM-dd");
                // 长日期格式
                SetLocaleInfo(x, LocaleSlongdate, "yyyy-MM-dd");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        /// <summary>
        /// 获得真实IP信息 通过能提供IP查询的网站
        /// 2016-01-24 吉日嘎拉 改进服务器诊断能力
        /// </summary>
        /// <param name="ipUrl">提供IP显示的网址</param>
        /// <returns>IP地址</returns>
        public static string GetIpByWebRequest(string ipUrl = "")
        {
            var result = string.Empty;

            if (string.IsNullOrWhiteSpace(ipUrl))
            {
                ipUrl = BaseSystemInfo.UserCenterHost + "/UserCenterV" + BaseSystemInfo.DatabaseTableVersion + "/PermissionService.ashx?function=GetClientIP";
            }
            try
            {
                var uri = new Uri(ipUrl);
                var webRequest = WebRequest.Create(uri);
                using (var stream = webRequest.GetResponse().GetResponseStream())
                {
                    var sr = new System.IO.StreamReader(stream, System.Text.Encoding.Default);
                    result = sr.ReadToEnd();
                }
            }
            catch (Exception)
            {
            }

            return result;
        }
    }
    /// <summary>
    /// 系统时间
    /// </summary>
    [StructLayoutAttribute(LayoutKind.Sequential)]
    public class SystemTime
    {
        /// <summary>
        /// vYear
        /// </summary>
        public ushort vYear;
        /// <summary>
        /// vMonth
        /// </summary>
        public ushort vMonth;
        /// <summary>
        /// vDayOfWeek
        /// </summary>
        public ushort vDayOfWeek;
        /// <summary>
        /// vDay
        /// </summary>
        public ushort vDay;
        /// <summary>
        /// vHour
        /// </summary>
        public ushort vHour;
        /// <summary>
        /// vMinute
        /// </summary>
        public ushort vMinute;
        /// <summary>
        /// vSecond
        /// </summary>
        public ushort vSecond;
    }
    /// <summary>
    /// 设置系统时间
    /// </summary>
    public class SetSystemDateTime
    {
        /// <summary>
        /// 获取本地时间
        /// </summary>
        /// <param name="systemTime"></param>
        [DllImportAttribute("Kernel32.dll")]
        public static extern void GetLocalTime(SystemTime systemTime);
        /// <summary>
        /// 设置本地时间
        /// </summary>
        /// <param name="systemTime"></param>
        [DllImportAttribute("Kernel32.dll")]
        public static extern void SetLocalTime(SystemTime systemTime);
    }
}