using GameLibrary;
using GameLibrary.Components;
using GameLibrary.Csv;
using GameLibrary.Data;
using GameLibrary.Events;
using GameLibrary.Replay;
using GameLibrary.States;

namespace GameLibraryTests
{
    // Fiches 04 et 05 : le filet de sécurité du cours.
    // Une partie est jouée, enregistrée, relue puis rejouée sur un nouveau GameManager.
    // Les deux parties doivent finir dans le même état. Si un test casse un jour,
    // une source de variation (temps, entrées, hasard) s'est glissée dans le jeu.
    //
    // Le scénario : déplacements, visite du magasin avec un CONFIRM sur un objet (fiche 04),
    // puis trois coups de pioche dont un annulé (fiche 05).
    //
    // Le monde : WORLD → DAISY_TOWN → GUN_SHOP, et WORLD → MINE_LEVEL_1.
    public class ReplayTests
    {
        private const float FIXED_DELTA_TIME = 0.02f;
        private const int SEED = 1234567;

        // ADAPTEZ : la durée d'un coup en pas fixes (voir MineStateTests).
        private const int MINUTES_PER_STRIKE = 2;
        private const int STRIKE_STEP_COUNT = 10;

        private const int COLT_STOCK_AT_START = 3;
        private const int VEIN_STOCK_AT_START = 10;

        private static readonly string[] ORE_ID_TABLE = new string[] { "COPPER_ORE", "SILVER_ORE", "GOLD_NUGGET" };

        // ADAPTEZ : remplissez votre GameContentData (noms des champs mines et minerais).
        private GameContentData CreateGameContent()
        {
            GameContentData game_content_data = new GameContentData();

            game_content_data._locationDataTable = new List<LocationData>();
            game_content_data._locationDataTable.Add(new LocationData("WORLD", "World", "", -1));
            game_content_data._locationDataTable.Add(new LocationData("DAISY_TOWN", "Daisy Town", "WORLD", 180));
            game_content_data._locationDataTable.Add(new LocationData("GUN_SHOP", "Gun Shop", "DAISY_TOWN", 5));
            game_content_data._locationDataTable.Add(new LocationData("MINE_LEVEL_1", "Mine Level 1", "WORLD", 240));
            game_content_data._shopDataTable = new List<ShopData>();
            game_content_data._shopDataTable.Add(new ShopData("GUN_SHOP", 8, 18));
            game_content_data._itemDataTable = new Dictionary<string, ItemData>();
            game_content_data._itemDataTable.Add("COLT", new ItemData("COLT", "Colt Peacemaker", 50));
            game_content_data._itemDataTable.Add("COPPER_ORE", new ItemData("COPPER_ORE", "Copper ore", 2));
            game_content_data._itemDataTable.Add("SILVER_ORE", new ItemData("SILVER_ORE", "Silver ore", 8));
            game_content_data._itemDataTable.Add("GOLD_NUGGET", new ItemData("GOLD_NUGGET", "Gold nugget", 40));
            game_content_data._inventoryDataTable = new List<InventoryEntryData>();
            game_content_data._inventoryDataTable.Add(new InventoryEntryData("GUN_SHOP", "COLT", COLT_STOCK_AT_START));
            game_content_data._heroDataTable = new List<HeroData>();
            game_content_data._heroDataTable.Add(new HeroData("LUCKY_LUKE", "Lucky Luke", 100, 40));
            game_content_data._mineDataTable = new List<MineData>();
            game_content_data._mineDataTable.Add(new MineData("MINE_LEVEL_1", MINUTES_PER_STRIKE, VEIN_STOCK_AT_START));
            game_content_data._mineOreDataTable = new List<MineOreData>();
            game_content_data._mineOreDataTable.Add(new MineOreData("MINE_LEVEL_1", "COPPER_ORE", 60));
            game_content_data._mineOreDataTable.Add(new MineOreData("MINE_LEVEL_1", "SILVER_ORE", 35));
            game_content_data._mineOreDataTable.Add(new MineOreData("MINE_LEVEL_1", "GOLD_NUGGET", 5));

            return game_content_data;
        }

        // ADAPTEZ : le constructeur de votre GameManager, et son initialisation s'il en a une.
        private GameManager CreateGameManager(EventManager event_manager)
        {
            return new GameManager(event_manager, CreateGameContent());
        }

        // ADAPTEZ : ce qu'il faut pour être en exploration à la racine, avec la graine donnée.
        private void StartGame(GameManager game_manager)
        {
            game_manager.StartNewGame(SEED);

            Assert.That(game_manager.GetCurrentGameFlowState(), Is.InstanceOf<ExploringState>(), "Après le démarrage, le jeu doit être en exploration.");
        }

        // ADAPTEZ : ce qu'il faut pour charger une sauvegarde et être en exploration,
        // comme après CONFIRM sur une sauvegarde du menu.
        private void LoadGame(GameManager game_manager, ReplayData replay_data)
        {
            game_manager.LoadReplay(replay_data);

            Assert.That(game_manager.GetCurrentGameFlowState(), Is.InstanceOf<ExploringState>(), "Après le chargement, le jeu doit être en exploration.");
        }

        // ADAPTEZ : le filon restant de la mine où se trouve le joueur.
        private int GetVeinStock(GameManager game_manager)
        {
            return game_manager.GetCurrentLocation().GetGameObject().GetComponent<MineComponent>().GetStock();
        }

        // Le stock du magasin : on remonte à la racine, puis on cherche le lieu par son Id.
        private int GetShopStock(GameManager game_manager, string shop_id, string item_id)
        {
            LocationComponent root_location = game_manager.GetCurrentLocation();

            while (root_location.GetParentLocation() != null)
            {
                root_location = root_location.GetParentLocation();
            }

            LocationComponent shop_location = FindLocation(root_location, shop_id);

            return shop_location.GetGameObject().GetComponent<InventoryComponent>().GetQuantity(item_id);
        }

        private LocationComponent FindLocation(LocationComponent location, string location_id)
        {
            LocationComponent found_location = null;

            if (location.GetGameObject().GetName() == location_id)
            {
                found_location = location;
            }

            for (int destination_index = 0; (destination_index < location.GetDestinationCount()) && (found_location == null); destination_index++)
            {
                found_location = FindLocation(location.GetDestinationAtIndex(destination_index).GetDestination(), location_id);
            }

            return found_location;
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

        private GameManager PlayScenario(EventManager event_manager)
        {
            GameManager game_manager = CreateGameManager(event_manager);

            StartGame(game_manager);
            RunSteps(game_manager, event_manager, 10);

            // Fiche 04 : WORLD → DAISY_TOWN → GUN_SHOP, CONFIRM sur le premier objet, retour.
            PlayAction(game_manager, event_manager, GameActionType.NAVIGATE_DOWN);
            PlayAction(game_manager, event_manager, GameActionType.CONFIRM);
            RunSteps(game_manager, event_manager, 5);
            PlayAction(game_manager, event_manager, GameActionType.NAVIGATE_DOWN);
            PlayAction(game_manager, event_manager, GameActionType.CONFIRM);
            PlayAction(game_manager, event_manager, GameActionType.NAVIGATE_DOWN);
            PlayAction(game_manager, event_manager, GameActionType.CONFIRM);
            RunSteps(game_manager, event_manager, 5);
            PlayAction(game_manager, event_manager, GameActionType.CANCEL);
            PlayAction(game_manager, event_manager, GameActionType.QUIT);

            // Fiche 05 : WORLD → MINE_LEVEL_1 (deuxième destination), un coup, un coup annulé, un coup.
            PlayAction(game_manager, event_manager, GameActionType.NAVIGATE_DOWN);
            PlayAction(game_manager, event_manager, GameActionType.NAVIGATE_DOWN);
            PlayAction(game_manager, event_manager, GameActionType.CONFIRM);
            PlayAction(game_manager, event_manager, GameActionType.CONFIRM);
            RunSteps(game_manager, event_manager, STRIKE_STEP_COUNT);
            PlayAction(game_manager, event_manager, GameActionType.CONFIRM);
            RunSteps(game_manager, event_manager, 3);
            PlayAction(game_manager, event_manager, GameActionType.CANCEL);
            PlayAction(game_manager, event_manager, GameActionType.CONFIRM);
            RunSteps(game_manager, event_manager, STRIKE_STEP_COUNT);
            RunSteps(game_manager, event_manager, 7);

            return game_manager;
        }

        private GameManager ReplayInANewGame(GameManager played_game, EventManager loaded_event_manager)
        {
            ContentReport report = new ContentReport();
            ReplayData replay_data = ReplayReader.Read(CsvReader.Read("save.csv", played_game.GetSaveText(), report), report);
            GameManager loaded_game = CreateGameManager(loaded_event_manager);

            Assert.That(report.GetErrorCount(), Is.EqualTo(0), "La sauvegarde produite par le jeu doit se relire sans erreur.");

            LoadGame(loaded_game, replay_data);
            RunSteps(loaded_game, loaded_event_manager, played_game.GetFixedStepIndex());

            return loaded_game;
        }

        // Vérifie que le scénario fait bien ce qu'on croit, avant de comparer les deux parties.
        [Test]
        public void PlayScenario_BuysOneColtAndMinesTwice()
        {
            EventManager event_manager = new EventManager();

            GameManager played_game = PlayScenario(event_manager);

            Assert.That(played_game.GetCurrentLocation().GetGameObject().GetName(), Is.EqualTo("MINE_LEVEL_1"));
            Assert.That(GetShopStock(played_game, "GUN_SHOP", "COLT"), Is.EqualTo(COLT_STOCK_AT_START - 1));
            Assert.That(GetVeinStock(played_game), Is.EqualTo(VEIN_STOCK_AT_START - 2));
        }

        [Test]
        public void LoadReplay_ReachesTheSameStepAndTheSameLocation()
        {
            EventManager played_event_manager = new EventManager();
            GameManager played_game = PlayScenario(played_event_manager);

            EventManager loaded_event_manager = new EventManager();
            GameManager loaded_game = ReplayInANewGame(played_game, loaded_event_manager);

            Assert.That(loaded_game.GetFixedStepIndex(), Is.EqualTo(played_game.GetFixedStepIndex()));
            Assert.That(loaded_game.GetCurrentLocation().GetGameObject().GetName(), Is.EqualTo(played_game.GetCurrentLocation().GetGameObject().GetName()));
        }

        [Test]
        public void LoadReplay_GivesTheSameShopStock()
        {
            EventManager played_event_manager = new EventManager();
            GameManager played_game = PlayScenario(played_event_manager);

            EventManager loaded_event_manager = new EventManager();
            GameManager loaded_game = ReplayInANewGame(played_game, loaded_event_manager);

            Assert.That(GetShopStock(loaded_game, "GUN_SHOP", "COLT"), Is.EqualTo(GetShopStock(played_game, "GUN_SHOP", "COLT")));
        }

        [Test]
        public void LoadReplay_GivesTheSameOresAndTheSameVein()
        {
            EventManager played_event_manager = new EventManager();
            GameManager played_game = PlayScenario(played_event_manager);

            EventManager loaded_event_manager = new EventManager();
            GameManager loaded_game = ReplayInANewGame(played_game, loaded_event_manager);

            for (int ore_index = 0; ore_index < ORE_ID_TABLE.Length; ore_index++)
            {
                string ore_id = ORE_ID_TABLE[ore_index];

                Assert.That(loaded_game.GetHeroInventory().GetQuantity(ore_id), Is.EqualTo(played_game.GetHeroInventory().GetQuantity(ore_id)), ore_id);
            }

            Assert.That(GetVeinStock(loaded_game), Is.EqualTo(GetVeinStock(played_game)));
        }

        // La sauvegarde de la partie rejouée contient toute l'histoire, à l'identique.
        [Test]
        public void LoadReplay_RecordsTheSameSaveText()
        {
            EventManager played_event_manager = new EventManager();
            GameManager played_game = PlayScenario(played_event_manager);

            EventManager loaded_event_manager = new EventManager();
            GameManager loaded_game = ReplayInANewGame(played_game, loaded_event_manager);

            Assert.That(loaded_game.GetSaveText(), Is.EqualTo(played_game.GetSaveText()));
        }
    }
}
