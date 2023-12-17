using LeaderAnalytics.LeaderPivot;
using System.Diagnostics;

namespace LeaderPivot.Tests;

[TestFixture]
public class VintageTests
{
    protected List<TestVintage> TestVintages { get; private set; } = new(2500);
    protected List<IDimensionT<TestVintage>> Dimensions = new List<IDimensionT<TestVintage>>();
    protected List<IMeasureT<TestVintage>> Measures = new List<IMeasureT<TestVintage>>();
    protected MatrixBuilder<TestVintage> MatrixBuilder;
    protected INodeBuilder<TestVintage> NodeBuilder;

    protected int TestDataCount { get; set; } = 2500;

    public VintageTests()
    {
        Dimensions.AddRange(new Dimension<TestVintage>[] {
            new Dimension<TestVintage>
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


            new Dimension<TestVintage>
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

        Measures.Add(new Measure<TestVintage> { Aggragate = x => x.Measure.Sum(y => Convert.ToDecimal(y.Value)), DisplayValue = "Value", Format = "{0:n3}", Sequence = 1, IsEnabled = true });

        BuildTestVintages();
    }

    protected void BuildTestVintages()
    {
        TestVintages.Clear();
        DateTime startDate = new DateTime(1966, 8, 20);

        for (int i = 0; i < TestDataCount; i++)
            TestVintages.Add(new TestVintage { ObsDate = startDate.AddDays(i), VintageDate = startDate.AddDays(i), Value = i.ToString() });
    }

    [SetUp]
    public async Task SetUp()
    {
        IValidator<TestVintage> validator = new Validator<TestVintage>();
        NodeBuilder = new NodeBuilder<TestVintage>();
        MatrixBuilder = new MatrixBuilder<TestVintage>(NodeBuilder, validator);
    }

    [Test]
    public async Task MatrixBuilderTest()
    {
        Stopwatch sw = Stopwatch.StartNew();
        MatrixBuilder.BuildMatrix(TestVintages, Dimensions, Measures, true);
        sw.Stop();
        Assert.IsTrue(sw.Elapsed.TotalSeconds < 60);
    }

    [Test]
    public async Task NodeBuilderTest()
    {
        Stopwatch sw = Stopwatch.StartNew();
        NodeBuilder.Build(TestVintages, Dimensions, Measures, true);
        sw.Stop();
        Assert.IsTrue(sw.Elapsed.TotalSeconds < 30);
    }
}
