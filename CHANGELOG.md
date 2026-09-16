# CHANGELOG

All notable changes to this project will be documented in this file.

The format is based on "Keep a Changelog" and follows Semantic Versioning.

## [Unreleased]

### Added
- New features.
- **多语言（输出消息本地化）**：新增 `Msg` 消息层（`src/DotNet.Util/Message/Msg.cs`）与内嵌语言包
  `MsgPack.zh-CN.cs` / `MsgPack.en.cs`，**默认中文（zh-CN）**，内置第一语言包**英文（en）**，共 406 条词条。
  - 支持 `Msg.Get` / `Msg.Format`（占位符）/ `Msg.Register`（代码注册）/ `Msg.LoadJsonOverride`（外部 JSON 覆盖）。
  - 回退链 `en-US → en → zh-CN → 键名本身`，任何情况不抛异常；`GetOrDefault` 可指定默认值。
  - 覆盖：异常消息、业务状态消息、日志、控制台输出、`AppMessage.Msg####`（248 键）、
    `AppMessage.Service`（33 键）、枚举描述（70 条）。
  - 新增 `Localization.md` 使用文档，README 增加中英文多语言说明。

### Changed
- Changes in existing functionality.
- **`AppMessage.Msg####` / `AppMessage.Service.*` 静态字段保留不动**（保持二进制兼容），
  字段值仍为中文默认值；多语言场景请改用 `Msg.Get("<字段名>")`。
- **`EnumUtil.ToDescription` / `GetEnumDescriptions` 改为本地化读取**：先取 `[EnumDescription]` 特性文本作默认值，
  再查语言包键 `Enum.<类型>.<成员>`；未登记词条回退特性原文。`Status.cs` / `AuditStatus.cs` 定义文件零改动。
- 异常/状态/日志/控制台消息调用点改为走 `Msg` 层（共 190 余处），对外行为在默认语言下不变。

### Deprecated
- Soon-to-be removed features.

### Removed
- Deprecated features removed in this release.

### Fixed
- Bug fixes.

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

