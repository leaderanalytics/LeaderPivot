using LeaderAnalytics.LeaderPivot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using LeaderAnalytics.LeaderPivot.TestData;

namespace ConsoleDemo
{
    class Program
    {
        private static NodeBuilder<SalesData> nodeBuilder;

        static void Main(string[] args)
        {
            nodeBuilder = new NodeBuilder<SalesData>();
            DisplaySalesDataDataGraph();
            DisplaySalesDataHeaderGraph();
        }


        static void DisplaySalesDataDataGraph()
        {
            Console.Clear();
            SalesDataService salesDataService = new SalesDataService();
            List<SalesData> salesData = salesDataService.GetSalesData();
            List<Dimension<SalesData>> dimensions = salesDataService.LoadDimensions();
            List<Measure<SalesData>> measures = salesDataService.LoadMeasures();
            bool displayGrandTotals = true;
            Validator<SalesData> validator = new Validator<SalesData>();
            validator.Validate(salesData, dimensions, measures);
            dimensions = validator.ValidateDimensions(dimensions);
            measures = validator.SortAndFilterMeasures(measures);
            Node<SalesData> dataNode = nodeBuilder.Build(salesData, dimensions, measures, displayGrandTotals);
            DisplayGraph(dataNode);
        }

        static void DisplaySalesDataHeaderGraph()
        {
            Console.Clear();
            SalesDataService salesDataService = new SalesDataService();
            List<SalesData> salesData = salesDataService.GetSalesData();
            List<Dimension<SalesData>> dimensions = salesDataService.LoadDimensions();
            List<Measure<SalesData>> measures = salesDataService.LoadMeasures();
            bool displayGrandTotals = true;
            Validator<SalesData> validator = new Validator<SalesData>();
            validator.Validate(salesData, dimensions, measures);
            dimensions = validator.ValidateDimensions(dimensions);
            measures = validator.SortAndFilterMeasures(measures);
            Node<SalesData> columnHeaderNode = nodeBuilder.BuildColumnHeaders(salesData, dimensions, measures, displayGrandTotals);
            DisplayGraph(columnHeaderNode);
        }

        static void DisplayGraph(Node<SalesData> v)
        {
            Console.Clear();
            Console.WriteLine(
                    "Level".PadRight(10) +
                    "Value".PadRight(20) +
                    "CellType".PadRight(20) +
                    "Row Dim DisplayValue".PadRight(30) +
                    "Col Dim DisplayValue".PadRight(30) +
                    "IsRow".PadRight(8) +
                    "ColumnKey".PadRight(30) +
                    "Node ID".PadRight(30)
                    );

            _DisplayGraph(v, 0);
            Console.ReadKey();
        }


        static void _DisplayGraph(Node<SalesData> v, int level)
        {   
            if (v.CellType != CellType.Root)
            {
                Console.WriteLine(
                    new String('*', level).PadRight(10) +
                    v.Value?.ToString()?.PadRight(20) +
                    v.CellType.ToString().PadRight(20) +
                    (v.RowDimension?.DisplayValue.PadRight(30) ?? new String(' ', 30)) +
                    (v.ColumnDimension?.DisplayValue.PadRight(30) ?? new String(' ', 30)) +
                    (v.IsRow ? "Y" : "N").PadRight(8) +
                    (v.ColumnKey?.PadRight(30) ?? new String(' ', 30)) +
                    v.ID?.PadRight(30)
                    );
            }
            
            if(v.Children?.Any()?? false)
                foreach (Node<SalesData> c in v.Children)
                    _DisplayGraph(c, level + 1);

        }
    }
}
