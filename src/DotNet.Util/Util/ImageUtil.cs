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
    /// 图片
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

        #region 图片压缩

        /// <summary>
        /// 无损压缩图片
        /// </summary>
        /// <param name="filePath">原图片地址</param>
        /// <param name="compressedFilePath">压缩后保存图片地址</param>
        /// <param name="quality">压缩质量（数字越小压缩率越高）1-100</param>
        /// <param name="maxSize">压缩后图片的最大大小（K）</param>
        /// <param name="isFirstCall">是否是第一次调用</param>
        /// <param name="isAutoRotate">是否自动旋转</param>
        /// <returns></returns>
        public static bool CompressImage(string filePath, string compressedFilePath, int quality = 90, int maxSize = 300, bool isFirstCall = true, bool isAutoRotate = false, bool keepExif = false, bool deleteOrignalImage = false)
        {
            //如果是第一次调用，原始图像的大小小于要压缩的大小，则直接复制文件，并且返回true
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
                // 取得原始图片的Exif信息
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
                    // 照片方向旋转
                    RotateImage(image);
                }
                var tFormat = image.RawFormat;
                var dHeight = image.Height / 2;
                var dWidth = image.Width / 2;
                int sW = 0, sH = 0;
                //按比例缩放
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

                //以下代码为保存图片时，设置压缩质量
                var ep = new EncoderParameters();
                long[] qy = new long[1];
                qy[0] = quality;//设置压缩的比例1-100
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
                        // 设置图片的Exif信息为原始信息
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

        #region 图片方向
        /// <summary>
        /// 根据图片exif调整方向
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

        #region 图片获取EXIF中的经纬度等信息

        public static ExifEntity GetExif(Image img)
        {
            var entity = new ExifEntity();
            try
            {
                //载入图片   
                //取得所有的属性(以PropertyId做排序)   
                var propertyItems = img.PropertyItems.OrderBy(x => x.Id);
                foreach (var item in propertyItems)
                {
                    //只取Id范围为0x0000到0x001e
                    if (item.Id >= 0x0000 && item.Id <= 0x001e)
                    {
                        switch (item.Id)
                        {
                            case 0x0002://设置纬度
                                if (item.Value.Length == 24)
                                {
                                    //degrees(将byte[0]~byte[3]转成uint, 除以byte[4]~byte[7]转成的uint)   
                                    double d = BitConverter.ToUInt32(item.Value, 0) * 1.0d / BitConverter.ToUInt32(item.Value, 4);
                                    //minutes(將byte[8]~byte[11]转成uint, 除以byte[12]~byte[15]转成的uint)   
                                    double m = BitConverter.ToUInt32(item.Value, 8) * 1.0d / BitConverter.ToUInt32(item.Value, 12);
                                    //seconds(將byte[16]~byte[19]转成uint, 除以byte[20]~byte[23]转成的uint)   
                                    double s = BitConverter.ToUInt32(item.Value, 16) * 1.0d / BitConverter.ToUInt32(item.Value, 20);
                                    double dblGPSLatitude = (((s / 60 + m) / 60) + d);
                                    entity.Latitude = dblGPSLatitude;
                                }
                                break;
                            case 0x0004: //设置经度
                                if (item.Value.Length == 24)
                                {
                                    //degrees(将byte[0]~byte[3]转成uint, 除以byte[4]~byte[7]转成的uint)   
                                    double d = BitConverter.ToUInt32(item.Value, 0) * 1.0d / BitConverter.ToUInt32(item.Value, 4);
                                    //minutes(将byte[8]~byte[11]转成uint, 除以byte[12]~byte[15]转成的uint)   
                                    double m = BitConverter.ToUInt32(item.Value, 8) * 1.0d / BitConverter.ToUInt32(item.Value, 12);
                                    //seconds(将byte[16]~byte[19]转成uint, 除以byte[20]~byte[23]转成的uint)   
                                    double s = BitConverter.ToUInt32(item.Value, 16) * 1.0d / BitConverter.ToUInt32(item.Value, 20);
                                    double dblGPSLongitude = (((s / 60 + m) / 60) + d);
                                    entity.Longitude = dblGPSLongitude;
                                }
                                break;
                        }
                    }
                    if (item.Id == 0x9003 || item.Id == 0x0132)//Id为0x9003表示拍照的时间
                    {
                        var propItemValue = item.Value;
                        var dateTimeStr = System.Text.Encoding.ASCII.GetString(propItemValue).Trim('\0');
                        //LogUtil.WriteLog("CreateTime:" + dateTimeStr);
                        var dt = dateTimeStr.IsNullOrEmpty() ? DateTime.Now : dateTimeStr.ToDateTime();
                        entity.CreateTime = dt;
                    }
                    if (item.Id == 0x9003)//Id为0x0132最后更新时间
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

        #region Exif实体
        public class ExifEntity
        {
            #region 属性
            /// <summary>
            /// 方向
            /// </summary>
            public int Orientation { get; set; } = 1;
            /// <summary>Geo哈希编码。基于wgs84坐标</summary>
            public String Hash { get; set; }

            /// <summary>经度。wgw84坐标</summary>
            public Double Longitude { get; set; }

            /// <summary>纬度。wgs84坐标</summary>
            public Double Latitude { get; set; }

            /// <summary>地址。例如XX高速、XX路</summary>
            public String Address { get; set; }

            /// <summary>标题。POI语义地址</summary>
            public String Title { get; set; }

            /// <summary>百度经度。bd09坐标</summary>
            public Double LongitudeBd09 { get; set; }

            /// <summary>百度纬度。bd09坐标</summary>
            public Double LatitudeBd09 { get; set; }

            /// <summary>火星经度。gcj02坐标，高德、腾讯</summary>
            public Double LongitudeGcj02 { get; set; }

            /// <summary>火星纬度。gcj02坐标，高德、腾讯</summary>
            public Double LatitudeGcj02 { get; set; }

            /// <summary>区域编码。乡镇四级地址，如321324114</summary>
            public Int32 AreaCode { get; set; }

            /// <summary>区域名称。乡镇四级地址</summary>
            public String AreaName { get; set; }

            /// <summary>省份编码</summary>
            public Int32 ProvinceId { get; set; }

            /// <summary>省份名称</summary>
            public String Province { get; set; }

            /// <summary>城市编码</summary>
            public Int32 CityId { get; set; }

            /// <summary>城市名称</summary>
            public String City { get; set; }

            /// <summary>区县编码</summary>
            public Int32 DistrictId { get; set; }

            /// <summary>区县名称</summary>
            public String District { get; set; }
            /// <summary>
            /// 创建时间
            /// </summary>
            public DateTime CreateTime { get; set; }
            /// <summary>
            /// 更新时间
            /// </summary>
            public DateTime UpdateTime { get; set; }
            #endregion
        }
        #endregion
    }
}
#endif