//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2024, DotNet.
//-----------------------------------------------------------------

namespace DotNet.Util
{
    /// <summary>
    /// DefaultRole
    /// 默认角色。
    /// 
    /// 这个应该是角色类别才对，不应该是用户类别
    /// 
    /// 修改记录
    ///
    ///		2009.07.08 版本：4.0 JiRiGaLa 命名为 DefaultRole。 
    ///		2008.09.27 版本：3.3 JiRiGaLa 命名为 Role。 
    ///		2007.12.20 版本：3.2 JiRiGaLa 增加 Config。 
    ///		2007.09.18 版本：3.1 JiRiGaLa 整理命名。 
    ///		2007.04.14 版本：3.0 JiRiGaLa 检查程序格式通过，不再进行修改主键操作。 
    ///     2007.03.26 版本：2.0 JiRiGaLa 结构优化整理。
    ///     2007.11.23 版本：1.1 JiRiGaLa 结构优化整理。
    ///     2006.11.23 版本：1.0 JiRiGaLa 结构优化整理。
    ///		
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2007.09.18</date>
    /// </author> 
    /// </summary>
    public enum DefaultRole
    {
        /// <summary>
        /// 系统配置员
        /// </summary>
        Config,
        /// <summary>
        /// 系统管理员
        /// </summary>
        Administrator,
        /// <summary>
        /// 系统管理组
        /// </summary>
        Administrators,
        /// <summary>
        /// 董事长
        /// </summary>
        ChairmanOfTheBoard,
        /// <summary>
        /// 副总裁
        /// </summary>
        VicePrecident,
        /// <summary>
        /// 总经理
        /// </summary>
        GeneralManager,
        /// <summary>
        /// 副经理
        /// </summary>
        ViceManager,
        /// <summary>
        /// 部长
        /// </summary>
        Minister,
        /// <summary>
        /// 副部长
        /// </summary>
        ViceMinsiter,
        /// <summary>
        /// 人力资源主管
        /// </summary>
        HumanResourceManager,
        /// <summary>
        /// 人力资源
        /// </summary>
        HumanResource,
        /// <summary>
        /// 财务经理
        /// </summary>
        FinanceManager,
        /// <summary>
        /// 财务人员
        /// </summary>
        Finance,
        /// <summary>
        /// 设备管理主管
        /// </summary>
        EquipmentManager,
        /// <summary>
        /// 设备管理人员
        /// </summary>
        Equipment,
        /// <summary>
        /// 普通员工
        /// </summary>
        Staff,
        /// <summary>
        /// 普通用户
        /// </summary>
        User  
    }
}  