using activity_00_tap_26_27.Core;
using activity_00_tap_26_27.Core.Events;
using GameLibrary;
using GameLibrary.Csv;
using GameLibrary.CSV;
using GameLibrary.Events;
using GameLibrary.Replay;

namespace GameLibraryTests.Replay
{
    public class ReplayReaderTests
    {
        private static readonly string[] REPLAY_HEADER = new string[] { "Step", "Type", "Value" };

        [Test]
        public void Read_ReturnsSeedActionsAndEndWithoutErrorForValidFile()
        {
            ContentReport report = new ContentReport();
            CsvTable csv_table = new CsvTable("save.csv");

            csv_table.SetHeader(REPLAY_HEADER);
            csv_table.AddRow(new CsvRow(2, new string[] { "0", "SEED", "1234567" }));
            csv_table.AddRow(new CsvRow(3, new string[] { "112", "ACTION", "NAVIGATE_DOWN" }));
            csv_table.AddRow(new CsvRow(4, new string[] { "140", "ACTION", "CONFIRM" }));
            csv_table.AddRow(new CsvRow(5, new string[] { "3215", "ACTION", "QUIT" }));
            csv_table.AddRow(new CsvRow(6, new string[] { "3215", "END", "" }));

            ReplayData replay_data = ReplayReader.Read(csv_table, report);

            Assert.That(report.GetErrorCount(), Is.EqualTo(0));
            Assert.That(replay_data._seed, Is.EqualTo(1234567));
            Assert.That(replay_data._actionTable.Count, Is.EqualTo(3));
            Assert.That(replay_data._actionTable[0]._fixedStepIndex, Is.EqualTo(112));
            Assert.That(replay_data._actionTable[0]._action, Is.EqualTo(GameActionType.NAVIGATE_DOWN));
            Assert.That(replay_data._actionTable[2]._action, Is.EqualTo(GameActionType.QUIT));
            Assert.That(replay_data._endFixedStepIndex, Is.EqualTo(3215));
        }

        [Test]
        public void Read_ReturnsEndStepMinusOneWhenThereIsNoEndLine()
        {
            ContentReport report = new ContentReport();
            CsvTable csv_table = new CsvTable("save.csv");

            csv_table.SetHeader(REPLAY_HEADER);
            csv_table.AddRow(new CsvRow(2, new string[] { "0", "SEED", "1234567" }));
            csv_table.AddRow(new CsvRow(3, new string[] { "112", "ACTION", "CONFIRM" }));

            ReplayData replay_data = ReplayReader.Read(csv_table, report);

            Assert.That(report.GetErrorCount(), Is.EqualTo(0));
            Assert.That(replay_data._endFixedStepIndex, Is.EqualTo(-1));
        }

        [Test]
        public void Read_KeepsTwoActionsOfTheSameStepInOrder()
        {
            ContentReport report = new ContentReport();
            CsvTable csv_table = new CsvTable("save.csv");

            csv_table.SetHeader(REPLAY_HEADER);
            csv_table.AddRow(new CsvRow(2, new string[] { "0", "SEED", "1234567" }));
            csv_table.AddRow(new CsvRow(3, new string[] { "112", "ACTION", "NAVIGATE_DOWN" }));
            csv_table.AddRow(new CsvRow(4, new string[] { "112", "ACTION", "CONFIRM" }));

            ReplayData replay_data = ReplayReader.Read(csv_table, report);

            Assert.That(report.GetErrorCount(), Is.EqualTo(0));
            Assert.That(replay_data._actionTable.Count, Is.EqualTo(2));
            Assert.That(replay_data._actionTable[1]._action, Is.EqualTo(GameActionType.CONFIRM));
        }

        [Test]
        public void Read_ReportsMissingColumn()
        {
            ContentReport report = new ContentReport();
            CsvTable csv_table = new CsvTable("save.csv");

            csv_table.SetHeader(new string[] { "Step", "Type" });
            csv_table.AddRow(new CsvRow(2, new string[] { "0", "SEED" }));

            ReplayReader.Read(csv_table, report);

            Assert.That(report.GetErrorCount(), Is.EqualTo(1));
            Assert.That(report.GetErrorTextAtIndex(0), Does.Contain("save.csv"));
            Assert.That(report.GetErrorTextAtIndex(0), Does.Contain("Value"));
        }

        [Test]
        public void Read_ReportsMissingSeedInEmptyFile()
        {
            ContentReport report = new ContentReport();
            CsvTable csv_table = new CsvTable("save.csv");

            csv_table.SetHeader(REPLAY_HEADER);

            ReplayReader.Read(csv_table, report);

            Assert.That(report.GetErrorCount(), Is.EqualTo(1));
            Assert.That(report.GetErrorTextAtIndex(0), Does.Contain("SEED"));
        }

        [Test]
        public void Read_ReportsStepThatIsNotAWholeNumber()
        {
            ContentReport report = new ContentReport();
            CsvTable csv_table = new CsvTable("save.csv");

            csv_table.SetHeader(REPLAY_HEADER);
            csv_table.AddRow(new CsvRow(2, new string[] { "0", "SEED", "1234567" }));
            csv_table.AddRow(new CsvRow(3, new string[] { "12.5", "ACTION", "CONFIRM" }));

            ReplayReader.Read(csv_table, report);

            Assert.That(report.GetErrorCount(), Is.EqualTo(1));
            Assert.That(report.GetErrorTextAtIndex(0), Does.Contain("line 3"));
            Assert.That(report.GetErrorTextAtIndex(0), Does.Contain("12.5"));
        }

        [Test]
        public void Read_ReportsNegativeStep()
        {
            ContentReport report = new ContentReport();
            CsvTable csv_table = new CsvTable("save.csv");

            csv_table.SetHeader(REPLAY_HEADER);
            csv_table.AddRow(new CsvRow(2, new string[] { "0", "SEED", "1234567" }));
            csv_table.AddRow(new CsvRow(3, new string[] { "-1", "ACTION", "CONFIRM" }));

            ReplayReader.Read(csv_table, report);

            Assert.That(report.GetErrorCount(), Is.EqualTo(1));
            Assert.That(report.GetErrorTextAtIndex(0), Does.Contain("line 3"));
            Assert.That(report.GetErrorTextAtIndex(0), Does.Contain("-1"));
        }

        [Test]
        public void Read_ReportsStepSmallerThanThePreviousOne()
        {
            ContentReport report = new ContentReport();
            CsvTable csv_table = new CsvTable("save.csv");

            csv_table.SetHeader(REPLAY_HEADER);
            csv_table.AddRow(new CsvRow(2, new string[] { "0", "SEED", "1234567" }));
            csv_table.AddRow(new CsvRow(3, new string[] { "140", "ACTION", "CONFIRM" }));
            csv_table.AddRow(new CsvRow(4, new string[] { "130", "ACTION", "CANCEL" }));

            ReplayReader.Read(csv_table, report);

            Assert.That(report.GetErrorCount(), Is.EqualTo(1));
            Assert.That(report.GetErrorTextAtIndex(0), Does.Contain("line 4"));
            Assert.That(report.GetErrorTextAtIndex(0), Does.Contain("130"));
        }

        [Test]
        public void Read_ReportsUnknownType()
        {
            ContentReport report = new ContentReport();
            CsvTable csv_table = new CsvTable("save.csv");

            csv_table.SetHeader(REPLAY_HEADER);
            csv_table.AddRow(new CsvRow(2, new string[] { "0", "SEED", "1234567" }));
            csv_table.AddRow(new CsvRow(3, new string[] { "112", "ACTON", "CONFIRM" }));

            ReplayReader.Read(csv_table, report);

            Assert.That(report.GetErrorCount(), Is.EqualTo(1));
            Assert.That(report.GetErrorTextAtIndex(0), Does.Contain("line 3"));
            Assert.That(report.GetErrorTextAtIndex(0), Does.Contain("ACTON"));
        }

        [Test]
        public void Read_ReportsFirstLineThatIsNotTheSeed()
        {
            ContentReport report = new ContentReport();
            CsvTable csv_table = new CsvTable("save.csv");

            csv_table.SetHeader(REPLAY_HEADER);
            csv_table.AddRow(new CsvRow(2, new string[] { "112", "ACTION", "CONFIRM" }));

            ReplayReader.Read(csv_table, report);

            Assert.That(report.GetErrorCount(), Is.EqualTo(1));
            Assert.That(report.GetErrorTextAtIndex(0), Does.Contain("line 2"));
            Assert.That(report.GetErrorTextAtIndex(0), Does.Contain("SEED"));
        }

        [Test]
        public void Read_ReportsSeedThatIsNotAtStepZero()
        {
            ContentReport report = new ContentReport();
            CsvTable csv_table = new CsvTable("save.csv");

            csv_table.SetHeader(REPLAY_HEADER);
            csv_table.AddRow(new CsvRow(2, new string[] { "5", "SEED", "1234567" }));

            ReplayReader.Read(csv_table, report);

            Assert.That(report.GetErrorCount(), Is.EqualTo(1));
            Assert.That(report.GetErrorTextAtIndex(0), Does.Contain("line 2"));
        }

        [Test]
        public void Read_ReportsSecondSeed()
        {
            ContentReport report = new ContentReport();
            CsvTable csv_table = new CsvTable("save.csv");

            csv_table.SetHeader(REPLAY_HEADER);
            csv_table.AddRow(new CsvRow(2, new string[] { "0", "SEED", "1234567" }));
            csv_table.AddRow(new CsvRow(3, new string[] { "112", "ACTION", "CONFIRM" }));
            csv_table.AddRow(new CsvRow(4, new string[] { "140", "SEED", "7654321" }));

            ReplayReader.Read(csv_table, report);

            Assert.That(report.GetErrorCount(), Is.EqualTo(1));
            Assert.That(report.GetErrorTextAtIndex(0), Does.Contain("line 4"));
        }

        [Test]
        public void Read_ReportsSeedThatIsNotAWholeNumber()
        {
            ContentReport report = new ContentReport();
            CsvTable csv_table = new CsvTable("save.csv");

            csv_table.SetHeader(REPLAY_HEADER);
            csv_table.AddRow(new CsvRow(2, new string[] { "0", "SEED", "abc" }));

            ReplayReader.Read(csv_table, report);

            Assert.That(report.GetErrorCount(), Is.EqualTo(1));
            Assert.That(report.GetErrorTextAtIndex(0), Does.Contain("line 2"));
            Assert.That(report.GetErrorTextAtIndex(0), Does.Contain("abc"));
        }

        [Test]
        public void Read_ReportsUnknownAction()
        {
            ContentReport report = new ContentReport();
            CsvTable csv_table = new CsvTable("save.csv");

            csv_table.SetHeader(REPLAY_HEADER);
            csv_table.AddRow(new CsvRow(2, new string[] { "0", "SEED", "1234567" }));
            csv_table.AddRow(new CsvRow(3, new string[] { "112", "ACTION", "JUMP" }));

            ReplayReader.Read(csv_table, report);

            Assert.That(report.GetErrorCount(), Is.EqualTo(1));
            Assert.That(report.GetErrorTextAtIndex(0), Does.Contain("line 3"));
            Assert.That(report.GetErrorTextAtIndex(0), Does.Contain("JUMP"));
        }

        [Test]
        public void Read_ReportsActionGivenAsANumber()
        {
            ContentReport report = new ContentReport();
            CsvTable csv_table = new CsvTable("save.csv");

            csv_table.SetHeader(REPLAY_HEADER);
            csv_table.AddRow(new CsvRow(2, new string[] { "0", "SEED", "1234567" }));
            csv_table.AddRow(new CsvRow(3, new string[] { "112", "ACTION", "3" }));

            ReplayReader.Read(csv_table, report);

            Assert.That(report.GetErrorCount(), Is.EqualTo(1));
            Assert.That(report.GetErrorTextAtIndex(0), Does.Contain("line 3"));
            Assert.That(report.GetErrorTextAtIndex(0), Does.Contain("\"3\""));
        }

        [Test]
        public void Read_ReportsEndThatIsNotTheLastLine()
        {
            ContentReport report = new ContentReport();
            CsvTable csv_table = new CsvTable("save.csv");

            csv_table.SetHeader(REPLAY_HEADER);
            csv_table.AddRow(new CsvRow(2, new string[] { "0", "SEED", "1234567" }));
            csv_table.AddRow(new CsvRow(3, new string[] { "112", "END", "" }));
            csv_table.AddRow(new CsvRow(4, new string[] { "140", "ACTION", "CONFIRM" }));

            ReplayReader.Read(csv_table, report);

            Assert.That(report.GetErrorCount(), Is.EqualTo(1));
            Assert.That(report.GetErrorTextAtIndex(0), Does.Contain("line 3"));
            Assert.That(report.GetErrorTextAtIndex(0), Does.Contain("END"));
        }

        [Test]
        public void Read_ReportsEveryErrorOfTheFile()
        {
            ContentReport report = new ContentReport();
            CsvTable csv_table = new CsvTable("save.csv");

            csv_table.SetHeader(REPLAY_HEADER);
            csv_table.AddRow(new CsvRow(2, new string[] { "0", "SEED", "1234567" }));
            csv_table.AddRow(new CsvRow(3, new string[] { "abc", "ACTION", "CONFIRM" }));
            csv_table.AddRow(new CsvRow(4, new string[] { "120", "ACTON", "CONFIRM" }));
            csv_table.AddRow(new CsvRow(5, new string[] { "130", "ACTION", "JUMP" }));

            ReplayReader.Read(csv_table, report);

            Assert.That(report.GetErrorCount(), Is.EqualTo(3));
        }

        // Le texte écrit par ReplayRecorder doit être relu sans erreur.
        [Test]
        public void Read_ReadsTheTextProducedByTheRecorderWithoutError()
        {
            ContentReport report = new ContentReport();
            ReplayRecorder recorder = new ReplayRecorder(1234567, new EventManager(), null);
            recorder.Record(112, GameActionType.NAVIGATE_DOWN);
            recorder.Record(112, GameActionType.CONFIRM);
            recorder.Record(3215, GameActionType.QUIT);

            CsvTable csv_table = CsvReader.Read("save.csv", recorder.GetCsvText(3215), report);
            ReplayData replay_data = ReplayReader.Read(csv_table, report);

            Assert.That(report.GetErrorCount(), Is.EqualTo(0));
            Assert.That(replay_data._seed, Is.EqualTo(1234567));
            Assert.That(replay_data._actionTable.Count, Is.EqualTo(3));
            Assert.That(replay_data._actionTable[1]._fixedStepIndex, Is.EqualTo(112));
            Assert.That(replay_data._actionTable[1]._action, Is.EqualTo(GameActionType.CONFIRM));
            Assert.That(replay_data._endFixedStepIndex, Is.EqualTo(3215));
        }
    }
}
