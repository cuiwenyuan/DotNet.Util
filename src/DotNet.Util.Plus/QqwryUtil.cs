using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Net;

namespace DotNet.Util
{
    /// <summary>
    /// Qqwry工具
    /// </summary>
    public static class QqwryUtil
    {
        #region 成员变量

        private const byte RedirectMode1 = 0x01;//名称存储模式一  
        private const byte RedirectMode2 = 0x02;//名称存储模式二  
        private const int IpRecordLength = 7; //每条索引的长度  

        private static long _beginIndex = 0;//索引开始  
        private static long _endIndex = 0;//索引结束  

        private static StLocation _loc = new StLocation() { City = "未知城市", Country = "未知国家", Area = "未知地区" };

        private static Stream _fs;

        #endregion

        #region 私有成员函数

        /// <summary>  
        /// 在索引区查找指定IP对应的记录区地址  
        /// </summary>  
        /// <param name="ip">字节型IP</param>  
        /// <returns></returns>  
        private static long SearchIpIndex(byte[] ip)
        {
            long index = 0;

            var nextIp = new byte[4];

            ReadIp(_beginIndex, ref nextIp);

            var flag = CompareIp(ip, nextIp);
            if (flag == 0) return _beginIndex;
            else if (flag < 0) return -1;

            for (long i = _beginIndex, j = _endIndex; i < j;)
            {
                index = GetMiddleOffset(i, j);

                ReadIp(index, ref nextIp);
                flag = CompareIp(ip, nextIp);

                if (flag == 0) return ReadLong(index + 4, 3);
                else if (flag > 0) i = index;
                else if (flag < 0)
                {
                    if (index == j)
                    {
                        j -= IpRecordLength;
                        index = j;
                    }
                    else
                    {
                        j = index;
                    }
                }
            }

            index = ReadLong(index + 4, 3);
            ReadIp(index, ref nextIp);

            flag = CompareIp(ip, nextIp);
            if (flag <= 0) return index;
            else return -1;
        }

        /// <summary>  
        /// 获取两个索引的中间位置  
        /// </summary>  
        /// <param name="begin">索引1</param>  
        /// <param name="end">索引2</param>  
        /// <returns></returns>  
        private static long GetMiddleOffset(long begin, long end)
        {
            var records = (end - begin) / IpRecordLength;
            records >>= 1;
            if (records == 0) records = 1;
            return begin + records * IpRecordLength;
        }

        /// <summary>  
        /// 读取记录区的地区名称  
        /// </summary>  
        /// <param name="offset">位置</param>  
        /// <returns></returns>  
        private static string ReadString(long offset)
        {
            _fs.Position = offset;

            var b = (byte)_fs.ReadByte();
            if (b == RedirectMode1 || b == RedirectMode2)
            {
                var areaOffset = ReadLong(offset + 1, 3);
                if (areaOffset == 0)
                    return "未知";

                else _fs.Position = areaOffset;
            }
            else
            {
                _fs.Position = offset;
            }

            var buf = new List<byte>();

            var i = 0;
            for (i = 0, buf.Add((byte)_fs.ReadByte()); buf[i] != (byte)(0); ++i, buf.Add((byte)_fs.ReadByte()))
            {
            }

            if (i > 0) return Encoding.Default.GetString(buf.ToArray(), 0, i);
            else return "";
        }

        /// <summary>  
        /// 从自定位置读取指定长度的字节，并转换为big-endian字节序(数据源文件为little-endian字节序)  
        /// </summary>  
        /// <param name="offset">开始读取位置</param>  
        /// <param name="length">读取长度</param>  
        /// <returns></returns>  
        private static long ReadLong(long offset, int length)
        {
            long ret = 0;
            _fs.Position = offset;
            for (var i = 0; i < length; i++)
            {
                ret |= ((_fs.ReadByte() << (i * 8)) & (0xFF * ((long)Math.Pow(16, i * 2))));
            }

            return ret;
        }

        /// <summary>  
        /// 从指定位置处读取一个IP  
        /// </summary>  
        /// <param name="offset">指定的位置</param>  
        /// <param name="buffIp">保存IP的缓存区</param>  
        private static void ReadIp(long offset, ref byte[] buffIp)
        {
            _fs.Position = offset;
            _fs.Read(buffIp, 0, buffIp.Length);

            for (var i = 0; i < buffIp.Length / 2; i++)
            {
                var temp = buffIp[i];
                buffIp[i] = buffIp[buffIp.Length - i - 1];
                buffIp[buffIp.Length - i - 1] = temp;
            }
        }

        /// <summary>  
        /// 比较两个IP是否相等，1:IP1大于IP2，-1：IP1小于IP2，0：IP1=IP2  
        /// </summary>  
        /// <param name="buffIp1">IP1</param>  
        /// <param name="buffIp2">IP2</param>  
        /// <returns></returns>  
        private static int CompareIp(byte[] buffIp1, byte[] buffIp2)
        {
            if (buffIp1.Length > 4 || buffIp2.Length > 4) throw new Exception("无效IP");

            for (var i = 0; i < 4; i++)
            {
                if ((buffIp1[i] & 0xFF) > (buffIp2[i] & 0xFF)) return 1;
                else if ((buffIp1[i] & 0xFF) < (buffIp2[i] & 0xFF)) return -1;
            }

            return 0;
        }

        /// <summary>  
        /// 从指定的地址获取区域名称  
        /// </summary>  
        /// <param name="offset"></param>  
        private static void GetAreaName(long offset)
        {
            _fs.Position = offset + 4;
            long flag = _fs.ReadByte();
            long contryIndex = 0;
            if (flag == RedirectMode1)
            {
                contryIndex = ReadLong(_fs.Position, 3);
                _fs.Position = contryIndex;

                flag = _fs.ReadByte();

                if (flag == RedirectMode2)    //是否仍然为重定向  
                {
                    _loc.Country = ReadString(ReadLong(_fs.Position, 3));
                    _fs.Position = contryIndex + 4;
                }
                else
                {
                    _loc.Country = ReadString(contryIndex);
                }
                _loc.City = ReadString(_fs.Position);
            }
            else if (flag == RedirectMode2)
            {
                contryIndex = ReadLong(_fs.Position, 3);
                _loc.Country = ReadString(contryIndex);
                _loc.City = ReadString(contryIndex + 3);
            }
            else
            {
                _loc.Country = ReadString(offset + 4);
                _loc.City = ReadString(_fs.Position);
            }
        }

        #endregion

        #region 公有成员函数

        /// <summary>  
        /// 加载数据库文件到缓存  
        /// </summary>  
        /// <param name="path">数据库文件地址</param>  
        /// <returns></returns>  
        private static void Init(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                path = Utils.GetMapPath("/plus/qqwry.dat");
            }
            _fs = new FileStream(path, FileMode.Open);
        }

        /// <summary>  
        /// 根据IP获取区域名  
        /// </summary>  
        /// <param name="ip">指定的IP</param>  
        /// <returns></returns>  
        public static StLocation GetLocation(string ip)
        {
            //自动初始化
            Init(null);

            IPAddress ipAddress = null;
            if (!IPAddress.TryParse(ip, out ipAddress)) throw new Exception("无效IP");

            var buffLocalIp = ipAddress.GetAddressBytes();

            _beginIndex = ReadLong(0, 4);
            _endIndex = ReadLong(4, 4);

            var offset = SearchIpIndex(buffLocalIp);
            if (offset != -1)
            {
                GetAreaName(offset);
            }

            _loc.Country = _loc.Country.Trim();
            _loc.City = _loc.City.Trim().Replace("CZ88.NET", "");

            _loc.Area = _loc.Country + "-" + _loc.City;

            //自动销毁
            Dispose();

            return _loc;
        }

        /// <summary>  
        /// 释放资源  
        /// </summary>  
        private static void Dispose()
        {
            _fs.Dispose();
        }

        #endregion

        /// <summary>  
        /// 存储地区的结构  
        /// </summary>  
        public struct StLocation
        {
            /// <summary>  
            /// 未使用  
            /// </summary>  
            public string Ip;

            /// <summary>  
            /// 国家名  
            /// </summary>  
            public string Country;

            /// <summary>  
            /// 城市名  
            /// </summary>  
            public string City;

            /// <summary>  
            /// 区域  
            /// </summary>  
            public string Area;
        }
    }
}
