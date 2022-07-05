//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System.Collections.Generic;

namespace DotNet.Business
{
    using Util;

    /// <summary>
    ///	DbSortUtil
    /// 通用排序逻辑基类（程序OK）
    /// 
    /// 修改记录
    ///
    ///     2021.08.30 版本:    1.7 Troy.Cui 类名修改
    ///     2018.09.19 版本:    1.6 Troy.Cui 修复SetTop和SetDown，数据库直接添加的数据，无法置顶和置底，SortCode越小越排在上面
    ///     2018.09.04 版本：   1.5 Troy.Cui 修复GetDownId和GetUpId
    ///     2018.08.20 版本：   1.4 Troy.Cui 修复相同的SortCode排序bug
    ///		2007.12.10 版本：   1.3 JiRiGaLa 改进 序列产生码的长度问题。
    ///		2007.11.01 版本：   1.2 JiRiGaLa 改进 BUOperatorInfo 去掉这个变量，可以提高性能，提高速度，基类的又一次飞跃。
    ///		2007.03.01 版本：   1.0 JiRiGaLa 将主键从 BUBaseUtil 类分离出来。
    ///	
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2007.12.10</date>
    /// </author> 
    /// </summary>
    public class DbSortUtil
    {
        /// <summary>
        /// 置顶排方法令命名定义
        /// </summary>
        public const string CommandSetTop       = "SetTop";

        /// <summary>
        /// 上移排序方法命名定义
        /// </summary>
        public const string CommandSetUp        = "SetUp";

        /// <summary>
        /// 下移排序方法命名定义
        /// </summary>
        public const string CommandSetDown      = "SetDown";

        /// <summary>
        /// 置底排序方法命名定义
        /// </summary>
        public const string CommandSetBottom    = "SetBottom";

        /// <summary>
        /// 交换排序方法命名定义
        /// </summary>
        public const string CommandSwap         = "Swap";

        //
        // 排序操作针对数据库的运算方式定义（好用高效的排序顺序设定方法）
        //

        #region 置顶排序(SortCode变小)
        /// <summary>
        /// 置顶排序(SortCode变小)
        /// </summary>
        /// <param name="dbHelper">当前数据库连接</param>
        /// <param name="tableName">表名</param>
        /// <param name="id">主键</param>
        /// <returns>影响行数</returns>
        public static int SetTop(IDbHelper dbHelper, string tableName, string id)
        {
            var managerSequence = new BaseSequenceManager(dbHelper);
            //先重置一下，避免手动添加的数据造成功能无效
            managerSequence.Reset(tableName);
            var sequence = managerSequence.GetReduction(tableName);
            var whereParameters =
                new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>(BaseUtil.FieldId, id)
                };
            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseUtil.FieldSortCode, sequence)
            };
            return dbHelper.SetProperty(tableName, whereParameters, parameters);
        }
        #endregion

        /// <summary>
        /// 上移
        /// </summary>
        /// <param name="dbHelper"></param>
        /// <param name="tableName"></param>
        /// <param name="id"></param>
        /// <param name="sequenceName"></param>
        /// <returns></returns>
        public static int SetTop(IDbHelper dbHelper, string tableName, string id, string sequenceName)
        {
            if (string.IsNullOrEmpty(sequenceName))
            {
                sequenceName = tableName;
            }
            var managerSequence = new BaseSequenceManager(dbHelper);
            //先重置一下，避免手动添加的数据造成功能无效
            managerSequence.Reset(sequenceName);
            var sequence = managerSequence.Increment(sequenceName);

            var whereParameters =
                new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>(BaseUtil.FieldId, id)
                };
            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseUtil.FieldSortCode, sequence)
            };
            return dbHelper.SetProperty(tableName, whereParameters, parameters);
        }


        #region 交换排序命令
        /// <summary>
        /// 交换排序方法
        /// </summary>
        /// <param name="dbHelper">当前数据库连接</param>
        /// <param name="tableName">表名</param>
        /// <param name="id">记录主键</param>
        /// <param name="targetId">目标记录主键</param>
        /// <returns>影响行数</returns>
        public static int Swap(IDbHelper dbHelper, string tableName, string id, string targetId)
        {
            var result = 0;
            // 要移动的主键的排序码
            var parameters =
                new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>(BaseUtil.FieldId, id)
                };
            var sortCode = dbHelper.GetProperty(tableName, parameters, BaseUtil.FieldSortCode);
            // 目标主键的排序码
            parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseUtil.FieldId, targetId)
            };
            var targetSortCode = dbHelper.GetProperty(tableName, parameters, BaseUtil.FieldSortCode);

            // 以下方法，在MySQL里不能正常运行，虽然效率是很高
            // 设置要移动的主键的排序码（注：少读取数据库一次，提高主键运行效率）
            // string sql = "UPDATE " + tableName
            //                    + " SET " + BaseUtil.FieldSortCode + " = (SELECT " + BaseUtil.FieldSortCode
            //                    + " FROM " + tableName
            //                    + " WHERE " + BaseUtil.FieldId + " = '" + targetId + "') "
            //                    + " WHERE " + BaseUtil.FieldId + " = '" + Id + "' ";
            // result = ExecuteNonQuery(sql);

            var whereParameters =
                new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>(BaseUtil.FieldId, id)
                };
            parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseUtil.FieldSortCode, targetSortCode)
            };
            // 设置目标主键的排序码
            result += dbHelper.SetProperty(tableName, whereParameters, parameters);
            whereParameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseUtil.FieldId, targetId)
            };
            parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseUtil.FieldSortCode, sortCode)
            };
            // 设置目标主键的排序码
            result += dbHelper.SetProperty(tableName, whereParameters, parameters);
            return result;
        }
        #endregion

        #region 置底排序(SortCode变大)
        /// <summary>
        /// 置底排序(SortCode变小)
        /// </summary>
        /// <param name="dbHelper">当前数据库连接</param>
        /// <param name="tableName">表名</param>
        /// <param name="id">主键</param>
        /// <returns>影响行数</returns>
        public static int SetBottom(IDbHelper dbHelper, string tableName, string id)
        {
            var managerSequence = new BaseSequenceManager(dbHelper);
            //先重置一下，避免手动添加的数据造成功能无效
            managerSequence.Reset(tableName);
            var sequence = managerSequence.Increment(tableName);
            var whereParameters =
                new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>(BaseUtil.FieldId, id)
                };
            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseUtil.FieldSortCode, sequence)
            };
            return dbHelper.SetProperty(tableName, whereParameters, parameters);
        }
        #endregion

        /// <summary>
        /// 置底
        /// </summary>
        /// <param name="dbHelper"></param>
        /// <param name="tableName"></param>
        /// <param name="id"></param>
        /// <param name="sequenceName"></param>
        /// <returns></returns>
        public static int SetBottom(IDbHelper dbHelper, string tableName, string id, string sequenceName)
        {
            if (string.IsNullOrEmpty(sequenceName))
            {
                sequenceName = tableName;
            }
            var managerSequence = new BaseSequenceManager(dbHelper);
            //先重置一下，避免手动添加的数据造成功能无效
            managerSequence.Reset(sequenceName);
            var sequence = managerSequence.Increment(sequenceName);

            var whereParameters =
                new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>(BaseUtil.FieldId, id)
                };
            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseUtil.FieldSortCode, sequence)
            };
            return dbHelper.SetProperty(tableName, whereParameters, parameters);
        }

        //以下是通过数据库底层进行排序位置交换（这些方法不常用）

        #region 获得上一个记录主键
        /// <summary>
        /// 获得上一个记录主键(SortCode变小)
        /// </summary>
        /// <param name="dbHelper">当前数据库连接</param>
        /// <param name="tableName">表名</param>
        /// <param name="categoryCode">类别主键</param>
        /// <param name="id">当前主键</param>
        /// <param name="whereSubQuery">Where条件</param>
        /// <returns>目标主键</returns>
        public static string GetUpId(IDbHelper dbHelper, string tableName, string categoryCode, string id, string whereSubQuery = null)
        {
            var subQuery = string.Empty;
            if (!string.IsNullOrEmpty(categoryCode))
            {
                subQuery = BaseUtil.FieldCategoryCode + " = '" + categoryCode + "' AND ";
            }
            if (!string.IsNullOrEmpty(whereSubQuery))
            {
                subQuery += whereSubQuery + " AND ";
            }

            var sb = Pool.StringBuilder.Get();
            sb.Append("SELECT COUNT(*) FROM " + tableName
                + " WHERE ( " + subQuery + BaseUtil.FieldSortCode + " < (SELECT TOP 1 ISNULL(" + BaseUtil.FieldSortCode
                + "," + BaseUtil.FieldId + ") FROM " + tableName
                + " WHERE ( " + subQuery + BaseUtil.FieldId + " = '" + id + "' ) "
                + " ORDER BY " + BaseUtil.FieldSortCode + " )) ");
            if (dbHelper.ExecuteScalar(sb.ToString()).Equals("0"))
            {
                sb.Clear();
                sb.Append("SELECT TOP 1 " + BaseUtil.FieldId
                                           + " FROM " + tableName
                                           + " WHERE ( " + subQuery + BaseUtil.FieldSortCode + " <= (SELECT TOP 1 ISNULL(" + BaseUtil.FieldSortCode
                                           + "," + BaseUtil.FieldId + ") FROM " + tableName
                                           + " WHERE ( " + subQuery + BaseUtil.FieldId + " = '" + id + "' ) "
                                           + " ORDER BY " + BaseUtil.FieldSortCode + " )) "
                                           + " ORDER BY " + BaseUtil.FieldSortCode + " DESC, " + BaseUtil.FieldId + " DESC ");
            }
            else
            {
                sb.Clear();
                sb.Append("SELECT TOP 1 " + BaseUtil.FieldId
                                           + " FROM " + tableName
                                           + " WHERE ( " + subQuery + BaseUtil.FieldSortCode + " < (SELECT TOP 1 ISNULL(" + BaseUtil.FieldSortCode
                                           + "," + BaseUtil.FieldId + ") FROM " + tableName
                                           + " WHERE ( " + subQuery + BaseUtil.FieldId + " = '" + id + "' ) "
                                           + " ORDER BY " + BaseUtil.FieldSortCode + " )) "
                                           + " ORDER BY " + BaseUtil.FieldSortCode + " DESC, " + BaseUtil.FieldId + " DESC ");
            }

            return dbHelper.ExecuteScalar(sb.Put()).ToString();
        }
        #endregion

        #region 获得上一个记录主键
        /// <summary>
        /// 获得上一个记录主键
        /// </summary>
        /// <param name="dbHelper">当前数据库连接</param>
        /// <param name="tableName">表名</param>
        /// <param name="id">当前主键</param>
        /// <param name="whereSubQuery">Where条件</param>
        /// <returns>目标主键</returns>
        public static string GetUpId(IDbHelper dbHelper, string tableName, string id, string whereSubQuery = null)
        {
            //有bug
            return GetUpId(dbHelper, tableName, id);
        }
        #endregion

        #region 上移(SortCode变大)

        /// <summary>
        /// 上移(SortCode变大)
        /// </summary>
        /// <param name="dbHelper">当前数据库连接</param>
        /// <param name="tableName">表名</param>
        /// <param name="categoryId">类别主键</param>
        /// <param name="id">当前主键</param>
        /// <param name="whereSubQuery"></param>
        /// <returns>目标主键</returns>
        public static int SetUp(IDbHelper dbHelper, string tableName, string categoryId, string id, string whereSubQuery = null)
        {
            var upId = GetUpId(dbHelper, tableName, categoryId, id, whereSubQuery);

            var result = 0;
            if (upId.Length == 0)
            {
                return result;
            }

            var parameters =
                new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>(BaseUtil.FieldId, id)
                };
            var sortCode = dbHelper.GetProperty(tableName, parameters, BaseUtil.FieldSortCode);

            parameters =
                new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>(BaseUtil.FieldId, upId)
                };
            var upSortCode = dbHelper.GetProperty(tableName, parameters, BaseUtil.FieldSortCode);
            
            //把upId的SortCode更新为当前SortCode
            var whereParameters =
                new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>(BaseUtil.FieldId, upId)
                };
            parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseUtil.FieldSortCode, sortCode)
            };
            dbHelper.SetProperty(tableName, whereParameters, parameters);

            //把当前的SortCode更新为upId的upSortCode
            whereParameters =
                new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>(BaseUtil.FieldId, id)
                };
            parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseUtil.FieldSortCode, upSortCode)
            };
            result = dbHelper.SetProperty(tableName, whereParameters, parameters);
            return result;
        }
        #endregion

        #region 上移(SortCode变大)

        /// <summary>
        /// 上移(SortCode变大)
        /// </summary>
        /// <param name="dbHelper">当前数据库连接</param>
        /// <param name="tableName">表名</param>
        /// <param name="id">当前主键</param>
        /// <param name="whereSubQuery"></param>
        /// <returns>目标主键</returns>
        public static int SetUp(IDbHelper dbHelper, string tableName, string id, string whereSubQuery = null)
        {
            return SetUp(dbHelper, tableName, string.Empty, id, whereSubQuery);
        }
        #endregion

        #region 获得下一个记录主键

        /// <summary>
        /// 获得下一个记录主键(SortCode变大)
        /// </summary>
        /// <param name="dbHelper">当前数据库连接</param>
        /// <param name="tableName">表名</param>
        /// <param name="categoryCode">类别主键</param>
        /// <param name="id">当前主键</param>
        /// <param name="whereSubQuery"></param>
        /// <returns>目标主键</returns>
        public static string GetDownId(IDbHelper dbHelper, string tableName, string categoryCode, string id, string whereSubQuery = null)
        {
            var subQuery = string.Empty;
            if (!string.IsNullOrEmpty(categoryCode))
            {
                subQuery = BaseUtil.FieldCategoryCode + " = '" + categoryCode + "' AND ";
            }
            if (!string.IsNullOrEmpty(whereSubQuery))
            {
                subQuery += whereSubQuery + " AND ";
            }

            var sb = Pool.StringBuilder.Get();
            sb.Append("SELECT COUNT(*) FROM " + tableName
                + " WHERE ( " + subQuery + BaseUtil.FieldSortCode + " > (SELECT TOP 1 ISNULL(" + BaseUtil.FieldSortCode
                + "," + BaseUtil.FieldId + ") FROM " + tableName
                + " WHERE ( " + subQuery + BaseUtil.FieldId + " = '" + id + "' ) "
                + " ORDER BY " + BaseUtil.FieldSortCode + " )) ");
            if (dbHelper.ExecuteScalar(sb.ToString()).Equals("0"))
            {
                sb.Clear();
                sb.Append("SELECT TOP 1 " + BaseUtil.FieldId
                                           + " FROM " + tableName
                                           + " WHERE ( " + subQuery + BaseUtil.FieldSortCode + " >= (SELECT TOP 1 ISNULL(" + BaseUtil.FieldSortCode
                                           + "," + BaseUtil.FieldId + ") FROM " + tableName
                                           + " WHERE ( " + subQuery + BaseUtil.FieldId + " = '" + id + "' ) "
                                           + " ORDER BY " + BaseUtil.FieldSortCode + " )) "
                                           + " ORDER BY " + BaseUtil.FieldSortCode + " ASC, " + BaseUtil.FieldId + " ASC");
            }
            else
            {
                sb.Clear();
                sb.Append("SELECT TOP 1 " + BaseUtil.FieldId
                                           + " FROM " + tableName
                                           + " WHERE ( " + subQuery + BaseUtil.FieldSortCode + " > (SELECT TOP 1 ISNULL(" + BaseUtil.FieldSortCode
                                           + "," + BaseUtil.FieldId + ") FROM " + tableName
                                           + " WHERE ( " + subQuery + BaseUtil.FieldId + " = '" + id + "' ) "
                                           + " ORDER BY " + BaseUtil.FieldSortCode + " )) "
                                           + " ORDER BY " + BaseUtil.FieldSortCode + " ASC, " + BaseUtil.FieldId + " ASC");
            }
            
            return dbHelper.ExecuteScalar(sb.Put()).ToString();
        }
        #endregion

        #region 获得下一个记录主键
        /// <summary>
        /// 获得下一个记录主键
        /// </summary>
        /// <param name="dbHelper">当前数据库连接</param>
        /// <param name="tableName">表名</param>
        /// <param name="id">当前主键</param>
        /// <returns>目标主键</returns>
        public static string GetDownId(IDbHelper dbHelper, string tableName, string id)
        {
            return GetDownId(dbHelper, tableName, string.Empty, id);
        }
        #endregion

        #region 下移(SortCode变小)

        /// <summary>
        /// 下移(SortCode变小)
        /// </summary>
        /// <param name="dbHelper">当前数据库连接</param>
        /// <param name="tableName">表名</param>
        /// <param name="categoryId">类别主键</param>
        /// <param name="id">当前主键</param>
        /// <param name="whereSubQuery"></param>
        /// <returns>目标主键</returns>
        public static int SetDown(IDbHelper dbHelper, string tableName, string categoryId, string id, string whereSubQuery = null)
        {
            var result = 0;
            var downId = GetDownId(dbHelper, tableName, categoryId, id, whereSubQuery);
            if (downId.Length == 0)
            {
                return result;
            }

            var parameters =
                new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>(BaseUtil.FieldId, id)
                };
            var sortCode = dbHelper.GetProperty(tableName, parameters, BaseUtil.FieldSortCode);
            parameters =
                new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>(BaseUtil.FieldId, downId)
                };
            var downSortCode = dbHelper.GetProperty(tableName, parameters, BaseUtil.FieldSortCode);

            //把downId的SortCode更新为当前SortCode
            var whereParameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseUtil.FieldId, downId)
            };
            parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseUtil.FieldSortCode, sortCode)
            };
            dbHelper.SetProperty(tableName, whereParameters, parameters);

            //把当前的SortCode更新为downId的downSortCode
            whereParameters =
                new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>(BaseUtil.FieldId, id)
                };
            parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseUtil.FieldSortCode, downSortCode)
            };
            result = dbHelper.SetProperty(tableName, whereParameters, parameters);
            return result;
        }
        #endregion

        #region 下移记录的方法
        /// <summary>
        /// 下移记录的方法
        /// </summary>
        /// <param name="dbHelper">当前数据库连接</param>
        /// <param name="tableName">表名</param>
        /// <param name="id">当前主键</param>
        /// <param name="whereSubQuery">Where条件</param>
        /// <returns>目标主键</returns>
        public static int SetDown(IDbHelper dbHelper, string tableName, string id, string whereSubQuery = null)
        {
            return SetDown(dbHelper, tableName, string.Empty, id, whereSubQuery);
        }
        #endregion
    }
}