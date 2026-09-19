# Msg#### 键语义化重命名映射表

> 方案：**C（直接改名）** —— 编号键从语言包彻底移除，只保留语义键。
> 前缀体系：**12 个**（`Label.*` 不单独成类，界面标签并入 `Common.*`）。
> 范围：248 个编号键全部处理。
> 状态：**已执行（2026-09-16）**，见文末「执行结果」。
>
> **2026-09-18 追加**：全部语义键已生成 **强类型入口**（`Msg.Typed.cs`）。
> 查到新键后，直接用 `Msg.<前缀>.<成员>` 即可，无需再写字符串键：

```
旧键  Msg0007
  ↓  （本表查得）
新键  Common.ParameterRequired
  ↓  （语言包文本含 {0}，故为方法）
强类型  Msg.Common.ParameterRequired("用户名")
```

> 判定规则：语言包文本**不含** `{n}` → 属性（`Msg.Common.UnknownError`）；
> **含** `{n}` → 方法（`Msg.Common.ParameterRequired(x)`）。

## 一、命名规则

- 格式 `<前缀>.<PascalCase>`，与已有 158 个语义键风格一致（如 `Logon.UserNotFound`）
- 前缀语义：
  | 前缀 | 含义 |
  |---|---|
  | `Common.*` | 通用提示、数据状态、并发冲突 + **全部界面字段标签** |
  | `Confirm.*` | 二次确认（"您确认…吗？"） |
  | `Validation.*` | 输入校验失败 |
  | `Result.*` | 操作结果（成功/失败） |
  | `Logon.*` | 登录、用户、密码（已有 14 个，本次新增 41 个） |
  | `Ip.*` | IP / MAC 地址限制 |
  | `File.*` | 文件上传、导出、文档 |
  | `Sequence.*` | 序列 / 编号生成 |
  | `Org.*` | 组织机构、部门、角色 |
  | `Workflow.*` | 工作流、审核流转 |
  | `System.*` | 系统、连接、服务状态 |
  | `Sign.*` | 签名 / 通讯配置（原 `Msgs*` / `Msgc*`） |

## 二、删除与合并（共 7 个编号键消失）

| 旧键 | 中文值 | 处置 | 原因 |
|---|---|---|---|
| `Msg0001` | 发生未知错误。 | **删除** | 已有 `Common.UnknownError` 同值 |
| `Msg0007` | 请输入{0}，不允许为空。 | **删除** | 已有 `Common.ParameterRequired` 同值 |
| `Msg3010` | 操作成功。 | **删除** | 已有 `Common.Success` 同值 |
| `Msg0202` | 提示信息 | **合并** → `Common.Prompt` | 与 `Msg0000` 同值 |
| `Msg9995` | 主键 | **合并** → `Common.PrimaryKey` | 与 `Msg9976` 同值 |
| `Msg0283` | 编号产生成功 | **合并** → `Sequence.GenerateSuccess` | 与 `Msg0214` 同值（差一个句号） |
| `Msg0203` | 您确认移动 "{0}" 到 "{1}" 吗？ | **合并** → `Confirm.Move` | 与 `Msg0038`（确认移动{0}到{1}吗？）同义 |

## 三、全量映射表

### Common.*（69 个，含并入的界面标签）

| 旧键 | 中文值 | 新键 |
|---|---|---|
| Msg0000 | 提示信息 | `Common.Prompt` |
| Msg0004 | 无任何数据被修改。 | `Common.NoDataModified` |
| Msg0005 | 记录未找到，请检查网络连接，也可能数据已被其他人删除，请联系系统管理员核实。 | `Common.RecordNotFound` |
| Msg0006 | 数据已被其他人修改，请按F5键重新更新取得数据。 | `Common.DataChangedByOthers` |
| Msg0017 | 当前记录不允许被删除。 | `Common.RecordDeleteNotAllowed` |
| Msg0018 | 当前记录{0}不允许被删除。 | `Common.RecordItemDeleteNotAllowed` |
| Msg0019 | 当前记录不允许被编辑，请按F5键重新获得最新资料。 | `Common.RecordEditNotAllowed` |
| Msg0020 | 当前记录{0}不允许被编辑，请按F5键重新获得最新资料。 | `Common.RecordItemEditNotAllowed` |
| Msg0021 | 当前记录已是第一条记录。 | `Common.IsFirstRecord` |
| Msg0022 | 当前记录已是最后一条记录。 | `Common.IsLastRecord` |
| Msg0033 | 资料已经被引用，有关联资料在。 | `Common.DataReferenced` |
| Msg0035 | {0}有子节点不允许被删除，有子节点还未被删除。 | `Common.ChildNodeExists` |
| Msg0036 | {0}不能移动到 {1}。 | `Common.MoveNotAllowed` |
| Msg0037 | {0}下的子节点不能移动到{1}。 | `Common.ChildNodeMoveNotAllowed` |
| Msg0040 | {0}错误。 | `Common.ItemError` |
| Msg0043 | 不能锁定数据。 | `Common.LockFailed` |
| Msg0044 | 成功锁定数据。 | `Common.LockSuccess` |
| Msg0091 | 新增操作权限项。 | `Common.AddPermissionItem` ⚠️ |
| Msg0212 | 查询内容 | `Common.QueryContent` |
| Msg0225 | 目前节点上有资料。 | `Common.NodeHasData` |
| Msg0226 | 无法删除自己。 | `Common.DeleteSelfNotAllowed` |
| Msg0232 | 用户名称 | `Common.UserFullName` ⚠️ |
| Msg0233 | 姓名 | `Common.RealName` |
| Msg0243 | 程序异常报告 | `Common.ExceptionReport` |
| Msg0246 | 没有要复制的数据！ | `Common.NoDataToCopy` |
| Msg0285 | 没有要保存的资料！ | `Common.NoDataToSave` |
| Msg0302 | 分类。 | `Common.Category` |
| Msg0750 | 调试信息 | `Common.DebugInfo` |
| Msg3020 | 操作失败。 | `Common.Failed` |
| Msg8700 | 手机号码 | `Common.Mobile` |
| Msg8800 | 电子邮件 | `Common.Email` |
| Msg8900 | 工号 | `Common.EmployeeNumber` |
| Msg9800 | 值 | `Common.Value` |
| Msg9914 | 用户关联。 | `Common.UserRelation` |
| Msg9916 | 显示   ▽ | `Common.Show` |
| Msg9917 | 隐藏   △ | `Common.Hide` |
| Msg9954 | 唯一用户名 | `Common.UniqueUserName` |
| Msg9955 | 资料 | `Common.Data` |
| Msg9956 | 未找到满足条件的记录 | `Common.NoRecordMatched` |
| Msg9957 | 用户名 | `Common.UserName` |
| Msg9959 | 新密码 | `Common.NewPassword` |
| Msg9960 | 确认密码 | `Common.ConfirmPassword` |
| Msg9961 | 原密码 | `Common.OldPassword` |
| Msg9964 | 密码 | `Common.Password` |
| Msg9969 | 基础编码 | `Common.BaseCode` |
| Msg9973 | 选单 | `Common.Menu` |
| Msg9974 | 文件夹 | `Common.Folder` |
| Msg9975 | 权限 | `Common.Permission` |
| Msg9976 | 主键 | `Common.PrimaryKey` |
| Msg9977 | 编号 | `Common.Code` |
| Msg9978 | 名称 | `Common.Name` |
| Msg9979 | 父节点主键 | `Common.ParentId` |
| Msg9980 | 父节点名称 | `Common.ParentName` |
| Msg9981 | 功能分类主键 | `Common.FunctionCategoryId` |
| Msg9982 | 唯一识别主键 | `Common.UniqueId` |
| Msg9983 | 主题 | `Common.Subject` |
| Msg9984 | 内容 | `Common.Content` |
| Msg9985 | 状态代码 | `Common.StatusCode` |
| Msg9986 | 次数 | `Common.Times` |
| Msg9987 | 有效 | `Common.Enabled` |
| Msg9988 | 备注 | `Common.Remark` |
| Msg9989 | 排序码 | `Common.SortCode` |
| Msg9990 | 建立者主键 | `Common.CreatedBy` |
| Msg9991 | 建立时间 | `Common.CreatedOn` |
| Msg9992 | 最后修改者主键 | `Common.ModifiedBy` |
| Msg9993 | 修改时间 | `Common.ModifiedOn` |
| Msg9994 | 排序 | `Common.Sort` |
| Msg9996 | 索引 | `Common.Index` |
| Msg9997 | 字段 | `Common.Field` |
| Msg9998 | 数据表 | `Common.Table` |
| Msg9999 | 数据库 | `Common.Database` |

### Confirm.*（27 个）

| 旧键 | 中文值 | 新键 |
|---|---|---|
| Msg0015 | 您确认删除吗？ | `Confirm.Delete` |
| Msg0016 | 您确认删除{0}吗？ | `Confirm.DeleteItem` |
| Msg0034 | 资料已经被引用，有关联资料在，是否强制删除资料？ | `Confirm.ForceDeleteReferenced` |
| Msg0038 / Msg0203 | 确认移动{0}到{1}吗？ | `Confirm.Move` |
| Msg0041 | 确认审核通过吗？ | `Confirm.AuditPass` |
| Msg0042 | 确认审核退回吗？ | `Confirm.AuditReject` |
| Msg0045 | 数据已经改变，想储存数据吗？ | `Confirm.SaveChangedData` |
| Msg0065 | 数据已经改变，不储存数据？ | `Confirm.DiscardChangedData` |
| Msg0075 | 您确认移除吗？ | `Confirm.Remove` |
| Msg0076 | 您确认移除{0}吗？ | `Confirm.RemoveItem` |
| Msg0200 | 您确认清除用户角色关联吗？ | `Confirm.ClearUserRole` |
| Msg0204 | 您确认退出应用程序吗？ | `Confirm.ExitApplication` |
| Msg0205 | 档案{0}已存在，要覆盖服务器上的档案吗？ | `Confirm.OverwriteFile` |
| Msg0207 | 您确认要删除图片吗？ | `Confirm.DeleteImage` |
| Msg0219 | 您确认重置序列吗？ | `Confirm.ResetSequence` |
| Msg0236 | 导出的目标文件已存在，要覆盖 "{0}" 吗？ | `Confirm.OverwriteExportFile` |
| Msg0239 | 您确认清除异常信息吗？ | `Confirm.ClearException` |
| Msg0275 | 您确认不输入退回理由吗？ | `Confirm.NoRejectReason` |
| Msg0276 | 您确认撤销审核流程中的单据吗？ | `Confirm.CancelWorkflow` |
| Msg0278 | 您确认提交给用户{0}审核吗？ | `Confirm.SubmitToUser` |
| Msg0281 | 您确认替换文件{0}吗？ | `Confirm.ReplaceFile` |
| Msg0284 | 已修改配置信息，需要保存吗？ | `Confirm.SaveConfiguration` |
| Msg0288 | 您确认提交给部门{0}审核吗？ | `Confirm.SubmitToDepartment` |
| Msg0290 | 您确认提交给角色{0}审核吗？ | `Confirm.SubmitToRole` |
| Msg0294 | 您确认要转发给{0}审核吗？ | `Confirm.ForwardTo` |
| Msg0301 | 您确定保存吗？ | `Confirm.Save` |
| Msg0600 | 您确认清除权限吗？ | `Confirm.ClearPermission` |
| Msg1001 | 您确认重置功能选单吗？ | `Confirm.ResetMenu` |
| Msg3000 | 您确认初始化系统吗？ | `Confirm.InitializeSystem` |

### Validation.*（35 个）

| 旧键 | 中文值 | 新键 |
|---|---|---|
| Msg0008 | {0}已重复。 | `Validation.Duplicated` |
| Msg0024 | 请至少选择一项{0}。 | `Validation.SelectAtLeastOne` |
| Msg0025 | {0}不能大于{1}。 | `Validation.NotGreaterThan` |
| Msg0026 | {0}不能小于{1}。 | `Validation.NotLessThan` |
| Msg0027 | {0}不能等于 {1}。 | `Validation.CanNotEqual` |
| Msg0028 | {0}不是有效的日期。 | `Validation.InvalidDate` |
| Msg0029 | {0}不是有效的字符。 | `Validation.InvalidChar` |
| Msg0030 | {0}不是有效的数字。 | `Validation.InvalidNumber` |
| Msg0031 | {0}不是有效的金额。 | `Validation.InvalidAmount` |
| Msg0032 | {0}名包含非法字符。 | `Validation.ContainsIllegalChar` |
| Msg0039 | {0}不等于{1}。 | `Validation.NotEqual` |
| Msg0064 | 您输入的分钟数值不正确，请检查。 | `Validation.InvalidMinutes` |
| Msg0208 | 开始时间不能大于结束时间。 | `Validation.StartTimeAfterEndTime` |
| Msg0213 | 编号总长度不要超过40位。 | `Validation.CodeTooLong` |
| Msg0223 | 用户名称不允许为空，请输入。 | `Validation.UserNameRequired` |
| Msg0229 | 所在单位不允许为空，请选择。 | `Validation.CompanyRequired` |
| Msg0231 | 密码不等于确认密码，请确认后重新输入。 | `Validation.PasswordMismatch` |
| Msg0234 | E-mail 格式不正确，请重新输入。 | `Validation.InvalidEmail` |
| Msg0240 | 内容不能为空 | `Validation.ContentRequired` |
| Msg0274 | 请选需要处理的数据。 | `Validation.SelectDataRequired` |
| Msg0277 | 请选择提交给哪个用户审核。 | `Validation.SelectAuditUser` |
| Msg0287 | 请选择提交给哪个部门审核。 | `Validation.SelectAuditDepartment` |
| Msg0289 | 请选择提交给哪个角色审核。 | `Validation.SelectAuditRole` |
| Msg0291 | 请选择提交给哪个角色或部门或人员审核。 | `Validation.SelectAuditor` |
| Msg0303 | 请选择{0}。 | `Validation.SelectItem` |
| Msg0304 | 用户、组织机构、角色必须选择一个。 | `Validation.SelectUserOrgOrRole` |
| Msg2000 | {0}不正确，请重新输入。 | `Validation.InvalidValue` |
| Msg9915 | 请设置约束条件。 | `Validation.ConstraintRequired` |
| Msg9919 | 请输入条件。 | `Validation.ConditionRequired` |
| Msg9920 | 请输入内容。 | `Validation.ContentEmpty` ⚠️ |
| Msg9921 | 缺少（ 符号。 | `Validation.MissingLeftBracket` |
| Msg9922 | 缺少 ）符号。 | `Validation.MissingRightBracket` |
| Msg9958 | 数据验证错误 | `Validation.DataError` |
| Msgc023 | 请至少选择一项。 | `Validation.SelectAtLeastOneItem` ⚠️ |
| Msgc024 | 只能选择一条数据。 | `Validation.SelectOnlyOne` |

### Result.*（20 个）

| 旧键 | 中文值 | 新键 |
|---|---|---|
| Msg0009 | 新增成功。 | `Result.AddSuccess` |
| Msg0010 | 更新成功。 | `Result.UpdateSuccess` |
| Msg0011 | 保存成功。 | `Result.SaveSuccess` |
| Msg0012 | 批量保存成功。 | `Result.BatchSaveSuccess` |
| Msg0013 | 删除成功。 | `Result.DeleteSuccess` |
| Msg0014 | 批量删除成功。 | `Result.BatchDeleteSuccess` |
| Msg0077 | 成功删除{0}条记录。 | `Result.DeleteRecordsSuccess` |
| Msg0209 | 清除成功。 | `Result.ClearSuccess` |
| Msg0210 | 重置成功。 | `Result.ResetSuccess` |
| Msg0228 | 设置关联用户成功。 | `Result.SetRelatedUserSuccess` |
| Msg0230 | 申请账号更新成功，请等待审核。 | `Result.ApplyAccountUpdateSuccess` |
| Msg0235 | 申请账号成功，请等待审核。 | `Result.ApplyAccountSuccess` |
| Msg0237 | 发送电子邮件成功。 | `Result.SendEmailSuccess` |
| Msg0238 | 清除异常信息成功。 | `Result.ClearExceptionSuccess` |
| Msg0241 | 发送电子邮件失败。 | `Result.SendEmailFailed` |
| Msg0242 | 移动成功。 | `Result.MoveSuccess` |
| Msg9918 | 验证表达式成功。 | `Result.ValidateExpressionSuccess` |
| Msg9962 | 修改{0}成功。 | `Result.ModifySuccess` |
| Msg9963 | 设置{0}成功。 | `Result.SetSuccess` |
| Msg9965 | 执行成功。 | `Result.ExecuteSuccess` |

### Logon.*（41 个，另已有 14 个）

| 旧键 | 中文值 | 新键 |
|---|---|---|
| Msg0046 | 最近{0}次内密码不能重复。。 | `Logon.PasswordCanNotRepeatInRecentTimes` |
| Msg0047 | 密码已过期，账号被锁定，请联系系统管理员。 | `Logon.PasswordExpiredAccountLocked` |
| Msg0048 | 拒绝登录，用户已经在线。 | `Logon.UserAlreadyOnline` |
| Msg0049 | 拒绝登录，网卡Mac地址不符合限制条件。 | `Logon.MacAddressNotAllowed` |
| Msg0050 | 拒绝登录，IP:{0}被限制访问,请联系系统管理人员。 | `Logon.IpAddressRestricted` |
| Msg0051 | 已到在线用户最大数量限制。 | `Logon.MaxOnlineUsersReached` |
| Msg0060 | 请先新增该职员的登入系统用户信息。 | `Logon.CreateLoginAccountFirst` |
| Msg0062 | 请设定新密码，原始密码未曾修改过。 | `Logon.PasswordNeverChanged` |
| Msg0063 | 请设定新密码，30天内未曾修改过密码。 | `Logon.PasswordNotChangedIn30Days` |
| Msg0066 | 请设定新密码，系统要求修改密码。 | `Logon.PasswordChangeRequired` |
| Msg0078 | 用户登入被拒，用户审核中。 | `Logon.UserAwaitingAudit` |
| Msg0079 | 用户被锁定，登入被拒绝，1分钟后登录或联系系统管理员。 | `Logon.UserLockedRetryAfterOneMinute` |
| Msg0080 | 用户账号未被激活，请及时激活用户账号。 | `Logon.AccountNotActivatedPrompt` ⚠️ |
| Msg0081 | 用户被锁定，登入被拒绝，不可早于： | `Logon.LockedNotBefore` |
| Msg0082 | 用户被锁定，登入被拒绝，不可晚于： | `Logon.LockedNotAfter` |
| Msg0083 | 用户被锁定，登入被拒绝，锁定开始日期： | `Logon.LockedStartDate` |
| Msg0084 | 用户被锁定，登入被拒绝，锁定结束日期： | `Logon.LockedEndDate` |
| Msg0087 | 用户已经上线，不允许重复登入。 | `Logon.DuplicateLoginNotAllowed` |
| Msg0088 | 密码错误，登入被拒绝。 | `Logon.PasswordIncorrect` |
| Msg0089 | 已超出用户在线数量上限： | `Logon.OnlineUserLimitExceeded` |
| Msg0090 | 登入被拒绝。 | `Logon.LoginDenied` |
| Msg0092 | 登入开始时间 | `Logon.StartTime` |
| Msg0093 | 登入结束时间 | `Logon.EndTime` |
| Msg0094 | 暂停开始时间 | `Logon.PauseStartTime` |
| Msg0095 | 暂停结束日期 | `Logon.PauseEndDate` |
| Msg0100 | {0}在在线，不允许删除。 | `Logon.UserOnlineDeleteNotAllowed` |
| Msg0101 | 目前用户{0}不允许删除自己。 | `Logon.DeleteSelfNotAllowed` |
| Msg0102 | 不允许使用连续重复密码。 | `Logon.PasswordSequentialNotAllowed` |
| Msg0211 | 已输入{0}次错误密码，不再允许继续登入，请重新启动程序进行登入。 | `Logon.TooManyPasswordAttempts` |
| Msg0300 | 下线通知，您的账号在另一地点登录，您被迫下线。 | `Logon.ForcedOfflineByOtherLogin` |
| Msg0400 | 您的帐户登录异常，被系统锁定{0}分钟，若有疑问请联系系统管理员。 | `Logon.AccountLockedForMinutes` |
| Msg8000 | 密码强度不符合要求，密码至少为8位数，且为数字加字母的组合。 | `Logon.PasswordStrengthInsufficient` |
| Msg9000 | 用户名称或密码错误 | `Logon.UserNameOrPasswordIncorrect` |
| Msg9910 | 用户未设置电子邮件地址。 | `Logon.EmailNotConfigured` |
| Msg9911 | 用户账号被锁定，1分钟后登录或联系系统管理员。 | `Logon.AccountLockedRetryAfterOneMinute` ⚠️ |
| Msg9912 | 用户还未激活账号。 | `Logon.AccountNotActivated` |
| Msg9913 | 用户账号已被激活。 | `Logon.AccountAlreadyActivated` |
| Msg9966 | 用户没有找到，请注意大小写。 | `Logon.UserNotFoundCaseSensitive` |
| Msg9967 | 密码错误，请注意大小写。 | `Logon.PasswordIncorrectCaseSensitive` |
| Msg9968 | 登入被拒绝，帐户已被停用，请与系统管理员联系。 | `Logon.AccountDisabled` |
| MsgReLogon | 重新登入 | `Logon.Relogin` |

### Ip.*（11 个）

| 旧键 | 中文值 | 新键 |
|---|---|---|
| Msg0052 | IP地址格式不正确。 | `Ip.InvalidIpFormat` |
| Msg0053 | MAC地址格式不正确。 | `Ip.InvalidMacFormat` |
| Msg0054 | 请填写IP地址或MAC地址信息。 | `Ip.IpOrMacRequired` |
| Msg0055 | 存在相同的IP地址。 | `Ip.DuplicateIpAddress` |
| Msg0056 | IP地址新增成功。 | `Ip.AddSuccess` |
| Msg0057 | IP地址新增失败。 | `Ip.AddFailed` |
| Msg0058 | 存在相同的MAC地址。 | `Ip.DuplicateMacAddress` |
| Msg0059 | MAC地址新增成功。 | `Ip.MacAddSuccess` |
| Msg0061 | MAC地址新增失败。 | `Ip.MacAddFailed` |
| Msg0085 | IP Address 不正确。 | `Ip.IpAddressIncorrect` ⚠️ |
| Msg0086 | MAC Address 不正确。 | `Ip.MacAddressIncorrect` ⚠️ |

### File.*（3 个）

| 旧键 | 中文值 | 新键 |
|---|---|---|
| Msg0201 | 您选择的档案不存在，请重新选择。 | `File.SelectedFileNotExists` |
| Msg0206 | 已经超过了上传限制，请检查要上传的档案大小。 | `File.UploadSizeExceeded` |
| Msg0244 | 您选择的文档不存在，请重新选择。 | `File.SelectedDocumentNotExists` |

### Sequence.*（5 个）

| 旧键 | 中文值 | 新键 |
|---|---|---|
| Msg0214 / Msg0283 | 编号产生成功。 | `Sequence.GenerateSuccess` |
| Msg0215 | 增序列 | `Sequence.Increment` |
| Msg0216 | 减序列 | `Sequence.Decrement` |
| Msg0217 | 步调 | `Sequence.Step` |
| Msg0218 | 序列重置成功。 | `Sequence.ResetSuccess` |

### Org.*（7 个）

| 旧键 | 中文值 | 新键 |
|---|---|---|
| Msg0282 | 上级机构 | `Org.ParentOrganization` |
| Msg0286 | 单位名称 | `Org.CompanyName` |
| Msg9900 | 公司 | `Org.Company` |
| Msg9901 | 部门 | `Org.Department` |
| Msg9970 | 职员 | `Org.Employee` |
| Msg9971 | 组织机构 | `Org.Organization` |
| Msg9972 | 角色 | `Org.Role` |

### Workflow.*（6 个）

| 旧键 | 中文值 | 新键 |
|---|---|---|
| Msg0279 | 工作流程发送成功。 | `Workflow.SendSuccess` |
| Msg0280 | 工作流程发送失败。 | `Workflow.SendFailed` |
| Msg0292 | 成功退回{0}项。 | `Workflow.RejectSuccess` |
| Msg0293 | 退回失败。 | `Workflow.RejectFailed` |
| Msg0295 | 转发成功{0}项。 | `Workflow.ForwardSuccess` |
| Msg0296 | 转发失败。 | `Workflow.ForwardFailed` |

### System.*（8 个）

| 旧键 | 中文值 | 新键 |
|---|---|---|
| Msg0002 | 数据库连接不正常。 | `System.DbConnectionFailed` |
| Msg0003 | WebService连接不正常。 | `System.WebServiceConnectionFailed` |
| Msg0700 | 已经成功连接到目标数据。 | `System.ConnectSuccess` |
| Msg0800 | 访问被拒绝，未经授权的访问。 | `System.AccessDenied` |
| Msg0900 | 服务调用被拒绝，用户未登入。 | `System.ServiceCallDeniedNotSignedIn` |
| Msg1000 | 系统设定讯息错误，请与软件开发商联系。 | `System.ConfigurationError` |
| Msg9660 | 服务未开始 | `System.ServiceNotStarted` |
| Msg9665 | 服务过期 | `System.ServiceExpired` |

### Sign.*（5 个）

| 旧键 | 中文值 | 新键 |
|---|---|---|
| Msgs857 | 签名私钥。 | `Sign.PrivateKey` |
| Msgs864 | 签名密码。 | `Sign.Password` |
| Msgs957 | 通讯用户名称。 | `Sign.CommunicationUserName` |
| Msgs964 | 通讯密码。 | `Sign.CommunicationPassword` |
| Msgs965 | 验证码 | `Sign.VerificationCode` |

## 四、⚠️ 疑难项（6 个，需你定夺）

| # | 项 | 问题 | 我的建议 |
|---|---|---|---|
| 1 | `Msg0232` 用户名称 vs `Msg9957` 用户名 | 中文几乎同义，但 `Msg9957` 用于 `Msg.Format("Msg0008", Msg.Get("Msg9957"))` → "用户名已重复。"（登录查重场景），`Msg0232` 是界面标签 | `Msg9957` → `Common.UserName`，`Msg0232` → `Common.UserFullName` |
| 2 | `Msg0080` vs `Msg9912` | 都是"账号未激活"。已有 `Logon.UserNotActivated`（"用户还未被激活，不允许设置密码。"） | `Msg9912` → `Logon.AccountNotActivated`，`Msg0080` → `Logon.AccountNotActivatedPrompt`（文本略有差别，保留两条） |
| 3 | `Msg0079` vs `Msg9911` | 都是"被锁定，1 分钟后"。`Msg0079` 带"登入被拒绝"，`Msg9911` 不带 | 分成 `Logon.UserLockedRetryAfterOneMinute` / `Logon.AccountLockedRetryAfterOneMinute`，**保留两条** |
| 4 | `Msg0091` 新增操作权限项。 | 语义不明（陈述句，既非确认也非结果） | 暂放 `Common.AddPermissionItem`，或改 `Result.AddPermissionItem` |
| 5 | `Msg0085/0086` vs `Msg0052/0053` | "IP Address 不正确。" 与 "IP地址格式不正确。" 近乎重复 | 保留两条（`Ip.IpAddressIncorrect` / `Ip.InvalidIpFormat`），或合并为一条 |
| 6 | `Msgc023` vs `Msg0024` | "请至少选择一项。" 与 "请至少选择一项{0}。" 仅差占位符 | 保留两条（`Validation.SelectAtLeastOneItem` / `SelectAtLeastOne`） |
| 7 | `Msg0046` 原文有两个句号 | "最近{0}次内密码不能重复。。" | 改名时**顺手修正为单个句号**（会改变 AppMessage 字段值 → 触发 `ZhCnPack_MatchesAppMessageFields` 失败，需同步改字段） |

## 六、执行结果（2026-09-16 已完成）

| 项 | 结果 |
|---|---|
| 旧键覆盖 | 248 / 248，零遗漏 |
| 新键数 | 242（248 − 6 合并删除） |
| 词条总数 | 406 → **400**（中英各一套，键序一致） |
| 残留编号键 | **0** |
| 新键前缀分布 | Common 74 · Logon 55 · Validation 35 · Confirm 30 · Result 21 · Ip 11 · System 8 · Org 7 · Workflow 6 · Sequence 5 · Sign 5 · File 3 |
| `AppMessage` 字段 | 248 个字段名与字段数**全部不变**（二进制兼容） |
| 调用点改造 | `BaseManager.cs` 48 处、`MsgTests.cs` 11 处 |
| 测试 | `MsgTests` 40/40 双档通过；全量非集成回归通过 |

### 与计划的 3 处偏差

| # | 计划 | 实际 | 原因 |
|---|---|---|---|
| 1 | `Msg0203` 合并 → `Confirm.Move` | 独立成键 **`Confirm.MoveQuoted`** | 两条格式串不同（`您确认移动 "{0}" 到 "{1}" 吗？` vs `确认移动{0}到{1}吗？`），合并会丢失引号占位符；保持字段值不变、零信息丢失 |
| 2 | `Msg0283` 合并 → `Sequence.GenerateSuccess` | 照计划合并，同时把 `AppMessage.Msg0283` 字段值由「编号产生成功」规范为「编号产生成功。」 | 仅补句号，与 `Msg0214` 对齐，否则字段值在语言包中找不到对应项 |
| 3 | — | `AppMessage.Msg0046` 字段值「…不能重复。。」→「…不能重复。」 | 疑难项 7，已确认执行 |

### 受影响文件

```
src/DotNet.Util/Resources/MsgPack.zh-CN.cs      键名重写 + Msg0046 双句号修正
src/DotNet.Util/Resources/MsgPack.en.cs         键名重写
src/DotNet.Util/Message/Msg.cs                  XML 注释示例改为语义键
src/DotNet.Util/Message/AppMessage.Message.cs   Msg0046 / Msg0283 字段值修正 + 说明注释
src/DotNet.Business/Util/BaseManager.cs         48 处调用点改语义键
src/DotNet.Util.Tests/Message/MsgTests.cs       调用点 + 反射校验改为「字段值存在于中文包值集合」+ 词条数 400
Localization.md / README.md / CHANGELOG.md      键命名规范、词条数、示例同步
```

---

## 追加执行结果：强类型层（2026-09-18）

语义键虽已可读，但仍是**字符串**——拼错时 `Msg.Get` 会静默回退返回键名本身（不抛异常），
编译期与 IDE 都无法发现。为此新增 `src/DotNet.Util/Message/Msg.Typed.cs`，
把上表 400 个语义键**全部**转成强类型成员。

| 项 | 结果 |
|---|---|
| 强类型成员 | **400**（属性 342 + 方法 58） |
| 与语言包键比对 | **双向零差集**（语言包有而强类型无：0；强类型有而语言包无：0） |
| 分组数 | 21 个顶层分组，`Enum` 下嵌套 `AuditStatus` / `Status` |
| 成员形态判定 | 语言包文本含 `{n}` → `params object[] args` 方法；否则 → 只读属性 |
| `culture` 重载 | **不提供**（强类型只按当前语言取值；需指定语言仍用 `Msg.Get(key, culture)`） |
| 增量覆盖 | 成员只持有键、不缓存文本，`Msg.Register` / `Msg.LoadJsonOverride` 对强类型**自动生效** |

### 迁移速查（本表 → 强类型）

| 若旧代码写法 | 改为 |
|---|---|
| `Msg.Get("Msg0001")` / `Msg.Get("Common.UnknownError")` | `Msg.Common.UnknownError` |
| `Msg.Format("Msg0007", x)` / `Msg.Format("Common.ParameterRequired", x)` | `Msg.Common.ParameterRequired(x)` |
| `AppMessage.Msg0001`（字段，恒中文） | 需随语言切换时改 `Msg.Common.UnknownError`；字段本身保留不动 |
| `Msg.Get("Enum.Status.Ok")` | `Msg.Enum.Status.Ok` |
| `Status.Ok.ToDescription()` | 不变（读取端已本地化） |
