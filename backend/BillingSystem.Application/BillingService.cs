using BillingSystem.Domain;

namespace BillingSystem.Application;

public sealed class BillingService
{
    private readonly PricingConfiguration _pricingConfiguration;
    private readonly IUsageStore _usageStore;
    private readonly PricingStrategyRegistry _strategyRegistry;

    public BillingService(PricingConfiguration pricingConfiguration, IUsageStore usageStore, PricingStrategyRegistry strategyRegistry)
    {
        _pricingConfiguration = pricingConfiguration;
        _usageStore = usageStore;
        _strategyRegistry = strategyRegistry;
    }

    public void RecordUsage(UsageEvent usageEvent)
    {
        ArgumentNullException.ThrowIfNull(usageEvent);
        _usageStore.Add(usageEvent);
    }

    public Invoice GenerateInvoice(string userId, DateTime start, DateTime end)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userId);

        var events = _usageStore.GetForUser(userId)
            .Where(e => e.TimestampUtc >= start && e.TimestampUtc < end)
            .OrderBy(e => e.TimestampUtc)
            .ToList();

        var lineItems = new List<InvoiceLineItem>();
        var serviceSubtotals = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase);

        foreach (var usageEvent in events)
        {
            var rule = _pricingConfiguration.GetRule(usageEvent.ServiceType);
            if (rule is null)
            {
                continue;
            }

            var amount = CalculateAmount(rule, usageEvent.Quantity);
            lineItems.Add(new InvoiceLineItem
            {
                ResourceId = usageEvent.ResourceId,
                ServiceType = usageEvent.ServiceType,
                Unit = usageEvent.Unit,
                Quantity = usageEvent.Quantity,
                Amount = amount
            });

            serviceSubtotals[usageEvent.ServiceType] = serviceSubtotals.GetValueOrDefault(usageEvent.ServiceType) + amount;
        }

        return new Invoice
        {
            UserId = userId,
            Start = start,
            End = end,
            LineItems = lineItems,
            ServiceSubtotals = serviceSubtotals.Select(kvp => new ServiceSubtotal { ServiceType = kvp.Key, Amount = kvp.Value }).ToList(),
            TotalAmount = lineItems.Sum(item => item.Amount)
        };
    }

    private decimal CalculateAmount(PricingRule rule, decimal quantity)
    {
        var strategy = _strategyRegistry.GetStrategy(rule.BillingType);
        return strategy.Calculate(rule, quantity);
    }
}
