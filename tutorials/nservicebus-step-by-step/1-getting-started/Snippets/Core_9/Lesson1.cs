﻿using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using NServiceBus;

namespace Core_9;
#pragma warning disable 1998

#region EmptyProgram
class Program
{
    static async Task Main(string[] args)
    {

    }
}
#endregion

class StepByStep
{
    #region Main

    static async Task Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);

        var endpointConfiguration = new EndpointConfiguration("ClientUI");

        endpointConfiguration.UseSerialization<SystemJsonSerializer>();

        var routing = endpointConfiguration.UseTransport(new LearningTransport());

    }
    #endregion

    static async Task Steps(string[] args)
    {
        #region ConsoleTitle
        Console.Title = "ClientUI";
        #endregion

        #region Setup
        var builder = Host.CreateApplicationBuilder(args);
        #endregion

        #region EndpointName
        var endpointConfiguration = new EndpointConfiguration("ClientUI");

        // Choose JSON to serialize and deserialize messages
        endpointConfiguration.UseSerialization<SystemJsonSerializer>();
        #endregion

        #region LearningTransport
        var transport = endpointConfiguration.UseTransport<LearningTransport>();
        #endregion

        #region Startup
        builder.UseNServiceBus(endpointConfiguration);

        await builder.Build().RunAsync();
        #endregion
    }
}

#pragma warning restore 1998