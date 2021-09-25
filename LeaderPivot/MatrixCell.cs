
using Microsoft.VisualBasic;

namespace LeaderAnalytics.LeaderPivot
{
    /// <summary>
    /// Represents a single value in a rectangular matrix.
    /// </summary>
    public class MatrixCell
    {
        public object Value { get; set; }
        public CellType CellType { get; set; }
        public int RowSpan { get; set; }
        public int ColSpan { get; set; }
        public string NodeID { get; set; }
        public bool IsExpanded { get; set; }
        public bool CanToggleExapansion { get; set; }

        public MatrixCell() => RowSpan = ColSpan = 1;

        public MatrixCell(CellType cellType, int rowSpan = 1, int colSpan = 1)
        {
            CellType = cellType;
            IsExpanded = true;
            RowSpan = rowSpan;
            ColSpan = colSpan;
        }

        public MatrixCell(Node node, int rowSpan = 1, int colSpan = 1)
        {
            Value = node.Value;
            NodeID = null;
            IsExpanded = node.IsExpanded;
            CanToggleExapansion = node.CanToggleExapansion;
            RowSpan = rowSpan;
            ColSpan = colSpan;
        }
    }
}
