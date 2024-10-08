---
title: Auditing Messages
summary: Send a copy of every successfully processed message to a central place for analysis and compliance purposes.
component: Core
reviewed: 2024-08-06
related:
 - nservicebus/operations
 - nservicebus/messaging/headers
 - nservicebus/messaging/discard-old-messages
redirects:
 - nservicebus/auditing-with-nservicebus
---

The distributed nature of parallel, message-driven systems makes them more difficult to debug than their sequential, synchronous, and centralized counterparts. For these reasons, NServiceBus provides built-in message auditing for every endpoint. When configured to audit, NServiceBus will capture a copy of every successfully processed message and forward it to a specified audit queue.

> [!NOTE]
> By default, auditing is not enabled and must be configured for each endpoint where auditing is required.

It is recommended to specify a central auditing queue for all related endpoints (i.e., endpoints that belong to the same system). This gives an overview of the entire system in one place and can help see how messages correlate. The audit queue can be considered a central record of everything that happened in the system.

> [!IMPORTANT]
> When auditing NServiceBus messages, it is important to have the capability to process messages sent to the audit queue: The Particular Service Platform, specifically [ServiceControl](/servicecontrol), processes messages from these auditing queues to provide diagnostic and visualization features. For more information, see the [ServiceInsight](/serviceinsight/) and [ServicePulse](/servicepulse/) documentation.

## Performance impact

Enabling auditing has an impact on storage and network resources.

### Message throughput

Enabling auditing on all endpoints doubles the global message throughput, which can sometimes be troublesome in high message volume environments.

In such environments, it might make sense only to enable auditing on relevant endpoints that need auditing.

### Queue storage capacity

Auditing successfully processed messages can result in storing huge amounts of message data. First in the audit queue and second in any application that will store these messages. If the audit storage logic stops processing audit messages, the audit queue size can grow very fast. Messaging infrastructure has a size limit on the amount of data that can be stored in a queue. If this storage limit is reached, messages can no longer be processed in all the endpoints that have auditing enabled.

Make sure the size limit is increased to allow for scheduled and unscheduled downtime.

### Audit store capacity

Perform capacity planning on the store where messages will be written.

When using ServiceControl, read the [ServiceControl capacity planning documentation](/servicecontrol/capacity-and-planning.md).

## Filtering audit messages

In some cases, it might be useful to exclude certain message types from being forwarded to the audit queue. This can be accomplished with a [custom behavior in the pipeline](/samples/pipeline/audit-filtering).

## Message headers

The audited message is enriched with additional headers, which contain information related to processing the message:

* Processing start and end times.
* Processing host ID and name.
* Processing machine address.
* Processing endpoint.

## Handling Audit messages

Audit messages can be handled in various ways: saved in a database, custom logged, etc. However, it is important not to leave the messages in the audit queue, as most queuing technologies have upper-bound limits on their queue sizes and depth. By not processing these messages, the limits of the underlying queue technology may be reached.

## Configuring auditing

partial: configuration

## Audit configuration options

Two settings control auditing:

### Queue Name

The queue name to forward audit messages.

### OverrideTimeToBeReceived

To force a [TimeToBeReceived](/nservicebus/messaging/discard-old-messages.md) on audit messages by setting `OverrideTimeToBeReceived` use the configuration syntax below.

Note that while the phrasing is "forwarding a message" in the implementation, it is actually "cloning and sending a new message". This is important when considering TimeToBeReceived since the time taken to receive and process the original message is not part of the TimeToBeReceived of the new audit message. In effect, the audit message receives the full-time allotment of whatever TimeToBeReceived is used.

> [!WARNING]
> MSMQ forces the same TimeToBeReceived on all messages in a transaction. Therefore, OverrideTimeToBeReceived is unsupported when using the [MSMQ Transport](/transports/msmq/). If OverrideTimeToBeReceived is detected when using MSMQ, an exception will be thrown with the following text:
>
> ```
> Setting a custom OverrideTimeToBeReceived for audits is not supported on transactional MSMQ
> ```

#### Default Value

If no OverrideTimeToBeReceived is defined then:

**Versions 5 and below**: TimeToBeReceived of the original message will be used.

**Versions 6 and above**: No TimeToBeReceived will be set.

## Filtering audit messages

When auditing is enabled, all messages are audited by default. To control which message types are audited, see the [audit filter sample](/samples/pipeline/audit-filtering/).

### Filtering individual properties

Auditing works by sending an exact copy of the received message to the audit queue, so filtering out individual properties is not supported.

For sensitive properties, e.g. credit card numbers or passwords, use [message property encryption](/nservicebus/security/property-encryption.md). For large properties, consider the [data bus feature](/nservicebus/messaging/claimcheck/) to avoid including the actual payload in the audited message.

## Additional audit information

Additional information can be added to audit messages using a [custom behavior](/nservicebus/pipeline/manipulate-with-behaviors.md), as shown in the following snippet. The additional data will be contained in the audit message headers.

snippet: AddAuditData

> [!NOTE]
> Message headers count towards the message size and the additional audit information has to honor the transport's message header limitations.

partial: custom-audit-action
