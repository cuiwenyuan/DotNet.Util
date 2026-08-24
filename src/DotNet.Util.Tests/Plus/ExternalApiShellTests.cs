using DotNet.Util;
using Xunit;

namespace DotNet.Util.Tests.Plus
{
    /// <summary>
    /// LocationUtil / WeChatMiniProgramUtil / BaiduOcrUtil 外壳测试
    ///
    /// 说明：这三个类的方法 URL 硬编码（聚合数据/百度/微信官方 API），无注入点，
    /// 纯单测只能覆盖：POCO 反序列化、无配置文件/断网时的安全降级路径（返回空/空串）。
    /// 真实 API 调用需网络 + 密钥，属 B 类待提供。
    /// </summary>
    public class ExternalApiShellTests
    {
        #region LocationUtil

        [Fact]
        public void LocationUtil_Objex_Deserializes()
        {
            var json = "{\"resultcode\":\"200\",\"reason\":\"Success\",\"result\":{\"area\":\"浙江\",\"location\":\"杭州\"}}";
            var obj = json.ToObject<LocationUtil.objex>();

            Assert.NotNull(obj);
            Assert.Equal("200", obj!.resultcode);
            Assert.Equal("浙江", obj.result!.area);
            Assert.Equal("杭州", obj.result.location);
        }

        [Fact]
        public void LocationUtil_Obj_Deserializes()
        {
            var json = "{\"data\":[{\"location\":\"浙江省杭州市\"}]}";
            var obj = json.ToObject<LocationUtil.obj>();

            Assert.NotNull(obj);
            Assert.Equal("浙江省杭州市", obj!.data[0].location);
        }

        [Fact]
        public void LocationUtil_GetLocation_NoNetwork_ReturnsEmpty()
        {
            // 无网络/API 不可达时内部 catch 返回空串（不抛异常）
            var result = LocationUtil.GetLocation("0.0.0.0");

            Assert.NotNull(result);
        }

        #endregion

        #region WeChatMiniProgramUtil

        [Fact]
        public void GetAccessToken_NoConfigFile_ReturnsEmpty()
        {
            // 无 ~/xmlconfig/WeChatMiniProgram.config 时 doc 为 null → 返回空 token
            var token = WeChatMiniProgramUtil.GetAccessToken();

            Assert.Equal(string.Empty, token);
        }

        [Fact]
        public void GetQrCode_NoAccessToken_ReturnsEmpty()
        {
            // accessToken 为空时直接返回空串
            var result = WeChatMiniProgramUtil.GetQrCode(1);

            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void AccessToken_Poco_Deserializes()
        {
            var json = "{\"access_token\":\"tok123\",\"expires_in\":\"7200\"}";
            var model = JsonUtil.JsonToObject<WeChatMiniProgramUtil.AccessToken>(json);

            Assert.Equal("tok123", model!.access_token);
            Assert.Equal("7200", model.expires_in);
        }

        #endregion

        #region BaiduOcrUtil

        [Fact]
        public void BaiduAccessToken_Poco_Deserializes()
        {
            var json = "{\"access_token\":\"token_abc\",\"expires_in\":\"2592000\"}";
            var model = JsonUtil.JsonToObject<BaiduOcrUtil.AccessToken>(json);

            Assert.Equal("token_abc", model!.access_token);
            Assert.Equal("2592000", model.expires_in);
        }

        #endregion
    }
}
