using BillingSystem.Application;
using BillingSystem.Domain;
using Microsoft.AspNetCore.Mvc;

namespace BillingSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class SeedController : ControllerBase
{
    private readonly BillingService _billingService;

    public SeedController(BillingService billingService)
    {
        _billingService = billingService;
    }

    [HttpPost("demo")]
    public IActionResult SeedDemoData()
    {
        _billingService.RecordUsage(new UsageEvent("user-1", "resource-storage", "storage", 200m, "GB-hour", new DateTime(2026, 6, 1, 12, 0, 0, DateTimeKind.Utc)));
        _billingService.RecordUsage(new UsageEvent("user-1", "resource-compute", "compute", 150m, "hour", new DateTime(2026, 6, 10, 10, 0, 0, DateTimeKind.Utc)));
        _billingService.RecordUsage(new UsageEvent("user-1", "resource-api", "api", 1200000m, "call", new DateTime(2026, 6, 15, 9, 0, 0, DateTimeKind.Utc)));
        _billingService.RecordUsage(new UsageEvent("user-2", "resource-storage", "storage", 50m, "GB-hour", new DateTime(2026, 6, 20, 8, 0, 0, DateTimeKind.Utc)));

        return Ok(new { message = "Demo billing data seeded." });
    }
}
