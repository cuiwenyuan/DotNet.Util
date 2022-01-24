//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;

namespace DotNet.Business
{
    using Model;
    using Util;

    /// <summary>
    /// BaseMessageManager（程序OK）
    /// 消息表
    ///
    /// 修改记录
    ///     
    ///     2016.01.27 版本：1.0 JiRiGaLa 创建主键。
    ///
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2016.01.27</date>
    /// </author>
    /// </summary>
    public partial class BaseMessageManager : BaseManager
    {
        /// <summary>
        /// 从缓存获取
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="departmentId"></param>
        /// <returns></returns>
        public static string[] GetUserByOrganizationByCache(string companyId, string departmentId = null)
        {
            string[] result = null;

            if (string.IsNullOrEmpty(companyId))
            {
                return result;
            }

            var key = string.Empty;

            var users = string.Empty;
            if (string.IsNullOrEmpty(departmentId))
            {
                departmentId = string.Empty;
                key = "OU:" + companyId;

            }
            else
            {
                key = "OU:" + companyId + ":" + departmentId;
            }

            result = CacheUtil.Cache(key, () => new BaseMessageManager().GetUserByOrganization(companyId, departmentId), true);

            return result;
        }

        #region public string[] GetUserByOrganization(string companyId, string departmentId = null) 按组织机构获取用户列表
        /// <summary>
        /// 按组织机构获取用户列表
        /// </summary>
        /// <param name="companyId">组织机构主键</param>
        /// <param name="departmentId">部门主键</param>
        /// <returns>数据表</returns>
        public string[] GetUserByOrganization(string companyId, string departmentId = null)
        {
            string[] result = null;

            var dbParameters = new List<IDbDataParameter>();
            var commandText = "SELECT " + BaseUserEntity.TableName + "." + BaseUserEntity.FieldId
                                        + "," + BaseUserEntity.TableName + "." + BaseUserEntity.FieldRealName
                                        + "," + BaseUserEntity.TableName + "." + BaseUserEntity.FieldDepartmentName
                             + " FROM " + BaseUserEntity.TableName
                             + " WHERE " + BaseUserEntity.TableName + "." + BaseUserEntity.FieldDeleted + " = " + DbHelper.GetParameter(BaseUserEntity.FieldDeleted)
                                    + " AND " + BaseUserEntity.TableName + "." + BaseUserEntity.FieldEnabled + " = " + DbHelper.GetParameter(BaseUserEntity.FieldEnabled)
                                    + " AND " + BaseUserEntity.TableName + "." + BaseUserEntity.FieldIsVisible + " = " + DbHelper.GetParameter(BaseUserEntity.FieldIsVisible)
                                    + " AND " + BaseUserEntity.TableName + "." + BaseUserEntity.FieldUserFrom + " = " + DbHelper.GetParameter(BaseUserEntity.FieldUserFrom);

            dbParameters.Add(DbHelper.MakeParameter(BaseUserEntity.FieldDeleted, 0));
            dbParameters.Add(DbHelper.MakeParameter(BaseUserEntity.FieldEnabled, 1));
            dbParameters.Add(DbHelper.MakeParameter(BaseUserEntity.FieldIsVisible, 1));
            dbParameters.Add(DbHelper.MakeParameter(BaseUserEntity.FieldUserFrom, "Base"));

            if (!string.IsNullOrEmpty(companyId))
            {
                // 直接按公司进行查询，执行效率高
                commandText += " AND " + BaseUserEntity.TableName + "." + BaseUserEntity.FieldCompanyId + " = " + DbHelper.GetParameter(BaseUserEntity.FieldCompanyId);
                dbParameters.Add(DbHelper.MakeParameter(BaseUserEntity.FieldCompanyId, companyId));
            }
            if (!string.IsNullOrEmpty(departmentId))
            {
                // 直接按公司进行查询，执行效率高
                commandText += " AND " + BaseUserEntity.TableName + "." + BaseUserEntity.FieldDepartmentId + " = " + DbHelper.GetParameter(BaseUserEntity.FieldDepartmentId);
                dbParameters.Add(DbHelper.MakeParameter(BaseUserEntity.FieldDepartmentId, departmentId));
            }
            commandText += " ORDER BY " + BaseUserEntity.TableName + "." + BaseUserEntity.FieldDepartmentName
                                  + "," + BaseUserEntity.TableName + "." + BaseUserEntity.FieldRealName;

            // 2015-11-12 吉日嘎拉 优化获取用户的方法
            var list = new List<string>();
            using (var dr = dbHelper.ExecuteReader(commandText, dbParameters.ToArray()))
            {
                while (dr.Read())
                {
                    list.Add(dr[BaseUserEntity.FieldId] + "=[" + dr[BaseUserEntity.FieldDepartmentName] + "] " + dr[BaseUserEntity.FieldRealName]);
                }
            }

            result = list.ToArray();

            return result;
        }
        #endregion
    }
}