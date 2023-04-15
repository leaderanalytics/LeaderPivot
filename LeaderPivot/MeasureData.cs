namespace LeaderAnalytics.LeaderPivot;

public class MeasureData<T> : IMeasureData<T>
{
    public IEnumerable<T> Measure { get; set; }
    public IEnumerable<T> RowGroup { get; set; }
    public IEnumerable<T> ColumnGroup { get; set; }
    public IDimensionT<T> RowDimension { get; set; }
    public IDimensionT<T> ColumnDimension { get; set; }

    public MeasureData(IEnumerable<T> measure, IEnumerable<T> rowGroup, IEnumerable<T> columnGroup, IDimensionT<T> rowDimension, IDimensionT<T> columnDimension)
    {
        Measure = measure;
        RowGroup = rowGroup;
        ColumnGroup = columnGroup;
        RowDimension = rowDimension;
        ColumnDimension = columnDimension;
    }
}
