using System;

namespace LeaderPivot
{
    public class Dimension<T>
    {
        public Func<T, object> Group { get; set; }          // The property used to group the data
        public Func<T, String> LeafNodeValue { get; set; }
        public bool IsRow { get; set; }
        public bool IsExpanded { get; set; }
        public bool IsAscending { get; set; }
        public string Header { get; set; }
        public int Sequence { get; set; }
        public string Format { get; set; }                  // Format string used to display the property used to group
        public bool DisplayTotals { get; set; }
    }
}
