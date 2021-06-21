
namespace LeaderAnalytics.LeaderPivot
{
    /// <summary>
    /// Represents a single value in a rectangular matrix.
    /// </summary>
    public class MatrixCell
    {
        public object Value { get; set; }
        public int RowSpan { get; set; }
        public int ColSpan { get; set; }

        public MatrixCell() => RowSpan = ColSpan = 1;

        public MatrixCell(object value, int rowSpan = 1, int colSpan = 1)
        {
            Value = value;
            RowSpan = rowSpan;
            ColSpan = colSpan;
        }
    }
}
