﻿//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System.Collections.Generic;

namespace DotNet.Business
{
    using Util;

    /// <summary>
    ///	BaseManager
    /// 通用基类部分
    /// 
    /// 总觉得自己写的程序不上档次，这些新技术也玩玩，也许做出来的东西更专业了。
    /// 修改记录
    /// 
    ///		2012.02.04 版本：1.0 JiRiGaLa 进行提炼，把代码进行分组。
    ///
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2012.02.04</date>
    /// </author> 
    /// </summary>
    public partial class BaseManager : IBaseManager
    {
        #region public string GetMax(string field = "Id") 获取最大值
        /// <summary>
        /// 获取最大值
        /// </summary>
        /// <param name="field">字段</param>
        /// <returns>最大值</returns>
        public string GetMax(string field = "Id")
        {
            var result = string.Empty;
            var sb = Pool.StringBuilder.Get();
            sb.Append("SELECT Max(" + field + ")" + " FROM " + CurrentTableName);
            var returnObject = dbHelper.ExecuteScalar(sb.Put());
            if (returnObject != null)
            {
                result = returnObject.ToString();
            }
            return result;
        }
        #endregion

        #region public virtual string GetCodeById(string id) 获取编码

        /// <summary>
        /// 获取编码
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="tableVersion">版本默认4为老版本</param>
        /// <returns>编号</returns>
        public virtual string GetCodeById(string id, int tableVersion = 4)
        {
            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseUtil.FieldId, id),
                new KeyValuePair<string, object>(tableVersion == 4 ? BaseUtil.FieldDeletionStateCode : BaseUtil.FieldDeleted, 0)
            };
            return DbUtil.GetProperty(DbHelper, CurrentTableName, parameters, BaseUtil.FieldCode);
        }
        #endregion

        #region public virtual string GetCodeByFullName(string fullName) 获取编号
        /// <summary>
        /// 获取名称
        /// </summary>
        /// <param name="fullName">名称</param>
        /// <returns>编码</returns>
        public virtual string GetCodeByFullName(string fullName)
        {
            return GetProperty(new KeyValuePair<string, object>(BaseUtil.FieldFullName, fullName), BaseUtil.FieldCode);
        }
        #endregion

        #region public virtual string GetFullNameByCode(string code) 获取名称
        /// <summary>
        /// 获取名称
        /// </summary>
        /// <param name="code">编码</param>
        /// <returns>名称</returns>
        public virtual string GetFullNameByCode(string code)
        {
            return GetProperty(new KeyValuePair<string, object>(BaseUtil.FieldCode, code), BaseUtil.FieldFullName);
        }
        #endregion

        #region public virtual string GetFullNameById(string id) 获取名称
        /// <summary>
        /// 获取名称
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns>名称</returns>
        public virtual string GetFullNameById(string id)
        {
            return GetProperty(new KeyValuePair<string, object>(BaseUtil.FieldId, id), BaseUtil.FieldFullName);
        }
        #endregion

        #region public virtual string GetParentId(string id) 获取父级主键
        /// <summary>
        /// 获取父节点主键
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns>父级主键</returns>
        public virtual string GetParentId(string id)
        {
            return GetProperty(new KeyValuePair<string, object>(BaseUtil.FieldId, id), BaseUtil.FieldParentId);
        }
        #endregion

        #region public virtual string GetParentIdByCode(string code) 获取父节点主键
        /// <summary>
        /// 获取父节点主键
        /// </summary>
        /// <param name="code">编号</param>
        /// <returns>父级主键</returns>
        public virtual string GetParentIdByCode(string code)
        {
            return GetProperty(new KeyValuePair<string, object>(BaseUtil.FieldCode, code), BaseUtil.FieldParentId);
        }
        #endregion

        #region public virtual string GetIdByCode(string code) 获取主键

        /// <summary>
        /// 获取主键
        /// </summary>
        /// <param name="code">编号</param>
        /// <param name="tableVersion">版本默认4为老版本</param>
        /// <returns>主键</returns>
        public virtual string GetIdByCode(string code, int tableVersion = 4)
        {
            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseUtil.FieldCode, code),
                new KeyValuePair<string, object>(tableVersion == 4 ? BaseUtil.FieldDeletionStateCode : BaseUtil.FieldDeleted, 0)
            };
            return DbUtil.GetProperty(DbHelper, CurrentTableName, parameters, BaseUtil.FieldId);
        }
        #endregion

        #region public string GetParentIdByCategory(string categoryCode, string code) 获取父节点主键
        /// <summary>
        /// 获取父节点主键
        /// </summary>
        /// <param name="categoryCode">分类主键</param>
        /// <param name="code">主键</param>
        /// <returns>父级主键</returns>
        public virtual string GetParentIdByCategory(string categoryCode, string code)
        {
            return GetProperty(new KeyValuePair<string, object>(BaseUtil.FieldCategoryCode, categoryCode), new KeyValuePair<string, object>(BaseUtil.FieldCode, code), BaseUtil.FieldParentId);
        }
        #endregion

        #region public virtual string GetFullNameByCategory(string categoryCode, string code) 获取名称
        /// <summary>
        /// 获取名称
        /// </summary>
        /// <param name="categoryCode">类别主键</param>
        /// <param name="code">编码</param>
        /// <returns>名称</returns>
        public virtual string GetFullNameByCategory(string categoryCode, string code)
        {
            return GetProperty(new KeyValuePair<string, object>(BaseUtil.FieldCategoryCode, categoryCode), new KeyValuePair<string, object>(BaseUtil.FieldCode, code), BaseUtil.FieldFullName);
        }
        #endregion

        //
        // 读取属性
        //
        /// <summary>
        /// 获取属性
        /// </summary>
        /// <param name="id"></param>
        /// <param name="targetField"></param>
        /// <returns></returns>
        public virtual string GetProperty(object id, string targetField)
        {
            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseUtil.FieldId, id)
            };
            return DbUtil.GetProperty(DbHelper, CurrentTableName, parameters, targetField);
        }
        /// <summary>
        /// 获取属性
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="targetField"></param>
        /// <returns></returns>
        public virtual string GetProperty(KeyValuePair<string, object> parameter, string targetField)
        {
            var parameters = new List<KeyValuePair<string, object>>
            {
                parameter
            };
            return DbUtil.GetProperty(DbHelper, CurrentTableName, parameters, targetField);
        }
        /// <summary>
        /// 获取属性
        /// </summary>
        /// <param name="parameter1"></param>
        /// <param name="parameter2"></param>
        /// <param name="targetField"></param>
        /// <returns></returns>
        public virtual string GetProperty(KeyValuePair<string, object> parameter1, KeyValuePair<string, object> parameter2, string targetField)
        {
            var parameters = new List<KeyValuePair<string, object>>
            {
                parameter1,
                parameter2
            };
            return DbUtil.GetProperty(DbHelper, CurrentTableName, parameters, targetField);
        }
        /// <summary>
        /// 获取属性
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="targetField"></param>
        /// <param name="orderBy"></param>
        /// <returns></returns>
        public virtual string GetProperty(List<KeyValuePair<string, object>> parameters, string targetField, string orderBy = null)
        {
            return DbUtil.GetProperty(DbHelper, CurrentTableName, parameters, targetField, 1, orderBy);
        }
        /// <summary>
        /// 获取编号
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public virtual string GetId(KeyValuePair<string, object> parameter)
        {
            var parameters = new List<KeyValuePair<string, object>>
            {
                parameter
            };
            return DbUtil.GetProperty(DbHelper, CurrentTableName, parameters, BaseUtil.FieldId);
        }
        /// <summary>
        /// 获取编号
        /// </summary>
        /// <param name="parameter1"></param>
        /// <param name="parameter2"></param>
        /// <returns></returns>
        public virtual string GetId(KeyValuePair<string, object> parameter1, KeyValuePair<string, object> parameter2)
        {
            var parameters = new List<KeyValuePair<string, object>>
            {
                parameter1,
                parameter2
            };
            return DbUtil.GetProperty(DbHelper, CurrentTableName, parameters, BaseUtil.FieldId);
        }
        /// <summary>
        /// 获取编号
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public virtual string GetId(List<KeyValuePair<string, object>> parameters)
        {
            return DbUtil.GetProperty(DbHelper, CurrentTableName, parameters, BaseUtil.FieldId);
        }
        /// <summary>
        /// 获取编号
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public virtual string GetId(params KeyValuePair<string, object>[] parameters)
        {
            var parameterList = new List<KeyValuePair<string, object>>();
            foreach (var parameter in parameters)
            {
                parameterList.Add(parameter);
            }
            return DbUtil.GetProperty(DbHelper, CurrentTableName, parameterList, BaseUtil.FieldId);
        }
    }
}