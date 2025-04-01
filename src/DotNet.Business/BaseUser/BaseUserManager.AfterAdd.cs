//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2025, DotNet.
//-----------------------------------------------------------------

namespace DotNet.Business
{
    using Model;
    using Util;

    /// <summary>
    /// BaseUserManager
    /// 用户管理
    /// 
    /// 修改记录
    /// 
    ///		2015.05.22 版本：1.0 JiRiGaLa 删除之后的处理。
    /// 
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2015.05.22</date>
    /// </author> 
    /// </summary>
    public partial class BaseUserManager : BaseManager
    {
        #region public int AfterAdd(BaseUserEntity entity)
        /// <summary>
        /// 添加之后，需要重新刷新缓存，否则其他读取数据的地方会乱了，或者不及时了
        /// </summary>
        /// <param name="entity">用户实体</param>
        /// <returns></returns>
        public int AfterAdd(BaseUserEntity entity)
        {
            var result = 0;
            // 2016-01-28 更新用户缓存
            SetCache(entity);
            CachePreheatingSpelling(entity);
            // AfterAddSynchronous(entity);

            return result;
        }
        #endregion
    }
}