//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using DotNet.Util;
using System;
using System.Collections.Generic;

namespace DotNet.Business
{
    /// <summary>
    ///	BaseManager
    /// 通用基类部分
    /// 
    /// 
    ///		2021.11.19 版本：1.0 Troy.Cui 新增此UpdateProperty方法，统一制定属性字段值更新写法
    ///
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2021.11.19</date>
    /// </author> 
    /// </summary>
    public partial class BaseManager : IBaseManager
    {
        /// <summary>
        /// 更新属性（带UserInfo自动更新字段）
        /// </summary>
        /// <param name="parameter">更新参数</param>
        /// <param name="clientIp">客户端IP</param>
        /// <returns></returns>
        public virtual int UpdateProperty(KeyValuePair<string, object> parameter, string clientIp = null)
        {
            var parameters = new List<KeyValuePair<string, object>> { parameter };
            AddUpdateInfo(parameters, clientIp);
            return DbUtil.SetProperty(DbHelper, CurrentTableName, null, parameters);
        }
        /// <summary>
        /// 更新属性（带UserInfo自动更新字段）
        /// </summary>
        /// <param name="id">数组条件参数</param>
        /// <param name="parameter">更新参数</param>
        /// <param name="clientIp">客户端IP</param>
        /// <returns></returns>
        public virtual int UpdateProperty(string id, KeyValuePair<string, object> parameter, string clientIp = null)
        {
            return UpdateProperty(new KeyValuePair<string, object>(PrimaryKey, id), parameter, clientIp);
        }
        /// <summary>
        /// 更新属性（带UserInfo自动更新字段）
        /// </summary>
        /// <param name="id">数组条件参数</param>
        /// <param name="parameter">更新参数</param>
        /// <param name="clientIp">客户端IP</param>
        /// <returns></returns>
        public virtual int UpdateProperty(object id, KeyValuePair<string, object> parameter, string clientIp = null)
        {
            return UpdateProperty(new KeyValuePair<string, object>(PrimaryKey, id), parameter, clientIp);
        }
        /// <summary>
        /// 更新属性（带UserInfo自动更新字段）
        /// </summary>
        /// <param name="id">数组条件参数</param>
        /// <param name="parameters">更新参数</param>
        /// <param name="clientIp">客户端IP</param>
        /// <returns></returns>
        public virtual int UpdateProperty(object id, List<KeyValuePair<string, object>> parameters, string clientIp = null)
        {
            return UpdateProperty(new KeyValuePair<string, object>(PrimaryKey, id), parameters, clientIp);
        }
        /// <summary>
        /// 更新属性（带UserInfo自动更新字段）
        /// </summary>
        /// <param name="ids">数组条件参数</param>
        /// <param name="parameter">更新参数</param>
        /// <param name="clientIp">客户端IP</param>
        /// <returns></returns>
        public virtual int UpdateProperty(object[] ids, KeyValuePair<string, object> parameter, string clientIp = null)
        {
            return UpdateProperty(PrimaryKey, ids, parameter, clientIp);
        }
        /// <summary>
        /// 更新属性（带UserInfo自动更新字段）
        /// </summary>
        /// <param name="ids">数组条件参数</param>
        /// <param name="parameters">更新参数</param>
        /// <param name="clientIp">客户端IP</param>
        /// <returns></returns>
        public virtual int UpdateProperty(object[] ids, List<KeyValuePair<string, object>> parameters, string clientIp = null)
        {
            return UpdateProperty(PrimaryKey, ids, parameters, clientIp);
        }
        /// <summary>
        /// 更新属性（带UserInfo自动更新字段）
        /// </summary>
        /// <param name="name">条件参数名</param>
        /// <param name="values">条件参数值</param>
        /// <param name="parameter">更新参数</param>
        /// <param name="clientIp">客户端IP</param>
        /// <returns></returns>
        public virtual int UpdateProperty(string name, object[] values, KeyValuePair<string, object> parameter, string clientIp = null)
        {
            var result = 0;
            if (values == null)
            {
                result += UpdateProperty(new KeyValuePair<string, object>(name, string.Empty), parameter, clientIp);
            }
            else
            {
                for (var i = 0; i < values.Length; i++)
                {
                    result += UpdateProperty(new KeyValuePair<string, object>(name, values[i]), parameter, clientIp);
                }
            }
            return result;
        }
        /// <summary>
        /// 更新属性（带UserInfo自动更新字段）
        /// </summary>
        /// <param name="name">条件参数名</param>
        /// <param name="values">条件参数值</param>
        /// <param name="parameters">更新参数</param>
        /// <param name="clientIp">客户端IP</param>
        /// <returns></returns>
        public virtual int UpdateProperty(string name, object[] values, List<KeyValuePair<string, object>> parameters, string clientIp = null)
        {
            var result = 0;
            if (values == null)
            {
                result += UpdateProperty(new KeyValuePair<string, object>(name, string.Empty), parameters, clientIp);
            }
            else
            {
                for (var i = 0; i < values.Length; i++)
                {
                    result += UpdateProperty(new KeyValuePair<string, object>(name, values[i]), parameters, clientIp);
                }
            }
            return result;
        }
        /// <summary>
        /// 更新属性（带UserInfo自动更新字段）
        /// </summary>
        /// <param name="whereParameter1">条件参数1</param>
        /// <param name="whereParameter2">条件参数2</param>
        /// <param name="parameter">更新参数</param>
        /// <param name="clientIp">客户端IP</param>
        /// <returns></returns>
        public virtual int UpdateProperty(KeyValuePair<string, object> whereParameter1, KeyValuePair<string, object> whereParameter2, KeyValuePair<string, object> parameter, string clientIp = null)
        {
            var whereParameters = new List<KeyValuePair<string, object>> { whereParameter1, whereParameter2 };
            var parameters = new List<KeyValuePair<string, object>> { parameter };
            AddUpdateInfo(parameters, clientIp);
            return DbUtil.SetProperty(DbHelper, CurrentTableName, whereParameters, parameters);
        }
        /// <summary>
        /// 更新属性（带UserInfo自动更新字段）
        /// </summary>
        /// <param name="whereParameter">条件参数</param>
        /// <param name="parameter">更新参数</param>
        /// <param name="clientIp">客户端IP</param>
        /// <returns></returns>
        public virtual int UpdateProperty(KeyValuePair<string, object> whereParameter, KeyValuePair<string, object> parameter, string clientIp = null)
        {
            var whereParameters = new List<KeyValuePair<string, object>> { whereParameter };
            var parameters = new List<KeyValuePair<string, object>> { parameter };
            AddUpdateInfo(parameters, clientIp);
            return DbUtil.SetProperty(DbHelper, CurrentTableName, whereParameters, parameters);
        }
        /// <summary>
        /// 更新属性（带UserInfo自动更新字段）
        /// </summary>
        /// <param name="whereParameters">条件参数</param>
        /// <param name="parameter">更新参数</param>
        /// <param name="clientIp">客户端IP</param>
        /// <returns></returns>
        public virtual int UpdateProperty(List<KeyValuePair<string, object>> whereParameters, KeyValuePair<string, object> parameter, string clientIp = null)
        {
            var parameters = new List<KeyValuePair<string, object>> { parameter };
            AddUpdateInfo(parameters, clientIp);
            return DbUtil.SetProperty(DbHelper, CurrentTableName, whereParameters, parameters);
        }
        /// <summary>
        /// 更新属性（带UserInfo自动更新字段）
        /// </summary>
        /// <param name="whereParameter">条件参数</param>
        /// <param name="parameters">更新参数</param>
        /// <param name="clientIp">客户端IP</param>
        /// <returns></returns>
        public virtual int UpdateProperty(KeyValuePair<string, object> whereParameter, List<KeyValuePair<string, object>> parameters, string clientIp = null)
        {
            var whereParameters = new List<KeyValuePair<string, object>> { whereParameter };
            AddUpdateInfo(parameters, clientIp);
            return DbUtil.SetProperty(DbHelper, CurrentTableName, whereParameters, parameters);
        }
        /// <summary>
        /// 更新属性（带UserInfo自动更新字段）
        /// </summary>
        /// <param name="whereParameters">条件参数</param>
        /// <param name="parameters">更新参数</param>
        /// <param name="clientIp">客户端IP</param>
        /// <returns></returns>
        public virtual int UpdateProperty(List<KeyValuePair<string, object>> whereParameters, List<KeyValuePair<string, object>> parameters, string clientIp = null)
        {
            AddUpdateInfo(parameters, clientIp);
            return DbUtil.SetProperty(DbHelper, CurrentTableName, whereParameters, parameters);
        }

        /// <summary>
        /// 添加更新信息
        /// </summary>
        /// <param name="parameters">更新参数列表</param>
        /// <param name="clientIp">客户端IP</param>
        private void AddUpdateInfo(List<KeyValuePair<string, object>> parameters, string clientIp = null)
        {
            if (UserInfo != null)
            {
                if (!parameters.Contains(new KeyValuePair<string, object>(BaseUtil.FieldUpdateUserId, UserInfo.Id)))
                {
                    parameters.Add(new KeyValuePair<string, object>(BaseUtil.FieldUpdateUserId, UserInfo.Id));
                }
                if (!parameters.Contains(new KeyValuePair<string, object>(BaseUtil.FieldUpdateUserName, UserInfo.UserName)))
                {
                    parameters.Add(new KeyValuePair<string, object>(BaseUtil.FieldUpdateUserName, UserInfo.UserName));
                }
                if (!parameters.Contains(new KeyValuePair<string, object>(BaseUtil.FieldUpdateBy, UserInfo.RealName)))
                {
                    parameters.Add(new KeyValuePair<string, object>(BaseUtil.FieldUpdateBy, UserInfo.RealName));
                }
                if (!parameters.Contains(new KeyValuePair<string, object>(BaseUtil.FieldUpdateTime, DateTime.Now)))
                {
                    parameters.Add(new KeyValuePair<string, object>(BaseUtil.FieldUpdateTime, DateTime.Now));
                }
                if (!parameters.Contains(new KeyValuePair<string, object>(BaseUtil.FieldUpdateIp, !string.IsNullOrEmpty(clientIp) ? clientIp : Utils.GetIp())))
                {
                    parameters.Add(new KeyValuePair<string, object>(BaseUtil.FieldUpdateIp, !string.IsNullOrEmpty(clientIp) ? clientIp : Utils.GetIp()));
                }
            }
        }
    }
}