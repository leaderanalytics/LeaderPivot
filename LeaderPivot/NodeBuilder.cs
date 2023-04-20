/* 
 * Copyright 2021 Leader Analytics 
 * LeaderAnalytics.com
 * SamWheat.com
 *  
 * Please do not remove this header.
 */

namespace LeaderAnalytics.LeaderPivot;

public class NodeBuilder<T> : INodeBuilder<T>
{
    private List<IMeasureT<T>> Measures { get; set; }
    private bool buildHeaders;
    private string grandTotalColumnID = Guid.NewGuid().ToString();
    private int grandTotalColumnSeq;
    private bool displayGrandTotals;
    private ColumnIDGraph ColumnIDGraph;

    public INodeT<T> Build(IEnumerable<T> data, List<IDimensionT<T>> dimensions, List<IMeasureT<T>> measures, bool displayGrandTotals)
    {
        buildHeaders = false;
        return BuildInternal(data, dimensions, measures, displayGrandTotals);
    }

    public INodeT<T> BuildColumnHeaders(IEnumerable<T> data, IEnumerable<IDimensionT<T>> dimensions, IEnumerable<IMeasureT<T>> measures, bool displayGrandTotals)
    {
        buildHeaders = true;
        return BuildInternal(data, dimensions.Where(x => !x.IsRow).ToList(), measures.ToList(), displayGrandTotals);
    }

    private INodeT<T> BuildInternal(IEnumerable<T> data, List<IDimensionT<T>> dimensions, List<IMeasureT<T>> measures, bool displayGrandTotals)
    {
        int rowDimCount = dimensions.Where(x => !x.IsRow).Count();
        INodeT<T> root = new Node<T>(null, null, null, CellType.Root);
        Measures = measures;
        this.displayGrandTotals = displayGrandTotals;
        ColumnIDGraph = new ColumnIDGraph(rowDimCount + 1);
        grandTotalColumnSeq = rowDimCount;
        BuildNodes(root, dimensions, data, new MeasureData<T>(null,data,null));
        return root;
    }

    private void BuildNodes(INodeT<T> parent, List<IDimensionT<T>> dimensions, IEnumerable<T> data, IMeasureData<T> measureData)
    {
        if (!(dimensions?.Any() ?? false))
            return;

        IDimensionT<T> dim = dimensions.First();
        var groups = dim.IsAscending ? data.GroupBy(dim.GroupValue).OrderBy(x => SortValue(dim, x.Key)) : data.GroupBy(dim.GroupValue).OrderByDescending(x => SortValue(dim, x.Key));
        INodeT<T> node = null;

        foreach (var grp in groups)
        {
            IDimensionT<T> rowDim = dim.IsRow ? dim : parent.RowDimension;
            IDimensionT<T> colDim = dim.IsRow ? parent.ColumnDimension : dim;

            if (dim.IsRow || buildHeaders)
            {
                node = new Node<T>(rowDim, colDim, DisplayValue(dim, grp.First()), CellType.GroupHeader);
                node.IsRow = dim.IsRow;
                node.CanToggleExapansion = !(node.IsRow ? node.RowDimension.IsLeaf : node.ColumnDimension.IsLeaf);
                parent.AddChild(node);
            }
            else
            {
                node = parent;
                node.RowDimension = rowDim;
                node.ColumnDimension = colDim;
            }

            if (!dim.IsRow && !dim.IsLeaf)
                ColumnIDGraph.SetColumnID(dim.Sequence, dim.ID, grp.Key);

            IMeasureData<T> localMeasureData = BuildMeasureData(measureData, node, grp, data, dim);

            if (!(!dim.IsRow && dim.IsLeaf)) // Cannot be both column and leaf since there are no more dimensions.
            {
                BuildNodes(node, dimensions.Skip(1).ToList(), grp, localMeasureData);

                if (dim.IsRow || buildHeaders)
                {
                    if (!dim.IsLeaf)
                    {
                        object nodeVal = DisplayValue(dim, grp.First()) + " Total";
                        node = new Node<T>(buildHeaders ? null : dim, node.ColumnDimension, nodeVal, CellType.TotalHeader);
                        node.IsRow = dim.IsRow;
                        parent.AddChild(node);

                        if (!buildHeaders)
                            BuildNodes(node, dimensions.Where(x => !x.IsRow).ToList(), grp, localMeasureData);
                    }

                    if (displayGrandTotals && !buildHeaders)
                    {
                        localMeasureData.ColumnGroup = grp;  // make percent of column calculations show up as 100%
                        BuildMeasures(node, null, localMeasureData, CellType.GrandTotal, "GrandTotal");
                    }
                }
            }

            if (!dim.IsRow)
            {
                if (buildHeaders)
                    BuildMeasureLabels(node, grp.Key);
                else
                {
                    CellType cellType = parent.CellType == CellType.GrandTotalHeader ? CellType.GrandTotal : parent.RowDimension.IsLeaf && dim.IsLeaf ? CellType.Measure : CellType.Total;
                    BuildMeasures(parent, dim, localMeasureData, cellType, grp.Key); // Add measures for total rows
                }
            }
        }

        if (displayGrandTotals && parent.CellType == CellType.Root)
        {
            node = new Node<T>(dim, node.ColumnDimension, "Grand Total", CellType.GrandTotalHeader);
            node.IsRow = !buildHeaders;
            parent.AddChild(node);

            if (buildHeaders)
                BuildMeasureLabels(node, "GrandTotal");
            else
            {
                IMeasureData<T> localMeasureData = BuildMeasureData(measureData, node, data, data, dim);
                BuildNodes(node, dimensions.Where(x => !x.IsRow).ToList(), data, localMeasureData);
                BuildMeasures(node, null, localMeasureData, CellType.GrandTotal, "GrandTotal");
            }
        }
    }

    private void BuildMeasures(INodeT<T> parent, IDimensionT<T> columnDim, IMeasureData<T> measureData, CellType cellType, string cellKey)
    {
        // Measure are always leaf node columns and are always expanded.
        foreach (IMeasureT<T> measure in Measures)
        {
            ColumnIDGraph.SetColumnID(columnDim?.Sequence ?? grandTotalColumnSeq, columnDim?.ID ?? grandTotalColumnID, cellKey, measure.DisplayValue);
            object val = string.IsNullOrEmpty(measure.Format) ? measure.Aggragate(measureData) : String.Format(measure.Format, measure.Aggragate(measureData));
            INodeT<T> child = new Node<T>(parent?.RowDimension, columnDim, val, cellType, ColumnIDGraph.GetColumnIDGraph());
            parent.AddChild(child);
        }
        ColumnIDGraph.ClearColumnID(columnDim?.Sequence ?? grandTotalColumnSeq);
    }

    private void BuildMeasureLabels(INodeT<T> parent, string columnGroupValue)
    {
        CellType cellType = parent.CellType == CellType.GroupHeader ? CellType.MeasureLabel : CellType.MeasureTotalLabel;
        string dimensionID = parent.CellType == CellType.GrandTotalHeader ? grandTotalColumnID : parent.ColumnDimension.ID;
        int dimensionSeq = parent.CellType == CellType.GrandTotalHeader ? grandTotalColumnSeq : parent.ColumnDimension.Sequence;

        foreach (IMeasureT<T> measure in Measures)
        {
            ColumnIDGraph.SetColumnID(dimensionSeq, dimensionID, columnGroupValue, measure.DisplayValue);
            INodeT<T> labelNode = new Node<T>(parent.RowDimension, parent.ColumnDimension, measure.DisplayValue, cellType, ColumnIDGraph.GetColumnIDGraph());
            parent.AddChild(labelNode);
        }
        ColumnIDGraph.ClearColumnID(parent.ColumnDimension.Sequence);
    }

    private IMeasureData<T> BuildMeasureData(IMeasureData<T> measureData, INodeT<T> node, IEnumerable<T> measure, IEnumerable<T> data, IDimensionT<T> dimension)
    {
        IEnumerable<T> lastRowGroup = null;

        if (dimension.IsRow )
            lastRowGroup = data;
        else
        {
            var measureGroup = measure as IGrouping<String, T>;
            lastRowGroup = measureData.RowGroup.GroupBy(dimension.GroupValue).Where(x => x.Key == measureGroup.Key).FirstOrDefault() ?? Enumerable.Empty<T>();
        }
        IMeasureData<T> localMeasureData = new MeasureData<T>(measure, lastRowGroup, data);
        return localMeasureData;
    }

    private string SortValue(IDimensionT<T> dimension, string data) => dimension.SortValue == null ? data : dimension.SortValue(data);

    private string DisplayValue(IDimensionT<T> dimension, T data) => dimension.HeaderValue == null ? dimension.GroupValue(data) : dimension.HeaderValue(data);
}
