using Saga.Orchestration.TransactionItem;

namespace SagaByW_API_Test.Models;

public class SaleImport:ITransactionItem
{
    public int SaleId { get; set; }
    public string? SaleName { get; set; }
    public string? ProductName { get; set; }

    public string GetBusinessId()
    {
        return "SaleImport_" + SaleId;
    }
}