using System.Data;
using DotNet.Util;
using Xunit;

namespace DotNet.Util.Tests.Util
{
    /// <summary>
    /// SqlUtil 测试
    /// LogSql 默认关闭，WriteLog 直接返回，不触发文件写入；此处仅验证调用不抛异常。
    /// </summary>
    public class SqlUtilTests
    {
        private sealed class StubParameter : IDbDataParameter
        {
            public StubParameter(string name, object value)
            {
                ParameterName = name;
                Value = value;
            }

            public byte Precision { get; set; }
            public byte Scale { get; set; }
            public int Size { get; set; }
            public DbType DbType { get; set; }
            public ParameterDirection Direction { get; set; }
            public bool IsNullable => true;
            public string? SourceColumn { get; set; }
            public DataRowVersion SourceVersion { get; set; }
            public string ParameterName { get; set; }
            public object? Value { get; set; }
        }

        [Fact]
        public void WriteLog_LogSqlDisabled_DoesNotThrow()
        {
            // 确保不写日志文件，避免依赖外部资源
            BaseSystemInfo.LogSql = false;

            var ex = Record.Exception(() => SqlUtil.WriteLog("SELECT 1", "Text"));
            Assert.Null(ex);
        }

        [Fact]
        public void WriteLog_WithParameters_LogSqlDisabled_DoesNotThrow()
        {
            BaseSystemInfo.LogSql = false;

            var parameter = new StubParameter("@id", 1);
            var ex = Record.Exception(() => SqlUtil.WriteLog("SELECT * FROM T", "Text", new IDbDataParameter[] { parameter }));
            Assert.Null(ex);
        }
    }
}
