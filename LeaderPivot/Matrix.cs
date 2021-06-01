using System;
using System.Collections.Generic;
using System.Linq;

namespace LeaderPivot
{
    public class Matrix<T>
    {
        protected IEnumerable<T> data;
        protected IEnumerable<Dimension<T>> dimensions;
        protected IEnumerable<Measure<T>> measures;
        protected bool DisplayGrandTotals;

        public Matrix(IEnumerable<T> data, IEnumerable<Dimension<T>> dimensions, IEnumerable<Measure<T>> measures, bool displayGrandTotals)
        {
            this.data = data;
            this.dimensions = dimensions.OrderBy(x => !x.IsRow).ThenBy(x => x.Sequence);
            this.measures = measures;
            this.DisplayGrandTotals = displayGrandTotals;
        }

        public Vector<T> GetVector()
        {
            return new Vector<T>(data, dimensions, measures, DisplayGrandTotals);
        }

        public Table BuildTable(Vector<T> vector)
        {
            
            Table t = new Table();
            Vector<T> columnHeaders = new Vector<T>(data, dimensions.Where(x => !x.IsRow), measures, DisplayGrandTotals, true);
            BuildColumnHeaders(columnHeaders, t, 0, 0);

            foreach (Vector<T> child in vector.Children)
                BuildTableBody(child, t, null);

            return t;
        }

        private void BuildColumnHeaders(Vector<T> vector, Table t, int index, int totalWidth)
        {
            if (index == 0)
            {
                totalWidth = GetColumnHeaderCount(vector, 0);
                // 1.) Add an empty cell at 0,0 that spans row headers in width and column headers in height
                // 2.) Add rows to accommodate the rest of the column headers

                // Count the number of expanded column dimensions.  This sets the height of the cell at 0,0.
                // Add one for row zero which will display sort buttons for column dimensions
                int rowsSpan = GetHeaderDepth(vector, false, 0) + 1;
                

                // We cant set the width of the cell at 0,0 because we don't have row data.
                int columnSpan = 1;

                // Add row zero.  Add a cell to that row at 0,0 spanning row headers and column headers.
                TableRow row = new TableRow();
                row.Cells.Add(new Cell(null, rowsSpan, columnSpan));
                t.Rows.Add(row);

                // Add a second cell to row zero that is as wide as the number of leaf node columns in vector.

                row.Cells.Add(new Cell(null, 1, totalWidth));

                // Add remaining rows to display column headers.  We have already added one.
                for (int i = 0; i < rowsSpan - 1; i++)
                    t.Rows.Add(new TableRow());

                index = 1;  // Don't add any cells to row 0.
            }


            for(int i=0;i<vector.Children.Count;i++)
            {
                Vector<T> child = vector.Children[i];

                if (child.IsExpanded)
                    BuildColumnHeaders(child, t, index + 1, totalWidth);

                TableRow row = t.Rows[index];
                int headerDepth = GetHeaderDepth(child, false, 0);
                row.Cells.Add(new Cell(child.Value, headerDepth, GetColumnHeaderCount(child, 0)));

                //if (child.DisplayTotals && child.IsExpanded && !child.IsLeafNode)
                //    row.Cells.Add(new Cell(child.Value + " Total", headerDepth, measures.Count()));

            }

            //if (index == 1 && DisplayGrandTotals)
            // {
            //     TR row = t.Rows[index];
            //     row.Cells.Add(new Cell("Grand Total", t.Rows.Count -1, measures.Count()));
            // }
        }

        // Finds the dimension (row or column) that has the greatest number of expanded levels.
        private int GetHeaderDepth(Vector<T> vector, bool checkRows, int maxDepth)
        {
            int tmp = maxDepth + 1;
            foreach (Vector<T> child in vector.Children.Where(x => (x.IsRow && checkRows || !x.IsRow && !checkRows) && x.IsExpanded))
                maxDepth = Math.Max(maxDepth, GetHeaderDepth(child, checkRows, tmp));

            return maxDepth;
        }

        // Returns the total number of cells required to display column headers for both collapsed and expanded dimensions.
        private int GetColumnHeaderCount(Vector<T> vector, int count)
        {
            int tmp = 0;

            foreach (Vector<T> child in vector.Children)
                tmp += GetColumnHeaderCount(child, count);
            
            
            if (!vector.IsRow)
                count = vector.IsExpanded ? (vector.Children.Count == 0 ? 1 : 0) : 1;
            
            return count + tmp;
        }

        private Table BuildTableBody(Vector<T> vector, Table t, TableRow row)
        {
            if (row == null)
            {
                row = new TableRow();
                t.Rows.Add(row);
            }
            Cell c = new Cell(vector.Value);
            row.Cells.Add(c);

            if (vector.IsExpanded)
            {
                foreach (Vector<T> child in vector.Children)
                {

                    if (child.IsRow)
                        BuildTableBody(child, t, null);
                    else
                        BuildTableBody(child, t, row);
                }
            }
            return t;
        }
    }
}
