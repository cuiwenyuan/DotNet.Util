using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace DotNet.Util
{
    /// <summary>
    /// 计算表达式的类
    /// </summary>
    public static class CalculateExpression
    {
        /// <summary>
        /// 接受一个string类型的表达式并计算结果,返回一个object对象,静态方法
        /// 安全实现：使用递归下降解析器计算，不再编译/执行任何 C# 代码，
        /// 因此不存在任意代码执行风险，且兼容所有目标框架（.NET Core/5+ 亦可使用）。
        /// </summary>
        /// <param name="expression">算术表达式，仅支持数字、+ - * / 和括号</param>
        /// <returns>计算结果（整数结果返回 int，否则返回 double）</returns>
        public static object Calculate(string expression)
        {
            // 安全校验：仅允许数字、四则运算符、括号和小数点，防止注入任意字符
            if (string.IsNullOrWhiteSpace(expression) || !Regex.IsMatch(expression, @"^[0-9+\-*/(). ]+$"))
            {
                throw new ArgumentException("表达式包含非法字符。");
            }

            var parser = new ExpressionParser(expression);
            var result = parser.Parse();

            // 整数结果返回 int，保持与原实现的返回类型一致
            if (result >= int.MinValue && result <= int.MaxValue && Math.Truncate(result) == result)
            {
                return (int)result;
            }
            return result;

            //object objCalc = Calculate("((1 + 2) * 3 + 6) / 5 ");
            //Console.WriteLine(objCalc.ToString());  //结果为3
            //Console.ReadLine();
            //1、不支持sin、cos等数学函数
            //2、不支持[]、{}等括号和除数字、+、-、*、/以外的字符，建议调用计算函数前进行输入的验证。
        }
    }

    /// <summary>
    /// 安全的算术表达式解析器（递归下降，不会编译或执行任何代码）
    /// </summary>
    internal sealed class ExpressionParser
    {
        private readonly string _text;
        private int _pos;

        public ExpressionParser(string text)
        {
            _text = text ?? string.Empty;
            _pos = 0;
        }

        public double Parse()
        {
            if (_text.Length == 0)
            {
                throw new ArgumentException("表达式不能为空。");
            }
            var value = ParseAdditive();
            SkipWhitespace();
            if (_pos < _text.Length)
            {
                throw new ArgumentException("表达式语法错误。");
            }
            return value;
        }

        private double ParseAdditive()
        {
            var left = ParseMultiplicative();
            while (true)
            {
                SkipWhitespace();
                if (_pos < _text.Length && _text[_pos] == '+')
                {
                    _pos++;
                    left += ParseMultiplicative();
                }
                else if (_pos < _text.Length && _text[_pos] == '-')
                {
                    _pos++;
                    left -= ParseMultiplicative();
                }
                else
                {
                    return left;
                }
            }
        }

        private double ParseMultiplicative()
        {
            var left = ParseUnary();
            while (true)
            {
                SkipWhitespace();
                if (_pos < _text.Length && _text[_pos] == '*')
                {
                    _pos++;
                    left *= ParseUnary();
                }
                else if (_pos < _text.Length && _text[_pos] == '/')
                {
                    _pos++;
                    var divisor = ParseUnary();
                    if (divisor == 0)
                    {
                        throw new DivideByZeroException("除数不能为零。");
                    }
                    //浮点除法，与原实现用 *1.0/ 的效果一致
                    left /= divisor;
                }
                else
                {
                    return left;
                }
            }
        }

        private double ParseUnary()
        {
            SkipWhitespace();
            if (_pos < _text.Length && (_text[_pos] == '-' || _text[_pos] == '+'))
            {
                var sign = _text[_pos] == '-' ? -1 : 1;
                _pos++;
                return sign * ParseUnary();
            }
            if (_pos < _text.Length && _text[_pos] == '(')
            {
                _pos++;
                var value = ParseAdditive();
                SkipWhitespace();
                if (_pos >= _text.Length || _text[_pos] != ')')
                {
                    throw new ArgumentException("括号不匹配。");
                }
                _pos++;
                return value;
            }
            return ParseNumber();
        }

        private double ParseNumber()
        {
            SkipWhitespace();
            var start = _pos;
            while (_pos < _text.Length && (char.IsDigit(_text[_pos]) || _text[_pos] == '.'))
            {
                _pos++;
            }
            if (start == _pos)
            {
                throw new ArgumentException("表达式语法错误。");
            }
            var token = _text.Substring(start, _pos - start);
            if (!double.TryParse(token, NumberStyles.Float, CultureInfo.InvariantCulture, out var value))
            {
                throw new ArgumentException("表达式包含非法数字。");
            }
            return value;
        }

        private void SkipWhitespace()
        {
            while (_pos < _text.Length && _text[_pos] == ' ')
            {
                _pos++;
            }
        }
    }
}
