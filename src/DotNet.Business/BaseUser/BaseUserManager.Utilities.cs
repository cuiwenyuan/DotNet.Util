//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System.Collections.Generic;

namespace DotNet.Business
{
    using Model;

    /// <summary>
    /// BaseUserManager
    /// 用户管理
    /// 
    /// 修改纪录
    /// 
    ///		2013.01.05 版本：1.0 JiRiGaLa	创建文件。
    /// 
    /// <author>
    ///		<name>JiRiGaLa</name>
    ///		<date>2013.01.05</date>
    /// </author> 
    /// </summary>
    public partial class BaseUserManager : BaseManager
    {
        /// <summary>
        /// GetUsersName
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static string GetUsersName(List<BaseUserEntity> list)
        {
            var result = string.Empty;

            foreach (var entity in list)
            {
                result += "," + entity.RealName;
            }
            if (!string.IsNullOrEmpty(result))
            {
                result = result.Substring(1);
            }
            return result;
        }
    }
}