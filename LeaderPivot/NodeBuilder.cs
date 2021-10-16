/* 
 * Copyright 2021 Leader Analytics 
 * LeaderAnalytics.com
 * SamWheat.com
 *  
 * Please do not remove this header.
 */

using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;


namespace LeaderAnalytics.LeaderPivot
{
    public class NodeBuilder<T>
    {
        private List<Measure<T>> Measures { get; set; }
        private MeasureData<T>[] MeasureDatas;
        private bool buildHeaders;
        private string grandTotalColumnID = Guid.NewGuid().ToString();
        private bool displayGrandTotals;

        public NodeBuilder()
        {
        }

        public Node<T> Build(IEnumerable<T> data, List<Dimension<T>> dimensions, List<Measure<T>> measures, bool displayGrandTotals)
        {
            buildHeaders = false;
            Measures = measures;
            MeasureDatas = new MeasureData<T>[dimensions.Count];
            this.displayGrandTotals = displayGrandTotals;
            Node<T> root = new Node<T>(null, null, null, CellType.Root);
            BuildNodes(root, dimensions, data);
            return root;
        }

        public Node<T> BuildColumnHeaders(IEnumerable<T> data, IEnumerable<Dimension<T>> dimensions, IEnumerable<Measure<T>> measures, bool displayGrandTotals)
        {
            buildHeaders = true;
            Node<T> root = new Node<T>(null, null, null, CellType.Root);
            this.displayGrandTotals = displayGrandTotals;
            BuildNodes(root, dimensions.Where(x => !x.IsRow).ToList(), data);
            return root;
        }

        private void BuildNodes(Node<T> parent, List<Dimension<T>> dimensions, IEnumerable<T> data)
        {
            if (!(dimensions?.Any() ?? false))
                return;

            Dimension<T> dim = dimensions.First();
            var groups = dim.IsAscending ? data.GroupBy(dim.GroupValue).OrderBy(x => SortValue(dim, x.Key)) : data.GroupBy(dim.GroupValue).OrderByDescending(x => SortValue(dim, x.Key));
            Node<T> node = null;

            foreach (var grp in groups)
            {
                Dimension<T> rowDim = dim.IsRow ? dim : parent.RowDimension;
                Dimension<T> colDim = dim.IsRow ? parent.ColumnDimension : dim;

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
                MeasureData<T> measureData = null;

                if (!buildHeaders)
                    measureData = BuildMeasureData(node, grp, data, dim);

                if (! (!dim.IsRow && dim.IsLeaf)) // Cannot be both column and leaf since there are no more dimensions.
                {
                    BuildNodes(node, dimensions.Skip(1).ToList(), grp);

                    if (dim.IsRow || buildHeaders) 
                    {
                        if (!dim.IsLeaf)
                        {
                            object nodeVal = DisplayValue(dim, grp.First()) + " Total";
                            node = new Node<T>(buildHeaders ? null : dim, node.ColumnDimension, nodeVal, CellType.TotalHeader);
                            node.IsRow = dim.IsRow;
                            parent.AddChild(node);
                            
                            if(!buildHeaders)
                                BuildNodes(node, dimensions.Where(x => !x.IsRow).ToList(), grp);
                        }
                        
                        if(displayGrandTotals && !buildHeaders)
                            BuildMeasures(node, null, measureData, CellType.GrandTotal, "GrandTotal");
                    }
                }

                if (!dim.IsRow)
                {
                    if (buildHeaders)
                        BuildMeasureLabels(node, grp.Key);
                    else
                    {
                        CellType cellType = parent.CellType == CellType.GrandTotalHeader ? CellType.GrandTotal : parent.RowDimension.IsLeaf && dim.IsLeaf ? CellType.Measure : CellType.Total;
                        BuildMeasures(parent, dim, measureData, cellType, grp.Key); // Add measures for total rows
                    }
                }
            }

            if (displayGrandTotals && parent.CellType == CellType.Root)
            {
                node = new Node<T>(dim, node.ColumnDimension, "Grand Total", CellType.GrandTotalHeader);
                node.IsRow = ! buildHeaders;
                parent.AddChild(node);

                if (buildHeaders)
                    BuildMeasureLabels(node, "GrandTotal");
                else
                {
                    BuildNodes(node, dimensions.Where(x => !x.IsRow).ToList(), data);
                    MeasureData<T> measureData = BuildMeasureData(node, data, data, dim); // Grand Totals column 
                    BuildMeasures(node, null, measureData, CellType.GrandTotal, "GrandTotal");
                }
            }
        }

        private void BuildMeasures(Node<T> parent, Dimension<T> columnDim,  MeasureData<T> measureData, CellType cellType, string cellKey)
        {
            // Measure are always leaf node columns and are always expanded.

            foreach (Measure<T> measure in Measures)
            {
                string nodeCellKey = string.IsNullOrEmpty(cellKey) ? null : CreateCellKey(columnDim?.ID ?? grandTotalColumnID, cellKey, measure.DisplayValue);
                object val = string.IsNullOrEmpty(measure.Format) ? measure.Aggragate(measureData) : String.Format(measure.Format, measure.Aggragate(measureData));
                Node<T> child = new Node<T>(parent?.RowDimension, columnDim, val, cellType, nodeCellKey);
                parent.AddChild(child);
            }
        }

        private void BuildMeasureLabels(Node<T> parent, string columnGroupValue)
        {
            CellType cellType = parent.CellType == CellType.GroupHeader ? CellType.MeasureLabel :  CellType.MeasureTotalLabel;
            string dimensionID = parent.CellType == CellType.GrandTotalHeader ? grandTotalColumnID : parent.ColumnDimension.ID;

            foreach (Measure<T> measure in Measures)
            {
                string columnKey =  CreateCellKey(dimensionID, columnGroupValue, measure.DisplayValue);
                Node<T> labelNode = new Node<T>(parent.RowDimension, parent.ColumnDimension, measure.DisplayValue, cellType, columnKey);
                parent.AddChild(labelNode);
            }
        }

        private MeasureData<T> BuildMeasureData(Node<T> node, IEnumerable<T> measure, IEnumerable<T> group, Dimension<T> dimension)
        {
            MeasureData<T> lastMeasureData = null;
            IEnumerable<T> lastColumnGroup = null;
            IEnumerable<T> lastRowGroup = null;
            Dimension<T> lastRowDimension = null;
            Dimension<T> lastColumnDimension = null;


            if (dimension.IsRow)
            {
                lastRowGroup = group;
                lastRowDimension = dimension;
            }
            else
            {
                var measureGroup = measure as IGrouping<String, T>;

                if (measureGroup != null)
                    lastMeasureData = MeasureDatas[node.RowDimension.Ordinal]; 

                lastColumnGroup = group;
                lastColumnDimension = dimension;
                lastRowGroup = lastMeasureData?.RowGroup?.GroupBy(dimension.GroupValue).Where(x => x.Key == measureGroup.Key).FirstOrDefault();
                lastRowDimension = lastMeasureData?.RowDimension;
            }

            MeasureData<T> measureData = new MeasureData<T>(measure, lastRowGroup, lastColumnGroup, lastRowDimension, lastColumnDimension);
            MeasureDatas[dimension.Ordinal] = measureData;
            return measureData;
        }

        private string SortValue(Dimension<T> dimension, string data) => dimension.SortValue == null ? data : dimension.SortValue(data);

        private string DisplayValue(Dimension<T> dimension, T data) => dimension.HeaderValue == null ? dimension.GroupValue(data) : dimension.HeaderValue(data);

        private string CreateCellKey(string dimensionID, string groupValue, string measureID) => $"[{dimensionID}:{groupValue}][{measureID}]";
    }

}
