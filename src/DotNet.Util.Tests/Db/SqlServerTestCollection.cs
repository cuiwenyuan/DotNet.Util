using Xunit;

namespace DotNet.Util.Tests.Db
{
    /// <summary>
    ///	SqlServerTestCollection
    /// 数据库（SQL Server）集成测试集合。
    /// 
    /// 背景：两处共享全局状态决定了连库测试必须串行。
    /// 1) DbUtil.ConnectionString / DbUtil.CurrentDbType 是 public static 字段
    ///    （见 DbUtil.cs，默认取 BaseSystemInfo），并行下会互相污染；
    /// 2) 集成测试共用同一批测试表（DupTestUser / DupTestOrder），
    ///    并行跑会出现「同一时刻既有用例在清表又有用例在断言行数」的竞态。
    /// 因此所有需要连真实数据库的测试类都归入本集合，由 xUnit 串行执行。
    /// 
    /// 修改记录
    /// 
    ///		2026.09.18 版本：1.0	Troy.Cui 建立（数据库测试补齐计划 P1）。
    ///	
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2026.09.18</date>
    /// </author> 
    /// </summary>
    [CollectionDefinition(Name, DisableParallelization = true)]
    public class SqlServerTestCollection
    {
        /// <summary>
        /// 集合名称
        /// </summary>
        public const string Name = "SqlServerTestCollection";
    }
}
