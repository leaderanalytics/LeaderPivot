using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaderAnalytics.LeaderPivot
{
    public class PagedQueryResult<T>
    {
        public List<T> Data { get; set; }
        int TotalCount { get; set; }
    }
}
