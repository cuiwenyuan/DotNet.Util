using System.Data;
using DotNet.Util;

using Xunit;

namespace DotNet.Util.Tests.Db
{
    /// <summary>
    /// SqlHelper 绾瓧绗︿覆/鍙傛暟澶勭悊娴嬭瘯锛堜笉杩炲簱锛屾棤缃戠粶/澶栭儴渚濊禆锛?    /// 浠呰鐩栦笉瑙﹀彂鏁版嵁搴撹繛鎺ョ殑瀛楃涓叉嫾鎺ヤ笌鍙傛暟鏋勯€犳柟娉曘€?    /// </summary>
    public class SqlHelperTests
    {
        [Fact]
        public void PlusSign_MultipleValues_JoinedByPlus()
        {
            var helper = new SqlHelper();

            var result = helper.PlusSign("a", "b", "c");

            Assert.Equal("a + b + c", result);
        }

        [Fact]
        public void PlusSign_SingleValue_ReturnsValueOnly()
        {
            var helper = new SqlHelper();

            var result = helper.PlusSign("x");

            Assert.Equal("x", result);
        }

        [Fact]
        public void PlusSign_Empty_ReturnsPlusToken()
        {
            var helper = new SqlHelper();

            var result = helper.PlusSign();

            Assert.Equal(" + ", result);
        }

        [Fact]
        public void GetParameter_PrependsAtSign()
        {
            var helper = new SqlHelper();

            var result = helper.GetParameter("Id");

            Assert.Equal("@Id", result);
        }

        [Fact]
        public void MakeInParam_SetsNameAndValue()
        {
            var helper = new SqlHelper();

            var p = helper.MakeInParam("Name", "Troy");

            Assert.Equal("@Name", p.ParameterName);
            Assert.Equal("Troy", p.Value);
        }

        [Fact]
        public void MakeParameter_WithSize_SetsAllProperties()
        {
            var helper = new SqlHelper();

            var p = helper.MakeParameter("p", "v", DbType.String, 50, ParameterDirection.Input);

            Assert.Equal("p", p.ParameterName);
            Assert.Equal("v", p.Value);
            Assert.Equal(ParameterDirection.Input, p.Direction);
            Assert.Equal(50, p.Size);
        }

        [Fact]
        public void MakeParameter_OutputWithNullValue_DoesNotSetValue()
        {
            var helper = new SqlHelper();

            var p = helper.MakeParameter("p", null, DbType.Int32, 0, ParameterDirection.Output);

            Assert.Equal(ParameterDirection.Output, p.Direction);
            Assert.Null(p.Value);
        }

        [Fact]
        public void CurrentDbType_IsSqlServer()
        {
            var helper = new SqlHelper();

            Assert.Equal(CurrentDbType.SqlServer, helper.CurrentDbType);
        }
    }
}
