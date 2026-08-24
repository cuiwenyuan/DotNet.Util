using System.ComponentModel.DataAnnotations.Schema;
using DotNet.Util;

using Xunit;

namespace DotNet.Util.Tests.Db
{
    /// <summary>
    /// EntityUtil 琛ㄨ〃杈惧紡娴嬭瘯锛堢函鍙嶅皠/鐗规€э紝涓嶈繛搴擄級
    /// </summary>
    public class EntityUtilTests
    {
        [Table("Base_User")]
        private sealed class AnnotatedEntity
        {
            public int Id { get; set; }
            public string? UserName { get; set; }
        }

        private sealed class PlainEntity
        {
            public int Id { get; set; }
        }

        [Fact]
        public void GetTableExpression_AnnotatedType_UsesTableNameFromAttribute()
        {
            var expr = EntityUtil.GetTableExpression(typeof(AnnotatedEntity));

            Assert.Equal("Base_User", expr.Name);
        }

        [Fact]
        public void GetTableExpression_PlainType_UsesClassName()
        {
            var expr = EntityUtil.GetTableExpression(typeof(PlainEntity));

            Assert.Equal("PlainEntity", expr.Name);
        }

        [Fact]
        public void GetTableExpression_Schema_ReadsFromAttribute()
        {
            var expr = EntityUtil.GetTableExpression(typeof(AnnotatedEntity));

            Assert.Null(expr.Schema);
        }

        [Fact]
        public void GetTableExpression_Columns_MatchPropertyCount()
        {
            var expr = EntityUtil.GetTableExpression(typeof(AnnotatedEntity));

            Assert.Equal(2, expr.Columns.Count);
        }

        [Fact]
        public void GetTableExpression_GenericOverload_ReturnsSameTable()
        {
            var expr = EntityUtil.GetTableExpression(typeof(AnnotatedEntity));
            var expr2 = EntityUtil.GetTableExpression(new AnnotatedEntity());

            Assert.Equal(expr.Name, expr2.Name);
        }

        [Fact]
        public void GetTableExpression_IsCached_ReturnsSameReference()
        {
            var expr1 = EntityUtil.GetTableExpression(typeof(PlainEntity));
            var expr2 = EntityUtil.GetTableExpression(typeof(PlainEntity));

            Assert.True(ReferenceEquals(expr1, expr2));
        }
    }
}
