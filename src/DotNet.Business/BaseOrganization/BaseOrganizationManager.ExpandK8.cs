//-----------------------------------------------------------------
// All Rights Reserved , Copyright (C) 2014 , Hairihan TECH, Ltd. 
//-----------------------------------------------------------------

using System;
using System.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using DotNet.Model;

namespace DotNet.Business
{
    using DotNet.Util;

    /// <summary>
    /// BaseOrganizationManager
    /// 组织机构管理
    /// 
    /// 修改记录
    /// 
    ///		2013.10.20 版本：2.0 JiRiGaLa	集成K8物流系统的登录功能。
    ///		2011.10.17 版本：1.0 JiRiGaLa	主键整理。
    /// 
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2011.10.17</date>
    /// </author> 
    /// </summary>
    public partial class BaseOrganizationManager : BaseManager
    {
        /// <summary>
        /// 同步数据
        /// </summary>
        /// <param name="all">同步所有数据</param>
        /// <returns>影响行数</returns>
        public int Synchronous(bool all = false)
        {
            int result = 0;
            string connectionString = ConfigurationUtil.AppSettings("K8Connection", BaseSystemInfo.EncryptDbConnection);
            string conditional = null;
            if (!all)
            {
                int id = 0;
                string commandText = "SELECT MAX(id) FROM BaseOrganization WHERE id < 1000000";
                Object maxObject = DbHelper.ExecuteScalar(commandText);
                if (maxObject != null)
                {
                    id = int.Parse(maxObject.ToString());
                }
                conditional = " AND ID > " + id.ToString();
            }
            result = ImportK8Organization(connectionString, conditional);
            return result;
        }

        /// <summary>
        /// 导入K8系统网点信息
        /// </summary>
        /// <param name="connectionString">数据库连接</param>
        /// <param name="conditional">条件，不需要同步所有的数据</param>
        /// <returns>影响行数</returns>
        public int ImportK8Organization(string connectionString = null, string conditional = null)
        {
            // delete from baseorganization where id < 1000000
            int result = 0;
            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = ConfigurationUtil.AppSettings("K8Connection", BaseSystemInfo.EncryptDbConnection);
            }
            if (!string.IsNullOrEmpty(connectionString))
            {
                // 01：可以从k8里读取公司、用户、密码的。
                IDbHelper dbHelper = DbHelperFactory.Create(CurrentDbType.Oracle, connectionString);
                BaseOrganizationManager organizationManager = new Business.BaseOrganizationManager(this.DbHelper, this.UserInfo);
                // 不不存在的组织机构删除掉TAB_SITE是远程试图
                string commandText = "DELETE FROM BASEORGANIZE WHERE id < 1000000 AND id NOT IN (SELECT id FROM TAB_SITE)";
                organizationManager.DbHelper.ExecuteNonQuery(commandText);

                // 同步数据
                commandText = "SELECT * FROM TAB_SITE WHERE BL_NOT_INPUT IS NULL OR BL_NOT_INPUT = 0 ";
                if (!string.IsNullOrEmpty(conditional))
                {
                    commandText += conditional;
                }

                var dataReader = dbHelper.ExecuteReader(commandText);

                while (dataReader.Read())
                {
                    // 这里需要从数据库读取、否则容易造成丢失数据
                    BaseOrganizationEntity entity = organizationManager.GetEntity(dataReader["ID"].ToString());
                    if (entity == null)
                    {
                        entity = new BaseOrganizationEntity();
                        //entity.Id = dr["ID"].ToString();
                    }
                    entity.Code = dataReader["SITE_CODE"].ToString();
                    if (string.IsNullOrEmpty(entity.ParentName) || !entity.ParentName.Equals(dataReader["SUPERIOR_SITE"].ToString()))
                    {
                        entity.ParentName = dataReader["SUPERIOR_SITE"].ToString();
                        entity.ParentId = 0;
                    }

                    entity.Name = dataReader["SITE_NAME"].ToString();
                    entity.ShortName = dataReader["SITE_NAME"].ToString();
                    entity.CategoryCode = dataReader["TYPE"].ToString();
                    entity.OuterPhone = dataReader["PHONE"].ToString();
                    entity.Fax = dataReader["FAX"].ToString();
                    entity.Province = dataReader["PROVINCE"].ToString();
                    entity.City = dataReader["CITY"].ToString();
                    entity.District = dataReader["RANGE_NAME"].ToString();

                    entity.CostCenter = dataReader["SUPERIOR_FINANCE_CENTER"].ToString();
                    entity.Area = dataReader["BIG_AREA_NAME"].ToString();
                    entity.CompanyName = dataReader["SITE1_NAME"].ToString();

                    if (!string.IsNullOrEmpty(dataReader["ORDER_BY"].ToString()))
                    {
                        entity.SortCode = int.Parse(dataReader["ORDER_BY"].ToString());
                    }
                    // 02：可以把读取到的数据能写入到用户中心的。
                    result = organizationManager.UpdateEntity(entity);
                    if (result == 0)
                    {
                        organizationManager.AddEntity(entity);
                    }
                }
                dataReader.Close();
                // 填充 parentname
                // select * from baseorganization where parentname is null
                commandText = @"update baseorganization set parentname = (select fullname from baseorganization t where t.id = baseorganization.parentId) where parentname is null";
                this.DbHelper.ExecuteNonQuery(commandText);
                // 填充 parentId
                // select * from baseorganization where parentId is null
                commandText = @"UPDATE baseorganization SET parentId = (SELECT Id FROM baseorganization t WHERE t.fullname = baseorganization.parentname) WHERE parentId IS NULL";
                // 100000 以下是基础数据的，100000 以上是通用权限管理系统的
                // UPDATE baseorganization SET parentId = (SELECT Id FROM baseorganization t WHERE t.fullname = baseorganization.parentname) WHERE parentId < 100000
                this.DbHelper.ExecuteNonQuery(commandText);
                // 更新错误数据
                commandText = @"UPDATE baseorganization SET parentId = null WHERE id = parentId";
                this.DbHelper.ExecuteNonQuery(commandText);
                // 设置员工的公司主键
                commandText = @"UPDATE baseuser SET companyid = (SELECT MAX(Id) FROM baseorganization WHERE baseorganization.fullname = baseuser.companyname AND baseorganization.Id < 1000000) WHERE companyId IS NULL OR companyId = ''";
                this.DbHelper.ExecuteNonQuery(commandText);
            }
            return result;
        }
    }
}