//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System;
using System.Data;
using System.IO;
using System.Web;
using NPOI.XSSF.UserModel;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System.Collections.Generic;

namespace DotNet.Util
{
    /// <summary>
    /// ExcelUtil
    /// 导出Excel格式数据
    /// 
    /// 修改记录
    ///
    ///     2020.03.08 版本：5.0 Troy Cui   标准化名称后缀为Util
    ///     2014.02.21 版本：4.0 Troy Cui   NPOI升级后标准化
    ///     2014.02.21 版本：4.0 老崔       增加按照指定的字段顺序导出
    ///     2012.04.02 版本：3.0 Pcsky      增加判断文件是否打开的方法
    ///     2012.04.02 版本：3.0 Pcsky      增加新的导出Excel方法，非Com+方式，改用.Net控件
    ///     2009.07.08 版本：2.0 JiRiGaLa	更新完善程序，将方法修改为静态方法。
    ///     2006.12.02 版本：1.0 JiRiGaLa	新创建。
    /// 
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2020.03.08</date>
    /// </author> 
    /// </summary>
    public partial class ExcelUtil
    {
#if NET40_OR_GREATER
        #region private void DeleteExistFile(string fileName) 删除已经存在的文件
        /// <summary>
        /// 删除已经存在的文件
        /// </summary>
        /// <param name="fileName">文件路径</param>
        private void DeleteExistFile(string fileName)
        {
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }
        }
        #endregion

        #region public static void ExportXlsByNPOI(System.Data.DataTable dt, Dictionary<string,string> fieldList, string fileName) 导出Excel格式文件(NPOI组件方式)
        /// <summary>
        /// 导出Excel格式文件(NPOI组件方式)
        /// </summary>
        /// <param name="dt">数据表</param>
        /// <param name="fieldList">数据表字段名-说明对应列表</param>
        /// <param name="fileName">文件名</param>
        public static void ExportXlsByNpoi(DataTable dt, Dictionary<string, string> fieldList, string fileName)
        {
            var fs = new FileStream(fileName, FileMode.Create, FileAccess.Write);
            var ms = new MemoryStream();
            //创建工作簿
            var workbook = new HSSFWorkbook();
            //创建表
            var sheet = workbook.CreateSheet();
            //创建表头
            var headerRow = sheet.CreateRow(0);

            // 写出表头
            if (fieldList == null)
            {
                for (var i = 0; i < dt.Columns.Count; i++)
                {
                    //增加了try Catch，解决字典fieldList中没有table列中项时，会出错。
                    //此处采用跳过的方式,表现方式是此列的表头没值
                    try
                    {
                        headerRow.CreateCell(i).SetCellValue(dt.Columns[i].ColumnName);
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }
            }
            else
            {
                var j = 0;
                foreach (var field in fieldList)
                {
                    try
                    {
                        headerRow.CreateCell(j).SetCellValue(field.Value);
                        j++;
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }
            }
            // 行索引号
            var rowIndex = 1;

            // 写出数据
            foreach (DataRow dataTableRow in dt.Rows)
            {
                var dataRow = sheet.CreateRow(rowIndex);
                if (fieldList == null)
                {
                    for (var i = 0; i < dt.Columns.Count; i++)
                    {
                        //if (dataGridView.Columns[i].Visible && (dataGridView.Columns[i].Name.ToUpper() != "colSelected".ToUpper()))
                        //{
                        switch (dt.Columns[i].DataType.ToString())
                        {
                            case "System.String":
                            default:
                                dataRow.CreateCell(i).SetCellValue(
                                    Convert.ToString(Convert.IsDBNull(dataTableRow[i]) ? "" : dataTableRow[i])
                                    );
                                break;
                            case "System.DateTime":
                                dataRow.CreateCell(i).SetCellValue(
                                    Convert.ToString(Convert.IsDBNull(dataTableRow[i]) ? "" : DateTime.Parse(dataTableRow[i].ToString()).ToString(BaseSystemInfo.DateTimeFormat)));
                                break;
                            case "System.Int16":
                            case "System.Int32":
                            case "System.Int64":
                            case "System.Decimal":
                            case "System.Double":
                                dataRow.CreateCell(i).SetCellValue(
                                    Convert.ToDouble(Convert.IsDBNull(dataTableRow[i]) ? 0 : dataTableRow[i])
                                    );
                                break;
                        }
                        //                    }
                    }
                }
                else
                {
                    var j = 0;
                    foreach (var field in fieldList)
                    //for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        //if (dataGridView.Columns[i].Visible && (dataGridView.Columns[i].Name.ToUpper() != "colSelected".ToUpper()))
                        //{
                        try
                        {
                            switch (dt.Columns[field.Key].DataType.ToString())
                            {
                                case "System.String":
                                default:
                                    dataRow.CreateCell(j).SetCellValue(
                                        Convert.ToString(Convert.IsDBNull(dataTableRow[field.Key]) ? "" : dataTableRow[field.Key])
                                        );
                                    break;
                                case "System.DateTime":
                                    dataRow.CreateCell(j).SetCellValue(
                                        Convert.ToString(Convert.IsDBNull(dataTableRow[field.Key]) ? "" : DateTime.Parse(dataTableRow[field.Key].ToString()).ToString(BaseSystemInfo.DateTimeFormat)));
                                    break;
                                case "System.Int16":
                                case "System.Int32":
                                case "System.Int64":
                                case "System.Decimal":

                                case "System.Double":
                                    dataRow.CreateCell(j).SetCellValue(
                                        Convert.ToDouble(Convert.IsDBNull(dataTableRow[field.Key]) ? 0 : dataTableRow[field.Key])
                                        );
                                    break;
                            }
                            //                    }
                            j++;
                        }
                        catch (Exception)
                        {
                            continue;
                        }
                    }
                }

                rowIndex++;
            }

            workbook.Write(ms);
            var data = ms.ToArray();

            ms.Flush();
            ms.Close();

            fs.Write(data, 0, data.Length);
            fs.Flush();
            fs.Close();
        }
        #endregion

        #region public static void ExportXlsxByNPOI(System.Data.DataTable dt, Dictionary<string,string> fieldList, string fileName) 导出高版本Excel格式文件(NPOI组件方式)

        /// <summary>
        /// 导出高版本Excel格式文件(NPOI组件方式)
        /// </summary>
        /// <param name="dt">数据表</param>
        /// <param name="fieldList">数据表字段名-说明对应列表</param>
        /// <param name="fileName">文件名</param>
        /// <param name="exportPicture">是否导出图片</param>
        public static void ExportXlsxByNpoi(DataTable dt, Dictionary<string, string> fieldList, string fileName, bool exportPicture = false)
        {
            var fs = new FileStream(fileName, FileMode.Create, FileAccess.Write);
            var ms = new MemoryStream();
            //创建工作簿
            var workbook = new XSSFWorkbook();
            //创建表
            var sheet = workbook.CreateSheet("Sheet1");
            //创建表头
            var headerRow = sheet.CreateRow(0);

            // 写出表头
            if (fieldList == null)
            {
                for (var i = 0; i < dt.Columns.Count; i++)
                {
                    //增加了try Catch，解决字典fieldList中没有table列中项时，会出错。
                    //此处采用跳过的方式,表现方式是此列的表头没值
                    try
                    {
                        headerRow.CreateCell(i).SetCellValue(dt.Columns[i].ColumnName);
                    }
                    catch (Exception ex)
                    {
                        LogUtil.WriteException(ex);
                        continue;
                    }
                }
            }
            else
            {
                var j = 0;
                foreach (var field in fieldList)
                {
                    try
                    {
                        headerRow.CreateCell(j).SetCellValue(field.Value);
                        j++;
                    }
                    catch (Exception ex)
                    {
                        LogUtil.WriteException(ex);
                        continue;
                    }
                }
            }
            // 行索引号
            var rowIndex = 1;
            var hasPicture = false;
            // 写出数据
            foreach (DataRow dtRow in dt.Rows)
            {

                var dataRow = sheet.CreateRow(rowIndex);
                if (fieldList == null)
                {
                    for (var i = 0; i < dt.Columns.Count; i++)
                    {
                        try
                        {
                            switch (dt.Columns[i].DataType.ToString())
                            {
                                case "System.String":
                                default:
                                    //获取后缀名
                                    var filePath = Convert.ToString(Convert.IsDBNull(dtRow[i]) ? "" : dtRow[i]);
                                    var suffix = filePath.Substring(
                                        filePath.LastIndexOf('.') + 1,
                                        filePath.Length - filePath.LastIndexOf('.') - 1
                                    );
                                    hasPicture = false;
                                    if (exportPicture &&
                                        (suffix.Equals("jpg", StringComparison.OrdinalIgnoreCase) || suffix.Equals("bmp", StringComparison.OrdinalIgnoreCase) || suffix.Equals("jpeg", StringComparison.OrdinalIgnoreCase) || suffix.Equals("gif", StringComparison.OrdinalIgnoreCase) ||
                                         suffix.Equals("png", StringComparison.OrdinalIgnoreCase)))
                                    {
                                        hasPicture = true;
                                        //正方形的例子50*20 x 10*256
                                        dataRow.Height = 100 * 20;
                                        sheet.SetColumnWidth(i, 20 * 256);
                                        AddPicture(workbook, sheet, filePath, rowIndex, i, suffix);

                                    }
                                    else
                                    {
                                        dataRow.CreateCell(i).SetCellValue(
                                            Convert.ToString(Convert.IsDBNull(dtRow[i]) ? "" : dtRow[i])
                                        );
                                    }
                                    break;
                                case "System.DateTime":
                                    dataRow.CreateCell(i).SetCellValue(
                                        Convert.ToString(Convert.IsDBNull(dtRow[i])
                                            ? ""
                                            : DateTime.Parse(dtRow[i].ToString())
                                                .ToString(BaseSystemInfo.DateTimeFormat)));
                                    break;
                                case "System.Int16":
                                case "System.Int32":
                                case "System.Int64":
                                case "System.Decimal":
                                case "System.Double":
                                    dataRow.CreateCell(i).SetCellValue(
                                        Convert.ToDouble(Convert.IsDBNull(dtRow[i]) ? 0 : dtRow[i])
                                    );
                                    break;
                            }
                            //列宽自适应会造成导出图片出问题
                            if (exportPicture && hasPicture && sheet.GetColumnWidth(i) != 20 * 256)
                            {
                                sheet.AutoSizeColumn(i);
                            }
                        }
                        catch (Exception ex)
                        {
                            LogUtil.WriteException(ex);
                            continue;
                        }
                    }
                }
                else
                {
                    var i = 0;
                    foreach (var field in fieldList)
                    //for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        try
                        {
                            switch (dt.Columns[field.Key].DataType.ToString())
                            {
                                case "System.String":
                                default:
                                    //获取后缀名
                                    var filePath = Convert.ToString(Convert.IsDBNull(dtRow[field.Key]) ? "" : dtRow[field.Key]);
                                    var suffix = filePath.Substring(
                                    filePath.LastIndexOf('.') + 1,
                                    filePath.Length - filePath.LastIndexOf('.') - 1
                                    );
                                    hasPicture = false;
                                    if (exportPicture && (suffix.Equals("jpg", StringComparison.OrdinalIgnoreCase) || suffix.Equals("bmp", StringComparison.OrdinalIgnoreCase) || suffix.Equals("jpeg", StringComparison.OrdinalIgnoreCase) || suffix.Equals("gif", StringComparison.OrdinalIgnoreCase) || suffix.Equals("png", StringComparison.OrdinalIgnoreCase)))
                                    {
                                        hasPicture = true;
                                        //正方形的例子50*20 x 10*256
                                        dataRow.Height = 100 * 20;
                                        sheet.SetColumnWidth(i, 20 * 256);
                                        AddPicture(workbook, sheet, filePath, rowIndex, i, suffix);
                                        //sheet.GetColumnWidth(i);

                                    }
                                    else
                                    {
                                        dataRow.CreateCell(i).SetCellValue(
                                            Convert.ToString(Convert.IsDBNull(dtRow[field.Key]) ? "" : dtRow[field.Key])
                                            );
                                    }
                                    break;
                                case "System.DateTime":
                                    dataRow.CreateCell(i).SetCellValue(
                                        Convert.ToString(Convert.IsDBNull(dtRow[field.Key]) ? "" : DateTime.Parse(dtRow[field.Key].ToString()).ToString(BaseSystemInfo.DateTimeFormat)));
                                    break;
                                case "System.Int16":
                                case "System.Int32":
                                case "System.Int64":
                                case "System.Decimal":

                                case "System.Double":
                                    dataRow.CreateCell(i).SetCellValue(
                                        Convert.ToDouble(Convert.IsDBNull(dtRow[field.Key]) ? 0 : dtRow[field.Key])
                                        );
                                    break;
                            }
                            //列宽自适应会造成导出图片出问题
                            if (exportPicture && hasPicture && sheet.GetColumnWidth(i) != 20 * 256)
                            {
                                sheet.AutoSizeColumn(i);
                            }
                            i++;
                        }
                        catch (Exception ex)
                        {
                            LogUtil.WriteException(ex);
                            continue;
                        }
                    }
                }

                rowIndex++;
            }

            workbook.Write(ms);
            var data = ms.ToArray();

            ms.Flush();
            ms.Close();

            fs.Write(data, 0, data.Length);
            fs.Flush();
            fs.Close();
        }
        #endregion

        #region 向sheet插入图片

        /// <summary>
        /// 向sheet插入图片
        /// </summary>
        /// <param name="workbook">工作簿</param>
        /// <param name="sheet">工作表</param>
        /// <param name="filePath">文件路径</param>
        /// <param name="row">第几行</param>
        /// <param name="col">第几列</param>
        /// <param name="suffix">后缀</param>
        private static void AddPicture(XSSFWorkbook workbook, ISheet sheet, string filePath, int row, int col, string suffix)
        {
            try
            {
                if (!string.IsNullOrEmpty(filePath))
                {
                    //转为本地路径
                    filePath = HttpContext.Current.Server.MapPath(filePath);
                    if (File.Exists(filePath))
                    {
                        var bytes = File.ReadAllBytes(filePath);
                        if (bytes.Length > 0)
                        {
                            var imagedId = 0;
                            var pictureType = new PictureType();
                            if (suffix.Equals("png", StringComparison.OrdinalIgnoreCase))
                            {
                                pictureType = PictureType.PNG;
                            }
                            else if (suffix.Equals("jpg", StringComparison.OrdinalIgnoreCase))
                            {
                                pictureType = PictureType.JPEG;
                            }
                            else
                            {
                                pictureType = PictureType.JPEG;
                            }
                            var patriarch = sheet.CreateDrawingPatriarch();
                            imagedId = workbook.AddPicture(bytes, pictureType);
                            //处理照片位置，【图片左上角为（col, row）第row+1行col+1列，右下角为（ col +1, row +1）第 col +1+1行row +1+1列，第三个参数为宽，第四个参数为高
                            var anchor = new XSSFClientAnchor(0, 0, 0, 0, col, row, col + 1, row + 1);
                            //dx1:图片左边相对excel格的位置(x偏移) 范围值为:0~1023;即输100 偏移的位置大概是相对于整个单元格的宽度的100除以1023大概是10分之一
                            //dy1:图片上方相对excel格的位置(y偏移) 范围值为:0~256 原理同上。
                            //dx2:图片右边相对excel格的位置(x偏移) 范围值为:0~1023; 原理同上。
                            //dy2:图片下方相对excel格的位置(y偏移) 范围值为:0~256 原理同上。
                            anchor.AnchorType = AnchorType.MoveDontResize;
                            //anchor.AnchorType = 3;
                            //load the picture and get the picture index in the workbook
                            var picture = (XSSFPicture)patriarch.CreatePicture(anchor, imagedId);
                            //Reset the image to the original size.
                            //picture.Resize();   //Note: Resize will reset client anchor you set. 这句话一定不要，这是用图片原始大小来显示
                            //picture.Resize(0.5);
                            picture.LineStyle = LineStyle.DashDotGel;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                LogUtil.WriteException(ex);
                //此处采用跳过的方式
                //throw ex;
            }
        }
        #endregion

#endif
    }
}