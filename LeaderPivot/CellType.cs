namespace LeaderAnalytics.LeaderPivot;

/// <summary>
/// Used in the creation of both Nodes and Matrices. 
/// </summary>
public enum CellType
{
    Root,               // The single node who's children represent the topmost elements of a node structure
    Measure,            // A leaf node value - often a group sum.
    Total,              // An aggregate of more or more Measures
    GrandTotal,         // Yep it's that
    MeasureLabel,       // Label for a Measure
    MeasureTotalLabel,  // Label for a Measure in a total column
    GroupHeader,        // Header for a group which contain one or more Measures
    TotalHeader,        // Header for a total row or column
    GrandTotalHeader    // Header for a grand total row or column
}
