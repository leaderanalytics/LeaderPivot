namespace LeaderAnalytics.LeaderPivot;
public class Measure<T> : Measure, IMeasureT<T>
{
    /// <summary>
    /// A function that takes T as a parameter and performs some aggregation i.e. x => x.Sum(y => y.Quantity)
    /// </summary>
    public Func<IMeasureData<T>, decimal> Aggragate { get; set; }
}

public class Measure : IMeasure
{
    /// <summary>
    /// A friendly value that identifies how the data being be aggregated i.e. Amount, Quantity, Price, etc.  This value can not be null and must be unique for each Measure.
    /// </summary>
    public string DisplayValue { get; set; }

    /// <summary>
    /// An argument used to format the calculated value for display.  For example:  "{0:C}" for currency amounts.
    /// </summary>
    public string Format { get; set; }

    /// <summary>
    /// If true, the measure will be calculated and displayed on the pivot table.  At least one measure must be enabled.  Default value is true.
    /// </summary>
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// If true, user can disable display of this measure.  Ignored if only one measure is defined.  Default value is true.
    /// </summary>
    public bool CanDisable { get; set; } = true;

    /// <summary>
    /// Ordinal position of the measure from left to right.
    /// </summary>
    public int Sequence { get; set; }

}
