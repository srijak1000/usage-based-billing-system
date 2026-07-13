using BillingSystem.Application;
using BillingSystem.Domain;
using BillingSystem.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace BillingSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class BillingController : ControllerBase
{
    private static readonly PricingConfiguration PricingConfiguration = new(
        new[]
        {
            new PricingRule("storage", BillingType.FlatPerUnit, "GB-hour", 0.02m),
            new PricingRule("compute", BillingType.Tiered, "hour", 0m, new[] { new TierDefinition(100m, 0.10m), new TierDefinition(1000m, 0.08m), new TierDefinition(decimal.MaxValue, 0.05m) }),
            new PricingRule("api", BillingType.FixedSubscriptionPlusOverage, "call", 50m, null, 1000000m, 0.001m)
        });

    private static readonly BillingService BillingService = new(PricingConfiguration, new InMemoryUsageStore());

    [HttpPost("usage")]
    public IActionResult RecordUsage([FromBody] UsageEvent usageEvent)
    {
        BillingService.RecordUsage(usageEvent);
        return Ok(new { message = "Usage recorded successfully." });
    }

    [HttpGet("invoices/{userId}")]
    public IActionResult GetInvoice(string userId, [FromQuery] DateTime? start, [FromQuery] DateTime? end)
    {
        var periodStart = start ?? new DateTime(2026, 6, 1, 0, 0, 0, DateTimeKind.Utc);
        var periodEnd = end ?? new DateTime(2026, 7, 1, 0, 0, 0, DateTimeKind.Utc);
        var invoice = BillingService.GenerateInvoice(userId, periodStart, periodEnd);
        return Ok(invoice);
    }
}
