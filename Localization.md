# 多语言（输出消息本地化）

`DotNet.Util` 的输出消息支持多语言。**默认中文（zh-CN）**，内置第一语言包**英文（en）**。

覆盖形态：异常消息、业务状态消息、日志文本、控制台输出、`AppMessage` 消息键、枚举描述。
不含：XML 注释、实体字段 `[Description]` 显示名（数量大且属 UI 层，未纳入本次改造）。

---

## 快速上手

```csharp
using DotNet.Util;

// 取值（默认当前语言，默认 zh-CN）
var text = Msg.Get("Common.UnknownError");                 // "发生未知错误。"
var tip  = Msg.Get("Common.UnknownError", "en");           // "An unknown error occurred."

// 带占位符
Msg.Format("Msg0007", Msg.Get("Msg9961"));     // "请输入原密码，不允许为空。"

// 切换语言（全局）
Msg.CurrentLanguage = "en";
Msg.Get("Common.UnknownError");                            // "An unknown error occurred."

// 恢复
Msg.Clear();                                   // 复位为 zh-CN 并重载内置语言包
```

> `Msg.CurrentLanguage` 默认读取 `BaseSystemInfo.CurrentLanguage`（已是 `zh-CN`）。

---

## 回退链

取值时按 `en-US` → `en` → `zh-CN` 逐级回退；**键不存在时返回键名本身，绝不抛异常**
（唯一例外：显式调用 `GetOrDefault` 时返回调用方给的默认值）。

```csharp
Msg.Get("Not.Exist.Key");                      // 返回 "Not.Exist.Key"
Msg.GetOrDefault("Not.Exist.Key", "兜底文本");  // 返回 "兜底文本"
```

---

## 键命名规范

全部键均为 `<前缀>.<PascalCase>` 语义命名，**已无编号键**：

| 前缀 | 含义 | 数量 | 示例 |
|---|---|---|---|
| `Common.*` | 通用消息、数据状态、并发冲突、界面字段标签 | 74 | `Common.UnknownError` |
| `Logon.*` | 登录、用户、密码、在线限制 | 55 | `Logon.UserNotFound` |
| `Validation.*` | 输入校验失败 | 35 | `Validation.InvalidEmail` |
| `Confirm.*` | 二次确认（"您确认…吗？"） | 30 | `Confirm.Delete` |
| `Result.*` | 操作结果（成功 / 失败） | 21 | `Result.SaveSuccess` |
| `Ip.*` | IP / MAC 地址限制 | 11 | `Ip.InvalidIpFormat` |
| `System.*` | 系统、连接、服务状态 | 8 | `System.DbConnectionFailed` |
| `Org.*` | 组织机构、部门、角色 | 7 | `Org.Department` |
| `Workflow.*` | 工作流、审核流转 | 6 | `Workflow.SendSuccess` |
| `Sequence.*` | 序列 / 编号生成 | 5 | `Sequence.GenerateSuccess` |
| `Sign.*` | 签名 / 通讯配置 | 5 | `Sign.VerificationCode` |
| `File.*` | 文件上传、导出、文档 | 3 | `File.UploadSizeExceeded` |
| `Enum.<类型>.<成员>` | 枚举描述 | 70 | `Enum.Status.Ok` |
| `Exception.*` | 异常消息 | 11 | `Exception.InvalidIp` |
| `Log.*` | 日志文本 | 12 | `Log.ConfigParseFailed` |
| `Console.*` | 控制台输出 | 4 | `Console.Completed` |
| `Service.*` | `AppMessage.Service` 功能名 | 33 | `Service.FileService` |
| `Business.*` | 业务操作错误消息 | 6 | `Business.DataAlreadyDeleted` |
| `Rmb.*` / `Qqwry.*` / `Sms.*` | 工具类返回消息 | 4 | `Rmb.NotNumeric` |

当前词条总数 **400**（中英各一套，键序一致）。

> **2026-09-16 迁移**：原 248 个 `Msg####` 编号键（如 `Msg0001`、`Msg9999`、`Msgs965`）已全部
> 按中文语义重命名到上表前缀体系，其中 6 个与已有语义键同值的编号键被合并删除（406 → 400）。
> 完整映射见仓库根目录 [`Msg-Key-Rename-Map.md`](./Msg-Key-Rename-Map.md)。

### 键区分大小写，语言名不区分

消息键按 **Ordinal（区分大小写）** 比较，必须与语言包中的键**完全一致**：

```csharp
Msg.Get("Common.UnknownError");   // "发生未知错误。"   命中
Msg.Get("common.unknownerror");   // "common.unknownerror"  未命中，按回退链返回键名本身
```

与之相对，**语言名（culture）不区分大小写**，以下写法等价：

```csharp
Msg.Get("Common.UnknownError", "zh-CN");
Msg.Get("Common.UnknownError", "zh-cn");   // 同上
Msg.CurrentLanguage = "EN";     // 等同 "en"
```

> 若从旧版本迁移时有依赖大小写不敏感的调用，需要统一改成规范键名，
> 否则会静默回退到键名本身（不抛异常，容易漏查）。

---

## 与 `AppMessage` 的关系

`AppMessage.Msg####` 与 `AppMessage.Service.*` 的**静态字段原样保留**（二进制兼容，已编译的下游程序集无需重新编译）。
字段值仍是中文，作为默认值存在；**多语言场景请改用 `Msg.Get("<语义键>")`**。

⚠️ **字段名 ≠ 语言包键**：2026-09-16 起语言包键已改为语义命名，`Msg.Get("Msg0001")` 这类
「用字段名直接查」的写法不再有效（会静默回退返回 `"Msg0001"` 字符串）。请按中文语义查对应键：

```csharp
AppMessage.Msg0001            // "发生未知错误。"（字段，恒中文，保持不变）
Msg.Get("Common.UnknownError")// 随当前语言切换 —— 这才是 Msg0001 现在的键
```

`MsgTests.ZhCnPack_MatchesAppMessageFields` 会用反射校验「每个字段的中文取值都存在于中文语言包」，
若有人改了字段值却忘了同步语言包，该用例立即失败。

---

## 枚举描述本地化

`[EnumDescription("中文")]` 是自定义特性，**参数必须是编译期常量，无法动态取值**。
因此本地化在**读取端**完成，枚举定义文件（`Status.cs` / `AuditStatus.cs`）零改动：

```csharp
Status.Ok.ToDescription();                 // zh-CN: "运行成功"
Msg.CurrentLanguage = "en";
Status.Ok.ToDescription();                 // en: "Operation succeeded"
```

未登记词条的第三方枚举**回退到特性原文**（不会返回 `Enum.Xxx.Member` 这种键名），
行为由 `MsgTests.EnumDescription_UnregisteredEnum_FallsBackToAttributeText` 锁定。

---

## 扩展：新增语言 / 覆盖文案

### 代码注册

```csharp
Msg.Register("ja", new Dictionary<string, string>
{
    { "Common.UnknownError", "不明なエラーが発生しました。" }
});
```

`Register` 为增量合并，同名键以最后注册者为准。

### 外部 JSON 覆盖（无需重新编译）

```csharp
Msg.LoadJsonOverride("en", @"C:\cfg\msg.en.json");
```

```json
{
  "Common.UnknownError": "Something went wrong.",
  "Logon.Success": "Welcome back."
}
```

文件不存在或 JSON 格式错误时静默忽略。

JSON 中的键同样**区分大小写**，必须与内置语言包中的键完全一致，否则不会覆盖到目标词条
（会静默新增一个永远不会被查到的键）：

```json
{
  "Common.UnknownError": "Something went wrong.",
  "logon.success": "…"
}
```

上例中 `logon.success` 不会覆盖 `Logon.Success`。

---

## 测试约定

语言切换是**全局静态状态**。任何断言「默认中文」的测试类必须标注：

```csharp
[Collection(MsgTestCollection.Name)]   // DisableParallelization = true
public class YourTests { ... }
```

否则会与 `MsgTests` 中的语言切换并行竞争，产生偶发失败。

---

## 验证状态（2026-09-16）

- `DotNet.Util` 10 档 TFM（net46/47/48、net6.0~net10.0、netstandard2.0/2.1）编译 0 错误
- `DotNet.Business` / `DotNet.Business.Web` / `DotNet.Util.Plus` 各 4 档 0 错误
- `MsgTests` 双档（net8.0 + net48）40/40 通过
- 键命名语义化改造完成：400 词条，编号键清零，`AppMessage` 字段名与字段数（248）保持不变
- 全量非集成回归通过（`HttpUtilTests` 为已知并行 flaky，单独复跑 8/8）

## 性能

`Msg.Get` 单次约 **64~93 ns**（Release / net8.0 实测），相对改造前的编译期常量增量约 85 ns；
`Msg.Format` 约 410 ns。库内 319 处调用点全部位于异常、日志与失败分支，无热路径，
相比一次数据库查询（毫秒级）占比不足万分之一，不构成性能瓶颈。

实现上已做两点优化：初始化标志与当前语言字段使用 `volatile` 保证多线程可见性；
内层消息键字典使用 `StringComparer.Ordinal` 避免无谓的忽略大小写哈希开销。
