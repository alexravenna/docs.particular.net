---
title: "NServiceBus Step-by-step: Sending a command"
reviewed: 2024-05-13
summary: In this 15-20 minute tutorial, you'll learn how to define NServiceBus messages and handlers, and send and receive a message.
redirects:
- tutorials/intro-to-nservicebus/2-sending-a-command
- tutorials/nservicebus-101/lesson-2
extensions:
- !!tutorial
  nextText: "Next: Multiple endpoints"
  nextUrl: tutorials/nservicebus-step-by-step/3-multiple-endpoints
---

Sending and receiving messages is a central characteristic of any NServiceBus system. Durable messages passed between processes allow reliable communication between those processes, even if one of them is temporarily unavailable. In this lesson we'll show how to send and process a message.

In the next 15-20 minutes, you will learn how to define messages and message handlers, send and receive a message locally, and use the built-in logging capabilities.

## What is a message

A [**message**](/nservicebus/messaging/messages-events-commands.md) is a collection of data sent via one-way communication between two endpoints. In NServiceBus, we define messages via simple classes.

In this lesson, we'll focus on a specific type of message: [commands](/nservicebus/messaging/messages-events-commands.md). Later on, in [Part 4: Publishing events](../4-publishing-events/) we'll expand to look at another type of message, `events`.

To define a command, create a class and mark it with the `ICommand` interface.

snippet: Command

By implementing this interface, we let NServiceBus know that the class is a command so that it can build up some metadata about the message type when the endpoint starts up. Any properties you create within the message constitutes the message data.

The name of your command class is important, as it allows others to infer the intent of the class without looking at its content. A command is an order to do something, so it should be named in the [imperative tense](https://en.wikipedia.org/wiki/Imperative_mood). 
`PlaceOrder` and `ChargeCreditCard` are good names for commands, because they are phrased as a command and are very specific. We could expect that `PlaceOrder` will place an order and `ChargeCreditCard` will charge money on a credit card. `CustomerMessage`, on the other hand, is not a good example. It is not in the imperative, and it's vague. Ideally another person should know exactly what a command's purpose is just by reading its name.

Command names should also convey business intent. `UpdateCustomerPropertyXYZ`, while more specific than `CustomerMessage` isn't a good name for a command because it's focused only on the data manipulation rather than the business meaning behind it. `MarkCustomerAsGold`, or something else that is domain oriented, is a better choice.

When sending a message, the endpoint's [serializer](/nservicebus/serialization/) will serialize the instance of the `DoSomething` class and add it to the contents of the outgoing message that goes to the queue. On the other end, the receiving endpoint will deserialize the message back to an instance of the message class so that it can be used by that endpoint.

Messages can even contain child objects or collections. The supported range of structures is dictated by the [choice of serializer](/nservicebus/serialization/#supported-serializers).

snippet: ComplexCommand

Messages are a contract between two endpoints. Any change to the message will likely involve a change on both the sender and receiver side. The more properties you have on a message, the more reasons it has to change, so keep your messages [as slim as possible](https://particular.net/blog/putting-your-events-on-a-diet).

Also, do not embed logic within your message classes. Each message should contain only automatic properties and not computed properties or methods. It is a good practice to initialize collection properties as shown above, so that you never have to deal with serializing a null collection.

In essence, messages should be carriers for data only. By keeping your messages small and giving them clear purpose, your code will be easy to understand and evolve.


## Organizing messages

Messages are data contracts and as such, they are shared between multiple endpoints. Therefore you should not put the classes in the same assembly with the endpoints; they should live in a separate class library.

**Message assemblies** should be entirely self-contained, meaning they should contain only NServiceBus message types, and any supporting types required by the messages themselves. For example, if a message uses an enumeration type for one of its properties, that enumeration type should also be contained within the same message assembly.

> [!NOTE]
> It is technically possible to embed messages within the endpoint assembly, but those messages can't be exchanged with other endpoints. Some of the samples in our documentation break this rule and embed the messages in the endpoint assembly in order to make the sample easier to understand. In this tutorial, we'll stick to keeping them in dedicated message assemblies.

Additionally, message assemblies should have no dependencies other than libraries included with the .NET Framework, and the NServiceBus core assembly, which is required to reference the `ICommand` interface.

Following these guidelines will make your message contracts easy to evolve in the future.

## Processing messages

To process a message, we create a [**message handler**](/nservicebus/handlers/), a class that implements `IHandleMessages<T>`, where `T` is a message type. A message handler looks like this:

snippet: EmptyHandler

The implementation of the `IHandleMessages<T>` interface is the `Handle` method, which NServiceBus will invoke when a message of type `T` (in this case `DoSomething`) arrives. The `Handle` method receives the message and an `IMessageHandlerContext` that contains contextual API for working with messages.

Since the handlers in the tutorials are very simple and mostly just log information, they don't need to have the `async` keyword in the method definition. However, it's possible to add it and modify the handler to not return a `Task`:

snippet: EmptyHandlerAsync

If you want to learn more about working with async methods, see [Asynchronous Handlers](/nservicebus/handlers/async-handlers.md).

It makes no difference whether handlers are implemented inside one or multiple classes. A single class can implement multiple `IHandleMessages<T>` for multiple message types, so you can group logically related message handlers together in the same class in order to make your code easier to understand. Just remember that each time a message is processed, a new instance of that class is instantiated by the framework. Thus, you can't set a private member variable in one message handler and then expect to have that value around when the next message (regardless of type) is processed.

snippet: MultiHandler

When NServiceBus starts up, it scans the types in all available assemblies, finds all message handler classes, and automatically wires them up, so that they will be invoked when messages arrive. There's no special configuration required - it just works.

## Exercise

Now let's take the solution we started in the [last lesson](../1-getting-started/) and modify it to send a message. When we're done, the ClientUI endpoint will send a PlaceOrder message to itself and then process that message, as depicted in the following diagram:

![Exercise 2 Diagram](diagram.svg)

### Create a messages assembly

To share messages between endpoints, they need to be self-contained in a separate assembly. Let's create that assembly now.

 1. In the solution, create a new project and select the **Class Library** project type.
 1. Set the name of the project to **Messages**.
 1. Remove the automatically created **Class1.cs** file from the project.
 1. Add the NServiceBus NuGet package to the **Messages** project.
 1. In the **ClientUI** project, add a reference to the **Messages** project.

### Create a message

Let's create our first command.

 1. In the **Messages** project, create a new class named `PlaceOrder`.
 1. Mark `PlaceOrder` as `public` and implement `ICommand`.
 1. Add a public property of type `string` named `OrderId`.

> [!WARNING]
> The .NET Framework contains its own interface named `ICommand` in the `System.Windows.Input` namespace. If you use tooling to resolve the namespace, be sure to select `NServiceBus.ICommand`. Most of the types you will need will reside in the `NServiceBus` namespace.

When complete, your `PlaceOrder` class should look like the following:

snippet: PlaceOrder

### Create a handler

Now that we've defined a message, we can create a corresponding message handler. For now, let's handle the message locally within the **ClientUI** endpoint.

 1. In the **ClientUI** project, create a new class named `PlaceOrderHandler`.
 1. Mark the handler class as public and implement the `IHandleMessages<PlaceOrder>` interface.
 1. Add a logger instance, which will allow you to take advantage of the same logging system used by NServiceBus. This has an important advantage over `Console.WriteLine()`: the entries written with the logger will appear in the log file in addition to the console. Use this line to add a primary constructor to your handler class that accepts a logger instance:
    ```cs
    public class PlaceOrderHandler(ILogger<PlaceOrderHandler> logger)
    ```
 1. Within the `Handle` method, use the logger to record the receipt of the `PlaceOrder` message, including the value of the `OrderId` message property:
    ```cs
    logger.LogInformation("Received PlaceOrder, OrderId = {orderId}", message.OrderId);
    ```
 1. Since everything we have done in this handler method is synchronous, return `Task.CompletedTask`.

When complete, your `PlaceOrderHandler` class should look like this:

snippet: PlaceOrderHandler

### Send a message

Now we have a message and a handler to process it. Let's send that message.

In the **ClientUI** project, we are currently stopping the endpoint when we press <kbd>Ctrl+C</kbd>. Let's change that, and put the user input processing logic in a long running task using a [`BackgroundService`](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/host/hosted-services?view=aspnetcore-8.0&tabs=visual-studio#backgroundservice-base-class)

Create a class named `InputLoopService` and add the following code:

snippet: InputLoopService

Let's take a closer look at the case when we want to place an order. In order to create the `PlaceOrder` command, create an instance of the `PlaceOrder` class and supply a unique value for the `OrderId`. Then, after logging the details, we can send it with the `SendLocal` method.

`SendLocal(object message, CancellationToken cancellationToken)` is a method that is available as an extension method on the `IMessageSession` interface, as we are using here, and also on the `IMessageHandlerContext` interface, which we saw when we were defining our message handler. The *Local* part means that we are not sending to an external endpoint (in a different process) so we intend to handle the message in the same endpoint that sent it. By using `SendLocal()`, we don't have to do anything special to tell the message where to go.

> [!NOTE]
> In this lesson, we're using `SendLocal` (rather than the more commonly used `Send` method) so that we can explore how to define, send, and process messages without needing a second endpoint to process them. With `SendLocal`, we also don't need to define routing rules to control where the sent messages go. We'll learn about these concepts [in the next lesson](../3-multiple-endpoints/).

Because `SendLocal()` returns a `Task`, we need to be sure to `await` it properly.

Now let's modify `Program.cs` method and register the `InputLoopService` in the host:

snippet: AddInputLoopService

### Running the solution

Now we are ready to run the solution. Whenever we press <kbd>P</kbd> on the terminal, a command message is sent and then processed by a handler class in the same project.
You should see something similar to this on your console:
```
 info: ClientUI.InputLoopService[0]
       Press 'P' to place an order, or 'Q' to quit.
 p
 info: ClientUI.InputLoopService[0]
       Sending PlaceOrder command, OrderId = d372b7e5-aa0c-4f04-9714-b2d92eccb85b
 info: ClientUI.InputLoopService[0]
       Press 'P' to place an order, or 'Q' to quit.
 info: ClientUI.PlaceOrderHandler[0]
       Received PlaceOrder, OrderId = d372b7e5-aa0c-4f04-9714-b2d92eccb85b
 p
 info: ClientUI.InputLoopService[0]
       Sending PlaceOrder command, OrderId = 4feacbf6-9592-43c4-be80-4863bd79dd8d
 info: ClientUI.InputLoopService[0]
       Press 'P' to place an order, or 'Q' to quit.
 info: ClientUI.PlaceOrderHandler[0]
       Received PlaceOrder, OrderId = 4feacbf6-9592-43c4-be80-4863bd79dd8d
```

Note how after sending a message, the prompt from `ClientUI.Program` is displayed _before_ the `ClientUI.PlaceOrderHandler` acknowledges receipt of the message. This is because rather than calling the `Handle` method as a direct method call, the message is sent asynchronously, and then control immediately returns to the `RunLoop` which repeats the prompt. It isn't until a bit later, when the message is received and processed, that we see the `Received PlaceOrder` notification.

## Summary

In this lesson we learned about messages, message assemblies, and message handlers. We created a message and a handler and we used `SendLocal()` to send the message to the same endpoint.

In the next lesson, we'll create a second messaging endpoint, move our message handler over to it, then configure the ClientUI to send the message to the new endpoint. We'll also be able to observe what happens when we send messages while the receiver endpoint is offline.
