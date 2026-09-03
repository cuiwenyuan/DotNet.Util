//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2026, DotNet.
//-----------------------------------------------------------------

using System;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;


namespace DotNet.Util
{
    /// <summary>
    /// SecretUtil
    /// </summary>
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
            if (commandText.IsNullOrEmpty())
            {
                return true;
            }

            var trimmed = commandText.Trim();
            if (trimmed.Length == 0)
            {
                return true;
            }

            var unsafeWords = new[] { "DELETE", "INSERT", "UPDATE", "TRUNCATE", "DROP", "ALTER", "EXEC", "EXECUTE" };
            foreach (var word in unsafeWords)
            {
                if (trimmed.IndexOf(word, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return false;
                }
            }

            // 进一步封堵常见注入模式
            return trimmed.IndexOf(";", StringComparison.Ordinal) < 0
                   && trimmed.IndexOf("--", StringComparison.Ordinal) < 0
                   && trimmed.IndexOf("/*", StringComparison.Ordinal) < 0
                   && trimmed.IndexOf("*/", StringComparison.Ordinal) < 0;
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

            var byteConverter = Encoding.UTF8;
            var buffer = byteConverter.GetBytes(dataToSign);
            try
            {
                using var cryptoServiceProvider = new RSACryptoServiceProvider();
                cryptoServiceProvider.ImportCspBlob(Convert.FromBase64String(privateKey));
                using var sha256 = SHA256.Create();
                var signedData = cryptoServiceProvider.SignData(buffer, sha256);
                result = Convert.ToBase64String(signedData);
            }
            catch (CryptographicException e)
            {
#if (DEBUG)
                Console.WriteLine(e.Message);
#endif
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
            var byteConverter = Encoding.UTF8;
            var buffer = byteConverter.GetBytes(dataToVerify);
            try
            {
                using var cryptoServiceProvider = new RSACryptoServiceProvider();
                cryptoServiceProvider.ImportCspBlob(Convert.FromBase64String(publicKey));
                using (var sha256 = SHA256.Create())
                {
                    result = cryptoServiceProvider.VerifyData(buffer, sha256, signedData);
                }
                if (!result)
                {
                    // 兼容旧版SHA-1签名的验证（仅验签，不再签发）
                    using var sha1 = new SHA1CryptoServiceProvider();
                    result = cryptoServiceProvider.VerifyData(buffer, sha1, signedData);
                }
            }
            catch (CryptographicException e)
            {
#if (DEBUG)
                Console.WriteLine(e.Message);
#endif
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
            // 移除已失效的注册时间限制（原逻辑在 2020-08 之后恒为 false，导致注册校验永久失败）。
            // 当前注册码校验逻辑已被注释禁用，CheckRegister 默认返回 true。
            // 一定要检查注册码,否则这个软件到处别人复制,我的基类也得不到保障了,这是我的心血,得会珍惜自己的劳动成果.
            // 2007.04.14 JiRiGaLa 改进注册方式,让底层程序更安全一些
            //if (BaseConfiguration.Instance.RegisterKey.Equals(CodeChange(BaseConfiguration.Instance.Database + BaseConfiguration.Instance.CustomerCompanyName)))
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
        /// <param name="length">散列长度，仅支持 16 位或 32 位</param>
        /// <returns>加密密码</returns>
        /// <exception cref="ArgumentOutOfRangeException">当 length 不是 16 或 32 时抛出</exception>
        public static string Md5(string password, int length)
        {
            // 修复：原实现仅当 length==16 时截断，其余任意值（8/20/0/64/负数）一律静默返回 32 位，
            // 不抛异常也不告警，调用方按参数期望的长度做截断或比对会静默出错。
            // 改为显式校验参数，非法值快速失败（与 DateUtil.GetDaysOfMonth 的处理方式一致）。
            if (length != 16 && length != 32)
            {
                throw new ArgumentOutOfRangeException(nameof(length), length, "Md5 散列长度仅支持 16 位或 32 位。");
            }

            var result = string.Empty;
            if (!password.IsNullOrEmpty())
            {
                //32位加密
                #region 方法1 .NET 4.5中已经废弃不用的API
                //result = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(password, "MD5").ToLower();
                #endregion

                #region 方法2
                //MD5CryptoServiceProvider md5Hasher = new MD5CryptoServiceProvider();
                //byte[] data = md5Hasher.ComputeHash(new UTF8Encoding().GetBytes(password));
                //var sb = PoolUtil.StringBuilder.Get();
                //foreach (var t in data)
                //{
                //    sb.Append(t.ToString("x2"));
                //}
                //result = sb.Return();
                #endregion

                #region 方法3
                //1.创建一个MD5对象
                var md5 = MD5.Create();
                //2.把字符串变一个byte[]
                var buffer = Encoding.UTF8.GetBytes(password);
                //3.将一个byte[]通过MD5计算到一个新的byte[]，新的byte[]就是计算md5后的结果。
                var md5Buffer = md5.ComputeHash(buffer);
                //释放资源
                md5.Clear();
                //4.将计算后的结果直接显示为字符串
                var sb = PoolUtil.StringBuilder.Get();
                foreach (var t in md5Buffer)
                {
                    //x2:把每个数字转换为16进制，并保留两位数字。
                    sb.Append(t.ToString("x2"));
                }
                result = sb.Return();
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
            if (!password.IsNullOrEmpty())
            {
                #region 方法3
                //1.创建一个MD5对象
                var sha1 = SHA1.Create();
                //2.把字符串变一个byte[]
                var buffer = Encoding.UTF8.GetBytes(password);
                //3.将一个byte[]通过SHA1计算到一个新的byte[]，新的byte[]就是计算SHA1后的结果。
                var sha1Buffer = sha1.ComputeHash(buffer);
                //释放资源
                sha1.Clear();
                //sha1.Dispose();//释放当前实例使用的所有资源
                //4.将计算后的结果直接显示为字符串
                var sb = PoolUtil.StringBuilder.Get();
                foreach (var t in sha1Buffer)
                {
                    //x2:把每个数字转换为16进制，并保留两位数字。
                    sb.Append(t.ToString("x2"));
                }
                result = sb.Return();
                #endregion
            }
            return result;
        }

        #region R9-1 用户口令哈希（PBKDF2-HMAC-SHA256，替代无盐/快速 MD5/SHA1）
        /// <summary>
        /// 口令哈希格式前缀，用于区分新旧哈希方案（R9-1）。
        /// 新格式示例：pbkdf2$100000$&lt;saltBase64&gt;$&lt;hashBase64&gt;
        /// </summary>
        public const string PasswordHashPrefix = "pbkdf2$";

        /// <summary>
        /// PBKDF2 默认迭代次数（R9-1）。
        /// 100k 迭代 SHA-256 在 .NET Framework 旧服务器上单次校验约数毫秒，远优于可离线暴破的 MD5。
        /// 迭代次数已写入哈希字符串，未来提高默认值无需对已落库哈希做迁移。
        /// </summary>
        private const int DefaultPbkdf2Iterations = 100000;

        /// <summary>
        /// PBKDF2 盐长度（字节，R9-1）。16 字节 = 128 bit，符合 NIST 建议。
        /// </summary>
        private const int Pbkdf2SaltBytes = 16;

        /// <summary>
        /// 计算用户口令的 PBKDF2-HMAC-SHA256 哈希（R9-1 修复：无盐/快速 MD5 改为加盐慢哈希）。
        /// 采用手写 PBKDF2 以保证 net46 / netstandard2.0 / net8+ 全 TFM 可用且统一为 SHA-256（非弱 SHA-1）。
        /// </summary>
        /// <param name="password">原始口令（不可为 null）</param>
        /// <param name="iterations">迭代次数，默认 <see cref="DefaultPbkdf2Iterations"/></param>
        /// <returns>格式 pbkdf2$iters$&lt;saltBase64&gt;$&lt;hashBase64&gt; 的哈希字符串</returns>
        /// <exception cref="ArgumentNullException">password 为 null</exception>
        public static string HashPassword(string password, int iterations = DefaultPbkdf2Iterations)
        {
            if (password == null)
            {
                throw new ArgumentNullException(nameof(password));
            }
            if (iterations <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(iterations), iterations, "迭代次数必须为正整数。");
            }

            var salt = new byte[Pbkdf2SaltBytes];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            var hash = Pbkdf2DeriveBytes(password, salt, iterations);
            return string.Concat(PasswordHashPrefix, iterations.ToString(CultureInfo.InvariantCulture), "$",
                Convert.ToBase64String(salt), "$", Convert.ToBase64String(hash));
        }

        /// <summary>
        /// 校验口令与已存储哈希是否匹配（R9-1）。
        /// 仅处理新格式（以 <see cref="PasswordHashPrefix"/> 开头）；老 MD5 格式请走 BaseUserManager.VerifyUserPassword 的兼容分支。
        /// 采用常数时间比较，避免计时侧信道。
        /// </summary>
        /// <param name="password">待校验的原始口令</param>
        /// <param name="storedHash">存储的哈希字符串（HashPassword 产出）</param>
        /// <returns>匹配返回 true；参数非法、格式不符或口令错误返回 false</returns>
        public static bool VerifyPassword(string password, string storedHash)
        {
            if (password == null || storedHash == null)
            {
                return false;
            }

            // 期望 4 段：pbkdf2 / iters / saltBase64 / hashBase64
            var parts = storedHash.Split('$');
            if (parts.Length != 4)
            {
                return false;
            }
            if (!string.Equals(parts[0], "pbkdf2", StringComparison.Ordinal))
            {
                return false;
            }
            if (!int.TryParse(parts[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out var iterations) || iterations <= 0)
            {
                return false;
            }

            byte[] salt;
            byte[] expected;
            try
            {
                salt = Convert.FromBase64String(parts[2]);
                expected = Convert.FromBase64String(parts[3]);
            }
            catch (FormatException)
            {
                return false;
            }

            var actual = Pbkdf2DeriveBytes(password, salt, iterations, expected.Length);
            if (actual.Length != expected.Length)
            {
                return false;
            }

            // 常数时间比较
            var diff = 0;
            for (var i = 0; i < actual.Length; i++)
            {
                diff |= actual[i] ^ expected[i];
            }
            return diff == 0;
        }

        /// <summary>
        /// 手写 PBKDF2-HMAC-SHA256（R9-1）。dkLen 默认 32 字节（单块，恰为 SHA-256 输出长度）。
        /// 算法：DK = T1 || T2 || ... ，其中 Ti = U1 ^ U2 ^ ... ^ Uc，
        /// U1 = HMAC(password, salt || INT_32_BE(i))，Uc = HMAC(password, U{c-1})。
        /// </summary>
        private static byte[] Pbkdf2DeriveBytes(string password, byte[] salt, int iterations, int dkLen = 32)
        {
            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(password)))
            {
                const int hLen = 32; // SHA-256
                var blocks = (dkLen + hLen - 1) / hLen;
                var output = new byte[blocks * hLen];
                var salted = new byte[salt.Length + 4];
                Buffer.BlockCopy(salt, 0, salted, 0, salt.Length);

                for (var block = 1; block <= blocks; block++)
                {
                    // salted = salt || INT_32_BE(block)
                    salted[salt.Length] = (byte)(block >> 24);
                    salted[salt.Length + 1] = (byte)(block >> 16);
                    salted[salt.Length + 2] = (byte)(block >> 8);
                    salted[salt.Length + 3] = (byte)block;

                    var u = hmac.ComputeHash(salted);
                    var offset = (block - 1) * hLen;
                    Buffer.BlockCopy(u, 0, output, offset, hLen);

                    for (var c = 2; c <= iterations; c++)
                    {
                        u = hmac.ComputeHash(u);
                        for (var j = 0; j < hLen; j++)
                        {
                            output[offset + j] ^= u[j];
                        }
                    }
                }

                if (dkLen == output.Length)
                {
                    return output;
                }
                var truncated = new byte[dkLen];
                Buffer.BlockCopy(output, 0, truncated, 0, dkLen);
                return truncated;
            }
        }
        #endregion

        /// <summary>
        /// Base64编码
        /// </summary>
        /// <param name="codeType"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public static string EncodeBase64(string codeType, string code)
        {
            // R9-14：移除 try/catch 静默吞异常，改为正确失败（无效编码/输入将抛出明确异常，
            // 而非静默返回原值，避免调用方误判编解码成功）
            var bytes = Encoding.GetEncoding(codeType).GetBytes(code);
            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// Base64解码
        /// </summary>
        /// <param name="codeType"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public static string DecodeBase64(string codeType, string code)
        {
            // R9-14：移除 try/catch 静默吞异常，改为正确失败（非法 Base64/编码将抛出明确异常，
            // 而非静默返回原值，避免调用方误判解码成功而使用损坏数据）
            var bytes = Convert.FromBase64String(code);
            return Encoding.GetEncoding(codeType).GetString(bytes);
        }

        /// <summary>
        /// AES数据加密（安全替代 DES，供新代码使用）
        /// <para>输出格式：Base64(16字节随机IV + 密文)，密钥由 SHA256(key) 派生为 32 字节</para>
        /// </summary>
        /// <param name="targetValue">目标字段</param>
        /// <returns>加密值</returns>
        public static string AesEncrypt(string targetValue)
        {
            return AesEncrypt(targetValue, BaseSystemInfo.SecurityKey);
        }

        /// <summary>
        /// AES数据加密
        /// </summary>
        /// <param name="targetValue">目标值</param>
        /// <param name="key">密钥</param>
        /// <returns>加密值</returns>
        public static string AesEncrypt(string targetValue, string key)
        {
            if (targetValue.IsNullOrEmpty())
            {
                return string.Empty;
            }
            if (key.IsNullOrEmpty())
            {
                key = BaseSystemInfo.SecurityKey;
            }

            using var aes = Aes.Create();
            aes.Key = AesKey(key);
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            aes.GenerateIV(); // 每次加密使用随机 IV，前置到密文，避免 IV 复用
            byte[] cipherWithIv;
            using (var ms = new MemoryStream())
            {
                ms.Write(aes.IV, 0, aes.IV.Length);
                using (var cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                using (var sw = new StreamWriter(cs, Encoding.UTF8))
                {
                    sw.Write(targetValue);
                }
                cipherWithIv = ms.ToArray(); // 结构：IV[16] + 密文
            }

            // R9-10：Encrypt-then-MAC，追加 HMAC-SHA256(IV+密文) 提供完整性/防篡改保护
            // 输出格式：Base64( IV[16] || 密文 || HMAC-SHA256[32] )
            using var hmac = new HMACSHA256(AesMacKey(key));
            var mac = hmac.ComputeHash(cipherWithIv);
            var output = new byte[cipherWithIv.Length + mac.Length];
            Buffer.BlockCopy(cipherWithIv, 0, output, 0, cipherWithIv.Length);
            Buffer.BlockCopy(mac, 0, output, cipherWithIv.Length, mac.Length);
            return Convert.ToBase64String(output);
        }

        /// <summary>
        /// AES数据解密（与 AesEncrypt 配套）
        /// </summary>
        /// <param name="targetValue">目标字段</param>
        /// <returns>解密值；解密失败返回空串</returns>
        public static string AesDecrypt(string targetValue)
        {
            return AesDecrypt(targetValue, BaseSystemInfo.SecurityKey);
        }

        /// <summary>
        /// AES数据解密
        /// </summary>
        /// <param name="targetValue">目标值</param>
        /// <param name="key">密钥</param>
        /// <returns>解密值；解密失败返回空串</returns>
        public static string AesDecrypt(string targetValue, string key)
        {
            if (targetValue.IsNullOrEmpty())
            {
                return string.Empty;
            }
            if (key.IsNullOrEmpty())
            {
                key = BaseSystemInfo.SecurityKey;
            }

            try
            {
                var fullCipher = Convert.FromBase64String(targetValue);
                if (fullCipher.Length <= AesBlockSize)
                {
                    return string.Empty; // 连 IV 都不够
                }

                // R9-10：新格式在 IV+密文 之后追加了 HMAC-SHA256(32字节)。
                // 通过校验 HMAC 判断是否为新格式，从而向后兼容历史无 MAC 的 AES 密文。
                byte[] iv;
                byte[] cipherText;
                if (fullCipher.Length >= AesBlockSize + AesMacSize + AesBlockSize)
                {
                    // 长度足以容纳 IV + 至少 1 个分组密文 + HMAC：尝试按新格式校验
                    var macKey = AesMacKey(key);
                    var expectedMac = new byte[AesMacSize];
                    Array.Copy(fullCipher, fullCipher.Length - AesMacSize, expectedMac, 0, AesMacSize);
                    using var hmac = new HMACSHA256(macKey);
                    var computedMac = hmac.ComputeHash(fullCipher, 0, fullCipher.Length - AesMacSize);
                    if (FixedTimeEquals(computedMac, expectedMac))
                    {
                        // 新格式：IV(前16) + 密文(中间) + HMAC(末尾32)
                        iv = new byte[AesBlockSize];
                        Array.Copy(fullCipher, 0, iv, 0, AesBlockSize);
                        cipherText = new byte[fullCipher.Length - AesBlockSize - AesMacSize];
                        Array.Copy(fullCipher, AesBlockSize, cipherText, 0, cipherText.Length);
                    }
                    else
                    {
                        // HMAC 不匹配：视为历史无 MAC 格式（或密文被篡改），按 IV+密文 处理
                        iv = new byte[AesBlockSize];
                        Array.Copy(fullCipher, 0, iv, 0, AesBlockSize);
                        cipherText = new byte[fullCipher.Length - AesBlockSize];
                        Array.Copy(fullCipher, AesBlockSize, cipherText, 0, cipherText.Length);
                    }
                }
                else
                {
                    // 长度不足，无法容纳 HMAC，按历史格式（IV + 密文）处理
                    iv = new byte[AesBlockSize];
                    Array.Copy(fullCipher, 0, iv, 0, AesBlockSize);
                    cipherText = new byte[fullCipher.Length - AesBlockSize];
                    Array.Copy(fullCipher, AesBlockSize, cipherText, 0, cipherText.Length);
                }

                using var aes = Aes.Create();
                aes.Key = AesKey(key);
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                aes.IV = iv;
                using var ms = new MemoryStream(cipherText);
                using var cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Read);
                using var sr = new StreamReader(cs, Encoding.UTF8);
                return sr.ReadToEnd();
            }
            catch
            {
                // 解密失败（含密文被篡改导致 PKCS7 校验失败）不抛异常，与 DesDecrypt 行为保持一致
                return string.Empty;
            }
        }

        /// <summary>
        /// SHA256 摘要，用于从密钥派生 AES 密钥
        /// </summary>
        private static byte[] Sha256Bytes(string key)
        {
            using var sha = SHA256.Create();
            return sha.ComputeHash(Encoding.UTF8.GetBytes(key));
        }

        // R9-10：AES-CBC 完整性保护相关常量与辅助方法（Encrypt-then-MAC）
        private const int AesBlockSize = 16;   // AES 分组长度（字节）
        private const int AesMacSize = 32;     // HMAC-SHA256 输出长度（字节）

        /// <summary>
        /// 从主密钥派生 32 字节 AES 密钥（SHA256）
        /// </summary>
        private static byte[] AesKey(string key)
        {
            return Sha256Bytes(key);
        }

        /// <summary>
        /// 从主密钥派生独立的 HMAC 密钥（与 AES 密钥分离，遵循加密/认证密钥分离原则）
        /// </summary>
        private static byte[] AesMacKey(string key)
        {
            using var hmac = new HMACSHA256(Sha256Bytes(key));
            return hmac.ComputeHash(Encoding.UTF8.GetBytes("DotNet.Util.AesHmac.v1"));
        }

        /// <summary>
        /// 常数时间字节比较，避免 HMAC 校验引入计时侧信道
        /// </summary>
        private static bool FixedTimeEquals(byte[] a, byte[] b)
        {
            if (a == null || b == null || a.Length != b.Length)
            {
                return false;
            }
            var diff = 0;
            for (var i = 0; i < a.Length; i++)
            {
                diff |= a[i] ^ b[i];
            }
            return diff == 0;
        }

        /// <summary>
        /// DES数据加密
        /// <para>警告：DES（56位密钥）已被证明不安全，此方法仅为兼容历史持久化数据保留；新代码请使用 AesEncrypt</para>
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
            if (targetValue.IsNullOrEmpty())
            {
                return string.Empty;
            }
            //修复：key 为空时 Md5 返回空串，Substring(0,8) 会越界
            if (key.IsNullOrEmpty())
            {
                key = BaseSystemInfo.SecurityKey;
            }

            var sb = PoolUtil.StringBuilder.Get();
            //修复：使用 using 释放 DES/MemoryStream/CryptoStream
            using var des = new DESCryptoServiceProvider();
            //修复：显式 UTF-8（原 Encoding.Default 依赖系统代码页，Framework=ANSI/GBK、Core=UTF-8，跨运行时密文不一致）
            var inputByteArray = Encoding.UTF8.GetBytes(targetValue);
            //通过两次哈希密码设置对称算法的初始化向量
            var keyHash = Sha1(Md5(key).Substring(0, 8));
            //通过两次哈希密码设置算法的机密密钥
            des.Key = Encoding.ASCII.GetBytes(keyHash.Substring(0, 8));
            //使用密钥散列的另一部分作为初始化向量，避免Key与IV相同
            des.IV = Encoding.ASCII.GetBytes(keyHash.Substring(8, 8));
            using var ms = new MemoryStream();
            using (var cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write))
            {
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
            }
            foreach (var b in ms.ToArray())
            {
                sb.AppendFormat("{0:X2}", b);
            }
            return sb.Return();
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
        /// <para>警告：DES（56位密钥）已被证明不安全，此方法仅为兼容历史持久化数据保留；新代码请使用 AesDecrypt</para>
        /// 20140219 吉日嘎拉 就是出错了，也不能让程序崩溃
        /// </summary>
        /// <param name="targetValue"></param>
        /// <param name="key">键</param>
        /// <returns></returns>
        public static string DesDecrypt(string targetValue, string key)
        {
            if (targetValue.IsNullOrEmpty())
            {
                return string.Empty;
            }
            //修复：key 为空时 Md5 返回空串，Substring(0,8) 会越界
            if (key.IsNullOrEmpty())
            {
                key = BaseSystemInfo.SecurityKey;
            }
            // 定义DES加密对象
            try
            {
                using var des = new DESCryptoServiceProvider();
                var len = targetValue.Length / 2;
                var inputByteArray = new byte[len];
                int x, i;
                for (x = 0; x < len; x++)
                {
                    //修复：原 (substring, 16).ToInt() 是元组字面量，ToInt 恒返回 0，导致解密永远失败；
                    //应把十六进制子串按 16 进制转换为 int
                    i = Convert.ToInt32(targetValue.Substring(x * 2, 2), 16);
                    inputByteArray[x] = (byte)i;
                }
                // 通过两次哈希密码设置对称算法的初始化向量
                var keyHash = Sha1(Md5(key).Substring(0, 8));
                // 通过两次哈希密码设置算法的机密密钥
                des.Key = Encoding.ASCII.GetBytes(keyHash.Substring(0, 8));
                // 使用密钥散列的另一部分作为初始化向量，避免Key与IV相同
                des.IV = Encoding.ASCII.GetBytes(keyHash.Substring(8, 8));
                // 定义内存流
                using var ms = new MemoryStream();
                // 定义加密流
                using (var cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(inputByteArray, 0, inputByteArray.Length);
                    cs.FlushFinalBlock();
                }
                //修复：先按 UTF-8 严格解码（新密文），失败回退 GBK（旧密文为 .NET Framework 时代 Encoding.Default=GBK 加密）
                var plainBytes = ms.ToArray();
                try
                {
                    var utf8Strict = new UTF8Encoding(false, true); // throwOnInvalidBytes
                    return utf8Strict.GetString(plainBytes);
                }
                catch (DecoderFallbackException)
                {
                    return Utils.GbkEncoding.GetString(plainBytes);
                }
            }
            catch (Exception ex)
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
