//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2024, DotNet.
//-----------------------------------------------------------------

namespace DotNet.Util
{
    /// <summary>
    /// CurrentDbType
    /// 有关数据库连接类型定义。
    /// 
    /// 修改记录
    /// 
    ///		2011.02.22 版本：3.1 JiRiGaLa 转移文件位置，还是不换为好。
    ///		2007.04.14 版本：3.0 JiRiGaLa 检查程序格式通过，不再进行修改主键操作。
    ///		2006.11.17 版本：2.1 JiRiGaLa 变量命规范化。
    ///		2006.04.18 版本：2.0 JiRiGaLa 重新调整主键的规范化。
    ///		
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2011.02.22</date>
    /// </author> 
    /// </summary>
    public enum CurrentDbType
    {
        /// <summary>
        /// 数据库类型：Oracle
        /// </summary>
        Oracle,
 
        /// <summary>
        /// 数据库类型：SqlServer
        /// </summary>
        SqlServer,
        
        /// <summary>
        /// 数据库类型：Access
        /// </summary>
        Access,
       
        /// <summary>
        /// 数据库类型：DB2
        /// </summary>
        Db2,

        /// <summary>
        /// 数据库类型：MySql
        /// </summary>
        MySql,

        /// <summary>
        /// 数据库类型：SQLite
        /// </summary>
        SqLite,

        /// <summary>
        /// 数据库类型：SQLite
        /// </summary>
        SQLite,

        /// <summary>
        /// 数据库类型：Ase
        /// </summary>
        Ase,

        /// <summary>
        /// 数据库类型：PostgreSql
        /// </summary>
        PostgreSql
    }
}