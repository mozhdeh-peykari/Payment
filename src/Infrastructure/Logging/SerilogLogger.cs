using Serilog;

namespace Infrastructure.Logging;

public class SerilogLogger : ILogger
{

    public void Info(string message, object data = null)
    {
        Log.Information(message + (data != null ? " {@Data}" : ""), data);
    }

    public void Warn(string message, object data = null)
    {
        Log.Warning(message + (data != null ? " {@Data}" : ""), data);
    }

    public void Error(string message, Exception ex = null, object data = null)
    {
        if (ex != null)
            Log.Error(ex, message + (data != null ? " {@Data}" : ""), data);
        else
            Log.Error(message + (data != null ? " {@Data}" : ""), data);
    }

    public void Debug(string message, object data = null)
    {
        Log.Debug(message + (data != null ? " {@Data}" : ""), data);
    }
}
