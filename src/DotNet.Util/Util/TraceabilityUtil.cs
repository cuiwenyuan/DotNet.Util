using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace DotNet.Util
{
    /// <summary>
    /// 追溯工具
    /// </summary>
    public class TraceabilityUtil
    {
        // 共享 Random 实例，避免高频调用时因种子相同（DateTime.Now.Ticks）生成重复序列；Random 非线程安全，访问加锁
        private static readonly Random SharedRandom = new Random();
        private static readonly object RandomLock = new object();
        private const string Alphabet = "0,1,2,3,4,5,6,7,8,9,a,b,c,d,e,f,g,h,i,j,k,l,m,n,o,p,q,r,s,t,u,v,w,x,y,z,A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z";

        /// <summary>
        /// 生成随机的62位字符串，包含0-9a-zA-Z
        /// </summary>
        /// <returns></returns>
        public static string GenerateKey()
        {
            return GenerateShuffledKey(1);
        }

        /// <summary>
        /// 根据传入的random，生成随机的62位字符串，包含0-9a-zA-Z
        /// </summary>
        /// <param name="random">额外洗牌轮数（建议 >= 1）</param>
        /// <returns></returns>
        public static string GenerateKey(int random)
        {
            if (random < 0) random = 0; // 保留 random=0 返回默认顺序的契约；仅纠负数
            return GenerateShuffledKey(random);
        }

        /// <summary>
        /// 基于共享 Random 做 Fisher-Yates 全洗牌，random 为洗牌轮数；每次调用返回 62 位字母表的均匀置换
        /// 修正 R8-16：原 new Random(DateTime.Now.Ticks) 高频调用种子相同导致重复 key；原洗牌仅交换下标 0 分布不均
        /// </summary>
        private static string GenerateShuffledKey(int rounds)
        {
            var chars = Alphabet.Split(',');
            lock (RandomLock)
            {
                for (var round = 0; round < rounds; round++)
                {
                    for (var i = chars.Length - 1; i > 0; i--)
                    {
                        var j = SharedRandom.Next(i + 1);
                        var tmp = chars[i];
                        chars[i] = chars[j];
                        chars[j] = tmp;
                    }
                }
            }
            return string.Join("", chars);
        }

        /// <summary>
        /// 混淆id为字符串
        /// </summary>
        /// <param name="id"></param>
        /// <param name="key">键</param>
        /// <returns></returns>
        public static string Mixup(long id, string key = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ")
        {
            //确保传进来的key也是62位的
            if (key.Length != 62)
            {
                key = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            }
            var code = Convert(id, key);
            var s = 0;
            foreach (var c in code)
            {
                s += (int)c;
            }
            var len = code.Length;
            var x = (s % len);
            var arr = code.ToCharArray();
            var newarr = new char[arr.Length];
            Array.Copy(arr, x, newarr, 0, len - x);
            Array.Copy(arr, 0, newarr, len - x, x);
            var newKey = "";
            foreach (var c in newarr)
            {
                newKey += c;
            }
            return newKey;
        }

        /// <summary>
        /// 解开混淆字符串
        /// </summary>
        /// <param name="code"></param>
        /// <param name="key">键</param>
        /// <returns></returns>
        public static long UnMixup(string code, string key = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ")
        {
            var s = 0;
            foreach (var c in code)
            {
                s += (int)c;
            }
            var len = code.Length;
            var x = (s % len);
            x = len - x;
            var arr = code.ToCharArray();
            var newarr = new char[arr.Length];
            Array.Copy(arr, x, newarr, 0, len - x);
            Array.Copy(arr, 0, newarr, len - x, x);
            var newKey = "";
            foreach (var c in newarr)
            {
                newKey += c;
            }
            return UnConvert(newKey, key);
        }

        /// <summary>
        /// 10进制转换为62进制
        /// </summary>
        /// <param name="id"></param>
        /// <param name="key">键</param>
        /// <returns></returns>
        private static string Convert(long id, string key)
        {
            if (id < 62)
            {
                return key[(int)id].ToString();
            }
            var y = (int)(id % 62);
            var x = (long)(id / 62);

            return Convert(x, key) + key[y];
        }

        /// <summary>
        /// 将62进制转为10进制
        /// </summary>
        /// <param name="code"></param>
        /// <param name="key">键</param>
        /// <returns></returns>
        private static long UnConvert(string code, string key)
        {
            var v = 0L;
            var len = code.Length;
            for (var i = len - 1; i >= 0; i--)
            {
                var t = key.IndexOf(code[i]);
                double s = (len - i) - 1;
                var m = (long)(Math.Pow(62, s) * t);
                v += m;
            }
            return v;
        }

        private static Int16 GetRnd(Random seekRand)
        {
            var s = (Int16)seekRand.Next(1, 11);
            return s;
        }
    }
}
