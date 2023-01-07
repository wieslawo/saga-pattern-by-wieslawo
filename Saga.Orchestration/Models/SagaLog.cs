
namespace Saga.Orchestration.Models
{
    public class SagaLog
    {
        public SagaLog(Guid id, string businessId, int step, string orchestratorType, SagaState sagaState)
        {
            Id = id;
            BusinessId = businessId;
            SagaStep = step;
            OrchestratorType = orchestratorType;
            State = sagaState;
        }
        public Guid Id { get; set; }
        public string BusinessId { get; set; }
        public int SagaStep { get; set; }
        public String OrchestratorType { get; set; }
        public SagaState State { get; set; }
        public DateTime CreationTime { get; set; }
    }
}
