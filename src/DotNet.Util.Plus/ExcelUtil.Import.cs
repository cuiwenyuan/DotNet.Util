//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2026, DotNet.
//-----------------------------------------------------------------

using System;
using System.Data;
using System.IO;
using System.Text;
#if NET46_OR_GREATER
using System.Windows.Forms;
#endif
using NPOI.SS.UserModel;

namespace DotNet.Util
{
    public partial class ExcelUtil
    {
#if NET46_OR_GREATER
        private int _returnStatus = 0;
        private string _returnMessage = null;

        /// <summary>
        /// 执行返回状态
        /// </summary>
        public int ReturnStatus => _returnStatus;

        /// <summary>
        /// 执行返回信息
        /// </summary>
        public string ReturnMessage => _returnMessage;

        /// <summary>
        /// 选择要导入的Excel文件
        /// </summary>
        /// <returns></returns>
        public static string SelectExcelFile()
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Excel文件(*.XLS)|*.XLS";

            openFileDialog.Multiselect = false;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                var filePath = openFileDialog.FileNames[0];
                return filePath;
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// 选择要导入的Excel文件(多版本)
        /// </summary>
        /// <returns></returns>
        public static string OpenXlsXlsxFile()
        {
            var filePath = string.Empty;

            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Excel 工作簿(*.xls,*.xlsx)|*.xls;*.xlsx|Excel 97-2003 工作簿(*.xls)|*.xls|Excel 2010 工作簿(*.xlsx)|*.xlsx|所有文件|*.*";
            // openFileDialog.Filter = "Excel 97-2003 工作簿(*.xls)|*.xls|Excel2010文件(*.xlsx)|*.xlsx|所有文件|*.*";
            openFileDialog.FilterIndex = 1;
            openFileDialog.RestoreDirectory = true;
            openFileDialog.Title = "选择要导入的EXCEL文件";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                filePath = openFileDialog.FileNames[0];
            }
            return filePath;
        }

        /// <summary>
        /// 选择要导入的文本文件
        /// </summary>
        /// <returns></returns>
        public static string SelectTxtFile()
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "文本文件(*.txt)|*.txt";

            openFileDialog.Multiselect = false;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                var filePath = openFileDialog.FileNames[0];
                return filePath;
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// 读取Excel
        /// 默认第一行为标头
        /// 支持Office 2007以上版本
        /// 替换原先的方式，不存在非托管方式无法释放资源的问题
        /// 适用于B/S C/S。服务器可免安装Office。
        /// Pcsky 2012.05.01
        /// </summary>
        /// <param name="path">excel文档路径</param>
        /// <param name="sheetIndex"></param>
        /// <returns></returns>
        public static DataTable ImportExcel(string path, int sheetIndex = 0 )
        {
            string columnName;
            var dt = new DataTable();
            //HSSFWorkbook wb;
            IWorkbook wb;
            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                //只支持2007及以下低版本
                //wb = new HSSFWorkbook(file);
                //通过接口的方式实现从xls到xlsx 2003、2007以上版本的全部支持
                wb = WorkbookFactory.Create(fs);

            }
            var sheet = wb.GetSheetAt(sheetIndex);
            //System.Collections.IEnumerator rows = sheet.GetRowEnumerator();
            var headerRow = sheet.GetRow(0);
            //修复：空表时第一行可能为 null
            if (headerRow == null)
            {
                return dt;
            }
            var cellCount = headerRow.LastCellNum;

            // 添加datatable的标题行
            //for (var i = 0; i < cellCount; i++)
            for (var i = headerRow.FirstCellNum; i < cellCount; i++)
            {
                //ICell cell = headerRow.GetCell(j);
                //dt.Columns.Add(cell.ToString());

                // 2012.09.13 Pcsky 处理空列
                if (headerRow.GetCell(i) == null)
                {
                    columnName = Guid.NewGuid().ToString("N");
                }
                else
                {
                    //修复：表头可能是数字/公式单元格，StringCellValue 会抛异常，统一取值
                    columnName = GetCellStringValue(headerRow.GetCell(i));
                }
                var column = new DataColumn(columnName);
                dt.Columns.Add(column);
            }

            //从第2行起添加内容行
            for (var i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++)
            {
                var row = sheet.GetRow(i);
                //修复：跳过空白行，避免 NullReferenceException
                if (row == null)
                {
                    continue;
                }
                var dr = dt.NewRow();

                // 2012.09.12 Pcsky 设置dataRow的索引号从0开始
                var k = 0;
                for (var j = row.FirstCellNum; j < cellCount; j++)
                {
                    //修复：原代码把 ICell 对象直接赋给了 string 列，应读取单元格的值
                    if (k < dt.Columns.Count)
                    {
                        dr[k] = GetCellStringValue(row.GetCell(j));
                    }
                    k++;
                }

                dt.Rows.Add(dr);
            }
            wb = null;
            sheet = null;
            return dt;
        }

        /// <summary>
        /// 读取单元格的字符串值（兼容空单元格、数字、日期、布尔、公式）
        /// </summary>
        /// <param name="cell">单元格</param>
        /// <returns>字符串值</returns>
        private static string GetCellStringValue(ICell cell)
        {
            if (cell == null)
            {
                return string.Empty;
            }
            try
            {
                switch (cell.CellType)
                {
                    case CellType.String:
                        return cell.StringCellValue ?? string.Empty;
                    case CellType.Numeric:
                        try
                        {
                            //日期格式的数字单元格：DateCellValue 可正常读取，纯数字会抛异常从而回退到数字取值
                            return cell.DateCellValue.ToString();
                        }
                        catch
                        {
                            return cell.NumericCellValue.ToString();
                        }
                    case CellType.Boolean:
                        return cell.BooleanCellValue.ToString();
                    case CellType.Formula:
                        return cell.CellFormula ?? string.Empty;
                    default:
                        return cell.ToString() ?? string.Empty;
                }
            }
            catch
            {
                return cell.ToString() ?? string.Empty;
            }
        }

        #region public static string CheckColumnExist(string columnNames, string needCheckColumnName) 判断是否存在这一列
        /// <summary>
        /// 判断是否存在这一列
        /// </summary>
        /// <param name="columnNames">当前存在的列组</param>
        /// <param name="needCheckColumnName">要求的列名组</param>
        /// <returns>提示信息</returns>
        public static string CheckColumnExist(string columnNames, string needCheckColumnName)
        {
            var result = string.Empty;
            if (!needCheckColumnName.Contains(columnNames))
            {
                result += "\"" + columnNames + "\"这一列不存在，需添加此列。\r\n";
            }
            return result;
        }
        #endregion

        #region public static StringBuilder CheckIsNullOrEmpty(DataTable dt, string checkStrings) 判断是选中段的值否为空
        /// <summary>
        /// 判断是选中段的值否为空
        /// </summary>
        /// <param name="dt">DataTable</param>
        /// <param name="checkStrings">检查的字段串</param>
        /// <returns>返回提示</returns>
        public static string CheckIsNullOrEmpty(DataTable dt, string[] checkStrings)
        {
            var result = PoolUtil.StringBuilder.Get();
            for (var j = 0; j < dt.Rows.Count; j++)
            {
                var rowErrors = new StringBuilder();
                for (var i = 0; i < checkStrings.Length; i++)
                {
                    if ((dt.Rows[j][checkStrings[i]].ToString()).IsNullOrEmpty())
                    {
                        rowErrors.Append("\"" + checkStrings[i] + "\"不能为空。");
                    }
                }
                if (rowErrors.Length > 0)
                {
                    //修复：确保“错误信息”列存在，避免写不存在的列抛 ArgumentException；且写入字符串而不是 StringBuilder
                    if (!dt.Columns.Contains("错误信息"))
                    {
                        dt.Columns.Add("错误信息");
                    }
                    dt.Rows[j]["错误信息"] = rowErrors.ToString();
                }
                result.Append(rowErrors.ToString());
            }
            return result.Return();
        }
        #endregion

        #region public static string DataTableColumn2String(DataTable dt)DataTable列转换成字符串
        /// <summary>
        /// DataTable列转换成字符串
        /// </summary>
        /// <param name="dt">DataTable</param>
        /// <returns>转换后的字符串</returns>
        public static string DataTableColumn2String(DataTable dt)
        {
            var sb = PoolUtil.StringBuilder.Get();
            for (var i = 0; i < dt.Columns.Count; i++)
            {
                if (i > 0)
                {
                    sb.Append(",");
                }
                sb.Append(dt.Columns[i].ColumnName);
            }
            return sb.Return();
        }
        #endregion
#endif

    }
}
