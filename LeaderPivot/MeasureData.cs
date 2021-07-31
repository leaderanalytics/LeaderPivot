using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaderAnalytics.LeaderPivot
{
    public class MeasureData<T>
    {
        public IEnumerable<T> Measure { get; set; }
        public IEnumerable<T> RowGroup { get; set; }
        public IEnumerable<T> ColumnGroup { get; set; }
        public Dimension<T> RowDimension { get; set; }
        public Dimension<T> ColumnDimension { get; set; }

        public MeasureData(IEnumerable<T> measure, IEnumerable<T> rowGroup, IEnumerable<T> columnGroup, Dimension<T> rowDimension, Dimension<T> columnDimension)
        {
            Measure = measure;
            RowGroup = rowGroup;
            ColumnGroup = columnGroup;
            RowDimension = rowDimension;
            ColumnDimension = columnDimension;
        }
    }
}
