---
title: Message routing
summary: How NServiceBus routes messages between endpoints.
reviewed: 2024-12-31
component: Core
isLearningPath: true
redirects:
- nservicebus/how-do-i-specify-to-which-destination-a-message-will-be-sent
- nservicebus/messaging/specify-message-destination
- nservicebus/messaging/message-owner
- nservicebus/msmq/sender-side-distribution
related:
- samples/pubsub
- nservicebus/messaging/publish-subscribe
- nservicebus/messaging/routing-extensibility
- nservicebus/concepts/priority-queues
---

The routing subsystem is responsible for finding destinations for messages. In most cases, the code which sends messages should not specify the destination of the message being sent:

snippet: BasicSend

The routing subsystem will provide the destination address based on the message type.

NServiceBus routing consists of two layers: logical and physical. Logical routing defines which logical endpoint should receive a given outgoing message. Physical routing defines to which physical instance of the selected endpoint the message should be delivered. While logical routing is a developer's concern, physical routing is controlled by operations.

[Broker transports](/transports/types.md#broker-transports) handle the physical routing automatically. Other transport might require specific configuration. See also [MSMQ Routing](/transports/msmq/routing.md).

## Command routing

As described in [messages, events, and commands](/nservicebus/messaging/messages-events-commands.md), NServiceBus distinguishes between these kinds of messages. Command messages are always routed to a single logical endpoint.

partial: commands

Per-namespace routes override assembly-level routes, while per-type routes override both namespace and assembly routes.

### Overriding the destination

In specific cases, like sending to the same endpoint or a specific queue (e.g., for integration purposes), the routing configuration can be overridden when sending a message. Refer to documentation on [sending messages](/nservicebus/messaging/send-a-message.md) for further details.

## Event routing

When events are published, they can be received by multiple logical endpoints. However, even in cases where those logical endpoints are scaled out, each event will be received by only one physical instance of a specific logical subscriber. It is important to note that before the event is published and delivered, the subscriber has to express its interest in that event by having a message handler for it.

### Native

[Multicast transports](/transports/types.md#multicast-enabled-transports) support the Publish-Subscribe pattern natively. In this case, the subscriber uses the transport APIs to create a route for a given subscribed message type.

### Message-driven

[Other transports](/transports/types.md#unicast-only-transports) do not support Publish-Subscribe natively. These transports emulate the publish behavior by sending a message to each subscriber directly. To do this, the publisher endpoint has to know its subscribers, and subscribers have to notify the publisher about their interest in a given event type. The notification message (known as the *subscribe* message) must be routed to the publisher.

partial: events

## Reply routing

Reply messages are always routed based on the `ReplyTo` header of the initial message, regardless of the endpoint's routing configuration. Only the sender of the initial message can influence the routing of a reply. Refer to documentation on [sending messages](/nservicebus/messaging/send-a-message.md) for further details.

## Make instance uniquely addressable

When using a message broker, multiple instances of a scaled-out endpoint are consuming from the same address via the [competing consumer](/nservicebus/scaling.md#scaling-out-to-multiple-nodes-competing-consumers) model.

To address specific instances of a scaled-out endpoint, instances can be configured to be individually addressable by providing a unique discriminator to each instance:

The following queues will be created for endpoint `Sales` configured with discriminator `B`:

- `Sales`
- `Sales-B`

```c#
var endpointConfiguration = new EndpointConfiguration("Sales");
endpointConfiguration.MakeInstanceUniquelyAddressable("B");
```

Uniquely addressable instances are used for [callbacks](/nservicebus/messaging/callbacks.md) but can be used for other purposes like data partitioning with processing affinity or a form or processing prioritization.

```c#
var options = new SendOptions();
options.RouteToSpecificInstance("B");
endpointInstance.Send(new MyMessage(), options);
```

Recommendations:

- Avoid hard-coding the discriminator when sending messages.
- Avoid using `MakeInstanceUniquelyAddressable` for priority queues.
- Consider using [routing extensibility](/nservicebus/messaging/routing-extensibility.md) for routing to specific instances.
