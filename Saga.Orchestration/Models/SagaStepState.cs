namespace Saga.Orchestration.Models;

public enum SagaStepState
{
    Pending,
    Success,
    SuccessAll,
    RollBacking,
    Cancelled,
    Fail
}