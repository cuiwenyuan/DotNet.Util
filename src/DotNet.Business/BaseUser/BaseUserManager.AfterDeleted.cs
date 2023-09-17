//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2023, DotNet.
//-----------------------------------------------------------------

namespace DotNet.Business
{
    using Util;
    using Model;

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
        #region public int AfterDeleted(object[] ids, bool enabled = false, bool recordUser = false)
        /// <summary>
        /// 删除后
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="enabled"></param>
        /// <param name="recordUser"></param>
        /// <returns></returns>
        public int AfterDeleted(object[] ids, bool enabled = false, bool recordUser = false)
        {
            var result = 0;

            for (var i = 0; i < ids.Length; i++)
            {
                var id = ids[i].ToString();

                var entity = GetEntityByCache(id);

                CachePreheatingSpelling(entity);

            }

            if (ids.Length > 0)
            {
                ////删除后把已经删除的数据搬迁到被删除表里去。
                //string where = BaseUserEntity.FieldId + " IN (" + StringUtil.ArrayToList((string[])ids, "") + ") ";
                //string commandText = @"INSERT INTO BASEUSER_DELETED SELECT * FROM " + BaseUserEntity.CurrentTableName + " WHERE " + where;
                //IDbHelper dbHelper1 = DbHelperFactory.Create(BaseSystemInfo.UserCenterDbType, BaseSystemInfo.UserCenterDbConnection);
                //dbHelper1.ExecuteNonQuery(commandText);

                // 进行删除操作
                Delete(ids);
            }

            return result;
        }
        #endregion
    }
}