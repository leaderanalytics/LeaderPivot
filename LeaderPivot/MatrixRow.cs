using System.Collections.Generic;

namespace LeaderAnalytics.LeaderPivot
{
    /// <summary>
    /// A row on a matrix.  A row has a theoretically unlimited number of columns.
    /// </summary>
    public class MatrixRow
    {
        public List<MatrixCell> Cells { get; set; }

        public MatrixRow() => Cells = new List<MatrixCell>();
    }
}
