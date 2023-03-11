//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2023, DotNet.
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
    ///		2015.06.15 版本：1.0 JiRiGaLa 编辑之后的处理。
    /// 
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2015.06.15</date>
    /// </author> 
    /// </summary>
    public partial class BaseUserManager : BaseManager
    {
        #region public int AfterUpdate(BaseUserEntity entity)
        /// <summary>
        /// 编辑之后，需要重新刷新缓存，否则其他读取数据的地方会乱了，或者不及时了
        /// </summary>
        /// <param name="entity">用户实体</param>
        /// <returns>影响行数</returns>
        public int AfterUpdate(BaseUserEntity entity)
        {
            var result = 0;
            // 用户的缓存需要更新
            SetCache(entity);
            // 用户的联系方式需要更新
            BaseUserContactManager.SetCache(entity.Id);
            // 2016-02-02 吉日嘎拉，修改数据后重新设置缓存。
            CachePreheatingSpelling(entity);
            // AfterUpdateSynchronous(entity);
            return result;
        }
        #endregion
    }
}