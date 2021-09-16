//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System.Data;


namespace DotNet.IService
{
    using Util;

    /// <summary>
    /// IComputerService
    /// 计算机接口
    /// 
    /// 修改记录
    /// 
    ///		2015.02.24 版本：1.0 JiRiGaLa 创建主键。
    ///		
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2015.02.24</date>
    /// </author> 
    /// </summary>
    public partial interface IStationService
    {
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <returns>数据表</returns>
        DataTable GetDataTable(BaseUserInfo userInfo);
	}
}