//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2020, DotNet.
//-----------------------------------------------------------------

namespace DotNet.Util
{
    /// <summary>
    /// State
    /// 程序运行状态。
    /// 
    /// 修改记录
    /// 
    ///     2016.04.13 版本：2.3 JiRiGaLa 实现登录限制的、用户名登录限制、密码登录限制。
    ///     2015.11.19 版本：2.2 JiRiGaLa IPLimit、LogOnLimit 功能增加、限制访问、限制ip的变量定义好。
    ///     2015.11.11 版本：2.1 JiRiGaLa 实现公司没找到的功能。
    ///     2013.02.11 版本：2.0 JiRiGaLa 其实这部分代码已经与AppMessage重复。
    ///		2007.12.09 版本：1.1 JiRiGaLa 重新命名为 StatusCode。
    ///		2007.12.04 版本：1.0 JiRiGaLa 重新调整主键的规范化。
    ///		
    /// <author>
    ///		<name>Troy Cui</name>
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

        [EnumDescription("缺失用户登录数据")]
        MissingData = 8,

        [EnumDescription("发生错误")]
        Error = 9,

        [EnumDescription("运行成功")]
        Ok = 10,

        [EnumDescription("添加成功")]
        OkAdd = 11,

        [EnumDescription("不能锁定数据")]
        CanNotLock = 12,

        [EnumDescription("成功锁定数据")]
        LockOk = 13,

        [EnumDescription("更新数据成功")]
        OkUpdate = 14,

        [EnumDescription("删除成功")]
        OkDelete = 15,

        [EnumDescription("数据已重复,不可以重复")]
        Exist = 16,

        [EnumDescription("编号已存在,不可以重复")]
        ErrorCodeExist = 17,

        [EnumDescription("名称已重复")]
        ErrorNameExist = 18,

        [EnumDescription("值已重复")]
        ErrorValueExist = 19,

        [EnumDescription("用户名已重复")]
        ErrorUserExist = 20,

        [EnumDescription("操作成功")]
        OkOperation = 21,

        [EnumDescription("数据已经被引用，有关联数据在")]
        ErrorDataRelated = 22,

        [EnumDescription("数据已被其他人删除")]
        ErrorDeleted = 23,

        [EnumDescription("更新数据失败")]
        ErrorUpdate = 24,

        [EnumDescription("数据已被其他人修改")]
        ErrorChanged = 24,

        [EnumDescription("未找到记录")]
        NotFound = 25,

        [EnumDescription("用户没有找到")]
        UserNotFound = 26,

        [EnumDescription("密码错误")]
        PasswordError = 27,

        [EnumDescription("验证码错误")]
        VerificationCodeError = 127,
       
        [EnumDescription("登录被拒绝")]
        LogOnDeny = 28,

        [EnumDescription("只允许登录一次")]
        ErrorOnLine = 29,

        [EnumDescription("Mac地址不正确，登录被阻止")]
        ErrorMacAddress = 30,

        [EnumDescription("IP地址不正确，登录被阻止")]
        ErrorIpAddress = 31,

        [EnumDescription("同时在线用户数量限制")]
        ErrorOnLineLimit = 32,

        [EnumDescription("密码不允许为空")]
        PasswordCanNotBeNull = 33,

        [EnumDescription("密码不允许重复")]
        PasswordCanNotBeRepeat = 34,

        [EnumDescription("设置密码成功")]
        SetPasswordOk = 35,

        [EnumDescription("原密码错误")]
        OldPasswordError = 36,

        [EnumDescription("修改密码成功")]
        ChangePasswordOk = 37,

        [EnumDescription("没有电子邮件地址")]
        UserNotEmail = 38,

        [EnumDescription("被锁定")]
        UserLocked = 39,

        [EnumDescription("用户未激活")]
        UserNotActive = 40,

        [EnumDescription("用户已被激活，不用重复激活")]
        UserIsActivate = 41,

        [EnumDescription("用户名或密码错误，由于安全原因不能告诉你具体哪个错误了")]
        ErrorLogOn = 42,

        [EnumDescription("待审核状态")]
        WaitForAudit = 43,

        [EnumDescription("用户名重复")]
        UserDuplicate = 44,

        [EnumDescription("用户登录")]
        UserLogOn = 45,

        [EnumDescription("已经超时，请重新登录")]
        Timeout = 46,

        [EnumDescription("修改密码")]
        ChangePassword = 47,

        [EnumDescription("密码不符合规范")]
        WeakPassword = 48,

        [EnumDescription("没有手机号码")]
        UserNotMobile = 49,

        [EnumDescription("访问被限制，过于频繁操作")]
        IpLimit = 50,

        [EnumDescription("登录被限制，过于频繁操作")]
        LogOnLimit = 51,

        [EnumDescription("登录被限制，用户名过于频繁操作")]
        UserNameLimit = 52,

        [EnumDescription("登录被限制，密码过于频繁操作")]
        PasswordLimit = 53,

        [EnumDescription("服务未开始")]
        ServiceNotStart = 60,

        [EnumDescription("服务到期")]
        ServiceExpired = 65,

        [EnumDescription("参数错误")]
        ParameterError = 77,

        [EnumDescription("退出系统")]
        SignOut = 98,

        [EnumDescription("公司没有找到")]
        CompanyNotFound = 126,

        [EnumDescription("数据被篡改，网络被劫持")]
        SignatureError = 198
    }
}