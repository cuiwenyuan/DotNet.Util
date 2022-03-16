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
        static volatile Int32 _logCount;

        //Mutex mmm = new Mutex();
        static FileLogUtil()
        {
            WriteTask = new Task((object obj) =>
                {
                    while (true)
                    {
                        Pause.WaitOne();
                        Pause.Reset();
                        //for (var i = 0; i <= LogQueue.Count; i++)
                        foreach (var logItem in LogQueue)
                        {
                            var val = default(Tuple<string, string>);
                            if (LogQueue.TryDequeue(out val))
                            {
                                //原子操作减1                            
                                Interlocked.Decrement(ref _logCount);
                                WriteText(val.Item1, val.Item2);
                            }
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
        /// <param name="customDirectory"></param>
        /// <param name="preFile"></param>
        /// <param name="infoData"></param>
        public static void WriteLog(string customDirectory, string preFile, string infoData)
        {
            //如果不给Log目录写入权限，日志队列积压将会导致内存暴增
            //if (_logCount > 1024)
            //{
            //    infoData += " : Current File Log Queue is " + _logCount + ", LogQueue Count is " + LogQueue.Count;
            //}
            var logPath = GetLogPath(customDirectory, preFile);
            LogQueue.Enqueue(new Tuple<string, string>(logPath, infoData));
            //原子操作加1
            Interlocked.Increment(ref _logCount);
            Pause.Set();
        }

        #region private GetLogPath & WriteText

        private static string GetLogPath(string customDirectory, string preFile)
        {
            string newFilePath;
            var logDir = string.IsNullOrEmpty(customDirectory) ? Path.Combine(Environment.CurrentDirectory, "Log") : customDirectory;
            if (!Directory.Exists(logDir))
            {
                Directory.CreateDirectory(logDir);
            }
            var filePaths = Directory.GetFiles(logDir, preFile, SearchOption.TopDirectoryOnly).ToList();

            if (filePaths.Count > 0)
            {
                var fileMaxLen = filePaths.Max(d => d.Length);
                var lastFilePath = filePaths.Where(d => d.Length == fileMaxLen).OrderByDescending(d => d).FirstOrDefault();
                if (lastFilePath != null && new FileInfo(lastFilePath).Length > 1 * 1024 * 1024)
                {
                    var no = new Regex(@"(?is)(?<=\()(.*)(?=\))").Match(Path.GetFileName(lastFilePath)).Value;
                    var tempNo = 0;
                    var parse = int.TryParse(no, out tempNo);
                    var formatNo = string.Format("-{0}", parse ? (tempNo + 1) : tempNo);
                    var newFileName = preFile.Replace(".txt", string.Format("_{0}.log", formatNo));
                    //以.log后缀的日志 2017.12.19 Troy Cui
                    newFileName = newFileName.Replace(".log", string.Format("_{0}.log", formatNo));
                    newFilePath = Path.Combine(logDir, newFileName);
                }
                else
                {
                    newFilePath = lastFilePath;
                }
            }
            else
            {
                var newFileName = preFile.Replace(".txt", string.Format("_{0}.txt", 0));
                //以.log后缀的日志 2017.12.19 Troy Cui
                newFileName = newFileName.Replace(".log", string.Format("_{0}.log", 0));
                newFilePath = Path.Combine(logDir, newFileName);
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
