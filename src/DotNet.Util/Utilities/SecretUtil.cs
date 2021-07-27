//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2020, DotNet.
//-----------------------------------------------------------------

using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;


namespace DotNet.Util
{
    public static partial class SecretUtil
    {
        #region public static string SqlSafe(string inputValue) 检查参数的安全性
        /// <summary>
        /// 检查参数的安全性
        /// </summary>
        /// <param name="inputValue">参数</param>
        /// <returns>安全的参数</returns>
        public static string SqlSafe(string inputValue)
        {
            inputValue = inputValue.Replace("'", "''");
            // value = value.Replace("%", "'%");
            return inputValue;
        }
        #endregion

        #region public static bool IsSqlSafe(string commandText) 检查参数的安全性
        /// <summary>
        /// 检查参数的安全性
        /// </summary>
        /// <param name="commandText">参数</param>
        /// <returns>安全的参数</returns>
        public static bool IsSqlSafe(string commandText)
        {
            var result = true;
            if (!string.IsNullOrEmpty(commandText))
            {
                var unSafeText = new string[] { "Delete", "Insert", "Update", "Truncate"};
                for (var i = 0; i < unSafeText.Length; i++)
                {
                    var unSafeString = unSafeText[i];
                    if (commandText.IndexOf(unSafeString, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        result = false;
                        break;
                    }
                }
            }
            return result;
        }
        #endregion

        /// <summary>
        /// 对数据进行签名
        /// 将来需要改进为，对散列值进行签名
        /// </summary>
        /// <param name="dataToSign">需要签名的数据</param>
        /// <param name="privateKey">私钥</param>
        /// <returns>签名结果</returns>
        public static string SignData(string dataToSign, string privateKey)
        {
            var result = string.Empty;

            var byteConverter = new ASCIIEncoding();
            var buffer = byteConverter.GetBytes(dataToSign);
            try
            {
                var cryptoServiceProvider = new RSACryptoServiceProvider();
                cryptoServiceProvider.ImportCspBlob(Convert.FromBase64String(privateKey));
                var signedData = cryptoServiceProvider.SignData(buffer, new SHA1CryptoServiceProvider());
                result = Convert.ToBase64String(signedData);
            }
            catch (CryptographicException e)
            {
                Console.WriteLine(e.Message);
            }

            return result;
        }

        /// <summary>
        /// 验证数字签名
        /// 将来需要改进为，按散列值进行验证
        /// </summary>
        /// <param name="dataToVerify">需要验证的数据</param>
        /// <param name="sign">签名</param>
        /// <param name="publicKey">公钥</param>
        /// <returns>正确</returns>
        public static bool VerifyData(string dataToVerify, string sign, string publicKey)
        {
            var result = false;

            var signedData = Convert.FromBase64String(sign);
            var byteConverter = new ASCIIEncoding();
            var buffer = byteConverter.GetBytes(dataToVerify);
            try
            {
                var cryptoServiceProvider = new RSACryptoServiceProvider();
                cryptoServiceProvider.ImportCspBlob(Convert.FromBase64String(publicKey));
                result = cryptoServiceProvider.VerifyData(buffer, new SHA1CryptoServiceProvider(), signedData);
            }
            catch (CryptographicException e)
            {
                Console.WriteLine(e.Message);
                return false;
            }

            return result;
        }

        #region public static bool CheckRegister() 检查注册码是否正确
        /// <summary>
        /// 检查注册码是否正确
        /// </summary>
        /// <returns>是否进行了注册</returns>
        public static bool CheckRegister()
        {
            var result = true;
            // if (BaseConfiguration.Instance.CustomerCompanyName.Length == 0)
            // {
            //     result = false;
            // }
            // 只能先用一年再说,否则会惹很多麻烦
            if (BaseSystemInfo.NeedRegister)
            {
                if ((DateTime.Now.Year >= 2020) && (DateTime.Now.Month > 7))
                {
                    result = false;
                }
            }
            // 一定要检查注册码,否则这个软件到处别人复制,我的基类也得不到保障了,这是我的心血,得会珍惜自己的劳动成果.
            // 2007.04.14 JiRiGaLa 改进注册方式,让底层程序更安全一些
            //if (BaseConfiguration.Instance.RegisterKey.Equals(CodeChange(BaseConfiguration.Instance.DataBase + BaseConfiguration.Instance.CustomerCompanyName)))
            //{
            //    result = true;
            //}
            return result;
        }
        #endregion


        //
        // 一 用户密码加密函数
        //

        /// <summary>
        /// 基于Md5的自定义加密字符串方法：输入一个字符串，返回一个由32个字符组成的十六进制的哈希散列（字符串）。
        /// </summary>
        /// <param name="password">密码</param>
        /// <returns>加密值</returns>
        public static string Md5(string password)
        {
            return Md5(password, 32);
        }

        /// <summary>
        /// 基于Md5的自定义加密字符串方法：输入一个字符串，返回一个由32个字符组成的十六进制的哈希散列（字符串）。
        /// </summary>
        /// <param name="password">密码</param>
        /// <param name="length">多少位</param>
        /// <returns>加密密码</returns>
        public static string Md5(string password, int length)
        {
            var result = string.Empty;
            if (!string.IsNullOrEmpty(password))
            {
                //32位加密
                #region 方法1 .NET 4.5中已经废弃不用的API
                //result = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(password, "MD5").ToLower();
                #endregion

                #region 方法2
                //MD5CryptoServiceProvider md5Hasher = new MD5CryptoServiceProvider();
                //byte[] data = md5Hasher.ComputeHash(new UTF8Encoding().GetBytes(password));
                //var sb = new StringBuilder();
                //foreach (var t in data)
                //{
                //    sb.Append(t.ToString("x2"));
                //}
                //result = sb.ToString();
                #endregion

                #region 方法3
                //1.创建一个MD5对象
                MD5 md5 = MD5.Create();
                //2.把字符串变一个byte[]
                byte[] buffer = Encoding.UTF8.GetBytes(password);
                //3.将一个byte[]通过MD5计算到一个新的byte[]，新的byte[]就是计算md5后的结果。
                byte[] md5Buffer = md5.ComputeHash(buffer);
                //释放资源
                md5.Clear();
                //4.将计算后的结果直接显示为字符串
                var sb = new StringBuilder();
                foreach (var t in md5Buffer)
                {
                    //x2:把每个数字转换为16进制，并保留两位数字。
                    sb.Append(t.ToString("x2"));
                }
                result = sb.ToString();
                #endregion

                //16位MD5加密（取32位加密的9~25字符）
                if (length == 16)
                {
                    result = result.Substring(8, 16);
                }
            }
            return result;
        }

        /// <summary>
        /// 基于Sha1的自定义加密字符串方法：输入一个字符串，返回一个由40个字符组成的十六进制的哈希散列（字符串）。
        /// </summary>
        /// <param name="password">密码</param>
        /// <returns>加密密码</returns>
        public static string Sha1(string password)
        {
            var result = string.Empty;
            if (!string.IsNullOrEmpty(password))
            {
                #region 方法3
                //1.创建一个MD5对象
                SHA1 sha1 = SHA1.Create();
                //2.把字符串变一个byte[]
                byte[] buffer = Encoding.UTF8.GetBytes(password);
                //3.将一个byte[]通过SHA1计算到一个新的byte[]，新的byte[]就是计算SHA1后的结果。
                byte[] sha1Buffer = sha1.ComputeHash(buffer);
                //释放资源
                sha1.Clear();
                //sha1.Dispose();//释放当前实例使用的所有资源
                //4.将计算后的结果直接显示为字符串
                var sb = new StringBuilder();
                foreach (var t in sha1Buffer)
                {
                    //x2:把每个数字转换为16进制，并保留两位数字。
                    sb.Append(t.ToString("x2"));
                }
                result = sb.ToString();
                #endregion
            }
            return result;
        }

        /// <summary>
        /// Base64编码
        /// </summary>
        /// <param name="codeType"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public static string EncodeBase64(string codeType, string code)
        {
            var encode = "";
            var bytes = Encoding.GetEncoding(codeType).GetBytes(code);
            try
            {
                encode = Convert.ToBase64String(bytes);
            }
            catch
            {
                encode = code;
            }
            return encode;
        }

        /// <summary>
        /// Base64解码
        /// </summary>
        /// <param name="codeType"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public static string DecodeBase64(string codeType, string code)
        {
            var decode = "";
            var bytes = Convert.FromBase64String(code);
            try
            {
                decode = Encoding.GetEncoding(codeType).GetString(bytes);
            }
            catch
            {
                decode = code;
            }
            return decode;
        }

        /// <summary>
        /// DES数据加密
        /// </summary>
        /// <param name="targetValue">目标字段</param>
        /// <returns>加密</returns>
        public static string DesEncrypt(string targetValue)
        {
            return DesEncrypt(targetValue, BaseSystemInfo.SecurityKey);
        }

        /// <summary>
        /// DES数据加密
        /// </summary>
        /// <param name="targetValue">目标值</param>
        /// <param name="key">密钥</param>
        /// <returns>加密值</returns>
        public static string DesEncrypt(string targetValue, string key)
        {
            if (string.IsNullOrEmpty(targetValue))
            {
                return string.Empty;
            }

            var result = new StringBuilder();
            var des = new DESCryptoServiceProvider();
            var inputByteArray = Encoding.Default.GetBytes(targetValue);
            //通过两次哈希密码设置对称算法的初始化向量   
            des.Key = Encoding.ASCII.GetBytes(Sha1(Md5(key).Substring(0, 8)).Substring(0, 8));
            //通过两次哈希密码设置算法的机密密钥   
            des.IV = Encoding.ASCII.GetBytes(Sha1(Md5(key).Substring(0, 8)).Substring(0, 8));
            var ms = new MemoryStream();
            var cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            foreach (var b in ms.ToArray())
            {
                result.AppendFormat("{0:X2}", b);
            }
            return result.ToString();
        }


        /// <summary>
        /// DES数据解密
        /// </summary>
        /// <param name="targetValue">目标字段</param>
        /// <returns>解密</returns>
        public static string DesDecrypt(string targetValue)
        {
            return DesDecrypt(targetValue, BaseSystemInfo.SecurityKey);
        }

        /// <summary>
        /// DES数据解密
        /// 20140219 吉日嘎拉 就是出错了，也不能让程序崩溃
        /// </summary>
        /// <param name="targetValue"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string DesDecrypt(string targetValue, string key)
        {
            if (string.IsNullOrEmpty(targetValue))
            {
                return string.Empty;
            }
            // 定义DES加密对象
            try
            {
                var des = new DESCryptoServiceProvider();
                var len = targetValue.Length / 2;
                var inputByteArray = new byte[len];
                int x, i;
                for (x = 0; x < len; x++)
                {
                    i = Convert.ToInt32(targetValue.Substring(x * 2, 2), 16);
                    inputByteArray[x] = (byte)i;
                }
                // 通过两次哈希密码设置对称算法的初始化向量   
                des.Key = Encoding.ASCII.GetBytes(Sha1(Md5(key).Substring(0, 8)).Substring(0, 8));
                // 通过两次哈希密码设置算法的机密密钥   
                des.IV = Encoding.ASCII.GetBytes(Sha1(Md5(key).Substring(0, 8)).Substring(0, 8));
                // 定义内存流
                var ms = new MemoryStream();
                // 定义加密流
                var cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
                return Encoding.Default.GetString(ms.ToArray());
            }
            catch(Exception ex)
            {
                LogUtil.WriteException(ex);
            }
            return string.Empty;
        }

        /// <summary>
        /// 查询匹配长度
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="cp">规则</param>
        /// <param name="s">默认值</param>
        /// <returns>匹配长度</returns>
        public static int MatcherLength(string str, string cp, string s)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return 0;
            }
            var mc = Regex.Matches(str, cp);
            return mc.Count;
        }


        /// <summary>
        /// 密码强度级别
        /// </summary>
        /// <param name="passWord">密码</param>
        /// <returns>强度级别</returns>
        public static int GetUserPassWordRate(string passWord)
        {
            /*  
             * 返回值值表示口令等级  
             * 0 不合法口令  
             * 1 太短  
             * 2 弱  
             * 3 一般  
             * 4 很好  
             * 5 极佳  
             */
            var i = 0;
            //if(pass==null || pass.length()==0)
            if (string.IsNullOrWhiteSpace(passWord))
            {
                return 0;
            }
            var hasLetter = MatcherLength(passWord, "[a-zA-Z]", "");
            var hasNumber = MatcherLength(passWord, "[0-9]", "");
            var passLen = passWord.Length;
            if (passLen >= 6)
            {
                /* 如果仅包含数字或仅包含字母 */
                if ((passLen - hasLetter) == 0 || (passLen - hasNumber) == 0)
                {
                    if (passLen < 8)
                    {
                        i = 2;
                    }
                    else
                    {
                        i = 3;
                    }
                }
                /* 如果口令大于6位且即包含数字又包含字母 */
                else if (hasLetter > 0 && hasNumber > 0)
                {
                    if (passLen >= 10)
                    {
                        i = 5;
                    }
                    else if (passLen >= 8)
                    {
                        i = 4;
                    }
                    else
                    {
                        i = 3;
                    }
                }
                /* 如果既不包含数字又不包含字母 */
                else if (hasLetter == 0 && hasNumber == 0)
                {
                    if (passLen >= 7)
                    {
                        i = 5;
                    }
                    else
                    {
                        i = 4;
                    }
                }
                /* 字母或数字有一方为0 */
                else if (hasNumber == 0 || hasLetter == 0)
                {
                    if ((passLen - hasLetter) == 0 || (passLen - hasNumber) == 0)
                    {
                        i = 2;
                    }
                    /*   
                     * 字母数字任意一种类型小于6且总长度大于等于6  
                     * 则说明此密码是字母或数字加任意其他字符组合而成  
                     */
                    else
                    {
                        if (passLen > 8)
                        {
                            i = 5;
                        }
                        else if (passLen == 8)
                        {
                            i = 4;
                        }
                        else
                        {
                            i = 3;
                        }
                    }
                }
            }
            else
            { 
                //口令小于6位则显示太短  
                if (passLen > 0)
                {
                    i = 1; //口令太短  
                }
                else
                {
                    i = 0;
                }
            }
            return i;
        }
    }
}