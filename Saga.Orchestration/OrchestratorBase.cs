using Saga.Orchestration.Action;
using Saga.Orchestration.Models;
using Saga.Orchestration.Persister;
using Saga.Orchestration.TransactionItem;

namespace Saga.Orchestration;

public abstract class OrchestratorBase<T> where T: ITransactionItem
{
    private readonly ISagaLogPersister _sagaLogPersister;
    protected T? TransactionItem { get; set; }
    protected List<SagaAction>? SagaActions { get; set; }

    protected OrchestratorBase(ISagaLogPersister sagaLogPersister)
    {
        _sagaLogPersister = sagaLogPersister;
    }

    public async Task<SagaStepState> OrchestrateAsync(T transactionItem)
    {
        TransactionItem = transactionItem;
        var businessId = transactionItem.GetBusinessId();
        var pendingSaga = await _sagaLogPersister.GetLastStepForBusinessId(businessId);
        if (pendingSaga == null)
        {
            var orchestratorType = GetType().FullName;
            if (orchestratorType != null)
            {
                var sagaId = Guid.NewGuid();
                foreach (var sagaAction in SagaActions!)
                {
                    try
                    {
                        await _sagaLogPersister.SaveLog(
                            new SagaLog(sagaId, businessId, sagaAction.StepNumber,
                                orchestratorType, SagaStepState.Pending));
                        var result = await sagaAction.Function.Invoke();
                        if (result.Valid)
                        {
                            await _sagaLogPersister.SaveLog(
                                new SagaLog(sagaId, businessId, sagaAction.StepNumber,
                                    orchestratorType, SagaStepState.Success));
                        }
                        else
                        {
                            await RollBackActions(sagaAction, sagaId, businessId, orchestratorType);
                            await _sagaLogPersister.SaveLog(
                                new SagaLog(sagaId, businessId, sagaAction.StepNumber,
                                    orchestratorType, SagaStepState.Fail, result.Message));

                            return SagaStepState.Fail;
                        }
                    }
                    catch (Exception e)
                    {
                        await RollBackActions(sagaAction, sagaId, businessId, orchestratorType);
                        await _sagaLogPersister.SaveLog(
                            new SagaLog(sagaId, businessId, sagaAction.StepNumber, 
                                orchestratorType, SagaStepState.Fail, e.Message));

                        return SagaStepState.Fail;
                    }
                }

                await _sagaLogPersister.SaveLog(
                    new SagaLog(sagaId, businessId, null,
                    orchestratorType, SagaStepState.SuccessAll));
            }
            else
            {
                throw new NotSupportedException("Orchestrator type not supported");
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

    private async Task RollBackActions(SagaAction sagaAction, Guid sagaId, string businessId, string orchestratorType)
    {
        foreach (var actionToRollBack in SagaActions!
                     .Where(s => s.StepNumber < sagaAction.StepNumber)
                     .OrderByDescending(s => s.StepNumber))
        {
            await _sagaLogPersister.SaveLog(
                new SagaLog(sagaId, businessId, actionToRollBack.StepNumber, orchestratorType, SagaStepState.RollBacking));
            if (actionToRollBack.RollbackFunction != null)
                await actionToRollBack.RollbackFunction.Invoke();
            await _sagaLogPersister.SaveLog(
                new SagaLog(sagaId, businessId, actionToRollBack.StepNumber,orchestratorType, SagaStepState.Cancelled));
        }
    }
}