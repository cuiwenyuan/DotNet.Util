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
    /// BaseParameterManager
    /// 参数类
    /// 
    /// 修改记录
    /// 
    ///     2015.07.16 版本：3.0 JiRiGaLa   进行分表保存。
    ///     2011.04.05 版本：2.2 JiRiGaLa   修改AddEntity 为public 方法，ip限制功能中使用
    ///     2009.04.01 版本：2.1 JiRiGaLa   创建者、修改者进行完善。
    ///     2008.04.30 版本：2.0 JiRiGaLa   按面向对象，面向服务进行改进。
    ///     2007.06.08 版本：1.4 JiRiGaLa   重新调整方法。
    ///		2006.02.05 版本：1.3 JiRiGaLa	重新调整主键的规范化。
    ///		2006.01.28 版本：1.2 JiRiGaLa	对一些方法进行改进，主键整理，调用性能也进行了修改，主键顺序进行整理。
    ///		2005.08.13 版本：1.1 JiRiGaLa	主键整理好。
    ///		2004.11.12 版本：1.0 JiRiGaLa	主键进行了绝对的优化，这是个好东西啊，平时要多用，用得要灵活些。
    ///
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2015.07.16</date>
    /// </author> 
    /// </summary>
    public partial class BaseParameterManager : BaseManager
    {
        #region public string AddParameter(BaseParameterEntity entity)
        /// <summary>
        /// 添加内容
        /// </summary>
        /// <param name="entity">内容</param>
        /// <param name="identity"></param>
        /// <param name="returnId"></param>
        /// <returns>主键</returns>
        public string AddParameter(BaseParameterEntity entity, bool identity = true, bool returnId = true)
        {
            var result = string.Empty;

            Identity = identity;
            ReturnId = returnId;

            // 此处检查this.exist()
            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseParameterEntity.FieldCategoryCode, entity.CategoryCode),
                new KeyValuePair<string, object>(BaseParameterEntity.FieldParameterId, entity.ParameterId),
                new KeyValuePair<string, object>(BaseParameterEntity.FieldParameterCode, entity.ParameterCode),
                new KeyValuePair<string, object>(BaseParameterEntity.FieldParameterContent, entity.ParameterContent),
                new KeyValuePair<string, object>(BaseParameterEntity.FieldDeleted, 0)
            };
            // 2015-12-22 吉日嘎拉 检查没有删除的，已删除的当日志用了
            if (Exists(parameters))
            {
                // 编号已重复
                StatusCode = Status.ErrorCodeExist.ToString();
            }
            else
            {
                result = AddEntity(entity);
                // 运行成功
                StatusCode = Status.OkAdd.ToString();
            }

            return result;
        }
        #endregion

        #region public int Update(BaseParameterEntity entity) 更新
        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="entity">参数基类表结构定义</param>
        /// <returns>影响行数</returns>
        public int UpdateParameter(BaseParameterEntity entity)
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
                // 文件夹名已重复
                StatusCode = Status.ErrorCodeExist.ToString();
            }
            else
            {
                // 进行更新操作
                result = UpdateEntity(entity);
                if (result == 1)
                {
                    StatusCode = Status.OkUpdate.ToString();
                }
                else
                {
                    // 数据可能被删除
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
            var sb = Pool.StringBuilder.Get().Append(" 1 = 1");
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
                new KeyValuePair<string, object>(BaseParameterEntity.FieldEnabled, 1),
                new KeyValuePair<string, object>(BaseParameterEntity.FieldDeleted, 0)
            };
            var dt = CacheUtil.Cache<DataTable>(cacheKey, () => GetDataTable(parameters), true, false, cacheTime);
            //查找
            sb.Replace(" 1 = 1 AND ", "");
            var drs = dt.Select(sb.Put());
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
                new KeyValuePair<string, object>(BaseParameterEntity.FieldDeleted, 0)
            };
            // 检测是否无效数据
            if ((parameterContent == null) || (parameterContent.Length == 0))
            {
                result = Delete(parameters);
            }
            else
            {
                // 检测是否存在
                result = SetProperty(parameters, new KeyValuePair<string, object>(BaseParameterEntity.FieldParameterContent, parameterContent));
                if (result == 0)
                {
                    // 进行增加操作
                    var entity = new BaseParameterEntity
                    {
                        CategoryCode = categoryCode,
                        ParameterId = parameterId,
                        ParameterCode = parameterCode,
                        ParameterContent = parameterContent,
                        Enabled = 1,
                        Deleted = 0,
                        SortCode = 1 // 不要排序了
                    };
                    AddParameter(entity);
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

        #region public int AddParameter(string categoryCode, string parameterId, string parameterCode, string parameterContent)
        /// <summary>
        /// 添加参数设置
        /// </summary>
        /// <param name="categoryCode">类别编号</param>
        /// <param name="parameterId">参数主键</param>
        /// <param name="parameterCode">编码</param>
        /// <param name="parameterContent">参数内容</param>
        /// <returns>主键</returns>
        public string AddParameter(string categoryCode, string parameterId, string parameterCode, string parameterContent)
        {
            return AddParameter(BaseParameterEntity.CurrentTableName, categoryCode, parameterId, parameterCode, parameterContent);
        }
        #endregion

        #region public int AddParameter(string tableName, string categoryCode, string parameterId, string parameterCode, string parameterContent)
        /// <summary>
        /// 添加参数设置
        /// 2015-07-24 吉日嘎拉 按表名来添加、尽管添加的功能实现。
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="categoryCode">类别编号</param>
        /// <param name="parameterId">参数主键</param>
        /// <param name="parameterCode">编码</param>
        /// <param name="parameterContent">参数内容</param>
        /// <returns>主键</returns>
        public string AddParameter(string tableName, string categoryCode, string parameterId, string parameterCode, string parameterContent)
        {
            var result = string.Empty;

            CurrentTableName = tableName;

            /*
            List<KeyValuePair<string, object>> parameters = new List<KeyValuePair<string, object>>();
            parameters.Add(new KeyValuePair<string, object>(BaseParameterEntity.FieldCategoryCode, categoryCode));
            parameters.Add(new KeyValuePair<string, object>(BaseParameterEntity.FieldParameterId, parameterId));
            parameters.Add(new KeyValuePair<string, object>(BaseParameterEntity.FieldParameterCode, parameterCode));
            parameters.Add(new KeyValuePair<string, object>(BaseParameterEntity.FieldDeleted, 0));
            // 检测是否无效数据
            if ((parameterContent == null) || (parameterContent.Length == 0))
            {
                this.SetDeleted(parameters);
            }
            else
            {
                parameters.Add(new KeyValuePair<string, object>(BaseParameterEntity.FieldParameterContent, parameterContent));
                result = this.GetProperty(parameters, BaseParameterEntity.FieldId);
                // 检测是否存在
                if (string.IsNullOrEmpty(result))
                {
             */
            // 进行增加操作
            var entity = new BaseParameterEntity
            {
                //Id = Guid.NewGuid().ToString("N"),
                CategoryCode = categoryCode,
                ParameterId = parameterId,
                ParameterCode = parameterCode,
                ParameterContent = parameterContent,
                Enabled = 1,
                Deleted = 0
            };
            result = AddEntity(entity);
            /*
                }
            }
            */

            return result;
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