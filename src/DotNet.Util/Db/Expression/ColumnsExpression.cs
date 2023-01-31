using System.Collections.Generic;
using System.Linq.Expressions;

namespace DotNet.Util
{
    /// <summary>
    /// 多个列表达式
    /// </summary>
    public class ColumnsExpression : Expression
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="columnExpressions"></param>
        public ColumnsExpression(List<ColumnExpression> columnExpressions)
        {
            ColumnExpressions = columnExpressions;
        }
        /// <summary>
        /// 
        /// </summary>
        public List<ColumnExpression> ColumnExpressions { get; set; }
    }
}