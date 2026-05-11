Build instructions

Requirements

- .NET SDKs corresponding to the targeted TFMs in the solution. The repository contains projects targeting legacy .NET Framework versions (4.5.2..4.8) and modern .NET (6..9). Use Visual Studio 2022/2023 or appropriate .NET SDK installations to build Framework and SDK-style projects.

Local build (recommended)

1. Open the solution in Visual Studio and build (recommended for .NET Framework projects).
2. Or run from a developer command prompt for the solution directory:

   - For SDK-style projects (dotnet CLI enabled): `dotnet build`.
   - For older .NET Framework projects, use Visual Studio or MSBuild on Windows: `msbuild /p:Configuration=Release YourSolution.sln`.

CI suggestions

- Provide separate build pipelines for legacy .NET Framework projects (Windows agents) and modern .NET SDK projects (cross-platform agents).
- Run static analyzers, unit tests, and packaging steps in CI.

Notes

Because the codebase spans multiple frameworks, ensure the build environment has required SDKs and toolsets for the Framework versions you need to target.
