namespace LeaderAnalytics.LeaderPivot;

public interface IMatrixCell
{
    bool CanToggleExapansion { get; set; }
    CellType CellType { get; set; }
    int ColSpan { get; set; }
    bool IsExpanded { get; set; }
    string NodeID { get; set; }
    int RowSpan { get; set; }
    object Value { get; set; }
}