namespace LeaderAnalytics.LeaderPivot;

/// <summary>
/// A row on a matrix.  A row has a theoretically unlimited number of columns.
/// </summary>
public class MatrixRow : IMatrixRow
{
    public List<IMatrixCell> Cells { get; set; }

    public MatrixRow() => Cells = new List<IMatrixCell>();
}
