# 需求文档（Requirements）

目的
- 为 AI 驱动的代码重写与维护提供明确的需求与验收标准。
- 确保在重构或自动生成代码时不破坏对外兼容性并满足功能与性能目标。

范围
- 涵盖 `DotNet.Util` 代码库内的核心功能模块：通用工具库（`DotNet.Util`）、扩展工具（`DotNet.Util.Plus`）、数据库适配层（`DotNet.Util.Db*`）、缓存（`DotNet.Util.Cache`）及示例业务项目。

功能性需求
1. 基本功能
   - 提供稳定的字符串、路径、IO、XML/JSON、反射等常用工具方法。
   - 提供文档（Word/Excel）导出和处理工具（在 `DotNet.Util.Plus` 中）。
   - 提供数据库表达式与多数据库适配器（MySql/Oracle/PostgreSql/SQLite/OleDb），供上层调用。
   - 提供缓存抽象与实现。

2. 对外兼容性
   - 公开的 NuGet 包 API 必须保持向后兼容，任何破坏兼容性的修改需在文档中说明并提升主版本号。

3. 多目标支持
   - 编译与测试需覆盖库声明的目标框架：`net452/net46/net47/net48/net6.0/net7.0/net8.0/net9.0/netstandard2.0/netstandard2.1`。

非功能性需求
- 可维护性：代码应清晰、模块化、易于测试与审查；AI 改写后不得降低可读性。
- 文档完备：提供 README、INSTALL、CONTRIBUTING、CHANGELOG、API 文档、架构说明、需求文档。
- 自动化：提供 CI（构建、测试、API 校验）、自动发布（可选）。
- 性能：数据库常用操作需在合理效率内；避免不必要的 IO 与内存分配。

验收标准
- 所有现有单元测试（如有）通过；新增或修改的功能对应有测试覆盖。
- `PUBLIC_API.txt` 与 `API_PROTECTION.md` 中列出的公开 API 保持一致，或在变更时更新并提供迁移说明。
- 文档齐备（至少包含 README、INSTALL、CONTRIBUTING、CHANGELOG、ARCHITECTURE、REQUIREMENTS）。

用例示例
- 用例 A：使用 `DotNet.Util` 提供的路径工具规范化路径（跨 Windows / Linux）。
- 用例 B：使用 `DotNet.Util.Plus` 导出 Word 模板（模板占位符替换）。
- 用例 C：通过 `DotNet.Util.Db.MySql` 将实体保存到 MySQL 数据库。

约束与假设
- 目标环境可能包括仅支持 .NET Framework 的旧系统与支持 modern .NET 的新系统。
- 部分第三方组件在不同目标框架下版本不一致，重写需兼容条件引用。

维护人/审批
- 建议由主维护者（仓库作者）审批对外兼容性变更并合并大版本更新。