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
            return await Task.FromResult(true);
        }

        public async Task<List<SagaLog>> GetPendings()
        {
            return (await Task.FromResult(SavedLogs.Where(l => l.State == SagaState.Pending))).ToList();
        }

        public async Task<SagaLog?> GetPendingForBusinessId(string businessId)
        {
            return await Task.FromResult(SavedLogs.SingleOrDefault(l => l.State == SagaState.Pending && l.BusinessId == businessId));
        }
    }
}
