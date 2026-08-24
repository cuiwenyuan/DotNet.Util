using System;
using System.IO;
using System.Threading;
using DotNet.Util;
using Xunit;

namespace DotNet.Util.Tests.Util
{
    /// <summary>
    /// FileLogUtil 测试
    ///
    /// 说明：FileLogUtil 采用"内存队列 + 后台长任务线程"异步落盘（见源码：
    /// 静态构造里启动 WriteTask，WriteLog 只 Enqueue 并 Set 信号，磁盘写入在
    /// 后台线程执行），因此测试必须等待后台线程 flush 后才能真正断言文件内容。
    /// 这里用临时目录（Path.GetTempPath() 下子目录）验证"写日志 → 落盘 → 内容可读"，
    /// 结束后清理临时文件。
    /// </summary>
    public class FileLogUtilTests
    {
        [Fact]
        public void WriteLog_CustomDirectory_FileCreatedWithContent()
        {
            var tempDir = Path.Combine(Path.GetTempPath(), "FileLogUtilTests_" + Guid.NewGuid().ToString("N"));
            try
            {
                var fileName = "test_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_0.log";
                var content = "FileLogUtil smoke test content " + Guid.NewGuid().ToString("N");

                FileLogUtil.WriteLog(tempDir, fileName, content, "log");

                // 后台线程异步落盘：轮询等待（最多 5 秒）
                var filePath = string.Empty;
                var deadline = DateTime.Now.AddSeconds(5);
                while (DateTime.Now < deadline)
                {
                    var files = Directory.GetFiles(tempDir, "test_*", SearchOption.TopDirectoryOnly);
                    if (files.Length > 0)
                    {
                        filePath = files[0];
                        break;
                    }
                    Thread.Sleep(50);
                }

                Assert.False(string.IsNullOrEmpty(filePath), "后台写日志线程未在 5 秒内落盘");
                var text = File.ReadAllText(filePath);
                Assert.Contains(content, text);
                // 追加了 Queue Time 标记
                Assert.Contains("[Queue Time is ", text);
            }
            finally
            {
                if (Directory.Exists(tempDir))
                {
                    Directory.Delete(tempDir, true);
                }
            }
        }

        [Fact]
        public void WriteLog_ExistingFile_AppendsContent()
        {
            var tempDir = Path.Combine(Path.GetTempPath(), "FileLogUtilTests_" + Guid.NewGuid().ToString("N"));
            try
            {
                var fileName = "append_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_0.log";
                FileLogUtil.WriteLog(tempDir, fileName, "first line", "log");
                FileLogUtil.WriteLog(tempDir, fileName, "second line", "log");

                var deadline = DateTime.Now.AddSeconds(5);
                string text = string.Empty;
                while (DateTime.Now < deadline)
                {
                    var files = Directory.GetFiles(tempDir, "append_*", SearchOption.TopDirectoryOnly);
                    if (files.Length > 0)
                    {
                        text = File.ReadAllText(files[0]);
                        if (text.Contains("first line") && text.Contains("second line"))
                        {
                            break;
                        }
                    }
                    Thread.Sleep(50);
                }

                Assert.Contains("first line", text);
                Assert.Contains("second line", text);
            }
            finally
            {
                if (Directory.Exists(tempDir))
                {
                    Directory.Delete(tempDir, true);
                }
            }
        }
    }
}
