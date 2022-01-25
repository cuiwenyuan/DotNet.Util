//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using DotNet.Util;
using System;

namespace DotNet.Business
{
    /// <summary>
    ///	BaseManager
    /// 通用基类部分
    /// 
    /// 修改记录
    /// 
    ///		2020.07.03 版本：Troy.Cui进行扩展。
    ///
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2020.07.03</date>
    /// </author> 
    /// </summary>
    public partial class BaseManager : IBaseManager
    {
        #region 是否被用过

        /// <summary>
        /// 是否被用过
        /// </summary>
        /// <param name="id">行id</param>
        /// <param name="tableName">要检查的关联表名</param>
        /// <param name="tableFieldName">要检查的关联表字段名</param>
        /// <param name="checkUserCompany">是否检查公司数据</param>
        /// <returns>是否</returns>
        public virtual bool IsUsed(string id, string tableName, string tableFieldName, bool checkUserCompany = false)
        {
            var result = false;
            if (DbUtil.Exists(dbHelper, tableName) && ValidateUtil.IsInt(id))
            {
                var sb = Pool.StringBuilder.Get();
                sb.Append("SELECT COUNT(*) FROM " + CurrentTableName + " A INNER JOIN " + tableName + " B ON B." + tableFieldName + " = A.Id WHERE A.Id = " + id + "");
                //未删除
                sb.Append(" AND A." + (BaseUtil.FieldDeleted) + " = 0 AND A." + BaseUtil.FieldEnabled + " = 1 ");
                sb.Append(" AND B." + (BaseUtil.FieldDeleted) + " = 0 AND B." + BaseUtil.FieldEnabled + " = 1 ");
                //当前用户所在公司或者系统公用数据
                if (checkUserCompany)
                {
                    sb.Append(" AND (A.UserCompanyId = 0 OR A.UserCompanyId = " + UserInfo.CompanyId + ")");
                    sb.Append(" AND (B.UserCompanyId = 0 OR B.UserCompanyId = " + UserInfo.CompanyId + ")");
                }
                var obj = ExecuteScalar(sb.Put());
                if (obj != null && Convert.ToInt32(obj) > 0)
                {
                    result = true;
                }
            }
            return result;
        }
        #endregion

        #region 是否被用过（批量）

        /// <summary>
        /// 是否被用过（批量）
        /// </summary>
        /// <param name="ids">行Ids</param>
        /// <param name="tableName">要检查的关联表名</param>
        /// <param name="tableFieldName">要检查的关联表字段名</param>
        /// <param name="checkUserCompany">是否检查公司数据</param>
        /// <returns>是否</returns>
        public virtual bool IsUsed(string[] ids, string tableName, string tableFieldName, bool checkUserCompany = false)
        {
            var result = false;
            for (var i = 0; i <= ids.Length - 1; i++)
            {
                result = IsUsed(ids[i], tableName, tableFieldName, checkUserCompany);
                if (result)
                {
                    return result;
                }
            }

            return result;

        }
        #endregion
    }
}