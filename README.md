# DotNet.Util
`DotNet.Util` is a collection of C# utility libraries targeting multiple frameworks. The projects provide common helpers (IO, path, reflection, serialization, XML/Word export, database adapters, caching, etc.) intended to be consumed by applications and other libraries.

Supported targets
- Multi-targeted: `net46`, `net47`, `net48`, `net6.0`, `net7.0`, `net8.0`, `net9.0`,  `net10.0`, `netstandard2.0`, `netstandard2.1`.

NuGet packages
- Official packages are published under the `Wangcaisoft.*` namespace on NuGet. Search: https://www.nuget.org/packages?q=wangcaisoft

Quick start
- Clone repository:

  ```sh
  git clone https://github.com/cuiwenyuan/DotNet.Util.git
  cd DotNet.Util
  ```

- Restore and build (recommended):

  ```sh
  dotnet restore
  dotnet build -c Release
  ```

- Notes: building `net4x` targets requires Windows and Visual Studio with the appropriate .NET Framework Developer Packs (minimum .NET Framework 4.6 as noted in project files).

Repository layout (overview)
- `src/DotNet.Util` — core utility library
- `src/DotNet.Util.Plus` — extended utilities (Excel/Word export, compression, etc.)
- `src/DotNet.Util.Db*` — database adapters and expression helpers (MySql/Oracle/PostgreSql/SQLite/OleDb)
- `src/DotNet.Util.Cache` — cache helpers
- `src/DotNet.Business*` — example business projects and web demo
- `src/DotNet.Model` — common models and entities

Documentation and next steps
- This repository includes source XML documentation generation. Key documentation files to add or review: `CONTRIBUTING.md`, `INSTALL.md`, `CHANGELOG.md`, and `API_PROTECTION.md` (present in repo).

Contributing
- See `CONTRIBUTING.md` for contribution workflow, coding style, testing and PR guidance.

License
- MIT — see `LICENSE` in repository root.

Contact
- For issues or feature requests open an Issue on GitHub.

Notes
- The project has a long history and aims for broad compatibility. When making breaking changes avoid modifying public API without an explicit migration plan and version bump.

# DotNet.Util
DotNet.Util is a set of .NET6 + .NET5 + Standard 2.0/2.1 utilities(partial but great majority support .net framework 4.52/4.6/4.7/4.8).  
DotNet.Util是一系列.NET6 + .NET5 + Standard 2.0/2.1组件工具，绝大部分支持.NET Framework 4.52/4.6/4.7/4.8平台，各项目默认支持netstandard2.1/netstandard2.0/net4.8/net4.7/net4.6/net4.52。

## 最低支持4.6
因为VS2026不再支持.NET版本：4.0、4.5、4.5.1，2026年5月11日起最低支持从NET46起。请根据VS2026的提示，自行下载NET46开发包。

最低支持4.5.2版本为2025年10月份的版本。

## Nuget
因为DotNet.被占用，目前NuGet的包以WangCaiSoft.开头。 
https://www.nuget.org/packages?q=wangcaisoft

## 中文简介（简要）
`DotNet.Util` 是一套多目标的 C# 工具库，包含常用的工具函数、文档导出与数据库适配器，支持 .NET Framework 和现代 .NET 平台。更多使用与构建说明请参考 `INSTALL.md` 和 `CONTRIBUTING.md`。


## 背景
组件历史悠久，大概2006年前后[吉日嘎啦](https://www.cnblogs.com/jirigala/)就开始积累这些类库，后来[崔文远](https://www.cuiwenyuan.cn)基于其2014年发布DotNet.Common V4组件继续升级、实战，2020年之后吸收[大石头](http://www.cnblogs.com/nnhy/)主导的[新生命](https://www.newlifex.com)之[NewLife.Core](https://github.com/NewLifeX/X)很多精华，最后呈现了这个开源的代码！

## 初衷
随着.NET5,.NET6,.NET7,.NET8,.NET9,.NET10陆续推出，越来越少的人使用.NET Framework，相信将本源码用于实际项目的会越来越少，所以如果你是互联网企业从业者，可能你要失望了。强烈推荐传统行业、制造业、中小企业，特别是外企（毕竟微软在国外市场很稳固，不像国内那么Java一统天下）、合资公司使用。 

在征得[吉日嘎啦](https://www.cnblogs.com/jirigala/)书面同意后，我将最新的代码开源出来，使用最宽松的MIT协议。正如他所说：就当为社会做一点贡献了！

## 联系方式
我是崔文远，2013年开始把自己弄得一些软件起了个名字：旺财软件。 

个人网站：https://www.cuiwenyuan.cn  
微信：cuiwenyuan1024

## 欢迎勾搭
如果您在使用中有任何问题，请使用GitHub的Issue功能提交给我，也欢迎加我微信沟通。  
