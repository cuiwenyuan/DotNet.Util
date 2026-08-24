using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using DotNet.Util;

using Xunit;

namespace DotNet.Util.Tests.Db
{
    /// <summary>
    /// DbQueryUtil 鍒楀悕瑙ｆ瀽娴嬭瘯锛堢函鍙嶅皠/鐗规€э紝涓嶈繛搴擄級
    /// </summary>
    public class DbQueryUtilTests
    {
        private sealed class Sample
        {
            [Column("user_name")]
            public string? UserName { get; set; }

            public int Age { get; set; }
        }

        [Fact]
        public void GetColumnName_WithColumnAttribute_ReturnsAttributeName()
        {
            var member = typeof(Sample).GetProperty("UserName")!;

            var name = DbQueryUtil.GetColumnName(member);

            Assert.Equal("user_name", name);
        }

        [Fact]
        public void GetColumnName_WithoutColumnAttribute_ReturnsMemberName()
        {
            var member = typeof(Sample).GetProperty("Age")!;

            var name = DbQueryUtil.GetColumnName(member);

            Assert.Equal("Age", name);
        }

        [Fact]
        public void GetColumnName_NullMember_ReturnsWildcard()
        {
            var name = DbQueryUtil.GetColumnName(null!);

            Assert.Equal("*", name);
        }
    }
}
