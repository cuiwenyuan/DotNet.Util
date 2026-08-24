//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2026, DotNet.
//-----------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using NPOI.XSSF.UserModel;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;

namespace DotNet.Util
{
    /// <summary>
    /// ExcelUtil 的纯 NPOI 部分（跨 .NET Framework / .NET Core / 5+ 可用）。
    /// 与 ExcelUtil.cs（仅含依赖 System.Web/HttpContext 的 Web 方法，受 #if NET46_OR_GREATER 保护）共同组成 partial class ExcelUtil。
    /// 拆分目的：原本整个 ExcelUtil 类体被 #if NET46_OR_GREATER 包裹，导致 .NET Core / 5+ 下编译为空类、无法使用 NPOI 导入导出能力。
    /// </summary>
    public partial class ExcelUtil
    {
        // 跨框架可用的分隔符，避免每次调用 new string[] 触发 CA1861
        private static readonly string[] DotSeparator = { "." };

        #region ExcelToTable
        /// <summary>
        /// Excel导入成Datable
        /// </summary>
        /// <param name="file">导入路径(包含文件名与扩展名)</param>
        /// <returns></returns>
        public static DataTable ExcelToDataTable(string file)
        {
            var dt = new DataTable();
            IWorkbook workbook;
            var fileExt = Path.GetExtension(file);
            using (var fs = new FileStream(file, FileMode.Open, FileAccess.Read))
            {
                //XSSFWorkbook 适用XLSX格式，HSSFWorkbook 适用XLS格式
                if (fileExt.Equals(".xlsx", StringComparison.OrdinalIgnoreCase)) { workbook = new XSSFWorkbook(fs); } else if (fileExt.Equals(".xls", StringComparison.OrdinalIgnoreCase)) { workbook = new HSSFWorkbook(fs); } else { workbook = null; }
                if (workbook == null) { return null; }
                var sheet = workbook.GetSheetAt(0);

                //表头  
                var header = sheet.GetRow(sheet.FirstRowNum);
                var columns = new List<int>();
                for (var i = 0; i < header.LastCellNum; i++)
                {
                    var obj = GetValueType(header.GetCell(i));
                    if (obj == null || obj.ToString() == string.Empty)
                    {
                        dt.Columns.Add(new DataColumn("Columns" + i));
                    }
                    else
                    {
                        dt.Columns.Add(new DataColumn(obj.ToString()));
                    }

                    columns.Add(i);
                }
                //数据  
                for (var i = sheet.FirstRowNum + 1; i <= sheet.LastRowNum; i++)
                {
                    var dr = dt.NewRow();
                    var hasValue = false;
                    foreach (var j in columns)
                    {
                        dr[j] = GetValueType(sheet.GetRow(i).GetCell(j));
                        if (dr[j] != null && dr[j].ToString() != string.Empty)
                        {
                            hasValue = true;
                        }
                    }
                    if (hasValue)
                    {
                        dt.Rows.Add(dr);
                    }
                }
            }
            return dt;
        }
        #endregion

        #region TableToExcel
        /// <summary>
        /// Datable导出成Excel
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="file">导出路径(包括文件名与扩展名)</param>
        public static void DataTableToExcel(DataTable dt, string file)
        {
            IWorkbook workbook;
            var fileExt = Path.GetExtension(file)?.ToLower(CultureInfo.InvariantCulture);
            switch (fileExt)
            {
                case ".xlsx":
                    workbook = new XSSFWorkbook();
                    break;
                case ".xls":
                    workbook = new HSSFWorkbook();
                    break;
                default:
                    workbook = null;
                    break;
            }
            if (workbook == null) { return; }
            var sheet = (dt.TableName).IsNullOrEmpty() ? workbook.CreateSheet("Sheet1") : workbook.CreateSheet(dt.TableName);

            //表头  
            var row = sheet.CreateRow(0);
            for (var i = 0; i < dt.Columns.Count; i++)
            {
                var cell = row.CreateCell(i);
                cell.SetCellValue(dt.Columns[i].ColumnName);
            }

            //数据  
            for (var i = 0; i < dt.Rows.Count; i++)
            {
                var row1 = sheet.CreateRow(i + 1);
                for (var j = 0; j < dt.Columns.Count; j++)
                {
                    var cell = row1.CreateCell(j);
                    cell.SetCellValue(dt.Rows[i][j].ToString());
                }
            }

            //转为字节数组  
            var stream = new MemoryStream();
            workbook.Write(stream);
            var buf = stream.ToArray();

            //保存为Excel文件  
            using (var fs = new FileStream(file, FileMode.Create, FileAccess.Write))
            {
                fs.Write(buf, 0, buf.Length);
                fs.Flush();
            }
        }
        #endregion

        #region GetValueType
        /// <summary>
        /// 获取单元格类型
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        private static object GetValueType(ICell cell)
        {
            if (cell == null)
                return null;
            switch (cell.CellType)
            {
                case CellType.Blank: //BLANK:  
                    return null;
                case CellType.Boolean: //BOOLEAN:  
                    return cell.BooleanCellValue;
                case CellType.Numeric: //NUMERIC:  
                    return cell.NumericCellValue;
                case CellType.String: //STRING:  
                    return cell.StringCellValue;
                case CellType.Error: //ERROR:  
                    return cell.ErrorCellValue;
                case CellType.Formula: //FORMULA:  
                default:
                    return "=" + cell.CellFormula;
            }
        }
        #endregion

        #region Excel导入

        /// <summary>
        /// 从Excel取数据并记录到List集合里
        /// </summary>
        /// <param name="cellHeader">单元头的值和名称：{ { "UserName", "姓名" }, { "Age", "年龄" } };</param>
        /// <param name="filePath">保存文件绝对路径</param>
        /// <param name="errorMsg">错误信息</param>
        /// <param name="startIndex">数据行开始序列，默认为1（即第二列，从0开始）</param>
        /// <returns>转换后的List对象集合</returns>
        public static List<T> ExcelToEntityList<T>(Dictionary<string, string> cellHeader, string filePath, out StringBuilder errorMsg, int startIndex = 1) where T : new()
        {
            var enlist = new List<T>();
            errorMsg = PoolUtil.StringBuilder.Get();
            try
            {
                if (filePath.EndsWith(".xls", StringComparison.OrdinalIgnoreCase)) // 2003
                {
                    enlist = Excel2003ToEntityList<T>(cellHeader, filePath, out errorMsg, startIndex);
                }
                else if (filePath.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase)) // 2007
                {
                    enlist = Excel2007ToEntityList<T>(cellHeader, filePath, out errorMsg, startIndex);
                }
                return enlist;
            }
            catch (Exception ex)
            {
                LogUtil.WriteException(ex);
                throw;
            }
        }

        /// <summary>
        /// 从Excel2003取数据并记录到List集合里
        /// </summary>
        /// <param name="cellHeader">单元头的Key和Value：{ { "UserName", "姓名" }, { "Age", "年龄" } };</param>
        /// <param name="filePath">保存文件绝对路径</param>
        /// <param name="errorMsg">错误信息</param>
        /// <param name="startIndex">开始行索引（默认1）</param>
        /// <returns>转换好的List对象集合</returns>
        private static List<T> Excel2003ToEntityList<T>(Dictionary<string, string> cellHeader, string filePath, out StringBuilder errorMsg, int startIndex = 1) where T : new()
        {
#pragma warning disable CA1510 // 跨框架兼容：ArgumentNullException.ThrowIfNull 在 .NET Framework 不存在
            if (cellHeader is null)
            {
                throw new ArgumentNullException(nameof(cellHeader));
            }
#pragma warning restore CA1510

            if (filePath.IsNullOrEmpty())
            {
                throw new ArgumentException($"'{nameof(filePath)}' cannot be null or empty.", nameof(filePath));
            }

            errorMsg = PoolUtil.StringBuilder.Get(); // 错误信息,Excel转换到实体对象时，会有格式的错误信息
            var ls = new List<T>(); // 转换后的集合
            try
            {
                using (var fs = File.OpenRead(filePath))
                {
                    var workbook = new HSSFWorkbook(fs);
                    var sheet = (HSSFSheet)workbook.GetSheetAt(0); // 获取此文件第一个Sheet页
                    for (var rowIndex = startIndex; rowIndex <= sheet.LastRowNum; rowIndex++)
                    {
                        // 1.判断当前行是否空行，若空行就不在进行读取下一行操作，结束Excel读取操作
                        var row = sheet.GetRow(rowIndex);
                        if (row == null)
                        {
                            break;
                        }
                        // 2.每一个Excel row转换为一个实体对象
                        var e = new T();
                        ExcelRowToEntity<T>(cellHeader, row, rowIndex, e, ref errorMsg);
                        ls.Add(e);
                    }
                }
                return ls;
            }
            catch (Exception ex)
            {
                LogUtil.WriteException(ex);
                throw;
            }
        }

        /// <summary>
        /// 从Excel2007取数据并记录到List集合里
        /// </summary>
        /// <param name="cellHeader">单元头的Key和Value：{ { "UserName", "姓名" }, { "Age", "年龄" } };</param>
        /// <param name="filePath">保存文件绝对路径</param>
        /// <param name="errorMsg">错误信息</param>
        /// <param name="startIndex">数据行开始序列，默认为1（即第二列，从0开始）</param>
        /// <returns>转换好的List对象集合</returns>
        private static List<T> Excel2007ToEntityList<T>(Dictionary<string, string> cellHeader, string filePath, out StringBuilder errorMsg, int startIndex = 1) where T : new()
        {
            errorMsg = PoolUtil.StringBuilder.Get(); // 错误信息,Excel转换到实体对象时，会有格式的错误信息
            var ls = new List<T>(); // 转换后的集合
            try
            {
                using (var fs = File.OpenRead(filePath))
                {
                    var workbook = new XSSFWorkbook(fs);
                    var sheet = (XSSFSheet)workbook.GetSheetAt(0); // 获取此文件第一个Sheet页
                    for (var rowIndex = startIndex; rowIndex <= sheet.LastRowNum; rowIndex++)
                    {
                        // 1.判断当前行是否空行，若空行就不在进行读取下一行操作，结束Excel读取操作
                        var row = sheet.GetRow(rowIndex);
                        if (row == null)
                        {
                            break;
                        }
                        // 2.每一个Excel row转换为一个实体对象
                        var en = new T();
                        ExcelRowToEntity<T>(cellHeader, row, rowIndex, en, ref errorMsg);
                        ls.Add(en);
                    }
                }
                return ls;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion Excel导入

        #region Excel 导入导出Common

        /// <summary>
        /// Excel row转换为实体对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cellHeader">单元头的Key和Value：{ { "UserName", "姓名" }, { "Age", "年龄" } };</param>
        /// <param name="row">Excel row</param>
        /// <param name="rowIndex">row index</param>
        /// <param name="t">实体</param>
        /// <param name="errorMsg">错误信息</param>
        private static void ExcelRowToEntity<T>(Dictionary<string, string> cellHeader, IRow row, int rowIndex, T t, ref StringBuilder errorMsg)
        {
            var keys = cellHeader.Keys.ToList(); // 要赋值的实体对象属性名称
            var errStr = ""; // 当前行转换时，是否有错误信息，格式为：第1行数据转换异常：XXX列；
            for (var i = 0; i < keys.Count; i++)
            {
                // 1.若属性头的名称包含'.',就表示是子类里的属性，那么就要遍历子类，eg：UserEn.TrueName
#pragma warning disable CA2249 // .NET Framework 无 string.Contains(char) 重载，故用 IndexOf(char) 保证跨框架兼容
                if (keys[i].IndexOf('.') >= 0)
#pragma warning restore CA2249
                {
                    // 1)解析子类属性
                    var propertyArray = keys[i].Split(DotSeparator, StringSplitOptions.RemoveEmptyEntries);
                    var subClassName = propertyArray[0]; // '.'前面的为子类的名称
                    var subClassPropertyName = propertyArray[1]; // '.'后面的为子类的属性名称
                    var subClassInfo = t.GetType().GetProperty(subClassName); // 获取子类的类型
                    if (subClassInfo != null)
                    {
                        // 2)获取子类的实例
                        var subClassEn = t.GetType().GetProperty(subClassName)?.GetValue(t, null);
                        // 3)根据属性名称获取子类里的属性信息
                        var propertyInfo = subClassInfo.PropertyType.GetProperty(subClassPropertyName);
                        if (propertyInfo != null)
                        {
                            try
                            {
                                // Excel单元格的值转换为对象属性的值，若类型不对，记录出错信息
                                propertyInfo.SetValue(subClassEn, GetExcelCellToProperty(propertyInfo.PropertyType, row.GetCell(i)), null);
                            }
                            catch (Exception ex)
                            {
                                LogUtil.WriteException(ex);
                                if (errStr.Length == 0)
                                {
                                    errStr = "第" + rowIndex + "行数据转换异常：";
                                }
                                errStr += cellHeader[keys[i]] + "列；";
                            }

                        }
                    }
                }
                else
                {
                    // 2.给指定的属性赋值
                    var propertyInfo = t.GetType().GetProperty(keys[i]);
                    if (propertyInfo != null)
                    {
                        try
                        {
                            // Excel单元格的值转换为对象属性的值，若类型不对，记录出错信息
                            propertyInfo.SetValue(t, GetExcelCellToProperty(propertyInfo.PropertyType, row.GetCell(i)), null);
                        }
                        catch (Exception ex)
                        {
                            LogUtil.WriteException(ex);
                            if (errStr.Length == 0)
                            {
                                errStr = "第" + rowIndex + "行数据转换异常：";
                            }
                            errStr += cellHeader[keys[i]] + "列；";
                        }
                    }
                }
            }
            // 若有错误信息，就添加到错误信息里
            if (errStr.Length > 0)
            {
                errorMsg.AppendLine(errStr);
            }
        }

        /// <summary>
        /// Excel Cell转换为实体的属性值
        /// </summary>
        /// <param name="distanceType">目标对象类型</param>
        /// <param name="sourceCell">对象属性的值</param>
        private static Object GetExcelCellToProperty(Type distanceType, ICell sourceCell)
        {
            var rs = distanceType.IsValueType ? Activator.CreateInstance(distanceType) : null;

            // 1.判断传递的单元格是否为空
            if (sourceCell == null || (sourceCell.ToString()).IsNullOrEmpty())
            {
                return rs;
            }

            // 2.Excel文本和数字单元格转换，在Excel里文本和数字是不能进行转换，所以这里预先存值
            object sourceValue = null;
            switch (sourceCell.CellType)
            {
                case CellType.Blank:
                    break;

                case CellType.Boolean:
                    break;

                case CellType.Error:
                    break;

                case CellType.Formula:
                    break;

                case CellType.Numeric:
                    sourceValue = sourceCell.NumericCellValue;
                    break;

                case CellType.String:
                    sourceValue = sourceCell.StringCellValue;
                    break;

                default:
                    break;
            }

            var valueDataType = distanceType.Name;

            // 在这里进行特定类型的处理
            switch (valueDataType.ToUpper(CultureInfo.InvariantCulture)) // 以防出错，全部大写（固定区域性）
            {
                case "STRING":
                    if (sourceValue != null) rs = sourceValue.ToString();
                    break;
                case "INT":
                case "INT16":
                case "INT32":
                    if (sourceValue != null) rs = (int)Convert.ChangeType(sourceCell.NumericCellValue.ToString(CultureInfo.InvariantCulture), distanceType, CultureInfo.InvariantCulture);
                    break;
                case "FLOAT":
                case "SINGLE":
                    if (sourceValue != null) rs = (float)Convert.ChangeType(sourceCell.NumericCellValue.ToString(CultureInfo.InvariantCulture), distanceType, CultureInfo.InvariantCulture);
                    break;
                case "DECIMAL":
                    if (sourceValue != null) rs = (decimal)Convert.ChangeType(sourceCell.NumericCellValue.ToString(CultureInfo.InvariantCulture), distanceType, CultureInfo.InvariantCulture);
                    break;
                case "DATE":
                case "DATETIME":
                    //rs = sourceCell.DateCellValue;
                    if (sourceValue != null) rs = (DateTime)Convert.ChangeType(sourceCell.ToString(), distanceType, CultureInfo.InvariantCulture);
                    break;
                case "GUID":
                    rs = (Guid)Convert.ChangeType(sourceCell.NumericCellValue.ToString(CultureInfo.InvariantCulture), distanceType, CultureInfo.InvariantCulture);
                    return rs;
            }
            return rs;
        }

        #endregion
    }
}
