//-----------------------------------------------------------------
// All Rights Reserved , Copyright (c) 2023 , DotNet. 
//-----------------------------------------------------------------


using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace DotNet.Util
{
    /// <summary>
    /// MongoDbHelper 数据库操作
    /// 有关数据库连接的方法。
    /// 
    /// 修改记录
    /// 
    ///		2016.09.01 版本：1.0 孙志广    创建。
    /// 
    /// 版本：1.0
    /// 
    /// <author>
    ///		<name>孙志广</name>
    ///		<date>2016.09.01</date>
    /// </author> 
    /// </summary>
    public class MongoDbHelper : DbHelper, IDbHelper
    {
        /// <summary>
        /// 数据库链接
        /// </summary>
        public static readonly string DbConnectionString = ConfigurationManager.AppSettings["TrackDbConnection"];

        #region 实例化 mongodb客户端
        private static MongoClient _clientInstance = null;
        public static MongoClient ClientInstance
        {
            get
            {
                if (_clientInstance == null)
                {
                    /// <summary>
                    /// OpenMas数据库连接字符串
                    /// </summary>
                    _clientInstance = new MongoClient(DbConnectionString);
                    // 集群
                    // _clientInstance = new MongoClient("mongodb://localhost:27017,mongodb://localhost:27018,mongodb://localhost:27019");
                    // 带用户名，带密码的
                    // mongodb://testuser:pass123@127.0.0.1:27017
                }

                return _clientInstance;
            }
        }
        #endregion

        /// <summary>
        /// 插入数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="databaseName"></param>
        /// <param name="collectionName"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static WriteConcernResult Insert<T>(string databaseName, string collectionName, T entity)
        {
            // ClientInstance.GetDatabase(databaseName).GetCollection<T>(collectionName).InsertOne(entity);
            WriteConcernResult result = ClientInstance.GetServer().GetDatabase(databaseName).GetCollection<T>(collectionName).Insert<T>(entity);
            return result;
        }

        /// <summary>
        /// 批量插入数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="databaseName"></param>
        /// <param name="collectionName"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public static BulkWriteResult BulkWrite<T>(string databaseName, string collectionName, List<T> list)
        {
            WriteModel<T>[] bulkModels = new WriteModel<T>[list.Count];
            Parallel.For(0, list.Count, (int i) => 
            {
                bulkModels[i] = new InsertOneModel<T>(list[i]);
            });

            BulkWriteResult result = ClientInstance.GetDatabase(databaseName).GetCollection<T>(collectionName).BulkWrite(bulkModels);

            return result;
        }

        /// <summary>
        /// 更新数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="databaseName"></param>
        /// <param name="collectionName"></param>
        /// <param name="query"></param>
        /// <param name="update"></param>
        /// <returns></returns>
        public static WriteConcernResult Update<T>(string databaseName, string collectionName, IMongoQuery query, IMongoUpdate update)
        {
            return ClientInstance.GetServer().GetDatabase(databaseName).GetCollection<T>(collectionName).Update(query, update);
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="databaseName"></param>
        /// <param name="collectionName"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public static BulkWriteResult Delete<T>(string databaseName, string collectionName, List<T> list)
        {
            var bulk = ClientInstance.GetServer().GetDatabase(databaseName).GetCollection<T>(collectionName).InitializeOrderedBulkOperation();
            foreach (var t in list)
            {
                bulk.Find(Query.EQ("_id", typeof(T).GetProperty("Id").GetValue(t).ToString())).RemoveOne();
            }
            BulkWriteResult result = bulk.Execute();
            return result;
        }

        /// <summary>
        /// 查询第一条数据
        /// </summary>
        /// <param name="databaseName"></param>
        /// <param name="collectionName"></param>
        /// <param name="listQuery"></param>
        /// <returns></returns>
        public static T FirstOrDefault<T>(string databaseName, string collectionName, List<IMongoQuery> listQuery)
        {
            if (listQuery == null || listQuery.Count < 1)
            {
                return default(T);
            }
            var query = Query.And(listQuery.ToArray());
            T entity = ClientInstance.GetServer()
                .GetDatabase(databaseName)//获取数据库
                .GetCollection<T>(collectionName)//获取表
                .Find(query)//查询集合
                .FirstOrDefault();
            return entity;
        }

        /// <summary>
        /// 根据Id值获取对应的实体。
        /// </summary>
        /// <typeparam name="T">实体类型。</typeparam>
        /// <param name="databaseName">数据库名称。</param>
        /// <param name="collectionName">集合（表）名称。</param>
        /// <param name="idValue">Id值。</param>
        /// <returns>Id值对应的实体。</returns>
        public static T FindEntityById<T>(string databaseName, string collectionName, string idValue)
        {
            T entity = ClientInstance.GetServer()
                    .GetDatabase(databaseName)//获取数据库
                    .GetCollection<T>(collectionName)//获取表
                    .FindOneById(idValue);//根据Id获取实体。
            return entity;
        }

        /// <summary>
        /// 判断条件查询的数据是否存在
        /// </summary>
        /// <param name="databaseName"></param>
        /// <param name="collectionName"></param>
        /// <param name="listQuery"></param>
        /// <returns></returns>
        public static bool Exists<T>(string databaseName, string collectionName, List<IMongoQuery> listQuery)
        {
            if (listQuery == null || listQuery.Count < 1)
            {
                return false;
            }
            var query = Query.And(listQuery.ToArray());

            return ClientInstance.GetServer()
                  .GetDatabase(databaseName)//获取数据库
                  .GetCollection<T>(collectionName)//获取表
                  .Find(query).Count() > 0;
        }

        /// <summary>
        /// 保存 更新或者新增
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="databaseName"></param>
        /// <param name="collectionName"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static WriteConcernResult Save<T>(string databaseName, string collectionName, T entity)
        {
            WriteConcernResult result = ClientInstance.GetServer()
                             .GetDatabase(databaseName)//获取数据库
                             .GetCollection<T>(collectionName)//获取表
                             .Save<T>(entity);
            return result;
        }

    }
}