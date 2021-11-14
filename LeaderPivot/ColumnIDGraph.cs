using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaderAnalytics.LeaderPivot
{
    internal class ColumnIDGraph
    {
        private string[] ColumnIDs;

        public ColumnIDGraph(int columnCount)
        {
            if (columnCount <= 0)
                throw new Exception($"{nameof(columnCount)} must be greater than zero.");

            ColumnIDs = new string[columnCount];
        }

        public  void SetColumnID(int dimensionSequence, string dimensionID, string groupValue, string measureID = null)
        {
            if (dimensionSequence < 0 || dimensionSequence > ColumnIDs.Length)
                throw new Exception($"{nameof(dimensionSequence)} cannot be less than zero or greater than dimension count.");
            else if(string.IsNullOrEmpty(dimensionID))
                throw new ArgumentNullException(nameof(dimensionID));
            else if (string.IsNullOrEmpty(groupValue))
                throw new ArgumentNullException(nameof(groupValue));

            string id = $"[{dimensionID}:{groupValue}{(string.IsNullOrEmpty(measureID) ? null : ":" + measureID)}]";
            ColumnIDs[dimensionSequence] = id;
        }

        public string GetColumnIDGraph() => String.Join('/', ColumnIDs.Where(x => x != null));
        public void ClearColumnID(int dimensionSequence) => ColumnIDs[dimensionSequence] = null;
    }
}
