//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2023, DotNet.
//-----------------------------------------------------------------

namespace DotNet.Business
{
    using Util;

    /// <summary>
    ///	BaseManager
    /// 通用基类部分
    /// 
    /// 修改记录
    /// 
    ///		2018.08.29 版本：Troy.Cui进行扩展。
    ///
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2018.08.29</date>
    /// </author> 
    /// </summary>
    public partial class BaseManager : IBaseManager
    {
        #region public virtual bool RemoveCache()

        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <returns></returns>
        public virtual bool RemoveCache()
        {
            var result = false;
            var cacheKey = "DataTable." + CurrentTableName;
            if (UserInfo != null)
            {
                cacheKey += "." + UserInfo.CompanyId;
            }
            result = CacheUtil.Remove(cacheKey);
            return result;
        }
        #endregion

        #region public virtual bool RemoveCache(int id)

        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <param name="id">编号</param>
        /// <returns></returns>
        public virtual bool RemoveCache(int id)
        {
            var result = false;
            var cacheKeyEntity = CurrentTableName + ".Entity.";
            if (id == 0)
            {
                CacheUtil.RemoveByRegex("^" + cacheKeyEntity + "+\\d+$");
            }
            else
            {
                cacheKeyEntity += id;
                result = CacheUtil.Remove(cacheKeyEntity);
            }
            return result;
        }
        #endregion

        #region public virtual bool RemoveCache(long id)

        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <param name="id">编号</param>
        /// <returns></returns>
        public virtual bool RemoveCache(long id)
        {
            var result = false;
            var cacheKeyEntity = CurrentTableName + ".Entity.";
            if (id == 0)
            {
                CacheUtil.RemoveByRegex("^" + cacheKeyEntity + "+\\d+$");
            }
            else
            {
                cacheKeyEntity += id;
                result = CacheUtil.Remove(cacheKeyEntity);
            }
            return result;
        }
        #endregion

        #region public virtual bool RemoveCache(string id)

        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <param name="id">编号</param>
        /// <returns></returns>
        public virtual bool RemoveCache(string id)
        {
            var result = false;
            var cacheKeyEntity = CurrentTableName + ".Entity.";
            if (string.IsNullOrEmpty(id))
            {
                CacheUtil.RemoveByRegex("^" + cacheKeyEntity + "+\\w+$");
            }
            else
            {
                cacheKeyEntity += id;
                result = CacheUtil.Remove(cacheKeyEntity);
            }
            return result;
        }
        #endregion
    }
}