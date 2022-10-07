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
using NPOI.XWPF.UserModel;
using NPOI.SS.Formula.Functions;
using NPOI.SS.Formula;
using Spire.Doc;
using Aspose;
using System.Text.RegularExpressions;

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
            LogUtil.WriteLog(BaseSystemInfo.BusinessDbConnection);

            var imagePath = Path.Combine(Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory), @"OCR\", "OCR02.jpeg");
            BaiduOcrUtil.GeneralBasic(imagePath);

            imagePath = Path.Combine(Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory), @"OCR\", "OCR01.jpeg");
            BaiduOcrUtil.AccurateBasic(imagePath);

            //for (int i = 0; i < 1000; i++)
            //{
            //    LogUtil.WriteLog(i.ToString());
            //}
            //for (int i = 0; i < 10000000; i++)
            //{
            //    LogUtil.WriteLog(i.ToString(), prefix: "D", logFileNamePattern: "yyyy-MM-dd");
            //}

            //for (int i = 0; i < 100000; i++)
            //{
            //    LogUtil.WriteLog(i.ToString(), prefix: "H", logFileNamePattern: "yyyy-MM-dd'_'HH'_'mm'_'ss");
            //}

            // 通过Word模板替换生成Word文档，用于合同模板生成合同
            //var baesPath = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory);
            //var templatePath = Path.Combine(baesPath, @"Contract\", "Template2.docx");
            //var templateDocument = WordUtil.GetXWPFDocument(templatePath);
            //if (templateDocument != null)
            //{
            //    var basicReplacements = new List<WordUtil.ReplacementBasic>();
            //    basicReplacements.Add(new WordUtil.ReplacementBasic
            //    {
            //        Type = WordUtil.PlaceholderTypeEnum.Text,
            //        Placeholder = "{姓名}",
            //        Text = "Troy"
            //    });
            //    basicReplacements.Add(new WordUtil.ReplacementBasic
            //    {
            //        Type = WordUtil.PlaceholderTypeEnum.Text,
            //        Placeholder = "{性别}",
            //        Text = "男"
            //    });
            //    WordUtil.ReplaceInWord(templateDocument, basicReplacements, null);
            //    var filePath = Path.Combine(baesPath, @"Contract\" + DateTime.Now.ToString("yyyyMMHH") + @"\", DateTime.Now.ToString(BaseSystemInfo.DateFormat) + ".docx");
            //    WordUtil.SaveXWPFDocument(filePath, templateDocument);

            //    // WordGlue 生成的格式不好看
            //    //using (var doc = new Doc(filePath))
            //    //{
            //    //    doc.SaveAs(filePath.Replace(".docx", "WordGlue.pdf"));
            //    //}

            //    // FreeSpire.Word 最多生成3页，500个段落
            //    //using (var doc = new Spire.Doc.Document())
            //    //{
            //    //    doc.LoadFromFile(filePath);
            //    //    doc.SaveToFile(filePath.Replace(".docx", "FreeSpire.Word.pdf"), Spire.Doc.FileFormat.PDF);
            //    //}
            //    //激活Aspose
            //    ActiveAspose();
            //    // Aspose
            //    var doc = new Aspose.Words.Document(filePath);
            //    doc.Save(filePath.Replace(".docx", "Aspose.Words.pdf"), Aspose.Words.SaveFormat.Pdf);
            //    doc.TryDispose();
            //}


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



        /// <summary>
        /// 激活Aspose
        /// </summary>
        private static void ActiveAspose()
        {
            // 最高支持到21.8.0
            // The subscription included in this license allows free upgrades until 27 Aug 2021, but this version of the product was released on 01 Sep 2021.Please renew the subscription or use a previous version of the product.
            //new Aspose.Words.License().SetLicense(new MemoryStream(Convert.FromBase64String("PExpY2Vuc2U+CiAgPERhdGE+CiAgICA8TGljZW5zZWRUbz5TdXpob3UgQXVuYm94IFNvZnR3YXJlIENvLiwgTHRkLjwvTGljZW5zZWRUbz4KICAgIDxFbWFpbFRvPnNhbGVzQGF1bnRlYy5jb208L0VtYWlsVG8+CiAgICA8TGljZW5zZVR5cGU+RGV2ZWxvcGVyIE9FTTwvTGljZW5zZVR5cGU+CiAgICA8TGljZW5zZU5vdGU+TGltaXRlZCB0byAxIGRldmVsb3BlciwgdW5saW1pdGVkIHBoeXNpY2FsIGxvY2F0aW9uczwvTGljZW5zZU5vdGU+CiAgICA8T3JkZXJJRD4yMDA2MDIwMTI2MzM8L09yZGVySUQ+CiAgICA8VXNlcklEPjEzNDk3NjAwNjwvVXNlcklEPgogICAgPE9FTT5UaGlzIGlzIGEgcmVkaXN0cmlidXRhYmxlIGxpY2Vuc2U8L09FTT4KICAgIDxQcm9kdWN0cz4KICAgICAgPFByb2R1Y3Q+QXNwb3NlLlRvdGFsIGZvciAuTkVUPC9Qcm9kdWN0PgogICAgPC9Qcm9kdWN0cz4KICAgIDxFZGl0aW9uVHlwZT5FbnRlcnByaXNlPC9FZGl0aW9uVHlwZT4KICAgIDxTZXJpYWxOdW1iZXI+OTM2ZTVmZDEtODY2Mi00YWJmLTk1YmQtYzhkYzBmNTNhZmE2PC9TZXJpYWxOdW1iZXI+CiAgICA8U3Vic2NyaXB0aW9uRXhwaXJ5PjIwMjEwODI3PC9TdWJzY3JpcHRpb25FeHBpcnk+CiAgICA8TGljZW5zZVZlcnNpb24+My4wPC9MaWNlbnNlVmVyc2lvbj4KICAgIDxMaWNlbnNlSW5zdHJ1Y3Rpb25zPmh0dHBzOi8vcHVyY2hhc2UuYXNwb3NlLmNvbS9wb2xpY2llcy91c2UtbGljZW5zZTwvTGljZW5zZUluc3RydWN0aW9ucz4KICA8L0RhdGE+CiAgPFNpZ25hdHVyZT5wSkpjQndRdnYxV1NxZ1kyOHFJYUFKSysvTFFVWWRrQ2x5THE2RUNLU0xDQ3dMNkEwMkJFTnh5L3JzQ1V3UExXbjV2bTl0TDRQRXE1aFAzY2s0WnhEejFiK1JIWTBuQkh1SEhBY01TL1BSeEJES0NGbWg1QVFZRTlrT0FxSzM5NVBSWmJRSGowOUNGTElVUzBMdnRmVkp5cUhjblJvU3dPQnVqT1oyeDc4WFE9PC9TaWduYXR1cmU+CjwvTGljZW5zZT4=")));

            // This license is disabled, please contact Aspose to obtain a new license.
            //new Aspose.Words.License().SetLicense(new MemoryStream(Convert.FromBase64String("DQo8TGljZW5zZT4NCjxEYXRhPg0KPExpY2Vuc2VkVG8+VGhlIFdvcmxkIEJhbms8L0xpY2Vuc2VkVG8+DQo8RW1haWxUbz5ra3VtYXIzQHdvcmxkYmFua2dyb3VwLm9yZzwvRW1haWxUbz4NCjxMaWNlbnNlVHlwZT5EZXZlbG9wZXIgU21hbGwgQnVzaW5lc3M8L0xpY2Vuc2VUeXBlPg0KPExpY2Vuc2VOb3RlPjEgRGV2ZWxvcGVyIEFuZCAxIERlcGxveW1lbnQgTG9jYXRpb248L0xpY2Vuc2VOb3RlPg0KPE9yZGVySUQ+MjEwMzE2MTg1OTU3PC9PcmRlcklEPg0KPFVzZXJJRD43NDQ5MTY8L1VzZXJJRD4NCjxPRU0+VGhpcyBpcyBub3QgYSByZWRpc3RyaWJ1dGFibGUgbGljZW5zZTwvT0VNPg0KPFByb2R1Y3RzPg0KPFByb2R1Y3Q+QXNwb3NlLlRvdGFsIGZvciAuTkVUPC9Qcm9kdWN0Pg0KPC9Qcm9kdWN0cz4NCjxFZGl0aW9uVHlwZT5Qcm9mZXNzaW9uYWw8L0VkaXRpb25UeXBlPg0KPFNlcmlhbE51bWJlcj4wM2ZiMTk5YS01YzhhLTQ4ZGItOTkyZS1kMDg0ZmYwNjZkMGM8L1NlcmlhbE51bWJlcj4NCjxTdWJzY3JpcHRpb25FeHBpcnk+MjAyMjA1MTY8L1N1YnNjcmlwdGlvbkV4cGlyeT4NCjxMaWNlbnNlVmVyc2lvbj4zLjA8L0xpY2Vuc2VWZXJzaW9uPg0KPExpY2Vuc2VJbnN0cnVjdGlvbnM+aHR0cHM6Ly9wdXJjaGFzZS5hc3Bvc2UuY29tL3BvbGljaWVzL3VzZS1saWNlbnNlPC9MaWNlbnNlSW5zdHJ1Y3Rpb25zPg0KPC9EYXRhPg0KPFNpZ25hdHVyZT5XbkJYNnJOdHpCclNMV3pBdFlqOEtkdDFLSUI5MlFrL2xEbFNmMlM1TFRIWGdkcS9QQ2NqWHVORmp0NEJuRmZwNFZLc3VsSjhWeFExakIwbmM0R1lWcWZLek14SFFkaXFuZU03NTJaMjlPbmdyVW40Yk0rc1l6WWVSTE9UOEpxbE9RN05rRFU0bUk2Z1VyQ3dxcjdnUVYxbDJJWkJxNXMzTEFHMFRjQ1ZncEE9PC9TaWduYXR1cmU+DQo8L0xpY2Vuc2U+DQo=")));

            //string LData = "DQo8TGljZW5zZT4NCjxEYXRhPg0KPExpY2Vuc2VkVG8+VGhlIFdvcmxkIEJhbms8L0xpY2Vuc2VkVG8+DQo8RW1haWxUbz5ra3VtYXIzQHdvcmxkYmFua2dyb3VwLm9yZzwvRW1haWxUbz4NCjxMaWNlbnNlVHlwZT5EZXZlbG9wZXIgU21hbGwgQnVzaW5lc3M8L0xpY2Vuc2VUeXBlPg0KPExpY2Vuc2VOb3RlPjEgRGV2ZWxvcGVyIEFuZCAxIERlcGxveW1lbnQgTG9jYXRpb248L0xpY2Vuc2VOb3RlPg0KPE9yZGVySUQ+MjEwMzE2MTg1OTU3PC9PcmRlcklEPg0KPFVzZXJJRD43NDQ5MTY8L1VzZXJJRD4NCjxPRU0+VGhpcyBpcyBub3QgYSByZWRpc3RyaWJ1dGFibGUgbGljZW5zZTwvT0VNPg0KPFByb2R1Y3RzPg0KPFByb2R1Y3Q+QXNwb3NlLlRvdGFsIGZvciAuTkVUPC9Qcm9kdWN0Pg0KPC9Qcm9kdWN0cz4NCjxFZGl0aW9uVHlwZT5Qcm9mZXNzaW9uYWw8L0VkaXRpb25UeXBlPg0KPFNlcmlhbE51bWJlcj4wM2ZiMTk5YS01YzhhLTQ4ZGItOTkyZS1kMDg0ZmYwNjZkMGM8L1NlcmlhbE51bWJlcj4NCjxTdWJzY3JpcHRpb25FeHBpcnk+MjAyMjA1MTY8L1N1YnNjcmlwdGlvbkV4cGlyeT4NCjxMaWNlbnNlVmVyc2lvbj4zLjA8L0xpY2Vuc2VWZXJzaW9uPg0KPExpY2Vuc2VJbnN0cnVjdGlvbnM+aHR0cHM6Ly9wdXJjaGFzZS5hc3Bvc2UuY29tL3BvbGljaWVzL3VzZS1saWNlbnNlPC9MaWNlbnNlSW5zdHJ1Y3Rpb25zPg0KPC9EYXRhPg0KPFNpZ25hdHVyZT5XbkJYNnJOdHpCclNMV3pBdFlqOEtkdDFLSUI5MlFrL2xEbFNmMlM1TFRIWGdkcS9QQ2NqWHVORmp0NEJuRmZwNFZLc3VsSjhWeFExakIwbmM0R1lWcWZLek14SFFkaXFuZU03NTJaMjlPbmdyVW40Yk0rc1l6WWVSTE9UOEpxbE9RN05rRFU0bUk2Z1VyQ3dxcjdnUVYxbDJJWkJxNXMzTEFHMFRjQ1ZncEE9PC9TaWduYXR1cmU+DQo8L0xpY2Vuc2U+DQo=";

            //MemoryStream stream3 = new MemoryStream(Convert.FromBase64String(LData));

            //Aspose.Pdf.License license3 = new Aspose.Pdf.License();
            //stream3.Seek(0, SeekOrigin.Begin);
            //license3.SetLicense(stream3);
        }
    }
}
