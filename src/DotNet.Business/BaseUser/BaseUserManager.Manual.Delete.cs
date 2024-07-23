//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2024, DotNet.
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
    ///		<name>Troy.Cui</name>
    ///		<date>2011.10.17</date>
    /// </author> 
    /// </summary>
    public partial class BaseUserManager : BaseManager
    {
        #region public override int Delete(string id) 删除实体
        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns>影响行数</returns>
        public override int Delete(string id)
        {
            var result = 0;

            result = Delete(new List<KeyValuePair<string, object>> { new KeyValuePair<string, object>(BaseUserEntity.FieldId, id) });

            // AfterDeleted(new object[] { id }, true, true);

            return result;
        }
        #endregion

        #region public override int SetDeleted(object[] ids, bool changeEnabled = true, bool recordUser = true, bool baseOperationLog = true, string clientIp = null, bool checkAllowDelete = false)
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="changeEnabled">修改有效状态</param>
        /// <param name="recordUser">记录修改用户</param>
        /// <param name="baseOperationLog">集中记录操作日志</param>
        /// <param name="clientIp">客户端IP</param>
        /// <param name="checkAllowDelete">检查允许删除</param>
        /// <returns></returns>
        public override int SetDeleted(object[] ids, bool changeEnabled = true, bool recordUser = true, bool baseOperationLog = true, string clientIp = null, bool checkAllowDelete = false)
        {
            var result = 0;
            result = base.SetDeleted(ids, changeEnabled: changeEnabled, recordUser: recordUser, checkAllowDelete: checkAllowDelete);

            for (var i = 0; i < ids.Length; i++)
            {
                var id = ids[i].ToString();
                var entity = GetEntityByCache(id);
                AfterUpdate(entity);
            }

            return result;
        }
        #endregion
    }
}