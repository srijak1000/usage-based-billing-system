using BillingSystem.Application;
using BillingSystem.Domain;
using Microsoft.AspNetCore.Mvc;

namespace BillingSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class BillingController : ControllerBase
{
    private readonly BillingService _billingService;

    public BillingController(BillingService billingService)
    {
        _billingService = billingService;
    }

    [HttpPost("usage")]
    public IActionResult RecordUsage([FromBody] UsageEvent usageEvent)
    {
        _billingService.RecordUsage(usageEvent);
        return Ok(new { message = "Usage recorded successfully." });
    }

    [HttpGet("invoices/{userId}")]
    public IActionResult GetInvoice(string userId, [FromQuery] DateTime? start, [FromQuery] DateTime? end)
    {
        var periodStart = start ?? new DateTime(2026, 6, 1, 0, 0, 0, DateTimeKind.Utc);
        var periodEnd = end ?? new DateTime(2026, 7, 1, 0, 0, 0, DateTimeKind.Utc);
        var invoice = _billingService.GenerateInvoice(userId, periodStart, periodEnd);
        return Ok(invoice);
    }
}
