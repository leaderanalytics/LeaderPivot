using LeaderPivot;
using System;
using System.Collections.Generic;
using System.Linq;
using TestData;

namespace ConsoleDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            DisplaySalesDataDataGraph();
            DisplaySalesDataHeaderGraph();
               
        }


        static void DisplaySalesDataDataGraph()
        {
            Console.Clear();
            SalesDataService salesDataService = new SalesDataService();
            List<SalesData> salesData = salesDataService.GetSalesData();
            List<Dimension<SalesData>> dimensions = salesDataService.LoadSalesDataDimensions();
            List<Measure<SalesData>> measures = salesDataService.LoadMeasures();
            bool displayGrandTotals = true;
            Matrix<SalesData> matrix = new Matrix<SalesData>(salesData, dimensions, measures, displayGrandTotals);
            Vector<SalesData> vector = matrix.GetVector();
            DisplayGraph(vector);
            Console.ReadKey();
        }

        static void DisplaySalesDataHeaderGraph()
        {
            Console.Clear();
            SalesDataService salesDataService = new SalesDataService();
            List<SalesData> salesData = salesDataService.GetSalesData();
            List<Dimension<SalesData>> dimensions = salesDataService.LoadSalesDataDimensions();
            List<Measure<SalesData>> measures = salesDataService.LoadMeasures();
            bool displayGrandTotals = true;
            Matrix<SalesData> matrix = new Matrix<SalesData>(salesData, dimensions, measures, displayGrandTotals);
            Vector<SalesData> columnHeaders = new Vector<SalesData>(salesData, dimensions.Where(x => !x.IsRow), measures, displayGrandTotals, true);
            DisplayGraph(columnHeaders);
            Console.ReadKey();
        }

        static void DisplayGraph(Vector<SalesData> v)
        {
            Console.Clear();
            _DisplayGraph(v, 0);
            Console.ReadKey();
        }


        static void _DisplayGraph(Vector<SalesData> v, int level)
        {
            Console.WriteLine(new String('*', level) + " " + v.Value + " " + v.ColumnKey);

            foreach (Vector<SalesData> c in v.Children)
                _DisplayGraph(c, level + 1);

        }
    }
}
