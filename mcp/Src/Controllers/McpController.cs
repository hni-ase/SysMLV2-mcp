using Microsoft.AspNetCore.Mvc;
using Services;

namespace Src.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class McpController : ControllerBase
    {
        private readonly McpService _mcpService;

        public McpController(McpService mcpService)
        {
            _mcpService = mcpService;
        }

        // Example MCP endpoint: GET /api/mcp/ping
        [HttpGet("ping")]
        public IActionResult Ping()
        {
            return Ok(new { message = "MCP endpoint is alive." });
        }

        // Example MCP endpoint: POST /api/mcp/data
        [HttpPost("data")]
        public IActionResult ReceiveData([FromBody] object data)
        {
            // Forward MCP data to the service for processing
            _mcpService.ProcessIncomingData(data);
            return Ok(new { status = "received", received = data });
        }
    }
}