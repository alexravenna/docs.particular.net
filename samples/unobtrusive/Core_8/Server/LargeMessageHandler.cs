using Messages;
using NServiceBus.Logging;

public class LargeMessageHandler : IHandleMessages<LargeMessage>
{
    static readonly ILog log = LogManager.GetLogger<LargeMessageHandler>();

    public Task Handle(LargeMessage message, IMessageHandlerContext context)
    {
        if (message.LargeDataBus == null)
        {
            log.Info($"Message [{message.GetType()}] received, id:{message.RequestId}");
        }
        else
        {
            log.Info($"Message [{message.GetType()}] received, id:{message.RequestId} and payload {message.LargeDataBus.Length} bytes");
        }
        return Task.CompletedTask;
    }
}
