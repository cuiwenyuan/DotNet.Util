//-----------------------------------------------------------------------
// <copyright file="BaseParameterManager.cs" company="DotNet">
//     Copyright (c) 2021, All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Data;
using System.Data.Common;
using System.Collections.Generic;

namespace DotNet.Business
{
    using Model;
    using Business;
    using Util;

    /// <summary>
    /// BaseParameterManager
    /// 参数表的基类结构定义管理层
    /// 
    /// 修改记录
    /// 
    ///	2021-10-07 版本：1.0 Troy.Cui 创建文件。
    ///		
    /// <author>
    ///	<name>Troy.Cui</name>
    ///	<date>2021-10-07</date>
    /// </author> 
    /// </summary>
    public partial class BaseParameterManager : BaseManager, IBaseManager
    {
        /// <summary>
        /// 获取当前用户的所有用户参数
        /// </summary>
        /// <returns></returns>
        public List<KeyValuePair<string, string>> GetUserParameterList()
        {
            var result = new List<KeyValuePair<string, string>>();
            var whereParameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseParameterEntity.FieldDeleted, 0),
                new KeyValuePair<string, object>(BaseParameterEntity.FieldEnabled, 1)
            };
            if (UserInfo != null)
            {
                whereParameters.Add(new KeyValuePair<string, object>(BaseParameterEntity.FieldParameterId, UserInfo.Id));
            }
            var list = GetList<BaseParameterEntity>(whereParameters, order: BaseParameterEntity.FieldParameterCode);
            foreach (var entity in list)
            {
                result.Add(new KeyValuePair<string, string>(entity.ParameterCode, entity.ParameterContent));
            }

            return result;
        }
    }
}
