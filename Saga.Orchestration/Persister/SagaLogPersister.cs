using System.Diagnostics;
using Saga.Orchestration.Models;

namespace Saga.Orchestration.Persister
{
    public class SagaLogPersister: ISagaLogPersister
    {
        public List<SagaLog> SavedLogs { get; set; }

        public SagaLogPersister()
        {
            SavedLogs = new List<SagaLog>();
        }

        public async Task<bool> SaveLog(SagaLog sagaLog)
        {
            sagaLog.CreationTime = DateTime.UtcNow;
            SavedLogs.Add(sagaLog);
            Debug.WriteLine($"Saved log for {sagaLog.BusinessId}, step {sagaLog.SagaStep} {Enum.GetName(typeof(SagaStepState), sagaLog.StepState)}");
            return await Task.FromResult(true);
        }

        public async Task<List<SagaLog>> GetPendings()
        {
            return (await Task.FromResult(SavedLogs.Where(l => l.StepState == SagaStepState.Pending))).ToList();
        }

        public async Task<SagaLog?> GetLastStepForBusinessId(string businessId)
        {
            return await Task.FromResult(SavedLogs.Where(l => l.BusinessId == businessId).MaxBy(l => l.CreationTime));
        }
    }
}
