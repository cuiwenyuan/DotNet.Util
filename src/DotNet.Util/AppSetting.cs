#if NETSTANDARD2_0_OR_GREATER
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System.IO;

namespace DotNet.Util
{
    public static partial class AppSetting
    {
        public static IConfiguration Configuration { get; private set; }

        public static string DbConnectionString
        {
            get { return _connection.DbConnectionString; }
        }

        public static string RedisConnectionString
        {
            get { return _connection.RedisConnectionString; }
        }

        public static bool UseRedis
        {
            get { return _connection.UseRedis; }
        }

        private static Connection _connection;

        public static string TokenHeaderName = "Authorization";


        /// <summary>
        /// JWT有效期(分钟=默认120)
        /// </summary>
        public static int ExpMinutes { get; private set; } = 120;

        public static string CurrentPath { get; private set; } = null;
        public static string DownLoadPath { get { return CurrentPath + "\\Download\\"; } }
        public static void Init(IServiceCollection services, IConfiguration configuration)
        {
            Configuration = configuration;

            //services.Configure<Connection>(configuration.GetSection("Connection"));
            //services.Configure<GlobalFilter>(configuration.GetSection("GlobalFilter"));
            
            var provider = services.BuildServiceProvider();
            //IWebHostEnvironment environment = provider.GetRequiredService<IWebHostEnvironment>();
            var environment = provider.GetService<IHostEnvironment>();
            CurrentPath = Path.Combine(environment.ContentRootPath, "").ReplacePath();

            _connection = provider.GetRequiredService<IOptions<Connection>>().Value;

        }
        // 多个节点name格式 ：["key:key1"]
        public static string GetSettingString(string key)
        {
            return Configuration[key];
        }
        // 多个节点,通过.GetSection("key")["key1"]获取
        public static IConfigurationSection GetSection(string key)
        {
            return Configuration.GetSection(key);
        }
    }

    public class Connection
    {
        public string DbConnectionString { get; set; }
        public string RedisConnectionString { get; set; }
        public bool UseRedis { get; set; }
    }
}
#endif