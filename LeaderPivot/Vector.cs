/* 
 * Copyright 2021 Leader Analytics 
 * www.LeaderAnalytics.com
 * www.SamWheat.com
 * Written by Sam Wheat
 * 
 */
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LeaderPivot
{
    public class Vector<T>
    {
        public List<Vector<T>> Children { get; set; }
        public bool IsExpanded { get; set; }
        public bool IsRow { get; set; }
        public bool DisplayTotals { get; set; }
        public bool IsLeafNode { get; set; }
        public object Value { get; set; }
        public CellType CellType { get; set; }
        private bool IsHeader { get; set; }
        private bool DisplayGrandTotals { get; set; }
        public string ColumnKey { get; set; }

        public Vector(IEnumerable<T> data, IEnumerable<Dimension<T>> dimensions, IEnumerable<Measure<T>> measures, bool displayGrandTotals, bool isHeader = false) : this()
        {
            IsHeader = isHeader;
            DisplayGrandTotals = displayGrandTotals;
            Build(this, data, dimensions, measures, string.Empty);
        }

        protected Vector() => Children = new List<Vector<T>>();


        protected virtual void Build(Vector<T> parent, IEnumerable<T> data, IEnumerable<Dimension<T>> dimensions, IEnumerable<Measure<T>> measures, string rootPath)
        {
            var template = dimensions.First();
            var childTemplates = dimensions.Skip(1);
            bool isLeafNode = !childTemplates.Any();
            IsExpanded = template.IsExpanded;
            DisplayTotals = template.DisplayTotals;
            
            if (rootPath is null)
                rootPath = string.Empty;

            string path = rootPath;
            
            if (!template.IsExpanded)
                return; // Nothing to do.  Display a sum.

            var groups = template.IsAscending ? data.GroupBy(template.Group).OrderBy(x => x.Key) : data.GroupBy(template.Group).OrderByDescending(x => x.Key);

            foreach (var grp in groups)
            {
                string grpKeyValue = grp.Key.ToString();
                path = template.IsRow ? rootPath : rootPath + grpKeyValue;
                Vector<T> child = (template.IsRow || IsHeader) ? null : parent;

                if (child == null)
                {
                    // Child will be null if template is a row.
                    CellType childCellType = CellType.GroupHeader;
                    string headerValue = FormattedCellValue(template, grp.First());
                    child = new Vector<T> { IsExpanded = template.IsExpanded, IsRow = template.IsRow, IsLeafNode = isLeafNode, Value = headerValue, CellType = childCellType };
                    parent.Children.Add(child);
                }

                if (!isLeafNode)  // isLeafNode and templateIsRow cannot both be true at the same time.
                    Build(child, grp, childTemplates, measures, path);

                if ((IsHeader && isLeafNode) || (!IsHeader && !childTemplates.Any(x => x.IsRow)))
                {
                    // Create measures on leaf nodes

                    if (IsHeader)
                        CreateMeasureHeaders(child, measures, path);
                    else
                    {
                        CellType cellType =(isLeafNode && child.CellType == CellType.GroupHeader) ? CellType.Measure : child.CellType == CellType.GrandTotalHeader ? CellType.GrandTotal: CellType.Total;
                        CreateMeasures(child, measures, template, grp, path, cellType); // Build Column measures, totals and grand totals
                    }
                }
                else if (template.IsRow || IsHeader)
                    CreateTotals(parent, measures, dimensions, grp, path, CellType.TotalHeader);
            }

            if (parent.CellType == CellType.Root && parent.DisplayGrandTotals)
                CreateTotals(parent, measures, dimensions, data, rootPath, CellType.GrandTotalHeader);
        }


        private void CreateTotals(Vector<T> vector, IEnumerable<Measure<T>> measures, IEnumerable<Dimension<T>> templates, IEnumerable<T> grp, string columnKey, CellType cellType)
        {
            Dimension<T> template = templates.First();
            Vector<T> total = new Vector<T> { IsExpanded = template.IsExpanded, IsRow = template.IsRow, IsLeafNode = false, CellType = cellType };
            total.Value = (cellType == CellType.TotalHeader ? FormattedCellValue(template, grp.First()) : "Grand") + " Total";
            vector.Children.Add(total);

            if (IsHeader)
                CreateMeasureHeaders(total, measures, columnKey);
            else
            {
                Build(total, grp, templates.Skip(1).Where(x => !x.IsRow), measures, columnKey);
                CreateMeasures(total, measures, template, grp, columnKey, CellType.GrandTotal);
            }
        }
        

        private void CreateMeasures(Vector<T> vector, IEnumerable<Measure<T>> measures, Dimension<T> template, IEnumerable<T> grp, string columnKey, CellType cellType)
        {
            // Measure are always leaf node columns.
            foreach (Measure<T> measure in measures)
            {
                decimal value = measure.Aggragate(grp);
                Vector<T> measureChild = new Vector<T> { IsExpanded = template.IsExpanded, IsRow = false, IsLeafNode = true, ColumnKey = columnKey + measure.Header, CellType = cellType };
                measureChild.Value = string.IsNullOrEmpty(measure.Format) ? value : String.Format(measure.Format, value);
                vector.Children.Add(measureChild);
            }
        }

        private void CreateMeasureHeaders(Vector<T> vector, IEnumerable<Measure<T>> measures, string columnKey)
        {
            // Measure headers are always expanded and are always displayed as column headers - never as row headers.
            foreach (Measure<T> measure in measures)
            {
                Vector<T> measureHeader = new Vector<T> { IsExpanded = true, IsRow = false, IsLeafNode = true, Value = measure.Header, ColumnKey = columnKey + measure.Header, CellType = CellType.MeasureHeader };
                vector.Children.Add(measureHeader);
            }
        }

        private string FormattedCellValue(Dimension<T> template, T data)
        {
            string header = (string.IsNullOrEmpty(template.Format) ? 
                template.Group(data) : 
                String.Format(template.Format, template.Group(data))).ToString();
            
            return header;
        }
    }
}
