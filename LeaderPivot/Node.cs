
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
        public bool IsExpanded { get; set; }
        public bool IsRow { get; set; }
        public bool IsLeafNode { get; set; }
        public CellType CellType { get; set; }
        public string CellKey { get; set; }
        public object Value { get; set; }
        public List<Node<T>> Children { get; set; }
        public string DimensionID { get; set; }

        public Node() => Children = new List<Node<T>>();
    }
}
