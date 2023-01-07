using Saga.Orchestration.Persister;
using Saga.Orchestration.Utils;

namespace Saga.Orchestration
{
    public class SagaChecker
    {
        private readonly ISagaLogPersister _sagaLogPersister;
        private readonly ISagaLogger _sagaLogger;

        public SagaChecker(ISagaLogPersister sagaLogPersister, ISagaLogger sagaLogger)
        {
            _sagaLogPersister = sagaLogPersister;
            _sagaLogger = sagaLogger;
        }

        public async Task CheckStatuses()
        {
            var pendingSagas = await _sagaLogPersister.GetPendings();
            _sagaLogger.LogError("Pending sagas", pendingSagas);
        }
    }
}
