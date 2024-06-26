Console.Title = "Server";
var endpointConfiguration = new EndpointConfiguration("Samples.Unobtrusive.Server");
endpointConfiguration.UseSerialization<SystemJsonSerializer>();
endpointConfiguration.UseTransport(new LearningTransport());
endpointConfiguration.UseDataBus<FileShareDataBus, SystemJsonDataBusSerializer>()
    .BasePath(@"..\..\..\..\DataBusShare\");

endpointConfiguration.ApplyCustomConventions();

var endpointInstance = await Endpoint.Start(endpointConfiguration);
await CommandSender.Start(endpointInstance);
await endpointInstance.Stop();

