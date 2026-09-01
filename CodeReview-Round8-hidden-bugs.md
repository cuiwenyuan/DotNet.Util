# DotNet.Util 第 8 轮 Code Review · 隐藏 Bug 报告

> 审查范围：前 7 轮已覆盖 **CsvUtil / DateUtil / ValidateUtil / SecretUtil**；本轮转向**未被深挖的高风险工具文件**。
> 审查方式：双 agent 并行扫描（字符串/解析/数学组 + IO/反射/集合/校验组）+ **实测复现**（独立控制台 `C:\Temp\verify_r8` 复制方法体，SDK 10.0.400）。
> 状态：**仅审查，未修改任何仓库代码**，等你确认修复范围。

---

## 一、🔴 P0 · 崩溃 / 静默数据损坏（3 项，均实测复现）

| # | 位置 | 问题 | 实测结论 |
|---|---|---|---|
| **R8-1** | `Validation.cs:61-88` `CheckPasswordStrength` | `password.IsNullOrEmpty()` 仅置 `returnValue=false` 但**未 `return`**，继续 `password.Length` → `null`/空串直接 `NullReferenceException` | ✅ 实测 `CheckPasswordStrength(null)` 抛 NRE（应为返回 `false`） |
| **R8-2** | `BaseUtil.Convert.cs`（L60/74/116/130/144/158/172/200/228/251/256/270/286/304 等 ~20 处）`ConvertToInt/ToDecimal/ToDateTime`… | 全程 `targetValue.ToString()` + `TryParse(...)` **未指定 `CultureInfo`**，且 `ToString()` 本身也用当前文化 → 跨国化环境**静默数值错误** | ✅ 实测 `ConvertToDecimal("1234.56")` 在 `de-DE` 下 = **123456**（"." 被当千分位）；`ConvertToDateTime("12/05/2026")` 日月歧义 |
| **R8-3** | `FileUtil.cs:150-159` `SaveFile` | `Path.GetDirectoryName("test.txt")=""` → `Directory.Exists("")=false` → `Directory.CreateDirectory("")` 抛 `ArgumentException`。**裸文件名（无目录）必崩** | ✅ 实测 `dir=""` → `CreateDirectory("")` 抛异常 |

---

## 二、🟠 P1 · 逻辑/契约/安全错误（6 项，均实测或代码确认）

| # | 位置 | 问题 | 实测/确认 |
|---|---|---|---|
| **R8-4** | `Validator.cs:51-57` `IsDouble` | 正则 `^([0-9])[0-9]*(\.\w*)?$` 中 `\w*` 匹配字母 → `IsDouble("123.abc")` 误判 `true` | ✅ 实测返回 `True`（应为 `false`） |
| **R8-5** | `Validation.cs:141-145` `IsMobile` | 号段白名单仅 `13/15/18/147`，**缺 14/16/17/19** → `170/199/166/198` 等大量合法号码被拒 | ✅ 实测 `IsMobile("17012345678")`、`IsMobile("19912345678")` 均返回 `False`（应为 `true`） |
| **R8-6** | `EnumUtil.cs:45,60` `EnumToDataTable` | 列固定 `System.Int32`，但 `Convert.ToInt32(...)` 底层为 `long/ulong` 且值 > `Int32.MaxValue` 时抛 `OverflowException`；L52 注释谎称"兼容 uint/long/ulong" | ✅ 实测 `enum Big:long{A=3000000000L}` 触发 `OverflowException` |
| **R8-7** | `StringUtil.cs:255-259` `StringToInList` | ① `id==null` 直接 NRE；② 值不转义单引号 → `O'Brien` 生成 `'O'Brien'` **SQL 语法错误 + 注入** | ✅ 实测 `null`→NRE；`O'Brien,Smith`→`O'Brien','Smith` 未转义 |
| **R8-8** | `StringUtil.cs:37-52` `GetLike` | 逐字符拼 `LIKE '%'+t+'%'`，**不调用 `SqlSafe`、不转义 `%`/`_`/`'`**；`search` 为空时返回非法 SQL `"()"` | 代码确认：空串 → `result=""` → `return "(" + "" + ")"` = `"()"`；含 `'` 生成 `LIKE '%'%'` |
| **R8-9** | `FileUtil.cs:549-611` `CopyDirectory` | 默认 `deleteSourceFile=true` **且** `deleteExistingFile=true` → 名为 Copy 实为 Move + 删目标；硬编码 `\\` + `LastIndexOf("\\")`，非 Windows/正斜杠路径出错 | 代码确认：L605 `File.Delete(sourceFileName)` 用默认 `true`；L572 递归亦带默认 `deleteSourceFile=true` |

---

## 三、🟡 P2 · 健壮性/注释/边界（7 项，代码确认）

| # | 位置 | 问题 |
|---|---|---|
| **R8-10** | `IpUtil.cs:220-221` `IsLocalIp` | `StartsWith("172.")` 覆盖整个 `172.0.0.0/8`，远超 RFC1918 私网 `172.16.0.0/12` → 公网 `172.32.x.x` 等被**误判为本地** | 
| **R8-11** | `StringUtil.cs:71` `GetSearchString` | `searchKey.Replace('[','_')` 把字面 `[` 变成 LIKE 通配符 `_`，改变查询语义（应 `[[]` 转义） |
| **R8-12** | `StringUtil.cs:325-338` `DeleteUnVisibleChar` | `foreach (var t in sourceString)` 未判空，`sourceString==null` 抛 NRE |
| **R8-13** | `Validator.cs:32-44` `IsNumeric(string)` | 注释称"判断 Int32"，但正则允许小数点，`IsNumeric("12.34")` 返回 `true` |
| **R8-14** | `ReflectionUtil.cs:21` `Bf` | `BindingFlags.DeclaredOnly` 导致 `GetProperty/GetField/SetProperty/SetField` **取不到基类继承成员**（如父类 `Id`），易 NRE |
| **R8-15** | `NewLife/EncodingUtil.cs:42-45` `Detect(Stream)` | `stream.Position = 0` + `stream.Length` 对非 seekable 流（网络流）抛 `NotSupportedException` |
| **R8-16** | `TraceabilityUtil.cs:22-23` `GenerateKey` | `new Random(DateTime.Now.Ticks)` 高频调用种子相同 → 重复 key；洗牌仅交换下标 0，分布不均（非加密强度） |

---

## 四、本轮审查覆盖度

- **已通读并取证**：`StringUtil.cs`、`Validator.cs`、`Validation.cs`、`BaseUtil.Convert.cs`、`EnumUtil.cs`、`FileUtil.cs`、`IpUtil.cs`、`ReflectionUtil.cs`、`EncodingUtil.cs`、`TraceabilityUtil.cs`、`RmbUtil.cs`、`ExpressionEvaluator.cs`、`ObjectUtil.cs`、`ListUtil.cs`、`XmlConfigUtil.cs`、`RandomUtil.cs`/`RandUtil.cs`、`BaseUtil.Sort.cs`、`BaseUtil.DataTable.cs`、`PathUtil.cs`、`StringUtil.CodeStyle.cs`、`StringUtil.PinyinHelper.cs`。
- **RmbUtil 数值逻辑**：用独立控制台对 0/1/亿/亿零1/千万/万亿/`10101010101.01` 等 + 亿组扫描**实测正确**，本轮未发现数值 bug。
- **未深挖**：`CsvUtil/DateUtil/ValidateUtil/SecretUtil`（前几轮已修，按约定未动）；`RandUtil` 主体在 `NewLife.Security.Rand`（不在本仓库源码）。

---

## 五、建议修复优先级（供你拍板）

1. **R8-1 / R8-3**：崩溃类，调用方极易踩到（空密码、裸文件名保存）→ 优先。
2. **R8-2**：静默数据损坏，跨国化部署高危，影响面最大（~20 处）→ 建议统一抽 `CultureInfo.InvariantCulture` 辅助方法。
3. **R8-4 / R8-5 / R8-6 / R8-7 / R8-8**：契约/安全类，实测复现 → 其次。
4. **R8-9**：破坏性默认参数，按需（是否要改默认行为需你确认，可能破坏现有调用方）。
5. **R8-10~R8-16**：健壮性/注释，低风险。

---

## 六、修复进度（已按 P0→P1 顺序开工并验证）

| # | 状态 | 改动文件 | 修复要点 |
|---|---|---|---|
| R8-1 | ✅ 已修+测 | `Validation.cs` | `CheckPasswordStrength` 空值分支加 `return false`，消除 NRE |
| R8-3 | ✅ 已修+测 | `FileUtil.cs` | `SaveFile` 目录为空时跳过 `CreateDirectory`，裸文件名不再崩溃 |
| R8-2 | ✅ 已修+测 | `BaseUtil.Convert.cs` | 全系列 `Convert.ToString(targetValue, InvariantCulture)` + `TryParse(..., NumberStyles.*, InvariantCulture, ...)`；加 `using System.Globalization` |
| R8-4 | ✅ 已修+测 | `Validator.cs` | `IsDouble` 正则 `(\.\w*)?` → `(\.[0-9]+)?` |
| R8-5 | ✅ 已修+测 | `Validation.cs` | `IsMobile` 扩展为 `^1[3-9][0-9]{9}$`（覆盖 14/16/17/19） |
| R8-6 | ✅ 已修+测 | `EnumUtil.cs` | `EnumToDataTable` 按 `Enum.GetUnderlyingType` 建列并直接存底层值；`GetEnumDescriptions` 同法去 `Convert.ToInt32` 溢出 |
| R8-7 | ✅ 已修+测 | `StringUtil.cs` | `StringToInList` 加 null 判空 + 值内单引号 `''` 转义（保持 `a','b','c` 契约） |
| R8-8 | ✅ 已修+测 | `StringUtil.cs` | `GetLike` 空串返回空 + 转义 `%`/`_`/`[`/`'` |
| R8-9 | 🔄 行为变更已拒·保留旧默认 | `FileUtil.cs` | **默认 `deleteSourceFile` 还原为 `true`（保留 Copy 实为 Move+删源旧语义，旧调用依赖）**；仅保留 `Path.Combine`/`Path.GetFileName` 跨平台路径加固；调用方若需纯复制须显式 `deleteSourceFile: false`。仓库内无其它调用点（仅递归自调用），下游项目若调用须自行显式传参。 |
| R8-10 | ✅ 已修+测 | `IpUtil.cs` | `IsLocalIp` 新增 `IsPrivate172` 仅认 RFC1918 `172.16.0.0/12`（172.16–31），公网 172.15/172.32 不再误判为本地 |
| R8-11 | ✅ 已修+测 | `StringUtil.cs` | `GetSearchString` 字面 `[`→`[[]`、`]`→`[]]` 逐字符转义（避免二次转义），不再误变 `_` 改变语义 |
| R8-12 | ✅ 已修+测 | `StringUtil.cs` | `DeleteUnVisibleChar` 加 `sourceString==null` 保护，返回空串（不再 NRE） |
| R8-13 | ✅ 已修+测 | `Validator.cs` | `IsNumeric` 注释改为"整数/小数"+正则收紧为 `^[-]?[0-9]+(\.[0-9]+)?$`（拒绝纯 `.`），与实现一致 |
| R8-14 | ✅ 已修+测 | `ReflectionUtil.cs` | `Bf` 移除 `BindingFlags.DeclaredOnly`，`GetProperty/GetField/SetProperty/SetField` 可取基类继承成员 |
| R8-15 | ✅ 已修+测 | `NewLife/EncodingUtil.cs` | `Detect(Stream)` 加 `!stream.CanSeek` 分支，非 seekable 流（网络流）按可读内容做 BOM 启发式，不再抛 NotSupportedException |
| R8-16 | ✅ 已修+测 | `TraceabilityUtil.cs` | `GenerateKey` 改用共享 `Random` 实例 + 加锁 + Fisher-Yates 全洗牌（修复同 tick 重复种子；保留 `random=0` 返回默认顺序契约），洗牌分布更均匀 |

---

_审查日期：2026-09-01 · 验证环境：net8.0 独立控制台（SDK 10.0.400），`C:\Temp\verify_r8` 实测 10/10 怀疑点全部复现；修复后 `Round8BugFixTests`（20）+`Round8P2BugFixTests`（26，R8-9 已按用户决定还原默认行为）用例全绿，全量非集成回归 1135 通过 / 1 失败（仅 `HttpUtilTests.Get_ReturnsBody` 本机 `HttpListener` 端口争用偶发，与改动无关）。`net48`/`netstandard2.0` 编译 0 错误。所有改动均未提交。_
