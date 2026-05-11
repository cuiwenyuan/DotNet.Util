# 生成全量逐表数据字典说明

本项目包含 `scripts/UserCenter.SQLServer.2008R2.sql`，提供了完整的数据库建表脚本及列注释（extended properties）。已添加一个生成脚本用于从该 SQL 脚本自动生成每个表的 Markdown 数据字典。

生成脚本：
- `src/tools/generate-data-dictionary.ps1`

默认行为：
- 从 `..\scripts\UserCenter.SQLServer.2008R2.sql` 读取脚本（相对于 `src/tools`）
- 解析 `CREATE TABLE [schema].[Table] ( ... ) ON ...` 块
- 收集 `EXEC sys.sp_addextendedproperty` 中的 `MS_Description` 值作为表或列的注释
- 在输出目录（默认 `..\doc\tables`）内为每个表生成 `TableName.md`，包含列清单、类型、NULL、额外说明与注释
- 同目录下生成 `DATA_DICTIONARY_GENERATED_INDEX.md` 列出所有表链接

如何运行（在 `src/tools` 目录或使用相对路径）示例：

PowerShell:

    cd src/tools
    .\generate-data-dictionary.ps1 -SqlPath ..\scripts\UserCenter.SQLServer.2008R2.sql -OutDir ..\doc\tables

输出位置：
- `src/doc/tables/` 下的每表 Markdown 文件
- `src/doc/DATA_DICTIONARY_FULL_GENERATION.md`（本文件）

注意与限制：
- 脚本使用简单的正则解析 CREATE TABLE，适用于本 SQL 脚本常见的格式。如脚本中存在非常规换行或复杂内嵌括号，可能需要微调正则。
- 默认只解析列定义与 extended property 注释；不会解析完整的约束（例如外键约束引用），也不会自动生成 ER 图（可扩展脚本实现）。

如需，我可以运行该脚本并把生成的每表 Markdown 文件加入仓库；但在 CI/远程环境下运行脚本前请先确认是否允许我执行终端命令。
