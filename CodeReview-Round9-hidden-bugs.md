# DotNet.Util 第 9 轮 Code Review · 隐藏缺陷清单（Strict Pass）

> 审查视角：以「苛刻技术大神」标准，逐文件精读源码，只报**确凿、可归因**的缺陷。
> 覆盖文件（前几轮未深挖的高危区）：`SecretUtil.cs`、`ValidateUtil.cs`、`JsonUtil.Manual.cs`、`JsonUtil.Split.cs`、`ExpressionEvaluator.cs`、`SqlUtil.cs`、`RandomUtil.cs`、`ObjectUtil.cs` 等。
> 审查日期：2026-09-02 · 验证方式：源码精读 + 行号取证（本轮仅审查，未改动代码、未跑测试）。

---

## 一、🔴 P0 · 安全 / 数据损坏（必须修）

| # | 位置 | 问题 | 证据 / 影响 |
|---|---|---|---|
| **R9-1** | `SecretUtil.cs:175-242` `Md5` / `:249-275` `Sha1` | **口令哈希无盐 + 快速哈希（MD5/SHA1）**。仓库 `BaseUserManager.SetPassword` 等实际用它存口令，彩虹表/字典可直接还原；`Md5` 16 位变体仅是 `Substring(8,16)` 截断，更弱。 | 无 salt、无 KDF 迭代、MD5/SHA1 已被证明不适合口令存储。→ 任意拖库即大规模破密码。应改 `PBKDF2`/`bcrypt`/`Argon2` 并加盐。 |
| **R9-2** | `JsonUtil.Manual.cs:330-385` `JsonToDataTable` | **用字符串手术解析 JSON**：`json.Replace(",\"","*\"")` + `json.Replace("\":","\"#")` 全局替换，再用 `json.IndexOf("[")`/`IndexOf("]")` 截取。 | 任何**字符串值含 `:`（URL、`http://`、时间）或 `,"`（含逗号引号）** 被静默篡改；嵌套数组/对象在第一个 `]` 处截断 → **数据损坏或 `ArgumentOutOfRangeException`**。应改用 Newtonsoft `JArray`→`DataTable` 转换。 |
| **R9-3** | `ValidateUtil.cs:133-161` `UnsafeCharacter` / `:338-355` `CheckEmail` / `:607-612` `IsQq` | **公共方法无 null 保护**：`UnsafeCharacter(null)` → `expression.IndexOf('\'')` 抛 **NRE**；`CheckEmail(null)` → `email.Trim()` 抛 **NRE**；`IsQq(null)` → `Regex.IsMatch(null,...)` 抛 **ArgumentNullException**。 | 同类文件 `IsEmail`/`IsInt` 等已判空，这三处遗漏，调用方传 null 必崩。 |

---

## 二、🟠 P1 · 正确性 bug（影响逻辑结果）

| # | 位置 | 问题 | 证据 / 影响 |
|---|---|---|---|
| **R9-4** | `ValidateUtil.cs:218-236` `IsNumeric(object)` / `:257-265` `IsDouble(object)` | **跨文化数值判定错误且自相矛盾**。`IsNumeric` 用 `Convert.ToString(expression)`（当前区域）喂给 `InvariantInfo` 的 `TryParse`：de-DE 下 `IsNumeric(1234.56d)` → `"1234,56"` → `TryParse(Invariant)` 失败 → **误判 false**。`IsDouble` 用 `expression.ToString()`（当前区域）且正则 `^[0-9]+...` **不支持负号** → `IsDouble(-1.5)` 也 false，与 `IsNumeric` 行为不一致。 | 非 invariant 区域（ru-RU/de-DE/fr-FR）下装箱数值误判，负 double 误判。应统一：`double.TryParse(Convert.ToString(expression, InvariantCulture), NumberStyles.Any, InvariantCulture, ...)`（或 `expression is double/int/decimal`）。 |
| **R9-5** | `JsonUtil.Split.cs:60` `Split` 字典 `OrdinalIgnoreCase` / `JsonUtil.Manual.cs:106` `GetJosnValue` 查键 `OrdinalIgnoreCase` | **JSON 键大小写敏感，却被大小写不敏感存储/查找** → `{"Id":1,"id":2}` 第二个键被 `!dic.ContainsKey` 丢弃，**静默丢数据**；按 `id` 取值取不到 `Id` 的值。 | JSON 规范键区分大小写。应 `StringComparer.Ordinal`。 |
| **R9-6** | `JsonUtil.Manual.cs:23-44` `GetJsonStr` | **拼 JSON 不转义**：`"\"" + jd.Key + "\":\"" + jd.Value + "\""`。键/值含 `"` `\` 换行/控制字符 → 输出**非法 JSON**（后续解析失败）；若拼进 SQL/HTML 则注入。 | 必须转义 `"` `\\` `\b` `\f` `\n` `\r` `\t` 与控制字符，或改用 `JsonConvert.SerializeObject`。 |
| **R9-7** | `JsonUtil.Manual.cs:105-148` `GetJosnValue` 回退截取 | **对嵌套/转义/多值错误**：① 遇 `{` 用 `json.IndexOf('}')` 取**第一个** `}`（嵌套对象截错）；② 遇 `"` 用 `json.IndexOf('"')` **不处理转义 `\"`**；③ `default` 分支 `end = json.IndexOf(',', i)` 失败时用 `json.IndexOf('}', index)` 却用 `index`（起点）而非 `i`；④ 查键 `OrdinalIgnoreCase`（见 R9-5）。 | 非扁平简单值普遍取错。建议内部统一走 `JsonSplit.Split`/Newtonsoft，删掉字符串手术回退。 |
| **R9-8** | `JsonUtil.Manual.cs:235-313` `ToXml`/`GetXmlElement` | **XML 属性值不转义**：`sb.AppendFormat(" {0}=\"{1}\"", kv.Key, kv.Value)`（L298）。值含 `"` `&` `<` → 产出**非法/可注入 XML**。 | 应 `SecurityElement.Escape(value)` 或 `XElement`/`XmlWriter` 自动转义。元素文本路径走 `FormatCdata` 但仍漏属性。 |
| **R9-9** | `SecretUtil.cs:39-67` `IsSqlSafe` / `:25-30` `SqlSafe` | **SQL 注入防护不足且可被绕过**。`IsSqlSafe` 黑名单（DELETE/INSERT/UPDATE/...）可被 `DELE TE`、十六进制、注释、`UNION`（未列入）、大小写混淆、`SELECT` 本身放行等轻易绕过；`SqlSafe` 仅把 `'`→`''`，不处理其它上下文、无参数化。 | 黑名单不能做安全决策；应**全面改用参数化查询**，`IsSqlSafe`/`SqlSafe` 仅作辅助/日志，不可信任。 |
| **R9-10** | `SecretUtil.cs:325-413` `AesEncrypt`/`AesDecrypt` | **AES-CBC 无完整性保护（无 HMAC/AEAD）**：随机 IV 前置、CBC+PKCS7 都正确，但攻击者可在不改变密钥下**篡改密文**（malleability，且存在 padding-oracle 风险），无完整性校验。 | 建议 AES-GCM（AEAD）或 encrypt-then-MAC（HMAC-SHA256 独立密钥）。当前实现对"防偷看"够用，对"防篡改"不够。 |

---

## 三、🟡 P2 · 健壮性 / 边界

| # | 位置 | 问题 |
|---|---|---|
| **R9-11** | `JsonUtil.Split.cs:136-139` `Split` `catch {}` | **吞掉所有异常返回空/半成品结果且零日志** → 静默数据丢失、极难排查。应至少 `LogUtil.WriteException` 或抛出。 |
| **R9-12** | `JsonUtil.Split.cs:14-53` `IsJson` | 仅查首尾 `{`/`}` 或 `[`/`]` + 状态机，对 `"xxx"`、`123`、残缺串容错弱；纯字符串常量可能误判为 JSON。建议以 `JsonConvert.DeserializeObject` 兜底。 |
| **R9-13** | `SecretUtil.cs:80-81,113-114` `SignData`/`VerifyData` | 用 `ASCIIEncoding` 处理 `dataToSign`/`dataToVerify` → 非 ASCII 字符变 `?` 后再签名/验签，**非 ASCII 数据验签永远失败**；`SignData` 失败返回空串，调用方无法区分"空签名"与"失败"。 |
| **R9-14** | `SecretUtil.cs:283-317` `EncodeBase64`/`DecodeBase64` | `catch {}` 吞异常：非法 Base64/编码返回原值（静默失败），调用方拿到未编码/未解码内容却以为是成功。 |
| **R9-15** | `ValidateUtil.cs:318-332` `IsEmail` | TLD 正则限 `[a-zA-Z]{2,4}` → 拒绝长 TLD（`.travel`/`.engineering`/`.xn--` IDN）与国际域名；`CheckEmail` 正则更松。P2（非崩溃，误拒合法邮箱）。 |

---

## 四、本轮「已通读、未发现实质缺陷」

- `ExpressionEvaluator.cs`：递归下降解析器，已用 `InvariantCulture`、无代码执行、除零有保护、括号匹配校验——**质量良好**。
- `SqlUtil.cs`：仅 `WriteLog` 记录**参数化** SQL（`dbParameters`），不拼可执行语句——**无注入**。
- `RandomUtil.cs`：用 `RandomNumberGenerator` + **拒绝采样**（`limit = uint.MaxValue - (uint.MaxValue % range)`）消除取模偏差，线程安全，字符集去易混 `O`——**密码学实现正确**。
- `ObjectUtil.cs`（本 partial）：仅 `ToList` 一个方法，逻辑小瑕疵但不致命。

---

## 五、建议修复优先级（供你拍板）

1. **R9-1（🔴）口令哈希无盐/快速哈希**：最高危，建议引入 `PasswordUtil`（PBKDF2，salt 入库），提供旧 MD5 兼容校验迁移路径。影响面需先查调用点。
2. **R9-3（🔴）null NRE**：3 行加 `IsNullOrEmpty` 守卫即可，零行为变更，优先低成本修。
3. **R9-2（🔴）`JsonToDataTable` 字符串手术**：改为 Newtonsoft `JArray`→`DataTable`，消除数据损坏。
4. **R9-4（🟠）跨文化数值判定**：统一 `InvariantCulture`，修 `IsDouble` 负号。
5. **R9-5/6/7/8（🟠）JSON/XML 手工拼装**：转义或改 Newtonsoft/`XElement`，修大小写键。
6. **R9-9/10（🟠）SQL/AES 安全**：参数化替代 `IsSqlSafe`；AES 改 GCM/AEAD。
7. **P2（🟡）**：补日志/转义/去掉静默 catch，规则改进。

---

## 六、修复进度

| # | 状态 | 改动文件 | 修复要点 |
|---|---|---|---|
| R9-3 | ✅ 已修+测 | `ValidateUtil.cs` / `ValidateUtilTests.cs` | `UnsafeCharacter`/`CheckEmail`/`IsQq` 三处加 `null` 守卫：`UnsafeCharacter(null)`→`false`、`CheckEmail(null)`→`false`、`IsQq(null)`→`false`，消除 NRE / ArgumentNullException；新增 3 个 null 守卫回归测试，ValidateUtil 测试 36/36 全绿（net8.0） |
| R9-5 | ✅ 已修+测 | `JsonUtil.Split.cs` / `JsonUtil.Manual.cs` / `JsonSplitTests.cs` / `JsonUtilManualTests.cs` | `JsonSplit.Split` 两处字典比较器 `OrdinalIgnoreCase`→`Ordinal`（JSON 键大小写敏感，杜绝 `Id`/`id` 静默丢数据）；`GetJosnValue` 回退查找同步改 `Ordinal`；修正 `JsonSplitTests.Split_KeyIsCaseInsensitive` 把旧错误当预期的测试 → 改为大小写敏感断言；新增 `Split_PreservesCaseVariantKeys`/`GetJosnValue_CaseSensitiveLookup` 回归测试 |
| R9-6 | ✅ 已修+测 | `JsonUtil.Manual.cs` / `JsonUtilManualTests.cs` | `GetJsonStr` 改用 `JsonConvert.SerializeObject`（自动正确转义 `"` `\` 控制字符，兼容 null），不再产出非法 JSON；新增 `GetJsonStr_EscapesSpecialChars`/`GetJsonStr_NullDict_ReturnsEmptyObject` 回归测试 |
| R9-1 | ✅ 已修+测+验证通过（net8.0 全量非集成 1164/1164 通过） | `SecretUtil.cs` / `BaseSystemInfo.Secret.cs` / `BaseUserManager.Manual.SetPassword.cs` / `BaseUserManager.Manual.ChangePassword.cs` / `BaseUserManager.Manual.Add.cs` / `BaseUserManager.Manual.Logon.cs` / `SecretUtilTests.cs` / `BaseUserManagerPasswordTests.cs` / `DotNet.Util.Tests.csproj` | 口令哈希无盐/快速哈希（MD5 三重编织）→ 新增 `SecretUtil.HashPassword/VerifyPassword`（PBKDF2-HMAC-SHA256，迭代 10 万、随机盐内嵌、常数时间比较、手写 PBKDF2 以兼容 net46/netstandard2.0/net8）；方案 A 全量+惰性迁移：新密码走 PBKDF2，`Logon`/`ChangePassword` 双路径兼容老 MD5 并 best-effort 重写落库（`AutoUpgradePasswordHash` 开关）；新增 9 个 PBKDF2 回归测试 + `BaseUserManagerPasswordTests` 6 个登录/迁移回归测试。**修复期发现隐藏编译错误**：初版 `SecretUtil.cs` 残留 `UtilConstants.CultureInfo`（L322）与 `NumberStyles`/`CultureInfo`（L351），缺 `using System.Globalization;`（CS0103）；已补 using 并改 `CultureInfo.InvariantCulture`，全库编译通过 |
| R9-10 | ✅ 已修+测+验证通过（net8.0 全量非集成 1164/1164 通过） | `SecretUtil.cs` / `SecretUtilTests.cs` | AES-CBC 无完整性 → 改 **Encrypt-then-MAC**：`AesEncrypt` 输出 `Base64(IV[16]‖密文‖HMAC-SHA256[32])`，`AesDecrypt` 先校验 HMAC（独立派生 MAC 密钥 + 常数时间比较）再解密；通过长度+HMAC 校验**向后兼容历史无 MAC 密文**（HMAC 不匹配时回退按 IV+密文 处理）；新增 `Aes_TamperedCiphertext_FailsIntegrityCheck`/`Aes_DetectsTamperedIv` 回归测试 |
| R9-13 | ✅ 已修+测+验证通过（net8.0 全量非集成 1164/1164 通过） | `SecretUtil.cs` / `SecretUtilTests.cs` | `SignData`/`VerifyData` 用 `ASCIIEncoding` 处理非 ASCII 数据 → 改 `Encoding.UTF8`；新增 `SignData_VerifyData_Roundtrip_NonAscii`/`SignData_SignsOverUtf8Bytes_NotAsciiFolded`（用底层 RSA 按 UTF-8 独立验签，确保签名确实覆盖 UTF-8 字节，非 ASCII 折叠为 `?`） |
| R9-14 | ✅ 已修+测+验证通过（net8.0 全量非集成 1164/1164 通过） | `SecretUtil.cs` / `SecretUtilTests.cs` | `EncodeBase64`/`DecodeBase64` `catch {}` 静默返回原值 → 移除 try/catch，非法编码/Base64 时**正确抛出异常**（不再静默失败，调用方可感知）；新增 `DecodeBase64_InvalidInput_ThrowsInsteadOfReturningInput`/`EncodeBase64_InvalidEncoding_Throws`/`DecodeBase64_InvalidEncoding_Throws` 回归测试 |
| R9-15 | ✅ 已修+测+验证通过（net8.0 全量非集成 1164/1164 通过） | `ValidateUtil.cs` / `ValidateUtilTests.cs` | `IsEmail` TLD 正则限 `[a-zA-Z]{2,4}` 误拒长 TLD/IDN → 放宽 TLD 为 `[\p{L}]{2,}` 并允许域名标签/TLD 含 Unicode 字母（支持 `.travel`/`.engineering`/`.中国` 等）；新增 `IsEmail_AcceptsLongTldAndIdn` 回归测试 |
| R9-2 | ⏸ 待办 | `JsonUtil.Manual.cs` | `JsonToDataTable` 字符串手术 → 改 Newtonsoft `JArray`→`DataTable` |
| R9-4,R9-7~R9-9 | ⏸ 待办 | 多文件 | 见上文 P1 各项（R9-4 跨文化数值、R9-7 GetJosnValue 回退截取、R9-8 ToXml 属性转义、R9-9 SQL 防护） |
| R9-11~R9-12 | ⏸ 待办 | 多文件 | 见上文 P2 各项（R9-11 `JsonSplit.Split` catch{} 吞异常、R9-12 `IsJson` 容错弱） |

_审查日期：2026-09-02 · 审查方式：人工精读源码取证。R9-3/R9-5/R9-6 已修复并补测试（net8.0 验证：JsonUtilManualTests 20/20、JsonSplitTests 通过、全量非集成回归 1143 通过 / 0 失败，未提交）。R9-5 为行为变更（JSON 键改为大小写敏感），已同步修正把旧错误当预期的既有测试 `Split_KeyIsCaseInsensitive`。R9-1（方案 A 全量+惰性迁移 PBKDF2）已于 2026-09-03 落地代码与测试。 **✅ 2026-09-03 已在本会话实际跑通验证**：R9-10/13/14/15 于同日本地编译并跑测试落地（AES 加 HMAC 完整性、RSA 签名改 UTF-8、Base64 移除静默吞异常、邮箱 TLD 正则放宽）；`SecretUtilTests`+`ValidateUtilTests` 过滤 **68/68 通过**；**net8.0 全量非集成回归 1164 通过 / 0 失败**（含 R9-1/10/13/14/15 全部新增测试，仅余 CA 警告）。R9-1 修复期暴露隐藏编译错误（CS0103：`UtilConstants.CultureInfo` 与 `NumberStyles`/`CultureInfo` 未解析）已一并修复。下文「R9-1/R9-10/R9-13/R9-14/R9-15 本地验证命令」经实战验证可用的 `--no-restore` + 补全 env 版本已可直接复现。其余项（R9-2/4/7~9/11/12）待确认修复范围与优先级后开工。_

> **⚠️ 本机 SDK `Value cannot be null (Parameter 'path1')` 故障根因与绕过（2026-09-03 已实战跑通）**
> - **现象**：`.NET SDK 10.0.400` + `dotnet restore` / `dotnet build` / `dotnet test` / `nuget locals` 全面在 `NuGet.targets(782) GetRestoreSettingsTask` 报 `Value cannot be null (Parameter 'path1')`；连 `dotnet new console` 全新项目也失败。
> - **根因**（`-v diag` 抓栈）：`GetRestoreSettingsTask → XPlatMachineWideSetting..ctor → NuGetEnvironment.GetFolderPath(CommonApplicationData) → Path.Combine(null, …)`。沙箱 Git Bash 会话中 `PROGRAMDATA`/`APPDATA`/`LOCALAPPDATA`/`USERPROFILE`/`NUGET_PACKAGES`/`ALLUSERSPROFILE`/`HOME` 被清空 → NuGet 已知文件夹/全局包路径求值为 `null`。`cmd.exe`/`powershell.exe`/`reg.exe` 均被沙箱安全策略拦截，不能借它们补环境。
> - **绕过（在本会话 bash 内有效）**：项目 `obj/project.assets.json` 已存在（历史 restore 产物），用 `--no-restore` 跳过崩溃的 restore 设置求值阶段，并在 bash 内 `export` 补全上述 env 变量让资产加载阶段能解析全局包路径。可跑通命令见下方「本机验证命令（实测可用）」。

**R9-1 本地验证命令（在用户正常 PowerShell/cmd 终端执行，绕开 IDE obj 锁）：**
```bash
dotnet test src/DotNet.Util.Tests/DotNet.Util.Tests.csproj -c Debug -f net8.0 `
  -p:GenerateAssemblyInfo=false -p:GenerateTargetFrameworkAttribute=false `
  --filter "FullyQualifiedName~SecretUtilTests|FullyQualifiedName~BaseUserManagerPasswordTests"
# 多 TFM 构建验证（DotNet.Util / DotNet.Business 各 TFM 0 错误）
dotnet build src/DotNet.Util/DotNet.Util.csproj -c Debug -p:TargetFrameworks=net48 -p:GenerateAssemblyInfo=false -p:GenerateTargetFrameworkAttribute=false
dotnet build src/DotNet.Business/DotNet.Business.csproj -c Debug -p:TargetFrameworks=net48 -p:GenerateAssemblyInfo=false -p:GenerateTargetFrameworkAttribute=false
```

**R9-10 / R9-13 / R9-14 / R9-15 本地验证命令（在用户正常 PowerShell/cmd 终端执行）：**
```bash
dotnet test src/DotNet.Util.Tests/DotNet.Util.Tests.csproj -c Debug -f net8.0 `
  -p:GenerateAssemblyInfo=false -p:GenerateTargetFrameworkAttribute=false `
  --filter "FullyQualifiedName~SecretUtilTests|FullyQualifiedName~ValidateUtilTests"
# 说明：
# - R9-13 的 RSA 签名测试（SignData_VerifyData_Roundtrip_NonAscii / SignsOverUtf8Bytes_NotAsciiFolded）
#   内部用 RSACryptoServiceProvider/ImportCspBlob，仅 Windows 可跑；非 Windows 平台已在测试内 return 跳过。
# - R9-10 的篡改检测测试依赖 HMAC 校验，新密文格式 Base64(IV[16]‖密文‖HMAC[32]) 与历史无 MAC 密文向后兼容。
# 多 TFM 构建验证（SecretUtil/ValidateUtil 所在程序集）
dotnet build src/DotNet.Util/DotNet.Util.csproj -c Debug -p:TargetFrameworks=net48 -p:GenerateAssemblyInfo=false -p:GenerateTargetFrameworkAttribute=false
dotnet build src/DotNet.Util/DotNet.Util.csproj -c Debug -p:TargetFrameworks=netstandard2.0 -p:GenerateAssemblyInfo=false -p:GenerateTargetFrameworkAttribute=false
```

