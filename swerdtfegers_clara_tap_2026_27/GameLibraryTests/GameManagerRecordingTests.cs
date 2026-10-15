using activity_00_tap_26_27.Core;
using activity_00_tap_26_27.Core.Events;
using activity_00_tap_26_27.Data;
using GameLibrary.CSV;

namespace GameLibraryTests;

public class GameManagerRecordingTests
{
    private const float FIXED_DELTA_TIME = 0.02f;

    // ADAPTEZ : remplissez votre GameContentData avec un monde minimal.
    private GameContentData CreateGameContent()
    {
        GameContentData game_content_data = new GameContentData();

        game_content_data._locationDataTable = new List<LocationData>();
        game_content_data._locationDataTable.Add(new LocationData("WORLD", "World", "", -1));
        game_content_data._locationDataTable.Add(new LocationData("DAISY_TOWN", "Daisy Town", "WORLD", 50));
        game_content_data._shopDataTable = new List<ShopData>();
        game_content_data._itemDataTable = new Dictionary<string, ItemData>();
        game_content_data._inventoryDataTable = new List<InventoryEntryData>();
        game_content_data._heroDataTable = new List<HeroData>();
        game_content_data._heroDataTable.Add(new HeroData("LUCKY_LUKE", "Lucky Luke", 100, 40));

        return game_content_data;
    }

    // ADAPTEZ : le constructeur de votre GameManager.
    private GameManager CreateGameManager(EventManager event_manager)
    {
        return new GameManager(event_manager, CreateGameContent());
    }

    private void RunSteps(GameManager game_manager, EventManager event_manager, int step_count)
    {
        for (int step_index = 0; step_index < step_count; step_index++)
        {
            game_manager.FixedUpdate(FIXED_DELTA_TIME);
            event_manager.ProcessDelayedEvents();
        }
    }

    // Comme une touche pressée : l'action est traitée à la fin du pas suivant.
    private void PlayAction(GameManager game_manager, EventManager event_manager, GameActionType action_type)
    {
        event_manager.TriggerDelayedEvent(new GameActionGameEvent(action_type));
        RunSteps(game_manager, event_manager, 1);
    }

    [Test]
    public void GetSaveText_ReturnsEmptyTextOutsideAGame()
    {
        EventManager event_manager = new EventManager();
        GameManager game_manager = CreateGameManager(event_manager);

        string save_text = game_manager.GetSaveText();

        Assert.That(save_text, Is.EqualTo(""));
    }

    [Test]
    public void StartNewGame_ResetsTheFixedStepIndexToZero()
    {
        EventManager event_manager = new EventManager();
        GameManager game_manager = CreateGameManager(event_manager);
        RunSteps(game_manager, event_manager, 25);

        game_manager.StartNewGame(1234567);

        Assert.That(game_manager.GetFixedStepIndex(), Is.EqualTo(0));
    }

    [Test]
    public void StartNewGame_RecordsAnActionAtTheStepWhereItIsProcessed()
    {
        EventManager event_manager = new EventManager();
        GameManager game_manager = CreateGameManager(event_manager);
        game_manager.StartNewGame(1234567);
        RunSteps(game_manager, event_manager, 2);

        PlayAction(game_manager, event_manager, GameActionType.CANCEL);

        Assert.That(game_manager.GetSaveText(), Does.Contain("0;SEED;1234567"));
        Assert.That(game_manager.GetSaveText(), Does.Contain("3;ACTION;CANCEL"));
    }

    [Test]
    public void LoadReplay_ReplaysEachActionAtItsStepAndRecordsItAgain()
    {
        EventManager played_event_manager = new EventManager();
        GameManager played_game = CreateGameManager(played_event_manager);
        played_game.StartNewGame(1234567);
        RunSteps(played_game, played_event_manager, 4);
        PlayAction(played_game, played_event_manager, GameActionType.CANCEL);
        RunSteps(played_game, played_event_manager, 3);
        ContentReport report = new ContentReport();
        ReplayData replay_data = ReplayReader.Read(CsvReader.Read("save.csv", played_game.GetSaveText(), report), report);

        EventManager loaded_event_manager = new EventManager();
        GameManager loaded_game = CreateGameManager(loaded_event_manager);
        loaded_game.LoadReplay(replay_data);
        RunSteps(loaded_game, loaded_event_manager, played_game.GetFixedStepIndex());

        Assert.That(report.GetErrorCount(), Is.EqualTo(0));
        Assert.That(loaded_game.GetFixedStepIndex(), Is.EqualTo(played_game.GetFixedStepIndex()));
        Assert.That(loaded_game.GetSaveText(), Is.EqualTo(played_game.GetSaveText()));
    }
}
