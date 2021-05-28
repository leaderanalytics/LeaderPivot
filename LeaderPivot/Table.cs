using System.Collections.Generic;

namespace LeaderPivot
{
    public class Table
    {
        public List<TableRow> Rows { get; set; }

        public Table() => Rows = new List<TableRow>();
    }
}
