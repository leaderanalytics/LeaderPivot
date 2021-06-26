﻿using LeaderAnalytics.LeaderPivot;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestData;

namespace WebDemo.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        public Matrix table { get; set; }

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
            SalesDataService salesDataService = new SalesDataService();
            List<SalesData> salesData = salesDataService.GetSalesData();
            List<Dimension<SalesData>> dimensions = salesDataService.LoadSalesDataDimensions();
            List<Measure<SalesData>> measures = salesDataService.LoadMeasures();
            bool displayGrandTotals = true;
            NodeBuilder<SalesData> nodeBuilder = new NodeBuilder<SalesData>();
            MatrixBuilder<SalesData> matrixBuilder = new MatrixBuilder<SalesData>(nodeBuilder);
            table = matrixBuilder.BuildMatrix(salesData, dimensions, measures, displayGrandTotals);
        }
    }
}