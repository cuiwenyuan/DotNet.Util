using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace DotNet.Util
{
    /// <summary>
    /// FileLogUtil多线程，高并发，文本持久化日志
    /// </summary>
    public static class FileLogUtil
    {
        static readonly ConcurrentQueue<Tuple<string, string>> LogQueue = new ConcurrentQueue<Tuple<string, string>>();
        static Task WriteTask = default(Task);
        static readonly ManualResetEvent Pause = new ManualResetEvent(false);

        //Mutex mmm = new Mutex();
        static FileLogUtil()
        {
            WriteTask = new Task((object obj) =>
                {
                    while (true)
                    {
                        Pause.WaitOne();
                        Pause.Reset();
                        var val = default(Tuple<string, string>);
                        while (LogQueue.TryDequeue(out val))
                        {
                            WriteText(val.Item1, val.Item2);
                        }
                    }
                }
                , null
                , TaskCreationOptions.LongRunning);
            WriteTask.Start();
        }

        /// <summary>
        /// WriteLog
        /// </summary>
        /// <param name="customDirectory">自定义目录</param>
        /// <param name="fileName">文件名</param>
        /// <param name="infoData">数据</param>
        /// <param name="extension">文件扩展名</param>
        public static void WriteLog(string customDirectory, string fileName, string infoData, string extension)
        {
            //如果不给Log目录写入权限，日志队列积压将会导致内存暴增
            if (LogQueue.Count > 1024)
            {
                infoData += " : LogQueue Count is " + LogQueue.Count;
            }
            var logPath = GetLogPath(customDirectory, fileName, extension);
            LogQueue.Enqueue(new Tuple<string, string>(logPath, infoData));
            Pause.Set();
        }

        #region private GetLogPath & WriteText

        /// <summary>
        /// 
        /// </summary>
        /// <param name="customDirectory">自定义目录</param>
        /// <param name="fileName">文件名</param>
        /// <param name="extension">文件后缀</param>
        /// <returns></returns>
        private static string GetLogPath(string customDirectory, string fileName, string extension)
        {
            string newFilePath;
            var logDir = string.IsNullOrEmpty(customDirectory) ? Path.Combine(Environment.CurrentDirectory, "Log") : customDirectory;
            if (!Directory.Exists(logDir))
            {
                Directory.CreateDirectory(logDir);
            }
            var fileNameKey = fileName.Substring(0, fileName.LastIndexOf("_") + 1);
            var fileNamePattern = "*." + extension;
            //var filePaths = Directory.GetFiles(logDir, fileNamePattern, SearchOption.TopDirectoryOnly).Where(s => s.Contains(fileNameKey)).ToList();
            var filePaths = Directory.GetFiles(logDir, fileNameKey + fileNamePattern, SearchOption.TopDirectoryOnly).ToList();

            if (filePaths.Count > 0)
            {
                var fileMaxLen = filePaths.Max(d => d.Length);
                var lastFilePath = filePaths.Where(d => d.Length == fileMaxLen).OrderByDescending(d => d).FirstOrDefault();

                if (lastFilePath != null && new FileInfo(lastFilePath).Length > BaseSystemInfo.LogFileMaxSize)
                {
                    var lastFileName = Path.GetFileName(lastFilePath);
                    var no = lastFileName.Substring(lastFileName.LastIndexOf("_") + 1, lastFileName.Length - lastFileName.LastIndexOf("_") - 1 - extension.Length - 1);
                    var tempNo = 0;
                    var parse = int.TryParse(no, out tempNo);
                    var newFileName = lastFileName.Substring(0, lastFileName.LastIndexOf("_") + 1) + (tempNo + 1) + "." + extension;
                    newFilePath = Path.Combine(logDir, newFileName);
                }
                else
                {
                    newFilePath = lastFilePath;
                }
            }
            else
            {
                newFilePath = Path.Combine(logDir, fileName);
            }
            return newFilePath;
        }
        /// <summary>
        /// WriteText
        /// </summary>
        /// <param name="logPath">日志路径</param>
        /// <param name="logContent">日志内容</param>
        private static void WriteText(string logPath, string logContent)
        {
            try
            {
                //不存在就创建
                if (!File.Exists(logPath))
                {
                    var directoryName = Path.GetDirectoryName(logPath);
                    if (!string.IsNullOrWhiteSpace(directoryName))
                    {
                        if (!Directory.Exists(directoryName))
                        {
                            Directory.CreateDirectory(directoryName);
                        }
                    }
                    using (var fs = new FileStream(logPath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
                    {
                        //转字节和字节数组有性能损耗
                        //byte[] buffer = Encoding.Default.GetBytes(DateTime.Now.ToString(BaseSystemInfo.DateTimeLongFormat) + " " + Thread.CurrentThread.ManagedThreadId + " " + logContent);
                        //fs.Write(buffer, 0, buffer.Length);
                        using (var sw = new StreamWriter(fs))
                        {
                            sw.WriteLine(DateTime.Now.ToString(BaseSystemInfo.DateTimeLongFormat) + " " + Thread.CurrentThread.ManagedThreadId + " " + logContent);
                            sw.Flush();
                            sw.Dispose();
                        }
                        fs.Dispose();
                    }
                }
                else
                {
                    using (var fs = new FileStream(logPath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
                    {
                        //转字节和字节数组有性能损耗
                        //byte[] buffer = Encoding.Default.GetBytes(DateTime.Now.ToString(BaseSystemInfo.DateTimeLongFormat) + " " + Thread.CurrentThread.ManagedThreadId + " " + logContent);
                        //fs.Write(buffer, 0, buffer.Length);
                        using (var sw = new StreamWriter(fs))
                        {
                            sw.WriteLine(DateTime.Now.ToString(BaseSystemInfo.DateTimeLongFormat) + " " + Thread.CurrentThread.ManagedThreadId + " " + logContent);
                            sw.Flush();
                            sw.Dispose();
                        }
                        fs.Dispose();
                    }

                    //using (var sw = new StreamWriter(logPath, true, Encoding.Default))
                    //{
                    //    sw.WriteLine(DateTime.Now.ToString(BaseSystemInfo.DateTimeLongFormat) + " " + Thread.CurrentThread.ManagedThreadId + " " + logContent);
                    //    sw.Flush();
                    //    sw.Dispose();
                    //}
                }
            }
            catch (Exception ex)
            {
                LogUtil.WriteException(ex);
                LogUtil.WriteLog("logPath:" + logPath + "logContent:" + logContent, "Exception");
            }
        }
        #endregion
    }
}
