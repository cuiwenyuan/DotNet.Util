using System.Reflection;
using DotNet.Util;
using Xunit;

namespace DotNet.Util.Tests.Util
{
    /// <summary>
    /// JsonUtil.JsonSplit.CharState 测试（纯逻辑，不依赖外部资源）
    ///
    /// 说明：CharState 是 JsonUtil.JsonSplit 内部的 private 嵌套类，测试项目无法直接引用，
    /// 因此这里用反射驱动其字段与 CheckIsError 方法，并配合公开入口 JsonUtil.IsJson
    /// 做端到端验证。
    /// </summary>
    public class CharStateTests
    {
        private const BindingFlags MemberFlags = BindingFlags.Instance | BindingFlags.NonPublic;

        private static Type ResolveCharStateType()
        {
            var type = typeof(JsonUtil).Assembly.GetType("DotNet.Util.JsonUtil+JsonSplit+CharState");
            Assert.NotNull(type);
            return type!;
        }

        private static object NewCharState() => Activator.CreateInstance(ResolveCharStateType(), true)!;

        private static object? GetField(object cs, string name)
        {
            var field = cs.GetType().GetField(name, MemberFlags);
            Assert.NotNull(field);
            return field!.GetValue(cs);
        }

        private static void SetField(object cs, string name, object value)
        {
            var field = cs.GetType().GetField(name, MemberFlags);
            Assert.NotNull(field);
            field!.SetValue(cs, value);
        }

        private static void CheckIsError(object cs, char c)
        {
            var method = cs.GetType().GetMethod("CheckIsError", MemberFlags);
            Assert.NotNull(method);
            method!.Invoke(cs, new object[] { c });
        }

        private static bool IsError(object cs) => (bool)GetField(cs, "IsError")!;

        #region 字段初始状态

        [Fact]
        public void NewInstance_HasExpectedInitialState()
        {
            var cs = NewCharState();

            Assert.False((bool)GetField(cs, "JsonStart")!);
            Assert.False((bool)GetField(cs, "ArrayStart")!);
            Assert.False((bool)GetField(cs, "ChildrenStart")!);
            Assert.False((bool)GetField(cs, "SetDicValue")!);
            Assert.False((bool)GetField(cs, "EscapeChar")!);
            Assert.False(IsError(cs));

            Assert.Equal(-1, (int)GetField(cs, "State")!);
            Assert.Equal(-1, (int)GetField(cs, "KeyStart")!);
            Assert.Equal(-1, (int)GetField(cs, "ValueStart")!);
        }

        #endregion

        #region CheckIsError

        [Fact]
        public void CheckIsError_OpenBrace_OnFreshState_NoError()
        {
            var cs = NewCharState();

            CheckIsError(cs, '{');

            Assert.False(IsError(cs));
        }

        [Fact]
        public void CheckIsError_OpenBrace_WhenAlreadyInKeyPhase_SetsError()
        {
            // JsonStart 且 State==0（取键阶段）时再遇到 '{'，属于重复开始
            var cs = NewCharState();
            SetField(cs, "JsonStart", true);
            SetField(cs, "State", 0);

            CheckIsError(cs, '{');

            Assert.True(IsError(cs));
        }

        [Fact]
        public void CheckIsError_CloseBrace_WithoutJsonStart_SetsError()
        {
            var cs = NewCharState();

            CheckIsError(cs, '}');

            Assert.True(IsError(cs));
        }

        [Fact]
        public void CheckIsError_CloseBrace_AfterJsonStart_NoError()
        {
            var cs = NewCharState();
            SetField(cs, "JsonStart", true);
            SetField(cs, "State", 1);

            CheckIsError(cs, '}');

            Assert.False(IsError(cs));
        }

        [Fact]
        public void CheckIsError_CloseBracket_DependsOnArrayStart()
        {
            var noArray = NewCharState();
            CheckIsError(noArray, ']');
            Assert.True(IsError(noArray));

            var inArray = NewCharState();
            SetField(inArray, "ArrayStart", true);
            CheckIsError(inArray, ']');
            Assert.False(IsError(inArray));
        }

        [Fact]
        public void CheckIsError_OpenBracket_RepeatedInKeyPhase_SetsError()
        {
            var cs = NewCharState();
            SetField(cs, "ArrayStart", true);
            SetField(cs, "State", 0);

            CheckIsError(cs, '[');

            Assert.True(IsError(cs));
        }

        [Fact]
        public void CheckIsError_Quote_DependsOnJsonStart()
        {
            var outside = NewCharState();
            CheckIsError(outside, '"');
            Assert.True(IsError(outside));

            var inside = NewCharState();
            SetField(inside, "JsonStart", true);
            CheckIsError(inside, '"');
            Assert.False(IsError(inside));

            var single = NewCharState();
            SetField(single, "JsonStart", true);
            CheckIsError(single, '\'');
            Assert.False(IsError(single));
        }

        [Fact]
        public void CheckIsError_Colon_WithoutJsonStart_SetsError()
        {
            var cs = NewCharState();

            CheckIsError(cs, ':');

            Assert.True(IsError(cs));
        }

        [Fact]
        public void CheckIsError_Comma_WithoutJsonAndArray_SetsError()
        {
            var cs = NewCharState();

            CheckIsError(cs, ',');

            Assert.True(IsError(cs));
        }

        [Fact]
        public void CheckIsError_Comma_InsideArray_NoError()
        {
            var cs = NewCharState();
            SetField(cs, "ArrayStart", true);

            CheckIsError(cs, ',');

            Assert.False(IsError(cs));
        }

        [Fact]
        public void CheckIsError_NormalChar_WithoutJsonStart_SetsError()
        {
            var cs = NewCharState();

            CheckIsError(cs, 'a');

            Assert.True(IsError(cs));
        }

        [Fact]
        public void CheckIsError_NormalChar_InsideJson_NoError()
        {
            var cs = NewCharState();
            SetField(cs, "JsonStart", true);
            SetField(cs, "KeyStart", 3);

            CheckIsError(cs, 'a');

            Assert.False(IsError(cs));
        }

        #endregion

        #region 通过公开入口 JsonUtil.IsJson 验证状态机整体行为

        [Fact]
        public void IsJson_ValidObject_ReturnsTrue()
        {
            Assert.True(JsonUtil.IsJson("{}"));
            Assert.True(JsonUtil.IsJson("{\"a\":1}"));
        }

        [Fact]
        public void IsJson_ValidArrayOfObject_ReturnsTrue()
        {
            Assert.True(JsonUtil.IsJson("[{\"a\":1}]"));
        }

        [Fact]
        public void IsJson_NullOrEmptyOrPlainText_ReturnsFalse()
        {
            Assert.False(JsonUtil.IsJson(null!));
            Assert.False(JsonUtil.IsJson(string.Empty));
            Assert.False(JsonUtil.IsJson("abc"));
        }

        [Fact]
        public void IsJson_UnclosedObject_ReturnsFalse()
        {
            Assert.False(JsonUtil.IsJson("{\"a\":1", out var errIndex));
            // 首尾括号不匹配时直接返回，错误位置保持 0
            Assert.Equal(0, errIndex);
        }

        #endregion
    }
}
