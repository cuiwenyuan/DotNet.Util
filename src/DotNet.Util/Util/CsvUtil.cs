//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2023, DotNet.
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
                sw.WriteLine(GetCsvFormatData(dataReader, fieldList: fieldList, separator: separator).Put());
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
            var csvRows = Pool.StringBuilder.Get();
            // 表头内容字符串
            var sb = Pool.StringBuilder.Get();
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
            csvRows.AppendLine(sb.Put());
            // 循环获取表中的所有内容
            while (dataReader.Read())
            {
                sb = Pool.StringBuilder.Get();
                for (var index = 0; index < dataReader.FieldCount - 1; index++)
                {
                    if (sb.Length > 0)
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
                // 最后一个逗号用空来替代
                if (!dataReader.IsDBNull(dataReader.FieldCount - 1))
                {
                    sb.Append(dataReader.GetValue(dataReader.FieldCount - 1).ToString().Replace(separator, ""));
                }
                csvRows.AppendLine(sb.Put());
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
            var sb = Pool.StringBuilder.Get();

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
                                    i++;
                                    WriteSpecialCharacter(drv[dc.ColumnName]?.ToString(), sb, separator);
                                    if (i < fieldList.Count)
                                    {
                                        sb.Append(separator);
                                    }
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
            if (!string.IsNullOrEmpty(content))
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
            var sb = Pool.StringBuilder.Get();
            foreach (DataTable dt in dataSet.Tables)
            {
                sb.Append(GetCsvFormatData(dt, fieldList: fieldList, separator: separator).Put());
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
            var sw = new StreamWriter(fileName, false, encoding ?? Encoding.UTF8);
            sw.WriteLine(GetCsvFormatData(dt, fieldList: fieldList, separator: separator).Put());
            sw.Flush();
            sw.Close();
            sw.TryDispose();
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
            var sw = new StreamWriter(fileName, false, encoding ?? Encoding.UTF8);
            sw.WriteLine(GetCsvFormatData(dataSet, fieldList: fieldList, separator: separator).ToString());
            sw.Flush();
            sw.Close();
        }
        #endregion

#if NET452_OR_GREATER

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
            HttpContext.Current.Response.Write(GetCsvFormatData(dt, fieldList: fieldList, separator: separator).Put());
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
            var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            var sr = new StreamReader(fs, encoding ?? EncodingUtil.Detect(fs));
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
                arr = line.Split(spr);

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
                            for (var i = 0; i < headColumnCount; i++)
                            {
                                if (ConvertColumnName(ReadSpecialCharacter(arr, i, separator), fieldList: fieldList).Equals(field.Value, StringComparison.OrdinalIgnoreCase)) dicFieldIndex.Add(field.Value, i);
                            }
                        }
                    }
                    else
                    {
                        //根据第一行实际列数，进行匹配映射来创建
                        for (var i = 0; i < headColumnCount; i++)
                        {
                            var dc = new DataColumn(ConvertColumnName(ReadSpecialCharacter(arr, i, separator), fieldList: fieldList));
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
                                for (var j = 0; j < lineColumnCount; j++)
                                {
                                    if (j == d.Value)
                                    {
                                        dr[d.Key] = ReadSpecialCharacter(arr, j, separator);
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
                            for (var j = 0; j < lineColumnCount; j++)
                            {
                                dr[j] = ReadSpecialCharacter(arr, j, separator);
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

            sr.Close();
            fs.Close();
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
        private static string ReadSpecialCharacter(string[] arr, int i, string separator)
        {
            var str = (arr[i] + "").Trim();
            if (str.StartsWith("\""))
            {
                var txt = "";
                if (str.EndsWith("\"") && !str.EndsWith("\"\""))
                {
                    txt = str.Trim('\"');
                }
                else
                {
                    // 找到下一个以引号结尾的项
                    for (var j = i + 1; j < arr.Length; j++)
                    {
                        if (arr[j].EndsWith("\""))
                        {
                            txt = arr.Skip(i).Take(j - i + 1).Join(separator + "").Trim('\"');
                            // 跳过去一大步
                            i = j;
                            break;
                        }
                    }
                }

                // 两个引号是一个引号的转义
                txt = txt.Replace("\"\"", "\"");
                str = txt;
            }
            return str;
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
            var result = arr.Length;
            for (var i = 0; i < arr.Length; i++)
            {
                var str = (arr[i] + "").Trim();
                if (str.StartsWith("\""))
                {
                    //var txt = "";
                    if (str.EndsWith("\"") && !str.EndsWith("\"\""))
                    {
                        //txt = str.Trim('\"');
                    }
                    else
                    {
                        // 找到下一个以引号结尾的项
                        for (var j = i + 1; j < arr.Length; j++)
                        {
                            if (arr[j].EndsWith("\""))
                            {
                                //txt = arr.Skip(i).Take(j - i + 1).Join(separator + "").Trim('\"');
                                // 跳过去一大步
                                i = j;
                                result -= (j - i);
                                break;
                            }
                        }
                    }
                }
            }
            return result;
        }
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