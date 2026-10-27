# Wangcaisoft.DotNet.Util.Db

> SQL Server / MSSQL 数据访问 / SQL Server data access

> **⚠️ 默认数据库类型说明：本包（`DotNet.Util.Db`，不带任何数据库后缀）默认且仅内置 SQL Server（MSSQL）实现。**
> 包内所有默认行为均指向 SQL Server：`DbHelper.CurrentDbType` 默认返回 `CurrentDbType.SqlServer`，`DbHelperFactory.Create()` / `GetHelper()` 的 `currentDbType` 参数默认值也是 `CurrentDbType.SqlServer`。
> 如需使用其他数据库，请安装对应的扩展包：`Wangcaisoft.DotNet.Util.Db.MySql` / `.Oracle` / `.PostgreSql` / `.SQLite` / `.OleDb`。
>
> **⚠️ Default database note: this package (`DotNet.Util.Db`, without any database suffix) targets SQL Server (MSSQL) by default and exclusively.**
> All defaults point to SQL Server: `DbHelper.CurrentDbType` defaults to `CurrentDbType.SqlServer`, and `DbHelperFactory.Create()` / `GetHelper()` default the `currentDbType` parameter to `CurrentDbType.SqlServer`.
> For other databases, install the corresponding extension package: `Wangcaisoft.DotNet.Util.Db.MySql` / `.Oracle` / `.PostgreSql` / `.SQLite` / `.OleDb`.

## 简介 / Introduction

`DotNet.Util.Db` 提供面向 SQL Server / MSSQL 的数据访问能力，包含 `DbHelper` 的 SQL Server 实现（`SqlHelper`）、一组静态 CRUD 助手（`DbUtil`）与 SQL 构造器（`SqlBuilder`），以及连接/事务的生命周期管理。

`DotNet.Util.Db` provides SQL Server / MSSQL data access: the `DbHelper` SQL Server implementation (`SqlHelper`), a set of static CRUD helpers (`DbUtil`), a `SqlBuilder`, plus connection/transaction lifecycle management.

## 包含内容 / What's Inside

- `SqlHelper`：`DbHelper` / `IDbHelper` 的 SQL Server 实现。
- `DbUtil`：静态扩展方法 —— `ExecuteNonQuery` / `ExecuteScalar` / `Fill` / `GetDataTable`（支持连接串或 `IDbHelper` 实例，支持参数化）。
- `SqlBuilder`：SQL 语句构造。
- 连接、事务（`BeginTransaction` / `Commit` / `Rollback`）、同步与异步 `Open` 管理。

## 安装 / Installation

```bash
dotnet add package Wangcaisoft.DotNet.Util.Db
```

## 目标框架 / Target Frameworks

net46 · net47 · net48 · net6.0 · net7.0 · net8.0 · net9.0 · net10.0 · netstandard2.0 · netstandard2.1

## 依赖 / Dependencies

- `Wangcaisoft.DotNet.Util`
- [System.Data.SqlClient](https://www.nuget.org/packages/System.Data.SqlClient) `4.9.1`
- [System.ValueTuple](https://www.nuget.org/packages/System.ValueTuple) `4.5.0` / `4.6.2`（按框架）

## 快速使用 / Quick Start

```csharp
using DotNet.Util;

// 默认即 SQL Server：无需指定数据库类型
// SQL Server is the default: no database type needs to be specified
IDbHelper dbHelper = DbHelperFactory.Create(); // => SqlHelper（CurrentDbType.SqlServer）

// 传入连接字符串 + SQL，自动管理连接生命周期
DataTable dt = DbUtil.Fill(connectionString, "SELECT * FROM Users");

// 参数化写入
int affected = DbUtil.ExecuteNonQuery(
    connectionString,
    "UPDATE Users SET Name = @Name WHERE Id = @Id",
    new System.Data.IDbDataParameter[] { /* 参数 */ });
```

> 说明：DbHelperFactory.Create() 不传参数时返回 SqlHelper（SQL Server 实现）。
> Note: DbHelperFactory.Create() without arguments returns SqlHelper (the SQL Server implementation).

> 更多 API 详见各类型随包提供的 XML 文档注释（IntelliSense）。

## 许可证 / License

MIT — ©2008-2026 Wangcaisoft

## 链接 / Links

- 仓库 / Repository: <https://github.com/cuiwenyuan/DotNet.Util>
