//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2020, DotNet.
//-----------------------------------------------------------------

namespace DotNet.Util
{
    /// <summary>
    /// PermissionOrganizeScope
    /// 常用权限范围。
    /// 
    /// 修改记录
    ///
    ///     2013.12.24 版本：3.0 JiRiGaLa 进行重命名。
    ///     2012.05.07 版本：2.0 JiRiGaLa 增加UserSubCompany。
    ///     2009.09.01 版本：1.0 Troy Cui 创建。
    ///		
    /// <author>
    ///		<name>Troy Cui</name>
    ///		<date>2013.12.24</date>
    /// </author> 
    /// </summary>
    public enum PermissionOrganizeScope
    {
        /// <summary>
        /// 自己的数据，仅本人
        /// </summary>
        OnlyOwnData = 0,

        /// <summary>
        /// 按详细设定的数据
        /// </summary>
        ByDetails = -1,

        /// <summary>
        /// 没有任何数据权限
        /// </summary>
        NotAllowed = -2,

        /// <summary>
        /// 全部数据
        /// </summary>
        AllData = -3,

        /// <summary>
        /// 所在的省
        /// </summary>
        Province = -4,

        /// <summary>
        /// 所在的市
        /// </summary>
        City = -5,

        /// <summary>
        /// 所在的县/区
        /// </summary>
        District = -6,

        /// <summary>
        /// 街道
        /// </summary>
        Street = -7,

        /// <summary>
        /// 用户所在公司数据
        /// </summary>
        UserCompany = -8,

        /// <summary>
        /// 用户所在分支机构数据
        /// </summary>
        UserSubCompany = -9,

        /// <summary>
        /// 用户所在部门数据
        /// </summary>
        UserDepartment = -10,

        /// <summary>
        /// 用户所在的子部门数据
        /// </summary>
        UserSubDepartment = -11,

        /// <summary>
        /// 用户所在工作组数据
        /// </summary>
        UserWorkgroup = -12
    }
}