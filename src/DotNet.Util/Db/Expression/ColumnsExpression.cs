﻿using System.Collections.Generic;
using System.Linq.Expressions;

namespace DotNet.Util
{
    /// <summary>
    /// 多个列表达式
    /// </summary>
    public class ColumnsExpression : Expression
    {
        public ColumnsExpression(List<ColumnExpression> columnExpressions)
        {
            ColumnExpressions = columnExpressions;
        }
        public List<ColumnExpression> ColumnExpressions { get; set; }
    }
}