using activity_00_tap_26_27.Core.Events;
using GameLibrary.Interfaces;

namespace GameLibraryTests;

public class EventManagerTests
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
       
        EventManager event_manager = new EventManager();
        FakeLogWriter fake_log_writer = new FakeLogWriter();
        LogManager log_manager = new LogManager(event_manager, fake_log_writer);

        event_manager.TriggerDelayedEvent(new LogMessageGameEvent("delayed test message"));
        event_manager.TriggerEvent(new LogMessageGameEvent("test message"));

        event_manager.ProcessDelayedEvents();

        Assert.That(fake_log_writer._logMessageTable.Count, Is.EqualTo(2));
        Assert.That(fake_log_writer._logMessageTable[0], Does.EndWith("test message\n"));
        Assert.That(fake_log_writer._logMessageTable[1], Does.EndWith("delayed test message\n"));
    }

}
