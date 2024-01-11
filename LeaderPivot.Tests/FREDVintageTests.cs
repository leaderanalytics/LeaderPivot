using LeaderAnalytics.LeaderPivot;
using System.Diagnostics;
using LeaderAnalytics.Vyntix.Fred.Model;
using LeaderAnalytics.Vyntix.Fred.Domain;
using LeaderAnalytics.Vyntix.Fred.FredClient;
using Microsoft.Extensions.Logging;
using Serilog;

namespace LeaderPivot.Tests;

[TestFixture]
public class FREDVintageTests
{
    protected List<FredObservation> Observations { get; private set; } 
    protected List<IDimensionT<FredObservation>> Dimensions = new();
    protected List<IMeasureT<FredObservation>> Measures = new();
    protected MatrixBuilder<FredObservation> MatrixBuilder;
    protected INodeBuilder<FredObservation> NodeBuilder;
    protected IFredClient FredClient;
    protected int TestDataCount { get; set; } = 2500;

    [OneTimeSetUp]
    protected async Task BuildObservations()
    {
        Dimensions.AddRange(new Dimension<FredObservation>[] {
            new Dimension<FredObservation>
            {
                DisplayValue = "Obs Date",
                GroupValue = x => x.ObsDate.ToString(Constants.DateFormat),
                HeaderValue = x => x.ObsDate.ToString(Constants.DateFormat),
                IsRow = true,
                IsExpanded = true,
                Sequence = 0,
                IsAscending = true,
                IsEnabled = true
            },


            new Dimension<FredObservation>
            {
                DisplayValue = "Vintage Date",
                GroupValue = x => x.VintageDate.ToString(Constants.DateFormat),
                HeaderValue = x => x.VintageDate.ToString(Constants.DateFormat),
                IsRow = false,
                IsExpanded = true,
                Sequence = 0,
                IsAscending = true,
                IsEnabled = true
            }});

        Measures.Add(new Measure<FredObservation> { Aggragate = x => x.Measure.Sum(y => y.Value ?? 0), DisplayValue = "Value", Format = "{0:n3}", Sequence = 1, IsEnabled = true });
        BuildFredClient();
        Observations = (await FredClient.GetObservations("CPIAUCSL")).Data;
    }

    private void BuildFredClient()
    {
        string path = "O:\\LeaderAnalytics\\Config\\Vyntix.Fred.FredClient\\apiKey.txt";
        string apiKey = System.IO.File.ReadAllText(path);
        HttpClient httpClient = new HttpClient() { BaseAddress = new Uri(FredClientConfig.BaseAPIURL) };
        FredClientConfig config = new FredClientConfig { MaxDownloadRetries = 3, ErrorDelay = 2000, MaxRequestsPerMinute = 60 }; // MaxDownloadRetries should be greater than 1
        ILoggerFactory loggerFactory = new LoggerFactory().AddSerilog();
        ILogger<IFredClient> logger = loggerFactory.CreateLogger<IFredClient>();
        FredClient = new JsonFredClient(apiKey, config, new VintageComposer(), httpClient, logger);
    }

    [SetUp]
    public async Task SetUp()
    {
        IValidator<FredObservation> validator = new Validator<FredObservation>();
        NodeBuilder = new NodeBuilder<FredObservation>();
        MatrixBuilder = new MatrixBuilder<FredObservation>(NodeBuilder, validator);
    }

    [Test]
    public async Task MatrixBuilderTest()
    {
        Assert.That(Observations.Count > 0);
        Stopwatch sw = Stopwatch.StartNew();
        Matrix matrix = MatrixBuilder.BuildMatrix(Observations, Dimensions, Measures, true);
        int rowCount = matrix.Rows.Count;
        int cellCount = matrix.Rows.Sum(x => x.Cells.Count);
        sw.Stop();
        Assert.That(sw.Elapsed.TotalSeconds < 60);
    }

    [Test]
    public async Task NodeBuilderTest()
    {
        Assert.That(Observations.Count > 0);
        Stopwatch sw = Stopwatch.StartNew();
        NodeBuilder.Build(Observations, Dimensions, Measures, true);
        sw.Stop();
        Assert.That(sw.Elapsed.TotalSeconds < 30);
    }
}
