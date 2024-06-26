---
title: NServiceBus Host Upgrade Version 6 to 7
summary: Instructions on how to upgrade NServiceBus host from version 6 to version 7
reviewed: 2021-01-23
component: Host
related:
 - nservicebus/upgrades/5to6
isUpgradeGuide: true
upgradeGuideCoreVersions:
 - 5
 - 6
---


## IConfigureThisEndpoint changes

`IConfigureThisEndpoint.Customize` is passed an instance of `EndpointConfiguration` instead of `BusConfiguration`.

```csharp
// For NServiceBus Host version 7.x
class CustomizingHostUpgrade :
    IConfigureThisEndpoint
{
    public void Customize(EndpointConfiguration endpointConfiguration)
    {
        // perform some custom configuration
        endpointConfiguration.UsePersistence<InMemoryPersistence>();
    }
}

// For NServiceBus Host version 6.x
class CustomizingHostUpgrade :
    IConfigureThisEndpoint
{
    public void Customize(BusConfiguration busConfiguration)
    {
        // perform some custom configuration
        busConfiguration.UsePersistence<InMemoryPersistence>();
    }
}
```

## IConfigureLogging and IConfigureLoggingForProfile<T> changes

These interfaces will be removed in version 8 of NServiceBus.Host. The logging can still be configured in the constructor of the class that implements `IConfigureThisEndpoint`.

```csharp
// For NServiceBus Host version 8.x
class CustomLogging :
    IConfigureThisEndpoint
{
    public void Customize(EndpointConfiguration configuration)
    {
        LogManager.Use<DefaultFactory>();
    }
}

// For NServiceBus Host version 7.x
class CustomLogging :
    IConfigureThisEndpoint
{
    public void Customize(EndpointConfiguration configuration)
    {
        LogManager.Use<DefaultFactory>();
    }
}

// For NServiceBus Host version 6.x
class CustomLogging : IConfigureThisEndpoint
{
    public void Customize(BusConfiguration configuration)
    {
        LogManager.Use<DefaultFactory>();
    }
}

// For NServiceBus Host version 5.x
class CustomLogging : IConfigureThisEndpoint
{
    public void Customize(BusConfiguration configuration)
    {
        LogManager.Use<DefaultFactory>();
    }
}
```

The way the runtime profile is detected will need to be re-created but a simple approach could be like this:

```csharp
class RuntimeProfile :
    IConfigureThisEndpoint
{
    public void Customize(BusConfiguration busConfiguration)
    {
        var profile = Environment.GetCommandLineArgs();

        if (profile.Contains("Production"))
        {
            // configure the production profile
        }
    }
}
```

## IWantToRunWhenEndpointStartsAndStops

An interface called [`IWantToRunWhenEndpointStartsAndStops`](/nservicebus/hosting/nservicebus-host/) has been added. This interface replaces the [`IWantToRunWhenBusStartsAndStops`](/nservicebus/lifecycle/endpointstartandstop.md) in NServiceBus core.

> [!CAUTION]
> The `Start` and `Stop` methods will block start up and shut down of the endpoint.


### Interface in version 5 of NServiceBus

```csharp
public class Bootstrapper :
    IWantToRunWhenBusStartsAndStops
{
    public void Start()
    {
        // Do startup actions here.
    }

    public void Stop()
    {
        // Do cleanup actions here.
    }
}
```


### Interface in version 7 of NServiceBus.Host

```csharp
// Use this in NServiceBus.Host or NServiceBus.Host.AzureCloudService
namespace Host_7.UpgradeGuides.Core5to6
{
    public class Bootstrapper :
        IWantToRunWhenEndpointStartsAndStops
    {
        public Task Start(IMessageSession session)
        {
            // Do startup actions here.
            // Either mark Start method as async or do the following
            return Task.CompletedTask;
        }

        public Task Stop(IMessageSession session)
        {
            // Do cleanup actions here.
            // Either mark Stop method as async or do the following
            return Task.CompletedTask;
        }
    }
}
```


The `IMessageSession` parameter provides all the necessary methods to send messages as part of the endpoint start up.

include: 5to6removePShelpers

> [!WARNING]
> If an `EndpointConfig.cs` file already exists in the project, be careful to not overwrite it when upgrading the `NServiceBus.Host` package. If Visual Studio detects a conflict, it will ask whether the file should be overwritten. To keep the old configuration, choose `No`.


## WCF integration

WCF integration using `WcfService` has been moved from the host to a separate NuGet package [NServiceBus.Wcf](https://www.nuget.org/packages/NServiceBus.Wcf/). That package must be used in order to use the WCF integration functionality when targeting NServiceBus version 6 and above. The NServiceBus.Wcf NuGet package **has no dependency** on the NServiceBus.Host NuGet package and can also be used in self-hosting scenarios.

The WCF integration has been augmented with additional functionality such as the ability to reply with messages to the client, cancel requests and reroute to other endpoints. More information can be found in [WCF](/nservicebus/wcf).

### Ambiguous type compilation error

By default referenced assemblies are imported into the [global namespace](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/namespaces/how-to-use-the-global-namespace-alias). This might lead to ambiguous type reference problems when implementing `WcfService<TRequest, TResponse>` or `IWcfService<TRequest, TResponse>`. To resolve the ambiguous type reference:

- In Solution Explorer, right-click under References on the `NServiceBus.Wcf` reference and switch to the properties pane, or use `Alt+Enter`
- Under the alias property set an alias for the assembly, for example `wcf`
- In the declaration of the WCF services use:

```
extern alias wcf;
using wcf::NServiceBus;
```
