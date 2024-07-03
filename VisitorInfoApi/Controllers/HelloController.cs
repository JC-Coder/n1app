// Controllers/HelloController.cs
using Microsoft.AspNetCore.Mvc;
using VisitorInfoApi.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace VisitorInfoApi.Controllers
{


    [Route("api/[controller]")]
    [ApiController]
    public class HelloController : ControllerBase
    {
        private readonly IVisitorInfoService _visitorInfoService;
        private readonly ILogger<HelloController> _logger;

        public HelloController(IVisitorInfoService visitorInfoService, ILogger<HelloController> logger)
        {
            _visitorInfoService = visitorInfoService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery(Name = "visitor_name")] string visitorName)
        {
            try
            {
                string ipAddress = GetClientIpAddress();

                // calls service to get visitor info
                var result = await _visitorInfoService.GetVisitorInfo(ipAddress, visitorName);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Unable to process request", message = ex.Message });
            }
        }

        private string GetClientIpAddress()
        {
            var baseIp = "105.112.119.81";
            string clientIp;

            // Check for Render-specific header
            var renderForwardedFor = HttpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrEmpty(renderForwardedFor))
            {
                clientIp = renderForwardedFor.Split(',')[0].Trim();
            }
            else
            {
                // Fallback to other methods if Render header is not present
                clientIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? baseIp;
            }

            _logger.LogInformation($"Original Client IP: {clientIp}");

            if (clientIp == "::1" || clientIp.StartsWith("172.") || clientIp.StartsWith("10."))
            {
                clientIp = baseIp;
            }

            if (clientIp.Contains("::ffff:"))
            {
                clientIp = clientIp.Replace("::ffff:", "");
            }

            _logger.LogInformation($"Final Client IP: {clientIp}");

            return clientIp;
        }
    }
}
