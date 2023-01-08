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
        TransactionItem = transactionItem; ;
        var lastSagaLog = await _sagaLogPersister.GetLastStepForBusinessId(transactionItem.GetBusinessId());
        if (lastSagaLog == null)
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
                            new SagaLog(sagaId, transactionItem.GetBusinessId(), sagaAction.StepNumber,
                                orchestratorType, SagaStepState.Pending));
                        var result = await sagaAction.Function.Invoke();
                        if (result.Valid)
                        {
                            await _sagaLogPersister.SaveLog(
                                new SagaLog(sagaId, transactionItem.GetBusinessId(), sagaAction.StepNumber,
                                    orchestratorType, SagaStepState.Success));
                        }
                        else
                        {
                            await RollBackActions(sagaAction, sagaId, transactionItem.GetBusinessId(), orchestratorType, result.Message);
                            return SagaStepState.Fail;
                        }
                    }
                    catch (Exception e)
                    {
                        await RollBackActions(sagaAction, sagaId, transactionItem.GetBusinessId(), orchestratorType, e.Message);
                        return SagaStepState.Fail;
                    }
                }
                await _sagaLogPersister.SaveLog(
                    new SagaLog(sagaId, transactionItem.GetBusinessId(), null,
                    orchestratorType, SagaStepState.SuccessAll));
            }
            else
            {
                throw new NotSupportedException("Orchestrator type not supported");
            }
        }
        else if (lastSagaLog.StepState == SagaStepState.Fail)
        {

        }
        else
        {
            //nothing to do ? or we have to check how long this status is ?
        }
           
        return SagaStepState.SuccessAll;
    }

    private async Task RollBackActions(SagaAction sagaAction, Guid sagaId, string businessId, 
        string orchestratorType, string message)
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
        await _sagaLogPersister.SaveLog(
            new SagaLog(sagaId, businessId, sagaAction.StepNumber,
                orchestratorType, SagaStepState.Fail, message));
    }
}