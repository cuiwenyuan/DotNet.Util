#if NETSTANDARD2_0_OR_GREATER
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System.IO;

namespace DotNet.Util
{
    /// <summary>
    /// AppSetting
    /// </summary>
    public static partial class AppSetting
    {
        /// <summary>
        /// Configuration
        /// </summary>
        public static IConfiguration Configuration { get; private set; }

        /// <summary>
        /// DbConnectionString
        /// </summary>
        public static string DbConnectionString
        {
            get { return _connection.DbConnectionString; }
        }

        /// <summary>
        /// RedisConnectionString
        /// </summary>
        public static string RedisConnectionString
        {
            get { return _connection.RedisConnectionString; }
        }

        /// <summary>
        /// UseRedis
        /// </summary>
        public static bool UseRedis
        {
            get { return _connection.UseRedis; }
        }

        private static Connection _connection;

        /// <summary>
        /// TokenHeaderName
        /// </summary>
        public static string TokenHeaderName = "Authorization";


        /// <summary>
        /// JWT有效期(分钟=默认120)
        /// </summary>
        public static int ExpMinutes { get; private set; } = 120;

        /// <summary>
        /// CurrentPath
        /// </summary>
        public static string CurrentPath { get; private set; } = null;
        /// <summary>
        /// DownLoadPath
        /// </summary>
        public static string DownLoadPath { get { return CurrentPath + "\\Download\\"; } }
        /// <summary>
        /// Init
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
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

        /// <summary>
        /// GetSettingString 多个节点name格式 ：["key:key1"]
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        public static string GetSettingString(string key)
        {
            return Configuration[key];
        }

        /// <summary>
        /// GetSection多个节点,通过.GetSection("key")["key1"]获取
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        public static IConfigurationSection GetSection(string key)
        {
            return Configuration.GetSection(key);
        }
    }

    /// <summary>
    /// Connection
    /// </summary>
    public class Connection
    {
        /// <summary>
        /// DbConnectionString
        /// </summary>
        public string DbConnectionString { get; set; }
        /// <summary>
        /// RedisConnectionString
        /// </summary>
        public string RedisConnectionString { get; set; }
        /// <summary>
        /// UseRedis
        /// </summary>
        public bool UseRedis { get; set; }
    }
}
#endif