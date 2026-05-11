Security checklist and recommendations

- Input validation: Validate and sanitize user inputs before using in SQL or commands.
- Database access: Use parameterized queries or an ORM that parameterizes queries for you. Review all uses of string-built SQL.
- Secrets: Do not store connection strings or credentials in source. Use configuration and secret stores (environment variables, Azure Key Vault, GitHub Secrets).
- Dependencies: Keep third-party packages up to date. Run `dotnet list package --outdated` periodically and review breaking changes before upgrades.
- Exception handling: Avoid leaking sensitive information in exception messages returned to clients or logs.
- Logging: Ensure logs do not contain secrets or PII. Centralize logging and use structured logging where feasible.
- Static analysis: Enable security analyzers in CI to detect common weaknesses (e.g., SQL injection, deserialization issues).

Quick hotspots to review (from automated scan)

- `DotNet.Util.Db.DbHelper.Async.cs` — async DB helpers; validate command parameterization.
- `DotNet.Util.HttpUtil.cs` — ensure proper handling of remote data and TLS/HTTPS.
- `DotNet.Util.NewLife.DataUtil.cs` and related utilities — review data parsing and deserialization for input validation.

