﻿/* 
 * Copyright 2021 Leader Analytics 
 * LeaderAnalytics.com
 * SamWheat.com
 * 
 * Please do not remove this header.
 */

namespace LeaderAnalytics.LeaderPivot;

public class MatrixBuilder<T>
{
    protected IEnumerable<T> data;
    protected IEnumerable<Dimension<T>> dimensions;
    protected List<Measure<T>> measures;
    protected bool DisplayGrandTotals;
    protected Node<T> dataNode;
    protected Node<T> columnHeaderNode;
    private int headerHeight;   // Total number of column header rows including one topmost empty row. Also includes measure headers.
    private int headerWidth;    // Total number of row header columns   
    private Dictionary<string, int> ColumnIndexDict;
    private HashSet<string> CollapsedNodeDict;
    private HashSet<int> LeafColumnDict;    // ColumnIndex of every column that is a leaf (not a total).
    private NodeBuilder<T> nodeBuilder;
    private Validator<T> validator;
    private bool fillCollapsedCell;
    private int fillColumnHeaderCount;

    public MatrixBuilder(NodeBuilder<T> nodeBuilder, Validator<T> validator)
    {
        this.validator = validator ?? throw new ArgumentNullException(nameof(validator));
        this.nodeBuilder = nodeBuilder ?? throw new ArgumentNullException(nameof(nodeBuilder));
        ColumnIndexDict = new Dictionary<string, int>();
        CollapsedNodeDict = new HashSet<string>();
        LeafColumnDict = new HashSet<int>();
    }

    public Matrix BuildMatrix(IEnumerable<T> data, IEnumerable<Dimension<T>> dimensions, IEnumerable<Measure<T>> measures, bool displayGrandTotals)
    {
        if (!(data?.Any() ?? false))
            return new Matrix();

        CollapsedNodeDict.Clear();
        validator.Validate(dimensions, measures);
        this.data = data;
        this.DisplayGrandTotals = displayGrandTotals;
        this.dimensions = validator.ValidateDimensions(dimensions);
        this.measures = validator.SortAndFilterMeasures(measures);
        dataNode = nodeBuilder.Build(data, this.dimensions.ToList(), this.measures, displayGrandTotals);
        columnHeaderNode = nodeBuilder.BuildColumnHeaders(data, this.dimensions, this.measures, displayGrandTotals);
        return buildMatrix();
    }

    public Matrix ToggleNodeExpansion(string nodeID)
    {
        if (CollapsedNodeDict.Contains(nodeID))
            CollapsedNodeDict.Remove(nodeID);
        else
            CollapsedNodeDict.Add(nodeID);

        return buildMatrix();
    }

    private Matrix buildMatrix()
    {
        ColumnIndexDict.Clear();
        LeafColumnDict.Clear();
        Matrix t = new Matrix();
        BuildColumnHeaders(columnHeaderNode, t, 0, 0);
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
            row.Cells.Add(new MatrixCell(CellType.MeasureLabel, headerHeight, columnSpan));
            t.Rows.Add(row);

            // Add a second cell to row zero that is as wide as the number of leaf node columns in node.

            row.Cells.Add(new MatrixCell(CellType.MeasureLabel, 1, totalWidth));

            // Add remaining rows to display column headers.  We have already added one.
            for (int i = 0; i < headerHeight - 1; i++)
                t.Rows.Add(new MatrixRow());

            index = 1;  // Don't add any cells to row 0.
        }

        bool isNodeExpanded = IsNodeExpanded(node.ID);
        int headerDepth = GetHeaderDepth(node, false, 0) + (isNodeExpanded ? 0 : 1);
        int rowSpan = 1;
        int colSpan = 1;
        int rowIndex = headerHeight - headerDepth;

        if (node.CellType != CellType.MeasureLabel && node.CellType != CellType.MeasureTotalLabel)
        {
            rowSpan = peerDepth - headerDepth;
            colSpan = isNodeExpanded ? GetLeafNodeCount(node, false) : measures.Count();
        }
        else
            ColumnIndexDict.Add(node.ColumnKey, t.Rows[rowIndex].Cells.Count);

        if (rowSpan > 1)
            rowIndex = rowIndex - (rowSpan - 1);

        if (node.CellType != CellType.Root && !fillCollapsedCell)
        {
            MatrixCell newMatrixCell = new MatrixCell(node, rowSpan, colSpan, isNodeExpanded);

            if (fillColumnHeaderCount > 0)
            {
                newMatrixCell.CellType = CellType.MeasureLabel;
                fillColumnHeaderCount--;
            }
            t.Rows[rowIndex].Cells.Add(newMatrixCell);

            if (newMatrixCell.CellType == CellType.MeasureLabel)
                LeafColumnDict.Add(t.Rows[rowIndex].Cells.Count - 1);
        }
        fillCollapsedCell = !isNodeExpanded && node.CellType == CellType.GroupHeader;

        if (fillCollapsedCell)
            fillColumnHeaderCount = measures.Count();

        if (isNodeExpanded && node.Children is not null)
            foreach (Node<T> child in node.Children)
                BuildColumnHeaders(child, t, index + 1, headerDepth);
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
        bool isRowExpanded = IsNodeExpanded(node.ID);

        if (node.IsRow)
        {
            if (isRowExpanded && node.CellType != CellType.TotalHeader && node.CellType != CellType.GrandTotalHeader)
                rowSpan = Math.Max(GetLeafNodeCount(node, true), 1);
            else if (!isRowExpanded || node.CellType == CellType.TotalHeader || node.CellType == CellType.GrandTotalHeader)
                colSpan = headerWidth - peerDepth + 1;

            // Add the node whether or not it is expanded.  If the node is not expanded
            // the next iteration of the call to BuildRows will be the total (since we will not drill into child nodes)
            // and we just display the total amounts.

            if (node.CellType != CellType.Root && !fillCollapsedCell)
            {
                MatrixCell newMatrixCell = new MatrixCell(node, rowSpan, colSpan, isRowExpanded);

                if (!isRowExpanded)
                    newMatrixCell.CellType = CellType.GroupHeader;

                t.Rows[rowIndex].Cells.Add(newMatrixCell);
            }
        }

        // Render measure cells
        rowSpan = colSpan = 1;
        int colIndex = 0;           // Where the column should be
        int colCount = 0;           // Ordinal position.  If less than colIndex insert dummy cells
        IEnumerable<Node<T>> columnData = node.Children?.Where(x => !x.IsRow);

        if (columnData?.Any() ?? false) // Only the leaf row dimension and totals rows will contain column data.
        {
            int collapsedColumnCount = 0;
            CellType rowCellType = node.CellType == CellType.GrandTotalHeader ? CellType.GrandTotal : (node.CellType == CellType.TotalHeader && !fillCollapsedCell) ? CellType.Total : CellType.Measure;

            foreach (Node<T> child in columnData)
            {
                if (!ColumnIndexDict.TryGetValue(child.ColumnKey, out colIndex))
                {
                    collapsedColumnCount = measures.Count;
                    continue; // Column data will not be found if column is collapsed.
                }

                while (colCount < colIndex)
                {
                    // data values are missing.  Insert dummy cells.
                    CellType missingCellType = rowCellType == CellType.Total || !LeafColumnDict.Contains(colCount) ? CellType.Total : CellType.Measure;
                    t.Rows[rowIndex].Cells.Add(new MatrixCell(missingCellType, rowSpan, colSpan));
                    colCount++;
                }
                MatrixCell newMatrixCell = new MatrixCell(child, rowSpan, colSpan, IsNodeExpanded(child.ID)) { CellType = collapsedColumnCount > 0 || fillCollapsedCell ? rowCellType : child.CellType };

                if (collapsedColumnCount == 0 && !(child.ColumnDimension?.IsLeaf ?? false))
                    newMatrixCell.CellType = CellType.Total;

                t.Rows[rowIndex].Cells.Add(newMatrixCell);
                colCount++;

                if (collapsedColumnCount > 0)
                    collapsedColumnCount--;
            }

            // fill the remainder of the row to total width cells if it has fewer elements 
            while (colCount < (ColumnIndexDict.Count))
            {
                CellType missingCellType = rowCellType == CellType.Total || !LeafColumnDict.Contains(colCount) ? CellType.Total : CellType.Measure;
                t.Rows[rowIndex].Cells.Add(new MatrixCell(missingCellType, rowSpan, colSpan));
                colCount++;
            }

            t.Rows.Add(new MatrixRow());
        }

        fillCollapsedCell = !isRowExpanded && node.CellType == CellType.GroupHeader;

        if (isRowExpanded && node.Children is not null)
            foreach (Node<T> child in node.Children.Where(x => x.IsRow))
                BuildRows(child, t, ++index, peerDepth + 1);
    }

    // Finds the dimension (row or column) that has the greatest number of expanded levels.
    private int GetHeaderDepth(Node<T> node, bool checkRows, int maxDepth)
    {
        int tmp = maxDepth + 1;

        if (IsNodeExpanded(node.ID) && node.Children is not null)
            foreach (Node<T> child in node.Children.Where(x => (checkRows && x.IsRow) || (!checkRows && !x.IsRow)))
                maxDepth = Math.Max(maxDepth, GetHeaderDepth(child, checkRows, tmp));

        return Math.Max(maxDepth, tmp);
    }

    // Counts the number of leaf nodes at all levels
    private int GetLeafNodeCount(Node<T> node, bool checkRows)
    {
        int count = 0;

        // if the node is not expanded, or has no children of the specified axis it is a leaf node.

        if ((node.CellType != CellType.Root) && (!IsNodeExpanded(node.ID) || (node.Children?.Count(x => (checkRows && x.IsRow) || (!checkRows && !x.IsRow)) ?? 0) == 0))
        {
            count = IsNodeExpanded(node.ID) ? 1 : 0; // don't count collapsed rows because we count the totals row.
        }
        else
        {
            if (node.Children is not null)
                foreach (Node<T> child in node.Children.Where(x => ((checkRows && x.IsRow) || (!checkRows && !x.IsRow))))
                    count += GetLeafNodeCount(child, checkRows);
        }

        return count;
    }

    private bool IsNodeExpanded(string nodeID) => !CollapsedNodeDict.Contains(nodeID);

}
