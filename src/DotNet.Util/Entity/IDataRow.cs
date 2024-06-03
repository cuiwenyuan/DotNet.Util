//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2024, DotNet.
//-----------------------------------------------------------------

namespace DotNet.Model
{
    /// <summary>
    /// IDataRow接口
    /// </summary>
    public partial interface IDataRow
    {
        /// <summary>
        /// this
        /// </summary>
        /// <param name="name">名称</param>
        /// <returns></returns>
        object this[string name] { get; }
        /// <summary>
        /// this
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        object this[int i] { get; }
        /// <summary>
        /// 判断是否含有指定字段
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns></returns>
        bool ContainsColumn(string columnName);
    }
}
