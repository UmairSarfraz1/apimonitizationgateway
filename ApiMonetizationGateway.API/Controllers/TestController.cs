using ApiMonetizationGateway.Core.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiMonetizationGateway.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly IRateLimitingService _rateLimitingService;

        public TestController(IRateLimitingService rateLimitingService)
        {
            _rateLimitingService = rateLimitingService;
        }

        [HttpGet(nameof(GetTestAsync))]
        public IActionResult GetTestAsync()
        {
            return Ok(new { message = "Success" });
        }


        [HttpGet(nameof(GetUserMonthlyUsageReport))]
        public async Task<IActionResult> GetUserMonthlyUsageReport() 
        {
            this.HttpContext.Request.Headers.TryGetValue("X-API-Key", out var apiKey);
            var response = await _rateLimitingService.GetUserMonthlyUsageReport(apiKey!);
            return Ok(response);
        }

        [HttpGet(nameof(GetUserMonthlyQuota))]
        public async Task<IActionResult> GetUserMonthlyQuota()
        {
            this.HttpContext.Request.Headers.TryGetValue("X-API-Key", out var apiKey);
            var response = await _rateLimitingService.GetUserMonthlyQuota(apiKey!);
            return Ok(new { APIUsageCount = response});
        }
    }
}
