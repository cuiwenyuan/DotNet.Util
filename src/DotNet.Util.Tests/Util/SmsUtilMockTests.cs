using System.Net;
using System.Net.Sockets;
using System.Text;
using DotNet.Util;
using Xunit;

namespace DotNet.Util.Tests.Util
{
    /// <summary>
    /// SmsUtil.Aliyun Send mock 测试
    /// 说明：Send 支持显式传入 serviceUrl/accessKeyId/accessKeySecret/signName，可指向本地 HttpListener
    /// 验证：① 请求 URL 携带 Aliyun 签名参数；② 响应 {"Code":"OK"} 时返回 true；③ 错误 Code 返回 false
    /// </summary>
    public class SmsUtilMockTests : IDisposable
    {
        private readonly HttpListener _listener;
        private readonly string _baseUrl;
        private readonly CancellationTokenSource _cts = new();
        private string _lastQuery = string.Empty;
        private string _responseBody = "{\"Code\":\"OK\",\"Message\":\"OK\"}";

        public SmsUtilMockTests()
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
                    _lastQuery = ctx.Request.Url?.Query ?? string.Empty;
                    var bytes = Encoding.UTF8.GetBytes(_responseBody);
                    ctx.Response.StatusCode = 200;
                    ctx.Response.ContentType = "application/json";
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
        public void Send_ValidMobile_OkResponse_ReturnsTrue()
        {
            var result = SmsUtil.Send(out var message, "13800138000", "{\"code\":\"1234\"}", "SMS_000000",
                serviceUrl: _baseUrl, accessKeyId: "AKID", accessKeySecret: "SECRET", signName: "旺财");

            Assert.True(result);
            Assert.Contains("发送成功", message);
        }

        [Fact]
        public void Send_RequestContainsSignatureParams()
        {
            SmsUtil.Send(out _, "13800138000", "{}", "SMS_1",
                serviceUrl: _baseUrl, accessKeyId: "AKID", accessKeySecret: "SECRET", signName: "Sign");

            Assert.Contains("Signature=", _lastQuery);
            Assert.Contains("AccessKeyId=AKID", _lastQuery);
            Assert.Contains("PhoneNumbers=13800138000", _lastQuery);
            Assert.Contains("Action=SendSms", _lastQuery);
        }

        [Fact]
        public void Send_ErrorCode_ReturnsFalse()
        {
            _responseBody = "{\"Code\":\"isv.OUT_OF_SERVICE\",\"Message\":\"业务停机\"}";
            var result = SmsUtil.Send(out var message, "13800138000", "{}", "SMS_1",
                serviceUrl: _baseUrl, accessKeyId: "AKID", accessKeySecret: "SECRET", signName: "Sign");

            Assert.False(result);
            Assert.Contains("业务停机", message);
        }

        [Fact]
        public void Send_InvalidMobile_ReturnsFalse()
        {
            var result = SmsUtil.Send(out var message, "123", "{}", "SMS_1");

            Assert.False(result);
            Assert.Contains("手机号码有误", message);
        }

        [Fact]
        public void Send_NullMobile_ReturnsFalse()
        {
            var result = SmsUtil.Send(out var message, null, "{}", "SMS_1");

            Assert.False(result);
            Assert.Contains("手机号码有误", message);
        }

        [Fact]
        public void Send_ParamObject_SerializesJson()
        {
            SmsUtil.Send(out _, "13800138000", null, "SMS_2",
                serviceUrl: _baseUrl, accessKeyId: "AKID", accessKeySecret: "SECRET", signName: "Sign",
                param: new { code = "8888" });

            Assert.Contains("TemplateParam", _lastQuery);
        }
    }
}
