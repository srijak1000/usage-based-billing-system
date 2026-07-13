using BillingSystem.Application;
using BillingSystem.Domain;
using BillingSystem.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace BillingSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class SeedController : ControllerBase
{
    private static readonly PricingConfiguration PricingConfiguration = new(
        new[]
        {
            new PricingRule("storage", BillingType.FlatPerUnit, "GB-hour", 0.02m),
            new PricingRule("compute", BillingType.Tiered, "hour", 0m, new[] { new TierDefinition(100m, 0.10m), new TierDefinition(1000m, 0.08m), new TierDefinition(decimal.MaxValue, 0.05m) }),
            new PricingRule("api", BillingType.FixedSubscriptionPlusOverage, "call", 50m, null, 1000000m, 0.001m)
        });

    private static readonly BillingService BillingService = new(PricingConfiguration, new InMemoryUsageStore());

    [HttpPost("demo")]
    public IActionResult SeedDemoData()
    {
        BillingService.RecordUsage(new UsageEvent("user-1", "resource-storage", "storage", 200m, "GB-hour", new DateTime(2026, 6, 1, 12, 0, 0, DateTimeKind.Utc)));
        BillingService.RecordUsage(new UsageEvent("user-1", "resource-compute", "compute", 150m, "hour", new DateTime(2026, 6, 10, 10, 0, 0, DateTimeKind.Utc)));
        BillingService.RecordUsage(new UsageEvent("user-1", "resource-api", "api", 1200000m, "call", new DateTime(2026, 6, 15, 9, 0, 0, DateTimeKind.Utc)));
        BillingService.RecordUsage(new UsageEvent("user-2", "resource-storage", "storage", 50m, "GB-hour", new DateTime(2026, 6, 20, 8, 0, 0, DateTimeKind.Utc)));

        return Ok(new { message = "Demo billing data seeded." });
    }
}
