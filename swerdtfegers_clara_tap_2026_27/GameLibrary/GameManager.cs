using activity_00_tap_26_27.Core;
using activity_00_tap_26_27.Core.Events;
using GameLibrary.Components;
using GameLibrary.Csv;
using GameLibrary.CSV;
using GameLibrary.Data;
using GameLibrary.Events;
using GameLibrary.Replay;
using GameLibrary.States;
using System;
using System.Collections.Generic;

namespace GameLibrary
{
    public class GameManager : IFixedStepClock
    {
        private readonly EventManager _eventManager;
        private readonly StateMachine _gameFlowStateStateMachine;
        private readonly List<GameObject> _gameObjectTable = new List<GameObject>();

        private LocationComponent _currentLocation;
        private int _selectedChildLocationIndex = -1;
        private int _fixedStepIndex = 0;

        private bool _shouldQuit = false;

        private readonly WorldBuilder _worldBuilder;
        private readonly HeroBuilder _heroBuilder;
        private ReplayRecorder _replayRecorder;
        private ReplayPlayer _replayPlayer;

        private readonly Dictionary<string, string> _saveTable = new Dictionary<string, string>();
        private int _replaySpeed;
        private Random _random;
        private GameObject _heroObject;

        public GameManager(EventManager event_manager, GameContentData game_content_data)
        {
            _eventManager = event_manager;
            _worldBuilder = new WorldBuilder(game_content_data);
            _heroBuilder = new HeroBuilder(game_content_data);

            _gameFlowStateStateMachine = new StateMachine(_eventManager);

            event_manager.TriggerEvent(new LogMessageGameEvent("Game manager started."));
        }

        public void StartNewGame(int seed)
        {
            if (_replayRecorder != null)
            {
                _replayRecorder.StopRecording();
                _gameObjectTable.Clear();
            }

            _eventManager.TriggerEvent(new LogMessageGameEvent($"New game started.  - seed {seed}"));

            _random = new Random(seed);
            _replayRecorder = new ReplayRecorder(seed, _eventManager, this);
            _replayPlayer = null;
            _fixedStepIndex = 0;
            LocationComponent root_location = _worldBuilder.Build(_eventManager);
            _heroObject = _heroBuilder.Build(_eventManager);

            _currentLocation = root_location;
        }

        public InventoryComponent GetHeroInventory()
        {
            return _heroObject.GetComponent<InventoryComponent>();
        }

        public WalletComponent GetHeroWallet()
        {
            return _heroObject.GetComponent<WalletComponent>();
        }

        public string GetSaveText()
        {
            string save_text = "";

            if (_replayRecorder != null)
            {
                save_text = _replayRecorder.GetCsvTextWithoutEnd();
            }

            return save_text;
        }

        public void LoadReplay(ReplayData replay_data)
        {
            StartNewGame(replay_data._seed);

            _replayPlayer = new ReplayPlayer(replay_data, _eventManager, this);

        }

        public IState GetCurrentGameFlowState()
        {
            return _gameFlowStateStateMachine.GetCurrentState();
        }

        public int GetSelectedDestinationIndex()
        {
            return _selectedChildLocationIndex;
        }

        private void OnRegisterGameObjectGameEvent(IGameEvent game_event)
        {
            RegisterGameObjectGameEvent register_game_object_game_event = game_event as RegisterGameObjectGameEvent;

            _gameObjectTable.Add(register_game_object_game_event._gameObject);
        }

        private void OnUnregisterGameObjectGameEvent(IGameEvent game_event)
        {
            UnregisterGameObjectGameEvent unregister_game_object_game_event = game_event as UnregisterGameObjectGameEvent;

            _gameObjectTable.Remove(unregister_game_object_game_event._gameObject);
        }

        public void FixedUpdate(float fixed_elapsed_time)
        {
            ++_fixedStepIndex;

            _gameFlowStateStateMachine.FixedUpdate(fixed_elapsed_time);

            for (int object_index = 0; object_index < _gameObjectTable.Count; object_index++)
            {
                GameObject game_object = _gameObjectTable[object_index];

                if (game_object.GetIsActive())
                {
                    game_object.FixedUpdate(fixed_elapsed_time);
                }
            }

            if (_replayPlayer != null)
            {
                _replayPlayer.FixedUpdate(fixed_elapsed_time);
            }
        }

        public void Update(float elapsed_time)
        {
            _gameFlowStateStateMachine.Update(elapsed_time);

            for (int object_index = 0; object_index < _gameObjectTable.Count; object_index++)
            {
                GameObject game_object = _gameObjectTable[object_index];

                if (game_object.GetIsActive())
                {
                    game_object.Update(elapsed_time);
                }
            }
        }

        public LocationComponent GetCurrentLocation()
        {
            return _currentLocation;
        }

        public bool GetShouldQuit()
        {
            return _shouldQuit;
        }

        public void RequestQuit()
        {
            _shouldQuit = true;
        }

        public void SetSelectedChildLocationIndex(int selected_child_location_index)
        {
            _selectedChildLocationIndex = selected_child_location_index;
        }

        public void SetCurrentLocation(LocationComponent current_location)
        {
            _currentLocation = current_location;
            SetSelectedChildLocationIndex(-1);
        }

        public void RequestSave()
        {
            string save_text = _replayRecorder.GetCsvText(_fixedStepIndex);

            _eventManager.TriggerEvent(new SaveRequestedGameEvent(save_text));
        }

        public void AddSave(string save_file_path, string save_content)
        {
            ContentReport content_report = new ContentReport();
            CsvTable csv_table = CsvReader.Read(save_file_path, save_content, content_report);
            ReplayData replay_save_data = ReplayReader.Read(csv_table, content_report);

            if (content_report.GetErrorCount() > 0)
            {
                save_file_path += " (damaged)";

                for (int error_index = 0; error_index < content_report.GetErrorCount(); error_index++)
                {
                    _eventManager.TriggerEvent(new LogMessageGameEvent(content_report.GetErrorTextAtIndex(error_index)));
                }
            }

            _saveTable.Add(save_file_path, save_content);
        }

        public Dictionary<string, string> GetSaveTable()
        {
            return _saveTable;
        }

        public int GetFixedStepIndex()
        {
            return _fixedStepIndex;
        }

        public void EndGame()
        {
            _replayRecorder.StopRecording();
            SetCurrentLocation(null);
            RequestSave();
        }

        public void SetReplaySpeed(int replay_speed)
        {
            _replaySpeed = replay_speed;
        }

        public bool GetIsReplaying()
        {
            return _replayPlayer != null && !_replayPlayer.GetIsFinished(_fixedStepIndex);
        }

        public int GetReplaySpeed()
        {
            return _replaySpeed;
        }

        public void ShowMenu()
        {
            _gameFlowStateStateMachine.SetInitialState(new MainMenuState(_gameFlowStateStateMachine, this, _eventManager));

            _eventManager.RegisterToEvent<RegisterGameObjectGameEvent>(OnRegisterGameObjectGameEvent);
            _eventManager.RegisterToEvent<UnregisterGameObjectGameEvent>(OnUnregisterGameObjectGameEvent);
        }

        public GameObject GetHeroObject()
        {
            return _heroObject;
        }
    }
}