using Microsoft.AspNetCore.Mvc;
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
        public async Task<string> Add(string id)
        {
            var businessId = $"Import_Order_{id}";
            var result = await _importSaleOrchestrator.OrchestrateAsync(businessId);
            return result.ToString();
        }
    }

}
