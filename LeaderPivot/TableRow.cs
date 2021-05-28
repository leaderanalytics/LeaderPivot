using System.Collections.Generic;

namespace LeaderPivot
{
    public class TableRow
    {
        public List<Cell> Cells { get; set; }

        public TableRow() => Cells = new List<Cell>();
    }
}
