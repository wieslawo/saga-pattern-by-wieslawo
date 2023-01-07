using Saga.Orchestration.Action;
using Saga.Orchestration.Models;
using Saga.Orchestration.Persister;
using Saga.Orchestration.Utils;

namespace Saga.Orchestration
{
    public abstract class OrchestratorBase
    {
        private readonly ISagaLogPersister _sagaLogPersister;
        private readonly ISagaLogger _sagaLogger;

        protected List<SagaAction> SagaActions { get; set; }

        public OrchestratorBase(ISagaLogPersister sagaLogPersister, ISagaLogger sagaLogger)
        {
            _sagaLogPersister = sagaLogPersister;
            _sagaLogger = sagaLogger;
        }


        public async Task<SagaState> OrchestrateAsync(string businessId)
        {
            var pendingSaga = await _sagaLogPersister.GetPendingForBusinessId(businessId);
            if (pendingSaga == null)
            {
                var sagaId = Guid.NewGuid();

                foreach (var sagaAction in SagaActions)
                {
                    try
                    {
                        await _sagaLogPersister.SaveLog(
                            new SagaLog(sagaId, businessId, sagaAction.StepNumber,GetType().FullName,SagaState.Pending));
                        var result = await sagaAction.Function.Invoke();
                    }
                    catch (Exception e)
                    {
                        _sagaLogger.LogError(e.Message, e);
                        await _sagaLogPersister.SaveLog(
                            new SagaLog(sagaId, businessId, sagaAction.StepNumber, GetType().FullName, SagaState.Fail));
                        //rollback all previous

                        return SagaState.Fail;
                    }
                }
            }
            else
            {

            }
            return SagaState.Success;
        }
    }

   
}