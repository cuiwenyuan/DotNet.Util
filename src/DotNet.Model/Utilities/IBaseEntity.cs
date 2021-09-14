//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System.Data;

namespace DotNet.Model
{
    /// <summary>
    ///	IBaseEntity
    /// 通用接口部分
    /// 
    /// 修改记录
    /// 
    ///		2012.11.11 版本：1.0 JiRiGaLa 整理接口。
    ///
    /// <author>
    ///		<name>JiRiGaLa</name>
    ///		<date>2012.11.11</date>
    /// </author> 
    /// </summary>
    public partial interface IBaseEntity
    {
        /// <summary>
        /// GetFrom
        /// </summary>
        /// <param name="dr"></param>
        /// <returns></returns>
        BaseEntity GetFrom(DataRow dr);
        /// <summary>
        /// GetFrom
        /// </summary>
        /// <param name="dataReader"></param>
        /// <returns></returns>
        BaseEntity GetFrom(IDataReader dataReader);
        /// <summary>
        /// GetSingle
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        BaseEntity GetSingle(DataTable dt);
    }
}