模块详细设计文档

版本：1.0
作者：自动化生成（GitHub Copilot）
日期：2026-05-10

摘要
----
本文档为 `DotNet.Util` 仓库的模块级详细设计（Module Design）。基于现有代码（`DotNet.Business`、`DotNet.Util`、`DotNet.Model` 等），说明各模块职责、主要类与接口、交互流程、错误处理、缓存策略、并发考虑、扩展点与测试建议，便于开发人员实现、维护与扩展。

适用范围
---------
- 本文档面向仓库维护者、开发者和架构师。涉及平台：.NET Framework 4.x、.NET Standard 2.x、.NET 6/7/8/9。

目录
-----
1. 模块概览
2. 模块详细（按模块）
   - DotNet.Model
   - DotNet.Util
   - DotNet.Business
   - DotNet.Test._452（示例/测试）
3. 典型调用流程
4. 关键交互接口与契约
5. 数据库与表约定说明
6. 缓存与一致性策略
7. 异常处理与日志策略
8. 并发与线程安全要点
9. 可扩展性与插件点
10. 测试策略
11. 部署与配置注意事项
12. 迁移与现代化建议

1. 模块概览
---------------
- `DotNet.Model`：数据实体、字段常量、表名常量，作为 DAL 与业务层的契约。
- `DotNet.Util`：通用工具库，包含 `DbUtil`、`DbHelperFactory`、`LogUtil`、`CacheUtil`、`ValidateUtil`、`JsonUtil`、`DisposeBase` 等。
- `DotNet.Business`：业务层管理器（Manager）集合，如 `BaseManager`、`BaseExceptionManager`、`BaseModuleManager`，实现核心业务逻辑与流程控制。
- `DotNet.Test._452`：示例控制台程序，用于本地功能演示与性能测试，不作为生产组件被引用。

2. 模块详细设计
------------------
2.1 DotNet.Model
- 职责：定义实体类与字段/表常量，提供从 `DataTable` 到实体的映射辅助（`BaseEntity.Create<T>` 等）。
- 重要类/接口：`BaseEntity`、`*Entity`（如 `BaseExceptionEntity`）
- 设计要点：实体仅表示数据结构；字段名常量用于构建 SQL/参数，避免代码硬编码字段字符串。

2.2 DotNet.Util
- 职责：封装数据库访问、缓存、日志、校验和公用辅助方法。
- 重要类/接口：
  - `IDbHelper` / `DbHelperFactory`：数据库适配器抽象，负责创建具体 DB 实现并提供 `ExecuteNonQuery`、`Fill`、`ExecuteScalar` 等。
  - `DbUtil`：静态便捷 API，基于 `DbHelperFactory` 实现常用操作。
  - `CacheUtil`：缓存读写封装，支持本地缓存与可选 Redis（需配置）。
  - `LogUtil`：日志写入封装，建议将其适配到可注入的 `ILogger`。
  - `DisposeBase` / `DisposeHelper`：统一的 Dispose 模式与辅助方法。
- 设计要点：工具模块尽量无副作用、线程安全；对外提供最小公共契约，避免全局可写静态数据。

2.3 DotNet.Business
- 职责：业务操作集合、权限/模块管理、异常记录、审计、发料/审核等通用方法。
- 重要类：
  - `BaseManager`：提供 CRUD、BatchSave、Audit、Issue、UndoAudit 等通用实现。关键字段：`PrimaryKey`、`CurrentTableName`。
  - `BaseExceptionManager`：集中异常存储、Search/分页/统计等。
  - `BaseModuleManager`：菜单模块管理，树形数据、权限视图组装等。
  - `ServiceUtil`：服务调用模板，封装 DB 生命周期、计时与日志记录。
- 设计要点：业务层作为中间层，不直接对外依赖 UI；业务方法需接受 `UserInfo` 或从上下文获取，明确事务边界与缓存失效策略。

2.4 DotNet.Test._452
- 职责：演示测试、性能验证和工具使用样例（如 Word/Aspose 示例）。
- 设计要点：保留但隔离于主库，避免在 CI 或生产路径执行测试脚本。

3. 典型调用流程
------------------
场景：客户端请求数据（通过服务或直接调用 Manager）
1. 客户端 -> ServiceUtil.ProcessXxx（校验授权）
2. ProcessDbHelp 创建 `IDbHelper`（以及事务）
3. Business Manager 执行业务逻辑（可能读取 CacheUtil）
4. 若需 DB，Manager 调用 `DbHelper` 执行 SQL（使用参数化）
5. 结果返回给 ServiceUtil，记录耗时与日志
6. 若发生异常，ServiceUtil 捕获并调用 `BaseExceptionManager.LogException` 记录异常

4. 关键交互接口与契约
---------------------
- IDbHelper
  - Fill(DataTable, string, IDbDataParameter[])
  - ExecuteNonQuery(string, IDbDataParameter[], CommandType)
  - ExecuteScalar(string, IDbDataParameter[], CommandType)
  - MakeParameter(name, value)
- CacheUtil.Cache<T>(string key, Func<T> factory, bool sliding = false, bool isRedis = false, TimeSpan? cacheTime = null)
- BaseManager.GetDataTableByPage(...)

契约要点：
- `IDbHelper.MakeParameter` 返回数据库特定参数占位符，`DbHelper.GetParameter(name)` 返回 SQL 文本中的参数占位符（例如 `@name`）。
- 所有对外暴露方法应声明其对 null 参数和异常的行为。

5. 数据库与表约定说明
----------------------
- 实体类提供 `CurrentTableName` 与 `FieldXxx` 常量，业务层使用这些常量构造 SQL。
- 推荐约定：所有用户输入应作为参数传入 SQL，且 LIKE 参数应在参数值中包含 `%`。
- 分表/多租户：代码中有按公司分表注释示例（GetTableSuffix），如启用请统一表后缀逻辑并确保索引/迁移策略。

6. 缓存与一致性策略
---------------------
- 缓存用例：静态字典、表级数据（例如模块树）使用 24 小时缓存。
- 缓存失效：业务写操作成功后调用 `RemoveCache()` 来主动清理相关缓存。
- 建议：对关键写路径添加缓存删除或消息总线通知以确保分布式环境一致性。

7. 异常处理与日志策略
----------------------
- 统一点：`BaseExceptionManager.LogException` 负责持久化异常并可写入 Windows 事件。
- 服务模板：`ServiceUtil.ProcessDbHelp` 在 catch 中调用 `BaseExceptionManager.LogException` 并 rethrow。
- 日志建议：使用结构化日志（包含 UserId、TraceId、Method、Args 摘要）。

8. 并发与线程安全要点
----------------------
- 静态可变字段禁止未经保护的写入。
- 对 `CacheUtil`、`LogUtil`、`BaseSystemInfo` 等全局对象的并发访问需确保线程安全实现。
- 在高并发场景下，避免在循环内频繁同步写日志或控制台输出，采用异步/批量策略。

9. 可扩展性与插件点
---------------------
- 数据库适配：新增 DB 类型（如 MySQL）只需实现 `IDbHelper` 并在 `DbHelperFactory` 注册。
- 缓存后端：`CacheUtil` 可支持本地与 Redis，通过配置切换实现。建议增加 `ICacheProvider` 接口用于注入。
- 日志适配：建议为 `LogUtil` 提供适配层使之实现 `ILogger`。

10. 测试策略
---------------
- 单元测试：对 `BaseManager` 的逻辑（状态变更、缓存移除、参数校验）编写单元测试，使用 Mock `IDbHelper`。
- 集成测试：对数据库访问提供独立测试 DB 或使用容器化 DB，验证 SQL 与参数化正确性。
- 性能测试：将 `DotNet.Test._452` 的关键场景抽象化并放入独立性能测试套件（限制并发、采样）。

11. 部署与配置注意事项
-----------------------
- 配置优先级：环境变量 > 配置文件（UserConfig）> 默认值。
- 机密：使用环境变量或密钥管理服务，不在代码库存储凭据。
- 多目标部署：根据消费者选择 .NET Framework 或 .NET 6+ 版本，确保依赖兼容性。

12. 迁移与现代化建议
----------------------
- 逐步在新模块采用 .NET 6+、DI 容器、ILogger 与 async/await。
- 将 `DbHelper` 的原生 SQL 调用逐步迁移到 Dapper（保持 SQL 可控）或引入 EF Core（需要更多改动）。
- 引入 GitHub Actions：自动化构建、测试与静态分析。

附录：常见重构任务清单
----------------------
- 全仓 SQL 参数化扫描并修复。
- 为业务层增加接口抽象，便于 Mock 与测试。
- 清理无用注释与死代码。
- 在关键模块引入单元测试并在 CI 中运行。

结束
---

如需，我可以：
- 根据该模块设计为 `BaseModuleManager` 或 `BaseManager` 生成接口抽象并提交 PR，或
- 为 `IDbHelper` 提供 Mock 实现并添加示例单元测试。

请选择下一步任务。
