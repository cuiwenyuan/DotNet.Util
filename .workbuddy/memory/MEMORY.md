# DotNet.Util 项目长期笔记

## 一、沙箱构建/测试环境（每次命令都要用）

**Bash 会话 PATH 被清空**（`ls`/`grep`/`dirname` 全 command not found，但 `pwd`/`echo` 正常 → 不是工具坏）。
每条命令开头加：
`export PATH="/usr/bin:/bin:$PATH:/c/Users/Troy/.workbuddy/binaries/PortableGit/versions/1.2.0/usr/bin:/c/Program Files/dotnet"`
dotnet 在 `/c/Program Files/dotnet`（10.0.401）。stderr 的 `shell-runtime-bash-env.sh: line 3: dirname: command not found` 是无害噪音。

**env 被清空 → restore 崩**：`PROGRAMDATA`/`APPDATA`/`USERPROFILE`/`NUGET_PACKAGES` 为空会抛
`NuGet.targets(782) → Path.Combine(null): Value cannot be null (Parameter 'path1')`，restore/build/test 全崩；
`cmd.exe`/`powershell.exe`/`reg.exe` 被沙箱拦截不能借。绕过：先 `export` 这 6 个变量
（`NUGET_PACKAGES='C:/Users/Troy/.nuget/packages'`、`APPDATA`/`LOCALAPPDATA` 指向 `C:/Users/Troy/AppData/...`、
`USERPROFILE`/`HOME`=`C:/Users/Troy`、`PROGRAMDATA`=`C:/ProgramData`），再加 `--no-restore`（obj/assets.json 已存在时）。
本机 VS/PowerShell 无此故障。**PowerShell 工具在本沙箱不可用（返回空 stdout）**，别依赖它。

**IDE 持有 obj 锁 → MSB3491 Access is denied**。禁用程序集信息生成即可，两个参数**必须同时加**
（只加 `-p:TargetFrameworks=<单TFM>` 会触发 CS0579）：
`-p:GenerateAssemblyInfo=false -p:GenerateTargetFrameworkAttribute=false`
`dotnet build-server shutdown` 对此锁无效。

**CS0579**：绝不把 `OutputPath`/`IntermediateOutputPath` 指到工程树内（如 `verify_obj/`）——其内生成的
`*.AssemblyAttributes.cs` 会被 glob 进源码 → `Duplicate TargetFrameworkAttribute`，多工程批量爆发。临时目录放工程树外。

**沙箱写入限制**：`C:/` 根目录写入被拒 → 重定向失败会让"命令根本没跑"却报 exit=1，极易误判。log/输出放工作区内。

**Edit 工具绝不能并行改同一文件**：同一条消息多个 Edit 会各自基于原内容写入、后写覆盖前写，
**只有一处存活**且仍返回 success → 同文件多处编辑**串行**，改完 grep 复核。

## 二、测试基线与 flaky 判据

- 基线 **1204 例**（排除集成测试），全量 net8.0 约 25~38s；`MsgTests` 相关 40 例。
- `IntegrationTests`（SQL Server/Redis/QQWry）无外部依赖必 FAIL，属预期。SQL Server 集成测试可用（本机 1433/1434 在听，
  Windows 集成认证免密）：`export DUP_TEST_SQLSERVER='Server=127.0.0.1,1433;Database=master;Trusted_Connection=True;TrustServerCertificate=True;'`
- **flaky 判据：失败点在不同测试间轮换即并行竞争，不要逐条当新 bug。**
  `HttpUtilTests`（临时端口 HttpListener 争用，多为 `WebException: (410) Gone`，单独跑 8/8）、
  `CacheUtilTests.RemoveByRegex_RemovesMatchingKeys`（共享全局静态缓存 key 互删，单独跑 3/3）。
- **net48 不要一次跑整个 Db 命名空间（217 例）**：会被 SIGTERM、stdout 全空、退出码 1
  → **net48 验证一律拆 filter**（net8.0 无限制，217 例 704ms）。
- 断言"默认中文"或会切 `Msg.CurrentLanguage` 的测试类必须标注
  `[Collection(MsgTestCollection.Name)]`（`Tests/Message/MsgTestCollection.cs`，DisableParallelization=true）。

## 三、已知缺陷与硬坑

**🔴 `DbHelper.Fill` 丢弃返回值会静默吞异常**：`Fill(DataTable dt, ...)` 内部 catch 后把**自己的局部** `dt` 置 null 返回，
调用方传入的 `dt` 不受影响 → **必须接返回值判空**，否则表现为"空表 + 不报错"。`DbUtil.LockNoWait.cs` 已修（null → -1）。
其余 12 处旧写法：`SQLBuilder.cs:660`、`DbUtil.Common.cs:356,377`、`DbUtil.Method.cs:162`、
`DbUtil.ParentChildrens.cs:70,112,140,238,291`、`BaseExceptionManager.Manual.cs:159`、`BaseManager.PreviousNext.cs:53`
（成功路径等价、失败路径静默）。彻底修需改 `Fill` 语义，属行为变更，待评估。

**🟡 net48 测试宿主下禁用 NewLife.Core 的 `ToDecimal`/`ToDouble`**：在 net48 的 xunit/VSTest 宿主下会让宿主在
用例通过后崩溃（返回值正确）；`ToInt`、`System.Convert.ToDecimal` 正常；net8.0 正常。**仅供测试宿主**——
工程树外独立 net48 控制台 EXITCODE=0 → 生产/net4x 用户不受影响。
升 `NewLife.Core` 11.18.2026.801 → 11.19.2026.901 **仍崩**，但升级本身健康（10 TFM + 14 项目 0 错误、无 NU1605）。
与输入类型无关，直调 `DefaultConvert.ToDecimal` 照样崩 → 该层 string 分支含 `stackalloc` + `Span<Char>` + Range 切片
（`tmp[..rs]`），net4x 靠 System.Memory 垫片，首要嫌疑（待 dump）。影响面 `.ToDecimal(` 9 + `.ToDouble(` 7；
产品 9 处（`RequestUtil.cs`×2、`NewLife/DataUtil.cs`×2、`DbUtil.Aggregate.cs`×1、`ExcelUtil.Export.cs`×4）。
若改用本库安全转换，注意 `Convert.ToDecimal(DBNull.Value)` 会抛，**不能裸换**，需包一层。

## 四、CI 发版（.github/workflows/publish-nuget.yml）

- 触发：`push: tags: v*` + `workflow_dispatch`（用户当前走**手动触发、不打 tag** 方案 B）。Secret：`NUGET_API_KEY`（勿打印）。
- 版本号自动算：`VersionPrefix=1.2` + `VersionSuffix=$([DateTime]::Now.ToString('yyyy.MMdd'))` → 今日 **1.2.2026.918**；
  **`DotNet.Util.Db.PostgreSql` 的 VersionPrefix=1.1**（是否统一 1.2 待定）。
- 流程：`restore src/DotNet.Util.Publish.slnf` → `build -c Release --no-restore` → `pack -c Release --no-build -o ./artifacts`
  → `List artifacts` → Push nupkg → Push snupkg。
- **必须先 build 再 pack --no-build**：直接 `dotnet pack` 打 solution 在 net10.0 等 TFM 上编不出 DLL，
  只报 `to be packed was not found on disk`，会**藏住真实编译错误**。
- **用 solution filter 而非整个 sln**：`src/DotNet.Util.Publish.slnf` 只含 13 个库工程，排除示例工程
  `DotNet.Test`(net6.0) 与 `DotNet.Test.452`(工程名 `DotNet.Test.46`，老式 packages.config 硬引用本地 `..\packages\` 的
  **Aspose.Words 25.9.0 + FreeSpire.Doc 12.2.0** 商业库，不入 Git → CI 编不过）。`dotnet pack` 支持 `.slnf`；
  `.slnf` 里 `solution.path` 相对 slnf 自身、projects 相对 .sln 目录，用反斜杠。
- **CI 编 net46/47/48 需参考程序集**（windows-latest 只有 4.6.2+，无 4.6.0）：`src/Directory.Build.props` 给 net4x 条件加
  `Microsoft.NETFramework.ReferenceAssemblies` 1.0.3（本地有 VS 参考程序集，不冲突）。
- ⚠️ **`dotnet nuget push` 的通配符只在"不带目录部分"时展开**：`"artifacts/*.nupkg"` / `"./artifacts/*.nupkg"`
  **不展开**，整串当字面文件名 → `error: File does not exist (./artifacts/*.nupkg).`，**报错文字与"包没生成"完全一样**，
  极易误判。可用：`"./artifacts/**/*.nupkg"`（带 `**`）、cwd 即目录时的 `"*.nupkg"`、或显式枚举。
  现行做法：pwsh `Get-ChildItem ./artifacts -Filter *.nupkg | ForEach-Object { dotnet nuget push $_.FullName ... }`
  + `--skip-duplicate`（日期制版本号同一天重跑必然重复，否则重跑必失败）。
  GitHub 的 pwsh 默认 `$ErrorActionPreference='stop'`，脚本内改回 `'Continue'`，失败靠 `$LASTEXITCODE` 判后 `exit 1`。
- 发版前置改动：`DotNet.Test` 加 `<IsPackable>false</IsPackable>`；`DotNet.Test.46.csproj` 末尾加空 `<Target Name="Pack" />`
  （否则 MSB4057）；`CHANGELOG.md` 收口 `## [1.2.2026.918] - 2026-09-18`。
- **验证 pack 必须抓退出码**，只看"已成功创建包"会漏 MSB4057（在输出前段，tail 会截断）。

## 五、多语言层（Msg）现状

- `src/DotNet.Util/Message/Msg.cs` + 内嵌语言包 `src/DotNet.Util/Resources/MsgPack*.cs`；默认 **zh-CN**、内置 **en**、
  词条 **400**（中英各一套，键序一致）。回退链 `en-US → en → zh-CN → 键名本身`，**绝不抛异常**。
- **248 个 `Msg####` 编号键已全改语义键**（Common/Logon/Validation/Confirm/Result/Ip/System/Org/Workflow/Sequence/Sign/File
  + Enum/Service/Log/Exception/Business/Console/Rmb/Qqwry/Sms）。**⚠️ 字段名 ≠ 键**：`AppMessage.Msg####` 字段名与字段数（248）
  保持不变（二进制兼容），但 `Msg.Get("Msg0001")` **已失效并静默返回 `"Msg0001"`**；查语义按项目根 `Msg-Key-Rename-Map.md`。
- 兼容决策（用户明确要求）：`AppMessage.Msg####` 与 `AppMessage.Service.*` 静态字段保持不动，多语言只改调用点。
- 枚举本地化改**读取端**：`[EnumDescription]` 须编译期常量 → 改 `EnumUtil.ToDescription`/`GetEnumDescriptions`，
  未登记词条回退特性原文（不能返回键名）；`Status.cs`/`AuditStatus.cs` 零改动。
- 键**区分大小写**（内层 `StringComparer.Ordinal`，外层 culture 字典仍 OrdinalIgnoreCase 以容忍 `"en-us"`）；
  锁外静态字段（`_initialized`/`_language`）一律 `volatile`。`Msg.Get` 64~93ns，319 个调用点全在异常/日志/失败分支，无热路径。
- 扫描遗留中文必须**大小写不敏感**（曾漏 `errorMessage` 120 处 / `statusMessage` 18 处）。

## 六、Db 测试补齐进度

- **P1~P5 全部完成（累计 119 个连库用例，见 `Db-Test-Coverage-Plan.md`）**；net8.0 Db 288/288、net48 集成 105+1。
- 存储过程：`GetRecordByPage` 需**手工建**；`DupTestUserList`/`DupTestUserGetById` 由 fixture `EnsureProcedures()` 幂等建
  （`CREATE PROCEDURE` 须为批处理首语句 → 用 `EXEC('')` 包裹）。调 sp 分页重载**必须显式传 sortExpression**，
  否则 sp 内拼出 NULL 后 `EXECUTE(NULL)` 报错。
- 护栏：连接串解析出 Database，非白名单 `DotNetUtilTest` 直接 Assert.Fail；串行集合 `SqlServerTestCollection`。
- 其他坑：`Delete(table, null)` 有 `List<KeyValuePair>`/`string` 重载二义 → 显式强转；
  `SqlBuilder.SetWhere(List)` 传 null 会 NRE；`BatchDelete` 无返回值（靠 `AggregateInt(MIN(Id))` 递归终止，只能断言剩余行数）；
  `Truncate` 会重置 IDENTITY 种子；`DbHelper.ExecuteReader` 用 `CommandBehavior.CloseConnection` → **reader 必须 Dispose**。
- `DbUtil` 不带 connectionString 的重载一律读 public static 字段 `DbUtil.ConnectionString`/`CurrentDbType`（`DbUtil.cs:179/184`）
  → 测试必须用 `_fixture.UseStaticConnection()` 作用域包裹并还原。
- net48 下 `AggregateDecimal` 用 `#if NET48 Skip` 跳过（见第三节 🟡）。

## 七、用户约定

- **禁止自动 `git commit` / `push` / 打 tag**：改动只在本机完成，汇报后等用户明确确认；Git 命令只给文本、
  由用户在 VS/PowerShell 执行。发版/提交拆成独立 commit。
- 修复/升级后必须**逐项目/TFM 构建验证 0 错误**；重视 net4x（net46/47/48）老用户兼容性。
- 代码检查先输出 Bug 清单 + 严重度（🔴/🟠/🟡），等确认后再改；严格增量修改、不擅自扩大范围。
- 升级 NuGet 前核实许可证（NPOI 2.8.0 商业 EULA 即回退，Apache-2.0 的 2.7.6 为安全上限）与 TFM 兼容性。
