using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// https://docs.microsoft.com/en-us/aspnet/core/performance/objectpool?view=aspnetcore-3.1

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

        public Node<T> Get(Dimension<T> rowDimension, Dimension<T> columnDmension, Measure<T> measure, object value, bool isRow)
        {
            cache.TryGetValue(nodeID, out Node<T> node);

            if (node == null)
            {
                node = new Node<T>(nodeID, dimension);
                node.CellType = cellType;
                node.CellKey = cellKey;
                node.Value = value;
                node.IsRow = isRow;
                node.IsExpanded = isExpanded;
                node.CanToggleExapansion = cellType == CellType.GroupHeader && !dimension.IsLeaf;
                cache.Add(nodeID, node);
            }
            else
                node.Children.Clear();

            return node;
        }
    }
}
