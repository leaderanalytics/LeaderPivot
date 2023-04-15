namespace LeaderAnalytics.LeaderPivot;

public interface IDimensionT<T> : IDimension
{
    Func<T, string> GroupValue { get; set; }
    Func<T, string> HeaderValue { get; set; }
}