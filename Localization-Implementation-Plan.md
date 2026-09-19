# DotNet.Util 输出消息多语言改造实施计划

> 目标：全库运行时输出消息支持多语言，默认 `zh-CN`，第一个语言包 `en`。
> 编制日期：2026-09-15 · 状态：**待用户确认后执行**

---

## 一、范围界定（已与用户确认）

### 1.1 纳入改造

| # | 类别 | 数量 | 主要位置 |
|---|---|---|---|
| 1 | `AppMessage` Msg 静态字段 | **240 个**（40 个带 `{0}`） | `DotNet.Util/Message/AppMessage.Message.cs` |
| 2 | `AppMessage.Service` 服务/功能名常量 | 33 个 | `DotNet.Util/Message/AppMessage.Service.cs` |
| 3 | 枚举描述 `[EnumDescription]` | **71 条**（共 73，2 条非中文） | `Status.cs`、`AuditStatus.cs` |
| 4 | 业务状态消息赋值 | **52 处** | `WebUtil.LogOn*.cs`(28)、Business 各 Manager(20)、`BaseResult.cs`(1) 等 |
| 5 | 日志 + 控制台输出 | **21 处** | `BaseSystemInfo.UserInfo.cs`(4)、`UserConfigUtil.cs`(3)、`Utils.cs`(2)、`DisposeBase.cs`(2) 等 |
| 6 | `throw new …Exception("中文")` | **15 处** | `ExpressionEvaluator.cs`(7)、`Utils.cs`(3)、`QqwryUtil.cs`(2) 等 |
| 7 | `return "中文"` | 3 处 | 待逐点确认 |

**合计约 435 个消息条目。**

### 1.2 明确排除

- ❌ `[Description("中文")]` **实体字段显示名**：467 处（`DotNet.Model` 453 + `BaseEntity.cs` 14）。
- ❌ XML 文档注释（`///`）中的中文。
- ❌ SQL、变量名、内部标识符、日志中的技术性标签。

### 1.3 需要你二次确认的边界

1. **测试代码中的中文断言**：现有测试（如 `BaseResultTests`）断言中文文案。改造后若语言变化会导致测试失败。
   → 建议：测试基类固定 `zh-CN`，并新增英文断言测试。**是否同意？**
2. **240 个 Msg 键是否全量翻译**：目前只有 **42 个被真实引用**（43 处调用），其余 198 个是"死资产"。
   → 建议：**全量翻译**（Msg 键是对外 API，半套语言包会造成不一致），但可分批提交。**是否同意？**

---

## 二、现状与硬约束

### 2.1 已有多语言骨架（但为空壳）

| 已有 | 位置 | 状态 |
|---|---|---|
| `MultiLanguage = true` | `BaseSystemInfo.Client.cs:121` | ✅ 可用 |
| `CurrentLanguage = "zh-CN"` | `BaseSystemInfo.Client.cs:126` | ✅ 默认即中文 |
| `AppMessage.GetLanguageResource()` | `AppMessage.cs:29` | ⚠️ 空壳（资源读取被注释） |
| `AppMessage.GetMessage(id)` | `AppMessage.cs:86` | ⚠️ 空壳（`return string.Empty`） |
| `ResourceManagerWrapper` | — | ❌ **类根本不存在**（3 处调用全注释） |

**结论：填实现有骨架，不引入新体系。**

### 2.2 硬约束（决定技术选型）

- `DotNet.Util` 目标框架 **10 档**：`net46;net47;net48;net6.0;net7.0;net8.0;net9.0;net10.0;netstandard2.0;netstandard2.1`
  → **排除一切仅 modern TFM 可用的 API**（如 `IStringLocalizer`、源生成器多语言）。
- 无 `.resx`、无 `ResourceManager`。`Newtonsoft.Json 13.0.4` 已引用。
- **`[EnumDescription("中文")]` 是自定义特性，参数必须是编译期常量** → 无法动态取值，
  只能在**读取端**翻译（见 3.4）。

---

## 三、技术设计

### 3.1 消息层核心 API（新建 `DotNet.Util/Message/Msg.cs`）

> ✅ **已实现**。下表为**落地后的真实签名**，与当初的设计稿有 3 处偏差，已在「偏差说明」中标注。

```csharp
public static partial class Msg
{
    // 当前语言，默认读 BaseSystemInfo.CurrentLanguage（"zh-CN"）
    public static string CurrentLanguage { get; set; }

    public static string Get(string key);                       // 按当前语言取值
    public static string Get(string key, string culture);       // 指定语言
    public static string GetOrDefault(string key, string defaultValue);
    public static string GetOrDefault(string key, string culture, string defaultValue);
    public static string GetEnumDescription(Type enumType, string memberName, string defaultValue);
    public static string Format(string key, params object[] args);   // 带 {0} 占位符格式化

    public static void Register(string culture, IDictionary<string, string> messages); // 注册语言包
    public static void LoadJsonOverride(string culture, string path); // 外部 JSON 覆盖（可选）
    public static IReadOnlyCollection<string> GetKeys(string culture);
    public static void Clear();                                 // 复位（测试隔离用）
}
```

**回退链**：精确匹配（`en-US`）→ 中性语言（`en`）→ 默认 `zh-CN` → 键不存在返回 `key` 本身（**绝不抛异常**）。

**与设计稿的偏差（2026-09-18 核对）**

| # | 设计稿 | 实际落地 | 原因 |
|---|---|---|---|
| 1 | `Get(string key, params object[] args)` | 拆为独立 `Format(string key, params object[] args)` | `Get(key)` 与 `Get(key, args)` 在 `args` 为空时产生重载二义，拆分后语义清晰 |
| 2 | `LoadJsonOverride(string path)` | `LoadJsonOverride(string culture, string path)` | 必须指定要覆盖哪个语言包，否则无处写入 |
| 3 | `Msg` 为单一 `static class` | `static partial class`，另增 `Msg.Typed.cs` 强类型层 | 见 3.7 |

### 3.2 语言包载体：代码内嵌字典（已与用户确认）

```
src/DotNet.Util/Resources/
├── MsgPack.cs                 // static partial class，合并各分区
├── MsgPack.zh-CN.cs           // 中文包（默认）
├── MsgPack.en.cs              // 英文包（第一个语言包）
└── （按模块继续分区，partial 合并）
```

- 用 `static partial class` + 静态只读字典，**10 档 TFM 全兼容**，无卫星程序集分发问题。
- 可选外部 JSON 覆盖：`Msg.LoadJsonOverride("messages.en.json")`，用已引用的 Newtonsoft.Json 解析。

### 3.3 键命名规范

> ⚠️ **2026-09-16 起已变更**：`Msg0001` 一类**编号键已全部从语言包移除**，不存在「沿用原键」这回事。
> 现行体系：全部键为 `<前缀>.<PascalCase>`，共 400 条，12 个业务前缀 + `Enum.*` + 若干工具类前缀。
> 编号键 → 语义键的完整映射见 [`Msg-Key-Rename-Map.md`](./Msg-Key-Rename-Map.md)。

| 来源 | 键格式 | 示例 | 强类型入口 |
|---|---|---|---|
| ~~现有 AppMessage~~ | ~~沿用原键~~ **已废弃** | ~~`Msg0001`~~ → `Common.UnknownError` | `Msg.Common.UnknownError` |
| 新增（工具类） | `模块.类.场景` | `Util.ExpressionEvaluator.IllegalChars` → `Exception.*` / `Validation.*` | `Msg.Exception.*` |
| 枚举描述 | `Enum.<枚举类型>.<成员>` | `Enum.Status.DbError`、`Enum.AuditStatus.Pause` | `Msg.Enum.Status.DbError` |

### 3.7 强类型层（2026-09-18 新增，P6）

语言包键是字符串，编译期无法校验、IDE 无补全、拼错后**静默回退返回键名**（不抛异常，难排查）。
为此新增 `src/DotNet.Util/Message/Msg.Typed.cs`，把 `Msg` 改为 `static partial class`，
按键前缀生成嵌套静态类，**每个词条一个成员**：

```csharp
// 语言包文本不含 {n} → 属性
public static string UnknownError => Get("Common.UnknownError");

// 语言包文本含 {n} → 方法
public static string ParameterRequired(params object[] args)
    => Format("Common.ParameterRequired", args);
```

约定：

1. **400 条词条 100% 覆盖**（342 属性 + 58 方法），由脚本比对语言包键与强类型成员，双向零差集；
2. 强类型**只按当前语言取值**，不提供 `culture` 重载——需指定语言时仍用 `Msg.Get(key, culture)`；
3. 强类型成员**只持有键、不缓存文本**，每次取值都走语言包，因此 `Register` / `LoadJsonOverride`
   的增量覆盖对强类型**自动生效**，无需改动 `Msg.Typed.cs`；
4. 库内新写调用点一律用强类型；字符串键保留给「键在编译期未知」的场景。

### 3.4 枚举描述本地化（关键技术点）

`[EnumDescription]` 值无法动态化，因此**改读取端**：

```
EnumUtil.ToDescription(enum)
   ├─ 读 [EnumDescription] 得到中文（保留，作为默认/回退值）
   ├─ 查语言包键 Enum.<类型>.<成员>
   └─ 命中则返回翻译，未命中回退中文
```

- **零改动** `Status.cs` / `AuditStatus.cs` 定义文件。
- `AppMessage.GetEnumMessage()` 已走 `EnumUtil.ToDescription`，自动受益。

### 3.5 `AppMessage` 248 字段处置（已确认：**方案 B — 静态字段不动**）

```csharp
// 保持原样，一字不改（保证二进制兼容）
public static string Msg0001 = "发生未知错误。";

// 需要多语言的调用点改用消息层（已完成 48 处）
result = Msg.Get("Msg0800");
result = Msg.Format("Msg0007", Msg.Get("Msg9961"));
```

- 字段保留中文固定值 → **二进制与源码双兼容**，现有 `AppMessageTests` 断言不受影响。
- 仅在 `AppMessage.Message.cs` 加了类级 `<remarks>` 说明，防止"两套并存"造成误用。
- 实测：**248 个字段**（240 个数字编号 + 8 个历史遗留命名 `Msgc023`/`Msgs857`/`MsgReLogon` 等，
  均无引用）。中文包由脚本从字段同步生成，由 `MsgTests.ZhCnPack_MatchesAppMessageFields` 校验一致性。

### 3.6 占位符处理

40 个 Msg 键带 `{0}`。英文语序可能与中文不同，语言包用 **`string.Format` 命名无关的位置参数 `{0}/{1}`**，
英文文案可自行调整占位符顺序。

---

## 四、分阶段实施计划

| 阶段 | 内容 | 产物 | 验证方式 |
|---|---|---|---|
| **P0** ✅ | 基础设施：`Msg` 消息层 + `MsgPack` 骨架 + zh-CN/en 两个包 + JSON 覆盖 + 回退逻辑 | `Msg.cs`、`MsgPack*.cs` | ✅ 10 档 0 错误；MsgTests 15/15；回归 1179 |
| **P1** ✅ | `AppMessage` 248 个词条全量中英；48 处调用点切到消息层；字段保留不动 | `MsgPack.zh-CN/en.cs`、`BaseManager.cs`、`MsgTests.cs` | ✅ 10 档 0 错误；MsgTests 19/19 双档；回归 1183 |
| **P2** ✅ | 枚举描述本地化：`EnumUtil.ToDescription` / `GetEnumDescriptions` 改造 + 70 条 en（Status 60 + AuditStatus 10） | `Msg.cs`（`GetOrDefault`/`GetEnumDescription`）、`EnumUtil.cs`、`MsgPack.zh-CN/en.cs` | ✅ 10 档 0 错误；MsgTests 24/24 双档；回归 1188 |
| **P3** ✅ | 异常消息 14 处 + 业务状态消息 12 处改调 `Msg.Get`/`Msg.Format`（16 个新键） | `ExpressionEvaluator.cs`、`Utils.cs`、`QqwryUtil.cs`、`BaseResult.cs`、3 个 `WebUtil.LogOn*.cs`、`BaseUserManager.Manual.Logon.cs`、`MsgTestCollection.cs` | ✅ 4 工程 × 4 档 0 错误；MsgTests 28/28 双档；回归 1192 |
| **P4** | 日志 17 处 + 控制台 4 处 + `AppMessage.Service` 33 常量 | 各点改 `Msg.Get/Format` + 49 条词条 | ✅ **已完成**（2026-09-16）：改造 21 处调用点（日志 17 + 控制台 4），新增 49 键（Log 12 / Console 4 / Service 33），字段保留不动；`DotNet.Util` 10 档 + Business/Plus 各 4 档 0 错误，全量非集成回归 1197 通过 / 0 失败 |
| **P5** | 全库扫描补齐 + 文档（README/CHANGELOG）+ 全 TFM 验证 + 全量回归 | 文档、补漏 | ✅ **已完成**（2026-09-16）：扫描查漏再修 142 处（errorMessage 120 / statusMessage 18 / return 3 / message 1）+ 20 个新键；新增 `Localization.md`，README（中英）与 CHANGELOG 已更新；`DotNet.Util` 10 档 + Business/Business.Web/Plus 各 4 档 0 错误，全量非集成回归 1202 通过 / 0 失败 |
| **P6** | 强类型调用层 `Msg.Typed.cs`：400 词条全部生成 `Msg.<分组>.<成员>` 入口 + 文档同步 | `Msg.Typed.cs`、`Localization.md`、README（根 + 主包）、CHANGELOG | ✅ **已完成**（2026-09-18）：21 个分组、342 属性 + 58 方法，与语言包键**双向零差集**（脚本比对 100% 覆盖）；`DotNet.Util` 10 档 + Business/Business.Web/Plus 各 4 档 0 错误，`MsgTests` 双档 40/40；全量非集成回归 1225 全绿 |

**每个阶段结束**：10 档 TFM 编译验证 0 错误 + 跑全量非集成测试 + 汇报，确认后再进下一阶段。

---

## 五、验收标准

1. `zh-CN` 与 `en` 语言包**键集合完全一致**（单测断言，防止漏翻）。
2. `BaseSystemInfo.CurrentLanguage = "en"` 后，关键消息返回英文（单测覆盖各类别各 ≥1 例）。
3. 缺失键**回退中文且不抛异常**（单测）。
4. 默认不改变任何现有行为：不设置时输出与改造前**逐字一致**（用现有测试做回归基线）。
5. `DotNet.Util` / `DotNet.Business` 等 **10 档 TFM 全部 0 错误**。
6. 全量非集成测试通过数 **≥ 1164**（当前基线）。

---

## 六、风险与权衡

| 风险 | 影响 | 应对 |
|---|---|---|
| 字段→属性**二进制不兼容** | 已编译下游需重编 | 若在意，改方案 B（字段不动，新层独立）→ **已采用方案 B** |
| 测试中硬编码中文断言 | 切换语言后测试失败 | 测试基类固定 `zh-CN`；新增英文断言测试 |
| 翻译工作量（约 435 条） | 工期 | 分阶段提交；P1 可只翻被引用 42 键先行（待你确认）→ **已全量翻译 400 条** |
| 中文一词多义 | 英文翻译不准确 | 键带模块前缀（`模块.类.场景`）消歧 |
| 语言切换线程安全 | 并发读取不一致 | 只读字典 + `volatile` 当前语言，切换为整体替换引用 |
| **字符串键拼错静默回退** | 不抛异常、返回键名，难排查 | **P6 强类型层**（2026-09-18）：编译期可查 + IDE 补全 + 400 条全覆盖 |
| **`DotNet.Util.Msg` 与 `DotNet.Web.UI.BasePage.Msg` 同名**（2026-09-15 引入） | 两包同时引用时 `Msg` 二义（`CS0104`/`CS0433`）；`MessageBox.cs` 现报 `CS0436` 警告 | 文档已给命名空间别名方案；根治需把 WebForm 弹窗类改名 `WebMsg` 并留 `[Obsolete]` 转发，**属破坏性变更，待大版本** |

---

## 七、待确认清单（**已全部确认并落地**，留档备查）

- [x] 1.3-1：测试代码中文断言处理方式 → **采用建议**：测试固定 `zh-CN` + 新增英文断言，
  语言切换测试统一挂 `[Collection(MsgTestCollection.Name)]`（`DisableParallelization = true`）。
- [x] 1.3-2：240 个 Msg 键是否全量翻译 → **全量翻译**（最终 400 条，中英键集合完全一致并由单测断言）。
- [x] 3.5：字段→属性的二进制不兼容是否可接受 → **采用方案 B**（静态字段不动，新层独立）。
- [x] 四：分阶段粒度是否认可 → **P0→P5 逐阶段确认完成**，另追加 **P6 强类型层**（2026-09-18 完成）。

## 八、当前状态（2026-09-18）

- 实施阶段 **P0 ~ P6 全部完成**，语言包 400 条（zh-CN / en 键集合一致）。
- 强类型层 `Msg.Typed.cs` **400 成员全覆盖**，`Localization.md` 已改为强类型优先的写法。
- 遗留项 1：**`DotNet.Util.Msg` 同名冲突**（见风险表），待大版本改名。
- 遗留项 2：本计划中 3.1 / 3.3 的设计稿描述已与实现不符，**已在对应小节就地标注偏差**，勿再按旧稿实施。
