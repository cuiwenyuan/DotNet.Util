# Wangcaisoft.DotNet.Util.Plus

> 扩展工具库 / Extended utility library

## 简介 / Introduction

`DotNet.Util.Plus` 在 `DotNet.Util` 之上提供一组扩展工具：Excel 导入导出、微信小程序、百度 OCR、IP 库、压缩、目录服务、HTTP 头处理与加解密扩展等。

`DotNet.Util.Plus` builds on `DotNet.Util` to provide extended helpers: Excel import/export, WeChat Mini Program, Baidu OCR, IP database, compression, directory services, HTTP header handling and cryptography extensions.

## 包含内容 / What's Inside

- `ExcelUtil`：基于 NPOI 的 Excel 读写与 HTML 转换。
- `WeChatMiniProgramUtil`：微信小程序相关工具（如小程序码获取）。
- `BaiduOcrUtil`：百度 OCR 识别。
- `QqwryUtil`：纯真 IP 库查询。
- 压缩（SharpZipLib）、目录服务（`System.DirectoryServices`）、HTTP 头（`Microsoft.Net.Http.Headers`）、加解密（BouncyCastle，.NET Framework）。

## 安装 / Installation

```bash
dotnet add package Wangcaisoft.DotNet.Util.Plus
```

## 目标框架 / Target Frameworks

net46 · net47 · net48 · net6.0 · net7.0 · net8.0 · net9.0 · net10.0 · netstandard2.0 · netstandard2.1

## 依赖 / Dependencies

- `Wangcaisoft.DotNet.Util`
- [Newtonsoft.Json](https://www.nuget.org/packages/Newtonsoft.Json) `13.0.4`
- [NPOI](https://www.nuget.org/packages/NPOI) `2.7.5`（.NET Framework 用 `2.5.6`）
- [SharpZipLib](https://www.nuget.org/packages/SharpZipLib) `1.4.2`（.NET Framework 用 `1.3.3`）
- [System.DirectoryServices](https://www.nuget.org/packages/System.DirectoryServices) `9.0.9`
- [Microsoft.Net.Http.Headers](https://www.nuget.org/packages/Microsoft.Net.Http.Headers) `2.3.4` / `8.0.20` / `9.0.9`（按框架）
- [Portable.BouncyCastle](https://www.nuget.org/packages/Portable.BouncyCastle) `1.9.0`（仅 .NET Framework）

## 快速使用 / Quick Start

```csharp
using DotNet.Util;

// 读取 Excel 到 DataTable（基于 NPOI）
DataTable dt = ExcelUtil.ExportExcelToDataTable("data.xlsx");

// 纯真 IP 库查询
var info = QqwryUtil.GetIpLocation("8.8.8.8");
```

> 更多 API 详见各类型随包提供的 XML 文档注释（IntelliSense）。

## 许可证 / License

MIT — ©2008-2026 Wangcaisoft

## 链接 / Links

- 仓库 / Repository: <https://github.com/cuiwenyuan/DotNet.Util>
