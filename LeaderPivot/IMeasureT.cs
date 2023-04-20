namespace LeaderAnalytics.LeaderPivot;

public interface IMeasureT<T> : IMeasure
{
    Func<IMeasureData<T>, decimal> Aggragate { get; set; }
}