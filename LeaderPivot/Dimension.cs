using System;

namespace LeaderAnalytics.LeaderPivot
{
    public class Dimension<T>
    {
        /// <summary>
        /// Function of T used to group the data.
        /// </summary>
        public Func<T, string> GroupValue { get; set; }              

        /// <summary>
        /// Function of T used as a header value.  GroupValue is used if not set.
        /// </summary>
        public Func<T, string> HeaderValue { get; set; }

        /// <summary>
        /// Function that takes the group key for further manipulation for sorting.  GroupValue is used if not set.
        /// </summary>
        public Func<string, string> SortValue { get; set; }

        /// <summary>
        /// Set this value to true if data is to be displayed horizontal axis in rows.  Set to false to display data on the vertical axis in columns
        /// </summary>
        public bool IsRow { get; set; }

        /// <summary>
        /// If true child dimensions, if any will be displayed.  If false only totals will be displayed.
        /// </summary>
        public bool IsExpanded { get; set; }

        /// <summary>
        /// Set to true to sort data in ascending order, false for descending order.
        /// </summary>
        public bool IsAscending { get; set; }

        /// <summary>
        /// Ordinal position of this dimension relative to others of axis (row or column).
        /// </summary>
        public int Sequence { get; set; }
    }
}
