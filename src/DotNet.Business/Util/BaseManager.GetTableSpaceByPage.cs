//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2024, DotNet.
//-----------------------------------------------------------------

using System.Data;
using System.Text;

namespace DotNet.Business
{
    using Util;

    /// <summary>
    ///	BaseManager
    /// 通用基类部分
    /// 
    /// 修改记录
    /// 
    ///		2020.09.26 版本：Troy.Cui进行扩展。
    ///
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2020.09.26</date>
    /// </author> 
    /// </summary>
    public partial class BaseManager : IBaseManager
    {
        #region 查询数据库表空间
        /// <summary>
        /// 查询数据库表空间
        /// </summary>
        /// <param name="searchKey">查询关键字</param>
        /// <param name="recordCount">记录数</param>
        /// <param name="pageNo">当前页</param>
        /// <param name="pageSize">每页显示</param>
        /// <param name="sortExpression">排序字段</param>
        /// <param name="sortDirection">排序方向</param>
        /// <returns>数据表</returns>
        public virtual DataTable GetTableSpaceByPage(string searchKey, out int recordCount, int pageNo = 1, int pageSize = 20, string sortExpression = "Id", string sortDirection = "DESC")
        {
            var sb = PoolUtil.StringBuilder.Get();
            switch (DbHelper.CurrentDbType)
            {
                case CurrentDbType.Access:
                    break;
                case CurrentDbType.SqLite:
                    break;
                case CurrentDbType.SqlServer:

                    sb.AppendLine("SELECT ROW_NUMBER() OVER (ORDER BY B.rows ASC) AS Id, A.name AS TableName, C.value AS TableDescription, B.rows AS TotalRows,CAST((8*CAST(B.dpages AS FLOAT)/1024) AS DECIMAL(18,2)) AS UsedSpace");
                    sb.AppendLine("FROM sysobjects AS A INNER JOIN sysindexes AS B ON A.id = B.id");
                    sb.AppendLine("LEFT JOIN sys.extended_properties C ON A.id = C.major_id AND minor_id = 0 AND C.name = 'MS_Description'");
                    sb.AppendLine("WHERE (A.type = 'u') AND(B.indid IN(0, 1))");
                    if (!string.IsNullOrEmpty(searchKey))
                    {
                        searchKey = StringUtil.GetLikeSearchKey(dbHelper.SqlSafe(searchKey));
                        sb.AppendLine(" AND (CAST(A.name AS NVARCHAR(MAX)) LIKE N'%" + searchKey + "%' OR CAST(C.value AS NVARCHAR(MAX)) LIKE N'%" + searchKey + "%')");
                    }
                    break;
                case CurrentDbType.Oracle:

                    break;
                case CurrentDbType.MySql:

                    break;

            }
            sb.Replace(" 1 = 1 AND ", "");
            return GetDataTableByPage(out recordCount, pageNo, pageSize, sortExpression, sortDirection, sb.Return(), null);
        }
        #endregion
    }
}