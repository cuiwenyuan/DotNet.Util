//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System.Collections.Generic;

namespace DotNet.Business
{
    using Model;
    using Util;

    /// <summary>
    /// BaseUserContactManager
    /// 用户管理
    /// 
    /// 修改记录
    /// 
    ///		2014.01.13 版本：1.0 JiRiGaLa	主键整理。
    /// 
    /// <author>
    ///		<name>JiRiGaLa</name>
    ///		<date>2014.01.13</date>
    /// </author> 
    /// </summary>
    public partial class BaseUserContactManager : BaseManager
    {
        // 当前的锁
        // private static object locker = new Object();

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="identity">自增量方式</param>
        /// <param name="returnId">返回主鍵</param>
        /// <returns>主键</returns>
        public string Add(BaseUserContactEntity entity, bool identity = false, bool returnId = false)
        {
            Identity = identity;
            ReturnId = returnId;
            return AddEntity(entity);
        }

        /// <summary>
        /// 按电子邮件获取用户主键
        /// </summary>
        /// <param name="email">电子邮件地址</param>
        /// <returns>主键</returns>
        public string GetIdByEmail(string email)
        {
            var parameters = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(BaseUserContactEntity.FieldEmail, email)
            };
            return GetId(parameters);
        }

        #region public int RemoveMobileBinding(string mobile) 解除手机认证帮定

        /// <summary>
        /// 解除手机认证帮定
        /// </summary>
        /// <param name="mobile"></param>
        /// <returns>影响行数</returns>
        public int RemoveMobileBinding(string mobile)
        {
            var result = 0;
            var sqlBuilder = new SqlBuilder(DbHelper);
            sqlBuilder.BeginUpdate(CurrentTableName);
            sqlBuilder.SetNull(BaseUserContactEntity.FieldMobile);
            sqlBuilder.SetWhere(BaseUserContactEntity.FieldMobile, mobile);
            result = sqlBuilder.EndUpdate();

            /*
            // 把主库的数据库认证也去掉
            string connectionString = ConfigurationHelper.AppSettings("K8Connection", BaseSystemInfo.EncryptDbConnection);
            if (!string.IsNullOrEmpty(connectionString))
            {
                IDbHelper dbHelper = DbHelperFactory.GetHelper(CurrentDbType.Oracle, connectionString);
                string commandText = string.Format(@"UPDATE TAB_USER 
                                                        SET Mobile = null
                                                      WHERE Id = {0} "
                    , dbHelper.GetParameter("Mobile"));
                dbHelper.ExecuteNonQuery(commandText, new IDbDataParameter[] { 
                    dbHelper.MakeParameter("Mobile", mobile)
                });
            }
            */

            return result;
        }
        #endregion

        /// <summary>
        /// 重新设置缓存（重新强制设置缓存）可以提供外部调用的
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns>用户信息</returns>
        public static BaseUserContactEntity SetCache(string id)
        {
            BaseUserContactEntity result = null;

            var manager = new BaseUserContactManager();
            result = manager.GetEntity(id);
            if (result != null)
            {
                SetCache(result);
            }

            return result;
        }

        #region public static string GetEmailByCache(string id) 通过编号获取选项的显示内容
        /// <summary>
        /// 通过主键获取姓名
        /// 这里是进行了内存缓存处理，减少数据库的I/O处理，提高程序的运行性能，
        /// 若有数据修改过，重新启动一下程序就可以了，这些基础数据也不是天天修改来修改去的，
        /// 所以没必要过度担忧，当然有需要时也可以写个刷新缓存的程序
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns>显示值</returns>
        public static string GetEmailByCache(string id)
        {
            var result = string.Empty;

            var entity = GetEntityByCache(id);
            if (entity != null)
            {
                result = entity.Email;
            }

            return result;
        }
        #endregion

        /// <summary>
        /// 提高效率，封装一个方法，让调用的人更方便调用
        /// 2015-11-11 吉日嘎拉 要考虑多个手机号码，问题就更加复杂了，其实简单就是硬道理
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns>手机号码</returns>
        public static string GetMobileByCache(string id)
        {
            var result = string.Empty;

            var entity = GetEntityByCache(id);
            if (entity != null)
            {
                result = entity.Mobile;
            }

            return result;
        }
        /// <summary>
        /// 从缓存中获取电话号码
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string GetTelephoneByCache(string id)
        {
            var result = string.Empty;

            var entity = GetEntityByCache(id);
            if (entity != null)
            {
                result = entity.Telephone;
            }

            return result;
        }

        /// <summary>
        /// 缓存预热,强制重新缓存
        /// </summary>
        /// <returns>影响行数</returns>
        public static int CachePreheating()
        {
            var result = 0;

            // 把所有的数据都缓存起来的代码
            var manager = new BaseUserContactManager();
            using (var dataReader = manager.ExecuteReader())
            {
                while (dataReader.Read())
                {
                    var entity = BaseEntity.Create<BaseUserContactEntity>(dataReader, false);
                    SetCache(entity);
                    result++;
                    System.Console.WriteLine(result + " : " + entity.Telephone);
                }
                dataReader.Close();
            }

            return result;
        }
    }
}