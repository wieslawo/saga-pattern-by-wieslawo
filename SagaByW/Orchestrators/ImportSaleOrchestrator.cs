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
                new SagaAction("Step 1", 1, StepOneOfImport, StepOneOfImportRollback),
                new SagaAction("Step 2", 2, StepTwoOfImport, null),
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

        public async Task<SagaActionResult> StepOneOfImportRollback()
        {
            await Task.Delay(10);
            Console.WriteLine(@"Pierwszy krok importu wycofany");

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


      

    }
}
