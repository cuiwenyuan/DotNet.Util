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
            using var zipStream = new ZipOutputStream(File.Create(destinationZipFilePath));
            // 压缩级别 0-9
            zipStream.SetLevel(compressionLevel);
            foreach (var sourceFilePath in sourceFilePaths ?? Array.Empty<string>())
            {
                if (File.Exists(sourceFilePath))
                {
                    CreateZipFile(sourceFilePath, zipStream, Path.GetDirectoryName(Path.GetFullPath(sourceFilePath)), skipFileExtensions);
                }
                else if (Directory.Exists(sourceFilePath))
                {
                    var filePath = Path.GetFullPath(sourceFilePath);
                    if (!filePath.EndsWith(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal) &&
                        !filePath.EndsWith(Path.AltDirectorySeparatorChar.ToString(), StringComparison.Ordinal))
                    {
                        filePath += Path.DirectorySeparatorChar;
                    }
                    CreateZipFiles(filePath, zipStream, filePath, skipFolders: skipFolders, skipFileExtensions: skipFileExtensions, keepRootFolders: keepRootFolders);
                }
            }
            zipStream.Finish();
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
                        CreateZipFile(file, zipStream, staticFile, skipFileExtensions, keepRootFolders, crc);
                    }
                }
            }
        }

        private static void CreateZipFile(string file, ZipOutputStream zipStream, string staticFile, string[] skipFileExtensions, bool keepRootFolders = false, Crc32 crc = null)
        {
            if (skipFileExtensions != null)
            {
                var extension = Path.GetExtension(file);
                if (skipFileExtensions.Any(item => extension.Equals("." + item.Replace(".", ""), StringComparison.OrdinalIgnoreCase)))
                {
                    return;
                }
            }

            var root = Path.GetFullPath(staticFile ?? string.Empty).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            var fullFile = Path.GetFullPath(file);
            var entryName = keepRootFolders && fullFile.StartsWith(root + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase)
                ? fullFile.Substring(root.Length + 1)
                : Path.GetFileName(fullFile);
            entryName = entryName.Replace(Path.DirectorySeparatorChar, '/').Replace(Path.AltDirectorySeparatorChar, '/');

            using var fileStream = File.OpenRead(fullFile);
            var checksum = crc ?? new Crc32();
            checksum.Reset();
            var buffer = new byte[81920];
            int bytesRead;
            while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) > 0)
            {
                var checksumBuffer = new byte[bytesRead];
                Buffer.BlockCopy(buffer, 0, checksumBuffer, 0, bytesRead);
                checksum.Update(checksumBuffer);
            }

            var entry = new ZipEntry(entryName)
            {
                DateTime = DateTime.Now,
                Size = fileStream.Length,
                Crc = checksum.Value
            };
            zipStream.PutNextEntry(entry);
            fileStream.Position = 0;
            while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) > 0)
            {
                zipStream.Write(buffer, 0, bytesRead);
            }
            zipStream.CloseEntry();
        }
    }
}
