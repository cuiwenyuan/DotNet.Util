namespace DotNet.Util
{
    /// <summary>
    /// WebResponseContent
    /// </summary>
    public class WebResponseContent
    {
        /// <summary>
        /// WebResponseContent
        /// </summary>
        public WebResponseContent()
        {
        }
        /// <summary>
        /// WebResponseContent
        /// </summary>
        /// <param name="status"></param>
        public WebResponseContent(bool status)
        {
            Status = status;
        }
        /// <summary>
        /// Status
        /// </summary>
        public bool Status { get; set; }
        /// <summary>
        /// Code
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// Message
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// Data
        /// </summary>
        public object Data { get; set; }
        /// <summary>
        /// OK
        /// </summary>
        /// <returns></returns>
        public WebResponseContent OK()
        {
            Status = true;
            return this;
        }
        /// <summary>
        /// Instance
        /// </summary>
        public static WebResponseContent Instance
        {
            get { return new WebResponseContent(); }
        }
        /// <summary>
        /// OK
        /// </summary>
        /// <param name="message"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public WebResponseContent OK(string message = null, object data = null)
        {
            Status = true;
            Message = message;
            Data = data;
            return this;
        }
        /// <summary>
        /// OK
        /// </summary>
        /// <param name="responseType"></param>
        /// <returns></returns>
        public WebResponseContent OK(ResponseType responseType)
        {
            return Set(responseType, true);
        }
        /// <summary>
        /// Error
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public WebResponseContent Error(string message = null)
        {
            Status = false;
            Message = message;
            return this;
        }
        /// <summary>
        /// Error
        /// </summary>
        /// <param name="responseType"></param>
        /// <returns></returns>
        public WebResponseContent Error(ResponseType responseType)
        {
            return Set(responseType, false);
        }
        /// <summary>
        /// Set
        /// </summary>
        /// <param name="responseType"></param>
        /// <returns></returns>
        public WebResponseContent Set(ResponseType responseType)
        {
            bool? b = null;
            return Set(responseType, b);
        }
        /// <summary>
        /// Set
        /// </summary>
        /// <param name="responseType"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public WebResponseContent Set(ResponseType responseType, bool? status)
        {
            return Set(responseType, null, status);
        }
        /// <summary>
        /// Set
        /// </summary>
        /// <param name="responseType"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public WebResponseContent Set(ResponseType responseType, string msg)
        {
            bool? b = null;
            return Set(responseType, msg, b);
        }
        /// <summary>
        /// Set
        /// </summary>
        /// <param name="responseType"></param>
        /// <param name="msg"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public WebResponseContent Set(ResponseType responseType, string msg, bool? status)
        {
            if (status != null)
            {
                Status = (bool)status;
            }
            Code = ((int)responseType).ToString();
            if (!string.IsNullOrEmpty(msg))
            {
                Message = msg;
                return this;
            }
            //Message = responseType.GetMsg();
            return this;
        }
        /// <summary>
        /// ResponseType
        /// </summary>
        public enum ResponseType
        {
            /// <summary>
            /// ServerError
            /// </summary>
            ServerError = 1,
            /// <summary>
            /// LoginExpiration
            /// </summary>
            LoginExpiration = 302,
            /// <summary>
            /// ParametersLack
            /// </summary>
            ParametersLack = 303,
            /// <summary>
            /// TokenExpiration
            /// </summary>
            TokenExpiration,
            /// <summary>
            /// PINError
            /// </summary>
            PINError,
            /// <summary>
            /// NoPermissions
            /// </summary>
            NoPermissions,
            /// <summary>
            /// NoRolePermissions
            /// </summary>
            NoRolePermissions,
            /// <summary>
            /// LoginError
            /// </summary>
            LoginError,
            /// <summary>
            /// AccountLocked
            /// </summary>
            AccountLocked,
            /// <summary>
            /// LoginSuccess
            /// </summary>
            LoginSuccess,
            /// <summary>
            /// SaveSuccess
            /// </summary>
            SaveSuccess,
            /// <summary>
            /// AuditSuccess
            /// </summary>
            AuditSuccess,
            /// <summary>
            /// OperSuccess
            /// </summary>
            OperSuccess,
            /// <summary>
            /// RegisterSuccess
            /// </summary>
            RegisterSuccess,
            /// <summary>
            /// ModifyPwdSuccess
            /// </summary>
            ModifyPwdSuccess,
            /// <summary>
            /// EidtSuccess
            /// </summary>
            EidtSuccess,
            /// <summary>
            /// DelSuccess
            /// </summary>
            DelSuccess,
            /// <summary>
            /// NoKey
            /// </summary>
            NoKey,
            /// <summary>
            /// NoKeyDel
            /// </summary>
            NoKeyDel,
            /// <summary>
            /// KeyError
            /// </summary>
            KeyError,
            /// <summary>
            /// Other
            /// </summary>
            Other
        }
    }
}
