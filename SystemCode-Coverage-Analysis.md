# SystemCode 双模式区分 — 覆盖率与潜在 Bug 分析

> 分析对象：`DotNet.Business`（含 `DotNet.Model` / `DotNet.Util` / `DotNet.Business.Web` 相关）
> 初版日期：2026-09-18
> 最后更新：2026-09-19（用户精简删除 `BasePermissionScope` 子系统后对齐当前代码）
> 目标：找出所有使用 SystemCode 区分子系统的地方，并判断最终读/写表的 SQL 是否同时支持两种模式。

---

## 0. 两种模式的实现机制（先对齐口径）

用户描述的两种区分方式，在本库里由 `BaseSystemInfo.UseBaseTable` 开关 + `GetXTableName(systemCode)` 表名函数实现：

| 模式 | 开关 | 物理表名来源 | 子系统区分手段 |
|------|------|--------------|----------------|
| **Mode A（分表模式）** | `UseBaseTable = false`（**默认值**） | `systemCode + "X"`，如 `BusinessModule`、`BusinessUserRole`、`BusinessPermission` | 靠**表名本身**区分子系统 |
| **Mode B（列模式）** | `UseBaseTable = true` | 固定 `BaseX`，如 `BaseModule`、`BaseUserRole` | 靠 **`SystemCode` 列**过滤区分子系统 |

- 表名函数（`Util/BaseManager.TableName.cs`）：现存活 **5 个** —— `GetModuleTableName` / `GetPermissionTableName` / `GetRoleTableName` / `GetRoleOrganizationTableName` / `GetUserRoleTableName`（`systemCode` 为空时回退为 `"Base"`）。
- ⚠️ **已删除的表名函数**：`GetPermissionScopeTableName`（于 commit `8818ee3` 删除）、`GetOrganizationScopeTableName`（更早清理，当前源码无定义亦无调用）。初版文档 §0 所列 7 个方法现仅存 5 个。
- 模式开关：`BaseSystemInfo.UseBaseTable`（默认 `false`），由配置 `appSettings["UseBaseTable"]` 决定。
- **Schema 事实（关键）**：`scripts/UserCenter.SQLServer.2008R2.sql` 中 `Base*` 与 `Business*` 两族表**都建有 `SystemCode` 列**（`nvarchar(50)`）。因此 `AND SystemCode = '...'` 这个过滤条件在两种模式下语法都合法。

---

## 1. 覆盖率矩阵（按功能点）

✅ = 两种模式都正确；🟠 = Mode B 跨系统泄漏 / Mode A 依赖构造；🔴 = 两种模式都错（安全相关）；➖ = 不参与子系统区分 / 已删除

| # | 功能点 / 方法 | 文件:行 | 读/写 | Mode A 表名 | Mode B 表名 | 带 SystemCode 过滤 | 结论 |
|---|--------------|---------|-------|-------------|-------------|-------------------|------|
| 1 | `BaseModuleManager.GetDataTableByPage` | `BaseModule/BaseModuleManager.Manual.cs:210-482` | 读 | `GetModuleTableName` | `BaseModule` | ✅ 无条件 `AND SystemCode='x'`（:253）+ 递归子查询(:356/359/360) + `INNER JOIN …SystemCode=…SystemCode`（:415/:470） | ✅ 两者都支持 |
| 2 | `GetModuleTree` / `GetEntitiesByCache` / `GetEntityByCache` | `BaseModuleManager.Manual.cs:675/883/792` | 读 | `GetModuleTableName` | `BaseModule` | ✅ 参数含 `FieldSystemCode` | ✅ |
| 3 | `UniqueAdd` / `UniqueUpdate` | `BaseModuleManager.Manual.cs:37/86` | 写 | `CurrentTableName`（构造传入） | `BaseModule` | ✅ 去重参数含 `FieldSystemCode` | ✅（依赖正确构造） |
| 4 | `BaseUserRoleManager.GetDataTableByPage` | `BaseUserRole/BaseUserRoleManager.cs:48` | 读 | `CurrentTableName` | `BaseUserRole` | ❌ 仅 Enabled/Deleted/时间/搜索，无 SystemCode | 🟠 Mode B 跨系统泄漏 |
| 5 | `BaseUserRoleManager.GetDataTable(myCompanyOnly)` | `BaseUserRole/BaseUserRoleManager.cs:105-117` | 读 | `CurrentTableName` | `BaseUserRole` | ❌ 仅 Enabled/Deleted；缓存键 `"Dt."+CurrentTableName+…`（:114）无 systemCode | 🟠 Mode B 泄漏 + 跨系统缓存污染 |
| 6 | `BaseUserRoleManager.Manual` 的 `GetList` | `BaseUserRole/BaseUserRoleManager.Manual.cs:133-140` | 读 | `CurrentTableName` | `BaseUserRole` | ✅ `whereParameters` 含 `FieldSystemCode, systemCode` | ✅（依赖正确构造） |
| 7 | `BaseRoleManager` 系列（含 `GetDataTableByPage`、角色权限子查询） | `BaseRole/BaseRoleManager.Manual.cs` 多处 | 读/写 | 模板 `FROM BaseUserRole` + `.Replace("BaseUserRole", userRoleTableName)`（:812/:829）；`AND SystemCode='x'`（:282） | `BaseRole` | ✅ 表名经 `.Replace` 切换 + 列过滤 | ✅（但拼接写法见 #7） |
| 8 | `BasePermissionManager`（cs/User/Role/Manual） | `BasePermission/*` | 读/写 | `GetPermissionTableName` | `BasePermission` | ✅ 参数化 `FieldSystemCode = GetParameter(FieldSystemCode)`（:327/:658） | ✅ |
| 9~13 | **`BasePermissionScopeManager` 全部方法**（GetUserIdsSql / CheckResourcePermissionScope / CheckRolePermissionScope / GetRoleIdsSql / GetDataTableByPage 等） | — | — | — | — | — | ➖ **已删除（2026-09-19 提交 `0cb4906` 整删子系统）** |
| 14 | `BaseLogonLogManager` 系列 | `BaseLogonLog/*` | 读/写 | `CurrentTableName` + `FieldSystemCode` 过滤（:64/:171） | `BaseLogonLog` | ✅（拼接写法，见 #7） | ✅（依赖正确构造） |
| 15 | `BaseOperationLogManager` / `BaseParameterManager` | `BaseOperationLog/*`、`BaseParameter/*` | 读/写 | `CurrentTableName` + `SystemCode` 赋值/过滤（:97/:138） | `BaseOperationLog`/`BaseParameter` | ✅（拼接写法，见 #7） | ✅（依赖正确构造） |
| 16 | `BaseUserLogonManager` 写 | `BaseUserLogon/BaseUserLogonManager.Manual.cs:840-919` | 写 | `CurrentTableName` | `BaseUserLogon` | ✅ 参数化 `FieldSystemCode` | ✅ |
| 17 | `BaseOrganizationManager` | `BaseOrganization/BaseOrganizationManager.cs` | — | — | — | ➖ 仅注释提 `SystemCode`（:809），实际不参与子系统区分 | ➖ |
| 18 | `BaseSystemManager.GetSystemCodes` | `BaseSystem/BaseSystemManager.cs:35` | 读 | 字典 `BaseSystemCode`（子系统清单） | 同 | ➖ 与 Mode A/B 解耦 | ➖ |

---

## 2. 系统性根因

1. **Mode A 只在“构造时传入 `GetXTableName(systemCode)`”的路径生效。**
   管理器默认构造 `new BaseXManager(UserInfo)`（或 `new BaseXManager()`）时，`CurrentTableName` 落到实体常量 `BaseXxxEntity.CurrentTableName = "BaseXxx"`（统一表）。因此只要调用方没显式传 `GetXTableName(systemCode)`，Mode A 的分表就被静默绕过，去查统一表——在 Mode A 下会漏掉各子系统的分表数据（**#6 根因**）。
   - 已正确传表名的：`BaseModuleManager.Manual.cs:682/893`、`BaseUserManager.Manual.Role.cs:273/670/701/1024/1059/1173`、`BasePermissionManager.User.cs:170/352`。
   - **未传表名（Mode A 静默失效）**：大量 `new BaseUserManager(UserInfo)`、`new BaseRoleManager(UserInfo)`、`new BasePermissionManager(userInfo)`、`new BaseOrganizationManager(UserInfo)`（见 `WebUtil.*`、`BasePage.*`、`BaseUserManager.Manual.*` 多处）。

2. **Mode B 只在“每条 SQL 都 `AND SystemCode='x'`”时安全。**
   Mode B 是统一表，子系统仅靠 `SystemCode` 列区分。任何读路径遗漏该过滤，就会把其它子系统的行一并捞出（跨系统泄漏）。`BaseUserRoleManager.GetDataTableByPage`/`GetDataTable`（#5）正是这种遗漏。

3. **两类不一致写法**：
   - “模板 + `.Replace("BaseXxx", tableName)`”写法（BaseRoleManager、BaseUserManager.Manual.Role.cs）→ Mode A 正确。
   - 参数化（`DbHelper.GetParameter(FieldSystemCode)`，BasePermissionManager/BaseUserLogonManager）vs 字符串拼接（其余多数）→ 后者有注入隐患且更易漏过滤（**#7**）。
   - 注：初版文档指出的 `BasePermissionScopeManager` 三处硬编码 `FROM BaseXxx`（Mode A 查错表 + Mode B 无过滤）**已随子系统整删而消失**。

---

## 3. 潜在 Bug 清单（严重度）

### 🔴 严重（两种模式都错，且涉及权限判定）
- **（原 #1 `GetUserIdsSql` / #2 `CheckResourcePermissionScope` / #3 `CheckRolePermissionScope`）** —— 这三个权限判定核心方法均位于 `BasePermissionScopeManager`，**已于 2026-09-19 提交 `0cb4906` 随 `BasePermissionScope` 子系统整删而移除**，不再是当前代码的风险点。
  - 若未来重新引入“权限范围（PermissionScope）”功能，须避免重蹈硬编码 `FROM BasePermissionScope` + 方法签名无 `systemCode` 参数的覆辙（即初版 §2.3 / §3 描述的两类错误）。

### 🟠 中（Mode B 跨系统泄漏）
- **#5 `BaseUserRoleManager.GetDataTableByPage` / `GetDataTable`** —— `BaseUserRole/BaseUserRoleManager.cs:48/105`
  无 SystemCode 过滤，完全依赖 `CurrentTableName`。Mode B（`BaseUserRole` 统一表）返回所有子系统用户-角色行；缓存键 `"Dt."+CurrentTableName+companyId`（:114）不含 systemCode → 跨系统缓存污染。
  - ⚠️ 历史：2026-09-19 曾按“范围 B”在 `GetDataTable` 加 SystemCode 过滤 + 缓存键 + 参数化（见 §6.6 历史记录），**但同次“功能精简”提交已回退该改动**，当前 `GetDataTable`（:105）回到原始形态（无 `systemCode` 形参、缓存键无子系统、用 `GetDataTable(sb.Return(), null, Enabled, Deleted)`）。

### 🟡 低（健壮性 / 一致性 / 注入）
- **#6 管理器默认构造未传 `GetXTableName`** —— 见 §2.1。Mode A 静默失效，是覆盖面最大的隐性风险。与 #5 根因交汇。
- **#7 字符串拼接 `systemCode` / `userId` 进 SQL** —— 当前**仍存在的拼接点**（与模式无关，但建议统一改为参数化，对齐 `BasePermissionManager`/`BaseUserLogonManager`）：
  - `BaseLogonLogManager.Manual.cs:64` / `:171`（`FieldSystemCode + " = N'" + systemCode + "'"`）
  - `BaseRoleManager.Manual.cs:282` / `:292` / `:303` / `:318` / `:330`（含缓存键 `:949/:989/:1031/:1069`）
  - `BaseModuleManager.Manual.cs:253` / `:268` / `:280` / `:285` / `:299` / `:311` / `:316` / `:330` / `:342` / `:356` / `:359` / `:360` / `:426` / `:472`
  - `BaseParameterManager.Manual.cs:97`
  - `BaseUserManager.Manual.Role.cs:596` / `:643`
  - `BasePermissionManager.User.cs:548` / `:554`（`SELECT … WHERE (ResourceCategory = '" + GetRoleTableName(systemCode) + "') AND (PermissionId = " + permissionId + ")"`，`permissionId` 为 int，注入风险较低但仍非参数化）
  - 对比 **已参数化 ✅**：`BasePermissionManager.cs:327/:658`、`BaseUserLogonManager.Manual.cs:840/880/915`、`BaseUserManager.Manual.Role.cs:499`。
  - ⚠️ 历史：2026-09-19 曾在 `BaseUserRoleManager.GetDataTable` 做参数化（§6.6 历史），**已随精简回退**；其余拼接点从未清扫。

### ✅ 已确认正确（两种模式）
- `BaseModuleManager.GetDataTableByPage`（表名 mode-aware + 无条件 SystemCode 过滤 + 递归子查询 + INNER JOIN on SystemCode）。
- `BaseModuleManager.GetModuleTree` / `GetEntitiesByCache` / `GetEntityByCache`。
- `BasePermissionManager` 各方法（参数化 `FieldSystemCode`）。
- `BaseRoleManager` 系列（`.Replace` 模板 + `AND SystemCode`）。
- `BaseUserLogonManager` 写路径（参数化 `FieldSystemCode`）。
- `SaveEntityChangeLog`（`BaseManager.SaveEntityChangeLog.cs:48-71`，写 `BaseChangeLog.SystemCode`）。

---

## 4. 修复建议（待用户确认后再动手）

1. **🟠#5**：`BaseUserRoleManager` 两方法补 SystemCode 过滤；或将 `CurrentTableName` 强制为 `GetUserRoleTableName(systemCode)`，且缓存键加入 systemCode。因 `GetDataTableByPage` 是 `override` 虚方法（签名锁死），彻底闭环需实例级 `_systemCode` 字段或改基类虚方法签名（与 #6 交汇）。
2. **🟡#6**：约定凡子系统相关管理器一律 `new BaseXManager(DbHelper, UserInfo, GetXTableName(systemCode))`；更彻底做法把 `UseBaseTable` 逻辑下沉到 `BaseManager` 默认 `CurrentTableName`，避免依赖调用方自觉传表名。
3. **🟡#7**：拼接改参数化 `DbHelper.GetParameter`。
4. （初版 §4 的 #1/#2/#3/#4 均针对已删除的 `BasePermissionScope` 子系统，不再适用；如重新引入权限范围功能再评估。）

> 注：本仓库通用约定——改动只在本机落地、不自动提交；上述修复按“先确认范围、再增量修改”推进。

---

## 5. 修订记录（2026-09-18 实施 #1/#2/#3 后）— ⚠️ 历史，代码载体已删除

> 本节记录的是 **2026-09-18** 针对 `BasePermissionScopeManager` 权限判定核心方法（`GetUserIdsSql` / `CheckResourcePermissionScope` / `CheckRolePermissionScope` 等）的“表名 mode-aware + 兼容 SystemCode 过滤”改造。**该子系统已于 2026-09-19 提交 `0cb4906` 整删**（含 `BasePermissionScopeManager.*.cs`、`BasePermissionScopeEntity.Auto.cs`），故本节所述代码**已不存在**，仅作历史留存参考。

### 5.1 关键更正：权限范围行的 `SystemCode` 列实际未被填充
实施前核查授权写入路径发现：
- `GrantResourcePermissionScopeTarget`（两个重载）**从不设置 `SystemCode`**；
- `BasePermissionScopeEntity.SystemCode` 默认值为 `"Base"`；
- 因此 **所有权限范围行物理存储的 `SystemCode` 均为 `"Base"`**，并未按子系统区分。

### 5.2 对修复方案的影响
- 原 §4 建议的 `AND SystemCode = 'systemCode'` 过滤**不能加**：
  - Mode A（`systemCode ≠ "Base"`）会过滤掉全部行 → 权限判定恒为“拒绝”，属于**安全回归**；
  - Mode B 所有行 `SystemCode="Base"`，该过滤仅在 `systemCode="Base"` 时命中，对其它子系统同样恒拒。
- 故 #1/#2/#3 实际落地**仅做表名 mode-aware**（读路径未加 `SystemCode` 列过滤，理由见 5.1）。
- **原 §3 描述的“Mode B 跨系统泄漏”在当前数据下不会发生**：因所有权限范围行 `SystemCode` 均为 `"Base"`，统一表内不存在可区分的子系统数据。

### 5.3 待决（超出 #1/#2/#3 范围，需用户决策）
若要让 Mode B 真正按子系统用 `SystemCode` 列区分，必须先让**授权写入路径**填充 `SystemCode`，再回头加读路径过滤。这是一个更大的数据模型改动，未在本轮实施。#4/#5 的 `SystemCode` 过滤同理依赖该前提（#4 表名已 mode-aware；#5 `BaseUserRoleManager` 未动）。

### 5.4 验证（历史）
- `dotnet build DotNet.Business -f net8.0 -c Release`：**0 错误**。
- `dotnet build DotNet.Business -f net48 -c Release`：**0 错误**。
- 未自动提交/推送（遵循本仓库约定）。

---

## 6. 修订记录（2026-09-19 实施“授权写入路径填充 SystemCode”）— ⚠️ 历史，代码载体已删除

> 本节（§6.1~§6.4）记录的是 **2026-09-19** 在 `BasePermissionScopeManager` 授权写入/读路径填充 `SystemCode` 的改造。**该文件已随 `0cb4906` 整删**，以下仅作历史留存，不再反映当前代码。

### 6.1 写入路径改造（核心，历史）
改动文件：`src/DotNet.Business/BasePermissionScope/BasePermissionScopeManager.cs`（已删除）
| 方法 | 改动 |
|------|------|
| `GrantResourcePermissionScopeTarget`（两个重载） | ① 签名新增首个参数 `string systemCode`；② 实体 `SystemCode = systemCode`；③ `Exists` 去重参数新增 `FieldSystemCode = systemCode`；④ `CurrentTableName = GetPermissionScopeTableName(systemCode)`。 |
| `AddPermission(string systemCode, ...)` | ① 签名新增首个参数 `systemCode`；② 实体 `SystemCode = systemCode`；③ `CurrentTableName = GetPermissionScopeTableName(systemCode)`。 |
| `AddPermission(BasePermissionScopeEntity entity)` | ① `CurrentTableName = GetPermissionScopeTableName(entity.SystemCode)`；② 调用 `PermissionScopeExists` 传入 `entity.SystemCode`。 |
| `PermissionScopeExists` | ① 签名新增首个参数 `string systemCode`；② 去重参数新增 `FieldSystemCode = systemCode`；③ `CurrentTableName = GetPermissionScopeTableName(systemCode)`。 |

### 6.2 读路径补 `SystemCode` 过滤（历史）
读路径采用兼容过滤 `AND (SystemCode = 'systemCode' OR SystemCode = 'Base')`：
- Mode A：分表已隔离，该过滤命中分表内全部行（新行 + 历史 `Base` 行）→ 行为不变。
- Mode B：新写入的 `=子系统` 行被精确隔离；历史 `Base` 行作为“全局/共享”权限对全部子系统可见（保留升级前共享语义）。
落地位置：`GetUserIdsSql` / `GetRoleIdsSql`（原 🟠#4）/ `CheckResourcePermissionScope`（原 🔴#2）/ `CheckRolePermissionScope`（原 🔴#3）/ `GetOrganizationIdsSql` / `GetResourceScopeIds`。

### 6.3 历史 `Base` 行兼容策略（历史）
升级后新授权均带正确 `SystemCode`；存量 `SystemCode='Base'` 行在 Mode B 下作为共享权限保留。如需“严格按子系统隔离”，可执行一次性数据迁移回填真实子系统后再去除 `OR SystemCode='Base'` 兼容分支。

### 6.4 验证（历史）
- `dotnet build DotNet.Business -f net8.0 -c Release`：**0 错误**（540 警告，均为既有 CA 文化/全球化类）。
- `dotnet build DotNet.Business -f net48 -c Release`：**0 错误**。
- 未自动提交/推送（遵循本仓库约定）。

### 6.5 仍待办（滚动更新）— ⚠️ 历史，含已回退项
> 2026-09-19 第二轮曾按“范围 B + #7”落地 `BaseUserRoleManager.GetDataTable` 的 SystemCode 过滤 + 缓存键 + 参数化（见 §6.6）。**该改动已于同次“功能精简”回退**，下方列出回退后仍未闭环项：
- 🟠#5（剩余）`BaseUserRoleManager.GetDataTableByPage`（`override` 虚方法，签名锁死）→ Mode B 分页路径仍可能跨系统泄漏；`RemoveCache`（`BaseUserRoleManager.Cache.cs`）仍依赖全局 `BaseSystemInfo.SystemCode`。
- 🟡#6 管理器默认构造未传 `GetXTableName`（Mode A 静默失效）。
- 🟡#7（部分）仅 `BaseUserRoleManager.GetDataTable` 曾参数化，已回退；仓库内仍有其它拼接点（BaseModule/BaseLogonLog/BaseParameter/BaseRole/BaseUser 等）未清扫。

### 6.6 修订记录（2026-09-19 “#5 范围 B + #7 参数化”：`BaseUserRoleManager.GetDataTable` 改造）— ⚠️ 已回退，历史留存

> ⚠️ 本节记录的是 2026-09-19 在 `BaseUserRoleManager.GetDataTable` 加 SystemCode 过滤 + 缓存键 + 参数化的改造。**该改动已于同次“功能精简”提交（`0cb4906` 回退 `GetDataTable`；`8818ee3` 删除 `GetPermissionScopeTableName`）整体回退**，当前 `BaseUserRoleManager.GetDataTable`（:105）已回到原始形态（无 `systemCode` 形参、缓存键无子系统、用 `GetDataTable(sb.Return(), null, Enabled, Deleted)`）。本节仅作历史留存。

#### 6.6.1 改动点（已回退）
- 新增可选参数 `string systemCode = null`；`effectiveSystemCode = systemCode ?? BaseSystemInfo.SystemCode ?? "Base"`。
- 读过滤：WHERE 改为 `(SystemCode = @SystemCode OR SystemCode = 'Base') AND Enabled = 1 AND Deleted = 0`。
- 参数化：`SystemCode` 值经 `dbHelper.GetParameter` + `MakeParameters` 绑定；直接 `DbHelper.Fill("SELECT * FROM "+CurrentTableName+" WHERE "+sb, dbParameters)` 手动拼 OR 条件。
- 缓存键：`"Dt." + CurrentTableName + "." + effectiveSystemCode + "." + companyId + "." + (myCompanyOnly?1:0)`。

#### 6.6.2 本轮未改（范围 B 约定，历史）
- `GetDataTableByPage`（`override` 虚方法，签名锁死）：Mode B 分页路径仍可能跨系统泄漏（#5 未闭环）。
- `RemoveCache`：仍依赖全局 `BaseSystemInfo.SystemCode`，多子系统显式 systemCode 场景清缓存不精确。
- #6 默认构造下沉表名：未做。

#### 6.6.3 验证（历史）
- `dotnet build DotNet.Business -f net8.0 -c Release`：**0 错误**。
- `dotnet build DotNet.Business -f net48 -c Release`：**0 错误**（修复 `GetDataTable` 的 CS1573 XML-doc 缺 `<param>` 后无相关告警）。
- 未自动提交/推送（遵循本仓库约定）。

---

## 7. 当前状态（2026-09-19 精简后）

### 7.1 精简提交清单
- **`0cb4906`**「删除BasePermissionScopeEntity相关的代码，不要那么复杂，实际上页面怎么用过，干脆精简再精简」（作者 Troy，2026-09-19）
  - 删除 `src/DotNet.Business/BasePermissionScope/BasePermissionScopeManager.cs`（−1484 行）
  - 删除 `src/DotNet.Model/BasePermissionScopeEntity.Auto.cs`（−240 行）
  - 删除 `BasePermissionScopeManager.Auto.cs` / `.Manual.cs`（−145 / −119）
  - 改 `BasePermissionManager.Manual.cs`、`BasePage.Organization.cs`、`BasePage.User.cs`
- **`8818ee3`**「删除GetPermissionScopeTableName」
  - 删除 `BaseManager.TableName.cs` 中的 `GetPermissionScopeTableName`（及更早已不存在的 `GetOrganizationScopeTableName`）

### 7.2 现存 SystemCode 双模式覆盖范围（源码实态）
- **仍存活的 `GetXTableName` 表名函数（共 5 个）**：`GetModuleTableName` / `GetPermissionTableName` / `GetRoleTableName` / `GetRoleOrganizationTableName` / `GetUserRoleTableName`。
- **`SystemCode` 仍被以下子系统使用**（读/写路径）：
  - `BaseModule`（读 `GetDataTableByPage` 等；拼接写法 #7 隐患）
  - `BasePermission`（读/写；参数化 ✅）
  - `BaseRole`（读/写；`.Replace` 模板 + 拼接写法 #7）
  - `BaseUserRole`（读 `GetDataTable`/`GetDataTableByPage`，🟠#5 泄漏；`GetList` 已带 `FieldSystemCode` ✅）
  - `BaseUser`（角色/权限查询，部分参数化、部分拼接）
  - `BaseLogonLog`（读/写；拼接写法 #7）
  - `BaseParameter` / `BaseOperationLog`（读/写；拼接写法 #7）
  - `BaseUserLogon`（写；参数化 ✅）
  - `BaseOrganization`（仅注释提及，不参与区分）
  - `BaseChangeLog`（`SaveEntityChangeLog` 写 `SystemCode`）

### 7.3 孤儿 / 死方法
- `GetPermissionScopeTableName`：**已彻底删除**（`8818ee3`），非孤儿。
- `GetOrganizationScopeTableName`：源码无定义与调用（疑似更早清理）；初版 §0 所列 7 个方法现仅存 5 个，文档已对齐。

### 7.4 #1~#7 当前剩余状态
| 编号 | 严重度 | 原描述 | 当前状态 |
|------|--------|--------|----------|
| #1 `GetUserIdsSql` | 🔴 | 硬编码 `FROM BasePermissionScope` | **已随子系统删除而移除**（`0cb4906`） |
| #2 `CheckResourcePermissionScope` | 🔴 | 同上 | 已移除 |
| #3 `CheckRolePermissionScope` | 🔴 | 同上 | 已移除 |
| #4 `GetRoleIdsSql` | 🟠 | 权限范围缺 SystemCode 过滤 | 已移除（载体删除） |
| #5 `BaseUserRoleManager` | 🟠 | `GetDataTable`/`GetDataTableByPage` 无 SystemCode | **回退为原始形态**（:48/:105），仍为 🟠 |
| #6 默认构造未传表名 | 🟡 | Mode A 静默失效 | **未处理**，仍存在 |
| #7 字符串拼接 | 🟡 | `systemCode`/`userId` 进 SQL | **未处理**（`BaseUserRoleManager.GetDataTable` 的参数化改动已回退；现存拼接点见 §3） |

### 7.5 验证（当前 HEAD）
- `dotnet build DotNet.Business -f net8.0 -c Release`：**0 错误**。
- `dotnet build DotNet.Business -f net48 -c Release`：**0 错误**。
- grep 复核：源码 `*.cs` 中已无 `BasePermissionScope` 引用；`GetPermissionScopeTableName` 仅存于 `bin/obj` 旧 XML 产物（构建后将被刷新）。
- 未自动提交/推送（遵循本仓库约定）。

---

## 8. Mode B（整合 Base Table + SystemCode 列区分）切换评估

> 用户最终目标：`UseBaseTable = true`，所有子系统共用 `BaseXxx` 统一表，**仅靠 `SystemCode` 列**区分子系统（当前为 Mode A 分表：`SystemCode="Business"` → 物理 `BusinessXxx` 表，工作正常）。本节约评估“切换到 Mode B 后，当前代码是否有潜在 Bug”。

### 8.1 Mode B 的机制变化
- `UseBaseTable = true` 时，`GetXTableName(systemCode)` 对**所有** `systemCode` 都返回 `BaseXxx` → 表名不再隔离子系统；隔离**完全依赖 `SystemCode` 列**。
- Mode B 两大铁律：① 每条**读** SQL 必须 `WHERE SystemCode = 'x'`；② 每次**写**必须 `entity.SystemCode = 'x'`，否则行落到实体默认值 `"Base"`（`BaseModuleEntity`/`BaseRoleEntity`/`BaseUserRoleEntity`/`BasePermissionEntity`/`BaseParameterEntity` 等的 `SystemCode` 默认均为 `"Base"`）。
- **正面效应**：原 #6“默认构造落到统一表”在 Mode A 是 Bug，在 Mode B 反成**正确**（统一表正是所需）；`GetXTableName` 退化为 no-op，表路由问题自然消解。即 Mode B 比 Mode A 在表路由上更健壮——前提是 `SystemCode` 的“读过滤 + 写赋值”纪律成立。

### 8.2 潜在 Bug（Mode B 视角）
| 编号 | 严重度 | 位置 | 问题 |
|------|--------|------|------|
| B1 | 🔴 | `BaseUserRoleManager.GetDataTable`（:105）/ `GetDataTableByPage`（:48） | 读**不**过滤 `SystemCode`，且缓存键无 systemCode。共享 `BaseUserRole` 表下 → 跨子系统泄漏全部用户-角色行 + 缓存互相污染。 |
| B2 | 🟠 | 所有管理器写路径（`Add`/`AddEntity`，如 `BaseUserRoleManager.Add`:37、`BaseModuleManager.UniqueAdd`:37、`BaseRoleManager`/`BasePermissionManager` 的 Add） | 写方法**不自行 stamp `entity.SystemCode`**，仅依赖调用方已赋值；基类 `BaseManager` 也无集中 stamp 机制（grep `BaseManager*.cs` 仅 `SaveEntityChangeLog` 用全局 `BaseSystemInfo.SystemCode`）。实体 `SystemCode` 默认 `"Base"`。若任一调用方/种子数据漏设 → 行落 `BaseXxx` 且 `SystemCode="Base"`：被按子系统过滤的读判为“不存在”（数据丢失），或被当作全局共享数据向所有子系统可见（泄漏）。 |
| B3 | ✅ 已修 | `BaseUserRoleManager.Cache.cs:47-49` `RemoveCache` | 清缓存键改用实例级 `UserInfo.SystemCode`（缺失回退全局），多子系统 Mode B 下精确失效各子系统 UserRole 缓存。 |
| B4 | 🟡 | 多处读路径字符串拼接 `SystemCode='x'`（#7，如 `BaseModuleManager.Manual.cs:253` 等） | 功能正确但注入式写法；Mode B 下正确性仍取决于传入值，建议参数化。 |
| B5 | ⚪ 部署前置 | 数据迁移 | 切 `UseBaseTable=true` 后代码改查 `BaseXxx`；须先把各 `BusinessXxx` 数据迁入 `BaseXxx` 且保留 `SystemCode` 列。否则 `BaseXxx` 为空 → 全量丢失（非代码 Bug，但为切换硬前置）。 |

### 8.3 现状对照（哪些已具备 Mode B 安全性）
- ✅ **读已过滤 `SystemCode`**：`BaseModuleManager.GetDataTableByPage`（:253）+ 递归子查询/INNER JOIN、`BaseModuleManager.GetModuleTree/GetEntitiesByCache/GetEntityByCache`、`BaseRoleManager` 系列（:282 等）、`BasePermissionManager`（:327/:658 参数化）、`BaseLogonLogManager`（:64/:171）、`BaseParameterManager`/`BaseOperationLogManager`、`BaseUserRoleManager.GetList`（:133 带 `systemCode` 参数）、`BaseUserLogonManager` 写（参数化）。
- ✅ **写依赖调用方设 `SystemCode`**：用户 Mode A 下“分表工作正常”且上述读均按 `SystemCode` 过滤 → 推断调用方**已在写时设 `entity.SystemCode`**（否则 Mode A 读也会空）。故 Mode B 写侧大概率 OK，但**缺集中兜底（B2）**。
- ❌ **读未过滤**：仅 `BaseUserRoleManager.GetDataTable`/`GetDataTableByPage`（B1）。

### 8.4 切换到 Mode B 的最小改造清单（待确认）
1. **修 B1**：`BaseUserRoleManager.GetDataTable`/`GetDataTableByPage` 补 `SystemCode` 过滤 + 缓存键并入 systemCode（因 `GetDataTableByPage` 是 `override` 虚方法，需实例级 `_systemCode` 字段或改基类虚方法签名）。
2. **修 B2（推荐集中兜底）**：在 `BaseManager.AddEntity`/`UpdateEntity` 增加“若 `entity.SystemCode` 为空则置为实例 `SystemCode`（取自构造入参 / `UserInfo.SystemCode`）”的逻辑，彻底消除漏设风险——这是让 Mode B 真正稳健的关键。
3. **修 B3**：`RemoveCache` 改用实例级 systemCode。
4. **前置 B5**：数据迁移脚本 `BusinessXxx → BaseXxx`（保留 `SystemCode` 列值）。
5. **验证**：net8.0/net48 构建 + 连库用例覆盖 Mode B 双子系统互不可见。

### 8.5 修订记录（2026-09-20 实施 B3）

- 文件：`src/DotNet.Business/BaseUserRole/BaseUserRoleManager.Cache.cs` 的 `RemoveCache`（:44-50）。
- 改动：原三处 `BaseSystemInfo.SystemCode` 改为实例级 `effectiveSystemCode = UserInfo.SystemCode`（若 `UserInfo.SystemCode` 为空则回退 `BaseSystemInfo.SystemCode`），并用于 `cacheKeyListSystemCode` / `cacheKeySystemCode` / `cacheKeySystemCodeUserId` 三个缓存键。
- 效果：多子系统 Mode B 下，用户-角色写操作触发的缓存清除精确指向“当前用户所属子系统”的 `List.<sc>.UserRole` / `Dt.<sc>.UserRole` / `Dt.<sc>.<UserId>.UserRole`，而非仅清全局子系统缓存；单子系统部署（`UserInfo.SystemCode` 与全局一致或空）行为不变，**无回归**。
- 范围控制：本轮**仅修 B3**，未动 B1（`GetDataTable`/`GetDataTableByPage` 的 `SystemCode` 过滤与缓存键）、B2（写集中兜底）、B4（拼接参数化）、B5（数据迁移）。
- 已知残留：`GetDataTable`（B1 未修）缓存键为 `"Dt." + CurrentTableName + "." + companyId + "." + (myCompanyOnly?1:0)`，不含 systemCode，故 `RemoveCache` 的 `cacheKeySystemCode`（`Dt.<sc>.UserRole`）仍不命中它——此为 B1 范畴，待后续一并处理。
- 验证：`dotnet build DotNet.Business -f net8.0` 与 `-f net48` 均 **0 错误**（29 警告，NU1603 版本解析，与本次无关）。
- 未自动提交/推送（遵循约定，待用户确认）。

> 注：§8 为 Mode B 切换评估；其中 B3 已实现，其余 B1/B2/B4/B5 待确认范围后继续。
