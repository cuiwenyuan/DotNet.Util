//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System.Collections.Generic;
using System.Data;


namespace DotNet.IService
{
    using Model;
    using Util;

    /// <summary>
    /// IMessageService
    /// 即时通讯组件接口
    /// 
    /// 修改记录
    /// 
    ///		2010.10.17 版本：1.1 JiRiGaLa 整理接口函数注释。
    ///		2008.04.15 版本：1.0 JiRiGaLa 创建主键。
    ///		
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2008.04.15</date>
    /// </author> 
    /// </summary>
    public partial interface IMessageService
    {
        /// <summary>
        /// 按组织机构获取用户列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="organizeId">公司主键</param>
        /// <param name="departmentId">部门主键</param>
        /// <returns>数据表</returns>
        string[] GetUserByOrganize(BaseUserInfo userInfo, string organizeId, string departmentId);

        /// <summary>
        /// 按角色获取用户列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="roleId">角色主键</param>
        /// <returns>数据表</returns>
        string[] GetUserByRole(BaseUserInfo userInfo, string roleId);

        /// <summary>
        /// 发送即时消息
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="receiverId">接收者主键</param>
        /// <param name="contents">内容</param>
        /// <param name="functionCode"></param>
        /// <returns>主键</returns>
        string Send(BaseUserInfo userInfo, string receiverId, string contents, string functionCode);

        /// <summary>
        /// 发送即时消息
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="organizeId">组织机构主键</param>
        /// <param name="roleId">角色主键</param>
        /// <param name="contents">内容</param>
        /// <returns>发送人数</returns>
        int SendGroupMessage(BaseUserInfo userInfo, string organizeId, string roleId, string contents);

        /// <summary>
        /// 发送系统提示消息
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="receiverId">接收者主键</param>
        /// <param name="contents">内容</param>
        /// <returns>主键</returns>
        string Remind(BaseUserInfo userInfo, string receiverId, string contents);

        /// <summary>
        /// 批量发送即时消息
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="receiverIds">接收者主键组</param>
        /// <param name="organizeIds"></param>
        /// <param name="roleIds">角色主键组</param>
        /// <param name="entity">消息实体</param>
        /// <returns>影响行数</returns>
        int BatchSend(BaseUserInfo userInfo, string[] receiverIds, string[] organizeIds, string[] roleIds, BaseMessageEntity entity);

        /// <summary>
        /// 广播消息
        /// 2015-09-29 吉日嘎拉 改进发送消息功能
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="allCompany">是否发全公司</param>
        /// <param name="roleIds">角色主键</param>
        /// <param name="areaIds">区域主键</param>
        /// <param name="companyIds">公司主键</param>
        /// <param name="subCompany">发下属公司</param>
        /// <param name="departmentIds">部门主键数组</param>
        /// <param name="subDepartment">发下属部门</param>
        /// <param name="userIds">用户主键数组</param>
        /// <param name="message">消息内容</param>
        /// <param name="onlineOnly">只发送给在线用户</param>
        /// <returns>影响行数</returns>
        int Broadcast(BaseUserInfo userInfo, bool allCompany, string[] roleIds, string[] areaIds, string[] companyIds, bool subCompany, string[] departmentIds, bool subDepartment, string[] userIds, string message, bool onlineOnly);

        ///// <summary>
        ///// 获取消息状态
        ///// </summary>
        ///// <param name="userInfo">用户</param>
        ///// <param name="onLineState">用户在线状态</param>
        ///// <param name="lastCheckTime">最后检查时间</param>
        ///// <returns>消息状态数组</returns>
        //string[] MessageCheck(BaseUserInfo userInfo, int onLineState, string lastCheckTime);

        /// <summary>
        /// 获取特定用户的新信息
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="receiverId">当前交互的用户</param>
        /// <returns>数据表</returns>
        DataTable ReadFromReceiver(BaseUserInfo userInfo, string receiverId);

        /// <summary>
        /// 阅读短信
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="id">主键</param>
        int Read(BaseUserInfo userInfo, string id);

        /// <summary>
        /// 检查在线状态
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="onLineState">用户在线状态</param>
        /// <returns>离线人数</returns>
        int CheckOnLine(BaseUserInfo userInfo, int onLineState);

        /// <summary>
        /// 获取在线用户列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <returns>数据表</returns>
        DataTable GetOnLineState(BaseUserInfo userInfo);

        /// <summary>
        /// 获取部门用户
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <returns>用户列表</returns>
        DataTable GetDepartmentUser(BaseUserInfo userInfo);

        /// <summary>
        /// 获取公司用户
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <returns>用户列表</returns>
        DataTable GetCompanyUser(BaseUserInfo userInfo);

        /// <summary>
        /// 获取最近联系人
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <returns>用户列表</returns>
        DataTable GetRecentContacts(BaseUserInfo userInfo);

        /// <summary>
        /// 查询用户
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="search">查询</param>
        /// <returns>用户列表</returns>
        DataTable Search(BaseUserInfo userInfo, string search);

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="recordCount">记录数</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">每页显示</param>
        /// <param name="whereClause">条件</param>
        /// <param name="dbParameters">参数</param>
        /// <param name="order">排序</param>
        /// <returns>数据表</returns>
        DataTable GetDataTableByPage(BaseUserInfo userInfo, out int recordCount, int pageIndex, int pageSize, string whereClause, List<KeyValuePair<string, object>> dbParameters, string order = null);
    }
}