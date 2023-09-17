using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SignalRTemplate.Shared.Interface;
using SignalRTemplate.SignalR;

namespace SignalRTemplate.Features.Push;

public class Enpoints : IEndPoints
{
    public void AddEndPoints(IEndpointRouteBuilder routes)
    {

        var pushRouter = routes.MapGroup("push").WithOpenApi().RequireAuthorization("ChannelHubAuthorizationPolicy");
        
        pushRouter.MapPost("all",
            async ([FromBody] All.Command request, IMediator mediator)
            => await mediator.Send(request));


        pushRouter.MapPost("user",
            async ([FromBody] User.Command request, IMediator mediator)
            => await mediator.Send(request));
    }
}
