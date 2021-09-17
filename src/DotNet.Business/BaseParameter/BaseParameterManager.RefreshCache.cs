//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System.Collections.Generic;

namespace DotNet.Business
{
    using Model;

    /// <summary>
    /// BaseParameterManager
    /// 参数管理
    /// 
    /// 修改纪录
    /// 
    ///		2016.03.11 版本：1.1 JiRiGaLa	设置系统参数的方法进行改进。
    ///		2016.03.01 版本：1.0 JiRiGaLa	分离代码。
    /// 
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2016.03.11</date>
    /// </author> 
    /// </summary>
    public partial class BaseParameterManager : BaseManager
    {
        /// <summary>
        /// 刷新缓存
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static int RefreshCache(string tableName)
        {
            var result = 0;

            var parameterManager = new BaseParameterManager(tableName);
            var list = parameterManager.GetList<BaseParameterEntity>(
                new KeyValuePair<string, object>(BaseParameterEntity.FieldDeleted, 0)
                , new KeyValuePair<string, object>(BaseParameterEntity.FieldEnabled, 1));

            foreach (var entity in list)
            {
                // 2016-03-11 吉日嘎拉 强制刷新缓存
                SetParameterByCache(tableName, entity);
                result++;
            }
            
            return result;
        }
    }
}