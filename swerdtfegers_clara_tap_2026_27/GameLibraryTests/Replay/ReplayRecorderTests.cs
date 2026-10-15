using activity_00_tap_26_27.Core;
using activity_00_tap_26_27.Core.Events;
using GameLibrary;
using GameLibrary.Events;
using GameLibrary.Replay;

namespace GameLibraryTests.Replay
{
    public class ReplayRecorderTests
    {
        // Une horloge dont le test choisit le pas : le recorder ne compte rien lui-même.
        private class FixedStepClockStub : IFixedStepClock
        {
            public int _fixedStepIndex = 0;

            public int GetFixedStepIndex()
            {
                return _fixedStepIndex;
            }
        }

        private ReplayRecorder CreateRecorder()
        {
            return new ReplayRecorder(1234567, new EventManager(), new FixedStepClockStub());
        }

        [Test]
        public void GetCsvTextWithoutEnd_StartsWithHeaderAndSeed()
        {
            ReplayRecorder recorder = CreateRecorder();

            string csv_text = recorder.GetCsvTextWithoutEnd();

            Assert.That(csv_text, Does.StartWith("Step;Type;Value"));
            Assert.That(csv_text, Does.Contain("0;SEED;1234567"));
        }

        [Test]
        public void Record_IncreasesActionCount()
        {
            ReplayRecorder recorder = CreateRecorder();

            recorder.Record(112, GameActionType.NAVIGATE_DOWN);
            recorder.Record(140, GameActionType.CONFIRM);

            Assert.That(recorder.GetActionCount(), Is.EqualTo(2));
        }

        [Test]
        public void GetCsvTextWithoutEnd_WritesActionsInOrderWithTheirStep()
        {
            ReplayRecorder recorder = CreateRecorder();
            recorder.Record(112, GameActionType.NAVIGATE_DOWN);
            recorder.Record(140, GameActionType.CONFIRM);

            string csv_text = recorder.GetCsvTextWithoutEnd();

            int first_index = csv_text.IndexOf("112;ACTION;NAVIGATE_DOWN");
            int second_index = csv_text.IndexOf("140;ACTION;CONFIRM");
            Assert.That(first_index, Is.GreaterThan(0));
            Assert.That(second_index, Is.GreaterThan(first_index));
        }

        [Test]
        public void GetCsvTextWithoutEnd_HasNoEndLine()
        {
            ReplayRecorder recorder = CreateRecorder();
            recorder.Record(112, GameActionType.CONFIRM);

            string csv_text = recorder.GetCsvTextWithoutEnd();

            Assert.That(csv_text, Does.Not.Contain("END"));
        }

        [Test]
        public void GetCsvText_EndsWithEndLine()
        {
            ReplayRecorder recorder = CreateRecorder();

            recorder.Record(3215, GameActionType.QUIT);

            string csv_text = recorder.GetCsvText(3215);

            Assert.That(csv_text.TrimEnd(), Does.EndWith("3215;END;"));
        }

        [Test]
        public void Constructor_SubscribesToActionsAndRecordsThemAtTheClockStep()
        {
            EventManager event_manager = new EventManager();
            FixedStepClockStub clock = new FixedStepClockStub();
            ReplayRecorder recorder = new ReplayRecorder(1234567, event_manager, clock);
            clock._fixedStepIndex = 42;

            event_manager.TriggerEvent(new GameActionGameEvent(GameActionType.CONFIRM));

            Assert.That(recorder.GetActionCount(), Is.EqualTo(1));
            Assert.That(recorder.GetCsvTextWithoutEnd(), Does.Contain("42;ACTION;CONFIRM"));
        }

        [Test]
        public void Constructor_RecordsDelayedActionsWhenTheyAreProcessed()
        {
            EventManager event_manager = new EventManager();
            FixedStepClockStub clock = new FixedStepClockStub();
            ReplayRecorder recorder = new ReplayRecorder(1234567, event_manager, clock);

            clock._fixedStepIndex = 7;
            event_manager.TriggerDelayedEvent(new GameActionGameEvent(GameActionType.NAVIGATE_DOWN));
            clock._fixedStepIndex = 8;
            event_manager.ProcessDelayedEvents();

            Assert.That(recorder.GetCsvTextWithoutEnd(), Does.Contain("8;ACTION;NAVIGATE_DOWN"));
        }

        [Test]
        public void StopRecording_IgnoresTheFollowingActions()
        {
            EventManager event_manager = new EventManager();
            ReplayRecorder recorder = new ReplayRecorder(1234567, event_manager, new FixedStepClockStub());
            event_manager.TriggerEvent(new GameActionGameEvent(GameActionType.CONFIRM));

            recorder.StopRecording();
            event_manager.TriggerEvent(new GameActionGameEvent(GameActionType.QUIT));

            Assert.That(recorder.GetActionCount(), Is.EqualTo(1));
        }
    }
}
