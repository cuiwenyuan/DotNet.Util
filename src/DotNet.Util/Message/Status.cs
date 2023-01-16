//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2023, DotNet.
//-----------------------------------------------------------------

namespace DotNet.Util
{
    /// <summary>
    /// Status
    /// 程序运行状态。
    /// 
    /// 修改记录
    /// 
    ///     2016.04.13 版本：2.3 JiRiGaLa 实现登录限制的、用户名登录限制、密码登录限制。
    ///     2015.11.19 版本：2.2 JiRiGaLa IPLimit、LogonLimit 功能增加、限制访问、限制ip的变量定义好。
    ///     2015.11.11 版本：2.1 JiRiGaLa 实现公司没找到的功能。
    ///     2013.02.11 版本：2.0 JiRiGaLa 其实这部分代码已经与AppMessage重复。
    ///		2007.12.09 版本：1.1 JiRiGaLa 重新命名为 StatusCode。
    ///		2007.12.04 版本：1.0 JiRiGaLa 重新调整主键的规范化。
    ///		
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2016.04.13</date>
    /// </author> 
    /// </summary>    
    public enum Status
    {
        /// <summary>
        /// 系统编号不正确，登录被阻止
        /// </summary>
        [EnumDescription("系统编号不正确，登录被阻止")]
        SystemCodeError = -2,

        /// <summary>
        /// 未授权的访问，访问被阻止
        /// </summary>
        [EnumDescription("未授权的访问，访问被阻止")]
        AccessDeny = -1,

        /// <summary>
        /// 数据库连接错误
        /// </summary>
        [EnumDescription("数据库连接错误")]
        DbError = 0,
        /// <summary>
        /// 用户已在线
        /// </summary>
        [EnumDescription("用户已在线")]
        UserOnline = 6,
        /// <summary>
        /// 用户未登录
        /// </summary>
        [EnumDescription("用户未登录")]
        UserNotLogin = 7,
        /// <summary>
        /// 缺失用户登录数据
        /// </summary>
        [EnumDescription("缺失用户登录数据")]
        MissingData = 8,
        /// <summary>
        /// 发生错误
        /// </summary>
        [EnumDescription("发生错误")]
        Error = 9,
        /// <summary>
        /// 运行成功
        /// </summary>
        [EnumDescription("运行成功")]
        Ok = 10,
        /// <summary>
        /// 添加成功
        /// </summary>
        [EnumDescription("添加成功")]
        OkAdd = 11,
        /// <summary>
        /// 不能锁定数据
        /// </summary>
        [EnumDescription("不能锁定数据")]
        CanNotLock = 12,
        /// <summary>
        /// 成功锁定数据
        /// </summary>
        [EnumDescription("成功锁定数据")]
        LockOk = 13,
        /// <summary>
        /// 更新数据成功
        /// </summary>
        [EnumDescription("更新数据成功")]
        OkUpdate = 14,
        /// <summary>
        /// 删除成功
        /// </summary>
        [EnumDescription("删除成功")]
        OkDelete = 15,
        /// <summary>
        /// 数据已重复,不可以重复
        /// </summary>
        [EnumDescription("数据已重复,不可以重复")]
        Exist = 16,
        /// <summary>
        /// 编号已存在,不可以重复
        /// </summary>
        [EnumDescription("编号已存在,不可以重复")]
        ErrorCodeExist = 17,
        /// <summary>
        /// 名称已重复
        /// </summary>
        [EnumDescription("名称已重复")]
        ErrorNameExist = 18,
        /// <summary>
        /// 值已重复
        /// </summary>
        [EnumDescription("值已重复")]
        ErrorValueExist = 19,
        /// <summary>
        /// 用户名已重复
        /// </summary>
        [EnumDescription("用户名已重复")]
        ErrorUserExist = 20,
        /// <summary>
        /// 操作成功
        /// </summary>
        [EnumDescription("操作成功")]
        OkOperation = 21,
        /// <summary>
        /// 数据已经被引用，有关联数据在
        /// </summary>
        [EnumDescription("数据已经被引用，有关联数据在")]
        ErrorDataRelated = 22,
        /// <summary>
        /// 数据已被其他人删除
        /// </summary>
        [EnumDescription("数据已被其他人删除")]
        ErrorDeleted = 23,
        /// <summary>
        /// 更新数据失败
        /// </summary>
        [EnumDescription("更新数据失败")]
        ErrorUpdate = 24,
        /// <summary>
        /// 数据已被其他人修改
        /// </summary>
        [EnumDescription("数据已被其他人修改")]
        ErrorChanged = 24,
        /// <summary>
        /// 未找到记录
        /// </summary>
        [EnumDescription("未找到记录")]
        NotFound = 25,
        /// <summary>
        /// 用户没有找到
        /// </summary>
        [EnumDescription("用户没有找到")]
        UserNotFound = 26,
        /// <summary>
        /// 密码错误
        /// </summary>
        [EnumDescription("密码错误")]
        PasswordError = 27,
        
        /// <summary>
        /// 登录被拒绝
        /// </summary>
        [EnumDescription("登录被拒绝")]
        LogonDeny = 28,
        /// <summary>
        /// 只允许登录一次
        /// </summary>
        [EnumDescription("只允许登录一次")]
        ErrorOnline = 29,
        /// <summary>
        /// Mac地址不正确，登录被阻止
        /// </summary>
        [EnumDescription("Mac地址不正确，登录被阻止")]
        ErrorMacAddress = 30,
        /// <summary>
        /// IP地址不正确，登录被阻止
        /// </summary>
        [EnumDescription("IP地址不正确，登录被阻止")]
        ErrorIpAddress = 31,
        /// <summary>
        /// 同时在线用户数量限制
        /// </summary>
        [EnumDescription("同时在线用户数量限制")]
        ErrorOnlineLimit = 32,
        /// <summary>
        /// 密码不允许为空
        /// </summary>
        [EnumDescription("密码不允许为空")]
        PasswordCanNotBeNull = 33,
        /// <summary>
        /// 密码不允许重复
        /// </summary>
        [EnumDescription("密码不允许重复")]
        PasswordCanNotBeRepeat = 34,
        /// <summary>
        /// 设置密码成功
        /// </summary>
        [EnumDescription("设置密码成功")]
        SetPasswordOk = 35,
        /// <summary>
        /// 原密码错误
        /// </summary>
        [EnumDescription("原密码错误")]
        OldPasswordError = 36,
        /// <summary>
        /// 修改密码成功
        /// </summary>
        [EnumDescription("修改密码成功")]
        ChangePasswordOk = 37,
        /// <summary>
        /// 没有电子邮件地址
        /// </summary>
        [EnumDescription("没有电子邮件地址")]
        UserNotEmail = 38,
        /// <summary>
        /// 被锁定
        /// </summary>
        [EnumDescription("被锁定")]
        UserLocked = 39,
        /// <summary>
        /// 用户未激活
        /// </summary>
        [EnumDescription("用户未激活")]
        UserNotActive = 40,
        /// <summary>
        /// 用户已被激活，不用重复激活
        /// </summary>
        [EnumDescription("用户已被激活，不用重复激活")]
        UserIsActivate = 41,
        /// <summary>
        /// 用户名或密码错误，由于安全原因不能告诉你具体哪个错误了
        /// </summary>
        [EnumDescription("用户名或密码错误，由于安全原因不能告诉你具体哪个错误了")]
        ErrorLogon = 42,
        /// <summary>
        /// 待审核状态
        /// </summary>
        [EnumDescription("待审核状态")]
        WaitForAudit = 43,
        /// <summary>
        /// 用户名重复
        /// </summary>
        [EnumDescription("用户名重复")]
        UserDuplicate = 44,
        /// <summary>
        /// 用户登录
        /// </summary>
        [EnumDescription("用户登录")]
        UserLogon = 45,
        /// <summary>
        /// 已经超时，请重新登录
        /// </summary>
        [EnumDescription("已经超时，请重新登录")]
        Timeout = 46,
        /// <summary>
        /// 修改密码
        /// </summary>
        [EnumDescription("修改密码")]
        ChangePassword = 47,
        /// <summary>
        /// 密码不符合规范
        /// </summary>
        [EnumDescription("密码不符合规范")]
        WeakPassword = 48,
        /// <summary>
        /// 没有手机号码
        /// </summary>
        [EnumDescription("没有手机号码")]
        UserNotMobile = 49,
        /// <summary>
        /// 访问被限制，过于频繁操作
        /// </summary>
        [EnumDescription("访问被限制，过于频繁操作")]
        IpLimit = 50,
        /// <summary>
        /// 登录被限制，过于频繁操作
        /// </summary>
        [EnumDescription("登录被限制，过于频繁操作")]
        LogonLimit = 51,
        /// <summary>
        /// 登录被限制，用户名过于频繁操作
        /// </summary>
        [EnumDescription("登录被限制，用户名过于频繁操作")]
        UserNameLimit = 52,
        /// <summary>
        /// 登录被限制，密码过于频繁操作
        /// </summary>
        [EnumDescription("登录被限制，密码过于频繁操作")]
        PasswordLimit = 53,
        /// <summary>
        /// 服务未开始
        /// </summary>
        [EnumDescription("服务未开始")]
        ServiceNotStart = 60,
        /// <summary>
        /// 服务到期
        /// </summary>
        [EnumDescription("服务到期")]
        ServiceExpired = 65,
        /// <summary>
        /// 参数错误
        /// </summary>
        [EnumDescription("参数错误")]
        ParameterError = 77,
        /// <summary>
        /// 参数错误
        /// </summary>
        [EnumDescription("不允许修改")]
        NotAllowEdit = 78,

        /// <summary>
        /// 退出系统
        /// </summary>
        [EnumDescription("退出系统")]
        SignOut = 98,
        /// <summary>
        /// 公司没有找到
        /// </summary>
        [EnumDescription("公司没有找到")]
        CompanyNotFound = 126,

        /// <summary>
        /// 验证码错误
        /// </summary>
        [EnumDescription("验证码错误")]
        VerificationCodeError = 127,

        /// <summary>
        /// 数据被篡改，网络被劫持
        /// </summary>
        [EnumDescription("数据被篡改，网络被劫持")]
        SignatureError = 198
    }
}