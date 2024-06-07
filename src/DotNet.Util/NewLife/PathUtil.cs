using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using DotNet.Util;
using NewLife.Data;
using static System.Net.Mime.MediaTypeNames;

namespace DotNet.Util
{
    /// <summary>路径操作帮助 - 来自NewLife</summary>
    /// <remarks>
    /// 文档 https://www.yuque.com/smartstone/nx/path_helper
    /// </remarks>
    public static partial class PathUtil
    {
        #region 属性
        /// <summary>基础目录。GetBasePath依赖于此，默认为当前应用程序域基础目录。用于X组件内部各目录，专门为函数计算而定制</summary>
        /// <remarks>
        /// 为了适应函数计算，该路径将支持从命令行参数和环境变量读取
        /// </remarks>
        public static String BasePath { get; set; }

        #endregion

        #region 静态构造
        static PathUtil()
        {
            BasePath = PathHelper.BasePath;
        }
        #endregion

        #region 路径操作辅助

        /// <summary>获取文件或目录基于应用程序域基目录的全路径，过滤相对目录</summary>
        /// <remarks>不确保目录后面一定有分隔符，是否有分隔符由原始路径末尾决定</remarks>
        /// <param name="path">文件或目录</param>
        /// <returns></returns>
        public static String GetFullPath(this String path)
        {
            return PathHelper.GetFullPath(path);
        }

        /// <summary>获取文件或目录的全路径，过滤相对目录。用于X组件内部各目录，专门为函数计算而定制</summary>
        /// <remarks>不确保目录后面一定有分隔符，是否有分隔符由原始路径末尾决定</remarks>
        /// <param name="path">文件或目录</param>
        /// <returns></returns>
        public static String GetBasePath(this String path)
        {
            return PathHelper.GetBasePath(path);
        }

        /// <summary>获取文件或目录基于当前目录的全路径，过滤相对目录</summary>
        /// <remarks>不确保目录后面一定有分隔符，是否有分隔符由原始路径末尾决定</remarks>
        /// <param name="path">文件或目录</param>
        /// <returns></returns>
        public static String GetCurrentPath(this String path)
        {
            return PathHelper.GetCurrentPath(path);
        }

        /// <summary>确保目录存在，若不存在则创建</summary>
        /// <remarks>
        /// 斜杠结尾的路径一定是目录，无视第二参数；
        /// 默认是文件，这样子只需要确保上一层目录存在即可，否则如果把文件当成了目录，目录的创建会导致文件无法创建。
        /// </remarks>
        /// <param name="path">文件路径或目录路径，斜杠结尾的路径一定是目录，无视第二参数</param>
        /// <param name="isfile">该路径是否是否文件路径。文件路径需要取目录部分</param>
        /// <returns></returns>
        public static String EnsureDirectory(this String path, Boolean isfile = true)
        {
            return PathHelper.EnsureDirectory(path, isfile);
        }

        /// <summary>合并多段路径</summary>
        /// <param name="path"></param>
        /// <param name="ps"></param>
        /// <returns></returns>
        public static String CombinePath(this String path, params String[] ps)
        {
            return PathHelper.CombinePath(path, ps);
        }
        #endregion

        #region 文件扩展
        /// <summary>文件路径作为文件信息</summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static FileInfo AsFile(this String file) => new(file.GetFullPath());

        /// <summary>从文件中读取数据</summary>
        /// <param name="file"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static Byte[] ReadBytes(this FileInfo file, Int32 offset = 0, Int32 count = -1)
        {
            return PathHelper.ReadBytes(file, offset, count);
        }

        /// <summary>把数据写入文件指定位置</summary>
        /// <param name="file"></param>
        /// <param name="data"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static FileInfo WriteBytes(this FileInfo file, Byte[] data, Int32 offset = 0)
        {
            return PathHelper.WriteBytes(file, data, offset);
        }

        ///// <summary>读取所有文本，自动检测编码</summary>
        ///// <remarks>性能较File.ReadAllText略慢，可通过提前检测BOM编码来优化</remarks>
        ///// <param name="file"></param>
        ///// <param name="encoding"></param>
        ///// <returns></returns>
        //public static String ReadText(this FileInfo file, Encoding encoding = null)
        //{
        //    return PathHelper.ReadText(file, encoding);
        //}

        ///// <summary>把文本写入文件，自动检测编码</summary>
        ///// <param name="file"></param>
        ///// <param name="text"></param>
        ///// <param name="encoding"></param>
        ///// <returns></returns>
        //public static FileInfo WriteText(this FileInfo file, String text, Encoding encoding = null)
        //{
        //    return PathHelper.WriteText(file, text, encoding);
        //}

        /// <summary>复制到目标文件，目标文件必须已存在，且源文件较新</summary>
        /// <param name="fi">源文件</param>
        /// <param name="destFileName">目标文件</param>
        /// <returns></returns>
        public static Boolean CopyToIfNewer(this FileInfo fi, String destFileName)
        {
            return PathHelper.CopyToIfNewer(fi, destFileName);
        }

        /// <summary>打开并读取</summary>
        /// <param name="file">文件信息</param>
        /// <param name="compressed">是否压缩</param>
        /// <param name="func">要对文件流操作的委托</param>
        /// <returns></returns>
        public static Int64 OpenRead(this FileInfo file, Boolean compressed, Action<Stream> func)
        {
            return PathHelper.OpenRead(file, compressed, func);
        }

        /// <summary>打开并写入</summary>
        /// <param name="file">文件信息</param>
        /// <param name="compressed">是否压缩</param>
        /// <param name="func">要对文件流操作的委托</param>
        /// <returns></returns>
        public static Int64 OpenWrite(this FileInfo file, Boolean compressed, Action<Stream> func)
        {
            return PathHelper.OpenWrite(file, compressed, func);
        }

        #endregion

        #region 目录扩展
        /// <summary>路径作为目录信息</summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        public static DirectoryInfo AsDirectory(this String dir) => new(dir.GetFullPath());

        /// <summary>获取目录内所有符合条件的文件，支持多文件扩展匹配</summary>
        /// <param name="di">目录</param>
        /// <param name="exts">文件扩展列表。比如*.exe;*.dll;*.config</param>
        /// <param name="allSub">是否包含所有子孙目录文件</param>
        /// <returns></returns>
        public static IEnumerable<FileInfo> GetAllFiles(this DirectoryInfo di, String exts = null, Boolean allSub = false)
        {
            return PathHelper.GetAllFiles(di, exts, allSub);
        }

        /// <summary>复制目录中的文件</summary>
        /// <param name="di">源目录</param>
        /// <param name="destDirName">目标目录</param>
        /// <param name="exts">文件扩展列表。比如*.exe;*.dll;*.config</param>
        /// <param name="allSub">是否包含所有子孙目录文件</param>
        /// <param name="callback">复制每一个文件之前的回调</param>
        /// <returns></returns>
        public static String[] CopyTo(this DirectoryInfo di, String destDirName, String exts = null, Boolean allSub = false, Action<String> callback = null)
        {
            return PathHelper.CopyTo(di, destDirName, exts, allSub, callback);
        }

        /// <summary>对比源目录和目标目录，复制双方都存在且源目录较新的文件</summary>
        /// <param name="di">源目录</param>
        /// <param name="destDirName">目标目录</param>
        /// <param name="exts">文件扩展列表。比如*.exe;*.dll;*.config</param>
        /// <param name="allSub">是否包含所有子孙目录文件</param>
        /// <param name="callback">复制每一个文件之前的回调</param>
        /// <returns></returns>
        public static String[] CopyToIfNewer(this DirectoryInfo di, String destDirName, String exts = null, Boolean allSub = false, Action<String> callback = null)
        {
            return PathHelper.CopyToIfNewer(di, destDirName, exts, allSub, callback);
        }

        /// <summary>从多个目标目录复制较新文件到当前目录</summary>
        /// <param name="di">当前目录</param>
        /// <param name="source">多个目标目录</param>
        /// <param name="exts">文件扩展列表。比如*.exe;*.dll;*.config</param>
        /// <param name="allSub">是否包含所有子孙目录文件</param>
        /// <returns></returns>
        public static String[] CopyIfNewer(this DirectoryInfo di, String[] source, String exts = null, Boolean allSub = false)
        {
            return PathHelper.CopyIfNewer(di, source, exts, allSub);
        }

        #endregion
    }
}