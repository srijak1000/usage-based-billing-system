namespace BillingSystem.Domain;

public enum BillingType
{
    FlatPerUnit,
    Tiered,
    FixedSubscriptionPlusOverage
}

public sealed class UsageEvent
{
    public UsageEvent()
    {
    }

    public UsageEvent(string userId, string resourceId, string serviceType, decimal quantity, string unit, DateTime timestampUtc)
    {
        UserId = userId;
        ResourceId = resourceId;
        ServiceType = serviceType;
        Quantity = quantity;
        Unit = unit;
        TimestampUtc = timestampUtc;
    }

    public string UserId { get; set; } = string.Empty;
    public string ResourceId { get; set; } = string.Empty;
    public string ServiceType { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public string Unit { get; set; } = string.Empty;
    public DateTime TimestampUtc { get; set; }
}

public sealed class TierDefinition
{
    public TierDefinition()
    {
    }

    public TierDefinition(decimal threshold, decimal rate)
    {
        Threshold = threshold;
        Rate = rate;
    }

    public decimal Threshold { get; set; }
    public decimal Rate { get; set; }
}

public sealed class PricingRule
{
    public PricingRule()
    {
    }

    public PricingRule(string serviceType, BillingType billingType, string unit, decimal baseAmount, IReadOnlyList<TierDefinition>? tiers = null, decimal? includedQuantity = null, decimal? overageRate = null)
    {
        ServiceType = serviceType;
        BillingType = billingType;
        Unit = unit;
        BaseAmount = baseAmount;
        Tiers = tiers ?? Array.Empty<TierDefinition>();
        IncludedQuantity = includedQuantity;
        OverageRate = overageRate;
    }

    public string ServiceType { get; set; } = string.Empty;
    public BillingType BillingType { get; set; }
    public string Unit { get; set; } = string.Empty;
    public decimal BaseAmount { get; set; }
    public decimal? IncludedQuantity { get; set; }
    public decimal? OverageRate { get; set; }
    public IReadOnlyList<TierDefinition> Tiers { get; set; } = Array.Empty<TierDefinition>();
}

public sealed class PricingConfiguration
{
    public PricingConfiguration()
    {
        Rules = new List<PricingRule>();
    }

    public PricingConfiguration(IEnumerable<PricingRule> rules)
    {
        Rules = rules.ToList();
    }

    public List<PricingRule> Rules { get; set; }

    public PricingRule? GetRule(string serviceType)
    {
        return Rules.FirstOrDefault(rule => string.Equals(rule.ServiceType, serviceType, StringComparison.OrdinalIgnoreCase));
    }
}

public sealed class Invoice
{
    public string UserId { get; init; } = string.Empty;
    public DateTime Start { get; init; }
    public DateTime End { get; init; }
    public IReadOnlyList<InvoiceLineItem> LineItems { get; init; } = Array.Empty<InvoiceLineItem>();
    public IReadOnlyList<ServiceSubtotal> ServiceSubtotals { get; init; } = Array.Empty<ServiceSubtotal>();
    public decimal TotalAmount { get; init; }
}

public sealed class InvoiceLineItem
{
    public string ResourceId { get; init; } = string.Empty;
    public string ServiceType { get; init; } = string.Empty;
    public string Unit { get; init; } = string.Empty;
    public decimal Quantity { get; init; }
    public decimal Amount { get; init; }
}

public sealed class ServiceSubtotal
{
    public string ServiceType { get; init; } = string.Empty;
    public decimal Amount { get; init; }
}
