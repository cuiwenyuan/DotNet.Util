#if NET452_OR_GREATER
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;

namespace DotNet.Util
{
    /// <summary>
    /// ͼƬ
    /// </summary>
    public class ImageUtil
    {
        #region GetImageCodecInfo
        /// <summary>
        /// GetImageCodecInfo
        /// </summary>
        /// <param name="imageFormat"></param>
        /// <returns></returns>
        public static ImageCodecInfo GetImageCodecInfoEncoder(ImageFormat imageFormat)
        {
            var imageCodecInfos = ImageCodecInfo.GetImageEncoders();
            foreach (var imageCodecInfo in imageCodecInfos)
            {
                if (imageCodecInfo.FormatID == imageFormat.Guid)
                {
                    return imageCodecInfo;
                }
            }
            return null;
        }

        #endregion

        #region ͼƬѹ��

        /// <summary>
        /// ����ѹ��ͼƬ
        /// </summary>
        /// <param name="filePath">ԭͼƬ��ַ</param>
        /// <param name="compressedFilePath">ѹ���󱣴�ͼƬ��ַ</param>
        /// <param name="quality">ѹ������������ԽСѹ����Խ�ߣ�1-100</param>
        /// <param name="maxSize">ѹ����ͼƬ������С��K��</param>
        /// <param name="isFirstCall">�Ƿ��ǵ�һ�ε���</param>
        /// <param name="isAutoRotate">�Ƿ��Զ���ת</param>
        /// <returns></returns>
        public static bool CompressImage(string filePath, string compressedFilePath, int quality = 90, int maxSize = 300, bool isFirstCall = true, bool isAutoRotate = false, bool keepExif = false, bool deleteOrignalImage = false)
        {
            //����ǵ�һ�ε��ã�ԭʼͼ��Ĵ�СС��Ҫѹ���Ĵ�С����ֱ�Ӹ����ļ������ҷ���true
            var fi = new FileInfo(filePath);
            if (fi.Exists)
            {
                if (isFirstCall && fi.Length < maxSize * 1024)
                {
                    if (!filePath.Equals(compressedFilePath, StringComparison.OrdinalIgnoreCase))
                    {
                        fi.CopyTo(compressedFilePath, true);
                        if (deleteOrignalImage)
                        {
                            fi.Delete();
                            fi.TryDispose();
                        }
                    }
                    return true;
                }
                var image = Image.FromFile(filePath);
                // ȡ��ԭʼͼƬ��Exif��Ϣ
                PropertyItem[] pis;
                if (keepExif)
                {
                    pis = image.PropertyItems;
                }
                else
                {
                    pis = null;
                }

                if (isAutoRotate)
                {
                    // ��Ƭ������ת
                    RotateImage(image);
                }
                var tFormat = image.RawFormat;
                var dHeight = image.Height / 2;
                var dWidth = image.Width / 2;
                int sW = 0, sH = 0;
                //����������
                var newSize = new Size(image.Width, image.Height);
                if (newSize.Width > dHeight || newSize.Width > dWidth)
                {
                    if ((newSize.Width * dHeight) > (newSize.Width * dWidth))
                    {
                        sW = dWidth;
                        sH = (dWidth * newSize.Height) / newSize.Width;
                    }
                    else
                    {
                        sH = dHeight;
                        sW = (newSize.Width * dHeight) / newSize.Height;
                    }
                }
                else
                {
                    sW = newSize.Width;
                    sH = newSize.Height;
                }

                var ob = new Bitmap(dWidth, dHeight);
                var g = Graphics.FromImage(ob);

                g.Clear(Color.WhiteSmoke);
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.DrawImage(image, new Rectangle((dWidth - sW) / 2, (dHeight - sH) / 2, sW, sH), 0, 0, image.Width, image.Height, GraphicsUnit.Pixel);
                g.Dispose();

                //���´���Ϊ����ͼƬʱ������ѹ������
                var ep = new EncoderParameters();
                long[] qy = new long[1];
                qy[0] = quality;//����ѹ���ı���1-100
                var eParam = new EncoderParameter(Encoder.Quality, qy);
                ep.Param[0] = eParam;

                try
                {
                    var arrayICI = ImageCodecInfo.GetImageEncoders();
                    ImageCodecInfo jpegICIinfo = null;
                    for (int x = 0; x < arrayICI.Length; x++)
                    {
                        if (arrayICI[x].FormatDescription.Equals("JPEG", StringComparison.OrdinalIgnoreCase))
                        {
                            jpegICIinfo = arrayICI[x];
                            break;
                        }
                    }
                    if (jpegICIinfo != null)
                    {
                        // ����ͼƬ��Exif��ϢΪԭʼ��Ϣ
                        if (pis != null)
                        {
                            foreach (var pi in pis)
                            {
                                ob.SetPropertyItem(pi);
                            }
                        }
                        ob.Save(compressedFilePath, jpegICIinfo, ep);
                        var fiCompressed = new FileInfo(compressedFilePath);
                        if (fiCompressed.Length > maxSize * 1024)
                        {
                            quality -= 10;
                            CompressImage(filePath, compressedFilePath, quality: quality, maxSize: maxSize, isFirstCall: false, isAutoRotate: false);
                        }
                    }
                    else
                    {
                        ob.Save(compressedFilePath, tFormat);
                    }
                    
                }
                catch (Exception ex)
                {
                    LogUtil.WriteException(ex);
                    return false;
                }
                finally
                {
                    image.Dispose();
                    ob.Dispose();
                }                
                if (deleteOrignalImage)
                {
                    fi.Delete();
                    fi.TryDispose();
                }
                return true;
            }
            return false;
        }
        #endregion

        #region ͼƬ����
        /// <summary>
        /// ����ͼƬexif��������
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        public static void RotateImage(Image img)
        {
            var propertyItems = img.PropertyItems;
            byte orientation = 0;
            var item = propertyItems.Where(i => i.Id == 274).ToArray();
            if (item.Length > 0)
            {
                orientation = item[0].Value[0];
                switch (orientation)
                {
                    case 1:
                        // No rotation required.
                        break;
                    case 2:
                        img.RotateFlip(RotateFlipType.RotateNoneFlipX);// horizontal flip
                        break;
                    case 3:
                        img.RotateFlip(RotateFlipType.Rotate180FlipNone);// right-top
                        break;
                    case 4:
                        img.RotateFlip(RotateFlipType.RotateNoneFlipY);// vertical flip
                        break;
                    case 5:
                        img.RotateFlip(RotateFlipType.Rotate90FlipX);
                        break;
                    case 6:
                        img.RotateFlip(RotateFlipType.Rotate90FlipNone);// right-top
                        break;
                    case 7:
                        img.RotateFlip(RotateFlipType.Rotate270FlipX);
                        break;
                    case 8:
                        img.RotateFlip(RotateFlipType.Rotate270FlipNone);// left-bottom
                        break;
                    default:
                        break;
                }
                if (orientation > 1)
                {
                    img.RemovePropertyItem(274);
                }

            }
        }
        #endregion

        #region ͼƬ��ȡEXIF�еľ�γ�ȵ���Ϣ

        public static ExifEntity GetExif(Image img)
        {
            var entity = new ExifEntity();
            try
            {
                //����ͼƬ   
                //ȡ�����е�����(��PropertyId������)   
                var propertyItems = img.PropertyItems.OrderBy(x => x.Id);
                foreach (var item in propertyItems)
                {
                    //ֻȡId��ΧΪ0x0000��0x001e
                    if (item.Id >= 0x0000 && item.Id <= 0x001e)
                    {
                        switch (item.Id)
                        {
                            case 0x0002://����γ��
                                if (item.Value.Length == 24)
                                {
                                    //degrees(��byte[0]~byte[3]ת��uint, ����byte[4]~byte[7]ת�ɵ�uint)   
                                    double d = BitConverter.ToUInt32(item.Value, 0) * 1.0d / BitConverter.ToUInt32(item.Value, 4);
                                    //minutes(��byte[8]~byte[11]ת��uint, ����byte[12]~byte[15]ת�ɵ�uint)   
                                    double m = BitConverter.ToUInt32(item.Value, 8) * 1.0d / BitConverter.ToUInt32(item.Value, 12);
                                    //seconds(��byte[16]~byte[19]ת��uint, ����byte[20]~byte[23]ת�ɵ�uint)   
                                    double s = BitConverter.ToUInt32(item.Value, 16) * 1.0d / BitConverter.ToUInt32(item.Value, 20);
                                    double dblGPSLatitude = (((s / 60 + m) / 60) + d);
                                    entity.Latitude = dblGPSLatitude;
                                }
                                break;
                            case 0x0004: //���þ���
                                if (item.Value.Length == 24)
                                {
                                    //degrees(��byte[0]~byte[3]ת��uint, ����byte[4]~byte[7]ת�ɵ�uint)   
                                    double d = BitConverter.ToUInt32(item.Value, 0) * 1.0d / BitConverter.ToUInt32(item.Value, 4);
                                    //minutes(��byte[8]~byte[11]ת��uint, ����byte[12]~byte[15]ת�ɵ�uint)   
                                    double m = BitConverter.ToUInt32(item.Value, 8) * 1.0d / BitConverter.ToUInt32(item.Value, 12);
                                    //seconds(��byte[16]~byte[19]ת��uint, ����byte[20]~byte[23]ת�ɵ�uint)   
                                    double s = BitConverter.ToUInt32(item.Value, 16) * 1.0d / BitConverter.ToUInt32(item.Value, 20);
                                    double dblGPSLongitude = (((s / 60 + m) / 60) + d);
                                    entity.Longitude = dblGPSLongitude;
                                }
                                break;
                        }
                    }
                    if (item.Id == 0x9003 || item.Id == 0x0132)//IdΪ0x9003��ʾ���յ�ʱ��
                    {
                        var propItemValue = item.Value;
                        var dateTimeStr = System.Text.Encoding.ASCII.GetString(propItemValue).Trim('\0');
                        //LogUtil.WriteLog("CreateTime:" + dateTimeStr);
                        var dt = dateTimeStr.IsNullOrEmpty() ? DateTime.Now : dateTimeStr.ToDateTime();
                        entity.CreateTime = dt;
                    }
                    if (item.Id == 0x9003)//IdΪ0x0132������ʱ��
                    {
                        var propItemValue = item.Value;
                        var dateTimeStr = System.Text.Encoding.ASCII.GetString(propItemValue).Trim('\0');
                        //LogUtil.WriteLog("UpdateTime:" + dateTimeStr);
                        var dt = dateTimeStr.IsNullOrEmpty() ? DateTime.Now : dateTimeStr.ToDateTime();
                        entity.UpdateTime = dt;
                    }

                    if (item.Id == 0x0112)
                    {
                        var propItemValue = item.Value;
                        entity.Orientation = propItemValue.ToInt();
                    }
                }
            }
            catch (Exception e)
            {
                LogUtil.WriteException(e);
            }
            return entity;
        }

        #endregion

        #region Exifʵ��
        public class ExifEntity
        {
            #region ����
            /// <summary>
            /// ����
            /// </summary>
            public int Orientation { get; set; } = 1;
            /// <summary>Geo��ϣ���롣����wgs84����</summary>
            public String Hash { get; set; }

            /// <summary>���ȡ�wgw84����</summary>
            public Double Longitude { get; set; }

            /// <summary>γ�ȡ�wgs84����</summary>
            public Double Latitude { get; set; }

            /// <summary>��ַ������XX���١�XX·</summary>
            public String Address { get; set; }

            /// <summary>���⡣POI�����ַ</summary>
            public String Title { get; set; }

            /// <summary>�ٶȾ��ȡ�bd09����</summary>
            public Double LongitudeBd09 { get; set; }

            /// <summary>�ٶ�γ�ȡ�bd09����</summary>
            public Double LatitudeBd09 { get; set; }

            /// <summary>���Ǿ��ȡ�gcj02���꣬�ߵ¡���Ѷ</summary>
            public Double LongitudeGcj02 { get; set; }

            /// <summary>����γ�ȡ�gcj02���꣬�ߵ¡���Ѷ</summary>
            public Double LatitudeGcj02 { get; set; }

            /// <summary>������롣�����ļ���ַ����321324114</summary>
            public Int32 AreaCode { get; set; }

            /// <summary>�������ơ������ļ���ַ</summary>
            public String AreaName { get; set; }

            /// <summary>ʡ�ݱ���</summary>
            public Int32 ProvinceId { get; set; }

            /// <summary>ʡ������</summary>
            public String Province { get; set; }

            /// <summary>���б���</summary>
            public Int32 CityId { get; set; }

            /// <summary>��������</summary>
            public String City { get; set; }

            /// <summary>���ر���</summary>
            public Int32 DistrictId { get; set; }

            /// <summary>��������</summary>
            public String District { get; set; }
            /// <summary>
            /// ����ʱ��
            /// </summary>
            public DateTime CreateTime { get; set; }
            /// <summary>
            /// ����ʱ��
            /// </summary>
            public DateTime UpdateTime { get; set; }
            #endregion
        }
        #endregion
    }
}
#endif