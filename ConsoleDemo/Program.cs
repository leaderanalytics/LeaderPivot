﻿using LeaderAnalytics.LeaderPivot;
using System;
using System.Collections.Generic;
using System.Linq;
using TestData;

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
            List<Dimension<SalesData>> dimensions = salesDataService.LoadSalesDataDimensions();
            List<Measure<SalesData>> measures = salesDataService.LoadMeasures();
            bool displayGrandTotals = true;
            Node<SalesData> dataNode = nodeBuilder.Build(salesData, dimensions, measures, displayGrandTotals);
            DisplayGraph(dataNode);
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
            Node<SalesData> columnHeaderNode = nodeBuilder.BuildColumnHeaders(salesData, dimensions, measures, displayGrandTotals);
            DisplayGraph(columnHeaderNode);
            Console.ReadKey();
        }

        static void DisplayGraph(Node<SalesData> v)
        {
            Console.Clear();
            _DisplayGraph(v, 0);
            Console.ReadKey();
        }


        static void _DisplayGraph(Node<SalesData> v, int level)
        {
            Console.WriteLine(new String('*', level) + "  " + v.Value + "\t\t " + v.CellKey + "\t\t " + v.CellType.ToString());

            foreach (Node<SalesData> c in v.Children)
                _DisplayGraph(c, level + 1);

        }
    }
}