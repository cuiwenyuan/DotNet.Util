//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2026, DotNet.
//-----------------------------------------------------------------

namespace DotNet.Util
{
    /// <summary>
    ///	Msg
    /// 强类型调用入口（partial）。
    /// 词条按语言包键前缀分组，取值随当前语言切换。
    /// 无占位符的词条为属性，含 {n} 的词条为方法。
    /// 底层仍走 Get / Format，外部 JSON 覆盖与回退链保持不变。
    /// </summary>
    public static partial class Msg
    {
        /// <summary>
        /// Business.* messages
        /// </summary>
        public static class Business
        {
            /// <summary>
            /// 数据已禁用，无需再次禁用
            /// </summary>
            public static string DataAlreadyDisabled => Get("Business.DataAlreadyDisabled");

            /// <summary>
            /// 系统数据无权操作
            /// </summary>
            public static string SystemDataNoPermission => Get("Business.SystemDataNoPermission");

            /// <summary>
            /// 非本公司数据无权操作
            /// </summary>
            public static string OtherCompanyDataNoPermission => Get("Business.OtherCompanyDataNoPermission");

            /// <summary>
            /// 数据已被删除，无需再次删除
            /// </summary>
            public static string DataAlreadyDeleted => Get("Business.DataAlreadyDeleted");

            /// <summary>
            /// 数据未被删除，无需撤销
            /// </summary>
            public static string DataNotDeleted => Get("Business.DataNotDeleted");

            /// <summary>
            /// 数据已启用，无需再次启用
            /// </summary>
            public static string DataAlreadyEnabled => Get("Business.DataAlreadyEnabled");

        }

        /// <summary>
        /// Common.* messages
        /// </summary>
        public static class Common
        {
            /// <summary>
            /// 选单
            /// </summary>
            public static string Menu => Get("Common.Menu");

            /// <summary>
            /// 文件夹
            /// </summary>
            public static string Folder => Get("Common.Folder");

            /// <summary>
            /// 密码
            /// </summary>
            public static string Password => Get("Common.Password");

            /// <summary>
            /// 基础编码
            /// </summary>
            public static string BaseCode => Get("Common.BaseCode");

            /// <summary>
            /// 权限
            /// </summary>
            public static string Permission => Get("Common.Permission");

            /// <summary>
            /// 名称
            /// </summary>
            public static string Name => Get("Common.Name");

            /// <summary>
            /// 父节点主键
            /// </summary>
            public static string ParentId => Get("Common.ParentId");

            /// <summary>
            /// 主键
            /// </summary>
            public static string PrimaryKey => Get("Common.PrimaryKey");

            /// <summary>
            /// 编号
            /// </summary>
            public static string Code => Get("Common.Code");

            /// <summary>
            /// 唯一用户名
            /// </summary>
            public static string UniqueUserName => Get("Common.UniqueUserName");

            /// <summary>
            /// 资料
            /// </summary>
            public static string Data => Get("Common.Data");

            /// <summary>
            /// 显示   ▽
            /// </summary>
            public static string Show => Get("Common.Show");

            /// <summary>
            /// 隐藏   △
            /// </summary>
            public static string Hide => Get("Common.Hide");

            /// <summary>
            /// 未找到满足条件的记录
            /// </summary>
            public static string NoRecordMatched => Get("Common.NoRecordMatched");

            /// <summary>
            /// 确认密码
            /// </summary>
            public static string ConfirmPassword => Get("Common.ConfirmPassword");

            /// <summary>
            /// 原密码
            /// </summary>
            public static string OldPassword => Get("Common.OldPassword");

            /// <summary>
            /// 用户名
            /// </summary>
            public static string UserName => Get("Common.UserName");

            /// <summary>
            /// 新密码
            /// </summary>
            public static string NewPassword => Get("Common.NewPassword");

            /// <summary>
            /// 父节点名称
            /// </summary>
            public static string ParentName => Get("Common.ParentName");

            /// <summary>
            /// 最后修改者主键
            /// </summary>
            public static string ModifiedBy => Get("Common.ModifiedBy");

            /// <summary>
            /// 修改时间
            /// </summary>
            public static string ModifiedOn => Get("Common.ModifiedOn");

            /// <summary>
            /// 建立者主键
            /// </summary>
            public static string CreatedBy => Get("Common.CreatedBy");

            /// <summary>
            /// 建立时间
            /// </summary>
            public static string CreatedOn => Get("Common.CreatedOn");

            /// <summary>
            /// 排序
            /// </summary>
            public static string Sort => Get("Common.Sort");

            /// <summary>
            /// 数据表
            /// </summary>
            public static string Table => Get("Common.Table");

            /// <summary>
            /// 数据库
            /// </summary>
            public static string Database => Get("Common.Database");

            /// <summary>
            /// 索引
            /// </summary>
            public static string Index => Get("Common.Index");

            /// <summary>
            /// 字段
            /// </summary>
            public static string Field => Get("Common.Field");

            /// <summary>
            /// 主题
            /// </summary>
            public static string Subject => Get("Common.Subject");

            /// <summary>
            /// 内容
            /// </summary>
            public static string Content => Get("Common.Content");

            /// <summary>
            /// 功能分类主键
            /// </summary>
            public static string FunctionCategoryId => Get("Common.FunctionCategoryId");

            /// <summary>
            /// 唯一识别主键
            /// </summary>
            public static string UniqueId => Get("Common.UniqueId");

            /// <summary>
            /// 状态代码
            /// </summary>
            public static string StatusCode => Get("Common.StatusCode");

            /// <summary>
            /// 备注
            /// </summary>
            public static string Remark => Get("Common.Remark");

            /// <summary>
            /// 排序码
            /// </summary>
            public static string SortCode => Get("Common.SortCode");

            /// <summary>
            /// 次数
            /// </summary>
            public static string Times => Get("Common.Times");

            /// <summary>
            /// 有效
            /// </summary>
            public static string Enabled => Get("Common.Enabled");

            /// <summary>
            /// 当前记录已是第一条记录。
            /// </summary>
            public static string IsFirstRecord => Get("Common.IsFirstRecord");

            /// <summary>
            /// 当前记录已是最后一条记录。
            /// </summary>
            public static string IsLastRecord => Get("Common.IsLastRecord");

            /// <summary>
            /// 当前记录不允许被编辑，请按F5键重新获得最新资料。
            /// </summary>
            public static string RecordEditNotAllowed => Get("Common.RecordEditNotAllowed");

            /// <summary>
            /// 当前记录{0}不允许被编辑，请按F5键重新获得最新资料。
            /// </summary>
            public static string RecordItemEditNotAllowed(params object[] args)
            {
                return Format("Common.RecordItemEditNotAllowed", args);
            }

            /// <summary>
            /// 资料已经被引用，有关联资料在。
            /// </summary>
            public static string DataReferenced => Get("Common.DataReferenced");

            /// <summary>
            /// {0}下的子节点不能移动到{1}。
            /// </summary>
            public static string ChildNodeMoveNotAllowed(params object[] args)
            {
                return Format("Common.ChildNodeMoveNotAllowed", args);
            }

            /// <summary>
            /// {0}错误。
            /// </summary>
            public static string ItemError(params object[] args)
            {
                return Format("Common.ItemError", args);
            }

            /// <summary>
            /// {0}有子节点不允许被删除，有子节点还未被删除。
            /// </summary>
            public static string ChildNodeExists(params object[] args)
            {
                return Format("Common.ChildNodeExists", args);
            }

            /// <summary>
            /// {0}不能移动到 {1}。
            /// </summary>
            public static string MoveNotAllowed(params object[] args)
            {
                return Format("Common.MoveNotAllowed", args);
            }

            /// <summary>
            /// 发生未知错误。
            /// </summary>
            public static string UnknownError => Get("Common.UnknownError");

            /// <summary>
            /// 提示信息
            /// </summary>
            public static string Prompt => Get("Common.Prompt");

            /// <summary>
            /// 请输入{0}，不允许为空。
            /// </summary>
            public static string ParameterRequired(params object[] args)
            {
                return Format("Common.ParameterRequired", args);
            }

            /// <summary>
            /// 操作成功。
            /// </summary>
            public static string Success => Get("Common.Success");

            /// <summary>
            /// 无任何数据被修改。
            /// </summary>
            public static string NoDataModified => Get("Common.NoDataModified");

            /// <summary>
            /// 当前记录不允许被删除。
            /// </summary>
            public static string RecordDeleteNotAllowed => Get("Common.RecordDeleteNotAllowed");

            /// <summary>
            /// 当前记录{0}不允许被删除。
            /// </summary>
            public static string RecordItemDeleteNotAllowed(params object[] args)
            {
                return Format("Common.RecordItemDeleteNotAllowed", args);
            }

            /// <summary>
            /// 记录未找到，请检查网络连接，也可能数据已被其他人删除，请联系系统管理员核实。
            /// </summary>
            public static string RecordNotFound => Get("Common.RecordNotFound");

            /// <summary>
            /// 数据已被其他人修改，请按F5键重新更新取得数据。
            /// </summary>
            public static string DataChangedByOthers => Get("Common.DataChangedByOthers");

            /// <summary>
            /// 不能锁定数据。
            /// </summary>
            public static string LockFailed => Get("Common.LockFailed");

            /// <summary>
            /// 调试信息
            /// </summary>
            public static string DebugInfo => Get("Common.DebugInfo");

            /// <summary>
            /// 操作失败。
            /// </summary>
            public static string Failed => Get("Common.Failed");

            /// <summary>
            /// 没有要保存的资料！
            /// </summary>
            public static string NoDataToSave => Get("Common.NoDataToSave");

            /// <summary>
            /// 分类。
            /// </summary>
            public static string Category => Get("Common.Category");

            /// <summary>
            /// 手机号码
            /// </summary>
            public static string Mobile => Get("Common.Mobile");

            /// <summary>
            /// 值
            /// </summary>
            public static string Value => Get("Common.Value");

            /// <summary>
            /// 用户关联。
            /// </summary>
            public static string UserRelation => Get("Common.UserRelation");

            /// <summary>
            /// 电子邮件
            /// </summary>
            public static string Email => Get("Common.Email");

            /// <summary>
            /// 工号
            /// </summary>
            public static string EmployeeNumber => Get("Common.EmployeeNumber");

            /// <summary>
            /// 查询内容
            /// </summary>
            public static string QueryContent => Get("Common.QueryContent");

            /// <summary>
            /// 目前节点上有资料。
            /// </summary>
            public static string NodeHasData => Get("Common.NodeHasData");

            /// <summary>
            /// 成功锁定数据。
            /// </summary>
            public static string LockSuccess => Get("Common.LockSuccess");

            /// <summary>
            /// 新增操作权限项。
            /// </summary>
            public static string AddPermissionItem => Get("Common.AddPermissionItem");

            /// <summary>
            /// 无法删除自己。
            /// </summary>
            public static string DeleteSelfNotAllowed => Get("Common.DeleteSelfNotAllowed");

            /// <summary>
            /// 程序异常报告
            /// </summary>
            public static string ExceptionReport => Get("Common.ExceptionReport");

            /// <summary>
            /// 没有要复制的数据！
            /// </summary>
            public static string NoDataToCopy => Get("Common.NoDataToCopy");

            /// <summary>
            /// 用户名称
            /// </summary>
            public static string UserFullName => Get("Common.UserFullName");

            /// <summary>
            /// 姓名
            /// </summary>
            public static string RealName => Get("Common.RealName");

        }

        /// <summary>
        /// Confirm.* messages
        /// </summary>
        public static class Confirm
        {
            /// <summary>
            /// 您确认撤销撤销审核流程中的单据吗？
            /// </summary>
            public static string CancelWorkflow => Get("Confirm.CancelWorkflow");

            /// <summary>
            /// 您确认提交给用户{0}审核吗？
            /// </summary>
            public static string SubmitToUser(params object[] args)
            {
                return Format("Confirm.SubmitToUser", args);
            }

            /// <summary>
            /// 您确认替换文件{0}吗？
            /// </summary>
            public static string ReplaceFile(params object[] args)
            {
                return Format("Confirm.ReplaceFile", args);
            }

            /// <summary>
            /// 您确认不输入退回理由吗？
            /// </summary>
            public static string NoRejectReason => Get("Confirm.NoRejectReason");

            /// <summary>
            /// 您确认重置序列吗？
            /// </summary>
            public static string ResetSequence => Get("Confirm.ResetSequence");

            /// <summary>
            /// 导出的目标文件已存在，要覆盖 ""{0}"" 吗？
            /// </summary>
            public static string OverwriteExportFile(params object[] args)
            {
                return Format("Confirm.OverwriteExportFile", args);
            }

            /// <summary>
            /// 您确认清除异常信息吗？
            /// </summary>
            public static string ClearException => Get("Confirm.ClearException");

            /// <summary>
            /// 已修改配置信息，需要保存吗？
            /// </summary>
            public static string SaveConfiguration => Get("Confirm.SaveConfiguration");

            /// <summary>
            /// 您确认清除权限吗？
            /// </summary>
            public static string ClearPermission => Get("Confirm.ClearPermission");

            /// <summary>
            /// 您确认重置功能选单吗？
            /// </summary>
            public static string ResetMenu => Get("Confirm.ResetMenu");

            /// <summary>
            /// 您确认初始化系统吗？
            /// </summary>
            public static string InitializeSystem => Get("Confirm.InitializeSystem");

            /// <summary>
            /// 您确定保存吗？
            /// </summary>
            public static string Save => Get("Confirm.Save");

            /// <summary>
            /// 您确认提交给部门{0}审核吗？
            /// </summary>
            public static string SubmitToDepartment(params object[] args)
            {
                return Format("Confirm.SubmitToDepartment", args);
            }

            /// <summary>
            /// 您确认提交给角色{0}审核吗？
            /// </summary>
            public static string SubmitToRole(params object[] args)
            {
                return Format("Confirm.SubmitToRole", args);
            }

            /// <summary>
            /// 您确认要转发给{0}审核吗？
            /// </summary>
            public static string ForwardTo(params object[] args)
            {
                return Format("Confirm.ForwardTo", args);
            }

            /// <summary>
            /// 确认审核通过吗？
            /// </summary>
            public static string AuditPass => Get("Confirm.AuditPass");

            /// <summary>
            /// 确认审核退回吗？
            /// </summary>
            public static string AuditReject => Get("Confirm.AuditReject");

            /// <summary>
            /// 数据已经改变，想储存数据吗？
            /// </summary>
            public static string SaveChangedData => Get("Confirm.SaveChangedData");

            /// <summary>
            /// 确认移动{0}到{1}吗？
            /// </summary>
            public static string Move(params object[] args)
            {
                return Format("Confirm.Move", args);
            }

            /// <summary>
            /// 您确认删除吗？
            /// </summary>
            public static string Delete => Get("Confirm.Delete");

            /// <summary>
            /// 您确认删除{0}吗？
            /// </summary>
            public static string DeleteItem(params object[] args)
            {
                return Format("Confirm.DeleteItem", args);
            }

            /// <summary>
            /// 资料已经被引用，有关联资料在，是否强制删除资料？
            /// </summary>
            public static string ForceDeleteReferenced => Get("Confirm.ForceDeleteReferenced");

            /// <summary>
            /// 数据已经改变，不储存数据？
            /// </summary>
            public static string DiscardChangedData => Get("Confirm.DiscardChangedData");

            /// <summary>
            /// 您确认退出应用程序吗？
            /// </summary>
            public static string ExitApplication => Get("Confirm.ExitApplication");

            /// <summary>
            /// 档案{0}已存在，要覆盖服务器上的档案吗？
            /// </summary>
            public static string OverwriteFile(params object[] args)
            {
                return Format("Confirm.OverwriteFile", args);
            }

            /// <summary>
            /// 您确认要删除图片吗？
            /// </summary>
            public static string DeleteImage => Get("Confirm.DeleteImage");

            /// <summary>
            /// 您确认移动 ""{0}"" 到 ""{1}"" 吗？
            /// </summary>
            public static string MoveQuoted(params object[] args)
            {
                return Format("Confirm.MoveQuoted", args);
            }

            /// <summary>
            /// 您确认移除吗？
            /// </summary>
            public static string Remove => Get("Confirm.Remove");

            /// <summary>
            /// 您确认移除{0}吗？
            /// </summary>
            public static string RemoveItem(params object[] args)
            {
                return Format("Confirm.RemoveItem", args);
            }

            /// <summary>
            /// 您确认清除用户角色关联吗？
            /// </summary>
            public static string ClearUserRole => Get("Confirm.ClearUserRole");

        }

        /// <summary>
        /// Console.* messages
        /// </summary>
        public static class Console
        {
            /// <summary>
            /// Parallel任务{0}开始工作……
            /// </summary>
            public static string ParallelTaskStart(params object[] args)
            {
                return Format("Console.ParallelTaskStart", args);
            }

            /// <summary>
            /// 任务{0}开始工作……
            /// </summary>
            public static string TaskStart(params object[] args)
            {
                return Format("Console.TaskStart", args);
            }

            /// <summary>
            /// {0} : 完成 
            /// </summary>
            public static string Completed(params object[] args)
            {
                return Format("Console.Completed", args);
            }

            /// <summary>
            /// 通用文字识别:
            /// </summary>
            public static string OcrGeneral => Get("Console.OcrGeneral");

        }

        /// <summary>
        /// Enum.* messages
        /// </summary>
        public static class Enum
        {
            /// <summary>
            /// Enum.AuditStatus.* messages
            /// </summary>
            public static class AuditStatus
            {
                /// <summary>
                /// 退回
                /// </summary>
                public static string AuditReject => Get("Enum.AuditStatus.AuditReject");

                /// <summary>
                /// 转发
                /// </summary>
                public static string Transmit => Get("Enum.AuditStatus.Transmit");

                /// <summary>
                /// 完成
                /// </summary>
                public static string AuditComplete => Get("Enum.AuditStatus.AuditComplete");

                /// <summary>
                /// 受理
                /// </summary>
                public static string AuditAccept => Get("Enum.AuditStatus.AuditAccept");

                /// <summary>
                /// 废弃
                /// </summary>
                public static string AuditQuash => Get("Enum.AuditStatus.AuditQuash");

                /// <summary>
                /// 草稿
                /// </summary>
                public static string Draft => Get("Enum.AuditStatus.Draft");

                /// <summary>
                /// 暂停
                /// </summary>
                public static string Pause => Get("Enum.AuditStatus.Pause");

                /// <summary>
                /// 提交
                /// </summary>
                public static string StartAudit => Get("Enum.AuditStatus.StartAudit");

                /// <summary>
                /// 待审
                /// </summary>
                public static string WaitForAudit => Get("Enum.AuditStatus.WaitForAudit");

                /// <summary>
                /// 通过
                /// </summary>
                public static string AuditPass => Get("Enum.AuditStatus.AuditPass");

            }

            /// <summary>
            /// Enum.Status.* messages
            /// </summary>
            public static class Status
            {
                /// <summary>
                /// 用户名或密码错误，由于安全原因不能告诉你具体哪个错误了
                /// </summary>
                public static string ErrorLogon => Get("Enum.Status.ErrorLogon");

                /// <summary>
                /// 用户已被激活，不用重复激活
                /// </summary>
                public static string UserIsActivate => Get("Enum.Status.UserIsActivate");

                /// <summary>
                /// 用户未激活
                /// </summary>
                public static string UserNotActive => Get("Enum.Status.UserNotActive");

                /// <summary>
                /// 待审核状态
                /// </summary>
                public static string WaitForAudit => Get("Enum.Status.WaitForAudit");

                /// <summary>
                /// 已经超时，请重新登录
                /// </summary>
                public static string Timeout => Get("Enum.Status.Timeout");

                /// <summary>
                /// 用户登录
                /// </summary>
                public static string UserLogon => Get("Enum.Status.UserLogon");

                /// <summary>
                /// 用户名重复
                /// </summary>
                public static string UserDuplicate => Get("Enum.Status.UserDuplicate");

                /// <summary>
                /// 被锁定
                /// </summary>
                public static string UserLocked => Get("Enum.Status.UserLocked");

                /// <summary>
                /// 密码不允许重复
                /// </summary>
                public static string PasswordCanNotBeRepeat => Get("Enum.Status.PasswordCanNotBeRepeat");

                /// <summary>
                /// 密码不允许为空
                /// </summary>
                public static string PasswordCanNotBeNull => Get("Enum.Status.PasswordCanNotBeNull");

                /// <summary>
                /// 同时在线用户数量限制
                /// </summary>
                public static string ErrorOnlineLimit => Get("Enum.Status.ErrorOnlineLimit");

                /// <summary>
                /// 设置密码成功
                /// </summary>
                public static string SetPasswordOk => Get("Enum.Status.SetPasswordOk");

                /// <summary>
                /// 没有电子邮件地址
                /// </summary>
                public static string UserNotEmail => Get("Enum.Status.UserNotEmail");

                /// <summary>
                /// 修改密码成功
                /// </summary>
                public static string ChangePasswordOk => Get("Enum.Status.ChangePasswordOk");

                /// <summary>
                /// 原密码错误
                /// </summary>
                public static string OldPasswordError => Get("Enum.Status.OldPasswordError");

                /// <summary>
                /// 不允许修改
                /// </summary>
                public static string NotAllowEdit => Get("Enum.Status.NotAllowEdit");

                /// <summary>
                /// 参数错误
                /// </summary>
                public static string ParameterError => Get("Enum.Status.ParameterError");

                /// <summary>
                /// 服务到期
                /// </summary>
                public static string ServiceExpired => Get("Enum.Status.ServiceExpired");

                /// <summary>
                /// 退出系统
                /// </summary>
                public static string SignOut => Get("Enum.Status.SignOut");

                /// <summary>
                /// 数据被篡改，网络被劫持
                /// </summary>
                public static string SignatureError => Get("Enum.Status.SignatureError");

                /// <summary>
                /// 验证码错误
                /// </summary>
                public static string VerificationCodeError => Get("Enum.Status.VerificationCodeError");

                /// <summary>
                /// 公司没有找到
                /// </summary>
                public static string CompanyNotFound => Get("Enum.Status.CompanyNotFound");

                /// <summary>
                /// 服务未开始
                /// </summary>
                public static string ServiceNotStart => Get("Enum.Status.ServiceNotStart");

                /// <summary>
                /// 没有手机号码
                /// </summary>
                public static string UserNotMobile => Get("Enum.Status.UserNotMobile");

                /// <summary>
                /// 密码不符合规范
                /// </summary>
                public static string WeakPassword => Get("Enum.Status.WeakPassword");

                /// <summary>
                /// 修改密码
                /// </summary>
                public static string ChangePassword => Get("Enum.Status.ChangePassword");

                /// <summary>
                /// 访问被限制，过于频繁操作
                /// </summary>
                public static string IpLimit => Get("Enum.Status.IpLimit");

                /// <summary>
                /// 登录被限制，密码过于频繁操作
                /// </summary>
                public static string PasswordLimit => Get("Enum.Status.PasswordLimit");

                /// <summary>
                /// 登录被限制，用户名过于频繁操作
                /// </summary>
                public static string UserNameLimit => Get("Enum.Status.UserNameLimit");

                /// <summary>
                /// 登录被限制，过于频繁操作
                /// </summary>
                public static string LogonLimit => Get("Enum.Status.LogonLimit");

                /// <summary>
                /// 成功锁定数据
                /// </summary>
                public static string LockOk => Get("Enum.Status.LockOk");

                /// <summary>
                /// 不能锁定数据
                /// </summary>
                public static string CanNotLock => Get("Enum.Status.CanNotLock");

                /// <summary>
                /// 添加成功
                /// </summary>
                public static string OkAdd => Get("Enum.Status.OkAdd");

                /// <summary>
                /// 更新数据成功
                /// </summary>
                public static string OkUpdate => Get("Enum.Status.OkUpdate");

                /// <summary>
                /// 编号已存在,不可以重复
                /// </summary>
                public static string ErrorCodeExist => Get("Enum.Status.ErrorCodeExist");

                /// <summary>
                /// 数据已重复,不可以重复
                /// </summary>
                public static string Exist => Get("Enum.Status.Exist");

                /// <summary>
                /// 删除成功
                /// </summary>
                public static string OkDelete => Get("Enum.Status.OkDelete");

                /// <summary>
                /// 运行成功
                /// </summary>
                public static string Ok => Get("Enum.Status.Ok");

                /// <summary>
                /// 数据库连接错误
                /// </summary>
                public static string DbError => Get("Enum.Status.DbError");

                /// <summary>
                /// 未授权的访问，访问被阻止
                /// </summary>
                public static string AccessDeny => Get("Enum.Status.AccessDeny");

                /// <summary>
                /// 系统编号不正确，登录被阻止
                /// </summary>
                public static string SystemCodeError => Get("Enum.Status.SystemCodeError");

                /// <summary>
                /// 用户已在线
                /// </summary>
                public static string UserOnline => Get("Enum.Status.UserOnline");

                /// <summary>
                /// 发生错误
                /// </summary>
                public static string Error => Get("Enum.Status.Error");

                /// <summary>
                /// 缺失用户登录数据
                /// </summary>
                public static string MissingData => Get("Enum.Status.MissingData");

                /// <summary>
                /// 用户未登录
                /// </summary>
                public static string UserNotLogin => Get("Enum.Status.UserNotLogin");

                /// <summary>
                /// 密码错误
                /// </summary>
                public static string PasswordError => Get("Enum.Status.PasswordError");

                /// <summary>
                /// 用户没有找到
                /// </summary>
                public static string UserNotFound => Get("Enum.Status.UserNotFound");

                /// <summary>
                /// 未找到记录
                /// </summary>
                public static string NotFound => Get("Enum.Status.NotFound");

                /// <summary>
                /// 登录被拒绝
                /// </summary>
                public static string LogonDeny => Get("Enum.Status.LogonDeny");

                /// <summary>
                /// IP地址不正确，登录被阻止
                /// </summary>
                public static string ErrorIpAddress => Get("Enum.Status.ErrorIpAddress");

                /// <summary>
                /// Mac地址不正确，登录被阻止
                /// </summary>
                public static string ErrorMacAddress => Get("Enum.Status.ErrorMacAddress");

                /// <summary>
                /// 只允许登录一次
                /// </summary>
                public static string ErrorOnline => Get("Enum.Status.ErrorOnline");

                /// <summary>
                /// 数据已被其他人修改
                /// </summary>
                public static string ErrorChanged => Get("Enum.Status.ErrorChanged");

                /// <summary>
                /// 用户名已重复
                /// </summary>
                public static string ErrorUserExist => Get("Enum.Status.ErrorUserExist");

                /// <summary>
                /// 值已重复
                /// </summary>
                public static string ErrorValueExist => Get("Enum.Status.ErrorValueExist");

                /// <summary>
                /// 名称已重复
                /// </summary>
                public static string ErrorNameExist => Get("Enum.Status.ErrorNameExist");

                /// <summary>
                /// 操作成功
                /// </summary>
                public static string OkOperation => Get("Enum.Status.OkOperation");

                /// <summary>
                /// 更新数据失败
                /// </summary>
                public static string ErrorUpdate => Get("Enum.Status.ErrorUpdate");

                /// <summary>
                /// 数据已被其他人删除
                /// </summary>
                public static string ErrorDeleted => Get("Enum.Status.ErrorDeleted");

                /// <summary>
                /// 数据已经被引用，有关联数据在
                /// </summary>
                public static string ErrorDataRelated => Get("Enum.Status.ErrorDataRelated");

            }

        }

        /// <summary>
        /// Exception.* messages
        /// </summary>
        public static class Exception
        {
            /// <summary>
            /// 表达式包含非法数字。
            /// </summary>
            public static string ExpressionIllegalNumber => Get("Exception.ExpressionIllegalNumber");

            /// <summary>
            /// 括号不匹配。
            /// </summary>
            public static string BracketMismatch => Get("Exception.BracketMismatch");

            /// <summary>
            /// {0}文件不存在！
            /// </summary>
            public static string FileNotFound(params object[] args)
            {
                return Format("Exception.FileNotFound", args);
            }

            /// <summary>
            /// 无效IP
            /// </summary>
            public static string InvalidIp => Get("Exception.InvalidIp");

            /// <summary>
            /// {0}文件不存在！无法备份此文件！
            /// </summary>
            public static string BackupFileNotFound(params object[] args)
            {
                return Format("Exception.BackupFileNotFound", args);
            }

            /// <summary>
            /// 除数不能为零。
            /// </summary>
            public static string DivideByZero => Get("Exception.DivideByZero");

            /// <summary>
            /// 不支持泛型,必须为基础实体类
            /// </summary>
            public static string GenericNotSupported => Get("Exception.GenericNotSupported");

            /// <summary>
            /// 当前操作系统用户没有权限写入文件 {0}
            /// </summary>
            public static string NoWritePermission(params object[] args)
            {
                return Format("Exception.NoWritePermission", args);
            }

            /// <summary>
            /// 表达式包含非法字符。
            /// </summary>
            public static string ExpressionIllegalChar => Get("Exception.ExpressionIllegalChar");

            /// <summary>
            /// 表达式语法错误。
            /// </summary>
            public static string ExpressionSyntaxError => Get("Exception.ExpressionSyntaxError");

            /// <summary>
            /// 表达式不能为空。
            /// </summary>
            public static string ExpressionEmpty => Get("Exception.ExpressionEmpty");

        }

        /// <summary>
        /// File.* messages
        /// </summary>
        public static class File
        {
            /// <summary>
            /// 您选择的文档不存在，请重新选择。
            /// </summary>
            public static string SelectedDocumentNotExists => Get("File.SelectedDocumentNotExists");

            /// <summary>
            /// 已经超过了上传限制，请检查要上传的档案大小。
            /// </summary>
            public static string UploadSizeExceeded => Get("File.UploadSizeExceeded");

            /// <summary>
            /// 您选择的档案不存在，请重新选择。
            /// </summary>
            public static string SelectedFileNotExists => Get("File.SelectedFileNotExists");

        }

        /// <summary>
        /// Ip.* messages
        /// </summary>
        public static class Ip
        {
            /// <summary>
            /// MAC地址新增成功。
            /// </summary>
            public static string MacAddSuccess => Get("Ip.MacAddSuccess");

            /// <summary>
            /// 存在相同的MAC地址。
            /// </summary>
            public static string DuplicateMacAddress => Get("Ip.DuplicateMacAddress");

            /// <summary>
            /// MAC地址新增失败。
            /// </summary>
            public static string MacAddFailed => Get("Ip.MacAddFailed");

            /// <summary>
            /// MAC Address 不正确。
            /// </summary>
            public static string MacAddressIncorrect => Get("Ip.MacAddressIncorrect");

            /// <summary>
            /// IP Address 不正确。
            /// </summary>
            public static string IpAddressIncorrect => Get("Ip.IpAddressIncorrect");

            /// <summary>
            /// IP地址新增失败。
            /// </summary>
            public static string AddFailed => Get("Ip.AddFailed");

            /// <summary>
            /// MAC地址格式不正确。
            /// </summary>
            public static string InvalidMacFormat => Get("Ip.InvalidMacFormat");

            /// <summary>
            /// IP地址格式不正确。
            /// </summary>
            public static string InvalidIpFormat => Get("Ip.InvalidIpFormat");

            /// <summary>
            /// 请填写IP地址或MAC地址信息。
            /// </summary>
            public static string IpOrMacRequired => Get("Ip.IpOrMacRequired");

            /// <summary>
            /// IP地址新增成功。
            /// </summary>
            public static string AddSuccess => Get("Ip.AddSuccess");

            /// <summary>
            /// 存在相同的IP地址。
            /// </summary>
            public static string DuplicateIpAddress => Get("Ip.DuplicateIpAddress");

        }

        /// <summary>
        /// Log.* messages
        /// </summary>
        public static class Log
        {
            /// <summary>
            /// RestartIisProcess 重写 web.config 失败
            /// </summary>
            public static string RestartIisFailed => Get("Log.RestartIisFailed");

            /// <summary>
            /// GetSourceTextByUrl 获取URL内容失败: {0}
            /// </summary>
            public static string GetSourceTextByUrlFailed(params object[] args)
            {
                return Format("Log.GetSourceTextByUrlFailed", args);
            }

            /// <summary>
            /// Dispose 释放对象失败: {0}
            /// </summary>
            public static string DisposeFailed(params object[] args)
            {
                return Format("Log.DisposeFailed", args);
            }

            /// <summary>
            /// 打开模板文件失败
            /// </summary>
            public static string OpenTemplateFailed => Get("Log.OpenTemplateFailed");

            /// <summary>
            /// 找不到模板文件
            /// </summary>
            public static string TemplateFileNotFound => Get("Log.TemplateFileNotFound");

            /// <summary>
            /// RemoteSaveAs 下载远程文件失败: {0}
            /// </summary>
            public static string RemoteSaveAsFailed(params object[] args)
            {
                return Format("Log.RemoteSaveAsFailed", args);
            }

            /// <summary>
            /// 比较的实体类型不一样或非实体类型
            /// </summary>
            public static string SaveEntityChangeLogTypeMismatch => Get("Log.SaveEntityChangeLogTypeMismatch");

            /// <summary>
            /// AddLogTask: 异常信息:{0}{1}错误源:{2}{1}堆栈信息:{3}
            /// </summary>
            public static string AddLogTaskFailed(params object[] args)
            {
                return Format("Log.AddLogTaskFailed", args);
            }

            /// <summary>
            /// AddLogTask: 异常信息:{0}userName:{1}{2}错误源:{3}{2}堆栈信息:{4}
            /// </summary>
            public static string AddLogTaskFailedWithUser(params object[] args)
            {
                return Format("Log.AddLogTaskFailedWithUser", args);
            }

            /// <summary>
            /// Dispose 释放成员对象失败: {0}
            /// </summary>
            public static string DisposeMemberFailed(params object[] args)
            {
                return Format("Log.DisposeMemberFailed", args);
            }

            /// <summary>
            /// UserConfigUtil 配置解析失败：{0}={1}，已使用默认值。
            /// </summary>
            public static string ConfigParseFailed(params object[] args)
            {
                return Format("Log.ConfigParseFailed", args);
            }

            /// <summary>
            /// 服务调用失败，请检查：{0}
            /// </summary>
            public static string ServiceFail(params object[] args)
            {
                return Format("Log.ServiceFail", args);
            }

        }

        /// <summary>
        /// Logon.* messages
        /// </summary>
        public static class Logon
        {
            /// <summary>
            /// 用户没有找到，请注意大小写。
            /// </summary>
            public static string UserNotFoundCaseSensitive => Get("Logon.UserNotFoundCaseSensitive");

            /// <summary>
            /// 用户账号已被激活。
            /// </summary>
            public static string AccountAlreadyActivated => Get("Logon.AccountAlreadyActivated");

            /// <summary>
            /// 用户还未激活账号。
            /// </summary>
            public static string AccountNotActivated => Get("Logon.AccountNotActivated");

            /// <summary>
            /// 重新登入
            /// </summary>
            public static string Relogin => Get("Logon.Relogin");

            /// <summary>
            /// 登入被拒绝，帐户已被停用，请与系统管理员联系。
            /// </summary>
            public static string AccountDisabled => Get("Logon.AccountDisabled");

            /// <summary>
            /// 密码错误，请注意大小写。
            /// </summary>
            public static string PasswordIncorrectCaseSensitive => Get("Logon.PasswordIncorrectCaseSensitive");

            /// <summary>
            /// 用户账号被锁定，1分钟后登录或联系系统管理员。
            /// </summary>
            public static string AccountLockedRetryAfterOneMinute => Get("Logon.AccountLockedRetryAfterOneMinute");

            /// <summary>
            /// 您的帐户登录异常，被系统锁定{0}分钟，若有疑问请联系系统管理员。
            /// </summary>
            public static string AccountLockedForMinutes(params object[] args)
            {
                return Format("Logon.AccountLockedForMinutes", args);
            }

            /// <summary>
            /// 下线通知，您的账号在另一地点登录，您被迫下线。
            /// </summary>
            public static string ForcedOfflineByOtherLogin => Get("Logon.ForcedOfflineByOtherLogin");

            /// <summary>
            /// 已输入{0}次错误密码，不再允许继续登入，请重新启动程序进行登入。
            /// </summary>
            public static string TooManyPasswordAttempts(params object[] args)
            {
                return Format("Logon.TooManyPasswordAttempts", args);
            }

            /// <summary>
            /// 用户未设置电子邮件地址。
            /// </summary>
            public static string EmailNotConfigured => Get("Logon.EmailNotConfigured");

            /// <summary>
            /// 用户名称或密码错误
            /// </summary>
            public static string UserNameOrPasswordIncorrect => Get("Logon.UserNameOrPasswordIncorrect");

            /// <summary>
            /// 密码强度不符合要求，密码至少为8位数，且为数字加字母的组合。
            /// </summary>
            public static string PasswordStrengthInsufficient => Get("Logon.PasswordStrengthInsufficient");

            /// <summary>
            /// 登录成功
            /// </summary>
            public static string Success => Get("Logon.Success");

            /// <summary>
            /// 用户还未被激活，不允许设置密码。
            /// </summary>
            public static string UserNotActivated => Get("Logon.UserNotActivated");

            /// <summary>
            /// 用户被锁定，不允许设置密码。
            /// </summary>
            public static string UserLockedCannotSetPassword => Get("Logon.UserLockedCannotSetPassword");

            /// <summary>
            /// 用户未找到，请重新输入用户名。
            /// </summary>
            public static string UserNotFoundRetry => Get("Logon.UserNotFoundRetry");

            /// <summary>
            /// 登录失败
            /// </summary>
            public static string Failed => Get("Logon.Failed");

            /// <summary>
            /// 域服务器返回信息{0}
            /// </summary>
            public static string DomainServerResponse(params object[] args)
            {
                return Format("Logon.DomainServerResponse", args);
            }

            /// <summary>
            /// 应用系统用户不存在，请联系管理员。
            /// </summary>
            public static string AppUserNotFound => Get("Logon.AppUserNotFound");

            /// <summary>
            /// 用户被锁定，不允许重置密码。
            /// </summary>
            public static string UserLockedCannotResetPassword => Get("Logon.UserLockedCannotResetPassword");

            /// <summary>
            /// 请用唯一用户名登录、若不知道唯一用户名、请向公司的管理员索取。
            /// </summary>
            public static string RequireUniqueUserName => Get("Logon.RequireUniqueUserName");

            /// <summary>
            /// 访问被拒绝、您的账户没有访问权限。
            /// </summary>
            public static string AccessDenied => Get("Logon.AccessDenied");

            /// <summary>
            /// 访问被拒绝、您的账户没有后台管理访问权限。
            /// </summary>
            public static string AccessDeniedAdmin => Get("Logon.AccessDeniedAdmin");

            /// <summary>
            /// 更新数据库失败，请重试！
            /// </summary>
            public static string UpdateDatabaseFailed => Get("Logon.UpdateDatabaseFailed");

            /// <summary>
            /// 新密码已发送到您的注册邮箱{0}，请注意查收。
            /// </summary>
            public static string PasswordResetMailSent(params object[] args)
            {
                return Format("Logon.PasswordResetMailSent", args);
            }

            /// <summary>
            /// 未找到对应的用户
            /// </summary>
            public static string UserNotFound => Get("Logon.UserNotFound");

            /// <summary>
            /// 不允许使用连续重复密码。
            /// </summary>
            public static string PasswordSequentialNotAllowed => Get("Logon.PasswordSequentialNotAllowed");

            /// <summary>
            /// 请设定新密码，系统要求修改密码。
            /// </summary>
            public static string PasswordChangeRequired => Get("Logon.PasswordChangeRequired");

            /// <summary>
            /// 请设定新密码，30天内未曾修改过密码。
            /// </summary>
            public static string PasswordNotChangedIn30Days => Get("Logon.PasswordNotChangedIn30Days");

            /// <summary>
            /// 请设定新密码，原始密码未曾修改过。
            /// </summary>
            public static string PasswordNeverChanged => Get("Logon.PasswordNeverChanged");

            /// <summary>
            /// 用户账号未被激活，请及时激活用户账号。
            /// </summary>
            public static string AccountNotActivatedPrompt => Get("Logon.AccountNotActivatedPrompt");

            /// <summary>
            /// 用户被锁定，登入被拒绝，1分钟后登录或联系系统管理员。
            /// </summary>
            public static string UserLockedRetryAfterOneMinute => Get("Logon.UserLockedRetryAfterOneMinute");

            /// <summary>
            /// 用户登入被拒，用户审核中。
            /// </summary>
            public static string UserAwaitingAudit => Get("Logon.UserAwaitingAudit");

            /// <summary>
            /// 请先新增该职员的登入系统用户信息。
            /// </summary>
            public static string CreateLoginAccountFirst => Get("Logon.CreateLoginAccountFirst");

            /// <summary>
            /// 拒绝登录，用户已经在线。
            /// </summary>
            public static string UserAlreadyOnline => Get("Logon.UserAlreadyOnline");

            /// <summary>
            /// 密码已过期，账号被锁定，请联系系统管理员。
            /// </summary>
            public static string PasswordExpiredAccountLocked => Get("Logon.PasswordExpiredAccountLocked");

            /// <summary>
            /// 最近{0}次内密码不能重复。
            /// </summary>
            public static string PasswordCanNotRepeatInRecentTimes(params object[] args)
            {
                return Format("Logon.PasswordCanNotRepeatInRecentTimes", args);
            }

            /// <summary>
            /// 已到在线用户最大数量限制。
            /// </summary>
            public static string MaxOnlineUsersReached => Get("Logon.MaxOnlineUsersReached");

            /// <summary>
            /// 拒绝登录，IP:{0}被限制访问,请联系系统管理人员。
            /// </summary>
            public static string IpAddressRestricted(params object[] args)
            {
                return Format("Logon.IpAddressRestricted", args);
            }

            /// <summary>
            /// 拒绝登录，网卡Mac地址不符合限制条件。
            /// </summary>
            public static string MacAddressNotAllowed => Get("Logon.MacAddressNotAllowed");

            /// <summary>
            /// 用户被锁定，登入被拒绝，不可早于：
            /// </summary>
            public static string LockedNotBefore => Get("Logon.LockedNotBefore");

            /// <summary>
            /// 暂停开始时间
            /// </summary>
            public static string PauseStartTime => Get("Logon.PauseStartTime");

            /// <summary>
            /// 登入结束时间
            /// </summary>
            public static string EndTime => Get("Logon.EndTime");

            /// <summary>
            /// 登入开始时间
            /// </summary>
            public static string StartTime => Get("Logon.StartTime");

            /// <summary>
            /// 目前用户{0}不允许删除自己。
            /// </summary>
            public static string DeleteSelfNotAllowed(params object[] args)
            {
                return Format("Logon.DeleteSelfNotAllowed", args);
            }

            /// <summary>
            /// {0}在在线，不允许删除。
            /// </summary>
            public static string UserOnlineDeleteNotAllowed(params object[] args)
            {
                return Format("Logon.UserOnlineDeleteNotAllowed", args);
            }

            /// <summary>
            /// 暂停结束日期
            /// </summary>
            public static string PauseEndDate => Get("Logon.PauseEndDate");

            /// <summary>
            /// 登入被拒绝。
            /// </summary>
            public static string LoginDenied => Get("Logon.LoginDenied");

            /// <summary>
            /// 用户被锁定，登入被拒绝，锁定结束日期：
            /// </summary>
            public static string LockedEndDate => Get("Logon.LockedEndDate");

            /// <summary>
            /// 用户被锁定，登入被拒绝，锁定开始日期：
            /// </summary>
            public static string LockedStartDate => Get("Logon.LockedStartDate");

            /// <summary>
            /// 用户被锁定，登入被拒绝，不可晚于：
            /// </summary>
            public static string LockedNotAfter => Get("Logon.LockedNotAfter");

            /// <summary>
            /// 已超出用户在线数量上限：
            /// </summary>
            public static string OnlineUserLimitExceeded => Get("Logon.OnlineUserLimitExceeded");

            /// <summary>
            /// 密码错误，登入被拒绝。
            /// </summary>
            public static string PasswordIncorrect => Get("Logon.PasswordIncorrect");

            /// <summary>
            /// 用户已经上线，不允许重复登入。
            /// </summary>
            public static string DuplicateLoginNotAllowed => Get("Logon.DuplicateLoginNotAllowed");

        }

        /// <summary>
        /// Org.* messages
        /// </summary>
        public static class Org
        {
            /// <summary>
            /// 职员
            /// </summary>
            public static string Employee => Get("Org.Employee");

            /// <summary>
            /// 组织机构
            /// </summary>
            public static string Organization => Get("Org.Organization");

            /// <summary>
            /// 角色
            /// </summary>
            public static string Role => Get("Org.Role");

            /// <summary>
            /// 部门
            /// </summary>
            public static string Department => Get("Org.Department");

            /// <summary>
            /// 上级机构
            /// </summary>
            public static string ParentOrganization => Get("Org.ParentOrganization");

            /// <summary>
            /// 单位名称
            /// </summary>
            public static string CompanyName => Get("Org.CompanyName");

            /// <summary>
            /// 公司
            /// </summary>
            public static string Company => Get("Org.Company");

        }

        /// <summary>
        /// Qqwry.* messages
        /// </summary>
        public static class Qqwry
        {
            /// <summary>
            /// 未知
            /// </summary>
            public static string Unknown => Get("Qqwry.Unknown");

        }

        /// <summary>
        /// Result.* messages
        /// </summary>
        public static class Result
        {
            /// <summary>
            /// 发送电子邮件失败。
            /// </summary>
            public static string SendEmailFailed => Get("Result.SendEmailFailed");

            /// <summary>
            /// 移动成功。
            /// </summary>
            public static string MoveSuccess => Get("Result.MoveSuccess");

            /// <summary>
            /// 清除异常信息成功。
            /// </summary>
            public static string ClearExceptionSuccess => Get("Result.ClearExceptionSuccess");

            /// <summary>
            /// 申请账号成功，请等待审核。
            /// </summary>
            public static string ApplyAccountSuccess => Get("Result.ApplyAccountSuccess");

            /// <summary>
            /// 发送电子邮件成功。
            /// </summary>
            public static string SendEmailSuccess => Get("Result.SendEmailSuccess");

            /// <summary>
            /// 执行成功。
            /// </summary>
            public static string ExecuteSuccess => Get("Result.ExecuteSuccess");

            /// <summary>
            /// 未知错误
            /// </summary>
            public static string UnknownError => Get("Result.UnknownError");

            /// <summary>
            /// 设置{0}成功。
            /// </summary>
            public static string SetSuccess(params object[] args)
            {
                return Format("Result.SetSuccess", args);
            }

            /// <summary>
            /// 验证表达式成功。
            /// </summary>
            public static string ValidateExpressionSuccess => Get("Result.ValidateExpressionSuccess");

            /// <summary>
            /// 修改{0}成功。
            /// </summary>
            public static string ModifySuccess(params object[] args)
            {
                return Format("Result.ModifySuccess", args);
            }

            /// <summary>
            /// 申请账号更新成功，请等待审核。
            /// </summary>
            public static string ApplyAccountUpdateSuccess => Get("Result.ApplyAccountUpdateSuccess");

            /// <summary>
            /// 批量保存成功。
            /// </summary>
            public static string BatchSaveSuccess => Get("Result.BatchSaveSuccess");

            /// <summary>
            /// 删除成功。
            /// </summary>
            public static string DeleteSuccess => Get("Result.DeleteSuccess");

            /// <summary>
            /// 保存成功。
            /// </summary>
            public static string SaveSuccess => Get("Result.SaveSuccess");

            /// <summary>
            /// 新增成功。
            /// </summary>
            public static string AddSuccess => Get("Result.AddSuccess");

            /// <summary>
            /// 更新成功。
            /// </summary>
            public static string UpdateSuccess => Get("Result.UpdateSuccess");

            /// <summary>
            /// 重置成功。
            /// </summary>
            public static string ResetSuccess => Get("Result.ResetSuccess");

            /// <summary>
            /// 设置关联用户成功。
            /// </summary>
            public static string SetRelatedUserSuccess => Get("Result.SetRelatedUserSuccess");

            /// <summary>
            /// 清除成功。
            /// </summary>
            public static string ClearSuccess => Get("Result.ClearSuccess");

            /// <summary>
            /// 批量删除成功。
            /// </summary>
            public static string BatchDeleteSuccess => Get("Result.BatchDeleteSuccess");

            /// <summary>
            /// 成功删除{0}条记录。
            /// </summary>
            public static string DeleteRecordsSuccess(params object[] args)
            {
                return Format("Result.DeleteRecordsSuccess", args);
            }

        }

        /// <summary>
        /// Rmb.* messages
        /// </summary>
        public static class Rmb
        {
            /// <summary>
            /// 非数字形式！
            /// </summary>
            public static string NotNumeric => Get("Rmb.NotNumeric");

            /// <summary>
            /// 溢出
            /// </summary>
            public static string Overflow => Get("Rmb.Overflow");

        }

        /// <summary>
        /// Sequence.* messages
        /// </summary>
        public static class Sequence
        {
            /// <summary>
            /// 步调
            /// </summary>
            public static string Step => Get("Sequence.Step");

            /// <summary>
            /// 序列重置成功。
            /// </summary>
            public static string ResetSuccess => Get("Sequence.ResetSuccess");

            /// <summary>
            /// 减序列
            /// </summary>
            public static string Decrement => Get("Sequence.Decrement");

            /// <summary>
            /// 编号产生成功。
            /// </summary>
            public static string GenerateSuccess => Get("Sequence.GenerateSuccess");

            /// <summary>
            /// 增序列
            /// </summary>
            public static string Increment => Get("Sequence.Increment");

        }

        /// <summary>
        /// Service.* messages
        /// </summary>
        public static class Service
        {
            /// <summary>
            /// 依部门取得用户列表
            /// </summary>
            public static string UserServiceGetDataTableByDepartment => Get("Service.UserServiceGetDataTableByDepartment");

            /// <summary>
            /// 新增用户
            /// </summary>
            public static string UserServiceAddUser => Get("Service.UserServiceAddUser");

            /// <summary>
            /// 取得列表
            /// </summary>
            public static string UserServiceGetDataTable => Get("Service.UserServiceGetDataTable");

            /// <summary>
            /// 取得实体
            /// </summary>
            public static string UserServiceGetEntity => Get("Service.UserServiceGetEntity");

            /// <summary>
            /// 用户管理服务
            /// </summary>
            public static string UserService => Get("Service.UserService");

            /// <summary>
            /// 依文件夹删除档案
            /// </summary>
            public static string FileServiceDeleteByFolder => Get("Service.FileServiceDeleteByFolder");

            /// <summary>
            ///  申请帐户：
            /// </summary>
            public static string UserServiceApplication => Get("Service.UserServiceApplication");

            /// <summary>
            /// 请审核。
            /// </summary>
            public static string UserServiceCheck => Get("Service.UserServiceCheck");

            /// <summary>
            /// 查询用户
            /// </summary>
            public static string UserServiceSearch => Get("Service.UserServiceSearch");

            /// <summary>
            /// 更新用户
            /// </summary>
            public static string UserServiceUpdateUser => Get("Service.UserServiceUpdateUser");

            /// <summary>
            /// 设置用户的预设角色
            /// </summary>
            public static string UserServiceSetDefaultRole => Get("Service.UserServiceSetDefaultRole");

            /// <summary>
            /// 设置用户审核状态
            /// </summary>
            public static string UserServiceSetUserAuditStates => Get("Service.UserServiceSetUserAuditStates");

            /// <summary>
            /// 依主键取得列表
            /// </summary>
            public static string UserServiceGetDataTableByIds => Get("Service.UserServiceGetDataTableByIds");

            /// <summary>
            /// 依角色取得列表
            /// </summary>
            public static string UserServiceGetDataTableByRole => Get("Service.UserServiceGetDataTableByRole");

            /// <summary>
            /// 判断用户是否在某个角色中
            /// </summary>
            public static string UserServiceUserInRole => Get("Service.UserServiceUserInRole");

            /// <summary>
            /// 取得用户的角色列表
            /// </summary>
            public static string UserServiceGetRoleDataTable => Get("Service.UserServiceGetRoleDataTable");

            /// <summary>
            /// 依文件ID取得档案列表
            /// </summary>
            public static string FileServiceGetDataTableByIds => Get("Service.FileServiceGetDataTableByIds");

            /// <summary>
            ///  的文件夾
            /// </summary>
            public static string FileServiceFolder => Get("Service.FileServiceFolder");

            /// <summary>
            /// 用户空间
            /// </summary>
            public static string FileServiceUserSpace => Get("Service.FileServiceUserSpace");

            /// <summary>
            ///  的文件
            /// </summary>
            public static string FileServiceFile => Get("Service.FileServiceFile");

            /// <summary>
            /// 发送的文件
            /// </summary>
            public static string FileServiceSendFile => Get("Service.FileServiceSendFile");

            /// <summary>
            /// 系统创建目录
            /// </summary>
            public static string FileServiceSystemCreateDirectory => Get("Service.FileServiceSystemCreateDirectory");

            /// <summary>
            /// 档案服务
            /// </summary>
            public static string FileService => Get("Service.FileService");

            /// <summary>
            /// 公共文档
            /// </summary>
            public static string FileServiceShareFolder => Get("Service.FileServiceShareFolder");

            /// <summary>
            /// 公司文档
            /// </summary>
            public static string FileServiceCompanyFile => Get("Service.FileServiceCompanyFile");

            /// <summary>
            /// 下载文件
            /// </summary>
            public static string FileServiceDownload => Get("Service.FileServiceDownload");

            /// <summary>
            /// 判断是否存在
            /// </summary>
            public static string FileServiceExists => Get("Service.FileServiceExists");

            /// <summary>
            /// 依文件夹取得档案列表
            /// </summary>
            public static string FileServiceGetDataTableByFolder => Get("Service.FileServiceGetDataTableByFolder");

            /// <summary>
            /// 上传档案
            /// </summary>
            public static string FileServiceUpload => Get("Service.FileServiceUpload");

            /// <summary>
            ///  发送文件 
            /// </summary>
            public static string FileServiceSendFileFrom => Get("Service.FileServiceSendFileFrom");

            /// <summary>
            /// 收到的文件
            /// </summary>
            public static string FileServiceReceiveFile => Get("Service.FileServiceReceiveFile");

            /// <summary>
            /// 取得实体
            /// </summary>
            public static string FileServiceGetEntity => Get("Service.FileServiceGetEntity");

            /// <summary>
            /// ，请注意查收。
            /// </summary>
            public static string FileServiceCheckReceiveFile => Get("Service.FileServiceCheckReceiveFile");

        }

        /// <summary>
        /// Sign.* messages
        /// </summary>
        public static class Sign
        {
            /// <summary>
            /// 通讯密码。
            /// </summary>
            public static string CommunicationPassword => Get("Sign.CommunicationPassword");

            /// <summary>
            /// 验证码
            /// </summary>
            public static string VerificationCode => Get("Sign.VerificationCode");

            /// <summary>
            /// 通讯用户名称。
            /// </summary>
            public static string CommunicationUserName => Get("Sign.CommunicationUserName");

            /// <summary>
            /// 签名私钥。
            /// </summary>
            public static string PrivateKey => Get("Sign.PrivateKey");

            /// <summary>
            /// 签名密码。
            /// </summary>
            public static string Password => Get("Sign.Password");

        }

        /// <summary>
        /// Sms.* messages
        /// </summary>
        public static class Sms
        {
            /// <summary>
            /// 手机号码有误！
            /// </summary>
            public static string InvalidMobile => Get("Sms.InvalidMobile");

        }

        /// <summary>
        /// System.* messages
        /// </summary>
        public static class System
        {
            /// <summary>
            /// 系统设定讯息错误，请与软件开发商联系。
            /// </summary>
            public static string ConfigurationError => Get("System.ConfigurationError");

            /// <summary>
            /// 服务调用被拒绝，用户未登入。
            /// </summary>
            public static string ServiceCallDeniedNotSignedIn => Get("System.ServiceCallDeniedNotSignedIn");

            /// <summary>
            /// 服务过期
            /// </summary>
            public static string ServiceExpired => Get("System.ServiceExpired");

            /// <summary>
            /// 服务未开始
            /// </summary>
            public static string ServiceNotStarted => Get("System.ServiceNotStarted");

            /// <summary>
            /// WebService连接不正常。
            /// </summary>
            public static string WebServiceConnectionFailed => Get("System.WebServiceConnectionFailed");

            /// <summary>
            /// 数据库连接不正常。
            /// </summary>
            public static string DbConnectionFailed => Get("System.DbConnectionFailed");

            /// <summary>
            /// 访问被拒绝，未经授权的访问。
            /// </summary>
            public static string AccessDenied => Get("System.AccessDenied");

            /// <summary>
            /// 已经成功连接到目标数据。
            /// </summary>
            public static string ConnectSuccess => Get("System.ConnectSuccess");

        }

        /// <summary>
        /// Validation.* messages
        /// </summary>
        public static class Validation
        {
            /// <summary>
            /// 请选择提交给哪个角色或部门或人员审核。
            /// </summary>
            public static string SelectAuditor => Get("Validation.SelectAuditor");

            /// <summary>
            /// 请选择提交给哪个角色审核。
            /// </summary>
            public static string SelectAuditRole => Get("Validation.SelectAuditRole");

            /// <summary>
            /// 用户、组织机构、角色必须选择一个。
            /// </summary>
            public static string SelectUserOrgOrRole => Get("Validation.SelectUserOrgOrRole");

            /// <summary>
            /// 请选择{0}。
            /// </summary>
            public static string SelectItem(params object[] args)
            {
                return Format("Validation.SelectItem", args);
            }

            /// <summary>
            /// 请选需要处理的数据。
            /// </summary>
            public static string SelectDataRequired => Get("Validation.SelectDataRequired");

            /// <summary>
            /// 内容不能为空
            /// </summary>
            public static string ContentRequired => Get("Validation.ContentRequired");

            /// <summary>
            /// 请选择提交给哪个部门审核。
            /// </summary>
            public static string SelectAuditDepartment => Get("Validation.SelectAuditDepartment");

            /// <summary>
            /// 请选择提交给哪个用户审核。
            /// </summary>
            public static string SelectAuditUser => Get("Validation.SelectAuditUser");

            /// <summary>
            /// {0}不正确，请重新输入。
            /// </summary>
            public static string InvalidValue(params object[] args)
            {
                return Format("Validation.InvalidValue", args);
            }

            /// <summary>
            /// 数据验证错误
            /// </summary>
            public static string DataError => Get("Validation.DataError");

            /// <summary>
            /// 缺少 ）符号。
            /// </summary>
            public static string MissingRightBracket => Get("Validation.MissingRightBracket");

            /// <summary>
            /// 只能选择一条数据。
            /// </summary>
            public static string SelectOnlyOne => Get("Validation.SelectOnlyOne");

            /// <summary>
            /// 请至少选择一项。
            /// </summary>
            public static string SelectAtLeastOneItem => Get("Validation.SelectAtLeastOneItem");

            /// <summary>
            /// 请输入条件。
            /// </summary>
            public static string ConditionRequired => Get("Validation.ConditionRequired");

            /// <summary>
            /// 请设置约束条件。
            /// </summary>
            public static string ConstraintRequired => Get("Validation.ConstraintRequired");

            /// <summary>
            /// 缺少（ 符号。
            /// </summary>
            public static string MissingLeftBracket => Get("Validation.MissingLeftBracket");

            /// <summary>
            /// 请输入内容。
            /// </summary>
            public static string ContentEmpty => Get("Validation.ContentEmpty");

            /// <summary>
            /// E-mail 格式不正确，请重新输入。
            /// </summary>
            public static string InvalidEmail => Get("Validation.InvalidEmail");

            /// <summary>
            /// {0}不是有效的日期。
            /// </summary>
            public static string InvalidDate(params object[] args)
            {
                return Format("Validation.InvalidDate", args);
            }

            /// <summary>
            /// {0}不能等于 {1}。
            /// </summary>
            public static string CanNotEqual(params object[] args)
            {
                return Format("Validation.CanNotEqual", args);
            }

            /// <summary>
            /// {0}不是有效的数字。
            /// </summary>
            public static string InvalidNumber(params object[] args)
            {
                return Format("Validation.InvalidNumber", args);
            }

            /// <summary>
            /// {0}不是有效的字符。
            /// </summary>
            public static string InvalidChar(params object[] args)
            {
                return Format("Validation.InvalidChar", args);
            }

            /// <summary>
            /// 请至少选择一项{0}。
            /// </summary>
            public static string SelectAtLeastOne(params object[] args)
            {
                return Format("Validation.SelectAtLeastOne", args);
            }

            /// <summary>
            /// {0}已重复。
            /// </summary>
            public static string Duplicated(params object[] args)
            {
                return Format("Validation.Duplicated", args);
            }

            /// <summary>
            /// {0}不能小于{1}。
            /// </summary>
            public static string NotLessThan(params object[] args)
            {
                return Format("Validation.NotLessThan", args);
            }

            /// <summary>
            /// {0}不能大于{1}。
            /// </summary>
            public static string NotGreaterThan(params object[] args)
            {
                return Format("Validation.NotGreaterThan", args);
            }

            /// <summary>
            /// {0}不是有效的金额。
            /// </summary>
            public static string InvalidAmount(params object[] args)
            {
                return Format("Validation.InvalidAmount", args);
            }

            /// <summary>
            /// 用户名称不允许为空，请输入。
            /// </summary>
            public static string UserNameRequired => Get("Validation.UserNameRequired");

            /// <summary>
            /// 编号总长度不要超过40位。
            /// </summary>
            public static string CodeTooLong => Get("Validation.CodeTooLong");

            /// <summary>
            /// 密码不等于确认密码，请确认后重新输入。
            /// </summary>
            public static string PasswordMismatch => Get("Validation.PasswordMismatch");

            /// <summary>
            /// 所在单位不允许为空，请选择。
            /// </summary>
            public static string CompanyRequired => Get("Validation.CompanyRequired");

            /// <summary>
            /// {0}不等于{1}。
            /// </summary>
            public static string NotEqual(params object[] args)
            {
                return Format("Validation.NotEqual", args);
            }

            /// <summary>
            /// {0}名包含非法字符。
            /// </summary>
            public static string ContainsIllegalChar(params object[] args)
            {
                return Format("Validation.ContainsIllegalChar", args);
            }

            /// <summary>
            /// 开始时间不能大于结束时间。
            /// </summary>
            public static string StartTimeAfterEndTime => Get("Validation.StartTimeAfterEndTime");

            /// <summary>
            /// 您输入的分钟数值不正确，请检查。
            /// </summary>
            public static string InvalidMinutes => Get("Validation.InvalidMinutes");

        }

        /// <summary>
        /// Workflow.* messages
        /// </summary>
        public static class Workflow
        {
            /// <summary>
            /// 退回失败。
            /// </summary>
            public static string RejectFailed => Get("Workflow.RejectFailed");

            /// <summary>
            /// 转发成功{0}项。
            /// </summary>
            public static string ForwardSuccess(params object[] args)
            {
                return Format("Workflow.ForwardSuccess", args);
            }

            /// <summary>
            /// 转发失败。
            /// </summary>
            public static string ForwardFailed => Get("Workflow.ForwardFailed");

            /// <summary>
            /// 工作流程发送成功。
            /// </summary>
            public static string SendSuccess => Get("Workflow.SendSuccess");

            /// <summary>
            /// 工作流程发送失败。
            /// </summary>
            public static string SendFailed => Get("Workflow.SendFailed");

            /// <summary>
            /// 成功退回{0}项。
            /// </summary>
            public static string RejectSuccess(params object[] args)
            {
                return Format("Workflow.RejectSuccess", args);
            }

        }

    }
}
