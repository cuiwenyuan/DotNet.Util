//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2020, DotNet.
//-----------------------------------------------------------------

//using DotNet.Util.DbUtilities;
//using DotNet.Util.DbUtilities.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DotNet.Util
{
    /// <summary>
    /// DbHelperFactory
    /// 数据库服务工厂。
    /// 
    /// 修改记录
    /// 
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
    ///		<name>Troy Cui</name>
    ///		<date>2011.10.09</date>
    /// </author> 
    /// </summary>
    public class DbHelperFactory
    {
        ///// <summary>
        ///// 服务对像实例存储字典，第一个Key为数据库类型，第二个Key为服务对象的类型名称。
        ///// </summary>
        //static Dictionary<CurrentDbType, Dictionary<string, IDbHelperService>> _services = 
        //    new Dictionary<CurrentDbType, Dictionary<string, IDbHelperService>>();

        ///// <summary>
        ///// 服务注册是否已完成标记.
        ///// </summary>
        //static Dictionary<CurrentDbType, bool> _serviceRegisterCompleted = new Dictionary<CurrentDbType, bool>();

        ///// <summary>
        ///// 为数据库操作类注册一个关联服务。
        ///// </summary>
        ///// <typeparam name="T">服务类型，必须实现<see cref="IDbHelperService"/>接口。</typeparam>
        ///// <param name="dbType"><see cref="IDbHelper"/></param>
        ///// <param name="dbService">服务实例</param>
        //public static void RegisterService<T>(CurrentDbType dbType, T dbService) where T : IDbHelperService
        //{
        //    lock (_services)
        //    {
        //        if (!_services.ContainsKey(dbType))
        //            _services.Add(dbType, new Dictionary<string, IDbHelperService>());
        //        Dictionary<string, IDbHelperService> dicHelpers = _services[dbType];
        //        lock (dicHelpers)
        //        {
        //            string typeName = typeof(T).FullName;
        //            if (typeName != null && !dicHelpers.ContainsKey(typeName))
        //                dicHelpers.Add(typeName, dbService);
        //            else
        //                dicHelpers[typeName] = dbService;
        //        }
        //    }
        //}

        ///// <summary>
        ///// 获取所有可用的服务实例。
        ///// </summary>
        ///// <param name="dbType"><see cref="IDbHelper"/></param>
        ///// <returns><see cref="IDbHelperService"/> 的数组。
        ///// </returns>
        //public static IDbHelperService[] GetServices(CurrentDbType dbType)
        //{
        //    IDbHelperService[] services = new IDbHelperService[0];
        //    if (_services.ContainsKey(dbType))
        //        services = _services[dbType].Values.ToArray();
        //    return services;
        //}

        ///// <summary>
        ///// 获取指定数据库类型 <param name="dbType"/> 的指定类型的服务实例。
        ///// </summary>
        ///// <param name="dbType"><see cref="IDbHelper"/></param>
        ///// <returns><see cref="IDbHelperService"/> 服务实例。
        ///// </returns>
        //public static T GetService<T>(CurrentDbType dbType) where T : IDbHelperService
        //{
        //    T service = default(T);
        //    if (_services.ContainsKey(dbType))
        //    {
        //        Dictionary<string, IDbHelperService> dicHelpers = _services[dbType];
        //        string typeName = typeof(T).FullName;
        //        if (dicHelpers.ContainsKey(typeName))
        //            service = (T)dicHelpers[typeName];
        //    }
        //    if (service == null)
        //        throw new Exception(string.Format("数据库类型 {0} 不支持类型为 {0} 的服务。", typeof(T).ToString()));
        //    return service;
        //}

        /// <summary>
        /// 获取指定的数据库连接
        /// </summary>
        /// <param name="connectionString">数据库连接串</param>
        /// <returns>数据库访问类</returns>
        public static IDbHelper GetHelper(string connectionString)
        {
            var dbType = CurrentDbType.SqlServer;
            return GetHelper(dbType, connectionString);
        }

        /// <summary>
        /// 获取指定的数据库连接
        /// </summary>
        /// <param name="dbType">数据库类型</param>
        /// <param name="connectionString">数据库连接串</param>
        /// <returns>数据库访问类</returns>
        public static IDbHelper GetHelper(CurrentDbType dbType = CurrentDbType.SqlServer, string connectionString = null)
        {
            // 这里是每次都获取新的数据库连接,否则会有并发访问的问题存在
            var dbHelperClass = DbHelper.GetDbHelperClass(dbType);
            var dbHelper = (IDbHelper)Assembly.Load("DotNet.Util").CreateInstance(dbHelperClass, true);
            if (dbHelper != null)
            {
                if (!string.IsNullOrEmpty(connectionString))
                {
                    dbHelper.ConnectionString = connectionString;
                }
                else
                {
                    dbHelper.ConnectionString = BaseSystemInfo.BusinessDbConnection;
                }
            }
            //lock (_serviceRegisterCompleted)
            //{
            //    bool completed = false;
            //    if (_serviceRegisterCompleted.ContainsKey(dbType))
            //        completed = _serviceRegisterCompleted[dbType];
            //    else
            //        _serviceRegisterCompleted.Add(dbType, completed);
            //    if (!completed)
            //    {
            //        switch (dbType)
            //        {
            //            case CurrentDbType.MySql:
            //                DbHelperFactory.RegisterService<IBatcherHelperService>(dbHelper.CurrentDbType, new MySqlBatcherService());
            //                DbHelperFactory.RegisterService<ISyntaxHelperService>(dbHelper.CurrentDbType, new MySqlSyntaxService());
            //                break;
            //            case CurrentDbType.Oracle:
            //                DbHelperFactory.RegisterService<IBatcherHelperService>(dbHelper.CurrentDbType, new OracleBatcherService());
            //                DbHelperFactory.RegisterService<ISyntaxHelperService>(dbHelper.CurrentDbType, new OracleSyntaxService());
            //                break;
            //            case CurrentDbType.SqlServer:
            //                DbHelperFactory.RegisterService<IBatcherHelperService>(dbHelper.CurrentDbType, new MsSqlBatcherService());
            //                DbHelperFactory.RegisterService<ISyntaxHelperService>(dbHelper.CurrentDbType, new MsSqlSyntaxService());
            //                break;
            //            case CurrentDbType.SqLite:
            //                DbHelperFactory.RegisterService<IBatcherHelperService>(dbHelper.CurrentDbType, new SQLiteBatcherService());
            //                DbHelperFactory.RegisterService<ISyntaxHelperService>(dbHelper.CurrentDbType, new SQLiteSyntaxService());
            //                break;
            //            default:
            //                throw new Exception(string.Format("不支持的数据库类型 {0}。", dbType));
            //        }
            //    }
            //}
            return dbHelper;
        }
    }
}