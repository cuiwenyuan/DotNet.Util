//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System;
using System.Collections.Specialized;
using System.Net;
using System.Text;

namespace DotNet.Business
{
    using Model;
    using Util;

    /// <summary>
    /// BaseUserManager
    /// 用户管理
    /// 
    /// 修改记录
    ///
    ///     2020.12.08 版本：1.5 Troy.Cui    使用CacheUtil缓存
    ///		2015.04.23 版本：1.0 JiRiGaLa	主键整理。
    /// 
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2015.04.23</date>
    /// </author> 
    /// </summary>
    public partial class BaseUserManager : BaseManager
    {
        /// <summary>
        /// 离职处理
        /// </summary>
        /// <param name="userEntity"></param>
        /// <param name="userLogonEntity"></param>
        /// <param name="comment"></param>
        /// <returns>影响行数</returns>
        public int Leave(BaseUserEntity userEntity, BaseUserLogonEntity userLogonEntity, string comment)
        {
            var result = 0;

            if (userEntity != null)
            {
                // 更新用户实体
                UpdateEntity(userEntity);
            }

            // 更新登录信息
            if (userLogonEntity != null)
            {
                var userLogonManager = new BaseUserLogonManager(UserInfo);
                userLogonManager.UpdateEntity(userLogonEntity);
            }

            // 2016-03-17 吉日嘎拉 停止吉信的号码
            if (userEntity != null && !string.IsNullOrEmpty(userEntity.NickName))
            {
                //AfterLeaveStopIm(userEntity);
            }

            // 2016-03-17 吉日嘎拉 停止吉信的号码
            if (userEntity != null && userEntity.Id > 0)
            {
                BaseUserContactEntity userContactEntity = null;
                // 2015-12-08 吉日嘎拉 提高效率、从缓存获取数据
                userContactEntity = BaseUserContactManager.GetEntityByCache(userEntity.Id);

                if (userContactEntity != null && !string.IsNullOrEmpty(userContactEntity.CompanyEmail))
                {
                    ChangeUserMailStatus(userContactEntity.CompanyEmail, true);
                }
            }

            return result;
        }

        private string GetStatus(string response)
        {
            /*
             * 返回：
                0 - 更改用户邮箱状态成功；
                1 - 参数不正确或参数格式无效；
                2 - 邮箱用户不存在或读取用户信息失败；
                3 - 更改用户邮箱状态失败。
                */
            var result = "更改用户邮箱状态成功";
            if (response.Contains("1"))
            {
                result = "参数不正确或参数格式无效";
            }
            if (response.Contains("2"))
            {
                result = "邮箱用户不存在或读取用户信息失败";
            }
            if (response.Contains("3"))
            {
                result = "更改用户邮箱状态失败";
            }
            return result;
        }

        /// <summary>
        /// 改变用户邮箱状态
        /// </summary>
        /// <param name="email">公司邮箱</param>
        /// <param name="stop">1：禁止，0：启用</param>
        /// <returns></returns>
        public BaseResult ChangeUserMailStatus(string email, bool stop = true)
        {
            var result = new BaseResult();
            try
            {
                // 停止邮箱内网请求地址：http://192.168.0.201:6080/roperate.php?optcmd=modifyuserstatus&user=xumingyue&domain=wangcaisoft.com&status=1
                // 开启邮箱内网请求地址：http://192.168.0.201:6080/roperate.php?optcmd=modifyuserstatus&user=xumingyue&domain=wangcaisoft.com&status=0
                if (!string.IsNullOrEmpty(email))
                {
                    var array = email.Split('@');
                    var status = stop ? 1 : 0;
                    // 1：表示停止，0：表示启用
                    var requestUrl = string.Format("http://192.168.0.201:6080/roperate.php?optcmd=modifyuserstatus&user={0}&domain={1}&status={2}", array[0], array[1], status);
                    var webClient = new WebClient();
                    var responseArray = webClient.UploadValues(requestUrl, new NameValueCollection());
                    var response = Encoding.UTF8.GetString(responseArray);
                    if (!string.IsNullOrEmpty(response))
                    {
                        if (response.ToUpper() == "OK")
                        {
                            result.Status = true;
                            result.StatusCode = "success";
                            result.StatusMessage = string.Format("{0}用户邮箱状态成功", stop ? "禁止" : "启用");
                            result.RecordCount = 1;
                        }
                        else
                        {
                            result.Status = false;
                            result.StatusCode = "failed";
                            result.StatusMessage = GetStatus(response);
                            result.RecordCount = 0;
                        }
                    }
                    else
                    {
                        result.Status = false;
                        result.StatusCode = "failed";
                        result.StatusMessage = "返回值为空";
                        result.RecordCount = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                var writeMessage = "BaseUserManager.AfterLeave:发生时间:" + DateTime.Now
                     + Environment.NewLine + "Message:" + ex.Message
                     + Environment.NewLine + "Source:" + ex.Source
                     + Environment.NewLine + "StackTrace:" + ex.StackTrace
                     + Environment.NewLine + "TargetSite:" + ex.TargetSite
                     + Environment.NewLine;

                LogUtil.WriteLog(writeMessage, "Exception");

                result.Status = false;
                result.StatusCode = "failed";
                result.StatusMessage = ex.Source;
                result.RecordCount = 0;
            }

            return result;
        }

        //public bool AfterLeaveStopIm(BaseUserEntity userEntity)
        //{
        //    var result = false;

        //    // 2016-03-17 吉日嘎拉 停止吉信的号码
        //    if (userEntity != null && !string.IsNullOrEmpty(userEntity.NickName))
        //    {
        //        //{"a":"structure-delete-user","v":0.1,"t":"loginname"}
        //        //返回：{"ret":0} 表示成功
        //        try
        //        {
        //            var url = "http://jixin.wangcaisoft.com:8280/mng/im.service";
        //            var webClient = new WebClient();
        //            var postValues = new NameValueCollection();
        //            var ht = new Hashtable();
        //            ht.Add("a", "structure-delete-user");
        //            ht.Add("v", "0.1");
        //            ht.Add("t", userEntity.NickName);
        //            var data = new JavaScriptSerializer().Serialize(ht);
        //            data = SecretUtil.EncodeBase64("utf-8", data);
        //            postValues.Add("data", data);
        //            var responseArray = webClient.UploadValues(url, postValues);
        //            data = Encoding.UTF8.GetString(responseArray);
        //            data = SecretUtil.DecodeBase64("utf-8", data);
        //            var o = JObject.Parse(data);
        //            var jToken = o["ret"];
        //            if (string.Equals("0", jToken.ToString(), StringComparison.OrdinalIgnoreCase))
        //            {
        //                return true;
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            var writeMessage = "BaseUserManager.AfterLeaveStopIM:发生时间:" + DateTime.Now
        //                + Environment.NewLine + "Message:" + ex.Message
        //                + Environment.NewLine + "Source:" + ex.Source
        //                + Environment.NewLine + "StackTrace:" + ex.StackTrace
        //                + Environment.NewLine + "TargetSite:" + ex.TargetSite
        //                + Environment.NewLine;

        //            LogUtil.WriteLog(writeMessage, "Exception");
        //        }
        //    }

        //    return result;
        //}
    }
}