using System;
using System.Text;

namespace DotNet.Util
{
    /// <summary>对象池扩展StringBuilder</summary>
    /// <remarks>
    /// 文档 https://www.yuque.com/smartstone/nx/object_pool
    /// </remarks>
    public static partial class Pool
    {
        #region StringBuilder
        /// <summary>字符串构建器池</summary>
        public static IPool<StringBuilder> StringBuilder { get; set; } = new StringBuilderPool();

        /// <summary>归还一个字符串构建器到对象池</summary>
        /// <param name="sb">StringBuilder</param>
        /// <param name="returnResult">是否需要返回结果</param>
        /// <returns></returns>
        public static String Put(this StringBuilder sb, Boolean returnResult = true)
        {
            if (sb == null) return null;

            var str = returnResult ? sb.ToString() : null;

            Pool.StringBuilder.Put(sb);

            return str;
        }

        #region StringBuilderPool

        /// <summary>字符串构建器池</summary>
        public class StringBuilderPool : Pool<StringBuilder>
        {
            /// <summary>初始容量。默认最大字符数的初始值100</summary>
            public Int32 InitialCapacity { get; set; } = 100;

            /// <summary>最大容量。超过该大小时不进入池内，默认4k</summary>
            public Int32 MaximumCapacity { get; set; } = 4 * 1024;

            /// <summary>创建</summary>
            /// <returns></returns>
            protected override StringBuilder OnCreate() => new(InitialCapacity);

            /// <summary>归还</summary>
            /// <param name="sb"></param>
            /// <returns></returns>
            public override Boolean Put(StringBuilder sb)
            {
                if (sb.Capacity > MaximumCapacity)
                {
                    return false;
                }

                sb.Clear();

                return true;
            }
        }

        #endregion

        #endregion
    }
}