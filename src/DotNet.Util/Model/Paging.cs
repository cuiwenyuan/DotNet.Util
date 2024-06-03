//-----------------------------------------------------------------
// All Rights Reserved. Copyright (c) 2024, DotNet.
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
        private int _pageNo = 1;
        /// <summary>
        /// 页编号（从1开始，1代表第1页）
        /// </summary>
        public int PageNo
        {
            get
            {
                return _pageNo;
            }
            set
            {
                if (value > 1)
                {
                    _pageNo = value;
                }
                else
                {
                    _pageNo = 1;
                }
            }
        }
        private int _pageIndex = 0;
        /// <summary>
        /// 页索引（从0开始，0代表第1页）
        /// </summary>
        [Obsolete("Please use PageNo from 2022-08-18")]
        public int PageIndex
        {
            get
            {
                return _pageIndex;
            }
            set
            {
                if (value >= 0)
                {
                    _pageIndex = value;
                    if (_pageNo != _pageIndex + 1)
                    {
                        _pageNo = _pageIndex + 1;
                    }
                }
                else
                {
                    _pageIndex = 0;
                }
            }
        }
        private int _pageSize = 20;
        /// <summary>
        /// 每页显示的记录数
        /// </summary>
        public int PageSize
        {
            get
            {
                return _pageSize;
            }
            set
            {
                if (value > 1)
                {
                    _pageSize = value;
                }
                else
                {
                    _pageSize = 20;
                }
            }
        }

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
        public string SortExpression { get; set; } = BaseUtil.FieldCreateTime;

        /// <summary>
        /// 排序方向（DESC或ASC）
        /// </summary>
        public string SortDirection { get; set; } = "DESC";
    }
}