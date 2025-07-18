using System.Threading.Tasks;
using NServiceBus;
using Microsoft.Extensions.Logging;
using Shared;

namespace Receiver;

sealed class MyCommandHandler(ILogger<MyCommandHandler> logger) : IHandleMessages<MyCommand>
{
    public Task Handle(MyCommand commandMessage, IMessageHandlerContext context)
    {
        logger.LogInformation("Hello from {HandlerName}", nameof(MyCommandHandler));
        return Task.CompletedTask;
    }
}