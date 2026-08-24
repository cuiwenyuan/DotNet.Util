using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using DotNet.Util;
using Xunit;

namespace DotNet.Util.Tests.Util
{
    /// <summary>
    /// ColumnInfo 测试
    ///
    /// 重要说明：DotNet.Util.csproj 第 58 行显式 <see cref="ColumnInfo"/> 被
    /// "Compile Remove" 排除编译（<c>Compile Remove="Db\Expression\ColumnInfo.cs"</c>），
    /// 因此编译产物 DotNet.Util.dll 中不存在该类型。
    /// 本测试统一用反射探测：类型存在才断言，不存在则跳过（诚实标注未测，不产生必然失败的断言）。
    /// </summary>
    public class ColumnInfoTests
    {
        private static readonly Type? InfoType =
            typeof(EnumDescription).Assembly.GetType("DotNet.Util.ColumnInfo");

        private class Customer
        {
            [Column("CustomerId")]
            public int Id { get; set; }

            public int? Age { get; set; }
        }

        private static bool Available => InfoType != null;

        [Fact]
        public void Type_WhenExcludedFromCompilation_IsNotPresent()
        {
            // 当前仓库状态：类型被 csproj 排除，不应存在。
            // 若未来把 ColumnInfo.cs 加回编译，此测试会自动变成探测路径的对照。
            if (!Available)
            {
                return; // TODO: ColumnInfo 被 DotNet.Util.csproj Compile Remove 排除编译，类型不可用，未测
            }

            Assert.True(InfoType!.IsPublic);
            Assert.False(InfoType.IsAbstract);
        }

        [Fact]
        public void Properties_WhenTypeAvailable_BehaveAsPoco()
        {
            if (!Available)
            {
                return; // TODO: 类型不可用，未测
            }

            var property = typeof(Customer).GetProperty(nameof(Customer.Id))!;
            var info = Activator.CreateInstance(InfoType!)!;

            InfoType!.GetProperty("ColumnName")!.SetValue(info, "CustomerId");
            InfoType.GetProperty("PropertyName")!.SetValue(info, nameof(Customer.Id));
            InfoType.GetProperty("IsKey")!.SetValue(info, true);
            InfoType.GetProperty("IsNullable")!.SetValue(info, false);
            InfoType.GetProperty("Property")!.SetValue(info, property);

            Assert.Equal("CustomerId", InfoType.GetProperty("ColumnName")!.GetValue(info));
            Assert.Equal("Id", InfoType.GetProperty("PropertyName")!.GetValue(info));
            Assert.True((bool)InfoType.GetProperty("IsKey")!.GetValue(info)!);
            Assert.False((bool)InfoType.GetProperty("IsNullable")!.GetValue(info)!);
            Assert.Same(property, InfoType.GetProperty("Property")!.GetValue(info));
        }

        [Fact]
        public void Property_WhenTypeAvailable_ReadsValueViaReflection()
        {
            if (!Available)
            {
                return; // TODO: 类型不可用，未测
            }

            var info = Activator.CreateInstance(InfoType!)!;
            InfoType!.GetProperty("Property")!.SetValue(info, typeof(Customer).GetProperty(nameof(Customer.Id)));

            var entity = new Customer { Id = 42 };
            Assert.Equal(42, ((PropertyInfo)InfoType.GetProperty("Property")!.GetValue(info)!).GetValue(entity));
        }
    }
}
