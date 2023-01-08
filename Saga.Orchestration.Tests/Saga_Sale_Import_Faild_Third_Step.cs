using System.Diagnostics;
using Saga.Orchestration.Action;
using Saga.Orchestration.Models;
using Saga.Orchestration.Persister;
using Saga.Orchestration.TransactionItem;
using Xunit;

namespace Saga.Orchestration.Tests;

public class Saga_Sale_Import_Faild_Third_Step
{
    [Fact]
    public async Task Saga_Not_Import()
    {
        var sale = new SaleImport
        {
            SaleId = 1,
            SaleName = "sale_1",
            ProductId = 1,
            ProducName = "product_1"
        };

        var importSaleOrchestrator = new ImportSaleOrchestrator(new SagaLogPersister());
        var result = await importSaleOrchestrator.OrchestrateAsync(sale);
        
        Assert.Equal(SagaStepState.Fail, result);
    }

    private class ImportSaleOrchestrator : OrchestratorBase<SaleImport>
    {
        public ImportSaleOrchestrator(ISagaLogPersister sagaLogPersister) : base(sagaLogPersister)
        {
            SagaActions = new List<SagaAction>
            {
                new SagaAction("Step 1", 1, StepOneOfImport, null),
                new SagaAction("Step 2", 2, StepTwoOfImport, StepTwoOfImportRollback),
                new SagaAction("Step 3", 3, StepThreeOfImport, null)
            };
        }

        public async Task<SagaActionResult> StepOneOfImport()
        {
            Debug.WriteLine("Calling Google for sale: " + TransactionItem!.SaleName);
            HttpClient client = new HttpClient();
            await client.GetAsync("https://www.google.com");

            return new SagaActionResult()
            {
                Message = "Ok"
            };
        }


        public async Task<SagaActionResult> StepTwoOfImport()
        {
            await Task.Delay(10);
            Debug.WriteLine("Drugi krok importu, product: " + TransactionItem!.ProducName);

            return new SagaActionResult()
            {
                Message = "Ok"
            };
        }


        public async Task<SagaActionResult> StepTwoOfImportRollback()
        {
            await Task.Delay(10);
            Console.WriteLine(@"Drugi krok importu wycofany product: " + TransactionItem!.ProducName);

            return new SagaActionResult()
            {
                Message = "Ok"
            };
        }


        public async Task<SagaActionResult> StepThreeOfImport()
        {
            await Task.Delay(10);
            Console.WriteLine(@"Trzeci krok importu product: " + TransactionItem!.ProducName);
            throw new Exception("Problem w trzecim kroku");
        }
    }

    private class SaleImport : ITransactionItem
    {
        public int SaleId { get; set; }
        public string? SaleName { get; set; }
        public int ProductId { get; set; }
        public string? ProducName { get; set; }

        public string GetBusinessId()
        {
            return "SaleImport_" + SaleId;
        }
    }
}