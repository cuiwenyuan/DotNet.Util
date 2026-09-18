# Wangcaisoft.DotNet.Util.Db.PostgreSql

> PostgreSQL 数据访问 / PostgreSQL data access

## 简介 / Introduction

`DotNet.Util.Db.PostgreSql` 在 `DotNet.Util.Db` 之上提供 PostgreSQL 的 `IDbHelper` 实现（`PostgreSqlHelper`），可与 `DbUtil` 静态方法配合使用。

`DotNet.Util.Db.PostgreSql` provides the PostgreSQL `IDbHelper` implementation (`PostgreSqlHelper`) on top of `DotNet.Util.Db`, usable with `DbUtil` static helpers.

## 包含内容 / What's Inside

- `PostgreSqlHelper`：`DbHelper` / `IDbHelper` 的 PostgreSQL 实现（基于 `Npgsql`）。
- 复用 `DotNet.Util.Db` 的 `DbUtil` 静态 CRUD 助手与连接/事务管理。

## 安装 / Installation

```bash
dotnet add package Wangcaisoft.DotNet.Util.Db.PostgreSql
```

## 目标框架 / Target Frameworks

net46 · net47 · net48 · net6.0 · net7.0 · net8.0 · net9.0 · net10.0 · netstandard2.1
（注意：本包不含 netstandard2.0）

## 依赖 / Dependencies

- `Wangcaisoft.DotNet.Util.Db`
- [Npgsql](https://www.nuget.org/packages/Npgsql) `4.0.17` / `4.1.14` / `8.0.3` / `9.0.4`（按框架）

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
