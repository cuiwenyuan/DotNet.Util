//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2023, DotNet.
//-----------------------------------------------------------------

namespace DotNet.Util
{
    /// <summary>
    /// Ashx一般处理程序返回实体
    /// </summary>
    public class AshxResult
    {
        /// <summary>
        /// 服务器返回的状态（0为失败1为成功）
        /// </summary>
        public int status { get; set; } = 0;
        /// <summary>
        /// 服务器返回的消息
        /// </summary>
        public string msg { get; set; }
        /// <summary>
        /// 服务器返回的数据
        /// </summary>
        public object data { get; set; }

        /// <summary>
        /// 设置成功消息
        /// </summary>
        /// <param name="returnMessage">返回消息</param>
        /// <param name="returnData">返回数据</param>
        /// <returns></returns>
        public string Success(string returnMessage, object returnData = null)
        {
            status = 1;
            msg = returnMessage;
            data = returnData;
            return JsonUtil.ObjectToJson(this);
        }
        /// <summary>
        /// 设置失败消息
        /// </summary>
        /// <param name="returnMessage">返回消息</param>
        /// <param name="returnData">返回数据</param>
        /// <returns></returns>
        public string Fail(string returnMessage, object returnData = null)
        {
            status = 0;
            msg = returnMessage;
            data = returnData;
            return JsonUtil.ObjectToJson(this);
        }
    }
}
