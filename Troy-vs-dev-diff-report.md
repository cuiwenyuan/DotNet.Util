# Troy 分支 vs dev 分支 差异与合并可行性报告

> 生成时间：2026-08-25 ｜ 合并基线 `b7294bc` ｜ 比对：`origin/dev...Troy`  
> 关注点：能否自动合并 + 是否影响 .NET Framework（net4x）老用户

## 一、能否自动合并：✅ 可以，零冲突

| 检查项                                               | 结果                                         |
| ------------------------------------------------- | ------------------------------------------ |
| 合并基线                                              | `b7294bc1974b5268831eb3ffe997d7b238ce71f8` |
| `origin/dev` 是否有 Troy 没有的提交                       | **无**（`Troy...origin/dev` 反向 diff 为空）      |
| 试合并 `git merge-tree --write-tree origin/dev Troy` | **EXIT=0**，输出 tree `7aa60705...`           |
| 冲突文件                                              | **0**                                      |

**结论**：`origin/dev` 是 Troy 的真子集，Troy 合入 dev 为 **fast-forward**，不存在冲突风险，可安全自动合并。

## 二、改动规模

| 类型                              | 数量                      |
| ------------------------------- | ----------------------- |
| C# 文件                           | 497                     |
| 文档（md/html）                     | 29                      |
| SQL/脚本/其他（sql/ps1/yml/json/png） | 5                       |
| 合计                              | 552 文件，+27212 / -6158 行 |

## 三、.NET Framework 兼容性评估（你最担心的点）

### 3.1 编译级风险：✅ 未发现

对 Troy 新增/修改的 .cs 行扫描 net4x 不支持的 API：

- `IndexOf(';')` / `LastIndexOf('@')` 等 `char` 重载 → net4x 自 .NET 1.1 起即存在，**安全**。
- `ArgumentNullException.ThrowIfNull` → 全仓仅出现在**注释**（`#pragma` 说明文字），真实代码用的是 `throw new ArgumentNullException(...)`，**安全**。
- `Span<` / `HashCode.Combine` / `string.Create` / `StringSplitOptions.TrimEntries` / `DateTime.UnixEpoch` / `Enumerable.Chunk` / `SearchValues` / `JsonNode` → **均未出现**。
- 新增 `#if` 条件编译行 **131 行**（框架分支逻辑变多），需真实 net4x 构建验证（见第六节）。

### 3.2 加密兼容性：✅ 安全（老密文可解开）

- `BaseSystemInfo.SecurityKey` 默认值 **未变**（`"DotNet.Troy.Cui.2018"`）。
- `SecretUtil`：
  - **新增** `AesEncrypt` / `AesDecrypt`（AES-256-CBC，随机 IV 前置，SHA256(key)→32 字节）—— 纯增量，不替换旧方法。
  - `DesEncrypt` / `DesDecrypt` **保留**（仅修了一处空 key 的越界 Bug：`key 为空→回退 SecurityKey`，算法本体 `Sha1(Md5(key).Substring(0,8))` 未变）。
  - 因此历史 DES/AES 密文**向后兼容**，老用户数据可正常解密。

### 3.3 ⚠️ 真实兼容性影响 ①：net452 TFM 被全仓移除

13 个 `.csproj` 的 `<TargetFrameworks>` 由 `net452;net46;net47;net48;...` 改为 `net46;net47;net48;...;net10.0`。

- **影响**：仍运行在 **.NET Framework 4.5.2** 的消费者将无法引用新包。
- **4.6 / 4.7 / 4.8 仍支持**，不受影响。
- 这是有意决策，但请确认没有 4.5.2 老用户依赖。

### 3.4 ⚠️ 真实兼容性影响 ②：依赖主版本跳跃

| 包                                   | dev            | Troy                        | 风险                         |
| ----------------------------------- | -------------- | --------------------------- | -------------------------- |
| MySql.Data                          | 9.4.0          | **26.7.0**                  | 大版本跳跃（9→26），MySQL 消费者需回归测试 |
| NewLife.Core                        | 11.7.2025.1001 | 11.18.2026.801              | 跨年升级，建议回归                  |
| NewLife.Redis                       | 6.3.2025.1001  | 6.6.2026.801                | 同上                         |
| JWT                                 | 11.0.0         | 11.1.0                      | 次版本，低风险                    |
| System.ValueTuple                   | 4.6.1          | 4.6.2                       | 补丁，低风险                     |
| Microsoft.Extensions.Caching.Memory | 9.0.9 / 8.0.0  | 经 `$(ExtensionsVersion)` 统一 | 含 CVE 修复（8.0.0→8.0.1）      |

> 注：`Microsoft.Extensions.Caching.Memory` 现由 `<ExtensionsVersion>` 条件引用（net6/7→8.0.1，net8+→10.0.11），即前面修掉的 CVE-2024-43483。

## 四、改动较大的核心文件（行为变更需关注）

| 文件                      | 改动   | 关注点                  |
| ----------------------- | ---- | -------------------- |
| `Util/SecretUtil.cs`    | +291 | 见 3.2，向后兼容           |
| `Util/XmlConfigUtil.cs` | +440 | 配置解析逻辑，建议回归配置读写      |
| `Util/RequestUtil.cs`   | ±198 | Web 请求处理，注意请求/响应解析行为 |
| `Util/ThumbnailUtil.cs` | +238 | 缩略图生成                |
| `Util/Utils.cs`         | +249 | 通用工具，覆盖面广            |
| `Util/ValidateUtil.cs`  | +158 | 校验规则，可能影响入参校验结果      |
| `Util/RmbUtil.cs`       | +90  | 人民币大写转换              |
| `Util/StringUtil.cs`    | +94  | 字符串处理                |
| `Util/RandomUtil.cs`    | +71  | 随机值生成                |

## 五、合并前建议清理的杂质

- ⚠️ **`tmp_toint_test/`**（2 文件）：临时验证项目被误提交进 Troy，不在 `.sln` 内，但会污染仓库。建议合并前删除或加 `.gitignore`。
- 生成的文档/HTML/SQL/`replace-string-isnullorempty3.ps1`：非代码风险，按需保留。

## 六、合并后验证建议（net4x 无法在本环境构建，需 Windows 实测）

1. **net4x 构建**：在 Windows 上 `dotnet build -f net48`（及 net46/net47）确认 131 处 `#if` 分支编译通过。
2. **逻辑回归**：跑现有测试（net8 下 1041 用例已绿），覆盖 Http/XML/加密/校验等核心逻辑。
3. **MySQL 回归**：若有 MySQL 消费者，用 `MySql.Data 26.7.0` 实测连通与读写。
4. **4.5.2 消费者**：确认无外部项目锁定 net452。

## 七、建议合并命令（fast-forward，无冲突）

```bash
git checkout dev
git merge Troy            # fast-forward
git push origin dev
# 之后按你的流程 dev -> master，再在 master 打 v* tag 触发发版
```

---

*本报告仅做差异分析，未执行任何 git 合并/提交操作。*

