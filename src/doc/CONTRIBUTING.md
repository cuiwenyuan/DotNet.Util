Contribution guidelines

Branching and commits

- Use feature branches named `feature/<short-desc>` or `fix/<short-desc>`.
- Keep commits small and focused. Follow conventional commit messages where possible.

Pull requests

- Open a PR against `Troy` (current working branch) or the appropriate integration branch.
- Include a short description, motivation, and test cases if applicable.
- Ensure the build passes and static analyzers are clean.

Coding standards

- Follow existing repository conventions (naming, folder layout).
- Prefer `async`/`await` returning `Task` instead of `async void`.
- Replace `throw new Exception(...)` with explicit exception types.
- Use parameterized queries for database access.

Testing

- Add unit tests for new features and bug fixes.
- For DB-related code, use integration tests or an in-memory provider/mocks where feasible.

Review

- PRs should be reviewed by at least one maintainer.

Security

- Do not commit secrets or credentials. Use environment variables or secret stores in CI.

License

- Respect project license (if present). If adding new code, ensure license compatibility.
