using System.Diagnostics;

namespace Saga.Orchestration.Utils;

public class SagaLogger : ISagaLogger
{
    public void LogError(string message, params object[] args)
    {
        Debug.WriteLine(message, args);
    }
}