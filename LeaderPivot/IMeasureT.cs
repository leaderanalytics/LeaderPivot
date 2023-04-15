namespace LeaderAnalytics.LeaderPivot;

public interface IMeasureT<T> : IMeasure
{
    Func<MeasureData<T>, decimal> Aggragate { get; set; }
}