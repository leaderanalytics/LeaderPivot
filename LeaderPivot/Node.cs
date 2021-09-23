
using System;
using System.Collections.Generic;
using System.Linq;

namespace LeaderAnalytics.LeaderPivot
{
    /// <summary>
    /// Represents an element in a hierarchical data structure
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Node<T> : Node
    {
        public Dimension<T> RowDimension { get; set; }
        public Dimension<T> ColumnDimension { get; set; }
        public List<Node<T>> Children { get; set; }
        public bool isGrandTotal;
        public bool isLabel;

        public Node(string id, Dimension<T> rowDimension, Dimension<T> columnDimension, object val, string cellKey = null, bool isGrandTotal=false, bool isLabel=false)
        {
            ID = id ?? throw new Exception(nameof(id));
            RowDimension = rowDimension ;
            ColumnDimension = columnDimension;
            Value = val;
            CellKey = cellKey;
            this.isGrandTotal = isGrandTotal;
            this.isLabel = isLabel;
            SetCellType();
        }

        public void AddChild(Node<T> child)
        {
            if (Children == null)
                Children = new List<Node<T>>();
            
            Children.Add(child);
        }

        private void SetCellType()
        {
            if (RowDimension is null && ColumnDimension is null)
                CellType = CellType.Root;
            else if (RowDimension is null)
                CellType = isGrandTotal ? CellType.GrandTotalHeader : ColumnDimension.IsLeaf ? CellType.GroupHeader : CellType.GrandTotalHeader;
            else if (ColumnDimension is null)
                CellType = isGrandTotal ? CellType.GrandTotalHeader : RowDimension.IsLeaf ? CellType.GroupHeader : CellType.GrandTotalHeader;
            else if (isGrandTotal)
                CellType = CellType.GrandTotal;
            else if (!RowDimension.IsLeaf || ColumnDimension.IsLeaf)
                CellType = CellType.Total;
            else
                CellType = CellType.Measure;
        }
    }


    public abstract class Node
    {
        private bool _IsExpanded;
        public string ID { get; set; }                  // Uniquely identifies this node. [RowDimID:Key][ColDimID:Key][MeasureID]
        public CellType CellType { get; set; }
        public string CellKey { get; set; }             // Identifies which column a cell should be rendered in, since not all nodes have identical hierarchies.  Not unique.  Not the same as ID.
        public object Value { get; set; }
        public bool IsRow { get; set; }                 // Not always the same as the dimension, notably grand totals.
        public bool IsExpanded 
        {
            get => _IsExpanded ;
            set => _IsExpanded = value;
        }
        public bool CanToggleExapansion { get; set; }   // User can not toggle expansion on leaf dimensions (last dimension for each axis).  


        public static string CreateID(string dimensionID, string grpValue)
        {
            return $"[{dimensionID}:{grpValue}]";
        }
    }
}
