using Saga.Orchestration.TransactionItem;

namespace SagaByW.Models;

public class SaleImport:ITransactionItem
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