# DotNet.Util 对标 NewLife.Core · 维度欠缺分析报告

> 对标对象：NewLife.Core（规范仓库 `NewLifeX/X`，核心库，MIT，支持 .NET 2.0–10.0）
> 被对标：本仓库 `DotNet.Util`（17 个项目，多 TFM：`net46/47/48 + net6–10 + netstandard2.0/2.1`）
> 方法：先盘点本库实际模块/工具类，再对照 NewLife.Core 官方 README「主要功能」模块表与源码维度；对本库"疑似欠缺"项用全仓 grep 反向核验（证据见各维度）。
> 生成时间：2026-08-25

---

## 一、维度总览表

| # | 维度 | NewLife.Core 能力 | 本库状态 | 判定 |
|---|------|-------------------|----------|------|
| 1 | 基础扩展（Convert/String/Path/IO/Runtime/DisposeBase/Reflect） | Utility、StringHelper、PathHelper、IOHelper、Runtime、DisposeBase、Reflect（高性能反射） | BaseUtil.Convert、StringUtil、PathUtil、FileUtil、MachineInfo、NewLife.DisposeBase、ReflectionUtil | ✅ 持平 |
| 2 | 类型安全/校验/验证 | ValidateHelper 系列 | ValidateUtil、Validation、Validator | ✅ 持平 |
| 3 | 安全/加密 | SecurityHelper：RSA/AES/DES/MD5/RC4/DSA/CRC | SecretUtil(RSA/DES/AES/MD5) + NewLife.SecurityUtil(RC4) | 🟡 缺 DSA/CRC |
| 4 | 日志 | ILog（多输出、异步写入） | LogUtil、FileLogUtil | 🟡 缺异步多 Sink |
| 5 | 缓存 | 内存 / Redis | CacheUtil、MemoryUtil、RedisUtil | ✅ 持平 |
| 6 | 配置 | XML / Json / Http | XmlConfigUtil、BaseConfiguration、UserConfigUtil（XML） | 🟡 缺 Json/Http 配置源 |
| 7 | 序列化 | Binary / Json / Xml | JsonUtil、XmlUtil、XmlSerializationUtil | 🟡 缺 Binary |
| 8 | 模型/实体 | ModelBase、Entity | BaseEntity、IBaseEntity、FieldDescription | ✅ 持平 |
| 9 | 数据库/ORM | XCode（反向工程、8 库、分表分库、LINQ） | DbHelper + DbUtil + SQLBuilder（裸 ADO + 微 ORM 映射） | 🔴 缺完整 ORM |
| 10 | **定时调度** | TimerX（高精度、Cron、异步周期）+ Cron 表达式 | 无（grep 零命中） | 🔴 完全缺失 |
| 11 | **依赖注入 IOC** | ObjectContainer（IServiceProvider） | 无（grep 零命中） | 🔴 完全缺失 |
| 12 | **网络通信服务端** | 网络库：Tcp/Udp/Http/WebSocket/IPv6，十万级并发；ApiClient | 仅 HttpUtil（客户端）、RequestUtil、WebUpload；无服务端（grep 仅测试含 TcpListener） | 🔴 完全缺失 |
| 13 | **RPC/远程调用** | Tcp/Udp/Http/Json，身份验证+加密 | 无（grep 零命中） | 🔴 完全缺失 |
| 14 | 脚本引擎/动态编译 | ScriptEngine、动态对象 | 无（grep 零命中） | 🔴 完全缺失 |
| 15 | 进程管理 | ProcessHelper | 无（grep 零命中） | 🟠 缺失 |
| 16 | 压缩 | Zip/GZip 等 | 仅 ZipUtil（Plus，Zip） | 🟡 缺 GZip/7z |
| 17 | **APM/链路追踪** | ITracer（APM、星尘平台集成） | 仅 TraceabilityUtil（本地追踪，无平台） | 🟠 弱 |
| 18 | **消息队列** | MQ（发布订阅） | 仅 BaseMessageQueueEntity（实体表，非引擎） | 🔴 完全缺失 |
| 19 | Windows 服务/守护 | NewLife.Agent（服务/守护/监控） | 无 | 🟠 缺失 |
| 20 | 本地化/多语言 | 资源框架 | 仅 CultureInfo 解析；UserConfigUtil 有 MultiLanguage 开关但无实现框架 | 🟠 弱 |
| 21 | 模版引擎 | XTemplate（T4 风格，生成实体/页面） | 仅 WordUtil.TemplateExport（Word） | 🟡 弱 |
| 22 | 对象池 | Pool | PoolUtil（NewLife 封装） | ✅ 持平 |

> 图例：✅ 持平/已覆盖　🟡 部分覆盖　🟠 明显偏弱　🔴 完全缺失（本库最大短板）

---

## 二、重点欠缺维度详解（🔴/🟠）

### ① 定时调度 / Cron（🔴 完全缺失）
- **NewLife**：`TimerX` 高精度定时器，支持 **Cron 表达式**（秒级）、异步周期执行；是整个框架"心跳"基础。
- **本库**：无任何调度抽象。定时任务需业务自己用 `System.Threading.Timer`/`Task` 手写，无统一 Cron、无周期任务注册表。
- **影响**：后台作业、心跳、缓存过期扫描等场景无标准件，各项目重复造轮子且易错。
- **建议**：引入 `Quartz.Net` 或 `FluentScheduler`；或封装一个轻量 `CronTimer` 工具类（复用现有 `BaseSystemInfo` 配置）。

### ② 依赖注入容器 IOC（🔴 完全缺失）
- **NewLife**：`ObjectContainer` 实现 `IServiceProvider`，`AddSingleton/AddTransient/AddScoped` 全家桶，贯穿全栈。
- **本库**：grep 零命中。组件全靠静态类 + `BaseSystemInfo` 全局单例，无 DI。
- **影响**：在 ASP.NET Core / 现代宿主里集成困难；测试时需靠 `BaseSystemInfo` 反射赋值（见集成测试里的 `StubDbHelper`），可测性差。
- **建议**：至少补 `Microsoft.Extensions.DependencyInjection` 的 `ServiceCollection` 扩展入口（本库已引用 `Microsoft.Extensions.*`，成本低），提供 `AddDotNetUtil()`。

### ③ 网络通信服务端 / 高性能网络（🔴 完全缺失）
- **NewLife**：独立网络库，Tcp/Udp/Http/WebSocket/IPv6，**十万级并发**，含 `ApiClient`、Pipeline、会话管理。
- **本库**：只有 **HTTP 客户端**能力（`HttpUtil`、`HttpRequestUtil`、`SmsUtil`、`MailUtil`）。grep 确认 `TcpListener` 仅存在于测试 mock，无生产级网络服务端。
- **影响**：本库定位是"工具库"而非"应用框架"，此维度缺口在预期内；但若要做中间层/代理/长连接，需另引 NewLife.Net 或 DotNetty。
- **建议**：**不建议自研**；按需引用 `NewLife.Net` 或 `DotNetty`。明确本库边界即可。

### ④ RPC / 远程过程调用（🔴 完全缺失）
- **NewLife**：Tcp/Udp/Http/Json RPC，自带身份验证与数据加密。
- **本库**：无。
- **建议**：同 ③，按需引用，不自研。

### ⑤ 完整 ORM（🔴 架构级差距）
- **NewLife**：XCode——反向工程建表、8 种数据库、无限分表分库、LINQ 查询、实体缓存。
- **本库**：`DbHelper` + `DbUtil.*`（按 `BaseEntity` 属性映射做 CRUD/分页）+ `SQLBuilder`。属于**裸 ADO + 微 ORM 映射**，无 LINQ、无反向工程、无跨库分片。
- **影响**：复杂查询仍需手写 SQL；多库分片需业务自管。
- **建议**：保留现有 `DbUtil`（轻量、可控，适合 ERP 场景），把"要不要上 XCode"作为独立决策；可先补 `Dapper` 作为中间层以提效而不引入重 ORM。

### ⑥ 脚本引擎 / 动态编译（🔴 完全缺失）
- **NewLife**：`ScriptEngine`（C# 脚本求值）、动态对象。
- **本库**：无（仅有 `ExpressionEvaluator` 做简单表达式求值，非完整脚本）。
- **建议**：一般业务不需要；若要做规则引擎，再评估 Roslyn `CSharpScript`。

### ⑦ 消息队列引擎（🔴 完全缺失）
- **NewLife**：`MQ`（发布/订阅）。
- **本库**：仅有 `BaseMessageQueueEntity`（**一张队列表的实体**，不是 MQ 引擎）。
- **建议**：按需引用 `NewLife.MQ` / `RabbitMQ` / `CAP`；本库不该自研 MQ。

### ⑧ 进程管理 / Windows 服务守护（🟠 缺失）
- **NewLife**：`ProcessHelper` + `NewLife.Agent`（Windows 服务安装/守护/监控）。
- **本库**：无。
- **影响**：把本库组件部署为服务时需自带宿主。
- **建议**：长驻服务可用 `NewLife.Agent` 或 Topshelf；非核心，按需。

### ⑨ APM / 链路追踪平台（🟠 弱）
- **NewLife**：`ITracer` 对接星尘 APM，全链路追踪。
- **本库**：`TraceabilityUtil` 仅做本地调用链记录，无平台对接、无指标上报。
- **建议**：已有 `TraceabilityUtil` 基础，可加 OpenTelemetry 导出器低成本补齐。

### ⑩ 本地化 / 多语言资源框架（🟠 弱）
- **NewLife**：完整资源/本地化框架。
- **本库**：仅 `CultureInfo` 解析；`UserConfigUtil` 有 `MultiLanguage` 开关但**无实现框架**（grep 见 `ResourceManagerWrapper` 被注释掉）。
- **建议**：若真要做多语言，引入 `IStringLocalizer` + resx；否则维持现状。

---

## 三、本库相对 NewLife.Core 的反向优势（不必补）

本库 DNA 是 **ERP/权限域工具库**，以下维度 NewLife.Core（纯基础设施）反而没有或较弱，属本库特色，对标时**不算欠缺**：

| 维度 | 本库独有/更强 |
|------|--------------|
| 业务权限域 | `DotNet.Business`：组织/角色/权限/菜单/消息（`BaseUserInfo`、`PermissionOrganizationScope` 等） |
| 身份/财务工具 | `ValidateUtil`（身份证/银行卡校验）、`RmbUtil`（人民币大写）、`VerifyCodeImage`/`CaptchaUtil`（验证码） |
| Office 生态 | `ExcelUtil`(Npoi 导入导出)、`WordUtil`(模板)、`CsvUtil` |
| 第三方集成 | `BaiduOcrUtil`、`WeChatMiniProgramUtil`、`QqwryUtil`（IP 归属）、`SmsUtil.Aliyun`、`MailUtil`、`DomainUtil` |
| 图像 | `ImageUtil`/`ThumbnailUtil`/`WatermarkUtil`/`DrawingUtil` |
| 数据库方言 | `Db.MySql/Oracle/PostgreSql/SQLite/OleDb` 多提供程序 |

---

## 四、结论与建议路线

**定位差异（先想清）**：NewLife.Core 是"全栈基础设施框架"，本库是"ERP 工具 + 权限域库"。二者目标不同，缺口多在**基础设施维度**，且多数（网络/RPC/MQ/服务）**本就不该自研**——按需引用 NewLife 生态即可。

**值得在本库内补的"短板"（投入产出比高）**：
1. 🔴 **IOC 入口**：补 `AddDotNetUtil()`（已引 `Microsoft.Extensions.*`，成本极低，直接提升现代宿主集成力与可测性）。
2. 🔴 **定时调度**：引 `Quartz.Net` 或封装轻量 `CronTimer`（后台作业的标准件）。
3. 🟡 **二进制序列化**：若需高性能传输，补 `MemoryPack`/`MessagePack`（Json 已够用则可跳过）。
4. 🟠 **APM 导出**：在 `TraceabilityUtil` 上加 OpenTelemetry，低成本接平台。
5. 🟡 **Json 配置源**：`BaseConfiguration` 支持 Json/Http 配置（当前仅 XML）。

**明确不自研、按需引用**：网络服务端、RPC、MQ、Windows 服务、脚本引擎、完整 ORM——引用 NewLife 生态或社区库。

**不缺（已覆盖，无需动作）**：基础扩展、安全（RSA/DES/AES/MD5/RC4）、缓存、配置(XML)、模型/实体、对象池、校验验证。

> 注：本报告所有"缺失"结论均经全仓 grep 反向核验（见各维度证据），非凭印象。NewLife.Core 维度以官方 README 与 `NewLifeX/X` 仓库为准。
