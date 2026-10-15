using activity_00_tap_26_27.Core;
using activity_00_tap_26_27.Core.Events;
using GameLibrary;
using GameLibrary.Events;
using GameLibrary.Replay;

namespace GameLibraryTests.Replay
{
    public class ReplayPlayerTests
    {
        private const int NO_END = -1;

        private List<GameActionType> _receivedActions = new List<GameActionType>();

        private class FixedStepClockStub : IFixedStepClock
        {
            public int _fixedStepIndex = 0;

            public int GetFixedStepIndex()
            {
                return _fixedStepIndex;
            }
        }

        [SetUp]
        public void SetUp()
        {
            _receivedActions = new List<GameActionType>();
        }

        private EventManager CreateListenedEventManager()
        {
            EventManager event_manager = new EventManager();

            event_manager.RegisterToEvent<GameActionGameEvent>(OnGameActionGameEvent);

            return event_manager;
        }

        private void OnGameActionGameEvent(IGameEvent game_event)
        {
            GameActionGameEvent action_event = (GameActionGameEvent)game_event;

            _receivedActions.Add(action_event._gameActionType);
        }

        [Test]
        public void TriggerActionsOfStep_TriggersNothingBeforeTheActionStep()
        {
            EventManager event_manager = CreateListenedEventManager();
            List<ReplayActionData> actions = new List<ReplayActionData>();
            actions.Add(new ReplayActionData(5, GameActionType.CONFIRM));
            ReplayPlayer replay_player = new ReplayPlayer(new ReplayData(1234567, actions, NO_END), event_manager, new FixedStepClockStub());

            replay_player.TriggerActionsOfStep(4, event_manager);
            event_manager.ProcessDelayedEvents();

            Assert.That(_receivedActions.Count, Is.EqualTo(0));
        }

        [Test]
        public void TriggerActionsOfStep_TriggersTheActionDelayed()
        {
            EventManager event_manager = CreateListenedEventManager();
            List<ReplayActionData> actions = new List<ReplayActionData>();
            actions.Add(new ReplayActionData(5, GameActionType.CONFIRM));
            ReplayPlayer replay_player = new ReplayPlayer(new ReplayData(1234567, actions, NO_END), event_manager, new FixedStepClockStub());

            replay_player.TriggerActionsOfStep(5, event_manager);
            int count_before_processing = _receivedActions.Count;
            event_manager.ProcessDelayedEvents();

            Assert.That(count_before_processing, Is.EqualTo(0));
            Assert.That(_receivedActions.Count, Is.EqualTo(1));
            Assert.That(_receivedActions[0], Is.EqualTo(GameActionType.CONFIRM));
        }

        [Test]
        public void TriggerActionsOfStep_TriggersTwoActionsOfTheSameStepInOrder()
        {
            EventManager event_manager = CreateListenedEventManager();
            List<ReplayActionData> actions = new List<ReplayActionData>();
            actions.Add(new ReplayActionData(5, GameActionType.NAVIGATE_DOWN));
            actions.Add(new ReplayActionData(5, GameActionType.CONFIRM));
            ReplayPlayer replay_player = new ReplayPlayer(new ReplayData(1234567, actions, NO_END), event_manager, new FixedStepClockStub());

            replay_player.TriggerActionsOfStep(5, event_manager);
            event_manager.ProcessDelayedEvents();

            Assert.That(_receivedActions.Count, Is.EqualTo(2));
            Assert.That(_receivedActions[0], Is.EqualTo(GameActionType.NAVIGATE_DOWN));
            Assert.That(_receivedActions[1], Is.EqualTo(GameActionType.CONFIRM));
        }

        [Test]
        public void TriggerActionsOfStep_TriggersEachActionOnceWhenCalledForEveryStep()
        {
            EventManager event_manager = CreateListenedEventManager();
            List<ReplayActionData> actions = new List<ReplayActionData>();
            actions.Add(new ReplayActionData(2, GameActionType.NAVIGATE_DOWN));
            actions.Add(new ReplayActionData(4, GameActionType.CONFIRM));
            ReplayPlayer replay_player = new ReplayPlayer(new ReplayData(1234567, actions, NO_END), event_manager, new FixedStepClockStub());

            for (int step_index = 1; step_index <= 6; step_index++)
            {
                replay_player.TriggerActionsOfStep(step_index, event_manager);
                event_manager.ProcessDelayedEvents();
            }

            Assert.That(_receivedActions.Count, Is.EqualTo(2));
        }

        [Test]
        public void TriggerActionsOfStep_TriggersNothingAtTheEndStep()
        {
            EventManager event_manager = CreateListenedEventManager();
            List<ReplayActionData> actions = new List<ReplayActionData>();
            actions.Add(new ReplayActionData(10, GameActionType.QUIT));
            ReplayPlayer replay_player = new ReplayPlayer(new ReplayData(1234567, actions, 10), event_manager, new FixedStepClockStub());

            replay_player.TriggerActionsOfStep(10, event_manager);
            event_manager.ProcessDelayedEvents();

            Assert.That(_receivedActions.Count, Is.EqualTo(0));
        }

        [Test]
        public void GetIsFinished_ReturnsTrueOnlyFromTheEndStep()
        {
            List<ReplayActionData> actions = new List<ReplayActionData>();
            actions.Add(new ReplayActionData(3, GameActionType.CONFIRM));
            ReplayPlayer replay_player = new ReplayPlayer(new ReplayData(1234567, actions, 10), new EventManager(), new FixedStepClockStub());

            bool is_finished_after_last_action = replay_player.GetIsFinished(4);
            bool is_finished_before_end = replay_player.GetIsFinished(9);
            bool is_finished_at_end = replay_player.GetIsFinished(10);

            Assert.That(is_finished_after_last_action, Is.False);
            Assert.That(is_finished_before_end, Is.False);
            Assert.That(is_finished_at_end, Is.True);
        }

        [Test]
        public void GetIsFinished_ReturnsTrueAfterTheLastActionWhenThereIsNoEnd()
        {
            List<ReplayActionData> actions = new List<ReplayActionData>();
            actions.Add(new ReplayActionData(3, GameActionType.CONFIRM));
            ReplayPlayer replay_player = new ReplayPlayer(new ReplayData(1234567, actions, NO_END), new EventManager(), new FixedStepClockStub());

            bool is_finished_at_last_action = replay_player.GetIsFinished(3);
            bool is_finished_after_last_action = replay_player.GetIsFinished(4);

            Assert.That(is_finished_at_last_action, Is.False);
            Assert.That(is_finished_after_last_action, Is.True);
        }

        [Test]
        public void GetIsFinished_ReturnsTrueAtOnceWithoutActionAndWithoutEnd()
        {
            ReplayPlayer replay_player = new ReplayPlayer(new ReplayData(1234567, new List<ReplayActionData>(), NO_END), new EventManager(), new FixedStepClockStub());

            bool is_finished = replay_player.GetIsFinished(1);

            Assert.That(is_finished, Is.True);
        }

        [Test]
        public void FixedUpdate_TriggersTheActionsOfTheClockStep()
        {
            EventManager event_manager = CreateListenedEventManager();
            FixedStepClockStub clock = new FixedStepClockStub();
            List<ReplayActionData> actions = new List<ReplayActionData>();
            actions.Add(new ReplayActionData(5, GameActionType.CONFIRM));
            ReplayPlayer replay_player = new ReplayPlayer(new ReplayData(1234567, actions, NO_END), event_manager, clock);

            clock._fixedStepIndex = 4;
            replay_player.FixedUpdate(0.02f);
            event_manager.ProcessDelayedEvents();
            int count_at_step_four = _receivedActions.Count;
            clock._fixedStepIndex = 5;
            replay_player.FixedUpdate(0.02f);
            event_manager.ProcessDelayedEvents();

            Assert.That(count_at_step_four, Is.EqualTo(0));
            Assert.That(_receivedActions.Count, Is.EqualTo(1));
        }

        [Test]
        public void FixedUpdate_TriggersNothingFromTheEndStep()
        {
            EventManager event_manager = CreateListenedEventManager();
            FixedStepClockStub clock = new FixedStepClockStub();
            List<ReplayActionData> actions = new List<ReplayActionData>();
            actions.Add(new ReplayActionData(10, GameActionType.QUIT));
            ReplayPlayer replay_player = new ReplayPlayer(new ReplayData(1234567, actions, 10), event_manager, clock);

            clock._fixedStepIndex = 10;
            replay_player.FixedUpdate(0.02f);
            event_manager.ProcessDelayedEvents();

            Assert.That(_receivedActions.Count, Is.EqualTo(0));
        }
    }
}
