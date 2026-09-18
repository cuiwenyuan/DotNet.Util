using System;
using System.Collections.Generic;
using DotNet.Util;
using Xunit;

namespace DotNet.Util.Tests.Db
{
    /// <summary>
    ///	DbUtilWriteIntegrationTests
    /// DbUtil 写入类方法（Insert / UpdateRecord / SetProperty / Delete / BatchDelete / Truncate /
    /// GetProperty / GetProperties / GetWhereString）的集成测试 —— 数据库测试补齐计划 P4。
    /// 
    /// 覆盖：
    /// 1) Insert —— 字段值写入、null 值落地为 NULL 字面量、decimal/DateTime 类型持久化；
    /// 2) UpdateRecord —— 按主键更新、null 值置空、无匹配返回 0；
    /// 3) SetProperty —— 条件参数更新、附加手写 whereSql、无匹配返回 0；
    /// 4) Delete —— KeyValuePair 条件、数组 IN、null 值 IS NULL、纯条件字符串、null 参数清表；
    /// 5) BatchDelete —— 按批次递归删除、条件不匹配时不动数据；
    /// 6) Truncate —— 清空表、重置自增种子；
    /// 7) GetProperty —— 取值、无匹配返回空串、TOP + ORDER BY；
    /// 8) GetProperties —— 数组 IN 版本、参数 + TOP 版本、DISTINCT 版本、空数组边界；
    /// 9) GetWhereString —— 两个重载生成的 SQL 文本（不执行，只校验拼装结果）。
    /// 
    /// 安全：全部操作仅在白名单库 DotNetUtilTest 内执行，连接串由 SqlServerTestFixture 校验。
    /// 默认不通过：未设置环境变量 DUP_TEST_SQLSERVER 时 fixture 构造即抛明确异常。
    /// 
    /// 修改记录
    /// 
    ///		2026.09.18 版本：1.0	Troy.Cui 建立（数据库测试补齐计划 P4）。
    ///	
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2026.09.18</date>
    /// </author> 
    /// </summary>
    [Collection(SqlServerTestCollection.Name)]
    public class DbUtilWriteIntegrationTests : IClassFixture<SqlServerTestFixture>
    {
        private readonly SqlServerTestFixture _fixture;

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="fixture">数据库测试基础设施</param>
        public DbUtilWriteIntegrationTests(SqlServerTestFixture fixture)
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
        /// 查询指定 Code 的用户行（约定每个用例内 Code 唯一）
        /// </summary>
        /// <param name="code">用户编码</param>
        private System.Data.DataRow GetUserByCode(string code)
        {
            var dt = _fixture.Fill($"SELECT TOP 1 * FROM {Table} WHERE Code = N'{code}'");
            Assert.Equal(1, dt.Rows.Count);
            return dt.Rows[0];
        }

        /// <summary>
        /// 构造单字段条件
        /// </summary>
        /// <param name="name">字段名</param>
        /// <param name="value">字段值</param>
        private static List<KeyValuePair<string, object>> Where(string name, object value)
        {
            return new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(name, value)
            };
        }

        #endregion

        #region Insert

        [Fact]
        public void Insert_WithFieldsAndValues_ReturnsOneAndPersistsRow()
        {
            _fixture.ResetData();

            int result;
            using (var db = CreateHelper())
            {
                result = db.Insert(Table, new[] { "Code", "Name", "Age" }, new object[] { "I0001", "Inserted", 30 });
            }

            Assert.Equal(1, result);
            Assert.Equal(1, _fixture.UserCount());

            var row = GetUserByCode("I0001");
            Assert.Equal("Inserted", Convert.ToString(row["Name"]));
            Assert.Equal(30, Convert.ToInt32(row["Age"]));
        }

        [Fact]
        public void Insert_WithNullValue_WritesDatabaseNull()
        {
            _fixture.ResetData();

            using (var db = CreateHelper())
            {
                // SqlBuilder.SetValue 在 Insert 时遇到 null 直接写 NULL 字面量（不加参数）
                db.Insert(Table, new[] { "Code", "Name", "Age" }, new object[] { "I0002", null!, null! });
            }

            var row = GetUserByCode("I0002");
            Assert.Equal(DBNull.Value, row["Name"]);
            Assert.Equal(DBNull.Value, row["Age"]);
        }

        [Fact]
        public void Insert_WithDecimalAndDateTime_PersistsTypedValues()
        {
            _fixture.ResetData();
            var createTime = new DateTime(2026, 3, 4, 5, 6, 7);

            using (var db = CreateHelper())
            {
                db.Insert(Table, new[] { "Code", "Score", "CreateTime" }, new object[] { "I0003", 88.55m, createTime });
            }

            var row = GetUserByCode("I0003");
            Assert.Equal(88.55m, Convert.ToDecimal(row["Score"]));
            Assert.Equal(createTime, Convert.ToDateTime(row["CreateTime"]));
        }

        #endregion

        #region UpdateRecord

        [Fact]
        public void UpdateRecord_ByKeyField_UpdatesTargetField()
        {
            _fixture.ResetData();
            _fixture.SeedUsers(3);

            int result;
            using (var db = CreateHelper())
            {
                result = db.UpdateRecord(Table, "Code", "U0002", "Name", "Updated");
            }

            Assert.Equal(1, result);
            Assert.Equal("Updated", Convert.ToString(GetUserByCode("U0002")["Name"]));
        }

        [Fact]
        public void UpdateRecord_WithNullValue_SetsFieldToNull()
        {
            _fixture.ResetData();
            _fixture.SeedUsers(3);

            using (var db = CreateHelper())
            {
                // Update 时 null 值生成 " SET Age = NULL"
                db.UpdateRecord(Table, "Code", "U0001", "Age", null!);
            }

            Assert.Equal(DBNull.Value, GetUserByCode("U0001")["Age"]);
        }

        [Fact]
        public void UpdateRecord_NoMatchingRow_ReturnsZero()
        {
            _fixture.ResetData();
            _fixture.SeedUsers(2);

            int result;
            using (var db = CreateHelper())
            {
                result = db.UpdateRecord(Table, "Code", "NOT-EXISTS", "Name", "Nobody");
            }

            Assert.Equal(0, result);
        }

        #endregion

        #region SetProperty

        [Fact]
        public void SetProperty_WithWhereParameters_UpdatesRows()
        {
            _fixture.ResetData();
            _fixture.SeedUsers(3);

            var values = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>("Name", "SetProp"),
                new KeyValuePair<string, object>("Enabled", false)
            };

            int result;
            using (var db = CreateHelper())
            {
                result = db.SetProperty(Table, Where("Code", "U0003"), values);
            }

            Assert.Equal(1, result);

            var row = GetUserByCode("U0003");
            Assert.Equal("SetProp", Convert.ToString(row["Name"]));
            Assert.Equal(false, Convert.ToBoolean(row["Enabled"]));
        }

        [Fact]
        public void SetProperty_WithWhereSql_AppliesBothConditions()
        {
            _fixture.ResetData();
            _fixture.SeedUsers(5);   // Age = 21 ~ 25

            var values = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>("Name", "Filtered")
            };

            int notMatched;
            int matched;
            using (var db = CreateHelper())
            {
                // 参数条件命中 U0001（Age=21），但手写条件不成立 → 0 行
                notMatched = db.SetProperty(Table, Where("Code", "U0001"), values, "Age > 100");
                // 同样的参数条件 + 成立的手写条件 → 1 行
                matched = db.SetProperty(Table, Where("Code", "U0001"), values, "Age > 20");
            }

            Assert.Equal(0, notMatched);
            Assert.Equal(1, matched);
            Assert.Equal("Filtered", Convert.ToString(GetUserByCode("U0001")["Name"]));
        }

        [Fact]
        public void SetProperty_NoMatchingRow_ReturnsZero()
        {
            _fixture.ResetData();
            _fixture.SeedUsers(2);

            var values = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>("Name", "Nobody")
            };

            int result;
            using (var db = CreateHelper())
            {
                result = db.SetProperty(Table, Where("Code", "NOT-EXISTS"), values);
            }

            Assert.Equal(0, result);
        }

        #endregion

        #region Delete

        [Fact]
        public void Delete_WithParameters_DeletesMatchingRows()
        {
            _fixture.ResetData();
            _fixture.SeedUsers(5);

            int result;
            using (var db = CreateHelper())
            {
                result = db.Delete(Table, Where("Code", "U0002"));
            }

            Assert.Equal(1, result);
            Assert.Equal(4, _fixture.UserCount());
        }

        [Fact]
        public void Delete_WithArrayParameter_UsesInCondition()
        {
            _fixture.ResetData();
            _fixture.SeedUsers(5);

            int result;
            using (var db = CreateHelper())
            {
                result = db.Delete(Table, Where("Code", new[] { "U0002", "U0004" }));
            }

            Assert.Equal(2, result);
            Assert.Equal(3, _fixture.UserCount());
        }

        [Fact]
        public void Delete_WithNullParameterValue_UsesIsNullCondition()
        {
            _fixture.ResetData();

            using (var db = CreateHelper())
            {
                db.Insert(Table, new[] { "Code", "ParentId" }, new object[] { "D0001", 100 });
                db.Insert(Table, new[] { "Code", "ParentId" }, new object[] { "D0002", null! });
            }

            int result;
            using (var db = CreateHelper())
            {
                result = db.Delete(Table, Where("ParentId", null!));
            }

            Assert.Equal(1, result);
            Assert.Equal(1, _fixture.UserCount());
            Assert.Equal(100, Convert.ToInt32(GetUserByCode("D0001")["ParentId"]));
        }

        [Fact]
        public void Delete_WithCondition_DeletesMatchingRows()
        {
            _fixture.ResetData();
            _fixture.SeedUsers(5);

            int result;
            using (var db = CreateHelper())
            {
                result = db.Delete(Table, "Age >= 24");
            }

            Assert.Equal(2, result);   // Age 24、25 两行
            Assert.Equal(3, _fixture.UserCount());
        }

        [Fact]
        public void Delete_WithNullParameters_DeletesAllRows()
        {
            _fixture.ResetData();
            _fixture.SeedUsers(4);

            int result;
            using (var db = CreateHelper())
            {
                // parameters 为 null → 不生成 WHERE，整表删除（白名单库内安全）
                // 显式转换以消除 Delete(tableName, List) 与 Delete(tableName, string) 的重载二义性
                result = db.Delete(Table, (List<KeyValuePair<string, object>>)null!);
            }

            Assert.Equal(4, result);
            Assert.Equal(0, _fixture.UserCount());
        }

        #endregion

        #region BatchDelete

        [Fact]
        public void BatchDelete_WithCondition_RemovesAllMatchingRows()
        {
            _fixture.ResetData();

            using (var db = CreateHelper())
            {
                for (var i = 1; i <= 5; i++)
                {
                    db.Insert(Table, new[] { "Code" }, new object[] { "BATCH" + i });
                }
                db.Insert(Table, new[] { "Code" }, new object[] { "KEEP1" });
                db.Insert(Table, new[] { "Code" }, new object[] { "KEEP2" });

                // 每批 2 行，递归删除直到 MIN(Id) 归零
                db.BatchDelete(Table, "Code LIKE 'BATCH%'", 2);
            }

            Assert.Equal(2, _fixture.UserCount());
        }

        [Fact]
        public void BatchDelete_WithNonMatchingCondition_KeepsAllRows()
        {
            _fixture.ResetData();
            _fixture.SeedUsers(3);

            using (var db = CreateHelper())
            {
                db.BatchDelete(Table, "Code LIKE 'NO-MATCH%'", 100);
            }

            Assert.Equal(3, _fixture.UserCount());
        }

        #endregion

        #region Truncate

        [Fact]
        public void Truncate_EmptiesTable()
        {
            _fixture.ResetData();
            _fixture.SeedUsers(5);

            using (var db = CreateHelper())
            {
                db.Truncate(Table);
            }

            Assert.Equal(0, _fixture.UserCount());
        }

        [Fact]
        public void Truncate_ResetsIdentitySeed()
        {
            _fixture.ResetData();

            int lastIdBeforeTruncate;
            using (var db = CreateHelper())
            {
                for (var i = 1; i <= 3; i++)
                {
                    db.Insert(Table, new[] { "Code" }, new object[] { "T" + i });
                }
            }

            lastIdBeforeTruncate = Convert.ToInt32(_fixture.ExecuteScalar($"SELECT MAX(Id) FROM {Table}"));
            Assert.True(lastIdBeforeTruncate >= 3);

            using (var db = CreateHelper())
            {
                db.Truncate(Table);
                db.Insert(Table, new[] { "Code" }, new object[] { "AFTER" });
            }

            // TRUNCATE 会重置 IDENTITY 种子，新行 Id 从 1 重新开始
            Assert.Equal(1, Convert.ToInt32(_fixture.ExecuteScalar($"SELECT MAX(Id) FROM {Table}")));
        }

        #endregion

        #region GetProperty

        [Fact]
        public void GetProperty_ByParameters_ReturnsTargetFieldValue()
        {
            _fixture.ResetData();
            _fixture.SeedUsers(3);

            string name;
            using (var db = CreateHelper())
            {
                name = db.GetProperty(Table, Where("Code", "U0003"), "Name");
            }

            Assert.Equal("User3", name);
        }

        [Fact]
        public void GetProperty_NoMatchingRow_ReturnsEmptyString()
        {
            _fixture.ResetData();
            _fixture.SeedUsers(2);

            string name;
            using (var db = CreateHelper())
            {
                name = db.GetProperty(Table, Where("Code", "NOT-EXISTS"), "Name");
            }

            Assert.Equal(string.Empty, name);
        }

        [Fact]
        public void GetProperty_WithTopLimitAndOrderBy_ReturnsExpectedRow()
        {
            _fixture.ResetData();
            _fixture.SeedUsers(5);   // SortCode = 1 ~ 5

            string name;
            using (var db = CreateHelper())
            {
                // SELECT TOP 1 Name FROM ... WHERE Enabled = @Enabled ORDER BY SortCode DESC
                name = db.GetProperty(Table, Where("Enabled", true), "Name", 1, "SortCode DESC");
            }

            Assert.Equal("User5", name);
        }

        #endregion

        #region GetProperties

        [Fact]
        public void GetProperties_ByNameAndValues_ReturnsTargetFieldArray()
        {
            _fixture.ResetData();
            _fixture.SeedUsers(5);

            string[] names;
            using (var db = CreateHelper())
            {
                names = db.GetProperties(Table, "Code", new object[] { "U0002", "U0004" }, "Name");
            }

            Assert.Equal(2, names.Length);
            Assert.Contains("User2", names);
            Assert.Contains("User4", names);
        }

        [Fact]
        public void GetProperties_WithEmptyValues_ReturnsEmptyArray()
        {
            _fixture.ResetData();
            _fixture.SeedUsers(2);

            string[] names;
            using (var db = CreateHelper())
            {
                names = db.GetProperties(Table, "Code", new object[0], "Name");
            }

            Assert.Empty(names);
        }

        [Fact]
        public void GetProperties_WithTopLimit_ReturnsLimitedRows()
        {
            _fixture.ResetData();
            _fixture.SeedUsers(5);

            string[] codes;
            using (var db = CreateHelper())
            {
                // SqlServer 分支生成 SELECT TOP 3 Code FROM ...（带 TOP 时不再是 DISTINCT）
                codes = db.GetProperties(Table, Where("Enabled", true), 3, "Code");
            }

            Assert.Equal(3, codes.Length);
        }

        [Fact]
        public void GetProperties_WithoutTopLimit_ReturnsDistinctValues()
        {
            _fixture.ResetData();
            _fixture.SeedUsers(5);

            string[] codes;
            using (var db = CreateHelper())
            {
                codes = db.GetProperties(Table, new List<KeyValuePair<string, object>>(), null, "Code");
            }

            Assert.Equal(5, codes.Length);
        }

        #endregion

        #region GetWhereString

        [Fact]
        public void GetWhereString_WithKeyValuePairs_GeneratesExpectedSql()
        {
            using (var db = CreateHelper())
            {
                var parameters = new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>("Code", "U0001"),
                    new KeyValuePair<string, object>("Name", null!),
                    new KeyValuePair<string, object>("Age", new[] { 21, 22 })
                };

                var sql = db.GetWhereString(parameters, " AND ");

                Assert.Equal("Code = @Code AND Name IS NULL AND Age IN ('21','22')", sql);
            }
        }

        [Fact]
        public void GetWhereString_WithNamesAndValues_GeneratesExpectedSqlAndClearsNames()
        {
            using (var db = CreateHelper())
            {
                var names = new[] { "Code", "Name", "Age" };
                var values = new object[] { "U0001", null!, new[] { 21, 22 } };

                var sql = db.GetWhereString(ref names, values, " AND ");

                Assert.Equal("Code = @Code AND Name IS NULL AND Age IN ('21','22')", sql);
                // 不需要参数化的条件（null 与数组）会把对应的 name 置空
                Assert.Null(names[1]);
                Assert.Null(names[2]);
            }
        }

        #endregion
    }
}
