using LeaderAnalytics.LeaderPivot;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LeaderAnalytics.LeaderPivot.TestData;

namespace WebDemo.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        public Matrix table { get; set; }
        private bool displayGrandTotals = false;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
            SalesDataService salesDataService = new SalesDataService();
            List<SalesData> salesData = salesDataService.GetSalesData();
            List<Dimension<SalesData>> dimensions = salesDataService.LoadDimensions();
            List<Measure<SalesData>> measures = salesDataService.LoadMeasures();
            displayGrandTotals = true;
            NodeBuilder<SalesData> nodeBuilder = new NodeBuilder<SalesData>(NodeCache<SalesData>.Instance);
            Validator<SalesData> validator = new Validator<SalesData>();
            MatrixBuilder<SalesData> matrixBuilder = new MatrixBuilder<SalesData>(nodeBuilder, validator);
            table = matrixBuilder.BuildMatrix(salesData, dimensions, measures, displayGrandTotals);
        }
    }
}
