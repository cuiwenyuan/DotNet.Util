﻿#if NETSTANDARD2_0_OR_GREATER
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;

namespace DotNet.Util
{
    /// <summary>
    /// DrawingUtil
    /// </summary>
    public static class DrawingUtil
    {//颜色列表，用于验证码、噪线、噪点 
        private static readonly Color[] _colors = new[] { Color.Black, Color.Red, Color.Blue, Color.Green, Color.Orange, Color.Brown, Color.Brown, Color.DarkBlue };

        /// <summary>
        /// 绘制验证码图片，返回图片的字节数组
        /// </summary>
        /// <param name="code"></param>
        /// <param name="length">验证码长度</param>
        /// <returns></returns>
        public static byte[] DrawVerifyCode(out string code, int length = 6)
        {
            code = RandomUtil.GetNumber(length);
            //创建画布
            var bmp = new Bitmap(4 + 16 * code.Length, 40);
            //字体
            var font = new Font("Times New Roman", 16);

            var r = new Random();

            var g = Graphics.FromImage(bmp);
            g.Clear(Color.White);
            //画噪线 
            for (var i = 0; i < 4; i++)
            {
                var x1 = r.Next(bmp.Width);
                var y1 = r.Next(bmp.Height);
                var x2 = r.Next(bmp.Width);
                var y2 = r.Next(bmp.Height);
                g.DrawLine(new Pen(_colors.RandomGet()), x1, y1, x2, y2);
            }

            //画验证码字符串 
            for (var i = 0; i < code.Length; i++)
            {
                g.DrawString(code[i].ToString(), font, new SolidBrush(_colors.RandomGet()), (float)i * 16 + 2, 8);
            }

            //将验证码图片写入内存流，并将其以 "image/Png" 格式输出 
            var ms = new MemoryStream();
            try
            {
                bmp.Save(ms, ImageFormat.Png);
                return ms.ToArray();
            }
            catch (Exception)
            {
                return null;
            }
            finally
            {
                g.Dispose();
                bmp.Dispose();
            }
        }

        /// <summary>
        /// 绘制验证码图片，返回图片的Base64字符串
        /// </summary>
        /// <param name="code"></param>
        /// <param name="length">验证码长度</param>
        /// <returns></returns>
        public static string DrawVerifyCodeBase64String(out string code, int length = 6)
        {
            var bytes = DrawVerifyCode(out code, length);

            return "data:image/png;base64," + Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// 随机获取数组中的一个
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arr"></param>
        /// <returns></returns>
        public static T RandomGet<T>(this T[] arr)
        {
            if (arr == null || !arr.Any())
                return default(T);

            var r = new Random();

            return arr[r.Next(arr.Length)];
        }
    }
}
#endif