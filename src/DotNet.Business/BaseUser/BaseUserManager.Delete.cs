//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System.Collections.Generic;

namespace DotNet.Business
{
    using Model;

    /// <summary>
    /// BaseUserManager
    /// 用户管理
    /// 
    /// 修改记录
    /// 
    ///		2011.10.17 版本：1.0 JiRiGaLa	主键整理。
    /// 
    /// <author>
    ///		<name>JiRiGaLa</name>
    ///		<date>2011.10.17</date>
    /// </author> 
    /// </summary>
    public partial class BaseUserManager : BaseManager
    {
        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns>影响行数</returns>
        public int Delete(string id)
        {
            var result = 0;

            result = Delete(new List<KeyValuePair<string, object>> { new KeyValuePair<string, object>(BaseUserEntity.FieldId, id) });

            // AfterDeleted(new object[] { id }, true, true);

            return result;
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="enabled"></param>
        /// <param name="recordUser"></param>
        /// <param name="tableVersion">版本默认4为老版本</param>
        /// <returns></returns>
        public override int SetDeleted(object[] ids, bool enabled = true, bool recordUser = true, int tableVersion = 4)
        {
            //LogUtil.WriteLog("SetDeleted", "Log");


            var result = 0;
            result = base.SetDeleted(ids, enabled, recordUser, tableVersion);

            //LogUtil.WriteLog("AfterSetDeleted", "Log");
            for (var i = 0; i < ids.Length; i++)
            {
                var id = ids[i].ToString();
                var entity = GetEntityByCache(id);
                AfterUpdate(entity);
            }
           
            // result = AfterDeleted(ids, enabled, modifiedUser);
            return result;
        }
    }
}