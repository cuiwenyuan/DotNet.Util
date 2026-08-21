# Wangcaisoft.DotNet.Model

> 实体模型 / Entity models

## 简介 / Introduction

`DotNet.Model` 提供一套示例性的领域实体模型，覆盖用户（User）、角色（Role）、菜单（Menu）、权限（Permission）、组织机构（Organization）等常见对象，供 `DotNet.Business` 与上层应用复用。

`DotNet.Model` ships a set of sample domain entity models for User, Role, Menu, Permission, Organization and similar objects, reused by `DotNet.Business` and upper-layer applications.

## 包含内容 / What's Inside

- 用户 / 角色 / 菜单 / 模块 / 权限 / 组织机构 / 字典 / 日志 等基础实体（Entity）。
- 数据注解（`System.ComponentModel.Annotations`）标注的字段元数据。

## 安装 / Installation

```bash
dotnet add package Wangcaisoft.DotNet.Model
```

## 目标框架 / Target Frameworks

net46 · net47 · net48 · net6.0 · net7.0 · net8.0 · net9.0 · net10.0 · netstandard2.0 · netstandard2.1

## 依赖 / Dependencies

- `Wangcaisoft.DotNet.Util`
- [System.ComponentModel.Annotations](https://www.nuget.org/packages/System.ComponentModel.Annotations) `5.0.0`

## 快速使用 / Quick Start

```csharp
using DotNet.Model;

// 直接使用内置实体
var user = new BaseUserEntity
{
    UserName = "admin",
    RealName = "Administrator"
};
```

> 更多 API 详见各类型随包提供的 XML 文档注释（IntelliSense）。

## 许可证 / License

MIT — ©2008-2026 Wangcaisoft

## 链接 / Links

- 仓库 / Repository: <https://github.com/cuiwenyuan/DotNet.Util>
