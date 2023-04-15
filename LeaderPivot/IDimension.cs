namespace LeaderAnalytics.LeaderPivot;

public interface IDimension
{
    bool CanReposition { get; set; }
    bool CanRepositionAcrossAxis { get; set; }
    bool CanSort { get; set; }
    bool CanToggleExpansion { get; set; }
    string DisplayValue { get; set; }
    string ID { get; }
    bool IsAscending { get; set; }
    bool IsEnabled { get; set; }
    bool IsExpanded { get; set; }
    bool IsLeaf { get;  }
    int Ordinal { get;  }
    bool IsRow { get; set; }
    int Sequence { get; set; }
    Func<string, string> SortValue { get; set; }
}