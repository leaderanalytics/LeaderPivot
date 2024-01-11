namespace LeaderAnalytics.LeaderPivot;

/// <summary>
/// Represents a rectangular data structure with a theoretically 
/// unbounded number of columns.
/// </summary>
public class Matrix : IMatrix
{
    public List<IMatrixRow> Rows { get; set; }

    public Matrix() => Rows = new List<IMatrixRow>();
}
