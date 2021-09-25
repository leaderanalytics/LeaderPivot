﻿/* 
 * Copyright 2021 Leader Analytics 
 * LeaderAnalytics.com
 * SamWheat.com
 * 
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

        public NodeBuilder()
        {
        }

        public Node<T> Build(IEnumerable<T> data, List<Dimension<T>> dimensions, List<Measure<T>> measures)
        {
            Measures = measures;
            MeasureDatas = new MeasureData<T>[dimensions.Count];
            Node<T> root = new Node<T>(Guid.NewGuid().ToString(),null, null, null, CellType.Root);
            BuildNodes(root, dimensions, data);
            return root;
        }

        public Node<T> BuildColumnHeaders(IEnumerable<T> data, IEnumerable<Dimension<T>> dimensions, IEnumerable<Measure<T>> measures)
        {
            buildHeaders = true;
            Node<T> root = new Node<T>(Guid.NewGuid().ToString(), null, null, null, CellType.Root);
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

                if (dim.IsRow || buildHeaders) // if (dim.IsRow || dim.IsLeaf)
                {
                    string nodeID = Node.CreateID(dim.ID, grp.Key);
                    node = new Node<T>(nodeID, rowDim, colDim, DisplayValue(dim, grp.First()), CellType.GroupHeader);
                    node.IsRow = dim.IsRow;
                    parent.AddChild(node);
                }
                else
                {
                    node = parent;
                    node.RowDimension = rowDim;
                    node.ColumnDimension = colDim;
                }

                if (! (!dim.IsRow && dim.IsLeaf)) // Cannot be both column and leaf since there are no more dimensions.
                {
                    BuildNodes(node, dimensions.Skip(1).ToList(), grp);

                    if (dim.IsRow || buildHeaders) 
                    {
                        MeasureData<T> measureData = null;

                        if (!dim.IsLeaf)
                        {
                            object nodeVal = DisplayValue(dim, grp.First()) + " Total";
                            string nodeID = Node.CreateID(dim.ID, nodeVal.ToString());
                            node = new Node<T>(nodeID, dim, node.ColumnDimension, nodeVal, CellType.TotalHeader);
                            parent.AddChild(node);
                            
                            if(!buildHeaders)
                                BuildNodes(node, dimensions.Where(x => !x.IsRow).ToList(), grp);
                        }
                        
                        if(!buildHeaders)
                        {
                            measureData = BuildMeasureData(node, grp, data, dim); // Append Grand Totals column to each row
                            BuildMeasures(node, parent, measureData, CellType.GrandTotal);
                        }
                    }
                }

                if (!dim.IsRow)
                {
                    if (buildHeaders)
                        CreateMeasureLabels(node);
                    else
                    {
                        CellType cellType = parent.CellType == CellType.GroupHeader ? CellType.Measure : parent.CellType == CellType.TotalHeader ? CellType.Total : CellType.GrandTotal;
                        MeasureData<T> measureData = BuildMeasureData(node, grp, data, dim);
                        BuildMeasures(parent, node, measureData, cellType);
                    }
                }
            }

            if (parent.CellType == CellType.Root)
            {
                string nodeID = Node.CreateID(dim.ID, Guid.NewGuid().ToString());
                node = new Node<T>(nodeID, dim, node.ColumnDimension, "Grand Total", CellType.GrandTotalHeader);
                parent.AddChild(node);

                if (buildHeaders)
                    CreateMeasureLabels(node);
                else
                {
                    BuildNodes(node, dimensions.Where(x => !x.IsRow).ToList(), data);
                    MeasureData<T> measureData = BuildMeasureData(node, data, data, dim); // Grand Totals column 
                    BuildMeasures(node, parent, measureData, CellType.GrandTotal);
                }
            }
        }

        private void BuildMeasures(Node<T> parentRow, Node<T> parentColumn,  MeasureData<T> measureData, CellType cellType)
        {
            // Measure are always leaf node columns and are always expanded.

            foreach (Measure<T> measure in Measures)
            {
                object val = string.IsNullOrEmpty(measure.Format) ? measure.Aggragate(measureData) : String.Format(measure.Format, measure.Aggragate(measureData));
                Node<T> child = new Node<T>(parentRow?.ID + parentColumn?.ID + $"[{measure.DisplayValue}]", parentRow?.RowDimension, parentColumn?.ColumnDimension, val, cellType);
                parentRow.AddChild(child);
            }
        }

        private void CreateMeasureLabels(Node<T> parent)
        {
            CellType cellType = parent.CellType == CellType.GroupHeader ? CellType.MeasureLabel :  CellType.MeasureTotalLabel;

            foreach (Measure<T> measure in Measures)
            {
                Node<T> labelNode = new Node<T>(parent.ID + "Label", parent.RowDimension, parent.ColumnDimension, measure.DisplayValue, cellType);
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
                    lastMeasureData = MeasureDatas[node.RowDimension.Ordinal]; // lastMeasureData = MeasureDatas[node.Dimension.Ordinal];

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
    }

}
