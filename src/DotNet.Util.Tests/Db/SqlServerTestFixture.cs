using System;
using System.Data;
using DotNet.Util;

namespace DotNet.Util.Tests.Db
{
    /// <summary>
    ///	SqlServerTestFixture
    /// SQL Server 集成测试基础设施。
    /// 
    /// 职责：
    /// 1) 环境变量门控 —— 未设置 DUP_TEST_SQLSERVER 时以明确异常提示，不静默跳过；
    /// 2) 库名白名单 —— 只允许连接 DotNetUtilTest，防止 Truncate/BatchDelete/Delete
    ///    误清业务库（这是本类最关键的安全护栏）；
    /// 3) 测试表结构保障 —— 幂等建表（DupTestUser / DupTestOrder）；
    /// 4) 数据生命周期 —— ResetData 清表、SeedUsers 灌种子数据。
    /// 
    /// 用法：测试类标注 [Collection(SqlServerTestCollection.Name)] 并注入本 fixture。
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
    public sealed class SqlServerTestFixture : IDisposable
    {
        #region 常量

        /// <summary>
        /// 连接串环境变量名
        /// </summary>
        public const string EnvVarName = "DUP_TEST_SQLSERVER";

        /// <summary>
        /// 允许连接的数据库名（白名单内才可执行破坏性操作）
        /// </summary>
        public const string AllowedDatabase = "DotNetUtilTest";

        /// <summary>
        /// 主测试表名
        /// </summary>
        public const string TableUser = "DupTestUser";

        /// <summary>
        /// 从测试表名
        /// </summary>
        public const string TableOrder = "DupTestOrder";

        /// <summary>
        /// 无参测试存储过程名（GetFromProcedure 无参重载用）
        /// </summary>
        public const string ProcedureUserList = "DupTestUserList";

        /// <summary>
        /// 带主键参数的测试存储过程名（GetFromProcedure 带 id 重载用）
        /// </summary>
        public const string ProcedureUserGetById = "DupTestUserGetById";

        /// <summary>
        /// 种子数据使用的固定时间（避免依赖 GETDATE()，便于断言）
        /// </summary>
        public static readonly DateTime SeedCreateTime = new DateTime(2026, 1, 1, 0, 0, 0);

        /// <summary>
        /// 种子数据使用的固定修改时间
        /// </summary>
        public static readonly DateTime SeedUpdateTime = new DateTime(2026, 1, 2, 0, 0, 0);

        #endregion

        #region 属性

        /// <summary>
        /// 已校验通过的数据库连接串
        /// </summary>
        public string ConnectionString { get; }

        /// <summary>
        /// 连接串指向的数据库名（已通过白名单校验）
        /// </summary>
        public string Database { get; }

        #endregion

        #region 构造与释放

        /// <summary>
        /// 构造：读取环境变量、校验白名单、建表
        /// </summary>
        public SqlServerTestFixture()
        {
            var cs = Environment.GetEnvironmentVariable(EnvVarName);
            if (string.IsNullOrWhiteSpace(cs))
            {
                throw new InvalidOperationException(
                    $"数据库集成测试未启用：请设置环境变量 {EnvVarName} 后重跑。默认不通过。" +
                    $"例如：{EnvVarName}=Server=127.0.0.1,1433;Database={AllowedDatabase};Trusted_Connection=True;TrustServerCertificate=True;");
            }

            ConnectionString = cs.Trim();
            Database = ValidateConnectionString(ConnectionString);
            EnsureSchema();
        }

        /// <summary>
        /// 释放：清空测试数据（保留表结构，便于排查）
        /// </summary>
        public void Dispose()
        {
            try
            {
                ResetData();
            }
            catch
            {
                // 释放阶段不因清理失败而掩盖真实测试结果
            }
        }

        #endregion

        #region 连接串校验（安全护栏）

        /// <summary>
        /// 校验连接串并返回其数据库名。
        /// 未指定库名、或库名不在白名单内时抛异常 —— 这是防止误清业务库的关键护栏。
        /// </summary>
        /// <param name="connectionString">数据库连接串</param>
        /// <returns>数据库名</returns>
        public static string ValidateConnectionString(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException("连接串为空。");
            }

            var database = ExtractDatabase(connectionString);
            if (string.IsNullOrWhiteSpace(database))
            {
                throw new InvalidOperationException(
                    $"连接串未指定数据库（Database / Initial Catalog）。为安全起见拒绝执行，请使用 {AllowedDatabase}。");
            }

            if (!string.Equals(database, AllowedDatabase, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    $"安全护栏：数据库名「{database}」不在白名单内（仅允许 {AllowedDatabase}）。" +
                    "集成测试包含 Truncate / BatchDelete / Delete 等清空整表的操作，拒绝在非白名单库上执行。");
            }

            return database;
        }

        /// <summary>
        /// 从连接串中解析 Database / Initial Catalog。
        /// 手写解析而非用 SqlConnectionStringBuilder，避免 net48 与 net8.0 两档的 API 差异。
        /// </summary>
        /// <param name="connectionString">数据库连接串</param>
        /// <returns>数据库名，未指定时返回空字符串</returns>
        public static string ExtractDatabase(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                return string.Empty;
            }

            foreach (var part in connectionString.Split(';'))
            {
                if (string.IsNullOrWhiteSpace(part))
                {
                    continue;
                }

                var index = part.IndexOf('=');
                if (index <= 0)
                {
                    continue;
                }

                var key = part.Substring(0, index).Trim();
                var value = part.Substring(index + 1).Trim();

                if (string.Equals(key, "Database", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(key, "Initial Catalog", StringComparison.OrdinalIgnoreCase))
                {
                    return value;
                }
            }

            return string.Empty;
        }

        #endregion

        #region SQL 执行辅助

        /// <summary>
        /// 创建已设置连接串的数据库访问对象（调用方负责释放）
        /// </summary>
        public SqlHelper CreateHelper()
        {
            return new SqlHelper { ConnectionString = ConnectionString };
        }

        /// <summary>
        /// 执行非查询语句，返回受影响行数
        /// </summary>
        /// <param name="sql">SQL 语句</param>
        public int ExecuteNonQuery(string sql)
        {
            using (var db = CreateHelper())
            {
                try
                {
                    return db.ExecuteNonQuery(sql);
                }
                finally
                {
                    db.Close();
                }
            }
        }

        /// <summary>
        /// 执行查询并返回首行首列
        /// </summary>
        /// <param name="sql">SQL 语句</param>
        public object ExecuteScalar(string sql)
        {
            using (var db = CreateHelper())
            {
                try
                {
                    return db.ExecuteScalar(sql);
                }
                finally
                {
                    db.Close();
                }
            }
        }

        /// <summary>
        /// 执行查询并返回数据表
        /// </summary>
        /// <param name="sql">SQL 语句</param>
        public DataTable Fill(string sql)
        {
            using (var db = CreateHelper())
            {
                try
                {
                    return db.Fill(sql);
                }
                finally
                {
                    db.Close();
                }
            }
        }

        #endregion

        #region 静态连接作用域

        /// <summary>
        /// 临时把 DbUtil 的静态连接串与数据库类型切到测试库，Dispose 时恢复原值。
        /// 
        /// 背景：DbUtil 中不带 connectionString 的重载（ExecuteNonQuery / ExecuteScalar /
        /// ExecuteReader / Fill / MakeParameter / ExecuteCommandWithSplitter）一律读取
        /// public static 字段 DbUtil.ConnectionString 与 DbUtil.CurrentDbType。
        /// 直接改写会污染同进程内的其它测试，因此统一用本作用域包裹，
        /// 并配合 SqlServerTestCollection 串行执行。
        /// </summary>
        /// <returns>可释放的作用域，建议配合 using 使用</returns>
        public IDisposable UseStaticConnection()
        {
            return new StaticConnectionScope(ConnectionString);
        }

        /// <summary>
        /// 创建命令参数（由当前 provider 实现，跨 net48 / net8.0 两档可用）
        /// </summary>
        /// <param name="name">参数名</param>
        /// <param name="value">参数值</param>
        public IDbDataParameter CreateParameter(string name, object value)
        {
            using (var db = CreateHelper())
            {
                return db.MakeParameter(name, value);
            }
        }

        /// <summary>
        /// 静态连接作用域实现：构造时改写静态字段，释放时还原
        /// </summary>
        private sealed class StaticConnectionScope : IDisposable
        {
            private readonly string _oldConnectionString;
            private readonly CurrentDbType _oldDbType;

            /// <summary>
            /// 构造：切到指定连接串，数据库类型固定为 SqlServer
            /// </summary>
            /// <param name="connectionString">测试库连接串</param>
            public StaticConnectionScope(string connectionString)
            {
                _oldConnectionString = DbUtil.ConnectionString;
                _oldDbType = DbUtil.CurrentDbType;

                DbUtil.ConnectionString = connectionString;
                DbUtil.CurrentDbType = CurrentDbType.SqlServer;
            }

            /// <summary>
            /// 释放：还原静态字段
            /// </summary>
            public void Dispose()
            {
                DbUtil.ConnectionString = _oldConnectionString;
                DbUtil.CurrentDbType = _oldDbType;
            }
        }

        #endregion

        #region 表结构

        /// <summary>
        /// 幂等建表：表不存在时创建
        /// </summary>
        public void EnsureSchema()
        {
            ExecuteNonQuery($@"
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[{TableUser}]') AND type = N'U')
BEGIN
    CREATE TABLE [dbo].[{TableUser}] (
        [Id]            INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [ParentId]      INT NULL,
        [Code]          NVARCHAR(50) NULL,
        [Name]          NVARCHAR(50) NULL,
        [Age]           INT NULL,
        [Score]         DECIMAL(18,2) NULL,
        [Enabled]       BIT NULL,
        [Deleted]       BIT NULL,
        [SortCode]      INT NULL,
        [CreateUserId]  NVARCHAR(50) NULL,
        [CreateTime]    DATETIME NULL,
        [UpdateUserId]  NVARCHAR(50) NULL,
        [UpdateTime]    DATETIME NULL
    );
END");

            ExecuteNonQuery($@"
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[{TableOrder}]') AND type = N'U')
BEGIN
    CREATE TABLE [dbo].[{TableOrder}] (
        [Id]         INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [UserId]     INT NULL,
        [Code]       NVARCHAR(50) NULL,
        [Amount]     DECIMAL(18,2) NULL,
        [CreateTime] DATETIME NULL
    );
END");
            EnsureProcedures();
        }

        /// <summary>
        /// 幂等创建测试用存储过程（P5：GetFromProcedure 需要真实存储过程）。
        /// CREATE PROCEDURE 必须是批处理首语句，故用 EXEC 包裹。
        /// </summary>
        public void EnsureProcedures()
        {
            ExecuteNonQuery($@"
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[{ProcedureUserList}]') AND type = N'P')
BEGIN
    EXEC('CREATE PROCEDURE [dbo].[{ProcedureUserList}]
    AS
    BEGIN
        SET NOCOUNT ON;
        SELECT * FROM [dbo].[{TableUser}] ORDER BY [Id];
    END');
END");

            ExecuteNonQuery($@"
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[{ProcedureUserGetById}]') AND type = N'P')
BEGIN
    EXEC('CREATE PROCEDURE [dbo].[{ProcedureUserGetById}]
        @Id INT
    AS
    BEGIN
        SET NOCOUNT ON;
        SELECT * FROM [dbo].[{TableUser}] WHERE [Id] = @Id;
    END');
END");
        }

        /// <summary>
        /// 删除测试表（结构变更时使用）
        /// </summary>
        public void DropTables()
        {
            ExecuteNonQuery($"IF OBJECT_ID(N'[dbo].[{TableUser}]') IS NOT NULL DROP TABLE [dbo].[{TableUser}];");
            ExecuteNonQuery($"IF OBJECT_ID(N'[dbo].[{TableOrder}]') IS NOT NULL DROP TABLE [dbo].[{TableOrder}];");
        }

        /// <summary>
        /// 判断主测试表是否存在
        /// </summary>
        public bool UserTableExists()
        {
            var v = ExecuteScalar($"SELECT COUNT(*) FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[{TableUser}]') AND type = N'U'");
            return Convert.ToInt32(v) > 0;
        }

        #endregion

        #region 数据生命周期

        /// <summary>
        /// 清空两张测试表的数据（保留表结构与自增种子）
        /// </summary>
        public void ResetData()
        {
            ExecuteNonQuery($"DELETE FROM [dbo].[{TableUser}];");
            ExecuteNonQuery($"DELETE FROM [dbo].[{TableOrder}];");
        }

        /// <summary>
        /// 灌入指定数量的用户种子数据。
        /// 字段取值固定（Age = 20 + i、Score = 80.00 + i、SortCode = i），便于聚合与排序断言。
        /// </summary>
        /// <param name="count">行数</param>
        public void SeedUsers(int count)
        {
            for (var i = 1; i <= count; i++)
            {
                ExecuteNonQuery($@"
INSERT INTO [dbo].[{TableUser}]
    ([ParentId], [Code], [Name], [Age], [Score], [Enabled], [Deleted], [SortCode], [CreateUserId], [CreateTime], [UpdateUserId], [UpdateTime])
VALUES
    (NULL, 'U{i:0000}', N'User{i}', {20 + i}, {80 + i}.00, 1, 0, {i}, 'seed-creator', '{SeedCreateTime:yyyy-MM-dd HH:mm:ss}', 'seed-updater', '{SeedUpdateTime:yyyy-MM-dd HH:mm:ss}');");
            }
        }

        /// <summary>
        /// 灌入订单种子数据
        /// </summary>
        /// <param name="userId">关联的用户主键</param>
        /// <param name="count">行数</param>
        public void SeedOrders(int userId, int count)
        {
            for (var i = 1; i <= count; i++)
            {
                ExecuteNonQuery($@"
INSERT INTO [dbo].[{TableOrder}] ([UserId], [Code], [Amount], [CreateTime])
VALUES ({userId}, 'O{i:0000}', {10 * i}.00, '{SeedCreateTime:yyyy-MM-dd HH:mm:ss}');");
            }
        }

        /// <summary>
        /// 查询主测试表的当前行数
        /// </summary>
        public int UserCount()
        {
            return Convert.ToInt32(ExecuteScalar($"SELECT COUNT(*) FROM [dbo].[{TableUser}]"));
        }

        /// <summary>
        /// 查询订单表的当前行数
        /// </summary>
        public int OrderCount()
        {
            return Convert.ToInt32(ExecuteScalar($"SELECT COUNT(*) FROM [dbo].[{TableOrder}]"));
        }

        #endregion
    }
}
