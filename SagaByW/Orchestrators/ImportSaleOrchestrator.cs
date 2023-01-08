using System.Diagnostics;
using Saga.Orchestration;
using Saga.Orchestration.Action;
using Saga.Orchestration.Persister;
using SagaByW_API_Test.Models;

namespace SagaByW_API_Test.Orchestrators
{
    public class ImportSaleOrchestrator: OrchestratorBase<SaleImport>
    {
        public ImportSaleOrchestrator(ISagaLogPersister sagaLogPersister) : base(sagaLogPersister)
        {
            SagaActions = new List<SagaAction>
            {
                new SagaAction("Step 1", 1, StepOneOfImport, null),
                new SagaAction("Step 2", 2, StepTwoOfImport, StepTwoOfImportRollback),
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
            Debug.WriteLine("Drugi krok importu, product: " + TransactionItem!.ProductName );

            return new SagaActionResult()
            {
                Message = "Ok"
            };
        }


        public async Task<SagaActionResult> StepTwoOfImportRollback()
        {
            await Task.Delay(10);
            Console.WriteLine(@"Drugi krok importu wycofany product: " + TransactionItem!.ProductName);

            return new SagaActionResult()
            {
                Message = "Ok"
            };
        }

    }
}
