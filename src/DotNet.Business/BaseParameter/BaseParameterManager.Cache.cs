//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2022, DotNet.
//-----------------------------------------------------------------

using System;

namespace DotNet.Business
{
    using Model;
    using Util;

    /// <remarks>
    /// BaseParameterManager
    /// 参数表缓存
    /// 
    /// 修改记录
    /// 
    ///     2016.03.01 版本：1.0 JiRiGaLa 创建。
    ///	
    /// <author>  
    ///		<name>Troy.Cui</name>
    ///		<date>2016.03.01</date>
    /// </author> 
    /// </remarks>
    public partial class BaseParameterManager
    {
        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="entity"></param>
        public static void SetParameterByCache(string tableName, BaseParameterEntity entity)
        {
            var key = "Parameter:" + tableName + ":" + entity.CategoryCode + ":" + entity.ParameterId + ":" + entity.ParameterCode;
            CacheUtil.Set<string>(key, entity.ParameterContent);
        }

        /// <summary>
        /// 从缓存获取
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="categoryCode"></param>
        /// <param name="parameterId"></param>
        /// <param name="parameterCode"></param>
        /// <param name="refreshCache"></param>
        /// <returns></returns>
        public static string GetParameterByCache(string tableName, string categoryCode, string parameterId, string parameterCode, bool refreshCache = false)
        {
            var result = string.Empty;

            if (!string.IsNullOrEmpty(tableName) && !string.IsNullOrEmpty(categoryCode) && !string.IsNullOrEmpty(parameterId) && !string.IsNullOrEmpty(parameterCode))
            {
                var key = "Parameter:" + tableName + ":" + categoryCode + ":" + parameterId + ":" + parameterCode;
                result = CacheUtil.Cache(key, () => new BaseParameterManager(tableName).GetParameter(tableName, categoryCode, parameterId, parameterCode), true, refreshCache);
            }

            return result;
        }
    }
}