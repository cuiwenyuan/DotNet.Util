using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DotNet.Util
{
    /// <summary>
    /// 实体转表结构
    /// </summary>
    public class EntityUtil
    {
        /// <summary>
        /// 获取指定实体类的表表达式
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static TableExpression GetTableExpression<T>(T t)
        {
            return GetTableExpression(typeof(T));
        }
        /// <summary>
        /// 获取指定实体类型的表表达式
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static TableExpression GetTableExpression(Type type)
        {
            var key = "getTableExpression" + type.FullName;
            if (ReflectionUtil.CacheDictionary.TryGetValue(key, out var cacheValue))
            {
                return (TableExpression)cacheValue;
            }

            var result = new TableExpression(type);
            ReflectionUtil.CacheDictionary.TryAdd(key, result);
            return result;
        }
    }
}
