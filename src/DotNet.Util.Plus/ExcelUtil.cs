//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2024, DotNet.
//-----------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
#if NET452_OR_GREATER
using System.Web;
#endif
using NPOI.XSSF.UserModel;
using NPOI.HSSF.UserModel;
using NPOI.SS.Converter;
using NPOI.SS.UserModel;

namespace DotNet.Util
{
    /// <summary>
    /// ExcelUtil
    /// 导出Excel格式数据
    /// 
    /// 修改记录
    /// 
    ///     2018.02.28 版本：4.1 Troy Cui       增加Datatable的互转。
    ///     2017.10.31 版本：4.0 Troy Cui       新创建。
    /// 
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2009.07.08</date>
    /// </author> 
    /// </summary>
    public partial class ExcelUtil
    {
#if NET452_OR_GREATER
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
            var fileExt = Path.GetExtension(file)?.ToLower();
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
            var sheet = string.IsNullOrEmpty(dt.TableName) ? workbook.CreateSheet("Sheet1") : workbook.CreateSheet(dt.TableName);

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

        #region ExcelToHtml
        /// <summary>
        /// Excel转Html
        /// </summary>
        /// <param name="excelFilePath">Excel文件目录</param>
        /// <param name="excelPreviewFolder">Excel预览文件夹</param>
        public static void ExcelToHtml(string excelFilePath, string excelPreviewFolder = "excel")
        {
            if (!string.IsNullOrEmpty(excelFilePath))
            {
                IWorkbook wb;
                using (var fs = new FileStream(Utils.GetMapPath(excelFilePath), FileMode.Open, FileAccess.Read))
                {
                    // 只支持2007及以下低版本
                    //wb = new HSSFWorkbook(file);
                    // 通过接口的方式实现从xls到xlsx 2003、2007以上版本的全部支持
                    wb = WorkbookFactory.Create(fs);

                }
                var excelToHtmlConverter = new ExcelToHtmlConverter();

                // 设置输出参数
                excelToHtmlConverter.OutputColumnHeaders = false;
                excelToHtmlConverter.OutputHiddenColumns = false;
                excelToHtmlConverter.OutputHiddenRows = false;
                excelToHtmlConverter.OutputLeadingSpacesAsNonBreaking = false;
                excelToHtmlConverter.OutputRowNumbers = false;
                excelToHtmlConverter.UseDivsToSpan = true;

                // 处理的Excel文件
                excelToHtmlConverter.ProcessWorkbook(wb);

                //检查上传的物理路径是否存在，不存在则创建
                if (!Directory.Exists(Utils.GetMapPath(excelPreviewFolder)))
                {
                    Directory.CreateDirectory(Utils.GetMapPath(excelPreviewFolder));
                }
                var htmlFileName = excelFilePath.Replace(".", DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day +
                                                                 DateTime.Now.Hour + ".");
                var htmlFile = HttpContext.Current.Server.MapPath("/") + excelPreviewFolder + "/" + htmlFileName + ".html";
                //输出的html文件   需创建对应的文件目录  这里是根目录下的doc文件夹
                excelToHtmlConverter.Document.Save(htmlFile);
                if (HttpContext.Current.Request.Url.Port == 80)
                {
                    HttpContext.Current.Response.Redirect("http://" + HttpContext.Current.Request.Url.Host + "/" + excelPreviewFolder + "/" + htmlFileName +
                                                          ".html");
                }
                else
                {
                    HttpContext.Current.Response.Redirect("http://" + HttpContext.Current.Request.Url.Host + ":" +
                                                          HttpContext.Current.Request.Url.Port + "/" + excelPreviewFolder + "/" + htmlFileName +
                                                          ".html");
                }
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
                if (Regex.IsMatch(filePath, ".xls$")) // 2003
                {
                    enlist = Excel2003ToEntityList<T>(cellHeader, filePath, out errorMsg, startIndex);
                }
                else if (Regex.IsMatch(filePath, ".xlsx$")) // 2007
                {
                    enlist = Excel2007ToEntityList<T>(cellHeader, filePath, out errorMsg, startIndex);
                }
                return enlist;
            }
            catch (Exception ex)
            {
                LogUtil.WriteException(ex);
                throw ex;
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
            if (cellHeader is null)
            {
                throw new ArgumentNullException(nameof(cellHeader));
            }

            if (string.IsNullOrEmpty(filePath))
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
                throw ex;
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
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion Excel导入

        #region Excel导出

        /// <summary>
        /// 实体类集合导出到EXCLE2003
        /// </summary>
        /// <param name="cellHeader">单元头的Key和Value：{ { "UserName", "姓名" }, { "Age", "年龄" } };</param>
        /// <param name="iList">数据源</param>
        /// <param name="sheetName">工作表名</param>
        /// <returns>文件的下载地址</returns>
        public static string EntityListToExcel2003(Dictionary<string, string> cellHeader, IList iList, string sheetName)
        {
            try
            {
                var fileName = sheetName + "-" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".xls"; // 文件名称
                var urlPath = "UpFiles/ExcelFiles/" + fileName; // 文件下载的URL地址，供给前台下载
                var filePath = HttpContext.Current.Server.MapPath("\\" + urlPath); // 文件路径

                // 1.检测是否存在文件夹，若不存在就建立个文件夹
                var directoryName = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(directoryName))
                {
                    Directory.CreateDirectory(directoryName);
                }

                // 2.解析单元格头部，设置单元头的中文名称
                var workbook = new HSSFWorkbook(); // 工作簿
                var sheet = workbook.CreateSheet(sheetName); // 工作表
                var row = sheet.CreateRow(0);
                var keys = cellHeader.Keys.ToList();
                for (var i = 0; i < keys.Count; i++)
                {
                    row.CreateCell(i).SetCellValue(cellHeader[keys[i]]); // 列名为Key的值
                }

                // 3.List对象的值赋值到Excel的单元格里
                var rowIndex = 1; // 从第二行开始赋值(第一行已设置为单元头)
                foreach (var e in iList)
                {
                    var rowTmp = sheet.CreateRow(rowIndex);
                    for (var i = 0; i < keys.Count; i++) // 根据指定的属性名称，获取对象指定属性的值
                    {
                        var cellValue = ""; // 单元格的值
                        object properotyValue = null; // 属性的值
                        System.Reflection.PropertyInfo propertyInfo = null; // 属性的信息

                        // 3.1 若属性头的名称包含'.',就表示是子类里的属性，那么就要遍历子类，eg：UserEn.UserName
                        if (keys[i].IndexOf(".", StringComparison.Ordinal) >= 0)
                        {
                            // 3.1.1 解析子类属性(这里只解析1层子类，多层子类未处理)
                            var propertyArray = keys[i].Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries);
                            var subClassName = propertyArray[0]; // '.'前面的为子类的名称
                            var subClassPropertyName = propertyArray[1]; // '.'后面的为子类的属性名称
                            var subClassInfo = e.GetType().GetProperty(subClassName); // 获取子类的类型
                            if (subClassInfo != null)
                            {
                                // 3.1.2 获取子类的实例
                                var subClassEn = e.GetType().GetProperty(subClassName)?.GetValue(e, null);
                                // 3.1.3 根据属性名称获取子类里的属性类型
                                propertyInfo = subClassInfo.PropertyType.GetProperty(subClassPropertyName);
                                if (propertyInfo != null)
                                {
                                    properotyValue = propertyInfo.GetValue(subClassEn, null); // 获取子类属性的值
                                }
                            }
                        }
                        else
                        {
                            // 3.2 若不是子类的属性，直接根据属性名称获取对象对应的属性
                            propertyInfo = e.GetType().GetProperty(keys[i]);
                            if (propertyInfo != null)
                            {
                                properotyValue = propertyInfo.GetValue(e, null);
                            }
                        }

                        // 3.3 属性值经过转换赋值给单元格值
                        if (properotyValue != null)
                        {
                            cellValue = properotyValue.ToString();
                            // 3.3.1 对时间初始值赋值为空
                            if (cellValue.Trim() == "0001/1/1 0:00:00" || cellValue.Trim() == "0001/1/1 23:59:59")
                            {
                                cellValue = "";
                            }
                        }

                        // 3.4 填充到Excel的单元格里
                        rowTmp.CreateCell(i).SetCellValue(cellValue);
                    }
                    rowIndex++;
                }

                // 4.生成文件
                var fs = new FileStream(filePath, FileMode.Create);
                workbook.Write(fs);
                fs.Close();

                // 5.返回下载路径
                return urlPath;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion Excel导出

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
                if (keys[i].IndexOf(".", StringComparison.Ordinal) >= 0)
                {
                    // 1)解析子类属性
                    var propertyArray = keys[i].Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries);
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
            if (sourceCell == null || string.IsNullOrEmpty(sourceCell.ToString()))
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

                case CellType.Unknown:
                    break;

                default:
                    break;
            }

            var valueDataType = distanceType.Name;

            // 在这里进行特定类型的处理
            switch (valueDataType.ToUpper()) // 以防出错，全部大写
            {
                case "STRING":
                    if (sourceValue != null) rs = sourceValue.ToString();
                    break;
                case "INT":
                case "INT16":
                case "INT32":
                    if (sourceValue != null) rs = (int)Convert.ChangeType(sourceCell.NumericCellValue.ToString(), distanceType);
                    break;
                case "FLOAT":
                case "SINGLE":
                    if (sourceValue != null) rs = (float)Convert.ChangeType(sourceCell.NumericCellValue.ToString(), distanceType);
                    break;
                case "DECIMAL":
                    if (sourceValue != null) rs = (decimal)Convert.ChangeType(sourceCell.NumericCellValue.ToString(), distanceType);
                    break;
                case "DATE":
                case "DATETIME":
                    //rs = sourceCell.DateCellValue;
                    if (sourceValue != null) rs = (DateTime)Convert.ChangeType(sourceCell.ToString(), distanceType);
                    break;
                case "GUID":
                    rs = (Guid)Convert.ChangeType(sourceCell.NumericCellValue.ToString(), distanceType);
                    return rs;
            }
            return rs;
        }

        #endregion

        #region 上传Excel文件到服务器

        /// <summary>
        /// 保存Excel文件
        /// <para>Excel的导入导出都会在服务器生成一个文件</para>
        /// <para>路径：UpFiles/ExcelFiles</para>
        /// </summary>
        /// <param name="file">传入的文件对象</param>
        /// <returns>如果保存成功则返回文件的位置;如果保存失败则返回空</returns>
        public static string SaveExcelFile(HttpPostedFile file)
        {
            try
            {
                var fileName = file.FileName.Insert(file.FileName.LastIndexOf('.'), "-" + DateTime.Now.ToString("yyyyMMddHHmmssfff"));
                var filePath = Path.Combine(HttpContext.Current.Server.MapPath("~/UpFiles/ExcelFiles"), fileName);
                var directoryName = Path.GetDirectoryName(filePath);
                if (directoryName != null && !Directory.Exists(directoryName))
                {
                    Directory.CreateDirectory(directoryName);
                }
                file.SaveAs(filePath);
                return filePath;
            }
            catch
            {
                return string.Empty;
            }
        }
        #endregion
#endif
    }
}