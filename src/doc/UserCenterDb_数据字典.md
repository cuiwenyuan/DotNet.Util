# UserCenterDb 数据字典

本文档描述了 UserCenterDb 数据库中各表的结构信息。

## BaseCalendar

**表描述**: 日历

| 序号 | 字段名 | 小写字段名 | 字段描述 | 类型 | 长度 | 小数 | 默认值 | 主键 | 自增 | 允许空 |
|---|---|---|---|---|---|---|---|---|---|---|
| 1 | Id | id | 编号 | int | 10 |  |  | Y | Y | N |
| 2 | UserCompanyId | userCompanyId | 公司编号 | int | 10 |  | 0 | N | N | N |
| 3 | UserSubCompanyId | userSubCompanyId | 子公司编号 | int | 10 |  | 0 | N | N | N |
| 4 | FiscalYear | fiscalYear | 年度 | smallint | 5 |  |  | N | N | Y |
| 5 | FiscalMonth | fiscalMonth | 月份 | tinyint | 3 |  |  | N | N | Y |
| 6 | FiscalDay | fiscalDay | 日 | tinyint | 3 |  |  | N | N | Y |
| 7 | TransactionDate | transactionDate | 操作日期 | date | 10 |  | getdate() | N | N | Y |
| 8 | SortCode | sortCode | 排序编号 | int | 10 |  | 0 | N | N | N |
| 9 | Deleted | deleted | 是否删除 | tinyint | 3 |  | 0 | N | N | N |
| 10 | Enabled | enabled | 是否有效 | tinyint | 3 |  | 1 | N | N | N |
| 11 | CreateTime | createTime | 创建时间 | datetime | 23 |  | getdate() | N | N | N |
| 12 | CreateUserId | createUserId | 创建人编号 | int | 10 |  | 0 | N | N | N |
| 13 | CreateUserName | createUserName | 创建人用户名 | nvarchar | 50 |  |  | N | N | Y |
| 14 | CreateBy | createBy | 创建人姓名 | nvarchar | 50 |  |  | N | N | Y |
| 15 | CreateIp | createIp | 创建IP | nvarchar | 50 |  |  | N | N | Y |
| 16 | UpdateTime | updateTime | 修改时间 | datetime | 23 |  | getdate() | N | N | N |
| 17 | UpdateUserId | updateUserId | 修改人编号 | int | 10 |  | 0 | N | N | N |
| 18 | UpdateUserName | updateUserName | 修改人用户名 | nvarchar | 50 |  |  | N | N | Y |
| 19 | UpdateBy | updateBy | 修改人姓名 | nvarchar | 50 |  |  | N | N | Y |
| 20 | UpdateIp | updateIp | 修改IP | nvarchar | 50 |  |  | N | N | Y |

## BaseChangeLog

**表描述**: 变更日志

| 序号 | 字段名 | 小写字段名 | 字段描述 | 类型 | 长度 | 小数 | 默认值 | 主键 | 自增 | 允许空 |
|---|---|---|---|---|---|---|---|---|---|---|
| 1 | Id | id | 主键 | int | 10 |  |  | Y | Y | N |
| 2 | SystemCode | systemCode | 子系统编码 | nvarchar | 50 |  | Base | N | N | N |
| 3 | TableName | tableName | 表名 | nvarchar | 50 |  |  | N | N | Y |
| 4 | TableDescription | tableDescription | 表备注 | nvarchar | 200 |  |  | N | N | Y |
| 5 | ColumnName | columnName | 列名 | nvarchar | 50 |  |  | N | N | Y |
| 6 | ColumnDescription | columnDescription | 列备注 | nvarchar | 200 |  |  | N | N | Y |
| 7 | RecordKey | recordKey | 记录主键 | nvarchar | 50 |  |  | N | N | Y |
| 8 | OldKey | oldKey | 原值主键 | nvarchar | 50 |  |  | N | N | Y |
| 9 | OldValue | oldValue | 原值 | nvarchar | 200 |  |  | N | N | Y |
| 10 | NewKey | newKey | 现值主键 | nvarchar | 50 |  |  | N | N | Y |
| 11 | NewValue | newValue | 现值 | nvarchar | 200 |  |  | N | N | Y |
| 12 | SortCode | sortCode | 排序编号 | int | 10 |  | 0 | N | N | N |
| 13 | Deleted | deleted | 是否删除 | tinyint | 3 |  | 0 | N | N | N |
| 14 | Enabled | enabled | 是否有效 | tinyint | 3 |  | 1 | N | N | N |
| 15 | CreateTime | createTime | 创建时间 | datetime | 23 |  | getdate() | N | N | N |
| 16 | CreateUserId | createUserId | 创建人编号 | int | 10 |  | 0 | N | N | N |
| 17 | CreateUserName | createUserName | 创建人用户名 | nvarchar | 50 |  |  | N | N | Y |
| 18 | CreateBy | createBy | 创建人姓名 | nvarchar | 50 |  |  | N | N | Y |
| 19 | CreateIp | createIp | 创建IP | nvarchar | 50 |  |  | N | N | Y |
| 20 | UpdateTime | updateTime | 修改时间 | datetime | 23 |  | getdate() | N | N | N |
| 21 | UpdateUserId | updateUserId | 修改人编号 | int | 10 |  | 0 | N | N | N |
| 22 | UpdateUserName | updateUserName | 修改人用户名 | nvarchar | 50 |  |  | N | N | Y |
| 23 | UpdateBy | updateBy | 修改人姓名 | nvarchar | 50 |  |  | N | N | Y |
| 24 | UpdateIp | updateIp | 修改IP | nvarchar | 50 |  |  | N | N | Y |

## BaseDictionary

**表描述**: 字典

| 序号 | 字段名 | 小写字段名 | 字段描述 | 类型 | 长度 | 小数 | 默认值 | 主键 | 自增 | 允许空 |
|---|---|---|---|---|---|---|---|---|---|---|
| 1 | Id | id | 编号 | int | 10 |  |  | Y | Y | N |
| 2 | TenantId | tenantId | 租户号 | int | 10 |  | 0 | N | N | N |
| 3 | Code | code | 编码 | nvarchar | 50 |  |  | N | N | N |
| 4 | Name | name | 名称 | nvarchar | 100 |  |  | N | N | N |
| 5 | IsTree | isTree | 树型结构 | tinyint | 3 |  | 0 | N | N | N |
| 6 | AllowEdit | allowEdit | 允许编辑 | tinyint | 3 |  | 1 | N | N | N |
| 7 | AllowDelete | allowDelete | 允许删除 | tinyint | 3 |  | 1 | N | N | N |
| 8 | Description | description | 描述 | nvarchar | 4000 |  |  | N | N | Y |
| 9 | SortCode | sortCode | 排序编号 | int | 10 |  | 0 | N | N | N |
| 10 | Deleted | deleted | 是否删除 | tinyint | 3 |  | 0 | N | N | N |
| 11 | Enabled | enabled | 是否有效 | tinyint | 3 |  | 1 | N | N | N |
| 12 | CreateTime | createTime | 创建时间 | datetime | 23 |  | getdate() | N | N | N |
| 13 | CreateUserId | createUserId | 创建人编号 | int | 10 |  | 0 | N | N | N |
| 14 | CreateUserName | createUserName | 创建人用户名 | nvarchar | 50 |  |  | N | N | Y |
| 15 | CreateBy | createBy | 创建人姓名 | nvarchar | 50 |  |  | N | N | Y |
| 16 | CreateIp | createIp | 创建IP | nvarchar | 50 |  |  | N | N | Y |
| 17 | UpdateTime | updateTime | 修改时间 | datetime | 23 |  | getdate() | N | N | N |
| 18 | UpdateUserId | updateUserId | 修改人编号 | int | 10 |  | 0 | N | N | N |
| 19 | UpdateUserName | updateUserName | 修改人用户名 | nvarchar | 50 |  |  | N | N | Y |
| 20 | UpdateBy | updateBy | 修改人姓名 | nvarchar | 50 |  |  | N | N | Y |
| 21 | UpdateIp | updateIp | 修改IP | nvarchar | 50 |  |  | N | N | Y |

## BaseDictionaryItem

**表描述**: 字典项

| 序号 | 字段名 | 小写字段名 | 字段描述 | 类型 | 长度 | 小数 | 默认值 | 主键 | 自增 | 允许空 |
|---|---|---|---|---|---|---|---|---|---|---|
| 1 | Id | id | 编号 | int | 10 |  |  | Y | Y | N |
| 2 | DictionaryId | dictionaryId | 字典编号 | int | 10 |  |  | N | N | N |
| 3 | ParentId | parentId | 父节点主键 | int | 10 |  | 0 | N | N | N |
| 4 | ItemKey | itemKey | 键 | nvarchar | 50 |  |  | N | N | N |
| 5 | ItemName | itemName | 名称 | nvarchar | 200 |  |  | N | N | Y |
| 6 | ItemValue | itemValue | 值 | nvarchar | 200 |  |  | N | N | N |
| 7 | Language | language | 语言(i18n) | nvarchar | 50 |  | global | N | N | N |
| 8 | AllowEdit | allowEdit | 允许编辑 | tinyint | 3 |  | 1 | N | N | N |
| 9 | AllowDelete | allowDelete | 允许删除 | tinyint | 3 |  | 1 | N | N | N |
| 10 | Description | description | 描述 | nvarchar | 4000 |  |  | N | N | Y |
| 11 | SortCode | sortCode | 排序编号 | int | 10 |  | 0 | N | N | N |
| 12 | Deleted | deleted | 是否删除 | tinyint | 3 |  | 0 | N | N | N |
| 13 | Enabled | enabled | 是否有效 | tinyint | 3 |  | 1 | N | N | N |
| 14 | CreateTime | createTime | 创建时间 | datetime | 23 |  | getdate() | N | N | N |
| 15 | CreateUserId | createUserId | 创建人编号 | int | 10 |  | 0 | N | N | N |
| 16 | CreateUserName | createUserName | 创建人用户名 | nvarchar | 50 |  |  | N | N | Y |
| 17 | CreateBy | createBy | 创建人姓名 | nvarchar | 50 |  |  | N | N | Y |
| 18 | CreateIp | createIp | 创建IP | nvarchar | 50 |  |  | N | N | Y |
| 19 | UpdateTime | updateTime | 修改时间 | datetime | 23 |  | getdate() | N | N | N |
| 20 | UpdateUserId | updateUserId | 修改人编号 | int | 10 |  | 0 | N | N | N |
| 21 | UpdateUserName | updateUserName | 修改人用户名 | nvarchar | 50 |  |  | N | N | Y |
| 22 | UpdateBy | updateBy | 修改人姓名 | nvarchar | 50 |  |  | N | N | Y |
| 23 | UpdateIp | updateIp | 修改IP | nvarchar | 50 |  |  | N | N | Y |

## BaseException

**表描述**: 系统异常

| 序号 | 字段名 | 小写字段名 | 字段描述 | 类型 | 长度 | 小数 | 默认值 | 主键 | 自增 | 允许空 |
|---|---|---|---|---|---|---|---|---|---|---|
| 1 | Id | id | 主键 | int | 10 |  |  | Y | Y | N |
| 2 | SystemCode | systemCode | 子系统编码 | nvarchar | 50 |  |  | N | N | Y |
| 3 | EventId | eventId | 事件编号 | int | 10 |  |  | N | N | Y |
| 4 | Category | category | 类别 | nvarchar | 100 |  |  | N | N | Y |
| 5 | Priority | priority | 优先级 | int | 10 |  |  | N | N | Y |
| 6 | Severity | severity | 严重级别 | nvarchar | 50 |  |  | N | N | Y |
| 7 | Title | title | 标题 | nvarchar | 256 |  |  | N | N | Y |
| 8 | Timestamp | timestamp | 时间戳 | datetime | 23 |  |  | N | N | Y |
| 9 | MachineName | machineName | 机器名 | nvarchar | 50 |  |  | N | N | Y |
| 10 | IpAddress | ipAddress | IP地址 | nvarchar | 50 |  |  | N | N | Y |
| 11 | AppDomainName | appDomainName | 应用域 | nvarchar | 4000 |  |  | N | N | Y |
| 12 | ProcessId | processId | 进程编号 | nvarchar | 256 |  |  | N | N | Y |
| 13 | ProcessName | processName | 进程名 | nvarchar | 4000 |  |  | N | N | Y |
| 14 | ThreadName | threadName | 线程名 | nvarchar | 4000 |  |  | N | N | Y |
| 15 | Win32ThreadId | win32ThreadId | 线程编号 | nvarchar | 128 |  |  | N | N | Y |
| 16 | Message | message | 消息 | nvarchar | 4000 |  |  | N | N | Y |
| 17 | FormattedMessage | formattedMessage | 格式化消息 | nvarchar | 4000 |  |  | N | N | Y |
| 18 | SortCode | sortCode | 排序编号 | int | 10 |  | 0 | N | N | N |
| 19 | Deleted | deleted | 是否删除 | tinyint | 3 |  | 0 | N | N | N |
| 20 | Enabled | enabled | 是否有效 | tinyint | 3 |  | 1 | N | N | N |
| 21 | CreateTime | createTime | 创建时间 | datetime | 23 |  | getdate() | N | N | N |
| 22 | CreateUserId | createUserId | 创建人编号 | int | 10 |  | 0 | N | N | N |
| 23 | CreateUserName | createUserName | 创建人用户名 | nvarchar | 50 |  |  | N | N | Y |
| 24 | CreateBy | createBy | 创建人姓名 | nvarchar | 50 |  |  | N | N | Y |
| 25 | CreateIp | createIp | 创建IP | nvarchar | 50 |  |  | N | N | Y |
| 26 | UpdateTime | updateTime | 修改时间 | datetime | 23 |  | getdate() | N | N | N |
| 27 | UpdateUserId | updateUserId | 修改人编号 | int | 10 |  | 0 | N | N | N |
| 28 | UpdateUserName | updateUserName | 修改人用户名 | nvarchar | 50 |  |  | N | N | Y |
| 29 | UpdateBy | updateBy | 修改人姓名 | nvarchar | 50 |  |  | N | N | Y |
| 30 | UpdateIp | updateIp | 修改IP | nvarchar | 50 |  |  | N | N | Y |

## BaseLog

**表描述**: 系统日志

| 序号 | 字段名 | 小写字段名 | 字段描述 | 类型 | 长度 | 小数 | 默认值 | 主键 | 自增 | 允许空 |
|---|---|---|---|---|---|---|---|---|---|---|
| 1 | Id | id | 主键 | int | 10 |  |  | Y | Y | N |
| 2 | CompanyId | companyId | 公司主键 | int | 10 |  | 0 | N | N | N |
| 3 | UserId | userId | 用户主键 | int | 10 |  | 0 | N | N | N |
| 4 | UserName | userName | 用户名 | nvarchar | 50 |  |  | N | N | Y |
| 5 | RealName | realName | 用户姓名 | nvarchar | 50 |  |  | N | N | Y |
| 6 | Service | service | 服务 | nvarchar | 50 |  |  | N | N | Y |
| 7 | TaskId | taskId | 任务 | nvarchar | 50 |  |  | N | N | Y |
| 8 | Parameters | parameters | 操作记录,添加,编辑,删除参数 | nvarchar | 4000 |  |  | N | N | Y |
| 9 | ClientIp | clientIp | IP地址 | nvarchar | 50 |  |  | N | N | Y |
| 10 | ServerIp | serverIp | IP地址 | nvarchar | 50 |  |  | N | N | Y |
| 11 | UrlReferrer | urlReferrer | 上一网络地址 | nvarchar | 4000 |  |  | N | N | Y |
| 12 | WebUrl | webUrl | 网络地址 | nvarchar | 4000 |  |  | N | N | Y |
| 13 | ElapsedTicks | elapsedTicks | 耗时 | decimal | 18 |  | 0 | N | N | Y |
| 14 | StartTime | startTime | 开始时间 | datetime | 23 |  | getdate() | N | N | Y |
| 15 | Description | description | 描述 | nvarchar | 4000 |  |  | N | N | Y |
| 16 | SortCode | sortCode | 排序编号 | int | 10 |  | 0 | N | N | N |
| 17 | Deleted | deleted | 是否删除 | tinyint | 3 |  | 0 | N | N | N |
| 18 | Enabled | enabled | 是否有效 | tinyint | 3 |  | 1 | N | N | N |
| 19 | CreateTime | createTime | 创建时间 | datetime | 23 |  | getdate() | N | N | N |
| 20 | CreateUserId | createUserId | 创建人编号 | int | 10 |  | 0 | N | N | N |
| 21 | CreateUserName | createUserName | 创建人用户名 | nvarchar | 50 |  |  | N | N | Y |
| 22 | CreateBy | createBy | 创建人姓名 | nvarchar | 50 |  |  | N | N | Y |
| 23 | CreateIp | createIp | 创建IP | nvarchar | 50 |  |  | N | N | Y |
| 24 | UpdateTime | updateTime | 修改时间 | datetime | 23 |  | getdate() | N | N | N |
| 25 | UpdateUserId | updateUserId | 修改人编号 | int | 10 |  | 0 | N | N | N |
| 26 | UpdateUserName | updateUserName | 修改人用户名 | nvarchar | 50 |  |  | N | N | Y |
| 27 | UpdateBy | updateBy | 修改人姓名 | nvarchar | 50 |  |  | N | N | Y |
| 28 | UpdateIp | updateIp | 修改IP | nvarchar | 50 |  |  | N | N | Y |

## BaseLogonLog

**表描述**: 登录日志

| 序号 | 字段名 | 小写字段名 | 字段描述 | 类型 | 长度 | 小数 | 默认值 | 主键 | 自增 | 允许空 |
|---|---|---|---|---|---|---|---|---|---|---|
| 1 | Id | id | 主键 | int | 10 |  |  | Y | Y | N |
| 2 | SystemCode | systemCode | 子系统编码 | nvarchar | 50 |  |  | N | N | Y |
| 3 | SourceType | sourceType | 发起请求的终端应用类型 | nvarchar | 50 |  |  | N | N | Y |
| 4 | UserId | userId | 用户主键 | int | 10 |  | 0 | N | N | N |
| 5 | UserName | userName | 用户名 | nvarchar | 50 |  |  | N | N | Y |
| 6 | NickName | nickName | 昵称 | nvarchar | 50 |  |  | N | N | Y |
| 7 | RealName | realName | 真实姓名 | nvarchar | 50 |  |  | N | N | Y |
| 8 | CompanyId | companyId | 公司主键 | int | 10 |  | 0 | N | N | N |
| 9 | CompanyName | companyName | 公司名称 | nvarchar | 50 |  |  | N | N | Y |
| 10 | CompanyCode | companyCode | 公司编码 | nvarchar | 50 |  |  | N | N | Y |
| 11 | Province | province | 省份 | nvarchar | 50 |  |  | N | N | Y |
| 12 | City | city | 城市 | nvarchar | 50 |  |  | N | N | Y |
| 13 | Service | service | 服务 | nvarchar | 50 |  |  | N | N | Y |
| 14 | ElapsedTicks | elapsedTicks | 耗时 | int | 10 |  | 0 | N | N | N |
| 15 | TargetApplication | targetApplication | 登录的目标应用 | nvarchar | 50 |  |  | N | N | Y |
| 16 | TargetIp | targetIp | 登录的目标服务器端IP | nvarchar | 50 |  |  | N | N | Y |
| 17 | Result | result | 操作结果（Success 1/Fail 0） | tinyint | 3 |  | 0 | N | N | N |
| 18 | OperationType | operationType | 操作类型（Login 1/Logout 0） | tinyint | 3 |  | 1 | N | N | N |
| 19 | LogonStatus | logonStatus | 登录状态 | nvarchar | 50 |  |  | N | N | Y |
| 20 | LogLevel | logLevel | 登录级别（0，正常；1、注意；2，危险；3、攻击） | tinyint | 3 |  | 0 | N | N | N |
| 21 | IpAddress | ipAddress | IP地址 | nvarchar | 50 |  |  | N | N | Y |
| 22 | IpAddressName | ipAddressName | IP地址位置名称 | nvarchar | 200 |  |  | N | N | Y |
| 23 | MacAddress | macAddress | MAC地址 | nvarchar | 50 |  |  | N | N | Y |
| 24 | SortCode | sortCode | 排序编号 | int | 10 |  | 0 | N | N | N |
| 25 | Deleted | deleted | 是否删除 | tinyint | 3 |  | 0 | N | N | N |
| 26 | Enabled | enabled | 是否有效 | tinyint | 3 |  | 1 | N | N | N |
| 27 | CreateTime | createTime | 创建时间 | datetime | 23 |  | getdate() | N | N | N |
| 28 | CreateUserId | createUserId | 创建人编号 | int | 10 |  | 0 | N | N | N |
| 29 | CreateUserName | createUserName | 创建人用户名 | nvarchar | 50 |  |  | N | N | Y |
| 30 | CreateBy | createBy | 创建人姓名 | nvarchar | 50 |  |  | N | N | Y |
| 31 | CreateIp | createIp | 创建IP | nvarchar | 50 |  |  | N | N | Y |
| 32 | UpdateTime | updateTime | 修改时间 | datetime | 23 |  | getdate() | N | N | N |
| 33 | UpdateUserId | updateUserId | 修改人编号 | int | 10 |  | 0 | N | N | N |
| 34 | UpdateUserName | updateUserName | 修改人用户名 | nvarchar | 50 |  |  | N | N | Y |
| 35 | UpdateBy | updateBy | 修改人姓名 | nvarchar | 50 |  |  | N | N | Y |
| 36 | UpdateIp | updateIp | 修改IP | nvarchar | 50 |  |  | N | N | Y |

## BaseMessageFailed

**表描述**: 失败消息

| 序号 | 字段名 | 小写字段名 | 字段描述 | 类型 | 长度 | 小数 | 默认值 | 主键 | 自增 | 允许空 |
|---|---|---|---|---|---|---|---|---|---|---|
| 1 | Id | id | 编号 | int | 10 |  |  | Y | Y | N |
| 2 | UserCompanyId | userCompanyId | 公司编号 | int | 10 |  | 0 | N | N | N |
| 3 | UserSubCompanyId | userSubCompanyId | 子公司编号 | int | 10 |  | 0 | N | N | N |
| 4 | Source | source | 来源 | nvarchar | 50 |  |  | N | N | Y |
| 5 | MessageType | messageType | 消息类型 | nvarchar | 50 |  | Email | N | N | Y |
| 6 | Recipient | recipient | 接收人 | nvarchar | 1024 |  |  | N | N | Y |
| 7 | Subject | subject | 主题 | nvarchar | 255 |  |  | N | N | Y |
| 8 | Body | body | 正文内容 | nvarchar | 4000 |  |  | N | N | Y |
| 9 | FailCount | failCount | 失败次数 | int | 10 |  | 0 | N | N | Y |
| 10 | Error | error | 错误信息 | nvarchar | 4000 |  |  | N | N | Y |
| 11 | SortCode | sortCode | 排序编号 | int | 10 |  | 0 | N | N | N |
| 12 | Deleted | deleted | 是否删除 | tinyint | 3 |  | 0 | N | N | N |
| 13 | Enabled | enabled | 是否有效 | tinyint | 3 |  | 1 | N | N | N |
| 14 | CreateTime | createTime | 创建时间 | datetime | 23 |  | getdate() | N | N | N |
| 15 | CreateUserId | createUserId | 创建人编号 | int | 10 |  | 0 | N | N | N |
| 16 | CreateUserName | createUserName | 创建人用户名 | nvarchar | 50 |  |  | N | N | Y |
| 17 | CreateBy | createBy | 创建人姓名 | nvarchar | 50 |  |  | N | N | Y |
| 18 | CreateIp | createIp | 创建IP | nvarchar | 50 |  |  | N | N | Y |
| 19 | UpdateTime | updateTime | 修改时间 | datetime | 23 |  | getdate() | N | N | N |
| 20 | UpdateUserId | updateUserId | 修改人编号 | int | 10 |  | 0 | N | N | N |
| 21 | UpdateUserName | updateUserName | 修改人用户名 | nvarchar | 50 |  |  | N | N | Y |
| 22 | UpdateBy | updateBy | 修改人姓名 | nvarchar | 50 |  |  | N | N | Y |
| 23 | UpdateIp | updateIp | 修改IP | nvarchar | 50 |  |  | N | N | Y |

## BaseMessageQueue

**表描述**: 消息队列

| 序号 | 字段名 | 小写字段名 | 字段描述 | 类型 | 长度 | 小数 | 默认值 | 主键 | 自增 | 允许空 |
|---|---|---|---|---|---|---|---|---|---|---|
| 1 | Id | id | 编号 | int | 10 |  |  | Y | Y | N |
| 2 | UserCompanyId | userCompanyId | 公司编号 | int | 10 |  | 0 | N | N | N |
| 3 | UserSubCompanyId | userSubCompanyId | 子公司编号 | int | 10 |  | 0 | N | N | N |
| 4 | Source | source | 来源 | nvarchar | 50 |  |  | N | N | Y |
| 5 | MessageType | messageType | 消息类型 | nvarchar | 50 |  | Email | N | N | Y |
| 6 | Recipient | recipient | 接收人 | nvarchar | 1024 |  |  | N | N | Y |
| 7 | Subject | subject | 主题 | nvarchar | 255 |  |  | N | N | Y |
| 8 | Body | body | 正文内容 | nvarchar | 4000 |  |  | N | N | Y |
| 9 | FailCount | failCount | 失败次数 | int | 10 |  | 0 | N | N | Y |
| 10 | SortCode | sortCode | 排序编号 | int | 10 |  | 0 | N | N | N |
| 11 | Deleted | deleted | 是否删除 | tinyint | 3 |  | 0 | N | N | N |
| 12 | Enabled | enabled | 是否有效 | tinyint | 3 |  | 1 | N | N | N |
| 13 | CreateTime | createTime | 创建时间 | datetime | 23 |  | getdate() | N | N | N |
| 14 | CreateUserId | createUserId | 创建人编号 | int | 10 |  | 0 | N | N | N |
| 15 | CreateUserName | createUserName | 创建人用户名 | nvarchar | 50 |  |  | N | N | Y |
| 16 | CreateBy | createBy | 创建人姓名 | nvarchar | 50 |  |  | N | N | Y |
| 17 | CreateIp | createIp | 创建IP | nvarchar | 50 |  |  | N | N | Y |
| 18 | UpdateTime | updateTime | 修改时间 | datetime | 23 |  | getdate() | N | N | N |
| 19 | UpdateUserId | updateUserId | 修改人编号 | int | 10 |  | 0 | N | N | N |
| 20 | UpdateUserName | updateUserName | 修改人用户名 | nvarchar | 50 |  |  | N | N | Y |
| 21 | UpdateBy | updateBy | 修改人姓名 | nvarchar | 50 |  |  | N | N | Y |
| 22 | UpdateIp | updateIp | 修改IP | nvarchar | 50 |  |  | N | N | Y |

## BaseMessageSucceed

**表描述**: 成功消息

| 序号 | 字段名 | 小写字段名 | 字段描述 | 类型 | 长度 | 小数 | 默认值 | 主键 | 自增 | 允许空 |
|---|---|---|---|---|---|---|---|---|---|---|
| 1 | Id | id | 编号 | int | 10 |  |  | Y | Y | N |
| 2 | UserCompanyId | userCompanyId | 公司编号 | int | 10 |  | 0 | N | N | N |
| 3 | UserSubCompanyId | userSubCompanyId | 子公司编号 | int | 10 |  | 0 | N | N | N |
| 4 | Source | source | 来源 | nvarchar | 50 |  |  | N | N | Y |
| 5 | MessageType | messageType | 消息类型 | nvarchar | 50 |  | Email | N | N | Y |
| 6 | Recipient | recipient | 接收人 | nvarchar | 1024 |  |  | N | N | Y |
| 7 | Subject | subject | 主题 | nvarchar | 255 |  |  | N | N | Y |
| 8 | Body | body | 正文内容 | nvarchar | 4000 |  |  | N | N | Y |
| 9 | SortCode | sortCode | 排序编号 | int | 10 |  | 0 | N | N | N |
| 10 | Deleted | deleted | 是否删除 | tinyint | 3 |  | 0 | N | N | N |
| 11 | Enabled | enabled | 是否有效 | tinyint | 3 |  | 1 | N | N | N |
| 12 | CreateTime | createTime | 创建时间 | datetime | 23 |  | getdate() | N | N | N |
| 13 | CreateUserId | createUserId | 创建人编号 | int | 10 |  | 0 | N | N | N |
| 14 | CreateUserName | createUserName | 创建人用户名 | nvarchar | 50 |  |  | N | N | Y |
| 15 | CreateBy | createBy | 创建人姓名 | nvarchar | 50 |  |  | N | N | Y |
| 16 | CreateIp | createIp | 创建IP | nvarchar | 50 |  |  | N | N | Y |
| 17 | UpdateTime | updateTime | 修改时间 | datetime | 23 |  | getdate() | N | N | N |
| 18 | UpdateUserId | updateUserId | 修改人编号 | int | 10 |  | 0 | N | N | N |
| 19 | UpdateUserName | updateUserName | 修改人用户名 | nvarchar | 50 |  |  | N | N | Y |
| 20 | UpdateBy | updateBy | 修改人姓名 | nvarchar | 50 |  |  | N | N | Y |
| 21 | UpdateIp | updateIp | 修改IP | nvarchar | 50 |  |  | N | N | Y |

## BaseModule

**表描述**: 模块菜单操作

| 序号 | 字段名 | 小写字段名 | 字段描述 | 类型 | 长度 | 小数 | 默认值 | 主键 | 自增 | 允许空 |
|---|---|---|---|---|---|---|---|---|---|---|
| 1 | Id | id | 主键 | int | 10 |  |  | Y | Y | N |
| 2 | SystemCode | systemCode | 子系统编码 | nvarchar | 50 |  | Base | N | N | N |
| 3 | ParentId | parentId | 父节点主键 | int | 10 |  | 0 | N | N | N |
| 4 | Code | code | 编号 | nvarchar | 100 |  |  | N | N | Y |
| 5 | Name | name | 名称 | nvarchar | 100 |  |  | N | N | Y |
| 6 | CategoryCode | categoryCode | 菜单分类 | nvarchar | 50 |  | Application | N | N | Y |
| 7 | ImageUrl | imageUrl | 图标位置 | nvarchar | 200 |  |  | N | N | Y |
| 8 | ImageIndex | imageIndex | 图标编号 | nvarchar | 50 |  |  | N | N | Y |
| 9 | SelectedImageIndex | selectedImageIndex | 选中状态图标编号 | nvarchar | 50 |  |  | N | N | Y |
| 10 | NavigateUrl | navigateUrl | Web网址 | nvarchar | 200 |  |  | N | N | Y |
| 11 | Target | target | 目标窗体中打开BS | nvarchar | 100 |  | fraContent | N | N | Y |
| 12 | FormName | formName | 窗体名CS | nvarchar | 100 |  |  | N | N | Y |
| 13 | AssemblyName | assemblyName | 动态连接库CS | nvarchar | 100 |  |  | N | N | Y |
| 14 | PermissionScopeTables | permissionScopeTables | 需要数据权限过滤的表(,符号分割) | nvarchar | 4000 |  |  | N | N | Y |
| 15 | IsMenu | isMenu | 是菜单项 | tinyint | 3 |  | 1 | N | N | N |
| 16 | IsPublic | isPublic | 是否公开 | tinyint | 3 |  | 0 | N | N | N |
| 17 | IsExpand | isExpand | 是否展开 | tinyint | 3 |  | 1 | N | N | N |
| 18 | IsScope | isScope | 权限域 | tinyint | 3 |  | 0 | N | N | N |
| 19 | IsVisible | isVisible | 是否可见 | tinyint | 3 |  | 1 | N | N | N |
| 20 | AllowEdit | allowEdit | 允许编辑 | tinyint | 3 |  | 1 | N | N | N |
| 21 | AllowDelete | allowDelete | 允许删除 | tinyint | 3 |  | 1 | N | N | N |
| 22 | LastCall | lastCall | 最后呼叫时间 | datetime | 23 |  |  | N | N | Y |
| 23 | WebBrowser | webBrowser | 浏览器 | nvarchar | 50 |  |  | N | N | Y |
| 24 | AuthorizedDays | authorizedDays | 认证天数 | int | 10 |  | 0 | N | N | N |
| 25 | Description | description | 描述 | nvarchar | 4000 |  |  | N | N | Y |
| 26 | SortCode | sortCode | 排序编号 | int | 10 |  | 0 | N | N | N |
| 27 | Deleted | deleted | 是否删除 | tinyint | 3 |  | 0 | N | N | N |
| 28 | Enabled | enabled | 是否有效 | tinyint | 3 |  | 1 | N | N | N |
| 29 | CreateTime | createTime | 创建时间 | datetime | 23 |  | getdate() | N | N | N |
| 30 | CreateUserId | createUserId | 创建人编号 | int | 10 |  | 0 | N | N | N |
| 31 | CreateUserName | createUserName | 创建人用户名 | nvarchar | 50 |  |  | N | N | Y |
| 32 | CreateBy | createBy | 创建人姓名 | nvarchar | 50 |  |  | N | N | Y |
| 33 | CreateIp | createIp | 创建IP | nvarchar | 50 |  |  | N | N | Y |
| 34 | UpdateTime | updateTime | 修改时间 | datetime | 23 |  | getdate() | N | N | N |
| 35 | UpdateUserId | updateUserId | 修改人编号 | int | 10 |  | 0 | N | N | N |
| 36 | UpdateUserName | updateUserName | 修改人用户名 | nvarchar | 50 |  |  | N | N | Y |
| 37 | UpdateBy | updateBy | 修改人姓名 | nvarchar | 50 |  |  | N | N | Y |
| 38 | UpdateIp | updateIp | 修改IP | nvarchar | 50 |  |  | N | N | Y |

## BaseOperationLog

**表描述**: 操作日志

| 序号 | 字段名 | 小写字段名 | 字段描述 | 类型 | 长度 | 小数 | 默认值 | 主键 | 自增 | 允许空 |
|---|---|---|---|---|---|---|---|---|---|---|
| 1 | Id | id | 主键 | int | 10 |  |  | Y | Y | N |
| 2 | SystemCode | systemCode | 子系统编码 | nvarchar | 50 |  | Base | N | N | N |
| 3 | TableName | tableName | 表名 | nvarchar | 50 |  |  | N | N | N |
| 4 | TableDescription | tableDescription | 表备注 | nvarchar | 200 |  |  | N | N | Y |
| 5 | Operation | operation | 操作类型 | nvarchar | 50 |  |  | N | N | N |
| 6 | RecordKey | recordKey | 记录主键 | nvarchar | 50 |  |  | N | N | N |
| 7 | SortCode | sortCode | 排序编号 | int | 10 |  | 0 | N | N | N |
| 8 | Deleted | deleted | 是否删除 | tinyint | 3 |  | 0 | N | N | N |
| 9 | Enabled | enabled | 是否有效 | tinyint | 3 |  | 1 | N | N | N |
| 10 | CreateTime | createTime | 创建时间 | datetime | 23 |  | getdate() | N | N | N |
| 11 | CreateUserId | createUserId | 创建人编号 | int | 10 |  | 0 | N | N | N |
| 12 | CreateUserName | createUserName | 创建人用户名 | nvarchar | 50 |  |  | N | N | Y |
| 13 | CreateBy | createBy | 创建人姓名 | nvarchar | 50 |  |  | N | N | Y |
| 14 | CreateIp | createIp | 创建IP | nvarchar | 50 |  |  | N | N | Y |
| 15 | UpdateTime | updateTime | 修改时间 | datetime | 23 |  | getdate() | N | N | N |
| 16 | UpdateUserId | updateUserId | 修改人编号 | int | 10 |  | 0 | N | N | N |
| 17 | UpdateUserName | updateUserName | 修改人用户名 | nvarchar | 50 |  |  | N | N | Y |
| 18 | UpdateBy | updateBy | 修改人姓名 | nvarchar | 50 |  |  | N | N | Y |
| 19 | UpdateIp | updateIp | 修改IP | nvarchar | 50 |  |  | N | N | Y |

## BaseOrganization

**表描述**: 组织机构

| 序号 | 字段名 | 小写字段名 | 字段描述 | 类型 | 长度 | 小数 | 默认值 | 主键 | 自增 | 允许空 |
|---|---|---|---|---|---|---|---|---|---|---|
| 1 | Id | id | 主键 | int | 10 |  |  | Y | Y | N |
| 2 | ParentId | parentId | 父级主键 | int | 10 |  | 0 | N | N | N |
| 3 | ParentName | parentName | 父级名称 | nvarchar | 50 |  |  | N | N | Y |
| 4 | Code | code | 编号 | nvarchar | 50 |  |  | N | N | Y |
| 5 | Name | name | 名称 | nvarchar | 50 |  |  | N | N | N |
| 6 | ShortName | shortName | 简称 | nvarchar | 50 |  |  | N | N | Y |
| 7 | StandardName | standardName | 标准名称 | nvarchar | 50 |  |  | N | N | Y |
| 8 | StandardCode | standardCode | 标准编号 | nvarchar | 50 |  |  | N | N | Y |
| 9 | QuickQuery | quickQuery | 快速查询 | nvarchar | 100 |  |  | N | N | Y |
| 10 | SimpleSpelling | simpleSpelling | 简拼 | nvarchar | 50 |  |  | N | N | Y |
| 11 | CategoryCode | categoryCode | 分类编码 | nvarchar | 50 |  |  | N | N | Y |
| 12 | OuterPhone | outerPhone | 外线电话 | nvarchar | 100 |  |  | N | N | Y |
| 13 | InnerPhone | innerPhone | 内线电话 | nvarchar | 100 |  |  | N | N | Y |
| 14 | Fax | fax | 传真 | nvarchar | 50 |  |  | N | N | Y |
| 15 | PostalCode | postalCode | 邮编 | nvarchar | 50 |  |  | N | N | Y |
| 16 | Province | province | 省名称 | nvarchar | 50 |  |  | N | N | Y |
| 17 | City | city | 市名称 | nvarchar | 50 |  |  | N | N | Y |
| 18 | District | district | 县名称 | nvarchar | 50 |  |  | N | N | Y |
| 19 | CompanyId | companyId | 所属公司主键 | int | 10 |  | 0 | N | N | N |
| 20 | CompanyCode | companyCode | 所属公司编号 | nvarchar | 50 |  |  | N | N | Y |
| 21 | CompanyName | companyName | 所属公司名称 | nvarchar | 50 |  |  | N | N | Y |
| 22 | Area | area | 所属区域 | nvarchar | 50 |  |  | N | N | Y |
| 23 | CostCenter | costCenter | 成本中心 | nvarchar | 50 |  |  | N | N | Y |
| 24 | FinancialCenter | financialCenter | 财务中心 | nvarchar | 50 |  |  | N | N | Y |
| 25 | Address | address | 地址 | nvarchar | 50 |  |  | N | N | Y |
| 26 | Web | web | 网址 | nvarchar | 50 |  |  | N | N | Y |
| 27 | Bank | bank | 开户行 | nvarchar | 50 |  |  | N | N | Y |
| 28 | BankAccount | bankAccount | 银行帐号 | nvarchar | 50 |  |  | N | N | Y |
| 29 | Layer | layer | 层 | int | 10 |  |  | N | N | Y |
| 30 | Longitude | longitude | 百度经度 | nvarchar | 16 |  |  | N | N | Y |
| 31 | Latitude | latitude | 百度维度 | nvarchar | 16 |  |  | N | N | Y |
| 32 | ContainChildNodes | containChildNodes | 是否有子节点 | tinyint | 3 |  | 0 | N | N | N |
| 33 | IsInnerOrganization | isInnerOrganization | 是否内部组织机构 | tinyint | 3 |  | 1 | N | N | N |
| 34 | ProvinceId | provinceId | 省主键 | int | 10 |  |  | N | N | Y |
| 35 | CityId | cityId | 市主键 | int | 10 |  |  | N | N | Y |
| 36 | DistrictId | districtId | 县主键 | int | 10 |  |  | N | N | Y |
| 37 | StreetId | streetId | 街道主键 | int | 10 |  |  | N | N | Y |
| 38 | Street | street | 街道名称 | nvarchar | 50 |  |  | N | N | Y |
| 39 | CostCenterId | costCenterId | 成本中心主键 | nvarchar | 50 |  |  | N | N | Y |
| 40 | FinancialCenterId | financialCenterId | 财务中心主键 | nvarchar | 50 |  |  | N | N | Y |
| 41 | Leader | leader | 领导 | nvarchar | 50 |  |  | N | N | Y |
| 42 | LeaderMobile | leaderMobile | 领导手机 | nvarchar | 50 |  |  | N | N | Y |
| 43 | Manager | manager | 经理 | nvarchar | 50 |  |  | N | N | Y |
| 44 | ManagerMobile | managerMobile | 经理手机 | nvarchar | 50 |  |  | N | N | Y |
| 45 | EmergencyCall | emergencyCall | 紧急联系电话 | nvarchar | 50 |  |  | N | N | Y |
| 46 | BusinessPhone | businessPhone | 业务咨询电话 | nvarchar | 50 |  |  | N | N | Y |
| 47 | Description | description | 描述 | nvarchar | 4000 |  |  | N | N | Y |
| 48 | SortCode | sortCode | 排序编号 | int | 10 |  | 0 | N | N | N |
| 49 | Deleted | deleted | 是否删除 | tinyint | 3 |  | 0 | N | N | N |
| 50 | Enabled | enabled | 是否有效 | tinyint | 3 |  | 1 | N | N | N |
| 51 | CreateTime | createTime | 创建时间 | datetime | 23 |  | getdate() | N | N | N |
| 52 | CreateUserId | createUserId | 创建人编号 | int | 10 |  | 0 | N | N | N |
| 53 | CreateUserName | createUserName | 创建人用户名 | nvarchar | 50 |  |  | N | N | Y |
| 54 | CreateBy | createBy | 创建人姓名 | nvarchar | 50 |  |  | N | N | Y |
| 55 | CreateIp | createIp | 创建IP | nvarchar | 50 |  |  | N | N | Y |
| 56 | UpdateTime | updateTime | 修改时间 | datetime | 23 |  | getdate() | N | N | N |
| 57 | UpdateUserId | updateUserId | 修改人编号 | int | 10 |  | 0 | N | N | N |
| 58 | UpdateUserName | updateUserName | 修改人用户名 | nvarchar | 50 |  |  | N | N | Y |
| 59 | UpdateBy | updateBy | 修改人姓名 | nvarchar | 50 |  |  | N | N | Y |
| 60 | UpdateIp | updateIp | 修改IP | nvarchar | 50 |  |  | N | N | Y |

## BaseOrganizationScope

**表描述**: 组织机构权限范围

| 序号 | 字段名 | 小写字段名 | 字段描述 | 类型 | 长度 | 小数 | 默认值 | 主键 | 自增 | 允许空 |
|---|---|---|---|---|---|---|---|---|---|---|
| 1 | Id | id | 主键 | int | 10 |  |  | Y | Y | N |
| 2 | SystemCode | systemCode | 子系统编码 | nvarchar | 50 |  | Base | N | N | N |
| 3 | ResourceCategory | resourceCategory | 什么类型的 | nvarchar | 50 |  |  | N | N | Y |
| 4 | ResourceId | resourceId | 什么资源主键 | nvarchar | 50 |  |  | N | N | Y |
| 5 | PermissionId | permissionId | 有什么权限（模块菜单）主键 | int | 10 |  |  | N | N | Y |
| 6 | AllData | allData | 全部数据 | tinyint | 3 |  |  | N | N | Y |
| 7 | Province | province | 所在的省 | tinyint | 3 |  |  | N | N | Y |
| 8 | City | city | 所在的市 | tinyint | 3 |  |  | N | N | Y |
| 9 | District | district | 所在的县/区 | tinyint | 3 |  |  | N | N | Y |
| 10 | Street | street | 街道 | tinyint | 3 |  |  | N | N | Y |
| 11 | UserCompany | userCompany | 用户所在公司的数据 | tinyint | 3 |  |  | N | N | Y |
| 12 | UserSubCompany | userSubCompany | 用户所在分公司的数据 | tinyint | 3 |  |  | N | N | Y |
| 13 | UserDepartment | userDepartment | 用户所在部门的数据 | tinyint | 3 |  |  | N | N | Y |
| 14 | UserSubDepartment | userSubDepartment | 用户所在子部门的数据 | tinyint | 3 |  |  | N | N | Y |
| 15 | UserWorkgroup | userWorkgroup | 用户所在工作组的数据 | tinyint | 3 |  |  | N | N | Y |
| 16 | OnlyOwnData | onlyOwnData | 仅仅用户自己的数据 | tinyint | 3 |  | 1 | N | N | N |
| 17 | NotAllowed | notAllowed | 不允许查看数据 | tinyint | 3 |  | 0 | N | N | Y |
| 18 | ByDetails | byDetails | 按详细设置 | tinyint | 3 |  | 0 | N | N | Y |
| 19 | ContainChild | containChild | 包含子节点的数据 | tinyint | 3 |  |  | N | N | N |
| 20 | Description | description | 描述 | nvarchar | 4000 |  |  | N | N | Y |
| 21 | SortCode | sortCode | 排序编号 | int | 10 |  | 0 | N | N | N |
| 22 | Deleted | deleted | 是否删除 | tinyint | 3 |  | 0 | N | N | N |
| 23 | Enabled | enabled | 是否有效 | tinyint | 3 |  | 1 | N | N | N |
| 24 | CreateTime | createTime | 创建时间 | datetime | 23 |  | getdate() | N | N | N |
| 25 | CreateUserId | createUserId | 创建人编号 | int | 10 |  | 0 | N | N | N |
| 26 | CreateUserName | createUserName | 创建人用户名 | nvarchar | 50 |  |  | N | N | Y |
| 27 | CreateBy | createBy | 创建人姓名 | nvarchar | 50 |  |  | N | N | Y |
| 28 | CreateIp | createIp | 创建IP | nvarchar | 50 |  |  | N | N | Y |
| 29 | UpdateTime | updateTime | 修改时间 | datetime | 23 |  | getdate() | N | N | N |
| 30 | UpdateUserId | updateUserId | 修改人编号 | int | 10 |  | 0 | N | N | N |
| 31 | UpdateUserName | updateUserName | 修改人用户名 | nvarchar | 50 |  |  | N | N | Y |
| 32 | UpdateBy | updateBy | 修改人姓名 | nvarchar | 50 |  |  | N | N | Y |
| 33 | UpdateIp | updateIp | 修改IP | nvarchar | 50 |  |  | N | N | Y |

## BaseParameter

**表描述**: 参数

| 序号 | 字段名 | 小写字段名 | 字段描述 | 类型 | 长度 | 小数 | 默认值 | 主键 | 自增 | 允许空 |
|---|---|---|---|---|---|---|---|---|---|---|
| 1 | Id | id | 主键 | int | 10 |  |  | Y | Y | N |
| 2 | SystemCode | systemCode | 子系统编码 | nvarchar | 50 |  | Base | N | N | N |
| 3 | CategoryCode | categoryCode | 分类编号 | nvarchar | 100 |  |  | N | N | Y |
| 4 | ParameterId | parameterId | 参数主键 | nvarchar | 100 |  |  | N | N | Y |
| 5 | ParameterCode | parameterCode | 参数编码 | nvarchar | 100 |  |  | N | N | Y |
| 6 | ParameterContent | parameterContent | 参数内容 | nvarchar | 4000 |  |  | N | N | Y |
| 7 | Description | description | 描述 | nvarchar | 4000 |  |  | N | N | Y |
| 8 | SortCode | sortCode | 排序编号 | int | 10 |  | 0 | N | N | N |
| 9 | Deleted | deleted | 是否删除 | tinyint | 3 |  | 0 | N | N | N |
| 10 | Enabled | enabled | 是否有效 | tinyint | 3 |  | 1 | N | N | N |
| 11 | CreateTime | createTime | 创建时间 | datetime | 23 |  | getdate() | N | N | N |
| 12 | CreateUserId | createUserId | 创建人编号 | int | 10 |  | 0 | N | N | N |
| 13 | CreateUserName | createUserName | 创建人用户名 | nvarchar | 50 |  |  | N | N | Y |
| 14 | CreateBy | createBy | 创建人姓名 | nvarchar | 50 |  |  | N | N | Y |
| 15 | CreateIp | createIp | 创建IP | nvarchar | 50 |  |  | N | N | Y |
| 16 | UpdateTime | updateTime | 修改时间 | datetime | 23 |  | getdate() | N | N | N |
| 17 | UpdateUserId | updateUserId | 修改人编号 | int | 10 |  | 0 | N | N | N |
| 18 | UpdateBy | updateBy | 修改人姓名 | nvarchar | 50 |  |  | N | N | Y |
| 19 | UpdateUserName | updateUserName | 修改人用户名 | nvarchar | 50 |  |  | N | N | Y |
| 20 | UpdateIp | updateIp | 修改IP | nvarchar | 50 |  |  | N | N | Y |

## BasePermission

**表描述**: 权限

| 序号 | 字段名 | 小写字段名 | 字段描述 | 类型 | 长度 | 小数 | 默认值 | 主键 | 自增 | 允许空 |
|---|---|---|---|---|---|---|---|---|---|---|
| 1 | Id | id | 主键 | int | 10 |  |  | Y | Y | N |
| 2 | SystemCode | systemCode | 子系统编码 | nvarchar | 50 |  | Base | N | N | N |
| 3 | ResourceCategory | resourceCategory | 资料类别 | nvarchar | 50 |  |  | N | N | Y |
| 4 | ResourceId | resourceId | 资源主键 | nvarchar | 50 |  |  | N | N | Y |
| 5 | PermissionId | permissionId | 权限（菜单模块）主键 | nvarchar | 50 |  |  | N | N | Y |
| 6 | CompanyId | companyId | 公司主键 | int | 10 |  | 0 | N | N | N |
| 7 | CompanyName | companyName | 公司名称 | nvarchar | 50 |  |  | N | N | Y |
| 8 | PermissionConstraint | permissionConstraint | 权限条件限制 | nvarchar | 200 |  |  | N | N | Y |
| 9 | Description | description | 描述 | nvarchar | 4000 |  |  | N | N | Y |
| 10 | SortCode | sortCode | 排序编号 | int | 10 |  | 0 | N | N | N |
| 11 | Deleted | deleted | 是否删除 | tinyint | 3 |  | 0 | N | N | N |
| 12 | Enabled | enabled | 是否有效 | tinyint | 3 |  | 1 | N | N | N |
| 13 | CreateTime | createTime | 创建时间 | datetime | 23 |  | getdate() | N | N | N |
| 14 | CreateUserId | createUserId | 创建人编号 | int | 10 |  | 0 | N | N | N |
| 15 | CreateUserName | createUserName | 创建人用户名 | nvarchar | 50 |  |  | N | N | Y |
| 16 | CreateBy | createBy | 创建人姓名 | nvarchar | 50 |  |  | N | N | Y |
| 17 | CreateIp | createIp | 创建IP | nvarchar | 50 |  |  | N | N | Y |
| 18 | UpdateTime | updateTime | 修改时间 | datetime | 23 |  | getdate() | N | N | N |
| 19 | UpdateUserId | updateUserId | 修改人编号 | int | 10 |  | 0 | N | N | N |
| 20 | UpdateUserName | updateUserName | 修改人用户名 | nvarchar | 50 |  |  | N | N | Y |
| 21 | UpdateBy | updateBy | 修改人姓名 | nvarchar | 50 |  |  | N | N | Y |
| 22 | UpdateIp | updateIp | 修改IP | nvarchar | 50 |  |  | N | N | Y |

## BasePermissionScope

**表描述**: 数据权限

| 序号 | 字段名 | 小写字段名 | 字段描述 | 类型 | 长度 | 小数 | 默认值 | 主键 | 自增 | 允许空 |
|---|---|---|---|---|---|---|---|---|---|---|
| 1 | Id | id | 主键 | int | 10 |  |  | Y | Y | N |
| 2 | SystemCode | systemCode | 子系统编码 | nvarchar | 50 |  | Base | N | N | N |
| 3 | ResourceCategory | resourceCategory | 什么类型的 | nvarchar | 50 |  |  | N | N | Y |
| 4 | ResourceId | resourceId | 什么资源主键 | int | 10 |  |  | N | N | N |
| 5 | TargetCategory | targetCategory | 对什么类型的 | nvarchar | 50 |  |  | N | N | Y |
| 6 | TargetId | targetId | 对什么资源主键 | int | 10 |  |  | N | N | N |
| 7 | PermissionId | permissionId | 有什么权限（模块菜单）主键 | int | 10 |  |  | N | N | N |
| 8 | ContainChild | containChild | 包含子节点 | tinyint | 3 |  | 0 | N | N | N |
| 9 | PermissionConstraint | permissionConstraint | 有什么权限约束表达式 | nvarchar | 200 |  |  | N | N | Y |
| 10 | StartTime | startTime | 开始生效时间 | datetime | 23 |  |  | N | N | Y |
| 11 | EndTime | endTime | 结束生效时间 | datetime | 23 |  |  | N | N | Y |
| 12 | Description | description | 描述 | nvarchar | 4000 |  |  | N | N | Y |
| 13 | SortCode | sortCode | 排序编号 | int | 10 |  | 0 | N | N | N |
| 14 | Deleted | deleted | 是否删除 | tinyint | 3 |  | 0 | N | N | N |
| 15 | Enabled | enabled | 是否有效 | tinyint | 3 |  | 1 | N | N | N |
| 16 | CreateTime | createTime | 创建时间 | datetime | 23 |  | getdate() | N | N | N |
| 17 | CreateUserId | createUserId | 创建人编号 | int | 10 |  | 0 | N | N | N |
| 18 | CreateUserName | createUserName | 创建人用户名 | nvarchar | 50 |  |  | N | N | Y |
| 19 | CreateBy | createBy | 创建人姓名 | nvarchar | 50 |  |  | N | N | Y |
| 20 | CreateIp | createIp | 创建IP | nvarchar | 50 |  |  | N | N | Y |
| 21 | UpdateTime | updateTime | 修改时间 | datetime | 23 |  | getdate() | N | N | N |
| 22 | UpdateUserId | updateUserId | 修改人编号 | int | 10 |  | 0 | N | N | N |
| 23 | UpdateUserName | updateUserName | 修改人用户名 | nvarchar | 50 |  |  | N | N | Y |
| 24 | UpdateBy | updateBy | 修改人姓名 | nvarchar | 50 |  |  | N | N | Y |
| 25 | UpdateIp | updateIp | 修改IP | nvarchar | 50 |  |  | N | N | Y |

## BaseRole

**表描述**: 角色

| 序号 | 字段名 | 小写字段名 | 字段描述 | 类型 | 长度 | 小数 | 默认值 | 主键 | 自增 | 允许空 |
|---|---|---|---|---|---|---|---|---|---|---|
| 1 | Id | id | 主键 | int | 10 |  |  | Y | Y | N |
| 2 | SystemCode | systemCode | 子系统编码 | nvarchar | 50 |  | Base | N | N | N |
| 3 | OrganizationId | organizationId | 组织机构主键 | int | 10 |  | 0 | N | N | N |
| 4 | Code | code | 角色编号 | nvarchar | 100 |  |  | N | N | Y |
| 5 | Name | name | 角色名称 | nvarchar | 200 |  |  | N | N | Y |
| 6 | CategoryCode | categoryCode | 角色分类 | nvarchar | 50 |  |  | N | N | Y |
| 7 | AllowEdit | allowEdit | 允许编辑 | tinyint | 3 |  | 1 | N | N | N |
| 8 | AllowDelete | allowDelete | 允许删除 | tinyint | 3 |  | 1 | N | N | N |
| 9 | IsVisible | isVisible | 是否显示 | tinyint | 3 |  | 1 | N | N | N |
| 10 | Description | description | 描述 | nvarchar | 4000 |  |  | N | N | Y |
| 11 | SortCode | sortCode | 排序编号 | int | 10 |  | 0 | N | N | N |
| 12 | Deleted | deleted | 是否删除 | tinyint | 3 |  | 0 | N | N | N |
| 13 | Enabled | enabled | 是否有效 | tinyint | 3 |  | 1 | N | N | N |
| 14 | CreateTime | createTime | 创建时间 | datetime | 23 |  | getdate() | N | N | N |
| 15 | CreateUserId | createUserId | 创建人编号 | int | 10 |  | 0 | N | N | N |
| 16 | CreateUserName | createUserName | 创建人用户名 | nvarchar | 50 |  |  | N | N | Y |
| 17 | CreateBy | createBy | 创建人姓名 | nvarchar | 50 |  |  | N | N | Y |
| 18 | CreateIp | createIp | 创建IP | nvarchar | 50 |  |  | N | N | Y |
| 19 | UpdateTime | updateTime | 修改时间 | datetime | 23 |  | getdate() | N | N | N |
| 20 | UpdateUserId | updateUserId | 修改人编号 | int | 10 |  | 0 | N | N | N |
| 21 | UpdateBy | updateBy | 修改人姓名 | nvarchar | 50 |  |  | N | N | Y |
| 22 | UpdateUserName | updateUserName | 修改人用户名 | nvarchar | 50 |  |  | N | N | Y |
| 23 | UpdateIp | updateIp | 修改IP | nvarchar | 50 |  |  | N | N | Y |

## BaseRoleOrganization

**表描述**: 角色组织机构

| 序号 | 字段名 | 小写字段名 | 字段描述 | 类型 | 长度 | 小数 | 默认值 | 主键 | 自增 | 允许空 |
|---|---|---|---|---|---|---|---|---|---|---|
| 1 | Id | id | 主键 | int | 10 |  |  | Y | Y | N |
| 2 | SystemCode | systemCode | 子系统编码 | nvarchar | 50 |  | Base | N | N | N |
| 3 | RoleId | roleId | 角色主键 | int | 10 |  | 0 | N | N | N |
| 4 | OrganizationId | organizationId | 组织机构主键 | int | 10 |  | 0 | N | N | N |
| 5 | Description | description | 描述 | nvarchar | 4000 |  |  | N | N | Y |
| 6 | SortCode | sortCode | 排序编号 | int | 10 |  | 0 | N | N | N |
| 7 | Deleted | deleted | 是否删除 | tinyint | 3 |  | 0 | N | N | N |
| 8 | Enabled | enabled | 是否有效 | tinyint | 3 |  | 1 | N | N | N |
| 9 | CreateTime | createTime | 创建时间 | datetime | 23 |  | getdate() | N | N | N |
| 10 | CreateUserId | createUserId | 创建人编号 | int | 10 |  | 0 | N | N | N |
| 11 | CreateUserName | createUserName | 创建人用户名 | nvarchar | 50 |  |  | N | N | Y |
| 12 | CreateBy | createBy | 创建人姓名 | nvarchar | 50 |  |  | N | N | Y |
| 13 | CreateIp | createIp | 创建IP | nvarchar | 50 |  |  | N | N | Y |
| 14 | UpdateTime | updateTime | 修改时间 | datetime | 23 |  | getdate() | N | N | N |
| 15 | UpdateUserId | updateUserId | 修改人编号 | int | 10 |  | 0 | N | N | N |
| 16 | UpdateUserName | updateUserName | 修改人用户名 | nvarchar | 50 |  |  | N | N | Y |
| 17 | UpdateBy | updateBy | 修改人姓名 | nvarchar | 50 |  |  | N | N | Y |
| 18 | UpdateIp | updateIp | 修改IP | nvarchar | 50 |  |  | N | N | Y |

## BaseSequence

**表描述**: 序列

| 序号 | 字段名 | 小写字段名 | 字段描述 | 类型 | 长度 | 小数 | 默认值 | 主键 | 自增 | 允许空 |
|---|---|---|---|---|---|---|---|---|---|---|
| 1 | Id | id | 主键 | int | 10 |  |  | Y | Y | N |
| 2 | Name | name | 名称 | nvarchar | 100 |  |  | N | N | N |
| 3 | Prefix | prefix | 前缀 | nvarchar | 50 |  |  | N | N | Y |
| 4 | Delimiter | delimiter | 分隔符 | nvarchar | 50 |  |  | N | N | Y |
| 5 | Sequence | sequence | 升序序列 | int | 10 |  | 10000000 | N | N | N |
| 6 | Reduction | reduction | 倒序序列 | int | 10 |  | 9999999 | N | N | N |
| 7 | Step | step | 步长 | int | 10 |  | 1 | N | N | N |
| 8 | IsVisible | isVisible | 是否显示 | tinyint | 3 |  | 1 | N | N | N |
| 9 | Description | description | 描述 | nvarchar | 4000 |  |  | N | N | Y |
| 10 | SortCode | sortCode | 排序编号 | int | 10 |  | 0 | N | N | N |
| 11 | Deleted | deleted | 是否删除 | tinyint | 3 |  | 0 | N | N | N |
| 12 | Enabled | enabled | 是否有效 | tinyint | 3 |  | 1 | N | N | N |
| 13 | CreateTime | createTime | 创建时间 | datetime | 23 |  | getdate() | N | N | N |
| 14 | CreateUserId | createUserId | 创建人编号 | int | 10 |  | 0 | N | N | N |
| 15 | CreateUserName | createUserName | 创建人用户名 | nvarchar | 50 |  |  | N | N | Y |
| 16 | CreateBy | createBy | 创建人姓名 | nvarchar | 50 |  |  | N | N | Y |
| 17 | CreateIp | createIp | 创建IP | nvarchar | 50 |  |  | N | N | Y |
| 18 | UpdateTime | updateTime | 修改时间 | datetime | 23 |  | getdate() | N | N | N |
| 19 | UpdateUserId | updateUserId | 修改人编号 | int | 10 |  | 0 | N | N | N |
| 20 | UpdateBy | updateBy | 修改人姓名 | nvarchar | 50 |  |  | N | N | Y |
| 21 | UpdateUserName | updateUserName | 修改人用户名 | nvarchar | 50 |  |  | N | N | Y |
| 22 | UpdateIp | updateIp | 修改IP | nvarchar | 50 |  |  | N | N | Y |

## BaseStaff

**表描述**: 员工

| 序号 | 字段名 | 小写字段名 | 字段描述 | 类型 | 长度 | 小数 | 默认值 | 主键 | 自增 | 允许空 |
|---|---|---|---|---|---|---|---|---|---|---|
| 1 | Id | id | 主键 | int | 10 |  |  | Y | Y | N |
| 2 | UserId | userId | 用户主键 | int | 10 |  | 0 | N | N | N |
| 3 | UserName | userName | 用户名 | nvarchar | 50 |  |  | N | N | Y |
| 4 | EmployeeNumber | employeeNumber | 工号 | nvarchar | 50 |  |  | N | N | Y |
| 5 | RealName | realName | 姓名 | nvarchar | 50 |  |  | N | N | Y |
| 6 | ChineseName | chineseName | 中文名 | nvarchar | 50 |  |  | N | N | Y |
| 7 | EnglishName | englishName | 英文名 | nvarchar | 50 |  |  | N | N | Y |
| 8 | QuickQuery | quickQuery | 快速查找，记忆符 | nvarchar | 100 |  |  | N | N | Y |
| 9 | CompanyId | companyId | 公司主键 | int | 10 |  | 0 | N | N | N |
| 10 | CompanyName | companyName | 公司名称 | nvarchar | 50 |  |  | N | N | Y |
| 11 | SubCompanyId | subCompanyId | 分支机构主键 | int | 10 |  | 0 | N | N | N |
| 12 | SubCompanyName | subCompanyName | 子公司名称 | nvarchar | 50 |  |  | N | N | Y |
| 13 | DepartmentId | departmentId | 部门主键 | int | 10 |  | 0 | N | N | N |
| 14 | DepartmentName | departmentName | 部门名称 | nvarchar | 50 |  |  | N | N | Y |
| 15 | WorkgroupId | workgroupId | 工作组主键 | int | 10 |  | 0 | N | N | N |
| 16 | WorkgroupName | workgroupName | 工作组名称 | nvarchar | 50 |  |  | N | N | Y |
| 17 | DutyId | dutyId | 职位主键 | int | 10 |  | 0 | N | N | N |
| 18 | Gender | gender | 性别 | nvarchar | 50 |  |  | N | N | Y |
| 19 | Birthday | birthday | 生日 | date | 10 |  |  | N | N | Y |
| 20 | Age | age | 年龄 | smallint | 5 |  |  | N | N | Y |
| 21 | Height | height | 身高 | nvarchar | 10 |  |  | N | N | Y |
| 22 | Weight | weight | 体重 | nvarchar | 10 |  |  | N | N | Y |
| 23 | IdentificationCode | identificationCode | 唯一身份Id | nvarchar | 50 |  |  | N | N | Y |
| 24 | IdCard | idCard | 身份证号码 | nvarchar | 50 |  |  | N | N | Y |
| 25 | Nation | nation | 国籍 | nvarchar | 50 |  |  | N | N | Y |
| 26 | Education | education | 最高学历 | nvarchar | 50 |  |  | N | N | Y |
| 27 | School | school | 毕业院校 | nvarchar | 200 |  |  | N | N | Y |
| 28 | GraduationDate | graduationDate | 毕业日期 | date | 10 |  |  | N | N | Y |
| 29 | Major | major | 专业 | nvarchar | 50 |  |  | N | N | Y |
| 30 | Degree | degree | 最高学位 | nvarchar | 50 |  |  | N | N | Y |
| 31 | TitleId | titleId | 职称主键 | nvarchar | 50 |  |  | N | N | Y |
| 32 | TitleDate | titleDate | 职称评定日期 | nvarchar | 50 |  |  | N | N | Y |
| 33 | TitleLevel | titleLevel | 职称等级 | nvarchar | 50 |  |  | N | N | Y |
| 34 | WorkingDate | workingDate | 工作时间 | date | 10 |  |  | N | N | Y |
| 35 | JoinInDate | joinInDate | 加入本单位时间 | date | 10 |  |  | N | N | Y |
| 36 | OfficePostCode | officePostCode | 办公邮编 | nvarchar | 50 |  |  | N | N | Y |
| 37 | OfficeAddress | officeAddress | 办公地址 | nvarchar | 200 |  |  | N | N | Y |
| 38 | OfficePhone | officePhone | 办公电话 | nvarchar | 50 |  |  | N | N | Y |
| 39 | OfficeFax | officeFax | 办公传真 | nvarchar | 50 |  |  | N | N | Y |
| 40 | HomePostCode | homePostCode | 家庭住址邮编 | nvarchar | 50 |  |  | N | N | Y |
| 41 | HomeAddress | homeAddress | 家庭住址 | nvarchar | 50 |  |  | N | N | Y |
| 42 | HomePhone | homePhone | 住宅电话 | nvarchar | 50 |  |  | N | N | Y |
| 43 | HomeFax | homeFax | 家庭传真 | nvarchar | 50 |  |  | N | N | Y |
| 44 | PlateNumber1 | plateNumber1 | 第一辆车牌号 | nvarchar | 50 |  |  | N | N | Y |
| 45 | PlateNumber2 | plateNumber2 | 第二辆车牌号 | nvarchar | 50 |  |  | N | N | Y |
| 46 | PlateNumber3 | plateNumber3 | 第三辆车牌号 | nvarchar | 50 |  |  | N | N | Y |
| 47 | RewardCard | rewardCard | 奖金卡号 | nvarchar | 50 |  |  | N | N | Y |
| 48 | MedicalCard | medicalCard | 医疗卡号 | nvarchar | 50 |  |  | N | N | Y |
| 49 | UnionMember | unionMember | 工会证号 | nvarchar | 50 |  |  | N | N | Y |
| 50 | Email | email | Email | nvarchar | 100 |  |  | N | N | Y |
| 51 | Mobile | mobile | 手机 | nvarchar | 50 |  |  | N | N | Y |
| 52 | QQ | qq | QQ | nvarchar | 50 |  |  | N | N | Y |
| 53 | WeChat | weChat | 微信 | nvarchar | 50 |  |  | N | N | Y |
| 54 | ShortNumber | shortNumber | 短号 | nvarchar | 50 |  |  | N | N | Y |
| 55 | Telephone | telephone | 电话 | nvarchar | 50 |  |  | N | N | Y |
| 56 | Extension | extension | 分机 | nvarchar | 50 |  |  | N | N | Y |
| 57 | EmergencyContact | emergencyContact | 紧急联系 | nvarchar | 200 |  |  | N | N | Y |
| 58 | EmergencyMobile | emergencyMobile | 紧急联系手机 | nvarchar | 50 |  |  | N | N | Y |
| 59 | EmergencyTelephone | emergencyTelephone | 紧急联系电话 | nvarchar | 50 |  |  | N | N | Y |
| 60 | NativePlace | nativePlace | 籍贯 | nvarchar | 100 |  |  | N | N | Y |
| 61 | BankName | bankName | 开户行 | nvarchar | 50 |  |  | N | N | Y |
| 62 | BankAccount | bankAccount | 银行卡号 | nvarchar | 50 |  |  | N | N | Y |
| 63 | BankUserName | bankUserName | 开户行姓名 | nvarchar | 50 |  |  | N | N | Y |
| 64 | Province | province | 籍贯省 | nvarchar | 50 |  |  | N | N | Y |
| 65 | City | city | 籍贯市 | nvarchar | 50 |  |  | N | N | Y |
| 66 | District | district | 籍贯区 | nvarchar | 50 |  |  | N | N | Y |
| 67 | CurrentProvince | currentProvince | 当前省 | nvarchar | 50 |  |  | N | N | Y |
| 68 | CurrentCity | currentCity | 当前市 | nvarchar | 50 |  |  | N | N | Y |
| 69 | CurrentDistrict | currentDistrict | 当前区 | nvarchar | 50 |  |  | N | N | Y |
| 70 | Party | party | 政治面貌 | nvarchar | 50 |  |  | N | N | Y |
| 71 | Nationality | nationality | 民族 | nvarchar | 50 |  |  | N | N | Y |
| 72 | WorkingProperty | workingProperty | 工作性质 | nvarchar | 50 |  |  | N | N | Y |
| 73 | Competency | competency | 职业资格 | nvarchar | 50 |  |  | N | N | Y |
| 74 | Marriage | marriage | 婚姻情况 | nvarchar | 50 |  |  | N | N | Y |
| 75 | WeddingDate | weddingDate | 结婚日期 | date | 10 |  |  | N | N | Y |
| 76 | DivorceDate | divorceDate | 离婚日期 | date | 10 |  |  | N | N | Y |
| 77 | Child1Birthday | child1Birthday | 第一个孩子生日 | date | 10 |  |  | N | N | Y |
| 78 | Child2Birthday | child2Birthday | 第二个孩子生日 | date | 10 |  |  | N | N | Y |
| 79 | Child3Birthday | child3Birthday | 第三个孩子生日 | date | 10 |  |  | N | N | Y |
| 80 | Child4Birthday | child4Birthday | 第四个孩子生日 | date | 10 |  |  | N | N | Y |
| 81 | Child5Birthday | child5Birthday | 第五个孩子生日 | date | 10 |  |  | N | N | Y |
| 82 | IsDimission | isDimission | 是否离职 | tinyint | 3 |  | 0 | N | N | N |
| 83 | DimissionDate | dimissionDate | 离职日期 | date | 10 |  |  | N | N | Y |
| 84 | DimissionCause | dimissionCause | 离职原因 | nvarchar | 50 |  |  | N | N | Y |
| 85 | DimissionWhereabouts | dimissionWhereabouts | 离职去向 | nvarchar | 100 |  |  | N | N | Y |
| 86 | Ext1 | ext1 | 扩展信息1 | nvarchar | 4000 |  |  | N | N | Y |
| 87 | Ext2 | ext2 | 扩展信息2 | nvarchar | 4000 |  |  | N | N | Y |
| 88 | Ext3 | ext3 | 扩展信息3 | nvarchar | 4000 |  |  | N | N | Y |
| 89 | Ext4 | ext4 | 扩展信息4 | nvarchar | 4000 |  |  | N | N | Y |
| 90 | Ext5 | ext5 | 扩展信息5 | nvarchar | 4000 |  |  | N | N | Y |
| 91 | Description | description | 描述 | nvarchar | 4000 |  |  | N | N | Y |
| 92 | SortCode | sortCode | 排序编号 | int | 10 |  | 0 | N | N | N |
| 93 | Deleted | deleted | 是否删除 | tinyint | 3 |  | 0 | N | N | N |
| 94 | Enabled | enabled | 是否有效 | tinyint | 3 |  | 1 | N | N | N |
| 95 | CreateTime | createTime | 创建时间 | datetime | 23 |  | getdate() | N | N | N |
| 96 | CreateUserId | createUserId | 创建人编号 | int | 10 |  | 0 | N | N | N |
| 97 | CreateUserName | createUserName | 创建人用户名 | nvarchar | 50 |  |  | N | N | Y |
| 98 | CreateBy | createBy | 创建人姓名 | nvarchar | 50 |  |  | N | N | Y |
| 99 | CreateIp | createIp | 创建IP | nvarchar | 50 |  |  | N | N | Y |
| 100 | UpdateTime | updateTime | 修改时间 | datetime | 23 |  | getdate() | N | N | N |
| 101 | UpdateUserId | updateUserId | 修改人编号 | int | 10 |  | 0 | N | N | N |
| 102 | UpdateUserName | updateUserName | 修改人用户名 | nvarchar | 50 |  |  | N | N | Y |
| 103 | UpdateBy | updateBy | 修改人姓名 | nvarchar | 50 |  |  | N | N | Y |
| 104 | UpdateIp | updateIp | 修改IP | nvarchar | 50 |  |  | N | N | Y |

## BaseUploadLog

**表描述**: 文件上传日志

| 序号 | 字段名 | 小写字段名 | 字段描述 | 类型 | 长度 | 小数 | 默认值 | 主键 | 自增 | 允许空 |
|---|---|---|---|---|---|---|---|---|---|---|
| 1 | Id | id | 编号 | int | 10 |  |  | Y | Y | N |
| 2 | SystemCode | systemCode | 子系统编码 | nvarchar | 50 |  | Base | N | N | N |
| 3 | UserCompanyId | userCompanyId | 公司编号 | int | 10 |  | 0 | N | N | N |
| 4 | UserSubCompanyId | userSubCompanyId | 子公司编号 | int | 10 |  | 0 | N | N | N |
| 5 | FileName | fileName | 文件名 | nvarchar | 100 |  |  | N | N | Y |
| 6 | FileExtension | fileExtension | 文件扩展名 | nvarchar | 100 |  |  | N | N | Y |
| 7 | FilePath | filePath | 文件名 | nvarchar | 200 |  |  | N | N | Y |
| 8 | FileSize | fileSize | 文件大小 | int | 10 |  |  | N | N | Y |
| 9 | Remark | remark | 备注 | nvarchar | 4000 |  |  | N | N | Y |
| 10 | SortCode | sortCode | 排序编号 | int | 10 |  | 0 | N | N | N |
| 11 | Deleted | deleted | 是否删除 | tinyint | 3 |  | 0 | N | N | N |
| 12 | Enabled | enabled | 是否有效 | tinyint | 3 |  | 1 | N | N | N |
| 13 | CreateTime | createTime | 创建时间 | datetime | 23 |  | getdate() | N | N | N |
| 14 | CreateUserId | createUserId | 创建人编号 | int | 10 |  | 0 | N | N | N |
| 15 | CreateUserName | createUserName | 创建人用户名 | nvarchar | 50 |  |  | N | N | Y |
| 16 | CreateBy | createBy | 创建人姓名 | nvarchar | 50 |  |  | N | N | Y |
| 17 | CreateIp | createIp | 创建IP | nvarchar | 50 |  |  | N | N | Y |
| 18 | UpdateTime | updateTime | 修改时间 | datetime | 23 |  | getdate() | N | N | N |
| 19 | UpdateUserId | updateUserId | 修改人编号 | int | 10 |  | 0 | N | N | N |
| 20 | UpdateUserName | updateUserName | 修改人用户名 | nvarchar | 50 |  |  | N | N | Y |
| 21 | UpdateBy | updateBy | 修改人姓名 | nvarchar | 50 |  |  | N | N | Y |
| 22 | UpdateIp | updateIp | 修改IP | nvarchar | 50 |  |  | N | N | Y |

## BaseUser

**表描述**: 用户账号

| 序号 | 字段名 | 小写字段名 | 字段描述 | 类型 | 长度 | 小数 | 默认值 | 主键 | 自增 | 允许空 |
|---|---|---|---|---|---|---|---|---|---|---|
| 1 | Id | id | 主键 | int | 10 |  |  | Y | Y | N |
| 2 | UserFrom | userFrom | 来源 | nvarchar | 100 |  |  | N | N | Y |
| 3 | UserName | userName | 用户名 | nvarchar | 50 |  |  | N | N | N |
| 4 | RealName | realName | 姓名 | nvarchar | 50 |  |  | N | N | Y |
| 5 | NickName | nickName | 呢称 | nvarchar | 50 |  |  | N | N | Y |
| 6 | AvatarUrl | avatarUrl | 头像 | nvarchar | 512 |  |  | N | N | Y |
| 7 | Code | code | 编号 | nvarchar | 50 |  |  | N | N | Y |
| 8 | EmployeeNumber | employeeNumber | 工号 | nvarchar | 50 |  |  | N | N | Y |
| 9 | IdCard | idCard | 身份证号码 | nvarchar | 50 |  |  | N | N | Y |
| 10 | QuickQuery | quickQuery | 快速查询 | nvarchar | 100 |  |  | N | N | Y |
| 11 | SimpleSpelling | simpleSpelling | 简拼 | nvarchar | 50 |  |  | N | N | Y |
| 12 | CompanyId | companyId | 公司主键 | int | 10 |  | 0 | N | N | N |
| 13 | CompanyCode | companyCode | 公司编码 | nvarchar | 50 |  |  | N | N | Y |
| 14 | CompanyName | companyName | 公司名称 | nvarchar | 50 |  |  | N | N | Y |
| 15 | SubCompanyId | subCompanyId | 分支机构主键 | int | 10 |  | 0 | N | N | N |
| 16 | SubCompanyName | subCompanyName | 分支机构名称 | nvarchar | 50 |  |  | N | N | Y |
| 17 | DepartmentId | departmentId | 部门主键 | int | 10 |  | 0 | N | N | N |
| 18 | DepartmentName | departmentName | 部门名称 | nvarchar | 50 |  |  | N | N | Y |
| 19 | SubDepartmentId | subDepartmentId | 子部门主键 | int | 10 |  | 0 | N | N | N |
| 20 | SubDepartmentName | subDepartmentName | 子部门名称 | nvarchar | 50 |  |  | N | N | Y |
| 21 | WorkgroupId | workgroupId | 工作组主键 | int | 10 |  | 0 | N | N | N |
| 22 | WorkgroupName | workgroupName | 工作组名称 | nvarchar | 50 |  |  | N | N | Y |
| 23 | WorkCategory | workCategory | 工作行业 | nvarchar | 50 |  |  | N | N | Y |
| 24 | SecurityLevel | securityLevel | 安全级别 | int | 10 |  | 0 | N | N | N |
| 25 | Title | title | 职称、职位 | nvarchar | 50 |  |  | N | N | Y |
| 26 | Duty | duty | 岗位 | nvarchar | 50 |  |  | N | N | Y |
| 27 | Lang | lang | 语言 | nvarchar | 50 |  | CN | N | N | Y |
| 28 | Gender | gender | 性别 | nvarchar | 50 |  |  | N | N | Y |
| 29 | Birthday | birthday | 生日 | date | 10 |  |  | N | N | Y |
| 30 | Score | score | 积分 | int | 10 |  | 0 | N | N | Y |
| 31 | Fans | fans | 粉丝数量 | int | 10 |  | 0 | N | N | Y |
| 32 | HomeAddress | homeAddress | 家庭住址 | nvarchar | 200 |  |  | N | N | Y |
| 33 | Signature | signature | 个性签名 | nvarchar | 200 |  |  | N | N | Y |
| 34 | Theme | theme | 系统样式选择 | nvarchar | 50 |  |  | N | N | Y |
| 35 | IsStaff | isStaff | 是否员工 | tinyint | 3 |  | 0 | N | N | N |
| 36 | IsVisible | isVisible | 是否显示 | tinyint | 3 |  | 1 | N | N | N |
| 37 | Country | country | 国家 | nvarchar | 50 |  |  | N | N | Y |
| 38 | State | state | 州 | nvarchar | 50 |  |  | N | N | Y |
| 39 | Province | province | 省份 | nvarchar | 50 |  |  | N | N | Y |
| 40 | City | city | 城市 | nvarchar | 50 |  |  | N | N | Y |
| 41 | District | district | 区域 | nvarchar | 50 |  |  | N | N | Y |
| 42 | AuditStatus | auditStatus | 审核状态 | nvarchar | 50 |  |  | N | N | Y |
| 43 | ManagerUserId | managerUserId | 经理用户编号 | int | 10 |  | 0 | N | N | N |
| 44 | IsAdministrator | isAdministrator | 是否系统管理员 | tinyint | 3 |  | 0 | N | N | N |
| 45 | IsCheckBalance | isCheckBalance | 是否检查余额 | tinyint | 3 |  | 0 | N | N | N |
| 46 | Description | description | 描述 | nvarchar | 4000 |  |  | N | N | Y |
| 47 | SortCode | sortCode | 排序编号 | int | 10 |  | 0 | N | N | N |
| 48 | Deleted | deleted | 是否删除 | tinyint | 3 |  | 0 | N | N | N |
| 49 | Enabled | enabled | 是否有效 | tinyint | 3 |  | 1 | N | N | N |
| 50 | CreateTime | createTime | 创建时间 | datetime | 23 |  | getdate() | N | N | N |
| 51 | CreateUserId | createUserId | 创建人编号 | int | 10 |  | 0 | N | N | N |
| 52 | CreateUserName | createUserName | 创建人用户名 | nvarchar | 50 |  |  | N | N | Y |
| 53 | CreateBy | createBy | 创建人姓名 | nvarchar | 50 |  |  | N | N | Y |
| 54 | CreateIp | createIp | 创建IP | nvarchar | 50 |  |  | N | N | Y |
| 55 | UpdateTime | updateTime | 修改时间 | datetime | 23 |  | getdate() | N | N | N |
| 56 | UpdateUserId | updateUserId | 修改人编号 | int | 10 |  | 0 | N | N | N |
| 57 | UpdateUserName | updateUserName | 修改人用户名 | nvarchar | 50 |  |  | N | N | Y |
| 58 | UpdateBy | updateBy | 修改人姓名 | nvarchar | 50 |  |  | N | N | Y |
| 59 | UpdateIp | updateIp | 修改IP | nvarchar | 50 |  |  | N | N | Y |

## BaseUserContact

**表描述**: 用户联系方式

| 序号 | 字段名 | 小写字段名 | 字段描述 | 类型 | 长度 | 小数 | 默认值 | 主键 | 自增 | 允许空 |
|---|---|---|---|---|---|---|---|---|---|---|
| 1 | Id | id | 主键 | int | 10 |  |  | Y | Y | N |
| 2 | UserId | userId | 用户编号 | int | 10 |  | 0 | N | N | N |
| 3 | ShowEmail | showEmail | 显示邮箱 | tinyint | 3 |  | 1 | N | N | N |
| 4 | Email | email | 邮箱 | nvarchar | 100 |  |  | N | N | Y |
| 5 | EmailValidated | emailValidated | 邮箱是否验证 | tinyint | 3 |  | 0 | N | N | N |
| 6 | ShowMobile | showMobile | 显示手机 | tinyint | 3 |  | 1 | N | N | Y |
| 7 | Mobile | mobile | 手机 | nvarchar | 50 |  |  | N | N | Y |
| 8 | MobileValidated | mobileValidated | 手机是否验证 | tinyint | 3 |  | 0 | N | N | N |
| 9 | MobileValidatedTime | mobileValidatedTime | 手机验证时间 | datetime | 23 |  |  | N | N | Y |
| 10 | ShortNumber | shortNumber | 短号 | nvarchar | 50 |  |  | N | N | Y |
| 11 | Telephone | telephone | 电话 | nvarchar | 50 |  |  | N | N | Y |
| 12 | Extension | extension | 分机 | nvarchar | 50 |  |  | N | N | Y |
| 13 | QQ | qq | QQ | nvarchar | 50 |  |  | N | N | Y |
| 14 | WW | ww | 旺旺 | nvarchar | 50 |  |  | N | N | Y |
| 15 | IM | im | 即时通讯 | nvarchar | 50 |  |  | N | N | Y |
| 16 | WeChat | weChat | 微信 | nvarchar | 50 |  |  | N | N | Y |
| 17 | WeChatValidated | weChatValidated | 微信是否验证 | tinyint | 3 |  | 0 | N | N | N |
| 18 | WeChatOpenId | weChatOpenId | 微信OpenId | nvarchar | 50 |  |  | N | N | Y |
| 19 | CompanyId | companyId | 公司主键 | int | 10 |  | 0 | N | N | N |
| 20 | CompanyEmail | companyEmail | 公司邮箱 | nvarchar | 50 |  |  | N | N | Y |
| 21 | EmergencyContact | emergencyContact | 紧急联系人 | nvarchar | 50 |  |  | N | N | Y |
| 22 | EmergencyMobile | emergencyMobile | 紧急联系手机 | nvarchar | 50 |  |  | N | N | Y |
| 23 | EmergencyTelephone | emergencyTelephone | 紧急联系电话 | nvarchar | 50 |  |  | N | N | Y |
| 24 | SortCode | sortCode | 排序编号 | int | 10 |  | 0 | N | N | N |
| 25 | Deleted | deleted | 是否删除 | tinyint | 3 |  | 0 | N | N | N |
| 26 | Enabled | enabled | 是否有效 | tinyint | 3 |  | 1 | N | N | N |
| 27 | CreateTime | createTime | 创建时间 | datetime | 23 |  | getdate() | N | N | N |
| 28 | CreateUserId | createUserId | 创建人编号 | int | 10 |  | 0 | N | N | N |
| 29 | CreateUserName | createUserName | 创建人用户名 | nvarchar | 50 |  |  | N | N | Y |
| 30 | CreateBy | createBy | 创建人姓名 | nvarchar | 50 |  |  | N | N | Y |
| 31 | CreateIp | createIp | 创建IP | nvarchar | 50 |  |  | N | N | Y |
| 32 | UpdateTime | updateTime | 修改时间 | datetime | 23 |  | getdate() | N | N | N |
| 33 | UpdateUserId | updateUserId | 修改人编号 | int | 10 |  | 0 | N | N | N |
| 34 | UpdateUserName | updateUserName | 修改人用户名 | nvarchar | 50 |  |  | N | N | Y |
| 35 | UpdateBy | updateBy | 修改人姓名 | nvarchar | 50 |  |  | N | N | Y |
| 36 | UpdateIp | updateIp | 修改IP | nvarchar | 50 |  |  | N | N | Y |

## BaseUserLogon

**表描述**: 用户登录信息

| 序号 | 字段名 | 小写字段名 | 字段描述 | 类型 | 长度 | 小数 | 默认值 | 主键 | 自增 | 允许空 |
|---|---|---|---|---|---|---|---|---|---|---|
| 1 | Id | id | 主键 | int | 10 |  |  | Y | Y | N |
| 2 | UserId | userId | 用户编号 | int | 10 |  | 0 | N | N | N |
| 3 | UserPassword | userPassword | 用户密码 | nvarchar | 100 |  |  | N | N | Y |
| 4 | OpenId | openId | 单点登录标识 | nvarchar | 50 |  | newid() | N | N | N |
| 5 | AllowStartTime | allowStartTime | 允许登录时间开始 | datetime | 23 |  |  | N | N | Y |
| 6 | AllowEndTime | allowEndTime | 允许登录时间结束 | datetime | 23 |  |  | N | N | Y |
| 7 | LockStartTime | lockStartTime | 暂停用户开始日期 | datetime | 23 |  |  | N | N | Y |
| 8 | LockEndTime | lockEndTime | 暂停用户结束日期 | datetime | 23 |  |  | N | N | Y |
| 9 | FirstVisitTime | firstVisitTime | 第一次访问时间 | datetime | 23 |  |  | N | N | Y |
| 10 | PreviousVisitTime | previousVisitTime | 上一次访问时间 | datetime | 23 |  |  | N | N | Y |
| 11 | LastVisitTime | lastVisitTime | 最后访问时间 | datetime | 23 |  |  | N | N | Y |
| 12 | ChangePasswordTime | changePasswordTime | 最后修改密码日期 | datetime | 23 |  |  | N | N | Y |
| 13 | LogonCount | logonCount | 登录次数 | int | 10 |  | 0 | N | N | N |
| 14 | ConcurrentUser | concurrentUser | 是否并发用户 | tinyint | 3 |  | 0 | N | N | N |
| 15 | ShowCount | showCount | 展示次数 | tinyint | 3 |  | 0 | N | N | N |
| 16 | PasswordErrorCount | passwordErrorCount | 密码连续错误次数 | int | 10 |  | 0 | N | N | N |
| 17 | UserOnline | userOnline | 在线状态 | tinyint | 3 |  | 0 | N | N | N |
| 18 | CheckIpAddress | checkIpAddress | IP访问限制 | tinyint | 3 |  | 0 | N | N | N |
| 19 | VerificationCode | verificationCode | 验证码 | nvarchar | 50 |  |  | N | N | Y |
| 20 | IpAddress | ipAddress | 登录IP地址 | nvarchar | 50 |  |  | N | N | Y |
| 21 | MacAddress | macAddress | 登录MAC地址 | nvarchar | 50 |  |  | N | N | Y |
| 22 | Question | question | 密码提示问题 | nvarchar | 50 |  |  | N | N | Y |
| 23 | AnswerQuestion | answerQuestion | 密码提示答案 | nvarchar | 200 |  |  | N | N | Y |
| 24 | Salt | salt | 密码加盐 | nvarchar | 50 |  |  | N | N | Y |
| 25 | OpenIdTimeoutTime | openIdTimeoutTime | OpenId过期时间 | datetime | 23 |  |  | N | N | Y |
| 26 | SystemCode | systemCode | 子系统编码 | nvarchar | 50 |  |  | N | N | Y |
| 27 | IpAddressName | ipAddressName | IP地址所在位置名称 | nvarchar | 50 |  |  | N | N | Y |
| 28 | PasswordStrength | passwordStrength | 密码强度 | tinyint | 3 |  |  | N | N | Y |
| 29 | ComputerName | computerName | 电脑名 | nvarchar | 50 |  |  | N | N | Y |
| 30 | NeedModifyPassword | needModifyPassword | 是否需要修改密码 | tinyint | 3 |  | 0 | N | N | N |
| 31 | SortCode | sortCode | 排序编号 | int | 10 |  | 0 | N | N | N |
| 32 | Deleted | deleted | 是否删除 | tinyint | 3 |  | 0 | N | N | N |
| 33 | Enabled | enabled | 是否有效 | tinyint | 3 |  | 1 | N | N | N |
| 34 | CreateTime | createTime | 创建时间 | datetime | 23 |  | getdate() | N | N | N |
| 35 | CreateUserId | createUserId | 创建人编号 | int | 10 |  | 0 | N | N | N |
| 36 | CreateUserName | createUserName | 创建人用户名 | nvarchar | 50 |  |  | N | N | Y |
| 37 | CreateBy | createBy | 创建人姓名 | nvarchar | 50 |  |  | N | N | Y |
| 38 | CreateIp | createIp | 创建IP | nvarchar | 50 |  |  | N | N | Y |
| 39 | UpdateTime | updateTime | 修改时间 | datetime | 23 |  | getdate() | N | N | N |
| 40 | UpdateUserId | updateUserId | 修改人编号 | int | 10 |  | 0 | N | N | N |
| 41 | UpdateUserName | updateUserName | 修改人用户名 | nvarchar | 50 |  |  | N | N | Y |
| 42 | UpdateBy | updateBy | 修改人姓名 | nvarchar | 50 |  |  | N | N | Y |
| 43 | UpdateIp | updateIp | 修改IP | nvarchar | 50 |  |  | N | N | Y |

## BaseUserOAuth

**表描述**: 用户OAuth

| 序号 | 字段名 | 小写字段名 | 字段描述 | 类型 | 长度 | 小数 | 默认值 | 主键 | 自增 | 允许空 |
|---|---|---|---|---|---|---|---|---|---|---|
| 1 | Id | id | 主键 | int | 10 |  |  | Y | Y | N |
| 2 | SystemCode | systemCode | 子系统编码 | nvarchar | 50 |  |  | N | N | Y |
| 3 | UserId | userId | 用户编号 | int | 10 |  |  | N | N | N |
| 4 | Name | name | OAuth Name | nvarchar | 50 |  |  | N | N | N |
| 5 | AccessToken | accessToken | OAuth Access Token | nvarchar | 4000 |  |  | N | N | N |
| 6 | RefreshToken | refreshToken | OAuth Refresh Token | nvarchar | 4000 |  |  | N | N | Y |
| 7 | OpenId | openId | OAuth OpenId | nvarchar | 200 |  |  | N | N | N |
| 8 | UnionId | unionId | OAuth UnionId | nvarchar | 200 |  |  | N | N | Y |
| 9 | Description | description | 描述 | nvarchar | 4000 |  |  | N | N | Y |
| 10 | SortCode | sortCode | 排序编号 | int | 10 |  | 0 | N | N | N |
| 11 | Deleted | deleted | 是否删除 | tinyint | 3 |  | 0 | N | N | N |
| 12 | Enabled | enabled | 是否有效 | tinyint | 3 |  | 1 | N | N | N |
| 13 | CreateTime | createTime | 创建时间 | datetime | 23 |  | getdate() | N | N | N |
| 14 | CreateUserId | createUserId | 创建人编号 | int | 10 |  | 0 | N | N | N |
| 15 | CreateUserName | createUserName | 创建人用户名 | nvarchar | 50 |  |  | N | N | Y |
| 16 | CreateBy | createBy | 创建人姓名 | nvarchar | 50 |  |  | N | N | Y |
| 17 | CreateIp | createIp | 创建IP | nvarchar | 50 |  |  | N | N | Y |
| 18 | UpdateTime | updateTime | 修改时间 | datetime | 23 |  | getdate() | N | N | N |
| 19 | UpdateUserId | updateUserId | 修改人编号 | int | 10 |  | 0 | N | N | N |
| 20 | UpdateUserName | updateUserName | 修改人用户名 | nvarchar | 50 |  |  | N | N | Y |
| 21 | UpdateBy | updateBy | 修改人姓名 | nvarchar | 50 |  |  | N | N | Y |
| 22 | UpdateIp | updateIp | 修改IP | nvarchar | 50 |  |  | N | N | Y |

## BaseUserOrganization

**表描述**: 用户兼任

| 序号 | 字段名 | 小写字段名 | 字段描述 | 类型 | 长度 | 小数 | 默认值 | 主键 | 自增 | 允许空 |
|---|---|---|---|---|---|---|---|---|---|---|
| 1 | Id | id | 主键 | int | 10 |  |  | Y | Y | N |
| 2 | UserId | userId | 用户账户主键 | int | 10 |  | 0 | N | N | N |
| 3 | CompanyId | companyId | 公司主键 | int | 10 |  | 0 | N | N | N |
| 4 | SubCompanyId | subCompanyId | 分支机构主键 | int | 10 |  | 0 | N | N | N |
| 5 | DepartmentId | departmentId | 部门主键 | int | 10 |  | 0 | N | N | N |
| 6 | SubDepartmentId | subDepartmentId | 子部门主键 | int | 10 |  | 0 | N | N | N |
| 7 | WorkgroupId | workgroupId | 工作组主键 | int | 10 |  | 0 | N | N | N |
| 8 | Description | description | 描述 | nvarchar | 4000 |  |  | N | N | Y |
| 9 | SortCode | sortCode | 排序编号 | int | 10 |  | 0 | N | N | N |
| 10 | Deleted | deleted | 是否删除 | tinyint | 3 |  | 0 | N | N | N |
| 11 | Enabled | enabled | 是否有效 | tinyint | 3 |  | 1 | N | N | N |
| 12 | CreateTime | createTime | 创建时间 | datetime | 23 |  | getdate() | N | N | N |
| 13 | CreateUserId | createUserId | 创建人编号 | int | 10 |  | 0 | N | N | N |
| 14 | CreateUserName | createUserName | 创建人用户名 | nvarchar | 50 |  |  | N | N | Y |
| 15 | CreateBy | createBy | 创建人姓名 | nvarchar | 50 |  |  | N | N | Y |
| 16 | CreateIp | createIp | 创建IP | nvarchar | 50 |  |  | N | N | Y |
| 17 | UpdateTime | updateTime | 修改时间 | datetime | 23 |  | getdate() | N | N | N |
| 18 | UpdateUserId | updateUserId | 修改人编号 | int | 10 |  | 0 | N | N | N |
| 19 | UpdateUserName | updateUserName | 修改人用户名 | nvarchar | 50 |  |  | N | N | Y |
| 20 | UpdateBy | updateBy | 修改人姓名 | nvarchar | 50 |  |  | N | N | Y |
| 21 | UpdateIp | updateIp | 修改IP | nvarchar | 50 |  |  | N | N | Y |

## BaseUserRole

**表描述**: 用户角色

| 序号 | 字段名 | 小写字段名 | 字段描述 | 类型 | 长度 | 小数 | 默认值 | 主键 | 自增 | 允许空 |
|---|---|---|---|---|---|---|---|---|---|---|
| 1 | Id | id | 主键 | int | 10 |  |  | Y | Y | N |
| 2 | SystemCode | systemCode | 子系统编码 | nvarchar | 50 |  | Base | N | N | N |
| 3 | UserId | userId | 用户主键 | int | 10 |  |  | N | N | N |
| 4 | RoleId | roleId | 角色主键 | int | 10 |  |  | N | N | N |
| 5 | Description | description | 描述 | nvarchar | 4000 |  |  | N | N | Y |
| 6 | SortCode | sortCode | 排序编号 | int | 10 |  | 0 | N | N | N |
| 7 | Deleted | deleted | 是否删除 | tinyint | 3 |  | 0 | N | N | N |
| 8 | Enabled | enabled | 是否有效 | tinyint | 3 |  | 1 | N | N | N |
| 9 | CreateTime | createTime | 创建时间 | datetime | 23 |  | getdate() | N | N | N |
| 10 | CreateUserId | createUserId | 创建人编号 | int | 10 |  | 0 | N | N | N |
| 11 | CreateUserName | createUserName | 创建人用户名 | nvarchar | 50 |  |  | N | N | Y |
| 12 | CreateBy | createBy | 创建人姓名 | nvarchar | 50 |  |  | N | N | Y |
| 13 | CreateIp | createIp | 创建IP | nvarchar | 50 |  |  | N | N | Y |
| 14 | UpdateTime | updateTime | 修改时间 | datetime | 23 |  | getdate() | N | N | N |
| 15 | UpdateUserId | updateUserId | 修改人编号 | int | 10 |  | 0 | N | N | N |
| 16 | UpdateBy | updateBy | 修改人姓名 | nvarchar | 50 |  |  | N | N | Y |
| 17 | UpdateUserName | updateUserName | 修改人用户名 | nvarchar | 50 |  |  | N | N | Y |
| 18 | UpdateIp | updateIp | 修改IP | nvarchar | 50 |  |  | N | N | Y |

## BusinessModule

**表描述**: 模块菜单

| 序号 | 字段名 | 小写字段名 | 字段描述 | 类型 | 长度 | 小数 | 默认值 | 主键 | 自增 | 允许空 |
|---|---|---|---|---|---|---|---|---|---|---|
| 1 | Id | id | 主键 | int | 10 |  |  | Y | Y | N |
| 2 | SystemCode | systemCode | 子系统编码 | nvarchar | 50 |  | Base | N | N | N |
| 3 | ParentId | parentId | 父节点主键 | int | 10 |  | 0 | N | N | N |
| 4 | Code | code | 编码 | nvarchar | 100 |  |  | N | N | Y |
| 5 | Name | name | 名称 | nvarchar | 100 |  |  | N | N | Y |
| 6 | CategoryCode | categoryCode | 菜单分类System,Application | nvarchar | 50 |  |  | N | N | Y |
| 7 | ImageUrl | imageUrl | 图标位置 | nvarchar | 200 |  |  | N | N | Y |
| 8 | ImageIndex | imageIndex | 图标编号 | nvarchar | 50 |  |  | N | N | Y |
| 9 | SelectedImageIndex | selectedImageIndex | 选中状态图标编号 | nvarchar | 50 |  |  | N | N | Y |
| 10 | NavigateUrl | navigateUrl | Web网址 | nvarchar | 200 |  |  | N | N | Y |
| 11 | Target | target | 目标窗体中打开BS | nvarchar | 100 |  |  | N | N | Y |
| 12 | FormName | formName | 窗体名CS | nvarchar | 100 |  |  | N | N | Y |
| 13 | AssemblyName | assemblyName | 动态连接库CS | nvarchar | 100 |  |  | N | N | Y |
| 14 | PermissionScopeTables | permissionScopeTables | 需要数据权限过滤的表(,符号分割) | nvarchar | 500 |  |  | N | N | Y |
| 15 | IsMenu | isMenu | 是菜单项 | tinyint | 3 |  | 1 | N | N | Y |
| 16 | IsPublic | isPublic | 是否公开 | tinyint | 3 |  | 0 | N | N | N |
| 17 | IsExpand | isExpand | 是否展开 | tinyint | 3 |  | 0 | N | N | N |
| 18 | IsScope | isScope | 权限域 | tinyint | 3 |  | 0 | N | N | N |
| 19 | IsVisible | isVisible | 是否可见 | tinyint | 3 |  | 1 | N | N | N |
| 20 | AllowEdit | allowEdit | 允许编辑 | tinyint | 3 |  | 1 | N | N | N |
| 21 | AllowDelete | allowDelete | 允许删除 | tinyint | 3 |  | 1 | N | N | N |
| 22 | LastCall | lastCall | 最后呼叫时间 | datetime | 23 |  |  | N | N | Y |
| 23 | WebBrowser | webBrowser | 浏览器 | nvarchar | 50 |  |  | N | N | Y |
| 24 | AuthorizedDays | authorizedDays | 认证天数 | int | 10 |  | 0 | N | N | Y |
| 25 | Description | description | 描述 | nvarchar | 4000 |  |  | N | N | Y |
| 26 | SortCode | sortCode | 排序编号 | int | 10 |  | 0 | N | N | N |
| 27 | Deleted | deleted | 是否删除 | tinyint | 3 |  | 0 | N | N | N |
| 28 | Enabled | enabled | 是否有效 | tinyint | 3 |  | 1 | N | N | N |
| 29 | CreateTime | createTime | 创建时间 | datetime | 23 |  | getdate() | N | N | N |
| 30 | CreateUserId | createUserId | 创建人编号 | int | 10 |  | 0 | N | N | N |
| 31 | CreateUserName | createUserName | 创建人用户名 | nvarchar | 50 |  |  | N | N | Y |
| 32 | CreateBy | createBy | 创建人姓名 | nvarchar | 50 |  |  | N | N | Y |
| 33 | CreateIp | createIp | 创建IP | nvarchar | 50 |  |  | N | N | Y |
| 34 | UpdateTime | updateTime | 修改时间 | datetime | 23 |  | getdate() | N | N | N |
| 35 | UpdateUserId | updateUserId | 修改人编号 | int | 10 |  | 0 | N | N | N |
| 36 | UpdateUserName | updateUserName | 修改人用户名 | nvarchar | 50 |  |  | N | N | Y |
| 37 | UpdateBy | updateBy | 修改人姓名 | nvarchar | 50 |  |  | N | N | Y |
| 38 | UpdateIp | updateIp | 修改IP | nvarchar | 50 |  |  | N | N | Y |

## BusinessOrganizationScope

**表描述**: 基于组织机构的权限范围

| 序号 | 字段名 | 小写字段名 | 字段描述 | 类型 | 长度 | 小数 | 默认值 | 主键 | 自增 | 允许空 |
|---|---|---|---|---|---|---|---|---|---|---|
| 1 | Id | id | 主键 | int | 10 |  |  | Y | Y | N |
| 2 | SystemCode | systemCode | 子系统编码 | nvarchar | 50 |  | Base | N | N | N |
| 3 | ResourceCategory | resourceCategory | 什么类型的 | nvarchar | 50 |  |  | N | N | Y |
| 4 | ResourceId | resourceId | 什么资源主键 | nvarchar | 50 |  |  | N | N | Y |
| 5 | PermissionId | permissionId | 有什么权限（模块菜单）主键 | int | 10 |  |  | N | N | Y |
| 6 | AllData | allData | 全部数据 | tinyint | 3 |  |  | N | N | Y |
| 7 | Province | province | 所在的省 | tinyint | 3 |  |  | N | N | Y |
| 8 | City | city | 所在的市 | tinyint | 3 |  |  | N | N | Y |
| 9 | District | district | 所在的县/区 | tinyint | 3 |  |  | N | N | Y |
| 10 | Street | street | 街道 | tinyint | 3 |  |  | N | N | Y |
| 11 | UserCompany | userCompany | 用户所在公司的数据 | tinyint | 3 |  |  | N | N | Y |
| 12 | UserSubCompany | userSubCompany | 用户所在分公司的数据 | tinyint | 3 |  |  | N | N | Y |
| 13 | UserDepartment | userDepartment | 用户所在部门的数据 | tinyint | 3 |  |  | N | N | Y |
| 14 | UserSubDepartment | userSubDepartment | 用户所在子部门的数据 | tinyint | 3 |  |  | N | N | Y |
| 15 | UserWorkgroup | userWorkgroup | 用户所在工作组的数据 | tinyint | 3 |  |  | N | N | Y |
| 16 | OnlyOwnData | onlyOwnData | 仅仅用户自己的数据 | tinyint | 3 |  | 1 | N | N | N |
| 17 | NotAllowed | notAllowed | 不允许查看数据 | tinyint | 3 |  | 0 | N | N | Y |
| 18 | ByDetails | byDetails | 按详细设置 | tinyint | 3 |  | 0 | N | N | Y |
| 19 | ContainChild | containChild | 包含子节点的数据 | tinyint | 3 |  |  | N | N | N |
| 20 | Description | description | 描述 | nvarchar | 4000 |  |  | N | N | Y |
| 21 | SortCode | sortCode | 排序编号 | int | 10 |  | 0 | N | N | N |
| 22 | Deleted | deleted | 是否删除 | tinyint | 3 |  | 0 | N | N | N |
| 23 | Enabled | enabled | 是否有效 | tinyint | 3 |  | 1 | N | N | N |
| 24 | CreateTime | createTime | 创建时间 | datetime | 23 |  | getdate() | N | N | N |
| 25 | CreateUserId | createUserId | 创建人编号 | int | 10 |  | 0 | N | N | N |
| 26 | CreateUserName | createUserName | 创建人用户名 | nvarchar | 50 |  |  | N | N | Y |
| 27 | CreateBy | createBy | 创建人姓名 | nvarchar | 50 |  |  | N | N | Y |
| 28 | CreateIp | createIp | 创建IP | nvarchar | 50 |  |  | N | N | Y |
| 29 | UpdateTime | updateTime | 修改时间 | datetime | 23 |  | getdate() | N | N | N |
| 30 | UpdateUserId | updateUserId | 修改人编号 | int | 10 |  | 0 | N | N | N |
| 31 | UpdateUserName | updateUserName | 修改人用户名 | nvarchar | 50 |  |  | N | N | Y |
| 32 | UpdateBy | updateBy | 修改人姓名 | nvarchar | 50 |  |  | N | N | Y |
| 33 | UpdateIp | updateIp | 修改IP | nvarchar | 50 |  |  | N | N | Y |

## BusinessPermission

**表描述**: 权限

| 序号 | 字段名 | 小写字段名 | 字段描述 | 类型 | 长度 | 小数 | 默认值 | 主键 | 自增 | 允许空 |
|---|---|---|---|---|---|---|---|---|---|---|
| 1 | Id | id | 主键 | int | 10 |  |  | Y | Y | N |
| 2 | SystemCode | systemCode | 子系统编码 | nvarchar | 50 |  | Base | N | N | N |
| 3 | ResourceCategory | resourceCategory | 资料类别 | nvarchar | 50 |  |  | N | N | Y |
| 4 | ResourceId | resourceId | 资源主键 | nvarchar | 50 |  |  | N | N | Y |
| 5 | PermissionId | permissionId | 权限（菜单模块）主键 | nvarchar | 50 |  |  | N | N | Y |
| 6 | CompanyId | companyId | 公司主键 | int | 10 |  | 0 | N | N | N |
| 7 | CompanyName | companyName | 公司名称 | nvarchar | 50 |  |  | N | N | Y |
| 8 | PermissionConstraint | permissionConstraint | 权限条件限制 | nvarchar | 200 |  |  | N | N | Y |
| 9 | Description | description | 描述 | nvarchar | 4000 |  |  | N | N | Y |
| 10 | SortCode | sortCode | 排序编号 | int | 10 |  | 0 | N | N | N |
| 11 | Deleted | deleted | 是否删除 | tinyint | 3 |  | 0 | N | N | N |
| 12 | Enabled | enabled | 是否有效 | tinyint | 3 |  | 1 | N | N | N |
| 13 | CreateTime | createTime | 创建时间 | datetime | 23 |  | getdate() | N | N | N |
| 14 | CreateUserId | createUserId | 创建人编号 | int | 10 |  | 0 | N | N | N |
| 15 | CreateUserName | createUserName | 创建人用户名 | nvarchar | 50 |  |  | N | N | Y |
| 16 | CreateBy | createBy | 创建人姓名 | nvarchar | 50 |  |  | N | N | Y |
| 17 | CreateIp | createIp | 创建IP | nvarchar | 50 |  |  | N | N | Y |
| 18 | UpdateTime | updateTime | 修改时间 | datetime | 23 |  | getdate() | N | N | N |
| 19 | UpdateUserId | updateUserId | 修改人编号 | int | 10 |  | 0 | N | N | N |
| 20 | UpdateUserName | updateUserName | 修改人用户名 | nvarchar | 50 |  |  | N | N | Y |
| 21 | UpdateBy | updateBy | 修改人姓名 | nvarchar | 50 |  |  | N | N | Y |
| 22 | UpdateIp | updateIp | 修改IP | nvarchar | 50 |  |  | N | N | Y |

## BusinessPermissionScope

**表描述**: 数据权限表

| 序号 | 字段名 | 小写字段名 | 字段描述 | 类型 | 长度 | 小数 | 默认值 | 主键 | 自增 | 允许空 |
|---|---|---|---|---|---|---|---|---|---|---|
| 1 | Id | id | 主键 | int | 10 |  |  | Y | Y | N |
| 2 | SystemCode | systemCode | 子系统编码 | nvarchar | 50 |  | Base | N | N | N |
| 3 | ResourceCategory | resourceCategory | 什么类型的 | nvarchar | 50 |  |  | N | N | Y |
| 4 | ResourceId | resourceId | 什么资源主键 | int | 10 |  |  | N | N | N |
| 5 | TargetCategory | targetCategory | 对什么类型的 | nvarchar | 50 |  |  | N | N | Y |
| 6 | TargetId | targetId | 对什么资源主键 | int | 10 |  |  | N | N | N |
| 7 | PermissionId | permissionId | 有什么权限（模块菜单）主键 | int | 10 |  |  | N | N | N |
| 8 | ContainChild | containChild | 包含子节点 | tinyint | 3 |  | 0 | N | N | N |
| 9 | PermissionConstraint | permissionConstraint | 有什么权限约束表达式 | nvarchar | 200 |  |  | N | N | Y |
| 10 | StartTime | startTime | 开始生效时间 | datetime | 23 |  |  | N | N | Y |
| 11 | EndTime | endTime | 结束生效时间 | datetime | 23 |  |  | N | N | Y |
| 12 | Description | description | 描述 | nvarchar | 4000 |  |  | N | N | Y |
| 13 | SortCode | sortCode | 排序编号 | int | 10 |  | 0 | N | N | N |
| 14 | Deleted | deleted | 是否删除 | tinyint | 3 |  | 0 | N | N | N |
| 15 | Enabled | enabled | 是否有效 | tinyint | 3 |  | 1 | N | N | N |
| 16 | CreateTime | createTime | 创建时间 | datetime | 23 |  | getdate() | N | N | N |
| 17 | CreateUserId | createUserId | 创建人编号 | int | 10 |  | 0 | N | N | N |
| 18 | CreateUserName | createUserName | 创建人用户名 | nvarchar | 50 |  |  | N | N | Y |
| 19 | CreateBy | createBy | 创建人姓名 | nvarchar | 50 |  |  | N | N | Y |
| 20 | CreateIp | createIp | 创建IP | nvarchar | 50 |  |  | N | N | Y |
| 21 | UpdateTime | updateTime | 修改时间 | datetime | 23 |  | getdate() | N | N | N |
| 22 | UpdateUserId | updateUserId | 修改人编号 | int | 10 |  | 0 | N | N | N |
| 23 | UpdateUserName | updateUserName | 修改人用户名 | nvarchar | 50 |  |  | N | N | Y |
| 24 | UpdateBy | updateBy | 修改人姓名 | nvarchar | 50 |  |  | N | N | Y |
| 25 | UpdateIp | updateIp | 修改IP | nvarchar | 50 |  |  | N | N | Y |

## BusinessRole

**表描述**: 角色

| 序号 | 字段名 | 小写字段名 | 字段描述 | 类型 | 长度 | 小数 | 默认值 | 主键 | 自增 | 允许空 |
|---|---|---|---|---|---|---|---|---|---|---|
| 1 | Id | id | 主键 | int | 10 |  |  | Y | Y | N |
| 2 | SystemCode | systemCode | 子系统编码 | nvarchar | 50 |  | Base | N | N | N |
| 3 | OrganizationId | organizationId | 组织机构主键 | int | 10 |  | 0 | N | N | N |
| 4 | Code | code | 角色编号 | nvarchar | 100 |  |  | N | N | Y |
| 5 | Name | name | 角色名称 | nvarchar | 200 |  |  | N | N | Y |
| 6 | CategoryCode | categoryCode | 角色分类 | nvarchar | 50 |  |  | N | N | Y |
| 7 | AllowEdit | allowEdit | 允许编辑 | tinyint | 3 |  | 1 | N | N | N |
| 8 | AllowDelete | allowDelete | 允许删除 | tinyint | 3 |  | 1 | N | N | N |
| 9 | IsVisible | isVisible | 是否显示 | tinyint | 3 |  | 1 | N | N | N |
| 10 | Description | description | 描述 | nvarchar | 4000 |  |  | N | N | Y |
| 11 | SortCode | sortCode | 排序编号 | int | 10 |  | 0 | N | N | N |
| 12 | Deleted | deleted | 是否删除 | tinyint | 3 |  | 0 | N | N | N |
| 13 | Enabled | enabled | 是否有效 | tinyint | 3 |  | 1 | N | N | N |
| 14 | CreateTime | createTime | 创建时间 | datetime | 23 |  | getdate() | N | N | N |
| 15 | CreateUserId | createUserId | 创建人编号 | int | 10 |  | 0 | N | N | N |
| 16 | CreateUserName | createUserName | 创建人用户名 | nvarchar | 50 |  |  | N | N | Y |
| 17 | CreateBy | createBy | 创建人姓名 | nvarchar | 50 |  |  | N | N | Y |
| 18 | CreateIp | createIp | 创建IP | nvarchar | 50 |  |  | N | N | Y |
| 19 | UpdateTime | updateTime | 修改时间 | datetime | 23 |  | getdate() | N | N | N |
| 20 | UpdateUserId | updateUserId | 修改人编号 | int | 10 |  | 0 | N | N | N |
| 21 | UpdateUserName | updateUserName | 修改人用户名 | nvarchar | 50 |  |  | N | N | Y |
| 22 | UpdateBy | updateBy | 修改人姓名 | nvarchar | 50 |  |  | N | N | Y |
| 23 | UpdateIp | updateIp | 修改IP | nvarchar | 50 |  |  | N | N | Y |

## BusinessUserRole

**表描述**: 用户角色

| 序号 | 字段名 | 小写字段名 | 字段描述 | 类型 | 长度 | 小数 | 默认值 | 主键 | 自增 | 允许空 |
|---|---|---|---|---|---|---|---|---|---|---|
| 1 | Id | id | 主键 | int | 10 |  |  | Y | Y | N |
| 2 | SystemCode | systemCode | 子系统编码 | nvarchar | 50 |  | Base | N | N | N |
| 3 | UserId | userId | 用户主键 | int | 10 |  |  | N | N | N |
| 4 | RoleId | roleId | 角色主键 | int | 10 |  |  | N | N | N |
| 5 | Description | description | 描述 | nvarchar | 4000 |  |  | N | N | Y |
| 6 | SortCode | sortCode | 排序编号 | int | 10 |  | 0 | N | N | N |
| 7 | Deleted | deleted | 是否删除 | tinyint | 3 |  | 0 | N | N | N |
| 8 | Enabled | enabled | 是否有效 | tinyint | 3 |  | 1 | N | N | N |
| 9 | CreateTime | createTime | 创建时间 | datetime | 23 |  | getdate() | N | N | N |
| 10 | CreateUserId | createUserId | 创建人编号 | int | 10 |  | 0 | N | N | N |
| 11 | CreateUserName | createUserName | 创建人用户名 | nvarchar | 50 |  |  | N | N | Y |
| 12 | CreateBy | createBy | 创建人姓名 | nvarchar | 50 |  |  | N | N | Y |
| 13 | CreateIp | createIp | 创建IP | nvarchar | 50 |  |  | N | N | Y |
| 14 | UpdateTime | updateTime | 修改时间 | datetime | 23 |  | getdate() | N | N | N |
| 15 | UpdateUserId | updateUserId | 修改人编号 | int | 10 |  | 0 | N | N | N |
| 16 | UpdateUserName | updateUserName | 修改人用户名 | nvarchar | 50 |  |  | N | N | Y |
| 17 | UpdateBy | updateBy | 修改人姓名 | nvarchar | 50 |  |  | N | N | Y |
| 18 | UpdateIp | updateIp | 修改IP | nvarchar | 50 |  |  | N | N | Y |
