//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2022, DotNet.
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
    ///		2022.12.18 版本：2.0 Troy.Cui 重构为Update方法，新增可选参数addUpdateInfo，默认为true
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
        /// 更新（带UserInfo自动更新字段）
        /// </summary>
        /// <param name="parameter">更新参数</param>
        /// <param name="clientIp">客户端IP</param>
        /// <param name="addUpdateInfo">是否添加更新信息</param>
        /// <returns></returns>
        public virtual int Update(KeyValuePair<string, object> parameter, string clientIp = null, bool addUpdateInfo = true)
        {
            var parameters = new List<KeyValuePair<string, object>> { parameter };
            AddUpdateInfo(parameters, clientIp, addUpdateInfo);
            return DbHelper.SetProperty(CurrentTableName, null, parameters);
        }
        /// <summary>
        /// 更新（带UserInfo自动更新字段）
        /// </summary>
        /// <param name="id">数组条件参数</param>
        /// <param name="parameter">更新参数</param>
        /// <param name="clientIp">客户端IP</param>
        /// <param name="addUpdateInfo">是否添加更新信息</param>
        /// <returns></returns>
        public virtual int Update(string id, KeyValuePair<string, object> parameter, string clientIp = null, bool addUpdateInfo = true)
        {
            return Update(new KeyValuePair<string, object>(PrimaryKey, id), parameter, clientIp, addUpdateInfo);
        }
        /// <summary>
        /// 更新（带UserInfo自动更新字段）
        /// </summary>
        /// <param name="id">数组条件参数</param>
        /// <param name="parameter">更新参数</param>
        /// <param name="clientIp">客户端IP</param>
        /// <param name="addUpdateInfo">是否添加更新信息</param>
        /// <returns></returns>
        public virtual int Update(object id, KeyValuePair<string, object> parameter, string clientIp = null, bool addUpdateInfo = true)
        {
            return Update(new KeyValuePair<string, object>(PrimaryKey, id), parameter, clientIp, addUpdateInfo);
        }
        /// <summary>
        /// 更新（带UserInfo自动更新字段）
        /// </summary>
        /// <param name="id">数组条件参数</param>
        /// <param name="parameters">更新参数</param>
        /// <param name="clientIp">客户端IP</param>
        /// <param name="addUpdateInfo">是否添加更新信息</param>
        /// <returns></returns>
        public virtual int Update(object id, List<KeyValuePair<string, object>> parameters, string clientIp = null, bool addUpdateInfo = true)
        {
            return Update(new KeyValuePair<string, object>(PrimaryKey, id), parameters, clientIp, addUpdateInfo);
        }
        /// <summary>
        /// 更新（带UserInfo自动更新字段）
        /// </summary>
        /// <param name="ids">数组条件参数</param>
        /// <param name="parameter">更新参数</param>
        /// <param name="clientIp">客户端IP</param>
        /// <param name="addUpdateInfo">是否添加更新信息</param>
        /// <returns></returns>
        public virtual int Update(object[] ids, KeyValuePair<string, object> parameter, string clientIp = null, bool addUpdateInfo = true)
        {
            return Update(PrimaryKey, ids, parameter, clientIp, addUpdateInfo);
        }
        /// <summary>
        /// 更新（带UserInfo自动更新字段）
        /// </summary>
        /// <param name="ids">数组条件参数</param>
        /// <param name="parameters">更新参数</param>
        /// <param name="clientIp">客户端IP</param>
        /// <param name="addUpdateInfo">是否添加更新信息</param>
        /// <returns></returns>
        public virtual int Update(object[] ids, List<KeyValuePair<string, object>> parameters, string clientIp = null, bool addUpdateInfo = true)
        {
            return Update(PrimaryKey, ids, parameters, clientIp, addUpdateInfo);
        }
        /// <summary>
        /// 更新（带UserInfo自动更新字段）
        /// </summary>
        /// <param name="name">条件参数名</param>
        /// <param name="values">条件参数值</param>
        /// <param name="parameter">更新参数</param>
        /// <param name="clientIp">客户端IP</param>
        /// <param name="addUpdateInfo">是否添加更新信息</param>
        /// <returns></returns>
        public virtual int Update(string name, object[] values, KeyValuePair<string, object> parameter, string clientIp = null, bool addUpdateInfo = true)
        {
            var result = 0;
            if (values == null)
            {
                result += Update(new KeyValuePair<string, object>(name, string.Empty), parameter, clientIp, addUpdateInfo);
            }
            else
            {
                for (var i = 0; i < values.Length; i++)
                {
                    result += Update(new KeyValuePair<string, object>(name, values[i]), parameter, clientIp, addUpdateInfo);
                }
            }
            return result;
        }
        /// <summary>
        /// 更新（带UserInfo自动更新字段）
        /// </summary>
        /// <param name="name">条件参数名</param>
        /// <param name="values">条件参数值</param>
        /// <param name="parameters">更新参数</param>
        /// <param name="clientIp">客户端IP</param>
        /// <param name="addUpdateInfo">是否添加更新信息</param>
        /// <returns></returns>
        public virtual int Update(string name, object[] values, List<KeyValuePair<string, object>> parameters, string clientIp = null, bool addUpdateInfo = true)
        {
            var result = 0;
            if (values == null)
            {
                result += Update(new KeyValuePair<string, object>(name, string.Empty), parameters, clientIp, addUpdateInfo);
            }
            else
            {
                for (var i = 0; i < values.Length; i++)
                {
                    result += Update(new KeyValuePair<string, object>(name, values[i]), parameters, clientIp, addUpdateInfo);
                }
            }
            return result;
        }
        /// <summary>
        /// 更新（带UserInfo自动更新字段）
        /// </summary>
        /// <param name="whereParameter1">条件参数1</param>
        /// <param name="whereParameter2">条件参数2</param>
        /// <param name="parameter">更新参数</param>
        /// <param name="clientIp">客户端IP</param>
        /// <param name="addUpdateInfo">是否添加更新信息</param>
        /// <returns></returns>
        public virtual int Update(KeyValuePair<string, object> whereParameter1, KeyValuePair<string, object> whereParameter2, KeyValuePair<string, object> parameter, string clientIp = null, bool addUpdateInfo = true)
        {
            var whereParameters = new List<KeyValuePair<string, object>> { whereParameter1, whereParameter2 };
            var parameters = new List<KeyValuePair<string, object>> { parameter };
            AddUpdateInfo(parameters, clientIp, addUpdateInfo);
            return DbHelper.SetProperty(CurrentTableName, whereParameters, parameters);
        }
        /// <summary>
        /// 更新（带UserInfo自动更新字段）
        /// </summary>
        /// <param name="whereParameter">条件参数</param>
        /// <param name="parameter">更新参数</param>
        /// <param name="clientIp">客户端IP</param>
        /// <param name="addUpdateInfo">是否添加更新信息</param>
        /// <returns></returns>
        public virtual int Update(KeyValuePair<string, object> whereParameter, KeyValuePair<string, object> parameter, string clientIp = null, bool addUpdateInfo = true)
        {
            var whereParameters = new List<KeyValuePair<string, object>> { whereParameter };
            var parameters = new List<KeyValuePair<string, object>> { parameter };
            AddUpdateInfo(parameters, clientIp, addUpdateInfo);
            return DbHelper.SetProperty(CurrentTableName, whereParameters, parameters);
        }
        /// <summary>
        /// 更新（带UserInfo自动更新字段）
        /// </summary>
        /// <param name="whereParameters">条件参数</param>
        /// <param name="parameter">更新参数</param>
        /// <param name="clientIp">客户端IP</param>
        /// <param name="addUpdateInfo">是否添加更新信息</param>
        /// <returns></returns>
        public virtual int Update(List<KeyValuePair<string, object>> whereParameters, KeyValuePair<string, object> parameter, string clientIp = null, bool addUpdateInfo = true)
        {
            var parameters = new List<KeyValuePair<string, object>> { parameter };
            AddUpdateInfo(parameters, clientIp, addUpdateInfo);
            return DbHelper.SetProperty(CurrentTableName, whereParameters, parameters);
        }
        /// <summary>
        /// 更新（带UserInfo自动更新字段）
        /// </summary>
        /// <param name="whereParameter">条件参数</param>
        /// <param name="parameters">更新参数</param>
        /// <param name="clientIp">客户端IP</param>
        /// <param name="addUpdateInfo">是否添加更新信息</param>
        /// <returns></returns>
        public virtual int Update(KeyValuePair<string, object> whereParameter, List<KeyValuePair<string, object>> parameters, string clientIp = null, bool addUpdateInfo = true)
        {
            var whereParameters = new List<KeyValuePair<string, object>> { whereParameter };
            AddUpdateInfo(parameters, clientIp, addUpdateInfo);
            return DbHelper.SetProperty(CurrentTableName, whereParameters, parameters);
        }
        /// <summary>
        /// 更新（带UserInfo自动更新字段）
        /// </summary>
        /// <param name="whereParameters">条件参数</param>
        /// <param name="parameters">更新参数</param>
        /// <param name="clientIp">客户端IP</param>
        /// <param name="addUpdateInfo">是否添加更新信息</param>
        /// <returns></returns>
        public virtual int Update(List<KeyValuePair<string, object>> whereParameters, List<KeyValuePair<string, object>> parameters, string clientIp = null, bool addUpdateInfo = true)
        {
            AddUpdateInfo(parameters, clientIp, addUpdateInfo);
            return DbHelper.SetProperty(CurrentTableName, whereParameters, parameters);
        }

        /// <summary>
        /// 添加更新信息
        /// </summary>
        /// <param name="parameters">更新参数列表</param>
        /// <param name="clientIp">客户端IP</param>
        /// <param name="addUpdateInfo">是否添加更新信息</param>
        private void AddUpdateInfo(List<KeyValuePair<string, object>> parameters, string clientIp = null, bool addUpdateInfo = true)
        {
            if (UserInfo != null && addUpdateInfo)
            {
                if (parameters.FindAll(t => t.Key.Equals(BaseUtil.FieldUpdateUserId)).Count == 0)
                {
                    parameters.Add(new KeyValuePair<string, object>(BaseUtil.FieldUpdateUserId, UserInfo.UserId));
                }
                if (parameters.FindAll(t => t.Key.Equals(BaseUtil.FieldUpdateUserName)).Count == 0)
                {
                    parameters.Add(new KeyValuePair<string, object>(BaseUtil.FieldUpdateUserName, UserInfo.UserName));
                }
                if (parameters.FindAll(t => t.Key.Equals(BaseUtil.FieldUpdateBy)).Count == 0)
                {
                    parameters.Add(new KeyValuePair<string, object>(BaseUtil.FieldUpdateBy, UserInfo.RealName));
                }
                if (parameters.FindAll(t => t.Key.Equals(BaseUtil.FieldUpdateTime)).Count == 0)
                {
                    parameters.Add(new KeyValuePair<string, object>(BaseUtil.FieldUpdateTime, DateTime.Now));
                }
                if (parameters.FindAll(t => t.Key.Equals(BaseUtil.FieldUpdateIp)).Count == 0)
                {
                    parameters.Add(new KeyValuePair<string, object>(BaseUtil.FieldUpdateIp, !string.IsNullOrEmpty(clientIp) ? clientIp : Utils.GetIp()));
                }
            }
        }
    }
}