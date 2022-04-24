//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Reflection;
using System.Text;

namespace DotNet.Model
{
    /// <remarks>
    /// BaseEntity
    /// 基础实体
    /// 
    /// 修改记录
    /// 
    ///	版本：5.0 2022.01.12  Troy.Cui    参数完善。
    ///	版本：1.0 2015.07.08  JiRiGaLa    IDataReader 进行完善。
    ///	
    /// <author>  
    ///		<name>Troy.Cui</name>
    ///		<date>2015.07.08</date>
    /// </author> 
    /// </remarks>
    [Serializable]
    public abstract class BaseEntity : IBaseEntity
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        protected BaseEntity()
        {
        }

        #region IBaseEntity 成员
        /// <summary>
        /// GetFrom
        /// </summary>
        /// <param name="dr"></param>
        /// <returns></returns>
        protected abstract BaseEntity GetFrom(IDataRow dr);

        /// <summary>
        /// GetFrom
        /// </summary>
        /// <param name="dr"></param>
        /// <returns></returns>
        public BaseEntity GetFrom(DataRow dr)
        {
            return GetFrom(new DrDataRow(dr));
        }
        /// <summary>
        /// GetFrom
        /// </summary>
        /// <param name="dataReader"></param>
        /// <returns></returns>
        public BaseEntity GetFrom(IDataReader dataReader)
        {
            return GetFrom(new DrDataReader(dataReader));
        }
        /// <summary>
        /// GetSingle
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public BaseEntity GetSingle(DataTable dt)
        {
            if ((dt == null) || (dt.Rows.Count == 0))
            {
                return null;
            }
            foreach (DataRow dr in dt.Rows)
            {
                GetFrom(dr);
                break;
            }
            return this;
        }

        #endregion

        /// <summary>
        /// 从数据行读取
        /// </summary>
        /// <param name="dr">数据行</param>
        public virtual void GetFromExtend(DataRow dr)
        {
            GetFromExtend(new DrDataRow(dr));
        }

        /// <summary>
        /// 从数据流读取
        /// </summary>
        /// <param name="dataReader">数据流</param>
        public virtual void GetFromExtend(IDataReader dataReader)
        {
            GetFromExtend(new DrDataReader(dataReader));
        }

        /// <summary>
        /// 从自定义数据流读取
        /// </summary>
        /// <param name="dr">数据流</param>
        public virtual void GetFromExtend(IDataRow dr)
        {

        }

        /// <summary>
        /// 可以按各种特殊需要获取字符串的长度
        /// </summary>
        /// <param name="text">字符串</param>
        /// <param name="encoding">编码，默认gb2312，可选UTF8</param>
        /// <returns>长度</returns>
        private int GetLength(string text, string encoding = "gb2312")
        {
            return Encoding.GetEncoding(encoding).GetBytes(text).Length;
        }

        #region public virtual bool IsValid(out string message)
        /// <summary>
        /// 后台输入验证
        /// 2013.06.12 JiRiGaLa 完善
        /// </summary>
        /// <returns></returns>
        public virtual bool IsValid(out string message)
        {
            var result = true;
            message = string.Empty;
            foreach (var propertyInfo in GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                var name = propertyInfo.Name;
                var value = propertyInfo.GetValue(this, null);
                if ((propertyInfo.PropertyType.IsValueType || propertyInfo.PropertyType.Name.StartsWith("String")) && value != null)
                {
                    var validObject = (GetType().GetProperty(name)?.GetCustomAttributes(typeof(StringLengthAttribute), false));
                    if (validObject != null && validObject.Length > 0)
                    {
                        var stringLengthAttribute = (StringLengthAttribute)validObject[0];
                        if (stringLengthAttribute.MaximumLength < GetLength(value.ToString()))
                        {
                            result = false;
                            // name 这个可以是返回的字段
                            // value 这个可以是返回的出错的内容
                            // 这里是返回消息
                            message = stringLengthAttribute.ErrorMessage;
                            break;
                        }
                    }
                }
            }
            return result;
        }
        #endregion

        #region public static T Create<T>()
        /// <summary>
        /// 创建实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Create<T>() where T : BaseEntity, new()
        {
            return new T();
        }
        #endregion

        #region public static T Create<T>(DataTable dt)
        /// <summary>
        /// 创建实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static T Create<T>(DataTable dt) where T : BaseEntity, new()
        {
            if ((dt == null) || (dt.Rows.Count == 0))
            {
                return null;
            }
            var entity = Create<T>();
            entity.GetFrom(dt.Rows[0]);
            return entity;
        }
        #endregion

        #region public static T Create<T>(DataRow dr)
        /// <summary>
        /// 创建实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dr"></param>
        /// <returns></returns>
        public static T Create<T>(DataRow dr) where T : BaseEntity, new()
        {
            var entity = Create<T>();
            entity.GetFrom(dr);
            return entity;
        }
        #endregion

        #region public static T Create<T>(IDataReader dataReader, bool close = true)
        /// <summary>
        /// 创建实体(没有对象时需要返回null)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataReader"></param>
        /// <param name="close"></param>
        /// <returns></returns>
        public static T Create<T>(IDataReader dataReader, bool close = true) where T : BaseEntity, new()
        {
            // 2015-07-14 没有对象时需要返回 null, 否则很多程序逻辑需要修改了
            T entity = null;
            if (close)
            {
                using (dataReader)
                {
                    if (dataReader != null)
                    {
                        while (dataReader.Read())
                        {
                            entity = Create<T>();
                            entity.GetFrom(dataReader);
                            //只读取第一行
                            break;
                        }
                    }                    
                }
            }
            else
            {
                entity = Create<T>();
                entity.GetFrom(dataReader);
            }
            return entity;
        }
        #endregion

        #region GetList public static List<T> GetList<T>(DataTable dt)
        /// <summary>
        /// 获取List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static List<T> GetList<T>(DataTable dt) where T : BaseEntity, new()
        {
            if ((dt == null) || (dt.Rows.Count == 0))
            {
                return new List<T>();
            }
            var ls = new List<T>();
            foreach (DataRow dr in dt.Rows)
            {
                ls.Add(Create<T>(dr));
            }
            return ls;
        }
        #endregion

        #region public static List<T> GetList<T>(IDataReader dataReader)
        /// <summary>
        /// 获取List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataReader"></param>
        /// <returns></returns>
        public static List<T> GetList<T>(IDataReader dataReader) where T : BaseEntity, new()
        {
            var ls = new List<T>();
            if ((dataReader == null))
            {
                return ls;
            }
            using (dataReader)
            {
                while (dataReader.Read())
                {
                    ls.Add(Create<T>(dataReader, false));
                }
            }
            return ls;
        }
        #endregion
    }
}