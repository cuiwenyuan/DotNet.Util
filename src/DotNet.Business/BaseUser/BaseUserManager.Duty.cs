//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System.Collections.Generic;

namespace DotNet.Business
{
    using DotNet.Util;
    using Model;

    /// <summary>
    /// BaseUserManager
    /// 用户管理
    /// 
    /// 修改记录
    /// 
    ///		2012.04.12 版本：1.0 JiRiGaLa	主键整理。
    /// 
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2012.04.12</date>
    /// </author> 
    /// </summary>
    public partial class BaseUserManager : BaseManager
    {
        /// <summary>
        /// 获取有岗位的组织机构主键
        /// </summary>
        /// <param name="userId">用户主键</param>
        /// <param name="dutyName">岗位名称</param>
        /// <returns>主键数组</returns>
        public string[] GetOrganizationIdsByDutyName(string userId, string dutyName = "部门主管")
        {
            // 这里需要一个转换的过程，先找到系统角色里这个角色是什么编号
            var dutyCode = "Manager";
            var roleManager = new BaseRoleManager(UserInfo);
            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseRoleEntity.FieldRealName, dutyName),
                new KeyValuePair<string, object>(BaseRoleEntity.FieldCategoryCode, "SystemRole"),
                new KeyValuePair<string, object>(BaseRoleEntity.FieldEnabled, 1),
                new KeyValuePair<string, object>(BaseRoleEntity.FieldDeleted, 0)
            };
            roleManager.GetId(parameters);
            // 这里需要返回公司的主键数组
            return GetOrganizationIdsByDuty(userId, dutyCode);
        }

        /// <summary>
        /// 获取有岗位的组织机构主键
        /// </summary>
        /// <param name="userId">用户主键</param>
        /// <param name="dutyCode">岗位主键</param>
        /// <returns>主键数组</returns>
        public string[] GetOrganizationIdsByDuty(string userId, string dutyCode = "Manager")
        {
            // 定义返回值
            string[] result = null;
            // 当前用户主键确定
            if (!ValidateUtil.IsInt(userId))
            {
                userId = UserInfo.Id.ToString();
            }
            // 当前用户的所有角色
            var roleIds = GetRoleIds(userId);
            // 当前角色拥有的所有的岗位的组织机构主键数组
            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseRoleEntity.FieldId, roleIds),
                new KeyValuePair<string, object>(BaseRoleEntity.FieldCode, dutyCode),
                new KeyValuePair<string, object>(BaseRoleEntity.FieldCategoryCode, "Duty"),
                new KeyValuePair<string, object>(BaseRoleEntity.FieldEnabled, 1),
                new KeyValuePair<string, object>(BaseRoleEntity.FieldDeleted, 0)
            };
            result = GetProperties(parameters, BaseRoleEntity.FieldOrganizationId);
            // 返回所有的组织机构主键
            return result;
        }
    }
}