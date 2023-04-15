namespace LeaderAnalytics.LeaderPivot;

public interface IMeasure
{
    bool CanDisable { get; set; }
    string DisplayValue { get; set; }
    string Format { get; set; }
    bool IsEnabled { get; set; }
    int Sequence { get; set; }
}