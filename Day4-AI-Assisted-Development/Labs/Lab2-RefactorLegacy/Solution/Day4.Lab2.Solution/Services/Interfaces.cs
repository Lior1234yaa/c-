using Day4.Lab2.Domain;

namespace Day4.Lab2.Services;

public interface ISubscriptionSource
{
    IReadOnlyList<Subscription> Load();
}

public interface IPricingService
{
    Bill Price(Subscription subscription);
}
