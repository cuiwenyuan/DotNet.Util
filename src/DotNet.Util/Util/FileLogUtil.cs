using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
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

        static FileLogUtil()
        {
            WriteTask = new Task((object obj) =>
            {
                while (true)
                {
                    //等待信号通知
                    Pause.WaitOne();
                    var val = default(Tuple<string, string>);
                    while (LogQueue.TryDequeue(out val))
                    {
                        var sb = PoolUtil.StringBuilder.Get();
                        sb.Append(DateTime.Now.ToString(BaseSystemInfo.DateTimeLongFormat));
                        sb.Append(" ");
                        sb.Append(Thread.CurrentThread.ManagedThreadId);
                        sb.Append(val.Item2);
                        Trace.WriteLine(sb);
                        //大并发时，写入文本日志速度太慢会积压，内存占用高
                        WriteText(val.Item1, sb.Return());

                        //var logPath = val.Item1;
                        ////从Framework 4.5开始，启动一个由后台线程实现的Task，也可以使用静态方法 Task.Run
                        //Task.Run(() =>
                        //{
                        //    WriteText(logPath, sb.Return());
                        //});
                    }
                    //重新设置信号
                    Pause.Reset();
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
        /// <param name="logContent">数据</param>
        /// <param name="extension">文件扩展名</param>
        public static void WriteLog(string customDirectory, string fileName, string logContent, string extension)
        {
            var logPath = GetLogPath(customDirectory, fileName, extension);
            var sb = PoolUtil.StringBuilder.Get();
            sb.Append(logContent);
            sb.Append(" [Queue Time is " + DateTime.Now.ToString(BaseSystemInfo.DateTimeLongFormat) + "]");
            //如果不给Log目录写入权限，日志队列积压将会导致内存暴增
            // ConcurrentQueue.Count性能特别差，所以暂时去掉了这里的判断
            //if (LogQueue.Count > 1024)
            //{
            //    sb.Append(" [LogQueue Count is " + LogQueue.Count + "]");
            //}            
            LogQueue.Enqueue(new Tuple<string, string>(logPath, sb.Return()));
            //通知线程往磁盘中写日志
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
                    var tempNo = no.ToInt();
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
                //FileMode.Append：追加内容，只能与枚举FileAccess.Write联合使用，文件不存在会自动创建一个新文件
                using (var fs = new FileStream(logPath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
                {
                    using (var sw = new StreamWriter(fs, Encoding.UTF8))
                    {
                        sw.AutoFlush = true;
                        sw.WriteLine(logContent);
                    }
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