//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2025, DotNet.
//-----------------------------------------------------------------

using System;
using System.Data;
using System.IO;
using System.Text;
#if NET452_OR_GREATER
using System.Windows.Forms;
#endif
using NPOI.SS.UserModel;

namespace DotNet.Util
{
    public partial class ExcelUtil
    {
#if NET452_OR_GREATER
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
            openFileDialog.FilterIndex = 0;
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
                    columnName = headerRow.GetCell(i).StringCellValue;
                }
                var column = new DataColumn(columnName);
                dt.Columns.Add(column);
            }

            //从第2行起添加内容行
            for (var i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++)
            {
                var row = sheet.GetRow(i);
                var dr = dt.NewRow();

                // 2012.09.12 Pcsky 设置dataRow的索引号从0开始
                var k = 0;
                for (var j = row.FirstCellNum; j < cellCount; j++)
                {
                    //if (row.GetCell(j) != null)
                    //{
                    //dataRow[j] = row.GetCell(j).ToString();

                    dr[k] = row.GetCell(j);
                    k++;
                    //}
                }

                dt.Rows.Add(dr);
            }
            wb = null;
            sheet = null;
            return dt;
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
                for (var i = 0; i < checkStrings.Length; i++)
                {
                    if (string.IsNullOrEmpty(dt.Rows[j][checkStrings[i]].ToString()))
                    {
                        result.Append("\"" + checkStrings[i] + "\"不能为空。");
                        dt.Rows[j]["错误信息"] = result;
                    }
                }
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
