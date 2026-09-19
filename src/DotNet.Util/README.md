# Wangcaisoft.DotNet.Util

> 基础工具库 / Core utility library

## 简介 / Introduction

`DotNet.Util` 是一套覆盖 .NET 10/9/8/7/6/5 与 .NET Standard 2.0/2.1，并向下兼容 .NET Framework 4.6/4.7/4.8 的通用工具集合。本包是整套 `Wangcaisoft.DotNet.*` 系列的基础依赖，提供字符串/类型转换、日期、加解密、IO、HTTP、IP 与地理位置、验证码、文件上传、缓存、全局配置等能力。

`DotNet.Util` is a general-purpose utility set targeting .NET 10/9/8/7/6/5, .NET Standard 2.0/2.1 and .NET Framework 4.6/4.7/4.8. It is the foundational dependency of the `Wangcaisoft.DotNet.*` family, offering string/type conversion, date, cryptography, IO, HTTP, IP/geo, captcha, upload, caching and global configuration.

## 包含内容 / What's Inside

- `Util`：字符串/类型转换（`Utils`）、日期（`DateUtil`）、加解密（`SecretUtil`）、IO（`FileUtil`）、HTTP（`HttpUtil`）、IP 与纯真 IP 库（`IpUtil`/`QqwryUtil`）、验证码（`VerifyCodeImage`）、文件上传（`WebUpload`）、校验（`ValidateUtil`）。
- `BaseSystemInfo` / `Configuration`：全局配置（连接串、注册表、缓存开关等）。
- `Cache`：`CacheUtil` 内存缓存统一接口。
- `Db/Expression`：SQL 表达式、列/表元数据。
- `Entity` / `Model` / `Message`：基础实体与消息模型。
- `NewLife`：编码检测等扩展（`EncodingUtil`）。

## 安装 / Installation

```bash
dotnet add package Wangcaisoft.DotNet.Util
```

## 目标框架 / Target Frameworks

net46 · net47 · net48 · net6.0 · net7.0 · net8.0 · net9.0 · net10.0 · netstandard2.0 · netstandard2.1

## 依赖 / Dependencies

- [NewLife.Core](https://www.nuget.org/packages/NewLife.Core) `11.19.2026.901`
- [Newtonsoft.Json](https://www.nuget.org/packages/Newtonsoft.Json) `13.0.4`
- （现代框架）Microsoft.AspNetCore.* `2.3.12`、Microsoft.CSharp `4.7.0`、Microsoft.Extensions.*（net6/7 用 `8.0.1`，其余 `10.0.11`）、Microsoft.Windows.Compatibility / System.Drawing.Common（net6/7 用 `8.0.30`，其余 `10.0.11`）、System.Management / System.Text.Encoding.CodePages（net6/7 用 `8.0.0`，其余 `10.0.11`）、System.Runtime.Loader `4.3.0`、System.ComponentModel.Annotations `5.0.0`

## 快速使用 / Quick Start

```csharp
using DotNet.Util;

// 读取全局配置（appSettings / 注册表等）
BaseSystemInfo.Settings();

// 常用校验与字符串工具
bool isDate = ValidateUtil.IsDateTime("2026-08-21");
string trimmed = Utils.ClearLastChar("a,b,c,");   // -> "a,b,c"
```

> 更多 API 详见各类型随包提供的 XML 文档注释（IntelliSense）。

## 多语言 / Multi-language

输出消息已支持多语言，**默认中文（zh-CN）**，内置第一语言包**英文（en）**，共 400 条词条。
推荐使用**强类型成员**（编译期可查、IntelliSense 可补全，400 条全覆盖）：

Output messages are localized. **Chinese (zh-CN)** is the default and **English (en)** is the first built-in pack (400 entries). Prefer the **strongly-typed members** (compile-time checked, IntelliSense friendly, 100% coverage):

```csharp
using DotNet.Util;

Msg.Common.UnknownError                // 发生未知错误。/ An unknown error occurred.
Msg.Common.ParameterRequired("用户名")  // 请输入用户名，不允许为空。
Msg.Enum.Status.Ok                     // 运行成功 / Operation succeeded
Msg.Service.FileService                // 档案服务 / Archive service

Msg.CurrentLanguage = "en";            // 切换语言，强类型成员随之变化
Msg.Common.ParameterRequired("UserName")
                                       // Please enter UserName; it cannot be empty.
```

- 无占位符的词条是**属性**，含 `{0}` 的词条是**方法**（`params object[] args`）。
  Entries without placeholders are **properties**; those with `{0}` are **methods**.
- 键在编译期未知、或需显式指定语言时用字符串键：`Msg.Get("Common.UnknownError")`、`Msg.Get("Common.UnknownError", "en")`、`Msg.Format("Common.ParameterRequired", arg)`。
  Use string keys when the key is unknown at compile time or a culture must be explicit.
- 回退链 `en-US → en → zh-CN → 键名本身`，任何情况不抛异常；`Msg.Register` 可增量注册语言，`Msg.LoadJsonOverride(culture, path)` 支持外部 JSON 覆盖。
  Fallback `en-US → en → zh-CN → the key itself`, never throws; `Msg.Register` adds packs, `Msg.LoadJsonOverride(culture, path)` applies external JSON overrides.
- 覆盖：异常消息、业务状态消息、日志、控制台输出、`AppMessage` 消息键、枚举描述。
  Covers exceptions, business status, logs, console output, `AppMessage` keys and enum descriptions.
- 完整说明见仓库根目录 [`Localization.md`](https://github.com/cuiwenyuan/DotNet.Util/blob/main/Localization.md)。
  Full guide: [`Localization.md`](https://github.com/cuiwenyuan/DotNet.Util/blob/main/Localization.md).

> ⚠️ `Wangcaisoft.DotNet.Web.UI.BasePage` 包内另有一个同名类型 `DotNet.Util.Msg`（WebForm 的 JS 弹窗辅助类），
> 与多语言层不是同一个类型；两个包同时引用时请使用命名空间别名区分。
> The `Wangcaisoft.DotNet.Web.UI.BasePage` package ships a **different** type also named `DotNet.Util.Msg` (WebForm JS alerts); use a namespace alias when referencing both.

## 许可证 / License

MIT — ©2008-2026 Wangcaisoft

## 链接 / Links

- 仓库 / Repository: <https://github.com/cuiwenyuan/DotNet.Util>
- 在线文档 / Docs: <https://github.com/cuiwenyuan/DotNet.Util>
