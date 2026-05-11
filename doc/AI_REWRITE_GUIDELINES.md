# AI 驱动重写指南（AI Rewrite Guidelines）

目的
- 为使用 AI 协助重写或重构本仓库代码提供明确的约束与流程，保证可回溯、可审查、对外兼容。

守则（必须遵守）
1. 保持公共 API 稳定：任何对 `public`/`protected` 的签名或行为变动，必须在 PR 中清晰说明并在 `PUBLIC_API.txt` 中更新。
2. 小步提交：每次 AI 变更应聚焦单一逻辑点，产生可审查的差异（diff）。
3. 测试覆盖：AI 所作修改应尽量配套添加/更新单元测试。
4. 人工复核：重要模块的 AI 提交必须由维护者或有经验的开发者审核后合并。
5. 语义版本控制：若变更破坏向后兼容，应提升主版本并提供迁移说明。

AI 使用建议
- Prompt 设计：给 AI 明确任务范围、不可变更的公有 API 列表与测试要求。
- 代码上下文：在调用 AI 前，收集并提供受影响文件的相关上下文（引用、单元测试、接口文档）。
- 输出格式：要求 AI 仅输出补丁/变更的可应用补丁（如 git patch）或直接修改仓库文件（如本工具的 apply_patch）。

质量检查（自动）
- 构建：`dotnet build -c Release`（多目标矩阵）
- 测试：`dotnet test`（相关测试项目）
- API 校验：生成实际 `PUBLIC_API` 并与仓库中提交的 `PUBLIC_API.txt` 对比
- 静态分析：可选 Roslyn 分析器或 `dotnet format`/`dotnet analyzers` 检查风格与潜在问题

回滚策略
- 每次 AI 提交应关联明确的 issue/PR；若发现问题，优先 revert PR，并记录回滚原因在 `CHANGELOG` 或 PR 评论中。

示例流程（AI 参与的修复）
1. 打开 Issue，描述问题与目标。
2. 维护者创建分支并在分支上运行 AI（或调用自动化工具）。
3. AI 生成代码变更并附带单元测试。
4. CI 运行构建/测试/API 校验。
5. 人工审查变更，合并并发布（如合格）。

工具推荐
- `PublicApiGenerator`（生成 API 列表）
- `dotnet-format`（代码格式化）
- `nunit/xunit/ MSTest`（单元测试）

附录：AI prompt 范例（简体中文）

- 任务：修复 `PathUtil.Normalize` 在 Windows 下对 UNC 路径处理错误，确保通过以下单元测试（附加测试）并保持对外 API 不变。要求输出可直接应用的补丁。

---

我可以基于该指南帮助：生成 `PUBLIC_API.txt`、在 CI 中添加 API 校验、或为特定模块（如路径处理、Word 导出、DB 适配）生成 AI 重写任务模版。