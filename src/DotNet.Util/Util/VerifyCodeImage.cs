//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2024, DotNet.
//-----------------------------------------------------------------

using System;
using System.Drawing;
using System.IO;
using System.Linq;
#if NET452_OR_GREATER
using System.Web;
#endif
#if NETSTANDARD2_0_OR_GREATER
using Microsoft.AspNetCore.Http;
#endif
#if NET5_0_OR_GREATER
using Microsoft.AspNetCore.Http;
#endif
namespace DotNet.Util
{
    /// <summary>
    /// 验证码图片
    /// </summary>
    public class VerifyCodeImage
    {
        /// <summary>
        /// 验证码图片
        /// </summary>
        public VerifyCodeImage()
        {
        }

        #region 验证码长度(默认4个验证码的长度)
        int _length = 4;
        /// <summary>
        /// 长度
        /// </summary>
        public int Length
        {
            get => _length;
            set => _length = value;
        }
        #endregion

        #region 验证码字体大小(为了显示扭曲效果，默认40像素，可以自行修改)
        int _fontSize = 50;
        /// <summary>
        /// 字体大小
        /// </summary>
        public int FontSize
        {
            get => _fontSize;
            set => _fontSize = value;
        }
        #endregion

        #region 边框补(默认1像素)
        int _padding = 2;
        /// <summary>
        /// 边距
        /// </summary>
        public int Padding
        {
            get => _padding;
            set => _padding = value;
        }
        #endregion

        #region 是否输出燥点(默认不输出)
        bool _chaos = true;
        /// <summary>
        /// 噪点
        /// </summary>
        public bool Chaos
        {
            get => _chaos;
            set => _chaos = value;
        }
        #endregion

        #region 输出燥点的颜色(默认灰色)
        Color _chaosColor = Color.LightGray;
        /// <summary>
        /// 噪点颜色
        /// </summary>
        public Color ChaosColor
        {
            get => _chaosColor;
            set => _chaosColor = value;
        }
        #endregion

        #region 自定义背景色(默认白色)
        Color _backgroundColor = Color.White;
        /// <summary>
        /// 背景色
        /// </summary>
        public Color BackgroundColor
        {
            get => _backgroundColor;
            set => _backgroundColor = value;
        }
        #endregion

        #region 自定义随机颜色数组
        Color[] _colors = { Color.Black, Color.Red, Color.DarkBlue, Color.Green, Color.Orange, Color.Brown, Color.DarkCyan, Color.Purple };
        /// <summary>
        /// 颜色
        /// </summary>
        public Color[] Colors
        {
            get => _colors;
            set => _colors = value;
        }
        #endregion

        #region 自定义字体数组
        string[] _fonts = { "Arial", "Georgia" };
        /// <summary>
        /// Fonts
        /// </summary>
        public string[] Fonts
        {
            get => _fonts;
            set => _fonts = value;
        }
        #endregion

        #region 自定义随机码字符串序列(使用逗号分隔)
        //  去除 0,1,i,l,o,I,L,O
        string _codeSerial = "2,3,4,5,6,7,8,9,a,b,c,d,e,f,g,h,j,k,m,n,p,q,r,s,t,u,v,w,x,y,z,A,B,C,D,E,F,G,H,J,K,M,N,P,Q,R,S,T,U,V,W,X,Y,Z";
        /// <summary>
        /// CodeSerial
        /// </summary>
        public string CodeSerial
        {
            get => _codeSerial;
            set => _codeSerial = value;
        }
        #endregion

        #region 产生波形滤镜效果
        //private const double PI = 3.1415926535897932384626433832795;
        private const double Pi2 = 6.283185307179586476925286766559;
        /// <summary>
        /// 正弦曲线Wave扭曲图片（Edit By 51aspx.com）
        /// </summary>
        /// <param name="srcBmp">图片路径</param>
        /// <param name="bXDir">如果扭曲则选择为True</param>
        /// <param name="dMultValue">波形的幅度倍数，越大扭曲的程度越高，一般为3</param>
        /// <param name="dPhase">波形的起始相位，取值区间[0-2*PI)</param>
        /// <returns></returns>
        public Bitmap TwistImage(Bitmap srcBmp, bool bXDir, double dMultValue, double dPhase)
        {
            var bitmap = new Bitmap(srcBmp.Width, srcBmp.Height);

            // 将位图背景填充为白色
            var graphics = Graphics.FromImage(bitmap);
            graphics.FillRectangle(new SolidBrush(Color.White), 0, 0, bitmap.Width, bitmap.Height);
            graphics.Dispose();

            var dBaseAxisLen = bXDir ? (double)bitmap.Height : (double)bitmap.Width;

            for (var i = 0; i < bitmap.Width; i++)
            {
                for (var j = 0; j < bitmap.Height; j++)
                {
                    double dx = 0;
                    dx = bXDir ? (Pi2 * (double)j) / dBaseAxisLen : (Pi2 * (double)i) / dBaseAxisLen;
                    dx += dPhase;
                    var dy = Math.Sin(dx);

                    // 取得当前点的颜色
                    int nOldX = 0, nOldY = 0;
                    nOldX = bXDir ? i + (int)(dy * dMultValue) : i;
                    nOldY = bXDir ? j : j + (int)(dy * dMultValue);

                    var color = srcBmp.GetPixel(i, j);
                    if (nOldX >= 0 && nOldX < bitmap.Width
                     && nOldY >= 0 && nOldY < bitmap.Height)
                    {
                        bitmap.SetPixel(nOldX, nOldY, color);
                    }
                }
            }
            return bitmap;
        }
        #endregion

        #region 生成校验码图片
        /// <summary>
        /// 创建验证码图片
        /// </summary>
        /// <param name="code"></param>
        /// <param name="multValue"></param>
        /// <returns></returns>
        public Bitmap CreateImage(string code, double multValue)
        {
            var fSize = FontSize;
            var fWidth = fSize + Padding;

            var imageWidth = (int)(code.Length * fWidth) + 4 + Padding * 2;
            var imageHeight = fSize * 2 + Padding;

            var bitmap = new Bitmap(imageWidth, imageHeight);

            var graphics = Graphics.FromImage(bitmap);

            graphics.Clear(BackgroundColor);

            var rand = new Random();

            // 给背景添加随机生成的燥点
            if (Chaos)
            {

                var pen = new Pen(ChaosColor, 0);
                var c = Length * 10;

                for (var i = 0; i < c; i++)
                {
                    var x = rand.Next(bitmap.Width);
                    var y = rand.Next(bitmap.Height);

                    graphics.DrawRectangle(pen, x, y, 1, 1);
                }
            }

            int left = 0, top = 0, top1 = 1, top2 = 1;

            var n1 = (imageHeight - FontSize - Padding * 2);
            var n2 = n1 / 4;
            top1 = n2;
            top2 = n2 * 2;

            Font font;
            Brush brush;

            int cindex, findex;

            // 随机字体和颜色的验证码字符
            for (var i = 0; i < code.Length; i++)
            {
                cindex = rand.Next(Colors.Length - 1);
                findex = rand.Next(Fonts.Length - 1);

                font = new Font(Fonts[findex], fSize, FontStyle.Bold);
                brush = new SolidBrush(Colors[cindex]);

                if (i % 2 == 1)
                {
                    top = top2;
                }
                else
                {
                    top = top1;
                }

                left = i * fWidth;

                graphics.DrawString(code.Substring(i, 1), font, brush, left, top);
            }

            // 画一个边框 边框颜色为Color.Gainsboro
            graphics.DrawRectangle(new Pen(Color.Gainsboro, 0), 0, 0, bitmap.Width - 1, bitmap.Height - 1);
            graphics.Dispose();

            // 产生波形
            bitmap = TwistImage(bitmap, true, multValue, 4);

            return bitmap;
        }
        #endregion

        #region 生成随机字符码
        /// <summary>
        /// 创建验证码
        /// </summary>
        /// <param name="codeLength"></param>
        /// <returns></returns>
        public string CreateVerifyCode(int codeLength)
        {
            if (codeLength == 0)
            {
                codeLength = Length;
            }
            var arr = CodeSerial.Split(',').Distinct<string>().Where(t => !string.IsNullOrEmpty(t)).ToArray();
            var code = "";
            var randValue = -1;
            var random = new Random(unchecked((int)DateTime.Now.Ticks));
            for (var i = 0; i < codeLength; i++)
            {
                randValue = random.Next(0, arr.Length - 1);
                code += arr[randValue];
            }
            return code;
        }
        /// <summary>
        /// 创建验证码
        /// </summary>
        /// <returns></returns>
        public string CreateVerifyCode()
        {
            return CreateVerifyCode(0);
        }
        #endregion

        #region 将创建好的图片输出到页面
        /// <summary>
        /// 将创建好的图片输出到页面
        /// </summary>
        /// <param name="code">验证码</param>
        /// <param name="multValue">扭曲度(越大越扭曲)</param>
        /// <param name="httpContext">上下文</param>
        public void CreateImageOnPage(string code, double multValue, HttpContext httpContext)
        {
            var memoryStream = new System.IO.MemoryStream();
            var bitmap = CreateImage(code, multValue);
            bitmap.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Jpeg);
#if NET452_OR_GREATER
            httpContext.Response.ClearContent();
            httpContext.Response.ContentType = "image/Jpeg";
            httpContext.Response.BinaryWrite(memoryStream.GetBuffer());
#elif NETSTANDARD2_0_OR_GREATER
            //TODO:.NET STANDARD 2.0的实现方式
#endif

            memoryStream.Close();
            memoryStream = null;
            bitmap.Dispose();
            bitmap = null;
        }

        /// <summary>
        /// 绘制验证码图片，返回图片的Base64字符串
        /// </summary>
        /// <param name="code">验证码</param>
        /// <param name="multValue">扭曲度(越大越扭曲)</param>
        /// <returns></returns>
        public string GetVerifyCodeBase64(string code, double multValue)
        {
            var bitmap = CreateImage(code, multValue);
            return "data:image/png;base64," + ToBase64(bitmap);
        }
        /// <summary>
        /// 图片转为base64编码的文本
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        private string ToBase64(Bitmap bitmap)
        {
            try
            {
                var ms = new MemoryStream();
                bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                var arr = new byte[ms.Length];
                ms.Position = 0;
                ms.Read(arr, 0, (int)ms.Length);
                ms.Close();
                return Convert.ToBase64String(arr);
            }
            catch (Exception ex)
            {
                LogUtil.WriteException(ex);
                return "";
            }
        }

                /// <summary>
        /// base64编码的文本转为图片
        /// </summary>
        /// <param name="inputStr"></param>
        /// <returns></returns>
        private Bitmap ToImage(string inputStr)
        {
            try
            {
                var array = Convert.FromBase64String(inputStr);
                var ms = new MemoryStream(array);
                var bitmap = new Bitmap(ms);
                ms.Close();
                return bitmap;
            }
            catch (Exception ex)
            {
                LogUtil.WriteException(ex);
                return null;
            }
        }
        #endregion
    }
}