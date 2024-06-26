﻿using NServiceBus;
using NServiceBus.Gateway;

Console.Title = "RemoteSite";
var endpointConfiguration = new EndpointConfiguration("Samples.Gateway.RemoteSite");
endpointConfiguration.EnableInstallers();
endpointConfiguration.UseTransport<LearningTransport>();

#region RemoteSiteGatewayConfig
var gatewayConfig = endpointConfiguration.Gateway(new InMemoryDeduplicationConfiguration());
gatewayConfig.AddReceiveChannel("http://localhost:25899/RemoteSite/");
#endregion

var endpointInstance = await Endpoint.Start(endpointConfiguration);
Console.WriteLine("\r\nPress any key to stop program\r\n");
Console.ReadKey();
await endpointInstance.Stop();