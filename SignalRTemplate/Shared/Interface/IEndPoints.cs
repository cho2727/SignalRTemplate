using SignalRTemplate.Shared.Injectables;

namespace SignalRTemplate.Shared.Interface;

public interface IEndPoints : ITransient
{
    void AddEndPoints(IEndpointRouteBuilder routes);
}
