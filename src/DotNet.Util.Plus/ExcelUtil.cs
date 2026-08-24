//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2026, DotNet.
//-----------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
#if NET46_OR_GREATER
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
    /// <remarks>
    /// 纯 NPOI 的导入导出方法（ExcelToDataTable / DataTableToExcel / ExcelToEntityList / EntityListToExcel2003 等）
    /// 已拆分至 ExcelUtil.Npoi.cs，跨 .NET Framework / .NET Core / 5+ 均可用。
    /// 本文件仅保留依赖 System.Web / HttpContext 的 Web 方法，受 #if NET46_OR_GREATER 保护；在 .NET Core / 5+ 下本类仅由 Npoi 部分提供方法。
    /// </remarks>
    public partial class ExcelUtil
    {
#if NET46_OR_GREATER
        #region ExcelToHtml
        /// <summary>
        /// Excel转Html
        /// </summary>
        /// <param name="excelFilePath">Excel文件目录</param>
        /// <param name="excelPreviewFolder">Excel预览文件夹</param>
        public static void ExcelToHtml(string excelFilePath, string excelPreviewFolder = "excel")
        {
            if (!excelFilePath.IsNullOrEmpty())
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
                // 修复：原 Replace(".", ...) 会替换路径中所有点（含目录/版本号/扩展名），且日期分量被整数相加后丢失。
                // 改为仅取文件名（无扩展名）+ 时间戳，生成唯一且合法的 HTML 文件名。
                var timeStamp = DateTime.Now.ToString("yyyyMMddHH");
                var htmlFileName = Path.GetFileNameWithoutExtension(excelFilePath) + timeStamp;
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
                //修复：sheetName 可能含路径分隔符/非法文件名字符（甚至 ..\），生成文件名与工作表名前先消毒
                var safeSheetName = string.IsNullOrEmpty(sheetName) ? "Sheet" : sheetName;
                var invalidFileNameChars = Path.GetInvalidFileNameChars();
                foreach (var c in invalidFileNameChars)
                {
                    safeSheetName = safeSheetName.Replace(c, '_');
                }
                safeSheetName = safeSheetName.Replace("..", "_"); // 防目录穿越
                if (string.IsNullOrEmpty(safeSheetName))
                {
                    safeSheetName = "Sheet";
                }
                var fileName = safeSheetName + "-" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".xls"; // 文件名称
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
                var sheet = workbook.CreateSheet(safeSheetName); // 工作表
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
                //修复：file.FileName 为客户端可控，先用 Path.GetFileName 去除路径（防 ..\ 目录穿越），
                //再基于文件名主干安全插入时间戳；无扩展名时不再抛 ArgumentOutOfRangeException
                var originalFileName = Path.GetFileName(file.FileName);
                if (string.IsNullOrEmpty(originalFileName))
                {
                    return string.Empty;
                }
                var extension = Path.GetExtension(originalFileName);
                var baseName = Path.GetFileNameWithoutExtension(originalFileName);
                var fileName = baseName + "-" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + extension;
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
