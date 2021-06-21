using System;
using System.Collections.Generic;

namespace LeaderAnalytics.LeaderPivot
{
    public class Measure<T>
    {
        public Func<IEnumerable<T>, decimal> Aggragate { get; set; }
        public string Format { get; set; }
        public string Header { get; set; }
    }
}
