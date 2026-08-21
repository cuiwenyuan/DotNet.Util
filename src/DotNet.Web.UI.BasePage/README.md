# Wangcaisoft.DotNet.Web.UI.BasePage

> WebForms 页面基类 / WebForms BasePage

## 简介 / Introduction

`DotNet.Web.UI.BasePage` 提供 WebForms 应用的基础页面类 `BasePage`，封装了登录态校验、权限检查、缓存清理、菜单与页面权限等 WebForms 通用能力。仅支持 .NET Framework（net4x）。

`DotNet.Web.UI.BasePage` provides the WebForms base page class `BasePage`,封装 login-state validation, permission checks, cache clearing, menu and page-permission capabilities for WebForms apps. .NET Framework (net4x) only.

## 包含内容 / What's Inside

- `BasePage`：WebForms 页面基类（权限校验、缓存清理、菜单、登录态等）。
- 与 `DotNet.Business.Web`、`DotNet.Business`、`DotNet.Model`、`DotNet.Util.Cache`、`DotNet.Util` 协同。
- 内置 JWT 支持。

## 安装 / Installation

```bash
dotnet add package Wangcaisoft.DotNet.Web.UI.BasePage
```

## 目标框架 / Target Frameworks

net46 · net47 · net48（仅 .NET Framework）

## 依赖 / Dependencies

- `Wangcaisoft.DotNet.Business.Web`
- `Wangcaisoft.DotNet.Business`
- `Wangcaisoft.DotNet.Model`
- `Wangcaisoft.DotNet.Util.Cache`
- `Wangcaisoft.DotNet.Util`
- [JWT](https://www.nuget.org/packages/JWT) `11.0.0`

## 快速使用 / Quick Start

```csharp
using DotNet.Web.UI;

// 继承 BasePage 以获得权限校验、缓存清理等能力
public class Default : BasePage
{
    protected void Page_Load(object sender, EventArgs e) { }
}
```

> 更多 API 详见各类型随包提供的 XML 文档注释（IntelliSense）。

## 许可证 / License

MIT — ©2008-2026 Wangcaisoft

## 链接 / Links

- 仓库 / Repository: <https://github.com/cuiwenyuan/DotNet.Util>
