//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2023, DotNet.
//-----------------------------------------------------------------

using System;
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
        #region 获取当前数据库大小
        /// <summary>
        /// 获取当前数据库大小
        /// </summary>
        /// <returns></returns>
        public virtual decimal GetDatabaseSize()
        {
            var result = 0M;
            var sb = PoolUtil.StringBuilder.Get();
            switch (DbHelper.CurrentDbType)
            {
                case CurrentDbType.Access:
                    break;
                case CurrentDbType.SqLite:
                    break;
                case CurrentDbType.SqlServer:
                    sb.Append("EXEC sp_spaceused");
                    break;
                case CurrentDbType.Oracle:
                    break;
                case CurrentDbType.MySql:
                    break;
            }
            try
            {
                var dt = Fill(sb.Return());
                if (dt != null && dt.Rows.Count > 0)
                {
                    //database_name,database_size
                    result = decimal.Parse(dt.Rows[0][1].ToString().Replace("MB", "").Trim()) / 1024;
                }
            }
            catch (Exception ex)
            {
                LogUtil.WriteException(ex);
            }

            return result;
        }
        #endregion
    }
}