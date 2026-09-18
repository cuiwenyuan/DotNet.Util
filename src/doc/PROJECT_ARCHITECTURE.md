项目技术架构文档

版本：1.0
作者：自动化生成（GitHub Copilot）
日期：2026-05-10

概述
------
本仓库为企业级 .NET 工程集合，包含工具库（DotNet.Util）、业务层（DotNet.Business）、数据实体（DotNet.Model）以及测试/示例程序（DotNet.Test._452）等模块。项目支持多目标框架（.NET Framework 4.x 系列、.NET Standard 2.x、.NET 6/7/8/9），便于在不同运行时环境中复用核心库。

目标受众
--------
- 开发人员：理解整体模块划分、组件依赖与开发约束。
- 运维/部署工程师：了解运行时依赖与部署建议。
- 架构/安全工程师：评估扩展性、安全性与改进优先级。

高层架构（Layered Architecture）
---------------------------------
1. Presentation / Host
   - 位置：可能存在于调用方或上层服务（仓库中包含 `DotNet.Test._452` 作为示例控制台应用）。
   - 责任：接受请求、展示结果或作为批处理驱动程序。

2. Service / API 边界
   - 代表：`ServiceUtil` 中的 `Process*Db` 系列方法。
   - 责任：授权校验、统一 DB 访问入口、事务/日志/耗时统计的基础设施封装。

3. Business（业务层）
   - 命名空间：`DotNet.Business`
   - 代表类：`BaseManager`、`BaseExceptionManager`、`BaseModuleManager` 等。
   - 责任：实现业务逻辑、调度 DAL、缓存管理、权限与审计逻辑。

4. Data Access（数据访问层）
   - 代表：`DbHelper` / `DbHelperFactory`、`DbUtil`。
   - 责任：数据库连接、命令执行、批量删除、事务与跨数据库类型适配（抽象 CurrentDbType）。

5. Domain / Model
   - 命名空间：`DotNet.Model`（实体常量如 `BaseExceptionEntity` 存放字段/表名）
   - 责任：数据结构、字段常量、序列化与映射逻辑。

6. Common Utilities
   - 命名空间：`DotNet.Util`
   - 代表：`LogUtil`（日志）、`CacheUtil`（缓存）、`ValidateUtil`、`JsonUtil`、`DisposeBase` 等。
   - 责任：通用工具函数、缓存策略、日志与异常辅助。

组件与关键模块说明
-------------------
- `DbHelperFactory` / `IDbHelper`
  - 多数据库抽象，按 `BaseSystemInfo` 中配置创建对应 `IDbHelper` 实例（可支持 SQL Server、Oracle 等）。
  - 所有 SQL 执行通过 `IDbHelper` 路径，便于切换 DB 实现。

- `BaseManager`（基类）
  - 提供通用 CRUD、批量保存、审计、发料/审核等通用业务方法。
  - 子类（如 `BaseModuleManager`）继承并实现模块特定逻辑。

- `BaseExceptionManager`
  - 集中异常记录入口，支持写入数据库与 Windows 事件日志（受 `BaseSystemInfo` 控制）。

- `ServiceUtil`
  - 将服务请求封装为 `ProcessDb` 等模板方法，附带耗时统计、日志记录与异常统一处理。

- `CacheUtil`
  - 抽象缓存读取/写入，许多方法使用了 24 小时（86400000 毫秒）默认缓存策略。

- `DisposeBase` / `DisposeHelper`
  - 提供线程安全的 Dispose 基类及通用销毁辅助，便于统一资源释放。

部署与运行时要求
-----------------
- 目标框架：库组件多目标编译；在部署时需根据消费端选择合适的运行时（.NET Framework 或 .NET 6+）。
- 配置来源：`BaseSystemInfo` 和 `UserConfigUtil` 等用于读取连接字符串与运行时配置（注意机密信息不可硬编码）。
- 数据库：支持多种 DB，通过 `DbHelperFactory` 根据配置创建适配器；需要在部署环境中准备连接字符串与相应的 DB 用户权限。

数据流示意（典型请求）
---------------------
请求（UI/API/脚本）
  -> ServiceUtil.ProcessXxx（鉴权、计时）
    -> ProcessDbHelp（创建 IDbHelper）
      -> Business Manager（业务逻辑）
        -> 调用 DbHelper 执行 SQL 或调用 CacheUtil
          -> 返回数据并记录日志/异常

缓存与日志
-----------
- 缓存：`CacheUtil.Cache<T>(key, func, ...)` 在多个管理器中用于缓存表或实体列表，默认缓存时长例：24 小时。应避免在并发高频写场景中使用长时缓存。
- 日志：通过 `LogUtil` 写入日志；异常通过 `BaseExceptionManager.LogException` 集中记录。库层避免直接写 Console，生产环境使用集中日志方案（建议将 `LogUtil` 适配到 ILogger/ELK/Seq）。

安全及注意事项（当前痛点）
------------------------
- SQL 注入：存在使用字符串拼接构造 SQL 的位置（已有若干处已改造为参数化），需全面替换为参数化查询或使用轻量 ORM（如 Dapper）。
- 全局可变静态状态：`BaseSystemInfo`、部分工具类的静态字段需审计，避免在多线程或多租户场景导致竞态。
- 配置与密钥：禁止将机密写入代码，建议使用环境变量或密钥管理服务（Azure Key Vault、AWS Secrets Manager 等）。

扩展性与可伸缩性
-----------------
- 多目标库设计便于在不同运行时复用代码；在现代化改造时可优先在 .NET 6/7 项目中引入异步 API 与依赖注入（DI 体系）。
- 若需纵向扩展 DB 性能，可在 `DbHelper` 层实现连接池与批量操作优化；水平扩展则需关注会话/缓存一致性。

测试与质量保障
----------------
- 当前仓库测试覆盖较少。推荐引入：
  - 单元测试：覆盖业务逻辑边界、SQL 参数化拼装、异常处理。
  - 集成测试：针对数据库操作提供可选的测试 DB 环境。
  - 静态分析：`dotnet format`、Roslyn 分析器、CodeQL 安全审计。

运维与监控建议
----------------
- 日志集中化：将 `LogUtil` 输出接入集中式日志平台，配置分级（DEBUG/INFO/WARN/ERROR）。
- 异常告警：对 `BaseExceptionManager` 记录的严重异常触发告警（邮件/Webhook/PagerDuty）。
- 性能监控：对关键路径（DB 查询、批量任务）埋点耗时指标并监控。

迁移与现代化建议（可选）
---------------------
1. 在新服务/模块中使用 .NET 6+，并启用 DI（Microsoft.Extensions.DependencyInjection）与 `ILogger<T>`。
2. 逐步替换 `DbHelper` 字符串拼接 SQL 到参数化 Dapper 查询或引入 EF Core（仅用于新模块）。
3. 为核心库启用 nullable reference types 并修复相关警告。
4. 为项目添加 GitHub Actions CI：编译、静态分析、测试与安全扫描。

总结与下一步
------------
本项目为可复用的企业级工具与业务库，设计上使用分层架构与丰富的工具抽象。当前的主要改进点集中在数据访问安全（参数化 SQL）、静态可变状态审计、测试覆盖与 CI 流程。建议先从参数化 SQL 与 CI 建设入手，随后按优先级推进可空处理、异步化与测试覆盖。

如需我：
- 生成 CI 工作流文件并提交，或
- 将指定的一组 SQL 方法批量改为参数化，或
- 为某个管理器添加单元测试，请指定下一步任务.
