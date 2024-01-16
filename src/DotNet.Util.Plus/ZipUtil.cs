using System;
using System.IO;
using System.Linq;
using ICSharpCode.SharpZipLib.Checksum;
using ICSharpCode.SharpZipLib.Zip;
using NPOI.SS.UserModel;

namespace DotNet.Util
{
    /// <summary>
    /// Zip压缩工具
    /// </summary>
    public static partial class ZipUtil
    {
        /// <summary>
        /// 压缩文件
        /// ZipUtil.CreateZip(folderDirectory, string.Format(@"{0}\zip.zip", AppDomain.CurrentDomain.BaseDirectory));
        /// </summary>
        /// <param name="sourceFilePaths"></param>
        /// <param name="destinationZipFilePath"></param>
        /// <param name="compressionLevel">压缩级别 0-9</param>
        /// <param name="skipFolders">跳过文件夹名称</param>
        /// <param name="skipFileExtensions">跳过文件扩展名（后缀）</param>
        /// <param name="keepRootFolders">保持根目录结构</param>
        public static void CreateZip(string[] sourceFilePaths, string destinationZipFilePath, int compressionLevel = 6, string[] skipFolders = null, string[] skipFileExtensions = null, bool keepRootFolders = false)
        {
            var zipStream = new ZipOutputStream(File.Create(destinationZipFilePath));
            // 压缩级别 0-9
            zipStream.SetLevel(compressionLevel);
            foreach (var sourceFilePath in sourceFilePaths)
            {
                var filePath = sourceFilePath;
                if (filePath[filePath.Length - 1] != Path.DirectorySeparatorChar)
                {
                    filePath += Path.DirectorySeparatorChar;
                }
                if (Directory.Exists(filePath))
                {
                    CreateZipFiles(filePath, zipStream, filePath, skipFolders: skipFolders, skipFileExtensions: skipFileExtensions, keepRootFolders: keepRootFolders);
                }
            }
            zipStream.Finish();
            zipStream.Close();
        }

        /// <summary>
        /// 递归压缩文件
        /// </summary>
        /// <param name="sourceFilePath">待压缩的文件或文件夹路径</param>
        /// <param name="zipStream">打包结果的zip文件路径（类似 D:\WorkSpace\a.zip）,全路径包括文件名和.zip扩展名</param>
        /// <param name="staticFile"></param>
        /// <param name="skipFolders">跳过文件夹名称</param>
        /// <param name="skipFileExtensions">跳过文件扩展名（后缀）</param>
        /// <param name="keepRootFolders">保持根目录结构</param>
        private static void CreateZipFiles(string sourceFilePath, ZipOutputStream zipStream, string staticFile, string[] skipFolders = null, string[] skipFileExtensions = null, bool keepRootFolders = false)
        {
            var crc = new Crc32();
            var files = Directory.GetFileSystemEntries(sourceFilePath);
            foreach (var file in files)
            {
                var skip = false;
                if (skipFolders != null)
                {
                    foreach (var skipFolder in skipFolders)
                    {
                        var subFolders = file.Split("\\");
                        foreach (var subFolder in subFolders)
                        {
                            if (subFolder.Equals(skipFolder, StringComparison.OrdinalIgnoreCase))
                            {
                                skip = true;
                                break;
                            }
                        }
                    }
                }

                //如果当前是文件夹，递归
                if (!skip && Directory.Exists(file))
                {
                    CreateZipFiles(file, zipStream, staticFile, skipFolders: skipFolders, skipFileExtensions: skipFileExtensions, keepRootFolders: keepRootFolders);
                }
                else
                {

                    if (skipFileExtensions != null)
                    {
                        var fi = new FileInfo(file);
                        foreach (var extension in skipFileExtensions)
                        {
                            if (fi.Extension.Equals("." + extension.Replace(".", ""), StringComparison.OrdinalIgnoreCase))
                            {
                                skip = true;
                                break;
                            }
                        }
                    }
                    if (!skip)
                    {
                        //如果是文件，开始压缩
                        var fileStream = File.OpenRead(file);
                        var buffer = new byte[fileStream.Length];
                        fileStream.Read(buffer, 0, buffer.Length);
                        var tempFile = file.Substring(staticFile.LastIndexOf("\\") + 1);
                        if (keepRootFolders)
                        {
                            tempFile = file.Substring(staticFile.IndexOf("\\") + 1);
                        }
                        var entry = new ZipEntry(tempFile);

                        entry.DateTime = DateTime.Now;
                        entry.Size = fileStream.Length;
                        fileStream.Close();
                        crc.Reset();
                        crc.Update(buffer);
                        entry.Crc = crc.Value;
                        zipStream.PutNextEntry(entry);

                        zipStream.Write(buffer, 0, buffer.Length);
                    }
                }
            }
        }
    }
}
