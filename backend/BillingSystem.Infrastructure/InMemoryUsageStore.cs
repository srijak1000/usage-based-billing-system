using BillingSystem.Domain;

namespace BillingSystem.Infrastructure;

public sealed class InMemoryUsageStore : IUsageStore
{
    private readonly List<UsageEvent> _events = new();

    public void Add(UsageEvent usageEvent)
    {
        _events.Add(usageEvent);
    }

    public IReadOnlyList<UsageEvent> GetForUser(string userId)
    {
        return _events.Where(e => string.Equals(e.UserId, userId, StringComparison.OrdinalIgnoreCase)).ToList();
    }

    public IReadOnlyList<UsageEvent> GetAll()
    {
        return _events.ToList();
    }
}
