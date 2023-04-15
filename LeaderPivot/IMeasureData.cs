namespace LeaderAnalytics.LeaderPivot;

public interface IMeasureData<T>
{
    IDimensionT<T> ColumnDimension { get; set; }
    IEnumerable<T> ColumnGroup { get; set; }
    IEnumerable<T> Measure { get; set; }
    IDimensionT<T> RowDimension { get; set; }
    IEnumerable<T> RowGroup { get; set; }
}