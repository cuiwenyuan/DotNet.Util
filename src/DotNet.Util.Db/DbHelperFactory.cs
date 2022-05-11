//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

//using DotNet.Util.DbUtil;
//using DotNet.Util.DbUtil.Syntax;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace DotNet.Util
{
    /// <summary>
    /// DbHelperFactory
    /// 数据库服务工厂。
    /// 
    /// 修改记录
    ///
    ///     2022.05.09 版本：4.0 Troy.Cui 增加Create方法，创建实例的简写方法
    ///     2022.04.22 版本：3.0 Troy.Cui 精简代码，删除不必要的代码
    ///		2016.10.12 版本：2.2 LiuHaiyang 增加关联服务注册和获取的方法。
    ///		2011.10.09 版本：2.1 JiRiGaLa 改进直接用数据库连接就可以打开连接的方法。
    ///		2011.10.08 版本：2.0 JiRiGaLa 支持多类型的多种数据库。
    ///		2011.09.18 版本：1.4 JiRiGaLa 整理代码注释。
    ///		2011.03.30 版本：1.3 JiRiGaLa 增加数据库连串的构造函数。
    ///		2009.07.23 版本：1.2 JiRiGaLa 每次都获取一个新的数据库连接，解决并发错误问题。
    ///		2008.09.23 版本：1.1 JiRiGaLa 优化改进为单实例模式。
    ///		2008.08.26 版本：1.0 JiRiGaLa 创建数据库服务工厂。
    /// 
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2022.05.09</date>
    /// </author> 
    /// </summary>
    public class DbHelperFactory
    {
        #region 创建支持并发缓存的单一实例 Create

        private static readonly ConcurrentDictionary<string, IDbHelper> DbHelpers = new(StringComparer.OrdinalIgnoreCase);
        /// <summary>
        /// 获取指定的数据库连接
        /// </summary>
        /// <param name="dbType">数据库类型</param>
        /// <param name="connectionString">数据库连接串</param>
        /// <returns>数据库访问类</returns>
        public static IDbHelper Create(CurrentDbType dbType = CurrentDbType.SqlServer, string connectionString = null)
        {
            if (string.IsNullOrEmpty(connectionString)) throw new ArgumentNullException(nameof(connectionString));

            var dbHelper = GetHelper(dbType, connectionString);

            return dbHelper;
        }

        #endregion

        #region 获取实例 GetHelper

        /// <summary>
        /// 获取指定的数据库连接
        /// </summary>
        /// <param name="dbType">数据库类型</param>
        /// <param name="connectionString">数据库连接串</param>
        /// <returns>数据库访问类</returns>
        public static IDbHelper GetHelper(CurrentDbType dbType = CurrentDbType.SqlServer, string connectionString = null)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = BaseSystemInfo.UserCenterDbConnection;
            }
            var dbHelperClass = DbHelper.GetDbHelperClass(dbType);
            var dbHelper = (IDbHelper)Assembly.Load("DotNet.Util.Db").CreateInstance(dbHelperClass, true);
            // 千万不要用以下代码，不然会经常数据库访问异常！
            // Dictionary.TryGetValue 在多线程高并发下有可能抛出空异常
            //var dbHelper = DbHelpers.GetOrAdd(key: Convert.ToBase64String(Encoding.UTF8.GetBytes(connectionString)), valueFactory: _ => (IDbHelper)Assembly.Load("DotNet.Util.Db").CreateInstance(dbHelperClass, true));

            if (dbHelper != null)
            {

                dbHelper.ConnectionString = connectionString;
            }

            return dbHelper;
        }

        #endregion

    }
}