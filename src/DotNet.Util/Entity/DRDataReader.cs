//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2022, DotNet.
//-----------------------------------------------------------------

using System.Data;

namespace DotNet.Model
{
    /// <summary>
    /// DrDataReader
    /// </summary>
    public class DrDataReader : IDataRow
    {
        private readonly IDataReader _dr;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dr"></param>
        public DrDataReader(IDataReader dr)
        {
            _dr = dr;
        }

        #region IDataRow 成员
        /// <summary>
        /// this
        /// </summary>
        /// <param name="name">名称</param>
        /// <returns></returns>
        public object this[string name] => _dr[name];
        /// <summary>
        /// this
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public object this[int i] => _dr[i];

        /// <summary>
        /// 判断是否含有指定字段
        /// </summary>
        /// <param name="columnName">字段名字</param>
        /// <returns></returns>
        public bool ContainsColumn(string columnName)
        {
            for (var i = 0; i < _dr.FieldCount; i++)
            {
                // 2015-09-15 吉日嘎拉 这个需要大小写问题注意，Oracle 里会会自动变成大写
                if (_dr.GetName(i).Equals(columnName, System.StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }
        #endregion
    }
}
