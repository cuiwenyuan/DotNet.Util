# UserCenter 数据字典（摘要）

说明：本文件根据 `scripts/UserCenter.SQLServer.2008R2.sql` 提取并整理。列出主要表的字段、类型、是否可空与注释（若 SQL 中有 extended properties）。这是摘要版，包含常用表与关键字段；需要完整逐表导出可再生成。

---

## 表：`BaseUser`
说明：用户账号
- 主键：`Id` (int, NOT NULL) — 用户主键
- `UserName` (nvarchar(50), NOT NULL) — 用户名
- `RealName` (nvarchar(50), NULL) — 姓名
- `CompanyId` (int, NOT NULL) — 公司主键
- `DepartmentId` (int, NOT NULL) — 部门主键
- `IsAdministrator` (tinyint, NOT NULL) — 是否系统管理员
- `CreateTime` (datetime, NOT NULL) — 创建时间
- 常用索引：`CreateTime`, `CreateUserId`, `Deleted`, `Enabled`, `SortCode`, `UpdateTime`, `UpdateUserId`

备注：表有大量用户属性（头像、工号、身份证、地理信息等），并使用 `TEXTIMAGE_ON` 存放大文本字段（`Description`）。

---

## 表：`BaseUserLogon`
说明：用户登录信息
- 主键：`Id` (int, NOT NULL)
- `UserId` (int, NOT NULL) — 关联 `BaseUser.Id`
- `UserPassword` (nvarchar(100), NULL)
- `OpenId` (nvarchar(50), NOT NULL) — 单点登录标识
- `LastVisitTime` (datetime, NULL)
- `PasswordErrorCount` (int, NOT NULL)
- `NeedModifyPassword` (tinyint, NOT NULL)
- `CreateTime`, `CreateUserId`, `Deleted`, `Enabled`

索引：`CreateTime`, `CreateUserId`, `Deleted`, `Enabled`, `SortCode`, `UpdateTime`, `UpdateUserId`

---

## 表：`BaseUserContact`
说明：用户联系方式
- 主键：`Id`
- `UserId` (int, NOT NULL) — 关联 `BaseUser.Id`
- `Email` (nvarchar(100), NULL)
- `Mobile` (nvarchar(50), NULL)
- `WeChatOpenId` (nvarchar(50), NULL)
- 验证状态字段：`EmailValidated`, `MobileValidated`, `WeChatValidated`

---

## 表：`BaseRole`
说明：角色
- 主键：`Id`
- `OrganizationId` (int, NOT NULL)
- `Code` (nvarchar(100), NULL)
- `Name` (nvarchar(200), NULL)
- `AllowEdit`, `AllowDelete`, `IsVisible` (tinyint)
- `CreateTime`, `CreateUserId` 等通用审计字段

---

## 表：`BaseUserRole`
说明：用户-角色关系
- 主键：`Id`
- `UserId` (int, NOT NULL) — 关联 `BaseUser.Id`
- `RoleId` (int, NOT NULL) — 关联 `BaseRole.Id`
- `SystemCode` (nvarchar(50), NOT NULL)

索引及审计字段同常见模式。

---

## 表：`BaseOrganization`
说明：组织机构
- 主键：`Id`
- `ParentId` (int, NOT NULL) — 父结点引用
- `Name` (nvarchar(50), NOT NULL)
- `CompanyId` (int, NOT NULL) — 所属公司
- 地址、电话、经纬度、层级 `Layer` 等字段

---

## 表：`BaseModule` / `BusinessModule`
说明：模块/菜单定义（BaseModule 为核心）
- 主键：`Id`
- `ParentId` (int), `Code`, `Name`, `IsMenu` (tinyint)
- `PermissionScopeTables`（需要权限过滤的表）为 `nvarchar(max)` 或 `nvarchar(500)`

---

## 表：权限相关（摘要）
- `BasePermission` / `BusinessPermission`：资源权限记录，字段包括 `ResourceCategory`, `ResourceId`, `PermissionId`, `CompanyId` 等。
- `BasePermissionScope` / `BusinessPermissionScope`：权限范围配置，支持生效时间、包含子节点标记等。
- `BaseOrganizationScope` / `BusinessOrganizationScope`: 基于组织的权限范围细化。

---

## 常见审计字段与约定
- 审计字段：`CreateTime`, `CreateUserId`, `CreateUserName`, `CreateBy`, `CreateIp`, `UpdateTime`, `UpdateUserId`, `UpdateUserName`, `UpdateBy`, `UpdateIp`
- 软删除：`Deleted` (tinyint)
- 有效标记：`Enabled` (tinyint)
- 排序：`SortCode` (int)

---

如果需要我可以：
- 生成完整逐表的数据字典（包含每列的类型、NULL、默认、注释），或
- 将所有表自动映射到模型类并检查字段差异。

下一步建议：确认是否需要全量导出（所有表）以及是否把数据字典拆成每个表单独文件。
