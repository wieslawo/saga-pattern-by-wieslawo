using Saga.Orchestration.Persister;

namespace Saga.Orchestration
{
    public class SagaChecker
    {
        private readonly ISagaLogPersister _sagaLogPersister;

        public SagaChecker(ISagaLogPersister sagaLogPersister)
        {
            _sagaLogPersister = sagaLogPersister;
        }

        public async Task CheckStatuses()
        {
            var pendingSagas = await _sagaLogPersister.GetPendings();
            // What to do with not finished sagas ?
        }
    }
}
