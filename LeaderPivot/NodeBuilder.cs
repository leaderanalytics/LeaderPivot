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
            Validate(data, dimensions, measures, false);
            return BuildNodes(new Node<T>(), data, dimensions, measures, false, displayGrandTotals, null);
        }

        public Node<T> BuildColumnHeaders(IEnumerable<T> data, IEnumerable<Dimension<T>> dimensions, IEnumerable<Measure<T>> measures, bool displayGrandTotals)
        {
            Validate(data, dimensions, measures, true);
            return BuildNodes(new Node<T>(), data, dimensions.Where(x => ! x.IsRow), measures, true, displayGrandTotals, null);
        }

        private Node<T> BuildNodes(Node<T> parent, IEnumerable<T> data, IEnumerable<Dimension<T>> dimensions, IEnumerable<Measure<T>> measures, bool buildHeaders, bool displayGrandTotals, string rootPath)
        {
            var template = dimensions.First();
            var childTemplates = dimensions.Skip(1);
            bool isLeafNode = !childTemplates.Any();
            parent.IsExpanded = template.IsExpanded;

            if (rootPath is null)
                rootPath = string.Empty;

            string path = rootPath;

            if (!template.IsExpanded)
                return parent; // Nothing to do.  Display a sum.

            var groups = template.IsAscending ? data.GroupBy(template.GroupValue).OrderBy(x => SortValue(template, x.Key)) : data.GroupBy(template.GroupValue).OrderByDescending(x => SortValue(template, x.Key));

            foreach (var grp in groups)
            {
                string grpKeyValue = grp.Key.ToString();
                path = template.IsRow ? rootPath : rootPath + grpKeyValue;
                Node<T> child = (template.IsRow || buildHeaders) ? null : parent;

                if (child == null)
                {
                    // Child will be null if template is a row.
                    CellType childCellType = CellType.GroupHeader;
                    string headerValue = DisplayValue(template, grp.First());
                    child = new Node<T> { IsExpanded = template.IsExpanded, IsRow = template.IsRow, IsLeafNode = isLeafNode, Value = headerValue, CellType = childCellType };
                    parent.Children.Add(child);
                }

                if (!isLeafNode)  // isLeafNode and templateIsRow cannot both be true at the same time.
                    BuildNodes(child, grp, childTemplates, measures, buildHeaders, displayGrandTotals, path);

                if ((buildHeaders && isLeafNode) || (!buildHeaders && !childTemplates.Any(x => x.IsRow)))
                {
                    // Create measures on leaf nodes

                    if (buildHeaders)
                        CreateMeasureHeaders(child, measures, path);
                    else
                    {
                        CellType cellType = (isLeafNode && child.CellType == CellType.GroupHeader) ? CellType.Measure : child.CellType == CellType.GrandTotalHeader ? CellType.GrandTotal : CellType.Total;
                        CreateMeasures(child, measures, template, grp, path, cellType); // Build Column measures, totals and grand totals
                    }
                }
                else if (template.IsRow || buildHeaders)
                    CreateTotals(parent, measures, dimensions, grp, path, buildHeaders, displayGrandTotals, CellType.TotalHeader);
            }

            if (parent.CellType == CellType.Root && displayGrandTotals)
                CreateTotals(parent, measures, dimensions, data, rootPath, buildHeaders, displayGrandTotals, CellType.GrandTotalHeader);

            return parent;
        }

        private void CreateTotals(Node<T> parentNode, IEnumerable<Measure<T>> measures, IEnumerable<Dimension<T>> templates, IEnumerable<T> grp, string columnKey, bool buildHeaders, bool displayGrandTotals, CellType cellType)
        {
            Dimension<T> template = templates.First();
            Node<T> total = new Node<T> { IsExpanded = template.IsExpanded, IsRow = template.IsRow, IsLeafNode = false, CellType = cellType };
            total.Value = (cellType == CellType.TotalHeader ? DisplayValue(template, grp.First()) : "Grand") + " Total";
            parentNode.Children.Add(total);

            if (buildHeaders)
                CreateMeasureHeaders(total, measures, columnKey);
            else
            {
                BuildNodes(total, grp, templates.Skip(1).Where(x => !x.IsRow), measures, buildHeaders, displayGrandTotals, columnKey);
                CreateMeasures(total, measures, template, grp, columnKey, CellType.GrandTotal);
            }
        }

        private void CreateMeasures(Node<T> parentNode, IEnumerable<Measure<T>> measures, Dimension<T> template, IEnumerable<T> grp, string columnKey, CellType cellType)
        {
            // Measure are always leaf node columns.
            foreach (Measure<T> measure in measures)
            {
                decimal value = measure.Aggragate(grp);
                Node<T> measureChild = new Node<T> { IsExpanded = template.IsExpanded, IsRow = false, IsLeafNode = true, CellKey = columnKey + measure.Header, CellType = cellType };
                measureChild.Value = string.IsNullOrEmpty(measure.Format) ? value : String.Format(measure.Format, value);
                parentNode.Children.Add(measureChild);
            }
        }

        private void CreateMeasureHeaders(Node<T> parentNode, IEnumerable<Measure<T>> measures, string columnKey)
        {
            // Measure headers are always expanded and are always displayed as column headers - never as row headers.
            foreach (Measure<T> measure in measures)
            {
                Node<T> measureHeader = new Node<T> { IsExpanded = true, IsRow = false, IsLeafNode = true, Value = measure.Header, CellKey = columnKey + measure.Header, CellType = CellType.MeasureHeader };
                parentNode.Children.Add(measureHeader);
            }
        }

        private string DisplayValue(Dimension<T> template, T data) => template.HeaderValue == null ? template.GroupValue(data) : template.HeaderValue(data);

        private string SortValue(Dimension<T> template, string data) => template.SortValue == null ? data : template.SortValue(data);

        private void Validate(IEnumerable<T> data, IEnumerable<Dimension<T>> dimensions, IEnumerable<Measure<T>> measures, bool buildingHeaders)
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

            if(measures.GroupBy(x => x.Header).Any(x => x.Count() > 1))
                throw new Exception("The Header property for each measure must be unique.");

            dimensions = dimensions.OrderBy(x => !x.IsRow).ThenBy(x => x.Sequence);
        }
    }
}
