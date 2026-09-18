using System;
using System.Collections.Generic;
using System.Data;
using DotNet.Util;
using Xunit;

namespace DotNet.Util.Tests.Db
{
    /// <summary>
    ///	DbUtilQueryIntegrationTests
    /// DbUtil 查询类方法（GetDataTable / GetDataTableByPage / Count / GetCount / Exists / TableExists）
    /// 的集成测试 —— 数据库测试补齐计划 P3。
    /// 
    /// 覆盖：
    /// 1) GetDataTable ×3 —— IN 查询、KeyValuePair 条件 + TOP、IDbDataParameter 条件 + 指定字段；
    /// 2) GetDataTableByPage ×5 —— 自定义 SQL 分页、带条件分页、表名分页（含参数）、
    ///    带 out recordCount 分页（含 maxOutPut 截断）、末页余数；
    /// 3) Count / DistinctCount / GetCount ×2 / Exists / TableExists。
    /// 
    /// 暂不覆盖：`GetDataTableByPage(out recordCount, pageNo, pageSize, sortExpression, ...)`
    /// （DbUtil.GetDataTableByPage.cs:162）—— 该重载调用存储过程 `GetRecordByPage`，
    /// 需先建存储过程，留到 P5 与 GetFromProcedure 一并处理。
    /// 
    /// 默认不通过：未设置环境变量 DUP_TEST_SQLSERVER 时 fixture 构造即抛明确异常。
    /// 
    /// 修改记录
    /// 
    ///		2026.09.18 版本：1.0	Troy.Cui 建立（数据库测试补齐计划 P3）。
    ///	
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2026.09.18</date>
    /// </author> 
    /// </summary>
    [Collection(SqlServerTestCollection.Name)]
    public class DbUtilQueryIntegrationTests : IClassFixture<SqlServerTestFixture>
    {
        private readonly SqlServerTestFixture _fixture;

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="fixture">数据库测试基础设施</param>
        public DbUtilQueryIntegrationTests(SqlServerTestFixture fixture)
        {
            _fixture = fixture;
        }

        #region 私有辅助

        /// <summary>
        /// 带架构前缀的表名（GetDataTable / 分页 / 计数类方法用）
        /// </summary>
        private string Table => "[dbo].[" + SqlServerTestFixture.TableUser + "]";

        /// <summary>
        /// 纯表名（TableExists 内部会自动包 []，不能带架构前缀）
        /// </summary>
        private string BareTable => SqlServerTestFixture.TableUser;

        private SqlHelper CreateHelper()
        {
            return _fixture.CreateHelper();
        }

        #endregion

        #region GetDataTable

        [Fact]
        public void GetDataTable_ByNameAndValues_ReturnsMatchingRows()
        {
            _fixture.ResetData();
            _fixture.SeedUsers(5);

            using (var db = CreateHelper())
            {
                var dt = db.GetDataTable(Table, "Code", new object[] { "U0001", "U0003" }, "Id");

                Assert.Equal(2, dt.Rows.Count);
                Assert.Equal("U0001", Convert.ToString(dt.Rows[0]["Code"]));
                Assert.Equal("U0003", Convert.ToString(dt.Rows[1]["Code"]));
            }
        }

        [Fact]
        public void GetDataTable_NullValues_UsesIsNullCondition()
        {
            _fixture.ResetData();
            _fixture.SeedUsers(3);

            using (var db = CreateHelper())
            {
                // values 为空数组时退化成 WHERE Code IS NULL，种子数据 Code 均非空 → 0 行
                var dt = db.GetDataTable(Table, "Code", new object[0]);

                Assert.NotNull(dt);
                Assert.Empty(dt.Rows);
            }
        }

        [Fact]
        public void GetDataTable_ByKeyValueParameters_FiltersRows()
        {
            _fixture.ResetData();
            _fixture.SeedUsers(5);

            using (var db = CreateHelper())
            {
                var parameters = new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>("Code", "U0002")
                };

                var dt = db.GetDataTable(Table, parameters, 0, "Id");

                Assert.Single(dt.Rows);
                Assert.Equal("User2", Convert.ToString(dt.Rows[0]["Name"]));
            }
        }

        [Fact]
        public void GetDataTable_ByKeyValueParameters_WithTopLimit_LimitsRows()
        {
            _fixture.ResetData();
            _fixture.SeedUsers(5);

            using (var db = CreateHelper())
            {
                var dt = db.GetDataTable(Table, new List<KeyValuePair<string, object>>(), 2, "Id");

                Assert.Equal(2, dt.Rows.Count);
            }
        }

        [Fact]
        public void GetDataTable_ByConditions_FiltersRows()
        {
            _fixture.ResetData();
            _fixture.SeedUsers(5);

            using (var db = CreateHelper())
            {
                // 种子 Age = 20 + i（i 从 1）→ 21..25，>= 24 命中 U0004、U0005
                var dt = db.GetDataTable(Table, null, "Age >= 24", 0, "Id");

                Assert.Equal(2, dt.Rows.Count);
            }
        }

        [Fact]
        public void GetDataTable_WithSelectField_ReturnsOnlySelectedColumns()
        {
            _fixture.ResetData();
            _fixture.SeedUsers(3);

            using (var db = CreateHelper())
            {
                var dt = db.GetDataTable(Table, null, null, 0, "Id", "Code, Name");

                Assert.Equal(3, dt.Rows.Count);
                Assert.Equal(2, dt.Columns.Count);
                Assert.Equal("U0001", Convert.ToString(dt.Rows[0]["Code"]));
            }
        }

        #endregion

        #region GetDataTableByPage

        [Fact]
        public void GetDataTableByPage_BySql_ReturnsSecondPage()
        {
            _fixture.ResetData();
            _fixture.SeedUsers(5);

            using (var db = CreateHelper())
            {
                var dt = db.GetDataTableByPage(5, 2, 2,
                    "SELECT * FROM " + Table, null, "Id", "ASC");

                Assert.Equal(2, dt.Rows.Count);
                Assert.Equal("U0003", Convert.ToString(dt.Rows[0]["Code"]));
                Assert.Equal("U0004", Convert.ToString(dt.Rows[1]["Code"]));
            }
        }

        [Fact]
        public void GetDataTableByPage_BySqlWithCondition_FiltersRows()
        {
            _fixture.ResetData();
            _fixture.SeedUsers(5);

            using (var db = CreateHelper())
            {
                // Age >= 23 命中 U0003..U0005（3 条），取第 1 页 2 条
                var dt = db.GetDataTableByPage(3, 1, 2,
                    "SELECT * FROM " + Table, "Age >= 23", null, "Id", "ASC");

                Assert.Equal(2, dt.Rows.Count);
                Assert.Equal("U0003", Convert.ToString(dt.Rows[0]["Code"]));
            }
        }

        [Fact]
        public void GetDataTableByPage_ByTableName_ReturnsSecondPage()
        {
            _fixture.ResetData();
            _fixture.SeedUsers(5);

            using (var db = CreateHelper())
            {
                var dt = db.GetDataTableByPage(Table, "*", 2, 2, string.Empty, "Id");

                Assert.Equal(2, dt.Rows.Count);
                Assert.Equal("U0003", Convert.ToString(dt.Rows[0]["Code"]));
            }
        }

        [Fact]
        public void GetDataTableByPage_ByTableNameWithParameters_FiltersRows()
        {
            _fixture.ResetData();
            _fixture.SeedUsers(5);

            using (var db = CreateHelper())
            {
                var parameters = new[] { _fixture.CreateParameter("Age", 24) };

                var dt = db.GetDataTableByPage(Table, "*", 1, 2, "Age >= @Age", parameters, "Id");

                // Age >= 24 命中 U0004、U0005
                Assert.Equal(2, dt.Rows.Count);
                Assert.Equal("U0004", Convert.ToString(dt.Rows[0]["Code"]));
            }
        }

        [Fact]
        public void GetDataTableByPage_WithRecordCount_ReturnsCountAndPage()
        {
            _fixture.ResetData();
            _fixture.SeedUsers(5);

            using (var db = CreateHelper())
            {
                var dt = db.GetDataTableByPage(out var recordCount, Table, "*", 2, 2,
                    string.Empty, null, "Id");

                Assert.Equal(5, recordCount);
                Assert.Equal(2, dt.Rows.Count);
            }
        }

        [Fact]
        public void GetDataTableByPage_WithMaxOutPut_CapsRecordCount()
        {
            _fixture.ResetData();
            _fixture.SeedUsers(5);

            using (var db = CreateHelper())
            {
                var dt = db.GetDataTableByPage(out var recordCount, Table, "*", 1, 2,
                    string.Empty, null, "Id", 3);

                Assert.Equal(3, recordCount);
                Assert.Equal(2, dt.Rows.Count);
            }
        }

        [Fact]
        public void GetDataTableByPage_LastPage_ReturnsRemainingRows()
        {
            _fixture.ResetData();
            _fixture.SeedUsers(5);

            using (var db = CreateHelper())
            {
                var dt = db.GetDataTableByPage(Table, "*", 3, 2, string.Empty, "Id");

                Assert.Single(dt.Rows);
                Assert.Equal("U0005", Convert.ToString(dt.Rows[0]["Code"]));
            }
        }

        #endregion

        #region Count / DistinctCount

        [Fact]
        public void Count_WithoutCondition_ReturnsTotal()
        {
            _fixture.ResetData();
            _fixture.SeedUsers(5);

            using (var db = CreateHelper())
            {
                Assert.Equal(5, db.Count(Table));
            }
        }

        [Fact]
        public void Count_WithCondition_ReturnsFilteredTotal()
        {
            _fixture.ResetData();
            _fixture.SeedUsers(5);

            using (var db = CreateHelper())
            {
                Assert.Equal(3, db.Count(Table, "Age >= 23"));
            }
        }

        [Fact]
        public void DistinctCount_ReturnsDistinctValueCount()
        {
            _fixture.ResetData();
            _fixture.SeedUsers(5);

            using (var db = CreateHelper())
            {
                // 种子数据 Enabled 恒为 1 → 去重后 1；Age 各不同 → 5
                Assert.Equal(1, db.DistinctCount(Table, "Enabled"));
                Assert.Equal(5, db.DistinctCount(Table, "Age"));
            }
        }

        #endregion

        #region GetCount

        [Fact]
        public void GetCount_ByKeyValueParameters_ReturnsCount()
        {
            _fixture.ResetData();
            _fixture.SeedUsers(3);

            using (var db = CreateHelper())
            {
                var parameters = new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>("Code", "U0001")
                };

                Assert.Equal(1, db.GetCount(Table, parameters));
            }
        }

        [Fact]
        public void GetCount_ByCondition_ReturnsCount()
        {
            _fixture.ResetData();
            _fixture.SeedUsers(5);

            using (var db = CreateHelper())
            {
                Assert.Equal(2, db.GetCount(Table, "Age >= 24"));
            }
        }

        [Fact]
        public void GetCount_ByConditionWithParameters_ReturnsCount()
        {
            _fixture.ResetData();
            _fixture.SeedUsers(5);

            using (var db = CreateHelper())
            {
                var parameters = new[] { _fixture.CreateParameter("Age", 24) };

                Assert.Equal(2, db.GetCount(Table, "Age >= @Age", parameters));
            }
        }

        [Fact]
        public void GetCount_WithExcludeParameter_ExcludesMatchingValue()
        {
            _fixture.ResetData();
            _fixture.SeedUsers(3);

            using (var db = CreateHelper())
            {
                var parameters = new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>("Code", "U0001")
                };

                // 追加 Code <> 'U0001' → 命中 0 行
                var excluded = new KeyValuePair<string, object>("Code", "U0001");
                Assert.Equal(0, db.GetCount(Table, parameters, excluded));

                // 追加 Code <> 'U0002' → 仍命中 U0001
                var notExcluded = new KeyValuePair<string, object>("Code", "U0002");
                Assert.Equal(1, db.GetCount(Table, parameters, notExcluded));
            }
        }

        #endregion

        #region Exists / TableExists

        [Fact]
        public void Exists_ReturnsTrue_WhenRowMatches()
        {
            _fixture.ResetData();
            _fixture.SeedUsers(2);

            using (var db = CreateHelper())
            {
                var parameters = new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>("Code", "U0002")
                };

                Assert.True(db.Exists(Table, parameters));
            }
        }

        [Fact]
        public void Exists_ReturnsFalse_WhenNoRowMatches()
        {
            _fixture.ResetData();
            _fixture.SeedUsers(2);

            using (var db = CreateHelper())
            {
                var parameters = new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>("Code", "NOT-EXIST")
                };

                Assert.False(db.Exists(Table, parameters));
            }
        }

        [Fact]
        public void TableExists_ReturnsTrue_ForExistingTable()
        {
            using (var db = CreateHelper())
            {
                // 内部 SQL 会拼 object_id(N'[表名]')，只能传纯表名，不能带 [dbo]. 前缀
                Assert.True(db.TableExists(BareTable));
            }
        }

        [Fact]
        public void TableExists_ReturnsFalse_ForMissingTable()
        {
            using (var db = CreateHelper())
            {
                Assert.False(db.TableExists("NoSuchTableForDupTest"));
            }
        }

        #endregion
    }
}
