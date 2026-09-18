# Wangcaisoft.DotNet.Util.Db

> SQL Server / MSSQL 数据访问 / SQL Server data access

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
- [System.Data.SqlClient](https://www.nuget.org/packages/System.Data.SqlClient) `4.9.0`
- [System.ValueTuple](https://www.nuget.org/packages/System.ValueTuple) `4.5.0` / `4.6.1`（按框架）

## 快速使用 / Quick Start

```csharp
using DotNet.Util;

// 传入连接字符串 + SQL，自动管理连接生命周期
DataTable dt = DbUtil.Fill(connectionString, "SELECT * FROM Users");

// 参数化写入
int affected = DbUtil.ExecuteNonQuery(
    connectionString,
    "UPDATE Users SET Name = @Name WHERE Id = @Id",
    new System.Data.IDbDataParameter[] { /* 参数 */ });
```

> 更多 API 详见各类型随包提供的 XML 文档注释（IntelliSense）。

## 许可证 / License

MIT — ©2008-2026 Wangcaisoft

## 链接 / Links

- 仓库 / Repository: <https://github.com/cuiwenyuan/DotNet.Util>
