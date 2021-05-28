
namespace LeaderPivot
{
    public class Cell
    {
        public object Value { get; set; }
        public int RowSpan { get; set; }
        public int ColSpan { get; set; }

        public Cell() => RowSpan = ColSpan = 1;

        public Cell(object value, int rowSpan = 1, int colSpan = 1)
        {
            Value = value;
            RowSpan = rowSpan;
            ColSpan = colSpan;
        }
    }
}
