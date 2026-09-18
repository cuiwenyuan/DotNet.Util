using System;
using System.Collections.Generic;
using System.Data;
using DotNet.Util;
using Xunit;

namespace DotNet.Util.Tests.Db
{
    /// <summary>
    ///	DbUtilAdvancedIntegrationTests
    /// DbUtil 高级方法（存储过程分页 / GetFromProcedure / Aggregate / IsUpdate / LockNoWait /
    /// Ado 连接扩展 / ParentChildrens 树形遍历）的集成测试 —— 数据库测试补齐计划 P5。
    /// 
    /// 覆盖：
    /// 1) GetDataTableByPage（存储过程版，P3 遗留）—— 首页 TOP 分支、次页 ROW_NUMBER 分支、
    ///    带 condition 分页，并校验 out recordCount 输出参数；
    /// 2) GetFromProcedure ×2 —— 无参存储过程、带主键参数存储过程；
    /// 3) AggregateInt / AggregateDecimal / AggregateDateTime —— SUM / MIN / MAX 聚合；
    /// 4) IsUpdate ×2 重载 —— 未修改 / 他人修改 / 时间更新 / 按非主键字段定位；
    /// 5) LockNoWait ×2 —— SQL Server 不支持 FOR UPDATE NOWAIT，执行必然报错；
    ///    DbHelper.Fill 内部吞掉异常返回 null，LockNoWait 识别到后返回 -1；
    /// 6) Ado 连接扩展 —— IsOpen / IsClose / CloseConnection / Reopen；
    /// 7) ParentChildrens —— GetChildrens（含 idOnly）、GetChildrensByCode、GetParentsByCode。
    /// 
    /// 前置依赖（P5 比 P1~P4 多两项环境要求）：
    /// - 存储过程 `GetRecordByPage` 必须已存在于 DotNetUtilTest 库（分页存储过程重载调用它）；
    /// - `DupTestUserList` / `DupTestUserGetById` 由 SqlServerTestFixture.EnsureProcedures 幂等创建。
    /// 
    /// 安全：全部操作仅在白名单库 DotNetUtilTest 内执行，连接串由 SqlServerTestFixture 校验。
    /// 默认不通过：未设置环境变量 DUP_TEST_SQLSERVER 时 fixture 构造即抛明确异常。
    /// 
    /// 修改记录
    /// 
    ///		2026.09.18 版本：1.0	Troy.Cui 建立（数据库测试补齐计划 P5）。
    ///	
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2026.09.18</date>
    /// </author> 
    /// </summary>
    [Collection(SqlServerTestCollection.Name)]
    public class DbUtilAdvancedIntegrationTests : IClassFixture<SqlServerTestFixture>
    {
        private readonly SqlServerTestFixture _fixture;

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="fixture">数据库测试基础设施</param>
        public DbUtilAdvancedIntegrationTests(SqlServerTestFixture fixture)
        {
            _fixture = fixture;
        }

        #region 私有辅助

        /// <summary>
        /// 带架构前缀的表名
        /// </summary>
        private string Table => "[dbo].[" + SqlServerTestFixture.TableUser + "]";

        private SqlHelper CreateHelper()
        {
            return _fixture.CreateHelper();
        }

        /// <summary>
        /// 按 Code 取主键值
        /// </summary>
        /// <param name="code">用户编码</param>
        private int GetIdByCode(string code)
        {
            return Convert.ToInt32(_fixture.ExecuteScalar($"SELECT Id FROM {Table} WHERE Code = N'{code}'"));
        }

        /// <summary>
        /// 灌入树形结构数据：T-A → T-B → T-C（链），另有独立的 T-D
        /// </summary>
        private void SeedTree()
        {
            _fixture.ResetData();

            var rootId = InsertUser("T-A", null);
            var childId = InsertUser("T-B", rootId);
            InsertUser("T-C", childId);
            InsertUser("T-D", null);

            int InsertUser(string code, int? parentId)
            {
                var parentSql = parentId.HasValue ? parentId.Value.ToString() : "NULL";
                _fixture.ExecuteNonQuery($"INSERT INTO {Table} ([ParentId], [Code], [Name]) VALUES ({parentSql}, N'{code}', N'{code}');");
                return GetIdByCode(code);
            }
        }

        /// <summary>
        /// 灌入编码层级数据：001 / 001001 / 001001002 / 002（前缀即父子关系）
        /// </summary>
        private void SeedCodeTree()
        {
            _fixture.ResetData();
            foreach (var code in new[] { "001", "001001", "001001002", "002" })
            {
                _fixture.ExecuteNonQuery($"INSERT INTO {Table} ([Code], [Name]) VALUES (N'{code}', N'{code}');");
            }
        }

        #endregion

        #region GetDataTableByPage（存储过程版）

        [Fact]
        public void GetDataTableByPage_Procedure_FirstPage_ReturnsTopRowsAndRecordCount()
        {
            _fixture.ResetData();
            _fixture.SeedUsers(5);

            DataTable dt;
            int recordCount;
            using (var db = CreateHelper())
            {
                // 存储过程第 1 页走 "SELECT TOP n ... ORDER BY" 分支
                dt = db.GetDataTableByPage(out recordCount, 1, 2, "Id", "ASC", Table, null, "*");
            }

            Assert.Equal(5, recordCount);
            Assert.Equal(2, dt.Rows.Count);
            Assert.Equal("U0001", Convert.ToString(dt.Rows[0]["Code"]));
            Assert.Equal("U0002", Convert.ToString(dt.Rows[1]["Code"]));
        }

        [Fact]
        public void GetDataTableByPage_Procedure_SecondPage_UsesRowNumberBranch()
        {
            _fixture.ResetData();
            _fixture.SeedUsers(5);

            DataTable dt;
            int recordCount;
            using (var db = CreateHelper())
            {
                // 非首页走 ROW_NUMBER 分支，结果集多一列 ROWS
                dt = db.GetDataTableByPage(out recordCount, 2, 2, "Id", "ASC", Table, null, "*");
            }

            Assert.Equal(5, recordCount);
            Assert.Equal(2, dt.Rows.Count);
            Assert.True(dt.Columns.Contains("ROWS"));
            Assert.Equal("U0003", Convert.ToString(dt.Rows[0]["Code"]));
        }

        [Fact]
        public void GetDataTableByPage_Procedure_WithCondition_CountsFilteredRows()
        {
            _fixture.ResetData();
            _fixture.SeedUsers(5);   // Age = 21 ~ 25

            DataTable dt;
            int recordCount;
            using (var db = CreateHelper())
            {
                dt = db.GetDataTableByPage(out recordCount, 1, 10, "Id", "ASC", Table, "Age >= 24", "*");
            }

            Assert.Equal(2, recordCount);   // Age 24、25
            Assert.Equal(2, dt.Rows.Count);
        }

        #endregion

        #region GetFromProcedure

        [Fact]
        public void GetFromProcedure_WithoutParameter_ReturnsProcedureResult()
        {
            _fixture.ResetData();
            _fixture.SeedUsers(3);

            DataTable dt;
            using (var db = CreateHelper())
            {
                dt = db.GetFromProcedure(SqlServerTestFixture.ProcedureUserList, "DupUsers");
            }

            Assert.Equal(3, dt.Rows.Count);
            Assert.Equal("DupUsers", dt.TableName);
        }

        [Fact]
        public void GetFromProcedure_WithIdParameter_ReturnsSingleRow()
        {
            _fixture.ResetData();
            _fixture.SeedUsers(3);
            var id = GetIdByCode("U0002");

            DataTable dt;
            using (var db = CreateHelper())
            {
                // 内部用 BaseUtil.FieldId（Id）作为参数名，存储过程需有同名 @Id 参数
                dt = db.GetFromProcedure(SqlServerTestFixture.ProcedureUserGetById, "DupUser", id.ToString());
            }

            Assert.Equal(1, dt.Rows.Count);
            Assert.Equal("U0002", Convert.ToString(dt.Rows[0]["Code"]));
        }

        #endregion

        #region Aggregate

        [Fact]
        public void AggregateInt_SumAndMinMax_ReturnsExpectedValues()
        {
            _fixture.ResetData();
            _fixture.SeedUsers(5);   // Age = 21 ~ 25

            int sum;
            int min;
            int max;
            using (var db = CreateHelper())
            {
                sum = db.AggregateInt(Table, "Age", function: "SUM");
                min = db.AggregateInt(Table, "Age", function: "MIN");
                max = db.AggregateInt(Table, "Age", function: "MAX");
            }

            Assert.Equal(115, sum);   // 21+22+23+24+25
            Assert.Equal(21, min);
            Assert.Equal(25, max);
        }

        [Fact]
        public void AggregateInt_WithCondition_AggregatesFilteredRows()
        {
            _fixture.ResetData();
            _fixture.SeedUsers(5);

            int sum;
            using (var db = CreateHelper())
            {
                sum = db.AggregateInt(Table, "Age", "Age >= 24", "SUM");
            }

            Assert.Equal(49, sum);   // 24+25
        }

#if NET48
        // ⚠️ net48 档跳过：NewLife.Core 11.18.2026.801 的 object.ToDecimal()（Utility.ToDecimal）
        // 在本机 net48 测试宿主（testhost.net48.exe）上会引发**宿主进程崩溃**（用例本身通过、
        // 进程随后挂掉）。ToInt / ToDateTime 同样路径均正常，仅 ToDecimal 复现（已复现 2 次）。
        // 该问题已上报，定位与修复后再启用本用例。
        [Fact(Skip = "net48 上 NewLife.Core 的 object.ToDecimal() 会导致测试宿主崩溃，已上报，待修复后启用")]
#else
        [Fact]
#endif
        public void AggregateDecimal_SumScore_ReturnsExpectedValue()
        {
            _fixture.ResetData();
            _fixture.SeedUsers(5);   // Score = 81 ~ 85

            decimal sum;
            using (var db = CreateHelper())
            {
                sum = db.AggregateDecimal(Table, "Score", function: "SUM");
            }

            Assert.Equal(415m, sum);
        }

        [Fact]
        public void AggregateDateTime_MaxCreateTime_ReturnsLatestValue()
        {
            _fixture.ResetData();
            _fixture.SeedUsers(3);

            DateTime max;
            using (var db = CreateHelper())
            {
                max = db.AggregateDateTime(Table, "CreateTime", function: "MAX");
            }

            Assert.Equal(SqlServerTestFixture.SeedCreateTime, max);
        }

        #endregion

        #region IsUpdate

        [Fact]
        public void IsUpdate_UnchangedRecord_ReturnsFalse()
        {
            _fixture.ResetData();
            _fixture.SeedUsers(3);
            var id = GetIdByCode("U0001");

            bool result;
            using (var db = CreateHelper())
            {
                result = db.IsUpdate(Table, id, "seed-updater", SqlServerTestFixture.SeedUpdateTime);
            }

            Assert.False(result);
        }

        [Fact]
        public void IsUpdate_DifferentUserIdOrNewerTime_ReturnsTrue()
        {
            _fixture.ResetData();
            _fixture.SeedUsers(3);
            var id = GetIdByCode("U0001");

            bool byOtherUser;
            bool byOlderTime;
            using (var db = CreateHelper())
            {
                byOtherUser = db.IsUpdate(Table, id, "another-updater", SqlServerTestFixture.SeedUpdateTime);
                byOlderTime = db.IsUpdate(Table, id, "seed-updater", SqlServerTestFixture.SeedUpdateTime.AddDays(-1));
            }

            Assert.True(byOtherUser);
            Assert.True(byOlderTime);
        }

        [Fact]
        public void IsUpdate_ByFieldName_LocatesRowByNonPrimaryKey()
        {
            _fixture.ResetData();
            _fixture.SeedUsers(3);

            bool unchanged;
            using (var db = CreateHelper())
            {
                // 用 Code 字段定位，而非默认的 Id
                unchanged = db.IsUpdate(Table, "Code", "U0002", "seed-updater", SqlServerTestFixture.SeedUpdateTime);
            }

            Assert.False(unchanged);
        }

        #endregion

        #region LockNoWait

        [Fact]
        public void LockNoWait_WithParameterList_ReturnsMinusOneOnSqlServer()
        {
            _fixture.ResetData();
            _fixture.SeedUsers(3);

            int result;
            using (var db = CreateHelper())
            {
                result = db.LockNoWait(Table, new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>("Code", "U0001")
                });
            }

            // "FOR UPDATE NOWAIT" 是 Oracle 语法，SQL Server 执行必然报错，返回 -1。
            // 修复前（2026.09.18 已修）：LockNoWait 丢弃了 Fill 的返回值，
            // 而 DbHelper.Fill 内部 catch 后把它**自己的局部变量** dt 置 null 返回，
            // 调用方拿到的仍是自己 new 出来的空表 → 异常被静默吞掉、恒返回 0。
            // 现实现改为接收 Fill 返回值，null 即视为失败返回 -1。
            Assert.Equal(-1, result);
        }

        [Fact]
        public void LockNoWait_WithParamsArray_ReturnsMinusOneOnSqlServer()
        {
            _fixture.ResetData();
            _fixture.SeedUsers(3);

            int result;
            using (var db = CreateHelper())
            {
                result = db.LockNoWait(Table, new KeyValuePair<string, object>("Code", "U0002"));
            }

            // 同 LockNoWait_WithParameterList_ReturnsMinusOneOnSqlServer 的说明：
            // params 重载只是把数组转成 List 再调用同一实现，行为一致
            Assert.Equal(-1, result);
        }

        #endregion

        #region Ado 连接扩展

        [Fact]
        public void Ado_IsOpenAndIsClose_ReflectConnectionState()
        {
            using (var db = CreateHelper())
            {
                var connection = db.Open();
                try
                {
                    Assert.True(connection.IsOpen());
                    Assert.False(connection.IsClose());
                }
                finally
                {
                    connection.CloseConnection();
                    db.Close();
                }

                Assert.True(connection.IsClose());
                Assert.False(connection.IsOpen());
            }
        }

        [Fact]
        public void Ado_Reopen_ReopensConnection()
        {
            using (var db = CreateHelper())
            {
                var connection = db.Open();
                try
                {
                    connection.CloseConnection();
                    Assert.True(connection.IsClose());

                    connection.Reopen();
                    Assert.True(connection.IsOpen());
                }
                finally
                {
                    connection.CloseConnection();
                    db.Close();
                }
            }
        }

        #endregion

        #region ParentChildrens

        [Fact]
        public void GetChildrens_ByParentId_ReturnsWholeSubTree()
        {
            SeedTree();
            var rootId = GetIdByCode("T-A");

            DataTable dt;
            using (var db = CreateHelper())
            {
                // CTE 递归：锚点 Id IN ('rootId')，再按 ParentId 递归出 B、C
                dt = db.GetChildrens(Table, "Id", rootId.ToString(), "ParentId");
            }

            Assert.Equal(3, dt.Rows.Count);
        }

        [Fact]
        public void GetChildrens_WithIdOnly_ReturnsDescendantsOnly()
        {
            SeedTree();
            var rootId = GetIdByCode("T-A");

            DataTable dt;
            using (var db = CreateHelper())
            {
                // idOnly 分支的锚点是 ParentId IN ('rootId')，不含根自身
                dt = db.GetChildrens(Table, "Id", rootId.ToString(), "ParentId", null, true);
            }

            Assert.Equal(2, dt.Rows.Count);
            Assert.Equal(1, dt.Columns.Count);
        }

        [Fact]
        public void GetChildrensByCode_WithCodePrefix_ReturnsMatchingRows()
        {
            SeedCodeTree();

            DataTable dt;
            using (var db = CreateHelper())
            {
                // LEFT(Code, LEN('001')) = '001'
                dt = db.GetChildrensByCode(Table, "Code", "001", "Code");
            }

            Assert.Equal(3, dt.Rows.Count);   // 001 / 001001 / 001001002
        }

        [Fact]
        public void GetParentsByCode_WithDeepCode_ReturnsAllAncestors()
        {
            SeedCodeTree();

            DataTable dt;
            using (var db = CreateHelper())
            {
                // LEFT(@Code, LEN(Code)) = Code → 找出所有是 '001001002' 前缀的行
                dt = db.GetParentsByCode(Table, "Code", "001001002", "Code");
            }

            Assert.Equal(3, dt.Rows.Count);
        }

        #endregion
    }
}
