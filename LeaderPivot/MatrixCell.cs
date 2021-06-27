
namespace LeaderAnalytics.LeaderPivot
{
    /// <summary>
    /// Represents a single value in a rectangular matrix.
    /// </summary>
    public class MatrixCell
    {
        public object Value { get; set; }
        public CellType CellType { get; set; }
        public bool IsExpanded { get; set; }
        public int RowSpan { get; set; }
        public int ColSpan { get; set; }
        public string DimensionID { get; set; }

        public MatrixCell() => RowSpan = ColSpan = 1;

        public MatrixCell(object value, CellType cellType, bool isExpanded, int rowSpan = 1, int colSpan = 1)
        {
            Value = value;
            CellType = cellType;
            IsExpanded = isExpanded;
            RowSpan = rowSpan;
            ColSpan = colSpan;
        }
    }
}
