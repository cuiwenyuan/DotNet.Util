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
- 已知 flaky：`HttpUtilTests` 在全套并行执行时偶发 1 个失败（本机临时端口
  `HttpListener` 争用），单独复跑 8/8 通过，与业务改动无关，不必排查。
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
- ⚠️ **CS0579 坑（多 TFM 验证编译时）**：SDK 风格工程默认 glob `**/*.cs`（仅排除 obj/bin）。**绝不要把自定义 `OutputPath`/`IntermediateOutputPath` 指向工程目录树下**（如 `verify_obj/`）——其内自动生成的 `*.AssemblyAttributes.cs`/`*.AssemblyInfo.cs` 声明 `[assembly: TargetFrameworkAttribute(...)]`，会被当成源码一起编 → 与 SDK 生成的重复 → `CS0579 Duplicate TargetFrameworkAttribute`，且多工程批量爆发。临时验证输出目录必须建在**工程树之外**（如 `/tmp/verify`）并及时清理。

## 基线数据
- 测试数基线（2026-09-01 收尾后）：**1090 个（1089 通过 / 0 失败 / 0 跳过，排除集成测试）**。
- 集成测试 `IntegrationTests`（SQL Server / Redis / QQWry）无外部依赖时必 FAIL，属预期。

## 用户约定
- **禁止自动 `git commit` / `push` / 打 tag**：改动只在本机完成，汇报后等用户明确确认，
  再由 AI 给出 Git 命令文本供用户自行执行（用户偏好 PowerShell 执行 Git）。
- 修复/升级后必须**逐项目/TFM 构建验证 0 错误**；关注 net4x（net46/47/48）老用户兼容性。
- 代码检查先输出 Bug 清单 + 严重度，等确认后再改。
