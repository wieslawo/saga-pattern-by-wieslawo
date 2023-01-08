namespace Saga.Orchestration.Retry;
public static class RetryService
{
    public static async Task<T> Do<T>(Func<Task<T>> func, RetryModel retryModel,
        Action<Exception, RetryInfoModel> onException = null)
    {
        var numberOfAttempts = retryModel?.Delays?.Length ?? 1;
        var exceptions = new List<Exception>();

        for (var attempt = 1; attempt <= numberOfAttempts; attempt++)
        {
            try
            {
                return await func();
            }
            catch (Exception e)
            {
                onException?.Invoke(e, new RetryInfoModel(attempt, numberOfAttempts));
                exceptions.Add(e);

                if (retryModel?.Delays?.Length >= attempt)
                {
                    await Task.Delay(retryModel.Delays[attempt - 1]);
                }
            }
        }

        throw new AggregateException(exceptions);
    }
}