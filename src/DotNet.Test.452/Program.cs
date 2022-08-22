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

            CacheUtil.redisEnabled = true;
            //for (int i = 0; i < 10; i++)
            //{
            //    CacheUtil.Set("Test" + i, DateTime.Now.ToString(BaseSystemInfo.DateTimeFormat), cacheTime: new TimeSpan(0, 0, 0, i), isRedis: true);
            //}

            for (int i = 0; i < 10; i++)
            {
                var entity = new BaseUserContactEntity();
                entity.Id = 1;
                entity.Email = "Troy.Cui@email.com";
                CacheUtil.Set("entity" + i, entity, cacheTime: new TimeSpan(0, i, i, i), isRedis: true);
            }

            for (int i = 0; i < 10; i++)
            {
                var entity = CacheUtil.Get<BaseUserContactEntity>("entity" + i, isRedis: true);
                Console.WriteLine(JsonUtil.ObjectToJson(entity));
            }

            Console.ReadLine();
            //// Oracle
            //var connectionString = "";
            //var dbHelper = DbHelperFactory.Create(CurrentDbType.Oracle, connectionString);
            //var commandText = string.Empty;
            //dbHelper.ExecuteNonQuery(commandText);
        }
    }
}
