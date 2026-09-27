using activity_00_tap_26_27.Core.Events;

namespace GameLibraryTests;

public class EventManagerTests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void ProcessEvents_DoesNotExecuteDelayedEventBeforeCall_AndExecutesAfterCall()
    {
        EventManager event_manager = new EventManager();
        bool _wasProcessed = false;
        



        event_manager.ProcessDelayedEvents();

        Assert.That(_wasProcessed, Is.False);

        event_manager.ProcessDelayedEvents();
        

        Assert.That(_wasProcessed, Is.True);
    }
}
