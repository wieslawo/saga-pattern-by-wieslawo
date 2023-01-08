namespace Saga.Orchestration.Retry;

public class RetryInfoModel
{
    public int RetryNumber { get; }

    public int RetryMaxCount { get; }

    public bool IsFinalRetry { get; }

    public RetryInfoModel(int retryNumber, int retryMaxCount)
    {
        RetryNumber = retryNumber;
        RetryMaxCount = retryMaxCount;
        IsFinalRetry = retryNumber == retryMaxCount;
    }
}

public class RetryModel
{
    public TimeSpan[] Delays { get; }

    public RetryModel(params TimeSpan[] delays)
    {
        Delays = delays;
    }

    public RetryModel(int delay)
    {
        Delays = new []{ TimeSpan.FromMilliseconds(delay) };
    }

    public RetryModel(int delay, int retryAttempts)
    {
        Delays = new TimeSpan[retryAttempts];
        for (int i = 0; i < delay;)
        {
            Delays[i] = TimeSpan.FromMilliseconds(delay*(i+1));
        }
    }
}
