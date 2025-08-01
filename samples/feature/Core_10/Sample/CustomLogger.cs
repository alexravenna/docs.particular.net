using System;
using System.Diagnostics;
using System.Text.Json;
using NServiceBus;
using Microsoft.Extensions.Logging;
using NServiceBus.Logging;

#region CustomLogger
public class CustomLogger
{
    static ILog log = LogManager.GetLogger<CustomLogger>();
    static readonly JsonSerializerOptions options = new() { WriteIndented = true };

    public IDisposable StartTimer(string name)
    {
        return new Log(name);
    }

    public void WriteSaga(IContainSagaData sagaData)
    {
        var serialized = JsonSerializer.Serialize(sagaData, options);
        log.Warn($"Saga State: \r\n{serialized}");
    }

    class Log(string name) :
        IDisposable
    {
        readonly Stopwatch stopwatch = Stopwatch.StartNew();

        public void Dispose()
        {
            log.Warn($"{name} took {stopwatch.ElapsedMilliseconds}ms to process");
        }
    }
}

#endregion