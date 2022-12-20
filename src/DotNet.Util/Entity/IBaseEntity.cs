//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2022, DotNet.
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
    ///		2022.04.22 版本：2.0 Troy.Cui 整理接口。
    ///		2012.11.11 版本：1.0 JiRiGaLa 整理接口。
    ///
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2022.04.22</date>
    /// </author> 
    /// </summary>
    public partial interface IBaseEntity
    {
        /// <summary>
        /// GetFrom
        /// </summary>
        /// <param name="dr">DataRow</param>
        /// <returns></returns>
        BaseEntity GetFrom(DataRow dr);
        /// <summary>
        /// GetFrom
        /// </summary>
        /// <param name="dataReader">IDataReader</param>
        /// <returns></returns>
        BaseEntity GetFrom(IDataReader dataReader);
        /// <summary>
        /// GetSingle
        /// </summary>
        /// <param name="dt">DataTable</param>
        /// <returns></returns>
        BaseEntity GetSingle(DataTable dt);

        /// <summary>
        /// 验证有效性
        /// </summary>
        /// <returns></returns>
        bool IsValid(out string message);
    }
}