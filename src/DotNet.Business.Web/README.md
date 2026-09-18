# Wangcaisoft.DotNet.Business.Web

> Web 业务层 / Web business layer

## 简介 / Introduction

`DotNet.Business.Web` 在 `DotNet.Business` 之上提供面向 WebForm / WebApi / MVC 的客户端示例与 Web 工具（如 `WebUtil`），并内置 JWT 支持，便于在 Web 场景中调用业务层与鉴权。

`DotNet.Business.Web` builds on `DotNet.Business` to provide WebForm / WebApi / MVC client samples and web utilities (e.g. `WebUtil`), with built-in JWT support for invoking the business layer and authenticating in web scenarios.

## 包含内容 / What's Inside

- `WebUtil` 等 Web 场景辅助类。
- 基于 JWT 的令牌签发 / 校验封装。
- 与 `DotNet.Business`、`DotNet.Model`、`DotNet.Util.Cache`、`DotNet.Util` 协同。

## 安装 / Installation

```bash
dotnet add package Wangcaisoft.DotNet.Business.Web
```

## 目标框架 / Target Frameworks

net46 · net47 · net48 · net6.0 · net7.0 · net8.0 · net9.0 · net10.0 · netstandard2.0 · netstandard2.1

## 依赖 / Dependencies

- `Wangcaisoft.DotNet.Business`
- `Wangcaisoft.DotNet.Model`
- `Wangcaisoft.DotNet.Util.Cache`
- `Wangcaisoft.DotNet.Util`
- [JWT](https://www.nuget.org/packages/JWT) `11.0.0`

## 快速使用 / Quick Start

```csharp
using DotNet.Business;

// 复用业务层管理类
var manager = new BaseUserManager();
```

> 更多 API 详见各类型随包提供的 XML 文档注释（IntelliSense）。

## 许可证 / License

MIT — ©2008-2026 Wangcaisoft

## 链接 / Links

- 仓库 / Repository: <https://github.com/cuiwenyuan/DotNet.Util>
