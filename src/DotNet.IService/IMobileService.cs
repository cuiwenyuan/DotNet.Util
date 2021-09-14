//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System.Collections.Generic;
using System.Data;


namespace DotNet.IService
{
    using Util;

    /// <summary>
    /// IMobileService
    /// 手机短信接口
    /// 
    /// 修改记录
    /// 
    ///		2013.12.02 版本：1.0 JiRiGaLa 手机短信的服务脱离。
    ///		
    /// <author>
    ///		<name>JiRiGaLa</name>
    ///		<date>2013.12.02</date>
    /// </author> 
    /// </summary>
    public partial interface IMobileService
    {
        /// <summary>
        /// 手机号码是否存在？
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="mobile">手机号码</param>
        /// <returns>存在</returns>
        bool Exists(BaseUserInfo userInfo, string mobile);

        /// <summary>
        /// 短信是否发送成功？
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="mobile">手机号码</param>
        /// <returns>是否成功到达</returns>
        bool SentSuccessfully(BaseUserInfo userInfo, string mobile);

        /// <summary>
        /// 获取可发送短信的余额
        /// </summary>
        /// <param name="userInfo">当前用户信息</param>
        /// <param name="applicationCode">公司编号、应用编号</param>
        /// <param name="accountCode">用户工号、用户账户</param>
        /// <returns>短信余额</returns>
        int GetRemainingCount(BaseUserInfo userInfo, string applicationCode, string accountCode);

        /// <summary>
        /// 手机短信充值
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="applicationCode">这里应用编号，可以是用户所属公司的Id</param>
        /// <param name="applicationName">账户名称</param>
        /// <param name="accountCode"></param>
        /// <param name="accountName"></param>
        /// <param name="messageCount">充值条数</param>
        /// <returns>影响行数</returns>
        int Recharge(BaseUserInfo userInfo, string applicationCode, string applicationName, string accountCode, string accountName, int messageCount);

        /// <summary>
        /// 发送手机消息
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="mobile">手机号码</param>
        /// <param name="message">内容</param>
        /// <param name="hotline">客户热线</param>
        /// <param name="confidentialInformation">机密信息</param>
        /// <param name="channel"></param>
        /// <returns>主键</returns>
        int SendMobileMessage(BaseUserInfo userInfo, string mobile, string message, bool hotline, bool confidentialInformation, string channel);

        /// <summary>
        /// 发送手机验证码
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="mobile">手机号码</param>
        /// <param name="system">系统</param>
        /// <param name="channel">手机通道</param>
        /// <returns>验证码</returns>
        bool GetVerificationCode(BaseUserInfo userInfo, string mobile, string system, string channel);

        /// <summary>
        /// 发送验证码的数量
        /// </summary>
        /// <param name="mobile">手机号码</param>
        /// <returns>发送个数</returns>
        int GetSendVerificationCodeCount(string mobile);

        /// <summary>
        /// 获取发送密码次数
        /// </summary>
        /// <param name="mobile">手机号码</param>
        /// <returns>发送次数</returns>
        int GetSendUserPasswordCount(string mobile);

        /// <summary>
        /// 发送手机验证码
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="mobile">手机号码</param>
        /// <param name="userPassword">用户密码</param>
        /// <param name="channel"></param>
        /// <returns>发送是否正常</returns>
        bool SendUserPassword(BaseUserInfo userInfo, string mobile, string userPassword, string channel);
        
        /// <summary>
        /// 获取发送列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="beginDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        /// <returns>发送列表</returns>
        DataTable GetSentLog(BaseUserInfo userInfo, string beginDate, string endDate);

        /// <summary>
        /// 获取分页数据（防注入功能的）
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="recordCount">记录条数</param>
        /// <param name="tableName">数据来源表名</param>
        /// <param name="selectField">选择字段</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">每页显示多少条</param>
        /// <param name="conditions">查询条件</param>
        /// <param name="dbParameters">查询参数</param>
        /// <param name="orderBy">排序字段</param>
        /// <returns>数据表</returns>
        DataTable GetDataTableByPage(BaseUserInfo userInfo, out int recordCount, string tableName, string selectField, int pageIndex, int pageSize, string conditions, List<KeyValuePair<string, object>> dbParameters, string orderBy);

        /// <summary>
        /// 忘记密码按手机号码获取
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="userName">用户名</param>
        /// <param name="mobile">手机号码</param>
        /// <returns>成功</returns>
        bool GetPasswordByMobile(BaseUserInfo userInfo, string userName, string mobile);
    }
}