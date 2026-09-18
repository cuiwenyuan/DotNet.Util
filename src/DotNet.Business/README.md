# Wangcaisoft.DotNet.Business

> 业务逻辑层 / Business logic layer

## 简介 / Introduction

`DotNet.Business` 提供一套面向用户、角色、菜单、模块、权限、组织机构、序列号、日志、字典等场景的业务管理类（Manager）。它组合了 `DotNet.Util.Db`、`DotNet.Util.Cache`、`DotNet.Model` 与 `DotNet.Util`，是搭建管理后台的核心业务层。

`DotNet.Business` provides business manager classes for Users, Roles, Menus, Modules, Permissions, Organizations, Sequences, Logs, Dictionaries, and more. It composes `DotNet.Util.Db`, `DotNet.Util.Cache`, `DotNet.Model` and `DotNet.Util` — the core business layer for admin backends.

## 包含内容 / What's Inside

- 用户 / 角色 / 角色组织 / 权限 / 权限范围（PermissionScope）管理。
- 菜单 / 模块 / 组织机构 / 组织范围管理。
- 序列号（Sequence）、日志（Log / OperationLog / LogonLog）、字典（Dictionary / DictionaryItem）、异常、消息队列（MessageQueue / MessageSucceed / MessageFailed）、日程（Calendar）、变更日志（ChangeLog）等管理类。

## 安装 / Installation

```bash
dotnet add package Wangcaisoft.DotNet.Business
```

## 目标框架 / Target Frameworks

net46 · net47 · net48 · net6.0 · net7.0 · net8.0 · net9.0 · net10.0 · netstandard2.0 · netstandard2.1

## 依赖 / Dependencies

- `Wangcaisoft.DotNet.Util`
- `Wangcaisoft.DotNet.Util.Db`
- `Wangcaisoft.DotNet.Util.Cache`
- `Wangcaisoft.DotNet.Model`

## 快速使用 / Quick Start

```csharp
using DotNet.Business;

// 用户管理示例
var manager = new BaseUserManager();
var user = manager.GetEntity(primaryKey);
```

> 更多 API 详见各类型随包提供的 XML 文档注释（IntelliSense）。

## 许可证 / License

MIT — ©2008-2026 Wangcaisoft

## 链接 / Links

- 仓库 / Repository: <https://github.com/cuiwenyuan/DotNet.Util>
