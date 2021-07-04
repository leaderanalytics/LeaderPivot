using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaderAnalytics.LeaderPivot
{
    public class NodeCache<T>
    {
        private static NodeCache<T> _instance;
        private Dictionary<string, Node<T>> cache;

        public static NodeCache<T> Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new NodeCache<T>();

                return _instance;
            }
        }

        private NodeCache()
        {
            cache = new Dictionary<string, Node<T>>();
        }

        public Node<T> Get(string nodeID, Dimension<T> dimension, CellType cellType, string cellKey, object value, bool isRow, bool isExpanded)
        {
            cache.TryGetValue(nodeID, out Node<T> node);

            if (node == null)
            {
                node = new Node<T>(nodeID, dimension);
                node.CellType = cellType;
                node.CellKey = cellKey;
                node.Value = value;
                node.IsRow = isRow;
                node.IsExpanded = dimension.IsExpanded;
                cache.Add(nodeID, node);
            }
            else
                node.Children.Clear();

            return node;
        }
    }
}
