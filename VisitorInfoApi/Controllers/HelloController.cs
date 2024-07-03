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

             // Check for X-Forwarded-For header
    var forwardedFor = HttpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
    if (!string.IsNullOrEmpty(forwardedFor))
    {
        // X-Forwarded-For can contain multiple IPs; the client IP is typically the first one
        clientIp = forwardedFor.Split(',')[0].Trim();
    }
    else
    {
        // If X-Forwarded-For is not present, try X-Real-IP
        var realIp = HttpContext.Request.Headers["X-Real-IP"].FirstOrDefault();
        if (!string.IsNullOrEmpty(realIp))
        {
            clientIp = realIp;
        }
        else
        {
            // If neither header is present, fall back to RemoteIpAddress
            clientIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? baseIp;
        }
    }


            _logger.LogInformation($"Client IP: {clientIp}");

            if (clientIp == "::1")
            {
                clientIp = baseIp;
            }

            if (clientIp.Contains("::ffff:"))
            {
                clientIp = clientIp.Replace("::ffff:", "");
            }

             _logger.LogInformation($"Client IP 2: {clientIp}");

            return clientIp;
        }
    }
}
