# DotNet.Util 项目长期笔记

## 一、沙箱构建/测试环境
- 每条 Bash 命令开头加：`export PATH="/usr/bin:/bin:$PATH:/c/Users/Troy/.workbuddy/binaries/PortableGit/versions/1.2.0/usr/bin:/c/Program Files/dotnet"`，dotnet 在 `/c/Program Files/dotnet`。
- env 清空会令 restore/build 崩（`NuGet.targets(782) Path.Combine(null)`）→ 先 `export` 这 6 个：`NUGET_PACKAGES='C:/Users/Troy/.nuget/packages'`、`APPDATA`/`LOCALAPPDATA`→`C:/Users/Troy/AppData/...`、`USERPROFILE`/`HOME`=`C:/Users/Troy`、`PROGRAMDATA`=`C:/ProgramData`。stderr 的 `dirname: command not found`/`cd: null directory` 无害。
- 必须同时加 `-p:GenerateAssemblyInfo=false -p:GenerateTargetFrameworkAttribute=false`（IDE 持 obj 锁 → MSB3491；单加 `-p:TargetFrameworks` 触发 CS0579）。`dotnet build-server shutdown` 对锁无效。
- CS0579：勿把 `OutputPath`/`IntermediateOutputPath` 指到工程树内（glob 进 `*.AssemblyAttributes.cs` → 多工程批量爆）。临时目录放工程树外。
- 沙箱 `C:/` 根写被拒 → 重定向失败致 exit=1 误判；log/输出放工作区内。
- Edit 工具禁并行改同文件（后写覆盖前写仍 success）→ 同文件多处编辑串行 + grep 复核。

## 二、测试基线与 flaky
- 基线 1204 例（排除集成），全量 net8.0 ≈25~38s；MsgTests 40 例。
- `IntegrationTests`（SQL Server/Redis/QQWry）无外部依赖必 FAIL（预期）。SQL Server 集成可用（本机 1433/1434，Windows 集成免密）：`export DUP_TEST_SQLSERVER='Server=127.0.0.1,1433;Database=master;Trusted_Connection=True;TrustServerCertificate=True;'`
- flaky 判据：失败点轮换=并行竞争。`HttpUtilTests`（临时端口，`WebException:(410)Gone`，单跑 8/8）、`CacheUtilTests.RemoveByRegex`（共享静态缓存，单跑 3/3）。
- net48 勿一次跑整个 Db 命名空间（217 例）→ SIGTERM + stdout 空 + 退出码 1；net48 验证拆 filter（net8.0 无限制）。
- 切 `Msg.CurrentLanguage` 的断言类标 `[Collection(MsgTestCollection.Name)]`（DisableParallelization=true）。

## 三、已知缺陷
- 🔴 `DbHelper.Fill` 丢返回值静默吞异常：`Fill(DataTable dt,...)` catch 后把**局部** dt 置 null 返回，调用方 dt 不变 → 必须接返回值判空。`DbUtil.LockNoWait.cs` 已修（null→-1）。旧写法 12 处：`SQLBuilder.cs:660`、`DbUtil.Common.cs:356,377`、`DbUtil.Method.cs:162`、`DbUtil.ParentChildrens.cs:70,112,140,238,291`、`BaseExceptionManager.Manual.cs:159`、`BaseManager.PreviousNext.cs:53`。彻底修需改 `Fill` 语义（行为变更，待评估）。
- 🟡 net48 测试宿主禁用 NewLife.Core `ToDecimal`/`ToDouble`（用例通过后宿主崩，返回值正确；`ToInt`/`Convert.ToDecimal` 正常；net8.0 正常）。升 11.18→11.19.2026.901 仍崩，升级健康（10 TFM+14 项目 0 错误）。`DefaultConvert.ToDecimal` 直调也崩 → 嫌疑 `stackalloc`+`Span<Char>`+Range 切片（net4x 靠 System.Memory 垫片）。注意 `Convert.ToDecimal(DBNull.Value)` 抛，不能裸换。仅供测试宿主，生产/net4x 用户不受影响。

## 四、CI 发版（publish-nuget.yml）
- 触发 `push: tags: v*` + `workflow_dispatch`（用户手动、不打 tag）。Secret `NUGET_API_KEY` 勿打印。
- 版本 `VersionPrefix=1.2`+`VersionSuffix=$([DateTime]::Now.ToString('yyyy.MMdd'))`；PostgreSql 1.1（待统一）。同版本号不可覆盖重推。
- 流程：`restore src/DotNet.Util.Publish.slnf`→`build -c Release --no-restore`→`pack --no-build -o ./artifacts`→列产物→push。**必须先 build 再 pack --no-build**（直接 pack 会在 net10.0 等编不出 DLL 只报 `not found on disk` 藏错误）。✅ 2026-09-18 首发 13 包成功。
- 用 `.slnf`（仅 13 库工程，排除 `DotNet.Test`(net6.0)/`DotNet.Test.46` 硬引用未入 Git 的 Aspose/Spire）。
- net4x 参考程序集：`Directory.Build.props` 给 net46/47/48 加 `Microsoft.NETFramework.ReferenceAssemblies` 1.0.3 + **`PrivateAssets="all"`**（否则泄漏 nuspec）。改后须重 restore。
- ⚠️ `dotnet nuget push` 通配符仅无目录部分时展开 → `"./artifacts/*.nupkg"` 不展开报 `File does not exist`（与"包没生成"同文案）。用 `"./artifacts/**/*.nupkg"` 或 pwsh 枚举 + `--skip-duplicate`。
- ⚠️ NuGet icon 不显示先查网络：`api.nuget.org` 被 302 到 `nuget.azure.cn` 镜像返回 `application/octet-stream`+`nosniff` 拒渲染 → 包侧无问题勿改 csproj；验证 `wsrv.nl/?url=api.nuget.org/v3-flatcontainer/<id>/<ver>/icon`。
- 包内 README 在 nuget.org 渲染，依赖段须与 csproj 同步（曾落后一年已修 11 个）。
- 发版前置：`DotNet.Test` 加 `IsPackable=false`；`DotNet.Test.46.csproj` 加空 `<Target Name="Pack" />`；CHANGELOG 收口。`pack` 须抓退出码。

## 五、多语言层 Msg
- `Msg.cs`+`Resources/MsgPack*.cs`；默认 zh-CN、内置 en、词条 400。回退 `en-US→en→zh-CN→键名`，绝不抛。
- 248 个 `Msg####` 编号键已改语义键（字段名≠键，`Msg.Get("Msg0001")` 失效静默返回键名；查语义见 `Msg-Key-Rename-Map.md`）。`AppMessage.Msg####`/`Service.*` 静态字段保持不动（二进制兼容），只改调用点。
- 枚举本地化改读取端 `EnumUtil.ToDescription`/`GetEnumDescriptions`（未登记回退特性原文）。
- 2026-09-18 强类型层 `Msg.Typed.cs`（400 成员，21 分组）。不提供 culture 重载；成员只持键，`Register`/`LoadJsonOverride` 自动生效。
- ⚠️ `DotNet.Util.Msg` 同名冲突（2026-09-15，未修）：`DotNet.Web.UI.BasePage` 包内另有同名 `Msg`（WebForm 弹窗）→ 本包 CS0436 警告，消费端两包同引时 `Msg` 二义。根治改名 `WebMsg`（破坏性，待大版本）。
- 键区分大小写；扫描遗留中文须大小写不敏感。`src/doc/*.md` 已转 UTF-8 无 BOM（全仓非 UTF-8=0）；Edit 保持原编码，改非 UTF-8 后须解码验证。勿加 `.gitattributes eol=crlf`（CRLF/LF 并存）。

## 六、Db 测试
- P1~P5 完成（119 连库用例）；net8.0 Db 288/288、net48 集成 105+1。
- `GetRecordByPage` sp 需手工建；`DupTestUserList`/`DupTestUserGetById` 由 fixture `EnsureProcedures()` 幂等建（`CREATE PROCEDURE` 须批首语句 → `EXEC('')` 包裹）。调 sp 分页重载须显式传 sortExpression（否则 `EXECUTE(NULL)` 报错）。
- 护栏：连接串 Database 非白名单 `DotNetUtilTest`→Assert.Fail；串行集合 `SqlServerTestCollection`。
- 坑：`Delete(table,null)` 重载二义→显式强转；`SqlBuilder.SetWhere(List)` 传 null→NRE；`BatchDelete` 无返回值（靠 `AggregateInt(MIN(Id))` 递归）；`Truncate` 重置 IDENTITY；`ExecuteReader` 用 `CloseConnection`→reader 须 Dispose。
- `DbUtil` 无 connectionString 重载读 static `DbUtil.ConnectionString`/`CurrentDbType` → 测试用 `_fixture.UseStaticConnection()` 包裹还原。net48 `AggregateDecimal` `#if NET48 Skip`。

## 七、用户约定
- 禁自动 commit/push/tag：改动本机完成，汇报等确认；Git 命令只给文本。发版/提交拆独立 commit。
- 修复/升级后必逐项目/TFM 构建验证 0 错误；重视 net4x 兼容。
- 代码检查先给 Bug 清单+严重度（🔴/🟠/🟡），确认后再改；严格增量、不擅自扩范围。
- 升级 NuGet 先核实许可证（NPOI 2.8.0 商业 EULA 回退，Apache-2.0 2.7.6 安全上限）与 TFM 兼容。
