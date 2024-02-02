//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2023, DotNet.
//-----------------------------------------------------------------

using System.Collections.Generic;
using System.Data;
using System.Reflection;


namespace DotNet.Business
{
    using IService;
    using Model;
    using Util;

    /// <summary>
    /// BaseStaffService
    /// 员工服务
    /// 
    /// 修改记录
    ///		2007.05.22 版本：1.0 JiRiGaLa 窗体与数据库连接的分离。
    ///		
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2007.05.22</date>
    /// </author> 
    /// </summary>


    public class BaseStaffService : IBaseStaffService
    {
        #region public DataTable GetAddressDataTable(BaseUserInfo userInfo, string organizationId, string searchKey)
        /// <summary>
        /// 获取内部通讯录
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="organizationId">组织机构主键</param>
        /// <param name="searchKey">查询内容</param>
        /// <returns>数据表</returns>
        public DataTable GetAddressDataTable(BaseUserInfo userInfo, string organizationId, string searchKey)
        {
            var dt = new DataTable(BaseStaffEntity.CurrentTableName);

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                var manager = new BaseStaffManager(dbHelper, userInfo);
                dt = manager.GetAddressDataTable(organizationId, searchKey);
                dt.TableName = BaseStaffEntity.CurrentTableName;
            });
            return dt;
        }
        #endregion

        #region public DataTable GetAddressDataTableByPage(BaseUserInfo userInfo, string organizationId, string searchKey, out int recordCount, int pageNo = 1, int pageSize = 100, string sort = null)
        /// <summary>
        /// 获取内部通讯录
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="organizationId">组织机构主键</param>
        /// <param name="searchKey">查询内容</param>
        /// <param name="recordCount"></param>
        /// <param name="pageSize">分页的条数</param>
        /// <param name="sort"></param>
        /// <param name="pageNo">当前页数</param>
        /// <returns>数据表</returns>
        public DataTable GetAddressDataTableByPage(BaseUserInfo userInfo, string organizationId, string searchKey, out int recordCount, int pageNo = 1, int pageSize = 100, string sort = null)
        {
            var dt = new DataTable(BaseStaffEntity.CurrentTableName);
            var myrecordCount = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                // 这里是不获取所有的列，只获取自己需要的列的标准方法
                var manager = new BaseStaffManager(dbHelper, userInfo)
                {
                    SelectFields = "Id, Code, RealName, DepartmentName, DutyName, OfficePhone, Extension, Mobile, ShortNumber, Email, QQ, Description"
                };
                dt = manager.GetAddressDataTableByPage(organizationId, searchKey, out myrecordCount, pageSize, pageNo, sort);
                dt.TableName = BaseStaffEntity.CurrentTableName;
            });
            recordCount = myrecordCount;
            return dt;
        }
        #endregion

        #region public int UpdateAddress(BaseUserInfo userInfo, BaseStaffEntity entity, out Status status, out string statusMessage) 更新通讯地址
        /// <summary>
        /// 更新通讯地址
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="entity">实体</param>
        /// <param name="status">状态</param>
        /// <param name="statusMessage">返回状态信息</param>
        /// <returns>影响行数</returns>
        public int UpdateAddress(BaseUserInfo userInfo, BaseStaffEntity entity, out Status status, out string statusMessage)
        {
            var result = 0;

            var returnCode = Status.Ok;
            var returnMessage = string.Empty;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                var manager = new BaseStaffManager(dbHelper, userInfo);
                // for (int i = 0; i < result.Rows.Count; i++)
                // {
                result += manager.UpdateAddress(entity, out returnCode);
                // }
                returnMessage = manager.GetStateMessage(returnCode);
            });
            status = returnCode;
            statusMessage = returnMessage;
            return result;
        }
        #endregion

        /// <summary>
        /// 批量更新通讯地址
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="dt">数据表</param>
        /// <returns>影响行数</returns>
        public int BatchUpdateAddress(BaseUserInfo userInfo, DataTable dt)
        {
            var result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                // 这里是只更新部分字段的例子
                foreach (DataRow dr in dt.Rows)
                {
                    var sqlBuilder = new SqlBuilder(dbHelper);
                    sqlBuilder.BeginUpdate(BaseStaffEntity.CurrentTableName);
                    // 这里是界面上显示的字段，需要更新的字段
                    sqlBuilder.SetValue(BaseStaffEntity.FieldOfficePhone, dr[BaseStaffEntity.FieldOfficePhone].ToString());
                    sqlBuilder.SetValue(BaseStaffEntity.FieldExtension, dr[BaseStaffEntity.FieldExtension].ToString());
                    sqlBuilder.SetValue(BaseStaffEntity.FieldMobile, dr[BaseStaffEntity.FieldMobile].ToString());
                    sqlBuilder.SetValue(BaseStaffEntity.FieldShortNumber, dr[BaseStaffEntity.FieldShortNumber].ToString());
                    sqlBuilder.SetValue(BaseStaffEntity.FieldEmail, dr[BaseStaffEntity.FieldEmail].ToString());
                    sqlBuilder.SetValue(BaseStaffEntity.FieldQq, dr[BaseStaffEntity.FieldQq].ToString());
                    sqlBuilder.SetValue(BaseStaffEntity.FieldDescription, dr[BaseStaffEntity.FieldDescription].ToString());
                    sqlBuilder.SetWhere(BaseStaffEntity.FieldId, dr[BaseStaffEntity.FieldId].ToString());
                    result += sqlBuilder.EndUpdate();
                }
            });
            return result;
        }

        #region public string AddStaff(BaseUserInfo userInfo, BaseStaffEntity entity, out Status status, out string statusMessage) 添加员工
        /// <summary>
        /// 添加员工
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="entity">实体</param>
        /// <param name="status">状态</param>
        /// <param name="statusMessage">返回状消息</param>
        /// <returns>主键</returns>
        public string AddStaff(BaseUserInfo userInfo, BaseStaffEntity entity, out Status status, out string statusMessage)
        {
            var result = string.Empty;

            var returnCode = Status.Ok;
            var returnMessage = string.Empty;
            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                // 1.若添加用户成功，添加员工。
                var manager = new BaseStaffManager(dbHelper, userInfo);
                result = manager.UniqueAdd(entity, out returnCode);
                returnMessage = manager.GetStateMessage(returnCode);
                // 2.自己不用给自己发提示信息，这个提示信息是为了提高工作效率的
            });
            status = returnCode;
            statusMessage = returnMessage;
            return result;
        }
        #endregion

        #region public int UpdateStaff(BaseUserInfo userInfo, BaseStaffEntity entity, out Status status, out string statusMessage) 更新员工
        /// <summary>
        /// 更新员工
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="entity">实体</param>
        /// <param name="status">状态</param>
        /// <param name="statusMessage">返回状消息</param>
        /// <returns>影响行数</returns>
        public int UpdateStaff(BaseUserInfo userInfo, BaseStaffEntity entity, out Status status, out string statusMessage)
        {
            var result = 0;

            var returnCode = Status.Ok;
            var returnMessage = string.Empty;
            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                // 3.员工的修改
                var manager = new BaseStaffManager(dbHelper, userInfo);
                result = manager.UniqueUpdate(entity, out returnCode);
                returnMessage = manager.GetStateMessage(returnCode);
            });
            status = returnCode;
            statusMessage = returnMessage;
            return result;
        }
        #endregion

        #region public DataTable GetDataTable(BaseUserInfo userInfo) 获取员工列表
        /// <summary>
        /// 获取员工列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <returns>数据表</returns>
        public DataTable GetDataTable(BaseUserInfo userInfo)
        {
            var dt = new DataTable(BaseStaffEntity.CurrentTableName);

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                var manager = new BaseStaffManager(dbHelper, userInfo);
                dt = manager.GetDataTable(new KeyValuePair<string, object>(BaseStaffEntity.FieldDeleted, 0), BaseStaffEntity.FieldSortCode);
                dt.TableName = BaseStaffEntity.CurrentTableName;
            });
            return dt;
        }
        #endregion

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="id">主键</param>
        /// <returns>实体</returns>
        public BaseStaffEntity GetEntity(BaseUserInfo userInfo, string id)
        {
            BaseStaffEntity entity = null;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                var manager = new BaseStaffManager(dbHelper, userInfo);
                entity = manager.GetEntity(id);
            });
            return entity;
        }

        #region public DataTable GetDataTableByIds(BaseUserInfo userInfo, string[] ids) 获取员工列表
        /// <summary>
        /// 获取员工列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="ids">组织主键</param>
        /// <returns>数据表</returns>
        public DataTable GetDataTableByIds(BaseUserInfo userInfo, string[] ids)
        {
            var dt = new DataTable(BaseStaffEntity.CurrentTableName);

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                var manager = new BaseStaffManager(dbHelper, userInfo);
                dt = manager.GetDataTable(BaseStaffEntity.FieldId, ids, BaseStaffEntity.FieldSortCode);
                dt.TableName = BaseStaffEntity.CurrentTableName;
            });
            return dt;
        }
        #endregion

        #region public DataTable GetDataTableByCompany(BaseUserInfo userInfo, string companyId, bool containChildren) 按公司获取部门员工
        /// <summary>
        /// 按公司获取部门员工
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="companyId">公司主键</param>
        /// <param name="containChildren">含子部门</param>
        /// <returns>数据表</returns>
        public DataTable GetDataTableByCompany(BaseUserInfo userInfo, string companyId, bool containChildren)
        {
            var dt = new DataTable(BaseStaffEntity.CurrentTableName);

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                var manager = new BaseStaffManager(dbHelper, userInfo);
                if (containChildren)
                {
                    dt = manager.GetChildrenStaffs(companyId);
                }
                else
                {
                    dt = manager.GetDataTableByCompany(companyId);
                }
                dt.TableName = BaseStaffEntity.CurrentTableName;
            });
            return dt;
        }
        #endregion

        #region public DataTable GetDataTableByDepartment(BaseUserInfo userInfo, string departmentId, bool containChildren) 按部门获取员工列表
        /// <summary>
        /// 按部门获取员工列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="departmentId">部门主键</param>
        /// <param name="containChildren">含子部门</param>
        /// <returns>数据表</returns>
        public DataTable GetDataTableByDepartment(BaseUserInfo userInfo, string departmentId, bool containChildren)
        {
            var dt = new DataTable(BaseStaffEntity.CurrentTableName);

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                var manager = new BaseStaffManager(dbHelper, userInfo);
                if (containChildren)
                {
                    dt = manager.GetChildrenStaffs(departmentId);
                }
                else
                {
                    dt = manager.GetDataTableByDepartment(departmentId);
                }
                dt.TableName = BaseStaffEntity.CurrentTableName;
            });
            return dt;
        }
        #endregion

        #region public DataTable GetDataTableByOrganization(BaseUserInfo BaseUserInfo, string organizationId, bool containChildren) 按公司获取员工列表
        /// <summary>
        /// 按公司获取员工列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="organizationId">组织主键</param>
        /// <param name="containChildren">含子部门</param>
        /// <returns>数据表</returns>
        public DataTable GetDataTableByOrganization(BaseUserInfo userInfo, string organizationId, bool containChildren)
        {
            var dt = new DataTable(BaseStaffEntity.CurrentTableName);
            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                // 用户已经不存在的需要整理干净，防止数据不完整。
                var sb = PoolUtil.StringBuilder.Get();
                sb.Append("UPDATE " + BaseStaffEntity.CurrentTableName + " SET UserId = NULL WHERE (UserId NOT IN (SELECT Id FROM " + BaseUserEntity.CurrentTableName + "))");
                dbHelper.ExecuteNonQuery(sb.Return());
                dbHelper.Close();
                // 这里是读取的服务器连接
                dbHelper.Open(BaseSystemInfo.UserCenterReadDbConnection);
                var manager = new BaseStaffManager(dbHelper, userInfo);
                if (containChildren)
                {
                    dt = manager.GetChildrenStaffs(organizationId);
                }
                else
                {
                    dt = manager.GetDataTableByOrganization(organizationId);
                }
                dt.TableName = BaseStaffEntity.CurrentTableName;
            });
            return dt;
        }
        #endregion

        /// <summary>
        /// 获取下属员工
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        /// <param name="organizationId"></param>
        /// <returns></returns>
        public DataTable GetChildrenStaffs(BaseUserInfo userInfo, string organizationId)
        {
            var dt = new DataTable(BaseStaffEntity.CurrentTableName);

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                // 获得组织机构列表
                var manager = new BaseStaffManager(dbHelper, userInfo);
                dt = manager.GetChildrenStaffs(organizationId);
                dt.DefaultView.Sort = BaseStaffEntity.FieldSortCode;
                dt.TableName = BaseStaffEntity.CurrentTableName;
            });
            return dt;
        }
        /// <summary>
        /// 获取父级下属员工
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        /// <param name="organizationId"></param>
        /// <returns></returns>
        public DataTable GetParentChildrenStaffs(BaseUserInfo userInfo, string organizationId)
        {
            var dt = new DataTable(BaseStaffEntity.CurrentTableName);

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                // 获得组织机构列表
                var manager = new BaseStaffManager(dbHelper, userInfo);
                dt = manager.GetParentChildrenStaffs(organizationId);
                dt.DefaultView.Sort = BaseStaffEntity.FieldSortCode;
                dt.TableName = BaseStaffEntity.CurrentTableName;
            });
            return dt;
        }

        #region public DataTable Search(BaseUserInfo userInfo, string organizationId, string searchKey) 获得员工列表
        /// <summary>
        /// 获得员工列表
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="organizationId">组织主键</param>
        /// <param name="searchKey">查询</param>
        /// <returns>数据表</returns>
        public DataTable Search(BaseUserInfo userInfo, string organizationId, string searchKey)
        {
            var dt = new DataTable(BaseStaffEntity.CurrentTableName);

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                var manager = new BaseStaffManager(dbHelper, userInfo);
                dt = manager.Search(organizationId, searchKey, false);
                dt.TableName = BaseStaffEntity.CurrentTableName;
            });
            return dt;
        }
        #endregion

        #region public int SetStaffUser(BaseUserInfo userInfo, string staffId, string userId)
        /// <summary>
        /// 员工关联用户
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="staffId">员工主键</param>
        /// <param name="userId">用户主键</param>
        /// <returns>影响行数</returns>
        public int SetStaffUser(BaseUserInfo userInfo, string staffId, string userId)
        {
            var result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                var manager = new BaseStaffManager(dbHelper, userInfo);
                if (ValidateUtil.IsInt(userId))
                {
                    result = manager.Update(staffId, new KeyValuePair<string, object>(BaseStaffEntity.FieldUserId, userId));
                }
                else
                {
                    // 一个用户只能帮定到一个帐户上，检查是否已经绑定过这个用户了。
                    var staffIds = manager.GetIds(new KeyValuePair<string, object>(BaseStaffEntity.FieldUserId, userId), new KeyValuePair<string, object>(BaseStaffEntity.FieldDeleted, 0));
                    if (staffIds == null || staffIds.Length == 0)
                    {
                        result = manager.Update(staffId, new KeyValuePair<string, object>(BaseStaffEntity.FieldUserId, userId));
                        var userManager = new BaseUserManager(dbHelper, userInfo);
                        var userEntity = userManager.GetEntity(userId);
                        result = manager.Update(staffId, new KeyValuePair<string, object>(BaseStaffEntity.FieldUserName, userEntity.UserName));
                    }
                }
            });
            return result;
        }
        #endregion

        /// <summary>
        /// 同步数据
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="all">同步所有数据</param>
        /// <returns>影响行数</returns>
        public int Synchronous(BaseUserInfo userInfo, bool all = false)
        {
            var result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                var staffManager = new BaseStaffManager(dbHelper, userInfo);
                // result = staffManager.Synchronous(all);
            });

            return result;
        }

        #region public int DeleteUser(BaseUserInfo userInfo, string staffId)
        /// <summary>
        /// 删除员工关联的用户
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="staffId">员工主键</param>
        /// <returns>影响行数</returns>
        public int DeleteUser(BaseUserInfo userInfo, string staffId)
        {
            var result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                var manager = new BaseStaffManager(dbHelper, userInfo);
                result = manager.DeleteUser(staffId);
            });
            return result;
        }
        #endregion

        #region public int Delete(BaseUserInfo userInfo, string id) 单个删除
        /// <summary>
        /// 单个删除
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="id">主键</param>
        /// <returns>影响行数</returns>
        public int Delete(BaseUserInfo userInfo, string id)
        {
            var result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                var manager = new BaseStaffManager(dbHelper, userInfo);
                result = manager.Delete(id);
            });
            return result;
        }
        #endregion

        #region public int BatchDelete(BaseUserInfo userInfo, string[] ids) 批量删除
        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="ids">主键数组</param>
        /// <returns>影响行数</returns>
        public int BatchDelete(BaseUserInfo userInfo, string[] ids)
        {
            var result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                var manager = new BaseStaffManager(dbHelper, userInfo);
                result = manager.BatchDelete(ids);
            });

            return result;
        }
        #endregion

        #region public int SetDeleted(BaseUserInfo userInfo, string[] ids) 批量设置删除
        /// <summary>
        /// 批量设置删除
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="ids">主键数组</param>
        /// <returns>影响行数</returns>
        public int SetDeleted(BaseUserInfo userInfo, string[] ids)
        {
            var result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                var userManager = new BaseUserManager(dbHelper, userInfo);
                var staffManager = new BaseStaffManager(dbHelper, userInfo);
                BaseStaffEntity staffEntity = null;
                for (var i = 0; i < ids.Length; i++)
                {
                    // 删除相应的用户
                    staffEntity = staffManager.GetEntity(ids[i]);
                    if (staffEntity != null && staffEntity.UserId > 0)
                    {
                        userManager.SetDeleted(staffEntity.UserId);
                    }
                    // 删除职员
                    result = staffManager.SetDeleted(ids[i], true, true);
                }
            });
            return result;
        }
        #endregion

        #region public int MoveTo(BaseUserInfo userInfo, string id, string organizationId) 移动数据
        /// <summary>
        /// 移动数据
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="id">组织机构主键数组</param>
        /// <param name="organizationId">父结点主键</param>
        /// <returns>影响行数</returns>
        public int MoveTo(BaseUserInfo userInfo, string id, string organizationId)
        {
            var result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                var manager = new BaseStaffManager(dbHelper, userInfo);
                result = manager.Update(id, new KeyValuePair<string, object>(BaseStaffEntity.FieldDepartmentId, organizationId));
            });

            return result;
        }
        #endregion

        #region public int BatchMoveTo(BaseUserInfo userInfo, string[] ids, string organizationId) 移动数据
        /// <summary>
        /// 批量移动数据
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="ids">组织机构主键数组</param>
        /// <param name="organizationId">父结点主键</param>
        /// <returns>影响行数</returns>
        public int BatchMoveTo(BaseUserInfo userInfo, string[] ids, string organizationId)
        {
            var result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                var manager = new BaseStaffManager(dbHelper, userInfo);
                for (var i = 0; i < ids.Length; i++)
                {
                    result += manager.Update(ids[i], new KeyValuePair<string, object>(BaseStaffEntity.FieldDepartmentId, organizationId));
                }
            });
            return result;
        }
        #endregion

        #region public int ChangeDepartment(BaseUserInfo userInfo, string id, string CompanyId, string DepartmentId) 部门变动
        /// <summary>
        /// 部门变动
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="id">员工主键</param>
        /// <param name="companyId">单位主键</param>
        /// <param name="departmentId">部门主键</param>
        /// <returns>影响行数</returns>
        public int ChangeDepartment(BaseUserInfo userInfo, string id, string companyId, string departmentId)
        {
            var result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                var manager = new BaseStaffManager(dbHelper, userInfo);
                var parameters = new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>(BaseStaffEntity.FieldCompanyId, companyId),
                    new KeyValuePair<string, object>(BaseStaffEntity.FieldDepartmentId, departmentId)
                };
                result = manager.Update(new KeyValuePair<string, object>(BaseStaffEntity.FieldId, id), parameters);
            });

            return result;
        }
        #endregion

        #region public int BatchSave(BaseUserInfo userInfo, DataTable result) 批量保存员工
        /// <summary>
        /// 批量保存员工
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="dt">数据表</param>
        /// <returns>影响行数</returns>
        public int BatchSave(BaseUserInfo userInfo, DataTable dt)
        {
            var result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                var manager = new BaseStaffManager(dbHelper, userInfo);
                result = manager.BatchSave(dt);
                // ReturnDataTable = Staff.GetDataTableByOrganization(organizationId);
            });
            return result;
        }
        #endregion

        #region public int ResetSortCode(BaseUserInfo userInfo) 重新排序数据
        /// <summary>
        /// 重新排序数据
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <returns>影响行数</returns>
        public int ResetSortCode(BaseUserInfo userInfo)
        {
            var result = 0;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterWriteDb(userInfo, parameter, (dbHelper) =>
            {
                var manager = new BaseStaffManager(dbHelper);
                result = manager.ResetSortCode();
            });
            return result;
        }
        #endregion

        #region  public string GetId(BaseUserInfo userInfo,string name, object value) 获取主键
        /// <summary>
        /// 获取主键
        /// </summary>
        /// <param name="userInfo">用户</param>
        /// <param name="name">查询的参数</param>
        /// <param name="value">参数值</param>
        /// <returns>影响行数</returns>
        public string GetId(BaseUserInfo userInfo, string name, object value)
        {
            var result = string.Empty;

            var parameter = ServiceInfo.Create(userInfo, MethodBase.GetCurrentMethod());
            ServiceUtil.ProcessUserCenterReadDb(userInfo, parameter, (dbHelper) =>
            {
                var manager = new BaseStaffManager(dbHelper);
                result = manager.GetId(new KeyValuePair<string, object>(name, value));
            });
            return result;
        }
        #endregion
    }
}