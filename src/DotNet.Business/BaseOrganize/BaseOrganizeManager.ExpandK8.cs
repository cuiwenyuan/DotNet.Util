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
    /// BaseOrganizeManager
    /// 组织机构管理
    /// 
    /// 修改纪录
    /// 
    ///		2013.10.20 版本：2.0 JiRiGaLa	集成K8物流系统的登录功能。
    ///		2011.10.17 版本：1.0 JiRiGaLa	主键整理。
    /// 
    /// <author>
    ///		<name>JiRiGaLa</name>
    ///		<date>2011.10.17</date>
    /// </author> 
    /// </summary>
    public partial class BaseOrganizeManager : BaseManager
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
                string commandText = "SELECT MAX(id) FROM BaseOrganize WHERE id < 1000000";
                Object maxObject = DbHelper.ExecuteScalar(commandText);
                if (maxObject != null)
                {
                    id = int.Parse(maxObject.ToString());
                }
                conditional = " AND ID > " + id.ToString();
            }
            result = ImportK8Organize(connectionString, conditional);
            return result;
        }

        /// <summary>
        /// 导入K8系统网点信息
        /// </summary>
        /// <param name="connectionString">数据库连接</param>
        /// <param name="conditional">条件，不需要同步所有的数据</param>
        /// <returns>影响行数</returns>
        public int ImportK8Organize(string connectionString = null, string conditional = null)
        {
            // delete from baseorganize where id < 1000000
            int result = 0;
            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = ConfigurationUtil.AppSettings("K8Connection", BaseSystemInfo.EncryptDbConnection);
            }
            if (!string.IsNullOrEmpty(connectionString))
            {
                // 01：可以从k8里读取公司、用户、密码的。
                IDbHelper dbHelper = DbHelperFactory.GetHelper(CurrentDbType.Oracle, connectionString);
                BaseOrganizeManager organizeManager = new Business.BaseOrganizeManager(this.DbHelper, this.UserInfo);
                // 不不存在的组织机构删除掉TAB_SITE是远程试图
                string commandText = "DELETE FROM BASEORGANIZE WHERE id < 1000000 AND id NOT IN (SELECT id FROM TAB_SITE)";
                organizeManager.DbHelper.ExecuteNonQuery(commandText);

                // 同步数据
                commandText = "SELECT * FROM TAB_SITE WHERE BL_NOT_INPUT IS NULL OR BL_NOT_INPUT = 0 ";
                if (!string.IsNullOrEmpty(conditional))
                {
                    commandText += conditional;
                }
                using (IDataReader dr = dbHelper.ExecuteReader(commandText))
                {
                    while (dr.Read())
                    {
                        // 这里需要从数据库读取、否则容易造成丢失数据
                        BaseOrganizeEntity entity = organizeManager.GetObject(dr["ID"].ToString());
                        if (entity == null)
                        {
                            entity = new BaseOrganizeEntity();
                            entity.Id = dr["ID"].ToString();
                        }
                        entity.Code = dr["SITE_CODE"].ToString();
                        if (string.IsNullOrEmpty(entity.ParentName) || !entity.ParentName.Equals(dr["SUPERIOR_SITE"].ToString()))
                        {
                            entity.ParentName = dr["SUPERIOR_SITE"].ToString();
                            entity.ParentId = null;
                        }
                        
                        entity.FullName = dr["SITE_NAME"].ToString();
                        entity.ShortName = dr["SITE_NAME"].ToString();
                        entity.CategoryCode = dr["TYPE"].ToString();
                        entity.OuterPhone = dr["PHONE"].ToString();
                        entity.Fax = dr["FAX"].ToString();
                        entity.Province = dr["PROVINCE"].ToString();
                        entity.City = dr["CITY"].ToString();
                        entity.District = dr["RANGE_NAME"].ToString();

                        entity.CostCenter = dr["SUPERIOR_FINANCE_CENTER"].ToString();
                        entity.Area = dr["BIG_AREA_NAME"].ToString();
                        entity.CompanyName = dr["SITE1_NAME"].ToString();
                        entity.JoiningMethods = dr["NC_TYPE"].ToString();

                        if (!string.IsNullOrEmpty(dr["ORDER_BY"].ToString()))
                        {
                            entity.SortCode = int.Parse(dr["ORDER_BY"].ToString());
                        }
                        // 02：可以把读取到的数据能写入到用户中心的。
                        result = organizeManager.UpdateObject(entity);
                        if (result == 0)
                        {
                            organizeManager.AddObject(entity);
                        }
                    }
                    dr.Close();
                }
                // 填充 parentname
                // select * from baseorganize where parentname is null
                commandText = @"update baseorganize set parentname = (select fullname from baseorganize t where t.id = baseorganize.parentId) where parentname is null";
                this.DbHelper.ExecuteNonQuery(commandText);
                // 填充 parentId
                // select * from baseorganize where parentId is null
                commandText = @"UPDATE baseorganize SET parentId = (SELECT Id FROM baseorganize t WHERE t.fullname = baseorganize.parentname) WHERE parentId IS NULL";
                // 100000 以下是基础数据的，100000 以上是通用权限管理系统的
                // UPDATE baseorganize SET parentId = (SELECT Id FROM baseorganize t WHERE t.fullname = baseorganize.parentname) WHERE parentId < 100000
                this.DbHelper.ExecuteNonQuery(commandText);
                // 更新错误数据
                commandText = @"UPDATE baseorganize SET parentId = null WHERE id = parentId";
                this.DbHelper.ExecuteNonQuery(commandText);
                // 设置员工的公司主键
                commandText = @"UPDATE baseuser SET companyid = (SELECT MAX(Id) FROM baseorganize WHERE baseorganize.fullname = baseuser.companyname AND baseorganize.Id < 1000000) WHERE companyId IS NULL OR companyId = ''";
                this.DbHelper.ExecuteNonQuery(commandText);
            }
            return result;
        }
    }
}