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
        /// <summary>
        /// 生成随机的62位字符串，包含0-9a-zA-Z
        /// </summary>
        /// <returns></returns>
        public static string GenerateKey()
        {
            var chars = "0,1,2,3,4,5,6,7,8,9,a,b,c,d,e,f,g,h,i,j,k,l,m,n,o,p,q,r,s,t,u,v,w,x,y,z,A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z".Split(',');
            var seed = unchecked((int)DateTime.Now.Ticks);
            var random = new Random(seed);
            for (var i = 0; i < 100000; i++)
            {
                var r = random.Next(1, chars.Length);
                var f = chars[0];
                chars[0] = chars[r - 1];
                chars[r - 1] = f;
            }
            return string.Join("", chars);
        }

        /// <summary>
        /// 根据传入的random，生成随机的62位字符串，包含0-9a-zA-Z
        /// </summary>
        /// <returns></returns>
        public static string GenerateKey(int random)
        {
            var chars = "0,1,2,3,4,5,6,7,8,9,a,b,c,d,e,f,g,h,i,j,k,l,m,n,o,p,q,r,s,t,u,v,w,x,y,z,A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z".Split(',');
            for (var i = 0; i < random; i++)
            {
                var r = random;
                var f = chars[0];
                if (random > chars.Length)
                {
                    r = random % 62;
                }
                chars[0] = chars[r - 1];
                chars[r - 1] = f;
            }
            return string.Join("", chars);
        }

        /// <summary>
        /// 混淆id为字符串
        /// </summary>
        /// <param name="id"></param>
        /// <param name="key"></param>
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
        /// <param name="key"></param>
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
        /// <param name="key"></param>
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
        /// <param name="key"></param>
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
            Int16 s = (Int16)seekRand.Next(1, 11);
            return s;
        }
    }
}
