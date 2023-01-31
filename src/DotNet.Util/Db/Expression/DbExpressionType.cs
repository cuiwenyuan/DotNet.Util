namespace DotNet.Util
{
    /// <summary>
    /// 
    /// </summary>
    public enum DbExpressionType
    {
        /// <summary>
        /// 
        /// </summary>
        Query = 1000,
        /// <summary>
        /// 
        /// </summary>
        Select,
        /// <summary>
        /// 
        /// </summary>
        Column,
        /// <summary>
        /// 
        /// </summary>
        Table,
        /// <summary>
        /// 
        /// </summary>
        Join,
        /// <summary>
        /// 
        /// </summary>
        Where,
        /// <summary>
        /// 
        /// </summary>
        WhereCondition,
        /// <summary>
        /// 
        /// </summary>
        WhereTrueCondition,
        /// <summary>
        /// 
        /// </summary>
        FunctionWhereCondition,
        /// <summary>
        /// 
        /// </summary>
        OrderBy,
        /// <summary>
        /// 
        /// </summary>
        GroupBy
    }
}