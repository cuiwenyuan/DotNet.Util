//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2026, DotNet.
//-----------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
#if NETSTANDARD2_0_OR_GREATER
using Microsoft.AspNetCore.Http;
#endif
namespace DotNet.Util
{
    /// <summary>
    /// BaseExportCSV
    /// 导出CSV格式数据
    ///
    /// 修改记录
    ///
    ///     2021.12.31 版本：5.0 Troy.Cui	ToDataTable方法增加fieldList和fieldListOnly用于读取控制
    ///     2021.09.21 版本：4.0 Troy.Cui	增加ToDataTable方法，并增加fieldList字典控制csv输出
    ///     2009.07.08 版本：3.0 JiRiGaLa	更新完善程序，将方法修改为静态方法。
    ///     2007.08.11 版本：2.0 JiRiGaLa	更新完善程序。
    ///     2006.12.01 版本：1.0 JiRiGaLa	新创建。
    ///
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2009.07.08</date>
    /// </author>
    /// </summary>
    public partial class CsvUtil
    {
        #region ExportCsv IDataReader导出CSV格式文件
        /// <summary>
        /// IDataReader导出CSV格式文件
        /// </summary>
        /// <param name="dataReader">IDataReader</param>
        /// <param name="fileName">文件全路径</param>
        /// <param name="fieldList">字段列表字典(字段,描述)</param>
        /// <param name="encoding">编码类型</param>
        /// <param name="separator">分隔符</param>
        public static void ExportCsv(IDataReader dataReader, string fileName, Dictionary<string, string> fieldList = null, Encoding encoding = null, string separator = ",")
        {
            if (File.Exists(fileName))
            {
                FileUtil.DeleteFile(fileName);
            }
            using (var fs = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                var sw = new StreamWriter(fs, encoding ?? Encoding.UTF8);
                sw.WriteLine(GetCsvFormatData(dataReader, fieldList: fieldList, separator: separator).Return());
                sw.Close();
                fs.Close();
                sw.TryDispose();
                fs.TryDispose();
            }
        }
        #endregion

        #region GetCsvFormatData 通过dataReader获得CSV格式数据
        /// <summary>
        /// 通过dataReader获得CSV格式数据
        /// </summary>
        /// <param name="dataReader"></param>
        /// <param name="fieldList">字段列表字典(字段,描述)</param>
        /// <param name="separator">分隔符</param>
        /// <returns></returns>
        private static StringBuilder GetCsvFormatData(IDataReader dataReader, Dictionary<string, string> fieldList = null, string separator = ",")
        {
            //TODO:fieldList的处理
            // 返回总字符串
            var csvRows = PoolUtil.StringBuilder.Get();
            // 表头内容字符串
            var sb = PoolUtil.StringBuilder.Get();
            // 循环输出表头内容
            for (var index = 0; index < dataReader.FieldCount; index++)
            {
                //如果表头名字不为空，获取内容
                if (dataReader.GetName(index) != null)
                {
                    sb.Append(dataReader.GetName(index));
                }
                //在获取表头内容之后加上,
                if (index < dataReader.FieldCount - 1)
                {
                    sb.Append(separator);
                }
            }
            // 先把表头正行数据加载到StringBuilder对象csvRows中
            csvRows.AppendLine(sb.Return());
            // 循环获取表中的所有内容
            while (dataReader.Read())
            {
                sb = PoolUtil.StringBuilder.Get();
                for (var index = 0; index < dataReader.FieldCount; index++)
                {
                    // 除第一列外，其余列前面都要加上分隔符，避免空值导致列错位
                    if (index > 0)
                    {
                        sb.Append(separator);
                    }
                    if (!dataReader.IsDBNull(index))
                    {
                        var value = dataReader.GetValue(index).ToString();
                        if (dataReader.GetFieldType(index) == typeof(string))
                        {
                            WriteSpecialCharacter(value, sb, separator);
                        }
                        else
                        {
                            sb.Append(value);
                        }
                    }
                }
                csvRows.AppendLine(sb.Return());
            }
            dataReader.Close();
            return csvRows;
        }
        #endregion

        #region GetCsvFormatData 通过DataTable获得CSV格式数据
        /// <summary>
        /// 通过DataTable获得CSV格式数据
        /// </summary>
        /// <param name="dt">数据表</param>
        /// <param name="fieldList">字段列表字典(字段,描述)</param>
        /// <param name="separator">分隔符</param>
        /// <returns>CSV字符串数据</returns>
        private static StringBuilder GetCsvFormatData(DataTable dt, Dictionary<string, string> fieldList = null, string separator = ",")
        {
            var sb = PoolUtil.StringBuilder.Get();

            #region 检查字段列表

            if (dt != null && fieldList != null)
            {
                var keys = fieldList.Keys.ToArray();
                for (var i = 0; i < keys.Length; i++)
                {
                    var hasColumnName = false;
                    foreach (DataColumn dc in dt.Columns)
                    {
                        if (dc.ColumnName.Equals(keys[i], StringComparison.OrdinalIgnoreCase))
                        {
                            hasColumnName = true;
                            break;
                        }
                    }
                    if (!hasColumnName)
                    {
                        //表内不存在此字段，就不要输出这个列了
                        fieldList.Remove(keys[i]);
                    }
                }
            }

            #endregion

            #region 生成dt新表

            if (dt != null && fieldList != null && fieldList.Count > 0)
            {
                //对DataTable筛选指定字段，并保存为新表
                //这些列名，确保DataTable中存在，否则会报错误
                var dtNew = dt.DefaultView.ToTable(false, fieldList.Keys.ToArray());
                dt = new DataTable();
                dt = dtNew.Copy();
            }

            #endregion

            #region 写出表头

            if (dt != null)
            {
                foreach (DataColumn dc in dt.Columns)
                {
                    if (sb.Length > 0 && (fieldList == null || (fieldList != null && fieldList.ContainsKey(dc.ColumnName))))
                    {
                        sb.Append(separator);
                    }
                    if (fieldList == null || fieldList?.Count == 0)
                    {
                        WriteSpecialCharacter(dc.ColumnName, sb, separator);
                    }
                    else
                    {
                        if (fieldList.ContainsKey(dc.ColumnName))
                        {
                            WriteSpecialCharacter(dc.ColumnName, sb, separator);
                        }
                    }
                }
                if (dt.Rows.Count > 0)
                {
                    sb.Append("\n");
                }
            }
            #endregion

            #region 写出数据
            if (dt != null && dt.Rows.Count > 0)
            {
                var c = 1;
                foreach (DataRowView drv in dt.DefaultView)
                {
                    var i = 1;
                    //var j = 1;
                    try
                    {
                        foreach (DataColumn dc in dt.Columns)
                        {
                            //j++;
                            if (fieldList == null || fieldList?.Count == 0)
                            {

                                WriteSpecialCharacter(drv[dc.ColumnName]?.ToString(), sb, separator);
                                if (i < dt.Columns.Count)
                                {
                                    sb.Append(separator);
                                }
                                i++;
                                //LogUtil.WriteLog(j + "," + i + "," + dt.Columns.Count + "," + fieldList?.Count + "," + dc.ColumnName + ":" + drv[dc.ColumnName]?.ToString());
                            }
                            else
                            {
                                if (fieldList.ContainsKey(dc.ColumnName))
                                {
                                    WriteSpecialCharacter(drv[dc.ColumnName]?.ToString(), sb, separator);
                                    if (i < fieldList.Count)
                                    {
                                        sb.Append(separator);
                                    }
                                    i++;
                                }
                                //LogUtil.WriteLog(j + "," + i + "," + dt.Columns.Count + "," + fieldList?.Count + ":" + drv[dc.ColumnName]?.ToString());
                            }

                        }
                        //最后一行不需要输出换行符
                        if (c < dt.Rows.Count)
                        {
                            sb.Append("\n");
                        }

                    }
                    catch (Exception ex)
                    {
                        LogUtil.WriteException(ex);
                        continue;
                    }
                    finally
                    {
                        c++;
                    }

                }
            }

            #endregion

            return sb;
        }
        #endregion

        #region WriteSpecialCharacter 写入CSV特殊字符
        /// <summary>
        /// 写入CSV特殊字符
        /// </summary>
        /// <param name="content"></param>
        /// <param name="sb"></param>
        /// <param name="separator"></param>
        private static void WriteSpecialCharacter(string content, StringBuilder sb, string separator)
        {
            if (!content.IsNullOrEmpty())
            {
                if (content.Contains("\""))
                {
                    sb.AppendFormat("\"{0}\"", content.Replace("\"", "\"\""));
                }
                else if (content.Contains(separator) || content.Contains("\r") || content.Contains("\n"))
                {
                    sb.AppendFormat("\"{0}\"", content);
                }
                else
                {
                    sb.Append(content);
                }
            }
            else
            {
                sb.Append("");
            }
        }
        #endregion

        #region GetCsvFormatData 通过DataSet获得CSV格式数据
        /// <summary>
        /// 通过DataSet获得CSV格式数据
        /// </summary>
        /// <param name="dataSet">数据权限</param>
        /// <param name="fieldList">字段列表字典(字段,描述)</param>
        /// <param name="separator">分隔符</param>
        /// <returns>CSV字符串数据</returns>
        private static StringBuilder GetCsvFormatData(DataSet dataSet, Dictionary<string, string> fieldList = null, string separator = ",")
        {
            var sb = PoolUtil.StringBuilder.Get();
            foreach (DataTable dt in dataSet.Tables)
            {
                sb.Append(GetCsvFormatData(dt, fieldList: fieldList, separator: separator).Return());
            }
            return sb;
        }
        #endregion

        #region ExportCsv DataTable导出CSV格式文件
        /// <summary>
        /// DataTable导出CSV格式文件
        /// </summary>
        /// <param name="dt">数据表</param>
        /// <param name="fileName">文件名</param>
        /// <param name="fieldList">字段列表字典(字段,描述)</param>
        /// <param name="encoding">编码类型</param>
        /// <param name="separator">分隔符</param>
        public static void ExportCsv(DataTable dt, string fileName, Dictionary<string, string> fieldList = null, Encoding encoding = null, string separator = ",")
        {
            //修复：使用 using 确保 StreamWriter 在异常路径也释放
            using (var sw = new StreamWriter(fileName, false, encoding ?? Encoding.UTF8))
            {
                sw.WriteLine(GetCsvFormatData(dt, fieldList: fieldList, separator: separator).Return());
                sw.Flush();
            }
        }
        #endregion

        #region ExportCsv DataSet导出CSV格式文件
        /// <summary>
        /// DataSet导出CSV格式文件
        /// </summary>
        /// <param name="dataSet">数据权限</param>
        /// <param name="fileName">文件名</param>
        /// <param name="fieldList">字段列表字典(字段,描述)</param>
        /// <param name="encoding">编码类型</param>
        /// <param name="separator">分隔符</param>
        public static void ExportCsv(DataSet dataSet, string fileName, Dictionary<string, string> fieldList = null, Encoding encoding = null, string separator = ",")
        {
            //修复：使用 using 确保 StreamWriter 在异常路径也释放
            using (var sw = new StreamWriter(fileName, false, encoding ?? Encoding.UTF8))
            {
                sw.WriteLine(GetCsvFormatData(dataSet, fieldList: fieldList, separator: separator).ToString());
                sw.Flush();
            }
        }
        #endregion

#if NET46_OR_GREATER

        #region GetResponseCsv 在浏览器中获得CSV格式文件
        /// <summary>
        /// 在浏览器中获得CSV格式文件
        /// </summary>
        /// <param name="dt">数据表</param>
        /// <param name="fileName">输出文件名</param>
        /// <param name="fieldList">字段列表字典(字段,描述)</param>
        /// <param name="separator">分隔符</param>
        public static void GetResponseCsv(DataTable dt, string fileName, Dictionary<string, string> fieldList = null, string separator = ",")
        {
            HttpContext.Current.Response.ClearHeaders();
            HttpContext.Current.Response.ContentEncoding = Encoding.GetEncoding("utf-8");
            HttpContext.Current.Response.AppendHeader("Content-disposition", "attachment;filename=" + fileName);
            HttpContext.Current.Response.ContentType = "application/ms-excel";
            HttpContext.Current.Response.Write(GetCsvFormatData(dt, fieldList: fieldList, separator: separator).Return());
            HttpContext.Current.Response.End();
        }
        #endregion

        #region GetResponseCsv 在浏览器中获得CSV格式文件
        /// <summary>
        /// 在浏览器中获得CSV格式文件
        /// </summary>
        /// <param name="dataSet">数据权限</param>
        /// <param name="fileName">输出文件名</param>
        /// <param name="fieldList">字段列表字典(字段,描述)</param>
        public static void GetResponseCsv(DataSet dataSet, string fileName, Dictionary<string, string> fieldList = null)
        {
            HttpContext.Current.Response.ClearHeaders();
            HttpContext.Current.Response.ContentEncoding = Encoding.GetEncoding("utf-8");
            HttpContext.Current.Response.AppendHeader("Content-disposition", "attachment;filename=" + fileName);
            HttpContext.Current.Response.ContentType = "application/ms-excel";
            HttpContext.Current.Response.Write(GetCsvFormatData(dataSet).ToString());
            HttpContext.Current.Response.End();
            //读取文件下载
            //String OutTemplateCSV = Server.MapPath("~/DownLoadFiles/ExcelExport/Common/Log/LogGeneral.csv");
            //var sw = new StreamWriter(OutTemplateCSV, false, System.Text.Encoding.GetEncoding("gb2312"));
            //sw.WriteLine(GetCSVFormatData(dataSet).ToString());
            //sw.Flush();
            //sw.Close();
            //Response.Redirect("../../../DownLoadFiles/ExcelExport/Common/Log/LogGeneral.csv");
        }
        #endregion
#endif

        #region ToDataTable 转为DataTable
        /// <summary>
        /// 读取CSV文件内容并转为DataTable
        /// </summary>
        /// <param name="fileName">完整路径文件名</param>
        /// <param name="separator">分隔符，默认为标准的英文,</param>
        /// <param name="firstLineIsHeader">第一行是否为表头，默认为否</param>
        /// <param name="encoding">编码类型</param>
        /// <param name="fieldList">字段列表字典(csv字段(无表头用C1,C2,C3,..格式),DataTable字段)</param>
        /// <param name="fieldListOnly">仅按照字段列表字典导入</param>
        /// <returns>DataTable自定义列名或以C1-CN开头的列名</returns>
        public static DataTable ToDataTable(string fileName, string separator = ",", bool firstLineIsHeader = false, Encoding encoding = null, Dictionary<string, string> fieldList = null, bool fieldListOnly = false)
        {
            var dt = new DataTable();
            //修复：使用 using 确保异常路径也释放文件句柄（原 Close 在方法尾、无 try/finally 保护）
            using var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            using var sr = new StreamReader(fs, encoding ?? EncodingUtil.Detect(fs));
            //记录每次读取的一行记录
            var line = "";
            //记录每行记录中的各字段内容
            string[] arr;
            //标示列数
            var headColumnCount = 0;
            var lineColumnCount = 0;
            //字段是否已经添加
            var isColumnAdded = false;

            //字段和列的对应关系，仅从指定列导入时会用到
            var dicFieldIndex = new Dictionary<string, int>();
            //逐行读取CSV中的数据
            while ((line = sr.ReadLine()) != null)
            {
                var spr = separator.ToCharArray();
                // 修复：原先先 line.Split 拆分、再靠「是否以引号结尾」的启发式还原带引号字段，
                // 该启发式不可靠——字段内容以转义双引号 "" 结尾时（如 "He said ""hi"""）会被误判为未闭合，
                // 导致该字段被清空且后续列错位。改为按 RFC 4180 逐字符状态机一次性正确拆分。
                arr = SplitCsvLine(line, spr).ToArray();

                if (firstLineIsHeader)
                {
                    firstLineIsHeader = false;
                    isColumnAdded = true;
                    headColumnCount = GetLength(arr, separator);
                    #region 生成DataTable数据列
                    //根据指定列名创建
                    if (fieldList != null && fieldListOnly)
                    {
                        foreach (var field in fieldList)
                        {
                            var dc = new DataColumn(field.Value);
                            dt.Columns.Add(dc);
                            //映射CSV的字段列索引
                            for (var i = 0; i < arr.Length; i++)
                            {
                                var columnIndex = i;
                                if (ConvertColumnName(ReadSpecialCharacter(arr, ref i, separator), fieldList: fieldList).Equals(field.Value, StringComparison.OrdinalIgnoreCase)) dicFieldIndex.Add(field.Value, columnIndex);
                            }
                        }
                    }
                    else
                    {
                        //根据第一行实际列数，进行匹配映射来创建
                        for (var i = 0; i < arr.Length; i++)
                        {
                            var dc = new DataColumn(ConvertColumnName(ReadSpecialCharacter(arr, ref i, separator), fieldList: fieldList));
                            dt.Columns.Add(dc);
                        }
                    }
                    #endregion
                }
                else
                {
                    #region 生成DataTable数据列
                    if (!isColumnAdded)
                    {
                        isColumnAdded = true;
                        headColumnCount = GetLength(arr, separator);
                        //根据指定列名创建
                        if (fieldList != null && fieldListOnly)
                        {
                            var fieldListIndex = 0;
                            foreach (var field in fieldList)
                            {
                                var dc = new DataColumn(field.Value);
                                dt.Columns.Add(dc);
                                //按照顺序映射列索引
                                dicFieldIndex.Add(field.Value, fieldListIndex);
                                fieldListIndex++;
                            }
                        }
                        else
                        {
                            //根据第一行实际列数，进行匹配映射来创建
                            for (var i = 0; i < headColumnCount; i++)
                            {
                                var dc = new DataColumn(ConvertColumnName("C" + (i + 1), fieldList: fieldList));
                                dt.Columns.Add(dc);
                            }
                        }
                    }
                    #endregion

                    lineColumnCount = GetLength(arr, separator);
                    //
                    //生成指定列或自动创建列
                    #region 写数据行
                    if (fieldList != null && fieldListOnly)
                    {
                        if (lineColumnCount > 0)
                        {
                            var dr = dt.NewRow();
                            foreach (var d in dicFieldIndex)
                            {
                                for (var j = 0; j < arr.Length; j++)
                                {
                                    // 修复：必须始终取值以推进 j（合并字段时 j 会跳跃），否则后续索引错位
                                    var columnIndex = j;
                                    var cellValue = ReadSpecialCharacter(arr, ref j, separator);
                                    if (columnIndex == d.Value)
                                    {
                                        dr[d.Key] = cellValue;
                                    }
                                }
                            }
                            dt.Rows.Add(dr);
                        }
                    }
                    else
                    {
                        //此行的列数要跟表头的列数一致才认为有效
                        if (lineColumnCount == headColumnCount)
                        {
                            var dr = dt.NewRow();
                            // 修复：j 是 arr(拆分后)索引，合并字段时会跳跃，列索引必须独立计数
                            var columnIndex = 0;
                            for (var j = 0; j < arr.Length && columnIndex < lineColumnCount; j++)
                            {
                                dr[columnIndex++] = ReadSpecialCharacter(arr, ref j, separator);
                            }
                            dt.Rows.Add(dr);
                        }
                        else
                        {
                            LogUtil.WriteLog("headColumnCount:" + headColumnCount + ",lineColumnCount:" + lineColumnCount + "line:" + line, "CsvUtil.InvalidLine");
                        }
                    }
                    #endregion
                }
            }

            return dt;
        }
        #endregion

        #region ReadSpecialCharacter 读取CSV特殊字符
        /// <summary>
        /// 读取CSV特殊字符
        /// </summary>
        /// <param name="arr"></param>
        /// <param name="i"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        private static string ReadSpecialCharacter(string[] arr, ref int i, string separator)
        {
            // 修复：arr 已由 SplitCsvLine 按 RFC 4180 正确拆分，引号包裹与 "" 转义均已处理完毕，
            // 此处不再需要合并片段（原合并启发式的缺陷见 SplitCsvLine 注释）。
            // 保留 ref int i 仅为了兼容既有调用点签名，不再修改其值。
            return i >= 0 && i < arr.Length ? arr[i] : string.Empty;
        }
        #endregion

        #region GetLength 获取长度
        /// <summary>
        /// 获取长度
        /// </summary>
        /// <param name="arr"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        private static int GetLength(string[] arr, string separator)
        {
            // 修复：arr 已由 SplitCsvLine 正确拆分，字段数即列数，无需再扣减合并项。
            // （原实现在此靠启发式扣减列数，必须与 ReadSpecialCharacter 的合并逻辑严格一致，
            //   两者任一出错都会导致列数与实际字段数不符 -> 整行被静默丢弃，仅写日志。）
            return arr == null ? 0 : arr.Length;
        }

        #region SplitCsvLine 按 RFC 4180 拆分单行 CSV
        /// <summary>
        /// 按 RFC 4180 规则将一行 CSV 正确拆分为字段列表。
        /// 支持：引号包裹字段、字段内含分隔符、两个连续双引号 "" 表示一个字面量双引号的转义。
        /// </summary>
        /// <param name="line">单行内容（不含换行符）</param>
        /// <param name="separators">分隔符字符集合</param>
        /// <returns>字段列表；引号已去除、"" 已还原为单个引号</returns>
        private static List<string> SplitCsvLine(string line, char[] separators)
        {
            var result = new List<string>();
            if (line == null)
            {
                return result;
            }
            if (line.Length == 0)
            {
                result.Add(string.Empty);
                return result;
            }

            var sb = PoolUtil.StringBuilder.Get();
            var i = 0;
            while (i <= line.Length)
            {
                // 跳过后导/前导空白（分隔符本身不算空白）
                while (i < line.Length && char.IsWhiteSpace(line[i]) && !IsSeparator(line[i], separators))
                {
                    i++;
                }

                string value;
                if (i < line.Length && line[i] == '"')
                {
                    // 引号包裹字段：内容原样保留（含首尾空格），"" 还原为单个引号
                    i++;
                    sb.Clear();
                    while (i < line.Length)
                    {
                        var c = line[i];
                        if (c == '"')
                        {
                            // 连续两个引号是转义，表示一个字面量引号
                            if (i + 1 < line.Length && line[i + 1] == '"')
                            {
                                sb.Append('"');
                                i += 2;
                                continue;
                            }
                            // 单个引号 = 字段结束
                            i++;
                            break;
                        }
                        sb.Append(c);
                        i++;
                    }
                    value = sb.ToString();
                    // 跳过闭合引号之后、分隔符之前的空白
                    while (i < line.Length && !IsSeparator(line[i], separators) && char.IsWhiteSpace(line[i]))
                    {
                        i++;
                    }
                }
                else
                {
                    // 未加引号字段：取到下一个分隔符，并去除首尾空白
                    var start = i;
                    while (i < line.Length && !IsSeparator(line[i], separators))
                    {
                        i++;
                    }
                    value = start >= i ? string.Empty : line.Substring(start, i - start).Trim();
                }

                result.Add(value);

                if (i < line.Length && IsSeparator(line[i], separators))
                {
                    i++;
                    // 行尾分隔符 -> 末尾还有一个空字段
                    if (i == line.Length)
                    {
                        result.Add(string.Empty);
                        break;
                    }
                }
                else
                {
                    break;
                }
            }

            sb.Return(false);
            return result;
        }

        /// <summary>
        /// 判断字符是否为分隔符
        /// </summary>
        /// <param name="c">待判断字符</param>
        /// <param name="separators">分隔符字符集合</param>
        /// <returns>是分隔符返回 true，否则返回 false</returns>
        private static bool IsSeparator(char c, char[] separators)
        {
            if (separators == null)
            {
                return false;
            }
            for (var k = 0; k < separators.Length; k++)
            {
                if (separators[k] == c)
                {
                    return true;
                }
            }
            return false;
        }
        #endregion
        #endregion

        #region 列名转换
        /// <summary>
        /// ConvertColumnName
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="fieldList">字段列表字典(csv字段(无表头用C1,C2,C3,..格式),DataTable字段)</param>
        /// <returns></returns>
        private static string ConvertColumnName(string columnName, Dictionary<string, string> fieldList = null)
        {
            if (fieldList != null)
            {
                if (fieldList.ContainsKey(columnName))
                {
                    columnName = fieldList[columnName];
                }
            }
            return columnName;
        }
        #endregion
    }
}
