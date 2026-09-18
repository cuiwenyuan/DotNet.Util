# DotNet.Util 数据库测试补齐计划

> 目标：补齐 `DbUtil` / `DbHelper` 中**必须连真实数据库**才能覆盖的路径。
> 现状：仅 `DbHelperIntegrationTests.cs` 4 个用例（`Open` / `ExecuteScalar` / `Fill` / `ExecuteNonQuery`），
> 已于 2026-09-18 在本机 SQL Server 上双档（net8.0 + net48）实测 4/4 通过。
> 已确认方案：允许测试代码自建表 / 单一白名单 / 逐阶段确认 / P5 纳入 / 破坏性用例照测。
> 状态：**P1~P5 全部完成（2026-09-18）**，计划收官。

---

## 一、缺口量化

`src/DotNet.Util.Db/` 下 `DbUtil` 共 **18 个 partial 文件、169 个公开重载**，其中绝大多数是
`this IDbHelper dbHelper` 扩展方法，必须连库。目前连库路径覆盖率为 **0**。

| 文件 | 重载数 | 主要方法 | 需真实库 |
|---|---:|---|---|
| `DbUtil.Common.cs` | 22 | `Insert` `UpdateRecord` `GetFromProcedure`×2 `GetWhereString`×2 | ✅ |
| `DbUtil.Method.cs` | 20 | `ExecuteNonQuery`×2 `ExecuteScalar`×2 `ExecuteReader`×2 `Fill`×2 `ExecuteCommandWithSplitter`×2 | ✅ |
| `DbUtil.ParentChildrens.cs` | 20 | 父子表遍历系列 | ✅ |
| `DbUtil.cs` | 18 | `GetDbHelper` `ToDbTime` `GetParameter`（部分为纯逻辑，已覆盖） | 部分 |
| `DbUtil.ExecuteReader.cs` | 14 | `ExecuteReader` 重载群 | ✅ |
| `DbUtil.GetDataTableByPage.cs` | 12 | 分页查询 6 个重载 | ✅ |
| `DbUtil.Exists.cs` | 10 | `GetCount`×2 `Exists` `TableExists` `SequenceExists` | ✅（末者除外） |
| `DbUtil.ExecuteReaderByPage.cs` | 10 | 分页 DataReader 5 个重载 | ✅ |
| `DbUtil.Aggregate.cs` | 6 | `AggregateInt` `AggregateDecimal` `AggregateDateTime` | ✅ |
| `DbUtil.GetDataTable.cs` | 6 | `GetDataTable` 3 个重载 | ✅ |
| `DbUtil.IsUpdate.cs` | 6 | `IsUpdate` 3 个重载（依赖约定字段） | ✅ |
| `DbUtil.Delete.cs` | 5 | `Delete`×2 `BatchDelete` `Truncate` | ✅ |
| `DbUtil.Ado.cs` | 4 | `CloseConnection` `Reopen` `IsOpen` `IsClose` | ✅ |
| `DbUtil.Count.cs` | 4 | `Count` `DistinctCount` | ✅ |
| `DbUtil.GetProperties.cs` | 4 | `GetProperties`×2 | ✅ |
| `DbUtil.LockNoWait.cs` | 4 | `LockNoWait`×2 | ✅ |
| `DbUtil.GetProperty.cs` | 2 | `GetProperty` | ✅ |
| `DbUtil.SetProperty.cs` | 2 | `SetProperty` | ✅ |

**已覆盖（纯逻辑，不需连库）**：`GetCommandType` `GetDbHelperClass` `GetDbHelperDll`
`GetDbNow` `ToDbTime` `GetParameter` `SqlSafe` `GetSafeSortDirection` `GetSafeSortExpression`
`ToTableName` —— 由 `DbUtilStaticTests`（7）/ `DbUtilMethodTests`（3）覆盖。

---

## 二、测试基建设计

### 2.1 连接串与环境开关

沿用现有约定：环境变量 `DUP_TEST_SQLSERVER`，未设置时 `Assert.Fail` 明确提示（不静默跳过）。

已验证可用的连接串：

```
Server=127.0.0.1,1433;Database=DotNetUtilTest;Trusted_Connection=True;TrustServerCertificate=True;
```

### 2.2 ⚠️ 安全护栏（强制）

`Truncate` / `BatchDelete` / `Delete` 会**清空整表**。计划增加一个连接串白名单校验：

```csharp
// 解析连接串的 Database / Initial Catalog，必须属于允许列表
// 非白名单库直接 Assert.Fail，防止误连业务库被清空
private static readonly string[] AllowedDatabases = { "DotNetUtilTest" };
```

这是本计划最重要的一条 —— 缺了它，一旦有人把环境变量指向业务库，跑一次测试就会清表。

### 2.3 测试表设计

需覆盖 `IsUpdate` 依赖的约定字段（`BaseUtil.Field.cs`：`Id` `ParentId` `Code` `Name`
`Enabled` `Deleted` `SortCode` `CreateUserId` `CreateTime` `UpdateUserId` `UpdateTime`），
以及 `ParentChildrens` 需要的 `ParentId` 自引用。

```sql
CREATE TABLE DupTestUser (
    Id            INT IDENTITY(1,1) PRIMARY KEY,
    ParentId      INT NULL,
    Code          NVARCHAR(50) NULL,
    Name          NVARCHAR(50) NULL,
    Age           INT NULL,
    Score         DECIMAL(18,2) NULL,
    Enabled       BIT NULL,
    Deleted       BIT NULL,
    SortCode      INT NULL,
    CreateUserId  NVARCHAR(50) NULL,
    CreateTime    DATETIME NULL,
    UpdateUserId  NVARCHAR(50) NULL,
    UpdateTime    DATETIME NULL
);
```

另建第二张表 `DupTestOrder`（含 `UserId` 外键语义）用于 `GetProperties` / 多表场景。

### 2.4 并行隔离（必须）

`DbUtil.ConnectionString` 与 `DbUtil.CurrentDbType` 是 **public static 字段**
（`DbUtil.cs:179/184`，默认取 `BaseSystemInfo`），并行测试会互相污染；测试表也是共享资源。

→ 新建 `SqlServerTestCollection`（仿照现有 `MsgTestCollection`，
`[CollectionDefinition(DisableParallelization = true)]`），**所有连库测试类统一标注**。

### 2.5 数据生命周期

- 类级 `IClassFixture` 建表（幂等：`IF NOT EXISTS`）
- 每个用例结束后 `DELETE` 清空（不用 `Truncate`，除非在专门用例里测）
- 全部跑完可选 drop（默认保留，便于排查）

---

## 三、分阶段执行计划

| 阶段 | 内容 | 预估用例 | 说明 |
|---|---|---:|---|
| **P1** ✅ | 基建：`SqlServerTestFixture`（连接串 gate + 库名白名单 + 建表/清表 + 种子数据）、`SqlServerTestCollection`、建表 SQL | 26 | **已完成**（原估 0，实际产出守卫 17 + 基建自测 9） |
| **P2** ✅ | 核心执行 `DbUtil.Method.cs`：`ExecuteNonQuery`×2 `ExecuteScalar`×2 `ExecuteReader`×2 `Fill`×2 `ExecuteCommandWithSplitter`×2 `MakeParameter` | 22 | **已完成**（原估 ~15，实际 22） |
| **P3** ✅ | 查询类：`GetDataTable`×3 `GetDataTableByPage`×5 `Count` `DistinctCount` `GetCount`×2 `Exists` `TableExists` | 24 | **已完成**（原估 ~25）。1 个存储过程重载留 P5 |
| **P4** ✅ | 写入类：`Insert` `UpdateRecord` `Delete`×2 `BatchDelete` `Truncate` `SetProperty` `GetProperty` `GetProperties`×2 `GetWhereString`×2 | 27 | **已完成**（原估 ~18）。含破坏性操作，护栏已端到端验证 |
| **P5** ✅ | 高级：`AggregateInt/Decimal/DateTime` `IsUpdate`×2 `LockNoWait`×2 `ParentChildrens` `GetFromProcedure`×2 `Ado`×4，以及 P3 遗留的存储过程分页重载 | 20 | **已完成**（原估 ~15）。net48 档跳 1 例（见下） |

**累计已完成 119 个用例**（P1 26 + P2 22 + P3 24 + P4 27 + P5 20），计划收官。

完成时：集成测试 48 → **167**；常规非集成回归 **1225**（集成用例被 filter 排除，不进常规回归）。

### P1 交付物与验证结果（2026-09-18）

新增文件（均在 `src/DotNet.Util.Tests/Db/`）：

| 文件 | 说明 | 是否进常规回归 |
|---|---|---|
| `SqlServerTestCollection.cs` | 串行集合定义（`DisableParallelization = true`） | — |
| `SqlServerTestFixture.cs` | 连接串 gate + 白名单 + 幂等建表 + 数据生命周期 | — |
| `SqlServerTestGuardTests.cs` | 17 个**纯逻辑**用例，锁死白名单行为 | ✅ 进（永远可跑） |
| `SqlServerTestFixtureIntegrationTests.cs` | 9 个连库用例，验收基建 | ❌ 被 `!~IntegrationTests` 排除 |

验证：

- net8.0：守卫 17/17、连库 9/9
- net48：守卫 + 连库合计 26/26
- **护栏端到端实测**：把 `DUP_TEST_SQLSERVER` 指向 `master`，9 个用例全部以
  「安全护栏：数据库名「master」不在白名单内」失败，未执行任何 SQL
- 全量非集成回归 **1225**（1208 常规 + 17 守卫），1 个失败为 `HttpUtilTests` 已知 flaky
  （单独复跑 8/8）

测试表：`DupTestUser`（含 `ParentId` 与 `CreateUserId`/`CreateTime`/`UpdateUserId`/`UpdateTime`
约定字段）、`DupTestOrder`，已在 `DotNetUtilTest` 库创建。

---

### P2 交付物与验证结果（2026-09-18）

新增文件：`src/DotNet.Util.Tests/Db/DbUtilMethodIntegrationTests.cs`（22 个连库用例，
**不进常规回归**，被 `!~IntegrationTests` 排除）。

`SqlServerTestFixture` 新增两项能力（支撑无连接串重载）：

| 成员 | 用途 |
|---|---|
| `UseStaticConnection()` | 临时把 `DbUtil.ConnectionString` / `DbUtil.CurrentDbType` 切到测试库，`Dispose` 时还原。`DbUtil` 里不带连接串的重载一律读这两个 public static 字段 |
| `CreateParameter(name, value)` | 由当前 provider 创建参数，跨 net48（`System.Data.SqlClient`）与 net8.0（`Microsoft.Data.SqlClient`）两档通用 |

用例分布：

| 方法 | 用例数 | 覆盖点 |
|---|---:|---|
| `ExecuteNonQuery` | 6 | 增/改/删影响行数、参数化写入取值、无匹配返回 0、静态连接串重载 |
| `ExecuteScalar` | 4 | 首行首列、参数化、空结果返回 null、静态连接串重载 |
| `ExecuteReader` | 3 | 流式读取、参数化过滤、静态连接串重载（用完必须 Dispose，底层 `CommandBehavior.CloseConnection`） |
| `Fill` | 4 | 填充 DataTable、参数化、空结果保留列且 `TableName == "DotNet"`、静态连接串重载 |
| `MakeParameter` | 2 | 参数名规范化为 `@Name`、`null` 转 `DBNull.Value` |
| `ExecuteCommandWithSplitter` | 3 | 默认 `\r\nGO\r\n`、自定义分隔符、空脚本不执行 |

验证：

- net8.0：P2 单独 22/22；Db 命名空间全量 **217/217**
- net48：P2 单独 22/22；集成部分（P1 26 + P2 22 + 原有 `DbHelperIntegrationTests` 4）**52/52**
- 常规全量非集成回归 **1225**，1 个失败为 `HttpUtilTests.Post_NullParam_ReturnsEcho`
  （已知并行 flaky，失败点轮换，单独复跑 8/8）

⚠️ **net48 档的运行限制**：一次性跑 Db 命名空间全量（217 例）会被会话信号中断，
需按 filter 拆分跑（集成部分单独跑可过）。P3~P5 沿用此方式。

---

### P3 交付物与验证结果（2026-09-18）

新增文件：`src/DotNet.Util.Tests/Db/DbUtilQueryIntegrationTests.cs`（24 个连库用例，
**不进常规回归**）。

用例分布：

| 方法 | 用例数 | 覆盖点 |
|---|---:|---|
| `GetDataTable` | 6 | IN 查询、空值数组退化 `IS NULL`、`KeyValuePair` 条件、`TOP` 限制、`conditions` 过滤、指定 `selectField` |
| `GetDataTableByPage` | 7 | 自定义 SQL 分页、带 `condition` 分页、表名分页、带参数分页、`out recordCount`、`maxOutPut` 截断、末页余数 |
| `Count` | 2 | 无条件总数、带条件过滤 |
| `DistinctCount` | 1 | 去重字段计数（同值字段 → 1，唯一字段 → 5） |
| `GetCount` | 4 | `KeyValuePair` 参数、`condition`、`condition + 参数`、追加排除条件 `<>` |
| `Exists` | 2 | 命中 / 未命中 |
| `TableExists` | 2 | 存在表 / 不存在的表 |

**留到 P5 的 1 个重载**：`GetDataTableByPage(out recordCount, pageNo, pageSize, sortExpression,
sortDirection, tableName, condition, selectField)`（`DbUtil.GetDataTableByPage.cs:162`）
调用**存储过程 `GetRecordByPage`**，需先建 sp，与 `GetFromProcedure` 一并处理。

⚠️ **三个踩坑点（写 P4/P5 时直接复用）**：
1. `TableExists` 内部 SQL 是 `object_id(N'[{0}]')` —— **只能传纯表名**，带 `[dbo].` 前缀会查不到。
2. `GetSafeSortDirection(null)` 回退为 **`DESC`**（不是 ASC）；`GetSafeSortExpression` 为空时
   回退为 `BaseUtil.FieldCreateTime`。分页断言要显式传 `ASC` 或按 DESC 预期。
3. `SqlHelper.CurrentDbType` 是只读属性、恒为 `SqlServer` → 分页一定走 SqlServer 分支，
   无需在测试里额外设置。

验证：

- net8.0：P3 单独 24/24；Db 命名空间全量 **241/241**
- net48：P3 单独 24/24；集成部分（P1 26 + P2 22 + P3 24 + 原 `DbHelperIntegrationTests` 4）**76/76**
- 常规全量非集成回归 **1225 全绿**（无失败）

---

### P4 交付物与验证结果（2026-09-18）

新增文件：`src/DotNet.Util.Tests/Db/DbUtilWriteIntegrationTests.cs`（27 个连库用例，
**不进常规回归**，被 `!~IntegrationTests` 排除）。

用例分布：

| 方法 | 用例数 | 覆盖点 |
|---|---:|---|
| `Insert` | 3 | 字段值写入并返回影响行数 1、null 值落地为 `NULL` 字面量、`decimal`/`DateTime` 类型原值持久化 |
| `UpdateRecord` | 3 | 按主键字段更新、null 值生成 `SET x = NULL`、无匹配返回 0 |
| `SetProperty` | 3 | KeyValuePair 条件更新、附加手写 `whereSql`（不成立→0 / 成立→1）、无匹配返回 0 |
| `Delete` | 5 | KeyValuePair 单值、数组值→`IN (...)`、null 值→`IS NULL`、纯条件字符串、`parameters=null` 整表删除 |
| `BatchDelete` | 2 | 按 `batchSize` 递归删完匹配行、条件不匹配时一行不动 |
| `Truncate` | 2 | 清空表、重置 IDENTITY 种子（新行 Id 回到 1） |
| `GetProperty` | 3 | 取值、无匹配返回空串、`TOP 1 + ORDER BY` |
| `GetProperties` | 4 | 数组 IN 版本、空数组返回空、带 `topLimit`（SqlServer 走 `SELECT TOP n`，非 DISTINCT）、不带限制走 `DISTINCT` |
| `GetWhereString` | 2 | 两个重载拼装的 SQL 文本（不执行）；`ref names` 版本会把 null/数组对应的 name 置空 |

验证：

- net8.0：P4 单独 **27/27**；Db 命名空间全量 **268/268**
- net48：P4 单独 **27/27**；集成部分（P1 连库 9 + P2 22 + P3 24 + P4 27 + 原 `DbHelperIntegrationTests` 4）**86/86**
- **护栏端到端实测**：把 `DUP_TEST_SQLSERVER` 指向 `master`，27 个用例全部以
  「安全护栏：数据库名「master」不在白名单内」失败，未执行任何 SQL
- 常规全量非集成回归 **1225**，1 个失败为 `HttpUtilTests.Get_ReturnsBody`
  （已知并行 flaky，`(410) Gone`；单独复跑 8/8）

⚠️ **踩坑点（写 P5 时复用）**：
1. `Delete(tableName, null)` 存在 **重载二义性**（`List<KeyValuePair>` 版 vs `string` 版），
   必须显式转换：`db.Delete(Table, (List<KeyValuePair<string, object>>)null!)`。
2. `SqlBuilder.SetWhere(List<KeyValuePair>)` 对 **null 集合会 NRE**（直接 foreach），
   `SetProperty` 的 `whereParameters` 不能传 null。
3. `BatchDelete` 递归终止依赖 `AggregateInt(..., function: "MIN")`，表空或条件不匹配时
   返回 0 即停止；它无返回值、用 `Console.WriteLine` 输出批次信息，测试只能断言剩余行数。
4. `TRUNCATE` 会重置 IDENTITY —— 若后续用例断言自增 Id，需考虑这个副作用
   （本阶段专门用一个用例锁死该行为）。

---

### P5 交付物与验证结果（2026-09-18，收官）

新增文件：`src/DotNet.Util.Tests/Db/DbUtilAdvancedIntegrationTests.cs`（20 个连库用例，
**不进常规回归**）。

`SqlServerTestFixture` 新增能力：`EnsureProcedures()` —— 幂等创建 `DupTestUserList`
（无参）与 `DupTestUserGetById`（`@Id`）两个测试存储过程（`CREATE PROCEDURE` 必须是批处理
首语句，故用 `EXEC('...')` 包裹），支撑 `GetFromProcedure` 两个重载。

用例分布：

| 方法 | 用例数 | 覆盖点 |
|---|---:|---|
| `GetDataTableByPage`（sp 版） | 3 | 首页 `SELECT TOP n` 分支、次页 `ROW_NUMBER` 分支（多一列 `ROWS`）、带 `condition` 分页；均校验 `out recordCount` |
| `GetFromProcedure` | 2 | 无参存储过程、带 `@Id` 参数存储过程（内部用 `BaseUtil.FieldId` 作参数名） |
| `AggregateInt` | 2 | SUM/MIN/MAX、带 condition 聚合 |
| `AggregateDecimal` | 1 | SUM(Score)（net48 档跳过，见下） |
| `AggregateDateTime` | 1 | MAX(CreateTime) |
| `IsUpdate` | 3 | 未修改 false、他人修改 true、时间更新 true、按非主键字段（Code）定位 |
| `LockNoWait` | 2 | List 版与 params 版（SQL Server 上执行报错 → 返回 -1；缺陷 1 修复前恒为 0） |
| `Ado` 连接扩展 | 2 | `IsOpen`/`IsClose`、`CloseConnection`/`Reopen` |
| `ParentChildrens` | 4 | `GetChildrens` 整棵子树、`GetChildrens(idOnly)` 只含后代、`GetChildrensByCode`、`GetParentsByCode` |

**环境依赖（比 P1~P4 多两项）**：
1. `GetRecordByPage` 存储过程必须已存在于 `DotNetUtilTest`（由使用者手工创建，分页 sp 重载调用它）；
2. `DupTestUserList` / `DupTestUserGetById` 由 fixture 自动创建。

验证：

- net8.0：P5 单独 **20/20**；Db 命名空间全量 **288/288**
- net48：P5 单独 **19 通过 + 1 跳过**；集成部分（P1 连库 9 + P2 22 + P3 24 + P4 27 + P5 20
  + 原 `DbHelperIntegrationTests` 4）**105 通过 + 1 跳过 = 106**
- **护栏端到端实测**：把 `DUP_TEST_SQLSERVER` 指向 `master`，20 个用例全部以
  「安全护栏：数据库名「master」不在白名单内」失败，未执行任何 SQL
- 常规全量非集成回归 **1225 全绿**（0 失败）

#### P5 过程中发现的缺陷

| # | 位置 | 问题 | 严重度 | 状态 |
|---|---|---|---|---|
| 1 | `DbUtil.LockNoWait.cs` | `dbHelper.Fill(dt, ...)` 的**返回值被丢弃**。`DbHelper.Fill` 内部 catch 异常后把自己的局部变量置 `null` 并返回（`DbHelper.Method.cs:677`），调用方 `dt` 仍是空表 → 异常被静默吞掉，`result` 恒为 0，方法里 `catch` 分支的 `-1` 永远不可达。SQL Server（不支持 `FOR UPDATE NOWAIT`）上表现为"永远返回 0 且不报错" | 🟠 | ✅ **已修复（2026-09-18）**：改为接收 `Fill` 返回值，`null` 即视为失败返回 `-1`；两个用例断言同步改为 `Assert.Equal(-1, result)` |
| 2 | `NewLife.Core` 的 `Utility.ToDecimal` **与** `Utility.ToDouble` | net48（.NET Framework）档调用 `object.ToDecimal()` / `object.ToDouble()` 会让**xunit/VSTest 测试宿主在用例结束后崩溃**（返回值正确）。`ToInt` / `System.Convert.ToDecimal` 正常 | 🟡（仅影响 net48 测试宿主） | 🔍 **已升级 11.19.2026.901 验证：仍崩溃**；但独立 net48 控制台正常 → 非生产问题 |

对应处理：缺陷 2 的 `AggregateDecimal_SumScore_ReturnsExpectedValue` 已用
`#if NET48 [Fact(Skip = ...)]` 在该档跳过，缺陷 2 处理后删除条件编译即可恢复。

#### 缺陷 2 定位结论（2026-09-18 最小复现 + 打点，探针已删除）

对照实验（每个 1~2 次，net48 档）：

| 探针 | 内容 | 结果 |
|---|---|---|
| P0 | 空用例（对照） | ✅ 正常 |
| P1 | `((object)12.5m).ToDecimal()` | ❌ 宿主崩溃（2/2） |
| P2 | `Convert.ToDecimal((object)12.5m)`（BCL） | ✅ 正常 |
| P3 | `((object)"12.5").ToDecimal()` | ❌ 崩溃 |
| P4 | `((object)12.5d).ToDecimal()` | ❌ 崩溃 |
| P5 | `((object)null).ToDecimal()` | ❌ 崩溃 |
| P6 | `((object)12).ToInt()` | ✅ 正常 |
| P7 | 打点：`ToDecimal` 前/后/`Sleep(3s)` 后各写标记 | 三个标记**全部写入**、sleep 走完、断言通过 → ❌ 之后崩 |
| P8 | `global::NewLife.Utility.Convert.ToDecimal(12.5m, 0m)`（绕开扩展方法直调 `DefaultConvert`） | ❌ 崩溃 |
| P9 | `((object)12.5d).ToDouble()` | ❌ **同样崩溃** |
| P10 | `((object)"12").ToInt()`（string 分支） | ✅ 正常 |

结论：

1. **与输入类型无关、与是否走扩展方法无关** → 问题在 `DefaultConvert.ToDecimal` / `ToDouble` 这一层，
   而不是某个分支逻辑；
2. **崩溃时刻在方法返回之后**（P7 证明结果已正确得出、sleep 也走完）→ 属进程收尾期崩溃，
   表现为"业务结果正确，但宿主/进程以崩溃收场"；
3. **net8.0 同路径完全正常** → .NET Framework runtime 特有；
4. 上游源码里这两个方法的 string 分支含 `Span<Char> tmp = stackalloc Char[len]` 与 `tmp[..rs]`
   （Range 切片），在 net4x 上依赖 `System.Memory` 垫片 —— **首要嫌疑**，但定性需 dump / 异常码。

影响面（本仓库 `.ToDecimal(` + `.ToDouble(` 共 **16 处 / 9 文件**）：

- 产品代码 **9 处**：`RequestUtil.cs`×2、`NewLife/DataUtil.cs`×2（decimal + double）、
  `DbUtil.Aggregate.cs`×1、`DotNet.Util.Plus/ExcelUtil.Export.cs`×4（`ToDouble`）
- 测试代码 **7 处**（含 `DataUtilTests`、`SqlServerTestFixtureIntegrationTests`、
  `DbUtilWriteIntegrationTests`、`DbUtilAdvancedIntegrationTests`×2）
  → 意味着 **net48 全量回归只要跑到这些用例就会宿主崩溃，不只是 P5 那一例**。

处置方案待选：

| 方案 | 做法 | 权衡 |
|---|---|---|
| **A 规避** | 本库 9 处产品调用点改用 `Convert.ToDecimal` / `Convert.ToDouble`（或自有转换），net4x 不再触碰 NewLife 的这两个扩展方法 | 最低风险、立即可行；不保护下游业务代码 |
| **B 升级** | 升级 `NewLife.Core` 并验证 | 需确认上游是否已修复；多 TFM 全量回归成本 |
| **C 取证** | 用 procdump/WinDbg 抓异常码后向上游报 issue | 证据最硬，但需要额外工具与一轮时间 |

#### B（升级）执行结果 —— 2026-09-18

- 升级 `NewLife.Core` **11.18.2026.801 → 11.19.2026.901**（`src/DotNet.Util/DotNet.Util.csproj`）。
- 升级后重跑探针（`TempNet48CrashProbe`，已删除）：**`ToDecimal` 与 `ToDouble` 在 net48 测试宿主下依然崩溃**
  （现象与升级前完全一致：用例先通过，随后"测试主机进程崩溃"）。
  上游 release notes 只列出 DSAHelper/Crc16/Crc32/PerfCounter/JwtBuilder/PinYin/CacheLock/MemoryCache/
  ProcessHelper/PKCS7/Asn1 等 20+ 修复，**未涉及该问题** → B 未达成目标。
- **但升级本身是健康的**：10 个 TFM + 14 个项目构建 0 错误，net8.0 全量 1225、Db 集成 288、
  net48 集成 105+1 跳过，全部通过，无 NU1605 版本冲突（`DotNet.Util.Cache` 的 `NewLife.Redis 6.6.2026.801`
  与新版本共存正常）。

#### 关键补充：崩溃仅限测试宿主，不影响真实应用

额外做了**工程树外的独立 net48 控制台**验证（引用同版本 `NewLife.Core`，调用 `((object)12.5m).ToDecimal()`）：

```
start
value=12.5
end
EXITCODE=0
```

→ 普通 .NET Framework 程序调用完全正常、退出码 0。
说明该崩溃是 **xunit/VSTest 测试宿主特有**（大概率与其 AppDomain 卸载 / 后台线程收尾有关），
**生产/net4x 终端用户不受影响**。因此本缺陷严重度由 🔴 下调为 🟡（只影响 net48 档的单元测试可执行性）。

遗留影响：net48 档**全量**回归只要执行到 `ToDecimal`/`ToDouble` 路径（产品 9 处 + 测试 7 处）
就会宿主崩溃。当前 `DbUtilAdvancedIntegrationTests.AggregateDecimal_SumScore_ReturnsExpectedValue`
仍以 `#if NET48 [Fact(Skip = ...)]` 跳过；其余含该路径的用例需按 filter 规避或改用
本库自有安全转换（即方案 A）。

**缺陷 1 的同类模式（未修，供评估）**：以下调用点同样"丢弃 `Fill` 返回值、继续用传入的 `dt`"，
成功路径等价，但一旦 `Fill` 内部吞异常返回 `null`，异常会同样被静默吞掉（表现为空表而非报错）：

- `DotNet.Util.Db`：`SQLBuilder.cs:660`、`DbUtil.Common.cs:356,377`（存储过程版）、
  `DbUtil.Method.cs:162`、`DbUtil.ParentChildrens.cs:70,112,140,238,291`
- `DotNet.Business`：`BaseExceptionManager.Manual.cs:159`、`BaseManager.PreviousNext.cs:53`

彻底方案是让 `DbHelper.Fill` 在异常时改为抛出（或提供 `TryFill`），属**行为变更**，需单独评估。

---

## 四、明确排除（不测）

| 方法 | 原因 |
|---|---|
| `SequenceExists` | Oracle 专属，且 `autoCreate=true` 会**在 Oracle 上创建序列**。在 SQL Server 上语义不成立，测试无意义 |
| `DbUtil.Aggregate` 的 Oracle/MySql 分支 | 无对应数据库实例 |
| `GetFromProcedure` 的复杂参数版本 | 需要建存储过程（`P5` 视情况只测最简单的无参版本） |

---

## 五、风险与对策

| 风险 | 等级 | 对策 |
|---|---|---|
| 误连业务库被 `Truncate` 清空 | 🔴 | 库名白名单（2.2），未命中直接 Fail |
| 并行导致静态字段/测试表污染 | 🟠 | `SqlServerTestCollection` 全局串行 |
| `LockNoWait` 在 SQL Server 上的锁语义（`UPDLOCK/NOWAIT`）可能超时 | 🟠 | 单独超时控制，失败降级为"验证不抛异常" |
| `ParentChildrens` 20 个重载需父子表且逻辑复杂 | 🟡 | P5 只覆盖主路径，不追求全覆盖 |
| net48 档 `System.Data.SqlClient`（框架版）行为差异 | 🟡 | 每阶段跑双档，不一致时按 TFM 分支断言 |
| 建表 DDL 需要 db_owner 权限 | 🟡 | Windows 集成认证当前账号应为 sysadmin，如遇权限不足改用 `dbo` 前缀或预建表 |

---

## 六、需要你拍板

1. **测试表命名与 DDL**：允许测试代码自行 `CREATE TABLE`（库内建 `DupTestUser` / `DupTestOrder`），
   还是你手工预建、测试只读写？
2. **库名白名单**：是否就用 `DotNetUtilTest` 单一白名单？（我倾向严格单一）
3. **阶段粒度**：P1→P2→P3→P4→P5 逐阶段确认推进，还是 P1+P2 一起做完再看？
   （用户历史偏好：分阶段按优先级单点推进，每阶段结束确认）
4. **P5 取舍**：`ParentChildrens`（20 重载、需父子表、逻辑最复杂）与
   `GetFromProcedure`（需建存储过程）是否要纳入？还是只做到 P4 收尾？
5. **破坏性用例**：`Truncate` / `BatchDelete` 是否测？（它们会清空测试表，
   在白名单库内是安全的，但会让后续用例依赖顺序）

---

## 七、验收标准

- 每个阶段结束：net8.0 + net48 **双档全绿**
- 未设置 `DUP_TEST_SQLSERVER` 时，全部用例以明确的 `Assert.Fail` 提示（不静默跳过、不误判为通过）
- 全量非集成回归无新增失败（除已知 `HttpUtilTests` / `CacheUtilTests` 并行 flaky）
- 未设置环境变量时，CI/他人机器跑测试的体验与现状一致（不因新增用例而变红）
