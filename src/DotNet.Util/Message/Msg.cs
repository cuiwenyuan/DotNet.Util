//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2026, DotNet.
//-----------------------------------------------------------------

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Newtonsoft.Json;

namespace DotNet.Util
{
    /// <summary>
    ///	Msg
    /// 多语言消息层：按当前语言取得输出消息。
    /// 
    /// 推荐调用：Msg.Common.UnknownError、Msg.Common.ParameterRequired("用户名")。
    /// 动态键仍可用 Msg.Get("Common.UnknownError") / Msg.Format(...)。
    /// 
    /// 默认语言为 zh-CN（与 BaseSystemInfo.CurrentLanguage 保持一致），en 为第一个语言包。
    /// 取值回退链：精确语言(en-US) → 中性语言(en) → 默认语言(zh-CN) → 返回键本身。
    /// 
    /// 设计约定：
    /// 1) 任何情况下均不抛异常，缺失键时回退到中文，再缺失则返回键本身；
    /// 2) 语言包以代码内嵌字典承载，支持外部 JSON 覆盖（LoadJsonOverride）；
    /// 3) 线程安全：语言包使用 ConcurrentDictionary，切换语言仅替换字符串引用。
    /// 
    /// 修改记录
    /// 
    ///		2026.09.15 版本：1.0	Troy.Cui 建立，为全库输出消息提供多语言支持。
    ///	
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2026.09.15</date>
    /// </author> 
    /// </summary>
    public static partial class Msg
    {
        /// <summary>
        /// 默认语言，与 BaseSystemInfo.CurrentLanguage 的初始值保持一致
        /// </summary>
        public const string DefaultLanguage = "zh-CN";

        // 语言包：culture → (key → 文本)
        private static readonly ConcurrentDictionary<string, ConcurrentDictionary<string, string>> Packs =
            new ConcurrentDictionary<string, ConcurrentDictionary<string, string>>(StringComparer.OrdinalIgnoreCase);

        private static readonly object InitLock = new object();

        // volatile：EnsureInitialized 的快路径在锁外读取，需保证其它线程写入后的可见性
        private static volatile bool _initialized;

        // volatile：CurrentLanguage 的读写均无锁（切换语言后立即对其它线程可见）
        private static volatile string _language;

        #region public static string CurrentLanguage 当前语言

        /// <summary>
        /// 当前语言。未显式设置时跟随 BaseSystemInfo.CurrentLanguage（默认为 zh-CN）。
        /// </summary>
        public static string CurrentLanguage
        {
            get
            {
                if (!string.IsNullOrEmpty(_language))
                {
                    return _language;
                }
                var language = BaseSystemInfo.CurrentLanguage;
                return string.IsNullOrEmpty(language) ? DefaultLanguage : language;
            }
            set => _language = value;
        }

        #endregion

        #region public static string Get(string key) 按当前语言取得消息

        /// <summary>
        /// 按当前语言取得消息。缺失时依次回退：中性语言 → 默认语言(zh-CN) → 键本身。
        /// 键区分大小写，须与语言包中的键完全一致。
        /// </summary>
        /// <param name="key">消息键，如 Common.UnknownError、Enum.Status.DbError</param>
        /// <returns>消息文本</returns>
        public static string Get(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return string.Empty;
            }

            EnsureInitialized();

            var language = CurrentLanguage;
            var text = Lookup(key, language);
            if (text != null)
            {
                return text;
            }

            // 回退到默认语言（中文）
            if (!string.Equals(language, DefaultLanguage, StringComparison.OrdinalIgnoreCase))
            {
                text = Lookup(key, DefaultLanguage);
                if (text != null)
                {
                    return text;
                }
            }

            // 最终回退：返回键本身，保证调用方不会拿到空串或异常
            return key;
        }

        #endregion

        #region public static string Get(string key, string culture) 按指定语言取得消息

        /// <summary>
        /// 按指定语言取得消息，不走当前语言，回退规则同 Get(key)。
        /// </summary>
        /// <param name="key">消息键</param>
        /// <param name="culture">语言，如 en、en-US</param>
        /// <returns>消息文本</returns>
        public static string Get(string key, string culture)
        {
            if (string.IsNullOrEmpty(key))
            {
                return string.Empty;
            }

            EnsureInitialized();

            var text = Lookup(key, culture);
            if (text != null)
            {
                return text;
            }

            if (!string.IsNullOrEmpty(culture) &&
                !string.Equals(culture, DefaultLanguage, StringComparison.OrdinalIgnoreCase))
            {
                text = Lookup(key, DefaultLanguage);
                if (text != null)
                {
                    return text;
                }
            }

            return key;
        }

        #endregion

        #region public static string GetOrDefault(string key, string defaultValue) 取得消息，缺失时用默认值

        /// <summary>
        /// 按当前语言取得消息；语言包中不存在该键时返回 <paramref name="defaultValue"/>。
        /// 与 Get 的区别：Get 在彻底缺失时返回键本身，本方法返回调用方提供的默认值。
        /// 典型用途：枚举描述本地化——键缺失时回退到 [EnumDescription] 特性里写的文本。
        /// </summary>
        /// <param name="key">消息键，如 Enum.Status.Ok</param>
        /// <param name="defaultValue">缺失时的默认值，通常是特性中的原文</param>
        /// <returns>消息文本或默认值</returns>
        public static string GetOrDefault(string key, string defaultValue)
        {
            if (string.IsNullOrEmpty(key))
            {
                return defaultValue;
            }

            EnsureInitialized();

            var language = CurrentLanguage;
            var text = Lookup(key, language);
            if (text != null)
            {
                return text;
            }

            if (!string.Equals(language, DefaultLanguage, StringComparison.OrdinalIgnoreCase))
            {
                text = Lookup(key, DefaultLanguage);
                if (text != null)
                {
                    return text;
                }
            }

            return defaultValue;
        }

        /// <summary>
        /// 按指定语言取得消息，不走当前语言；缺失时返回 <paramref name="defaultValue"/>。
        /// </summary>
        /// <param name="key">消息键</param>
        /// <param name="culture">语言，如 en、en-US</param>
        /// <param name="defaultValue">缺失时的默认值</param>
        /// <returns>消息文本或默认值</returns>
        public static string GetOrDefault(string key, string culture, string defaultValue)
        {
            if (string.IsNullOrEmpty(key))
            {
                return defaultValue;
            }

            EnsureInitialized();

            var text = Lookup(key, culture);
            if (text != null)
            {
                return text;
            }

            if (!string.IsNullOrEmpty(culture) &&
                !string.Equals(culture, DefaultLanguage, StringComparison.OrdinalIgnoreCase))
            {
                text = Lookup(key, DefaultLanguage);
                if (text != null)
                {
                    return text;
                }
            }

            return defaultValue;
        }

        #endregion

        #region public static string GetEnumDescription(...) 枚举成员本地化描述

        /// <summary>
        /// 枚举描述键前缀，完整键形如 Enum.Status.Ok、Enum.AuditStatus.Draft。
        /// </summary>
        public const string EnumKeyPrefix = "Enum.";

        /// <summary>
        /// 取得枚举成员的本地化描述。
        /// 键缺失时返回 <paramref name="defaultValue"/>，即 [EnumDescription] 特性里写的原文，
        /// 因此第三方/私有枚举即使未登记词条也能正常工作。
        /// </summary>
        /// <param name="enumType">枚举类型</param>
        /// <param name="memberName">枚举成员名</param>
        /// <param name="defaultValue">缺失时的默认值</param>
        /// <returns>本地化描述或默认值</returns>
        public static string GetEnumDescription(Type enumType, string memberName, string defaultValue)
        {
            if (enumType == null || string.IsNullOrEmpty(memberName))
            {
                return defaultValue;
            }

            var key = EnumKeyPrefix + enumType.Name + "." + memberName;
            return GetOrDefault(key, defaultValue);
        }

        /// <summary>
        /// 按指定语言取得枚举成员的本地化描述，不走当前语言。
        /// </summary>
        /// <param name="enumType">枚举类型</param>
        /// <param name="memberName">枚举成员名</param>
        /// <param name="culture">语言，如 en</param>
        /// <param name="defaultValue">缺失时的默认值</param>
        /// <returns>本地化描述或默认值</returns>
        public static string GetEnumDescription(Type enumType, string memberName, string culture, string defaultValue)
        {
            if (enumType == null || string.IsNullOrEmpty(memberName))
            {
                return defaultValue;
            }

            var key = EnumKeyPrefix + enumType.Name + "." + memberName;
            return GetOrDefault(key, culture, defaultValue);
        }

        #endregion

        #region public static string Format(string key, params object[] args) 格式化消息

        /// <summary>
        /// 取得消息并按占位符 {0} {1} 格式化，用于带变量的消息（如"请输入{0}，不允许为空。"）。
        /// </summary>
        /// <param name="key">消息键</param>
        /// <param name="args">占位参数</param>
        /// <returns>格式化后的消息文本</returns>
        public static string Format(string key, params object[] args)
        {
            var text = Get(key);
            if (args == null || args.Length == 0)
            {
                return text;
            }

            try
            {
                return string.Format(CultureInfo.CurrentCulture, text, args);
            }
            catch (FormatException)
            {
                // 占位符不匹配时返回原文，绝不因消息格式化而中断业务流程
                return text;
            }
        }

        #endregion

        #region public static void Register(string culture, IDictionary<string, string> messages) 注册语言包

        /// <summary>
        /// 注册（或合并）指定语言的消息。同名键以最后注册者为准，可用于增量覆盖。
        /// 注册前会先确保内置语言包已就位，因此外部注册总能覆盖内置词条。
        /// 语言名不区分大小写（"en-US" 与 "en-us" 等价），但消息键区分大小写（Ordinal）。
        /// </summary>
        /// <param name="culture">语言，如 zh-CN、en</param>
        /// <param name="messages">消息字典</param>
        public static void Register(string culture, IDictionary<string, string> messages)
        {
            EnsureInitialized();
            RegisterCore(culture, messages);
        }

        /// <summary>
        /// 写入语言包，不触发初始化。供内置语言包登记（MsgPack）使用，避免与 EnsureInitialized 递归。
        /// </summary>
        internal static void RegisterCore(string culture, IDictionary<string, string> messages)
        {
            if (string.IsNullOrEmpty(culture) || messages == null)
            {
                return;
            }

            // 内层键字典用 Ordinal：键由代码生成、调用点也是精确字面量，无需忽略大小写，
            // 且可避免 hash 开销；外层语言名（culture）仍用 OrdinalIgnoreCase 容忍 "en-us"。
            // 注意：这意味着消息键区分大小写，Msg.Get("common.unknownerror") 不会命中 "Common.UnknownError"。
            var pack = Packs.GetOrAdd(culture,
                _ => new ConcurrentDictionary<string, string>(StringComparer.Ordinal));

            foreach (var item in messages)
            {
                pack[item.Key] = item.Value;
            }
        }

        #endregion

        #region public static void LoadJsonOverride(string culture, string path) 外部 JSON 覆盖

        /// <summary>
        /// 从外部 JSON 文件加载消息并覆盖（合并）到指定语言包。
        /// 文件不存在或格式错误时静默跳过，不影响内置语言包。
        /// JSON 中的键须与语言包键大小写完全一致，否则不会覆盖到目标词条。
        /// </summary>
        /// <param name="culture">语言，如 en</param>
        /// <param name="path">JSON 文件绝对路径，内容形如 {"Common.UnknownError":"..."}</param>
        public static void LoadJsonOverride(string culture, string path)
        {
            if (string.IsNullOrEmpty(culture) || string.IsNullOrEmpty(path) || !global::System.IO.File.Exists(path))
            {
                return;
            }

            try
            {
                var json = global::System.IO.File.ReadAllText(path);
                var messages = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                if (messages != null)
                {
                    Register(culture, messages);
                }
            }
            catch (IOException)
            {
                // 外部覆盖属于可选能力，失败时保持内置语言包可用
            }
            catch (JsonException)
            {
                // JSON 格式错误同上，静默跳过
            }
        }

        #endregion

        #region public static void Clear() 清空全部语言包

        /// <summary>
        /// 清空全部语言包并复位当前语言，下次取值时重新初始化。主要用于单元测试隔离。
        /// </summary>
        public static void Clear()
        {
            Packs.Clear();
            _language = null;
            MsgPack.Reset();
            lock (InitLock)
            {
                _initialized = false;
            }
        }

        #endregion

        #region public static IReadOnlyCollection<string> GetKeys(string culture) 取得指定语言的全部键

        /// <summary>
        /// 取得指定语言包中的全部消息键，用于校验各语言包条目是否一致。
        /// </summary>
        /// <param name="culture">语言，如 zh-CN、en</param>
        /// <returns>键集合，语言包不存在时返回空集合</returns>
        public static IReadOnlyCollection<string> GetKeys(string culture)
        {
            EnsureInitialized();

            if (string.IsNullOrEmpty(culture) || !Packs.TryGetValue(culture, out var pack))
            {
                return new List<string>();
            }

            var keys = new List<string>(pack.Keys);
            keys.Sort(StringComparer.Ordinal);
            return keys;
        }

        #endregion

        #region private static string Lookup(string key, string culture) 按语言查找（含中性语言回退）

        /// <summary>
        /// 在指定语言中查找消息，先精确匹配（en-US），再回退中性语言（en）。
        /// </summary>
        private static string Lookup(string key, string culture)
        {
            if (string.IsNullOrEmpty(culture))
            {
                return null;
            }

            if (Packs.TryGetValue(culture, out var pack) && pack.TryGetValue(key, out var text))
            {
                return text;
            }

            // 中性语言回退：en-US → en
            var index = culture.IndexOf('-');
            if (index > 0)
            {
                var neutral = culture.Substring(0, index);
                if (Packs.TryGetValue(neutral, out var neutralPack) && neutralPack.TryGetValue(key, out var neutralText))
                {
                    return neutralText;
                }
            }

            return null;
        }

        #endregion

        #region private static void EnsureInitialized() 确保内置语言包已注册

        /// <summary>
        /// 惰性初始化内置语言包。Register 本身不会触发本方法，避免递归。
        /// </summary>
        private static void EnsureInitialized()
        {
            if (_initialized)
            {
                return;
            }

            lock (InitLock)
            {
                if (_initialized)
                {
                    return;
                }

                MsgPack.EnsureRegistered();
                _initialized = true;
            }
        }

        #endregion
    }
}
