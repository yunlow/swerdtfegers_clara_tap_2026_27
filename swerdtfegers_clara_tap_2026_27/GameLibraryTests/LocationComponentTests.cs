using activity_00_tap_26_27.Core;
using activity_00_tap_26_27.Core.Events;
using GameLibrary.Components;
using GameLibrary.Interfaces;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;

namespace GameLibraryTests;

public class LocationComponentTests
{
    private class FakeLogWriter : ILogWriter
    {
        public readonly List<string> _logMessageTable = new List<string>();

        public void WriteLine(string line)
        {
            _logMessageTable.Add(line);
        }
    }

    [Test]
    public void LogMessage_WriteTheCorrectSentence()
    {
        //ARRANGE
        EventManager event_manager = new EventManager();
        FakeLogWriter fake_log_writer = new FakeLogWriter();
        LogManager log_manager = new LogManager(event_manager, fake_log_writer);

        //ACT
        event_manager.TriggerEvent(new LogMessageGameEvent("test message"));

        //ASSERT
        Assert.That(fake_log_writer._logMessageTable.Count, Is.EqualTo(1));
        Assert.That(fake_log_writer._logMessageTable[0], Does.EndWith("test message\n"));
    }

    [Test]
    public void LogMessage_DoesNotReorderSentences()
    {
        //ARRANGE
        EventManager event_manager = new EventManager();
        FakeLogWriter fake_log_writer = new FakeLogWriter();
        LogManager log_manager = new LogManager(event_manager, fake_log_writer);

        //ACT
        event_manager.TriggerEvent(new LogMessageGameEvent("first test message"));
        event_manager.TriggerEvent(new LogMessageGameEvent("second test message"));

        //ASSERT
        Assert.That(fake_log_writer._logMessageTable.Count, Is.EqualTo(2));
        Assert.That(fake_log_writer._logMessageTable[0], Does.EndWith("first test message\n"));
        Assert.That(fake_log_writer._logMessageTable[1], Does.EndWith("second test message\n"));
    }
}
