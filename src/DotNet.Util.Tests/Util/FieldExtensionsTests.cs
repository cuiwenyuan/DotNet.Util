using DotNet.Model;
using Xunit;

namespace DotNet.Util.Tests.Util
{
    /// <summary>
    /// FieldExtensions 测试
    /// </summary>
    /// <remarks>
    /// FieldExtensions / FieldDescription 位于 DotNet.Model 命名空间（源码在 DotNet.Util 程序集的 Entity 目录下）。
    /// 其中 ToDescription(this Type, string) 被标注为 [Obsolete(..., true)]，编译期即报错，无法编写调用测试。
    /// </remarks>
    public class FieldExtensionsTests
    {
        private class Customer
        {
            [FieldDescription("客户编号")]
            public string Code = string.Empty;

            [FieldDescription("客户名称", false)]
            public string? Name { get; set; }

            public int Age { get; set; }

            public string NoAttributeField = string.Empty;
        }

        private class Empty
        {
        }

        #region GetCustomAttribute

        [Fact]
        public void GetCustomAttribute_OnField_ReturnsAttribute()
        {
            var attribute = typeof(Customer).GetCustomAttribute(nameof(Customer.Code), typeof(FieldDescription));

            var description = Assert.IsType<FieldDescription>(attribute);
            Assert.Equal("客户编号", description.Text);
            Assert.True(description.NeedLog);
        }

        [Fact]
        public void GetCustomAttribute_OnProperty_ReturnsAttribute()
        {
            var attribute = typeof(Customer).GetCustomAttribute(nameof(Customer.Name), typeof(FieldDescription));

            var description = Assert.IsType<FieldDescription>(attribute);
            Assert.Equal("客户名称", description.Text);
            Assert.False(description.NeedLog);
        }

        [Fact]
        public void GetCustomAttribute_MemberWithoutAttribute_ReturnsNull()
        {
            Assert.Null(typeof(Customer).GetCustomAttribute(nameof(Customer.Age), typeof(FieldDescription)));
            Assert.Null(typeof(Customer).GetCustomAttribute(nameof(Customer.NoAttributeField), typeof(FieldDescription)));
        }

        [Fact]
        public void GetCustomAttribute_UnknownMemberName_ReturnsNull()
        {
            Assert.Null(typeof(Customer).GetCustomAttribute("NotExists", typeof(FieldDescription)));
            Assert.Null(typeof(Empty).GetCustomAttribute("Anything", typeof(FieldDescription)));
        }

        [Fact]
        public void GetCustomAttribute_OtherAttributeType_ReturnsNull()
        {
            Assert.Null(typeof(Customer).GetCustomAttribute(nameof(Customer.Code), typeof(ObsoleteAttribute)));
        }

        #endregion

        #region FieldDescription

        [Fact]
        public void FieldDescription_OnField_ReturnsText()
        {
            Assert.Equal("客户编号", typeof(Customer).FieldDescription(nameof(Customer.Code)));
        }

        [Fact]
        public void FieldDescription_OnProperty_ReturnsText()
        {
            Assert.Equal("客户名称", typeof(Customer).FieldDescription(nameof(Customer.Name)));
        }

        [Fact]
        public void FieldDescription_WithoutAttribute_ReturnsEmptyString()
        {
            Assert.Equal(string.Empty, typeof(Customer).FieldDescription(nameof(Customer.Age)));
        }

        [Fact]
        public void FieldDescription_UnknownMemberName_ReturnsEmptyString()
        {
            Assert.Equal(string.Empty, typeof(Customer).FieldDescription("NotExists"));
        }

        #endregion

        #region ToDescription(string)

        [Fact]
        public void ToDescription_KnownStringMemberName_ReturnsInputUnchanged()
        {
            // 实现是 enumeration.GetType()（结果恒为 typeof(string)）再 GetMember(值)，
            // 命中的是 String 类型自身的成员，其上不可能有 FieldDescription，因此永远原样返回
            Assert.Equal("Length", "Length".ToDescription());
            Assert.Equal("Empty", "Empty".ToDescription());
        }

        [Fact]
        public void ToDescription_ArbitraryString_ReturnsInputUnchanged()
        {
            Assert.Equal("客户名称", "客户名称".ToDescription());
            Assert.Equal("Enabled", "Enabled".ToDescription());
        }

        [Fact]
        public void ToDescription_NullString_Throws()
        {
            string? value = null;

            // 扩展方法内部直接调用 GetType()，未做空校验
            Assert.Throws<NullReferenceException>(() => value!.ToDescription());
        }

        #endregion

        [Fact]
        public void FieldExtensions_IsStaticPartialClassInDotNetModelNamespace()
        {
            var type = typeof(FieldExtensions);

            Assert.True(type.IsAbstract && type.IsSealed);
            Assert.Equal("DotNet.Model", type.Namespace);
        }
    }
}
