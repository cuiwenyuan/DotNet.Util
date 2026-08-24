using DotNet.Util;
using Xunit;

namespace DotNet.Util.Tests.Util
{
    /// <summary>
    /// DisposeHelper.TryDispose 测试（纯逻辑，不依赖外部资源）
    /// </summary>
    public class DisposeHelperTests
    {
        private sealed class FakeDisposable : IDisposable
        {
            public int DisposeCount { get; private set; }

            public bool Disposed => DisposeCount > 0;

            public void Dispose() => DisposeCount++;
        }

        private sealed class PlainObject
        {
            public int Value { get; set; }
        }

        [Fact]
        public void TryDispose_Null_ReturnsNull()
        {
            object? obj = null;

            Assert.Null(obj.TryDispose());
        }

        [Fact]
        public void TryDispose_Disposable_DisposesAndReturnsSameInstance()
        {
            var item = new FakeDisposable();

            var result = item.TryDispose();

            Assert.True(item.Disposed);
            Assert.Equal(1, item.DisposeCount);
            Assert.Same(item, result);
        }

        [Fact]
        public void TryDispose_NonDisposable_ReturnsSameInstanceWithoutThrowing()
        {
            var obj = new PlainObject { Value = 7 };

            var result = obj.TryDispose();

            Assert.Same(obj, result);
            Assert.Equal(7, ((PlainObject)result!).Value);
        }

        [Fact]
        public void TryDispose_ListOfDisposables_DisposesEveryItem()
        {
            var a = new FakeDisposable();
            var b = new FakeDisposable();
            var list = new List<FakeDisposable> { a, b };

            var result = list.TryDispose();

            Assert.True(a.Disposed);
            Assert.True(b.Disposed);
            Assert.Same(list, result);
        }

        [Fact]
        public void TryDispose_ArrayOfDisposables_DisposesEveryItem()
        {
            var a = new FakeDisposable();
            var b = new FakeDisposable();
            var array = new[] { a, b };

            array.TryDispose();

            Assert.Equal(1, a.DisposeCount);
            Assert.Equal(1, b.DisposeCount);
        }

        [Fact]
        public void TryDispose_NonListEnumerable_DisposesEveryItem()
        {
            // HashSet 是 IEnumerable 但不是 IList，走"先收集再逐个销毁"分支
            var a = new FakeDisposable();
            var b = new FakeDisposable();
            var set = new HashSet<FakeDisposable> { a, b };

            set.TryDispose();

            Assert.True(a.Disposed);
            Assert.True(b.Disposed);
        }

        [Fact]
        public void TryDispose_EmptyCollection_DoesNotThrow()
        {
            var list = new List<FakeDisposable>();

            Assert.Same(list, list.TryDispose());
        }

        [Fact]
        public void TryDispose_String_ReturnsSameStringWithoutThrowing()
        {
            // 字符串是 IEnumerable<char>，但元素不是 IDisposable，应原样返回
            const string text = "abc";

            Assert.Same(text, text.TryDispose());
        }

        [Fact]
        public void TryDispose_CalledTwice_DisposesTwice()
        {
            // TryDispose 本身不做幂等保护，是否幂等取决于对象自身
            var item = new FakeDisposable();

            item.TryDispose();
            item.TryDispose();

            Assert.Equal(2, item.DisposeCount);
        }

        // TODO: Dispose 抛异常的分支会走 LogUtil.WriteLog 写日志（需文件系统等外部资源），未测。
    }
}
