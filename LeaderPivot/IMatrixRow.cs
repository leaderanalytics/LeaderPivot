namespace LeaderAnalytics.LeaderPivot;

public interface IMatrixRow
{
    List<IMatrixCell> Cells { get; set; }
}