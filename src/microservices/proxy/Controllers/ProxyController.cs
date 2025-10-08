using CinemaAbyss.Proxy.Services;

using Microsoft.AspNetCore.Mvc;

namespace CinemaAbyss.Proxy.Controllers;

[Route("api")]
[ApiController]
public class ProxyController : ControllerBase
{
    private readonly IProxyService _proxyService;
    private readonly ILogger<ProxyController> _logger;

    public ProxyController(IProxyService proxyService, ILogger<ProxyController> logger)
    {
        _proxyService = proxyService;
        _logger = logger;
    }

    [HttpGet("movies")]
    public Task<IActionResult> GetMovies() => ProxyRequest("movies", "/api/movies");

    [HttpGet("movies/{id}")]
    public Task<IActionResult> GetMovie(string id) => ProxyRequest("movies", $"/api/movies/{id}");

    [HttpPost("movies")]
    public Task<IActionResult> CreateMovie() => ProxyRequest("movies", "/api/movies");

    [HttpPut("movies/{id}")]
    public Task<IActionResult> UpdateMovie(string id) => ProxyRequest("movies", $"/api/movies/{id}");

    [HttpDelete("movies/{id}")]
    public Task<IActionResult> DeleteMovie(string id) => ProxyRequest("movies", $"/api/movies/{id}");

    [HttpGet("events")]
    public Task<IActionResult> GetEvents() => ProxyRequest("events", "/api/events");

    [HttpPost("events")]
    public Task<IActionResult> CreateEvent() => ProxyRequest("events", "/api/events");

    [HttpGet("{*path}")]
    [HttpPost("{*path}")]
    [HttpPut("{*path}")]
    [HttpDelete("{*path}")]
    public Task<IActionResult> CatchAll(string path) => ProxyRequest("monolith", $"/api/{path}");

    private async Task<IActionResult> ProxyRequest(string serviceName, string path)
    {
        try
        {
            var response = await _proxyService.ForwardRequestAsync(Request, serviceName, path);

            var content = await response.Content.ReadAsStringAsync();
            var result = new ObjectResult(content)
            {
                StatusCode = (int)response.StatusCode
            };

            foreach (var header in response.Headers)
            {
                Response.Headers[header.Key] = header.Value.ToArray();
            }

            Response.Headers.Remove("transfer-encoding");

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error proxying request to {ServiceName}", serviceName);
            return StatusCode(500, new { error = "Internal server error", details = ex.Message });
        }
    }
}
