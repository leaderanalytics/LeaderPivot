
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
        public TotalType totalType;
        public bool isLabel;

        public Node(Dimension<T> rowDimension, Dimension<T> columnDimension, object val, CellType cellType, string columnKey = null)
        {
            RowDimension = rowDimension;
            ColumnDimension = columnDimension;
            Value = val;
            ColumnKey = columnKey;
            CellType = cellType;
        }

        public void AddChild(Node<T> child)
        {
            if (Children == null)
                Children = new List<Node<T>>();
            
            Children.Add(child);
        }
    }


    public abstract class Node
    {
        public string ID { get; private set; } = Guid.NewGuid().ToString();
        private bool _IsExpanded = true;
        public CellType CellType { get; set; }
        public string ColumnKey { get; set; }             // Identifies which column a cell should be rendered in, since not all nodes have identical hierarchies.  Not unique.  Not the same as ID.
        public object Value { get; set; }
        public bool IsRow { get; set; }                 // Not always the same as the dimension, notably grand totals.
        public bool IsExpanded 
        {
            get => _IsExpanded ;
            set => _IsExpanded = value;
        }
        public bool CanToggleExapansion { get; set; }   // User can not toggle expansion on leaf dimensions (last dimension for each axis).  
    }
}
