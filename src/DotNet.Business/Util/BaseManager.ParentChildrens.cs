//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2023, DotNet.
//-----------------------------------------------------------------

using System.Data;

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
        //
        // 树型结构的算法，递归算法
        //

        #region public DataTable GetParentsByCode(string fieldCode, string code, string order) 获取父节点列表
        /// <summary>
        /// 获取父节点列表
        /// </summary>
        /// <param name="fieldCode">编码字段</param>
        /// <param name="code">编码</param>
        /// <param name="order">排序</param>
        /// <returns>数据表</returns>
        public DataTable GetParentsByCode(string fieldCode, string code, string order)
        {
            return DbHelper.GetParentsByCode(CurrentTableName, fieldCode, code, order);
        }
        #endregion

        #region public DataTable GetChildrens(string fieldId, string id, string fieldParentId, string order) 获取子节点列表
        /// <summary>
        /// 获取子节点列表
        /// </summary>
        /// <param name="fieldId">主键字段</param>
        /// <param name="id">值</param>
        /// <param name="fieldParentId">父亲节点字段</param>
        /// <param name="order">排序</param>
        /// <returns>数据表</returns>
        public DataTable GetChildrens(string fieldId, string id, string fieldParentId, string order)
        {
            return DbHelper.GetChildrens(CurrentTableName, fieldId, id, fieldParentId, order);
        }
        #endregion

        #region public DataTable GetChildrensByCode(string fieldCode, string code, string order) 获取子节点列表
        /// <summary>
        /// 获取子节点列表
        /// </summary>
        /// <param name="fieldCode">编码字段</param>
        /// <param name="code">编码</param>
        /// <param name="order">排序</param>
        /// <returns>数据表</returns>
        public DataTable GetChildrensByCode(string fieldCode, string code, string order)
        {
            return DbHelper.GetChildrensByCode(CurrentTableName, fieldCode, code, order);
        }
        #endregion

        #region public DataTable GetParentChildrensByCode(string fieldCode, string code, string order) 获取父子节点列表
        /// <summary>
        /// 获取父子节点列表
        /// </summary>
        /// <param name="fieldCode">编码字段</param>
        /// <param name="code">编码</param>
        /// <param name="order">排序</param>
        /// <returns>数据表</returns>
        public DataTable GetParentChildrensByCode(string fieldCode, string code, string order)
        {
            return DbHelper.GetParentChildrensByCode(CurrentTableName, fieldCode, code, order);
        }
        #endregion


        #region public string[] GetParentsIdByCode(string fieldCode, string code) 获取父节点列表
        /// <summary>
        /// 获取父节点列表
        /// </summary>
        /// <param name="fieldCode">编码字段</param>
        /// <param name="code">编码</param>
        /// <returns>主键数组</returns>
        public string[] GetParentsIdByCode(string fieldCode, string code)
        {
            return DbHelper.GetParentsIdByCode(CurrentTableName, fieldCode, code, string.Empty);
        }
        #endregion

        #region public string[] GetChildrensId(string fieldId, string id, string fieldParentId) 获取子节点列表
        /// <summary>
        /// 获取子节点列表
        /// </summary>
        /// <param name="fieldId">主键字段</param>
        /// <param name="id">值</param>
        /// <param name="fieldParentId">父亲节点字段</param>
        /// <returns>主键数组</returns>
        public string[] GetChildrensId(string fieldId, string id, string fieldParentId)
        {
            return DbHelper.GetChildrensId(CurrentTableName, fieldId, id, fieldParentId, string.Empty);
        }
        #endregion

        #region public string[] GetChildrensByIdCode(string fieldCode, string code) 获取子节点列表
        /// <summary>
        /// 获取子节点列表
        /// </summary>
        /// <param name="fieldCode">编码字段</param>
        /// <param name="code">编码</param>
        /// <returns>主键数组</returns>
        public string[] GetChildrensIdByCode(string fieldCode, string code)
        {
            return DbHelper.GetChildrensIdByCode(CurrentTableName, fieldCode, code, string.Empty);
        }
        #endregion

        #region public string[] GetParentChildrensIdByCode(string fieldCode, string code) 获取父子节点列表
        /// <summary>
        /// 获取父子节点列表
        /// </summary>
        /// <param name="fieldCode">编码字段</param>
        /// <param name="code">编码</param>
        /// <returns>主键数组</returns>
        public string[] GetParentChildrensIdByCode(string fieldCode, string code)
        {
            return DbHelper.GetParentChildrensIdByCode(CurrentTableName, fieldCode, code, string.Empty);
        }
        #endregion

        #region public string GetParentIdByCode(string fieldCode, string code) 获取父节点
        /// <summary>
        /// 获取父节点
        /// </summary>
        /// <param name="fieldCode">编码字段</param>
        /// <param name="code">编号</param>
        /// <returns>主键</returns>
        public string GetParentIdByCode(string fieldCode, string code)
        {
            return DbHelper.GetParentIdByCode(CurrentTableName, fieldCode, code);
        }
        #endregion
    }
}