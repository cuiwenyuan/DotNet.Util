using System;
using DotNet.Business;
using DotNet.Util;
using Xunit;

namespace DotNet.Util.Tests.BaseUser
{
    /// <summary>
    /// R9-1 口令哈希迁移双路径校验测试（新 PBKDF2 / 老 MD5 兼容）。
    /// 仅验证纯逻辑（不触达数据库）：BaseUserManager.VerifyUserPassword 内部依赖
    /// EncryptUserPassword（MD5）与 SecretUtil.VerifyPassword（PBKDF2），均无需 DbHelper。
    /// </summary>
    public class BaseUserManagerPasswordTests
    {
        private static BaseUserManager CreateManager() => new BaseUserManager();

        [Fact]
        public void VerifyUserPassword_NewFormat_Matches()
        {
            var mgr = CreateManager();
            var stored = SecretUtil.HashPassword("P@ssw0rd");
            var ok = mgr.VerifyUserPassword("P@ssw0rd", stored, null, out var isLegacy);
            Assert.True(ok);
            Assert.False(isLegacy);
        }

        [Fact]
        public void VerifyUserPassword_LegacyUnsalted_Matches()
        {
            // 老格式：无盐 MD5（历史库中存在）
            var mgr = CreateManager();
            var stored = SecretUtil.Md5("P@ssw0rd", 32).ToUpper();
            var ok = mgr.VerifyUserPassword("P@ssw0rd", stored, null, out var isLegacy);
            Assert.True(ok);
            Assert.True(isLegacy);
        }

        [Fact]
        public void VerifyUserPassword_LegacySalted_Matches()
        {
            // 老格式：有盐 MD5（EncryptUserPassword 三重编织）
            var mgr = CreateManager();
            var salt = RandomUtil.GetString(20);
            var stored = mgr.EncryptUserPassword("P@ssw0rd", salt);
            var ok = mgr.VerifyUserPassword("P@ssw0rd", stored, salt, out var isLegacy);
            Assert.True(ok);
            Assert.True(isLegacy);
        }

        [Fact]
        public void VerifyUserPassword_WrongPassword_NoMatch()
        {
            var mgr = CreateManager();
            var newStored = SecretUtil.HashPassword("P@ssw0rd");
            Assert.False(mgr.VerifyUserPassword("wrong", newStored, null, out _));

            var legacyStored = SecretUtil.Md5("P@ssw0rd", 32).ToUpper();
            Assert.False(mgr.VerifyUserPassword("wrong", legacyStored, null, out _));
        }

        [Fact]
        public void VerifyUserPassword_NullStoredHash_EmptyInputMatches()
        {
            var mgr = CreateManager();
            // 库里无密码、输入为空 → 视为匹配；输入非空 → 不匹配
            Assert.True(mgr.VerifyUserPassword("", null, null, out var isLegacy1));
            Assert.False(isLegacy1);
            Assert.False(mgr.VerifyUserPassword("x", null, null, out _));
        }

        [Fact]
        public void SetPassword_ProducesNewFormat_WhenServerEncryptEnabled()
        {
            // 正向确认：新密码经 HashPassword 后是 pbkdf2$ 前缀（迁移后落库格式）
            var hash = SecretUtil.HashPassword("NewP@ss");
            Assert.StartsWith(SecretUtil.PasswordHashPrefix, hash);
        }
    }
}
