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
        private bool IsHeader { get; set; }
        private bool DisplayGrandTotals { get; set; }

        public Vector(IEnumerable<T> data, IEnumerable<Dimension<T>> dimensions, IEnumerable<Measure<T>> measures, bool displayGrandTotals, bool isHeader = false) : this()
        {
            IsHeader = isHeader;
            DisplayGrandTotals = displayGrandTotals;
            Build(this, data, dimensions, measures, 0);
        }

        protected Vector() => Children = new List<Vector<T>>();


        protected virtual void Build(Vector<T> parent, IEnumerable<T> data, IEnumerable<Dimension<T>> dimensions, IEnumerable<Measure<T>> measures, int level)
        {
            var template = dimensions.First();
            var childTemplates = dimensions.Skip(1);
            bool isLeafNode = !childTemplates.Any();
            IsExpanded = template.IsExpanded;
            DisplayTotals = template.DisplayTotals;

            if (!template.IsExpanded)
                return; // Nothing to do.  Display a sum.

            var sortedGroups = template.IsAscending ? data.GroupBy(template.Group).OrderBy(x => x.Key) : data.GroupBy(template.Group).OrderByDescending(x => x.Key);

            foreach (var grp in sortedGroups)
            {
                // Only append headers if building headers (IsHeader is true) or if building a row.
                Vector<T> child = null;
                if (!template.IsRow && !IsHeader)
                    child = parent;
                else
                {
                    child = new Vector<T> { IsExpanded = template.IsExpanded, IsRow = template.IsRow, IsLeafNode = isLeafNode };
                    child.Value = string.IsNullOrEmpty(template.Format) ? template.Group(grp.First()) : String.Format(template.Format, template.Group(grp.First()));
                    parent.Children.Add(child);
                }

                if (!isLeafNode)
                {
                    Build(child, grp, childTemplates, measures, level + 1);

                    if (template.DisplayTotals)
                    {
                        if (!template.IsRow && !IsHeader)
                        {
                            if (IsHeader)
                                CreateMeasureHeaders(parent, measures);
                            else
                                CreateMeasures(parent, measures, template, grp);
                        }
                        else if (IsHeader || childTemplates.Any(x => x.IsRow)) // don't create a total row for a leaf row - there is only one row so no need to sum. 
                        {
                            string header = (string.IsNullOrEmpty(template.Format) ? template.Group(grp.First()) : String.Format(template.Format, template.Group(grp.First()))).ToString() + " Total";
                            Vector<T> totals = new Vector<T> { IsExpanded = template.IsExpanded, IsRow = template.IsRow, IsLeafNode = isLeafNode, Value = header };
                            parent.Children.Add(totals);
                            
                            if(IsHeader)
                                CreateMeasureHeaders(totals, measures);
                            else
                                Build(totals, grp, childTemplates.Where(x => !x.IsRow), measures, level + 1);
                        }
                    }
                }
                else
                {
                    if (IsHeader)
                        CreateMeasureHeaders(child, measures);
                    else
                        CreateMeasures(child, measures, template, grp);
                }
            }


            if (DisplayGrandTotals && level == 0)
            {
                Vector<T> grandTotal = new Vector<T> { IsExpanded = template.IsExpanded, IsRow = template.IsRow, IsLeafNode = isLeafNode, Value = "Grand Total" };
                parent.Children.Add(grandTotal);

                if (IsHeader)
                    CreateMeasureHeaders(grandTotal, measures);
                else
                    Build(grandTotal, data, childTemplates.Where(x => !x.IsRow), measures, level + 1);
            }
        }

        private void CreateMeasures(Vector<T> vector, IEnumerable<Measure<T>> measures, Dimension<T> template, IEnumerable<T> grp)
        {
            foreach (Measure<T> measure in measures)
            {
                decimal value = measure.Aggragate(grp);
                Vector<T> measureChild = new Vector<T> { IsExpanded = template.IsExpanded, IsRow = template.IsRow, IsLeafNode = true };
                measureChild.Value = string.IsNullOrEmpty(measure.Format) ? value : String.Format(measure.Format, value);
                vector.Children.Add(measureChild);
            }
        }

        private void CreateMeasureHeaders(Vector<T> vector, IEnumerable<Measure<T>> measures)
        {
            // Measure headers are always expanded and are always displayed as column headers - never as row headers.
            foreach (Measure<T> measure in measures)
            {
                Vector<T> measureHeader = new Vector<T> { IsExpanded = true, IsRow = false, IsLeafNode = true, Value = measure.Header };
                vector.Children.Add(measureHeader);
            }
        }
    }
}
