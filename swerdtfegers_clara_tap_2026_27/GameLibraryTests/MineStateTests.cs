using GameLibrary;
using GameLibrary.Components;
using GameLibrary.Data;
using GameLibrary.Events;
using GameLibrary.States;

namespace GameLibraryTests
{
    // Fiche 05, partie 2 : le coup de pioche.
    // Les tests jouent comme un joueur, à travers GameManager : ils ne dépendent donc
    // pas du découpage de vos états (un état « mine » et un état « coup en cours »,
    // ou un seul état). Seules les méthodes marquées ADAPTEZ connaissent vos noms.
    //
    // Le monde : WORLD → MINE_LEVEL_1, un seul héros au sac vide.
    public class MineStateTests
    {
        private const float FIXED_DELTA_TIME = 0.02f;
        private const int SEED = 1234567;

        // ADAPTEZ : la durée d'un coup en pas fixes, avec la conversion des magasins.
        // Fiche 03 : 10 minutes de jeu par seconde réelle, pas de 0.02 s → 0.2 minute par pas.
        // 2 minutes par coup → 10 pas.
        private const int MINUTES_PER_STRIKE = 2;
        private const int STRIKE_STEP_COUNT = 10;

        private static readonly string[] ORE_ID_TABLE = new string[] { "COPPER_ORE", "SILVER_ORE", "GOLD_NUGGET" };

        // ADAPTEZ : remplissez votre GameContentData (noms des champs mines et minerais).
        private GameContentData CreateGameContent(int vein_stock)
        {
            GameContentData game_content_data = new GameContentData();

            game_content_data._locationDataTable = new List<LocationData>();
            game_content_data._locationDataTable.Add(new LocationData("WORLD", "World", "", -1));
            game_content_data._locationDataTable.Add(new LocationData("MINE_LEVEL_1", "Mine Level 1", "WORLD", 15));
            game_content_data._shopDataTable = new List<ShopData>();
            game_content_data._itemDataTable = new Dictionary<string, ItemData>();
            game_content_data._itemDataTable.Add("COPPER_ORE", new ItemData("COPPER_ORE", "Copper ore", 2));
            game_content_data._itemDataTable.Add("SILVER_ORE", new ItemData("SILVER_ORE", "Silver ore", 8));
            game_content_data._itemDataTable.Add("GOLD_NUGGET", new ItemData("GOLD_NUGGET", "Gold nugget", 40));
            game_content_data._inventoryDataTable = new List<InventoryEntryData>();
            game_content_data._heroDataTable = new List<HeroData>();
            game_content_data._heroDataTable.Add(new HeroData("LUCKY_LUKE", "Lucky Luke", 100, 40));
            game_content_data._mineDataTable = new List<MineData>();
            game_content_data._mineDataTable.Add(new MineData("MINE_LEVEL_1", MINUTES_PER_STRIKE, vein_stock));
            game_content_data._mineOreDataTable = new List<MineOreData>();
            game_content_data._mineOreDataTable.Add(new MineOreData("MINE_LEVEL_1", "COPPER_ORE", 60));
            game_content_data._mineOreDataTable.Add(new MineOreData("MINE_LEVEL_1", "SILVER_ORE", 35));
            game_content_data._mineOreDataTable.Add(new MineOreData("MINE_LEVEL_1", "GOLD_NUGGET", 5));

            return game_content_data;
        }

        // ADAPTEZ : le constructeur de votre GameManager, et son initialisation s'il en a une.
        private GameManager CreateGameManager(EventManager event_manager, int vein_stock)
        {
            return new GameManager(event_manager, CreateGameContent(vein_stock));
        }

        // ADAPTEZ : ce qu'il faut pour être en exploration à la racine, avec la graine donnée.
        private void StartGame(GameManager game_manager)
        {
            game_manager.StartNewGame(SEED);

            Assert.That(game_manager.GetCurrentGameFlowState(), Is.InstanceOf<ExploringState>(), "Après le démarrage, le jeu doit être en exploration.");
        }

        // ADAPTEZ : le filon restant de la mine où se trouve le joueur.
        private int GetVeinStock(GameManager game_manager)
        {
            return game_manager.GetCurrentLocation().GetGameObject().GetComponent<MineComponent>().GetStock();
        }

        private int GetOreQuantity(GameManager game_manager, string ore_id)
        {
            return game_manager.GetHeroInventory().GetQuantity(ore_id);
        }

        private int GetTotalOreQuantity(GameManager game_manager)
        {
            int total_quantity = 0;

            for (int ore_index = 0; ore_index < ORE_ID_TABLE.Length; ore_index++)
            {
                total_quantity += GetOreQuantity(game_manager, ORE_ID_TABLE[ore_index]);
            }

            return total_quantity;
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

        private GameManager StartGameInMine(EventManager event_manager, int vein_stock)
        {
            GameManager game_manager = CreateGameManager(event_manager, vein_stock);

            StartGame(game_manager);
            PlayAction(game_manager, event_manager, GameActionType.NAVIGATE_DOWN);
            PlayAction(game_manager, event_manager, GameActionType.CONFIRM);

            return game_manager;
        }

        private void PlayFullStrike(GameManager game_manager, EventManager event_manager)
        {
            PlayAction(game_manager, event_manager, GameActionType.CONFIRM);
            RunSteps(game_manager, event_manager, STRIKE_STEP_COUNT);
        }

        [Test]
        public void Strike_AddsOneOreToTheHeroAndRemovesOneFromTheVein()
        {
            EventManager event_manager = new EventManager();
            GameManager game_manager = StartGameInMine(event_manager, 10);

            PlayFullStrike(game_manager, event_manager);

            Assert.That(GetTotalOreQuantity(game_manager), Is.EqualTo(1));
            Assert.That(GetVeinStock(game_manager), Is.EqualTo(9));
        }

        [Test]
        public void Strike_ChangesNothingBeforeItsLastStep()
        {
            EventManager event_manager = new EventManager();
            GameManager game_manager = StartGameInMine(event_manager, 10);
            PlayAction(game_manager, event_manager, GameActionType.CONFIRM);

            RunSteps(game_manager, event_manager, STRIKE_STEP_COUNT - 1);
            int ore_quantity_before_last_step = GetTotalOreQuantity(game_manager);
            int vein_stock_before_last_step = GetVeinStock(game_manager);
            RunSteps(game_manager, event_manager, 1);

            Assert.That(ore_quantity_before_last_step, Is.EqualTo(0));
            Assert.That(vein_stock_before_last_step, Is.EqualTo(10));
            Assert.That(GetTotalOreQuantity(game_manager), Is.EqualTo(1));
            Assert.That(GetVeinStock(game_manager), Is.EqualTo(9));
        }

        [Test]
        public void Confirm_DuringAStrikeIsIgnored()
        {
            EventManager event_manager = new EventManager();
            GameManager game_manager = StartGameInMine(event_manager, 10);
            PlayAction(game_manager, event_manager, GameActionType.CONFIRM);
            RunSteps(game_manager, event_manager, 3);

            PlayAction(game_manager, event_manager, GameActionType.CONFIRM);
            RunSteps(game_manager, event_manager, STRIKE_STEP_COUNT);

            Assert.That(GetTotalOreQuantity(game_manager), Is.EqualTo(1));
            Assert.That(GetVeinStock(game_manager), Is.EqualTo(9));
        }

        [Test]
        public void Cancel_DuringAStrikeChangesNeitherTheInventoryNorTheVein()
        {
            EventManager event_manager = new EventManager();
            GameManager game_manager = StartGameInMine(event_manager, 10);
            PlayAction(game_manager, event_manager, GameActionType.CONFIRM);
            RunSteps(game_manager, event_manager, STRIKE_STEP_COUNT / 2);

            PlayAction(game_manager, event_manager, GameActionType.CANCEL);
            RunSteps(game_manager, event_manager, STRIKE_STEP_COUNT);

            Assert.That(GetTotalOreQuantity(game_manager), Is.EqualTo(0));
            Assert.That(GetVeinStock(game_manager), Is.EqualTo(10));
        }

        // Une partie avec un coup annulé, une partie sans : même graine, mêmes minerais.
        // Si le jet était tiré au début du coup, la suite serait décalée d'un cran.
        [Test]
        public void Cancel_DuringAStrikeDoesNotChangeTheFollowingDraws()
        {
            EventManager cancelled_event_manager = new EventManager();
            GameManager game_with_cancel = StartGameInMine(cancelled_event_manager, 10);
            EventManager reference_event_manager = new EventManager();
            GameManager game_without_cancel = StartGameInMine(reference_event_manager, 10);

            PlayAction(game_with_cancel, cancelled_event_manager, GameActionType.CONFIRM);
            RunSteps(game_with_cancel, cancelled_event_manager, STRIKE_STEP_COUNT / 2);
            PlayAction(game_with_cancel, cancelled_event_manager, GameActionType.CANCEL);

            for (int strike_index = 0; strike_index < 5; strike_index++)
            {
                PlayFullStrike(game_with_cancel, cancelled_event_manager);
                PlayFullStrike(game_without_cancel, reference_event_manager);
            }

            for (int ore_index = 0; ore_index < ORE_ID_TABLE.Length; ore_index++)
            {
                string ore_id = ORE_ID_TABLE[ore_index];

                Assert.That(GetOreQuantity(game_with_cancel, ore_id), Is.EqualTo(GetOreQuantity(game_without_cancel, ore_id)), ore_id);
            }

            Assert.That(GetVeinStock(game_with_cancel), Is.EqualTo(5));
        }

        [Test]
        public void Confirm_OnAnExhaustedVeinChangesNothing()
        {
            EventManager event_manager = new EventManager();
            GameManager game_manager = StartGameInMine(event_manager, 0);

            PlayFullStrike(game_manager, event_manager);
            RunSteps(game_manager, event_manager, STRIKE_STEP_COUNT);

            Assert.That(GetTotalOreQuantity(game_manager), Is.EqualTo(0));
            Assert.That(GetVeinStock(game_manager), Is.EqualTo(0));
        }

        [Test]
        public void Confirm_IsRefusedOnceTheVeinIsExhausted()
        {
            EventManager event_manager = new EventManager();
            GameManager game_manager = StartGameInMine(event_manager, 2);

            PlayFullStrike(game_manager, event_manager);
            PlayFullStrike(game_manager, event_manager);
            PlayFullStrike(game_manager, event_manager);

            Assert.That(GetTotalOreQuantity(game_manager), Is.EqualTo(2));
            Assert.That(GetVeinStock(game_manager), Is.EqualTo(0));
        }

        [Test]
        public void Cancel_OutsideAStrikeReturnsToExploration()
        {
            EventManager event_manager = new EventManager();
            GameManager game_manager = StartGameInMine(event_manager, 10);

            PlayAction(game_manager, event_manager, GameActionType.CANCEL);

            Assert.That(game_manager.GetCurrentGameFlowState(), Is.InstanceOf<ExploringState>());
        }
    }
}
