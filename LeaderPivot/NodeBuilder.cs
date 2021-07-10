/* 
 * Copyright 2021 Leader Analytics 
 * LeaderAnalytics.com
 * SamWheat.com
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;


namespace LeaderAnalytics.LeaderPivot
{
    public class NodeBuilder<T>
    {
        private NodeCache<T> nodeCache;
        private bool displayGrandTotals;
        private bool buildHeaders;
        private List<string> NodeIDs;
        private List<string> ColumnKeys;

        public NodeBuilder(NodeCache<T> nodeCache)
        {
            this.nodeCache = nodeCache;
            NodeIDs = new List<string>();
            ColumnKeys = new List<string>();
        }

        public Node<T> Build(IEnumerable<T> data, IEnumerable<Dimension<T>> dimensions, IEnumerable<Measure<T>> measures, bool displayGrandTotals)
        {
            this.displayGrandTotals = displayGrandTotals;
            return BuildNodes(new Node<T>("Root", dimensions.First(x => x.IsRow)) { IsExpanded = true }, data, dimensions, measures, 0);
        }

        public Node<T> BuildColumnHeaders(IEnumerable<T> data, IEnumerable<Dimension<T>> dimensions, IEnumerable<Measure<T>> measures, bool displayGrandTotals)
        {
            this.displayGrandTotals = displayGrandTotals;
            buildHeaders = true;
            return BuildNodes(new Node<T>("Root", dimensions.First(x => !x.IsRow)) { IsExpanded = true }, data, dimensions.Where(x => ! x.IsRow), measures, 0);
        }

        private Node<T> BuildNodes(Node<T> parent, IEnumerable<T> data, IEnumerable<Dimension<T>> dimensions, IEnumerable<Measure<T>> measures, int depthIndex)
        {
            var dimension = dimensions.First();
            var childDimensions = dimensions.Skip(1);
            bool isLeafNode = !childDimensions.Any();
            var groups = dimension.IsAscending ? data.GroupBy(dimension.GroupValue).OrderBy(x => SortValue(dimension, x.Key)) : data.GroupBy(dimension.GroupValue).OrderByDescending(x => SortValue(dimension, x.Key));
            

            foreach (var grp in groups)
            {
                string grpKeyValue = grp.Key.ToString();
                
                if(!dimension.IsRow)
                    SetColumnKey(dimension.Sequence, grpKeyValue);

                SetNodeID(depthIndex, grpKeyValue);
                Node<T> child = (dimension.IsRow || buildHeaders) ? null : parent;
                string nodeID = GetNodeID(depthIndex);
                string columnKey = GetColumnKey(depthIndex);

                if (child == null)
                {
                    // Child will be null if dimension is a row.
                    child = nodeCache.Get(nodeID, dimension, CellType.GroupHeader, columnKey, DisplayValue(dimension, grp.First()), dimension.IsRow, dimension.IsExpanded);
                    parent.Children.Add(child);
                }

                if (!isLeafNode)  // isLeafNode and dimension.IsRow cannot both be true at the same time.
                    BuildNodes(child, grp, childDimensions, measures, ++depthIndex);

                if ((buildHeaders && isLeafNode) || (!buildHeaders && !childDimensions.Any(x => x.IsRow)))
                {
                    // Create measures on leaf nodes

                    if (buildHeaders)
                        CreateMeasureHeaders(child, measures, nodeID, columnKey);
                    else
                    {
                        CellType cellType = (isLeafNode && child.CellType == CellType.GroupHeader) ? CellType.Measure : (dimension.IsRow || child.CellType == CellType.GrandTotalHeader) ? CellType.GrandTotal : CellType.Total;
                        CreateMeasures(child, measures, dimension, grp, nodeID, columnKey, cellType); // Build column measures, totals, and grand totals
                    }
                }
                else if (dimension.IsRow || buildHeaders)
                    CreateTotals(parent, measures, dimensions, grp, nodeID, columnKey, CellType.TotalHeader, depthIndex);
            }

            if (parent.CellType == CellType.Root && displayGrandTotals)
                CreateTotals(parent, measures, dimensions, data, GetNodeID(depthIndex) + "GrandTotal", GetColumnKey(depthIndex) , CellType.GrandTotalHeader, depthIndex);  // need oldColumnKey here

            return parent;
        }

        private void CreateTotals(Node<T> parentNode, IEnumerable<Measure<T>> measures, IEnumerable<Dimension<T>> dimensions, IEnumerable<T> grp, string nodeID, string columnKey, CellType cellType, int depthIndex)
        {
            // Total rows are always expanded
            Dimension<T> dimension = dimensions.First();
            object val = (cellType == CellType.TotalHeader ? DisplayValue(dimension, grp.First()) : "Grand") + " Total";
            
            
            
            columnKey = columnKey + val;




            Node<T> total = nodeCache.Get(nodeID + cellType.ToString(), dimension, cellType, columnKey, val, dimension.IsRow, true);
            parentNode.Children.Add(total);

            if (buildHeaders)
                CreateMeasureHeaders(total, measures, nodeID, columnKey);
            else
            {
                BuildNodes(total, grp, dimensions.Skip(1).Where(x => !x.IsRow), measures, ++depthIndex);
                CreateMeasures(total, measures, dimension, grp, nodeID, columnKey, CellType.GrandTotal);
            }
        }

        private void CreateMeasures(Node<T> parentNode, IEnumerable<Measure<T>> measures, Dimension<T> dimension, IEnumerable<T> grp, string nodeID, string columnKey, CellType cellType)
        {
            // Measure are always leaf node columns and are always expanded.
            foreach (Measure<T> measure in measures)
            {
                object val  = string.IsNullOrEmpty(measure.Format) ? measure.Aggragate(grp) : String.Format(measure.Format, measure.Aggragate(grp));
                Node<T> measureChild = nodeCache.Get(nodeID + measure.DisplayValue , dimension, cellType, columnKey + measure.DisplayValue, val, false, true);
                parentNode.Children.Add(measureChild);
            }
        }

        private void CreateMeasureHeaders(Node<T> parentNode, IEnumerable<Measure<T>> measures, string nodeID, string columnKey)
        {
            // Measure headers are always expanded and are always displayed as column headers - never as row headers.
            foreach (Measure<T> measure in measures)
            {
                Node<T> measureHeader = nodeCache.Get(nodeID + measure.DisplayValue + "Header", parentNode.Dimension, CellType.MeasureHeader, columnKey + measure.DisplayValue, measure.DisplayValue, false, true);
                parentNode.Children.Add(measureHeader);
            }
        }

        private string DisplayValue(Dimension<T> dimension, T data) => dimension.HeaderValue == null ? dimension.GroupValue(data) : dimension.HeaderValue(data);

        private string SortValue(Dimension<T> dimension, string data) => dimension.SortValue == null ? data : dimension.SortValue(data);
        private string GetNodeID(int depthIndex) => String.Join('/', NodeIDs.Take(depthIndex + 1).ToArray());

        private string GetColumnKey(int depthIndex) => String.Join('/', ColumnKeys.Take(depthIndex + 1).ToArray());
        
        private void SetNodeID(int depthIndex, string value)
        {
            if (NodeIDs.Count == depthIndex)
                NodeIDs.Add(value);
            else
                NodeIDs[depthIndex] = value;
        }

        private void SetColumnKey(int depthIndex, string value)
        {
            if (ColumnKeys.Count == depthIndex)
                ColumnKeys.Add(value);
            else
                ColumnKeys[depthIndex] = value;
        }
    }
}
