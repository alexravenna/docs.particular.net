using System;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Logging;
using NServiceBus.MessageRouting.RoutingSlips;

class Program
{
    static ILog log = LogManager.GetLogger(typeof(Program));

    static async Task Main()
    {
        Console.Title = "Sender";
        var endpointConfiguration = new EndpointConfiguration("Samples.RoutingSlips.Sender");

        endpointConfiguration.UsePersistence<LearningPersistence>();
        endpointConfiguration.UseTransport<LearningTransport>();
        // Choose JSON to serialize and deserialize messages
        endpointConfiguration.UseSerialization<SystemJsonSerializer>();

        #region enableRoutingSlips
        endpointConfiguration.EnableFeature<RoutingSlips>();
        #endregion


        var endpoint = await Endpoint.Start(endpointConfiguration);


        Console.WriteLine("Press Enter to send a message");
        Console.WriteLine("Press any other key to exit");
        #region alternate
        var toggle = false;
        while (true)
        {
            var key = Console.ReadKey();
            if (key.Key != ConsoleKey.Enter)
            {
                break;
            }
            if (toggle)
            {
                await SendToABC(endpoint);
            }
            else
            {
                await SendToAC(endpoint);
            }

            toggle = !toggle;
        }
        #endregion
        await endpoint.Stop();
    }
    #region SendAC
    static Task SendToAC(IEndpointInstance endpoint)
    {
        var sequentialProcess = new SequentialProcess
        {
            StepAInfo = "Foo",
            StepCInfo = "Baz",
        };

        log.Info("Sending message for step A, C");
        return endpoint.Route(sequentialProcess, Guid.NewGuid(),
            "Samples.RoutingSlips.StepA",
            "Samples.RoutingSlips.StepC",
            "Samples.RoutingSlips.ResultHost");
    }
    #endregion

    #region SendABC
    static Task SendToABC(IEndpointInstance endpoint)
    {
        #region multi-message-Send
        var sequentialProcess = new SequentialProcess
        {
            StepAInfo = "Foo",
            StepBInfo = "Bar",
            StepCInfo = "Baz",
        };
        #endregion

        log.Info("Sending message for step A, B, C");
        return endpoint.Route(sequentialProcess, Guid.NewGuid(),
            "Samples.RoutingSlips.StepA",
            "Samples.RoutingSlips.StepB",
            "Samples.RoutingSlips.StepC",
            "Samples.RoutingSlips.ResultHost");
    }
    #endregion
}