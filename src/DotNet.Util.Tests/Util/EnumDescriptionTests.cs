using System.Reflection;
using DotNet.Util;
using Xunit;

namespace DotNet.Util.Tests.Util
{
    /// <summary>
    /// EnumDescription 特性测试（纯逻辑，不依赖外部资源）
    /// </summary>
    public class EnumDescriptionTests
    {
        private enum LocalStatus
        {
            [EnumDescription("有描述")]
            WithDescription = 1,

            NoDescription = 2
        }

        [Fact]
        public void Ctor_SetsText()
        {
            var attribute = new EnumDescription("暂停");

            Assert.Equal("暂停", attribute.Text);
        }

        [Fact]
        public void Ctor_NullOrEmptyText_KeptAsIs()
        {
            Assert.Null(new EnumDescription(null!).Text);
            Assert.Equal(string.Empty, new EnumDescription(string.Empty).Text);
        }

        [Fact]
        public void Type_IsAttribute()
        {
            Assert.True(typeof(EnumDescription).IsSubclassOf(typeof(Attribute)));
        }

        [Fact]
        public void Attribute_CanBeReadFromEnumField()
        {
            var field = typeof(LocalStatus).GetField(nameof(LocalStatus.WithDescription));
            Assert.NotNull(field);

            var attribute = field!.GetCustomAttribute<EnumDescription>();

            Assert.NotNull(attribute);
            Assert.Equal("有描述", attribute!.Text);
        }

        [Fact]
        public void Attribute_AbsentOnUnmarkedField()
        {
            var field = typeof(LocalStatus).GetField(nameof(LocalStatus.NoDescription));
            Assert.NotNull(field);
            Assert.Null(field!.GetCustomAttribute<EnumDescription>());
        }

        [Fact]
        public void Attribute_UsedByLibraryEnums()
        {
            var field = typeof(AuditStatus).GetField(nameof(AuditStatus.Pause));
            Assert.NotNull(field);

            var attribute = field!.GetCustomAttribute<EnumDescription>();

            Assert.NotNull(attribute);
            Assert.Equal("暂停", attribute!.Text);
        }
    }
}
