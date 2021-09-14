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
    /// ����ͼ�ο�DTcms
	/// </summary>
	public class ThumbnailUtil
	{
		private Image _srcImage;
		private string _srcFileName;		
		
		/// <summary>
		/// ����
		/// </summary>
		/// <param name="fileName">ԭʼͼƬ·��</param>
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
		/// �ص�
		/// </summary>
		/// <returns></returns>
		public bool ThumbnailCallback()
		{
			return false;
		}

		/// <summary>
		/// ��������ͼ,��������ͼ��Image����
		/// </summary>
		/// <param name="width">����ͼ���</param>
		/// <param name="height">����ͼ�߶�</param>
		/// <returns>����ͼ��Image����</returns>
		public Image GetImage(int width,int height)
		{
			Image img;
			var callb = new Image.GetThumbnailImageAbort(ThumbnailCallback); 
 			img = _srcImage.GetThumbnailImage(width,height,callb, IntPtr.Zero);
 			return img;
		}

		/// <summary>
		/// ��������ͼ
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
		/// ��������ͼ������
		/// </summary>
		/// <param name="width">����ͼ�Ŀ��</param>
		/// <param name="height">����ͼ�ĸ߶�</param>
		/// <param name="imgformat">�����ͼ���ʽ</param>
		/// <returns>����ͼ��Image����</returns>
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
		/// ����ͼƬ
		/// </summary>
		/// <param name="image">Image ����</param>
		/// <param name="savePath">����·��</param>
		/// <param name="ici">ָ����ʽ�ı�������</param>
		private static void SaveImage(Image image, string savePath, ImageCodecInfo ici)
		{
			//���� ԭͼƬ ����� EncoderParameters ����
			var parameters = new EncoderParameters(1);
			parameters.Param[0] = new EncoderParameter(Encoder.Quality, ((long) 100));
			image.Save(savePath, ici, parameters);
			parameters.Dispose();
		}

		/// <summary>
		/// ��ȡͼ���������������������Ϣ
		/// </summary>
		/// <param name="mimeType">��������������Ķ���;�����ʼ�����Э�� (MIME) ���͵��ַ���</param>
		/// <returns>����ͼ���������������������Ϣ</returns>
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
		/// �����³ߴ�
		/// </summary>
		/// <param name="width">ԭʼ���</param>
		/// <param name="height">ԭʼ�߶�</param>
		/// <param name="maxWidth">����¿��</param>
		/// <param name="maxHeight">����¸߶�</param>
		/// <returns></returns>
		private static Size ResizeImage(int width, int height, int maxWidth, int maxHeight)
		{
            //�˴�2012-02-05�޸Ĺ�=================
            if (maxWidth <= 0)
                maxWidth = width;
            if (maxHeight <= 0)
                maxHeight = height;
            //����2012-02-05�޸Ĺ�=================
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
        /// �õ�ͼƬ��ʽ
        /// </summary>
        /// <param name="name">�ļ�����</param>
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
		/// ����С������
		/// </summary>
		/// <param name="image">ͼƬ����</param>
		/// <param name="newFileName">�µ�ַ</param>
		/// <param name="newSize">���Ȼ���</param>
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
                //���ø�������ֵ��
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                //���ø�����,���ٶȳ���ƽ���̶�
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
				//���������ͼ�沢��͸������ɫ���
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
        /// ����С������
        /// </summary>
        /// <param name="fileName">ͼƬ�ļ���</param>
        /// <param name="newFileName">�µ�ַ</param>
        /// <param name="newSize">���Ȼ���</param>
        public static void MakeSquareImage(string fileName, string newFileName, int newSize)
        {
            MakeSquareImage(Image.FromFile(fileName), newFileName, newSize);
        }

        /// <summary>
        /// ����Զ��С������
		/// </summary>
		/// <param name="url">ͼƬurl</param>
		/// <param name="newFileName">�µ�ַ</param>
		/// <param name="newSize">���Ȼ���</param>
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
		/// ��������ͼ
		/// </summary>
		/// <param name="original">ͼƬ����</param>
		/// <param name="newFileName">��ͼ·��</param>
		/// <param name="maxWidth">�����</param>
		/// <param name="maxHeight">���߶�</param>
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
        /// ��������ͼ
        /// </summary>
        /// <param name="fileName">�ļ���</param>
        /// <param name="newFileName">��ͼ·��</param>
        /// <param name="maxWidth">�����</param>
        /// <param name="maxHeight">���߶�</param>
        public static void MakeThumbnailImage(string fileName, string newFileName, int maxWidth, int maxHeight)
        {
            //2012-02-05�޸Ĺ���֧���滻
            var imageBytes = File.ReadAllBytes(fileName);
            var img = Image.FromStream(new MemoryStream(imageBytes));
            MakeThumbnailImage(img, newFileName, maxWidth, maxHeight);
            //ԭ��
            //MakeThumbnailImage(Image.FromFile(fileName), newFileName, maxWidth, maxHeight);
        }

        #region 2012-2-19 ��������ͼƬ����ͼ����
        /// <summary>
        /// ��������ͼ
        /// </summary>
        /// <param name="physicalFileName">Դͼ·��������·����</param>
        /// <param name="newPhysicalFileName">����ͼ·��������·����</param>
        /// <param name="width">����ͼ���</param>
        /// <param name="height">����ͼ�߶�</param>
        /// <param name="mode">��������ͼ�ķ�ʽ</param>    
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
                    case "HW": //ָ���߿����ţ����ף�
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
                    case "W": //ָ�����߰�����
                        toheight = originalImage.Height * width / originalImage.Width;
                        break;
                    case "H": //ָ���ߣ�������
                        towidth = originalImage.Width * height / originalImage.Height;
                        break;
                    case "Cut": //ָ���߿�ü��������Σ�
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

                //�½�һ��bmpͼƬ
                var b = new Bitmap(towidth, toheight);
                try
                {
                    //�½�һ������
                    var g = Graphics.FromImage(b);
                    //���ø�������ֵ��
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    //���ø�����,���ٶȳ���ƽ���̶�
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    //��ջ�������͸������ɫ���
                    g.Clear(Color.White);
                    //g.Clear(Color.Transparent);
                    //��ָ��λ�ò��Ұ�ָ����С����ԭͼƬ��ָ������
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

        #region 2012-10-30 ����ͼƬ�ü�����
        /// <summary>
        /// �ü�ͼƬ������
        /// </summary>
        /// <param name="fileName">Դͼ·��������·����</param>
        /// <param name="newFileName">����ͼ·��������·����</param>
        /// <param name="maxWidth">����ͼ���</param>
        /// <param name="maxHeight">����ͼ�߶�</param>
        /// <param name="cropWidth">�ü����</param>
        /// <param name="cropHeight">�ü��߶�</param>
        /// <param name="x">X��</param>
        /// <param name="y">Y��</param>
        public static bool MakeThumbnailImage(string fileName, string newFileName, int maxWidth, int maxHeight, int cropWidth, int cropHeight, int x, int y)
        {
            var imageBytes = File.ReadAllBytes(fileName);
            var originalImage = Image.FromStream(new MemoryStream(imageBytes));
            var b = new Bitmap(cropWidth, cropHeight);
            try
            {
                using (var g = Graphics.FromImage(b))
                {
                    //���ø�������ֵ��
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    //���ø�����,���ٶȳ���ƽ���̶�
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    //��ջ�������͸������ɫ���
                    g.Clear(Color.Transparent);
                    //��ָ��λ�ò��Ұ�ָ����С����ԭͼƬ��ָ������
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
        /// ����Զ������ͼ
        /// </summary>
        /// <param name="url">ͼƬURL</param>
        /// <param name="newFileName">��ͼ·��</param>
        /// <param name="maxWidth">�����</param>
        /// <param name="maxHeight">���߶�</param>
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
        /// ��ȡͼƬ��
        /// </summary>
        /// <param name="url">ͼƬURL</param>
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