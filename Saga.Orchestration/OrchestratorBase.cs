using Saga.Orchestration.Action;
using Saga.Orchestration.Models;
using Saga.Orchestration.Persister;

namespace Saga.Orchestration
{
    public abstract class OrchestratorBase
    {
        private readonly ISagaLogPersister _sagaLogPersister;

        protected List<SagaAction> SagaActions { get; set; }

        public OrchestratorBase(ISagaLogPersister sagaLogPersister)
        {
            _sagaLogPersister = sagaLogPersister;
        }


        public async Task<SagaStepState> OrchestrateAsync(string businessId)
        {
            var pendingSaga = await _sagaLogPersister.GetLastStepForBusinessId(businessId);
            if (pendingSaga == null)
            {
                var sagaId = Guid.NewGuid();

                foreach (var sagaAction in SagaActions)
                {
                    try
                    {
                        await _sagaLogPersister.SaveLog(
                            new SagaLog(sagaId, businessId, sagaAction.StepNumber,
                                GetType().FullName,SagaStepState.Pending));
                        var result = await sagaAction.Function.Invoke();
                        await _sagaLogPersister.SaveLog(
                            new SagaLog(sagaId, businessId, sagaAction.StepNumber,
                                GetType().FullName, SagaStepState.Success));
                    }
                    catch (Exception e)
                    {
                        await RollBackActions(sagaAction, sagaId, businessId);
                        await _sagaLogPersister.SaveLog(
                            new SagaLog(sagaId, businessId, sagaAction.StepNumber, 
                                GetType().FullName, SagaStepState.Fail, e.Message));

                        return SagaStepState.Fail;
                    }
                }
               
            }
            else if (pendingSaga.StepState == SagaStepState.Fail)
            {

            }
            else
            {
                //nothing to do ? or we have to check how long this status is ?
            }
            return SagaStepState.Success;
        }

        private async Task RollBackActions(SagaAction sagaAction, Guid sagaId, string businessId)
        {
            foreach (var actionToRollBack in SagaActions
                         .Where(s => s.StepNumber < sagaAction.StepNumber)
                         .OrderByDescending(s => s.StepNumber))
            {
                await _sagaLogPersister.SaveLog(
                    new SagaLog(sagaId, businessId, actionToRollBack.StepNumber,
                        GetType().FullName, SagaStepState.RollBacking));
                if (actionToRollBack.RollbackFunction != null)
                    await actionToRollBack.RollbackFunction.Invoke();
                await _sagaLogPersister.SaveLog(
                    new SagaLog(sagaId, businessId, actionToRollBack.StepNumber,
                        GetType().FullName, SagaStepState.Cancelled));
            }
        }
    }

   
}