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
	/// BaseItemDetailsManager 
    /// 主键管理表结构定义部分（程序OK）
    ///
	/// 注意事项;
	///		Id 为主键
	///		CreateOn不为空，默认值
	///		ParentId、FullName 需要建立唯一索引
	///		CategoryId 是为了解决多重数据库兼容的问题
	///		ParentId 是为了解决形成树行结构的问题
	///
	/// 修改记录
    ///
    ///     2018.09.05 版本：4.6 Troy.Cui  增加缓存重写和GetItemName
    ///     2011.03.29 版本：4.5 JiRiGaLa  允许重复的名称，但是不允许编号和名称都重复。
    ///     2009.07.01 版本：4.4 JiRiGaLa  按某种权限获取主键列表。
    ///     2007.12.03 版本：4.3 JiRiGaLa  进行规范化整理。
    ///     2007.06.03 版本：4.2 JiRiGaLa  进行改进整理。
    ///     2007.05.31 版本：4.1 JiRiGaLa  进行改进整理。
    ///		2007.01.15 版本：4.0 JiRiGaLa  重新整理主键。
    ///		2007.01.03 版本：3.0 JiRiGaLa  进行大批量主键整理。
    ///		2006.12.05 版本：2.0 JiRiGaLa  GetFullName 方法更新。
	///		2006.01.23 版本：1.0 JiRiGaLa  获取ItemDetails方法的改进。
	///		2004.11.12 版本：1.0 JiRiGaLa  主键进行了绝对的优化，基本上看上去还过得去了。
    ///     2007.12.03 版本：2.2 JiRiGaLa  进行规范化整理。
    ///     2007.05.30 版本：2.1 JiRiGaLa  整理主键，调整GetFrom()方法,增加AddEntity(),UpdateEntity(),DeleteObject()
    ///		2007.01.15 版本：2.0 JiRiGaLa  重新整理主键。
	///		2006.02.06 版本：1.1 JiRiGaLa  重新调整主键的规范化。
	///		2005.10.03 版本：1.0 JiRiGaLa  表中添加是否可删除，可修改字段。
	///
	/// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2007.12.03</date>
	/// </author>
	/// </summary>
    public partial class BaseItemDetailsManager : BaseManager //, IBaseItemDetailsManager
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbHelper"></param>
        /// <param name="tableName"></param>
        public BaseItemDetailsManager(IDbHelper dbHelper, string tableName)
            : this(dbHelper)
        {
            CurrentTableName = tableName;
        }

        /// <summary>
        /// 获取清单
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public List<BaseItemDetailsEntity> GetListByTable(string tableName)
        {
            List<BaseItemDetailsEntity> result = null;
            var sql = "   SELECT * "
                            + " FROM " + tableName
                            + "    WHERE " + BaseItemDetailsEntity.FieldDeleted + " = 0 "
                            + "          AND " + BaseItemDetailsEntity.FieldEnabled + " = 1 "
                            + " ORDER BY " + BaseItemDetailsEntity.FieldSortCode;
            using (var dataReader = DbHelper.ExecuteReader(sql))
            {
                result = GetList<BaseItemDetailsEntity>(dataReader);
            }
            return result;
        }

        #region public string Add(BaseItemDetailsEntity entity, out string statusCode) 添加
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="statusCode">返回状态码</param>
        /// <returns>主键</returns>
        public string Add(BaseItemDetailsEntity entity, out string statusCode)
        {
            var result = string.Empty;
            // 检查编号是否重复
            if (!string.IsNullOrEmpty(entity.ItemCode))
            {
                var parameters = new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>(BaseItemDetailsEntity.FieldItemCode, entity.ItemCode),
                    new KeyValuePair<string, object>(BaseItemDetailsEntity.FieldDeleted, 0)
                };
                if (Exists(parameters))
                {
                    // 编号已重复
                    statusCode = Status.ErrorCodeExist.ToString();
                    return result;
                }
            }

            /*

            if (!string.IsNullOrEmpty(entity.ItemName))
            {
                if (this.Exists(new KeyValuePair<string, object>(BaseItemDetailsEntity.FieldItemName, entity.ItemName)))
                {
                    // 名称已重复
                    statusCode = Status.ErrorNameExist.ToString();
                    return result;
                }
            }

            if (!string.IsNullOrEmpty(entity.ItemValue))
            {
                if (this.Exists(new KeyValuePair<string, object>(BaseItemDetailsEntity.FieldItemValue, entity.ItemValue)))
                {
                    // 值已重复
                    statusCode = Status.ErrorValueExist.ToString();
                    return result;
                }
            }

            List<KeyValuePair<string, object>> parameters = new List<KeyValuePair<string, object>>();
            if (entity.ParentId.HasValue)
            {
                parameters.Add(new KeyValuePair<string, object>(BaseItemDetailsEntity.FieldParentId, entity.ParentId));
            }
            parameters.Add(new KeyValuePair<string, object>(BaseItemDetailsEntity.FieldItemCode, entity.ItemCode));
            parameters.Add(new KeyValuePair<string, object>(BaseItemDetailsEntity.FieldItemName, entity.ItemName));
            parameters.Add(new KeyValuePair<string, object>(BaseItemDetailsEntity.FieldDeleted, 0));

            if (this.Exists(parameters))
            {
                // 名称已重复
                statusCode = Status.Exist.ToString();
                return result;
            }
            */

            // 运行成功
            result = AddEntity(entity);
            statusCode = Status.OkAdd.ToString();
            return result;
        }
        #endregion

        #region public int Update(BaseItemDetailsEntity entity, out string statusCode) 更新
        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="statusCode">返回状态码</param>
        /// <returns>影响行数</returns>
        public int Update(BaseItemDetailsEntity entity, out string statusCode)
        {
            var result = 0;
            // 检查是否已被其他人修改            
            //if (DbUtil.IsModifed(DbHelper, this.CurrentTableName, itemDetailsEntity.Id, itemDetailsEntity.ModifiedUserId, itemDetailsEntity.ModifiedOn))
            //{
            //    // 数据已经被修改
            //    statusCode = StatusCode.ErrorChanged.ToString();
            //}
            // 检查编号是否重复
            // if (this.Exists(BaseItemDetailsEntity.FieldItemCode, itemDetailsEntity.ItemCode, itemDetailsEntity.Id))
            // if (this.Exists(BaseItemDetailsEntity.FieldItemValue, itemDetailsEntity.ItemValue, itemDetailsEntity.Id))
            // 检查名称是否重复

            var parameters = new List<KeyValuePair<string, object>>();
            if (entity.ParentId.HasValue)
            {
                parameters.Add(new KeyValuePair<string, object>(BaseItemDetailsEntity.FieldParentId, entity.ParentId));
            }
            parameters.Add(new KeyValuePair<string, object>(BaseItemDetailsEntity.FieldItemCode, entity.ItemCode));
            // parameters.Add(new KeyValuePair<string, object>(BaseItemDetailsEntity.FieldItemName, entity.ItemName));
            parameters.Add(new KeyValuePair<string, object>(BaseItemDetailsEntity.FieldDeleted, 0));

            if (Exists(parameters, entity.Id))
            {
                // 名称已重复
                statusCode = Status.Exist.ToString();
                return result;
            }
            result = UpdateEntity(entity);
            if (result == 1)
            {
                statusCode = Status.OkUpdate.ToString();
            }
            else
            {
                statusCode = Status.ErrorDeleted.ToString();
            }
            return result;
        }
        #endregion

        #region public int BatchSave(DataTable result) 批量进行保存
        /// <summary>
        /// 批量进行保存
        /// </summary>
        /// <param name="dt">数据表</param>
        /// <returns>影响行数</returns>
        public override int BatchSave(DataTable dt)
        {
            var result = 0;
            var entity = new BaseItemDetailsEntity();
            foreach (DataRow dr in dt.Rows)
            {
                // 删除状态
                if (dr.RowState == DataRowState.Deleted)
                {
                    var id = dr[BaseItemDetailsEntity.FieldId, DataRowVersion.Original].ToString();
                    if (id.Length > 0)
                    {
                        if (entity.AllowDelete == 1)
                        {
                            result += Delete(id);
                        }
                    }
                }
                // 被修改过
                if (dr.RowState == DataRowState.Modified)
                {
                    var id = dr[BaseItemDetailsEntity.FieldId, DataRowVersion.Original].ToString();
                    if (id.Length > 0)
                    {
                        entity.GetFrom(dr);
                        if (!entity.IsPublic.HasValue)
                        {
                            entity.IsPublic = 1;
                        }
                        if (!entity.Enabled.HasValue)
                        {
                            entity.Enabled = 1;
                        }
                        if (!entity.DeletionStateCode.HasValue)
                        {
                            entity.DeletionStateCode = 0;
                        }
                        // 判断是否允许编辑
                        if (entity.AllowEdit == 1)
                        {
                            result += UpdateEntity(entity);
                        }
                        else
                        {
                            // 不允许编辑，但是排序还是允许的
                            result += SetProperty(id, new KeyValuePair<string, object>(BaseItemDetailsEntity.FieldSortCode, entity.SortCode));
                        }
                    }
                }
                // 添加状态
                if (dr.RowState == DataRowState.Added)
                {
                    entity.GetFrom(dr);
                    if (!entity.IsPublic.HasValue)
                    {
                        entity.IsPublic = 1;
                    }
                    result += AddEntity(entity).Length > 0 ? 1 : 0;
                }
                if (dr.RowState == DataRowState.Unchanged)
                {
                    continue;
                }
                if (dr.RowState == DataRowState.Detached)
                {
                    continue;
                }
            }
            return result;
        }
        #endregion

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="list"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public int Save(List<BaseItemDetailsEntity> list, string tableName)
        {
            var result = 0;
            if (list != null)
            {
                CurrentTableName = tableName;
                foreach (var entity in list)
                {
                    result = UpdateEntity(entity);
                    if (result == 0)
                    {
                        AddEntity(entity);
                    }
                }
                /*
                this.Delete();
                foreach (var entity in list)
                {
                    result += this.AddEntity(entity).Length > 0 ? 1 : 0;
                }
                */
            }
            return result;
        }

        #region public int Save(DataTable result) 批量进行保存
        /// <summary>
        /// 批量进行保存
        /// </summary>
        /// <param name="dt">数据表</param>
        /// <returns>影响行数</returns>
        public int Save(DataTable dt)
        {
            var result = 0;
            if (dt != null)
            {
                CurrentTableName = dt.TableName;
                Delete();
                var entity = new BaseItemDetailsEntity();
                foreach (DataRow dr in dt.Rows)
                {
                    entity.GetFrom(dr);
                    result += AddEntity(entity).Length > 0 ? 1 : 0;
                }
            }
            return result;
        }
        #endregion

        #region public DataTable GetDataTableByPermission(string userId, string resourceCategory, string permissionCode) 按某种权限获取主键列表
        /// <summary>
        /// 按某种权限获取主键列表
        /// </summary>
        /// <param name="userId">用户主键</param>
        /// <param name="resourceCategory">资源分类</param>
        /// <param name="permissionCode">权限编号</param>
        /// <returns>数据表</returns>
        public DataTable GetDataTableByPermission(string userId, string resourceCategory, string permissionCode = "Resource.ManagePermission")
        {
            var permissionScopeManager = new BasePermissionScopeManager(DbHelper, UserInfo, CurrentTableName); ;
            var ids = permissionScopeManager.GetResourceScopeIds(UserInfo.SystemCode, userId, resourceCategory, permissionCode);
            var dt = GetDataTable(ids);
            dt.DefaultView.Sort = BaseItemDetailsEntity.FieldSortCode;
            return dt;
        }
        #endregion

        /// <summary>
        /// 移动
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="parentId">父级主键</param>
        /// <returns>影响行数</returns>
        public int MoveTo(string id, string parentId)
        {
            return SetProperty(id, new KeyValuePair<string, object>(BaseItemDetailsEntity.FieldParentId, parentId));
        }

        #region public static List<BaseItemDetailsEntity> GetEntitiesByCache(string tableName, bool refresh = false) 获取模块菜单表，从缓存读取
        /// <summary>
        /// 获取模块菜单表，从缓存读取
        /// 2016-03-14 吉日嘎拉 更新有缓存功能
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="refresh">刷新</param>
        /// <returns>选项数据列表</returns>
        public static List<BaseItemDetailsEntity> GetEntitiesByCache(string tableName, bool refresh = false)
        {
            List<BaseItemDetailsEntity> result = null;
            var key = "ItemDetails:" + tableName;
            result = CacheUtil.Cache(key, () =>
            {
                var itemDetailsManager = new BaseItemDetailsManager(tableName);
                var parameters = new List<KeyValuePair<string, object>>
                {
                    // 管理的时候无效的也需要被管理
                    new KeyValuePair<string, object>(BaseItemDetailsEntity.FieldEnabled, 1),
                    new KeyValuePair<string, object>(BaseItemDetailsEntity.FieldDeleted, 0)
                };
                return itemDetailsManager.GetList<BaseItemDetailsEntity>(parameters, BaseItemDetailsEntity.FieldSortCode);
            }, true, refresh);

            return result;
        }
        #endregion

        /// <summary>
        /// 从缓存获取实体
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static BaseItemDetailsEntity GetEntityByCache(string tableName, string id)
        {
            BaseItemDetailsEntity result = null;
            var key = "ItemDetails:" + tableName;
            if (!string.IsNullOrEmpty(id))
            {
                key = "ItemDetails:" + tableName + ":" + id;
            }
            result = CacheUtil.Cache(key, () => new BaseItemDetailsManager().GetEntity(id), true);
            return result;
        }

        #region 获取ItemName
        /// <summary>
        /// 获取ItemName（有缓存）
        /// </summary>
        /// <returns></returns>
        public string GetItemName(string itemCode = null, string itemValue = null)
        {
            var result = string.Empty;
            var sb = Pool.StringBuilder.Get().Append(" 1 = 1");
            if (!string.IsNullOrEmpty(itemCode))
            {
                sb.Append(" AND ItemCode = '" + itemCode + "'");
            }
            if (!string.IsNullOrEmpty(itemValue))
            {
                sb.Append(" AND ItemValue = '" + itemValue + "'");
            }

            var cacheKey = "DataTable." + CurrentTableName;
            //万一系统里开的公司多了，还是不要按公司来分
            //if (UserInfo != null)
            //{
            //    cacheKey += "." + UserInfo.CompanyId;
            //}

            var cacheTime = TimeSpan.FromMilliseconds(86400000);

            var parameters = new List<KeyValuePair<string, object>>
            {
                //这里只要有效的，没被删除的
                new KeyValuePair<string, object>(BaseItemDetailsEntity.FieldEnabled, 1),
                new KeyValuePair<string, object>(BaseItemDetailsEntity.FieldDeleted, 0)
            };

            var dt = CacheUtil.Cache<DataTable>(cacheKey, () => GetDataTable(parameters), true, false, cacheTime);
            //查找
            sb.Replace(" 1 = 1 AND ", "");
            var drs = dt.Select(sb.Put());
            if (drs.Length > 0)
            {
                result = drs[0]["ItemName"].ToString();
            }

            return result;
        }

        #endregion

        #region 删除缓存

        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <returns></returns>
        public override bool RemoveCache()
        {
            var result = false;
            var cacheKey = "DataTable." + CurrentTableName;
            var cacheKeyTree = "DataTable." + CurrentTableName + ".List";
            if (UserInfo != null)
            {
                //cacheKey += "." + UserInfo.CompanyId;
                //cacheKeyTree = "DataTable." + UserInfo.SystemCode + ".ModuleTree";
            }

            CacheUtil.Remove(cacheKeyTree);
            result = CacheUtil.Remove(cacheKey);
            return result;
        }
        #endregion
    }
}