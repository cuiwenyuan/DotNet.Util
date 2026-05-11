This file lists findings from a quick automated scan (searching for TODO/FIXME/exception patterns). Treat this as pointers for manual inspection.

Files flagged (examples)

- `DotNet.Util.Plus\QqwryUtil.cs` — contains `TODO` markers; inspect implementation completeness.
- `DotNet.Util\Message\AppMessage.Message.cs` — contains `TODO` comments and large generated/message handling code; review for correctness and performance.
- `DotNet.Test.452\Program.cs` — demo/test program with `TODO` blocks; keep test/demo code out of library artifacts.
- `DotNet.Util\Util\StringUtil.cs` — contains commented TODOs; review string edge cases.
- `DotNet.Util\BaseSystemInfo\BaseSystemInfo.Client.cs` — contains TODO markers.
- `DotNet.Util.Db.OleDb\OleDbHelper.cs` — has TODO comment near provider initialization; review provider-specific behavior.
- `DotNet.Util\Util\HttpUtil.cs` — large method sections with TODOs; review error handling and timeout behavior.
- `DotNet.Util.Db.DbHelper.Async.cs` — async helpers; review `async void` usage and proper Task-returning signatures.
- `DotNet.Business\BaseException\BaseExceptionManager.Manual.cs` — manual exception manager has TODOs.

Common patterns to inspect

- `TODO`/`FIXME` comments — indicate incomplete work or known issues.
- `throw new Exception(...)` — replace with specific exception types.
- `async void` — change to `async Task` where applicable.
- Console or demo code inside library projects.

Next actions

- Create issues for each TODO found and assign owners.
- Prioritize DB parameterization and async correctness.
- Add unit tests around repaired hotspots.

