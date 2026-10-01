using activity_00_tap_26_27.Core;
using activity_00_tap_26_27.Core.Events;
using GameLibrary;
using GameLibrary.Components;


namespace GameLibraryTests
{
    public class GameManagerTests
    {
        [Test]
        public void GetSelectedDestinationIndex_ReturnMinusOneAfterConstruction()
        {
            EventManager event_manager = new EventManager();
            GameManager game_manager = new GameManager(event_manager);

            Assert.That(game_manager.GetSelectedDestinationIndex(), Is.EqualTo(-1));
        }

        [Test]
        public void GetSelectedDestinationIndex_ReturnMinusOneAfterConfirmCommand()
        {
            EventManager event_manager = new EventManager();
            GameManager game_manager = new GameManager(event_manager);

            event_manager.TriggerEvent(new GameActionGameEvent(GameActionType.CONFIRM));

            Assert.That(game_manager.GetSelectedDestinationIndex(), Is.EqualTo(-1));
        }

        [Test]
        public void GetSelectedDestinationIndex_ReturnZeroAfterNavigateDownCommand()
        {
            EventManager event_manager = new EventManager();
            GameManager game_manager = new GameManager(event_manager);

            event_manager.TriggerEvent(new GameActionGameEvent(GameActionType.NAVIGATE_DOWN));

            Assert.That(game_manager.GetSelectedDestinationIndex(), Is.EqualTo(0));
        }

        [Test]
        public void GetCurrentLocation_ReturnChildAfterNavigateDownAndConfirmCommand()
        {
            EventManager event_manager = new EventManager();
            GameManager game_manager = new GameManager(event_manager);
            LocationComponent initial_location = game_manager.GetCurrentLocation();

            event_manager.TriggerEvent(new GameActionGameEvent(GameActionType.NAVIGATE_DOWN));
            event_manager.TriggerEvent(new GameActionGameEvent(GameActionType.CONFIRM));

            Assert.That(game_manager.GetCurrentLocation(), Is.Not.EqualTo(initial_location));
        }
    }
}