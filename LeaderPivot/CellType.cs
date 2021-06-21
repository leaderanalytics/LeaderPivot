using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaderPivot
{
    /// <summary>
    /// Used in the creation of both Nodes and Matrices. 
    /// </summary>
    public enum CellType
    {
        Root,               // The single node who's children represent the topmost elements of a node structure
        Measure,            // A leaf node value - often a group sum.
        Total,              // An aggregate of more or more Measures
        GrandTotal,         // Yep it's that
        MeasureHeader,      // Header for a Measure
        GroupHeader,        // Header for a group which contain one or more Measures
        TotalHeader,        // Header for a total row
        GrandTotalHeader    // Header for a grand total
    }
}
