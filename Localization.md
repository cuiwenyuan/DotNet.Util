# 多语言（输出消息本地化）

`DotNet.Util` 的输出消息支持多语言。**默认中文（zh-CN）**，内置第一语言包**英文（en）**。

**推荐用法是强类型成员**：`Msg.Common.UnknownError`、`Msg.Common.ParameterRequired("用户名")`。
字符串键（`Msg.Get("Common.UnknownError")`）仍然可用，适合配置驱动、插件化等键在编译期未知的场景。

覆盖形态：异常消息、业务状态消息、日志文本、控制台输出、`AppMessage` 消息键、枚举描述。
不含：XML 注释、实体字段 `[Description]` 显示名（数量大且属 UI 层，未纳入本次改造）。

---

## 一、快速上手（强类型，推荐）

```csharp
using DotNet.Util;

// 无占位符的词条是「属性」
Msg.Common.UnknownError                // "发生未知错误。"
Msg.Validation.InvalidEmail            // "E-mail 格式不正确，请重新输入。"
Msg.Enum.Status.Ok                     // "运行成功"
Msg.Logon.UserNotFound                 // "未找到对应的用户"
Msg.Service.FileService                // "档案服务"

// 含 {0} {1} 占位符的词条是「方法」，参数直接传入
Msg.Common.ParameterRequired("用户名")  // "请输入用户名，不允许为空。"
Msg.Log.ConfigParseFailed("Timeout", "abc")
// "UserConfigUtil 配置解析失败：Timeout=abc，已使用默认值。"

// 切换语言（全局生效，强类型取值随当前语言变化）
Msg.CurrentLanguage = "en";
Msg.Common.UnknownError                // "An unknown error occurred."
Msg.Common.ParameterRequired("UserName")
// "Please enter UserName; it cannot be empty."
Msg.Enum.Status.Ok                     // "Operation succeeded"

// 复位为默认语言并重载内置语言包
Msg.Clear();
```

> `Msg.CurrentLanguage` 未显式设置时读取 `BaseSystemInfo.CurrentLanguage`（已是 `zh-CN`）。

### 强类型成员一览（`Msg.Typed.cs`）

词条按语言包键前缀分组为嵌套静态类，**400 个词条 100% 覆盖**（属性 342 + 方法 58）：

| 分组 | 词条 | 属性 | 方法 | 强类型入口示例 |
|---|---:|---:|---:|---|
| `Business` | 6 | 6 | 0 | `Msg.Business.DataAlreadyDeleted` |
| `Common` | 74 | 67 | 7 | `Msg.Common.UnknownError` |
| `Confirm` | 30 | 19 | 11 | `Msg.Confirm.Delete` |
| `Console` | 4 | 1 | 3 | `Msg.Console.Completed("步骤")` |
| `Enum.AuditStatus` | 10 | 10 | 0 | `Msg.Enum.AuditStatus.WaitForAudit` |
| `Enum.Status` | 60 | 60 | 0 | `Msg.Enum.Status.Ok` |
| `Exception` | 11 | 8 | 3 | `Msg.Exception.InvalidIp` |
| `File` | 3 | 3 | 0 | `Msg.File.UploadSizeExceeded` |
| `Ip` | 11 | 11 | 0 | `Msg.Ip.InvalidIpFormat` |
| `Log` | 12 | 4 | 8 | `Msg.Log.ConfigParseFailed(k, v)` |
| `Logon` | 55 | 47 | 8 | `Msg.Logon.UserNotFound` |
| `Org` | 7 | 7 | 0 | `Msg.Org.Department` |
| `Qqwry` | 1 | 1 | 0 | `Msg.Qqwry.Unknown` |
| `Result` | 21 | 18 | 3 | `Msg.Result.SaveSuccess` |
| `Rmb` | 2 | 2 | 0 | `Msg.Rmb.NotNumeric` |
| `Sequence` | 5 | 5 | 0 | `Msg.Sequence.GenerateSuccess` |
| `Service` | 33 | 33 | 0 | `Msg.Service.FileService` |
| `Sign` | 5 | 5 | 0 | `Msg.Sign.VerificationCode` |
| `Sms` | 1 | 1 | 0 | `Msg.Sms.InvalidMobile` |
| `System` | 8 | 8 | 0 | `Msg.System.DbConnectionFailed` |
| `Validation` | 35 | 22 | 13 | `Msg.Validation.InvalidEmail` |
| `Workflow` | 6 | 4 | 2 | `Msg.Workflow.SendSuccess` |
| **合计** | **400** | **342** | **58** | |

判定规则很简单：**语言包文本含 `{0}` → 方法（`params object[] args`）；否则 → 属性**。
方法在参数个数不匹配时返回**原文**（不抛异常），见「回退链」。

### 何时改用字符串键

强类型层**只按当前语言取值**，不提供指定语言的重载。以下场景用字符串键：

```csharp
// 1) 需要显式指定语言（如导出多语言报表、给指定用户发信）
Msg.Get("Common.UnknownError", "en");        // "An unknown error occurred."
Msg.GetOrDefault("Common.UnknownError", "en", "兜底文本");

// 2) 键在编译期未知（配置驱动、插件、按约定拼键）
var key = "Enum." + enumType.Name + "." + memberName;
Msg.GetOrDefault(key, fallback);

// 3) 等价于强类型的动态写法
Msg.Get("Common.UnknownError");                      // 同 Msg.Common.UnknownError
Msg.Format("Common.ParameterRequired", "用户名");     // 同 Msg.Common.ParameterRequired("用户名")
```

强类型成员内部正是调用 `Get` / `Format`，**两者行为完全一致**（回退链、JSON 覆盖、大小写规则均相同）。

---

## 二、回退链

取值时按 `en-US` → `en` → `zh-CN` 逐级回退；**键不存在时返回键名本身，绝不抛异常**
（唯一例外：显式调用 `GetOrDefault` 时返回调用方给的默认值）。

```csharp
Msg.Get("Not.Exist.Key");                      // 返回 "Not.Exist.Key"
Msg.GetOrDefault("Not.Exist.Key", "兜底文本");  // 返回 "兜底文本"
```

`Msg.Format` 的占位符个数与实参不匹配时，捕获 `FormatException` 并**返回未格式化的原文**，
不会因消息格式化中断业务流程。

---

## 三、键命名规范

全部键均为 `<前缀>.<PascalCase>` 语义命名，**已无编号键**：

| 前缀 | 含义 | 数量 | 强类型入口示例 |
|---|---|---:|---|
| `Common.*` | 通用消息、数据状态、并发冲突、界面字段标签 | 74 | `Msg.Common.UnknownError` |
| `Logon.*` | 登录、用户、密码、在线限制 | 55 | `Msg.Logon.UserNotFound` |
| `Validation.*` | 输入校验失败 | 35 | `Msg.Validation.InvalidEmail` |
| `Confirm.*` | 二次确认（"您确认…吗？"） | 30 | `Msg.Confirm.Delete` |
| `Result.*` | 操作结果（成功 / 失败） | 21 | `Msg.Result.SaveSuccess` |
| `Ip.*` | IP / MAC 地址限制 | 11 | `Msg.Ip.InvalidIpFormat` |
| `System.*` | 系统、连接、服务状态 | 8 | `Msg.System.DbConnectionFailed` |
| `Org.*` | 组织机构、部门、角色 | 7 | `Msg.Org.Department` |
| `Workflow.*` | 工作流、审核流转 | 6 | `Msg.Workflow.SendSuccess` |
| `Sequence.*` | 序列 / 编号生成 | 5 | `Msg.Sequence.GenerateSuccess` |
| `Sign.*` | 签名 / 通讯配置 | 5 | `Msg.Sign.VerificationCode` |
| `File.*` | 文件上传、导出、文档 | 3 | `Msg.File.UploadSizeExceeded` |
| `Enum.<类型>.<成员>` | 枚举描述 | 70 | `Msg.Enum.Status.Ok` |
| `Exception.*` | 异常消息 | 11 | `Msg.Exception.InvalidIp` |
| `Log.*` | 日志文本 | 12 | `Msg.Log.ConfigParseFailed(k, v)` |
| `Console.*` | 控制台输出 | 4 | `Msg.Console.Completed("步骤")` |
| `Service.*` | `AppMessage.Service` 功能名 | 33 | `Msg.Service.FileService` |
| `Business.*` | 业务操作错误消息 | 6 | `Msg.Business.DataAlreadyDeleted` |
| `Rmb.*` / `Qqwry.*` / `Sms.*` | 工具类返回消息 | 4 | `Msg.Rmb.NotNumeric`、`Msg.Qqwry.Unknown`、`Msg.Sms.InvalidMobile` |

当前词条总数 **400**（中英各一套，键集合完全一致，由单测断言）。

> **2026-09-16 迁移**：原 248 个 `Msg####` 编号键（如 `Msg0001`、`Msg9999`、`Msgs965`）已全部
> 按中文语义重命名到上表前缀体系，其中 6 个与已有语义键同值的编号键被合并删除（406 → 400）。
> 完整映射见仓库根目录 [`Msg-Key-Rename-Map.md`](./Msg-Key-Rename-Map.md)。
>
> **2026-09-18 强类型层**：新增 `Msg.Typed.cs`，为全部 400 个词条生成强类型入口。
> 旧代码里的 `Msg.Get("Msg0007")`、`Msg.Format("Msg0007", ...)` 一类写法**早已失效**
> （这些键已从语言包移除，会静默返回 `"Msg0007"` 字符串），请按映射表改到强类型成员，
> 例如 `Msg0007` → `Common.ParameterRequired` → **`Msg.Common.ParameterRequired(x)`**。

### 键区分大小写，语言名不区分

消息键按 **Ordinal（区分大小写）** 比较，必须与语言包中的键**完全一致**：

```csharp
Msg.Common.UnknownError           // "发生未知错误。"   命中
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

## 四、与 `AppMessage` 的关系

`AppMessage.Msg####` 与 `AppMessage.Service.*` 的**静态字段原样保留**（二进制兼容，已编译的下游程序集无需重新编译）。
字段值仍是中文，作为默认值存在；**多语言场景请改用强类型成员**。

⚠️ **字段名 ≠ 语言包键**：2026-09-16 起语言包键已改为语义命名，`Msg.Get("Msg0001")` 这类
「用字段名直接查」的写法不再有效（会静默回退返回 `"Msg0001"` 字符串）。请按中文语义查对应键：

```csharp
AppMessage.Msg0001            // "发生未知错误。"（字段，恒中文，保持不变）
Msg.Common.UnknownError       // 随当前语言切换 —— 这才是 Msg0001 现在的键
```

`MsgTests.ZhCnPack_MatchesAppMessageFields` 会用反射校验「每个字段的中文取值都存在于中文语言包」，
若有人改了字段值却忘了同步语言包，该用例立即失败。

---

## 五、枚举描述本地化

`[EnumDescription("中文")]` 是自定义特性，**参数必须是编译期常量，无法动态取值**。
因此本地化在**读取端**完成，枚举定义文件（`Status.cs` / `AuditStatus.cs`）零改动：

```csharp
Status.Ok.ToDescription();                 // zh-CN: "运行成功"
Msg.CurrentLanguage = "en";
Status.Ok.ToDescription();                 // en: "Operation succeeded"

// 需要直接取枚举词条时（不经过 ToDescription）
Msg.Enum.Status.Ok;                        // 同上，随当前语言
Msg.Enum.AuditStatus.WaitForAudit;         // "待审" / "Pending"
```

未登记词条的第三方枚举**回退到特性原文**（不会返回 `Enum.Xxx.Member` 这种键名），
行为由 `MsgTests.EnumDescription_UnregisteredEnum_FallsBackToAttributeText` 锁定。

---

## 六、扩展：新增语言 / 覆盖文案

### 代码注册

```csharp
Msg.Register("ja", new Dictionary<string, string>
{
    { "Common.UnknownError", "不明なエラーが発生しました。" }
});
```

`Register` 为增量合并，同名键以最后注册者为准。
注册后强类型成员 `Msg.Common.UnknownError` 在 `ja` 下即返回新文案——**无需改动 `Msg.Typed.cs`**，
因为强类型成员只持有键、每次取值都走语言包。

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

## 七、⚠️ `DotNet.Web.UI.BasePage` 的同名 `Msg` 冲突

`DotNet.Web.UI.BasePage` 包内有一个**同名同命名空间**的类型 `DotNet.Util.Msg`
（WebForm 的 JS 弹窗辅助类：`Msg.Alert(...)` / `Msg.ShowConfirmAlert(...)` 等），
与多语言消息层 `DotNet.Util.Msg` **不是同一个类型**，且 partial 不跨程序集合并。

后果：

- 同时引用 `Wangcaisoft.DotNet.Util` 与 `Wangcaisoft.DotNet.Web.UI.BasePage` 时，
  裸写 `Msg` 会产生**二义性**（消费端报 `CS0104`/`CS0433`，`DotNet.Web.UI.BasePage` 自身编译产生
  `CS0436` 警告，见 `MessageBox.cs`）。
- 在该场景下请使用**命名空间别名**区分：

```csharp
using MsgLocalization = DotNet.Util.Msg;   // 多语言消息层（来自 DotNet.Util）
// 或：extern alias + 完全限定名

var text = MsgLocalization.Common.UnknownError;
```

> 规划中的根治方案是把 WebForm 弹窗类改名为 `WebMsg` 并保留 `[Obsolete]` 转发，
> 属**破坏性变更**，需随大版本发布（见 `Localization-Implementation-Plan.md` 风险表）。

---

## 八、测试约定

语言切换是**全局静态状态**。任何断言「默认中文」的测试类必须标注：

```csharp
[Collection(MsgTestCollection.Name)]   // DisableParallelization = true
public class YourTests { ... }
```

否则会与 `MsgTests` 中的语言切换并行竞争，产生偶发失败。

---

## 九、验证状态（2026-09-18）

- `Msg.Typed.cs` 强类型层 **400 个成员**，与语言包键 **一一对应、零遗漏零多余**（脚本比对 100% 覆盖）
- 中英语言包键集合**完全一致**（各 400，差集为空，由单测断言）
- `DotNet.Util` 10 档 TFM（net46/47/48、net6.0~net10.0、netstandard2.0/2.1）编译 0 错误
- `DotNet.Business` / `DotNet.Business.Web` / `DotNet.Util.Plus` 各 4 档 0 错误
- `MsgTests` 双档（net8.0 + net48）40/40 通过，含强类型断言（属性、方法、枚举分组）
- 全量非集成回归通过（`HttpUtilTests` 为已知并行 flaky，单独复跑 8/8）

## 十、性能

`Msg.Get` 单次约 **64~93 ns**（Release / net8.0 实测）；`Msg.Format` 约 410 ns。
强类型成员只是在此之上多一层**属性/方法转发**（`=> Get("...")`），开销为一次额外的非虚调用，
JIT 通常内联，实测与直接 `Msg.Get` 无显著差异。

库内调用点全部位于异常、日志与失败分支，无热路径，
相比一次数据库查询（毫秒级）占比不足万分之一，不构成性能瓶颈。

实现上已做两点优化：初始化标志与当前语言字段使用 `volatile` 保证多线程可见性；
内层消息键字典使用 `StringComparer.Ordinal` 避免无谓的忽略大小写哈希开销。
