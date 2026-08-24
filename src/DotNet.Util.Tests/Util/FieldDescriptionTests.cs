using System.Reflection;
using DotNet.Model;
using Xunit;

namespace DotNet.Util.Tests.Util
{
    /// <summary>
    /// FieldDescription 特性测试
    /// </summary>
    public class FieldDescriptionTests
    {
        private class Customer
        {
            [FieldDescription("客户编号")]
            public string Code = string.Empty;

            [FieldDescription("客户名称", false)]
            public string? Name { get; set; }

            public int Age { get; set; }
        }

        private class BaseEntity
        {
            [FieldDescription("主键")]
            public virtual int Id { get; set; }
        }

        private class DerivedEntity : BaseEntity
        {
            public override int Id { get; set; }
        }

        [Fact]
        public void Ctor_WithTextOnly_NeedLogDefaultsToTrue()
        {
            var attribute = new FieldDescription("客户名称");

            Assert.Equal("客户名称", attribute.Text);
            Assert.True(attribute.NeedLog);
        }

        [Fact]
        public void Ctor_WithNeedLogFalse_KeepsFlag()
        {
            var attribute = new FieldDescription("客户名称", false);

            Assert.Equal("客户名称", attribute.Text);
            Assert.False(attribute.NeedLog);
        }

        [Fact]
        public void Ctor_AcceptsNullOrEmptyText()
        {
            Assert.Null(new FieldDescription(null!).Text);
            Assert.Equal(string.Empty, new FieldDescription(string.Empty).Text);
        }

        [Fact]
        public void IsAttribute_AndMembersAreReadOnly()
        {
            var type = typeof(FieldDescription);

            Assert.True(type.IsSubclassOf(typeof(Attribute)));
            Assert.Null(type.GetProperty(nameof(FieldDescription.Text))!.SetMethod);
            Assert.Null(type.GetProperty(nameof(FieldDescription.NeedLog))!.SetMethod);
        }

        [Fact]
        public void AttributeUsage_TargetsPropertyAndField_AndIsInherited()
        {
            var usage = typeof(FieldDescription).GetCustomAttribute<AttributeUsageAttribute>()!;

            Assert.Equal(AttributeTargets.Property | AttributeTargets.Field, usage.ValidOn);
            Assert.True(usage.Inherited);
            Assert.False(usage.AllowMultiple);
        }

        [Fact]
        public void CanBeReadFromField()
        {
            var field = typeof(Customer).GetField(nameof(Customer.Code))!;

            var attribute = field.GetCustomAttribute<FieldDescription>()!;

            Assert.NotNull(attribute);
            Assert.Equal("客户编号", attribute.Text);
            Assert.True(attribute.NeedLog);
        }

        [Fact]
        public void CanBeReadFromProperty()
        {
            var property = typeof(Customer).GetProperty(nameof(Customer.Name))!;

            var attribute = property.GetCustomAttribute<FieldDescription>()!;

            Assert.NotNull(attribute);
            Assert.Equal("客户名称", attribute.Text);
            Assert.False(attribute.NeedLog);
        }

        [Fact]
        public void MemberWithoutAttribute_ReturnsNull()
        {
            var property = typeof(Customer).GetProperty(nameof(Customer.Age))!;

            Assert.Null(property.GetCustomAttribute<FieldDescription>());
            Assert.False(Attribute.IsDefined(property, typeof(FieldDescription)));
        }

        [Fact]
        public void Inherited_IsHonoredOnOverriddenProperty()
        {
            var property = typeof(DerivedEntity).GetProperty(nameof(BaseEntity.Id))!;

            // Inherited = true，重写属性上通过 inherit:true 依然能取到基类特性
            Assert.NotNull(property.GetCustomAttribute<FieldDescription>(inherit: true));
            Assert.Null(property.GetCustomAttribute<FieldDescription>(inherit: false));
        }

        [Fact]
        public void MultipleMembers_HaveIndependentAttributeInstances()
        {
            var code = typeof(Customer).GetField(nameof(Customer.Code))!.GetCustomAttribute<FieldDescription>()!;
            var name = typeof(Customer).GetProperty(nameof(Customer.Name))!.GetCustomAttribute<FieldDescription>()!;

            Assert.NotSame(code, name);
            Assert.NotEqual(code.Text, name.Text);
        }
    }
}
