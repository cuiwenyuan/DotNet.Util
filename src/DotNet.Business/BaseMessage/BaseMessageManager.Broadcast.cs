//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System;

namespace DotNet.Business
{
    using Model;
    using Util;

    /// <summary>
    /// BaseMessageManager
    /// 消息表
    ///
    /// 修改记录
    /// 
    ///     2016.04.06 版本：1.2 JiRiGaLa 增加 消息提醒过期时间提醒 功能。
    ///     2016.04.06 版本：1.1 JiRiGaLa 增加 functionCode 功能。
    ///     2015.09.29 版本：1.0 JiRiGaLa 分离广播功能。
    ///
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2016.04.06</date>
    /// </author>
    /// </summary>
    public partial class BaseMessageManager : BaseManager
    {
        /// <summary>
        /// 定义委托
        /// </summary>
        /// <param name="systemCode">系统编码</param>
        /// <param name="allcompany"></param>
        /// <param name="roleIds"></param>
        /// <param name="areaIds"></param>
        /// <param name="companyIds"></param>
        /// <param name="subCompany"></param>
        /// <param name="departmentIds"></param>
        /// <param name="subDepartment"></param>
        /// <param name="userIds"></param>
        /// <param name="message"></param>
        /// <param name="onlineOnly"></param>
        /// <param name="functionCode"></param>
        /// <param name="expireAt"></param>
        delegate int BroadcastDelegate(string systemCode, bool allcompany, string[] roleIds, string[] areaIds, string[] companyIds, bool subCompany, string[] departmentIds, bool subDepartment, string[] userIds, string message, bool onlineOnly, MessageFunction functionCode = MessageFunction.SystemPush, DateTime? expireAt = null);

        /// <summary>
        /// 广播
        /// </summary>
        /// <param name="systemCode"></param>
        /// <param name="allcompany"></param>
        /// <param name="roleIds"></param>
        /// <param name="areaIds"></param>
        /// <param name="companyIds"></param>
        /// <param name="subCompany"></param>
        /// <param name="departmentIds"></param>
        /// <param name="subDepartment"></param>
        /// <param name="userIds"></param>
        /// <param name="message"></param>
        /// <param name="onlineOnly"></param>
        /// <param name="functionCode"></param>
        /// <param name="expireAt"></param>
        /// <returns></returns>
        public int Broadcast(string systemCode, bool allcompany, string[] roleIds, string[] areaIds, string[] companyIds, bool subCompany, string[] departmentIds, bool subDepartment, string[] userIds, string message, bool onlineOnly, MessageFunction functionCode = MessageFunction.SystemPush, DateTime? expireAt = null)
        {
            var result = 0;

            var broadcastDelegate = new BroadcastDelegate(BroadcastProcess);
            broadcastDelegate.BeginInvoke(systemCode, allcompany, roleIds, areaIds, companyIds, subCompany, departmentIds, subDepartment, userIds, message, onlineOnly, functionCode, expireAt, null, null);

            return result;
        }

        /// <summary>
        /// 广播
        /// </summary>
        /// <param name="systemCode"></param>
        /// <param name="allcompany"></param>
        /// <param name="roleIds"></param>
        /// <param name="areaIds"></param>
        /// <param name="companyIds"></param>
        /// <param name="subCompany"></param>
        /// <param name="departmentIds"></param>
        /// <param name="subDepartment"></param>
        /// <param name="userIds"></param>
        /// <param name="message"></param>
        /// <param name="onlineOnly"></param>
        /// <param name="functionCode"></param>
        /// <param name="expireAt"></param>
        /// <returns></returns>
        public int BroadcastProcess(string systemCode, bool allcompany, string[] roleIds, string[] areaIds, string[] companyIds, bool subCompany, string[] departmentIds, bool subDepartment, string[] userIds, string message, bool onlineOnly, MessageFunction functionCode = MessageFunction.SystemPush, DateTime? expireAt = null)
        {
            var result = 0;

            // 1: 这里需要生成一个sql语句。
            // 2: 只发给中天客户端，中天客户端有登录过的用户？
            // 3: 一边读取一边执行发送指令，马上能见效。
            // 4: 节约数据库资源，节约服务器资源。
            // 5: 广播提醒，过期时间1周就可以了，太时间了，浪费内存资源。

            // 2015-09-29 吉日嘎拉 从最高到最低的顺序判断
            var commandText = " SELECT " + BaseUserEntity.FieldId
                                + " FROM " + BaseUserEntity.TableName
                                + " WHERE " + BaseUserEntity.FieldEnabled + " = 1 "
                                + "       AND " + BaseUserEntity.FieldDeleted + " = 0 "
                                + "       AND " + BaseUserEntity.FieldUserFrom + " = 'Base' ";

            if (allcompany)
            {
                // 什么都不过滤
            }
            if (areaIds != null)
            {
                // 这个需要进行叠加处理
                commandText += " AND " + BaseUserEntity.FieldCompanyId + " IN ( "
                            + " SELECT " + BaseOrganizeEntity.FieldId
                            + " FROM " + BaseOrganizeEntity.TableName
                            + "  WHERE " + BaseOrganizeEntity.FieldEnabled + " = 1 "
                                + "        AND " + BaseOrganizeEntity.FieldDeleted + " = 0 "
                                + "        AND (" + BaseOrganizeEntity.FieldProvinceId + " IN (" + StringUtil.ArrayToList(areaIds) + ")"
                                + "         OR " + BaseOrganizeEntity.FieldCityId + " IN (" + StringUtil.ArrayToList(areaIds) + ")"
                                + "         OR " + BaseOrganizeEntity.FieldStreetId + " IN (" + StringUtil.ArrayToList(areaIds) + ")"
                                + "         OR " + BaseOrganizeEntity.FieldDistrictId + " IN (" + StringUtil.ArrayToList(areaIds) + ")) )";
            }
            if (subCompany)
            {
                commandText += " AND " + BaseUserEntity.FieldCompanyId + " IN ( "

                            + "  SELECT " + BaseOrganizeEntity.FieldId
                            + " FROM " + BaseOrganizeEntity.TableName
                            + "   WHERE " + BaseOrganizeEntity.FieldEnabled + " = 1 "
                                 + "AND " + BaseOrganizeEntity.FieldDeleted + " = 0 "
                                 + "AND " + BaseOrganizeEntity.FieldId + " IN (" + StringUtil.ArrayToList(companyIds) + ")  UNION "

                            + "  SELECT " + BaseOrganizeEntity.FieldId
                            + " FROM " + BaseOrganizeEntity.TableName
                            + "   WHERE " + BaseOrganizeEntity.FieldEnabled + " = 1 "
                                 + "AND " + BaseOrganizeEntity.FieldDeleted + " = 0 "
                              + " START WITH " + BaseOrganizeEntity.FieldParentId + " IN (" + StringUtil.ArrayToList(companyIds) + ") "
                            + " CONNECT BY PRIOR " + BaseOrganizeEntity.FieldId + " = " + BaseOrganizeEntity.FieldParentId + ")";
            }
            if (companyIds != null && companyIds.Length > 0)
            {
                commandText += " AND " + BaseUserEntity.FieldCompanyId + " IN (" + StringUtil.ArrayToList(companyIds) + ")";
            }
            if (subDepartment)
            {
                commandText += " AND " + BaseUserEntity.FieldDepartmentId + " IN ( "
                            + "  SELECT " + BaseDepartmentEntity.FieldId
                            + " FROM " + BaseDepartmentEntity.TableName
                            + "   WHERE " + BaseDepartmentEntity.FieldEnabled + " = 1 "
                                + " AND " + BaseDepartmentEntity.FieldDeleted + " = 0 "
                                + " AND " + BaseOrganizeEntity.FieldDeleted + " = 0 "
                                 + "AND " + BaseDepartmentEntity.FieldId + " IN (" + StringUtil.ArrayToList(departmentIds) + ") UNION "

                            + "  SELECT " + BaseDepartmentEntity.FieldId
                            + " FROM " + BaseDepartmentEntity.TableName
                            + "   WHERE " + BaseDepartmentEntity.FieldEnabled + " = 1 "
                                + " AND " + BaseDepartmentEntity.FieldDeleted + " = 0 "
                                + " AND " + BaseOrganizeEntity.FieldDeleted + " = 0 "
                                + " START WITH " + BaseDepartmentEntity.FieldParentId + " IN (" + StringUtil.ArrayToList(departmentIds) + ") "
                            + " CONNECT BY PRIOR " + BaseDepartmentEntity.FieldId + " = " + BaseDepartmentEntity.FieldParentId + ") ";
            }
            if (departmentIds != null && departmentIds.Length > 0)
            {
                commandText += " AND " + BaseUserEntity.FieldDepartmentId + " IN (" + StringUtil.ArrayToList(departmentIds) + ")";
            }
            if (roleIds != null && roleIds.Length > 0)
            {
                var tableName = systemCode + "UserRole";
                commandText += " AND " + BaseUserEntity.FieldId + " IN ( SELECT UserId FROM " + tableName + " WHERE RoleId IN (" + StringUtil.ArrayToList(roleIds) + "))";
            }
            if (userIds != null && userIds.Length > 0)
            {
                commandText += " AND " + BaseUserEntity.FieldId + " IN (" + StringUtil.ArrayToList(userIds) + ")";
            }
            var userManager = new BaseUserManager(UserInfo);
            using (var dataReader = userManager.ExecuteReader(commandText))
            {
                while (dataReader.Read())
                {
                    var receiverIds = new string[] { dataReader[BaseUserEntity.FieldId].ToString() };
                    var entity = new BaseMessageEntity
                    {
                        // entity.Id = BaseUtil.NewGuid();
                        FunctionCode = functionCode.ToString(),
                        // entity.FunctionCode = MessageFunction.SystemPush.ToString();
                        // entity.FunctionCode = MessageFunction.Remind.ToString();
                        Contents = message,
                        CreateCompanyId = UserInfo.CompanyId,
                        CreateCompanyName = UserInfo.CompanyName,
                        CreateDepartmentId = UserInfo.DepartmentId,
                        CreateDepartmentName = UserInfo.DepartmentName,
                        CreateUserId = UserInfo.Id,
                        CreateBy = UserInfo.RealName,
                        IpAddress = UserInfo.IpAddress,
                        CreateOn = DateTime.Now,
                        IsNew = 1,
                        ReadCount = 0,
                        DeletionStateCode = 0
                    };

                    if (!expireAt.HasValue)
                    {
                        expireAt = DateTime.Now.AddDays(5);
                    }
                    result = Send(entity, receiverIds, false, expireAt);
                }
            }
            return result;
        }
    }
}