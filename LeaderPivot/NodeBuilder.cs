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
    public class NodeBuilder<T>
    {
        public Node<T> Build(IEnumerable<T> data, IEnumerable<Dimension<T>> dimensions, IEnumerable<Measure<T>> measures, bool displayGrandTotals)
        {
            Validate(data, ref dimensions, ref measures, false);
            return BuildNodes(new Node<T>(), data, dimensions, measures, false, displayGrandTotals, null);
        }

        public Node<T> BuildColumnHeaders(IEnumerable<T> data, IEnumerable<Dimension<T>> dimensions, IEnumerable<Measure<T>> measures, bool displayGrandTotals)
        {
            Validate(data, ref dimensions, ref measures, true);
            return BuildNodes(new Node<T>(), data, dimensions.Where(x => ! x.IsRow), measures, true, displayGrandTotals, null);
        }

        private Node<T> BuildNodes(Node<T> parent, IEnumerable<T> data, IEnumerable<Dimension<T>> dimensions, IEnumerable<Measure<T>> measures, bool buildHeaders, bool displayGrandTotals, string rootPath)
        {
            var dimension = dimensions.First();
            var childDimensions = dimensions.Skip(1);
            bool isLeafNode = !childDimensions.Any();
            parent.IsExpanded = dimension.IsExpanded;

            if (rootPath is null)
                rootPath = string.Empty;

            string path = rootPath;

            if (!dimension.IsExpanded)
                return parent; // Nothing to do.  Display a sum.

            var groups = dimension.IsAscending ? data.GroupBy(dimension.GroupValue).OrderBy(x => SortValue(dimension, x.Key)) : data.GroupBy(dimension.GroupValue).OrderByDescending(x => SortValue(dimension, x.Key));

            foreach (var grp in groups)
            {
                string grpKeyValue = grp.Key.ToString();
                path = dimension.IsRow ? rootPath : rootPath + grpKeyValue;
                Node<T> child = (dimension.IsRow || buildHeaders) ? null : parent;

                if (child == null)
                {
                    // Child will be null if dimension is a row.
                    CellType childCellType = CellType.GroupHeader;
                    child = new Node<T> { IsExpanded = dimension.IsExpanded, IsRow = dimension.IsRow, IsLeafNode = isLeafNode, Value = DisplayValue(dimension, grp.First()), CellType = childCellType };
                    parent.Children.Add(child);
                }

                if (!isLeafNode)  // isLeafNode and dimension.IsRow cannot both be true at the same time.
                    BuildNodes(child, grp, childDimensions, measures, buildHeaders, displayGrandTotals, path);

                if ((buildHeaders && isLeafNode) || (!buildHeaders && !childDimensions.Any(x => x.IsRow)))
                {
                    // Create measures on leaf nodes

                    if (buildHeaders)
                        CreateMeasureHeaders(child, measures, path);
                    else
                    {
                        CellType cellType = (isLeafNode && child.CellType == CellType.GroupHeader) ? CellType.Measure : child.CellType == CellType.GrandTotalHeader ? CellType.GrandTotal : CellType.Total;
                        CreateMeasures(child, measures, dimension, grp, path, cellType); // Build Column measures, totals and grand totals
                    }
                }
                else if (dimension.IsRow || buildHeaders)
                    CreateTotals(parent, measures, dimensions, grp, path, buildHeaders, displayGrandTotals, CellType.TotalHeader);
            }

            if (parent.CellType == CellType.Root && displayGrandTotals)
                CreateTotals(parent, measures, dimensions, data, rootPath, buildHeaders, displayGrandTotals, CellType.GrandTotalHeader);

            return parent;
        }

        private void CreateTotals(Node<T> parentNode, IEnumerable<Measure<T>> measures, IEnumerable<Dimension<T>> dimensions, IEnumerable<T> grp, string columnKey, bool buildHeaders, bool displayGrandTotals, CellType cellType)
        {
            Dimension<T> dimension = dimensions.First();
            Node<T> total = new Node<T> { IsExpanded = dimension.IsExpanded, IsRow = dimension.IsRow, IsLeafNode = false, CellType = cellType };
            total.Value = (cellType == CellType.TotalHeader ? DisplayValue(dimension, grp.First()) : "Grand") + " Total";
            parentNode.Children.Add(total);

            if (buildHeaders)
                CreateMeasureHeaders(total, measures, columnKey);
            else
            {
                BuildNodes(total, grp, dimensions.Skip(1).Where(x => !x.IsRow), measures, buildHeaders, displayGrandTotals, columnKey);
                CreateMeasures(total, measures, dimension, grp, columnKey, CellType.GrandTotal);
            }
        }

        private void CreateMeasures(Node<T> parentNode, IEnumerable<Measure<T>> measures, Dimension<T> dimension, IEnumerable<T> grp, string columnKey, CellType cellType)
        {
            // Measure are always leaf node columns.
            foreach (Measure<T> measure in measures)
            {
                decimal value = measure.Aggragate(grp);
                Node<T> measureChild = new Node<T> { IsExpanded = dimension.IsExpanded, IsRow = false, IsLeafNode = true, CellKey = columnKey + measure.DisplayValue, CellType = cellType };
                measureChild.Value = string.IsNullOrEmpty(measure.Format) ? value : String.Format(measure.Format, value);
                parentNode.Children.Add(measureChild);
            }
        }

        private void CreateMeasureHeaders(Node<T> parentNode, IEnumerable<Measure<T>> measures, string columnKey)
        {
            // Measure headers are always expanded and are always displayed as column headers - never as row headers.
            foreach (Measure<T> measure in measures)
            {
                Node<T> measureHeader = new Node<T> { IsExpanded = true, IsRow = false, IsLeafNode = true, Value = measure.DisplayValue, CellKey = columnKey + measure.DisplayValue, CellType = CellType.MeasureHeader };
                parentNode.Children.Add(measureHeader);
            }
        }

        private string DisplayValue(Dimension<T> dimension, T data) => dimension.HeaderValue == null ? dimension.GroupValue(data) : dimension.HeaderValue(data);

        private string SortValue(Dimension<T> dimension, string data) => dimension.SortValue == null ? data : dimension.SortValue(data);

        private void Validate(IEnumerable<T> data, ref IEnumerable<Dimension<T>> dimensions, ref IEnumerable<Measure<T>> measures, bool buildingHeaders)
        {

            if (!(data?.Any() ?? false))
                throw new ArgumentNullException(nameof(data) + " cannot be null and must contain at least one element.");

            if (!(dimensions?.Any() ?? false))
                throw new ArgumentNullException(nameof(dimensions) + " cannot be null and must contain at least one element.");

            if (!(measures?.Any() ?? false))
                throw new ArgumentNullException(nameof(measures) + " cannot be null and must contain at least one element.");

            if (!dimensions.Any(x => !x.IsRow))
                throw new Exception($"At least one column dimension is required.  One or more dimensions with {nameof(Dimension<T>.IsRow)} property set to false are required.");

            if (buildingHeaders && !dimensions.Any(x => x.IsRow))
                throw new Exception($"At least one row dimension is required.  One or more dimensions with {nameof(Dimension<T>.IsRow)} property set to true are required.");

            if(dimensions.Any(x => string.IsNullOrEmpty(x.DisplayValue)))
                throw new Exception("DisplayValue property for each Dimension is required.");

            if (measures.Any(x => string.IsNullOrEmpty(x.DisplayValue)))
                throw new Exception("DisplayValue property for each Measure is required.");

            if (dimensions.GroupBy(x => x.DisplayValue).Any(x => x.Count() > 1))
                throw new Exception("DisplayValue property for each dimension must be unique.");

            if (measures.GroupBy(x => x.DisplayValue).Any(x => x.Count() > 1))
                throw new Exception("DisplayValue property for each measure must be unique.");

            dimensions = dimensions.OrderBy(x => !x.IsRow).ThenBy(x => x.Sequence).ToList();
            measures = measures.OrderBy(x => x.Sequence).ToList();
        }
    }
}
