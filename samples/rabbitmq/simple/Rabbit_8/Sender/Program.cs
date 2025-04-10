﻿using NServiceBus;
using Shared;

Console.Title = "SimpleSender";

#region ConfigureRabbit
var endpointConfiguration = new EndpointConfiguration("Samples.RabbitMQ.SimpleSender");
var transport = endpointConfiguration.UseTransport<RabbitMQTransport>();
transport.UseConventionalRoutingTopology(QueueType.Quorum);
transport.ConnectionString("host=localhost");
#endregion

endpointConfiguration.UseSerialization<SystemJsonSerializer>();
transport.Routing().RouteToEndpoint(typeof(MyCommand), "Samples.RabbitMQ.SimpleReceiver");
endpointConfiguration.EnableInstallers();

var endpointInstance = await Endpoint.Start(endpointConfiguration);
await SendMessages(endpointInstance);
await endpointInstance.Stop();

static async Task SendMessages(IMessageSession messageSession)
{
    Console.WriteLine("Press [c] to send a command, or [e] to publish an event. Press [Esc] to exit.");

    while (true)
    {
        var input = Console.ReadKey();
        Console.WriteLine();

        switch (input.Key)
        {
            case ConsoleKey.C:
                await messageSession.Send(new MyCommand());
                break;
            case ConsoleKey.E:
                await messageSession.Publish(new MyEvent());
                break;
            case ConsoleKey.Escape:
                return;
        }
    }
}
