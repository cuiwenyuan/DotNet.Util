//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2024, DotNet.
//-----------------------------------------------------------------

using System;

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
        public string message { get; set; }
        /// <summary>
        /// 服务器返回的消息
        /// </summary>
        [Obsolete("Pleaes use message from 2024-06-01")]
        public string msg { get; set; }
        /// <summary>
        /// 服务器返回的数据
        /// </summary>
        public object data { get; set; }

        #region 动态方法

        /// <summary>
        /// 设置成功消息
        /// </summary>
        /// <param name="returnMessage">返回消息</param>
        /// <param name="returnData">返回数据</param>
        /// <returns></returns>
        public string Success(string returnMessage, object returnData = null)
        {
            status = 1;
            message = returnMessage;
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
            message = returnMessage;
            msg = returnMessage;
            data = returnData;
            return JsonUtil.ObjectToJson(this);
        }

        #endregion

        #region 静态方法

        /// <summary>
        /// Ashx返回结果
        /// </summary>
        public AshxResult()
        {

        }
        /// <summary>
        /// Ashx返回结果
        /// </summary>
        /// <param name="status"></param>
        /// <param name="message"></param>
        /// <param name="data"></param>
        public AshxResult(int status, string message, object data)
        {
            this.status = status;
            this.message = message;
            this.msg = message;
            this.data = data;
        }

        /// <summary>
        /// 设置成功结果
        /// </summary>
        /// <param name="returnMessage">返回消息</param>
        /// <param name="returnData">返回数据</param>
        /// <returns></returns>
        public static string Ok(string returnMessage, object returnData = null)
        {
            return JsonUtil.ObjectToJson(new AshxResult(1, returnMessage, returnData));
        }
        /// <summary>
        /// 设置失败结果
        /// </summary>
        /// <param name="returnMessage">返回消息</param>
        /// <param name="returnData">返回数据</param>
        /// <returns></returns>
        public static string Ng(string returnMessage, object returnData = null)
        {
            return JsonUtil.ObjectToJson(new AshxResult(0, returnMessage, returnData));
        }

        #endregion
    }
}
