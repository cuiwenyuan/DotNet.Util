# NuGet 包引用明细

> 本文档按项目列出所有 NuGet 包引用、版本及适用的目标框架（TFM）。
> 图例：TFM 列标注该版本适用的框架；`全部` 表示对该项目的所有目标框架生效。
>
> 解决方案目标框架集合：`net462` / `net47` / `net48` / `net6.0` / `net8.0` / `net10.0` / `netstandard2.0` / `netstandard2.1`

---

## DotNet.Util（核心库）

目标框架：`net462;net47;net48;net6.0;net8.0;net10.0;netstandard2.0;netstandard2.1`

| 包 | 版本 | 适用 TFM |
|---|---|---|
| NewLife.Core | 12.0.2026.1024 | 全部 |
| Newtonsoft.Json | 13.0.4 | 全部 |
| Microsoft.AspNetCore.Http | 2.3.13 | net6.0 / net8.0 / net10.0 / netstandard2.0 / netstandard2.1 |
| Microsoft.AspNetCore.Session | 2.3.13 | net6.0 / net8.0 / net10.0 / netstandard2.0 / netstandard2.1 |
| Microsoft.CSharp | 4.7.0 | net6.0 / net8.0 / net10.0 / netstandard2.0 / netstandard2.1 |
| Microsoft.Extensions.DependencyModel | 8.0.1 | net6.0 |
| Microsoft.Extensions.DependencyModel | 10.0.11 | net8.0 / net10.0 / netstandard2.0 / netstandard2.1 |
| Microsoft.Windows.Compatibility | 8.0.30 | net6.0 |
| Microsoft.Windows.Compatibility | 10.0.11 | net8.0 / net10.0 / netstandard2.0 / netstandard2.1 |
| System.Drawing.Common | 8.0.30 | net6.0 |
| System.Drawing.Common | 10.0.11 | net8.0 / net10.0 / netstandard2.0 / netstandard2.1 |
| System.Management | 8.0.0 | net6.0 |
| System.Management | 10.0.11 | net8.0 / net10.0 / netstandard2.0 / netstandard2.1 |
| System.Text.Encoding.CodePages | 8.0.0 | net6.0 |
| System.Text.Encoding.CodePages | 10.0.14 | net8.0 / net10.0 / netstandard2.0 / netstandard2.1 |
| System.Runtime.Loader | 4.3.0 | net6.0 / net8.0 / net10.0 / netstandard2.0 / netstandard2.1 |
| System.ComponentModel.Annotations | 5.0.0 | net6.0 / net8.0 / net10.0 / netstandard2.0 / netstandard2.1 |

> 注：上表中带变量控制的包按两组条件取值 ——
> `net6.0`：ExtensionsVersion=8.0.1、SystemVersion=8.0.30、SystemVersionLegacy=8.0.0、CodePagesVersion=8.0.0
> `net8.0/net10.0/netstandard2.0/netstandard2.1`：ExtensionsVersion=10.0.11、SystemVersion=10.0.11、SystemVersionLegacy=10.0.11、CodePagesVersion=10.0.14

---

## DotNet.Util.Plus

目标框架：`net462;net47;net48;net6.0;net8.0;net10.0;netstandard2.0;netstandard2.1`

| 包 | 版本 | 适用 TFM |
|---|---|---|
| Newtonsoft.Json | 13.0.4 | 全部 |
| NPOI | 2.7.6 | net6.0 / net8.0 / net10.0 / netstandard2.0 / netstandard2.1 |
| NPOI | 2.5.6 | net462 / net47 / net48 |
| SharpZipLib | 1.4.2 | net48 / net6.0 / net8.0 / net10.0 / netstandard2.0 / netstandard2.1 |
| SharpZipLib | 1.3.3 | net462 / net47 |
| System.DirectoryServices | 8.0.0 | net6.0 |
| System.DirectoryServices | 10.0.11 | net8.0 / net10.0 / netstandard2.0 / netstandard2.1 |
| Microsoft.Extensions.Hosting | 8.0.1 | net6.0 |
| Microsoft.Extensions.Hosting | 10.0.11 | net8.0 / net10.0 / netstandard2.0 / netstandard2.1 |
| Microsoft.Extensions.FileProviders.Physical | 8.0.0 | net6.0 |
| Microsoft.Extensions.FileProviders.Physical | 10.0.11 | net8.0 / net10.0 / netstandard2.0 / netstandard2.1 |

> 版本约束说明：
> - `System.DirectoryServices`：9.x/10.x 不再支持 net6.0，net6.0 用最高兼容版 8.0.0
> - `Microsoft.Extensions.FileProviders.Physical`：8.0.x 仅 8.0.0，net6.0 用 8.0.0
> - `NPOI`：2.8.0 起改为商业许可，net framework 保持免费版最高 2.5.6（net48 因 2.7.6 的 net472 资产缺 SetUnderline/CellType.Unknown 等 API 也用 2.5.6）

---

## DotNet.Util.Cache

目标框架：`net462;net47;net48;net6.0;net8.0;net10.0;netstandard2.0;netstandard2.1`

| 包 | 版本 | 适用 TFM |
|---|---|---|
| Microsoft.Extensions.Caching.Memory | 8.0.1 | net6.0 |
| Microsoft.Extensions.Caching.Memory | 10.0.11 | net8.0 / net10.0 / netstandard2.0 / netstandard2.1 |
| NewLife.Redis | 7.0.2026.1024 | 全部 |

---

## DotNet.Model

目标框架：`net462;net47;net48;net6.0;net8.0;net10.0;netstandard2.0;netstandard2.1`

| 包 | 版本 | 适用 TFM |
|---|---|---|
| System.ComponentModel.Annotations | 5.0.0 | netstandard2.0 / netstandard2.1 |

---

## DotNet.Util.Db（数据库访问基座）

目标框架：`net462;net47;net48;net6.0;net8.0;net10.0;netstandard2.0;netstandard2.1`

| 包 | 版本 | 适用 TFM |
|---|---|---|
| System.Data.SqlClient | 4.9.1 | net6.0 / net8.0 / netstandard2.0 / netstandard2.1 |
| System.ValueTuple | 4.6.2 | net462 / net47 / net48 / net6.0 / net8.0 / netstandard2.0 / netstandard2.1（不含 net10.0） |

> 注：本项目目标框架虽含 net10.0，但上述两包的引用条件均未覆盖 net10.0（net10.0 下无 ValueTuple/SqlClient 包引用）。

---

## DotNet.Util.Db.SQLite

目标框架：`net462;net47;net48;net6.0;net8.0;net10.0;netstandard2.0;netstandard2.1`

| 包 | 版本 | 适用 TFM |
|---|---|---|
| Stub.System.Data.SQLite.Core.NetFramework | 1.0.119 | net462 |
| System.Data.SQLite.Core | 1.0.119 | net462 |
| Microsoft.Data.Sqlite | 10.0.14 | net47 / net48 / net6.0 / net8.0 / net10.0 / netstandard2.0 / netstandard2.1 |
| System.Data.SQLite | 2.0.4 | net47 / net48 / net6.0 / net8.0 / net10.0 / netstandard2.0 / netstandard2.1 |
| System.ValueTuple | 4.6.2 | 全部 |

---

## DotNet.Util.Db.Oracle

目标框架：`net462;net47;net48;net6.0;net8.0;net10.0;netstandard2.0;netstandard2.1`

| 包 | 版本 | 适用 TFM |
|---|---|---|
| Oracle.ManagedDataAccess | 19.22.0 | net462 |
| Oracle.ManagedDataAccess | 21.20.0 | net462 / net47 |
| Oracle.ManagedDataAccess | 23.26.300 | net48 |
| Oracle.ManagedDataAccess.Core | 2.19.290 | netstandard2.0 |
| Oracle.ManagedDataAccess.Core | 23.26.300 | netstandard2.1 / net6.0 / net8.0 / net10.0 |
| System.ValueTuple | 4.6.2 | net462 / net47 / net48 |

---

## DotNet.Util.Db.MySql

目标框架：`net462;net47;net48;net6.0;net8.0;net10.0;netstandard2.0;netstandard2.1`

| 包 | 版本 | 适用 TFM |
|---|---|---|
| MySql.Data | 8.0.32.1 | net462（26.x 不支持 net46，用此版） |
| MySql.Data | 26.7.0 | net47 / net48 / net6.0 / net8.0 / net10.0 / netstandard2.0 / netstandard2.1 |
| System.ValueTuple | 4.6.2 | 全部 |

> 注：csproj 中 `MySql.Data 8.0.32.1` 的条件写的是 `net462 or net461`，但项目 TFM 无 net461，实际仅 net462 命中。net462 已从 26.7.0 组排除以避免 NU1504 重复引用。

---

## DotNet.Util.Db.OleDb

目标框架：`net462;net47;net48;net6.0;net8.0;net10.0;netstandard2.0;netstandard2.1`

| 包 | 版本 | 适用 TFM |
|---|---|---|
| System.ValueTuple | 4.6.2 | net462 / net47 / net48 |

---

## DotNet.Util.Db.PostgreSql

目标框架：`net462;net47;net48;net6.0;net8.0;net10.0;netstandard2.1`

| 包 | 版本 | 适用 TFM |
|---|---|---|
| Npgsql | 4.0.17 | net462 |
| Npgsql | 8.0.3 | net47 / net48 / netstandard2.1 |
| Npgsql | 9.0.4 | net6.0 |
| Npgsql | 10.0.3 | net8.0 / net10.0 |
| System.Text.Json | 8.0.5 | net47 / net48 |
| System.ValueTuple | 4.6.2 | net462 / net47 / net48 |

> 注：csproj 中另有 `Npgsql 4.1.14 (net461)` 与 `Npgsql 8.0.3 (netstandard2.0)` 的条件项，但本项目目标框架列表不含 net461/netstandard2.0，这两组条件实际不命中，故上表未计入。

---

## DotNet.Business

目标框架：`net462;net47;net48;net6.0;net8.0;net10.0;netstandard2.0;netstandard2.1`

| 包 | 版本 | 适用 TFM |
|---|---|---|
| （无直接 NuGet 包引用） | — | — |

---

## DotNet.Business.Web

目标框架：`net462;net47;net48;net6.0;net8.0;net10.0;netstandard2.0;netstandard2.1`

| 包 | 版本 | 适用 TFM |
|---|---|---|
| JWT | 11.1.0 | 全部 |

---

## DotNet.Web.UI.BasePage

目标框架：`net462;net47;net48`

| 包 | 版本 | 适用 TFM |
|---|---|---|
| JWT | 11.1.0 | 全部 |

---

## DotNet.Util.Tests（单元测试）

目标框架：`net8.0;net48`

| 包 | 版本 | 适用 TFM |
|---|---|---|
| xunit | 2.9.3 | 全部 |
| xunit.runner.visualstudio | 4.0.0 | 全部 |
| Microsoft.NET.Test.Sdk | 18.9.0 | net8.0 |
| coverlet.collector | 10.0.1 | net8.0 |
| Microsoft.NET.Test.Sdk | 16.11.0 | net48 |
| Microsoft.NETFramework.ReferenceAssemblies | 1.0.3 | net48 |

---

## DotNet.Util.Db.Tests（数据库测试）

目标框架：`net8.0`

| 包 | 版本 | 适用 TFM |
|---|---|---|
| Microsoft.NET.Test.Sdk | 18.9.0 | 全部 |
| xunit | 2.9.3 | 全部 |
| xunit.runner.visualstudio | 4.0.0 | 全部 |
| coverlet.collector | 10.0.1 | 全部 |

---

## DotNet.Test（控制台测试，net10.0）

目标框架：`net10.0`

| 包 | 版本 | 适用 TFM |
|---|---|---|
| System.Memory | 4.6.3 | 全部 |
| System.Runtime.CompilerServices.Unsafe | 6.1.2 | 全部 |

---

## DotNet.Test.46（旧式 packages.config 控制台示例，net462）

目标框架：`net462`（旧式非 SDK 项目，使用 `packages.config` + `HintPath`）

| 包 | 版本 | 适用 TFM |
|---|---|---|
| Aspose.Words | 25.9.0 | net462 |
| FreeSpire.Doc | 12.2.0 | net462 |
| NewLife.Core | 12.0.2026.1024 | net462 |
| NPOI | 2.5.6 | net462 |
| Portable.BouncyCastle | 1.9.0 | net462 |
| SharpZipLib | 1.3.3 | net462 |
| System.Buffers | 4.6.1 | net462 |
| System.Memory | 4.6.3 | net462 |
| System.Numerics.Vectors | 4.6.1 | net462 |
| System.Runtime.CompilerServices.Unsafe | 6.1.2 | net462 |
| System.Threading.Tasks.Extensions | 4.6.3 | net462 |
| System.ValueTuple | 4.6.2 | net462 |

> 注：本项目为示例工程，不参与 NuGet 发布（已提供空 `Pack` 目标）。
