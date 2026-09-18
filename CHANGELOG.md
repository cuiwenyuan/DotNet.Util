# CHANGELOG

All notable changes to this project will be documented in this file.

The format is based on "Keep a Changelog" and follows Semantic Versioning.

## [Unreleased]

### Added
- New features.
- **多语言（输出消息本地化）**：新增 `Msg` 消息层（`src/DotNet.Util/Message/Msg.cs`）与内嵌语言包
  `MsgPack.zh-CN.cs` / `MsgPack.en.cs`，**默认中文（zh-CN）**，内置第一语言包**英文（en）**，共 400 条词条。
  - 支持 `Msg.Get` / `Msg.Format`（占位符）/ `Msg.Register`（代码注册）/ `Msg.LoadJsonOverride`（外部 JSON 覆盖）。
  - 回退链 `en-US → en → zh-CN → 键名本身`，任何情况不抛异常；`GetOrDefault` 可指定默认值。
  - 覆盖：异常消息、业务状态消息、日志、控制台输出、`AppMessage` 消息（248 条，键已语义化）、
    `AppMessage.Service`（33 键）、枚举描述（70 条）。
  - 新增 `Localization.md` 使用文档，README 增加中英文多语言说明。
  - **键命名语义化**：原 248 个 `Msg####` 编号键（`Msg0001` / `Msg9999` / `Msgs965` 等）全部按中文语义
    重命名为 11 个业务前缀（`Common` / `Logon` / `Validation` / `Confirm` / `Result` / `Ip` / `System` /
    `Org` / `Workflow` / `Sequence` / `Sign` / `File`），其中 6 个与已有语义键同值的编号键合并删除
    （406 → 400）。完整映射见 `Msg-Key-Rename-Map.md`。字段名 `AppMessage.Msg####` 与字段数（248）不变。

### Changed
- Changes in existing functionality.
- **`AppMessage.Msg####` / `AppMessage.Service.*` 静态字段保留不动**（保持二进制兼容），
  字段值仍为中文默认值；多语言场景请改用 `Msg.Get("<语义键>")`（字段名不再等于语言包键）。
- **`EnumUtil.ToDescription` / `GetEnumDescriptions` 改为本地化读取**：先取 `[EnumDescription]` 特性文本作默认值，
  再查语言包键 `Enum.<类型>.<成员>`；未登记词条回退特性原文。`Status.cs` / `AuditStatus.cs` 定义文件零改动。
- 异常/状态/日志/控制台消息调用点改为走 `Msg` 层（共 190 余处），对外行为在默认语言下不变。
- **依赖升级：`NewLife.Core` 11.18.2026.801 → 11.19.2026.901**（MIT，声明支持 .NET Framework 4.5 ~ .NET 10）。
  - 验证：`DotNet.Util` 10 个 TFM（net46/47/48、net6.0~net10.0、netstandard2.0/2.1）逐档构建 **0 错误**；
    其余 13 个项目同档构建 **0 错误**；net8.0 全量回归 **1225 全绿**、Db 集成 **288 全绿**、net48 集成 **105 通过 + 1 跳过**。
  - ⚠️ 该版本**未修复** net48 测试宿主下 `object.ToDecimal()` / `object.ToDouble()` 导致的宿主崩溃
    （见 Fixed 之后说明与 `Db-Test-Coverage-Plan.md` 缺陷 2）。

### Deprecated
- Soon-to-be removed features.

### Removed
- Deprecated features removed in this release.

### Fixed
- Bug fixes.
- **修复 `DbUtil.LockNoWait` 静默吞异常、恒返回 0**：`DbHelper.Fill` 内部捕获异常后会把
  它自己的局部变量置 `null` 并返回（`DbHelper.Method.cs`），而 `LockNoWait` 丢弃了 `Fill` 的返回值、
  继续使用自己 `new` 出来的空表，导致异常被吞掉、`catch` 分支的 `-1` 永远不可达。
  现改为接收 `Fill` 返回值，`null` 即视为执行失败并返回 `-1`（Oracle 锁冲突场景语义正确）。
  修复后 SQL Server（不支持 `FOR UPDATE NOWAIT`）上由"恒返回 0"变为"返回 -1"。

### Security
- Vulnerability fixes.

---

## [vX.Y.Z] - YYYY-MM-DD

### Added
- 

### Changed
- 

### Deprecated
- 

### Removed
- 

### Fixed
- 

### Security
- 

Release guidelines
- Update the `Unreleased` section during development with categorized entries.
- When preparing a release, move entries from `Unreleased` to a new version section and set the release date.
- Use semantic versioning: MAJOR.MINOR.PATCH.
- Reference PR numbers and contributors where appropriate, e.g., `(#123) �� @contributor`.

Automation (optional)
- Consider adding a release workflow on CI to validate build and tests before publishing a release and updating the changelog.

