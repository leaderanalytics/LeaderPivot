using LeaderAnalytics.LeaderPivot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using TestData;

namespace ConsoleDemo
{
    class Program
    {
        private static NodeBuilder<SalesData> nodeBuilder;

        static void Main(string[] args)
        {
            nodeBuilder = new NodeBuilder<SalesData>(NodeCache<SalesData>.Instance);
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
            List<Dimension<SalesData>> dimensions = salesDataService.LoadSalesDataDimensions();
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
                    "CellKey".PadRight(30) +
                    "CellType".PadRight(20) +
                    "Dim DisplayValue".PadRight(20) +
                    "IsRow".PadRight(8) +
                    "IsExp".PadRight(8) +
                    "ID".PadRight(30)

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
                    v.CellKey?.PadRight(30) +
                    v.CellType.ToString().PadRight(20) +
                    v.Dimension.DisplayValue.PadRight(20) +
                    (v.IsRow ? "Y" : "N").PadRight(8) +
                    (v.IsExpanded ? "Y" : "N").PadRight(8) +
                    v.ID.PadRight(30)

                    );
            }
            foreach (Node<SalesData> c in v.Children)
                _DisplayGraph(c, level + 1);

        }
    }
}
