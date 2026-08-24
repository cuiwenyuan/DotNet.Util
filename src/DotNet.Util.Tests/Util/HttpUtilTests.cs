using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;
using DotNet.Util;
using Xunit;

namespace DotNet.Util.Tests.Util
{
    /// <summary>
    /// HttpUtil 测试：本地 HttpListener 回显服务器（复用 HttpRequestUtilTests 模式，127.0.0.1 避免 IPv6 回退延迟）
    /// </summary>
    public class HttpUtilTests : IDisposable
    {
        private readonly HttpListener _listener;
        private readonly string _baseUrl;
        private readonly CancellationTokenSource _cts = new();

        public HttpUtilTests()
        {
            var port = GetFreePort();
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
        public void Get_ReturnsBody()
        {
            var result = HttpUtil.Get(_baseUrl + "get");
            Assert.Equal("echo:", result);
        }

        [Fact]
        public void Get_WithHeaders_ReturnsBody()
        {
            var headers = new Hashtable { { "X-Test", "1" } };
            var result = HttpUtil.Get(_baseUrl + "get", headers);
            Assert.Equal("echo:", result);
        }

        [Fact]
        public void Get_WithEncoding_ReturnsBody()
        {
            var result = HttpUtil.Get(_baseUrl + "get", Encoding.UTF8);
            Assert.Equal("echo:", result);
        }

        [Fact]
        public void Post_ReturnsEchoedBody()
        {
            var result = HttpUtil.Post(_baseUrl + "post", "hello");
            Assert.Equal("echo:hello", result);
        }

        [Fact]
        public void Post_NullParam_ReturnsEcho()
        {
            var result = HttpUtil.Post(_baseUrl + "post", null);
            Assert.Equal("echo:", result);
        }

        [Fact]
        public void Post_Binary_ReturnsEcho()
        {
            var result = HttpUtil.Post(_baseUrl + "post", new Dictionary<object, object> { { "a", "1" } }, new byte[] { 1, 2, 3 });
            Assert.NotNull(result);
        }

        [Fact]
        public void Get_UnreachableUrl_Throws()
        {
            // 指向一个未监听的端口，验证异常传播（不吞异常）
            var url = $"http://127.0.0.1:{GetFreePort()}/nope";
            Assert.ThrowsAny<Exception>(() => HttpUtil.Get(url));
        }

        [Fact]
        public void Post_UnreachableUrl_Throws()
        {
            var url = $"http://127.0.0.1:{GetFreePort()}/nope";
            Assert.ThrowsAny<Exception>(() => HttpUtil.Post(url, "x"));
        }
    }
}
