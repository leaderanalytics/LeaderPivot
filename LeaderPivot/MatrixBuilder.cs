/* 
 * Copyright 2021 Leader Analytics 
 * www.LeaderAnalytics.com
 * www.SamWheat.com
 * Written by Sam Wheat
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;

namespace LeaderAnalytics.LeaderPivot
{
    public class MatrixBuilder<T>
    {
        protected IEnumerable<T> data;
        protected IEnumerable<Dimension<T>> dimensions;
        protected IEnumerable<Measure<T>> measures;
        protected bool DisplayGrandTotals;
        private int headerHeight;   // Total number of column header rows including one topmost empty row. Also includes measure headers.
        private int headerWidth;    // Total number of row header columns   
        private Dictionary<string, int> ColumnIndexDict;
        private NodeBuilder<T> nodeBuilder;
        private Validator<T> validator;

        public MatrixBuilder(NodeBuilder<T> nodeBuilder, Validator<T> validator)
        {
            this.validator = validator ?? throw new ArgumentNullException(nameof(validator));
            this.nodeBuilder = nodeBuilder ?? throw new ArgumentNullException(nameof(nodeBuilder));    
            ColumnIndexDict = new Dictionary<string, int>();
        }
        
        public Matrix BuildMatrix(IEnumerable<T> data, IEnumerable<Dimension<T>> dimensions, IEnumerable<Measure<T>> measures, bool displayGrandTotals)
        {
            this.data = data;
            this.measures = measures;
            this.DisplayGrandTotals = displayGrandTotals;
            ColumnIndexDict.Clear();
            validator.Validate(data, dimensions, measures);
            dimensions = validator.SortAndFilterDimensions(dimensions);
            measures = validator.SortAndFilterMeasures(measures);
            Node<T> dataNode = nodeBuilder.Build(data, dimensions, measures, displayGrandTotals);
            Node<T> columnHeaders = nodeBuilder.BuildColumnHeaders(data, dimensions, measures, displayGrandTotals);
            Matrix t = new Matrix();
            BuildColumnHeaders(columnHeaders, t, 0, 0);
            BuildRows(dataNode, t, 0, 0);
            return t;
        }

        private void BuildColumnHeaders(Node<T> node, Matrix t, int index, int peerDepth)
        {
            if (index == 0)
            {
                int totalWidth = GetLeafNodeCount(node, false);
                // 1.) Add an empty cell at 0,0 that spans row headers in width and column headers in height
                // 2.) Add rows to accommodate the rest of the column headers

                // Count the number of expanded column dimensions.  This sets the height of the cell at 0,0.
                // GetHeaderDepth is including the root node which is effectively a placeholder.  We want to add
                // one for row zero which will display sort buttons for column dimensions so we keep the otherwise
                // incorrect value.
                headerHeight = GetHeaderDepth(node, false, 0);
                

                // We cant set the width of the cell at 0,0 because we don't have row data.
                int columnSpan = 1;

                // Add row zero.  Add a cell to that row at 0,0 spanning row headers and column headers.
                MatrixRow row = new MatrixRow();
                row.Cells.Add(new MatrixCell(null, CellType.MeasureHeader, true, headerHeight, columnSpan));
                t.Rows.Add(row);

                // Add a second cell to row zero that is as wide as the number of leaf node columns in node.

                row.Cells.Add(new MatrixCell(null, CellType.MeasureHeader, true, 1, totalWidth));

                // Add remaining rows to display column headers.  We have already added one.
                for (int i = 0; i < headerHeight - 1; i++)
                    t.Rows.Add(new MatrixRow());

                index = 1;  // Don't add any cells to row 0.
            }

            int tmpHeaderDepth = 0;
            
            foreach (Node<T> child in node.Children)
                tmpHeaderDepth = Math.Max(tmpHeaderDepth, GetHeaderDepth(node, false, 0));

            foreach (Node<T> child in node.Children)
                BuildColumnHeaders(child, t, index + 1, tmpHeaderDepth);
           
            int headerDepth = GetHeaderDepth(node, false, 0);
            int rowSpan = 1;
            int colSpan = 1;
            int rowIndex = headerHeight - headerDepth;

            if (node.CellType != CellType.MeasureHeader)
            {
                rowSpan = peerDepth - headerDepth;
                colSpan = GetLeafNodeCount(node, false);
            }
            else
                ColumnIndexDict.Add(node.CellKey, t.Rows[rowIndex].Cells.Count);

            if (rowSpan > 1)
                rowIndex = rowIndex - (rowSpan - 1);

            if(node.CellType != CellType.Root)
                t.Rows[rowIndex].Cells.Add(new MatrixCell(node.Value, node.CellType, node.IsExpanded, rowSpan, colSpan));
        }

        private void BuildRows(Node<T> node, Matrix t, int index, int peerDepth)
        {
            if (index == 0)
            {
                headerWidth = GetHeaderDepth(node, true, 0) - 1;
                t.Rows[0].Cells[0].ColSpan = headerWidth;
                t.Rows.Add(new MatrixRow());
                index = 1;
            }

            int rowIndex = t.Rows.Count - 1;
            int rowSpan = 1;
            int colSpan = 1;

            if (node.IsRow)
            {
                if (node.CellType != CellType.TotalHeader && node.CellType != CellType.GrandTotalHeader) 
                    rowSpan = Math.Max(GetLeafNodeCount(node, true),1);
                else if(node.CellType == CellType.TotalHeader || node.CellType == CellType.GrandTotalHeader)
                    colSpan = headerWidth - peerDepth + 1;

                if (node.CellType != CellType.Root)
                    t.Rows[rowIndex].Cells.Add(new MatrixCell(node.Value, node.CellType, node.IsExpanded, rowSpan, colSpan));
            }
            
            // Render measure cells
            rowSpan = colSpan = 1;
            int colIndex = 0;  // Where the column should be
            int colCount = 0;  // Ordinal position.  If less than colIndex, insert dummy cells
            IEnumerable<Node<T>> columnData = node.Children.Where(x => !x.IsRow);
            
            if (columnData.Any())
            {
                foreach (Node<T> child in columnData)
                {
                    colIndex = ColumnIndexDict[child.CellKey];

                    while (colCount < colIndex)
                    {
                        t.Rows[rowIndex].Cells.Add(new MatrixCell(string.Empty, CellType.Measure, true, rowSpan, colSpan));
                        colCount++;
                    }
                    t.Rows[rowIndex].Cells.Add(new MatrixCell(child.Value, child.CellType, true, rowSpan, colSpan));
                    colCount++;
                }
                
                t.Rows.Add(new MatrixRow());
            }

            foreach (Node<T> child in node.Children.Where(x => x.IsRow))
                BuildRows(child, t, ++index, peerDepth + 1);
        }

        // Finds the dimension (row or column) that has the greatest number of expanded levels.
        private int GetHeaderDepth(Node<T> node, bool checkRows, int maxDepth)
        {
            int tmp = maxDepth + 1;

            foreach (Node<T> child in node.Children.Where(x => (x.IsRow && checkRows || !x.IsRow && !checkRows) )) 
                maxDepth = Math.Max(maxDepth, GetHeaderDepth(child, checkRows, tmp));

            return Math.Max(maxDepth, tmp);
        }

        // Counts the number of leaf nodes at all levels
        private int GetLeafNodeCount(Node<T> node, bool checkRows)
        {
            int count = 0;

            // if the node is not expanded, or has no children of the specified axis it is a leaf node.

            if (node.CellType != CellType.Root && ((checkRows && node.IsRow) || (!checkRows && !node.IsRow))
                && (node.Children.Count(x => (checkRows && x.IsRow) || (!checkRows && !x.IsRow)) == 0))
            {
                count = 1;
            }
            else
            {
                foreach (Node<T> child in node.Children)
                    count += GetLeafNodeCount(child, checkRows);
            }
            
            return count;
        }
    }
}
