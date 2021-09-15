//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System.Data;

namespace DotNet.Util
{
    /// <summary>
    /// IDbUtilExtend
    /// 数据库访问扩展接口
    /// 
    /// 修改记录
    /// 
    ///		2010.7.13 版本：1.0 yanzi 数据库访问扩展接口。
    /// 
    /// <author>
    ///		<name>yanzi</name>
    ///		<date>2010.07.13</date>
    /// </author> 
    /// </summary>
    public partial interface IDbHelperExtend
    {
        /// <summary>
        /// 利用Net SqlBulkCopy 批量导入数据库,速度超快
        /// </summary>
        /// <param name="dt">源内存数据表</param>
        void SqlBulkCopyData(DataTable dt);
    }
}
