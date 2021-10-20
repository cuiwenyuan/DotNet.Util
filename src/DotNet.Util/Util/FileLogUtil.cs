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
        static readonly ManualResetEvent Pause = new ManualResetEvent(false);
        static volatile Int32 _logCount;

        //Mutex mmm = new Mutex();
        static FileLogUtil()
        {
            var task = new Task((object obj) =>
                {
                    while (true)
                    {
                        var val = default(Tuple<string, string>);
                        Pause.WaitOne();
                        Pause.Reset();
                        foreach (var logItem in LogQueue)
                        {
                            var temp = new List<string[]>();
                            var logPath = logItem.Item1;
                            var logMergeContent = logItem.Item2;
                            var logArr = temp.FirstOrDefault(d => d[0].Equals(logPath));
                            if (logArr != null)
                            {
                                logArr[1] = string.Concat(logArr[1], logMergeContent);
                            }
                            else
                            {
                                logArr = new string[] { logPath, logMergeContent };
                                temp.Add(logArr);
                            }
                            //LogQueue.TryDequeue(out val);
                            while (LogQueue.TryDequeue(out val))
                            {
                                //原子操作减1
                                Interlocked.Decrement(ref _logCount);
                            }
                            foreach (var item in temp)
                            {
                                WriteText(item[0], item[1]);
                            }
                        }
                    }
                }
                , null
                , TaskCreationOptions.LongRunning);
            task.Start();
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
            if (_logCount > 1024) return;
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
            //修复中文字符乱码的问题Troy.Cui 2017-07-25
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
                    //using (var fs = new FileStream(writerFileName, FileMode.OpenOrCreate, FileAccess.Write))
                    using (var fs = new FileStream(logPath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
                    {
                        //byte[] buffer = Encoding.Default.GetBytes(string.Empty);
                        ////使用utf-8编码格式
                        //fs.Write(buffer, 0, buffer.Length);
                    }
                }
                using (var sw = new StreamWriter(logPath, true, Encoding.Default))
                {
                    sw.WriteLine(DateTime.Now.ToString(BaseSystemInfo.DateTimeLongFormat) + " " + Thread.CurrentThread.ManagedThreadId + " " + logContent);
                    sw.Flush();
                }
            }
            catch
            {
                // ignored
            }
        }
        #endregion
    }
}
