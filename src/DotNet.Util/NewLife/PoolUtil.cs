using NewLife.Collections;
using System;
using System.IO;
using System.Text;
using System.Threading;

namespace DotNet.Util
{
    /// <summary>对象池</summary>
    /// <remarks>
    /// 文档 https://www.yuque.com/smartstone/nx/object_pool
    /// </remarks>
    public static partial class PoolUtil
    {
        #region StringBuilder
        /// <summary>字符串构建器池</summary>
        public static IPool<StringBuilder> StringBuilder { get; set; } = new StringBuilderPool();

        /// <summary>归还一个字符串构建器到对象池</summary>
        /// <param name="sb">StringBuilder</param>
        /// <param name="returnResult">是否需要返回结果</param>
        /// <returns></returns>
        public static String Return(this StringBuilder sb, Boolean returnResult = true)
        {
            if (sb == null) return null;

            var str = returnResult ? sb.ToString() : null;

            PoolUtil.StringBuilder.Put(sb);

            return str;
        }

        /// <summary>归还一个字符串构建器到对象池</summary>
        /// <param name="sb">StringBuilder</param>
        /// <param name="returnResult">是否需要返回结果</param>
        /// <returns></returns>
        [Obsolete("Pleaes use Return from 2024-02-01", true)]
        public static String Put(this StringBuilder sb, Boolean returnResult = true)
        {
            if (sb == null) return null;

            var str = returnResult ? sb.ToString() : null;

            PoolUtil.StringBuilder.Put(sb);

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

        #region MemoryStream
        /// <summary>内存流池</summary>
        public static IPool<MemoryStream> MemoryStream { get; set; } = new MemoryStreamPool();

        /// <summary>归还一个内存流到对象池</summary>
        /// <param name="ms"></param>
        /// <param name="returnResult">是否需要返回结果</param>
        /// <returns></returns>
        public static Byte[] Return(this MemoryStream ms, Boolean returnResult = true)
        {
            if (ms == null)
            {
                return null;
            }

            var buf = returnResult ? ms.ToArray() : null;

            Pool.MemoryStream.Put(ms);

            return buf;
        }

        /// <summary>归还一个内存流到对象池</summary>
        /// <param name="ms"></param>
        /// <param name="returnResult">是否需要返回结果</param>
        /// <returns></returns>
        [Obsolete("Pleaes use Return from 2024-02-01", true)]
        public static Byte[] Put(this MemoryStream ms, Boolean returnResult = true)
        {
            if (ms == null)
            {
                return null;
            }

            var buf = returnResult ? ms.ToArray() : null;

            Pool.MemoryStream.Put(ms);

            return buf;
        }

        #region MemoryStreamPool

        /// <summary>内存流池</summary>
        public class MemoryStreamPool : Pool<MemoryStream>
        {
            /// <summary>初始容量。默认1024个</summary>
            public Int32 InitialCapacity { get; set; } = 1024;

            /// <summary>最大容量。超过该大小时不进入池内，默认64k</summary>
            public Int32 MaximumCapacity { get; set; } = 64 * 1024;

            /// <summary>创建</summary>
            /// <returns></returns>
            protected override MemoryStream OnCreate() => new(InitialCapacity);

            /// <summary>归还</summary>
            /// <param name="ms"></param>
            /// <returns></returns>
            public override Boolean Put(MemoryStream ms)
            {
                if (ms.Capacity > MaximumCapacity) return false;

                ms.Position = 0;
                ms.SetLength(0);

                return true;
            }
        }


        #endregion

        #endregion
    }
}