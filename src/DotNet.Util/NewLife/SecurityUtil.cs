using NewLife.Data;
using System;
using System.IO;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Cryptography;
using System.Text;

namespace DotNet.Util
{
    /// <summary>安全算法 - 来自NewLife</summary>
    /// <remarks>
    /// 文档 https://www.yuque.com/smartstone/nx/security_helper
    /// </remarks>
    public static class SecurityUtil
    {
        #region 哈希

        /// <summary>MD5散列</summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static Byte[] MD5(this Byte[] data)
        {
            return NewLife.SecurityHelper.MD5(data);
        }

        /// <summary>MD5散列</summary>
        /// <param name="data"></param>
        /// <param name="encoding">字符串编码，默认Default</param>
        /// <returns></returns>
        public static String MD5(this String data, Encoding encoding = null)
        {
            return NewLife.SecurityHelper.MD5(data, encoding);
        }

        /// <summary>MD5散列</summary>
        /// <param name="data"></param>
        /// <param name="encoding">字符串编码，默认Default</param>
        /// <returns></returns>
        public static String MD5_16(this String data, Encoding encoding = null)
        {
            return NewLife.SecurityHelper.MD5_16(data, encoding);
        }

        /// <summary>Crc散列</summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static UInt32 Crc(this Byte[] data) => NewLife.SecurityHelper.Crc(data);

        /// <summary>Crc16散列</summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static UInt16 Crc16(this Byte[] data) => NewLife.SecurityHelper.Crc16(data);

        /// <summary>SHA128</summary>
        /// <param name="data"></param>
        /// <param name="key">键</param>
        /// <returns></returns>
        public static Byte[] SHA1(this Byte[] data, Byte[] key) => NewLife.SecurityHelper.SHA1(data, key);

        /// <summary>SHA256</summary>
        /// <param name="data"></param>
        /// <param name="key">键</param>
        /// <returns></returns>
        public static Byte[] SHA256(this Byte[] data, Byte[] key) => NewLife.SecurityHelper.SHA256(data, key);

        /// <summary>SHA384</summary>
        /// <param name="data"></param>
        /// <param name="key">键</param>
        /// <returns></returns>
        public static Byte[] SHA384(this Byte[] data, Byte[] key) => NewLife.SecurityHelper.SHA384(data, key);

        /// <summary>SHA512</summary>
        /// <param name="data"></param>
        /// <param name="key">键</param>
        /// <returns></returns>
        public static Byte[] SHA512(this Byte[] data, Byte[] key) => NewLife.SecurityHelper.SHA512(data, key);

        #endregion

        #region 同步加密扩展
        /// <summary>对称加密算法扩展</summary>
        /// <remarks>注意：CryptoStream会把 outstream 数据流关闭</remarks>
        /// <param name="sa"></param>
        /// <param name="instream"></param>
        /// <param name="outstream"></param>
        /// <returns></returns>
        public static SymmetricAlgorithm Encrypt(this SymmetricAlgorithm sa, Stream instream, Stream outstream)
        {
            return NewLife.SecurityHelper.Encrypt(sa, instream, outstream);
        }

        /// <summary>对称加密算法扩展</summary>
        /// <param name="sa">算法</param>
        /// <param name="data">数据</param>
        /// <param name="pass">密码</param>
        /// <param name="mode">模式。.Net默认CBC，Java默认ECB</param>
        /// <param name="padding">填充算法。默认PKCS7，等同Java的PKCS5</param>
        /// <returns></returns>
        public static Byte[] Encrypt(this SymmetricAlgorithm sa, Byte[] data, Byte[] pass = null, CipherMode mode = CipherMode.CBC, PaddingMode padding = PaddingMode.PKCS7)
        {
            return NewLife.SecurityHelper.Encrypt(sa, data, pass: pass, mode: mode, padding: padding);
        }

        /// <summary>对称解密算法扩展
        /// <para>注意：CryptoStream会把 instream 数据流关闭</para>
        /// </summary>
        /// <param name="sa"></param>
        /// <param name="instream"></param>
        /// <param name="outstream"></param>
        /// <returns></returns>
        public static SymmetricAlgorithm Decrypt(this SymmetricAlgorithm sa, Stream instream, Stream outstream)
        {
            return NewLife.SecurityHelper.Encrypt(sa, instream, outstream);
        }

        /// <summary>对称解密算法扩展</summary>
        /// <param name="sa">算法</param>
        /// <param name="data">数据</param>
        /// <param name="pass">密码</param>
        /// <param name="mode">模式。.Net默认CBC，Java默认ECB</param>
        /// <param name="padding">填充算法。默认PKCS7，等同Java的PKCS5</param>
        /// <returns></returns>
        public static Byte[] Decrypt(this SymmetricAlgorithm sa, Byte[] data, Byte[] pass = null, CipherMode mode = CipherMode.CBC, PaddingMode padding = PaddingMode.PKCS7)
        {
            return NewLife.SecurityHelper.Encrypt(sa, data, pass: pass, mode: mode, padding: padding);
        }

        #endregion

        #region RC4
        /// <summary>RC4对称加密算法</summary>
        /// <param name="data"></param>
        /// <param name="pass"></param>
        /// <returns></returns>
        public static Byte[] RC4(this Byte[] data, Byte[] pass) => NewLife.SecurityHelper.RC4(data, pass);
        #endregion
    }
}