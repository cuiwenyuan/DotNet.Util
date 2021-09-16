//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;

namespace DotNet.Business
{
    using Model;
    using Util;

    /// <remarks>
    /// BaseStaffManager
    /// 职员管理
    /// 
    /// 修改记录
    /// 
    ///	版本：1.0 2012.12.17    JiRiGaLa    选项管理从缓存读取，通过编号显示名称的函数完善。
    ///	
    /// <author>  
    ///		<name>Troy.Cui</name>
    ///		<date>2012.12.17</date>
    /// </author> 
    /// </remarks>
    public partial class BaseStaffManager
    {
        // 当前的锁
        private static object _locker = new Object();

        #region public static void ClearCache() 清除缓存
        /// <summary>
        /// 清除缓存
        /// </summary>
        public static void ClearCache()
        {
            lock (BaseSystemInfo.UserLock)
            {
                CacheUtil.Remove(BaseStaffEntity.TableName);
            }
        }
        #endregion

        #region public static List<BaseStaffEntity> GetEntities() 获取职员表，从缓存读取
        /// <summary>
        /// 获取职员表，从缓存读取
        /// </summary>
        public static List<BaseStaffEntity> GetEntities()
        {
            return CacheUtil.Cache(BaseStaffEntity.TableName, () => new BaseStaffManager(BaseStaffEntity.TableName).GetList<BaseStaffEntity>(), true);
        }
        #endregion

        #region public static string GetRealName(string id) 通过编号获取选项的显示内容
        /// <summary>
        /// 通过编号获取选项的显示内容
        /// 这里是进行了内存缓存处理，减少数据库的I/O处理，提高程序的运行性能，
        /// 若有数据修改过，重新启动一下程序就可以了，这些基础数据也不是天天修改来修改去的，
        /// 所以没必要过度担忧，当然有需要时也可以写个刷新缓存的程序
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns>显示值</returns>
        public static string GetRealName(string id)
        {
            var result = id;
            if (!string.IsNullOrEmpty(id))
            {
                var entityList = GetEntities();
                var staffEntity = entityList.FirstOrDefault(entity => entity.Id.HasValue && entity.Id.ToString().Equals(id));
                if (staffEntity != null)
                {
                    result = staffEntity.RealName;
                }
            }
            return result;
        }
        #endregion

        /// <summary>
        /// 从缓存获取实体
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static BaseStaffEntity GetEntityByCodeByCache(string code)
        {
            BaseStaffEntity result = null;
            if (!string.IsNullOrEmpty(code))
            {
                var cacheKey = "StaffByCode" + code;
                result = CacheUtil.Cache(cacheKey, () => new BaseStaffManager().GetEntityByCode(code), true);
            }
            return result;
        }
    }
}