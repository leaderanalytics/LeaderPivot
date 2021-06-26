using System;

namespace LeaderAnalytics.LeaderPivot
{
    public class Dimension<T>
    {
        /// <summary>
        /// A friendly value that identifies how the data will be grouped i.e. City, Product Name, etc.  This value is displayed on the button that allows the user to reposition the dimension.
        /// This value is also used to uniquely identify the dimension.  DisplayValue can not be null and must be unique for each Dimension.
        /// </summary>
        public string DisplayValue { get; set; }

        /// <summary>
        /// Function of T used to group the data.
        /// </summary>
        public Func<T, string> GroupValue { get; set; }              

        /// <summary>
        /// Function of T used to display a friendly row or column header value.  GroupValue is used if not set.
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
        /// Ordinal position of this dimension relative to others of same axis (row or column).
        /// </summary>
        public int Sequence { get; set; }

        /// <summary>
        /// If true, user will be allowed to toggle ascending / descending sorting.  Default value is true.
        /// </summary>
        public bool CanSort { get; set; } = true;

        /// <summary>
        /// If true, user can drag this dimension up or down the row or column hierarchy.  Default value is true.
        /// </summary>
        public bool CanReposition { get; set; } = true;

        /// <summary>
        /// If true, user can drag this dimension from row to column axis and vice versa.  Ignored if CanReposition is false.  Default value is true.
        /// </summary>
        public bool CanRepositionAcrossAxis { get; set; } = true;
    }
}
