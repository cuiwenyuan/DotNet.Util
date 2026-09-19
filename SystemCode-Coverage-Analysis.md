# SystemCode 双模式区分 — 覆盖率与潜在 Bug 分析

> 分析对象：`DotNet.Business`（含 `DotNet.Model` / `DotNet.Util` / `DotNet.Business.Web` 相关）
> 分析日期：2026-09-18
> 目标：找出所有使用 SystemCode 区分子系统的地方，并判断最终读/写表的 SQL 是否同时支持两种模式。

---

## 0. 两种模式的实现机制（先对齐口径）

用户描述的两种区分方式，在本库里由 `BaseSystemInfo.UseBaseTable` 开关 + `GetXTableName(systemCode)` 表名函数实现：

| 模式 | 开关 | 物理表名来源 | 子系统区分手段 |
|------|------|--------------|----------------|
| **Mode A（分表模式）** | `UseBaseTable = false`（**默认值**） | `systemCode + "X"`，如 `BusinessModule`、`BusinessUserRole`、`BusinessPermission` | 靠**表名本身**区分子系统 |
| **Mode B（列模式）** | `UseBaseTable = true` | 固定 `BaseX`，如 `BaseModule`、`BaseUserRole` | 靠 **`SystemCode` 列**过滤区分子系统 |

- 表名函数（`Util/BaseManager.TableName.cs`）：`GetModuleTableName` / `GetPermissionTableName` / `GetRoleTableName` / `GetUserRoleTableName` / `GetPermissionScopeTableName` / `GetRoleOrganizationTableName` / `GetOrganizationScopeTableName`。`systemCode` 为空时回退为 `"Base"`。
- 模式开关：`BaseSystemInfo.UseBaseTable`（`BaseSystemInfo.Permission.cs:24`，默认 `false`），由配置 `appSettings["UseBaseTable"]` 决定（`ConfigurationUtil.cs:228`、`UserConfigUtil.cs:575`）。
- **Schema 事实（关键）**：`scripts/UserCenter.SQLServer.2008R2.sql` 中 `Base*` 与 `Business*` 两族表**都建有 `SystemCode` 列**（`nvarchar(50)`，NOT NULL 或 NULL）。因此 `AND SystemCode = '...'` 这个过滤条件在两种模式下语法都合法——前提是 Mode A 的分表确实被创建且含该列。

---

## 1. 覆盖率矩阵（按功能点）

✅ = 两种模式都正确；🟠 = Mode B 跨系统泄漏 / Mode A 依赖构造；🔴 = 两种模式都错（安全相关）；➖ = 不参与子系统区分

| # | 功能点 / 方法 | 文件:行 | 读/写 | Mode A 表名 | Mode B 表名 | 带 SystemCode 过滤 | 结论 |
|---|--------------|---------|-------|-------------|-------------|-------------------|------|
| 1 | `BaseModuleManager.GetDataTableByPage` | `BaseModule/BaseModuleManager.Manual.cs:210-482` | 读 | `GetModuleTableName` | `BaseModule` | ✅ 无条件 `AND SystemCode='x'`（:253）+ 递归子查询(:356/359/360) + `INNER JOIN …SystemCode=…SystemCode`（:415/:470） | ✅ 两者都支持 |
| 2 | `GetModuleTree` / `GetEntitiesByCache` / `GetEntityByCache` | `BaseModuleManager.Manual.cs:675/883/792` | 读 | `GetModuleTableName` | `BaseModule` | ✅ 参数含 `FieldSystemCode`（:689/:902/:…） | ✅ |
| 3 | `UniqueAdd` / `UniqueUpdate` | `BaseModuleManager.Manual.cs:37/86` | 写 | `CurrentTableName`（构造传入） | `BaseModule` | ✅ 去重参数含 `FieldSystemCode`（:43/:92） | ✅（依赖正确构造） |
| 4 | `BaseUserRoleManager.GetDataTableByPage` | `BaseUserRole/BaseUserRoleManager.cs:48` | 读 | `CurrentTableName` | `BaseUserRole` | ❌ 仅 Enabled/Deleted/时间/搜索，无 SystemCode | 🟠 Mode B 跨系统泄漏 |
| 5 | `BaseUserRoleManager.GetDataTable(myCompanyOnly)` | `BaseUserRole/BaseUserRoleManager.cs:105-117` | 读 | `CurrentTableName` | `BaseUserRole` | ❌ 仅 Enabled/Deleted；缓存键 `"Dt."+CurrentTableName+…`（:114）无 systemCode | 🟠 Mode B 泄漏 + 跨系统缓存污染 |
| 6 | `BaseUserRoleManager.Manual` 的 `GetList` | `BaseUserRole/BaseUserRoleManager.Manual.cs:133-140` | 读 | `CurrentTableName` | `BaseUserRole` | ✅ `whereParameters` 含 `FieldSystemCode, systemCode` | ✅（依赖正确构造） |
| 7 | `BaseRoleManager` 系列（含 `GetDataTableByPage`、角色权限子查询） | `BaseRole/BaseRoleManager.Manual.cs` 多处 | 读/写 | 模板 `FROM BaseUserRole` + `.Replace("BaseUserRole", userRoleTableName)`（:812/:829）；`AND SystemCode='x'`（:282） | `BaseRole` | ✅ 表名经 `.Replace` 切换 + 列过滤 | ✅ |
| 8 | `BasePermissionManager`（cs/User/Role/Manual） | `BasePermission/*` | 读/写 | `GetPermissionTableName` | `BasePermission` | ✅ 参数化 `FieldSystemCode = GetParameter(FieldSystemCode)`（:327/:658） | ✅ |
| 9 | `BasePermissionScopeManager.GetDataTableByPage` | `BasePermissionScope/BasePermissionScopeManager.cs:915` | 读 | `CurrentTableName = GetPermissionScopeTableName(systemCode)` | `BasePermissionScope` | ✅ 表名 mode-aware | ✅ |
| 10 | `GetRoleIdsSql` | `BasePermissionScope/BasePermissionScopeManager.cs:467-504` | 读 | ✅ `GetPermissionScopeTableName/GetRoleTableName/GetUserRoleTableName` | `BasePermissionScope` | ❌ WHERE 无 `AND SystemCode='x'` | 🟠 Mode B 跨系统泄漏 |
| 11 | `GetUserIdsSql` | `BasePermissionScope/BasePermissionScopeManager.cs:614-672` | 读 | ❌ **硬编码 `FROM BasePermissionScope`**（:619），无 `GetPermissionScopeTableName`；子查询 `FROM BaseUserRoleEntity.CurrentTableName`（:664） | `BasePermissionScope` | ❌ 无 SystemCode | 🔴 两者都错 |
| 12 | `CheckResourcePermissionScope` | `BasePermissionScope/BasePermissionScopeManager.cs:1083-1102` | 读 | ❌ **硬编码 `FROM BasePermissionScope`**（:1086） | `BasePermissionScope` | ❌ 无 SystemCode；且方法签名**无 systemCode 参数** | 🔴 两者都错（权限判定核心） |
| 13 | `CheckRolePermissionScope` | `BasePermissionScope/BasePermissionScopeManager.cs:1118-1143` | 读 | ❌ **硬编码 `FROM BasePermissionScope` + `FROM BaseUserRole`**（:1122/:1128） | `BasePermissionScope`/`BaseUserRole` | ❌ 无 SystemCode；方法签名**无 systemCode 参数** | 🔴 两者都错（权限判定核心） |
| 14 | `BaseLogonLogManager` 系列 | `BaseLogonLog/*` | 读/写 | `CurrentTableName` + `FieldSystemCode` 过滤（:64/:171） | `BaseLogonLog` | ✅ | ✅（依赖正确构造） |
| 15 | `BaseOperationLogManager` / `BaseParameterManager` | `BaseOperationLog/*`、`BaseParameter/*` | 读/写 | `CurrentTableName` + `SystemCode` 赋值/过滤（:97/:138） | `BaseOperationLog`/`BaseParameter` | ✅ | ✅（依赖正确构造） |
| 16 | `BaseUserLogonManager` 写 | `BaseUserLogon/BaseUserLogonManager.Manual.cs:840-919` | 写 | `CurrentTableName` | `BaseUserLogon` | ✅ 参数化 `FieldSystemCode` | ✅ |
| 17 | `BaseOrganizationManager` | `BaseOrganization/BaseOrganizationManager.cs` | — | — | — | ➖ 仅注释提 `SystemCode`（:809），实际不参与子系统区分 | ➖ |
| 18 | `BaseSystemManager.GetSystemCodes` | `BaseSystem/BaseSystemManager.cs:35` | 读 | 字典 `BaseSystemCode`（子系统清单） | 同 | ➖ 与 Mode A/B 解耦 | ➖ |

---

## 2. 系统性根因

1. **Mode A 只在“构造时传入 `GetXTableName(systemCode)`”的路径生效。**
   管理器默认构造 `new BaseXManager(UserInfo)`（或 `new BaseXManager()`）时，`CurrentTableName` 落到实体常量 `BaseXxxEntity.CurrentTableName = "BaseXxx"`（统一表）。因此只要调用方没显式传 `GetXTableName(systemCode)`，Mode A 的分表就被静默绕过，去查统一表——在 Mode A 下会漏掉各子系统的分表数据。
   - 已正确传表名的：`BaseModuleManager.Manual.cs:682/893`、`BaseUserManager.Manual.Role.cs:273/670/701/1024/1059/1173`、`BasePermissionManager.User.cs:170/352`、`BasePermissionScopeManager.cs:577`。
   - **未传表名（Mode A 静默失效）**：大量 `new BaseUserManager(UserInfo)`、`new BaseRoleManager(UserInfo)`、`new BasePermissionManager(userInfo)`、`new BaseOrganizationManager(UserInfo)`（见 `WebUtil.*`、`BasePage.*`、`BaseUserManager.Manual.*` 多处）。

2. **Mode B 只在“每条 SQL 都 `AND SystemCode='x'`”时安全。**
   Mode B 是统一表，子系统仅靠 `SystemCode` 列区分。任何读路径遗漏该过滤，就会把其它子系统的行一并捞出（跨系统泄漏）。权限判定类方法（#11/#12/#13）正是这种遗漏，且它们的方法签名里根本没有 `systemCode` 入参，无法补过滤。

3. **两类不一致写法**：
   - “模板 + `.Replace("BaseXxx", tableName)`”写法（BaseRoleManager、BaseUserManager.Manual.Role.cs）→ Mode A 正确。
   - “直接硬编码 `FROM BaseXxx`”写法（BasePermissionScopeManager 三处）→ Mode A 查错表、Mode B 无过滤。
   - 参数化（`DbHelper.GetParameter(FieldSystemCode)`，BasePermissionManager/BaseUserLogonManager）vs 字符串拼接（其余多数）→ 后者有注入隐患且更易漏过滤。

---

## 3. 潜在 Bug 清单（严重度）

### 🔴 严重（两种模式都错，且涉及权限判定）
- **#1 `GetUserIdsSql`** — `BasePermissionScope/BasePermissionScopeManager.cs:619`
  硬编码 `FROM BasePermissionScope`，无 `GetPermissionScopeTableName`、无 SystemCode。
  - Mode A：查统一表而非 `systemCode+"PermissionScope"` → 返回该子系统错误的“按权限取下属用户”结果。
  - Mode B：查统一表但无 SystemCode → 跨系统返回所有子系统的授权用户。
- **#2 `CheckResourcePermissionScope`** — `BasePermissionScope/BasePermissionScopeManager.cs:1086`
  硬编码 `FROM BasePermissionScope`，无 SystemCode，方法签名无 `systemCode` 参数。这是“用户是否对某资源有权限”的核心判定 → 安全相关跨系统误判。
- **#3 `CheckRolePermissionScope`** — `BasePermissionScope/BasePermissionScopeManager.cs:1122/1128`
  硬编码 `FROM BasePermissionScope` + `FROM BaseUserRole`，无 SystemCode，签名无 `systemCode` → “用户角色是否有权限”核心判定跨系统误判。

### 🟠 中（Mode B 跨系统泄漏）
- **#4 `GetRoleIdsSql`** — `BasePermissionScope/BasePermissionScopeManager.cs:467`
  表名已 mode-aware（`GetPermissionScopeTableName` 等），但 WHERE 缺 `AND SystemCode='x'` → Mode B 跨系统泄漏角色权限范围。
- **#5 `BaseUserRoleManager.GetDataTableByPage` / `GetDataTable`** — `BaseUserRole/BaseUserRoleManager.cs:48/105`
  无 SystemCode 过滤，完全依赖 `CurrentTableName`。Mode B（`BaseUserRole` 统一表）返回所有子系统用户-角色行；缓存键 `"Dt."+CurrentTableName+companyId`（:114）不含 systemCode → 跨系统缓存污染。

### 🟡 低（健壮性 / 一致性 / 注入）
- **#6 管理器默认构造未传 `GetXTableName`** — 见 §2.1。Mode A 静默失效，是覆盖面最大的隐性风险。
- **#7 字符串拼接 `systemCode` / `userId` 进 SQL** — `BaseModuleManager.Manual.cs:253`、`BaseRoleManager.Manual.cs:282`、`BasePermissionScopeManager.Manual.cs:43`、`BaseUserManager.Manual.Role.cs:752`、`BaseParameterManager.Manual.cs:97`、`BaseLogonLogManager.Manual.cs:64/171`。与模式无关，但建议统一改为参数化（对齐 `BasePermissionManager`/`BaseUserLogonManager`）。

### ✅ 已确认正确（两种模式）
- `BaseModuleManager.GetDataTableByPage`（表名 mode-aware + 无条件 SystemCode 过滤 + 递归子查询 + INNER JOIN on SystemCode）。
- `BaseModuleManager.GetModuleTree` / `GetEntitiesByCache` / `GetEntityByCache`。
- `BasePermissionManager` 各方法（参数化 `FieldSystemCode`）。
- `BaseRoleManager` 系列（`.Replace` 模板 + `AND SystemCode`）。
- `BaseUserLogonManager` 写路径（参数化 `FieldSystemCode`）。
- `SaveEntityChangeLog`（`BaseManager.SaveEntityChangeLog.cs:48-71`，写 `BaseChangeLog.SystemCode`）。

---

## 4. 修复建议（待用户确认后再动手）

1. **🔴#1/#2/#3（权限判定）**：
   - 表名改为 `GetPermissionScopeTableName(systemCode)`（#1 还需 `GetUserRoleTableName(systemCode)` 替换子查询里的 `BaseUserRoleEntity.CurrentTableName`）。
   - WHERE 增加 `AND SystemCode = DbHelper.GetParameter(FieldSystemCode)`（参数化）或 `AND SystemCode = '” + DbHelper.SqlSafe(systemCode) + “'”`。
   - **必须给 `CheckResourcePermissionScope` / `CheckRolePermissionScope` 增加 `systemCode` 参数**，并逐层上溯调用方（`CheckUserModulePermission`/`CheckRoleModulePermission` 目前完全不传 systemCode）。
2. **🟠#4**：`GetRoleIdsSql` 已有表名参数，补 `AND permissionScopeTableName.SystemCode = systemCode`（参数化）。
3. **🟠#5**：`BaseUserRoleManager` 两方法补 SystemCode 过滤；或将 `CurrentTableName` 强制为 `GetUserRoleTableName(systemCode)`，且缓存键加入 systemCode。
4. **🟡#6**：约定凡子系统相关管理器一律 `new BaseXManager(DbHelper, UserInfo, GetXTableName(systemCode))`；更彻底的做法是把 `UseBaseTable` 逻辑下沉到 `BaseManager` 默认 `CurrentTableName`，避免依赖调用方自觉传表名。
5. **🟡#7**：拼接改参数化 `DbHelper.GetParameter`。

> 注：本仓库通用约定——改动只在本机落地、不自动提交；上述修复按“先确认范围、再增量修改”推进。

---

## 5. 修订记录（2026-09-18 实施 #1/#2/#3 后）

### 5.1 关键更正：权限范围行的 `SystemCode` 列实际未被填充
实施前核查授权写入路径发现：
- `GrantResourcePermissionScopeTarget`（两个重载，:1156 / :1203）**从不设置 `SystemCode`**；
- `BasePermissionScopeEntity.SystemCode` 默认值为 `"Base"`（`BasePermissionScopeEntity.Auto.cs:37`）；
- 因此 **所有权限范围行物理存储的 `SystemCode` 均为 `"Base"`**，并未按子系统区分。

### 5.2 对修复方案的影响
- 原 §4 建议的 `AND SystemCode = 'systemCode'` 过滤**不能加**：
  - Mode A（`systemCode ≠ "Base"`）会过滤掉全部行 → 权限判定恒为“拒绝”，属于**安全回归**；
  - Mode B 所有行 `SystemCode="Base"`，该过滤仅在 `systemCode="Base"` 时命中，对其它子系统同样恒拒。
- 故 #1/#2/#3 实际落地**仅做表名 mode-aware**：
  - `GetUserIdsSql`（:614）：`BasePermissionScope` → `GetPermissionScopeTableName(systemCode)`；子查询 `BaseUserRoleEntity.CurrentTableName` → `GetUserRoleTableName(systemCode)`。
  - `CheckResourcePermissionScope`（:1085）：增 `systemCode` 形参；`BasePermissionScope` → `GetPermissionScopeTableName(systemCode)`。
  - `CheckRolePermissionScope`（:1121）：增 `systemCode` 形参；`BasePermissionScope` → `GetPermissionScopeTableName(systemCode)`、`BaseUserRole` → `GetUserRoleTableName(systemCode)`。
  - 并给 `CheckUserModulePermission` / `CheckRoleModulePermission` 增 `systemCode` 形参，逐层上溯到 `IsModuleAuthorized(systemCode, ...)`（:1051）的调用点（:1056 / :1061）。
  - **未加 `SystemCode` 列过滤**（理由见 5.1）。
  - Mode A：现查询正确的分表（`systemCode+"PermissionScope"` / `systemCode+"UserRole"`），修复原 🔴。
  - Mode B：表名回退为 `BasePermissionScope`（与原硬编码等价），行为不变。
- **原 §3 描述的“Mode B 跨系统泄漏”在当前数据下不会发生**：因所有权限范围行 `SystemCode` 均为 `"Base"`，统一表内不存在可区分的子系统数据。

### 5.3 待决（超出 #1/#2/#3 范围，需用户决策）
若要让 Mode B 真正按子系统用 `SystemCode` 列区分，必须先让**授权写入路径**填充 `SystemCode`（给 `GrantResourcePermissionScopeTarget` 增加 `systemCode` 入参并在实体上赋值），再回头加读路径过滤。这是一个更大的数据模型改动，未在本轮实施。#4/#5 的 `SystemCode` 过滤同理依赖该前提（#4 表名已 mode-aware；#5 `BaseUserRoleManager` 未动）。

### 5.4 验证
- `dotnet build DotNet.Business -f net8.0 -c Release`：**0 错误**（1752 警告均为既有 CA 文化/全球化类告警，与本次改动无关）。
- `dotnet build DotNet.Business -f net48 -c Release`：**0 错误**。
- 未自动提交/推送（遵循本仓库约定）。

---

## 6. 修订记录（2026-09-19 实施“授权写入路径填充 SystemCode”）

> 对应 §5.3 待决项：让 Mode B 真正按 `SystemCode` 列区分所需的“更大的数据模型改动”。

### 6.1 写入路径改造（核心）
目标：授权落库时把真实 `systemCode` 写入 `SystemCode` 列，使 Mode B 的统一表能按列隔离子系统；同时让去重（幂等）按子系统维度进行，避免新子系统授权被旧 `Base` 行误判为已存在而跳过。

改动文件：`src/DotNet.Business/BasePermissionScope/BasePermissionScopeManager.cs`（行号以 2026-09-19 写入路径落地时为准；若文件后续有增删行，请以方法名定位）

| 方法 | 改动 |
|------|------|
| `GrantResourcePermissionScopeTarget`（两个重载，现 :1180 / :1227） | ① 签名**新增首个参数 `string systemCode`**（破坏性 API 变更）；② 实体构造新增 `SystemCode = systemCode`；③ `Exists` 去重参数列表新增 `FieldSystemCode = systemCode`；④ 方法体开头 `CurrentTableName = GetPermissionScopeTableName(systemCode)` 路由到正确的（mode-aware）表。 |
| `AddPermission(string systemCode, ...)`（4 参重载，现 :213） | ① 签名新增首个参数 `string systemCode`；② 实体 `SystemCode = systemCode`；③ `CurrentTableName = GetPermissionScopeTableName(systemCode)`。 |
| `AddPermission(BasePermissionScopeEntity entity)`（现 :233） | ① `CurrentTableName = GetPermissionScopeTableName(entity.SystemCode)` 路由表；② 调用 `PermissionScopeExists` 时传入 `entity.SystemCode`。 |
| `PermissionScopeExists`（现 :161） | ① 签名**新增首个参数 `string systemCode`**；② 去重参数新增 `FieldSystemCode = systemCode`；③ `CurrentTableName = GetPermissionScopeTableName(systemCode)`。 |

- 仓库内 `src` **无这两个授权方法的调用方**（grep 仅命中定义本身 + `bin/obj` 的 xml 产物），故本仓编译不受影响；外部权限服务（独立仓库）调用需补传 `systemCode`（或 `userInfo.SystemCode`）。
- 写路径**不擅自“修正”** `GrantResourcePermissionScopeTarget` 重载二里 `TargetCategory`/`TargetId` 既有互换写法（属既有行为，超出本次范围）。

### 6.2 读路径补 `SystemCode` 过滤（使写入的列真正生效）
写路径落库后，读路径必须按 `SystemCode` 过滤，Mode B 才算“真正区分”。为避免破坏既有数据，采用**兼容过滤** `AND (SystemCode = 'systemCode' OR SystemCode = 'Base')`：
- Mode A：分表已按表名隔离子系统，该过滤 `(=子系统 OR ='Base')` 命中分表内全部行（新行 + 历史 `Base` 行）→ 行为不变。
- Mode B：统一表内，新写入的 `=子系统` 行被精确隔离；历史 `Base` 行作为“全局/共享”权限对全部子系统可见（保留升级前共享语义，不丢权限）。

落地位置（均用 `permissionScopeTableName` / `CurrentTableName` 别名 + `BasePermissionScopeEntity.FieldSystemCode`）：

| 方法 | 行 | 说明 |
|------|----|------|
| `GetUserIdsSql`（用户权限范围） | :621 起 WHERE | 首个分支补 `OR SystemCode='Base'` |
| `GetRoleIdsSql`（角色权限范围，原 🟠#4） | :475 起 WHERE | 补过滤 |
| `CheckResourcePermissionScope`（原 🔴#2） | :1085 起 WHERE | 补过滤 |
| `CheckRolePermissionScope`（原 🔴#3） | :1121 起 WHERE | 补过滤 |
| `GetOrganizationIdsSql`（组织权限范围） | :274 起 WHERE | 补过滤（用 `BasePermissionScopeEntity.CurrentTableName` 别名） |
| `GetResourceScopeIds`（数据权限范围，中央入口） | :911 起 3 个分支 | 每个 SELECT 分支均补过滤（覆盖 `GetUserIds`/`GetOrganizationIds`/`GetOrganizationDt` 等下游） |

### 6.3 历史 `Base` 行兼容策略
- 升级后新授权均带正确 `SystemCode`；存量 `SystemCode='Base'` 行在 Mode B 下作为共享权限保留（见 6.2 兼容过滤）。
- 如需“严格按子系统隔离、历史行不跨子系统可见”，可执行一次性数据迁移将 `BasePermissionScope` 中 `SystemCode='Base'` 的行回填为真实子系统（需业务侧提供映射，当前无此信息），迁移后再把兼容过滤的 `OR SystemCode='Base'` 去掉即可。

### 6.4 验证
- `dotnet build DotNet.Business -f net8.0 -c Release`：**0 错误**（540 警告，均为既有 CA 文化/全球化类，与本次无关）。
- `dotnet build DotNet.Business -f net48 -c Release`：**0 错误**。
- grep 复核：`BasePermissionScopeManager.cs` 已无 `FROM BasePermissionScope` / `FROM BaseUserRole` / `BasePermissionScope.` / `BaseUserRole.` 硬编码；写/读入口均经 `GetPermissionScopeTableName(systemCode)` 路由。
- 未自动提交/推送（遵循本仓库约定）。

### 6.5 仍待办（滚动更新）

> 2026-09-19 第二轮已按“范围 B + #7”落地：`BaseUserRoleManager.GetDataTable` 已完成 `SystemCode` 过滤 + 缓存键 + 参数化（详见 §6.6）。下方仅列**仍未闭环**项。

- 🟠#5（剩余）`BaseUserRoleManager.GetDataTableByPage`（`override` 虚方法，签名锁死，无法加 `systemCode` 参数）→ Mode B 分页路径**仍可能跨系统泄漏**；`RemoveCache`（`BaseUserRoleManager.Cache.cs`）仍依赖全局 `BaseSystemInfo.SystemCode`，多子系统显式 `systemCode` 场景清缓存不精确。需以**实例级 `_systemCode` 字段**（新增加法构造 `BaseUserRoleManager(userInfo, systemCode)`）或改基类虚方法签名解决——此即与 #6 的根因交汇点。
- 🟡#6 管理器默认构造未传 `GetXTableName`（Mode A 静默失效，系统性根因）。与 #5 剩余项同源，可一并收口。
- 🟡#7（部分）本轮已在 `BaseUserRoleManager.GetDataTable` 将 `systemCode` 值改为 `DbHelper.GetParameter` 参数化；但仓库内仍有其它 `SystemCode`/`userId` 字符串拼接点（如 `BaseUserManager.Manual.Role.cs:752` 的 `SystemCode = '` + systemCode + `''`）未统一参数化，属注入隐患，待后续清扫。

### 6.6 修订记录（2026-09-19 实施“#5 范围 B + #7 参数化”：`BaseUserRoleManager.GetDataTable` 改造）

用户确认范围：**B（最小，仅改 `GetDataTable` + 缓存键，不动 `override GetDataTableByPage` / 默认构造）+ 顺带做 #7（参数化），不做 #6**。

#### 6.6.1 改动点（`src/DotNet.Business/BaseUserRole/BaseUserRoleManager.cs` 的 `GetDataTable`）
- 新增可选参数 `string systemCode = null`（向后兼容，仓库内无其它调用方）。
- 解析生效子系统：`effectiveSystemCode = systemCode ?? BaseSystemInfo.SystemCode ?? "Base"`。
- **读过滤（`#5`）**：WHERE 改为 `(SystemCode = @SystemCode OR SystemCode = 'Base') AND Enabled = 1 AND Deleted = 0`。
  - Mode B（`CurrentTableName = BaseUserRole` 被全子系统共用）下真正按 `SystemCode` 列隔离，消除跨系统泄漏；
  - OR `'Base'` 兼容升级前历史行（与 6.2 读路径策略一致，避免静默丢弃存量授权）；
  - Mode A（分表）下 `CurrentTableName` 已隔离，此过滤仅进一步收敛、行为不变。
- **参数化（`#7`）**：`SystemCode` 值经 `dbHelper.GetParameter(FieldSystemCode)` 占位 + `dbHelper.MakeParameters(...)` 绑定，**不再字符串拼接**，杜绝注入。
  - ⚠️ 实现要点：KVP 版 `GetDataTable` 会经 `GetWhereString` 自动追加 `SystemCode = @SystemCode`，若同时手写 OR 条件会产生 `(A OR B) AND A` 误抵消 `OR 'Base'` 分支 → 故本处直接 `DbHelper.Fill("SELECT * FROM " + CurrentTableName + " WHERE " + sb, dbParameters)`，手动拼接 OR 条件仅占位、值由 `MakeParameters` 绑定。
- **缓存键（`#5`）**：`cacheKey = "Dt." + CurrentTableName + "." + effectiveSystemCode + "." + companyId + "." + (myCompanyOnly?1:0)`，并入 `effectiveSystemCode`，消除 Mode B 多子系统共享 `BaseUserRole` 表时的缓存互相污染；单子系统部署（无显式 systemCode）退化为 `BaseSystemInfo.SystemCode`，与 `RemoveCache` 既有键一致。

#### 6.6.2 本轮未改（范围 B 约定）
- `GetDataTableByPage`（`override` 虚方法，签名锁死，不可加参）：Mode A 靠分表名隔离仍安全；**Mode B 分页路径仍可能跨系统泄漏（#5 未闭环）**，需后续以实例级 `_systemCode` 字段或改基类虚方法签名解决。
- `RemoveCache`（`BaseUserRoleManager.Cache.cs`）：仍依赖全局 `BaseSystemInfo.SystemCode`，未随实例 systemCode 化；与 6.6.1 单子系统场景兼容，多子系统显式 systemCode 场景清缓存不精确（已知限制）。
- #6 默认构造下沉表名：未做（用户明确不做）。

#### 6.6.3 验证
- `dotnet build DotNet.Business -f net8.0 -c Release`：**0 错误**（540 警告，均为既有 CA 文化/全球化类）。
- `dotnet build DotNet.Business -f net48 -c Release`：**0 错误**（修复 `GetDataTable` 的 `CS1573` XML-doc 缺 `<param name="systemCode">` 后无相关告警）。
- 未自动提交/推送（遵循本仓库约定）。
