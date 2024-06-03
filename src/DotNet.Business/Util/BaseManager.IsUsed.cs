//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2024, DotNet.
//-----------------------------------------------------------------

using System;

namespace DotNet.Business
{
    using Util;

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
        /// <param name="currentTableFieldName">当前表的关联字段名（默认为Id）</param>
        /// <param name="checkUserCompany">是否检查公司数据</param>
        /// <returns>是否</returns>
        public virtual bool IsUsed(string id, string tableName, string tableFieldName, string currentTableFieldName = BaseUtil.FieldId, bool checkUserCompany = false)
        {
            var result = false;
            if (dbHelper.TableExists(tableName) && ValidateUtil.IsInt(id))
            {
                var sb = PoolUtil.StringBuilder.Get();
                sb.Append("SELECT COUNT(*) FROM " + CurrentTableName + " A INNER JOIN " + tableName + " B ON B." + tableFieldName + " = A." + currentTableFieldName + " WHERE A.Id = " + id + "");
                //未删除
                sb.Append(" AND A." + (BaseUtil.FieldDeleted) + " = 0 AND A." + BaseUtil.FieldEnabled + " = 1 ");
                sb.Append(" AND B." + (BaseUtil.FieldDeleted) + " = 0 AND B." + BaseUtil.FieldEnabled + " = 1 ");
                //当前用户所在公司或者系统公用数据
                if (checkUserCompany)
                {
                    sb.Append(" AND (A.UserCompanyId = 0 OR A.UserCompanyId = " + UserInfo.CompanyId + ")");
                    sb.Append(" AND (B.UserCompanyId = 0 OR B.UserCompanyId = " + UserInfo.CompanyId + ")");
                }
                var obj = ExecuteScalar(sb.Return());
                if (obj != null && obj.ToInt() > 0)
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
        /// <param name="currentTableFieldName">当前表的关联字段名（默认为Id）</param>
        /// <param name="checkUserCompany">是否检查公司数据</param>
        /// <returns>是否</returns>
        public virtual bool IsUsed(string[] ids, string tableName, string tableFieldName, string currentTableFieldName = BaseUtil.FieldId, bool checkUserCompany = false)
        {
            var result = false;
            for (var i = 0; i <= ids.Length - 1; i++)
            {
                result = IsUsed(ids[i], tableName, tableFieldName, currentTableFieldName: currentTableFieldName, checkUserCompany: checkUserCompany);
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