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

- [NewLife.Core](https://www.nuget.org/packages/NewLife.Core) `11.7.2025.1001`
- [Newtonsoft.Json](https://www.nuget.org/packages/Newtonsoft.Json) `13.0.4`
- （现代框架）Microsoft.AspNetCore.* `2.3.0`、Microsoft.CSharp `4.7.0`、Microsoft.Extensions.*（net6/7 用 `8.0.0`，其余 `9.0.9`）、Microsoft.Windows.Compatibility / System.Drawing.Common / System.Management（net6/7 用 `6.0.0`，其余 `9.0.9`）、System.Runtime.Loader `4.3.0`、System.ComponentModel.Annotations `5.0.0`

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

## 许可证 / License

MIT — ©2008-2026 Wangcaisoft

## 链接 / Links

- 仓库 / Repository: <https://github.com/cuiwenyuan/DotNet.Util>
- 在线文档 / Docs: <https://github.com/cuiwenyuan/DotNet.Util>
