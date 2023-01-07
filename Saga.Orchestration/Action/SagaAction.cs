namespace Saga.Orchestration.Action
{
    public class SagaAction
    {
        public SagaAction(string name, int stepNumber, 
            Func<Task<SagaActionResult>> function,
            Func<Task<SagaActionResult>> rollbackFunction)
        {
            Name = name;
            StepNumber = stepNumber;
            Function = function;
            RollbackFunction = rollbackFunction;
        }

        public string Name { get; set; }
        public int StepNumber { get; set; }
        public Func<Task<SagaActionResult>> Function { get; set; }
        public Func<Task<SagaActionResult>>? RollbackFunction { get; set; }
        
    }
}
