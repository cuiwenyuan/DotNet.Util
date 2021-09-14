#if NET40_OR_GREATER
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Net;

namespace DotNet.Util
{
	/// <summary>
    /// 缩略图参考DTcms
	/// </summary>
	public class ThumbnailUtil
	{
		private Image _srcImage;
		private string _srcFileName;		
		
		/// <summary>
		/// 创建
		/// </summary>
		/// <param name="fileName">原始图片路径</param>
		public bool SetImage(string fileName)
		{
			_srcFileName = Utils.GetMapPath(fileName);
			try
			{
				_srcImage = Image.FromFile(_srcFileName);
			}
			catch
			{
				return false;
			}
			return true;

		}

		/// <summary>
		/// 回调
		/// </summary>
		/// <returns></returns>
		public bool ThumbnailCallback()
		{
			return false;
		}

		/// <summary>
		/// 生成缩略图,返回缩略图的Image对象
		/// </summary>
		/// <param name="width">缩略图宽度</param>
		/// <param name="height">缩略图高度</param>
		/// <returns>缩略图的Image对象</returns>
		public Image GetImage(int width,int height)
		{
			Image img;
			var callb = new Image.GetThumbnailImageAbort(ThumbnailCallback); 
 			img = _srcImage.GetThumbnailImage(width,height,callb, IntPtr.Zero);
 			return img;
		}

		/// <summary>
		/// 保存缩略图
		/// </summary>
		/// <param name="width"></param>
		/// <param name="height"></param>
		public void SaveThumbnailImage(int width,int height)
		{
			switch(Path.GetExtension(_srcFileName).ToLower())
			{
				case ".png":
					SaveImage(width, height, ImageFormat.Png);
					break;
				case ".gif":
					SaveImage(width, height, ImageFormat.Gif);
					break;
				default:
					SaveImage(width, height, ImageFormat.Jpeg);
					break;
			}
		}

		/// <summary>
		/// 生成缩略图并保存
		/// </summary>
		/// <param name="width">缩略图的宽度</param>
		/// <param name="height">缩略图的高度</param>
		/// <param name="imgformat">保存的图像格式</param>
		/// <returns>缩略图的Image对象</returns>
		public void SaveImage(int width,int height, ImageFormat imgformat)
		{
            if (imgformat != ImageFormat.Gif && (_srcImage.Width > width) || (_srcImage.Height > height))
            {
                Image img;
                var callb = new Image.GetThumbnailImageAbort(ThumbnailCallback);
                img = _srcImage.GetThumbnailImage(width, height, callb, IntPtr.Zero);
                _srcImage.Dispose();
                img.Save(_srcFileName, imgformat);
                img.Dispose();
            }
		}

		#region Helper

		/// <summary>
		/// 保存图片
		/// </summary>
		/// <param name="image">Image 对象</param>
		/// <param name="savePath">保存路径</param>
		/// <param name="ici">指定格式的编解码参数</param>
		private static void SaveImage(Image image, string savePath, ImageCodecInfo ici)
		{
			//设置 原图片 对象的 EncoderParameters 对象
			var parameters = new EncoderParameters(1);
			parameters.Param[0] = new EncoderParameter(Encoder.Quality, ((long) 100));
			image.Save(savePath, ici, parameters);
			parameters.Dispose();
		}

		/// <summary>
		/// 获取图像编码解码器的所有相关信息
		/// </summary>
		/// <param name="mimeType">包含编码解码器的多用途网际邮件扩充协议 (MIME) 类型的字符串</param>
		/// <returns>返回图像编码解码器的所有相关信息</returns>
		private static ImageCodecInfo GetCodecInfo(string mimeType)
		{
			var codecInfo = ImageCodecInfo.GetImageEncoders();
			foreach(var ici in codecInfo)
			{
				if(ici.MimeType == mimeType)
                    return ici;
			}
			return null;
		}

		/// <summary>
		/// 计算新尺寸
		/// </summary>
		/// <param name="width">原始宽度</param>
		/// <param name="height">原始高度</param>
		/// <param name="maxWidth">最大新宽度</param>
		/// <param name="maxHeight">最大新高度</param>
		/// <returns></returns>
		private static Size ResizeImage(int width, int height, int maxWidth, int maxHeight)
		{
            //此次2012-02-05修改过=================
            if (maxWidth <= 0)
                maxWidth = width;
            if (maxHeight <= 0)
                maxHeight = height;
            //以上2012-02-05修改过=================
			var maxWidth2 = (decimal)maxWidth;
			var maxHeight2 = (decimal)maxHeight;
			var aspectRatio = maxWidth2 / maxHeight2;

			int newWidth, newHeight;
			var originalWidth = (decimal)width;
			var originalHeight = (decimal)height;
			
			if (originalWidth > maxWidth2 || originalHeight > maxHeight2) 
			{
				decimal factor;
				// determine the largest factor 
				if (originalWidth / originalHeight > aspectRatio) 
				{
					factor = originalWidth / maxWidth2;
					newWidth = Convert.ToInt32(originalWidth / factor);
					newHeight = Convert.ToInt32(originalHeight / factor);
				} 
				else 
				{
					factor = originalHeight / maxHeight2;
					newWidth = Convert.ToInt32(originalWidth / factor);
					newHeight = Convert.ToInt32(originalHeight / factor);
				}	  
			} 
			else 
			{
				newWidth = width;
				newHeight = height;
			}
			return new Size(newWidth,newHeight);			
		}

        /// <summary>
        /// 得到图片格式
        /// </summary>
        /// <param name="name">文件名称</param>
        /// <returns></returns>
        public static ImageFormat GetFormat(string name)
        {
            var ext = name.Substring(name.LastIndexOf(".") + 1);
            switch (ext.ToLower())
            {
                case "jpg":
                case "jpeg":
                    return ImageFormat.Jpeg;
                case "bmp":
                    return ImageFormat.Bmp;
                case "png":
                    return ImageFormat.Png;
                case "gif":
                    return ImageFormat.Gif;
                default:
                    return ImageFormat.Jpeg;
            }
        }
		#endregion

		/// <summary>
		/// 制作小正方形
		/// </summary>
		/// <param name="image">图片对象</param>
		/// <param name="newFileName">新地址</param>
		/// <param name="newSize">长度或宽度</param>
		public static void MakeSquareImage(Image image, string newFileName, int newSize)
		{	
			var i = 0;
			var width = image.Width;
			var height = image.Height;
			if (width > height)
				i = height;
			else
				i = width;

            var b = new Bitmap(newSize, newSize);

			try
			{
				var g = Graphics.FromImage(b);
                //设置高质量插值法
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                //设置高质量,低速度呈现平滑程度
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
				//清除整个绘图面并以透明背景色填充
				g.Clear(Color.Transparent);
				if (width < height)
					g.DrawImage(image,  new Rectangle(0, 0, newSize, newSize), new Rectangle(0, (height-width)/2, width, width), GraphicsUnit.Pixel);
				else
					g.DrawImage(image, new Rectangle(0, 0, newSize, newSize), new Rectangle((width-height)/2, 0, height, height), GraphicsUnit.Pixel);

                SaveImage(b, newFileName, GetCodecInfo("image/" + GetFormat(newFileName).ToString().ToLower()));
			}
			finally
			{
				image.Dispose();
				b.Dispose();
			}
		}

        /// <summary>
        /// 制作小正方形
        /// </summary>
        /// <param name="fileName">图片文件名</param>
        /// <param name="newFileName">新地址</param>
        /// <param name="newSize">长度或宽度</param>
        public static void MakeSquareImage(string fileName, string newFileName, int newSize)
        {
            MakeSquareImage(Image.FromFile(fileName), newFileName, newSize);
        }

        /// <summary>
        /// 制作远程小正方形
		/// </summary>
		/// <param name="url">图片url</param>
		/// <param name="newFileName">新地址</param>
		/// <param name="newSize">长度或宽度</param>
        public static void MakeRemoteSquareImage(string url, string newFileName, int newSize)
        {
            var stream = GetRemoteImage(url);
            if (stream == null)
                return;
            var original = Image.FromStream(stream);
            stream.Close();
            MakeSquareImage(original, newFileName, newSize);
        }

		/// <summary>
		/// 制作缩略图
		/// </summary>
		/// <param name="original">图片对象</param>
		/// <param name="newFileName">新图路径</param>
		/// <param name="maxWidth">最大宽度</param>
		/// <param name="maxHeight">最大高度</param>
        public static void MakeThumbnailImage(Image original, string newFileName, int maxWidth, int maxHeight)
		{
            var newSize = ResizeImage(original.Width, original.Height, maxWidth, maxHeight);

            using (Image displayImage = new Bitmap(original, newSize))
            {
                try
                {
                    displayImage.Save(newFileName, original.RawFormat);
                }
                finally
                {
                    original.Dispose();
                }
            }
		}

        /// <summary>
        /// 制作缩略图
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="newFileName">新图路径</param>
        /// <param name="maxWidth">最大宽度</param>
        /// <param name="maxHeight">最大高度</param>
        public static void MakeThumbnailImage(string fileName, string newFileName, int maxWidth, int maxHeight)
        {
            //2012-02-05修改过，支持替换
            var imageBytes = File.ReadAllBytes(fileName);
            var img = Image.FromStream(new MemoryStream(imageBytes));
            MakeThumbnailImage(img, newFileName, maxWidth, maxHeight);
            //原文
            //MakeThumbnailImage(Image.FromFile(fileName), newFileName, maxWidth, maxHeight);
        }

        #region 2012-2-19 新增生成图片缩略图方法
        /// <summary>
        /// 生成缩略图
        /// </summary>
        /// <param name="physicalFileName">源图路径（物理路径）</param>
        /// <param name="newPhysicalFileName">缩略图路径（物理路径）</param>
        /// <param name="width">缩略图宽度</param>
        /// <param name="height">缩略图高度</param>
        /// <param name="mode">生成缩略图的方式</param>    
        public static void MakeThumbnailImage(string physicalFileName, string newPhysicalFileName, int width, int height, string mode)
        {
            try
            {
                var originalImage = Image.FromFile(physicalFileName);
                var towidth = width;
                var toheight = height;

                var x = 0;
                var y = 0;
                var ow = originalImage.Width;
                var oh = originalImage.Height;

                switch (mode)
                {
                    case "HW": //指定高宽缩放（补白）
                        if ((double) originalImage.Width / (double) originalImage.Height >
                            (double) towidth / (double) toheight)
                        {
                            ow = originalImage.Width;
                            oh = originalImage.Width * height / towidth;
                            x = 0;
                            y = (originalImage.Height - oh) / 2;
                        }
                        else
                        {
                            oh = originalImage.Height;
                            ow = originalImage.Height * towidth / toheight;
                            y = 0;
                            x = (originalImage.Width - ow) / 2;
                        }
                        break;
                    case "W": //指定宽，高按比例
                        toheight = originalImage.Height * width / originalImage.Width;
                        break;
                    case "H": //指定高，宽按比例
                        towidth = originalImage.Width * height / originalImage.Height;
                        break;
                    case "Cut": //指定高宽裁减（不变形）
                        if ((double) originalImage.Width / (double) originalImage.Height >
                            (double) towidth / (double) toheight)
                        {
                            oh = originalImage.Height;
                            ow = originalImage.Height * towidth / toheight;
                            y = 0;
                            x = (originalImage.Width - ow) / 2;
                        }
                        else
                        {
                            ow = originalImage.Width;
                            oh = originalImage.Width * height / towidth;
                            x = 0;
                            y = (originalImage.Height - oh) / 2;
                        }
                        break;
                    default:
                        break;
                }

                //新建一个bmp图片
                var b = new Bitmap(towidth, toheight);
                try
                {
                    //新建一个画板
                    var g = Graphics.FromImage(b);
                    //设置高质量插值法
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    //设置高质量,低速度呈现平滑程度
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    //清空画布并以透明背景色填充
                    g.Clear(Color.White);
                    //g.Clear(Color.Transparent);
                    //在指定位置并且按指定大小绘制原图片的指定部分
                    g.DrawImage(originalImage, new Rectangle(0, 0, towidth, toheight), new Rectangle(x, y, ow, oh),
                        GraphicsUnit.Pixel);

                    SaveImage(b, newPhysicalFileName,
                        GetCodecInfo("image/" + GetFormat(newPhysicalFileName).ToString().ToLower()));
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    originalImage.Dispose();
                    b.Dispose();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        #endregion

        #region 2012-10-30 新增图片裁剪方法
        /// <summary>
        /// 裁剪图片并保存
        /// </summary>
        /// <param name="fileName">源图路径（绝对路径）</param>
        /// <param name="newFileName">缩略图路径（绝对路径）</param>
        /// <param name="maxWidth">缩略图宽度</param>
        /// <param name="maxHeight">缩略图高度</param>
        /// <param name="cropWidth">裁剪宽度</param>
        /// <param name="cropHeight">裁剪高度</param>
        /// <param name="x">X轴</param>
        /// <param name="y">Y轴</param>
        public static bool MakeThumbnailImage(string fileName, string newFileName, int maxWidth, int maxHeight, int cropWidth, int cropHeight, int x, int y)
        {
            var imageBytes = File.ReadAllBytes(fileName);
            var originalImage = Image.FromStream(new MemoryStream(imageBytes));
            var b = new Bitmap(cropWidth, cropHeight);
            try
            {
                using (var g = Graphics.FromImage(b))
                {
                    //设置高质量插值法
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    //设置高质量,低速度呈现平滑程度
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    //清空画布并以透明背景色填充
                    g.Clear(Color.Transparent);
                    //在指定位置并且按指定大小绘制原图片的指定部分
                    g.DrawImage(originalImage, new Rectangle(0, 0, cropWidth, cropHeight), x, y, cropWidth, cropHeight, GraphicsUnit.Pixel);
                    Image displayImage = new Bitmap(b, maxWidth, maxHeight);
                    SaveImage(displayImage, newFileName, GetCodecInfo("image/" + GetFormat(newFileName).ToString().ToLower()));
                    return true;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                originalImage.Dispose();
                b.Dispose();
            }
        }
        #endregion

        /// <summary>
        /// 制作远程缩略图
        /// </summary>
        /// <param name="url">图片URL</param>
        /// <param name="newFileName">新图路径</param>
        /// <param name="maxWidth">最大宽度</param>
        /// <param name="maxHeight">最大高度</param>
        public static void MakeRemoteThumbnailImage(string url, string newFileName, int maxWidth, int maxHeight)
        {
            var stream = GetRemoteImage(url);
            if(stream == null)
                return;
            var original = Image.FromStream(stream);
            stream.Close();
            MakeThumbnailImage(original, newFileName, maxWidth, maxHeight);
        }

        /// <summary>
        /// 获取图片流
        /// </summary>
        /// <param name="url">图片URL</param>
        /// <returns></returns>
        private static Stream GetRemoteImage(string url)
        {
            var request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.Method = "GET";
            request.ContentLength = 0;
            request.Timeout = 20000;
            HttpWebResponse response = null;

            try
            {
                response = (HttpWebResponse)request.GetResponse();
                return response.GetResponseStream();
            }
            catch
            {
                return null;
            }
        }
	}
}
#endif