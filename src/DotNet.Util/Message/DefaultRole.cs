//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2020, DotNet.
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
    ///		<name>Troy Cui</name>
    ///		<date>2007.09.18</date>
    /// </author> 
    /// </summary>
    public enum DefaultRole
    {
        Config,                 // 系统配置员
        Administrator,          // 系统管理员
        Administrators,         // 系统管理组
        ChairmanOfTheBoard,     // 董事长
        VicePrecident,          // 副总裁
        GeneralManager,         // 总经理
        ViceManager,            // 副经理
        Minister,               // 部长
        ViceMinsiter,           // 副部长
        HumanResourceManager,   // 人力资源主管
        HumanResource,          // 人力资源
        FinanceManager,         // 财务人员
        Finance,                // 财务人员
        EquipmentManager,       // 设备管理主管
        Equipment,              // 设备管理人员
        Staff,                  // 普通员工
        User                    // 普通用户
    }
}  