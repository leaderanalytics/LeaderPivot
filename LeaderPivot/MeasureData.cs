namespace LeaderAnalytics.LeaderPivot;

public class MeasureData<T> : IMeasureData<T>
{
    public IEnumerable<T> Measure { get; set; }
    public IEnumerable<T> RowGroup { get; set; }
    public IEnumerable<T> ColumnGroup { get; set; }

    public MeasureData(IEnumerable<T> measure, IEnumerable<T> rowGroup, IEnumerable<T> columnGroup)
    {
        Measure = measure;
        RowGroup = rowGroup;
        ColumnGroup = columnGroup;
    }
}
