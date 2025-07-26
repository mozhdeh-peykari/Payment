namespace Infrastructure.Logging;

public interface ILogger
{
    void Info(string message, object data = null);

    void Warn(string message, object data = null);

    void Error(string message, Exception ex = null, object data = null);

    void Debug(string message, object data = null);
}
