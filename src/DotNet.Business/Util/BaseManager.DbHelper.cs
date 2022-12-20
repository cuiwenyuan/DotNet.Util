//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2022, DotNet.
//-----------------------------------------------------------------

using System.Data;

namespace DotNet.Business
{
    /// <summary>
    ///	BaseManager
    /// 通用基类部分
    /// 调用DbHelper 执行语句
    /// 
    /// 总觉得自己写的程序不上档次，这些新技术也玩玩，也许做出来的东西更专业了。
    /// 修改记录
    /// 
    ///		2012.02.04 版本：1.0 JiRiGaLa 进行提炼，把代码进行分组。
    ///
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2012.02.04</date>
    /// </author> 
    /// </summary>
    public partial class BaseManager : IBaseManager
    {
        #region public virtual int ExecuteNonQuery(string commandText) 执行查询语句
        /// <summary>
        /// 执行查询语句
        /// </summary>
        /// <param name="commandText">sql查询</param>
        /// <returns>影响行数</returns>
        public virtual int ExecuteNonQuery(string commandText)
        {
            return DbHelper.ExecuteNonQuery(commandText);
        }
        #endregion

        #region public virtual object ExecuteScalar(string commandText) 执行查询语句
        /// <summary>
        /// 执行查询语句
        /// </summary>
        /// <param name="commandText">sql查询</param>
        /// <returns>object</returns>
        public virtual object ExecuteScalar(string commandText)
        {
           return DbHelper.ExecuteScalar(commandText);
        }
        #endregion

        #region public virtual int ExecuteNonQuery(string commandText, IDbDataParameter[] dbParameters) 执行查询语句
        /// <summary>
        /// 执行查询语句
        /// </summary>
        /// <param name="commandText">sql查询</param>
        /// <param name="dbParameters">参数集</param>
        /// <returns>影响行数</returns>
        public virtual int ExecuteNonQuery(string commandText, IDbDataParameter[] dbParameters)
        {
            return DbHelper.ExecuteNonQuery(commandText, dbParameters);
        }
        #endregion

        #region public virtual object ExecuteScalar(string commandText, IDbDataParameter[] dbParameters) 执行查询语句
        /// <summary>
        /// 执行查询语句
        /// </summary>
        /// <param name="commandText">sql查询</param>
        /// <param name="dbParameters">参数集</param>
        /// <returns>Object</returns>
        public virtual object ExecuteScalar(string commandText, IDbDataParameter[] dbParameters)
        {
            return DbHelper.ExecuteScalar(commandText, dbParameters);
        }
        #endregion

        #region public virtual DataTable Fill(string commandText) 填充数据表
        /// <summary>
        /// 填充数据表
        /// </summary>
        /// <param name="commandText">查询</param>
        /// <returns>数据表</returns>
        public virtual DataTable Fill(string commandText)
        {
            return DbHelper.Fill(commandText);
        }
        #endregion

        #region public virtual DataTable Fill(string commandText, IDbDataParameter[] dbParameters) 填充数据表
        /// <summary>
        /// 填充数据表
        /// </summary>
        /// <param name="commandText">sql查询</param>
        /// <param name="dbParameters">参数集</param>
        /// <returns>数据表</returns>
        public virtual DataTable Fill(string commandText, IDbDataParameter[] dbParameters)
        {
            return DbHelper.Fill(commandText, dbParameters);
        }
        #endregion

        #region public void SqlBulkCopyData(DataTable dt) 利用Net SqlBulkCopy 批量导入数据库,速度超快

        /// <summary>
        /// 利用Net SqlBulkCopy 批量导入数据库,速度超快
        /// </summary>
        /// <param name="dt">源内存数据表（先通过SELECT TOP 0获取空白DataTable）</param>
        /// <param name="destinationTableName">目标表名称</param>
        /// <param name="bulkCopyTimeout">超时限制（毫秒）</param>
        /// <param name="batchSize">批大小（默认0，即一次性导入）</param>
        public virtual bool SqlBulkCopyData(DataTable dt, string destinationTableName, int bulkCopyTimeout = 1000, int batchSize = 0)
        {
            return DbHelper.SqlBulkCopyData(dt, destinationTableName, bulkCopyTimeout, batchSize);
        }
        #endregion
    }
}