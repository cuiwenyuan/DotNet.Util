using System;
using System.IO;

namespace DotNet.Util
{
    /// <summary>对象池扩展</summary>
    /// <remarks>
    /// var ms = Pool.MemoryStream.Get();
    /// ms.Put();
    /// 文档 https://www.yuque.com/smartstone/nx/object_pool
    /// </remarks>
    public static partial class Pool
    {
        #region MemoryStream
        /// <summary>内存流池</summary>
        public static IPool<MemoryStream> MemoryStream { get; set; } = new MemoryStreamPool();

        /// <summary>归还一个内存流到对象池</summary>
        /// <param name="ms"></param>
        /// <param name="returnResult">是否需要返回结果</param>
        /// <returns></returns>
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