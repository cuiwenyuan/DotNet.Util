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

        //Mutex mmm = new Mutex();
        static FileLogUtil()
        {
            var writeTask = new Task((object obj) =>
                {
                    while (true)
                    {
                        Pause.WaitOne();
                        Pause.Reset();
                        var temp = new List<string[]>();
                        var val = default(Tuple<string, string>);
                        foreach (var logItem in LogQueue)
                        {
                            var logPath = logItem.Item1;
                            //var logMergeContent = string.Concat(logItem.Item2, Environment.NewLine, "-----------------------------------------------------------", Environment.NewLine);
                            //var logMergeContent = string.Concat(logItem.Item2, Environment.NewLine, "-----------------------------------------------------------");
                            //var logMergeContent = string.Concat(logItem.Item2, Environment.NewLine);
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
                            LogQueue.TryDequeue(out val);
                        }
                        foreach (var item in temp)
                        {
                            WriteText(item[0], item[1]);
                        }

                    }
                }
                , null
                , TaskCreationOptions.LongRunning);
            writeTask.Start();
        }
        /// <summary>
        /// WriteLog
        /// </summary>
        /// <param name="preFile"></param>
        /// <param name="infoData"></param>
        public static void WriteLog(string preFile, string infoData)
        {
            WriteLog(string.Empty, preFile, infoData);
        }

        /// <summary>
        /// WriteLog
        /// </summary>
        /// <param name="customDirectory"></param>
        /// <param name="preFile"></param>
        /// <param name="infoData"></param>
        public static void WriteLog(string customDirectory, string preFile, string infoData)
        {
            var logPath = GetLogPath(customDirectory, preFile);
            //string logContent = string.Concat(DateTime.Now, " ", infoData);
            var logContent = infoData;
            LogQueue.Enqueue(new Tuple<string, string>(logPath, logContent));
            Pause.Set();
        }

        #region private
        private static string GetLogPath(string customDirectory, string preFile)
        {
            string newFilePath;
            var logDir = string.IsNullOrEmpty(customDirectory) ? Path.Combine(Environment.CurrentDirectory, "logs") : customDirectory;
            if (!Directory.Exists(logDir))
            {
                Directory.CreateDirectory(logDir);
            }
            //string extension = ".log";
            //string fileNameNotExt = string.Concat(preFile, DateTime.Now.ToString("yyyyMMdd"));
            //string fileName = string.Concat(fileNameNotExt, extension);
            //string fileNamePattern = string.Concat(fileNameNotExt, "(*)", extension);
            //List<string> filePaths = Directory.GetFiles(logDir, fileNamePattern, SearchOption.TopDirectoryOnly).ToList();
            var filePaths = Directory.GetFiles(logDir, preFile, SearchOption.TopDirectoryOnly).ToList();

            if (filePaths.Count > 0)
            {
                var fileMaxLen = filePaths.Max(d => d.Length);
                var lastFilePath = filePaths.Where(d => d.Length == fileMaxLen).OrderByDescending(d => d).FirstOrDefault();
                if (lastFilePath != null && new FileInfo(lastFilePath).Length > 1 * 1024 * 1024)
                {
                    var no = new Regex(@"(?is)(?<=\()(.*)(?=\))").Match(Path.GetFileName(lastFilePath)).Value;
                    var tempno = 0;
                    var parse = int.TryParse(no, out tempno);
                    var formatno = string.Format("-{0}", parse ? (tempno + 1) : tempno);
                    //string newFileName = string.Concat(fileNameNotExt, formatno, extension);
                    var newFileName = preFile.Replace(".txt", string.Format("_{0}.log", formatno));
                    //以.log后缀的日志 2017.12.19 Troy Cui
                    newFileName = newFileName.Replace(".log", string.Format("_{0}.log", formatno));
                    newFilePath = Path.Combine(logDir, newFileName);
                }
                else
                {
                    newFilePath = lastFilePath;
                }
            }
            else
            {
                //string newFileName = string.Concat(fileNameNotExt, string.Format("({0})", 0), extension);
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
                // 将异常信息写入本地文件中
                var writerFileName = logPath;
                // 将异常信息写入本地文件中
                if (!File.Exists(writerFileName))
                {
                    var directoryName = Path.GetDirectoryName(logPath);
                    if (!string.IsNullOrWhiteSpace(directoryName))
                    {
                        if (!Directory.Exists(directoryName))
                        {
                            Directory.CreateDirectory(directoryName);
                        }
                    }
                    var fileStream = new FileStream(writerFileName, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
                    fileStream.Close();
                }
                using (var streamWriter = new StreamWriter(writerFileName, true, Encoding.Default))
                {
                    streamWriter.WriteLine(DateTime.Now.ToString(BaseSystemInfo.DateTimeLongFormat) + " " + Thread.CurrentThread.ManagedThreadId + " " + logContent);
                    //streamWriter.WriteLine(logContent);
                    streamWriter.Close();
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
