# Wangcaisoft.DotNet.Util.Cache

> 缓存工具 / Caching utilities

## 简介 / Introduction

`DotNet.Util.Cache` 在 `DotNet.Util` 之上提供统一的内存缓存与 Redis 缓存封装，业务层可通过一致的 `CacheUtil` 接口读写，并在配置切换时无缝在内存与 Redis 之间迁移。

`DotNet.Util.Cache` builds on `DotNet.Util` to provide unified in-memory and Redis caching. Business code uses a single `CacheUtil` interface and can switch between memory and Redis via configuration.

## 包含内容 / What's Inside

- 内存缓存：基于 `Microsoft.Extensions.Caching.Memory` 的统一缓存接口。
- Redis 缓存：基于 `NewLife.Redis` 的分布式缓存实现。
- 通过 `BaseSystemInfo` 配置缓存类型与 Redis 连接，对外暴露一致的 `CacheUtil` API。

## 安装 / Installation

```bash
dotnet add package Wangcaisoft.DotNet.Util.Cache
```

## 目标框架 / Target Frameworks

net46 · net47 · net48 · net6.0 · net7.0 · net8.0 · net9.0 · net10.0 · netstandard2.0 · netstandard2.1

## 依赖 / Dependencies

- `Wangcaisoft.DotNet.Util`
- [Microsoft.Extensions.Caching.Memory](https://www.nuget.org/packages/Microsoft.Extensions.Caching.Memory)（net6/7 用 `8.0.0`，其余 `9.0.9`）
- [NewLife.Redis](https://www.nuget.org/packages/NewLife.Redis) `6.3.2025.1001`

## 快速使用 / Quick Start

```csharp
using DotNet.Util;

// 内存 / Redis 统一缓存接口（由 BaseSystemInfo 决定后端）
CacheUtil.Set("cacheKey", someObject, expireMinutes: 30);
object value = CacheUtil.Get("cacheKey");
```

> 更多 API 详见各类型随包提供的 XML 文档注释（IntelliSense）。

## 许可证 / License

MIT — ©2008-2026 Wangcaisoft

## 链接 / Links

- 仓库 / Repository: <https://github.com/cuiwenyuan/DotNet.Util>
