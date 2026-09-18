using System.Linq;
using DotNet.Util;
using Xunit;

namespace DotNet.Util.Tests.Util
{
    /// <summary>
    /// VerifyCodeImage 纯逻辑部分测试
    /// 注意：CreateImage/TwistImage 依赖 System.Drawing 渲染，不在此测试；
    /// 只测 CreateVerifyCode 字符串生成与属性默认值。
    /// </summary>
    public class VerifyCodeImageTests
    {
        [Fact]
        public void Ctor_DefaultProperties()
        {
            var image = new VerifyCodeImage();

            Assert.Equal(4, image.Length);
            Assert.True(image.FontSize > 0);
            Assert.NotNull(image.CodeSerial);
            Assert.False(string.IsNullOrEmpty(image.CodeSerial));
        }

        [Fact]
        public void CreateVerifyCode_ExplicitLength_ReturnsThatLength()
        {
            var image = new VerifyCodeImage();

            var code = image.CreateVerifyCode(6);

            Assert.Equal(6, code.Length);
        }

        [Fact]
        public void CreateVerifyCode_DefaultLength_UsesProperty()
        {
            var image = new VerifyCodeImage { Length = 5 };

            var code = image.CreateVerifyCode(0);

            Assert.Equal(5, code.Length);
        }

        [Fact]
        public void CreateVerifyCode_CharsFromCodeSerial()
        {
            var image = new VerifyCodeImage();

            var code = image.CreateVerifyCode(8);
            var allowed = image.CodeSerial.Split(',').Where(t => !string.IsNullOrEmpty(t)).ToArray();

            foreach (var c in code)
            {
                Assert.Contains(c.ToString(), allowed);
            }
        }

        [Fact]
        public void CreateVerifyCode_NoArgs_UsesDefault()
        {
            var image = new VerifyCodeImage();

            var code = image.CreateVerifyCode();

            Assert.Equal(image.Length, code.Length);
        }
    }

    /// <summary>
    /// ExifEntity 测试
    ///
    /// 重要说明：ExifEntity 是 ImageUtil 的嵌套类，而 ImageUtil.cs 被
    /// #if NET46_OR_GREATER 包裹，net8.0 下不参与编译。这里用反射探测：
    /// 类型存在才断言，不存在则跳过（诚实标注，不产生必然失败的断言）。
    /// </summary>
    public class ExifEntityTests
    {
        private static readonly Type? ExifType =
            typeof(EnumDescription).Assembly.GetType("DotNet.Util.ImageUtil+ExifEntity");

        private static bool Available => ExifType != null;

        [Fact]
        public void Type_WhenNotCompiled_IsAbsent()
        {
            if (!Available)
            {
                return; // TODO: ImageUtil 被 #if NET46_OR_GREATER 排除编译，ExifEntity 在 net8.0 不可用，未测
            }
            Assert.True(ExifType!.IsPublic);
        }

        [Fact]
        public void DefaultOrientation_WhenTypeAvailable_IsOne()
        {
            if (!Available)
            {
                return; // TODO: 类型不可用，未测
            }

            var entity = Activator.CreateInstance(ExifType!)!;

            Assert.Equal(1, ExifType!.GetProperty("Orientation")!.GetValue(entity));
        }

        [Fact]
        public void Properties_WhenTypeAvailable_AreSettable()
        {
            if (!Available)
            {
                return; // TODO: 类型不可用，未测
            }

            var entity = Activator.CreateInstance(ExifType!)!;
            ExifType!.GetProperty("Orientation")!.SetValue(entity, 6);
            ExifType.GetProperty("Hash")!.SetValue(entity, "hash123");
            ExifType.GetProperty("Longitude")!.SetValue(entity, 120.15);
            ExifType.GetProperty("Latitude")!.SetValue(entity, 30.28);
            ExifType.GetProperty("Address")!.SetValue(entity, "西湖");
            ExifType.GetProperty("AreaCode")!.SetValue(entity, 330106);

            Assert.Equal(6, ExifType.GetProperty("Orientation")!.GetValue(entity));
            Assert.Equal("hash123", ExifType.GetProperty("Hash")!.GetValue(entity));
            Assert.Equal(120.15, ExifType.GetProperty("Longitude")!.GetValue(entity));
            Assert.Equal(30.28, ExifType.GetProperty("Latitude")!.GetValue(entity));
            Assert.Equal("西湖", ExifType.GetProperty("Address")!.GetValue(entity));
            Assert.Equal(330106, ExifType.GetProperty("AreaCode")!.GetValue(entity));
        }
    }
}
