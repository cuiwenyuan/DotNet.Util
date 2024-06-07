using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace DotNet.Util
{
    /// <summary>
    /// 参考力软
    /// Troy Cui 2017.12.19
    /// </summary>
    public static partial class HttpUtil
    {
        #region POST
        /// <summary>
        /// HTTP POST方式请求数据
        /// </summary>
        /// <param name="url">URL.</param>
        /// <param name="param">POST的数据</param>
        /// <returns></returns>
        public static string Post(string url, string param = null)
        {
            HttpWebRequest request;

            //如果是发送HTTPS请求  
            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                request = WebRequest.Create(url) as HttpWebRequest;
                request.ProtocolVersion = HttpVersion.Version10;
            }
            else
            {
                request = WebRequest.Create(url) as HttpWebRequest;
            }

            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.Accept = "*/*";
            request.Timeout = 15000;
            request.AllowAutoRedirect = false;



            StreamWriter sw = null;
            WebResponse response = null;
            string responseStr = null;

            try
            {
                sw = new StreamWriter(request.GetRequestStream());
                sw.Write(param);
                sw.Close();

                response = request.GetResponse();
                if (response != null)
                {
                    var sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                    responseStr = sr.ReadToEnd();
                    sr.Close();
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                request = null;
                sw = null;
                response = null;
            }

            return responseStr;
        }


        private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true; //总是接受  
        }
        /// <summary>
        /// 创建请求
        /// </summary>
        /// <param name="strUrl"></param>
        /// <param name="dicPara"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string BuildRequest(string strUrl, Dictionary<string, string> dicPara, string fileName)
        {
            var contentType = "image/jpeg";
            //待请求参数数组
            var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            var PicByte = new byte[fs.Length];
            fs.Read(PicByte, 0, PicByte.Length);
            var lengthFile = PicByte.Length;

            //构造请求地址

            //设置HttpWebRequest基本信息
            var request = (HttpWebRequest)HttpWebRequest.Create(strUrl);
            //设置请求方式：get、post
            request.Method = "POST";
            //设置boundaryValue
            var boundaryValue = DateTime.Now.Ticks.ToString("x");
            var boundary = "--" + boundaryValue;
            request.ContentType = "\r\nmultipart/form-data; boundary=" + boundaryValue;
            //设置KeepAlive
            request.KeepAlive = true;
            //设置请求数据，拼接成字符串
            var sbHtml = PoolUtil.StringBuilder.Get();
            foreach (var key in dicPara)
            {
                sbHtml.Append(boundary + "\r\nContent-Disposition: form-data; name=\"" + key.Key + "\"\r\n\r\n" + key.Value + "\r\n");
            }
            sbHtml.Append(boundary + "\r\nContent-Disposition: form-data; name=\"pic\"; filename=\"");
            sbHtml.Append(fileName);
            sbHtml.Append("\"\r\nContent-Type: " + contentType + "\r\n\r\n");
            var postHeader = sbHtml.Return();
            //将请求数据字符串类型根据编码格式转换成字节流
            var code = Encoding.GetEncoding("UTF-8");
            var postHeaderBytes = code.GetBytes(postHeader);
            var boundayBytes = Encoding.ASCII.GetBytes("\r\n" + boundary + "--\r\n");
            //设置长度
            long length = postHeaderBytes.Length + lengthFile + boundayBytes.Length;
            request.ContentLength = length;

            //请求远程HTTP
            var rs = request.GetRequestStream();
            Stream s = null;
            try
            {
                //发送数据请求服务器
                rs.Write(postHeaderBytes, 0, postHeaderBytes.Length);
                rs.Write(PicByte, 0, lengthFile);
                rs.Write(boundayBytes, 0, boundayBytes.Length);
                var HttpWResp = (HttpWebResponse)request.GetResponse();
                s = HttpWResp.GetResponseStream();
            }
            catch //(WebException e)
            {
                //LogResult(e.Message);
                return "";
            }
            finally
            {
                if (rs != null)
                {
                    rs.Close();
                }
            }

            //读取处理结果
            var sr = new StreamReader(s, code);
            var responseData = PoolUtil.StringBuilder.Get();

            String line;
            while ((line = sr.ReadLine()) != null)
            {
                responseData.Append(line);
            }
            s.Close();
            fs.Close();

            return responseData.Return();
        }
        #endregion

        #region Put
        /// <summary>
        /// HTTP Put方式请求数据.
        /// </summary>
        /// <param name="url">URL.</param>
        /// <param name="param">参数</param>
        /// <returns></returns>
        public static string Put(string url, string param = null)
        {
            HttpWebRequest request;

            //如果是发送HTTPS请求  
            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                request = WebRequest.Create(url) as HttpWebRequest;
                request.ProtocolVersion = HttpVersion.Version10;
            }
            else
            {
                request = WebRequest.Create(url) as HttpWebRequest;
            }
            request.Method = "PUT";
            request.ContentType = "application/x-www-form-urlencoded";
            request.Accept = "*/*";
            request.Timeout = 15000;
            request.AllowAutoRedirect = false;

            StreamWriter sw = null;
            WebResponse response = null;
            string responseStr = null;

            try
            {
                sw = new StreamWriter(request.GetRequestStream());
                sw.Write(param);
                sw.Close();

                response = request.GetResponse();
                if (response != null)
                {
                    var sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                    responseStr = sr.ReadToEnd();
                    sr.Close();
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                request = null;
                sw = null;
                response = null;
            }

            return responseStr;
        }
        #endregion

        #region Delete
        /// <summary>
        /// HTTP Delete方式请求数据.
        /// </summary>
        /// <param name="url">URL.</param>
        /// <param name="param">参数</param>
        /// <returns></returns>
        public static string Delete(string url, string param = null)
        {
            HttpWebRequest request;

            //如果是发送HTTPS请求  
            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                request = WebRequest.Create(url) as HttpWebRequest;
                request.ProtocolVersion = HttpVersion.Version10;
            }
            else
            {
                request = WebRequest.Create(url) as HttpWebRequest;
            }
            request.Method = "Delete";
            request.ContentType = "application/x-www-form-urlencoded";
            request.Accept = "*/*";
            request.Timeout = 15000;
            request.AllowAutoRedirect = false;

            StreamWriter sw = null;
            WebResponse response = null;
            string responseStr = null;

            try
            {
                sw = new StreamWriter(request.GetRequestStream());
                sw.Write(param);
                sw.Close();

                response = request.GetResponse();
                if (response != null)
                {
                    var sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                    responseStr = sr.ReadToEnd();
                    sr.Close();
                }
            }
            catch (Exception)
            {
                throw;
            }
            return responseStr;
        }
        #endregion

        #region Get
        /// <summary>
        /// HTTP GET方式请求数据.
        /// </summary>
        /// <param name="url">URL.</param>
        /// <param name="headht">Hashtable</param>
        /// <returns></returns>
        public static string Get(string url, Hashtable headht = null)
        {
            HttpWebRequest request;

            //如果是发送HTTPS请求  
            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                request = WebRequest.Create(url) as HttpWebRequest;
                request.ProtocolVersion = HttpVersion.Version10;
            }
            else
            {
                request = WebRequest.Create(url) as HttpWebRequest;
            }
            request.Method = "GET";
            //request.ContentType = "application/x-www-form-urlencoded";
            request.Accept = "*/*";
            request.Timeout = 15000;
            request.AllowAutoRedirect = false;
            WebResponse response = null;
            string responseStr = null;
            if (headht != null)
            {
                foreach (DictionaryEntry item in headht)
                {
                    request.Headers.Add(item.Key.ToString(), item.Value.ToString());
                }
            }

            try
            {
                response = request.GetResponse();

                if (response != null)
                {
                    var sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                    responseStr = sr.ReadToEnd();
                    sr.Close();
                }
            }
            catch (Exception)
            {
                throw;
            }
            return responseStr;
        }
        /// <summary>
        /// Get方法
        /// </summary>
        /// <param name="url"></param>
        /// <param name="encodeing"></param>
        /// <param name="headht"></param>
        /// <returns></returns>
        public static string Get(string url, Encoding encodeing, Hashtable headht = null)
        {
            HttpWebRequest request;

            //如果是发送HTTPS请求  
            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                request = WebRequest.Create(url) as HttpWebRequest;
                request.ProtocolVersion = HttpVersion.Version10;
            }
            else
            {
                request = WebRequest.Create(url) as HttpWebRequest;
            }
            request.Method = "GET";
            //request.ContentType = "application/x-www-form-urlencoded";
            request.Accept = "*/*";
            request.Timeout = 15000;
            request.AllowAutoRedirect = false;
            WebResponse response = null;
            string responseStr = null;
            if (headht != null)
            {
                foreach (DictionaryEntry item in headht)
                {
                    request.Headers.Add(item.Key.ToString(), item.Value.ToString());
                }
            }

            try
            {
                response = request.GetResponse();

                if (response != null)
                {
                    var sr = new StreamReader(response.GetResponseStream(), encodeing);
                    responseStr = sr.ReadToEnd();
                    sr.Close();
                }
            }
            catch (Exception)
            {
                throw;
            }
            return responseStr;
        }
        #endregion

        #region Post With Pic
        /// <summary>
        /// HTTP POST方式请求数据(带图片)
        /// </summary>
        /// <param name="url">URL</param>        
        /// <param name="param">POST的数据</param>
        /// <param name="fileByte">图片</param>
        /// <returns></returns>
        public static string Post(string url, IDictionary<object, object> param, byte[] fileByte)
        {
            var boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
            var boundarybytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");

            var wr = (HttpWebRequest)WebRequest.Create(url);
            wr.ContentType = "multipart/form-data; boundary=" + boundary;
            wr.Method = "POST";
            wr.KeepAlive = true;
            wr.Credentials = System.Net.CredentialCache.DefaultCredentials;

            var rs = wr.GetRequestStream();
            string responseStr = null;

            var formdataTemplate = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";
            foreach (string key in param.Keys)
            {
                rs.Write(boundarybytes, 0, boundarybytes.Length);
                var formitem = string.Format(formdataTemplate, key, param[key]);
                var formitembytes = System.Text.Encoding.UTF8.GetBytes(formitem);
                rs.Write(formitembytes, 0, formitembytes.Length);
            }
            rs.Write(boundarybytes, 0, boundarybytes.Length);

            var headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n";
            var header = string.Format(headerTemplate, "pic", fileByte, "text/plain");//image/jpeg
            var headerbytes = System.Text.Encoding.UTF8.GetBytes(header);
            rs.Write(headerbytes, 0, headerbytes.Length);

            rs.Write(fileByte, 0, fileByte.Length);

            var trailer = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
            rs.Write(trailer, 0, trailer.Length);
            rs.Close();

            WebResponse wresp = null;
            try
            {
                wresp = wr.GetResponse();
                var stream2 = wresp.GetResponseStream();
                var sr = new StreamReader(stream2);
                responseStr = sr.ReadToEnd();
                // logger.Error(string.Format("File uploaded, server response is: {0}", responseStr));
            }
            catch //(Exception ex)
            {
                //logger.Error("Error uploading file", ex);
                if (wresp != null)
                {
                    wresp.Close();
                    wresp = null;
                }
                throw;
            }
            return responseStr;
        }
        #endregion

        #region 下载图片 DownloadPicture
        
        /// <summary>
        /// 下载图片
        /// </summary>
        /// <param name="pictureUrl">图片Http地址</param>
        /// <param name="filePath">保存路径</param>
        /// <param name="folder">目录(前后不带/)</param>
        /// <param name="fileName"></param>
        /// <param name="fileExtension">文件后缀(以.开头)</param>
        /// <param name="timeOut">Request最大请求时间，如果为-1则无限制</param>
        /// <returns></returns>
        public static bool DownloadPicture(string pictureUrl, out string filePath, string folder = "WeChat", string fileName = null, string fileExtension = ".png", int timeOut = -1)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                fileName = DateTime.Now.ToString("yyyyMMddHHmmssffff");
            }
            filePath = AppDomain.CurrentDomain.BaseDirectory + "\\" + folder + "\\" + fileName + fileExtension;
            //是否存在文件夹,没存在则创建
            if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "\\" + folder + "\\"))
            {
                Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "\\" + folder + "\\");
            }
            var result = false;
            WebResponse response = null;
            Stream stream = null;
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(pictureUrl);
                if (timeOut != -1) request.Timeout = timeOut;
                response = request.GetResponse();
                stream = response.GetResponseStream();
                if (!response.ContentType.ToLower().StartsWith("text/"))
                    result = SaveBinaryFile(response, filePath);
            }
            catch (Exception e)
            {
                LogUtil.WriteException(e);
            }
            finally
            {
                stream?.Close();
                response?.Close();
            }
            return result;
        }
        private static bool SaveBinaryFile(WebResponse response, string savePath)
        {
            var result = false;
            var buffer = new byte[1024];
            Stream outStream = null;
            Stream inStream = null;
            try
            {
                if (File.Exists(savePath)) File.Delete(savePath);
                outStream = File.Create(savePath);
                inStream = response.GetResponseStream();
                int l;
                do
                {
                    l = inStream.Read(buffer, 0, buffer.Length);
                    if (l > 0) outStream.Write(buffer, 0, l);
                } while (l > 0);
                result = true;
            }
            finally
            {
                outStream?.Close();
                inStream?.Close();
            }
            return result;
        }

        #endregion

#if NETSTANDARD2_0_OR_GREATER
        /// <summary>
        /// HttpPostAsync
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postData"></param>
        /// <param name="contentType"></param>
        /// <param name="timeOut"></param>
        /// <param name="headers"></param>
        /// <returns></returns>
        public static Task<string> HttpPostAsync(string url, string postData = null, string contentType = null, int timeOut = 30, Dictionary<string, string> headers = null)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            if (!string.IsNullOrEmpty(contentType))
            {
                request.ContentType = contentType;
            }
            if (headers != null)
            {
                foreach (var header in headers)
                    request.Headers[header.Key] = header.Value;
            }

            try
            {
                var bytes = Encoding.UTF8.GetBytes(postData ?? "");
                using (var sendStream = request.GetRequestStream())
                {
                    sendStream.Write(bytes, 0, bytes.Length);
                }

                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    var responseStream = response.GetResponseStream();
                    var sr = new StreamReader(responseStream, Encoding.UTF8);
                    return sr.ReadToEndAsync();
                }
            }
            catch (Exception ex)
            {
                return Task.FromResult(ex.Message);
            }

        }
        /// <summary>
        /// HttpGetAsync
        /// </summary>
        /// <param name="url"></param>
        /// <param name="headers"></param>
        /// <returns></returns>
        public static Task<string> HttpGetAsync(string url, Dictionary<string, string> headers = null)
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(url);
                if (headers != null)
                {
                    foreach (var header in headers)
                        request.Headers[header.Key] = header.Value;
                }
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    var responseStream = response.GetResponseStream();
                    var sr = new StreamReader(responseStream, Encoding.UTF8);
                    return sr.ReadToEndAsync();
                }
            }
            catch (Exception ex)
            {
                return Task.FromResult(ex.Message);
            }
        }
#endif
    }
}
