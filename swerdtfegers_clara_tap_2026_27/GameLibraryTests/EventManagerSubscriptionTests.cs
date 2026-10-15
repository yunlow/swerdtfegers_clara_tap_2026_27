using activity_00_tap_26_27.Core;
using activity_00_tap_26_27.Core.Events;

namespace GameLibraryTests;

public class EventManagerSubscriptionTests
{
    private readonly List<string> _receivedTable = new List<string>();
    private EventManager _eventManager;

    [SetUp]
    public void SetUp()
    {
        _receivedTable.Clear();
        _eventManager = new EventManager();
    }

    private void OnFirst(IGameEvent game_event)
    {
        _receivedTable.Add("first");
    }

    private void OnSecond(IGameEvent game_event)
    {
        _receivedTable.Add("second");
    }

    private void OnFirstThatUnregistersItself(IGameEvent game_event)
    {
        _receivedTable.Add("first");
        _eventManager.UnregisterFromEvent<GameActionGameEvent>(OnFirstThatUnregistersItself);
    }

    private void OnFirstThatRegistersSecond(IGameEvent game_event)
    {
        _receivedTable.Add("first");
        _eventManager.RegisterToEvent<GameActionGameEvent>(OnSecond);
    }

    [Test]
    public void TriggerEvent_StillReachesTheNextListenerWhenOneUnregistersDuringDispatch()
    {
        _eventManager.RegisterToEvent<GameActionGameEvent>(OnFirstThatUnregistersItself);
        _eventManager.RegisterToEvent<GameActionGameEvent>(OnSecond);

        _eventManager.TriggerEvent(new GameActionGameEvent(GameActionType.CONFIRM));

        Assert.That(_receivedTable.Count, Is.EqualTo(2));
        Assert.That(_receivedTable[0], Is.EqualTo("first"));
        Assert.That(_receivedTable[1], Is.EqualTo("second"));
    }

    [Test]
    public void TriggerEvent_DoesNotSendTheCurrentEventToAListenerAddedDuringDispatch()
    {
        _eventManager.RegisterToEvent<GameActionGameEvent>(OnFirstThatRegistersSecond);

        _eventManager.TriggerEvent(new GameActionGameEvent(GameActionType.CONFIRM));

        Assert.That(_receivedTable.Count, Is.EqualTo(1));
        Assert.That(_receivedTable[0], Is.EqualTo("first"));
    }

    [Test]
    public void ProcessDelayedEvents_AllowsAListenerToUnregisterDuringDispatch()
    {
        _eventManager.RegisterToEvent<GameActionGameEvent>(OnFirstThatUnregistersItself);
        _eventManager.RegisterToEvent<GameActionGameEvent>(OnSecond);
        _eventManager.TriggerDelayedEvent(new GameActionGameEvent(GameActionType.CONFIRM));

        _eventManager.ProcessDelayedEvents();

        Assert.That(_receivedTable.Count, Is.EqualTo(2));
        Assert.That(_receivedTable[0], Is.EqualTo("first"));
        Assert.That(_receivedTable[1], Is.EqualTo("second"));
    }
}
