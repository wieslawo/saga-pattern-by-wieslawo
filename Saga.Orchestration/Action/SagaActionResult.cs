namespace Saga.Orchestration.Action
{
    public class SagaActionResult
    {
        public bool Valid { get; set; } = true;
        public string Message { get; set; } = string.Empty;
        public string ExceptionMessage { get; set; } = string.Empty;
    }
}
