
using System;
using System.Collections.Generic;
using System.Linq;

namespace LeaderAnalytics.LeaderPivot
{
    /// <summary>
    /// Represents an element in a hierarchical data structure
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Node<T>
    {
        public string ID { get; set; }          // Uniquely identifies this node.
        public Dimension<T> Dimension { get; set; }
        public CellType CellType { get; set; }
        public string CellKey { get; set; }     // Identifies which column a cell should be rendered in, since not all nodes have identical hierarchies.  Not unique.  Not the same as ID.
        public object Value { get; set; }
        public bool IsRow { get; set; }         // Not always the same as the dimension, notably grand totals.
        public bool IsExpanded { get; set; }
        public List<Node<T>> Children { get; set; }

        public Node(string id, Dimension<T> dimension)
        {
            ID = id ?? throw new Exception(nameof(id));
            Dimension = dimension ?? throw new Exception(nameof(dimension));
            Children = new List<Node<T>>();
        }
    }
}
