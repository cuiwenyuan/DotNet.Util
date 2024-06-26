﻿//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2024, DotNet.
//-----------------------------------------------------------------

namespace DotNet.Business
{
    using Model;
    using Util;

    /// <summary>
    /// BaseOrganizationManager
    /// 组织机构
    ///
    /// 修改记录
    /// 
    ///		2016.02.02 版本：1.0 JiRiGaLa	进行独立。
    ///
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2016.02.02</date>
    /// </author>
    /// </summary>
    public partial class BaseOrganizationManager : BaseManager
    {
        /// <summary>
        /// 编辑之后，需要重新刷新缓存，否则其他读取数据的地方会乱了，或者不及时了
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns>影响行数</returns>
        public int AfterUpdate(BaseOrganizationEntity entity)
        {
            var result = 0;
            SetCache(entity);
            return result;
        }
    }
}