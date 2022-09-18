//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;
using System.Runtime.InteropServices;

namespace DotNet.Util
{
    /// <summary>
    ///	FileUtil
    /// 文件帮助类
    /// 
    /// 修改记录
    /// 
    ///		2015.03.22 版本：1.4    JiRiGaLa 异常数据记录更多信息。
    ///		2012.05.03 版本：1.3    Pcsky增加一个读取文本文件内容的方法(GetTextFileContent)
    ///		2011.07.31 版本：1.2    Sunplay增加一个删除文件的方法(DeleteFile)。
    ///		2011.07.31 版本：1.1    Sunplay增加一个获取文件大小的方法(GetFileSize)。
    ///		2010.07.10 版本：1.0	JiRiGaLa 创建。
    ///	
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2015.03.22</date>
    /// </author> 
    /// </summary>
    public partial class FileUtil
    {
        /// <summary>
        /// 播放音乐
        /// </summary>
        /// <param name="pszSound"></param>
        /// <param name="hmod"></param>
        /// <param name="fdwSound"></param>
        /// <returns></returns>
        [DllImport("winmm.dll")]
        public static extern bool PlaySound(string pszSound, int hmod, int fdwSound);
        /// <summary>
        /// SndFilename
        /// </summary>
        public const int SndFilename = 0x00020000;
        /// <summary>
        /// SndAsync
        /// </summary>
        public const int SndAsync = 0x0001;
        /// <summary>
        /// 播放声音
        /// </summary>
        /// <param name="file"></param>
        public static void PlaySound(string file)
        {
            if (File.Exists(file))
            {
                PlaySound(file, 0, SndAsync | SndFilename);
            }
        }
        /// <summary>
        /// 回调
        /// </summary>
        /// <returns></returns>
        public static bool ThumbnailCallback() { return false; }

        /// <summary>
        /// 获取压缩的图片
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="maxHeightWidth">宽度</param>
        /// <returns>自动压缩后的图片</returns>
        public static Bitmap GetThumbnailImageFromFile(string fileName, int maxHeightWidth = 0)
        {
            var image = Image.FromFile(fileName);
            var height = image.Height;
            var width = image.Width;
            if (maxHeightWidth != 0)
            {
                if (image.Width >= image.Height)
                {
                    width = maxHeightWidth;
                    height = (maxHeightWidth * image.Height) / image.Width;
                }
                else
                {
                    height = maxHeightWidth;
                    width = (maxHeightWidth * image.Width) / image.Height;
                }
            }
            return new Bitmap(image.GetThumbnailImage(width, height, ThumbnailCallback, IntPtr.Zero));
        }

        #region public static string GetFriendlyFileSize(double fileSize) 有善的文件大小现实方式
        /// <summary>
        /// 有善的文件大小现实方式
        /// </summary>
        /// <param name="fileSize">文件大小</param>
        /// <returns>现实方式</returns>
        public static string GetFriendlyFileSize(double fileSize)
        {
            var result = string.Empty;
            if (fileSize < 1024)
            {
                result = fileSize.ToString("F1") + "Byte";
            }
            else
            {
                fileSize = fileSize / 1024;
                if (fileSize < 1024)
                {
                    result = fileSize.ToString("F1") + "KB";
                }
                else
                {
                    fileSize = fileSize / 1024;
                    if (fileSize < 1024)
                    {
                        result = fileSize.ToString("F1") + "M";
                    }
                    else
                    {
                        fileSize = fileSize / 1024;
                        result = fileSize.ToString("F1") + "GB";
                    }
                }
            }
            return result;
        }
        #endregion

        /// <summary>
        /// 读取文件
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <returns>字节</returns>
        public static byte[] GetFile(string fileName)
        {
            var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            var br = new BinaryReader(fs);
            var file = br.ReadBytes(((int)fs.Length));
            br.Close();
            fs.Close();
            return file;
        }

        /// <summary>
        /// 保存文件
        /// </summary>
        /// <param name="file">文件</param>
        /// <param name="fileName">文件名</param>
        public static void SaveFile(byte[] file, string fileName)
        {
            var directoryName = Path.GetDirectoryName(fileName);
            if (!Directory.Exists(directoryName))
            {
                Directory.CreateDirectory(directoryName);
            }
            var fs = new FileStream(fileName, FileMode.Create);
            fs.Write(file, 0, file.Length);
            fs.Close();
        }
        /// <summary>
        /// 图片转字节
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public static byte[] ImageToByte(Image image)
        {
            var ms = new MemoryStream();
            image.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
            var file = ms.GetBuffer();
            ms.Close();
            return file;
        }
        /// <summary>
        /// 字节转图片
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static Image ByteToImage(byte[] buffer)
        {
            Image image;
            using (var ms = new MemoryStream(buffer))
            {
                image = Image.FromStream(ms);
                ms.Close();
            }
            return image;
        }

        /// <summary>
        /// 向创建二进制文件，并向其中写入二进制信息
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="message">文件文本内容</param>
        public static void WriteBinaryFile(string fileName, string message)
        {
            Console.WriteLine(@"写入二进制文件信息开始。");
            FileStream fs = null;
            BinaryWriter bw = null;
            try
            {
                // 首先判断，文件是否已经存在
                if (File.Exists(fileName))
                {
                    // 如果文件已经存在，那么删除掉.
                    File.Delete(fileName);
                }
                // 注意第2个参数：
                // FileMode.Create 指定操作系统应创建新文件。如果文件已存在，它将被覆盖。
                fs = new FileStream(fileName, FileMode.Create, FileAccess.Write);
                bw = new BinaryWriter(fs);

                // 写入测试数据.
                // bw.Write(0x20);
                // bw.Write(1024.567d);
                // bw.Write(1024);

                // 注意，二进制写入 字符串信息的时候
                // 带长度前缀的字符串通过在字符串前面放置一个包含该字符串长度的字节或单词，来表示该字符串的长度。
                // 此方法首先将字符串长度作为一个四字节无符号整数写入，然后将这些字符写入流中。

                // 这里将先写入一个 0x07， 然后再写入 abcdefg
                // bw.Write("abcdefg");
                var binaryBytes = Encoding.UTF8.GetBytes(message);
                // binaryWriter.Write(binaryBytes);
                bw.Write(binaryBytes);

                // 关闭文件.
                bw.Close();
                fs.Close();

                bw = null;
                fs = null;
            }
            catch
            {
                // Console.WriteLine("在写入文件的过程中，发生了异常！");
                // Console.WriteLine(ex.Message);
                // Console.WriteLine(ex.StackTrace);
                throw;
            }
            finally
            {
                if (bw != null)
                {
                    try
                    {
                        bw.Close();
                    }
                    catch
                    {
                        // 最后关闭文件，无视 关闭是否会发生错误了.
                    }
                }
                if (fs != null)
                {
                    try
                    {
                        fs.Close();
                    }
                    catch
                    {
                        // 最后关闭文件，无视 关闭是否会发生错误了.
                    }
                }
            }
            // Console.WriteLine("写入二进制文件信息结束。");
        }

        /// <summary>
        /// 测试向从二进制文件中读取数据，并显示到终端上.
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <returns>文件内容</returns>
        public static string ReadBinaryFile(string fileName)
        {
            var message = string.Empty;
            // Console.WriteLine("读取二进制文件信息开始。");
            FileStream fs = null;
            BinaryReader br = null;
            try
            {
                // 首先判断，文件是否已经存在
                if (!File.Exists(fileName))
                {
                    // 如果文件不存在，那么提示无法读取！
                    // Console.WriteLine("二进制文件{0}不存在！", fileName);
                    return string.Empty;
                }


                fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                br = new BinaryReader(fs);

                // int a = br.ReadInt32();
                // double b = br.ReadDouble();
                // int c = br.ReadInt32();
                // byte len = br.ReadByte();
                // char[] d = br.ReadChars(len);

                // Console.WriteLine("第一个数据:{0}", a);
                // Console.WriteLine("第二个数据:{0}", b);
                // Console.WriteLine("第三个数据:{0}", c);
                // Console.WriteLine("第四个数据 (长度为{0}):", len);
                // foreach (char ch in d)
                // {
                //    Console.Write(ch);
                // }
                // Console.WriteLine();
                //完整的读取文件类容需要获取文件的长度
                var count = (int)fs.Length;
                var buffer = new byte[count];
                br.Read(buffer, 0, buffer.Length);
                message = Encoding.Default.GetString(buffer);
                // message = br.ReadString();

                // 读取完毕，关闭.
                br.Close();
                fs.Close();

                br = null;
                fs = null;
            }
            catch
            {
                // Console.WriteLine("在读取文件的过程中，发生了异常！");
                // Console.WriteLine(ex.Message);
                // Console.WriteLine(ex.StackTrace);
                throw;
            }
            finally
            {
                if (br != null)
                {
                    try
                    {
                        br.Close();
                    }
                    catch
                    {
                        // 最后关闭文件，无视 关闭是否会发生错误了.
                    }
                }

                if (fs != null)
                {
                    try
                    {
                        fs.Close();
                    }
                    catch
                    {
                        // 最后关闭文件，无视 关闭是否会发生错误了.
                    }
                }
            }
            // Console.WriteLine("读取二进制文件信息结束。");
            return message;
        }

        /// <summary>
        /// 删除文件 
        /// </summary>
        /// <param name="fileName">文件全路径</param>
        /// <returns>bool 是否删除成功</returns>
        public static bool DeleteFile(string fileName)
        {
            if (File.Exists(fileName) == true)
            {
                if (File.GetAttributes(fileName) == FileAttributes.Normal)
                {
                    File.Delete(fileName);
                }
                else
                {
                    File.SetAttributes(fileName, FileAttributes.Normal);
                    File.Delete(fileName);
                }
                return true;
            }
            else
            {
                // 文件不存在
                return false;
            }
        }

        #region 获取文件大小
        /// <summary>
        /// 根据文件名，得到文件的大小，单位分别是GB/MB/KB
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <returns>返回文件大小</returns>
        public static string GetFileSize(string fileName)
        {
            if (File.Exists(fileName) == true)
            {
                var fi = new FileInfo(fileName);
                var fl = fi.Length;
                if (fl > 1024 * 1024 * 1024)
                {
                    return Convert.ToString(Math.Round((fl + 0.00) / (1024 * 1024 * 1024), 2)) + " GB";
                }
                else if (fl > 1024 * 1024)
                {
                    return Convert.ToString(Math.Round((fl + 0.00) / (1024 * 1024), 2)) + " MB";
                }
                else
                {
                    return Convert.ToString(Math.Round((fl + 0.00) / 1024, 2)) + " KB";
                }
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 根据文件名，得到文件的大小，单位分别是GB/MB/KB
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <returns>返回文件大小</returns>
        public static string GetReadableSize(string fileName)
        {
            if (File.Exists(fileName) == true)
            {
                var fi = new FileInfo(fileName);
                var fl = fi.Length;
                return BytesToReadableSize(fl);
            }
            else
            {
                return null;
            }
        }

        private static readonly string[] Suffixes = new string[] { " B", " KB", " MB", " GB", " TB", " PB" };
        /// <summary>
        /// 获取指定字节大小的显示字符串
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static string BytesToReadableSize(long number)
        {
            double last = 1;
            for (var i = 0; i < Suffixes.Length; i++)
            {
                var current = Math.Pow(1024, i + 1);
                var temp = number / current;
                if (temp < 1)
                {
                    return (number / last).ToString("n2") + Suffixes[i];
                }
                last = current;
            }
            return number.ToString();
        }

        #endregion

        /// <summary>
        /// 读取文件文件内容
        /// 文本格式必须为utf-8
        /// (ansi格式容易产生乱码)
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <returns></returns>
        public static string GetTextFileContent(string fileName)
        {
            var sr = new StreamReader(fileName, Encoding.GetEncoding("utf-8"));
            return sr.ReadToEnd();
        }

        /// <summary>
        /// 根据文件后缀来获取MIME类型字符串
        /// </summary>
        /// <param name="extension">文件后缀</param>
        /// <returns></returns>
        public static string GetMimeType(string extension)
        {
            var mime = string.Empty;
            extension = extension.ToLower();
            switch (extension)
            {
                case ".avi": mime = "video/x-msvideo"; break;
                case ".bin":
                case ".exe":
                case ".msi":
                case ".dll":
                case ".class": mime = "application/octet-stream"; break;
                case ".csv": mime = "text/comma-separated-values"; break;
                case ".html":
                case ".htm":
                case ".shtml": mime = "text/html"; break;
                case ".css": mime = "text/css"; break;
                case ".js": mime = "text/javascript"; break;
                case ".doc":
                case ".dot":
                case ".docx": mime = "application/msword"; break;
                case ".xla":
                case ".xls":
                case ".xlsx": mime = "application/msexcel"; break;
                case ".ppt":
                case ".pptx": mime = "application/mspowerpoint"; break;
                case ".gz": mime = "application/gzip"; break;
                case ".gif": mime = "image/gif"; break;
                case ".bmp": mime = "image/bmp"; break;
                case ".jpeg":
                case ".jpg":
                case ".jpe":
                case ".png": mime = "image/jpeg"; break;
                case ".mpeg":
                case ".mpg":
                case ".mpe":
                case ".wmv": mime = "video/mpeg"; break;
                case ".mp3":
                case ".wma": mime = "audio/mpeg"; break;
                case ".pdf": mime = "application/pdf"; break;
                case ".rar": mime = "application/octet-stream"; break;
                case ".txt": mime = "text/plain"; break;
                case ".7z":
                case ".z": mime = "application/x-compress"; break;
                case ".zip": mime = "application/x-zip-compressed"; break;
                default:
                    mime = "application/octet-stream";
                    break;
            }
            return mime;
        }

        #region 复制目录内所有文件到目标目录

        /// <summary>
        /// 复制目录内所有文件到目标目录
        /// </summary>
        /// <param name="sourceDir">源目录</param>
        /// <param name="targetDir">目标目录</param>
        /// <param name="deleteExistingFile">是否删除已存在文件</param>
        /// <param name="overWrite">覆盖目标文件</param>
        /// <param name="deleteSourceFile">是否删除源文件</param>
        public static void CopyDirectory(string sourceDir, string targetDir, bool deleteExistingFile = true, bool overWrite = false, bool deleteSourceFile = true)
        {
            var folderName = sourceDir.Substring(sourceDir.LastIndexOf("\\", StringComparison.OrdinalIgnoreCase) + 1);

            var desfolderdir = targetDir + "\\" + folderName;

            if (targetDir.LastIndexOf("\\", StringComparison.OrdinalIgnoreCase) == (targetDir.Length - 1))
            {
                desfolderdir = targetDir + folderName;
            }
            var fileNames = Directory.GetFileSystemEntries(sourceDir);

            foreach (var sourceFileName in fileNames)// 遍历所有的文件和目录
            {
                if (Directory.Exists(sourceFileName))// 先当作目录处理如果存在这个目录就递归Copy该目录下面的文件
                {

                    var currentdir = desfolderdir + "\\" + sourceFileName.Substring(sourceFileName.LastIndexOf("\\", StringComparison.OrdinalIgnoreCase) + 1);
                    if (!Directory.Exists(currentdir))
                    {
                        Directory.CreateDirectory(currentdir);
                    }

                    CopyDirectory(sourceFileName, desfolderdir);
                }

                else // 否则直接copy文件
                {
                    var destFileName = sourceFileName.Substring(sourceFileName.LastIndexOf("\\", StringComparison.OrdinalIgnoreCase) + 1);

                    destFileName = desfolderdir + "\\" + destFileName;
                    if (File.Exists(destFileName))
                    {
                        if (deleteExistingFile)
                        {
                            File.Delete(destFileName); //Delete Existing Files
                        }
                    }

                    if (!Directory.Exists(desfolderdir))
                    {
                        Directory.CreateDirectory(desfolderdir);
                    }
                    try
                    {
                        File.Copy(sourceFileName, destFileName, overWrite);
                    }
                    catch (Exception ex)
                    {
                        LogUtil.WriteException(ex);
                        //if any error return
                        return;
                    }

                    if (deleteSourceFile)
                    {
                        File.Delete(sourceFileName);
                    }


                }
            }
        }

        #endregion

        #region 获取文件夹大小
        /// <summary>
        /// 获取文件夹大小
        /// </summary>
        /// <param name="dirPath"></param>
        /// <returns></returns>
        public static decimal GetDirectorySize(string dirPath)
        {
            var result = 0M;
            if (Directory.Exists(dirPath))
            {
                var di = new DirectoryInfo(dirPath);
                //获取di目录中所有文件的大小
                foreach (var item in di.GetFiles())
                {
                    result += item.Length;
                }

                //获取di目录中所有的文件夹,并保存到一个数组中,以进行递归
                var dis = di.GetDirectories();
                if (dis.Length > 0)
                {
                    foreach (var t in dis)
                    {
                        //递归dis.Length个文件夹,得到每隔dis[i]下面所有文件的大小
                        result += GetDirectorySize(t.FullName);
                    }
                }
            }

            return result;
        }
        #endregion

        #region GetTextFileContent
        /// <summary>
        /// 读取文件文件内容
        /// 文本格式必须为utf-8
        /// (ansi格式容易产生乱码)
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string GetTextFileContent(string fileName, string encoding = "gb2312")
        {
            //var sr = new StreamReader(fileName, Encoding.GetEncoding("utf-8"));
            var sr = new StreamReader(fileName, Encoding.GetEncoding(encoding));
            var message = sr.ReadToEnd();
            // 及时关闭
            sr.Close();

            return message;
        }
        #endregion
    }
}