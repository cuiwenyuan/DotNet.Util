//-----------------------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;


namespace DotNet.IService
{
    using Model;
    using Util;

    /// <summary>
    /// ILogOnService
    /// 
    /// 修改记录
    /// 
    ///		2016.02.13 版本：2.0 JiRiGaLa 添加接口taskId定义、可以有侦测服务的性能的能力，各种服务互相调用全程耗时能力掌控。
    ///		2009.04.15 版本：1.0 JiRiGaLa 添加接口定义。
    ///		
    /// <author>
    ///		<name>JiRiGaLa</name>
    ///		<date>2016.02.13</date>
    /// </author> 
    /// </summary>
    public partial interface ILogOnService
    {
        /// <summary>
        /// 获取服务器时间
        /// </summary>
        /// <param name="taskId">任务标识</param>
        /// <returns>时间</returns>
        DateTime GetServerDateTime(string taskId);

        /// <summary>
        /// 获取数据库服务器时间
        /// </summary>
        /// <param name="taskId">任务标识</param>
        /// <returns>时间</returns>
        
        DateTime GetDbDateTime(string taskId);

        /// <summary>
        /// 获取系统版本号
        /// </summary>
        /// <param name="taskId">任务标识</param>
        /// <returns>版本号</returns>
        string GetServerVersion(string taskId);

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="taskId">任务标识</param>
        /// <param name="userInfo">用户</param>
        /// <param name="id">主键</param>
        /// <returns>实体</returns>
        BaseUserLogOnEntity GetObject(string taskId, BaseUserInfo userInfo, string id);

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="taskId">任务标识</param>
        /// <param name="userInfo">用户</param>
        /// <param name="entity">实体</param>
        /// <returns>影响行数</returns>
        int Update(string taskId, BaseUserInfo userInfo, BaseUserLogOnEntity entity);

        /// <summary>
        /// 获得登录用户列表
        /// </summary>
        /// <param name="taskId">任务标识</param>
        /// <param name="userInfo">用户</param>
        /// <returns>数据表</returns>
        DataTable GetUserDT(string taskId, BaseUserInfo userInfo);

        /// <summary>
        /// 激活用户
        /// </summary>
        /// <param name="taskId">任务标识</param>
        /// <param name="userInfo">用户</param>
        /// <param name="openId">唯一识别码</param>
        /// <returns>用户实体</returns>
        BaseUserInfo AccountActivation(string taskId, BaseUserInfo userInfo, string openId);

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="taskId">任务标识</param>
        /// <param name="userInfo">用户</param>
        /// <param name="userName">用户名</param>
        /// <param name="password">密码</param>
        /// <param name="openId">单点登录标识</param>
        /// <returns>登录实体类</returns>
        UserLogOnResult UserLogOn(string taskId, BaseUserInfo userInfo, string userName, string password, string openId);

        /// <summary>
        /// 按唯一识别码登录
        /// </summary>
        /// <param name="taskId">任务标识</param>
        /// <param name="userInfo">用户</param>
        /// <param name="openId">唯一识别码</param>
        /// <returns>用户实体</returns>
        UserLogOnResult LogOnByOpenId(string taskId, BaseUserInfo userInfo, string openId);

        /// <summary>
        /// 获取新的OpenId
        /// </summary>
        /// <param name="taskId">任务标识</param>
        /// <param name="userInfo">用户</param>
        /// <returns>OpenId</returns>
        string CreateOpenId(string taskId, BaseUserInfo userInfo);

        /// <summary>
        /// 登录按公司
        /// </summary>
        /// <param name="taskId">任务标识</param>
        /// <param name="userInfo">用户</param>
        /// <param name="companyName">单位名称</param>
        /// <param name="userName">用户名</param>
        /// <param name="password">密码</param>
        /// <param name="openId">单点登录标识</param>
        /// <returns>登录实体类</returns>
        UserLogOnResult LogOnByCompany(string taskId, BaseUserInfo userInfo, string companyName, string userName, string password, string openId);

        /// <summary>
        /// 登录按昵称
        /// </summary>
        /// <param name="taskId">任务标识</param>
        /// <param name="userInfo">用户</param>
        /// <param name="nickName">昵称</param>
        /// <param name="password">密码</param>
        /// <param name="openId">单点登录标识</param>
        /// <returns>登录实体类</returns>
        UserLogOnResult LogOnByNickName(string taskId, BaseUserInfo userInfo, string nickName, string password, string openId);

        /// <summary>
        /// 用户在线
        /// </summary>
        /// <param name="taskId">任务标识</param>
        /// <param name="userInfo">用户</param>
        /// <param name="onLineState">用户在线状态</param>
        void OnLine(string taskId, BaseUserInfo userInfo, int onLineState);

        /// <summary>
        /// 用户离线
        /// </summary>
        /// <param name="taskId">任务标识</param>
        /// <param name="userInfo">用户</param>
        void SignOut(string taskId, BaseUserInfo userInfo);

        /// <summary>
        /// 检查在线状态(服务器专用)
        /// </summary>
        /// <param name="taskId">任务标识</param>
        /// <returns>离线人数</returns>
        int ServerCheckOnLine(string taskId);

        /// <summary>
        /// 设置密码
        /// </summary>
        /// <param name="taskId">任务标识</param>
        /// <param name="userInfo">用户</param>
        /// <param name="userIds">被设置的用户主键</param>
        /// <param name="password">新密码</param>
        /// <returns>影响行数</returns>
        int SetPassword(string taskId, BaseUserInfo userInfo, string[] userIds, string password);

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="taskId">任务标识</param>
        /// <param name="userInfo">用户</param>
        /// <param name="oldPassword">原始密码</param>
        /// <param name="newPassword">新密码</param>
        /// <returns>用户登录</returns>
        UserLogOnResult ChangePassword(string taskId, BaseUserInfo userInfo, string oldPassword, string newPassword);

        /// <summary>
        /// 锁定用户
        /// </summary>
        /// <param name="taskId">任务标识</param>
        /// <param name="userInfo">用户</param>
        /// <param name="userName">用户名</param>
        /// <returns>是否成功锁定</returns>
        bool LockUser(string taskId, BaseUserInfo userInfo, string userName);

        /// <summary>
        /// 判断证码是否正确
        /// </summary>
        /// <param name="taskId">任务标识</param>
        /// <param name="userInfo">用户</param>
        /// <param name="code">验证码</param>
        /// <returns>正确</returns>
        bool ValidateVerificationCode(string taskId, BaseUserInfo userInfo, string code);

        /// <summary>
        /// 忘记密码按电子邮件获取
        /// </summary>
        /// <param name="taskId">任务标识</param>
        /// <param name="userInfo">用户信息</param>
        /// <param name="userName">用户名</param>
        /// <param name="email">电子邮件</param>
        /// <returns>成功</returns>
        bool GetPasswordByEmail(string taskId, BaseUserInfo userInfo, string userName, string email);

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="taskId">任务标识</param>
        /// <param name="userInfo">用户</param>
        /// <param name="recordCount">记录数</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">每页显示</param>
        /// <param name="whereClause">条件</param>
        /// <param name="dbParameters">参数</param>
        /// <param name="order">排序</param>
        /// <returns>数据表</returns>
        DataTable GetDataTableByPage(string taskId, BaseUserInfo userInfo, out int recordCount, int pageIndex, int pageSize, string whereClause, List<KeyValuePair<string, object>> dbParameters, string order = null);
    }
}