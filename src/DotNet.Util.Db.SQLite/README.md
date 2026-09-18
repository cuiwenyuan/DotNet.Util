# Wangcaisoft.DotNet.Util.Db.SQLite

> SQLite 数据访问 / SQLite data access

## 简介 / Introduction

`DotNet.Util.Db.SQLite` 在 `DotNet.Util.Db` 之上提供 SQLite 的 `IDbHelper` 实现（`SQLiteHelper`），可与 `DbUtil` 静态方法配合使用。

`DotNet.Util.Db.SQLite` provides the SQLite `IDbHelper` implementation (`SQLiteHelper`) on top of `DotNet.Util.Db`, usable with `DbUtil` static helpers.

## 包含内容 / What's Inside

- `SQLiteHelper`：`DbHelper` / `IDbHelper` 的 SQLite 实现（基于 `System.Data.SQLite.Core`）。
- 复用 `DotNet.Util.Db` 的 `DbUtil` 静态 CRUD 助手与连接/事务管理。

## 安装 / Installation

```bash
dotnet add package Wangcaisoft.DotNet.Util.Db.SQLite
```

## 目标框架 / Target Frameworks

net46 · net47 · net48 · net6.0 · net7.0 · net8.0 · net9.0 · net10.0 · netstandard2.0 · netstandard2.1

## 依赖 / Dependencies

- `Wangcaisoft.DotNet.Util.Db`
- [System.Data.SQLite.Core](https://www.nuget.org/packages/System.Data.SQLite.Core) `1.0.119`
- [Stub.System.Data.SQLite.Core.NetFramework](https://www.nuget.org/packages/Stub.System.Data.SQLite.Core.NetFramework) `1.0.119`（.NET Framework）
- [System.ValueTuple](https://www.nuget.org/packages/System.ValueTuple) `4.5.0` / `4.6.1`（按框架）

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
