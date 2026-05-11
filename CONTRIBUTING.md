# Contributing to DotNet.Util

Thank you for your interest in contributing. This document describes how to report issues, propose changes, and submit pull requests for `DotNet.Util`.

Scope
- `DotNet.Util` is a multi-target C# utility library. Contributions may include bug fixes, tests, documentation, examples, and small feature additions.

How to file issues
1. Search existing issues to avoid duplicates.
2. Create a new issue including:
   - A concise title.
   - Reproduction steps or a small code snippet.
   - Target framework and runtime used (e.g., `net6.0`, `net48`, Windows/Linux).
   - Expected and actual behavior.

Reporting bugs
- Provide a minimal reproducible example when possible.
- Include exception stack traces and relevant `*.csproj` target frameworks.

Feature requests
- Explain the use case and provide a small API sketch if applicable.
- Prefer incremental, backward-compatible additions.

Development workflow
1. Fork the repository.
2. Create a branch named with the pattern: `type/short-description` (e.g., `fix/path-normalization`, `feat/word-export-template`).
3. Keep changes small and focused; one logical change per branch/PR.

Coding style
- Target C# idioms and follow existing project style. Use `var` where local type is obvious.
- Keep public APIs stable and avoid breaking changes without a migration plan.
- Add XML documentation for new public types/members when appropriate (`///` comments).

Testing
- Add or update unit tests for bug fixes and new features where practical.
- Ensure tests build across relevant target frameworks when possible.

Commit messages and PRs
- Use clear commit messages. Prefer present-tense summary: `Fix: normalize path separators on Windows`.
- Include a descriptive PR body: motivation, what changed, and any compatibility or migration notes.
- Add a short checklist in PR description:
  - [ ] Build succeeds locally (multi-target where applicable)
  - [ ] Tests added/updated
  - [ ] Documentation updated (if user-facing change)

Review and CI
- The maintainer will review and request changes when needed.
- PRs should be rebased or merged to resolve conflicts before merging.

Legal and license
- By contributing, you agree that your contributions are under the repository's MIT license.

Contact
- For questions, open an issue or mention the maintainer in the PR.

Thank you for helping improve `DotNet.Util`.