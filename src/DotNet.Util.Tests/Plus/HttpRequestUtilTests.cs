using System.Net;
using System.Net.Sockets;
using System.Text;
using DotNet.Util;
using Xunit;

namespace DotNet.Util.Tests.Plus
{
    /// <summary>
    /// HttpRequestUtil 测试：本地 HttpListener 回显服务器
    /// </summary>
    public class HttpRequestUtilTests : IDisposable
    {
        private readonly HttpListener _listener;
        private readonly string _baseUrl;
        private readonly CancellationTokenSource _cts = new();

        public HttpRequestUtilTests()
        {
            var port = GetFreePort();
            // 用 127.0.0.1 而非 localhost，避免 IPv6(::1) 优先探测导致的连接回退延迟
            _baseUrl = $"http://127.0.0.1:{port}/";
            _listener = new HttpListener();
            _listener.Prefixes.Add(_baseUrl);
            _listener.Start();
            _ = Task.Run(() => Serve(_cts.Token));
        }

        public void Dispose()
        {
            _cts.Cancel();
            _listener.Stop();
            _listener.Close();
        }

        private static int GetFreePort()
        {
            var tcp = new TcpListener(IPAddress.Loopback, 0);
            tcp.Start();
            var port = ((IPEndPoint)tcp.LocalEndpoint).Port;
#if NET8_0_OR_GREATER
            tcp.Dispose();
#endif
            return port;
        }

        private void Serve(CancellationToken token)
        {
            while (!token.IsCancellationRequested && _listener.IsListening)
            {
                try
                {
                    var ctx = _listener.GetContext();
                    using var reader = new StreamReader(ctx.Request.InputStream, ctx.Request.ContentEncoding ?? Encoding.UTF8);
                    var body = reader.ReadToEnd();
                    var response = "echo:" + body;
                    var bytes = Encoding.UTF8.GetBytes(response);
                    ctx.Response.StatusCode = 200;
                    ctx.Response.ContentType = "text/plain";
                    ctx.Response.ContentLength64 = bytes.Length;
                    ctx.Response.OutputStream.Write(bytes, 0, bytes.Length);
                    ctx.Response.Close();
                }
                catch
                {
                    // 监听停止时忽略
                }
            }
        }

        [Fact]
        public void WcGet_ReturnsBody()
        {
            var result = HttpRequestUtil.WcGet(_baseUrl + "get");
            Assert.Equal("echo:", result);
        }

        [Fact]
        public void WcPost_ReturnsBody()
        {
            var result = HttpRequestUtil.WcPost(_baseUrl + "post", "hello");
            Assert.Equal("echo:hello", result);
        }

        [Fact]
        public void HwGet_ReturnsBody()
        {
            var result = HttpRequestUtil.HwGet(_baseUrl + "hwget");
            Assert.Equal("echo:", result);
        }

        [Fact]
        public void HwPost_ReturnsBody()
        {
            var result = HttpRequestUtil.HwPost(_baseUrl + "hwpost", "data");
            Assert.Equal("echo:data", result);
        }
    }
}
