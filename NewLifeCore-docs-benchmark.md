# DotNet.Util 对标 NewLife.Core · 文档 / README 维度报告

> 范围说明：本对照**仅聚焦文档与 README 维度**，功能/API 完备性不在本报告内（见 `NewLifeCore-benchmark-report.md`）。
> 方法：先盘点本库文档资产，再对照 NewLife.Core 官方文档体系（newlifex.com 文档站、每模块 B 站视频、NuGet 包 README、详尽 XML 注释、团队博客），对"疑似欠缺"项做全仓 grep / 文件核验，保证有证据。

---

## 一、NewLife.Core 文档体系（对标基准，7 个维度）

| 维度 | NewLife.Core 实际做法 | 证据 |
|---|---|---|
| ① 官方文档站 | 独立站点 `newlifex.com`，**每个模块一个深度图文页**（如 `/core/xml`、`/core/packet`、`/core/memory_cache`、`/core/config`） | 搜索命中 5+ 模块页 |
| ② 视频教程 | **每个模块配 B 站视频**（如 `bilibili.com/video/BV1NN4y1P7B8`），图文+视频双轨 | 每个文档页底部均挂视频链接 |
| ③ XML 注释规范 | 源码**每个公共成员必带 `<summary>`，常带 `<example>`/`<remarks>`**；配置类带 `[DisplayName]`/`[Description]` 特性 → IntelliSense 极友好 | `/core/config` 页展示 `Setting` 类注释+特性 |
| ④ NuGet 包 README | 每个发布包在 NuGet.org 页面展示**功能介绍 + 用法** | NuGet 包页 |
| ⑤ 团队博客 | cnblogs「新生命团队」深度原理文（性能数据、设计演进） | `/core/memory_cache` 含 2.87 亿 tps 实测 |
| ⑥ 社区 | QQ 群 / Issue / 微信群，作者直连 | 文档页与仓库 |
| ⑦ 文档页结构 | 每页含**基本用法 / 最佳实践 / 性能数据 / 源码链接**固定结构 | 各 `/core/*` 页 |

---

## 二、本库文档资产现状（已核验）

| 资产 | 现状 | 备注 |
|---|---|---|
| 根 README.md | ✅ 中英双语，4.4KB | 结构割裂：英文 API 概览段 + 中文"背景/初衷/联系作者"段，功能 API 介绍偏弱 |
| 子项目 README | ✅ **14 个**主项目均带 README（Tests 无，合理） | 质量较高，含 What's Inside / Quick Start / 依赖 / TFM（见 `DotNet.Util`/`DotNet.Util.Db` README 样例） |
| CHANGELOG.md | ⚠️ **空模板占位** | 只有 `Unreleased` + `vX.Y.Z` 占位，无真实版本记录 |
| CONTRIBUTING / INSTALL / LICENSE | ✅ 齐全 | — |
| 架构/设计文档 | ✅ `src/doc/` 含 PROJECT_ARCHITECTURE / MODULE_DESIGN / CODING_GUIDELINES + 2 份数据字典 | NewLife GitHub 内反而没这么系统，本库亮点 |
| XML 文档生成 | ✅ 10 个项目 `GenerateDocumentationFile=true` | 随包出 `.xml` 供 IntelliSense |
| XML 注释量 | ✅ 全库 9034 行 `///`；**公共类型注释覆盖率 86.9%**（145 类型中 126 带 `///`） | 类型级不错；方法级规范待查 |
| docs 站点 | ❌ 无 `docfx.json` / `mkdocs.yml`，无 `docs/` | 仅有 GitHub 内 md |
| NuGet 包内嵌 README | ❌ 无 `PackageReadmeFile` | 用户 `dotnet add` 后 NuGet 页面看不到功能介绍 |
| 视频教程 | ❌ 零 | — |
| 独立 sample/demo 项目 | ❌ 无（DotNet.Business* 是示例业务，非教学 sample） | — |

---

## 三、对标欠缺清单（文档维度）

### 🔴 完全缺失（最大短板）
| # | 欠缺项 | 影响 | 修复方向 | 成本 |
|---|---|---|---|---|
| 1 | **NuGet 包内嵌 README**（`PackageReadmeFile` 缺失） | 用户装包后 NuGet 页面无功能介绍，第一印象差；目前仅能看到 XML 注释碎片 | 14 个可打包 csproj 加 `<PackageReadmeFile>README.md</PackageReadmeFile>` + `<None Include="README.md" Pack="true" PackagePath="\"/>` | **极低**（每文件+2 行，复用现有 README） |
| 2 | **官方文档站 / Web Docs** | 无集中、可检索、带导航的文档；用户只能翻 GitHub | `mkdocs` 串起现有 README + `src/doc/`，GitHub Pages 托管（零成本）或 docfx 出 API 参考 | 低 |
| 3 | **视频教程体系** | 无"上手"视频，新用户门槛高 | 对标 NewLife 每模块 1 条 B 站短讯（可后续，非阻塞） | 中（需录制） |

### 🟠 明显偏弱
| # | 欠缺项 | 影响 | 修复方向 |
|---|---|---|---|
| 4 | **CHANGELOG 是空模板** | 消费者无法从 changelog 判断破坏性变更 / 升级风险 | 至少回填真实版本基线（从某 release 起记录 Added/Changed/Fixed/Security） |
| 5 | **XML 注释规范性** | 类型级 86.9% 尚可，但**方法级覆盖率偏低、缺 `<example>`/`<remarks>`、配置类缺 `[DisplayName]`/`[Description]` 特性**（对比 NewLife 每个成员必带 summary+example） | 定《XML 注释规范》：公共 API 必带 `<summary>`，关键方法补 `<example>`；可加 CI 检查 + 用 docfx 生成 API 参考倒逼补全 |
| 6 | **每模块"深度图文教程页"** | 本库只有概览 README，缺 NewLife `/core/xml` 那种"基本用法+最佳实践+性能数据"的教学页 | 在 docs 站为每个核心模块补 how-to 页 |

### 🟡 部分覆盖 / 本库反而有优势（无需补）
- **架构与编码规范文档**（`src/doc/`）——本库比 NewLife GitHub 内更系统，保持即可。
- **子项目 README 一致性**——14 个项目都较规范，是亮点，保持。
- **中英双语**——本库 README 中英对照，NewLife 主要中文；本库这点是加分项。

---

## 四、最高 ROI 文档改进建议（按成本排序）

1. **【优先级最高·成本极低】NuGet 包内嵌 README** —— 直接提升 14 个包在 NuGet.org 的专业度，复用现有 README，每 csproj +2 行。建议本轮就做。
2. **【低成本】回填 CHANGELOG** —— 把真实版本演进补上（至少从 net452 移除 / CVE 修复 / IOC 之前基线起记）。
3. **【低成本】起一个 mkdocs 文档站** —— 把现有 README + `src/doc/` 串成可检索站点，GitHub Pages 零托管成本。
4. **【中成本·可持续】XML 注释规范 + docfx API 参考** —— 定规范、补方法级 `<example>`，用 docfx 自动出 API 文档，CI 校验公共 API 注释率。
5. **【后续】视频教程** —— 每模块 1 条短讯，非阻塞。

---

## 五、与功能维度报告的关系
- 功能维度短板（IOC/调度/网络/RPC/MQ/脚本）见 `NewLifeCore-benchmark-report.md`。
- 文档维度与功能维度**正交**：即便功能不补，文档（尤其 NuGet README + CHANGELOG + docs 站）可独立提升本库专业度与可发现性，且不引入任何代码风险，适合作为低风险高收益的下一项。

---

_报告生成：2026-08-25 · 未提交（遵循"确认后执行"工作流）。_
