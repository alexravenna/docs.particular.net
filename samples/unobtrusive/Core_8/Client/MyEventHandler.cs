﻿using Events;
using NServiceBus.Logging;

public class MyEventHandler : IHandleMessages<MyEvent>
{
    static readonly ILog log = LogManager.GetLogger<MyEventHandler>();

    public Task Handle(MyEvent message, IMessageHandlerContext context)
    {
        log.Info($"MyEvent received from server with id:{message.EventId}");
        return Task.CompletedTask;
    }
}