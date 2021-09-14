//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System;
using DotNet.Util;
using System.Data;
using System.Text;

namespace DotNet.Business
{
    /// <summary>
    ///	BaseManager
    /// 通用基类部分
    /// 
    /// 修改纪录
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
            var sb = Pool.StringBuilder.Get();
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
                var dt = Fill(sb.Put());
                //database_name,database_size
                result = decimal.Parse(dt.Rows[0][1].ToString().Replace("MB", "").Trim()) / 1024;
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