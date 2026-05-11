AI Rewrite Task: PathUtil

Objective
- Improve robustness, cross-platform correctness, and test coverage for `PathUtil` wrappers and underlying path/IO helpers.

Scope
- Files: `src/DotNet.Util/NewLife/PathUtil.cs` and the underlying `PathHelper` implementations called by it (where applicable).
- Focus on behavior (not API surface): normalize paths, base/current path resolution, directory creation rules, file read/write helpers, combine path semantics, copy-if-newer semantics, compressed OpenRead/OpenWrite.

Goals
1. Fix edge cases around:
   - UNC paths and Windows device paths
   - Absolute vs. relative inputs on different platforms
   - Trailing directory separators and `isfile` interpretation for `EnsureDirectory`
   - Correct behavior when `null` or empty parts are passed to `CombinePath`
2. Hardening: ensure all public methods validate arguments and document thrown exceptions.
3. Add comprehensive unit tests covering cross-platform scenarios and concurrency where appropriate.
4. Maintain public API stability: do NOT change method names/signatures or return types.

Protected API (do not change)
- All public and extension methods declared in `PathUtil` (e.g., `GetFullPath`, `GetBasePath`, `GetCurrentPath`, `EnsureDirectory`, `CombinePath`, `AsFile`, `ReadBytes`, `WriteBytes`, `CopyToIfNewer`, `OpenRead`, `OpenWrite`, `GetAllFiles`, `CopyTo`, `CopyToIfNewer`, `CopyIfNewer`).

Acceptance criteria
- New or updated unit tests pass on CI (Windows and Linux where applicable).
- Added tests for edge cases listed above.
- No public API signature changes.
- Behavior is documented in XML comments if changed subtly.

Suggested implementation tasks (small, reviewable commits)
1. Add/extend argument validation (null/empty checks) and unit tests.
2. Normalize path handling: implement platform-aware normalization and add tests for UNC, absolute, relative.
3. Clarify `EnsureDirectory` behavior: when path ends with directory separator => treat as directory; when `isfile=true` => ensure parent directory; provide tests that create and cleanup temp dirs/files.
4. Add tests for `CombinePath` with empty/null parts, absolute segments that reset the prefix.
5. Add tests and safe wrappers for `OpenRead`/`OpenWrite` to ensure streams are properly disposed and exceptions are surfaced consistently.
6. Add test for `CopyToIfNewer` covering file timestamps.

Test data and utilities
- Use `System.IO.Path.GetTempPath()` and random temp directory per test.
- Use `File.SetLastWriteTimeUtc` to manipulate file timestamps for copy-if-newer tests.

Example unit tests (see `tests/PathUtil.Tests` project)
- `GetFullPath_ShouldResolveRelativeToBasePath`
- `CombinePath_ShouldHandleNullAndEmptyParts`
- `EnsureDirectory_ShouldCreateDirectory_WhenFilePathProvided`
- `CopyToIfNewer_ShouldCopyOnlyWhenSourceIsNewer`
- `OpenRead_OpenWrite_ShouldWorkWithCompressedFlag`

Notes for AI
- When proposing code changes, provide small patches with context and include unit tests in the same PR.
- Include explicit instructions for how to run the tests and expected outcomes.
- If a change requires modifying behavior that may break consumers, include a migration note and update `API_PROTECTION.md` and `CHANGELOG.md`.

