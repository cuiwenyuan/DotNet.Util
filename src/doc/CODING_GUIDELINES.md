编码规范（Coding Guidelines）

版本：1.0
适用范围：DotNet.Util 仓库（支持 .NET Framework 4.x、.NET Standard 2.x、.NET 6/7/8/9 项目）

说明：此文档基于本仓库现有代码风格与常见问题（如 SQL 拼接、资源释放、并发、可空处理等）制定，目的是统一编码风格、提高可维护性、安全性与可测试性。

1. 通用原则
- 优先可读性和一致性。代码应易于理解、易于审阅。
- 优先安全性：避免 SQL 注入、避免资源泄露、避免未处理异常。
- 小步改进：对历史代码采用逐步重构策略，优先修复高风险区域。

2. 代码风格
- 使用 C# 8+ 语法（仓库内针对 .NET 6+ 项目），对新项目启用 nullable reference types（在项目文件中 <Nullable>enable</Nullable>）。
- 命名：采用 PascalCase 用于类型、方法和属性；camelCase 用于局部变量和参数。常量使用 PascalCase。
- 文件与类型一一对应：每个 public 类型放在单独文件中，文件名与类型名一致。
- 行宽约束：建议不超过 120 列。

3. 注释和文档
- 公共 API 请添加 XML 注释（summary、param、returns）。
- 复杂算法或业务逻辑简要说明目的和重要边界条件。
- 删除过期代码而不是长时间注释保留；若必须保留，添加 TODO 与关联 issue 编号。

4. 空值和可空（Nullability）
- 新模块启用 nullable reference types 并修复警告。
- 对外公开的方法在入口进行参数校验（例如 null、空字符串、范围检查）。
- 使用 `?.` 和 `??` 以安全方式访问可空引用，必要时抛出明确的 ArgumentNullException。

5. 数据访问与 SQL
- 禁止使用字符串拼接构造包含外部输入的 SQL。必须使用参数化查询或 ORM（如 Dapper/EF Core）。
- 对 LIKE 查询统一处理通配符，并使用参数（例如："%value%" 作为参数值），不要直接把 "%" 拼接到 SQL 命令文本中。
- 所有构造动态 SQL 的位置应有单元测试或集成测试验证语句与参数对应关系。

6. 异常与日志
- 捕获异常时提供业务上下文（方法参数、关键状态），再记录或包装抛出。
- 库层不应直接写入 Console；统一使用 `LogUtil` 或可注入的日志接口（建议引入 `ILogger` 抽象用于新模块）。
- 不要吞掉异常（空的 catch 块）。如需忽略异常，请记录并说明原因。

7. 资源管理（IDisposable）
- 使用 `using`/`using var` 管理数据库连接、文件、流等 IDisposable 对象。
- 在需要自定义释放逻辑时，遵循标准的 Dispose 模式（或继承仓库中的 `DisposeBase`）。
- 对异步资源在 .NET 6+ 使用 `IAsyncDisposable` 与 `await using`。

8. 并发与线程安全
- 禁止在静态字段中保存可变对象（如随机写入的集合、非线程安全缓存），若必须，使用线程安全集合或锁。
- 日志与 I/O 操作应尽量异步或批量化，避免在高并发场景中频繁同步写入。
- 对并行任务的共享状态使用显式并发控制（lock、ConcurrentDictionary、SemaphoreSlim 等）。

9. 异步编程
- 对 I/O 密集型操作（数据库访问、文件、网络）在支持的项目中优先使用 `async/await`。
- 异步方法应返回 Task/Task<T>；避免使用 `async void`（仅事件处理器除外）。

10. 单元测试与可测试性
- 关键业务逻辑必须有单元测试覆盖。优先覆盖数据访问边界、异常场景与权限逻辑。
- 对外部依赖（数据库、文件、时钟、环境）使用抽象或接口以便 Mock。
- 将不稳定的集成测试放在单独的测试类别并可通过 CI 跳过或以环境变量控制。

11. 静态分析与格式化
- 在仓库中加入 `.editorconfig` 以统一编码风格。
- 在 CI 中运行 `dotnet format`、Roslyn 分析器（Microsoft.CodeAnalysis.Analyzers 或 StyleCop）并修复严重警告。

12. 依赖管理与安全
- 使用最新的安全支持版本的第三方包；定期运行依赖检查（例如 Dependabot）。
- 禁止在仓库内硬编码秘密（密码、API Key）。配置文件或密钥通过安全渠道管理（环境变量、KeyVault 等）。

13. 提交与 PR 规范
- 提交信息使用简短的英文或中英结合：<范围>: <简短描述>（例如: BaseException: parameterize Search）。
- 每个 PR 聚焦单一变更点，包含描述、影响范围和回归测试指引。
- 在 PR 中运行并通过单元测试与静态分析结果后再合并。

14. CI / 构建
- 推荐为主要分支启用 GitHub Actions：构建（所有 target frameworks）、单元测试、静态分析与基本安全扫描。
- 在 CI 中对变更做快速回归（编译 + 主要测试），对重要模块做更深层次的验证。

15. 迁移与多目标框架注意事项
- 当在 .NET Framework 与 .NET 6+ 同时维护代码时，避免使用仅在某一平台存在的 API；或为不同目标提供分支/条件编译。
- 在多目标项目中，使用条件编译符号（#if NET6_0）且明确标注理由。

16. 性能与日志策略
- 避免在高循环中大量同步日志输出。对非开发环境使用 INFO/WARN/ERROR 策略并支持采样。
- 对热点方法进行基准分析，必要时引入缓存或批量操作以降低 DB/IO 压力。

17. 本仓库特定约定
- 遵循现有的 Base* 管理器命名与实体命名规则（例如 `BaseExceptionManager`, `BaseModuleManager`）。
- 对数据库表名、字段名使用对应的 `Entity` 常量（如 `BaseExceptionEntity.FieldXxx`），避免硬编码字符串。

附件：建议工具链
- dotnet format
- Roslyn Analyzers / StyleCop
- FxCop/CodeQL（安全审计）
- GitHub Actions（CI）

---

若需我：
- 在仓库添加 `.editorconfig` 与基础 GitHub Actions CI 工作流，或
- 为某些方法（例如 `GetDataTableByPage`）批量参数化 SQL 并提交 PR。

请选择下一步任务。
