namespace Saga.Orchestration.Models
{
    public enum SagaStepState
    {
        Pending,
        RollBacking,
        Success,
        Cancelled,
        Fail,
    }
}
