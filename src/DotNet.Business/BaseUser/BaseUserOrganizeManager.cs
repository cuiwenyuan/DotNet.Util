//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System.Collections.Generic;
using System.Data;

namespace DotNet.Business
{
    using Model;
    using Util;

    /// <summary>
    /// BaseUserOrganizeManager
    /// 用户-组织结构关系管理
    /// 
    /// 修改记录
    /// 
    ///     2015.11.28 版本：1.1 JiRiGaLa	整理代码。
    ///     2010.09.25 版本：1.0 JiRiGaLa	创建。
    /// 
    /// <author>
    ///		<name>JiRiGaLa</name>
    ///		<date>2015.11.28</date>
    /// </author> 
    /// </summary>
    public partial class BaseUserOrganizeManager : BaseManager
    {
        /// <summary>
        /// 添加用户组织机构关系
        /// </summary>
        /// <param name="entity">用户组织机构实体</param>
        /// <param name="statusCode">状态码</param>
        /// <returns>主键</returns>
        public string Add(BaseUserOrganizeEntity entity, out string statusCode)
        {
            var result = string.Empty;
            // 判断数据是否重复了
            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseUserOrganizeEntity.FieldDeleted, 0),
                new KeyValuePair<string, object>(BaseUserOrganizeEntity.FieldEnabled, 1),
                new KeyValuePair<string, object>(BaseUserOrganizeEntity.FieldUserId, entity.UserId),
                new KeyValuePair<string, object>(BaseUserOrganizeEntity.FieldCompanyId, entity.CompanyId),
                new KeyValuePair<string, object>(BaseUserOrganizeEntity.FieldDepartmentId, entity.DepartmentId),
                new KeyValuePair<string, object>(BaseUserOrganizeEntity.FieldWorkgroupId, entity.WorkgroupId)
            };
            if (Exists(parameters))
            {
                // 用户名已重复
                statusCode = Status.Exist.ToString();
            }
            else
            {
                result = AddEntity(entity);
                // 运行成功
                statusCode = Status.OkAdd.ToString();
            }
            return result;
        }
        
        /// <summary>
        /// 获得用户的组织机构兼职情况
        /// </summary>
        /// <param name="userId">用户主键</param>
        /// <returns>数据表</returns>
        public DataTable GetUserOrganizeDt(string userId)
        {
            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseUserOrganizeEntity.FieldDeleted, 0),
                new KeyValuePair<string, object>(BaseUserOrganizeEntity.FieldEnabled, 1),
                new KeyValuePair<string, object>(BaseUserOrganizeEntity.FieldUserId, userId)
            };

            return GetDataTable(parameters);
        }
    }
}