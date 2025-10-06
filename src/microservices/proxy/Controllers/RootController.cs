using Microsoft.AspNetCore.Mvc;

namespace CinemaAbyss.Proxy.Controllers;

[ApiController]
public class RootController : ControllerBase
{
    [HttpGet("health")]
    public Task<IActionResult> OnHealth()
    {
        return Task.FromResult<IActionResult>(Ok(new { Status = true }));
    }
}
