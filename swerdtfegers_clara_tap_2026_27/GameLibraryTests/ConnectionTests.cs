using activity_00_tap_26_27.Core;
using activity_00_tap_26_27;
using GameLibrary.Components;
using GameLibrary;

namespace GameLibraryTests;

public class ConnectionTests
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
    public void GetDestination_ReturnsConstructionDestination()
    {
        LocationComponent destination = CreateLocation("Daisy Town");
        Connection connection = new Connection(destination, 3.0f);
        Assert.That(connection.GetDestinationLocation(), Is.EqualTo(destination));
        Assert.That(connection.GetTimeToReachDestination(), Is.EqualTo(3.0f).Within(0.001f));
    }
}
