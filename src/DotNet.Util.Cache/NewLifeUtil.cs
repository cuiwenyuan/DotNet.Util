using NewLife.Caching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNet.Util
{
    /// <summary>
    /// 新生命
    /// </summary>
    public static partial class NewLifeUtil
    {
        /// <summary>
        /// 内存缓存（默认字典缓存）
        /// </summary>
        public static MemoryCache MemoryCache { get; set; }

        /// <summary>
        /// Redis缓存
        /// </summary>
        public static Redis Redis { get; set; }

        /// <summary>
        /// 增强版Redis
        /// </summary>
        public static FullRedis FullRedis { get; set; }

    }
}
