using MediatR;
using Microsoft.AspNetCore.SignalR;
using SignalRTemplate.Shared.Interface;
using SignalRTemplate.SignalR;

namespace SignalRTemplate.Features.Push;

public class All
{
    public class Command : IRequest<Response>
    {
        public string Id { get; set; }
        public string Message { get; set; }
    }

    public class Response : BaseResponse
    {

    }


    public class CommandHandler : IRequestHandler<Command, Response>
    {
        private readonly IHubContext<ChannelHub> _hubContext;

        public CommandHandler(IHubContext<ChannelHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
        {
            var response = new Response { Result = false };
            await _hubContext.Clients.Group(request.Id).SendAsync("receiveMessage", request.Message);
            response.Result = true;
            return response;
        }
    }
}
