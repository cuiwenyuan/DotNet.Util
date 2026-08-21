# Wangcaisoft.DotNet.Util.Db.MySql

> MySQL 数据访问 / MySQL data access

## 简介 / Introduction

`DotNet.Util.Db.MySql` 在 `DotNet.Util.Db` 之上提供 MySQL 的 `IDbHelper` 实现（`MySqlHelper`），可与 `DbUtil` 静态方法配合使用，获得与 SQL Server 一致的数据库访问体验。

`DotNet.Util.Db.MySql` provides the MySQL `IDbHelper` implementation (`MySqlHelper`) on top of `DotNet.Util.Db`, usable with `DbUtil` static helpers for a SQL-Server-like experience.

## 包含内容 / What's Inside

- `MySqlHelper`：`DbHelper` / `IDbHelper` 的 MySQL 实现（基于 `MySql.Data`）。
- 复用 `DotNet.Util.Db` 的 `DbUtil` 静态 CRUD 助手与连接/事务管理。

## Sponsors

[![Wangcaisoft](https://img.shields.io/badge/Sponsor-Wangcaisoft-blue)](https://www.cuiwenyuan.cn)

## 安装 / Installation

```bash
dotnet add package Wangcaisoft.DotNet.Util.Db.MySql
```

## 目标框架 / Target Frameworks

net46 · net47 · net48 · net6.0 · net7.0 · net8.0 · net9.0 · net10.0 · netstandard2.0 · netstandard2.1

## 依赖 / Dependencies

- `Wangcaisoft.DotNet.Util.Db`
- [MySql.Data](https://www.nuget.org/packages/MySql.Data) `9.4.0`（net46/461 用 `8.0.32.1`）

## 快速使用 / Quick Start

```csharp
using DotNet.Util;

// 各数据库提供具体的 IDbHelper 实现，配合 DbUtil 静态方法使用
DataTable dt = DbUtil.Fill(connectionString, "SELECT * FROM Users");
```

> 更多 API 详见各类型随包提供的 XML 文档注释（IntelliSense）。

## 许可证 / License

MIT — ©2008-2026 Wangcaisoft

## 链接 / Links

- 仓库 / Repository: <https://github.com/cuiwenyuan/DotNet.Util>
