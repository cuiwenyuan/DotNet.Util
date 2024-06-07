using System;
#if NET452_OR_GREATER
using System.Web;
#endif
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.IO;

namespace DotNet.Util
{
    /// <summary>
    /// 水印参考DTcms
    /// </summary>
    public class WatermarkUtil
    {
#if NET452_OR_GREATER
        /// <summary>
        /// 图片水印来自DTcms
        /// </summary>
        /// <param name="imgPath">服务器图片相对路径</param>
        /// <param name="newPath">保存文件名</param>
        /// <param name="watermarkFileName">水印文件相对路径</param>
        /// <param name="watermarkPosition">图片水印位置 0=不使用 1=左上 2=中上 3=右上 4=左中 5=居中 6=右中 7=左下 8=中下 9=右下</param>
        /// <param name="quality">附加水印图片质量,0-100</param>
        /// <param name="watermarkTransparency">水印的透明度 1--10 10为不透明</param>
        public static void AddImageSignPic(string imgPath, string newPath, string watermarkFileName, int watermarkPosition, int quality, int watermarkTransparency)
        {
            if (!File.Exists(Utils.GetMapPath(imgPath)))
                return;
            var imageBytes = File.ReadAllBytes(Utils.GetMapPath(imgPath));
            var img = Image.FromStream(new MemoryStream(imageBytes));
            newPath = Utils.GetMapPath(newPath);

            if (watermarkFileName.StartsWith("/") == false)
            {
                watermarkFileName = "/" + watermarkFileName;
            }
            watermarkFileName = Utils.GetMapPath(watermarkFileName);
            if (!File.Exists(watermarkFileName))
                return;
            var g = Graphics.FromImage(img);
            //设置高质量插值法
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            //设置高质量,低速度呈现平滑程度
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;

            Image watermark = new Bitmap(watermarkFileName);

            if (watermark.Height >= img.Height || watermark.Width >= img.Width)
                return;

            var imageAttributes = new ImageAttributes();
            var colorMap = new ColorMap();

            colorMap.OldColor = Color.FromArgb(255, 0, 255, 0);
            colorMap.NewColor = Color.FromArgb(0, 0, 0, 0);
            ColorMap[] remapTable = { colorMap };

            imageAttributes.SetRemapTable(remapTable, ColorAdjustType.Bitmap);

            var transparency = 0.5F;
            if (watermarkTransparency >= 1 && watermarkTransparency <= 10)
                transparency = (watermarkTransparency / 10.0F);


            float[][] colorMatrixElements = {
                                                new float[] {1.0f,  0.0f,  0.0f,  0.0f, 0.0f},
                                                new float[] {0.0f,  1.0f,  0.0f,  0.0f, 0.0f},
                                                new float[] {0.0f,  0.0f,  1.0f,  0.0f, 0.0f},
                                                new float[] {0.0f,  0.0f,  0.0f,  transparency, 0.0f},
                                                new float[] {0.0f,  0.0f,  0.0f,  0.0f, 1.0f}
                                            };

            var colorMatrix = new ColorMatrix(colorMatrixElements);

            imageAttributes.SetColorMatrix(colorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

            var xpos = 0;
            var ypos = 0;

            switch (watermarkPosition)
            {
                case 1:
                    // 左上
                    xpos = (int)(img.Width * (float).01);
                    ypos = (int)(img.Height * (float).01);
                    break;
                case 2:
                    // 中上
                    xpos = (int)((img.Width * (float).50) - (watermark.Width / 2));
                    ypos = (int)(img.Height * (float).01);
                    break;
                case 3:
                    // 右上
                    xpos = (int)((img.Width * (float).99) - (watermark.Width));
                    ypos = (int)(img.Height * (float).01);
                    break;
                case 4:
                    // 左中
                    xpos = (int)(img.Width * (float).01);
                    ypos = (int)((img.Height * (float).50) - (watermark.Height / 2));
                    break;
                case 5:
                    // 居中
                    xpos = (int)((img.Width * (float).50) - (watermark.Width / 2));
                    ypos = (int)((img.Height * (float).50) - (watermark.Height / 2));
                    break;
                case 6:
                    // 右中
                    xpos = (int)((img.Width * (float).99) - (watermark.Width));
                    ypos = (int)((img.Height * (float).50) - (watermark.Height / 2));
                    break;
                case 7:
                    // 左下
                    xpos = (int)(img.Width * (float).01);
                    ypos = (int)((img.Height * (float).99) - watermark.Height);
                    break;
                case 8:
                    // 中下
                    xpos = (int)((img.Width * (float).50) - (watermark.Width / 2));
                    ypos = (int)((img.Height * (float).99) - watermark.Height);
                    break;
                case 9:
                    // 右下
                    xpos = (int)((img.Width * (float).99) - (watermark.Width));
                    ypos = (int)((img.Height * (float).99) - watermark.Height);
                    break;
            }

            g.DrawImage(watermark, new Rectangle(xpos, ypos, watermark.Width, watermark.Height), 0, 0, watermark.Width, watermark.Height, GraphicsUnit.Pixel, imageAttributes);

            var codecs = ImageCodecInfo.GetImageEncoders();
            ImageCodecInfo ici = null;
            foreach (var codec in codecs)
            {
                if (codec.MimeType.IndexOf("jpeg") > -1)
                    ici = codec;
            }
            var encoderParams = new EncoderParameters();
            var qualityParam = new long[1];
            if (quality < 0 || quality > 100)
                quality = 80;

            qualityParam[0] = quality;

            var encoderParam = new EncoderParameter(Encoder.Quality, qualityParam);
            encoderParams.Param[0] = encoderParam;

            if (ici != null)
                img.Save(newPath, ici, encoderParams);
            else
                img.Save(newPath);

            g.Dispose();
            img.Dispose();
            watermark.Dispose();
            imageAttributes.Dispose();
        }

        /// <summary>
        /// 文字水印
        /// </summary>
        /// <param name="imgPath">服务器图片相对路径</param>
        /// <param name="newPath">新图片路径</param>
        /// <param name="watermarkText">水印文字</param>
        /// <param name="watermarkPosition">图片水印位置 0=不使用 1=左上 2=中上 3=右上 4=左中 5=居中 6=右中 7=左下 8=中下 9=右下</param>
        /// <param name="quality">附加水印图片质量,0-100</param>
        /// <param name="fontName">字体</param>
        /// <param name="fontSize">字体大小</param>
        /// <param name="fontColor">字体颜色</param>
        /// <param name="bolderColor">边框颜色</param>
        /// <param name="alpha">透明度(0-255)值越大透明度越低</param>
        /// <param name="isBold">是否粗体</param>
        public static void AddImageSignText(string imgPath, string newPath, string watermarkText, int watermarkPosition, int quality = 100, string fontName = "微软雅黑", int fontSize = 48, string fontColor = "White", string bolderColor = "Black", int alpha = 255, bool isBold = false)
        {
            var imageBytes = File.ReadAllBytes(Utils.GetMapPath(imgPath));
            var img = Image.FromStream(new MemoryStream(imageBytes));
            newPath = Utils.GetMapPath(newPath);

            var g = Graphics.FromImage(img);
            //设置高质量插值法
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            //设置高质量,低速度呈现平滑程度
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            var drawFont = new Font(fontName, fontSize, isBold ? FontStyle.Bold : FontStyle.Regular, GraphicsUnit.Pixel);
            SizeF crSize;
            crSize = g.MeasureString(watermarkText, drawFont);

            float xpos = 0;
            float ypos = 0;

            switch (watermarkPosition)
            {
                case 1:
                    // 左上
                    xpos = (float)img.Width * (float).01;
                    ypos = (float)img.Height * (float).01;
                    break;
                case 2:
                    // 中上
                    xpos = ((float)img.Width * (float).50) - (crSize.Width / 2);
                    ypos = (float)img.Height * (float).01;
                    break;
                case 3:
                    // 右上
                    xpos = ((float)img.Width * (float).99) - crSize.Width;
                    ypos = (float)img.Height * (float).01;
                    break;
                case 4:
                    // 左中
                    xpos = (float)img.Width * (float).01;
                    ypos = ((float)img.Height * (float).50) - (crSize.Height / 2);
                    break;
                case 5:
                    // 居中
                    xpos = ((float)img.Width * (float).50) - (crSize.Width / 2);
                    ypos = ((float)img.Height * (float).50) - (crSize.Height / 2);
                    break;
                case 6:
                    // 右中
                    xpos = ((float)img.Width * (float).99) - crSize.Width;
                    ypos = ((float)img.Height * (float).50) - (crSize.Height / 2);
                    break;
                case 7:
                    // 左下
                    xpos = (float)img.Width * (float).01;
                    ypos = ((float)img.Height * (float).99) - crSize.Height;
                    break;
                case 8:
                    // 中下
                    xpos = ((float)img.Width * (float).50) - (crSize.Width / 2);
                    ypos = ((float)img.Height * (float).99) - crSize.Height;
                    break;
                case 9:
                    // 右下
                    xpos = ((float)img.Width * (float).99) - crSize.Width;
                    ypos = ((float)img.Height * (float).99) - crSize.Height;
                    break;
            }
            // 阴影字体
            g.DrawString(watermarkText, drawFont, new SolidBrush(Color.FromArgb(alpha < 0 ? 0 : alpha, ColorTranslator.FromHtml(bolderColor))), xpos + 1, ypos + 1);
            // 字体
            g.DrawString(watermarkText, drawFont, new SolidBrush(Color.FromArgb(alpha < 0 ? 0 : alpha, ColorTranslator.FromHtml(fontColor))), xpos, ypos);

            var codecs = ImageCodecInfo.GetImageEncoders();
            ImageCodecInfo ici = null;
            foreach (var codec in codecs)
            {
                if (codec.MimeType.IndexOf("jpeg") > -1)
                    ici = codec;
            }
            var encoderParams = new EncoderParameters();
            var qualityParam = new long[1];
            if (quality < 0 || quality > 100)
            {
                quality = 100;
            }

            qualityParam[0] = quality;

            var encoderParam = new EncoderParameter(Encoder.Quality, qualityParam);
            encoderParams.Param[0] = encoderParam;

            if (ici != null)
            {
                img.Save(newPath, ici, encoderParams);
            }
            else
            {
                img.Save(newPath);
            }

            g.Dispose();
            img.Dispose();
        }
#endif
    }
}
