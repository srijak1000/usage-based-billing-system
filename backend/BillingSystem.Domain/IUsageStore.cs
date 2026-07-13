namespace BillingSystem.Domain;

public interface IUsageStore
{
    void Add(UsageEvent usageEvent);
    IReadOnlyList<UsageEvent> GetForUser(string userId);
    IReadOnlyList<UsageEvent> GetAll();
}
