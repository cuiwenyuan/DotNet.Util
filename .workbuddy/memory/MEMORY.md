# DotNet.Util 项目长期笔记

## 构建/测试：绕开 IDE obj 锁（重要）

本机 IDE（WorkBuddy 桌面端，打开着 DotNet.Util 解决方案）会持有
`src/*/obj/Debug/net8.0/*.AssemblyInfoInputs.cache` 等文件的句柄，导致常规
`dotnet build` / `dotnet test` 报 `MSB3491 Access is denied`。

**解决方案**：禁用程序集信息生成，就不再需要写那个被锁的 cache 文件：

```bash
# 跑单测（排除需外部服务的 IntegrationTests）
dotnet test src/DotNet.Util.Tests/DotNet.Util.Tests.csproj -c Debug -f net8.0 \
  -p:GenerateAssemblyInfo=false -p:GenerateTargetFrameworkAttribute=false \
  --filter "FullyQualifiedName!~IntegrationTests"

# 单 TFM 编译验证（如 net48 / netstandard2.0 / net8.0）
dotnet build src/DotNet.Util/DotNet.Util.csproj -c Debug \
  -p:TargetFrameworks=net48 \
  -p:GenerateAssemblyInfo=false -p:GenerateTargetFrameworkAttribute=false
```

要点：
- 仅用 `-p:TargetFrameworks=<单TFM>` 覆盖会触发 **CS0579**（obj 内程序集特性重复），
  **必须同时**加两个 `Generate*=false` 才能过。
- `dotnet build-server shutdown` 对此锁无效（持锁者是 IDE，不是构建服务器）。
- 已知 flaky：`HttpUtilTests` 在全套并行执行时偶发失败（本机临时端口
  `HttpListener` 争用），单独复跑 8/8 通过，与业务改动无关，不必排查。
  **失败方法不固定**：2026-09-16 连跑 4 次，`Get_WithHeaders_ReturnsBody` 挂 2 次、
  `Get_ReturnsBody` 挂 1 次、1 次全过，失败数在 0~2 之间波动，异常多为
  `WebException: (410) Gone`。→ **只要失败点始终在 HttpUtilTests 类内轮换，即为 flaky**。
- 已知 flaky（同类）：`CacheUtilTests.RemoveByRegex_RemovesMatchingKeys` 在全套并行时
  偶发失败（多个测试类共享全局静态缓存，key 互相删除）。2026-09-16 连跑 4 次全量回归：
  run1 挂 CacheUtil、run2 挂 HttpUtil、run3/run4 全通过 → **失败落在不同测试上即可判定
  为并行竞争 flaky**，不要逐条当成新 bug 排查；单独复跑 CacheUtilTests 3/3 通过。
- **xUnit 全局静态状态隔离约定**：任何断言"默认中文"或会切换 `Msg.CurrentLanguage` 的
  测试类，必须标注 `[Collection(MsgTestCollection.Name)]`（定义在
  `src/DotNet.Util.Tests/Message/MsgTestCollection.cs`，`DisableParallelization = true`），
  否则并行执行时语言互相污染导致偶发失败。
- ⚠️ **另有一类更严重的环境故障（与 obj 锁无关）**：沙箱 Git Bash 会话中 `PROGRAMDATA`/`APPDATA`/`USERPROFILE`/`NUGET_PACKAGES` 等被清空 → `NuGet.targets(782) GetRestoreSettingsTask → NuGetEnvironment.GetFolderPath → Path.Combine(null)` 抛 `Value cannot be null (Parameter 'path1')`；`dotnet restore`/`build`/`test`/`nuget locals` 全面报此错（连 `dotnet new console` 也失败）。`cmd.exe`/`powershell.exe`/`reg.exe` 被沙箱安全策略拦截，不能借它们补环境。
  **已验证可用的绕过法**（在 bash 内补全 env + 跳过 restore）：
  ```bash
  export NUGET_PACKAGES='C:/Users/Troy/.nuget/packages' APPDATA='C:/Users/Troy/AppData/Roaming' \
         LOCALAPPDATA='C:/Users/Troy/AppData/Local' USERPROFILE='C:/Users/Troy' \
         PROGRAMDATA='C:/ProgramData' ALLUSERSPROFILE='C:/ProgramData' HOME='C:/Users/Troy'
  # 项目 obj/project.assets.json 已存在 → 用 --no-restore 跳过崩溃的 restore 设置求值阶段
  dotnet build <proj>.csproj -c Debug -f net8.0 --no-restore \
    -p:GenerateAssemblyInfo=false -p:GenerateTargetFrameworkAttribute=false
  ```
  注：`--no-restore` 仅绕 restore，**不**解决"全方案无 --no-restore 编译"（那仍会触发 path1）。本机（非沙箱）VS/PowerShell 无此故障。
- ⚠️ **另有一类 Shell 环境故障：PATH 被清空**（`dirname`/`ls`/`grep`/`head` 全部
  `command not found`，但 `pwd`/`cd`/`echo` 等内置命令仍可用，容易误判为工具坏了）。
  每个 Bash 调用都需重设 PATH（shell 状态不持久），在命令开头加：
  ```bash
  export PATH="/usr/bin:/bin:$PATH:/c/Users/Troy/.workbuddy/binaries/PortableGit/versions/1.2.0/usr/bin:/c/Program Files/dotnet"
  ```
  （dotnet 在 `/c/Program Files/dotnet`，实测 `dotnet --version` = 10.0.401）
  之后再叠加上面那组 `NUGET_PACKAGES/APPDATA/…` env 补全 + `--no-restore`。
  注：stderr 里的 `shim/shell-runtime-bash-env.sh: line 3: dirname: command not found`
  是无害噪音，只要 stdout 有正常输出即可忽略。
- ⚠️ **CS0579 坑（多 TFM 验证编译时）**：SDK 风格工程默认 glob `**/*.cs`（仅排除 obj/bin）。**绝不要把自定义 `OutputPath`/`IntermediateOutputPath` 指向工程目录树下**（如 `verify_obj/`）——其内自动生成的 `*.AssemblyAttributes.cs`/`*.AssemblyInfo.cs` 声明 `[assembly: TargetFrameworkAttribute(...)]`，会被当成源码一起编 → 与 SDK 生成的重复 → `CS0579 Duplicate TargetFrameworkAttribute`，且多工程批量爆发。临时验证输出目录必须建在**工程树之外**（如 `/tmp/verify`）并及时清理。

## 多语言改造（P0~P5，2026-09-15 ~ 09-16 完成，未提交）
- 消息层 `src/DotNet.Util/Message/Msg.cs` + 内嵌语言包 `src/DotNet.Util/Resources/MsgPack*.cs`；
  默认 **zh-CN**，内置 **en**，词条 **406 条**（中英各一套，键序一致）。
- 键前缀：`Common.` / `Msg####` / `Enum.<类型>.<成员>` / `Exception.` / `Result.` / `Logon.` /
  `Log.` / `Console.` / `Service.` / `Business.` / `Rmb.` `Qqwry.` `Sms.`。
- 回退链 `en-US → en → zh-CN → 返回键名本身`，**绝不抛异常**；`GetOrDefault` 可指定默认值。
- **对外兼容决策（用户明确要求）**：`AppMessage.Msg####` 与 `AppMessage.Service.*`
  **静态字段保持不动**（保二进制兼容），多语言只改调用点走 `Msg` 层；字段值留作中文默认值。
- **枚举本地化改读取端**：`[EnumDescription]` 参数必须是编译期常量，无法动态取值 →
  改 `EnumUtil.ToDescription`/`GetEnumDescriptions`，未登记词条回退特性原文（不能返回键名）。
  枚举定义文件 `Status.cs`/`AuditStatus.cs` 零改动。
- 语言包同步校验靠反射测试（`ZhCnPack_MatchesAppMessageFields` 等），改字段忘同步会立即失败。
- 文档：`Localization.md`（使用指南）+ README 中英两处小节 + CHANGELOG `[Unreleased]`。
- ⚠️ **扫描遗留中文必须大小写不敏感**：P3 因正则只写 `StatusMessage|ErrorMessage`（大写）
  漏掉小写局部变量 `errorMessage`(120 处) / `statusMessage`(18 处)，到 P5 才补齐。
  判定"已清"前务必显式枚举所有变体。
- **性能实测（2026-09-16）**：`Msg.Get` 单次 **64~93ns**（Release/net8.0），相对编译期常量
  增量约 85ns；`Msg.Format` ~410ns；`EnumUtil.ToDescription` ~2.1µs 但 96% 是既有反射成本
  （`GetType`+`ToString`+`GetCustomAttributes`），**非多语言引入**。319 处调用点全部落在
  异常/日志/失败分支，**无热路径** → 结论：不构成性能问题。
- **消息键区分大小写（Ordinal）**：`RegisterCore` 内层 pack 用 `StringComparer.Ordinal`，
  外层 culture 字典仍 `OrdinalIgnoreCase`（容忍 `"en-us"`）。改前已确认无调用点依赖
  大小写不敏感。有 2 个测试锁死该行为（`Get_KeyIsCaseSensitive_*` / `Get_CultureIsCaseInsensitive`）。
- **锁外静态字段已全部 volatile**：`Msg._initialized`（bool）与 `Msg._language`（string）
  均加锁外读写，2026-09-16 已都改为 `volatile`（`volatile string` 在 net46 起即支持，已验证）。
  后续新增锁外读写的静态字段沿用此约定。

## 基线数据
- 测试数基线（2026-09-01 收尾后）：**1090 个（1089 通过 / 0 失败 / 0 跳过，排除集成测试）**。
- 测试数基线（2026-09-16 多语言 P0~P5 + 性能优化 2 项后）：**1204 个**（排除集成测试），
  其中 `MsgTests` 相关集合 40 个。全量 net8.0 跑一次约 25~38s。
- 集成测试 `IntegrationTests`（SQL Server / Redis / QQWry）无外部依赖时必 FAIL，属预期。

## 用户约定
- **禁止自动 `git commit` / `push` / 打 tag**：改动只在本机完成，汇报后等用户明确确认，
  再由 AI 给出 Git 命令文本供用户自行执行（用户偏好 PowerShell 执行 Git）。
- 修复/升级后必须**逐项目/TFM 构建验证 0 错误**；关注 net4x（net46/47/48）老用户兼容性。
- 代码检查先输出 Bug 清单 + 严重度，等确认后再改。
