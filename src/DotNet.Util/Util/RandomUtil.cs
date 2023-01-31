//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2023, DotNet.
//-----------------------------------------------------------------

using System;

namespace DotNet.Util
{
    /// <summary>
    /// BaseRandom
    /// 产生随机数
    /// 
    /// 随机数管理，最大值、最小值可以自己进行设定。
    /// 
    /// 修改记录
    /// 
    ///     2021.08.05 版本：4.0 Troy.Cui	简化方法名，去掉Random。
    ///     2009.07.08 版本：3.0 JiRiGaLa	更新完善程序，将方法修改为静态方法。
    ///		2007.06.30 版本：3.2 JiRiGaLa   产生随机字符。
    ///		2006.02.05 版本：3.1 JiRiGaLa   重新调整主键的规范化。
    ///		2004.11.12 版本：3.0 JiRiGaLa   最后修改，改进成以后可以扩展到多种数据库的结构形式。
    ///	    2004.11.12 版本：3.0 JiRiGaLa   一些方法改进，主键格式优化，基本上看上去还过得去了。
    ///     2005.03.07 版本：2.0 JiRiGaLa   2005.03.07 更新程序排版。
    ///     2005.08.13 版本：1.0 JiRiGaLa   参数格式标准化。
    ///     
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2007.06.30</date>
    /// </author> 
    /// </summary>
    public partial class RandomUtil
    {
        /// <summary>
        /// 最小值
        /// </summary>
        public static int Minimum = 100000;
        /// <summary>
        /// 最大值
        /// </summary>
        public static int Maximal = 999999;
        /// <summary>
        /// 随机数长度
        /// </summary>
        public static int RandomLength = 6;

        private const string _randomString = "0123456789ABCDEFGHIJKMLNPQRSTUVWXYZ";
        private const string _randomNumber = "0123456789";
        private static Random _random = new Random(DateTime.Now.Millisecond);

        #region public static string GetString() 产生随机字符
        /// <summary>
        /// 产生随机字符
        /// </summary>
        /// <returns>字符串</returns>
        public static string GetString(int length = 0)
        {
            var result = string.Empty;
            if (length <= 0)
            {
                length = RandomLength;
            }
            for (var i = 0; i < length; i++)
            {
                var r = _random.Next(0, _randomString.Length - 1);
                result += _randomString[r];
            }
            return result;
        }
        #endregion

        #region public static string GetNumber() 产生随机整数
        /// <summary>
        /// 产生随机整数
        /// </summary>
        /// <returns>整数字符串</returns>
        public static string GetNumber(int length = 0)
        {
            var result = string.Empty;
            if (length <= 0)
            {
                length = RandomLength;
            }
            for (var i = 0; i < length; i++)
            {
                var r = _random.Next(0, _randomNumber.Length - 1);
                result += _randomNumber[r];
            }
            return result;
        }
        #endregion

        #region public static int GetRandom()
        /// <summary>
        /// 产生随机数
        /// </summary>
        /// <returns>随机数</returns>
        public static int GetRandom()
        {
            return _random.Next(Minimum, Maximal);
        }
        #endregion

        #region public static int GetRandom(int minimum, int maximal)
        /// <summary>
        /// 产生随机数
        /// </summary>
        /// <param name="minimum">最小值</param>
        /// <param name="maximal">最大值</param>
        /// <returns>随机数</returns>
        public static int GetRandom(int minimum, int maximal)
        {
            return _random.Next(minimum, maximal);
        }
        #endregion
    }
}