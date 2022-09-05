using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNet.Business;
using DotNet.Model;
using DotNet.Util;

namespace DotNet.Test._452
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //读取配置文件
            BaseConfiguration.GetSetting();

            BaseSystemInfo.LogSql = true;

            BatchDelete();

            //for (var i = 0; i < 100; i++)
            //{
            //    Task.Run(() =>
            //    {
            //        DbTest();
            //    });
            //}

            //CacheUtil.redisEnabled = true;
            //for (int i = 1000000; i < 1000100; i++)
            //{
            //    CacheUtil.Set("Test" + i, DateTime.Now.ToString(BaseSystemInfo.DateTimeFormat), cacheTime: new TimeSpan(0, 0, 0, i), isRedis: true);
            //}

            //for (int i = 1000000; i < 1000100; i++)
            //{
            //    Console.WriteLine(CacheUtil.Get<string>("Test" + i, isRedis: true));
            //}

            //for (int i = 10000; i < 11000; i++)
            //{
            //    var entity = new BaseUserContactEntity();
            //    entity.Id = 1;
            //    entity.Email = "Troy.Cui@email.com";
            //    CacheUtil.Set("entity" + i, entity, cacheTime: new TimeSpan(0, i, i, i), isRedis: true);
            //}

            //for (int i = 10000; i < 11000; i++)
            //{
            //    var entity = CacheUtil.Get<BaseUserContactEntity>("entity" + i, isRedis: true);
            //    Console.WriteLine(JsonUtil.ObjectToJson(entity));
            //}

            //for (int i = 0; i < 1000; i++)
            //{
            //    var ls = new List<BaseUserContactEntity>();
            //    for (int j = 0; j < RandUtil.Next(1, 10); j++)
            //    {
            //        ls.Add(new BaseUserContactEntity()
            //        {
            //            Id = j,
            //            Email = "Troy.Cui@" + j + ".com"
            //        });
            //    }

            //    CacheUtil.Set("listentity" + i, ls, cacheTime: new TimeSpan(0, i, i, i), isRedis: true);
            //}

            //for (int i = 0; i < 1000; i++)
            //{
            //    var ls = CacheUtil.Get<List<BaseUserContactEntity>>("listentity" + i, isRedis: true);
            //    Console.WriteLine(JsonUtil.ObjectToJson(ls));
            //}

            Console.WriteLine("Done");

            Console.ReadLine();
            //// Oracle
            //var connectionString = "";
            //var dbHelper = DbHelperFactory.Create(CurrentDbType.Oracle, connectionString);
            //var commandText = string.Empty;
            //dbHelper.ExecuteNonQuery(commandText);
        }
        private static void DbTest()
        {
            //var connectionString = "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=wangcaisoft.com)(PORT=1521)))(CONNECT_DATA=(SERVICE_NAME = oraprod)));User Id=wangcaisoft;Password=wangcaisoft;Pooling=true;MAX Pool Size=1024;Min Pool Size=2;Connection Lifetime=20;Connect Timeout=30;";
            //var dbHelper = DbHelperFactory.Create(CurrentDbType.Oracle, connectionString);
            //var manager = new EmailRecipientManager(dbHelper);
            //var entity = new EmailRecipientEntity();
            //entity.Recipient = "Troy.Cui@wangcaisoft.com";
            //entity.Name = "Troy.Cui";
            //entity.Category = "Shipping & Handling";
            //var entityId = manager.Add(entity);
            //Console.WriteLine(entityId);
        }

        private static void BatchDelete()
        {
            var dbHelper = DbHelperFactory.Create(CurrentDbType.SqlServer, "Data Source=localhost;Initial Catalog=DB_Test;User Id = sa ; Password = wangcaisoft.com;");
            dbHelper.BatchDelete("Common_MessageQueue", "CreateOn <= '" + DateTime.Now.AddDays(-365).ToString(BaseSystemInfo.DateTimeFormat) + "'", 100);
        }
    }
}
