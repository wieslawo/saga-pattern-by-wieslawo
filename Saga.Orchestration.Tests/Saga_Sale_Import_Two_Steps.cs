using System.Diagnostics;
using Saga.Orchestration.Action;
using Saga.Orchestration.Models;
using Saga.Orchestration.Persister;
using Saga.Orchestration.TransactionItem;
using Xunit;

namespace Saga.Orchestration.Tests;

public class Saga_Sale_Import_Two_Steps
{
    [Fact]
    public async Task Saga_Import()
    {
        var sale = new SaleImport
        {
            SaleId = 1,
            SaleName = "sale_1",
            ProductName = "product_1"
        };

        var importSaleOrchestrator = new ImportSaleOrchestrator(new SagaLogPersister());
        var result = await importSaleOrchestrator.OrchestrateAsync(sale);
            
        Assert.Equal(SagaStepState.SuccessAll, result);
    }


    private class SaleImport : ITransactionItem
    {
        public int SaleId { get; set; }
        public string? SaleName { get; set; }
        public string? ProductName { get; set; }

        public string GetBusinessId()
        {
            return "SaleImport_" + SaleId;
        }
    }


    private class ImportSaleOrchestrator : OrchestratorBase<SaleImport>
    {
        public ImportSaleOrchestrator(ISagaLogPersister sagaLogPersister) : base(sagaLogPersister)
        {
            SagaActions = new List<SagaAction>
            {
                new SagaAction("Step 1", 1, StepOneOfImport, null),
                new SagaAction("Step 2", 2, StepTwoOfImport, null),
            };
        }

        private async Task<SagaActionResult> StepOneOfImport()
        {
            await Task.Delay(10);
            Debug.WriteLine("Pierwszy krok for sale: " + TransactionItem!.SaleName);

            return new SagaActionResult()
            {
                Message = "Ok"
            };
        }

        private async Task<SagaActionResult> StepTwoOfImport()
        {
            await Task.Delay(10);
            Debug.WriteLine("Drugi krok importu, product: " + TransactionItem!.ProductName);

            return new SagaActionResult()
            {
                Message = "Ok"
            };
        }
    }
}
