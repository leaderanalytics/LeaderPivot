namespace LeaderAnalytics.LeaderPivot;

public interface INodeT<T> : INode
{
    List<INodeT<T>> Children { get; set; }
    IDimensionT<T> ColumnDimension { get; set; }
    IDimensionT<T> RowDimension { get; set; }

    void AddChild(INodeT<T> child);
}