namespace Saga.Orchestration.Action;

public class SagaAction
{
    public SagaAction(string name, int stepNumber, 
        Func<Task<SagaActionResult>> function,
        Func<Task<SagaActionResult>>? rollbackFunction,
        int retryAttempts = 1,
        int retryInterval = 0,
        int retryRollbackAttempts = 1,
        int retryRollbackInterval = 0)
    {
        Name = name;
        StepNumber = stepNumber;
        Function = function;
        RollbackFunction = rollbackFunction;
        RetryAttempts = retryAttempts;
        RetryInterval = retryInterval;
        RetryRollbackAttempts = retryRollbackAttempts;
        RetryRollbackInterval = retryRollbackInterval;
    }

    public string Name { get; set; }
    public int StepNumber { get; set; }
    public Func<Task<SagaActionResult>> Function { get; set; }
    public Func<Task<SagaActionResult>>? RollbackFunction { get; set; }
    public int RetryAttempts { get; set; }
    public int RetryInterval { get; set; }
    public int RetryRollbackAttempts { get; set; }
    public int RetryRollbackInterval { get; set; }

}