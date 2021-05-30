using LeaderPivot;
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
        public Table table { get; set; }

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
            Matrix<SalesData> matrix = new Matrix<SalesData>(salesData, dimensions, measures, displayGrandTotals);
            Vector<SalesData> vector = matrix.GetVector();
            table = matrix.BuildTable(vector);
        }
    }
}
