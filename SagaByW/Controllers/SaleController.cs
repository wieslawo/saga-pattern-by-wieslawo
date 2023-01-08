using Microsoft.AspNetCore.Mvc;
using SagaByW.Models;
using SagaByW.Orchestrators;

namespace SagaByW.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class SaleController : ControllerBase
    {
        private readonly ImportSaleOrchestrator _importSaleOrchestrator;

        public SaleController(ImportSaleOrchestrator importSaleOrchestrator)
        {
            _importSaleOrchestrator = importSaleOrchestrator;
        }

        [HttpGet]
        public string Get()
        {
            return "Welcome to web API";
        }

        [HttpGet("{id}")]
        public async Task<string> Add(int id)
        {
            var sale = new SaleImport
            {
                SaleId = id,
                SaleName = "sale_" + id,
                ProductId = id,
                ProducName = "product_" + id
            };

            var result = await _importSaleOrchestrator.OrchestrateAsync(sale);
            return result.ToString();
        }
    }

}
