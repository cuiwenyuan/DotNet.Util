//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2023, DotNet.
//-----------------------------------------------------------------

using System;
using System.Configuration;

namespace DotNet.Util
{
    /// <summary>
    /// BaseUserInfo
    /// 用户核心基础信息
    /// 
    /// 修改记录
    /// 
    ///     2015.05.17 JiRiGaLa 版本：4.1 进行令牌优化。
    ///     2014.07.01 JiRiGaLa 版本：4.0 增加数字签名 Signature。
    ///     2012.03.16 zhangyi  版本：3.7 修改注释方式，可以在其他类调用的时候显示其参数中文名称。
    ///		2011.09.12 JiRiGaLa 版本：2.1 公司名称、部门名称、工作组名称进行重构。
    ///		2011.05.11 JiRiGaLa 版本：2.0 增加安全通讯用户名、密码。
    ///		2008.08.26 JiRiGaLa 版本：1.2 整理主键。
    ///		2006.05.03 JiRiGaLa 版本：1.1 添加到工程项目中。
    ///		2006.01.21 JiRiGaLa 版本：1.0 远程传递参数用属性才可以。
    ///		
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2015.05.17</date>
    /// </author> 
    /// </summary>
    [Serializable]
    public partial class BaseUserInfo
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public BaseUserInfo()
        {
            ServiceUserName = BaseSystemInfo.ServiceUserName;
            ServicePassword = BaseSystemInfo.ServicePassword;
            // CurrentLanguage = BaseSystemInfo.CurrentLanguage;
            // 张祈璟20130619添加，为wcf取消延迟绑定
            GetSystemCode();
        }

        /// <summary>
        /// 用户端产生的友善的url
        /// 2016-05-17 吉日嘎拉 进行令牌优化。
        /// code说明 ： code作为换取access_token的票据，每次用户授权带上的code将不一样，code只能使用一次，5分钟未被使用自动过期。 
        /// </summary>
        /// <param name="url">访问地址</param>
        /// <param name="authorizationCode">令牌</param>
        /// <returns>访问地址</returns>
        public string GetUserParameter(string url, string authorizationCode = null)
        {
            if (!string.IsNullOrEmpty(url))
            {
                url = url.Replace("{Ticks}", DateTime.Now.ToString("yyyyMMddHHmmss"));
                url = url.Replace("{UserCode}", Code);
                url = url.Replace("{UserName}", UserName);
                url = url.Replace("{NickName}", NickName);
                //url = url.Replace("{Password}", Password);
                url = url.Replace("{Id}", Id);
                url = url.Replace("{UserId}", UserId.ToString());
                url = url.Replace("{OpenId}", OpenId);
                url = url.Replace("{CompanyId}", CompanyId);
                url = url.Replace("{CompanyCode}", CompanyCode);

                // 2016-05-17 吉日嘎拉 进行令牌优化
                if (!string.IsNullOrEmpty(authorizationCode))
                {
                    if (url.IndexOf("code", StringComparison.OrdinalIgnoreCase) < 0)
                    {
                        // 2016-05-19 吉日嘎拉 优化参数
                        if (url.IndexOf("?", StringComparison.OrdinalIgnoreCase) < 0)
                        {
                            url = url + "?code=" + authorizationCode;
                        }
                        else
                        {
                            url = url + "&code=" + authorizationCode;
                        }
                    }
                    else
                    {
                        url = url.Replace("{code}", authorizationCode);
                    }
                }
            }

            return url;
        }

        /// <summary>
        /// 获取当点登录的网址
        /// </summary>
        /// <param name="url">当前网址</param>
        /// <param name="isUrl">是否网址</param>
        /// <returns>单点登录网址</returns>
        public string GetUrl(string url, bool isUrl = true)
        {
            if (!string.IsNullOrEmpty(url))
            {
                url = GetUserParameter(url);
                if (isUrl && url.ToUpper().IndexOf("HTTP://", StringComparison.OrdinalIgnoreCase) < 0)
                {
                    url = BaseSystemInfo.WebHost + url;
                }
            }
            return url;
        }

        private string _id = string.Empty;
        /// <summary>
        /// 用户编号字符串(未登录用户为空)
        /// </summary>
        public virtual string Id
        {
            get => _id;
            set => _id = value;
        }

        private int _userId = 0;
        /// <summary>
        /// 用户编号(未登录用户为0)
        /// </summary>
        public virtual int UserId
        {
            get => _userId;
            set => _userId = value;
        }

        private string _serviceUserName = "Troy.Cui";
        /// <summary>
        /// 远程调用Service用户名（为了提高软件的安全性）
        /// </summary>
        public virtual string ServiceUserName
        {
            get => _serviceUserName;
            set => _serviceUserName = value;
        }

        private string _servicePassword = "CuiWenYuan";
        /// <summary>
        /// 远程调用Service密码（为了提高软件的安全性）
        /// </summary>
        public virtual string ServicePassword
        {
            get => _servicePassword;
            set => _servicePassword = value;
        }

        private string _openId = string.Empty;
        /// <summary>
        /// 单点登录唯一识别标识
        /// </summary>
        public virtual string OpenId
        {
            get => _openId;
            set => _openId = value;
        }

        private string _userName = string.Empty;
        /// <summary>
        /// 用户名（唯一值）
        /// </summary>
        public virtual string UserName
        {
            get => _userName;
            set => _userName = value;
        }

        private string _realname = string.Empty;
        /// <summary>
        /// 用户姓名
        /// </summary>
        public virtual string RealName
        {
            get => _realname;
            set => _realname = value;
        }

        private string _nickName = string.Empty;
        /// <summary>
        /// 昵称（也可以做用来做唯一用户名）
        /// </summary>
        public virtual string NickName
        {
            get => _nickName;
            set => _nickName = value;
        }

        private string _code = string.Empty;
        /// <summary>
        /// 编号（可扩展为工号、供应商编码、客户编码等）
        /// </summary>
        public virtual string Code
        {
            get => _code;
            set => _code = value;
        }

        private string _gender = string.Empty;
        /// <summary>
        /// 姓名（男、女、保密、未知）
        /// </summary>
        public virtual string Gender
        {
            get => _gender;
            set => _gender = value;
        }

        private string _birthday = string.Empty;
        /// <summary>
        /// 生日
        /// </summary>
        public virtual string Birthday
        {
            get => _birthday;
            set => _birthday = value;
        }

        private string _employeeNumber = string.Empty;
        /// <summary>
        /// 工号
        /// </summary>
        public virtual string EmployeeNumber
        {
            get => _employeeNumber;
            set => _employeeNumber = value;
        }

        private string _companyId = null;
        /// <summary>
        /// 当前的组织结构公司主键
        /// </summary>
        public virtual string CompanyId
        {
            get => _companyId;
            set => _companyId = value;
        }

        private string _companyCode = string.Empty;
        /// <summary>
        /// 当前的组织结构公司编号
        /// </summary>
        public virtual string CompanyCode
        {
            get => _companyCode;
            set => _companyCode = value;
        }

        private string _companyName = string.Empty;
        /// <summary>
        /// 当前的组织结构公司名称
        /// </summary>
        public virtual string CompanyName
        {
            get => _companyName;
            set => _companyName = value;
        }

        private string _departmentId = null;
        /// <summary>
        /// 当前的组织结构部门主键
        /// </summary>
        public virtual string DepartmentId
        {
            get => _departmentId;
            set => _departmentId = value;
        }

        private string _departmentCode = string.Empty;
        /// <summary>
        /// 当前的组织结构部门编号
        /// </summary>
        public virtual string DepartmentCode
        {
            get => _departmentCode;
            set => _departmentCode = value;
        }

        private string _departmentName = string.Empty;
        /// <summary>
        /// 当前的组织结构部门名称
        /// </summary>
        public virtual string DepartmentName
        {
            get => _departmentName;
            set => _departmentName = value;
        }

        private bool _isAdministrator = false;
        /// <summary>
        /// 是否超级管理员
        /// </summary>
        public virtual bool IsAdministrator
        {
            get => _isAdministrator;
            set => _isAdministrator = value;
        }

        private bool _identityAuthentication = false;
        /// <summary>
        /// 身份认证通过
        /// </summary>
        public virtual bool IdentityAuthentication
        {
            get => _identityAuthentication;
            set => _identityAuthentication = value;
        }

        //private string _password = string.Empty;
        ///// <summary>
        ///// 密码
        ///// </summary>
        //public virtual string Password
        //{
        //    get => _password;
        //    set => _password = value;
        //}

        private string _ipAddress = string.Empty;
        /// <summary>
        /// IP地址
        /// </summary>
        public virtual string IpAddress
        {
            get => _ipAddress;
            set => _ipAddress = value;
        }

        private string _macAddress = string.Empty;
        /// <summary>
        /// MAC地址
        /// </summary>
        public virtual string MacAddress
        {
            get => _macAddress;
            set => _macAddress = value;
        }

        private string _systemCode = string.Empty;
        /// <summary>
        /// 这里是设置，读取哪个系统的菜单
        /// </summary>
        public virtual string SystemCode
        {
            get => GetSystemCode();
            set => _systemCode = value;
        }

        private string _signature = string.Empty;
        /// <summary>
        /// 数字签名(防止篡改用户信息用)
        /// </summary>
        public virtual string Signature
        {
            get => _signature;
            set => _signature = value;
        }

        private string GetSystemCode()
        {
            if (string.IsNullOrEmpty(_systemCode))
            {
                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["SystemCode"]))
                {
                    _systemCode = ConfigurationManager.AppSettings["SystemCode"];
                }
                if (string.IsNullOrEmpty(_systemCode))
                {
                    _systemCode = BaseSystemInfo.SystemCode;
                }
                if (string.IsNullOrEmpty(_systemCode))
                {
                    _systemCode = "Base";
                }
            }
            return _systemCode;
        }

        /// <summary>
        /// 镜像数据
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        public void CloneData(BaseUserInfo userInfo)
        {
            _systemCode = userInfo.SystemCode;
        }

        /// <summary>
        /// 序列化
        /// </summary>
        /// <returns></returns>
        public string Serialize()
        {
            return JsonUtil.ObjectToJson(this);
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        public static BaseUserInfo Deserialize(string response)
        {
            BaseUserInfo userInfo = null;
            if (!string.IsNullOrEmpty(response))
            {
                userInfo = JsonUtil.JsonToObject<BaseUserInfo>(response);
            }
            return userInfo;
        }
    }
}