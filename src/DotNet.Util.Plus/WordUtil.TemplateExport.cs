//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2022, DotNet.
//-----------------------------------------------------------------

using System;
using System.Data;
using System.IO;
using System.Web;
using NPOI.XSSF.UserModel;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System.Collections.Generic;
using NPOI.XWPF.UserModel;
using NPOI.OpenXmlFormats.Wordprocessing;
using System.Linq;
using System.Text.RegularExpressions;

namespace DotNet.Util
{
    /// <summary>
    /// WordUtil
    /// 从Word模板中替换并导出Word
    /// 参考https://github.com/lc1055/EasyNPOI
    /// 
    /// 修改记录
    ///
    ///     2022.09.13 版本：1.0 Troy Cui   新增Word导出功能
    /// 
    /// <author>
    ///		<name>Troy.Cui</name>
    ///		<date>2022.09.13</date>
    /// </author> 
    /// </summary>
    public partial class WordUtil
    {
#if NET452_OR_GREATER

        //在NPOI中，每厘米对应的长度数值
        private const int NPOI_PICTURE_LENGTH_EVERY_CM = 360144;

        /// <summary>
        /// 从模板文件读取
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static XWPFDocument GetXWPFDocument(string filePath)
        {
            XWPFDocument word = null;

            if (!File.Exists(filePath))
            {
                LogUtil.WriteLog("找不到模板文件");
            }
            else
            {
                try
                {
                    using (FileStream fs = File.OpenRead(filePath))
                    {
                        word = new XWPFDocument(fs);
                    }
                }
                catch (Exception ex)
                {
                    LogUtil.WriteException(ex, "打开模板文件失败");
                }
            }

            return word;
        }

        /// <summary>
        /// 保存文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="doc"></param>
        public static void SaveXWPFDocument(string filePath, XWPFDocument doc)
        {
            FileStream fs = null;
            try
            {
                var dir = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                fs = new FileStream(filePath, FileMode.Create, FileAccess.Write);
                doc.Write(fs);
            }
            catch (Exception ex)
            {
                LogUtil.WriteException(ex);
                throw;                
            }
            finally
            {
                fs.Close();
            }
        }

        /// <summary>
        /// 替换Word中的所有占位符
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="basicReplacements">基本替换（文本/图片类）</param>
        /// <param name="gridReplacements">表格替换（多行）</param>
        public static void ReplaceInWord(XWPFDocument doc, List<ReplacementBasic> basicReplacements, List<ReplacementGrid> gridReplacements)
        {
            IEnumerator<XWPFParagraph> allParagraphs = doc.GetParagraphsEnumerator();
            XWPFParagraph paragraph;
            while (allParagraphs.MoveNext())
            {
                paragraph = allParagraphs.Current;
                ReplaceInParagraph(paragraph, basicReplacements);
            }

            IEnumerator<XWPFTable> allTables = doc.GetTablesEnumerator();
            XWPFTable table;
            while (allTables.MoveNext())
            {
                table = allTables.Current;
                ReplaceInTable(table, basicReplacements, gridReplacements);
            }
        }

        /// <summary>
        /// 替换单个表格中的占位符
        /// </summary>
        /// <param name="table"></param>
        /// <param name="basicReplacements"></param>
        /// <param name="gridReplacements"></param>
        public static void ReplaceInTable(XWPFTable table, List<ReplacementBasic> basicReplacements, List<ReplacementGrid> gridReplacements)
        {
            if (table == null)
            {
                return;
            }

            var rows = table.Rows;
            //存放所有grid占位符的行对象
            List<GridPlaceholderRow> gridPlaceholderRowList = new List<GridPlaceholderRow>();
            var _thisIsTempRowIndex = -1;
            var _rowIndex = 0;
            foreach (XWPFTableRow row in rows)
            {
                //遇到模板行跳出循环
                if (_rowIndex == _thisIsTempRowIndex)
                {
                    _thisIsTempRowIndex = -1;
                    _rowIndex++;
                    continue;
                }

                //从每一行的第一个单元格字符数据，如果存在grid占位符中，则需要进行grid处理
                var cells = row.GetTableCells();
                var cell = cells[0];
                var _gridPlaceholderName = cell.GetText();
                var gridReplacement = gridReplacements.FirstOrDefault(p => p.Placeholder == _gridPlaceholderName);
                if (gridReplacement != null)
                {
                    gridPlaceholderRowList.Add(new GridPlaceholderRow
                    {
                        index = _rowIndex,
                        row = row,
                        replacement = gridReplacement
                    });

                    //标记下一行就是模板行
                    _thisIsTempRowIndex = _rowIndex + 1;
                }
                else
                {
                    //循环单元格
                    foreach (XWPFTableCell ccell in cells)
                    {
                        ReplaceInParagraphs(ccell.Paragraphs, basicReplacements);
                    }
                }
                _rowIndex++;
            }
            _thisIsTempRowIndex = -1;

            if (gridPlaceholderRowList.Count > 0)
            {
                var addedRowCount = 0;
                foreach (var placeholder_row in gridPlaceholderRowList)
                {
                    var placeholderRowIndex = placeholder_row.index + addedRowCount;
                    var placeholderRow = placeholder_row.row;
                    var gridReplacement = placeholder_row.replacement;

                    //模板行
                    var tmplRowIndex = placeholderRowIndex + 1;
                    var tmplRow = table.GetRow(tmplRowIndex);

                    //对模板行的校验
                    if (tmplRow == null) continue;
                    var tmplRowCells = tmplRow.GetTableCells();
                    //列数不匹配
                    if (tmplRowCells.Count <= 0) continue;
                    //不含有占位符
                    var first_cell_text = tmplRowCells[0].GetText();
                    string regEx = @"\{.+?\}";
                    Regex r = new Regex(regEx);
                    var matched = r.IsMatch(first_cell_text);
                    if (!matched) continue;


                    var addRowStartIndex = tmplRowIndex;
                    foreach (var dataRow in gridReplacement.Rows)
                    {
                        addRowStartIndex++;

                        //此方法也可以复制行，但是会同时修改模板行
                        //CT_Row ctrow = new CT_Row();
                        //ctrow = tmplRow.GetCTRow();
                        //XWPFTableRow addedRow = new XWPFTableRow(ctrow, table);
                        //table.AddRow(addedRow, addRowStartIndex);

                        XWPFTableRow addedRow = CopyRow(tmplRow, table, addRowStartIndex);

                        ReplaceRow(addedRow, dataRow);

                        addedRowCount++;
                    }
                    //移去占位符行和模板行
                    table.RemoveRow(placeholderRowIndex);
                    table.RemoveRow(tmplRowIndex - 1);//上一行已被移除，所以这一行的索引还要减1
                    addedRowCount -= 2;
                }
            }

        }
        class GridPlaceholderRow
        {
            public int index { get; set; }
            public XWPFTableRow row { get; set; }
            public ReplacementGrid replacement { get; set; }
        }

        /// <summary>
        /// 替换表格中的一行 
        /// </summary>
        /// <param name="addedRow"></param>
        /// <param name="dataRow"></param>
        private static void ReplaceRow(XWPFTableRow addedRow, ReplacementRow dataRow)
        {
            //遍历新行的每个单元格，进行赋值
            foreach (var cell in addedRow.GetTableCells())
            {
                ReplaceInParagraphs(cell.Paragraphs, dataRow.Cells);
            }
        }

        //循环段落，无业务代码
        private static void ReplaceInParagraphs(IEnumerable<XWPFParagraph> xwpfParagraphs, List<ReplacementBasic> basicReplacements)
        {
            foreach (XWPFParagraph paragraph in xwpfParagraphs)
            {
                ReplaceInParagraph(paragraph, basicReplacements);
            }
        }

        /// <summary>
        /// 替换单个段落中的所有占位符(格式为{})
        /// </summary>
        /// <param name="paragraph"></param>
        /// <param name="basicReplacements"></param>
        public static void ReplaceInParagraph(XWPFParagraph paragraph, List<ReplacementBasic> basicReplacements)
        {
            if (paragraph == null || string.IsNullOrWhiteSpace(paragraph.Text))
            {
                return;
            }

            //用正则判断段落里是否含有占位符
            string regEx = @"\{.+?\}";
            Regex r = new Regex(regEx);
            var matched = r.IsMatch(paragraph.Text);
            if (!matched)
            {
                return;
            }

            if (basicReplacements != null && basicReplacements.Count > 0)
            {
                foreach (var replace in basicReplacements)
                {
                    if (replace.Type == PlaceholderTypeEnum.Text)
                    {
                        ReplaceTextInRun(paragraph, replace);
                    }
                    else if (replace.Type == PlaceholderTypeEnum.Picture)
                    {
                        ReplacePictureInRun(paragraph, replace);
                    }

                    //再次用正则判断段落里是否含有占位符
                    matched = r.IsMatch(paragraph.Text);
                    if (!matched)
                    {
                        return;
                    }
                }
            }

            //递归处理
            //ReplaceInParagraph(paragraph, basicReplacements);
        }

        /// <summary>
        /// 替换单个占位符(文本)
        /// </summary>
        /// <param name="paragraph"></param>
        /// <param name="replace"></param>
        private static void ReplaceTextInRun(XWPFParagraph paragraph, ReplacementBasic replace)
        {
            if (replace == null || replace.Type != PlaceholderTypeEnum.Text || string.IsNullOrWhiteSpace(paragraph.Text))
            {
                return;
            }

            TextSegment split = paragraph.SearchText("{ }", new PositionInParagraph());
            if (split != null)
            {
                paragraph.ReplaceText("{ }", "");
            }

            TextSegment ts = paragraph.SearchText(replace.Placeholder, new PositionInParagraph());
            if (ts == null || !(ts.BeginRun != ts.EndRun || ts.BeginChar != ts.EndChar))
            {
                return;
            }

            paragraph.ReplaceText(replace.Placeholder, replace.Text);

            //利用递归处理同一个占位符
            ReplaceTextInRun(paragraph, replace);
        }

        /// <summary>
        /// 替换单个占位符(图片)
        /// </summary>
        /// <param name="paragraph"></param>
        /// <param name="replace"></param>
        private static void ReplacePictureInRun(XWPFParagraph paragraph, ReplacementBasic replace)
        {
            if (replace == null || replace.Type != PlaceholderTypeEnum.Picture || string.IsNullOrWhiteSpace(paragraph.Text))
            {
                return;
            }
            var picList = replace.Pictures;
            if (picList == null || picList.Count() == 0)
            {
                replace.Text = "";
                replace.Type = PlaceholderTypeEnum.Text;
                ReplaceTextInRun(paragraph, replace);
                return;
            }
            TextSegment ts = paragraph.SearchText(replace.Placeholder, new PositionInParagraph());
            if (ts == null)
            {
                return;
            }

            var beginIndex = ts.BeginRun;
            var endIndex = ts.EndRun;
            var runs = paragraph.Runs;
            var begin_run = runs[beginIndex];

            //再beginrun的位置插入图片
            foreach (var picture in picList)
            {
                var pictureData = picture.PictureData;
                if (pictureData == null || pictureData.Length == 0)
                {
                    try
                    {
                        pictureData = File.OpenRead(picture.PictureUrl);
                    }
                    catch (Exception)
                    {
                    }
                }

                if (pictureData == null || pictureData.Length == 0)
                {
                    begin_run.AppendText(picture.FileName);
                    continue;
                }

                int height = (int)(Math.Ceiling(picture.Height * NPOI_PICTURE_LENGTH_EVERY_CM));
                int width = (int)(Math.Ceiling(picture.Width * NPOI_PICTURE_LENGTH_EVERY_CM));
                begin_run.AddPicture(pictureData, picture.PictureType.GetHashCode(), picture.FileName, width, height);

                NPOI.OpenXmlFormats.Dml.WordProcessing.CT_Inline inline = begin_run.GetCTR().GetDrawingList()[0].inline[0];
                inline.docPr.id = 1;

                pictureData.Dispose();
            }
            //然后清空所有run
            for (int i = beginIndex; i <= endIndex; i++)
            {
                var run = runs[i];
                if (run.Text.Contains(replace.Placeholder))
                {
                    var runText = run.Text.Replace(replace.Placeholder, "");
                    run.SetText(runText, 0);
                }
                else
                {
                    run.SetText("", 0);
                }
            }

            //利用递归处理同一个占位符
            ReplacePictureInRun(paragraph, replace);
        }



        /// <summary>
        /// 复制一行到指定位置
        /// 样式信息也复制了，但需要完善。
        /// </summary>
        /// <param name="sourceRow"></param>
        /// <param name="table"></param>
        /// <param name="rowIndex"></param>
        /// <returns></returns>
        public static XWPFTableRow CopyRow(XWPFTableRow sourceRow, XWPFTable table, int rowIndex)
        {
            //在表格指定位置新增一行
            var needRemove = false;
            if (table.NumberOfRows <= rowIndex)
            {
                table.CreateRow();
                needRemove = true;
            }
            XWPFTableRow targetRow = table.InsertNewTableRow(rowIndex);
            if (needRemove)
            {
                table.RemoveRow(rowIndex + 1);
            }

            //复制行属性
            targetRow.GetCTRow().trPr = sourceRow.GetCTRow().trPr;
            List<XWPFTableCell> sourceCells = sourceRow.GetTableCells();
            if (null == sourceCells)
            {
                return targetRow;
            }
            //复制列及其属性和内容
            foreach (var sourceCell in sourceCells)
            {
                XWPFTableCell targetCell = targetRow.AddNewTableCell();
                targetCell.RemoveParagraph(0);//新建cell会自动创建paragraph，将其删除，下面代码循环添加

                //列属性
                targetCell.GetCTTc().tcPr = sourceCell.GetCTTc().tcPr;

                //段落属性
                if (sourceCell.Paragraphs != null && sourceCell.Paragraphs.Count > 0)
                {
                    foreach (var sourcePa in sourceCell.Paragraphs)
                    {
                        if (sourcePa.Runs != null && sourcePa.Runs.Count > 0)
                        {
                            var targetPa = targetCell.AddParagraph();
                            targetPa.Alignment = sourcePa.Alignment;
                            foreach (var srcR in sourcePa.Runs)
                            {
                                XWPFRun tarR = targetPa.CreateRun();
                                tarR.SetText(srcR.Text);
                                tarR.TextPosition = srcR.TextPosition;
                                tarR.FontFamily = srcR.FontFamily;
                                tarR.FontSize = srcR.FontSize <= 0 ? 12 : srcR.FontSize;
                                tarR.IsBold = srcR.IsBold;
                                tarR.IsItalic = srcR.IsItalic;
                                tarR.IsCapitalized = srcR.IsCapitalized;
                                tarR.SetColor(srcR.GetColor());
                                tarR.SetUnderline(srcR.Underline);
                                tarR.CharacterSpacing = srcR.CharacterSpacing;
                            }
                        }
                        else
                        {
                            targetCell.SetText(sourceCell.GetText());
                        }
                    }
                }
                else
                {
                    targetCell.SetText(sourceCell.GetText());
                }
            }
            return targetRow;
        }

        #region Models
        /// <summary>
        /// 占位符替换对象(文本/图片)
        /// </summary>
        public class ReplacementBasic
        {
            /// <summary>
            /// 占位符
            /// </summary>
            public string Placeholder { get; set; }

            /// <summary>
            /// 替换的文本
            /// </summary>
            public string Text { get; set; }

            /// <summary>
            /// 替换的图片
            /// </summary>
            public IEnumerable<Picture> Pictures { get; set; }

            /// <summary>
            /// 占位符类型
            /// </summary>
            public PlaceholderTypeEnum Type { get; set; }
        }

        /// <summary>
        /// 占位符替换对象(表格)
        /// </summary>
        public class ReplacementGrid
        {
            /// <summary>
            /// 占位符
            /// </summary>
            public string Placeholder { get; set; }

            /// <summary>
            /// 替换的列表数据
            /// </summary>
            public List<ReplacementRow> Rows { get; set; } = new List<ReplacementRow>();

            /// <summary>
            /// 占位符类型
            /// </summary>
            public PlaceholderTypeEnum Type { get { return PlaceholderTypeEnum.Grid; } }
        }
        /// <summary>
        /// 占位符替换对象(行)
        /// </summary>
        public class ReplacementRow
        {
            /// <summary>
            /// 列
            /// </summary>
            public List<ReplacementBasic> Cells { get; set; } = new List<ReplacementBasic>();
        }
        #endregion

        #region Picture
        public class Picture
        {
            /// <summary>
            /// 图片流数据
            /// </summary>
            public Stream PictureData { get; set; }

            /// <summary>
            /// 图片绝对地址（后续程序会将此路径文件转为 <see cref="PictureData"/>）
            /// </summary>
            public string PictureUrl { get; set; }

            /// <summary>
            /// 图片类型，默认PNG
            /// </summary>
            public PictureTypeEnum PictureType { get; set; } = PictureTypeEnum.PNG;

            /// <summary>
            /// 图片文件名。若设置了，则当图片不存在时，会显示此文本
            /// </summary>
            public string FileName { get; set; } = "";

            /// <summary>
            /// 图片宽度，单位厘米，默认14
            /// </summary>
            public decimal Width { get; set; } = 1;

            /// <summary>
            /// 图片高度，单位厘米，默认8
            /// </summary>
            public decimal Height { get; set; } = 1;
        }
        #endregion

        #region Enums
        /// <summary>
        /// 占位符类型
        /// </summary>
        public enum PlaceholderTypeEnum
        {
            Text,
            Picture,
            Grid,
        }
        /// <summary>
        /// 图片类型
        /// </summary>
        public enum PictureTypeEnum
        {
            EMF = 2,
            WMF,
            PICT,
            JPEG,
            PNG,
            DIB,
            GIF,
            TIFF,
            EPS,
            BMP,
            WPG
        }
        #endregion
#endif
    }
}