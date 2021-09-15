//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System.Collections.Generic;
using System.Data;

namespace DotNet.Business
{
    using Model;
    using Util;

    /// <summary>
    /// BaseDepartmentManager
    /// 部门表
    ///
    /// 修改记录
    ///     
    ///     2015-12-15 版本：1.1 宋彪   添加按编号获取实体 按名称获取实体
    ///     2015-04-23 版本：1.1 潘齐民   添加锁
    ///     2014.12.08 版本：1.0 JiRiGaLa 创建。
    ///
    /// <author>
    ///		<name>JiRiGaLa</name>
    ///		<date>2014.12.08</date>
    /// </author>
    /// </summary>
    public partial class BaseDepartmentManager : BaseManager //, IBaseOrganizeManager
    {
        // 当前的锁
        // private static object locker = new Object();

        /// <summary>
        /// 获取名称
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static string GetNames(List<BaseDepartmentEntity> list)
        {
            var result = string.Empty;

            foreach (var entity in list)
            {
                result += "," + entity.FullName;
            }
            if (!string.IsNullOrEmpty(result))
            {
                result = result.Substring(1);
            }

            return result;
        }

        /// <summary>
        /// 按编号获取实体
        /// </summary>
        /// <param name="code">编号</param>
        public BaseDepartmentEntity GetEntityByCode(string code)
        {
            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseOrganizeEntity.FieldCode, code),
                new KeyValuePair<string, object>(BaseOrganizeEntity.FieldDeleted, 0)
            };
            return BaseEntity.Create<BaseDepartmentEntity>(GetDataTable(parameters));
        }

        /// <summary>
        /// 按名称获取实体
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="fullName">名称</param>
        public BaseDepartmentEntity GetEntityByName(string companyId, string fullName)
        {
            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseOrganizeEntity.FieldFullName, fullName),
                new KeyValuePair<string, object>(BaseOrganizeEntity.FieldCompanyId, companyId),
                new KeyValuePair<string, object>(BaseOrganizeEntity.FieldDeleted, 0),
                new KeyValuePair<string, object>(BaseOrganizeEntity.FieldEnabled, 1)
            };
            return BaseEntity.Create<BaseDepartmentEntity>(ExecuteReader(parameters));
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="statusCode"></param>
        /// <returns></returns>
        public string Add(BaseDepartmentEntity entity, out string statusCode)
        {
            var result = string.Empty;
            // 检查是否重复
            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseDepartmentEntity.FieldParentId, entity.ParentId),
                new KeyValuePair<string, object>(BaseDepartmentEntity.FieldFullName, entity.FullName),
                new KeyValuePair<string, object>(BaseDepartmentEntity.FieldDeleted, 0)
            };

            //注意Access 的时候，类型不匹配，会出错故此将 Id 传入
            if (BaseSystemInfo.UserCenterDbType == CurrentDbType.Access)
            {
                if (Exists(parameters, entity.Id))
                {
                    // 名称已重复
                    statusCode = Status.ErrorNameExist.ToString();
                }
                else
                {
                    parameters = new List<KeyValuePair<string, object>>
                    {
                        new KeyValuePair<string, object>(BaseDepartmentEntity.FieldCode, entity.Code),
                        new KeyValuePair<string, object>(BaseDepartmentEntity.FieldDeleted, 0)
                    };
                    if (entity.Code.Length > 0 && Exists(parameters))
                    {
                        // 编号已重复
                        statusCode = Status.ErrorCodeExist.ToString();
                    }
                    else
                    {
                        result = AddEntity(entity);
                        // 运行成功
                        statusCode = Status.OkAdd.ToString();

                        AfterAdd(entity);
                    }
                }
            }
            else if (Exists(parameters))
            {
                // 名称已重复
                statusCode = Status.ErrorNameExist.ToString();
            }
            else
            {
                parameters = new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>(BaseDepartmentEntity.FieldCode, entity.Code),
                    new KeyValuePair<string, object>(BaseDepartmentEntity.FieldDeleted, 0)
                };
                if (entity.Code.Length > 0 && Exists(parameters))
                {
                    // 编号已重复
                    statusCode = Status.ErrorCodeExist.ToString();
                }
                else
                {
                    result = AddEntity(entity);
                    // 运行成功
                    statusCode = Status.OkAdd.ToString();

                    AfterAdd(entity);
                }
            }


            return result;
        }

        /// <summary>
        /// 添加之后，需要重新刷新缓存，否则其他读取数据的地方会乱了，或者不及时了
        /// 宋彪
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        public int AfterAdd(BaseDepartmentEntity entity)
        {
            var result = 0;
            CacheContractAreaPreheatingSpelling(entity);
            return result;
        }

        /// <summary>
        /// 批量保存
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public override int BatchSave(DataTable dt)
        {
            var result = 0;
            var entity = new BaseDepartmentEntity();
            foreach (DataRow dr in dt.Rows)
            {
                // 删除状态
                if (dr.RowState == DataRowState.Deleted)
                {
                    var id = dr[BaseDepartmentEntity.FieldId, DataRowVersion.Original].ToString();
                    if (id.Length > 0)
                    {
                        result += DeleteObject(id);
                    }
                }
                // 被修改过
                if (dr.RowState == DataRowState.Modified)
                {
                    var id = dr[BaseDepartmentEntity.FieldId, DataRowVersion.Original].ToString();
                    if (id.Length > 0)
                    {
                        entity.GetFrom(dr);
                        result += UpdateEntity(entity);
                    }
                }
                // 添加状态
                if (dr.RowState == DataRowState.Added)
                {
                    entity.GetFrom(dr);
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

        /// <summary>
        /// 移动
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="parentId">父级主键</param>
        /// <returns>影响行数</returns>
        public int MoveTo(string id, string parentId)
        {
            return SetProperty(id, new KeyValuePair<string, object>(BaseDepartmentEntity.FieldParentId, parentId));
        }

        /// <summary>
        /// 从缓存获取获取实体
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="refreshCache"></param>
        /// <returns>实体</returns>
        public static BaseDepartmentEntity GetEntityByCache(string id, bool refreshCache = false)
        {
            BaseDepartmentEntity result = null;
            if (!string.IsNullOrEmpty(id))
            {
                var key = "D:" + id;

                result = CacheUtil.Cache(key, () => new BaseDepartmentManager().GetEntity(id), true, refreshCache);
            }

            return result;
        }
    }
}