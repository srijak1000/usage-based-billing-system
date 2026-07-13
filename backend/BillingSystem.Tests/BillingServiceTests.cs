using BillingSystem.Application;
using BillingSystem.Domain;
using BillingSystem.Infrastructure;
using Xunit;

namespace BillingSystem.Tests;

public class BillingServiceTests
{
    [Fact]
    public void GeneratesInvoiceForMultipleServicesAndPricingModels()
    {
        var pricingConfig = new PricingConfiguration(
            new[]
            {
                new PricingRule("storage", BillingType.FlatPerUnit, "GB-hour", 0.02m),
                new PricingRule("compute", BillingType.Tiered, "hour", 0m, new[] { new TierDefinition(100m, 0.10m), new TierDefinition(1000m, 0.08m), new TierDefinition(decimal.MaxValue, 0.05m) }),
                new PricingRule("api", BillingType.FixedSubscriptionPlusOverage, "call", 50m, null, 1000000m, 0.001m)
            });

        var usageStore = new InMemoryUsageStore();
        var service = new BillingService(pricingConfig, usageStore);

        service.RecordUsage(new UsageEvent("user-1", "resource-storage", "storage", 200m, "GB-hour", new DateTime(2026, 6, 1, 12, 0, 0, DateTimeKind.Utc)));
        service.RecordUsage(new UsageEvent("user-1", "resource-compute", "compute", 150m, "hour", new DateTime(2026, 6, 10, 10, 0, 0, DateTimeKind.Utc)));
        service.RecordUsage(new UsageEvent("user-1", "resource-api", "api", 1200000m, "call", new DateTime(2026, 6, 15, 9, 0, 0, DateTimeKind.Utc)));
        service.RecordUsage(new UsageEvent("user-2", "resource-storage", "storage", 50m, "GB-hour", new DateTime(2026, 6, 20, 8, 0, 0, DateTimeKind.Utc)));

        var invoice = service.GenerateInvoice("user-1", new DateTime(2026, 6, 1, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 7, 1, 0, 0, 0, DateTimeKind.Utc));

        Assert.Equal("user-1", invoice.UserId);
        Assert.Equal(3, invoice.LineItems.Count);
        Assert.Equal(3, invoice.ServiceSubtotals.Count);
        Assert.Equal(0.02m * 200m + 0.10m * 100m + 0.08m * 50m + 50m + 0.001m * 200000m, invoice.TotalAmount);
    }
}
