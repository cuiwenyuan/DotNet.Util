# INSTALL and Build Instructions for DotNet.Util

This document provides steps to set up a development environment and build `DotNet.Util`, which targets multiple frameworks (including .NET Framework and modern .NET).

Prerequisites
- `dotnet` SDK (recommended latest LTS, e.g., .NET 8/9) for building `net6+` and `netstandard` targets.
- For .NET Framework targets (`net452`, `net46`, `net47`, `net48`): Visual Studio with the corresponding Developer Packs installed (at least .NET Framework 4.5.2 Developer Pack as noted in `README.md`).
- Git client.
- Optional: NuGet CLI if you prefer explicit restore commands.

Clone repository

```sh
git clone https://github.com/cuiwenyuan/DotNet.Util.git
cd DotNet.Util
```

Restore dependencies

- Using `dotnet` (recommended for SDK-style projects):

```sh
dotnet restore
```

Build

- Build all projects (multi-targets will be built according to each project file):

```sh
dotnet build -c Release
```

Notes for .NET Framework targets
- Building `net4x` targets requires Windows and Visual Studio/MSBuild with the appropriate targeting packs. If you only have the `dotnet` SDK on a non-Windows system, `net4x` targets will not build.

Running tests
- If test projects exist, run:

```sh
dotnet test -c Release
```

Packing NuGet packages
- Many projects have `GeneratePackageOnBuild` enabled. To create NuGet packages explicitly:

```sh
dotnet pack -c Release
```

- Check `*.csproj` files for `PackageId` and `Version` to know produced package names.

Multi-target compatibility tips
- Some projects use conditional `PackageReference` or `Reference` nodes per `TargetFramework`. Inspect the `*.csproj` to see which packages apply for which frameworks.
- For changes that affect public APIs, consider building and running tests across all target frameworks you intend to support.

Troubleshooting
- Missing .NET Framework targeting errors: install the appropriate Developer Pack from Microsoft matching the target.
- NuGet restore failures: delete `~/.nuget/packages` cache and retry `dotnet restore`.
- If you need to build only a single project:

```sh
dotnet build src/DotNet.Util/ -c Release
```

Publishing
- Publishing is typically done by packaging and pushing the produced `.nupkg` to NuGet. This repository historically publishes under the `Wangcaisoft.*` package ids. Maintain semantic versioning when publishing.

Developer tips
- Use Visual Studio solution if you prefer an IDE for `net4x` debugging.
- Use CI (recommended) to validate multi-target builds; consider GitHub Actions templates that run `dotnet build` and `dotnet test` on Windows and Linux with multiple SDK versions.

If you want, I can add a sample GitHub Actions workflow that builds multi-target frameworks and runs tests automatically.