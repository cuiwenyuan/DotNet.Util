using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
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
            // 读取客户端配置文件
            BaseSystemInfo.ConfigurationFrom = ConfigurationCategory.UserConfig;
            UserConfigUtil.GetConfig();

            BaseSystemInfo.LogSql = true;
            BaseSystemInfo.LogException = true;

            // 通过Word模板替换生成Word文档，用于合同模板生成合同
            var baesPath = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory);
            var templatePath = Path.Combine(baesPath, @"Contract\", "Template.docx");
            var templateDocument = WordUtil.GetXWPFDocument(templatePath);
            if (templateDocument != null)
            {
                var basicReplacements = new List<WordUtil.ReplacementBasic>();
                basicReplacements.Add(new WordUtil.ReplacementBasic
                {
                    Type = WordUtil.PlaceholderTypeEnum.Text,
                    Placeholder = "{姓名}",
                    Text = "Troy"
                });
                basicReplacements.Add(new WordUtil.ReplacementBasic
                {
                    Type = WordUtil.PlaceholderTypeEnum.Text,
                    Placeholder = "{性别}",
                    Text = "男"
                });
                WordUtil.ReplaceInWord(templateDocument, basicReplacements, null);
                var filePath = Path.Combine(baesPath, @"Contract\" + DateTime.Now.ToString("yyyyMM") + @"\", DateTime.Now.ToString(BaseSystemInfo.DateFormat) + ".docx");
                WordUtil.SaveXWPFDocument(filePath, templateDocument);
            }
            

            //批量删除数据库表记录
            //BatchDelete();

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
            //var connectionString = "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=wangcaisoft.com)(PORT=1521)))(CONNECT_DATA=(SERVICE_NAME = WangCaiMa)));User Id=WangCaiMa;Password=123456;Pooling=true;MAX Pool Size=1024;Min Pool Size=2;Connection Lifetime=20;Connect Timeout=30;";
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
            var batchSize = new XmlConfigUtil().GetValue("BatchSize", defaultValue: "1000", "Global").ToInt();
            var dataStoredDays = new XmlConfigUtil().GetValue("DataStoredDays", defaultValue: "90", "Global").ToInt();
            //SELECT COUNT(*) FROM[Business_WeLinkPark].[dbo].[WeLinkPark_ConsumeDetail] WHERE DATEDIFF(d, CreateOn, GETDATE()) > 90;
            //SELECT COUNT(*) FROM[Business_WeLinkPark].[dbo].[WeLinkPark_ParkFee] WHERE DATEDIFF(d, CreateOn, GETDATE()) > 90;
            //SELECT COUNT(*) FROM[Business_WeLinkPark].[dbo].[WeLinkPark_ParkingFee] WHERE DATEDIFF(d, CreateOn, GETDATE()) > 90;
            //SELECT COUNT(*) FROM[Business_WeLinkPark].[dbo].[WeLinkPark_PayToken] WHERE DATEDIFF(d, CreateOn, GETDATE()) > 90;
            //SELECT COUNT(*) FROM[Business_WeLinkPark].[dbo].[WeLinkPark_ParkRecord] WHERE DATEDIFF(d, CreateOn, GETDATE()) > 90;
            //SELECT COUNT(*) FROM[Business_WeLinkPark].[dbo].[WeLinkPark_ParkSpace] WHERE DATEDIFF(d, CreateOn, GETDATE()) > 90;
            var dbHelper = DbHelperFactory.Create(CurrentDbType.SqlServer, BaseSystemInfo.BusinessDbConnection);
            dbHelper.BatchDelete("WeLinkPark_ConsumeDetail", "CreateOn <= '" + DateTime.Now.AddDays(-dataStoredDays).ToString(BaseSystemInfo.DateTimeFormat) + "'", batchSize: batchSize);
            dbHelper.BatchDelete("WeLinkPark_ParkFee", "CreateOn <= '" + DateTime.Now.AddDays(-dataStoredDays).ToString(BaseSystemInfo.DateTimeFormat) + "'", batchSize: batchSize);
            dbHelper.BatchDelete("WeLinkPark_ParkingFee", "CreateOn <= '" + DateTime.Now.AddDays(-dataStoredDays).ToString(BaseSystemInfo.DateTimeFormat) + "'", batchSize: batchSize);
            dbHelper.BatchDelete("WeLinkPark_PayToken", "CreateOn <= '" + DateTime.Now.AddDays(-dataStoredDays).ToString(BaseSystemInfo.DateTimeFormat) + "'", batchSize: batchSize);
            dbHelper.BatchDelete("WeLinkPark_ParkRecord", "CreateOn <= '" + DateTime.Now.AddDays(-dataStoredDays).ToString(BaseSystemInfo.DateTimeFormat) + "'", batchSize: batchSize);
            dbHelper.BatchDelete("WeLinkPark_ParkSpace", "CreateOn <= '" + DateTime.Now.AddDays(-dataStoredDays).ToString(BaseSystemInfo.DateTimeFormat) + "'", batchSize: batchSize);
        }
    }
}
