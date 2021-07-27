//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2020, DotNet.
//-----------------------------------------------------------------

using System;

namespace DotNet.Util
{
    /// <summary>
    /// BaseResult
    ///  
    ///  修改记录
    ///  
    ///  2016.05.12 版本：2.1 JiRiGaLa 增加 Signature 数字签名。
    ///  2016.01.07 版本：2.0 JiRiGaLa 增加 RecordCount。
    ///  2015.11.16 版本：1.1 SongBiao 增加JsonResult<T> 泛型 可以带数据返回。
    ///      2015.09.16 版本：1.1 JiRiGaLa Result 修改为 Status。
    ///      2015.09.15 版本：1.0 JiRiGaLa 添加返回标准定义。
    /// 		
    ///      <author>
    ///          <name>Troy Cui</name>
    ///          <date>2016.05.12</date>
    ///      </author> 
    ///  
    /// </summary>
    [Serializable]
    public class BaseResult
    {
        /// <summary>
        /// 操作是否成功
        /// 2015-09-16 吉日嘎拉 按宋彪建议进行修正
        /// </summary>
        public bool Status = false;

        /// <summary>
        /// 返回值
        /// </summary>
        public string ResultValue = "";

        /// <summary>
        /// 返回状态代码
        /// </summary>
        public string StatusCode = "UnknownError";

        /// <summary>
        /// 返回消息内容
        /// </summary>
        public string StatusMessage = "未知错误";

        /// <summary>
        /// 查询分页数据时返回记录条数用
        /// </summary>
        public int RecordCount = 0;

        /// <summary>
        /// 查询分页数据时返回分页条数用
        /// </summary>
        public int PageCount = 1;

        /// <summary>
        /// 数字签名(防止篡改)
        /// </summary>
        public string Signature = string.Empty;

        /// <summary>
        /// 对登录的用户进行数字签名
        /// </summary>
        /// <param name="userInfo">登录信息</param>
        /// <returns>进行过数字签名的用户登录信息</returns>
        public string CreateSignature(BaseUserInfo userInfo)
        {
            if (userInfo != null)
            {
                if (!string.IsNullOrEmpty(userInfo.Signature))
                {
                    // 需要签名的内容部分
                    var dataToSign = userInfo.Signature + "_"
                        + ResultValue + "_"
                        + Status + "_"
                        + StatusCode + "_"
                        + BaseSystemInfo.SecurityKey + "_";
                    // 进行签名
                    Signature = SecretUtil.Md5(dataToSign);
                }
            }

            return Signature;
        }

        /// <summary>
        /// 对登录的用户进行数字签名
        /// </summary>
        /// <param name="userInfo">登录信息</param>
        /// <returns>进行过数字签名的用户登录信息</returns>
        public bool VerifySignature(BaseUserInfo userInfo)
        {
            var result = false;

            if (userInfo != null)
            {
                if (!string.IsNullOrEmpty(userInfo.Signature))
                {
                    // 需要签名的内容部分
                    var dataToSign = userInfo.Signature + "_"
                        + ResultValue + "_"
                        + Status + "_"
                        + StatusCode + "_"
                        + BaseSystemInfo.SecurityKey + "_";
                    // 进行签名
                    result = Signature == SecretUtil.Md5(dataToSign);
                }
            }

            return result;
        }
    }

    /// <summary>
    /// Json格式带返回数据
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class JsonResult<T> : BaseResult
    {
        /// <summary>
        /// Data
        /// </summary>
        public T Data { get; set; } = default(T);
    }
}