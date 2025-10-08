using CinemaAbyss.Events.Kafka.Producers;

using Microsoft.AspNetCore.Mvc;

namespace CinemaAbyss.Events.Controllers;

[ApiController]
[Route("api/events")]
public class EventsController : ControllerBase
{
    [HttpPost("movie")]
    public async Task<IActionResult> OnMovieEvent(MovieEvent movieEvent, [FromServices] MovieEventMessageProducer producer)
    {
        await producer.PublishAsync(movieEvent, CancellationToken.None);
        return Created("/api/events/movie", new { Status = "success" });
    }

    [HttpPost("payment")]
    public async Task<IActionResult> OnPaymentEvent(PaymentEvent paymentEvent, [FromServices] PaymentEventMessageProducer producer)
    {
        await producer.PublishAsync(paymentEvent, CancellationToken.None);
        return Created("/api/events/payment", new { Status = "success" });
    }

    [HttpPost("user")]
    public async Task<IActionResult> OnUserEvent(UserEvent userEvent, [FromServices] UserEventMessageProducer producer)
    {
        await producer.PublishAsync(userEvent, CancellationToken.None);
        return Created("/api/events/user", new { Status = "success" });
    }

    [HttpGet("health")]
    public Task<IActionResult> OnHealth()
    {
        return Task.FromResult<IActionResult>(Ok(new { Status = true }));
    }
}
