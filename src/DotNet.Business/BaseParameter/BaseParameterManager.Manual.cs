//-----------------------------------------------------------------------
// <copyright file="BaseParameterManager.cs" company="DotNet">
//     Copyright (c) 2025, All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Data;
using System.Data.Common;
using System.Collections.Generic;

namespace DotNet.Business
{
    using Model;
    using Business;
    using Util;

    /// <summary>
    /// BaseParameterManager
    /// 参数管理层
    /// 
    /// 修改记录
    /// 
    ///	2021-10-07 版本：1.0 Troy.Cui 创建文件。
    ///		
    /// <author>
    ///	<name>Troy.Cui</name>
    ///	<date>2021-10-07</date>
    /// </author> 
    /// </summary>
    public partial class BaseParameterManager : BaseManager
    {
        #region public override DataTable GetDataTableByPage(string companyId, string departmentId, string userId, string startTime, string endTime, string searchKey, out int recordCount, int pageNo = 1, int pageSize = 20, string sortExpression = BaseParameterEntity.FieldCreateTime, string sortDirection = "DESC", bool showDisabled = false, bool showDeleted = false)
        /// <summary>
        /// 按条件分页查询(带记录状态Enabled和删除状态Deleted)
        /// </summary>
        /// <param name="companyId">查看公司主键</param>
        /// <param name="departmentId">查看部门主键</param>
        /// <param name="userId">查看用户主键</param>
        /// <param name="startTime">创建开始时间</param>
        /// <param name="endTime">创建结束时间</param>
        /// <param name="searchKey">查询关键字</param>
        /// <param name="recordCount">记录数</param>
        /// <param name="pageNo">当前页</param>
        /// <param name="pageSize">每页显示</param>
        /// <param name="sortExpression">排序字段</param>
        /// <param name="sortDirection">排序方向</param>
        /// <param name="showDisabled">是否显示无效记录</param>
        /// <param name="showDeleted">是否显示已删除记录</param>
        /// <param name="systemCode">子系统编码</param>
        /// <param name="parameterId">参数编号</param>
        /// <param name="parameterCode">参数编码</param>
        /// <returns>数据表</returns>
        public DataTable GetDataTableByPage(string companyId, string departmentId, string userId, string startTime, string endTime, string searchKey, out int recordCount, int pageNo = 1, int pageSize = 20, string sortExpression = BaseParameterEntity.FieldCreateTime, string sortDirection = "DESC", bool showDisabled = true, bool showDeleted = true, string systemCode = null, string parameterId = null, string parameterCode = null)
        {
            var sb = PoolUtil.StringBuilder.Get().Append(" 1 = 1");
            //是否显示无效记录
            if (!showDisabled)
            {
                sb.Append(" AND " + BaseParameterEntity.FieldEnabled + " = 1");
            }
            //是否显示已删除记录
            if (!showDeleted)
            {
                sb.Append(" AND " + BaseParameterEntity.FieldDeleted + " = 0");
            }

            if (ValidateUtil.IsInt(companyId))
            {
                //sb.Append(" AND " + BaseParameterEntity.FieldCompanyId + " = " + companyId);
            }
            // 只有管理员才能看到所有的
            //if (!(UserInfo.IsAdministrator && BaseSystemInfo.AdministratorEnabled))
            //{
            //sb.Append(" AND (" + BaseParameterEntity.FieldUserCompanyId + " = 0 OR " + BaseParameterEntity.FieldUserCompanyId + " = " + UserInfo.CompanyId + ")");
            //}
            if (ValidateUtil.IsInt(departmentId))
            {
                //sb.Append(" AND " + BaseParameterEntity.FieldDepartmentId + " = " + departmentId);
            }
            if (ValidateUtil.IsInt(userId))
            {
                //sb.Append(" AND " + BaseParameterEntity.FieldUserId + " = " + userId);
            }
            //创建时间
            if (ValidateUtil.IsDateTime(startTime))
            {
                sb.Append(" AND " + BaseParameterEntity.FieldCreateTime + " >= " + dbHelper.ToDbTime(startTime));
            }
            if (ValidateUtil.IsDateTime(endTime))
            {
                sb.Append(" AND " + BaseParameterEntity.FieldCreateTime + " <= " + dbHelper.ToDbTime(endTime.ToDateTime().Date.AddDays(1).AddMilliseconds(-1)));
            }
            if (!string.IsNullOrEmpty(systemCode))
            {
                systemCode = dbHelper.SqlSafe(systemCode);
                sb.Append(" AND " + BaseParameterEntity.FieldSystemCode + " = N'" + systemCode + "'");
            }
            if (!string.IsNullOrEmpty(parameterId))
            {
                parameterId = dbHelper.SqlSafe(parameterId);
                sb.Append(" AND " + BaseParameterEntity.FieldParameterId + " = N'" + parameterId + "'");
            }
            if (!string.IsNullOrEmpty(parameterCode))
            {
                parameterCode = dbHelper.SqlSafe(parameterCode);
                sb.Append(" AND " + BaseParameterEntity.FieldParameterCode + " = N'" + parameterCode + "'");
            }
            if (!string.IsNullOrEmpty(searchKey))
            {
                searchKey = StringUtil.GetLikeSearchKey(dbHelper.SqlSafe(searchKey));
                sb.Append(" AND (" + BaseParameterEntity.FieldParameterCode + " LIKE N'%" + searchKey + "%' OR " + BaseParameterEntity.FieldDescription + " LIKE N'%" + searchKey + "%')");
            }
            sb.Replace(" 1 = 1 AND ", " ");
            return GetDataTableByPage(out recordCount, pageNo, pageSize, sortExpression, sortDirection, CurrentTableName, sb.Return());
        }
        #endregion

        #region 获取当前用户的所有用户参数
        /// <summary>
        /// 获取当前用户的所有用户参数
        /// </summary>
        /// <returns></returns>
        public List<KeyValuePair<string, string>> GetUserParameterList()
        {
            var result = new List<KeyValuePair<string, string>>();
            var whereParameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseParameterEntity.FieldDeleted, 0),
                new KeyValuePair<string, object>(BaseParameterEntity.FieldEnabled, 1)
            };
            if (UserInfo != null)
            {
                whereParameters.Add(new KeyValuePair<string, object>(BaseParameterEntity.FieldParameterId, UserInfo.Id));
            }
            var list = GetList<BaseParameterEntity>(whereParameters, order: BaseParameterEntity.FieldParameterCode);
            foreach (var entity in list)
            {
                result.Add(new KeyValuePair<string, string>(entity.ParameterCode, entity.ParameterContent));
            }

            return result;
        }
        #endregion

        #region public string UniqueAdd(BaseParameterEntity entity)
        /// <summary>
        /// 添加内容
        /// </summary>
        /// <param name="entity">内容</param>
        /// <returns>主键</returns>
        public string UniqueAdd(BaseParameterEntity entity)
        {
            var result = string.Empty;

            // 此处检查this.exist()
            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseParameterEntity.FieldCategoryCode, entity.CategoryCode),
                new KeyValuePair<string, object>(BaseParameterEntity.FieldParameterId, entity.ParameterId),
                new KeyValuePair<string, object>(BaseParameterEntity.FieldParameterCode, entity.ParameterCode),
                new KeyValuePair<string, object>(BaseParameterEntity.FieldParameterContent, entity.ParameterContent),
                new KeyValuePair<string, object>(BaseParameterEntity.FieldDeleted, 0),
                new KeyValuePair<string, object>(BaseParameterEntity.FieldEnabled, 1)
            };
            // 2015-12-22 吉日嘎拉 检查没有删除的，已删除的当日志用了
            if (Exists(parameters))
            {
                // 编号已重复
                Status = Status.ErrorCodeExist;
                StatusCode = Status.ErrorCodeExist.ToString();
            }
            else
            {
                result = AddEntity(entity);
                if (!string.IsNullOrEmpty(result))
                {
                    // 运行成功
                    Status = Status.OkAdd;
                    StatusCode = Status.OkAdd.ToString();
                }
                else
                {
                    // 失败
                    Status = Status.Error;
                    StatusCode = Status.Error.ToString();
                }
            }

            return result;
        }
        #endregion

        #region public int UniqueUpdate(BaseParameterEntity entity) 更新
        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="entity">参数基类表结构定义</param>
        /// <returns>影响行数</returns>
        public int UniqueUpdate(BaseParameterEntity entity)
        {
            var result = 0;
            // 检查是否已被其他人修改
            //if (DbUtil.IsModifed(DbHelper, BaseParameterEntity.CurrentTableName, parameterEntity.Id, parameterEntity.UpdateUserId, parameterEntity.UpdateTime))
            //{
            //    // 数据已经被修改
            //    this.StatusCode = StatusCode.ErrorChanged.ToString();
            //}
            //else
            //{
            // 检查编号是否重复
            if (Exists(new KeyValuePair<string, object>(BaseParameterEntity.FieldParameterCode, entity.ParameterCode), entity.Id))
            {
                // 编码已重复
                Status = Status.ErrorCodeExist;
                StatusCode = Status.ErrorCodeExist.ToString();
            }
            else
            {
                // 进行更新操作
                result = UpdateEntity(entity);
                if (result == 1)
                {
                    Status = Status.OkUpdate;
                    StatusCode = Status.OkUpdate.ToString();
                }
                else
                {
                    // 数据可能被删除
                    Status = Status.ErrorDeleted;
                    StatusCode = Status.ErrorDeleted.ToString();
                }
            }
            // }
            return result;
        }
        #endregion

        #region public string GetParameter(string tableName, string categoryCode, string parameterId, string parameterCode)
        /// <summary>
        /// 获取参数
        /// 2015-07-24 吉日嘎拉，获取最新的一条，增加排序字段
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="categoryCode">类别编号</param>
        /// <param name="parameterId">参数主键</param>
        /// <param name="parameterCode">编码</param>
        /// <returns>参数值</returns>
        public string GetParameter(string tableName, string categoryCode, string parameterId, string parameterCode)
        {
            CurrentTableName = tableName;

            //var parameters = new List<KeyValuePair<string, object>>
            //{
            //    new KeyValuePair<string, object>(BaseParameterEntity.FieldCategoryCode, categoryCode),
            //    new KeyValuePair<string, object>(BaseParameterEntity.FieldParameterId, parameterId),
            //    new KeyValuePair<string, object>(BaseParameterEntity.FieldParameterCode, parameterCode),
            //    new KeyValuePair<string, object>(BaseParameterEntity.FieldDeleted, 0)
            //};
            //return GetProperty(parameters, BaseParameterEntity.FieldParameterContent, BaseParameterEntity.FieldCreateTime + " DESC");

            var result = string.Empty;
            var sb = PoolUtil.StringBuilder.Get().Append(" 1 = 1");
            if (!string.IsNullOrEmpty(categoryCode))
            {
                sb.Append(" AND " + BaseParameterEntity.FieldCategoryCode + " = '" + categoryCode + "'");
            }
            if (!string.IsNullOrEmpty(parameterId))
            {
                sb.Append(" AND " + BaseParameterEntity.FieldParameterId + " = '" + parameterId + "'");
            }
            if (!string.IsNullOrEmpty(parameterCode))
            {
                sb.Append(" AND " + BaseParameterEntity.FieldParameterCode + " = '" + parameterCode + "'");
            }

            var cacheKey = "Dt." + CurrentTableName;
            //万一系统里开的公司多了，还是不要按公司来分
            //if (UserInfo != null)
            //{
            //    cacheKey += "." + UserInfo.CompanyId;
            //}
            var cacheTime = TimeSpan.FromMilliseconds(86400000);
            var parameters = new List<KeyValuePair<string, object>>
            {
                //这里只要有效的，没被删除的
                new KeyValuePair<string, object>(BaseParameterEntity.FieldEnabled, 1),
                new KeyValuePair<string, object>(BaseParameterEntity.FieldDeleted, 0)
            };
            var dt = CacheUtil.Cache<DataTable>(cacheKey, () => GetDataTable(parameters), true, false, cacheTime);
            //查找
            sb.Replace(" 1 = 1 AND ", " ");
            var drs = dt.Select(sb.Return());
            if (drs.Length > 0)
            {
                result = drs[0][BaseParameterEntity.FieldParameterContent].ToString();
            }

            return result;
        }
        #endregion

        #region public string GetParameter(string categoryCode, string parameterId, string parameterCode)
        /// <summary>
        /// 获取参数
        /// </summary>
        /// <param name="categoryCode">类别编号</param>
        /// <param name="parameterId">参数主键</param>
        /// <param name="parameterCode">编码</param>
        /// <returns>参数值</returns>
        public string GetParameter(string categoryCode, string parameterId, string parameterCode)
        {
            return GetParameter(BaseParameterEntity.CurrentTableName, categoryCode, parameterId, parameterCode);
        }
        #endregion

        #region public int SetParameter(string tableName, string categoryCode, string parameterId, string parameterCode, string parameterContent)

        /// <summary>
        /// 更新参数设置
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="categoryCode">类别编号</param>
        /// <param name="parameterId">参数主键</param>
        /// <param name="parameterCode">编码</param>
        /// <param name="parameterContent">参数内容</param>
        /// <returns>影响行数</returns>
        public int SetParameter(string tableName, string categoryCode, string parameterId, string parameterCode, string parameterContent)
        {
            var result = 0;

            CurrentTableName = tableName;

            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseParameterEntity.FieldCategoryCode, categoryCode),
                new KeyValuePair<string, object>(BaseParameterEntity.FieldParameterId, parameterId),
                new KeyValuePair<string, object>(BaseParameterEntity.FieldParameterCode, parameterCode),
                new KeyValuePair<string, object>(BaseParameterEntity.FieldDeleted, 0),
                new KeyValuePair<string, object>(BaseParameterEntity.FieldEnabled, 1)
            };
            // 检测是否无效数据
            if ((parameterContent == null) || (parameterContent.Length == 0))
            {
                result = Delete(parameters);
            }
            else
            {
                // 检测是否存在
                result = Update(parameters, new KeyValuePair<string, object>(BaseParameterEntity.FieldParameterContent, parameterContent));
                if (result == 0)
                {
                    // 进行增加操作
                    var entity = new BaseParameterEntity
                    {
                        SystemCode = BaseSystemInfo.SystemCode,
                        CategoryCode = categoryCode,
                        ParameterId = parameterId,
                        ParameterCode = parameterCode,
                        ParameterContent = parameterContent,
                        Enabled = 1,
                        Deleted = 0,
                        SortCode = 1 // 不要排序了
                    };
                    UniqueAdd(entity);
                    result = 1;
                }
            }
            //移除缓存
            RemoveCache();
            return result;
        }
        #endregion

        #region public int SetParameter(string categoryCode, string parameterId, string parameterCode, string parameterContent)
        /// <summary>
        /// 更新参数设置
        /// </summary>
        /// <param name="categoryCode">类别编号</param>
        /// <param name="parameterId">参数主键</param>
        /// <param name="parameterCode">编码</param>
        /// <param name="parameterContent">参数内容</param>
        /// <returns>影响行数</returns>
        public int SetParameter(string categoryCode, string parameterId, string parameterCode, string parameterContent)
        {
            return SetParameter(BaseParameterEntity.CurrentTableName, categoryCode, parameterId, parameterCode, parameterContent);
        }
        #endregion

        #region public DataTable GetSystemParameter()
        /// <summary>
        /// 获取记录
        /// </summary>
        /// <returns>数据表</returns>
        public DataTable GetSystemParameter()
        {
            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseParameterEntity.FieldCategoryCode, "System"),
                new KeyValuePair<string, object>(BaseParameterEntity.FieldDeleted, 0),
                new KeyValuePair<string, object>(BaseParameterEntity.FieldEnabled, 1)
            };
            return GetDataTable(parameters);
        }
        #endregion

        #region public DataTable GetDataTableByParameter(string categoryCode, string parameterId)
        /// <summary>
        /// 获取记录
        /// 2015-12-22 吉日嘎拉 被删除的不显示出来
        /// </summary>
        /// <param name="categoryCode">类别编号</param>
        /// <param name="parameterId">参数主键</param>
        /// <returns>数据表</returns>
        public DataTable GetDataTableByParameter(string categoryCode, string parameterId)
        {
            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseParameterEntity.FieldCategoryCode, categoryCode),
                new KeyValuePair<string, object>(BaseParameterEntity.FieldParameterId, parameterId),
                new KeyValuePair<string, object>(BaseParameterEntity.FieldDeleted, 0)
            };
            return GetDataTable(parameters);
        }
        #endregion

        #region public DataTable GetDataTableParameterCode(string categoryCode, string parameterId, string parameterCode)
        /// <summary>
        /// 获取记录
        /// </summary>
        /// <param name="categoryCode">类别编号</param>
        /// <param name="parameterId">参数主键</param>
        /// <param name="parameterCode">编码</param>
        /// <returns>数据表</returns>
        public DataTable GetDataTableParameterCode(string categoryCode, string parameterId, string parameterCode)
        {
            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseParameterEntity.FieldCategoryCode, categoryCode),
                new KeyValuePair<string, object>(BaseParameterEntity.FieldParameterId, parameterId),
                new KeyValuePair<string, object>(BaseParameterEntity.FieldParameterCode, parameterCode)
            };
            return GetDataTable(parameters, BaseParameterEntity.FieldCreateTime);
        }
        #endregion

        #region public int DeleteByParameter(string categoryCode, string parameterId)
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="categoryCode">类别编号</param>
        /// <param name="parameterId">参数主键</param>
        /// <returns>影响行数</returns>
        public int DeleteByParameter(string categoryCode, string parameterId)
        {
            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseParameterEntity.FieldCategoryCode, categoryCode),
                new KeyValuePair<string, object>(BaseParameterEntity.FieldParameterId, parameterId)
            };
            return Delete(parameters);
        }
        #endregion

        #region public int DeleteByParameterCode(string categoryCode, string parameterId, string parameterCode)
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="categoryCode">类别编号</param>
        /// <param name="parameterId">参数主键</param>
        /// <param name="parameterCode">参数编号</param>
        /// <returns>影响行数</returns>
        public int DeleteByParameterCode(string categoryCode, string parameterId, string parameterCode)
        {
            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseParameterEntity.FieldCategoryCode, categoryCode),
                new KeyValuePair<string, object>(BaseParameterEntity.FieldParameterId, parameterId),
                new KeyValuePair<string, object>(BaseParameterEntity.FieldParameterCode, parameterCode)
            };
            return Delete(parameters);
        }
        #endregion

        #region public int Delete(string categoryCode, string parameterId, string parameterCode, string parameterContent)
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="categoryCode">类别编号</param>
        /// <param name="parameterId">参数主键</param>
        /// <param name="parameterCode">参数编号</param>
        /// <param name="parameterContent"></param>
        /// <returns>影响行数</returns>
        public int Delete(string categoryCode, string parameterId, string parameterCode, string parameterContent)
        {
            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseParameterEntity.FieldCategoryCode, categoryCode),
                new KeyValuePair<string, object>(BaseParameterEntity.FieldParameterId, parameterId),
                new KeyValuePair<string, object>(BaseParameterEntity.FieldParameterCode, parameterCode),
                new KeyValuePair<string, object>(BaseParameterEntity.FieldParameterContent, parameterContent)
            };
            return Delete(parameters);
        }
        #endregion
    }
}
