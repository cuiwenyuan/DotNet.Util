using System;
using System.Linq.Expressions;

namespace DotNet.Util
{
    /// <summary>
    /// db基础表达式
    /// </summary>
    public class DbBaseExpression : Expression
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="expressionType"></param>
        /// <param name="type"></param>
        protected DbBaseExpression(ExpressionType expressionType, Type type) : base()
        {
            Type = type;
            NodeType = expressionType;
        }
        /// <summary>
        /// 
        /// </summary>
        public override ExpressionType NodeType { get; }
        /// <summary>
        /// 
        /// </summary>
        public string NodeTypeName => ((DbExpressionType)NodeType).ToString();
        /// <summary>
        /// 
        /// </summary>
        public override Type Type { get; }
        
    }
}