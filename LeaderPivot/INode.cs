namespace LeaderAnalytics.LeaderPivot;

public interface INode
{
    bool CanToggleExapansion { get; set; }
    CellType CellType { get; set; }
    string ColumnKey { get; set; }
    string ID { get; }
    bool IsRow { get; set; }
    object Value { get; set; }
}