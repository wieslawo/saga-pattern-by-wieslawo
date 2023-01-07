
namespace Saga.Orchestration.Models
{
    public class SagaLog
    {
        public SagaLog(Guid id, string businessId, int step, string orchestratorType, SagaStepState sagaStepState, string? exceptionMessage = null)
        {
            Id = id;
            BusinessId = businessId;
            SagaStep = step;
            OrchestratorType = orchestratorType;
            StepState = sagaStepState;
            ExceptionMessage = exceptionMessage;
        }
        public Guid Id { get; set; }
        public string BusinessId { get; set; }
        public int SagaStep { get; set; }
        public String OrchestratorType { get; set; }
        public SagaStepState StepState { get; set; }
        public string? ExceptionMessage { get; set; }
        public DateTime CreationTime { get; set; }
    }
}
