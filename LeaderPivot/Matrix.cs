using System.Collections.Generic;

namespace LeaderAnalytics.LeaderPivot
{
    /// <summary>
    /// Represents a rectangular data structure.  Difference between a matrix and a table
    /// is that a matrix has a theoretically unbounded number of columns whereas
    /// a traditional table such as a database table has a fixed number of columns.
    /// </summary>
    public class Matrix
    {
        public List<MatrixRow> Rows { get; set; }

        public Matrix() => Rows = new List<MatrixRow>();
    }
}
