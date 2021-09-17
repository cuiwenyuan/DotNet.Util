using System;
using System.CodeDom.Compiler;
using System.Reflection;
using System.Text;
using Microsoft.CSharp;

namespace DotNet.Util
{
    /// <summary> 
    /// 计算表达式的类
    /// </summary>
    public static class CalculateExpression
        {
            /// <summary> 
            /// 接受一个string类型的表达式并计算结果,返回一个object对象,静态方法 
            /// </summary> 
            /// <param name="expression"></param> 
            /// <returns></returns> 
            public static object Calculate(string expression)
            {
                var className = "Calc";
                var methodName = "Run";
                expression = expression.Replace("/", "*1.0/");

                //设置编译参数
                var paras = new CompilerParameters();
                paras.GenerateExecutable = false;
                paras.GenerateInMemory = true;

                //创建动态代码
                var sb = Pool.StringBuilder.Get();
                sb.Append("public class " + className + "\n");
                sb.Append("{\n");
                sb.Append(" public object " + methodName + "()\n");
                sb.Append(" {\n");
                sb.Append(" return " + expression + ";\n");
                sb.Append(" }\n");
                sb.Append("}");

                //编译代码
                var result = new CSharpCodeProvider().CompileAssemblyFromSource(paras, sb.Put());

                //获取编译后的程序集。 
                var assembly = result.CompiledAssembly;

                //动态调用方法。 
                var eval = assembly.CreateInstance(className);
                var method = eval.GetType().GetMethod(methodName);
                var reobj = method.Invoke(eval, null);
                GC.Collect();
                return reobj;

                //object objCalc = Calculate("((1 + 2) * 3 + 6) / 5 ");
                //Console.WriteLine(objCalc.ToString());  //结果为3
                //Console.ReadLine();
                //1、不支持sin、cos等数学函数
                //2、不支持[]、{}等括号和除数字、+、-、*、/以外的字符，建议调用计算函数前进行输入的验证。
            }
    }
}
