using DotNet.Util;
using Xunit;

namespace DotNet.Util.Tests.Util
{
    /// <summary>
    /// BaseUtil.Field 测试（公共字段常量）
    /// </summary>
    public class BaseUtilFieldTests
    {
        [Fact]
        public void CoreFieldConstants()
        {
            Assert.Equal("Id", BaseUtil.FieldId);
            Assert.Equal("ParentId", BaseUtil.FieldParentId);
            Assert.Equal("Code", BaseUtil.FieldCode);
            Assert.Equal("Name", BaseUtil.FieldName);
            Assert.Equal("FullName", BaseUtil.FieldFullName);
            Assert.Equal("CategoryCode", BaseUtil.FieldCategoryCode);
            Assert.Equal("Enabled", BaseUtil.FieldEnabled);
            Assert.Equal("Deleted", BaseUtil.FieldDeleted);
            Assert.Equal("SortCode", BaseUtil.FieldSortCode);
        }

        [Fact]
        public void AuditFieldConstants()
        {
            Assert.Equal("IsAudited", BaseUtil.FieldIsAudited);
            Assert.Equal("AuditedDate", BaseUtil.FieldAuditedDate);
            Assert.Equal("AuditTime", BaseUtil.FieldAuditTime);
            Assert.Equal("AuditUserId", BaseUtil.FieldAuditUserId);
            Assert.Equal("AuditUserName", BaseUtil.FieldAuditUserName);
        }

        [Fact]
        public void CreateUpdateFieldConstants()
        {
            Assert.Equal("CreateUserId", BaseUtil.FieldCreateUserId);
            Assert.Equal("CreateUserName", BaseUtil.FieldCreateUserName);
            Assert.Equal("CreateBy", BaseUtil.FieldCreateBy);
            Assert.Equal("CreateTime", BaseUtil.FieldCreateTime);
            Assert.Equal("CreateIp", BaseUtil.FieldCreateIp);
            Assert.Equal("UpdateUserId", BaseUtil.FieldUpdateUserId);
            Assert.Equal("UpdateUserName", BaseUtil.FieldUpdateUserName);
            Assert.Equal("UpdateBy", BaseUtil.FieldUpdateBy);
            Assert.Equal("UpdateTime", BaseUtil.FieldUpdateTime);
            Assert.Equal("UpdateIp", BaseUtil.FieldUpdateIp);
        }

        [Fact]
        public void UserAndDepartmentConstants()
        {
            Assert.Equal("UserId", BaseUtil.FieldUserId);
            Assert.Equal("DepartmentId", BaseUtil.FieldDepartmentId);
            Assert.Equal("CompanyId", BaseUtil.FieldCompanyId);
        }

        [Fact]
        public void WorkflowStateConstants()
        {
            Assert.Equal("IsApproved", BaseUtil.FieldIsApproved);
            Assert.Equal("ApprovedTime", BaseUtil.FieldApprovedTime);
            Assert.Equal("IsRejected", BaseUtil.FieldIsRejected);
            Assert.Equal("IsClosed", BaseUtil.FieldIsClosed);
            Assert.Equal("IsConfirmed", BaseUtil.FieldIsConfirmed);
            Assert.Equal("IsReleased", BaseUtil.FieldIsReleased);
            Assert.Equal("IsCompleted", BaseUtil.FieldIsCompleted);
            Assert.Equal("IsFinished", BaseUtil.FieldIsFinished);
            Assert.Equal("IsCancelled", BaseUtil.FieldIsCancelled);
            Assert.Equal("IsScraped", BaseUtil.FieldIsScraped);
        }

        [Fact]
        public void StaticFields_HaveExpectedValues()
        {
            Assert.Equal(" AND ", BaseUtil.SqlLogicConditional);
            Assert.Equal("Selected", BaseUtil.SelectedColumn);
        }
    }
}
