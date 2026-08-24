using DotNet.Util;
using Xunit;

namespace DotNet.Util.Tests.Util
{
    /// <summary>
    /// DisposeBase 测试（纯逻辑，不依赖外部资源）
    /// </summary>
    public class DisposeBaseTests
    {
        private sealed class TestDisposable : DisposeBase
        {
            public int OverrideCallCount;

            public bool? LastDisposing;

            protected override void Dispose(bool disposing)
            {
                base.Dispose(disposing);

                OverrideCallCount++;
                LastDisposing = disposing;
            }
        }

        [Fact]
        public void Disposed_IsFalseBeforeDispose()
        {
            var obj = new TestDisposable();

            Assert.False(obj.Disposed);
        }

        [Fact]
        public void Dispose_SetsDisposedTrue()
        {
            var obj = new TestDisposable();

            obj.Dispose();

            Assert.True(obj.Disposed);
        }

        [Fact]
        public void Dispose_RaisesOnDisposedOnce()
        {
            var obj = new TestDisposable();
            var raised = 0;
            obj.OnDisposed += (s, e) => raised++;

            obj.Dispose();

            Assert.Equal(1, raised);
        }

        [Fact]
        public void Dispose_EventSenderIsTheInstance()
        {
            var obj = new TestDisposable();
            object? sender = null;
            EventArgs? args = null;
            obj.OnDisposed += (s, e) =>
            {
                sender = s;
                args = e;
            };

            obj.Dispose();

            Assert.Same(obj, sender);
            Assert.Same(EventArgs.Empty, args);
        }

        [Fact]
        public void Dispose_CalledTwice_RaisesEventOnlyOnce()
        {
            // 内部用 Interlocked.CompareExchange 保证只真正释放一次
            var obj = new TestDisposable();
            var raised = 0;
            obj.OnDisposed += (s, e) => raised++;

            obj.Dispose();
            obj.Dispose();

            Assert.Equal(1, raised);
            Assert.True(obj.Disposed);
        }

        [Fact]
        public void Dispose_InvokesDerivedOverrideWithDisposingTrue()
        {
            var obj = new TestDisposable();

            obj.Dispose();

            Assert.Equal(1, obj.OverrideCallCount);
            Assert.True(obj.LastDisposing);
        }

        [Fact]
        public void UsingBlock_DisposesInstance()
        {
            TestDisposable captured;
            using (var obj = new TestDisposable())
            {
                captured = obj;
                Assert.False(obj.Disposed);
            }

            Assert.True(captured.Disposed);
        }

        [Fact]
        public void Instance_ImplementsIDisposable2()
        {
            var obj = new TestDisposable();

            Assert.IsAssignableFrom<IDisposable2>(obj);
            Assert.IsAssignableFrom<IDisposable>(obj);
        }

        [Fact]
        public void NoEventSubscriber_DisposeDoesNotThrow()
        {
            var obj = new TestDisposable();

            obj.Dispose();

            Assert.True(obj.Disposed);
        }
    }
}
