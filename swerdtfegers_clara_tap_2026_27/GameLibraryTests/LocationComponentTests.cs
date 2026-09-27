using GameLibrary.Components;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;

namespace GameLibraryTests;

public class LocationComponentTests
{
    private LocationComponent CreateLocation(string name)
    {
        return new LocationComponent(name);
    }
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void Linking_LinkGoesBothWays_AKnowsB()
    {
        LocationComponent location_a = CreateLocation("Location A");
        LocationComponent location_b = CreateLocation("Location B");

        location_a.AddConnection(location_b, 5.0f);

        Assert.That(location_a.GetDestinationAtIndex(0).Equals(location_b));
    }
    [Test]
    public void Linking_LinkGoesBothWays_BKnowsA()
    {
        LocationComponent location_a = CreateLocation("Location A");
        LocationComponent location_b = CreateLocation("Location B");

        location_b.AddConnection(location_a, 5.0f);

        Assert.That(location_b.GetDestinationAtIndex(0).Equals(location_a));
    }
}
