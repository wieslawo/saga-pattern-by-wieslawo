using Saga.Orchestration;
using Saga.Orchestration.Action;
using Saga.Orchestration.Persister;
using Saga.Orchestration.Utils;
using System.Diagnostics;

namespace SagaByW.Orchestrators
{
    public class ImportSaleOrchestrator: OrchestratorBase
    {
        public ImportSaleOrchestrator(ISagaLogPersister sagaLogPersister, ISagaLogger sagaLogger) : base(sagaLogPersister, sagaLogger)
        {
            SagaActions = new List<SagaAction>
            {
                new SagaAction("Step 1", 1, StepOneOfImport, StepOneOfImportRollback),
                new SagaAction("Step 2", 2, StepTwoOfImport, StepTwoOfImportRollback)
            };
        }

        public async Task<SagaActionResult> StepOneOfImport()
        {
            Debug.WriteLine("Calling Google");
            HttpClient client = new HttpClient();
            var response = await client.GetAsync("https://www.google.com");

            return new SagaActionResult()
            {
                Message = "Ok"
            };
        }

        public async Task<SagaActionResult> StepOneOfImportRollback()
        {
            Debug.WriteLine("Calling Google rollback");
           
            return new SagaActionResult()
            {
                Message = "Ok"
            };
        }


        public async Task<SagaActionResult> StepTwoOfImport()
        {
            await Task.Delay(10);
            Debug.WriteLine("Druki krok importu");

            return new SagaActionResult()
            {
                Message = "Ok"
            };
        }


        public async Task<SagaActionResult> StepTwoOfImportRollback()
        {
            await Task.Delay(10);
            Console.WriteLine("Druki krok importu wycofany");

            return new SagaActionResult()
            {
                Message = "Ok"
            };
        }
    }
}
