using Saga.Orchestration.Models;

namespace Saga.Orchestration.Persister
{
    public interface ISagaLogPersister
    {
        public Task<bool> SaveLog(SagaLog sagaLog);
        public Task<List<SagaLog>> GetPendings();
        public Task<SagaLog?> GetPendingForBusinessId(string businessId);
    }
}
