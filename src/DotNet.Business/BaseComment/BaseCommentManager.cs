//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;

namespace DotNet.Business
{
    using Model;
    using Util;

    /// <summary>
    ///BaseCommentManager
    /// 评论的基类
    /// 
    /// 考虑了多数据问题，但是还没有解决好。
    /// 是否使用存储过程标志改进
    /// 表名，字段名映射改进，可以更换表名，字段名
    /// 方法修改为静态方法，调用简单化
    /// 添加的参数名也为字段名一致
    /// 修改了是否处理的默认值为-1，这样方便切换是否处理这个状态。
    /// 
    /// 修改记录
    ///     2012.10.09 版本：4.7 zgl            修改string.format sql语句拼接
    ///		2009.10.30 版本：4.6 JiRiGaLa	    TableName 可以自由设置的功能。
    ///		2006.02.05 版本：4.6 JiRiGaLa	    重新调整主键的规范化。
    ///		2005.08.13 版本：1.0 JiRiGaLa		将缺少的主键彻底完善好。
    ///		2005.07.11 版本：1.0 JiRiGaLa		主键排版更友善化。
    ///		2005.02.02 版本：1.0 JiRiGaLa		主键进行改进，规范化。
    ///		2004.07.23 版本：1.0 JiRiGaLa		标准接口进行了改进，来了装箱拆箱的Object技术，以前也是一直想玩，这次真的玩了感觉不错。
    ///											增加大分类为:功能提示Prompt,待审核WaitingAudit，代办事件Todolist，备注Memo。
    ///											修改了方法的排序顺序、这样方便大规模生产、复制粘贴、然后修改修改程序就可以了。
    ///											Worked 需要修改，所以为了简单期间用了1、-1两个状态。
    ///											其实整个系统里ID字段是不可以随便变的，一定要保持一致，除非有特殊需要。
    ///		2004.07.21 版本：1.0 JiRiGaLa		添加异常处理，让程序更加完美了一些，哎程序想写好，真是无止境。
    ///											增加GetProperty、SetProperty两个方法部分，同时增加了接口定义部分。
    ///		2004.07.20 版本：1.0 JiRiGaLa		分类ID字段修改为CategoryId，这样更符合更专业一点。
    ///											优先等级也修改为PriorityId，因为ID是唯一字段，Code为客户自定义字段。
    ///											出于外数据库外键的考虑，还是修改为ID合适，不然无法做数据库完整性校验。
    ///											方法的参数改进，程序的错误隐患少了很多，主键也进一步整理了。
    ///		2004.07.20 版本：1.0 JiRiGaLa		被修改的评论用最新产生的ID，这样可以解决被修改的不被发现的问题，有点创新啊。
    ///											同时2个人修改同一个评论的问题，虽然第二个问题发生的概率几乎是零。
    ///		2004.03.31 版本：1.0 JiRiGaLa		修改一些方法，修改发布评论的人为ID，因为以后有同名可能性,
    ///											添加主键折叠方法，添加最后修改人字段，IPAddress地址等．
    ///											尽量去掉SqlServer独有的方法，尽量满足能支持多数据的需求．
    ///		2004.03.22 版本：1.0 JiRiGaLa		变量名等进行修改，使用表结构定义等
    /// 
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2006.02.05</date>
    /// </author> 
    /// </summary>
    public partial class BaseCommentManager : BaseManager, IBaseManager
    {
        #region public string Add(string categoryCode, string objectId, string contents, string ipAddress) 添加评论
        /// <summary>
        /// 添加评论
        /// </summary>
        /// <param name="categoryCode">分类区分</param>
        /// <param name="objectId">标志区分</param>
        /// <param name="contents">内容</param>
        /// <param name="ipAddress">IP地址</param>
        /// <returns>评论主键</returns>
        public string Add(string categoryCode, string objectId, string contents, string ipAddress)
        {
            var commentEntity = new BaseCommentEntity
            {
                CreateUserId = UserInfo.Id,
                CategoryCode = categoryCode,
                ObjectId = objectId,
                Contents = contents,
                DeletionStateCode = 0,
                Enabled = 0,
                IpAddress = ipAddress,
                CreateBy = UserInfo.RealName
            };
            return Add(commentEntity, false, false);
        }
        #endregion

        #region public int Update(string id, string categoryId, string title, string content, bool worked, string priorityId, bool important) 更新评论
        /// <summary>
        /// 更新评论
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="categoryCode">分类区分</param>
        /// <param name="title">标题</param>
        /// <param name="contents">内容</param>
        /// <param name="worked">是否处理过</param>
        /// <param name="priorityId">优先级别</param>
        /// <param name="important">重要性</param>
        /// <returns>影响行数</returns>
        public int Update(string id, string categoryCode, string title, string contents, bool worked, string priorityId, bool important)
        {
            var commentEntity = new BaseCommentEntity
            {
                Id = id,
                ModifiedUserId = UserInfo.Id,
                CategoryCode = categoryCode,
                Title = title,
                Contents = contents
            };
            // commentEntity.Worked = worked;
            // commentEntity.PriorityId = priorityId;
            // commentEntity.Important = important;
            return Update(commentEntity);
        }
        #endregion

        #region public int ChangeWorked(string id) 更新处理状态
        /// <summary>
        /// 更新处理状态
        /// </summary>
        /// <param name="id">评论主键</param>
        /// <returns>影响行数</returns>
        public int ChangeWorked(string id)
        {
            var result = 0;
            /*
            string sql = "UPDATE " + BaseCommentEntity.TableName
                            + "    SET " + BaseCommentEntity.FieldWorked + " = (CASE " + BaseCommentEntity.FieldWorked + " WHEN 0 THEN 1 WHEN 1 THEN 0 END)"
                            + "  WHERE (" + BaseCommentEntity.FieldId + " = ? )";
            string[] names = new string[1];
            Object[] values = new Object[1];
            names[0] = BaseCommentEntity.FieldId;
            values[0] = id;
            result = DbHelper.ExecuteNonQuery(sql, DbHelper.MakeParameters(names, values));
            
             */
            return result;
        }
        #endregion

        #region public int SetWorked(string[] id, int worked) 设置评论处理
        /// <summary>
        /// 设置评论处理
        /// </summary>
        /// <param name="ids">评论主键数组</param>
        /// <param name="worked">处理状态</param>
        /// <returns>影响行数</returns>
        public int SetWorked(string[] ids, int worked)
        {
            var result = 0;
            var id = string.Empty;
            try
            {
                DbHelper.BeginTransaction();
                for (var i = 0; i < ids.Length; i++)
                {
                    id = ids[i];
                    // result += this.SetProperty(id, BaseCommentEntity.FieldWorked, worked.ToString());
                }
                DbHelper.CommitTransaction();
            }
            catch (Exception ex)
            {
                DbHelper.RollbackTransaction();
                BaseExceptionManager.LogException(DbHelper, UserInfo, ex);
                throw;
            }
            //Troy.Cui 2018.07.02
            finally
            {
                // 关闭数据库库连接
                DbHelper.Close();
            }
            return result;
        }
        #endregion

        #region public int ChageWorked(IDbHelper dbHelper, string id, bool worked) 更新处理状态
        /// <summary>
        /// 更新处理状态
        /// </summary>
        /// <param name="dbHelper"></param>
        /// <param name="id">评论主键</param>
        /// <param name="worked">处理状态</param>
        /// <returns>影响行数</returns>
        public int ChageWorked(IDbHelper dbHelper, string id, bool worked)
        {
            var sqlBuilder = new SqlBuilder(DbHelper);
            sqlBuilder.BeginUpdate(CurrentTableName);
            // sqlBuilder.SetValue(BaseCommentEntity.FieldWorked, worked ? 1 : -1);
            sqlBuilder.SetWhere(BaseCommentEntity.FieldId, id);
            return sqlBuilder.EndUpdate();
        }
        #endregion

        #region public DataTable Search(string departmentId, string searchKey) 查询
        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="departmentId">部门主键</param>
        /// <param name="searchKey">查询字符</param>
        /// <returns>数据表</returns>
        public DataTable Search(string departmentId, string searchKey)
        {
            return Search(departmentId, string.Empty, string.Empty, searchKey);
        }
        #endregion

        #region  public DataTable Search(string departmentId, string year, string month, string searchKey) 查询具体方法
        /// <summary>
        /// 查询具体方法
        /// </summary>
        /// <param name="departmentId">部门主键</param>
        /// <param name="year">年</param>
        /// <param name="month">月</param>
        /// <param name="searchKey">查询字符</param>
        /// <returns>数据表</returns>
        public DataTable Search(string departmentId, string year, string month, string searchKey)
        {
            var dt = new DataTable(CurrentTableName);
            var sql = String.Format("SELECT * FROM {0} WHERE ({1}=0) ", CurrentTableName,
                                            BaseCommentEntity.FieldDeleted);
            if (!string.IsNullOrEmpty(departmentId) && !departmentId.Equals("All"))
            {
                sql += String.Format(" AND {0}={1} ", BaseCommentEntity.FieldDepartmentId, departmentId);
            }

            var dbParameters = new List<IDbDataParameter>();
            if (!string.IsNullOrEmpty(year) && (!year.Equals("All")))
            {
                sql += String.Format(" AND YEAR({0}={1}) ", BaseCommentEntity.FieldCreateTime, DbHelper.GetParameter("CurrentYear"));
                dbParameters.Add(DbHelper.MakeParameter("CurrentYear", year));
            }
            if (!string.IsNullOrEmpty(month) && (!month.Equals("All")))
            {
                sql += String.Format(" AND MONTH({0}={1}) ", BaseCommentEntity.FieldCreateTime, DbHelper.GetParameter("CurrentMonth"));
                dbParameters.Add(DbHelper.MakeParameter("CurrentMonth", month));
            }
            if (!string.IsNullOrEmpty(searchKey))
            {
                sql += String.Format(" AND ({0} LIKE {1} OR {2} LIKE {3} OR {4} LIKE {5}) ",
                                          BaseCommentEntity.FieldTitle,
                                          DbHelper.GetParameter(BaseCommentEntity.FieldTitle),
                                          BaseCommentEntity.FieldCreateBy, 
                                          DbHelper.GetParameter(BaseCommentEntity.FieldCreateBy),
                                          BaseCommentEntity.FieldContents,
                                          DbHelper.GetParameter(BaseCommentEntity.FieldContents)
                    );
                searchKey = StringUtil.GetSearchString(searchKey);
                dbParameters.Add(DbHelper.MakeParameter(BaseCommentEntity.FieldTitle, searchKey));
                dbParameters.Add(DbHelper.MakeParameter(BaseCommentEntity.FieldCreateBy, searchKey));
                dbParameters.Add(DbHelper.MakeParameter(BaseCommentEntity.FieldContents, searchKey));
            }
            sql += String.Format(" ORDER BY {0} DESC ", BaseCommentEntity.FieldCreateTime);
            DbHelper.Fill(dt, sql, dbParameters.ToArray());
            return dt;
        }
        #endregion

        #region public DataTable GetPreviousNextId(string id) 获得主键列表中的，上一条，下一条记录的主键
        /// <summary>
        /// 获得主键列表中的，上一条，下一条记录的主键
        /// </summary>
        /// <param name="id">评论主键</param>
        /// <returns>数据表</returns>
        public DataTable GetPreviousNextId(string id)
        {
            var sql = String.Format("SELECT Id FROM {0} WHERE ({1} = ?) ORDER BY {2}", BaseCommentEntity.TableName, BaseCommentEntity.FieldCreateUserId, BaseCommentEntity.FieldCreateTime);
            var names = new string[1];
            var values = new Object[1];
            names[0] = BaseCommentEntity.FieldCreateUserId;
            values[0] = UserInfo.Id;
            var dt = new DataTable(BaseCommentEntity.TableName);
            DbHelper.Fill(dt, sql, DbHelper.MakeParameters(names, values));
            // this.NextId = this.GetNextId(result, UserInfo, id);
            // this.PreviousId = this.GetPreviousId(result, UserInfo, id);
            return dt;
        }
        #endregion

        #region public DataTable GetDataTableByPermissionScope(string[] departmentIds = null, int topLimit = 10) 获得最新评论列表
        /// <summary>
        /// 获得最新评论列表
        /// </summary>
        /// <param name="departmentIds">组织机构组件</param>
        /// <param name="topLimit">前几个</param>
        /// <returns>数据表</returns>
        public DataTable GetDataTableByPermissionScope(string[] departmentIds = null, int topLimit = 10)
        {
            var parametersList = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseCommentEntity.FieldDeleted, 0),
                new KeyValuePair<string, object>(BaseCommentEntity.FieldEnabled, 1)
            };
            if (departmentIds == null)
            {
                parametersList.Add(new KeyValuePair<string, object>(BaseCommentEntity.FieldObjectId, UserInfo.Id));
            }
            else
            {
                parametersList.Add(new KeyValuePair<string, object>(BaseCommentEntity.FieldDepartmentId, departmentIds));
            }
            return GetDataTable(parametersList, topLimit, BaseCommentEntity.FieldCreateTime + " DESC");
        }
        #endregion

        #region public DataTable GetDataTableByCategory(string categoryId, string objectId) 获取评论列表
        /// <summary>
        /// 获取评论列表
        /// </summary>
        /// <param name="categoryCode">分类主键</param>
        /// <param name="objectId">标志主键</param>
        /// <returns>数据表</returns>
        public DataTable GetDataTableByCategory(string categoryCode, string objectId)
        {
            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseCommentEntity.FieldCategoryCode, categoryCode),
                new KeyValuePair<string, object>(BaseCommentEntity.FieldObjectId, objectId),
                new KeyValuePair<string, object>(BaseCommentEntity.FieldDeleted, 0)
            };
            return GetDataTable(parameters, BaseCommentEntity.FieldCreateTime);
        }
        #endregion

        #region public DataTable GetDataTableByPage(BaseUserInfo userInfo, string departmentId, string searchKey, out int recordCount, int pageIndex = 0, int pageSize = 20, string sortExpression = null, string sortDirection = null) 分页查询
        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        /// <param name="recordCount">记录数</param>
        /// <param name="departmentIds"></param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">每页显示</param>
        /// <param name="sortExpression">排序字段</param>
        /// <param name="sortDirection">排序方向</param>
        /// <returns>数据表</returns>
        public DataTable GetDataTableByPage(BaseUserInfo userInfo, out int recordCount, string[] departmentIds = null, int pageIndex = 0, int pageSize = 20, string sortExpression = null, string sortDirection = null)
        {
            var condition = string.Format("{0} = 0 ", BaseCommentEntity.FieldDeleted);
            if (departmentIds != null && departmentIds.Length > 0)
            {
                condition += string.Format(" AND {0} IN ({1}) ", BaseCommentEntity.FieldDepartmentId, string.Join(",", departmentIds));
            }
            else
            {
                condition += string.Format(" AND {0} = '{1}' ", BaseCommentEntity.FieldObjectId, UserInfo.Id);
            }
            return GetDataTableByPage(out recordCount, pageIndex, pageSize, sortExpression, sortDirection, CurrentTableName, condition, null, "*");
        }
        #endregion
    }
}