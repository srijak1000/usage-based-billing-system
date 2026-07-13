using BillingSystem.Domain;

namespace BillingSystem.Application;

public interface IPricingStrategy
{
    BillingType BillingType { get; }
    decimal Calculate(PricingRule rule, decimal quantity);
}

public sealed class FlatPerUnitPricingStrategy : IPricingStrategy
{
    public BillingType BillingType => BillingType.FlatPerUnit;

    public decimal Calculate(PricingRule rule, decimal quantity)
    {
        return quantity * rule.BaseAmount;
    }
}

public sealed class TieredPricingStrategy : IPricingStrategy
{
    public BillingType BillingType => BillingType.Tiered;

    public decimal Calculate(PricingRule rule, decimal quantity)
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
}

public sealed class SubscriptionPricingStrategy : IPricingStrategy
{
    public BillingType BillingType => BillingType.FixedSubscriptionPlusOverage;

    public decimal Calculate(PricingRule rule, decimal quantity)
    {
        if (rule.IncludedQuantity is null || rule.OverageRate is null)
        {
            throw new InvalidOperationException("Subscription pricing requires included quantity and overage rate.");
        }

        return rule.BaseAmount + Math.Max(0, quantity - rule.IncludedQuantity.Value) * rule.OverageRate.Value;
    }
}

public sealed class PricingStrategyRegistry
{
    private readonly IReadOnlyDictionary<BillingType, IPricingStrategy> _strategies;

    public PricingStrategyRegistry(IEnumerable<IPricingStrategy> strategies)
    {
        _strategies = strategies.ToDictionary(strategy => strategy.BillingType);
    }

    public IPricingStrategy GetStrategy(BillingType billingType)
    {
        if (_strategies.TryGetValue(billingType, out var strategy))
        {
            return strategy;
        }

        throw new NotSupportedException($"Unsupported billing type: {billingType}");
    }
}
