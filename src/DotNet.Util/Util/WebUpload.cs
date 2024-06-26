﻿#if NET452_OR_GREATER
using System;
using System.Collections;
using System.Web;
using System.IO;
using System.Drawing;
using System.Net;
using System.Configuration;
using System.Collections.Generic;
using System.Drawing.Imaging;

namespace DotNet.Util
{
    /// <summary>
    /// 网页上传参考DTcms
    /// 
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2016-3-26</date>
    /// </author>
    /// </summary>
    public class WebUpload
    {
        private string _webpath = "/";
        private string _filepath = "UploadFiles";
        private int _filesave = 1;
        //private int _fileremote = 0;
        private int _attachsize = 0;
        private int _videosize = 0;
        private int _imgsize = 0;
        private int _imgmaxheight = 0;
        private int _imgmaxwidth = 0;
        private int _thumbnailheight = 180;
        private int _thumbnailwidth = 180;
        private int _watermarktype = 0;
        private int _watermarkposition = 9;
        private int _watermarkimgquality = 100;
        private string _watermarkpic = "";
        private int _watermarktransparency = 10;
        private string _watermarktext = "";
        private string _watermarkfont = "";
        private int _watermarkfontsize = 12;

        /// <summary>
        /// 构造函数
        /// </summary>
        public WebUpload()
        {
            
        }

        /// <summary>
        /// 裁剪图片并保存
        /// </summary>
        public bool CropSaveAs(string fileName, string newFileName, int maxWidth, int maxHeight, int cropWidth, int cropHeight, int x, int y)
        {
            var fileExt = Utils.GetFileExt(fileName); //文件扩展名，不含“.”
            if (!IsImage(fileExt))
            {
                return false;
            }
            var newFileDir = Utils.GetMapPath(newFileName.Substring(0, newFileName.LastIndexOf(@"/", StringComparison.OrdinalIgnoreCase) + 1));
            //检查是否有该路径，没有则创建
            if (!Directory.Exists(newFileDir))
            {
                Directory.CreateDirectory(newFileDir);
            }
            try
            {
                var fileFullPath = Utils.GetMapPath(fileName);
                var toFileFullPath = Utils.GetMapPath(newFileName);
                return ThumbnailUtil.MakeThumbnailImage(fileFullPath, toFileFullPath, 180, 180, cropWidth, cropHeight, x, y);
            }
            catch
            {
                return false;
            }
        }

        #region
        /// <summary>
        /// 文件上传方法
        /// </summary>
        /// <param name="postedFile">文件流</param>
        /// <param name="subFolder">子文件夹</param>
        /// <param name="isThumbnail">是否生成缩略图</param>
        /// <param name="isWater">是否打水印</param>
        /// <param name="thumbnailMode">缩略图模式（HW\W\H\Cut）</param>
        /// <returns>上传后文件信息</returns>
        public string FileSaveAs(HttpPostedFile postedFile, string subFolder, bool isThumbnail, bool isWater, string thumbnailMode = "Cut")
        {
            try
            {
                var fileExt = Utils.GetFileExt(postedFile.FileName); //文件扩展名，不含“.”
                var fileSize = postedFile.ContentLength; //获得文件大小，以字节为单位
                var fileName = postedFile.FileName.Substring(postedFile.FileName.LastIndexOf(@"\", StringComparison.Ordinal) + 1); //取得原文件名
                var newFileName = "original_" + Utils.GetRamCode() + "." + fileExt; //随机生成新的文件名
                var newThumbnailFileName = newFileName.Replace("original_", "thumbnail_"); //随机生成缩略图文件名
                var upLoadPath = GetUpLoadPath(subFolder); //上传目录相对路径
                var fullUpLoadPath = Utils.GetMapPath(upLoadPath); //上传目录的物理路径
                var newFilePath = upLoadPath + newFileName; //上传后的路径
                var newThumbnailPath = upLoadPath + newThumbnailFileName; //上传后的缩略图路径

                //检查文件扩展名是否合法
                if (!CheckFileExt(fileExt))
                {
                    return "{\"status\": 0, \"msg\": \"不允许上传" + fileExt + "类型的文件！\"}";
                }
                //检查文件大小是否合法
                if (!CheckFileSize(fileExt, fileSize))
                {
                    return "{\"status\": 0, \"msg\": \"文件超过限制的大小！\"}";
                }
                //检查上传的物理路径是否存在，不存在则创建
                if (!Directory.Exists(fullUpLoadPath))
                {
                    Directory.CreateDirectory(fullUpLoadPath);
                }

                //保存文件
                postedFile.SaveAs(fullUpLoadPath + newFileName);
                //如果是图片，检查图片是否超出最大尺寸，是则裁剪
                if (IsImage(fileExt) && (_imgmaxheight > 0 || _imgmaxwidth > 0))
                {
                    ThumbnailUtil.MakeThumbnailImage(fullUpLoadPath + newFileName, fullUpLoadPath + newFileName,
                        _imgmaxwidth, _imgmaxheight);
                }
                //如果是图片，检查是否需要生成缩略图，是则生成
                //LogUtil.WriteLog(fileExt);
                //LogUtil.WriteLog(IsImage(fileExt).ToString());
                //LogUtil.WriteLog(isThumbnail.ToString());
                //LogUtil.WriteLog(_thumbnailwidth.ToString());
                //LogUtil.WriteLog(_thumbnailheight.ToString());
                if (IsImage(fileExt) && isThumbnail && _thumbnailwidth > 0 && _thumbnailheight > 0)
                {
                    //LogUtil.WriteLog("MakeThumbnailImage");
                    ThumbnailUtil.MakeThumbnailImage(fullUpLoadPath + newFileName, fullUpLoadPath + newThumbnailFileName, _thumbnailwidth, _thumbnailheight, thumbnailMode);
                    //生成固定尺寸96的图标小图片icon
                    ThumbnailUtil.MakeThumbnailImage(fullUpLoadPath + newFileName, fullUpLoadPath + newThumbnailFileName.Replace("thumbnail_", "icon_"), 96, 96, thumbnailMode);
                    //生成固定尺寸180的小图片
                    ThumbnailUtil.MakeThumbnailImage(fullUpLoadPath + newFileName, fullUpLoadPath + newThumbnailFileName.Replace("thumbnail_", "small_"), 200, 200, thumbnailMode);
                    //生成固定尺寸300的小图片
                    ThumbnailUtil.MakeThumbnailImage(fullUpLoadPath + newFileName, fullUpLoadPath + newThumbnailFileName.Replace("thumbnail_", "middle_"), 350, 350, thumbnailMode);
                    //生成固定尺寸700的大图片
                    ThumbnailUtil.MakeThumbnailImage(fullUpLoadPath + newFileName, fullUpLoadPath + newThumbnailFileName.Replace("thumbnail_", "large_"), 700, 700, thumbnailMode);
                }
                else
                {
                    newThumbnailPath = newFilePath; //不生成缩略图则返回原图
                }
                //如果是图片，检查是否需要打水印
                if (IsWaterMark(fileExt) && isWater)
                {
                    switch (_watermarktype)
                    {
                        case 1:
                            WatermarkUtil.AddImageSignText(newFilePath, newFilePath,
                                _watermarktext, _watermarkposition,
                                _watermarkimgquality, _watermarkfont, _watermarkfontsize);
                            break;
                        case 2:
                            WatermarkUtil.AddImageSignPic(newFilePath, newFilePath,
                                _watermarkpic, _watermarkposition,
                                _watermarkimgquality, _watermarktransparency);
                            break;
                    }
                }
                //处理完毕，返回JOSN格式的文件信息
                return "{\"status\": 1, \"msg\": \"上传文件成功！\", \"name\": \""
                    + fileName + "\", \"path\": \"" + newFilePath + "\", \"thumb\": \""
                    + newThumbnailPath + "\", \"size\": " + fileSize + ", \"ext\": \"" + fileExt + "\"}";
            }
            catch
            {
                return "{\"status\": 0, \"msg\": \"上传过程中发生意外错误！\", \"name\": \""
                       + "" + "\", \"path\": \"" + "" + "\", \"thumb\": \""
                       + "" + "\", \"size\": " + 0 + ", \"ext\": \"" + "" + "\"}";
            }
        }

        #endregion

        #region

        /// <summary>
        /// 特定文件上传方法
        /// </summary>
        /// <param name="postedFile">文件流</param>
        /// <param name="isThumbnail">是否生成缩略图</param>
        /// <param name="isWater">是否打水印</param>
        /// <param name="itemCode">物品编码</param>
        /// <param name="subFolder">子目录</param>
        /// <param name="thumbnailMode">缩略图裁剪模式</param>
        /// <param name="isAutoRotate">是否自动旋转</param>
        /// <returns>上传后文件信息</returns>
        public string ItemCodeFileSaveAs(HttpPostedFile postedFile, bool isThumbnail, bool isWater, string itemCode, string subFolder = "ItemCode", string thumbnailMode = "Cut", bool isAutoRotate = false)
        {
            try
            {
                var fileExt = Utils.GetFileExt(postedFile.FileName); //文件扩展名，不含“.”
                var fileSize = postedFile.ContentLength; //获得文件大小，以字节为单位
                var fileName = postedFile.FileName.Substring(postedFile.FileName.LastIndexOf(@"\", StringComparison.Ordinal) + 1); //取得原文件名
                var newFileName = Utils.GetRamCode() + "." + fileExt; //随机生成新的文件名
                //重新定义文件名itemCode_original_日期随机数
                newFileName = itemCode + "_" + "original_" + Utils.GetRamCode() + "." + fileExt;
                var newThumbnailFileName = newFileName.Replace("original_", "thumbnail_"); //随机生成缩略图文件名
                var upLoadPath = GetUpLoadPath(subFolder); //上传目录相对路径
                var fullUpLoadPath = Utils.GetMapPath(upLoadPath); //上传目录的物理路径
                var newFilePath = upLoadPath + newFileName; //上传后的路径
                var newThumbnailPath = upLoadPath + newThumbnailFileName; //上传后的缩略图路径

                //检查文件扩展名是否合法
                if (!CheckFileExt(fileExt))
                {
                    return "{\"status\": 0, \"msg\": \"不允许上传" + fileExt + "类型的文件！\"}";
                }
                //检查文件大小是否合法
                if (!CheckFileSize(fileExt, fileSize))
                {
                    return "{\"status\": 0, \"msg\": \"文件超过限制的大小！\"}";
                }
                //检查上传的物理路径是否存在，不存在则创建
                if (!Directory.Exists(fullUpLoadPath))
                {
                    Directory.CreateDirectory(fullUpLoadPath);
                }

                //保存文件
                postedFile.SaveAs(fullUpLoadPath + newFileName);
                //如果是图片，检查图片是否超出最大尺寸，是则裁剪
                if (IsImage(fileExt) && (_imgmaxheight > 0 || _imgmaxwidth > 0))
                {
                    ThumbnailUtil.MakeThumbnailImage(fullUpLoadPath + newFileName, fullUpLoadPath + newFileName,
                        _imgmaxwidth, _imgmaxheight, isAutoRotate: isAutoRotate);
                }
                //如果是图片，检查是否需要生成缩略图，是则生成
                if (IsImage(fileExt) && isThumbnail && _thumbnailwidth > 0 && _thumbnailheight > 0)
                {
                    ThumbnailUtil.MakeThumbnailImage(fullUpLoadPath + newFileName, fullUpLoadPath + newThumbnailFileName, _thumbnailwidth, _thumbnailheight, thumbnailMode, isAutoRotate: isAutoRotate);
                    //生成固定尺寸96的小图片
                    ThumbnailUtil.MakeThumbnailImage(fullUpLoadPath + newFileName, fullUpLoadPath + newThumbnailFileName.Replace("thumbnail_", "icon_"), 96, 96, thumbnailMode, isAutoRotate: isAutoRotate);
                    //生成固定尺寸300的小图片
                    ThumbnailUtil.MakeThumbnailImage(fullUpLoadPath + newFileName, fullUpLoadPath + newThumbnailFileName.Replace("thumbnail_", "middle_"), 300, 300, thumbnailMode, isAutoRotate: isAutoRotate);
                    //生成固定尺寸700的大图片
                    ThumbnailUtil.MakeThumbnailImage(fullUpLoadPath + newFileName, fullUpLoadPath + newThumbnailFileName.Replace("thumbnail_", "large_"), 700, 700, thumbnailMode, isAutoRotate: isAutoRotate);
                }
                else
                {
                    newThumbnailPath = newFilePath; //不生成缩略图则返回原图
                }
                //如果是图片，检查是否需要打水印
                if (IsWaterMark(fileExt) && isWater)
                {
                    switch (_watermarktype)
                    {
                        case 1:
                            WatermarkUtil.AddImageSignText(newFilePath, newFilePath,
                                _watermarktext, _watermarkposition,
                                _watermarkimgquality, _watermarkfont, _watermarkfontsize);
                            break;
                        case 2:
                            WatermarkUtil.AddImageSignPic(newFilePath, newFilePath,
                                _watermarkpic, _watermarkposition,
                                _watermarkimgquality, _watermarktransparency);
                            break;
                    }
                }
                //处理完毕，返回JOSN格式的文件信息
                return "{\"status\": 1, \"msg\": \"上传文件成功！\", \"name\": \""
                    + fileName + "\", \"path\": \"" + newFilePath + "\", \"thumb\": \""
                    + newThumbnailPath + "\", \"size\": " + fileSize + ", \"ext\": \"" + fileExt + "\"}";
            }
            catch
            {
                return "{\"status\": 0, \"msg\": \"上传过程中发生意外错误！\"}";
            }
        }

        #endregion

        /// <summary>
        /// 保存远程文件到本地
        /// </summary>
        /// <param name="fileUri">URI地址</param>
        /// <returns>上传后的路径</returns>
        public string RemoteSaveAs(string fileUri)
        {
            var client = new WebClient();
            var fileExt = string.Empty; //文件扩展名，不含“.”
            if (fileUri.LastIndexOf(".", StringComparison.Ordinal) == -1)
            {
                fileExt = "gif";
            }
            else
            {
                fileExt = Utils.GetFileExt(fileUri);
            }
            var newFileName = Utils.GetRamCode() + "." + fileExt; //随机生成新的文件名
            var upLoadPath = GetUpLoadPath(); //上传目录相对路径
            var fullUpLoadPath = Utils.GetMapPath(upLoadPath); //上传目录的物理路径
            var newFilePath = upLoadPath + newFileName; //上传后的路径
            //检查上传的物理路径是否存在，不存在则创建
            if (!Directory.Exists(fullUpLoadPath))
            {
                Directory.CreateDirectory(fullUpLoadPath);
            }

            try
            {
                client.DownloadFile(fileUri, fullUpLoadPath + newFileName);
                //如果是图片，检查是否需要打水印
                if (IsWaterMark(fileExt))
                {
                    switch (_watermarktype)
                    {
                        case 1:
                            WatermarkUtil.AddImageSignText(newFilePath, newFilePath,
                                _watermarktext, _watermarkposition,
                                _watermarkimgquality, _watermarkfont, _watermarkfontsize);
                            break;
                        case 2:
                            WatermarkUtil.AddImageSignPic(newFilePath, newFilePath,
                                _watermarkpic, _watermarkposition,
                                _watermarkimgquality, _watermarktransparency);
                            break;
                    }
                }
            }
            catch
            {
                return string.Empty;
            }
            client.Dispose();
            return newFilePath;
        }

        /// <summary>
        /// 把经过base64编码的字符串保存为文件
        /// </summary>
        /// <param name="base64String">经base64加码后的字符串</param>
        /// <param name="uploadedFileName">上传后的文件名</param>
        /// <param name="subFolder">子目录</param>
        /// <param name="isThumbnail">是否生成缩略图</param>
        /// <param name="thumbnailMode"></param>
        /// <param name="isAutoRotate">是否自动旋转</param>
        /// <param name="keepExif">保留EXIF信息</param>
        /// <param name="createThumbnail">生成缩略图</param>
        /// <param name="createIcon">生成图标</param>
        /// <param name="createMiddle">生成中图</param>
        /// <param name="createLarge">生成大图</param>
        /// <param name="thumbnailWidth">缩略图宽度</param>
        /// <param name="thumbnailHeight">缩略图高度</param>
        /// <param name="iconWidth">图标宽度</param>
        /// <param name="iconHeight">图标高度</param>
        /// <param name="middleWidth">中图宽度</param>
        /// <param name="middleHeight">中图高度</param>
        /// <param name="largeWidth">大图宽度</param>
        /// <param name="largeHeight">大图高度</param>
        /// <returns>保存文件是否成功 </returns>  
        public string Base64SaveAs(string base64String, out string uploadedFileName, string subFolder = @"ItemPhoto", bool isThumbnail = true, string thumbnailMode = "W", bool isAutoRotate = false, bool keepExif = false, bool createThumbnail = true, bool createIcon = true, bool createMiddle = false, bool createLarge = true, int thumbnailWidth = 180, int thumbnailHeight = 180, int iconWidth = 90, int iconHeight = 90, int middleWidth = 500, int middleHeight = 500, int largeWidth = 700, int largeHeight = 700)
        {
            if (string.IsNullOrEmpty(thumbnailMode))
            {
                thumbnailMode = "W";
            }
            var fileExt = "jpeg";
            if (base64String.StartsWith("data:image/png"))
            {
                fileExt = "png";
            }
            if (base64String.StartsWith("data:image/jpeg"))
            {
                fileExt = "jpeg";
            }
            var newFileName = Utils.GetRamCode() + "_original." + fileExt; //随机生成新的文件名
            var uploadPath = GetUpLoadPath(subFolder); //上传目录相对路径
            var fullPhysicalUpLoadPath = Utils.GetMapPath(uploadPath); //上传目录的物理路径
            var newFilePath = uploadPath + newFileName; //上传后的路径
            var newPhysicalFilePath = Utils.GetMapPath(uploadPath + newFileName); //上传后的物理路径
            var newThumbnailFileName = newFileName.Replace("original", "thumbnail"); //随机生成缩略图文件名
            var newThumbnailPath = uploadPath + newThumbnailFileName; //上传后的缩略图路径
            var newPhysicalThumbnailPath = Utils.GetMapPath(uploadPath + newThumbnailFileName); //上传后的缩略图物理路径
            var newIconPath = uploadPath + newThumbnailFileName.Replace("thumbnail", "icon"); //上传后的缩略图路径
            var newPhysicalIconPath = Utils.GetMapPath(uploadPath + newThumbnailFileName.Replace("thumbnail", "icon")); //上传后的缩略图物理路径
            var newMiddlePath = uploadPath + newThumbnailFileName.Replace("thumbnail", "middle"); //上传后的缩略图路径
            var newPhysicalMiddlePath = Utils.GetMapPath(uploadPath + newThumbnailFileName.Replace("thumbnail", "middle")); //上传后的缩略图物理路径
            var newLargePath = uploadPath + newThumbnailFileName.Replace("thumbnail", "large"); //上传后的缩略图路径
            var newPhysicalLargePath = Utils.GetMapPath(uploadPath + newThumbnailFileName.Replace("thumbnail", "large")); //上传后的缩略图物理路径

            uploadedFileName = newFilePath;

            //检查上传的物理路径是否存在，不存在则创建
            if (!Directory.Exists(fullPhysicalUpLoadPath))
            {
                Directory.CreateDirectory(fullPhysicalUpLoadPath);
            }
            var fileSize = 0L;
            // 创建文件
            if (!string.IsNullOrEmpty(base64String) && !File.Exists(newPhysicalFilePath))
            {
                try
                {
                    var buffer = Convert.FromBase64String(base64String);
                    using (var ms = new MemoryStream(buffer, 0, buffer.Length))
                    {
                        // 读取数据
                        ms.Write(buffer, 0, buffer.Length);
                        // 从内存中生成图片
                        var image = Image.FromStream(ms, true);
                        var imageOriginal = (Image)image.Clone();

                        // 图像编码器和解码器
                        var encoder = ThumbnailUtil.GetCodecInfo("image/" + ThumbnailUtil.GetFormat(fileExt).ToString().ToLower());
                        // 编码参数
                        var parameters = new EncoderParameters(1);
                        // 设置质量
                        parameters.Param[0] = new EncoderParameter(Encoder.Quality, 100L);

                        // 照片方向旋转
                        if (isAutoRotate)
                        {
                            // 针对IPhone苹果相机横拍的不转向，竖拍的才转向，安卓手机拍照没问题
                            var exifTemp = ImageUtil.GetExif(image);
                            LogUtil.WriteLog(newPhysicalFilePath + JsonUtil.ObjectToJson(exifTemp), "Exif");
                            var orientation = exifTemp.Orientation;
                            if (orientation == 6)
                            {
                                // 保存原始
                                imageOriginal.Save(newPhysicalFilePath.Replace("original", "original_nochange"), encoder, parameters);
                                imageOriginal.Dispose();

                                // 这里的尺寸特别容易搞错，有Orientation的时候，宽和高反过来了
                                if (image.Size.Width > image.Size.Height)
                                {
                                    ImageUtil.RotateImage(image);
                                }
                                else if (image.Size.Width < image.Size.Height)
                                {
                                    // 强制为1，因为要保持横拍图的方向
                                    image.RemovePropertyItem(274);
                                }
                            }

                        }
                        // 保存图片
                        image.Save(newPhysicalFilePath, encoder, parameters);
                        // 获取图片大小
                        fileSize = new FileInfo(newPhysicalFilePath).Length;

                        // 获取EXIF
                        var exif = ImageUtil.GetExif(image);
                        LogUtil.WriteLog(newPhysicalFilePath + JsonUtil.ObjectToJson(exif), "Exif");

                        //处理完毕，返回JOSN格式的文件信息
                        //如果是图片，检查是否需要生成缩略图，是则生成
                        if (IsImage(fileExt) && isThumbnail && _thumbnailwidth > 0 && _thumbnailheight > 0)
                        {
                            if (createThumbnail)
                            {
                                // 生成系统默认尺寸的小图
                                ThumbnailUtil.MakeThumbnailImage((Image)image.Clone(), newPhysicalThumbnailPath, _thumbnailwidth, _thumbnailheight, thumbnailMode, isAutoRotate: isAutoRotate, keepExif: keepExif);
                            }
                            if (createIcon)
                            {
                                //生成固定尺寸96的小图片
                                ThumbnailUtil.MakeThumbnailImage((Image)image.Clone(), newPhysicalIconPath, iconWidth, iconHeight, thumbnailMode, isAutoRotate: isAutoRotate, keepExif: keepExif);
                            }
                            if (createMiddle)
                            {
                                //生成固定尺寸300的小图片
                                ThumbnailUtil.MakeThumbnailImage((Image)image.Clone(), newPhysicalMiddlePath, middleWidth, middleHeight, thumbnailMode, isAutoRotate: isAutoRotate, keepExif: keepExif);
                            }
                            if (createLarge)
                            {
                                //生成固定尺寸700的大图片
                                ThumbnailUtil.MakeThumbnailImage((Image)image.Clone(), newPhysicalLargePath, largeWidth, largeHeight, thumbnailMode, isAutoRotate: isAutoRotate, keepExif: keepExif);
                            }
                        }
                        else
                        {
                            newThumbnailPath = newFilePath; //不生成缩略图则返回原图
                            newIconPath = newFilePath; //不生成缩略图则返回原图
                            newMiddlePath = newFilePath; //不生成缩略图则返回原图
                            newLargePath = newFilePath; //不生成缩略图则返回原图
                        }
                        // 释放
                        image.Dispose();

                        var dic = new Dictionary<string, object>
                                {
                                    { "status", 1 },
                                    { "msg", "上传文件成功！" },
                                    { "name", "" + newFileName + "" },
                                    { "path", "" + newFilePath + "" },
                                    { "thumbnail", "" + newThumbnailPath + "" },
                                    { "icon", "" + newIconPath + "" },
                                    { "middle", "" + newMiddlePath + "" },
                                    { "large", "" + newLargePath + "" },
                                    { "size", "" + fileSize + "" },
                                    { "ext", "" + fileExt + "" },
                                    { "latitude", "" + exif.Latitude + "" },
                                    { "longitude", "" + exif.Longitude + "" },
                                    { "createTime", "" + exif.CreateTime + "" },
                                    { "updateTime", "" + exif.UpdateTime + "" }
                                };
                        return JsonUtil.ObjectToJson(dic);
                    }
                }
                catch (Exception e)
                {
                    LogUtil.WriteException(e);
                    return "{\"status\": 0, \"msg\": \"上传过程中发生意外错误！\"}";
                }
                finally
                {

                }
            }

            return "{\"status\": 0, \"msg\": \"上传过程中发生意外错误！\"}";

        }

        #region 私有方法
        /// <summary>
        /// 返回上传目录相对路径
        /// </summary>
        /// <param name="subFolder">子目录名称</param>
        private string GetUpLoadPath(string subFolder = null)
        {
            var path = _webpath + _filepath + "/"; //站点目录+上传目录
            if (!string.IsNullOrEmpty(subFolder))
            {
                path += subFolder + "/";
            }
            switch (_filesave)
            {
                case 1: //按年月日每天一个文件夹
                    path += DateTime.Now.ToString("yyyyMMdd");
                    break;
                default: //按年月/日存入不同的文件夹
                    path += DateTime.Now.ToString("yyyyMM") + "/" + DateTime.Now.ToString("dd");
                    break;
            }
            return path + "/";
        }

        /// <summary>
        /// 是否需要打水印
        /// </summary>
        /// <param name="fileExt">文件扩展名，不含“.”</param>
        private bool IsWaterMark(string fileExt)
        {
            //判断是否开启水印
            if (_watermarktype > 0)
            {
                //判断是否可以打水印的图片类型
                var al = new ArrayList();
                al.Add("bmp");
                al.Add("jpeg");
                al.Add("jpg");
                al.Add("png");
                if (al.Contains(fileExt.ToLower()))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 是否为图片文件
        /// </summary>
        /// <param name="fileExt">文件扩展名，不含“.”</param>
        private bool IsImage(string fileExt)
        {
            var al = new ArrayList();
            al.Add("bmp");
            al.Add("jpeg");
            al.Add("jpg");
            al.Add("gif");
            al.Add("png");
            if (al.Contains(fileExt.ToLower()))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 检查是否为合法的上传文件
        /// </summary>
        private bool CheckFileExt(string fileExt)
        {
            //检查危险文件
            string[] excExt = { "asp", "aspx", "ashx", "asa", "asmx", "asax", "php", "jsp", "htm", "html" };
            for (var i = 0; i < excExt.Length; i++)
            {
                if (excExt[i].Equals(fileExt, StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }
            }
            //检查合法文件
            var allowExt = (BaseSystemInfo.UploadFileExtension + "," + BaseSystemInfo.UploadVideoExtension).Split(',');
            for (var i = 0; i < allowExt.Length; i++)
            {
                if (allowExt[i].Equals(fileExt, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 检查文件大小是否合法
        /// </summary>
        /// <param name="fileExt">文件扩展名，不含“.”</param>
        /// <param name="fileSize">文件大小(B)</param>
        private bool CheckFileSize(string fileExt, int fileSize)
        {
            //将视频扩展名转换成ArrayList
            var lsVideoExt = new ArrayList(BaseSystemInfo.UploadVideoExtension.ToLower().Split(','));
            //判断是否为图片文件
            if (IsImage(fileExt))
            {
                if (_imgsize > 0 && fileSize > _imgsize * 1024)
                {
                    return false;
                }
            }
            else if (lsVideoExt.Contains(fileExt.ToLower()))
            {
                if (_videosize > 0 && fileSize > _videosize * 1024)
                {
                    return false;
                }
            }
            else
            {
                if (_attachsize > 0 && fileSize > _attachsize * 1024)
                {
                    return false;
                }
            }
            return true;
        }
        #endregion

    }
}
#endif