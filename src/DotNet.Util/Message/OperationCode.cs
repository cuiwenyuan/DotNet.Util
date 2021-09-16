//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

namespace DotNet.Util
{
    #region public public enum OperationCode 权限枚举类型
    /// <summary>
    /// 权限枚举类型
    /// 
    /// 修改记录
    ///
    ///		2008.05.10 版本：1.1 JiRiGaLa 命名为 OperationCode。 
    ///		2007.12.08 版本：1.0 JiRiGaLa 添加 Config、UpLoad、DownLoad 权限。 
    ///		
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2007.12.08</date>
    /// </author>
    /// </summary>
    public enum OperationCode
    {
        /// <summary>
        /// 访问权限
        /// </summary>
        Access = 1,
        /// <summary>
        /// 新增权限
        /// </summary>
        Add = 2,
        /// <summary>
        /// 编辑权限
        /// </summary>
        Edit = 3,
        /// <summary>
        /// 浏览权限
        /// </summary>
        View = 4,
        /// <summary>
        /// 删除权限
        /// </summary>
        Delete = 5,
        /// <summary>
        /// 查询权限
        /// </summary>
        Search = 6,
        /// <summary>
        /// 导入权限
        /// </summary>
        Import = 7,
        /// <summary>
        /// 导出权限
        /// </summary>
        Export = 8,
        /// <summary>
        /// 打印权限
        /// </summary>
        Print = 9,
        /// <summary>
        /// 审核权限
        /// </summary>
        Audit = 10,
        /// <summary>
        /// 撤销审核权限
        /// </summary>
        UndoAudit = 11,
        /// <summary>
        /// 管理权限
        /// </summary>
        Admin = 11,
        /// <summary>
        /// 配置权限
        /// </summary>
        Config = 12,
        /// <summary>
        /// 上传权限
        /// </summary>
        Upload = 13,
        /// <summary>
        /// 下载权限
        /// </summary>
        Download = 14
    }
    #endregion
}