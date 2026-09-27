using activity_00_tap_26_27.Core;
using activity_00_tap_26_27.Core.Events;
using GameLibrary.Components;

namespace GameLibraryTests;

public class GameManagerTests
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
    public void InitialState_HasNoSelectedDestination()
    {
        EventManager event_manager = new EventManager();
        GameManager game_manager = new GameManager(event_manager);

        LocationComponent current_location = game_manager.GetCurrentLocation();

        Assert.That(current_location, Is.Null);
    }

    [Test]

    public void ConfirmAction_WithoutSelection_DoesNotChangeLocation()
    {
        EventManager event_manager = new EventManager();
        GameManager game_manager = new GameManager(event_manager);
        GameActionType confirm_action = GameActionType.CONFIRM;
        LocationComponent initial_location = CreateLocation("Initial Location");

        
        game_manager.SetCurrentLocation(initial_location);
        event_manager.TriggerEvent(new GameActionGameEvent(confirm_action));

        LocationComponent current_location = game_manager.GetCurrentLocation();
        Assert.That(current_location, Is.EqualTo(initial_location));
    }

    [Test]
    public void NavigateDownAction_WithoutSelection_SelectsFirstDestination()
    {
        EventManager event_manager = new EventManager();
        GameManager game_manager = new GameManager(event_manager);
        GameActionType navigate_down_action = GameActionType.NAVIGATE_DOWN;
        LocationComponent initial_location = CreateLocation("Initial Location");
        LocationComponent destination_location = CreateLocation("Destination Location");

        initial_location.AddConnection(destination_location, 5.0f);
        game_manager.SetCurrentLocation(initial_location);
        event_manager.TriggerEvent(new GameActionGameEvent(navigate_down_action));
        int selected_index = game_manager.GetSelectedDestinationIndex();

        Assert.That(selected_index, Is.EqualTo(0));
    }

    [Test]
    public void NavigateDownAction_WithSelection_SelectsFirstDestination()
    {
        EventManager event_manager = new EventManager();
        GameManager game_manager = new GameManager(event_manager);
        GameActionType navigate_down_action = GameActionType.NAVIGATE_DOWN;
        LocationComponent initial_location = CreateLocation("Initial Location");
        LocationComponent destination_location = CreateLocation("Destination Location");

        initial_location.AddConnection(destination_location, 5.0f);
        game_manager.SetCurrentLocation(initial_location);
        event_manager.TriggerEvent(new GameActionGameEvent(navigate_down_action));
        event_manager.TriggerEvent(new GameActionGameEvent(navigate_down_action));
        int selected_index = game_manager.GetSelectedDestinationIndex();

        Assert.That(selected_index, Is.EqualTo(0));
    }
}
