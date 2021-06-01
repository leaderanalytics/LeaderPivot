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
            Build(this, data, dimensions, measures, string.Empty, 0);
        }

        protected Vector() => Children = new List<Vector<T>>();


        protected virtual void Build(Vector<T> parent, IEnumerable<T> data, IEnumerable<Dimension<T>> dimensions, IEnumerable<Measure<T>> measures, string rootPath, int level)
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
                path = template.IsRow ? rootPath : rootPath + grp.Key.ToString();

                // Only append headers if building headers (IsHeader is true) or if building a row.
                Vector<T> child = null;
                if (!template.IsRow && !IsHeader)
                    child = parent;
                else
                {
                    child = new Vector<T> { IsExpanded = template.IsExpanded, IsRow = template.IsRow, IsLeafNode = isLeafNode, Value = FormattedCellValue(template, grp.First(), false), CellType = CellType.GroupHeader };
                    parent.Children.Add(child);
                }

                if (!isLeafNode)
                {
                    Build(child, grp, childTemplates, measures, path, level + 1);

                    if (template.DisplayTotals)
                    {
                        if (!template.IsRow && !IsHeader)
                        {
                            if (IsHeader)
                                CreateMeasureHeaders(parent, measures, path);
                            else
                            {
                                CellType cellType = parent.CellType == CellType.GroupHeader ? CellType.Total : CellType.GrandTotal;
                                CreateMeasures(parent, measures, template, grp, path, cellType);
                            }
                        }
                        else if (IsHeader || childTemplates.Any(x => x.IsRow)) 
                        {
                            // Don't create a total row for a leaf row.  A leaf row may not be a truly leaf vector - it will have child vectors
                            // but they are columns.  Leaf columns are true leaf nodes. 
                            Vector<T> totals = new Vector<T> { IsExpanded = template.IsExpanded, IsRow = template.IsRow, IsLeafNode = isLeafNode, Value = FormattedCellValue(template,grp.First(), true), CellType = CellType.TotalHeader };
                            parent.Children.Add(totals);
                            
                            if(IsHeader)
                                CreateMeasureHeaders(totals, measures, path);
                            else
                                Build(totals, grp, childTemplates.Where(x => !x.IsRow), measures, path, level + 1);
                        }
                    }
                    
                    if (DisplayGrandTotals && !IsHeader && template.IsRow && ! childTemplates.Any(x => x.IsRow))
                        CreateMeasures(child, measures, template, grp, path, CellType.GrandTotal);
                }
                else
                {
                    if (IsHeader)
                        CreateMeasureHeaders(child, measures, path);
                    else
                    {
                        CellType cellType = child.CellType == CellType.GroupHeader ? CellType.Measure : child.CellType == CellType.TotalHeader ? CellType.Total : CellType.GrandTotal;
                        CreateMeasures(child, measures, template, grp, path, cellType);
                    }
                }
            }


            if (DisplayGrandTotals && level == 0)
            {
                Vector<T> grandTotal = new Vector<T> { IsExpanded = template.IsExpanded, IsRow = template.IsRow, IsLeafNode = isLeafNode, Value = "Grand Total", CellType = CellType.GrandTotalHeader };
                parent.Children.Add(grandTotal);

                if (IsHeader)
                    CreateMeasureHeaders(grandTotal, measures, path);
                else
                    Build(grandTotal, data, childTemplates.Where(x => !x.IsRow), measures, path, level + 1);
            }
        }

        private void CreateMeasures(Vector<T> vector, IEnumerable<Measure<T>> measures, Dimension<T> template, IEnumerable<T> grp, string columnKey, CellType cellType)
        {
            foreach (Measure<T> measure in measures)
            {
                decimal value = measure.Aggragate(grp);
                Vector<T> measureChild = new Vector<T> { IsExpanded = template.IsExpanded, IsRow = template.IsRow, IsLeafNode = true, ColumnKey = columnKey + measure.Header, CellType = cellType };
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

        private string FormattedCellValue(Dimension<T> template, T data, bool isTotal)
        {
            string header = (string.IsNullOrEmpty(template.Format) ? 
                template.Group(data) : 
                String.Format(template.Format, template.Group(data))).ToString() + (isTotal ? " Total" : string.Empty);
            
            return header;
        }
    }
}
