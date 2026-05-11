# API 文档与参考（概览）

目标
- 提供生成可维护 API 文档的流程与要点，支持自动化（CI）生成 HTML/Markdown API 参考。

建议工具链
- `DocFX` 或 `Sandcastle`：可用于从 XML 注释生成网站式 API 文档。
- `PublicApiGenerator`：生成 `PUBLIC_API.txt`，用于 API 兼容性检查。
- `Swagger`（仅对 Web/HTTP API 示例有效）。

生成步骤（推荐）
1. 确保在 `*.csproj` 中启用了 `<GenerateDocumentationFile>true</GenerateDocumentationFile>`（已启用）。
2. 使用 `dotnet build` 生成 XML 文档（在 `bin/` 里）。
3. 使用 `DocFX` 或 `PublicApiGenerator` 生成最终文档。

重要命名空间与类型（建议优先出文档）
- `DotNet.Util`：核心工具类（Path/IO/Reflection/Json/XML helper 等）。
- `DotNet.Util.Plus`：Word/Excel 导出相关类（例如 Word 模板导出器）。
- `DotNet.Util.Db` 与子项目：表达式与数据库访问器的核心接口与实现。

文档策略
- 对公共 API（`public`）生成完整参考；对内部实现（`internal`/`private`）保持简要或不生成。
- 在文档中增加使用示例（Snippet），尤其针对导出、数据库访问、路径处理等常见用例。

持续集成
- 在 CI 中添加步骤：构建 -> 生成 XML -> 使用 DocFX -> 将静态站点部署到 GitHub Pages（可选）。
- 在 PR 中展示由 DocFX 生成的变更或将文档部署到临时站点以供审查。

示例 DocFX 配置（高层）
- `docfx.json` 指向 `src/**/bin/**/*.xml` 和源代码以生成文档。

如果需要，我可以：
- 为 `netstandard2.1` 目标生成 `PUBLIC_API.txt` 并提交；
- 添加 `docfx.json` 模板与一个 GitHub Actions 工作流以构建并部署文档。