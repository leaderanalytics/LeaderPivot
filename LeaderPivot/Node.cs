
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
        public Dimension<T> Dimension { get; set; }
        public List<Node<T>> Children { get; set; }

        public Node(string id, Dimension<T> dimension)
        {
            ID = id ?? throw new Exception(nameof(id));
            Dimension = dimension ?? throw new Exception(nameof(dimension));
            Children = new List<Node<T>>();
        }
    }


    public abstract class Node
    {
        private bool _IsExpanded;
        public string ID { get; set; }                  // Uniquely identifies this node.
        public CellType CellType { get; set; }
        public string CellKey { get; set; }             // Identifies which column a cell should be rendered in, since not all nodes have identical hierarchies.  Not unique.  Not the same as ID.
        public object Value { get; set; }
        public bool IsRow { get; set; }                 // Not always the same as the dimension, notably grand totals.
        public bool IsExpanded 
        {
            get => CellType == CellType.GroupHeader ? _IsExpanded : true;
            set => _IsExpanded = value;
        }
        public bool CanToggleExapansion { get; set; }   // User can not toggle expansion on leaf dimensions (last dimension for each axis).  Also, IsExpanded must be true for leaf dimensions.
    }
}
