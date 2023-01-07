namespace Saga.Orchestration.Utils
{
    public interface ISagaLogger
    {
        void LogError(string message, params object[] args);
    }
}
