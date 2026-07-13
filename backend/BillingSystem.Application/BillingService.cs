using BillingSystem.Domain;

namespace BillingSystem.Application;

public sealed class BillingService
{
    private readonly PricingConfiguration _pricingConfiguration;
    private readonly IUsageStore _usageStore;

    public BillingService(PricingConfiguration pricingConfiguration, IUsageStore usageStore)
    {
        _pricingConfiguration = pricingConfiguration;
        _usageStore = usageStore;
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

    private static decimal CalculateAmount(PricingRule rule, decimal quantity)
    {
        return rule.BillingType switch
        {
            BillingType.FlatPerUnit => quantity * rule.BaseAmount,
            BillingType.Tiered => CalculateTieredAmount(rule, quantity),
            BillingType.FixedSubscriptionPlusOverage => CalculateSubscriptionAmount(rule, quantity),
            _ => throw new NotSupportedException($"Unsupported billing type: {rule.BillingType}")
        };
    }

    private static decimal CalculateTieredAmount(PricingRule rule, decimal quantity)
    {
        if (rule.Tiers.Count == 0)
        {
            throw new InvalidOperationException("Tiered pricing requires at least one tier.");
        }

        var total = 0m;
        var remaining = quantity;
        var previousThreshold = 0m;

        foreach (var tier in rule.Tiers)
        {
            var bucketSize = Math.Max(0, tier.Threshold - previousThreshold);
            var applied = Math.Min(remaining, bucketSize);
            total += applied * tier.Rate;
            remaining -= applied;
            previousThreshold = tier.Threshold;

            if (remaining <= 0)
            {
                break;
            }
        }

        if (remaining > 0)
        {
            var finalTier = rule.Tiers[^1];
            total += remaining * finalTier.Rate;
        }

        return total;
    }

    private static decimal CalculateSubscriptionAmount(PricingRule rule, decimal quantity)
    {
        if (rule.IncludedQuantity is null || rule.OverageRate is null)
        {
            throw new InvalidOperationException("Subscription pricing requires included quantity and overage rate.");
        }

        return rule.BaseAmount + Math.Max(0, quantity - rule.IncludedQuantity.Value) * rule.OverageRate.Value;
    }
}
