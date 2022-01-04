//-----------------------------------------------------------------
// All Rights Reserved. Copyright (C) 2021, DotNet.
//-----------------------------------------------------------------

using System;

namespace DotNet.Util
{
    /// <summary>
    /// 分页
    /// </summary>
    [Serializable]
    public class Paging
    {
        /// <summary>
        /// 页编号（从1开始，1代表第1页）
        /// </summary>
        public int PageNo { get; set; } = 1;
        /// <summary>
        /// 页索引（从0开始，0代表第1页）
        /// </summary>
        public int PageIndex { get; set; } = 0;

        /// <summary>
        /// 每页显示的记录数
        /// </summary>
        public int PageSize { get; set; } = 20;

        /// <summary>
        /// 总记录数
        /// </summary>
        public int RecordCount { get; set; } = 0;

        /// <summary>
        /// 总页数
        /// </summary>
        public int PageCount
        {
            get
            {
                if (PageSize == 0)
                {
                    return 0;
                }
                return (int)Math.Ceiling((decimal)RecordCount / (decimal)PageSize);
            }
        }

        /// <summary>
        /// 排序表达式（ORDER BY后的字段或多个字段）
        /// </summary>
        public string SortExpression { get; set; } = BaseSystemInfo.DatabaseTableVersion == 4 ? BaseUtil.FieldCreateOn : BaseUtil.FieldCreateTime;

        /// <summary>
        /// 排序方向（DESC或ASC）
        /// </summary>
        public string SortDirection { get; set; } = "DESC";
    }
}