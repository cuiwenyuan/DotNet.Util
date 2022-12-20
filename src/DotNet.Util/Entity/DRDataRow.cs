//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System.Data;

namespace DotNet.Model
{
    /// <summary>
    /// DrDataRow
    /// </summary>
    public class DrDataRow : IDataRow
    {
        private readonly DataRow _dr;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dr"></param>
        public DrDataRow(DataRow dr)
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
        /// <param name="columnName"></param>
        /// <returns></returns>
        public bool ContainsColumn(string columnName)
        {
            return _dr.Table.Columns.Contains(columnName);
        }
        #endregion
    }
}
