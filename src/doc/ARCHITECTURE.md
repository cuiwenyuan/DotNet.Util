# 技术架构文档（Architecture）

概述
- `DotNet.Util` 为类库集合，按功能划分为若干子项目：核心工具、扩展工具、数据库适配、缓存、示例/业务项目。
- 目的是提供可重用、轻量级的工具函数与跨数据库的数据访问适配层，供上层应用或库引用。

模块划分
- `DotNet.Util`（核心）
  - 职责：字符串、路径、反射、序列化辅助、HTTP 辅助等基础工具。
  - 依赖：`NewLife.Core`、`Newtonsoft.Json`（部分目标框架按条件引用）。

- `DotNet.Util.Plus`（扩展）
  - 职责：提供基于 NPOI 的 Excel 支持、Word 模板导出、压缩类操作、证书处理等高级工具。
  - 依赖：`NPOI`, `SharpZipLib`, `Portable.BouncyCastle`（条件为目标框架）

- `DotNet.Util.Db*`（数据库适配）
  - 职责：封装数据库访问、表达式构建、并提供 MySql/Oracle/PostgreSql/SQLite/OleDb 提供者。
  - 注意：SQL 生成与执行对外语义要稳定，避免在非必要情况下改变 SQL 行为。

- `DotNet.Util.Cache`（缓存）
  - 职责：提供本地缓存辅助功能与抽象。

- `DotNet.Business*`（示例）
  - 职责：演示如何在业务中使用工具库与数据库适配器。

依赖关系
- `DotNet.Util.Plus` -> `DotNet.Util`
- `DotNet.Business*` -> `DotNet.Util`、`DotNet.Util.Db*`

构建与 CI 建议
- 在 CI 中使用多阶段矩阵：针对 `net6.0`, `net7.0`, `net8.0`（Linux/Windows）和 `net472/net48`（Windows）进行构建与测试。
- 添加 API 兼容性检查阶段：生成并对比 `PUBLIC_API.txt`。

数据流（示例：数据库访问）
1. 业务层调用 ORM / 数据适配层接口。
2. 数据适配层解析实体/表达式，生成 SQL。
3. 适配器在内部使用 `DbProviderFactory` 或专用驱动执行 SQL 并返回结果。

可扩展性与插件点
- 新数据库提供者：按 `DotNet.Util.Db.*` 模式新增项目并实现适配接口。
- 增加 IO 或序列化支持：在核心库增加工具类并保持向后兼容的静态方法或扩展方法。

安全与兼容性注意事项
- 避免在 SQL 生成中引入可注入风险；优先使用参数化查询。
- 保持 `public` API 行为稳定，任何行为改变需在 `CHANGELOG.md` 与 `API_PROTECTION.md` 注明。

监控与性能
- 库本身不运行服务，但建议对数据库访问热点方法添加性能基准并在 CI 中运行（选项）。

图示（文本）

  业务代码
     |
     v
  DotNet.Util (核心工具)
     |
     v
  DotNet.Util.Plus (扩展功能)
     |
     v
  DotNet.Util.Db.* (数据库适配器)
     |
     v
  数据库（MySQL/Oracle/PostgreSQL/SQLite/OleDb）


维护与演进
- 小步快跑：优先修复 bug 与补文档，针对破坏性更改发布新主版本。
- 使用 `PUBLIC_API.txt` 与 API 对比工具在 PR/CI 中拦截意外的 public API 变更。