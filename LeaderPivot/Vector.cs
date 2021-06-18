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
            Build(this, data, dimensions, measures, string.Empty, false);
        }

        protected Vector() => Children = new List<Vector<T>>();


        protected virtual void Build(Vector<T> parent, IEnumerable<T> data, IEnumerable<Dimension<T>> dimensions, IEnumerable<Measure<T>> measures, string rootPath, bool isTotalsRow)
        {
            var template = dimensions.First();
            var childTemplates = dimensions.Skip(1);
            bool isLeafNode = !childTemplates.Any();
            IsExpanded = template.IsExpanded;
            DisplayTotals = template.DisplayTotals;

            if (rootPath == null)
                rootPath = string.Empty;

            string path = rootPath;
            
            if (!template.IsExpanded)
                return; // Nothing to do.  Display a sum.

            var groups = template.IsAscending ? data.GroupBy(template.Group).OrderBy(x => x.Key) : data.GroupBy(template.Group).OrderByDescending(x => x.Key);

            foreach (var grp in groups)
            {
                string grpKeyValue = grp.Key.ToString();
                path = template.IsRow ? rootPath : rootPath + grpKeyValue;
                Vector<T> child = (template.IsRow || IsHeader || isTotalsRow) ? null : parent;

                if (child == null)
                {
                    // Child will be null if template is a row.
                    CellType childCellType = parent.CellType == CellType.Root ? CellType.GrandTotalHeader : isTotalsRow ? CellType.TotalHeader : CellType.GroupHeader;
                    string headerValue = isTotalsRow ? (parent.Value ?? "Grand") + " Total" : FormattedCellValue(template, grp.First());
                    child = new Vector<T> { IsExpanded = template.IsExpanded, IsRow = template.IsRow, IsLeafNode = isLeafNode, Value = headerValue, CellType = childCellType };
                    parent.Children.Add(child);
                }

                if (!isLeafNode)  // isLeafNode and templateIsRow cannot both be true at the same time.
                    Build(child, grp, childTemplates, measures, path, false);

                if (!childTemplates.Any(x => x.IsRow)) // Only create measures on leaf node rows.  Totals rows are created outside this loop.
                {
                    if (IsHeader)
                    {
                        if(isLeafNode)
                            CreateMeasureHeaders(child, measures, path);
                    }
                    else
                    {
                        // Child will be null if IsHeader and isLeafNode are true
                        CellType cellType = isLeafNode ? CellType.Measure : template.IsRow ? CellType.GrandTotal : CellType.Total;
                        CreateMeasures(child, measures, template, grp, path, cellType); // Build Column measures, totals and grand totals
                    }
                }
            }


            if (template.DisplayTotals && (template.IsRow || IsHeader))
            {
                // Create Totals Row / Column
                

                if (IsHeader)
                {
                    CellType cellType = parent.Value == null ? CellType.GrandTotalHeader : CellType.TotalHeader;
                    parent.Children.Add(new Vector<T> { IsExpanded = template.IsExpanded, IsRow = template.IsRow, IsLeafNode = isLeafNode, Value = (parent.Value ?? "Grand") + " Total", CellType = cellType });
                    CreateMeasureHeaders(parent.Children.Last(), measures, rootPath);
                }
                else
                {
                    Build(parent, data, childTemplates.Where(x => !x.IsRow), measures, path, true);
                    CreateMeasures(parent.Children.Last(), measures, template, data, path, CellType.GrandTotal);
                }  
                
            }
        }

        

        private void CreateMeasures(Vector<T> vector, IEnumerable<Measure<T>> measures, Dimension<T> template, IEnumerable<T> grp, string columnKey, CellType cellType)
        {
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
