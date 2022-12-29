//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2022, DotNet.
//-----------------------------------------------------------------

using DotNet.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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
    ///	版本：5.0 2022.10.17  Troy.Cui    基类的公共字段、属性、赋值等完善。
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

        #region 公共属性

        /// <summary>
        /// 编号
        /// </summary>
        [Key]
        [FieldDescription("编号")]
        [Description("编号")]
        [Column(FieldId)]
        public int Id { get; set; }

        /// <summary>
        /// 排序编号
        /// </summary>
        [FieldDescription("排序编号")]
        [Description("排序编号")]
        [Column(FieldSortCode)]
        public int SortCode { get; set; } = 0;

        /// <summary>
        /// 是否删除
        /// </summary>
        [FieldDescription("是否删除")]
        [Description("是否删除")]
        [Column(FieldDeleted)]
        public int Deleted { get; set; } = 0;

        /// <summary>
        /// 是否有效
        /// </summary>
        [FieldDescription("是否有效")]
        [Description("是否有效")]
        [Column(FieldEnabled)]
        public int Enabled { get; set; } = 1;

        /// <summary>
        /// 创建时间
        /// </summary>
        [FieldDescription("创建时间")]
        [Description("创建时间")]
        [Column(FieldCreateTime)]
        public DateTime CreateTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 创建人编号
        /// </summary>
        [FieldDescription("创建人编号")]
        [Description("创建人编号")]
        [Column(FieldCreateUserId)]
        public int CreateUserId { get; set; } = 0;

        /// <summary>
        /// 创建人用户名
        /// </summary>
        [FieldDescription("创建人用户名")]
        [Description("创建人用户名")]
        [Column(FieldCreateUserName)]
        public string CreateUserName { get; set; } = string.Empty;

        /// <summary>
        /// 创建人姓名
        /// </summary>
        [FieldDescription("创建人姓名")]
        [Description("创建人姓名")]
        [Column(FieldCreateBy)]
        public string CreateBy { get; set; } = string.Empty;

        /// <summary>
        /// 创建IP
        /// </summary>
        [FieldDescription("创建IP")]
        [Description("创建IP")]
        [Column(FieldCreateIp)]
        public string CreateIp { get; set; } = string.Empty;

        /// <summary>
        /// 修改时间
        /// </summary>
        [FieldDescription("修改时间")]
        [Description("修改时间")]
        [Column(FieldUpdateTime)]
        public DateTime UpdateTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 修改人编号
        /// </summary>
        [FieldDescription("修改人编号")]
        [Description("修改人编号")]
        [Column(FieldUpdateUserId)]
        public int UpdateUserId { get; set; } = 0;

        /// <summary>
        /// 修改人姓名
        /// </summary>
        [FieldDescription("修改人姓名")]
        [Description("修改人姓名")]
        [Column(FieldUpdateBy)]
        public string UpdateBy { get; set; } = string.Empty;

        /// <summary>
        /// 修改人用户名
        /// </summary>
        [FieldDescription("修改人用户名")]
        [Description("修改人用户名")]
        [Column(FieldUpdateUserName)]
        public string UpdateUserName { get; set; } = string.Empty;

        /// <summary>
        /// 修改IP
        /// </summary>
        [FieldDescription("修改IP")]
        [Description("修改IP")]
        [Column(FieldUpdateIp)]
        public string UpdateIp { get; set; } = string.Empty;

        #endregion

        #region 公共字段

        ///<summary>
        /// 编号
        ///</summary>
        public const string FieldId = "Id";

        ///<summary>
        /// 排序编号
        ///</summary>
        public const string FieldSortCode = "SortCode";

        ///<summary>
        /// 是否删除
        ///</summary>
        public const string FieldDeleted = "Deleted";

        ///<summary>
        /// 是否有效
        ///</summary>
        public const string FieldEnabled = "Enabled";

        ///<summary>
        /// 创建时间
        ///</summary>
        public const string FieldCreateTime = "CreateTime";

        ///<summary>
        /// 创建人编号
        ///</summary>
        public const string FieldCreateUserId = "CreateUserId";

        ///<summary>
        /// 创建人用户名
        ///</summary>
        public const string FieldCreateUserName = "CreateUserName";

        ///<summary>
        /// 创建人姓名
        ///</summary>
        public const string FieldCreateBy = "CreateBy";

        ///<summary>
        /// 创建IP
        ///</summary>
        public const string FieldCreateIp = "CreateIp";

        ///<summary>
        /// 修改时间
        ///</summary>
        public const string FieldUpdateTime = "UpdateTime";

        ///<summary>
        /// 修改人编号
        ///</summary>
        public const string FieldUpdateUserId = "UpdateUserId";

        ///<summary>
        /// 修改人姓名
        ///</summary>
        public const string FieldUpdateBy = "UpdateBy";

        ///<summary>
        /// 修改人用户名
        ///</summary>
        public const string FieldUpdateUserName = "UpdateUserName";

        ///<summary>
        /// 修改IP
        ///</summary>
        public const string FieldUpdateIp = "UpdateIp";

        #endregion

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

        #region public virtual void GetFromExtend(DataRow dr) 从数据行读取
        /// <summary>
        /// 从数据行读取
        /// </summary>
        /// <param name="dr">数据行</param>
        public virtual void GetFromExtend(DataRow dr)
        {
            GetFromExtend(new DrDataRow(dr));
        }
        #endregion

        #region public virtual void GetFromExtend(IDataReader dataReader) 从数据流读取
        /// <summary>
        /// 从数据流读取
        /// </summary>
        /// <param name="dataReader">数据流</param>
        public virtual void GetFromExtend(IDataReader dataReader)
        {
            GetFromExtend(new DrDataReader(dataReader));
        }

        #endregion

        #region public virtual void GetFromExtend(IDataRow dr) 从自定义数据流读取
        /// <summary>
        /// 从自定义数据流读取
        /// </summary>
        /// <param name="dr">数据流</param>
        public virtual void GetFromExtend(IDataRow dr)
        {
        }
        #endregion

        #region public virtual void GetBase(IDataRow dr) 标准字段赋值
        /// <summary>
        /// 标准字段赋值
        /// </summary>
        /// <param name="dr">数据流</param>
        public virtual void GetBase(IDataRow dr)
        {
            if (dr.ContainsColumn(FieldId))
            {
                Id = BaseUtil.ConvertToInt(dr[FieldId]);
            }
            if (dr.ContainsColumn(FieldSortCode))
            {
                SortCode = BaseUtil.ConvertToInt(dr[FieldSortCode]);
            }
            if (dr.ContainsColumn(FieldDeleted))
            {
                Deleted = BaseUtil.ConvertToInt(dr[FieldDeleted]);
            }
            if (dr.ContainsColumn(FieldEnabled))
            {
                Enabled = BaseUtil.ConvertToInt(dr[FieldEnabled]);
            }
            if (dr.ContainsColumn(FieldCreateTime))
            {
                CreateTime = BaseUtil.ConvertToDateTime(dr[FieldCreateTime]);
            }
            if (dr.ContainsColumn(FieldCreateUserId))
            {
                CreateUserId = BaseUtil.ConvertToInt(dr[FieldCreateUserId]);
            }
            if (dr.ContainsColumn(FieldCreateUserName))
            {
                CreateUserName = BaseUtil.ConvertToString(dr[FieldCreateUserName]);
            }
            if (dr.ContainsColumn(FieldCreateBy))
            {
                CreateBy = BaseUtil.ConvertToString(dr[FieldCreateBy]);
            }
            if (dr.ContainsColumn(FieldCreateIp))
            {
                CreateIp = BaseUtil.ConvertToString(dr[FieldCreateIp]);
            }
            if (dr.ContainsColumn(FieldUpdateTime))
            {
                UpdateTime = BaseUtil.ConvertToDateTime(dr[FieldUpdateTime]);
            }
            if (dr.ContainsColumn(FieldUpdateUserId))
            {
                UpdateUserId = BaseUtil.ConvertToInt(dr[FieldUpdateUserId]);
            }
            if (dr.ContainsColumn(FieldUpdateBy))
            {
                UpdateBy = BaseUtil.ConvertToString(dr[FieldUpdateBy]);
            }
            if (dr.ContainsColumn(FieldUpdateUserName))
            {
                UpdateUserName = BaseUtil.ConvertToString(dr[FieldUpdateUserName]);
            }
            if (dr.ContainsColumn(FieldUpdateIp))
            {
                UpdateIp = BaseUtil.ConvertToString(dr[FieldUpdateIp]);
            }
        }

        #endregion

        #region private int GetLength(string text, string encoding = "gb2312")

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

        #endregion

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
        /// <param name="onlyFirstRow">只读取第一行</param>
        /// <returns></returns>
        public static T Create<T>(IDataReader dataReader, bool onlyFirstRow = true) where T : BaseEntity, new()
        {
            T entity = null;
            if (onlyFirstRow)
            {
                if (dataReader != null && !dataReader.IsClosed)
                {
                    while (dataReader.Read())
                    {
                        entity = Create<T>();
                        entity.GetFrom(dataReader);
                        //只读取第一行
                        break;
                    }
                    dataReader.Close();
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

        #region public static List<T> GetList<T>(DataTable dt)
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
            if (dataReader != null && !dataReader.IsClosed)
            {
                while (dataReader.Read())
                {
                    ls.Add(Create<T>(dataReader, false));
                }
                dataReader.Close();
            }

            return ls;
        }
        #endregion
    }
}