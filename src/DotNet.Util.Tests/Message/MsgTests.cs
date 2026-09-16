using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using DotNet.Util;
using Xunit;

namespace DotNet.Util.Tests.Message
{
    /// <summary>
    /// Msg 多语言消息层测试（P0 基础设施）
    /// </summary>
    [Collection(MsgTestCollection.Name)]
    public class MsgTests : IDisposable
    {
        public void Dispose()
        {
            // 复位全局静态状态，避免污染其它测试
            Msg.Clear();
        }

        #region 默认语言与取值

        [Fact]
        public void CurrentLanguage_DefaultsToZhCn()
        {
            Msg.Clear();
            Assert.Equal("zh-CN", Msg.CurrentLanguage);
        }

        [Fact]
        public void Get_DefaultLanguage_ReturnsChinese()
        {
            Msg.Clear();
            Assert.Equal("发生未知错误。", Msg.Get("Common.UnknownError"));
        }

        [Fact]
        public void Get_WhenLanguageIsEn_ReturnsEnglish()
        {
            Msg.Clear();
            Msg.CurrentLanguage = "en";
            Assert.Equal("An unknown error occurred.", Msg.Get("Common.UnknownError"));
        }

        [Fact]
        public void Get_WithExplicitCulture_IgnoresCurrentLanguage()
        {
            Msg.Clear();
            Msg.CurrentLanguage = "en";
            Assert.Equal("发生未知错误。", Msg.Get("Common.UnknownError", "zh-CN"));
            Assert.Equal("An unknown error occurred.", Msg.Get("Common.UnknownError", "en"));
        }

        #endregion

        #region 回退链

        [Fact]
        public void Get_NeutralCultureFallback_EnUsFallsBackToEn()
        {
            Msg.Clear();
            // 未注册 en-US，应回退到中性语言 en
            Assert.Equal("An unknown error occurred.", Msg.Get("Common.UnknownError", "en-US"));
        }

        [Fact]
        public void Get_UnknownLanguage_FallsBackToChinese()
        {
            Msg.Clear();
            // 法语未注册，应回退到默认中文
            Assert.Equal("发生未知错误。", Msg.Get("Common.UnknownError", "fr-FR"));
        }

        [Fact]
        public void Get_MissingKey_ReturnsKeyItself()
        {
            Msg.Clear();
            const string missingKey = "Common.ThisKeyDoesNotExist";
            Assert.Equal(missingKey, Msg.Get(missingKey));
            Assert.Equal(missingKey, Msg.Get(missingKey, "en"));
        }

        [Fact]
        public void Get_NullOrEmptyKey_ReturnsEmpty()
        {
            Msg.Clear();
            Assert.Equal(string.Empty, Msg.Get(null));
            Assert.Equal(string.Empty, Msg.Get(string.Empty));
        }

        #endregion

        #region 占位符格式化

        [Fact]
        public void Format_SubstitutesPlaceholder()
        {
            Msg.Clear();
            Assert.Equal("请输入用户名，不允许为空。", Msg.Format("Common.ParameterRequired", "用户名"));

            Msg.CurrentLanguage = "en";
            Assert.Equal("Please enter UserName; it cannot be empty.",
                Msg.Format("Common.ParameterRequired", "UserName"));
        }

        [Fact]
        public void Format_WithoutArgs_ReturnsRawText()
        {
            Msg.Clear();
            // 未提供参数时返回含占位符的原文，不抛异常
            Assert.Equal("请输入{0}，不允许为空。", Msg.Format("Common.ParameterRequired"));
        }

        #endregion

        #region 语言包注册与覆盖

        [Fact]
        public void Register_MergesIntoExistingPack()
        {
            Msg.Clear();
            Msg.Register("en", new Dictionary<string, string>
            {
                { "Common.UnknownError", "Overridden." }
            });

            Msg.CurrentLanguage = "en";
            Assert.Equal("Overridden.", Msg.Get("Common.UnknownError"));
            // 未覆盖的键仍取内置值
            Assert.Equal("The operation succeeded.", Msg.Get("Common.Success"));
        }

        [Fact]
        public void LoadJsonOverride_OverridesBuiltin()
        {
            Msg.Clear();
            var path = Path.Combine(Path.GetTempPath(), "dotnet.util.msg.override.test.json");
            File.WriteAllText(path, "{\"Common.UnknownError\":\"Custom error.\"}");

            try
            {
                Msg.LoadJsonOverride("en", path);
                Msg.CurrentLanguage = "en";
                Assert.Equal("Custom error.", Msg.Get("Common.UnknownError"));
            }
            finally
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
        }

        [Fact]
        public void LoadJsonOverride_MissingFile_IsIgnored()
        {
            Msg.Clear();
            Msg.LoadJsonOverride("en", Path.Combine(Path.GetTempPath(), "no.such.file.json"));

            Msg.CurrentLanguage = "en";
            Assert.Equal("An unknown error occurred.", Msg.Get("Common.UnknownError"));
        }

        #endregion

        #region 语言包一致性

        [Fact]
        public void Clear_ReloadsBuiltinPacks()
        {
            Msg.Clear();
            Msg.CurrentLanguage = "en";
            Assert.Equal("An unknown error occurred.", Msg.Get("Common.UnknownError"));
        }

        [Fact]
        public void AllPacks_HaveIdenticalKeySet()
        {
            Msg.Clear();

            var zhKeys = new List<string>(Msg.GetKeys("zh-CN"));
            var enKeys = new List<string>(Msg.GetKeys("en"));

            Assert.NotEmpty(zhKeys);
            Assert.Equal(zhKeys.Count, enKeys.Count);

            for (var i = 0; i < zhKeys.Count; i++)
            {
                Assert.Equal(zhKeys[i], enKeys[i]);
            }
        }

        #endregion

        #region AppMessage 词条（P1）

        [Fact]
        public void ZhCnPack_MatchesAppMessageFields()
        {
            Msg.Clear();

            // 语言包键已于 2026-09-16 由 Msg#### 编号改为语义键（Common.* / Logon.* …），
            // 而 AppMessage.MsgXXXX 字段名保持不变（二进制兼容），因此不能再用字段名查语言包。
            // 改为校验：每个 AppMessage 字段的中文取值，都必须在中文语言包里存在。
            // 若字段值被修改而未同步语言包，此用例会失败。
            var packValues = new HashSet<string>();
            foreach (var key in Msg.GetKeys("zh-CN"))
            {
                packValues.Add(Msg.Get(key));
            }

            var fields = typeof(AppMessage).GetFields(BindingFlags.Public | BindingFlags.Static);
            var count = 0;
            foreach (var field in fields)
            {
                if (field.FieldType != typeof(string) || !field.Name.StartsWith("Msg", StringComparison.Ordinal))
                {
                    continue;
                }

                var expected = (string)field.GetValue(null);
                Assert.True(packValues.Contains(expected),
                    "AppMessage." + field.Name + " 的值「" + expected + "」未同步到中文语言包。");
                count++;
            }

            // 240 个数字编号字段 + 8 个历史遗留命名（Msgc023/Msgs857/MsgReLogon 等）
            Assert.Equal(248, count);
        }

        [Fact]
        public void AppMessageKeys_AreFullyTranslated()
        {
            Msg.Clear();

            var zhKeys = new List<string>(Msg.GetKeys("zh-CN"));
            var enKeys = new List<string>(Msg.GetKeys("en"));

            // 键命名体系（2026-09-16 改造后）：11 个业务前缀 + 基础设施前缀
            // Common 74 / Logon 55 / Validation 35 / Confirm 30 / Result 21 / Ip 11
            // System 8 / Org 7 / Workflow 6 / Sequence 5 / Sign 5 / File 3 = 260
            // + Enum 70 + Service 33 + Log 12 + Exception 11 + Business 6 + Console 4 + Rmb 2 + Qqwry 1 + Sms 1 = 140
            // 合计 400（原 406 键中 6 个因与已有语义键同值而被合并删除）
            Assert.Equal(400, zhKeys.Count);
            Assert.Equal(zhKeys.Count, enKeys.Count);
            // 编号键已全部消失
            Assert.Equal(0, zhKeys.Count(k => k.StartsWith("Msg", StringComparison.Ordinal)));
            Assert.Equal(70, zhKeys.Count(k => k.StartsWith(Msg.EnumKeyPrefix, StringComparison.Ordinal)));
            Assert.Equal(12, zhKeys.Count(k => k.StartsWith("Log.", StringComparison.Ordinal)));
            Assert.Equal(4, zhKeys.Count(k => k.StartsWith("Console.", StringComparison.Ordinal)));
            Assert.Equal(33, zhKeys.Count(k => k.StartsWith("Service.", StringComparison.Ordinal)));
            // 每个键都必须含语义前缀，杜绝再次出现无含义编号
            Assert.Equal(zhKeys.Count, zhKeys.Count(k => k.IndexOf('.') > 0));
        }

        [Fact]
        public void Get_AppMessageKey_SwitchesLanguage()
        {
            Msg.Clear();

            Assert.Equal("提示信息", Msg.Get("Common.Prompt"));
            Assert.Equal("发生未知错误。", Msg.Get("Common.UnknownError"));

            Msg.CurrentLanguage = "en";
            Assert.Equal("Information", Msg.Get("Common.Prompt"));
            Assert.Equal("An unknown error occurred.", Msg.Get("Common.UnknownError"));
        }

        [Fact]
        public void Format_AppMessageKey_SwitchesLanguage()
        {
            Msg.Clear();

            // Common.ParameterRequired = "请输入{0}，不允许为空。"，Common.OldPassword = "原密码"
            Assert.Equal("请输入原密码，不允许为空。", Msg.Format("Common.ParameterRequired", Msg.Get("Common.OldPassword")));

            Msg.CurrentLanguage = "en";
            Assert.Equal("Please enter Current password; it cannot be empty.",
                Msg.Format("Common.ParameterRequired", Msg.Get("Common.OldPassword")));
        }

        #endregion

        #region 枚举描述（P2）

        /// <summary>
        /// 模拟第三方/业务方枚举：语言包中没有对应词条，必须回退到特性原文。
        /// </summary>
        private enum UnregisteredEnum
        {
            [EnumDescription("Third party text")]
            First
        }

        [Fact]
        public void EnumDescription_ZhCn_ReturnsAttributeText()
        {
            Msg.Clear();
            Assert.Equal("运行成功", Status.Ok.ToDescription());
            Assert.Equal("数据库连接错误", Status.DbError.ToDescription());
            Assert.Equal("待审", AuditStatus.WaitForAudit.ToDescription());
        }

        [Fact]
        public void EnumDescription_En_ReturnsEnglish()
        {
            Msg.Clear();
            Msg.CurrentLanguage = "en";

            Assert.Equal("Operation succeeded", Status.Ok.ToDescription());
            Assert.Equal("Database connection error", Status.DbError.ToDescription());
            Assert.Equal("Pending", AuditStatus.WaitForAudit.ToDescription());
        }

        [Fact]
        public void EnumDescription_UnregisteredEnum_FallsBackToAttributeText()
        {
            Msg.Clear();
            Msg.CurrentLanguage = "en";

            // 未登记词条的枚举回退到 [EnumDescription] 原文，绝不返回键名 Enum.UnregisteredEnum.First
            Assert.Equal("Third party text", UnregisteredEnum.First.ToDescription());
        }

        [Fact]
        public void ZhCnEnumPack_MatchesEnumDescriptionAttributes()
        {
            Msg.Clear();

            // 中文包枚举词条由 Status.cs / AuditStatus.cs 的特性同步而来；
            // 若特性被修改而未同步语言包，此用例会失败
            var count = 0;
            foreach (var enumType in new[] { typeof(Status), typeof(AuditStatus) })
            {
                foreach (var field in enumType.GetFields(BindingFlags.Public | BindingFlags.Static))
                {
                    if (!field.FieldType.IsEnum)
                    {
                        continue;
                    }

                    var attrs = field.GetCustomAttributes(typeof(EnumDescription), false);
                    if (attrs.Length == 0)
                    {
                        continue;
                    }

                    var expected = ((EnumDescription)attrs[0]).Text;
                    Assert.Equal(expected, Msg.Get(Msg.EnumKeyPrefix + enumType.Name + "." + field.Name));
                    count++;
                }
            }

            // Status 60 条 + AuditStatus 10 条
            Assert.Equal(70, count);
        }

        [Fact]
        public void GetEnumDescriptions_IsLocalized()
        {
            Msg.Clear();

            // AuditStatus 声明顺序：Pause(0) / Draft(1) / StartAudit(2)...
            var zh = EnumUtil.GetEnumDescriptions(typeof(AuditStatus));
            Assert.Equal("草稿", (string)zh[1]!);

            Msg.CurrentLanguage = "en";
            var en = EnumUtil.GetEnumDescriptions(typeof(AuditStatus));
            Assert.Equal("Draft", (string)en[1]!);
        }

        #endregion

        #region 异常消息与业务状态消息（P3）

        [Fact]
        public void ExceptionMessage_SwitchesLanguage()
        {
            Msg.Clear();
            Assert.Equal("无效IP", Msg.Get("Exception.InvalidIp"));
            Assert.Equal("表达式语法错误。", Msg.Get("Exception.ExpressionSyntaxError"));

            Msg.CurrentLanguage = "en";
            Assert.Equal("Invalid IP", Msg.Get("Exception.InvalidIp"));
            Assert.Equal("Expression syntax error.", Msg.Get("Exception.ExpressionSyntaxError"));
        }

        [Fact]
        public void ExceptionMessage_Format_SubstitutesFileName()
        {
            Msg.Clear();
            Assert.Equal("a.txt文件不存在！", Msg.Format("Exception.FileNotFound", "a.txt"));

            Msg.CurrentLanguage = "en";
            Assert.Equal("File a.txt does not exist!", Msg.Format("Exception.FileNotFound", "a.txt"));
        }

        [Fact]
        public void BaseResult_DefaultStatusMessage_IsLocalized()
        {
            Msg.Clear();
            Assert.Equal("未知错误", new BaseResult().StatusMessage);

            Msg.CurrentLanguage = "en";
            // 默认值在构造时取当前语言快照，切语言后新建对象即为英文
            Assert.Equal("Unknown error", new BaseResult().StatusMessage);
        }

        [Fact]
        public void LogonStatusMessage_SwitchesLanguage()
        {
            Msg.Clear();
            Assert.Equal("登录成功", Msg.Get("Logon.Success"));
            Assert.Equal("访问被拒绝、您的账户没有访问权限。", Msg.Get("Logon.AccessDenied"));

            Msg.CurrentLanguage = "en";
            Assert.Equal("Sign-in succeeded", Msg.Get("Logon.Success"));
            Assert.Equal("Access denied: your account has no access permission.", Msg.Get("Logon.AccessDenied"));
        }

        #endregion

        #region 日志与控制台输出（P4）

        [Fact]
        public void LogMessage_SwitchesLanguage()
        {
            Msg.Clear();
            Assert.Equal("找不到模板文件", Msg.Get("Log.TemplateFileNotFound"));
            Assert.Equal("打开模板文件失败", Msg.Get("Log.OpenTemplateFailed"));
            Assert.Equal("RestartIisProcess 重写 web.config 失败", Msg.Get("Log.RestartIisFailed"));

            Msg.CurrentLanguage = "en";
            Assert.Equal("Template file not found", Msg.Get("Log.TemplateFileNotFound"));
            Assert.Equal("Failed to open the template file", Msg.Get("Log.OpenTemplateFailed"));
            Assert.Equal("RestartIisProcess failed to rewrite web.config", Msg.Get("Log.RestartIisFailed"));
        }

        [Fact]
        public void LogMessage_Format_SubstitutesPlaceholder()
        {
            Msg.Clear();
            Assert.Equal("服务调用失败，请检查：userInfo.Id", Msg.Format("Log.ServiceFail", "userInfo.Id"));
            Assert.Equal("UserConfigUtil 配置解析失败：Port=abc，已使用默认值。",
                Msg.Format("Log.ConfigParseFailed", "Port", "abc"));
            Assert.Equal("RemoteSaveAs 下载远程文件失败: http://a/b.txt",
                Msg.Format("Log.RemoteSaveAsFailed", "http://a/b.txt"));

            Msg.CurrentLanguage = "en";
            Assert.Equal("Service invocation failed, please check: userInfo.Id",
                Msg.Format("Log.ServiceFail", "userInfo.Id"));
            Assert.Equal("UserConfigUtil failed to parse configuration: Port=abc. The default value is used.",
                Msg.Format("Log.ConfigParseFailed", "Port", "abc"));
            Assert.Equal("RemoteSaveAs failed to download the remote file: http://a/b.txt",
                Msg.Format("Log.RemoteSaveAsFailed", "http://a/b.txt"));
        }

        [Fact]
        public void ConsoleMessage_SwitchesLanguage()
        {
            Msg.Clear();
            Assert.Equal("通用文字识别:", Msg.Get("Console.OcrGeneral"));
            Assert.Equal("100 : 完成 ", Msg.Format("Console.Completed", 100));
            Assert.Equal("任务7开始工作……", Msg.Format("Console.TaskStart", 7));

            Msg.CurrentLanguage = "en";
            Assert.Equal("General text recognition:", Msg.Get("Console.OcrGeneral"));
            Assert.Equal("100 : done ", Msg.Format("Console.Completed", 100));
            Assert.Equal("Task 7 starting...", Msg.Format("Console.TaskStart", 7));
        }

        [Fact]
        public void ZhCnServicePack_MatchesAppMessageServiceFields()
        {
            Msg.Clear();

            // AppMessage.Service 的常量未被生产代码引用（仅注释中的历史写法），字段按约定保持不变；
            // 但语言包必须与之同步，供调用方通过 Msg.Get("Service.FileService") 取得本地化文本。
            var fields = typeof(AppMessage).GetFields(BindingFlags.Public | BindingFlags.Static);
            var count = 0;
            foreach (var field in fields)
            {
                if (field.FieldType != typeof(string))
                {
                    continue;
                }

                if (!field.Name.StartsWith("FileService", StringComparison.Ordinal)
                    && !field.Name.StartsWith("UserService", StringComparison.Ordinal))
                {
                    continue;
                }

                var expected = (string)field.GetValue(null);
                Assert.Equal(expected, Msg.Get("Service." + field.Name));
                count++;
            }

            // FileService 18 条 + UserService 15 条
            Assert.Equal(33, count);
        }

        [Fact]
        public void ServiceMessage_SwitchesLanguage()
        {
            Msg.Clear();
            Assert.Equal("档案服务", Msg.Get("Service.FileService"));
            Assert.Equal("用户管理服务", Msg.Get("Service.UserService"));

            Msg.CurrentLanguage = "en";
            Assert.Equal("Archive service", Msg.Get("Service.FileService"));
            Assert.Equal("User management service", Msg.Get("Service.UserService"));
        }

        #endregion

        #region 全库补漏（P5）

        [Fact]
        public void LeftoverLogonMessage_SwitchesLanguage()
        {
            Msg.Clear();
            Assert.Equal("登录失败", Msg.Get("Logon.Failed"));
            Assert.Equal("未找到对应的用户", Msg.Get("Logon.UserNotFound"));
            Assert.Equal("用户被锁定，不允许重置密码。", Msg.Get("Logon.UserLockedCannotResetPassword"));

            Msg.CurrentLanguage = "en";
            Assert.Equal("Sign-in failed", Msg.Get("Logon.Failed"));
            Assert.Equal("The corresponding user was not found", Msg.Get("Logon.UserNotFound"));
            Assert.Equal("The user is locked; resetting the password is not allowed.",
                Msg.Get("Logon.UserLockedCannotResetPassword"));
        }

        [Fact]
        public void LeftoverLogonMessage_Format_SubstitutesPlaceholder()
        {
            Msg.Clear();
            Assert.Equal("新密码已发送到您的注册邮箱a@b.com，请注意查收。",
                Msg.Format("Logon.PasswordResetMailSent", "a@b.com"));
            Assert.Equal("域服务器返回信息timeout", Msg.Format("Logon.DomainServerResponse", "timeout"));

            Msg.CurrentLanguage = "en";
            Assert.Equal("The new password has been sent to your registered mailbox a@b.com, please check it.",
                Msg.Format("Logon.PasswordResetMailSent", "a@b.com"));
            Assert.Equal("Domain server returned: timeout", Msg.Format("Logon.DomainServerResponse", "timeout"));
        }

        [Fact]
        public void BusinessErrorMessage_SwitchesLanguage()
        {
            Msg.Clear();
            Assert.Equal("数据已被删除，无需再次删除", Msg.Get("Business.DataAlreadyDeleted"));
            Assert.Equal("系统数据无权操作", Msg.Get("Business.SystemDataNoPermission"));
            Assert.Equal("非本公司数据无权操作", Msg.Get("Business.OtherCompanyDataNoPermission"));

            Msg.CurrentLanguage = "en";
            Assert.Equal("The data has already been deleted; there is no need to delete it again",
                Msg.Get("Business.DataAlreadyDeleted"));
            Assert.Equal("No permission to operate on system data", Msg.Get("Business.SystemDataNoPermission"));
            Assert.Equal("No permission to operate on data of another company",
                Msg.Get("Business.OtherCompanyDataNoPermission"));
        }

        [Fact]
        public void UtilityReturnMessage_SwitchesLanguage()
        {
            Msg.Clear();
            Assert.Equal("溢出", Msg.Get("Rmb.Overflow"));
            Assert.Equal("非数字形式！", Msg.Get("Rmb.NotNumeric"));
            Assert.Equal("未知", Msg.Get("Qqwry.Unknown"));
            Assert.Equal("手机号码有误！", Msg.Get("Sms.InvalidMobile"));

            Msg.CurrentLanguage = "en";
            Assert.Equal("Overflow", Msg.Get("Rmb.Overflow"));
            Assert.Equal("Not a numeric value!", Msg.Get("Rmb.NotNumeric"));
            Assert.Equal("Unknown", Msg.Get("Qqwry.Unknown"));
            Assert.Equal("Invalid mobile number!", Msg.Get("Sms.InvalidMobile"));
        }

        [Fact]
        public void Rmb_InvalidInput_ReturnsLocalizedMessage()
        {
            Msg.Clear();
            Assert.Equal("非数字形式！", RmbUtil.Capital("abc"));

            Msg.CurrentLanguage = "en";
            Assert.Equal("Not a numeric value!", RmbUtil.Capital("abc"));
        }

        #endregion

        #region 键大小写敏感性（内层字典改用 Ordinal 后）

        [Fact]
        public void Get_KeyIsCaseSensitive_MissFallsBackToKeyItself()
        {
            Msg.Clear();
            // 内层消息键字典用 Ordinal：大小写不一致视为缺失，回退返回键本身
            Assert.Equal("发生未知错误。", Msg.Get("Common.UnknownError"));
            Assert.Equal("common.unknownerror", Msg.Get("common.unknownerror"));
        }

        [Fact]
        public void Get_CultureIsCaseInsensitive()
        {
            Msg.Clear();
            // 外层语言名（culture）仍用 OrdinalIgnoreCase，容忍 "zh-cn" / "EN" 写法
            Assert.Equal("发生未知错误。", Msg.Get("Common.UnknownError", "zh-cn"));
            Assert.Equal("An unknown error occurred.", Msg.Get("Common.UnknownError", "EN"));
        }

        #endregion
    }
}
