﻿using Microsoft.Extensions.Logging;

public class MyEventHandler(ILogger<MyEventHandler> logger) : IHandleMessages<MyEvent>
{
    public Task Handle(MyEvent eventMessage, IMessageHandlerContext context)
    {
        logger.LogInformation("Received {Event} with a payload of {Length} bytes.", nameof(MyEvent), eventMessage.Data?.Length ?? 0);
        return Task.CompletedTask;
    }
}