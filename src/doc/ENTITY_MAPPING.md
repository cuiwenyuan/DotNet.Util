# 数据表 -> 实体类 映射（摘要）

以下为 `scripts/UserCenter.SQLServer.2008R2.sql` 中常用表与仓库中 `DotNet.Model` 项目对应的实体类文件（按命名约定匹配）：

- `BaseUser` -> `DotNet.Model\BaseUserEntity.Auto.cs` (或 `DotNet.Model\BaseUserEntity.cs`)（仓库文件：`DotNet.Model\BaseUserEntity.Auto.cs`）
- `BaseUserLogon` -> `DotNet.Model\BaseUserLogonEntity.Auto.cs`
- `BaseUserContact` -> `DotNet.Model\BaseUserContactEntity.Auto.cs`
- `BaseUserOAuth` -> `DotNet.Model\BaseUserOAuthEntity.Auto.cs`
- `BaseUserOrganization` -> `DotNet.Model\BaseUserOrganizationEntity.Auto.cs`
- `BaseUserRole` -> `DotNet.Model\BaseUserRoleEntity.Auto.cs`
- `BaseRole` -> `DotNet.Model\BaseRoleEntity.Auto.cs`
- `BaseRoleOrganization` -> `DotNet.Model\BaseRoleOrganizationEntity.Auto.cs`
- `BaseOrganization` -> `DotNet.Model\BaseOrganizationEntity.Auto.cs`
- `BaseOrganizationScope` -> `DotNet.Model\BaseOrganizationScopeEntity.Auto.cs`
- `BaseModule` -> `DotNet.Model\BaseModuleEntity.Auto.cs`
- `BasePermission` -> `DotNet.Model\BasePermissionEntity.Auto.cs`
- `BasePermissionScope` -> `DotNet.Model\BasePermissionScopeEntity.Auto.cs`
- `BaseSequence` -> `DotNet.Model\BaseSequenceEntity.Auto.cs`
- `BaseParameter` -> `DotNet.Model\BaseParameterEntity.Auto.cs`
- `BaseStaff` -> `DotNet.Model\BaseStaffEntity.Auto.cs`
- `BaseUploadLog` -> `DotNet.Model\BaseUploadLogEntity.Auto.cs`
- `BaseMessageQueue` -> `DotNet.Model\BaseMessageQueueEntity.Auto.cs`
- `BaseMessageSucceed` -> `DotNet.Model\BaseMessageSucceedEntity.Auto.cs`
- `BaseMessageFailed` -> `DotNet.Model\BaseMessageFailedEntity.Auto.cs`
- `BaseLogonLog` -> `DotNet.Model\BaseLogonLogEntity.Auto.cs`
- `BaseOperationLog` -> `DotNet.Model\BaseOperationLogEntity.Auto.cs`
- `BaseModule` (业务版) -> `DotNet.Model\BaseModuleEntity.Auto.cs`
- `BusinessRole` -> `DotNet.Model\BaseRoleEntity.Auto.cs`（注意：仓库中以 `BaseRole` 命名，业务表有 `BusinessRole`，字段结构相似）
- `BusinessModule` -> `DotNet.Model\BaseModuleEntity.Auto.cs`
- `BusinessPermission` / `BusinessPermissionScope` -> `DotNet.Model\BasePermissionEntity.Auto.cs` / `BasePermissionScopeEntity.Auto.cs`
- `BusinessUserRole` -> `DotNet.Model\BaseUserRoleEntity.Auto.cs`

注意：仓库中实体类名称以 `Base*Entity` 命名并带有 `.Auto.cs`（自动生成）后缀，数据库有 `Base` 与 `Business` 两套前缀同类型表。映射原则为：表名（去前缀 Business/ Base）与实体名对应，或手动比较字段差异以确认。

建议后续动作：
- 运行脚本逐表比对 SQL 列与实体类属性（可自动化脚本），并生成差异报告。
- 如需要我可以生成该差异报告并提交到 `doc/`。
