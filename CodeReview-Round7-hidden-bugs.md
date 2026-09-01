# DotNet.Util 第 7 轮 Code Review · 隐藏 Bug 清单

> 审查范围：聚焦**隐藏逻辑 bug**（非代码风格、非重复代码）。方法 = 源码深读 + **临时验证程序实测复现**（`C:\Temp\bugcheck`，net8.0 引用 DotNet.Util 实跑），所有结论均有实测输出佐证，非静态推测。
> 状态：**全部 7 项 Bug + P2 4 项 + 补充发现的 Bug 8 均已修复并通过仓内单测验证**（2026-09-01 收尾），改动**均未提交**，等你确认后提交。

---

## 一、🔴 P0 · 数据静默丢失/错乱（3 项）

### Bug 1 · `CsvUtil.GetLength` 赋值顺序颠倒 → 含引号字段的行被整行丢弃
**位置**：`src/DotNet.Util/Util/CsvUtil.cs:622-623`
```csharp
i = j;                 // ← 先赋值
result -= (j - i);     // ← 此时 i 已 == j，(j-i) 恒为 0
```
**后果**：`result -= 0`，列数合并逻辑**完全失效**，GetLength 永远返回 `arr.Length`。当 CSV 某字段被引号包裹且内含分隔符时，该行算出的列数 > 表头列数 → `ToDataTable` 走 `lineColumnCount != headColumnCount` 分支 → **整行被静默丢弃**（仅写日志）。

**实测**（表头 `Name,Desc,Age`，数据行含 `"hello,world"`）：
```
[A] 期望 3 列 2 行 -> 实际 3 列 0 行     ← 2 行数据全部丢失
```
**修复**：交换两行顺序 → `result -= (j - i); i = j;`

---

### Bug 2 · `CsvUtil.ReadSpecialCharacter` 的 `i = j` 对调用方无效 → 列错位、残留引号
**位置**：`src/DotNet.Util/Util/CsvUtil.cs:579`
```csharp
private static string ReadSpecialCharacter(string[] arr, int i, string separator)
{
    ...
    i = j;   // ← i 是值参数，修改的是局部副本，调用方 for 循环的 j 完全不受影响
}
```
**后果**：函数想"跳过去一大步"但做不到（值传递无法回传）。调用方 `ToDataTable` 的 `for (j=0; j<lineColumnCount; j++)` 仍会遍历已被合并进前一列的片段 → 重复读取、数据错位、残留转义引号。

**实测**（表头与数据行都含引号内逗号）：
```
[B] 期望 3 列 1 行 -> 实际 4 列 1 行
[Name][D,esc][esc"][Age]          ← 列名多出 1 列且残留引号
{Tom}{hello,world}{world"}{20}    ← 数据残留 world"，后续列全部右移错位
```
**修复**：改为 `ref int i`（或用返回值回传新索引），让调用方跳过已合并片段；同时配合 Bug 1 修复列数计算。

---

### Bug 3 · `DateUtil.GetStartTime/GetEndTime` 不处理时分秒 → 周期查询漏数据
**位置**：`src/DotNet.Util/Util/DateUtil.cs:544-587`
```csharp
case "Month":
    return now.AddDays(-now.Day + 1);                              // GetStartTime：未归零到 00:00:00
    return now.AddMonths(1).AddDays(-now.AddMonths(1).Day+1).AddDays(-1);  // GetEndTime：未补到 23:59:59
```
**后果**：两个方法只做 `AddDays/AddMonths`，**完全不触碰时分秒**，返回值沿用入参时刻。周期日期计算本身正确，但：
- `GetStartTime` 不归零 → 区间漏掉周期首日 `00:00:00 ~ 调用时刻` 的数据
- `GetEndTime` 不补 `23:59:59` → 区间漏掉周期末日 `调用时刻 ~ 23:59:59` 的数据

按业务典型用法 `BETWEEN GetStartTime(d,"Month") AND GetEndTime(d,"Month")` 做月度统计，**会静默漏掉首尾各一部分数据，且仅在特定时刻调用才明显，极难排查**。

**实测**（入参 `2026-02-15 10:30:45`）：
```
Week  : start=2026-02-09 10:30:45  end=2026-02-15 10:30:45  end是23:59:59? False
Month : start=2026-02-01 10:30:45  end=2026-02-28 10:30:45  end是23:59:59? False
Season: start=2026-01-01 10:30:45  end=2026-03-31 10:30:45  end是23:59:59? False
Year  : start=2026-01-01 10:30:45  end=2026-12-31 10:30:45  end是23:59:59? False
```
**修复**：`GetStartTime` 返回 `.Date`（00:00:00）；`GetEndTime` 返回 `.Date.AddDays(1).AddSeconds(-1)`。若担心破坏现有依赖，可加 `GetStartTimeOfDay/GetEndTimeOfDay` 新 API 并标记旧的 `[Obsolete]`。

---

### Bug 8 · `CsvUtil` 字段以转义双引号结尾时被清空（Bug 2 修复未尽，2026-09-01 补充发现）
**位置**：`src/DotNet.Util/Util/CsvUtil.cs:572`（`ReadSpecialCharacter`）、`:615`（`GetLength`）
```csharp
if (str.EndsWith("\"") && !str.EndsWith("\"\""))   // ← 判据本身不可靠
```
**后果**：当字段内容**以转义双引号结尾**时，末尾形如 `"""`，`EndsWith("\"\"")` 为真 → 误判为「字段未闭合」→ 走 else 分支向后寻找结束项 → 找不到则 `txt` 保持 `""` → **该字段被静默清空，且后续列整体错位**。

**实测**（复刻修复后方法体，`C:\Temp\verify_csv`）：
```
PASS | Tom,"hello,world",20      -> [hello,world]
PASS | Tom,"hello",20            -> [hello]
FAIL | Tom,"He said ""hi""",20   -> []   ← 期望 [He said "hi"]，字段被清空
FAIL | Tom,"He said ""hi"""      -> []   ← 同上
```
另发现 `str.Trim('"')` 会裁剪掉首尾**所有**引号（而非恰好一对），对以引号开头/结尾的内容同样有害。

**修复**：不再「先 `Split`、再靠启发式还原」，改为新增 `SplitCsvLine`，按 **RFC 4180 逐字符状态机**一次性正确拆分：
- 引号态内遇 `""` → 字面量引号并跳过 2 字符；遇单独 `"` → 字段结束
- 未加引号字段保留原 `Trim()` 行为；引号字段内容原样保留（含首尾空格）
- 正确处理空字段、`a,,b` 连续分隔符、`a,` 行尾分隔符

改造后 `arr` 已是正确字段数组，故 `GetLength` 简化为 `arr.Length`、`ReadSpecialCharacter` 简化为直接取值 —— **两者不再需要各自的合并启发式，从根上消除了「列数与实际字段数不一致导致整行被丢」的风险**。

> ✅ **已修复（2026-09-01）**：14 个场景独立实测全绿；仓内**启用了原先 Skip 的** `ToDataTable_EscapedDoubleQuoteInsideQuotedField`，并新增 `ToDataTable_EscapedDoubleQuoteAtEndOfLastColumn`、`ToDataTable_EscapedDoubleQuoteInMiddleOfField`。

---

## 二、🟠 P1 · 逻辑/契约错误（4 项）

### Bug 4 · `ValidateUtil.IsIpv4` 拒绝 `0.x.x.x`（正则首段缺 `|0`）
**位置**：`src/DotNet.Util/Util/ValidateUtil.cs:39`
```
第1段: (25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9])        ← 缺 |0
第2、3段: (...|[1-9]|0)                                                      ← 有 |0
第4段: (...|[0-9])                                                           ← 允许 0-9
```
**后果**：第 1 段无法匹配单个 `0` → **`0.0.0.0` 被判为非法 IPv4**。而 `0.0.0.0` 是合法地址（默认路由 / 服务监听所有网卡），用于绑定地址或白名单校验时会被误拒。

**实测**：
```
IsIpv4("0.0.0.0")       = False   ← 错误，应为 True
IsIpv4("0.1.2.3")       = False   ← 错误，应为 True
IsIpv4("127.0.0.1")     = True
IsIpv4("255.255.255.255")= True
```
**修复**：首段正则补 `|0`（与第 2/3 段一致），或改用 `IPAddress.TryParse` + `AddressFamily.InterNetwork` 判空校验（更稳，顺带解决前导零歧义）。

> ✅ **已修复（2026-08-29 晚）**：采用 `IPAddress.TryParse` + `AddressFamily.InterNetwork` 方案，并额外加「恰好 3 个点」的严格四段点分校验（避免 .NET 把 `1.2.3` 这类宽松写法也判成 IPv4，保持与原正则「严格 4 段」语义一致），同时**移除原回退到 `IsIpv6` 的逻辑**（方法名即「是否为 IPv4」，回退会导致语义与名称不符，且 `IsIpv6` 对 `::` 压缩写法判断有误）。`IsIpv4(null)` 现返回 `false`（不再抛异常）。新增 11 个 InlineData 用例 + `null` 用例，全绿。
> 见 `src/DotNet.Util/Util/ValidateUtil.cs`、`src/DotNet.Util.Tests/Util/ValidateUtilTests.cs`。

---

### Bug 5 · `DateUtil.WeekRange` 当 1/1 是周日时，1/1 不属于任何一周
**位置**：`src/DotNet.Util/Util/DateUtil.cs:389-393`
```csharp
var dayDiff = (-1) * firstOfWeek + 1;   // firstOfWeek: 周日=0
var dayAdd  = 7 - firstOfWeek;
```
当 1/1 是星期日时 `firstOfWeek=0` → `dayDiff=+1` → 第 1 周从 **1/2** 开始，把 1/1 排除在外；同时 `GetWeekOfYear(1/1)` 却返回 1 → **两个 API 自相矛盾**。

**实测**：
```
2023-01-01 是 Sunday   -> 第1周 = 2023-01-02 ~ 2023-01-08; 该周是否覆盖 1/1? False   ← 1/1 无归属
    GetWeekOfYear(2023-01-01) = 1                                                     ← 与上面矛盾
2026-01-01 是 Thursday -> 第1周 = 2025-12-29 ~ 2026-01-04; 覆盖 1/1? True   (正常)
2022-01-01 是 Saturday -> 第1周 = 2021-12-27 ~ 2022-01-02; 覆盖 1/1? True   (正常)
```
**修复**：`dayDiff = -((firstOfWeek + 6) % 7)`（周一为一周首日，周日则回退 6 天），保证 1/1 必被覆盖，并与 `GetWeekOfYear` 语义对齐。

> ✅ **已修复（2026-09-01）**：改为 `dayDiff = -((firstOfWeek + 6) % 7)`、`dayAdd = dayDiff + 6`。该公式**仅改变 1/1 为周日这一种情况**（`0 -> -6`，第 1 周变为上年 12/26 ~ 1/1），周一/周二/…/周六的结果与原实现完全一致，属于最小改动。
> 新增 10 个用例：`WeekRange_FirstWeekCoversJan1`（7 个年份覆盖全部星期情况）、`WeekRange_Jan1IsSunday_ReturnsPreviousMondayToJan1`、`WeekRange_FirstWeek_ConsistentWithGetWeekOfYear`、`WeekRange_WeekOrder2_ShiftsBy7Days`，全绿。

---

### Bug 6 · `SecretUtil.Md5(password, length)` 除 16 外静默忽略 length
**位置**：`src/DotNet.Util/Util/SecretUtil.cs:227-230`
```csharp
if (length == 16) { result = result.Substring(8, 16); }
```
**后果**：只有 `length==16` 才截断，其余**任意值**（8/20/0/64/负数）一律返回 32 位，不抛异常、不告警。调用方按参数期望长度做截断或比较会静默出错。

**实测**：
```
Md5("abc", 16) -> 长度 16: 3cd24fb0d6963f7d
Md5("abc",  8) -> 长度 32: 900150983cd24fb0d6963f7d28e17f72   ← 应为 8
Md5("abc", 20) -> 长度 32: ...                                ← 应为 20
Md5("abc",  0) -> 长度 32: ...                                ← 参数非法却无提示
Md5("abc", 64) -> 长度 32: ...                                ← 越界无提示
```
**修复**：加参数校验（仅允许 16/32，或 1..32 内截断），非法值抛 `ArgumentOutOfRangeException`；至少补 XML 注释说明"仅 16/32 有效"。

> ✅ **已修复（2026-09-01）**：方法入口加校验 `if (length != 16 && length != 32) throw new ArgumentOutOfRangeException(...)`，并更新 XML 注释（`<param>` 改为「仅支持 16 位或 32 位」，补 `<exception>`）。
> ⚠️ **行为变更**：原先传入 8/20/0/64/负数会**静默返回 32 位**，现在改为抛异常（快速失败）。仓库内全部调用点均使用 32（`BaseUserManager.Manual.SetPassword`、`ServiceUtil` 用单参重载 → 32），已核查无影响。
> 新增 7 个用例：`Md5_InvalidLength_Throws`（6 个 Theory 值）、`Md5_16Bit_EqualsMiddleOf32Bit`，全绿。

---

### Bug 7 · `DateUtil.GetDaysOfYear(DateTime)` 注释与实现不符
**位置**：`src/DotNet.Util/Util/DateUtil.cs:157-163`
```csharp
/// <summary>本年有多少天</summary>
/// <returns>本天在当年的天数</returns>        ← 注释说是「第几天」
public static int GetDaysOfYear(DateTime dt)
{
    return IsRuYear(dt.Year) ? 366 : 365;     ← 实际返回「该年总天数」
}
```
**后果**：`<returns>` 明确写"本天在当年的天数"（= DayOfYear），实现返回的却是年总天数。调用方按注释取"今天是第几天"会得到 365 —— **静默错误结果**。

**实测**：
```
GetDaysOfYear(2026-03-01) = 365   (DateTime.DayOfYear = 60)
```
**修复**：二选一 —— ① 改实现为 `return dt.DayOfYear;`（与注释一致）；② 改注释为"返回该年的总天数"（与实现一致）。建议 ②（保持向后兼容）+ 另加 `DayOfYear` 语义的新方法。

> ✅ **已修复（2026-09-01）**：采纳方案 ②，注释改为「该日期所在年份的总天数（平年 365，闰年 366）」，并用 `<remarks>` 明确指出**不是**「第几天」、需要时用 `dt.DayOfYear`。实现保持 365/366 不变，零行为变更、完全向后兼容。

---

## 三、🟡 P2 · 健壮性/注释（4 项）

| # | 位置 | 问题 | 说明 |
|---|---|---|---|
| 8 | `ValidateUtil.cs:36-45` | `IsIpv4(null)` 抛 `ArgumentNullException` | 实测确认。与 `IsIdCard`（有 `IsNullOrEmpty` 判空）不一致，建议补判空返回 false | ✅ **已修复**（随 Bug 4 一并改为 `IPAddress.TryParse` + 判空；`IsIpv6(null)` 亦补判空返回 false） |
| 9 | `DateUtil.cs:538-544` | `GetStartTime` 的 XML summary 误写为「获取结束时间」 | 复制粘贴错误，应为「获取开始时间」 | ✅ **已修复**（进展 2 随 `GetStartTimeOfDay` 新增一并修正 summary 为「获取开始时间」，并加 `[Obsolete]`+`<remarks>`） |
| 10 | `ValidateUtil.cs:381` | 15 位身份证注释「默认补足为19世纪」 | 代码拼 `"19"+YYMMDD` = 19xx 年（**20 世纪**），代码正确但注释错误 | ✅ **已修复**（2026-08-30：注释改为「默认补足为 19xx 年（即 20 世纪）」） |
| 11 | `DateUtil.cs:172-216` | `GetDaysOfMonth(year, 13)` 等非法月份**静默返回 0** | switch 无 default，建议抛 `ArgumentOutOfRangeException` | ✅ **已修复**（2026-08-30：`GetDaysOfMonth(int,int)` 在 switch 前加 `month<1 \|\| month>12` 抛 `ArgumentOutOfRangeException`；`DateTime` 重载因 `month` 取自 `dt.Month` 恒为 1-12 无需改；新增 `GetDaysOfMonth_InvalidMonth_Throws` 单测） |

---

## 四、本轮验证为「无问题」的项（放心）

| 模块 | 验证结论 |
|---|---|
| `SecretUtil.Des/Aes` 加解密往返 | ✅ 4 组密钥（20/8/5/1 字符）全部往返正确，短密钥自动补位，健壮 |
| `SecretUtil` 16 位 MD5 | ✅ `Substring(8,16)` 标准实现，正确 |
| `DateUtil` 时间戳往返（s/ms） | ✅ `GetTimeStamp` ↔ `GetLocalTime` 双精度往返一致 |
| `DateUtil` 周期日期计算 | ✅ Week/Month/Season/Year 的**日期**部分全部正确（问题只在时分秒，见 Bug 3） |
| `ValidateUtil.IsIdCard` | ✅ 18 位 GB 11643-1999 校验位算法（权重+校验码表）**完全正确** |
| `DateUtil.IsRuYear` 闰年 | ✅ `(n%400==0)||(n%4==0&&n%100!=0)` 正确 |
| `CsvUtil` 空转义字段 `""` | ✅ `Tom,"",20` 解析为 3 列、Desc 为空，正确 |

---

## 五、建议修复优先级

1. **先修 Bug 1 + Bug 2（CsvUtil）** —— 数据静默丢失/错乱，危害最大，且两处改动都很小（交换顺序 + 改 `ref int i`）。建议补 CSV 引号场景单测。
2. **再修 Bug 3（GetStartTime/GetEndTime）** —— 涉及公开 API 语义，建议**新增 `GetStartTimeOfDay/GetEndTimeOfDay`** 并对旧的标 `[Obsolete]`，避免破坏现有调用方。
3. **Bug 4（IsIpv4）** —— 一行正则修补，建议改用 `IPAddress.TryParse` 更彻底。
4. **Bug 5/6/7** —— 边界与契约问题，按影响面排期。
5. **P2 注释/健壮性** —— 随手续修。

---

_审查日期：2026-08-29 · 验证环境：net8.0（周期逻辑为框架无关代码，结论适用全 TFM）· **修复进度：全部 7 项 Bug + P2 全部 4 项 + 补充发现的 Bug 8 均已修复**，均未提交。_

_本轮（2026-09-01）收尾验证：_
- _**仓内 xunit 已可跑通**：绕开 IDE obj 锁的命令为 `dotnet test src/DotNet.Util.Tests/DotNet.Util.Tests.csproj -c Debug -f net8.0 -p:GenerateAssemblyInfo=false -p:GenerateTargetFrameworkAttribute=false --filter "FullyQualifiedName!~IntegrationTests"`。_
- _结果：**1089 通过 / 0 失败（排除集成测试）**。另有 1 个 `HttpUtilTests` 用例在全套并行时偶发失败（本机临时端口 `HttpListener` 争用，单独复跑 8/8 通过、且每次失败的用例不同），与本次改动无关。_
- _多 TFM 编译验证：`net48`、`netstandard2.0`、`net8.0` 均 **0 错误**（老框架兼容性确认）。_
- _测试数变化：1071 -> 1090（新增 19 个用例 + 启用 1 个原 Skip 用例）。_
